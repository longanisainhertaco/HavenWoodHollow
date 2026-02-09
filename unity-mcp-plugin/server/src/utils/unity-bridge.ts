/**
 * Unity Bridge - TCP/WebSocket Communication Layer
 * 
 * Implements the "Bridge Pattern" for persistent communication between
 * the MCP server and Unity Editor. Handles Unity domain reloads gracefully
 * by buffering requests and implementing automatic reconnection.
 * 
 * Key Features:
 * - Buffers requests when Unity is compiling/reloading
 * - Automatic reconnection with polling
 * - Self-healing compilation loop
 * - Thread-safe message queuing
 */

import * as net from 'net';

interface UnityMessage {
  id: string;
  type: 'request' | 'response' | 'event';
  method?: string;
  params?: Record<string, unknown>;
  result?: unknown;
  error?: { code: number; message: string };
}

interface PendingRequest {
  resolve: (value: unknown) => void;
  reject: (error: Error) => void;
  timeout: NodeJS.Timeout;
  method: string;
  params: Record<string, unknown>;
}

interface QueuedRequest {
  method: string;
  params: Record<string, unknown>;
  resolve: (value: unknown) => void;
  reject: (error: Error) => void;
}

type ConnectionState = 'disconnected' | 'connecting' | 'connected' | 'unity_compiling' | 'polling';

export class UnityBridge {
  private socket: net.Socket | null = null;
  private connectionState: ConnectionState = 'disconnected';
  private requestId = 0;
  private pendingRequests = new Map<string, PendingRequest>();
  private eventHandlers = new Map<string, ((data: unknown) => void)[]>();
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 10;
  private reconnectDelay = 1000;
  private currentPort = 8090;
  private pollingInterval: NodeJS.Timeout | null = null;
  private messageBuffer = '';
  
  // Request queue for when Unity is compiling
  private requestQueue: QueuedRequest[] = [];
  private isUnityCompiling = false;
  
  // Compilation error tracking for self-healing
  private lastCompilationErrors: string[] = [];
  private compilationCheckPending = false;

  /**
   * Connect to Unity Editor via TCP Socket
   * Uses TCP for reliability during domain reloads
   */
  async connect(port: number = 8090): Promise<void> {
    if (this.connectionState === 'connected') {
      throw new Error('Already connected to Unity');
    }

    this.currentPort = port;
    this.connectionState = 'connecting';
    
    return new Promise((resolve, reject) => {
      try {
        this.socket = new net.Socket();
        
        const connectionTimeout = setTimeout(() => {
          this.socket?.destroy();
          this.connectionState = 'disconnected';
          reject(new Error(`Connection timeout: Could not connect to Unity at localhost:${port}`));
        }, 10000);

        this.socket.connect(port, 'localhost', () => {
          clearTimeout(connectionTimeout);
          this.connectionState = 'connected';
          this.reconnectAttempts = 0;
          this.stopPolling();
          console.error(`Connected to Unity at localhost:${port}`);
          
          // Check for compilation errors after reconnect
          if (this.compilationCheckPending) {
            this.checkCompilationStatus();
          }
          
          // Process any queued requests
          this.processRequestQueue();
          
          resolve();
        });

        this.socket.on('data', (data) => {
          this.handleData(data);
        });

        this.socket.on('close', () => {
          console.error('Disconnected from Unity');
          this.handleDisconnect();
        });

        this.socket.on('error', (error) => {
          clearTimeout(connectionTimeout);
          console.error(`Socket error: ${error.message}`);
          if (this.connectionState === 'connecting') {
            this.connectionState = 'disconnected';
            reject(new Error(`Failed to connect to Unity: ${error.message}. Make sure Unity is running with the UnityBridge Editor plugin.`));
          }
        });
      } catch (error) {
        this.connectionState = 'disconnected';
        reject(new Error(`Failed to create socket connection: ${error instanceof Error ? error.message : 'Unknown error'}`));
      }
    });
  }

