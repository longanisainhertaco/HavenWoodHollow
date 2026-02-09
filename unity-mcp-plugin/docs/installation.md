# Installation Guide

Complete guide for installing and configuring the Unity MCP Plugin.

## Prerequisites

- **Unity**: Version 2021.3 LTS or newer
- **Node.js**: Version 18.0 or newer
- **Claude Desktop/Cowork/Code**: Latest version with MCP support

## Step 1: Install the Unity Package

### Option A: Package Manager (Recommended)

1. Open your Unity project
2. Go to **Window > Package Manager**
3. Click the **+** button
4. Select **Add package from git URL**
5. Enter: `https://github.com/longanisainhertaco/HavenWoodHollow.git?path=unity-mcp-plugin/unity-package`
6. Click **Add**

### Option B: Manual Installation

1. Download or clone this repository
2. Copy the entire `unity-package` folder
3. Paste it into your Unity project's `Packages` folder
4. Rename it to `com.havenwoodhollow.mcp-bridge`

The folder structure should look like:
```
YourProject/
├── Assets/
├── Packages/
│   ├── com.havenwoodhollow.mcp-bridge/
│   │   ├── Editor/
│   │   │   ├── MCPBridge.cs
│   │   │   ├── CommandRegistry.cs
│   │   │   └── UnityMCPBridge.Editor.asmdef
│   │   └── package.json
│   └── manifest.json
└── ProjectSettings/
```

### Option C: Embedded Package

1. Create a folder: `Assets/Plugins/MCPBridge/Editor`
2. Copy `MCPBridge.cs` and `CommandRegistry.cs` into it
3. Unity will compile automatically

## Step 2: Verify Unity Installation

After installation:

1. Open the Unity Console (**Window > General > Console**)
2. Look for: `[MCP Bridge] Initialized on port 8090`
3. If you see this message, the Unity plugin is working

### Troubleshooting Unity Installation

**Error: "Namespace 'CompilationPipeline' not found"**
- Add `using UnityEditor.Compilation;` to MCPBridge.cs

**Error: Port already in use**
- Another application is using port 8090
- Edit MCPBridge.cs and change `_port = 8090` to another port
- Remember to update the MCP server configuration too

## Step 3: Build the MCP Server

```bash
# Navigate to the server directory
cd unity-mcp-plugin/server

# Install dependencies
npm install

# Build TypeScript
npm run build

# Verify build succeeded
ls dist/index.js  # Should exist
```

## Step 4: Configure Claude Desktop

### Locate Config File

**macOS:**
```
~/Library/Application Support/Claude/claude_desktop_config.json
```

**Windows:**
```
%APPDATA%\Claude\claude_desktop_config.json
```

**Linux:**
```
~/.config/claude/claude_desktop_config.json
```

### Add MCP Server Configuration

Edit the config file to add:

```json
{
  "mcpServers": {
    "unity-game-dev": {
      "command": "node",
      "args": ["/absolute/path/to/unity-mcp-plugin/server/dist/index.js"],
      "env": {}
    }
  }
}
```

**Important**: Use the absolute path to `index.js`!

### Example Configurations

**macOS:**
```json
{
  "mcpServers": {
    "unity-game-dev": {
      "command": "node",
      "args": ["/Users/yourname/Projects/HavenWoodHollow/unity-mcp-plugin/server/dist/index.js"]
    }
  }
}
```

**Windows:**
```json
{
  "mcpServers": {
    "unity-game-dev": {
      "command": "node",
      "args": ["C:\\Projects\\HavenWoodHollow\\unity-mcp-plugin\\server\\dist\\index.js"]
    }
  }
}
```

## Step 5: Verify Connection

1. **Start Unity** with your project
2. **Start Claude Desktop** (restart if already running)
3. **Ask Claude**: "Connect to Unity and check the status"
4. Claude should report successful connection

### Expected Output
```
Claude: I'll connect to Unity now.

[Uses unity_connect tool]

Successfully connected to Unity Editor!
Project: YourProjectName
Unity Version: 2022.3.x
```

## Updating

### Update Unity Package

1. Remove old package from Package Manager
2. Reinstall using the same method

### Update MCP Server

```bash
cd unity-mcp-plugin/server
git pull  # or download new version
npm install
npm run build
```

Then restart Claude Desktop.

## Uninstalling

### Remove Unity Package

1. Open Package Manager
2. Find "MCP Bridge for Claude"
3. Click **Remove**

Or delete the folder manually from `Packages/`

### Remove MCP Server

1. Edit `claude_desktop_config.json`
2. Remove the `unity-game-dev` entry
3. Delete the server files
4. Restart Claude Desktop

## Next Steps

- Read the [Usage Guide](./usage-guide.md)
- Explore [Workflows](../workflows/README.md)
- Check [Troubleshooting](./troubleshooting.md)
