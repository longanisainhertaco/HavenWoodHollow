# Unity MCP Plugin

An MCP (Model Context Protocol) plugin for Claude Desktop, Cowork, and Claude Code that enables AI-assisted Unity game development.

## Features

- **Full Unity Editor Control**: Create, modify, and manage GameObjects, scenes, scripts, and assets
- **Self-Healing Architecture**: Survives Unity domain reloads during script compilation
- **Undo Support**: All operations are undoable via Ctrl+Z
- **Real-time Console Access**: Read Unity logs and compilation errors
- **Build Automation**: Build projects for multiple platforms

## Architecture

This plugin uses a "Bridge Pattern" with three components:

```
┌─────────────────┐     TCP Socket      ┌─────────────────┐
│  Claude Desktop │ ←─────────────────→ │  Unity Editor   │
│                 │    (port 8090)      │                 │
│  MCP Server     │                     │  MCP Bridge     │
│  (Node.js)      │                     │  (C# Plugin)    │
└─────────────────┘                     └─────────────────┘
```

### Why This Architecture?

Unity's domain reload (when scripts recompile) kills all connections. The external Node.js server:
- Buffers requests during recompilation
- Automatically reconnects when Unity restarts
- Reports compilation errors to Claude
- Maintains conversation state

## Installation

### macOS Quick Setup

For macOS, use the automated setup scripts:

```bash
chmod +x scripts/*.sh
./scripts/setup-mac.sh                                # Install deps & build
./scripts/configure-claude-desktop.sh                  # Register with Claude
./scripts/install-unity-package.sh /path/to/Project    # Install Unity package
./scripts/verify-installation.sh                       # Verify everything
```

See [`scripts/README.md`](../scripts/README.md) for full script documentation.

### Manual Installation

#### 1. Install Unity Package

Copy the `unity-package` folder into your Unity project:

```bash
# Option A: Copy directly
cp -r unity-mcp-plugin/unity-package YourUnityProject/Packages/com.havenwoodhollow.mcp-bridge

# Option B: Add via Package Manager
# In Unity: Window > Package Manager > + > Add package from git URL
# Enter: https://github.com/longanisainhertaco/HavenWoodHollow.git?path=unity-mcp-plugin/unity-package
```

#### 2. Build the MCP Server

```bash
cd unity-mcp-plugin/server
npm install
npm run build
```

#### 3. Configure Claude Desktop

Add to your `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "unity-game-dev": {
      "command": "node",
      "args": ["/path/to/unity-mcp-plugin/server/dist/index.js"],
      "env": {}
    }
  }
}
```

Or use the MCPB bundle format by creating:

```json
{
  "name": "unity-mcp-bridge",
  "version": "1.0.0",
  "server": {
    "type": "node",
    "entry_point": "server/dist/index.js"
  },
  "permissions": {
    "filesystem": ["path/to/unity/project"]
  }
}
```

#### 4. Start Using

1. Open your Unity project
2. The MCP Bridge starts automatically (check Console for "MCP Bridge Initialized")
3. Open Claude Desktop
4. Ask Claude to connect: "Connect to Unity and show me the scene hierarchy"

## Usage Examples

### Basic Connection
```
User: Connect to Unity and tell me about the current scene

Claude: *Uses unity_connect, then unity_scene_info and unity_hierarchy_view*
```

### Creating a Character
```
User: Create a player character with WASD movement

Claude: *Creates GameObject, writes PlayerController script, adds components*
```

### Building the Game
```
User: Build the game for Windows

Claude: *Configures build settings, executes build, reports results*
```

## Available Tools

| Tool | Description |
|------|-------------|
| `unity_connect` | Connect to Unity Editor |
| `unity_status` | Check connection status |
| `unity_scene_create` | Create a new scene |
| `unity_scene_load` | Load an existing scene |
| `unity_scene_save` | Save the current scene |
| `unity_hierarchy_view` | View scene hierarchy |
| `unity_gameobject_create` | Create GameObjects |
| `unity_gameobject_modify` | Modify GameObject properties |
| `unity_component_add` | Add components |
| `unity_script_create` | Create C# scripts |
| `unity_script_edit` | Edit existing scripts |
| `unity_console_read` | Read Unity console |
| `unity_build` | Build the project |
| `unity_play_mode` | Control play mode |

See [CLAUDE.md](./CLAUDE.md) for full tool documentation.

## Workflows

Pre-defined workflows for common tasks:

- **new-project-setup**: Initialize project structure
- **character-creation**: Create playable characters
- **level-design**: Set up game levels
- **ui-system**: Create UI screens

See [workflows/README.md](./workflows/README.md) for details.

## Development

### Project Structure
```
unity-mcp-plugin/
├── server/           # Node.js MCP Server
├── unity-package/    # Unity C# Plugin
├── workflows/        # Workflow definitions
├── skills/           # Reusable skills
├── subagents/        # Specialized agents
└── docs/             # Documentation
```

### Building from Source

```bash
# Server
cd server
npm install
npm run build
npm run dev  # Watch mode

# Unity Package
# Just copy to Unity project, compiles automatically
```

### Running Tests

```bash
cd server
npm test
```

## Troubleshooting

### Unity Console shows "MCP Bridge: Listener error"
- Check if port 8090 is already in use
- Restart Unity

### Claude says "Not connected to Unity"
- Ensure Unity Editor is running
- Check Unity Console for MCP Bridge messages
- Use `unity_connect` tool explicitly

### Scripts not working after creation
- Wait for Unity to finish compiling
- Check `unity_console_read` for compilation errors
- The server automatically buffers requests during compilation

### Build fails
- Check Unity Console for errors
- Ensure all scenes are in Build Settings
- Verify platform support is installed

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

CC0 1.0 Universal - See [LICENSE](../LICENSE)

## Acknowledgments

- [Anthropic](https://anthropic.com) for Claude and MCP
- [Unity Technologies](https://unity.com) for Unity Editor APIs
- The MCP community for inspiration and patterns
