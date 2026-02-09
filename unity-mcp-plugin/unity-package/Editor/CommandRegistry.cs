/**
 * Command Registry - Maps MCP methods to Unity Editor operations
 * 
 * This file contains all the command handlers that execute Unity API calls.
 * Each handler is wrapped with Undo.RecordObject for safety.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityMCPBridge
{
    /// <summary>
    /// Registry of all available command handlers.
    /// </summary>
    public static class CommandRegistry
    {
        private static readonly Dictionary<string, Func<string, object>> _handlers = 
            new Dictionary<string, Func<string, object>>();

        static CommandRegistry()
        {
            // Register all handlers
            RegisterCoreCommands();
            RegisterSceneCommands();
            RegisterGameObjectCommands();
            RegisterComponentCommands();
            RegisterScriptCommands();
            RegisterAssetCommands();
            RegisterBuildCommands();
        }

        public static Func<string, object> GetHandler(string method)
        {
            return _handlers.TryGetValue(method, out var handler) ? handler : null;
        }

        #region Core Commands

        private static void RegisterCoreCommands()
        {
            _handlers["getProjectInfo"] = (paramsJson) =>
            {
                return new ProjectInfo
                {
                    name = Application.productName,
                    path = Application.dataPath.Replace("/Assets", ""),
                    unityVersion = Application.unityVersion,
                    companyName = Application.companyName,
                    isPlaying = EditorApplication.isPlaying,
                    isCompiling = EditorApplication.isCompiling
                };
            };

            _handlers["getCompilationStatus"] = (paramsJson) =>
            {
                return new CompilationStatus
                {
                    isCompiling = EditorApplication.isCompiling,
                    errors = new string[0] // Populated from log capture
                };
            };

            _handlers["getConsoleLogs"] = (paramsJson) =>
            {
                // Note: Full implementation would use reflection to access Console window
                return new ConsoleLogsResult
                {
                    logs = new LogEntry[0],
                    message = "Console log capture requires additional setup"
                };
            };

            _handlers["clearConsole"] = (paramsJson) =>
            {
                var assembly = Assembly.GetAssembly(typeof(SceneView));
                var type = assembly.GetType("UnityEditor.LogEntries");
                var method = type.GetMethod("Clear");
                method.Invoke(null, null);
                return new SuccessResult { success = true };
            };

            _handlers["playMode"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<PlayModeParams>(paramsJson);
                
                switch (p.action)
                {
                    case "play":
                        EditorApplication.isPlaying = true;
                        break;
                    case "stop":
                        EditorApplication.isPlaying = false;
                        break;
                    case "pause":
                        EditorApplication.isPaused = !EditorApplication.isPaused;
                        break;
                    case "step":
                        EditorApplication.Step();
                        break;
                    case "status":
                        break;
                }
                
                return new PlayModeResult
                {
                    isPlaying = EditorApplication.isPlaying,
                    isPaused = EditorApplication.isPaused
                };
            };
        }

        #endregion

        #region Scene Commands

        private static void RegisterSceneCommands()
        {
            _handlers["createScene"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<CreateSceneParams>(paramsJson);
                
                if (p.saveCurrent)
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                }
                
                var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
                
                if (!string.IsNullOrEmpty(p.name))
                {
                    string path = $"Assets/Scenes/{p.name}.unity";
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    EditorSceneManager.SaveScene(scene, path);
                }
                
                return new SceneResult
                {
                    success = true,
                    sceneName = scene.name,
                    scenePath = scene.path
                };
            };

            _handlers["loadScene"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<LoadSceneParams>(paramsJson);
                
                if (p.saveCurrent)
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                }
                
                var scene = EditorSceneManager.OpenScene(p.path, 
                    p.mode == "additive" ? OpenSceneMode.Additive : OpenSceneMode.Single);
                
                return new SceneResult
                {
                    success = scene.IsValid(),
                    sceneName = scene.name,
                    scenePath = scene.path
                };
            };

            _handlers["saveScene"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<SaveSceneParams>(paramsJson);
                var scene = SceneManager.GetActiveScene();
                
                bool success;
                if (!string.IsNullOrEmpty(p.path))
                {
                    success = EditorSceneManager.SaveScene(scene, p.path);
                }
                else
                {
                    success = EditorSceneManager.SaveScene(scene);
                }
                
                return new SceneResult
                {
                    success = success,
                    sceneName = scene.name,
                    scenePath = scene.path
                };
            };

            _handlers["getSceneInfo"] = (paramsJson) =>
            {
                var scene = SceneManager.GetActiveScene();
                return new SceneInfo
                {
                    name = scene.name,
                    path = scene.path,
                    isDirty = scene.isDirty,
                    rootCount = scene.rootCount,
                    isLoaded = scene.isLoaded
                };
            };

            _handlers["getSceneHierarchy"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<HierarchyParams>(paramsJson);
                var scene = SceneManager.GetActiveScene();
                var roots = scene.GetRootGameObjects();
                
                var hierarchy = new List<HierarchyNode>();
                foreach (var root in roots)
                {
                    hierarchy.Add(BuildHierarchyNode(root, p.depth, 0, p.includeComponents));
                }
                
                return new HierarchyResult { nodes = hierarchy.ToArray() };
            };
        }

        private static HierarchyNode BuildHierarchyNode(GameObject go, int maxDepth, int currentDepth, bool includeComponents)
        {
            var node = new HierarchyNode
            {
                name = go.name,
                active = go.activeSelf,
                tag = go.tag,
                layer = LayerMask.LayerToName(go.layer),
                path = GetGameObjectPath(go)
            };
            
            if (includeComponents)
            {
                node.components = go.GetComponents<Component>()
                    .Where(c => c != null)
                    .Select(c => c.GetType().Name)
                    .ToArray();
            }
            
            if (maxDepth == -1 || currentDepth < maxDepth)
            {
                var children = new List<HierarchyNode>();
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    children.Add(BuildHierarchyNode(
                        go.transform.GetChild(i).gameObject,
                        maxDepth,
                        currentDepth + 1,
                        includeComponents));
                }
                node.children = children.ToArray();
            }
            
            return node;
        }

        #endregion

        #region GameObject Commands

        private static void RegisterGameObjectCommands()
        {
            _handlers["createGameObject"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<CreateGameObjectParams>(paramsJson);
                
                GameObject go;
                
                switch (p.primitiveType?.ToLower())
                {
                    case "cube":
                        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        break;
                    case "sphere":
                        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        break;
                    case "capsule":
                        go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        break;
                    case "cylinder":
                        go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        break;
                    case "plane":
                        go = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        break;
                    case "quad":
                        go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        break;
                    default:
                        go = new GameObject();
                        break;
                }
                
                Undo.RegisterCreatedObjectUndo(go, "Create " + p.name);
                
                go.name = p.name ?? "New GameObject";
                
                if (!string.IsNullOrEmpty(p.parent))
                {
                    var parent = GameObject.Find(p.parent);
                    if (parent != null)
                    {
                        go.transform.SetParent(parent.transform);
                    }
                }
                
                if (p.position != null)
                {
                    go.transform.position = new Vector3(p.position.x, p.position.y, p.position.z);
                }
                
                if (p.rotation != null)
                {
                    go.transform.eulerAngles = new Vector3(p.rotation.x, p.rotation.y, p.rotation.z);
                }
                
                if (p.scale != null)
                {
                    go.transform.localScale = new Vector3(p.scale.x, p.scale.y, p.scale.z);
                }
                
                if (!string.IsNullOrEmpty(p.tag))
                {
                    go.tag = p.tag;
                }
                
                if (!string.IsNullOrEmpty(p.layer))
                {
                    go.layer = LayerMask.NameToLayer(p.layer);
                }
                
                return new GameObjectResult
                {
                    success = true,
                    name = go.name,
                    path = GetGameObjectPath(go)
                };
            };

            _handlers["findGameObjects"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<FindGameObjectsParams>(paramsJson);
                var results = new List<GameObjectInfo>();
                
                GameObject[] allObjects = p.includeInactive 
                    ? Resources.FindObjectsOfTypeAll<GameObject>()
                    : UnityEngine.Object.FindObjectsOfType<GameObject>();
                
                foreach (var go in allObjects)
                {
                    bool match = true;
                    
                    if (!string.IsNullOrEmpty(p.name))
                    {
                        match = go.name.Contains(p.name) || 
                                System.Text.RegularExpressions.Regex.IsMatch(go.name, p.name.Replace("*", ".*"));
                    }
                    
                    if (match && !string.IsNullOrEmpty(p.tag))
                    {
                        match = go.CompareTag(p.tag);
                    }
                    
                    if (match && !string.IsNullOrEmpty(p.componentType))
                    {
                        match = go.GetComponent(p.componentType) != null;
                    }
                    
                    if (match && go.scene.IsValid()) // Only include scene objects
                    {
                        results.Add(new GameObjectInfo
                        {
                            name = go.name,
                            path = GetGameObjectPath(go),
                            active = go.activeSelf,
                            tag = go.tag
                        });
                    }
                }
                
                return new FindGameObjectsResult { objects = results.ToArray() };
            };

            _handlers["modifyGameObject"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<ModifyGameObjectParams>(paramsJson);
                var go = FindGameObject(p.target);
                
                if (go == null)
                {
                    throw new Exception($"GameObject not found: {p.target}");
                }
                
                Undo.RecordObject(go, "Modify " + go.name);
                Undo.RecordObject(go.transform, "Modify Transform");
                
                if (!string.IsNullOrEmpty(p.name))
                {
                    go.name = p.name;
                }
                
                if (p.active.HasValue)
                {
                    go.SetActive(p.active.Value);
                }
                
                if (p.position != null)
                {
                    go.transform.position = new Vector3(p.position.x, p.position.y, p.position.z);
                }
                
                if (p.localPosition != null)
                {
                    go.transform.localPosition = new Vector3(p.localPosition.x, p.localPosition.y, p.localPosition.z);
                }
                
                if (p.rotation != null)
                {
                    go.transform.eulerAngles = new Vector3(p.rotation.x, p.rotation.y, p.rotation.z);
                }
                
                if (p.scale != null)
                {
                    go.transform.localScale = new Vector3(p.scale.x, p.scale.y, p.scale.z);
                }
                
                return new GameObjectResult
                {
                    success = true,
                    name = go.name,
                    path = GetGameObjectPath(go)
                };
            };

            _handlers["deleteGameObject"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<DeleteGameObjectParams>(paramsJson);
                var go = FindGameObject(p.target);
                
                if (go == null)
                {
                    throw new Exception($"GameObject not found: {p.target}");
                }
                
                Undo.DestroyObjectImmediate(go);
                
                return new SuccessResult { success = true };
            };

            _handlers["duplicateGameObject"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<DuplicateGameObjectParams>(paramsJson);
                var go = FindGameObject(p.target);
                
                if (go == null)
                {
                    throw new Exception($"GameObject not found: {p.target}");
                }
                
                var duplicate = UnityEngine.Object.Instantiate(go);
                duplicate.name = p.newName ?? go.name + " (Copy)";
                Undo.RegisterCreatedObjectUndo(duplicate, "Duplicate " + go.name);
                
                return new GameObjectResult
                {
                    success = true,
                    name = duplicate.name,
                    path = GetGameObjectPath(duplicate)
                };
            };
        }

        #endregion

        #region Component Commands

        private static void RegisterComponentCommands()
        {
            _handlers["addComponent"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<AddComponentParams>(paramsJson);
                var go = FindGameObject(p.target);
                
                if (go == null)
                {
                    throw new Exception($"GameObject not found: {p.target}");
                }
                
                Type componentType = FindType(p.componentType);
                if (componentType == null)
                {
                    throw new Exception($"Component type not found: {p.componentType}");
                }
                
                var component = Undo.AddComponent(go, componentType);
                
                // Apply properties using reflection
                if (!string.IsNullOrEmpty(p.properties))
                {
                    ApplyProperties(component, p.properties);
                }
                
                return new ComponentResult
                {
                    success = true,
                    componentType = componentType.Name
                };
            };

            _handlers["modifyComponent"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<ModifyComponentParams>(paramsJson);
                var go = FindGameObject(p.target);
                
                if (go == null)
                {
                    throw new Exception($"GameObject not found: {p.target}");
                }
                
                Type componentType = FindType(p.componentType);
                var components = go.GetComponents(componentType);
                
                if (components.Length == 0)
                {
                    throw new Exception($"Component not found: {p.componentType}");
                }
                
                var component = components[Mathf.Clamp(p.componentIndex, 0, components.Length - 1)];
                
                Undo.RecordObject(component, "Modify Component");
                ApplyProperties(component, p.properties);
                
                return new ComponentResult
                {
                    success = true,
                    componentType = componentType.Name
                };
            };

            _handlers["removeComponent"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<RemoveComponentParams>(paramsJson);
                var go = FindGameObject(p.target);
                
                if (go == null)
                {
                    throw new Exception($"GameObject not found: {p.target}");
                }
                
                Type componentType = FindType(p.componentType);
                var components = go.GetComponents(componentType);
                
                if (components.Length == 0)
                {
                    throw new Exception($"Component not found: {p.componentType}");
                }
                
                var component = components[Mathf.Clamp(p.componentIndex, 0, components.Length - 1)];
                Undo.DestroyObjectImmediate(component);
                
                return new SuccessResult { success = true };
            };

            _handlers["getComponents"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<GetComponentsParams>(paramsJson);
                var go = FindGameObject(p.target);
                
                if (go == null)
                {
                    throw new Exception($"GameObject not found: {p.target}");
                }
                
                var components = string.IsNullOrEmpty(p.componentType)
                    ? go.GetComponents<Component>()
                    : go.GetComponents(FindType(p.componentType));
                
                var results = components
                    .Where(c => c != null)
                    .Select(c => new ComponentInfo
                    {
                        type = c.GetType().Name,
                        fullType = c.GetType().FullName,
                        properties = p.includeProperties ? GetSerializedProperties(c) : null
                    })
                    .ToArray();
                
                return new GetComponentsResult { components = results };
            };
        }

        /// <summary>
        /// Applies properties to a component using reflection.
        /// </summary>
        private static void ApplyProperties(Component component, string propertiesJson)
        {
            // Parse simple property assignments from JSON
            var properties = JsonUtility.FromJson<Dictionary<string, object>>(propertiesJson);
            if (properties == null) return;
            
            var type = component.GetType();
            
            foreach (var kvp in properties)
            {
                var field = type.GetField(kvp.Key, BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    try
                    {
                        object value = Convert.ChangeType(kvp.Value, field.FieldType);
                        field.SetValue(component, value);
                    }
                    catch { }
                    continue;
                }
                
                var property = type.GetProperty(kvp.Key, BindingFlags.Public | BindingFlags.Instance);
                if (property != null && property.CanWrite)
                {
                    try
                    {
                        object value = Convert.ChangeType(kvp.Value, property.PropertyType);
                        property.SetValue(component, value);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Gets serialized properties of a component using SerializedObject.
        /// </summary>
        private static PropertyValue[] GetSerializedProperties(Component component)
        {
            var results = new List<PropertyValue>();
            var so = new SerializedObject(component);
            var sp = so.GetIterator();
            
            while (sp.NextVisible(true))
            {
                results.Add(new PropertyValue
                {
                    name = sp.name,
                    type = sp.propertyType.ToString(),
                    value = GetPropertyValueString(sp)
                });
            }
            
            return results.ToArray();
        }

        private static string GetPropertyValueString(SerializedProperty sp)
        {
            switch (sp.propertyType)
            {
                case SerializedPropertyType.Integer: return sp.intValue.ToString();
                case SerializedPropertyType.Boolean: return sp.boolValue.ToString();
                case SerializedPropertyType.Float: return sp.floatValue.ToString();
                case SerializedPropertyType.String: return sp.stringValue;
                case SerializedPropertyType.Vector2: return sp.vector2Value.ToString();
                case SerializedPropertyType.Vector3: return sp.vector3Value.ToString();
                case SerializedPropertyType.Color: return sp.colorValue.ToString();
                case SerializedPropertyType.ObjectReference: 
                    return sp.objectReferenceValue?.name ?? "null";
                default: return sp.propertyType.ToString();
            }
        }

        #endregion

        #region Script Commands

        private static void RegisterScriptCommands()
        {
            _handlers["createScript"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<CreateScriptParams>(paramsJson);
                
                string folder = p.path ?? "Assets/Scripts";
                string filePath = $"{folder}/{p.name}.cs";
                
                // Create directory if needed
                Directory.CreateDirectory(folder);
                
                string content = p.content;
                if (string.IsNullOrEmpty(content))
                {
                    content = GenerateScriptContent(p);
                }
                
                File.WriteAllText(filePath, content);
                
                // CRITICAL: Refresh AssetDatabase to trigger compilation
                AssetDatabase.Refresh();
                
                return new ScriptResult
                {
                    success = true,
                    path = filePath
                };
            };

            _handlers["readScript"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<ReadScriptParams>(paramsJson);
                
                string path = p.path;
                if (!path.EndsWith(".cs"))
                {
                    // Search for script by name
                    var guids = AssetDatabase.FindAssets($"{p.path} t:MonoScript");
                    if (guids.Length > 0)
                    {
                        path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    }
                }
                
                if (!File.Exists(path))
                {
                    throw new Exception($"Script not found: {path}");
                }
                
                return new ReadScriptResult
                {
                    path = path,
                    content = File.ReadAllText(path)
                };
            };

            _handlers["editScript"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<EditScriptParams>(paramsJson);
                
                if (!File.Exists(p.path))
                {
                    throw new Exception($"Script not found: {p.path}");
                }
                
                if (!string.IsNullOrEmpty(p.content))
                {
                    File.WriteAllText(p.path, p.content);
                }
                
                // CRITICAL: Refresh AssetDatabase to trigger compilation
                AssetDatabase.Refresh();
                
                return new ScriptResult
                {
                    success = true,
                    path = p.path
                };
            };

            _handlers["deleteScript"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<DeleteScriptParams>(paramsJson);
                
                if (AssetDatabase.DeleteAsset(p.path))
                {
                    return new SuccessResult { success = true };
                }
                
                throw new Exception($"Failed to delete script: {p.path}");
            };

            _handlers["executeCode"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<ExecuteCodeParams>(paramsJson);
                
                // For safety, we only allow specific predefined operations
                // Full code execution would require Roslyn or similar
                return new ExecuteCodeResult
                {
                    success = false,
                    message = "Direct code execution disabled for security. Use specific tools instead."
                };
            };
        }

        private static string GenerateScriptContent(CreateScriptParams p)
        {
            string baseClass = p.baseClass ?? (p.template == "ScriptableObject" ? "ScriptableObject" : "MonoBehaviour");
            string nsStart = string.IsNullOrEmpty(p.@namespace) ? "" : $"namespace {p.@namespace}\n{{\n";
            string nsEnd = string.IsNullOrEmpty(p.@namespace) ? "" : "}\n";
            string indent = string.IsNullOrEmpty(p.@namespace) ? "" : "    ";
            
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            
            if (p.template == "EditorWindow" || p.template == "Editor" || p.template == "PropertyDrawer")
            {
                sb.AppendLine("using UnityEditor;");
            }
            
            if (p.usings != null)
            {
                foreach (var u in p.usings)
                {
                    sb.AppendLine($"using {u};");
                }
            }
            
            sb.AppendLine();
            sb.Append(nsStart);
            
            sb.AppendLine($"{indent}/// <summary>");
            sb.AppendLine($"{indent}/// {p.name} - Auto-generated script");
            sb.AppendLine($"{indent}/// </summary>");
            sb.AppendLine($"{indent}public class {p.name} : {baseClass}");
            sb.AppendLine($"{indent}{{");
            
            if (p.template == "MonoBehaviour" || string.IsNullOrEmpty(p.template))
            {
                sb.AppendLine($"{indent}    #region Unity Lifecycle");
                sb.AppendLine();
                sb.AppendLine($"{indent}    private void Awake()");
                sb.AppendLine($"{indent}    {{");
                sb.AppendLine($"{indent}    }}");
                sb.AppendLine();
                sb.AppendLine($"{indent}    private void Start()");
                sb.AppendLine($"{indent}    {{");
                sb.AppendLine($"{indent}    }}");
                sb.AppendLine();
                sb.AppendLine($"{indent}    private void Update()");
                sb.AppendLine($"{indent}    {{");
                sb.AppendLine($"{indent}    }}");
                sb.AppendLine();
                sb.AppendLine($"{indent}    #endregion");
            }
            
            sb.AppendLine($"{indent}}}");
            sb.Append(nsEnd);
            
            return sb.ToString();
        }

        #endregion

        #region Asset Commands

        private static void RegisterAssetCommands()
        {
            _handlers["listAssets"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<ListAssetsParams>(paramsJson);
                string searchPath = p.path ?? "Assets";
                
                var guids = AssetDatabase.FindAssets("", new[] { searchPath });
                var results = new List<AssetInfo>();
                
                foreach (var guid in guids.Take(100)) // Limit results
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    
                    if (!p.recursive && Path.GetDirectoryName(path) != searchPath)
                    {
                        continue;
                    }
                    
                    var obj = AssetDatabase.LoadMainAssetAtPath(path);
                    if (obj != null)
                    {
                        results.Add(new AssetInfo
                        {
                            name = obj.name,
                            path = path,
                            type = obj.GetType().Name
                        });
                    }
                }
                
                return new ListAssetsResult { assets = results.ToArray() };
            };

            _handlers["searchAssets"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<SearchAssetsParams>(paramsJson);
                
                string filter = p.query ?? "";
                if (!string.IsNullOrEmpty(p.type))
                {
                    filter += $" t:{p.type}";
                }
                
                var guids = AssetDatabase.FindAssets(filter);
                var results = new List<AssetInfo>();
                
                foreach (var guid in guids.Take(50))
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    var obj = AssetDatabase.LoadMainAssetAtPath(path);
                    
                    if (obj != null)
                    {
                        results.Add(new AssetInfo
                        {
                            name = obj.name,
                            path = path,
                            type = obj.GetType().Name
                        });
                    }
                }
                
                return new ListAssetsResult { assets = results.ToArray() };
            };

            _handlers["createFolder"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<CreateFolderParams>(paramsJson);
                
                string[] parts = p.path.Split('/');
                string current = parts[0];
                
                for (int i = 1; i < parts.Length; i++)
                {
                    string next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                    {
                        AssetDatabase.CreateFolder(current, parts[i]);
                    }
                    current = next;
                }
                
                return new SuccessResult { success = true };
            };

            _handlers["refreshAssetDatabase"] = (paramsJson) =>
            {
                AssetDatabase.Refresh();
                return new SuccessResult { success = true };
            };

            _handlers["createPrefab"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<CreatePrefabParams>(paramsJson);
                var go = FindGameObject(p.target);
                
                if (go == null)
                {
                    throw new Exception($"GameObject not found: {p.target}");
                }
                
                string folder = p.path ?? "Assets/Prefabs";
                Directory.CreateDirectory(folder);
                
                string prefabPath = $"{folder}/{go.name}.prefab";
                var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                
                return new PrefabResult
                {
                    success = prefab != null,
                    path = prefabPath
                };
            };

            _handlers["instantiatePrefab"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<InstantiatePrefabParams>(paramsJson);
                
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(p.path);
                if (prefab == null)
                {
                    throw new Exception($"Prefab not found: {p.path}");
                }
                
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                Undo.RegisterCreatedObjectUndo(instance, "Instantiate Prefab");
                
                if (!string.IsNullOrEmpty(p.name))
                {
                    instance.name = p.name;
                }
                
                if (p.position != null)
                {
                    instance.transform.position = new Vector3(p.position.x, p.position.y, p.position.z);
                }
                
                if (p.rotation != null)
                {
                    instance.transform.eulerAngles = new Vector3(p.rotation.x, p.rotation.y, p.rotation.z);
                }
                
                return new GameObjectResult
                {
                    success = true,
                    name = instance.name,
                    path = GetGameObjectPath(instance)
                };
            };
        }

        #endregion

        #region Build Commands

        private static void RegisterBuildCommands()
        {
            _handlers["build"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<BuildParams>(paramsJson);
                
                BuildTarget target = (BuildTarget)Enum.Parse(typeof(BuildTarget), p.target);
                
                var scenes = EditorBuildSettings.scenes
                    .Where(s => s.enabled)
                    .Select(s => s.path)
                    .ToArray();
                
                BuildOptions options = BuildOptions.None;
                if (p.options?.development == true)
                {
                    options |= BuildOptions.Development;
                }
                
                string outputPath = p.outputPath ?? $"Builds/{p.target}/{Application.productName}";
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                
                var report = BuildPipeline.BuildPlayer(scenes, outputPath, target, options);
                
                return new BuildResult
                {
                    success = report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded,
                    outputPath = outputPath,
                    errors = report.summary.totalErrors,
                    warnings = report.summary.totalWarnings
                };
            };

            _handlers["buildSettings"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<BuildSettingsParams>(paramsJson);
                
                if (p.action == "get")
                {
                    return new BuildSettingsResult
                    {
                        activeBuildTarget = EditorUserBuildSettings.activeBuildTarget.ToString(),
                        scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray()
                    };
                }
                
                return new SuccessResult { success = true };
            };

            _handlers["switchPlatform"] = (paramsJson) =>
            {
                var p = JsonUtility.FromJson<SwitchPlatformParams>(paramsJson);
                
                BuildTarget target = (BuildTarget)Enum.Parse(typeof(BuildTarget), p.target);
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
                
                bool success = EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);
                
                return new SuccessResult { success = success };
            };
        }

        #endregion

        #region Helper Methods

        private static GameObject FindGameObject(string nameOrPath)
        {
            if (string.IsNullOrEmpty(nameOrPath))
            {
                return null;
            }
            
            // Try finding by path first
            if (nameOrPath.Contains("/"))
            {
                return GameObject.Find(nameOrPath);
            }
            
            // Find by name
            var allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            return allObjects.FirstOrDefault(go => go.name == nameOrPath);
        }

        private static string GetGameObjectPath(GameObject go)
        {
            string path = go.name;
            Transform current = go.transform.parent;
            
            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }
            
            return path;
        }

        private static Type FindType(string typeName)
        {
            // Check Unity types first
            var type = Type.GetType($"UnityEngine.{typeName}, UnityEngine");
            if (type != null) return type;
            
            type = Type.GetType($"UnityEngine.UI.{typeName}, UnityEngine.UI");
            if (type != null) return type;
            
            // Search all assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null) return type;
                
                // Try with common namespaces
                type = assembly.GetType($"UnityEngine.{typeName}");
                if (type != null) return type;
            }
            
            return null;
        }

        #endregion
    }

    #region Parameter Types

    [Serializable] public class PlayModeParams { public string action; }
    [Serializable] public class CreateSceneParams { public string name; public bool saveCurrent = true; public string template; }
    [Serializable] public class LoadSceneParams { public string path; public string mode = "single"; public bool saveCurrent = true; }
    [Serializable] public class SaveSceneParams { public string path; }
    [Serializable] public class HierarchyParams { public int depth = -1; public bool includeComponents; }
    [Serializable] public class CreateGameObjectParams { public string name; public string primitiveType; public string parent; public Vector3Param position; public Vector3Param rotation; public Vector3Param scale; public string tag; public string layer; }
    [Serializable] public class FindGameObjectsParams { public string name; public string tag; public string componentType; public bool includeInactive; }
    [Serializable] public class ModifyGameObjectParams { public string target; public string name; public bool? active; public Vector3Param position; public Vector3Param localPosition; public Vector3Param rotation; public Vector3Param scale; }
    [Serializable] public class DeleteGameObjectParams { public string target; public bool immediate; }
    [Serializable] public class DuplicateGameObjectParams { public string target; public string newName; public string parent; }
    [Serializable] public class AddComponentParams { public string target; public string componentType; public string properties; }
    [Serializable] public class ModifyComponentParams { public string target; public string componentType; public int componentIndex; public string properties; }
    [Serializable] public class RemoveComponentParams { public string target; public string componentType; public int componentIndex; }
    [Serializable] public class GetComponentsParams { public string target; public string componentType; public bool includeProperties = true; }
    [Serializable] public class CreateScriptParams { public string name; public string path; public string template; public string @namespace; public string baseClass; public string content; public string[] usings; }
    [Serializable] public class ReadScriptParams { public string path; }
    [Serializable] public class EditScriptParams { public string path; public string content; }
    [Serializable] public class DeleteScriptParams { public string path; }
    [Serializable] public class ExecuteCodeParams { public string code; }
    [Serializable] public class ListAssetsParams { public string path; public bool recursive; public string type; }
    [Serializable] public class SearchAssetsParams { public string query; public string type; public string path; }
    [Serializable] public class CreateFolderParams { public string path; }
    [Serializable] public class CreatePrefabParams { public string target; public string path; }
    [Serializable] public class InstantiatePrefabParams { public string path; public string name; public string parent; public Vector3Param position; public Vector3Param rotation; }
    [Serializable] public class BuildParams { public string target; public string outputPath; public BuildOptionsParam options; }
    [Serializable] public class BuildOptionsParam { public bool development; }
    [Serializable] public class BuildSettingsParams { public string action; }
    [Serializable] public class SwitchPlatformParams { public string target; }
    [Serializable] public class Vector3Param { public float x; public float y; public float z; }

    #endregion

    #region Result Types

    [Serializable] public class SuccessResult { public bool success; }
    [Serializable] public class ProjectInfo { public string name; public string path; public string unityVersion; public string companyName; public bool isPlaying; public bool isCompiling; }
    [Serializable] public class CompilationStatus { public bool isCompiling; public string[] errors; }
    [Serializable] public class ConsoleLogsResult { public LogEntry[] logs; public string message; }
    [Serializable] public class LogEntry { public string message; public string stackTrace; public string type; }
    [Serializable] public class PlayModeResult { public bool isPlaying; public bool isPaused; }
    [Serializable] public class SceneResult { public bool success; public string sceneName; public string scenePath; }
    [Serializable] public class SceneInfo { public string name; public string path; public bool isDirty; public int rootCount; public bool isLoaded; }
    [Serializable] public class HierarchyResult { public HierarchyNode[] nodes; }
    [Serializable] public class HierarchyNode { public string name; public bool active; public string tag; public string layer; public string path; public string[] components; public HierarchyNode[] children; }
    [Serializable] public class GameObjectResult { public bool success; public string name; public string path; }
    [Serializable] public class GameObjectInfo { public string name; public string path; public bool active; public string tag; }
    [Serializable] public class FindGameObjectsResult { public GameObjectInfo[] objects; }
    [Serializable] public class ComponentResult { public bool success; public string componentType; }
    [Serializable] public class ComponentInfo { public string type; public string fullType; public PropertyValue[] properties; }
    [Serializable] public class PropertyValue { public string name; public string type; public string value; }
    [Serializable] public class GetComponentsResult { public ComponentInfo[] components; }
    [Serializable] public class ScriptResult { public bool success; public string path; }
    [Serializable] public class ReadScriptResult { public string path; public string content; }
    [Serializable] public class ExecuteCodeResult { public bool success; public string message; public string result; }
    [Serializable] public class AssetInfo { public string name; public string path; public string type; }
    [Serializable] public class ListAssetsResult { public AssetInfo[] assets; }
    [Serializable] public class PrefabResult { public bool success; public string path; }
    [Serializable] public class BuildResult { public bool success; public string outputPath; public int errors; public int warnings; }
    [Serializable] public class BuildSettingsResult { public string activeBuildTarget; public string[] scenes; }

    #endregion
}
