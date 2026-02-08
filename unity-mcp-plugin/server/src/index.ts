#!/usr/bin/env node
/**
 * Unity MCP Server - Main Entry Point
 * 
 * This server provides MCP (Model Context Protocol) integration for Unity game development,
 * enabling Claude Desktop, Cowork, and Claude Code to control and edit Unity projects.
 */

import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import {
  CallToolRequestSchema,
  ListToolsRequestSchema,
  ListResourcesRequestSchema,
  ReadResourceRequestSchema,
  ListPromptsRequestSchema,
  GetPromptRequestSchema,
} from '@modelcontextprotocol/sdk/types.js';

import { UnityBridge } from './utils/unity-bridge.js';
import { registerSceneTools } from './tools/scene-tools.js';
import { registerGameObjectTools } from './tools/gameobject-tools.js';
import { registerScriptTools } from './tools/script-tools.js';
import { registerAssetTools } from './tools/asset-tools.js';
import { registerBuildTools } from './tools/build-tools.js';
import { registerProjectTools } from './tools/project-tools.js';

const SERVER_NAME = 'unity-game-dev';
const SERVER_VERSION = '1.0.0';

// Initialize the Unity Bridge
const unityBridge = new UnityBridge();

// Create MCP Server
const server = new Server(
  {
    name: SERVER_NAME,
    version: SERVER_VERSION,
  },
  {
    capabilities: {
      tools: {},
      resources: {},
      prompts: {},
    },
  }
);

// Collect all tools from modules
const allTools: Map<string, { description: string; inputSchema: object; handler: (args: Record<string, unknown>) => Promise<unknown> }> = new Map();

// Register tools from each module
registerSceneTools(allTools, unityBridge);
registerGameObjectTools(allTools, unityBridge);
registerScriptTools(allTools, unityBridge);
registerAssetTools(allTools, unityBridge);
registerBuildTools(allTools, unityBridge);
registerProjectTools(allTools, unityBridge);

// Add connection tool
allTools.set('unity_connect', {
  description: 'Connect to a running Unity Editor instance. Must be called before using other Unity tools.',
  inputSchema: {
    type: 'object',
    properties: {
      port: {
        type: 'number',
        description: 'TCP port for Unity Bridge (default: 8090)',
        default: 8090,
      },
      projectPath: {
        type: 'string',
        description: 'Optional path to Unity project to verify connection',
      },
    },
    required: [],
  },
  handler: async (args) => {
    const port = (args.port as number) || 8090;
    const projectPath = args.projectPath as string | undefined;
    
    try {
      await unityBridge.connect(port);
      
      if (projectPath) {
        const projectInfo = await unityBridge.send('getProjectInfo', {});
        if (projectInfo.path !== projectPath) {
          return {
            success: false,
            message: `Connected to Unity but project path doesn't match. Expected: ${projectPath}, Got: ${projectInfo.path}`,
          };
        }
      }
      
      return {
        success: true,
        message: 'Successfully connected to Unity Editor',
        projectInfo: await unityBridge.send('getProjectInfo', {}),
      };
    } catch (error) {
      return {
        success: false,
        message: `Failed to connect to Unity: ${error instanceof Error ? error.message : 'Unknown error'}`,
      };
    }
  },
});

allTools.set('unity_disconnect', {
  description: 'Disconnect from the Unity Editor instance.',
  inputSchema: {
    type: 'object',
    properties: {},
    required: [],
  },
  handler: async () => {
    unityBridge.disconnect();
    return { success: true, message: 'Disconnected from Unity Editor' };
  },
});

allTools.set('unity_status', {
  description: 'Check the connection status with Unity Editor.',
  inputSchema: {
    type: 'object',
    properties: {},
    required: [],
  },
  handler: async () => {
    return {
      connected: unityBridge.isConnected(),
      message: unityBridge.isConnected() 
        ? 'Connected to Unity Editor' 
        : 'Not connected to Unity Editor. Use unity_connect first.',
    };
  },
});

// Handle list tools request
server.setRequestHandler(ListToolsRequestSchema, async () => {
  const tools = Array.from(allTools.entries()).map(([name, tool]) => ({
    name,
    description: tool.description,
    inputSchema: tool.inputSchema,
  }));
  
  return { tools };
});

// Handle tool calls
server.setRequestHandler(CallToolRequestSchema, async (request) => {
  const { name, arguments: args } = request.params;
  
  const tool = allTools.get(name);
  if (!tool) {
    throw new Error(`Unknown tool: ${name}`);
  }
  
  try {
    const result = await tool.handler(args || {});
    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify(result, null, 2),
        },
      ],
    };
  } catch (error) {
    return {
      content: [
        {
          type: 'text',
          text: `Error executing ${name}: ${error instanceof Error ? error.message : 'Unknown error'}`,
        },
      ],
      isError: true,
    };
  }
});

