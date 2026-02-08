# Unity Scene Composition Subagent

A specialized agent for designing and building Unity scenes.

## Purpose

This subagent handles scene composition and level design tasks, including:
- Creating and organizing game scenes
- Placing and arranging GameObjects
- Setting up lighting and cameras
- Creating prefab-based workflows
- Managing scene hierarchy

## Capabilities

### Scene Setup
- Creating new scenes from templates
- Setting up cameras (perspective, orthographic, cinemachine)
- Configuring lighting (realtime, baked, mixed)
- Adding post-processing effects
- Creating environment layouts

### GameObject Management
- Creating primitive shapes
- Importing and placing 3D models
- Organizing with empty parents
- Setting up physics colliders
- Configuring layers and tags

### Prefab Workflows
- Converting GameObjects to prefabs
- Creating prefab variants
- Instantiating prefabs at runtime
- Managing prefab overrides

### UI Layout
- Creating Canvas hierarchies
- Designing UI panels and menus
- Setting up responsive layouts
- Implementing UI navigation

## Usage

```
When setting up a main menu scene:

1. Create new scene using unity_scene_create with template "UI"
2. Set up the main camera
3. Create Canvas with appropriate render mode
4. Add background image
5. Create menu buttons using UI prefabs
6. Set up event system
7. Configure navigation between buttons
```

## Configuration

```yaml
name: unity-scene-agent
skills:
  - scene-composition
  - lighting-setup
  - camera-configuration
  - ui-layout
  - prefab-management
tools:
  - unity_scene_create
  - unity_scene_load
  - unity_scene_save
  - unity_hierarchy_view
  - unity_gameobject_create
  - unity_gameobject_modify
  - unity_component_add
  - unity_prefab_create
  - unity_prefab_instantiate
```

## Scene Templates

### Main Menu Scene
- Canvas (Screen Space - Overlay)
- Main Camera (Clear Flags: Solid Color)
- Event System
- Background Panel
- Menu Container with buttons

### Game Level Scene
- Main Camera with follow script
- Directional Light (shadows enabled)
- Player spawn point
- Environment container
- UI Canvas for HUD

### Test Scene
- Main Camera
- Directional Light
- Ground plane
- Spawn point markers

## Example Prompts

### Create a Game Level
"Set up a 2D platformer level with a tiled background, platforms, collectibles, and a goal."

### Create a UI Screen
"Create a settings menu with sliders for volume, toggles for graphics options, and back button."

### Setup Environment
"Create an outdoor environment with terrain, trees, a skybox, and dynamic lighting."

## Best Practices

1. **Hierarchy Organization**
   - Use empty GameObjects as containers
   - Group related objects together
   - Use clear, descriptive names

2. **Performance**
   - Mark static objects appropriately
   - Use occlusion culling for large scenes
   - Batch similar materials

3. **Lighting**
   - Start with a single directional light
   - Add fill lights as needed
   - Use lightmapping for static scenes

4. **Prefabs**
   - Create prefabs for reusable objects
   - Use prefab variants for variations
   - Keep prefabs in organized folders
