/**
 * Asset Tools - Unity Asset Management
 * 
 * Tools for importing, organizing, and managing assets in Unity projects.
 */

import { UnityBridge } from '../utils/unity-bridge.js';

type ToolHandler = (args: Record<string, unknown>) => Promise<unknown>;
type ToolDefinition = {
  description: string;
  inputSchema: object;
  handler: ToolHandler;
};
type ToolMap = Map<string, ToolDefinition>;

export function registerAssetTools(tools: ToolMap, bridge: UnityBridge): void {
  
  tools.set('unity_asset_import', {
    description: 'Import assets into the Unity project from external files.',
    inputSchema: {
      type: 'object',
      properties: {
        sourcePath: {
          type: 'string',
          description: 'Path to the file to import',
        },
        destinationPath: {
          type: 'string',
          description: 'Destination path in Assets folder',
          default: 'Assets',
        },
        importSettings: {
          type: 'object',
          description: 'Asset-specific import settings',
        },
      },
      required: ['sourcePath'],
    },
    handler: async (args) => {
      return await bridge.send('importAsset', args);
    },
  });

  tools.set('unity_asset_search', {
    description: 'Search for assets in the Unity project.',
    inputSchema: {
      type: 'object',
      properties: {
        query: {
          type: 'string',
          description: 'Search query (asset name or type)',
        },
        type: {
          type: 'string',
          description: 'Filter by asset type (e.g., "Texture2D", "AudioClip", "Prefab", "Material")',
        },
        path: {
          type: 'string',
          description: 'Limit search to specific folder',
        },
        labels: {
          type: 'array',
          description: 'Filter by asset labels',
          items: { type: 'string' },
        },
      },
      required: [],
    },
    handler: async (args) => {
      return await bridge.send('searchAssets', args);
    },
  });

  tools.set('unity_asset_list', {
    description: 'List assets in a specific folder.',
    inputSchema: {
      type: 'object',
      properties: {
        path: {
          type: 'string',
          description: 'Folder path (default: Assets)',
          default: 'Assets',
        },
        recursive: {
          type: 'boolean',
          description: 'Include subdirectories',
          default: false,
        },
        type: {
          type: 'string',
          description: 'Filter by asset type',
        },
      },
      required: [],
    },
    handler: async (args) => {
      return await bridge.send('listAssets', args);
    },
  });

  tools.set('unity_asset_info', {
    description: 'Get detailed information about an asset.',
    inputSchema: {
      type: 'object',
      properties: {
        path: {
          type: 'string',
          description: 'Path to the asset relative to Assets folder',
        },
        includeDependencies: {
          type: 'boolean',
          description: 'Include dependency information',
          default: false,
        },
        includeReferences: {
          type: 'boolean',
          description: 'Include references (what uses this asset)',
          default: false,
        },
      },
      required: ['path'],
    },
    handler: async (args) => {
      return await bridge.send('getAssetInfo', args);
    },
  });

  tools.set('unity_asset_move', {
    description: 'Move or rename an asset.',
    inputSchema: {
      type: 'object',
      properties: {
        sourcePath: {
          type: 'string',
          description: 'Current asset path',
        },
        destinationPath: {
          type: 'string',
          description: 'New asset path',
        },
      },
      required: ['sourcePath', 'destinationPath'],
    },
    handler: async (args) => {
      return await bridge.send('moveAsset', args);
    },
  });

  tools.set('unity_asset_delete', {
    description: 'Delete an asset from the project.',
    inputSchema: {
      type: 'object',
      properties: {
        path: {
          type: 'string',
          description: 'Path to the asset to delete',
        },
        deleteEmptyFolders: {
          type: 'boolean',
          description: 'Delete empty parent folders',
          default: false,
        },
      },
      required: ['path'],
    },
    handler: async (args) => {
      return await bridge.send('deleteAsset', args);
    },
  });

  tools.set('unity_asset_duplicate', {
    description: 'Duplicate an asset.',
    inputSchema: {
      type: 'object',
      properties: {
        path: {
          type: 'string',
          description: 'Path to the asset to duplicate',
        },
        newPath: {
          type: 'string',
          description: 'Path for the duplicate (optional)',
        },
      },
      required: ['path'],
    },
    handler: async (args) => {
      return await bridge.send('duplicateAsset', args);
    },
  });

  tools.set('unity_prefab_create', {
    description: 'Create a prefab from a GameObject in the scene.',
    inputSchema: {
      type: 'object',
      properties: {
        target: {
          type: 'string',
          description: 'Name or path of the GameObject to convert to prefab',
        },
        path: {
          type: 'string',
          description: 'Path to save the prefab (default: Assets/Prefabs)',
          default: 'Assets/Prefabs',
        },
        keepOriginal: {
          type: 'boolean',
          description: 'Keep the original GameObject (as prefab instance)',
          default: true,
        },
      },
      required: ['target'],
    },
    handler: async (args) => {
      return await bridge.send('createPrefab', args);
    },
  });

  tools.set('unity_prefab_instantiate', {
    description: 'Instantiate a prefab in the scene.',
    inputSchema: {
      type: 'object',
      properties: {
        path: {
          type: 'string',
          description: 'Path to the prefab asset',
        },
        name: {
          type: 'string',
          description: 'Name for the instance (optional)',
        },
        parent: {
          type: 'string',
          description: 'Parent GameObject for the instance',
        },
        position: {
          type: 'object',
          description: 'World position',
          properties: {
            x: { type: 'number' },
            y: { type: 'number' },
            z: { type: 'number' },
          },
        },
        rotation: {
          type: 'object',
          description: 'Euler rotation',
          properties: {
            x: { type: 'number' },
            y: { type: 'number' },
            z: { type: 'number' },
          },
        },
      },
      required: ['path'],
    },
    handler: async (args) => {
      return await bridge.send('instantiatePrefab', args);
    },
  });

  tools.set('unity_prefab_unpack', {
    description: 'Unpack a prefab instance in the scene.',
    inputSchema: {
      type: 'object',
      properties: {
        target: {
          type: 'string',
          description: 'Name or path of the prefab instance',
        },
        unpackMode: {
          type: 'string',
          description: 'How to unpack',
          enum: ['root', 'completely'],
          default: 'root',
        },
      },
      required: ['target'],
    },
    handler: async (args) => {
      return await bridge.send('unpackPrefab', args);
    },
  });

  tools.set('unity_material_create', {
    description: 'Create a new material asset.',
    inputSchema: {
      type: 'object',
      properties: {
        name: {
          type: 'string',
          description: 'Name for the material',
        },
        path: {
          type: 'string',
          description: 'Path to save the material (default: Assets/Materials)',
          default: 'Assets/Materials',
        },
        shader: {
          type: 'string',
          description: 'Shader to use (default: Standard)',
          default: 'Standard',
        },
        properties: {
          type: 'object',
          description: 'Shader properties to set (e.g., _Color, _MainTex)',
        },
      },
      required: ['name'],
    },
    handler: async (args) => {
      return await bridge.send('createMaterial', args);
    },
  });

  tools.set('unity_folder_create', {
    description: 'Create a new folder in the Assets directory.',
    inputSchema: {
      type: 'object',
      properties: {
        path: {
          type: 'string',
          description: 'Path for the new folder (relative to project root)',
        },
      },
      required: ['path'],
    },
    handler: async (args) => {
      return await bridge.send('createFolder', args);
    },
  });

  tools.set('unity_asset_refresh', {
    description: 'Refresh the Unity Asset Database to detect external changes.',
    inputSchema: {
      type: 'object',
      properties: {
        importOptions: {
          type: 'string',
          description: 'Import behavior',
          enum: ['default', 'forceUpdate', 'downloadFromCacheServer'],
          default: 'default',
        },
      },
      required: [],
    },
    handler: async (args) => {
      return await bridge.send('refreshAssetDatabase', args);
    },
  });
}
