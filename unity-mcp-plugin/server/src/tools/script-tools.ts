/**
 * Script Tools - Unity C# Script Management
 * 
 * Tools for creating, editing, and managing C# scripts in Unity projects.
 */

import { UnityBridge } from '../utils/unity-bridge.js';

type ToolHandler = (args: Record<string, unknown>) => Promise<unknown>;
type ToolDefinition = {
  description: string;
  inputSchema: object;
  handler: ToolHandler;
};
type ToolMap = Map<string, ToolDefinition>;

export function registerScriptTools(tools: ToolMap, bridge: UnityBridge): void {
  
  tools.set('unity_script_create', {
    description: 'Create a new C# script file with the specified template.',
    inputSchema: {
      type: 'object',
      properties: {
        name: {
          type: 'string',
          description: 'Name of the script (without .cs extension)',
        },
        path: {
          type: 'string',
          description: 'Folder path relative to Assets (default: "Assets/Scripts")',
          default: 'Assets/Scripts',
        },
        template: {
          type: 'string',
          description: 'Script template type',
          enum: ['MonoBehaviour', 'ScriptableObject', 'EditorWindow', 'Editor', 'PropertyDrawer', 'StateMachineBehaviour', 'Empty'],
          default: 'MonoBehaviour',
        },
        namespace: {
          type: 'string',
          description: 'Namespace to wrap the class in (optional)',
        },
        baseClass: {
          type: 'string',
          description: 'Custom base class (overrides template)',
        },
        interfaces: {
          type: 'array',
          description: 'Interfaces to implement',
          items: { type: 'string' },
        },
        usings: {
          type: 'array',
          description: 'Additional using statements',
          items: { type: 'string' },
        },
        content: {
          type: 'string',
          description: 'Full script content (overrides template)',
        },
      },
      required: ['name'],
    },
    handler: async (args) => {
      return await bridge.send('createScript', args);
    },
  });

  tools.set('unity_script_read', {
    description: 'Read the contents of a C# script file.',
    inputSchema: {
      type: 'object',
      properties: {
        path: {
          type: 'string',
          description: 'Path to the script relative to Assets folder or script name',
        },
      },
      required: ['path'],
    },
    handler: async (args) => {
      return await bridge.send('readScript', args);
    },
  });

  tools.set('unity_script_edit', {
    description: 'Edit an existing C# script file. Can replace content or make targeted changes.',
    inputSchema: {
      type: 'object',
      properties: {
        path: {
          type: 'string',
          description: 'Path to the script relative to Assets folder',
        },
        content: {
          type: 'string',
          description: 'New full content for the script (replaces everything)',
        },
        insertAt: {
          type: 'object',
          description: 'Insert code at a specific location',
          properties: {
            line: { type: 'number', description: 'Line number (1-based)' },
            code: { type: 'string', description: 'Code to insert' },
          },
        },
        replace: {
          type: 'object',
          description: 'Replace specific text',
          properties: {
            find: { type: 'string', description: 'Text to find' },
            replaceWith: { type: 'string', description: 'Replacement text' },
            all: { type: 'boolean', description: 'Replace all occurrences', default: false },
          },
        },
        addMethod: {
          type: 'object',
          description: 'Add a new method to the class',
          properties: {
            name: { type: 'string' },
            returnType: { type: 'string', default: 'void' },
            parameters: { type: 'string', description: 'Parameter list' },
            body: { type: 'string' },
            modifier: { type: 'string', enum: ['public', 'private', 'protected', 'internal'], default: 'private' },
            attributes: { type: 'array', items: { type: 'string' }, description: 'Method attributes like [SerializeField]' },
          },
        },
        addField: {
          type: 'object',
          description: 'Add a new field to the class',
          properties: {
            name: { type: 'string' },
            type: { type: 'string' },
            defaultValue: { type: 'string' },
            modifier: { type: 'string', enum: ['public', 'private', 'protected', 'internal'], default: 'private' },
            attributes: { type: 'array', items: { type: 'string' }, description: 'Field attributes like [SerializeField]' },
          },
        },
        addProperty: {
          type: 'object',
          description: 'Add a new property to the class',
          properties: {
            name: { type: 'string' },
            type: { type: 'string' },
            modifier: { type: 'string', enum: ['public', 'private', 'protected', 'internal'], default: 'public' },
            hasGet: { type: 'boolean', default: true },
            hasSet: { type: 'boolean', default: true },
            backingField: { type: 'string', description: 'Name of backing field' },
          },
        },
      },
      required: ['path'],
    },
    handler: async (args) => {
      return await bridge.send('editScript', args);
    },
  });

  tools.set('unity_script_delete', {
    description: 'Delete a C# script file from the project.',
    inputSchema: {
      type: 'object',
      properties: {
        path: {
          type: 'string',
          description: 'Path to the script relative to Assets folder',
        },
        deleteEmptyFolders: {
          type: 'boolean',
          description: 'Delete empty parent folders after deletion',
          default: false,
        },
      },
      required: ['path'],
    },
    handler: async (args) => {
      return await bridge.send('deleteScript', args);
    },
  });

  tools.set('unity_script_search', {
    description: 'Search for scripts in the project by name or content.',
    inputSchema: {
      type: 'object',
      properties: {
        query: {
          type: 'string',
          description: 'Search query (script name or content)',
        },
        searchContent: {
          type: 'boolean',
          description: 'Search within script contents',
          default: false,
        },
        regex: {
          type: 'boolean',
          description: 'Use regex for content search',
          default: false,
        },
        path: {
          type: 'string',
          description: 'Limit search to specific folder',
        },
      },
      required: ['query'],
    },
    handler: async (args) => {
      return await bridge.send('searchScripts', args);
    },
  });

  tools.set('unity_script_analyze', {
    description: 'Analyze a script for structure, dependencies, and potential issues.',
    inputSchema: {
      type: 'object',
      properties: {
        path: {
          type: 'string',
          description: 'Path to the script relative to Assets folder',
        },
        includeReferences: {
          type: 'boolean',
          description: 'Include references to/from other scripts',
          default: true,
        },
      },
      required: ['path'],
    },
    handler: async (args) => {
      return await bridge.send('analyzeScript', args);
    },
  });

  tools.set('unity_execute_code', {
    description: 'Execute C# code in the Unity Editor context. Useful for quick operations or testing.',
    inputSchema: {
      type: 'object',
      properties: {
        code: {
          type: 'string',
          description: 'C# code to execute (will be wrapped in a method)',
        },
        returnValue: {
          type: 'boolean',
          description: 'Whether the code returns a value',
          default: false,
        },
        usings: {
          type: 'array',
          description: 'Additional using statements needed',
          items: { type: 'string' },
        },
      },
      required: ['code'],
    },
    handler: async (args) => {
      return await bridge.send('executeCode', args);
    },
  });

  tools.set('unity_console_read', {
    description: 'Read Unity console logs.',
    inputSchema: {
      type: 'object',
      properties: {
        count: {
          type: 'number',
          description: 'Maximum number of log entries to return',
          default: 50,
        },
        filter: {
          type: 'string',
          description: 'Filter by log type',
          enum: ['all', 'log', 'warning', 'error', 'exception'],
          default: 'all',
        },
        search: {
          type: 'string',
          description: 'Search text within log messages',
        },
        clear: {
          type: 'boolean',
          description: 'Clear the console after reading',
          default: false,
        },
      },
      required: [],
    },
    handler: async (args) => {
      return await bridge.send('getConsoleLogs', args);
    },
  });

  tools.set('unity_console_clear', {
    description: 'Clear the Unity console.',
    inputSchema: {
      type: 'object',
      properties: {},
      required: [],
    },
    handler: async () => {
      return await bridge.send('clearConsole', {});
    },
  });
}
