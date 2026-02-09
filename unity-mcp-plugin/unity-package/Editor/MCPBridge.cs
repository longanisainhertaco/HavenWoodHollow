/**
 * Unity MCP Bridge - Editor Plugin
 * 
 * This is the Unity-side component of the MCP Bridge Pattern.
 * It runs inside the Unity Editor and handles commands from the MCP Server.
 * 
 * Key Features:
 * - [InitializeOnLoad] for automatic restart after domain reload
 * - Main thread dispatching for thread-safe Unity API calls
 * - Reflection-based component modification
 * - Undo support for all operations
 * - Compilation status broadcasting
 * 
 * Installation:
 * 1. Copy this entire folder to your Unity project's Assets/Editor folder
 * 2. Unity will automatically compile and start the bridge
 * 3. Configure the port in Edit > Preferences > MCP Bridge
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Compilation;
using UnityEngine.SceneManagement;

namespace UnityMCPBridge
{
    /// <summary>
    /// Main bridge class that handles TCP communication with the MCP Server.
    /// Uses [InitializeOnLoad] to automatically restart after Unity domain reloads.
    /// </summary>
    [InitializeOnLoad]
    public static class MCPBridge
    {
        private static TcpListener _listener;
        private static TcpClient _client;
        private static NetworkStream _stream;
        private static Thread _listenerThread;
        private static bool _isRunning;
        private static int _port = 8090;
        
        // Thread-safe action queue for main thread dispatch
        private static readonly Queue<Action> _mainThreadActions = new Queue<Action>();
        private static readonly object _actionLock = new object();
        
        // Compilation tracking
        private static bool _isCompiling;
        private static List<string> _compilationErrors = new List<string>();
        
        // Message buffer for incomplete messages
        private static StringBuilder _messageBuffer = new StringBuilder();

        static MCPBridge()
        {
            // Register for compilation events
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            CompilationPipeline.compilationFinished += OnCompilationFinished;
            
            // Register for log messages to capture errors
            Application.logMessageReceived += OnLogMessageReceived;
            
            // Register update callback for main thread dispatch
            EditorApplication.update += OnEditorUpdate;
            
            // Start the TCP server
            StartServer();
            
            Debug.Log($"[MCP Bridge] Initialized on port {_port}");
        }

        /// <summary>
        /// Starts the TCP server to listen for MCP Server connections.
        /// </summary>
        private static void StartServer()
        {
            if (_isRunning) return;
            
            _isRunning = true;
            _listenerThread = new Thread(ListenerThreadFunc)
            {
                IsBackground = true,
                Name = "MCP Bridge Listener"
            };
            _listenerThread.Start();
        }

        /// <summary>
        /// Stops the TCP server.
        /// </summary>
        public static void StopServer()
        {
            _isRunning = false;
            _client?.Close();
            _listener?.Stop();
            _listenerThread?.Join(1000);
        }

        /// <summary>
        /// Background thread function that listens for connections and messages.
        /// </summary>
        private static void ListenerThreadFunc()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Loopback, _port);
                _listener.Start();
                
                while (_isRunning)
                {
                    try
                    {
                        // Accept new connection (blocking)
                        if (_listener.Pending())
                        {
                            _client = _listener.AcceptTcpClient();
                            _stream = _client.GetStream();
                            Debug.Log("[MCP Bridge] Client connected");
                            
                            // Send initial status
                            SendEvent("connected", new Dictionary<string, object>
                            {
                                { "isCompiling", _isCompiling },
                                { "unityVersion", Application.unityVersion },
                                { "projectPath", Application.dataPath }
                            });
                        }
                        
                        // Read messages if connected
                        if (_client != null && _client.Connected && _stream != null && _stream.DataAvailable)
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                            if (bytesRead > 0)
                            {
                                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                                ProcessIncomingData(data);
                            }
                        }
                        
                        Thread.Sleep(10); // Prevent busy-waiting
                    }
                    catch (SocketException)
                    {
                        // Client disconnected
                        _client = null;
                        _stream = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MCP Bridge] Listener error: {ex.Message}");
            }
        }

        /// <summary>
        /// Processes incoming data, handling partial messages.
        /// </summary>
        private static void ProcessIncomingData(string data)
        {
            _messageBuffer.Append(data);
            string buffer = _messageBuffer.ToString();
            
            // Process complete messages (newline delimited)
            string[] messages = buffer.Split('\n');
            
            // Keep incomplete message in buffer
            _messageBuffer.Clear();
            if (!buffer.EndsWith("\n") && messages.Length > 0)
            {
                _messageBuffer.Append(messages[messages.Length - 1]);
                messages = messages.Take(messages.Length - 1).ToArray();
            }
            
            foreach (string message in messages)
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    HandleMessage(message);
                }
            }
        }

        /// <summary>
        /// Handles a single message from the MCP Server.
        /// </summary>
        private static void HandleMessage(string jsonMessage)
        {
            try
            {
                var message = JsonUtility.FromJson<MCPMessage>(jsonMessage);
                
                // Dispatch to main thread for Unity API calls
                EnqueueMainThread(() =>
                {
                    try
                    {
                        object result = ExecuteMethod(message.method, message.@params);
                        SendResponse(message.id, result, null);
                    }
                    catch (Exception ex)
                    {
                        SendResponse(message.id, null, new MCPError
                        {
                            code = -1,
                            message = ex.Message
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MCP Bridge] Error handling message: {ex.Message}");
            }
        }

        /// <summary>
        /// Executes a method based on the method name from the MCP Server.
        /// </summary>
        private static object ExecuteMethod(string method, string paramsJson)
        {
            var handler = CommandRegistry.GetHandler(method);
            if (handler != null)
            {
                return handler(paramsJson);
            }
            
            throw new Exception($"Unknown method: {method}");
        }

        /// <summary>
        /// Sends a response back to the MCP Server.
        /// </summary>
        private static void SendResponse(string id, object result, MCPError error)
        {
            var response = new MCPResponse
            {
                id = id,
                type = "response",
                result = result != null ? JsonUtility.ToJson(result) : null,
                error = error
            };
            
            SendMessage(JsonUtility.ToJson(response));
        }

        /// <summary>
        /// Sends an event to the MCP Server.
        /// </summary>
        public static void SendEvent(string eventName, Dictionary<string, object> data)
        {
            var evt = new MCPEvent
            {
                type = "event",
                method = eventName,
                @params = data != null ? DictionaryToJson(data) : null
            };
            
            SendMessage(JsonUtility.ToJson(evt));
        }

        /// <summary>
        /// Sends a raw message through the TCP connection.
        /// </summary>
        private static void SendMessage(string message)
        {
            try
            {
                if (_stream != null && _client != null && _client.Connected)
                {
                    byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                    _stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MCP Bridge] Error sending message: {ex.Message}");
            }
        }

        /// <summary>
        /// Enqueues an action to be executed on the main thread.
        /// </summary>
        public static void EnqueueMainThread(Action action)
        {
            lock (_actionLock)
            {
                _mainThreadActions.Enqueue(action);
            }
        }

        /// <summary>
        /// Called every editor update to process main thread actions.
        /// </summary>
        private static void OnEditorUpdate()
        {
            lock (_actionLock)
            {
                while (_mainThreadActions.Count > 0)
                {
                    try
                    {
                        _mainThreadActions.Dequeue()?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[MCP Bridge] Error in main thread action: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Called when Unity starts compiling.
        /// </summary>
        private static void OnCompilationStarted(object context)
        {
            _isCompiling = true;
            _compilationErrors.Clear();
            SendEvent("compilationStarted", null);
        }

        /// <summary>
        /// Called when Unity finishes compiling.
        /// </summary>
        private static void OnCompilationFinished(object context)
        {
            _isCompiling = false;
            SendEvent("compilationFinished", new Dictionary<string, object>
            {
                { "errors", _compilationErrors }
            });
        }

        /// <summary>
        /// Captures log messages for error reporting.
        /// </summary>
        private static void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                _compilationErrors.Add($"{condition}\n{stackTrace}");
            }
        }

        /// <summary>
        /// Helper to convert dictionary to JSON string.
        /// </summary>
        private static string DictionaryToJson(Dictionary<string, object> dict)
        {
            // Simple JSON serialization for dictionaries
            var parts = dict.Select(kvp => 
                $"\"{kvp.Key}\": {(kvp.Value is string ? $"\"{kvp.Value}\"" : kvp.Value?.ToString() ?? "null")}");
            return "{" + string.Join(", ", parts) + "}";
        }
    }

    #region Message Types
    
    [Serializable]
    public class MCPMessage
    {
        public string id;
        public string type;
        public string method;
        public string @params;
    }

    [Serializable]
    public class MCPResponse
    {
        public string id;
        public string type;
        public string result;
        public MCPError error;
    }

    [Serializable]
    public class MCPEvent
    {
        public string type;
        public string method;
        public string @params;
    }

    [Serializable]
    public class MCPError
    {
        public int code;
        public string message;
    }

    #endregion
}
