# Usage Guide

Complete guide to using the Unity MCP Plugin with Claude.

## Getting Started

### Connecting to Unity

Always start by connecting to Unity:

```
User: Connect to Unity

Claude: I'll connect to Unity Editor now.
[Uses unity_connect tool]
Successfully connected to Unity Editor!
```

### Checking Status

```
User: What's the current Unity status?

Claude: [Uses unity_status]
Connected to Unity Editor
- Project: MyGame
- Unity Version: 2022.3.10f1
- Currently in Edit mode
```

## Common Tasks

### Scene Management

#### Create a New Scene
```
User: Create a new scene called "Level1"

Claude: [Uses unity_scene_create]
Created scene "Level1" at Assets/Scenes/Level1.unity
```

#### View Scene Hierarchy
```
User: Show me what's in the current scene

Claude: [Uses unity_hierarchy_view]
Scene Hierarchy:
- Main Camera
  - AudioListener
- Directional Light
- Player
  - PlayerController
  - Rigidbody
```

#### Save Scene
```
User: Save the current scene

Claude: [Uses unity_scene_save]
Scene saved successfully.
```

### Creating GameObjects

#### Basic Object
```
User: Create a cube called "Platform" at position (0, -1, 0)

Claude: [Uses unity_gameobject_create]
Created "Platform" (Cube) at position (0, -1, 0)
```

#### Object with Components
```
User: Create a sphere with a Rigidbody for physics

Claude: 
[Uses unity_gameobject_create] Created "Sphere"
[Uses unity_component_add] Added Rigidbody component
```

#### Modify Existing Object
```
User: Move the Platform to (0, -2, 0)

Claude: [Uses unity_gameobject_modify]
Modified "Platform" position to (0, -2, 0)
```

### Script Creation

#### Create a Script
```
User: Create a script called "PlayerController" with movement code

Claude: [Uses unity_script_create]
Created PlayerController.cs at Assets/Scripts/PlayerController.cs
Waiting for Unity to compile...
```

#### Attach Script to Object
```
User: Add the PlayerController script to the Player object

Claude: [Uses unity_component_add]
Added PlayerController component to "Player"
```

#### Read/Edit Scripts
```
User: Show me the PlayerController script

Claude: [Uses unity_script_read]
Here's the content of PlayerController.cs:
[displays script]
```

### Working with Prefabs

#### Create a Prefab
```
User: Save the Player object as a prefab

Claude: [Uses unity_prefab_create]
Created prefab at Assets/Prefabs/Player.prefab
```

#### Instantiate Prefab
```
User: Add another player prefab to the scene at (5, 0, 0)

Claude: [Uses unity_prefab_instantiate]
Instantiated "Player" at (5, 0, 0)
```

### Play Mode

#### Control Play Mode
```
User: Enter play mode

Claude: [Uses unity_play_mode action=play]
Unity is now in Play mode.
```

```
User: Stop the game

Claude: [Uses unity_play_mode action=stop]
Stopped Play mode.
```

### Console and Debugging

#### Read Console Logs
```
User: Are there any errors in the console?

Claude: [Uses unity_console_read filter=error]
Found 2 errors:
1. NullReferenceException in PlayerController.cs line 42
2. Missing reference on "Enemy" GameObject
```

#### Clear Console
```
User: Clear the console

Claude: [Uses unity_console_clear]
Console cleared.
```

### Building

#### Check Build Settings
```
User: What are the current build settings?

Claude: [Uses unity_build_settings action=get]
Build Target: StandaloneWindows64
Scenes in Build:
1. Assets/Scenes/MainMenu.unity
2. Assets/Scenes/Level1.unity
```

#### Build Project
```
User: Build for Windows

Claude: [Uses unity_build]
Building for Windows...
Build completed successfully!
Output: Builds/StandaloneWindows64/MyGame.exe
```

## Advanced Usage

### Complex Scene Setup

```
User: Set up a platformer level with:
- A ground plane
- 5 floating platforms at different heights
- A player spawn point
- 3 collectible coins

Claude: [Executes multiple tools in sequence]
1. Created Ground plane at (0, 0, 0)
2. Created Platform_1 at (3, 1, 0)
3. Created Platform_2 at (6, 2.5, 0)
4. Created Platform_3 at (9, 4, 0)
5. Created Platform_4 at (12, 2, 0)
6. Created Platform_5 at (15, 3.5, 0)
7. Created SpawnPoint at (0, 1, 0)
8. Instantiated Coin prefabs at various positions

Level setup complete!
```

### Script Generation with Logic

```
User: Create an enemy AI that patrols between waypoints and chases the player when in range

Claude: [Uses unity_script_create with full implementation]
Created EnemyAI.cs with:
- Patrol state
- Chase state
- Detection range
- Waypoint system

[Shows generated code]
```

### Using Workflows

```
User: I want to start a new 2D platformer game

Claude: I'll use the new-project-setup workflow.

[Executes workflow steps]
1. Created folder structure
2. Set up project settings
3. Created GameManager
4. Created AudioManager
5. Created initial scene

Project setup complete! Ready for game development.
```

## Tips and Best Practices

### 1. Wait for Compilation
After creating or modifying scripts, wait for Unity to compile before trying to use them:
```
User: Create a PlayerController script

Claude: Created script. Please wait for Unity to compile...
[Checks unity_status for isCompiling]
Compilation complete! Now attaching to Player...
```

### 2. Check for Errors
Always check the console after complex operations:
```
Claude: [After creating scripts]
Let me check for any compilation errors...
[Uses unity_console_read]
No errors found. Scripts compiled successfully.
```

### 3. Use Descriptive Names
Ask for clear, descriptive names for objects and scripts:
```
User: Create an enemy

Better: Create an enemy called "PatrolEnemy" with an EnemyPatrol script
```

### 4. Save Frequently
Ask Claude to save scenes after making changes:
```
User: Save the scene after these changes
```

### 5. Use Prefabs
For objects you'll reuse, create prefabs:
```
User: Save this as a prefab so I can use it again
```

## Error Handling

Claude will report errors clearly:

```
Claude: I encountered an error:
- Tool: unity_component_add
- Error: Component type "InvalidComponent" not found

Would you like me to:
1. Search for similar component names
2. Try a different approach
```

## Next Steps

- Explore [Workflows](../workflows/README.md) for automated tasks
- Read about [Skills](../skills/README.md) for reusable operations
- Check [Troubleshooting](./troubleshooting.md) for common issues
