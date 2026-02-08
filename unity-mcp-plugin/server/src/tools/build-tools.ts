/**
 * Build Tools - Unity Build Pipeline
 * 
 * Tools for building Unity projects to various platforms.
 */

import { UnityBridge } from '../utils/unity-bridge.js';

type ToolHandler = (args: Record<string, unknown>) => Promise<unknown>;
type ToolDefinition = {
  description: string;
  inputSchema: object;
  handler: ToolHandler;
};
type ToolMap = Map<string, ToolDefinition>;

export function registerBuildTools(tools: ToolMap, bridge: UnityBridge): void {
  
  tools.set('unity_build', {
    description: 'Build the Unity project for a target platform.',
    inputSchema: {
      type: 'object',
      properties: {
        target: {
          type: 'string',
          description: 'Build target platform',
          enum: [
            'StandaloneWindows',
            'StandaloneWindows64',
            'StandaloneOSX',
            'StandaloneLinux64',
            'iOS',
            'Android',
            'WebGL',
            'PS4',
            'PS5',
            'XboxOne',
            'Switch',
          ],
        },
        outputPath: {
          type: 'string',
          description: 'Output path for the build',
        },
        scenes: {
          type: 'array',
          description: 'Specific scenes to include (uses build settings if not specified)',
          items: { type: 'string' },
        },
        options: {
          type: 'object',
          description: 'Build options',
          properties: {
            development: { type: 'boolean', description: 'Development build with debugging enabled' },
            allowDebugging: { type: 'boolean', description: 'Allow script debugging' },
            autoRunPlayer: { type: 'boolean', description: 'Run the built application after building' },
            compressWithLz4: { type: 'boolean', description: 'Compress build with LZ4' },
            strictMode: { type: 'boolean', description: 'Enable strict mode' },
          },
        },
      },
      required: ['target'],
    },
    handler: async (args) => {
      return await bridge.send('build', args);
    },
  });

  tools.set('unity_build_settings', {
    description: 'Get or modify build settings.',
    inputSchema: {
      type: 'object',
      properties: {
        action: {
          type: 'string',
          description: 'Action to perform',
          enum: ['get', 'set', 'addScene', 'removeScene', 'reorderScenes'],
        },
        settings: {
          type: 'object',
          description: 'Settings to apply (for set action)',
          properties: {
            scenes: { type: 'array', items: { type: 'string' } },
            target: { type: 'string' },
            targetGroup: { type: 'string' },
          },
        },
        scenePath: {
          type: 'string',
          description: 'Scene path (for addScene/removeScene)',
        },
        sceneOrder: {
          type: 'array',
          description: 'New scene order (for reorderScenes)',
          items: { type: 'string' },
        },
      },
      required: ['action'],
    },
    handler: async (args) => {
      return await bridge.send('buildSettings', args);
    },
  });

  tools.set('unity_player_settings', {
    description: 'Get or modify player settings.',
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
          description: 'Settings to apply (for set action)',
          properties: {
            companyName: { type: 'string' },
            productName: { type: 'string' },
            version: { type: 'string' },
            bundleIdentifier: { type: 'string' },
            defaultIcon: { type: 'string', description: 'Path to icon asset' },
            defaultCursor: { type: 'string', description: 'Path to cursor asset' },
            splashScreen: { type: 'object' },
            resolution: { type: 'object' },
            colorSpace: { type: 'string', enum: ['Gamma', 'Linear'] },
            graphicsAPI: { type: 'string' },
          },
        },
        platform: {
          type: 'string',
          description: 'Platform-specific settings',
        },
      },
      required: ['action'],
    },
    handler: async (args) => {
      return await bridge.send('playerSettings', args);
    },
  });

  tools.set('unity_quality_settings', {
    description: 'Get or modify quality settings.',
    inputSchema: {
      type: 'object',
      properties: {
        action: {
          type: 'string',
          description: 'Action to perform',
          enum: ['get', 'set', 'setLevel'],
        },
        level: {
          type: 'number',
          description: 'Quality level index (for setLevel)',
        },
        settings: {
          type: 'object',
          description: 'Settings to apply',
        },
      },
      required: ['action'],
    },
    handler: async (args) => {
      return await bridge.send('qualitySettings', args);
    },
  });

  tools.set('unity_switch_platform', {
    description: 'Switch the active build target platform.',
    inputSchema: {
      type: 'object',
      properties: {
        target: {
          type: 'string',
          description: 'Target platform',
          enum: [
            'StandaloneWindows',
            'StandaloneWindows64',
            'StandaloneOSX',
            'StandaloneLinux64',
            'iOS',
            'Android',
            'WebGL',
          ],
        },
      },
      required: ['target'],
    },
    handler: async (args) => {
      return await bridge.send('switchPlatform', args);
    },
  });

  tools.set('unity_addressables', {
    description: 'Manage Addressable Assets (if package is installed).',
    inputSchema: {
      type: 'object',
      properties: {
        action: {
          type: 'string',
          description: 'Action to perform',
          enum: ['getGroups', 'createGroup', 'addToGroup', 'removeFromGroup', 'build', 'getSettings'],
        },
        groupName: {
          type: 'string',
          description: 'Addressable group name',
        },
        assetPath: {
          type: 'string',
          description: 'Asset path',
        },
        address: {
          type: 'string',
          description: 'Addressable address for the asset',
        },
        labels: {
          type: 'array',
          description: 'Labels to assign',
          items: { type: 'string' },
        },
      },
      required: ['action'],
    },
    handler: async (args) => {
      return await bridge.send('addressables', args);
    },
  });
}
