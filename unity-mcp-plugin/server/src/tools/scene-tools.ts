/**
 * Scene Tools - Unity Scene Management
 * 
 * Tools for creating, loading, saving, and managing Unity scenes.
 */

import { UnityBridge } from '../utils/unity-bridge.js';

type ToolHandler = (args: Record<string, unknown>) => Promise<unknown>;
type ToolDefinition = {
  description: string;
  inputSchema: object;
  handler: ToolHandler;
};
type ToolMap = Map<string, ToolDefinition>;

export function registerSceneTools(tools: ToolMap, bridge: UnityBridge): void {
  
  tools.set('unity_scene_create', {
    description: 'Create a new Unity scene. Optionally saves the current scene first.',
    inputSchema: {
      type: 'object',
      properties: {
        name: {
          type: 'string',
          description: 'Name for the new scene (without .unity extension)',
        },
        saveCurrent: {
          type: 'boolean',
          description: 'Save the current scene before creating a new one',
          default: true,
        },
        template: {
          type: 'string',
          description: 'Scene template to use (empty, basic, 2d, 3d)',
          enum: ['empty', 'basic', '2d', '3d'],
          default: 'basic',
        },
      },
      required: ['name'],
    },
    handler: async (args) => {
      const name = args.name as string;
      const saveCurrent = args.saveCurrent !== false;
      const template = (args.template as string) || 'basic';
      
      return await bridge.send('createScene', {
        name,
        saveCurrent,
        template,
      });
    },
  });

  tools.set('unity_scene_load', {
    description: 'Load an existing Unity scene by path or name.',
    inputSchema: {
      type: 'object',
      properties: {
        path: {
          type: 'string',
          description: 'Path to the scene file (relative to Assets folder) or scene name',
        },
        mode: {
          type: 'string',
          description: 'How to load the scene',
          enum: ['single', 'additive'],
          default: 'single',
        },
        saveCurrent: {
          type: 'boolean',
          description: 'Save the current scene before loading',
          default: true,
        },
      },
      required: ['path'],
    },
    handler: async (args) => {
      const path = args.path as string;
      const mode = (args.mode as string) || 'single';
      const saveCurrent = args.saveCurrent !== false;
      
      return await bridge.send('loadScene', {
        path,
        mode,
        saveCurrent,
      });
    },
  });

  tools.set('unity_scene_save', {
    description: 'Save the current Unity scene.',
    inputSchema: {
      type: 'object',
      properties: {
        path: {
          type: 'string',
          description: 'Optional new path to save the scene (for Save As)',
        },
      },
      required: [],
    },
    handler: async (args) => {
      const path = args.path as string | undefined;
      
      return await bridge.send('saveScene', { path });
    },
  });

  tools.set('unity_scene_info', {
    description: 'Get information about the currently loaded scene(s).',
    inputSchema: {
      type: 'object',
      properties: {
        includeHierarchy: {
          type: 'boolean',
          description: 'Include the full hierarchy in the response',
          default: false,
        },
      },
      required: [],
    },
    handler: async (args) => {
      const includeHierarchy = args.includeHierarchy === true;
      
      return await bridge.send('getSceneInfo', { includeHierarchy });
    },
  });

  tools.set('unity_scene_list', {
    description: 'List all scenes in the project.',
    inputSchema: {
      type: 'object',
      properties: {
        includeDisabled: {
          type: 'boolean',
          description: 'Include scenes not in build settings',
          default: true,
        },
      },
      required: [],
    },
    handler: async (args) => {
      const includeDisabled = args.includeDisabled !== false;
      
      return await bridge.send('listScenes', { includeDisabled });
    },
  });

  tools.set('unity_hierarchy_view', {
    description: 'View the scene hierarchy. Can optionally filter by name or type.',
    inputSchema: {
      type: 'object',
      properties: {
        filter: {
          type: 'string',
          description: 'Filter GameObjects by name (supports wildcards)',
        },
        depth: {
          type: 'number',
          description: 'Maximum hierarchy depth to return (-1 for unlimited)',
          default: -1,
        },
        includeComponents: {
          type: 'boolean',
          description: 'Include component information for each GameObject',
          default: false,
        },
      },
      required: [],
    },
    handler: async (args) => {
      const filter = args.filter as string | undefined;
      const depth = (args.depth as number) ?? -1;
      const includeComponents = args.includeComponents === true;
      
      return await bridge.send('getSceneHierarchy', {
        filter,
        depth,
        includeComponents,
      });
    },
  });

  tools.set('unity_play_mode', {
    description: 'Control Unity Play Mode - enter, exit, or pause.',
    inputSchema: {
      type: 'object',
      properties: {
        action: {
          type: 'string',
          description: 'Action to perform',
          enum: ['play', 'pause', 'stop', 'step', 'status'],
        },
      },
      required: ['action'],
    },
    handler: async (args) => {
      const action = args.action as string;
      
      return await bridge.send('playMode', { action });
    },
  });
}
