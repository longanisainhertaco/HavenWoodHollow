/**
 * GameObject Tools - Unity GameObject Management
 * 
 * Tools for creating, modifying, and managing GameObjects in Unity scenes.
 */

import { UnityBridge } from '../utils/unity-bridge.js';

type ToolHandler = (args: Record<string, unknown>) => Promise<unknown>;
type ToolDefinition = {
  description: string;
  inputSchema: object;
  handler: ToolHandler;
};
type ToolMap = Map<string, ToolDefinition>;

export function registerGameObjectTools(tools: ToolMap, bridge: UnityBridge): void {
  
  tools.set('unity_gameobject_create', {
    description: 'Create a new GameObject in the scene with optional components and transform settings.',
    inputSchema: {
      type: 'object',
      properties: {
        name: {
          type: 'string',
          description: 'Name for the new GameObject',
        },
        primitiveType: {
          type: 'string',
          description: 'Create from a primitive type',
          enum: ['cube', 'sphere', 'capsule', 'cylinder', 'plane', 'quad', 'empty'],
          default: 'empty',
        },
        parent: {
          type: 'string',
          description: 'Name or path of parent GameObject',
        },
        position: {
          type: 'object',
          description: 'World position (x, y, z)',
          properties: {
            x: { type: 'number' },
            y: { type: 'number' },
            z: { type: 'number' },
          },
        },
        rotation: {
          type: 'object',
          description: 'Euler rotation (x, y, z)',
          properties: {
            x: { type: 'number' },
            y: { type: 'number' },
            z: { type: 'number' },
          },
        },
        scale: {
          type: 'object',
          description: 'Local scale (x, y, z)',
          properties: {
            x: { type: 'number' },
            y: { type: 'number' },
            z: { type: 'number' },
          },
        },
        layer: {
          type: 'string',
          description: 'Layer name to assign',
        },
        tag: {
          type: 'string',
          description: 'Tag to assign',
        },
        isStatic: {
          type: 'boolean',
          description: 'Mark as static for optimization',
        },
      },
      required: ['name'],
    },
    handler: async (args) => {
      return await bridge.send('createGameObject', args);
    },
  });

  tools.set('unity_gameobject_find', {
    description: 'Find GameObjects in the scene by name, tag, or type.',
    inputSchema: {
      type: 'object',
      properties: {
        name: {
          type: 'string',
          description: 'Find by name (supports wildcards)',
        },
        tag: {
          type: 'string',
          description: 'Find by tag',
        },
        componentType: {
          type: 'string',
          description: 'Find by component type (e.g., "Rigidbody", "Camera")',
        },
        includeInactive: {
          type: 'boolean',
          description: 'Include inactive GameObjects',
          default: false,
        },
      },
      required: [],
    },
    handler: async (args) => {
      return await bridge.send('findGameObjects', args);
    },
  });

  tools.set('unity_gameobject_modify', {
    description: 'Modify properties of an existing GameObject.',
    inputSchema: {
      type: 'object',
      properties: {
        target: {
          type: 'string',
          description: 'Name or path of the target GameObject',
        },
        name: {
          type: 'string',
          description: 'New name for the GameObject',
        },
        active: {
          type: 'boolean',
          description: 'Set active state',
        },
        parent: {
          type: 'string',
          description: 'New parent (name or path, null to unparent)',
        },
        position: {
          type: 'object',
          description: 'New world position',
          properties: {
            x: { type: 'number' },
            y: { type: 'number' },
            z: { type: 'number' },
          },
        },
        localPosition: {
          type: 'object',
          description: 'New local position',
          properties: {
            x: { type: 'number' },
            y: { type: 'number' },
            z: { type: 'number' },
          },
        },
        rotation: {
          type: 'object',
          description: 'New world rotation (Euler angles)',
          properties: {
            x: { type: 'number' },
            y: { type: 'number' },
            z: { type: 'number' },
          },
        },
        localRotation: {
          type: 'object',
          description: 'New local rotation (Euler angles)',
          properties: {
            x: { type: 'number' },
            y: { type: 'number' },
            z: { type: 'number' },
          },
        },
        scale: {
          type: 'object',
          description: 'New local scale',
          properties: {
            x: { type: 'number' },
            y: { type: 'number' },
            z: { type: 'number' },
          },
        },
        layer: {
          type: 'string',
          description: 'New layer name',
        },
        tag: {
          type: 'string',
          description: 'New tag',
        },
        isStatic: {
          type: 'boolean',
          description: 'Set static flag',
        },
      },
      required: ['target'],
    },
    handler: async (args) => {
      return await bridge.send('modifyGameObject', args);
    },
  });

  tools.set('unity_gameobject_delete', {
    description: 'Delete a GameObject from the scene.',
    inputSchema: {
      type: 'object',
      properties: {
        target: {
          type: 'string',
          description: 'Name or path of the GameObject to delete',
        },
        immediate: {
          type: 'boolean',
          description: 'Use immediate destruction (not recommended during gameplay)',
          default: false,
        },
      },
      required: ['target'],
    },
    handler: async (args) => {
      return await bridge.send('deleteGameObject', args);
    },
  });

  tools.set('unity_gameobject_duplicate', {
    description: 'Duplicate a GameObject in the scene.',
    inputSchema: {
      type: 'object',
      properties: {
        target: {
          type: 'string',
          description: 'Name or path of the GameObject to duplicate',
        },
        newName: {
          type: 'string',
          description: 'Name for the duplicate (optional)',
        },
        parent: {
          type: 'string',
          description: 'Parent for the duplicate (optional)',
        },
      },
      required: ['target'],
    },
    handler: async (args) => {
      return await bridge.send('duplicateGameObject', args);
    },
  });

  tools.set('unity_component_add', {
    description: 'Add a component to a GameObject.',
    inputSchema: {
      type: 'object',
      properties: {
        target: {
          type: 'string',
          description: 'Name or path of the target GameObject',
        },
        componentType: {
          type: 'string',
          description: 'Full type name of the component (e.g., "Rigidbody", "BoxCollider", "MyNamespace.MyComponent")',
        },
        properties: {
          type: 'object',
          description: 'Initial property values to set on the component',
        },
      },
      required: ['target', 'componentType'],
    },
    handler: async (args) => {
      return await bridge.send('addComponent', args);
    },
  });

  tools.set('unity_component_modify', {
    description: 'Modify properties of a component on a GameObject.',
    inputSchema: {
      type: 'object',
      properties: {
        target: {
          type: 'string',
          description: 'Name or path of the target GameObject',
        },
        componentType: {
          type: 'string',
          description: 'Type name of the component to modify',
        },
        componentIndex: {
          type: 'number',
          description: 'Index if multiple components of the same type exist (0-based)',
          default: 0,
        },
        properties: {
          type: 'object',
          description: 'Property values to set on the component',
        },
      },
      required: ['target', 'componentType', 'properties'],
    },
    handler: async (args) => {
      return await bridge.send('modifyComponent', args);
    },
  });

  tools.set('unity_component_remove', {
    description: 'Remove a component from a GameObject.',
    inputSchema: {
      type: 'object',
      properties: {
        target: {
          type: 'string',
          description: 'Name or path of the target GameObject',
        },
        componentType: {
          type: 'string',
          description: 'Type name of the component to remove',
        },
        componentIndex: {
          type: 'number',
          description: 'Index if multiple components of the same type exist (0-based)',
          default: 0,
        },
      },
      required: ['target', 'componentType'],
    },
    handler: async (args) => {
      return await bridge.send('removeComponent', args);
    },
  });

  tools.set('unity_component_get', {
    description: 'Get information about components on a GameObject.',
    inputSchema: {
      type: 'object',
      properties: {
        target: {
          type: 'string',
          description: 'Name or path of the target GameObject',
        },
        componentType: {
          type: 'string',
          description: 'Specific component type to get (optional, returns all if not specified)',
        },
        includeProperties: {
          type: 'boolean',
          description: 'Include serialized property values',
          default: true,
        },
      },
      required: ['target'],
    },
    handler: async (args) => {
      return await bridge.send('getComponents', args);
    },
  });
}