// Handle list resources request
server.setRequestHandler(ListResourcesRequestSchema, async () => {
  return {
    resources: [
      {
        uri: 'unity://project/info',
        name: 'Project Info',
        description: 'Current Unity project information including name, version, and settings',
        mimeType: 'application/json',
      },
      {
        uri: 'unity://scene/hierarchy',
        name: 'Scene Hierarchy',
        description: 'Current scene hierarchy with all GameObjects',
        mimeType: 'application/json',
      },
      {
        uri: 'unity://console/logs',
        name: 'Console Logs',
        description: 'Recent Unity console log entries',
        mimeType: 'application/json',
      },
      {
        uri: 'unity://assets/list',
        name: 'Asset List',
        description: 'List of assets in the project',
        mimeType: 'application/json',
      },
    ],
  };
});

// Handle read resource request
server.setRequestHandler(ReadResourceRequestSchema, async (request) => {
  const { uri } = request.params;
  
  if (!unityBridge.isConnected()) {
    return {
      contents: [
        {
          uri,
          mimeType: 'application/json',
          text: JSON.stringify({ error: 'Not connected to Unity. Use unity_connect first.' }),
        },
      ],
    };
  }
  
  try {
    let data: unknown;
    
    switch (uri) {
      case 'unity://project/info':
        data = await unityBridge.send('getProjectInfo', {});
        break;
      case 'unity://scene/hierarchy':
        data = await unityBridge.send('getSceneHierarchy', {});
        break;
      case 'unity://console/logs':
        data = await unityBridge.send('getConsoleLogs', { count: 100 });
        break;
      case 'unity://assets/list':
        data = await unityBridge.send('listAssets', { path: 'Assets' });
        break;
      default:
        throw new Error(`Unknown resource: ${uri}`);
    }
    
    return {
      contents: [
        {
          uri,
          mimeType: 'application/json',
          text: JSON.stringify(data, null, 2),
        },
      ],
    };
  } catch (error) {
    return {
      contents: [
        {
          uri,
          mimeType: 'application/json',
          text: JSON.stringify({ error: error instanceof Error ? error.message : 'Unknown error' }),
        },
      ],
    };
  }
});

// Handle list prompts request
server.setRequestHandler(ListPromptsRequestSchema, async () => {
  return {
    prompts: [
      {
        name: 'create_game_object',
        description: 'Guide for creating a new GameObject with components',
        arguments: [
          { name: 'objectType', description: 'Type of GameObject (e.g., Player, Enemy, UI)', required: true },
        ],
      },
      {
        name: 'create_script',
        description: 'Guide for creating a new C# script with Unity best practices',
        arguments: [
          { name: 'scriptName', description: 'Name of the script', required: true },
          { name: 'scriptType', description: 'Type of script (MonoBehaviour, ScriptableObject, Editor)', required: false },
        ],
      },
      {
        name: 'setup_scene',
        description: 'Guide for setting up a new game scene',
        arguments: [
          { name: 'sceneType', description: 'Type of scene (MainMenu, GameLevel, UI)', required: true },
        ],
      },
      {
        name: 'build_project',
        description: 'Guide for building the Unity project',
        arguments: [
          { name: 'platform', description: 'Target platform (Windows, Mac, Linux, Android, iOS, WebGL)', required: true },
        ],
      },
    ],
  };
});

// Handle get prompt request
server.setRequestHandler(GetPromptRequestSchema, async (request) => {
  const { name, arguments: args } = request.params;
  
  switch (name) {
    case 'create_game_object':
      return {
        messages: [
          {
            role: 'user',
            content: {
              type: 'text',
              text: `I want to create a ${args?.objectType || 'generic'} GameObject in Unity. Please help me:
1. First, check if we're connected to Unity
2. Create the appropriate GameObject with a descriptive name
3. Add relevant components based on the object type
4. Set appropriate initial values for the components
5. Position it in the scene appropriately`,
            },
          },
        ],
      };
      
    case 'create_script':
      return {
        messages: [
          {
            role: 'user',
            content: {
              type: 'text',
              text: `I want to create a new C# script called "${args?.scriptName || 'NewScript'}" of type ${args?.scriptType || 'MonoBehaviour'}. Please:
1. Create the script with proper Unity conventions
2. Include appropriate using statements
3. Add common Unity lifecycle methods as needed
4. Include XML documentation comments
5. Follow Unity C# best practices`,
            },
          },
        ],
      };
      
    case 'setup_scene':
      return {
        messages: [
          {
            role: 'user',
            content: {
              type: 'text',
              text: `I want to set up a ${args?.sceneType || 'game'} scene in Unity. Please help me:
1. Create a new scene or clear the current one
2. Add essential GameObjects for this scene type
3. Set up the camera appropriately
4. Add necessary lighting
5. Configure any required managers or systems`,
            },
          },
        ],
      };
      
    case 'build_project':
      return {
        messages: [
          {
            role: 'user',
            content: {
              type: 'text',
              text: `I want to build this Unity project for ${args?.platform || 'the current platform'}. Please:
1. Check the current build settings
2. Verify all scenes are included in the build
3. Set the correct build target
4. Configure platform-specific settings
5. Execute the build and report results`,
            },
          },
        ],
      };
      
    default:
      throw new Error(`Unknown prompt: ${name}`);
  }
});

// Main entry point
async function main() {
  const transport = new StdioServerTransport();
  await server.connect(transport);
  console.error('Unity MCP Server started');
}

main().catch((error) => {
  console.error('Fatal error:', error);
  process.exit(1);
});