  /**
   * Disconnect from Unity Editor
   */
  disconnect(): void {
    this.stopPolling();
    if (this.socket) {
      this.socket.destroy();
      this.socket = null;
    }
    this.connectionState = 'disconnected';
    this.pendingRequests.forEach((request) => {
      clearTimeout(request.timeout);
      request.reject(new Error('Disconnected from Unity'));
    });
    this.pendingRequests.clear();
  }

  /**
   * Check if connected to Unity
   */
  isConnected(): boolean {
    return this.connectionState === 'connected';
  }

  /**
   * Get current connection state
   */
  getConnectionState(): ConnectionState {
    return this.connectionState;
  }

  /**
   * Check if Unity is currently compiling
   */
  isCompiling(): boolean {
    return this.isUnityCompiling || this.connectionState === 'unity_compiling';
  }

  /**
   * Send a command to Unity and wait for response
   * Automatically queues requests if Unity is compiling
   */
  async send<T = unknown>(method: string, params: Record<string, unknown>): Promise<T> {
    // If Unity is compiling or disconnected, queue the request
    if (this.isUnityCompiling || this.connectionState === 'unity_compiling') {
      console.error(`Unity is compiling. Queuing request: ${method}`);
      return this.queueRequest<T>(method, params);
    }
    
    if (this.connectionState !== 'connected') {
      // Try to reconnect first
      if (this.connectionState === 'polling') {
        console.error(`Waiting for Unity to reconnect. Queuing request: ${method}`);
        return this.queueRequest<T>(method, params);
      }
      throw new Error('Not connected to Unity. Call connect() first.');
    }

    const id = `req_${++this.requestId}`;
    const message: UnityMessage = {
      id,
      type: 'request',
      method,
      params,
    };

    return new Promise((resolve, reject) => {
      const timeout = setTimeout(() => {
        this.pendingRequests.delete(id);
        reject(new Error(`Request timeout: ${method}`));
      }, 30000);

      this.pendingRequests.set(id, {
        resolve: resolve as (value: unknown) => void,
        reject,
        timeout,
        method,
        params,
      });

      try {
        const jsonMessage = JSON.stringify(message) + '\n'; // Newline delimiter
        this.socket!.write(jsonMessage);
      } catch (error) {
        clearTimeout(timeout);
        this.pendingRequests.delete(id);
        reject(new Error(`Failed to send message: ${error instanceof Error ? error.message : 'Unknown error'}`));
      }
    });
  }

  /**
   * Queue a request for when Unity finishes compiling
   */
  private queueRequest<T>(method: string, params: Record<string, unknown>): Promise<T> {
    return new Promise((resolve, reject) => {
      this.requestQueue.push({
        method,
        params,
        resolve: resolve as (value: unknown) => void,
        reject,
      });
    });
  }

  /**
   * Process queued requests after reconnection
   */
  private async processRequestQueue(): Promise<void> {
    console.error(`Processing ${this.requestQueue.length} queued requests`);
    
    while (this.requestQueue.length > 0) {
      const request = this.requestQueue.shift()!;
      try {
        const result = await this.send(request.method, request.params);
        request.resolve(result);
      } catch (error) {
        request.reject(error instanceof Error ? error : new Error('Unknown error'));
      }
    }
  }

  /**
   * Register an event handler for Unity events
   */
  on(event: string, handler: (data: unknown) => void): void {
    const handlers = this.eventHandlers.get(event) || [];
    handlers.push(handler);
    this.eventHandlers.set(event, handlers);
  }

  /**
   * Remove an event handler
   */
  off(event: string, handler: (data: unknown) => void): void {
    const handlers = this.eventHandlers.get(event) || [];
    const index = handlers.indexOf(handler);
    if (index !== -1) {
      handlers.splice(index, 1);
      this.eventHandlers.set(event, handlers);
    }
  }

  /**
   * Get last compilation errors (for self-healing loop)
   */
  getLastCompilationErrors(): string[] {
    return this.lastCompilationErrors;
  }

  /**
   * Handle incoming TCP data (may contain multiple messages)
   */
  private handleData(data: Buffer): void {
    this.messageBuffer += data.toString();
    
    // Process complete messages (newline delimited)
    const messages = this.messageBuffer.split('\n');
    this.messageBuffer = messages.pop() || ''; // Keep incomplete message in buffer
    
    for (const messageStr of messages) {
      if (messageStr.trim()) {
        this.handleMessage(messageStr);
      }
    }
  }

