# HavenWoodHollow - Unity MCP Plugin for Claude

This repository contains an MCP (Model Context Protocol) plugin that enables Claude Desktop, Cowork, and Claude Code to control and edit Unity projects for AI-assisted video game development.

## Project Overview

The Unity MCP Plugin provides a bridge between Claude and the Unity Editor, allowing:
- Creating and managing GameObjects and scenes
- Writing and editing C# scripts
- Managing assets and prefabs
- Building projects for multiple platforms
- Real-time console monitoring and debugging

## Quick Start

```bash
# Build the MCP server
cd unity-mcp-plugin/server
npm install
npm run build

# Install Unity package (copy to your Unity project)
cp -r unity-mcp-plugin/unity-package YourUnityProject/Packages/com.havenwoodhollow.mcp-bridge
```

See [unity-mcp-plugin/README.md](./unity-mcp-plugin/README.md) for complete installation instructions.

## Repository Structure

```
HavenWoodHollow/
├── unity-mcp-plugin/       # Main plugin directory
│   ├── server/             # Node.js MCP Server
│   │   ├── src/
│   │   │   ├── index.ts    # Main entry point
│   │   │   ├── tools/      # Tool implementations
│   │   │   └── utils/      # Unity bridge communication
│   │   ├── package.json
│   │   └── tsconfig.json
│   ├── unity-package/      # Unity C# Editor Plugin
│   │   ├── Editor/
│   │   │   ├── MCPBridge.cs        # TCP server + dispatch
│   │   │   └── CommandRegistry.cs   # Command handlers
│   │   └── package.json
│   ├── workflows/          # Pre-defined automation workflows
│   ├── skills/             # Reusable skill definitions
│   ├── subagents/          # Specialized agent configurations
│   ├── docs/               # Documentation
│   ├── manifest.json       # MCP bundle manifest
│   ├── CLAUDE.md           # Claude context file
│   └── README.md           # Plugin documentation
└── LICENSE                 # CC0 1.0 Universal
```

## Architecture

```
┌─────────────────────┐
│   Claude Desktop    │
│   Cowork / Code     │
└──────────┬──────────┘
           │ MCP Protocol (JSON-RPC)
           │
┌──────────▼──────────┐
│    MCP Server       │
│    (Node.js)        │
│                     │
│ • Tool definitions  │
│ • Request buffering │
│ • Self-healing      │
└──────────┬──────────┘
           │ TCP Socket (port 8090)
           │
┌──────────▼──────────┐
│   Unity Editor      │
│   MCP Bridge        │
│                     │
│ • Command execution │
│ • Main thread       │
│   dispatch          │
│ • Undo support      │
│ • Compilation       │
│   monitoring        │
└─────────────────────┘
```

## Key Features

### Self-Healing Connection
The plugin survives Unity domain reloads (when scripts recompile) by:
1. Buffering requests during disconnection
2. Automatically reconnecting when Unity restarts
3. Checking for compilation errors on reconnect
4. Processing queued requests

### Undo Support
All Unity operations are wrapped with `Undo.RecordObject()`, allowing users to undo AI-made changes with Ctrl+Z.

### Main Thread Safety
Unity API calls must happen on the main thread. The plugin uses `EditorApplication.update` to dispatch operations safely.

## Commands

### Connection
- `npm run build` - Build TypeScript server
- `npm start` - Start MCP server
- `npm run dev` - Watch mode for development

### Testing
- `npm test` - Run test suite

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make changes
4. Test with Unity
5. Submit pull request

## License

CC0 1.0 Universal - Public Domain Dedication

See [LICENSE](./LICENSE) for details.

## Related Resources

- [Anthropic MCP Documentation](https://modelcontextprotocol.io/)
- [Unity Editor Scripting](https://docs.unity3d.com/Manual/editor-EditorWindows.html)
- [Claude Desktop](https://claude.ai/download)
