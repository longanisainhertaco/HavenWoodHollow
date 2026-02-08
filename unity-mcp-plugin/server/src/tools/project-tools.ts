/**
 * Project Tools - Unity Project Management
 * 
 * Tools for project-level operations and information.
 */

import { UnityBridge } from '../utils/unity-bridge.js';

type ToolHandler = (args: Record<string, unknown>) => Promise<unknown>;
type ToolDefinition = {
  description: string;
  inputSchema: object;
  handler: ToolHandler;
};
type ToolMap = Map<string, ToolDefinition>;

export function registerProjectTools(tools: ToolMap, bridge: UnityBridge): void {
  
  tools.set('unity_project_info', {
    description: 'Get comprehensive information about the Unity project.',
    inputSchema: {
      type: 'object',
      properties: {
        includePackages: {
          type: 'boolean',
          description: 'Include installed packages list',
          default: true,
        },
        includeSettings: {
          type: 'boolean',
          description: 'Include project settings summary',
          default: false,
        },
      },
      required: [],
    },
    handler: async (args) => {
      return await bridge.send('getProjectInfo', args);
    },
  });

  tools.set('unity_packages_list', {
    description: 'List all installed packages in the project.',
    inputSchema: {
      type: 'object',
      properties: {
        includeBuiltIn: {
          type: 'boolean',
          description: 'Include built-in Unity packages',
          default: false,
        },
      },
      required: [],
    },
    handler: async (args) => {
      return await bridge.send('listPackages', args);
    },
  });

  tools.set('unity_package_add', {
    description: 'Add a package to the project.',
    inputSchema: {
      type: 'object',
      properties: {
        packageId: {
          type: 'string',
          description: 'Package identifier (e.g., "com.unity.textmeshpro")',
        },
        version: {
          type: 'string',
          description: 'Specific version (optional, uses latest if not specified)',
        },
        source: {
          type: 'string',
          description: 'Package source',
          enum: ['registry', 'git', 'disk', 'tarball'],
          default: 'registry',
        },
        url: {
          type: 'string',
          description: 'URL for git/tarball sources',
        },
      },
      required: ['packageId'],
    },
    handler: async (args) => {
      return await bridge.send('addPackage', args);
    },
  });

  tools.set('unity_package_remove', {
    description: 'Remove a package from the project.',
    inputSchema: {
      type: 'object',
      properties: {
        packageId: {
          type: 'string',
          description: 'Package identifier to remove',
        },
      },
      required: ['packageId'],
    },
    handler: async (args) => {
      return await bridge.send('removePackage', args);
    },
  });

  tools.set('unity_tags_layers', {
    description: 'Manage project tags and layers.',
    inputSchema: {
      type: 'object',
      properties: {
        action: {
          type: 'string',
          description: 'Action to perform',
          enum: ['getTags', 'getLayers', 'addTag', 'addLayer', 'removeTag'],
        },
        name: {
          type: 'string',
          description: 'Tag or layer name',
        },
        layerIndex: {
          type: 'number',
          description: 'Layer index (8-31 for user layers)',
        },
      },
      required: ['action'],
    },
    handler: async (args) => {
      return await bridge.send('tagsAndLayers', args);
    },
  });

  tools.set('unity_input_settings', {
    description: 'Get or modify input settings (legacy input manager or new input system).',
    inputSchema: {
      type: 'object',
      properties: {
        action: {
          type: 'string',
          description: 'Action to perform',
          enum: ['get', 'set', 'addAxis', 'removeAxis'],
        },
        settings: {
          type: 'object',
          description: 'Settings to apply',
        },
        axisName: {
          type: 'string',
          description: 'Axis name for add/remove operations',
        },
        axisConfig: {
          type: 'object',
          description: 'Axis configuration',
        },
      },
      required: ['action'],
    },
    handler: async (args) => {
      return await bridge.send('inputSettings', args);
    },
  });

  tools.set('unity_physics_settings', {
    description: 'Get or modify physics settings (3D or 2D).',
    inputSchema: {
      type: 'object',
      properties: {
        physics2D: {
          type: 'boolean',
          description: 'Use 2D physics settings',
          default: false,
        },
        action: {
          type: 'string',
          description: 'Action to perform',
          enum: ['get', 'set'],
        },
        settings: {
          type: 'object',
          description: 'Settings to apply',
          properties: {
            gravity: { type: 'object', properties: { x: { type: 'number' }, y: { type: 'number' }, z: { type: 'number' } } },
            defaultSolverIterations: { type: 'number' },
            defaultSolverVelocityIterations: { type: 'number' },
            bounceThreshold: { type: 'number' },
          },
        },
      },
      required: ['action'],
    },
    handler: async (args) => {
      return await bridge.send('physicsSettings', args);
    },
  });

  tools.set('unity_time_settings', {
    description: 'Get or modify time settings.',
    inputSchema: {
      type: 'object',
      properties: {
        action: {
          type: 'string',
          description: 'Action to perform',
          enum: ['get', 'set'],
        },
        settings: {
          type: 'object',
          description: 'Settings to apply',
          properties: {
            fixedDeltaTime: { type: 'number' },
            maximumDeltaTime: { type: 'number' },
            timeScale: { type: 'number' },
            maximumParticleDeltaTime: { type: 'number' },
          },
        },
      },
      required: ['action'],
    },
    handler: async (args) => {
      return await bridge.send('timeSettings', args);
    },
  });

  tools.set('unity_editor_preferences', {
    description: 'Get or set Unity Editor preferences.',
    inputSchema: {
      type: 'object',
      properties: {
        action: {
          type: 'string',
          description: 'Action to perform',
          enum: ['get', 'set', 'delete'],
        },
        key: {
          type: 'string',
          description: 'Preference key',
        },
        value: {
          type: 'string',
          description: 'Value to set',
        },
        type: {
          type: 'string',
          description: 'Value type',
          enum: ['string', 'int', 'float', 'bool'],
          default: 'string',
        },
      },
      required: ['action'],
    },
    handler: async (args) => {
      return await bridge.send('editorPreferences', args);
    },
  });

  tools.set('unity_undo', {
    description: 'Manage undo/redo operations.',
    inputSchema: {
      type: 'object',
      properties: {
        action: {
          type: 'string',
          description: 'Action to perform',
          enum: ['undo', 'redo', 'clear', 'getHistory'],
        },
        count: {
          type: 'number',
          description: 'Number of history items to return',
          default: 10,
        },
      },
      required: ['action'],
    },
    handler: async (args) => {
      return await bridge.send('undoOperations', args);
    },
  });

  tools.set('unity_selection', {
    description: 'Manage editor selection.',
    inputSchema: {
      type: 'object',
      properties: {
        action: {
          type: 'string',
          description: 'Action to perform',
          enum: ['get', 'set', 'add', 'clear'],
        },
        objects: {
          type: 'array',
          description: 'Object names or paths to select',
          items: { type: 'string' },
        },
      },
      required: ['action'],
    },
    handler: async (args) => {
      return await bridge.send('selection', args);
    },
  });
}
