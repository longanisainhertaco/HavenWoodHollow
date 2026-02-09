# Unity MCP Plugin - Claude Context

This is an MCP (Model Context Protocol) plugin for controlling and editing Unity projects via Claude Desktop, Cowork, and Claude Code.

## Quick Reference

### Essential Commands
```bash
# Build the MCP server
cd unity-mcp-plugin/server && npm install && npm run build

# Start the MCP server (usually done by Claude Desktop)
npm start
```

### Connection Flow
1. Start Unity Editor with the MCP Bridge package installed
2. MCP Bridge starts TCP listener on port 8090
3. MCP Server connects via TCP socket
4. Claude can now control Unity through tools

### Key Architecture
- **Bridge Pattern**: MCP Server (Node.js) ↔ TCP Socket ↔ Unity Plugin (C#)
- **Self-Healing**: Server buffers requests during Unity recompilation
- **Main Thread Dispatch**: Unity operations queue to EditorApplication.update

## Available Tools

### Connection
- `unity_connect` - Connect to Unity Editor (port 8090)
- `unity_disconnect` - Disconnect from Unity
- `unity_status` - Check connection status

### Scene Management
- `unity_scene_create` - Create new scene
- `unity_scene_load` - Load existing scene
- `unity_scene_save` - Save current scene
- `unity_hierarchy_view` - View scene hierarchy
- `unity_play_mode` - Control play/pause/stop

### GameObject Operations
- `unity_gameobject_create` - Create GameObjects
- `unity_gameobject_find` - Find by name/tag/component
- `unity_gameobject_modify` - Change properties
- `unity_gameobject_delete` - Delete objects
- `unity_component_add` - Add components
- `unity_component_modify` - Modify component properties

### Script Management
- `unity_script_create` - Create C# scripts
- `unity_script_read` - Read script content
- `unity_script_edit` - Modify scripts
- `unity_console_read` - Read Unity console

### Asset Operations
- `unity_asset_search` - Search project assets
- `unity_asset_list` - List folder contents
- `unity_prefab_create` - Create prefabs
- `unity_prefab_instantiate` - Instantiate prefabs
- `unity_material_create` - Create materials

### Build
- `unity_build` - Build project
- `unity_build_settings` - Get/set build settings
- `unity_switch_platform` - Change build target

## Common Workflows

### Creating a Player Character
1. `unity_gameobject_create` - Create capsule primitive
2. `unity_script_create` - Create PlayerController script
3. Wait for compilation
4. `unity_component_add` - Add script to GameObject
5. `unity_prefab_create` - Save as prefab

### Setting Up a Scene
1. `unity_scene_create` - Create new scene
2. `unity_gameobject_create` - Add floor plane
3. `unity_gameobject_create` - Add lighting
4. `unity_prefab_instantiate` - Add player
5. `unity_scene_save` - Save scene

## Unity API Notes

### Critical APIs Exposed
- `AssetDatabase.Refresh()` - Forces recompilation after script changes
- `Undo.RecordObject()` - All changes support Ctrl+Z
- `EditorApplication.isCompiling` - Check compilation status
- `Application.logMessageReceived` - Capture console errors

### Thread Safety
Unity API is NOT thread-safe. All operations are dispatched to main thread via `EditorApplication.update`.

### Domain Reload Handling
When Unity recompiles:
1. TCP connection drops
2. MCP Server enters polling mode
3. Requests are queued
4. Unity restarts, reconnects
5. Server checks for compile errors
6. Queued requests are processed

## File Structure

```
unity-mcp-plugin/
├── manifest.json           # Claude Desktop bundle manifest
├── server/                 # Node.js MCP Server
│   ├── src/
│   │   ├── index.ts       # Main entry point
│   │   ├── tools/         # Tool implementations
│   │   └── utils/
│   │       └── unity-bridge.ts  # TCP communication
│   ├── package.json
│   └── tsconfig.json
├── unity-package/          # Unity Editor Plugin
│   ├── Editor/
│   │   ├── MCPBridge.cs   # TCP listener + dispatch
│   │   └── CommandRegistry.cs  # Command handlers
│   └── package.json
├── workflows/              # Pre-defined workflows
├── skills/                 # Reusable skill definitions
├── subagents/              # Specialized agent configs
└── docs/                   # Documentation
```

## Best Practices

1. **Always connect first**: Use `unity_status` to verify connection
2. **Wait for compilation**: After creating/editing scripts, wait before using them
3. **Use Undo-safe operations**: All tools wrap changes in Undo.RecordObject
4. **Check console for errors**: Use `unity_console_read` after operations
5. **Save frequently**: Use `unity_scene_save` to persist changes

## Troubleshooting

### Connection Issues
- Verify Unity is running with MCP Bridge installed
- Check port 8090 is not in use
- Look for errors in Unity Console

### Compilation Errors
- Use `unity_console_read` to see errors
- Server will auto-detect compile errors on reconnect
- Fix script errors before continuing

### Performance
- Avoid rapid-fire operations during compilation
- Use batch operations where possible
- Monitor Unity Editor responsiveness
