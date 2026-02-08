# Unity MCP Skills

Skills are reusable, atomic capabilities that can be composed into larger workflows.
Each skill focuses on a single, well-defined task.

## Available Skills

### Scene Management
- `scene.create` - Create a new scene
- `scene.load` - Load an existing scene
- `scene.save` - Save the current scene
- `scene.setup-lighting` - Configure scene lighting

### GameObject Operations
- `gameobject.create` - Create GameObjects
- `gameobject.find` - Find GameObjects by criteria
- `gameobject.transform` - Modify transforms
- `gameobject.parent` - Set parent relationships

### Script Generation
- `script.monobehaviour` - Create MonoBehaviour scripts
- `script.scriptableobject` - Create ScriptableObject scripts
- `script.editor` - Create Editor scripts
- `script.interface` - Create interface definitions

### Component Management
- `component.physics` - Add physics components
- `component.collider` - Configure colliders
- `component.renderer` - Set up renderers
- `component.audio` - Add audio sources

### Asset Operations
- `asset.import` - Import external assets
- `asset.material` - Create materials
- `asset.prefab` - Create/instantiate prefabs
- `asset.organize` - Organize asset folders

### Build & Deploy
- `build.configure` - Configure build settings
- `build.execute` - Build for platform
- `build.test` - Run build tests

## Skill Definition Format

```yaml
skill:
  name: skill-name
  category: category
  description: What this skill does
  
inputs:
  - name: inputName
    type: string|number|boolean|array
    required: true/false
    default: defaultValue
    
outputs:
  - name: outputName
    type: expected type
    
implementation:
  # Tool calls to implement the skill
  steps:
    - tool: tool_name
      params:
        param: "{{input.inputName}}"
```

## Using Skills

Skills can be invoked directly or composed in workflows:

```yaml
# Direct invocation
skill: script.monobehaviour
inputs:
  name: PlayerController
  namespace: Game.Player

# In a workflow
steps:
  - skill: gameobject.create
    inputs:
      name: Player
      type: capsule
```