  /**
   * Handle incoming messages
   */
  private handleMessage(data: string): void {
    try {
      const message: UnityMessage = JSON.parse(data);

      if (message.type === 'response') {
        const pending = this.pendingRequests.get(message.id);
        if (pending) {
          clearTimeout(pending.timeout);
          this.pendingRequests.delete(message.id);

          if (message.error) {
            pending.reject(new Error(message.error.message));
          } else {
            pending.resolve(message.result);
          }
        }
      } else if (message.type === 'event' && message.method) {
        // Handle special events
        if (message.method === 'compilationStarted') {
          this.isUnityCompiling = true;
          this.connectionState = 'unity_compiling';
          console.error('Unity compilation started');
        } else if (message.method === 'compilationFinished') {
          this.isUnityCompiling = false;
          this.lastCompilationErrors = (message.params?.errors as string[]) || [];
          if (this.connectionState === 'unity_compiling') {
            this.connectionState = 'connected';
          }
          console.error(`Unity compilation finished. Errors: ${this.lastCompilationErrors.length}`);
        }
        
        // Dispatch to registered handlers
        const handlers = this.eventHandlers.get(message.method) || [];
        handlers.forEach((handler) => {
          try {
            handler(message.params);
          } catch (error) {
            console.error(`Event handler error for ${message.method}:`, error);
          }
        });
      }
    } catch (error) {
      console.error('Failed to parse Unity message:', error);
    }
  }

  /**
   * Check compilation status after reconnection
   */
  private async checkCompilationStatus(): Promise<void> {
    this.compilationCheckPending = false;
    try {
      const status = await this.send<{ isCompiling: boolean; errors: string[] }>('getCompilationStatus', {});
      this.lastCompilationErrors = status.errors || [];
      this.isUnityCompiling = status.isCompiling;
      
      if (this.lastCompilationErrors.length > 0) {
        console.error('Compilation errors detected:', this.lastCompilationErrors);
        // Emit event so MCP server can notify Claude
        const handlers = this.eventHandlers.get('compilationErrors') || [];
        handlers.forEach((handler) => handler({ errors: this.lastCompilationErrors }));
      }
    } catch (error) {
      console.error('Failed to check compilation status:', error);
    }
  }

  /**
   * Start polling for Unity reconnection
   */
  private startPolling(): void {
    if (this.pollingInterval) return;
    
    this.connectionState = 'polling';
    console.error('Starting reconnection polling...');
    
    this.pollingInterval = setInterval(async () => {
      if (this.reconnectAttempts >= this.maxReconnectAttempts) {
        this.stopPolling();
        console.error('Max reconnection attempts reached');
        // Reject all queued requests
        this.requestQueue.forEach((request) => {
          request.reject(new Error('Unity connection lost. Max reconnection attempts reached.'));
        });
        this.requestQueue = [];
        return;
      }
      
      this.reconnectAttempts++;
      console.error(`Reconnection attempt ${this.reconnectAttempts}/${this.maxReconnectAttempts}`);
      
      try {
        await this.connect(this.currentPort);
        // Success - polling will be stopped in connect()
      } catch {
        // Continue polling
      }
    }, this.reconnectDelay);
  }

  /**
   * Stop polling for reconnection
   */
  private stopPolling(): void {
    if (this.pollingInterval) {
      clearInterval(this.pollingInterval);
      this.pollingInterval = null;
    }
  }

  /**
   * Handle disconnection - implements self-healing workflow
   */
  private handleDisconnect(): void {
    this.connectionState = 'disconnected';
    
    // Reject pending requests that won't survive reconnect
    this.pendingRequests.forEach((request) => {
      clearTimeout(request.timeout);
      // Re-queue the request instead of rejecting
      this.requestQueue.push({
        method: request.method,
        params: request.params,
        resolve: request.resolve,
        reject: request.reject,
      });
    });
    this.pendingRequests.clear();

    // Mark that we need to check compilation status on reconnect
    this.compilationCheckPending = true;

    // Start polling for reconnection
    this.startPolling();
  }
}
