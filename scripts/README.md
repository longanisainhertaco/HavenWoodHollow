# macOS Deployment Scripts

One-command scripts to install, configure, and manage the HavenWoodHollow Unity MCP Plugin on macOS.

## Quick Start

```bash
# Make scripts executable (one-time)
chmod +x scripts/*.sh

# Optional: create a .env file to set defaults
cp .env.example .env
# Edit .env with your project path and preferred port

# Full setup — installs dependencies, builds the server
./scripts/setup-mac.sh

# Register MCP server with Claude Desktop
./scripts/configure-claude-desktop.sh

# Install the Unity package into your project
./scripts/install-unity-package.sh /path/to/YourUnityProject

# Open Unity, then start the server
./scripts/start-server.sh

# Verify everything is wired up correctly
./scripts/verify-installation.sh --unity-project /path/to/YourUnityProject
```

## Scripts

| Script | Purpose |
|--------|---------|
| [`setup-mac.sh`](#setup-macsh) | Full macOS environment setup |
| [`configure-claude-desktop.sh`](#configure-claude-desktopsh) | Register MCP server with Claude Desktop |
| [`install-unity-package.sh`](#install-unity-packagesh) | Install Unity editor package |
| [`start-server.sh`](#start-serversh) | Start the MCP server |
| [`verify-installation.sh`](#verify-installationsh) | Check that everything is configured |
| [`uninstall-mac.sh`](#uninstall-macsh) | Clean uninstall |

---

### `setup-mac.sh`

Full environment bootstrap for macOS. Checks and installs prerequisites, then builds the MCP server.

```bash
./scripts/setup-mac.sh [--skip-brew] [--skip-unity-check]
```

| Flag | Description |
|------|-------------|
| `--skip-brew` | Skip Homebrew installation check |
| `--skip-unity-check` | Skip Unity Hub / Editor detection |

**What it does:**
1. Verifies macOS
2. Checks / installs Homebrew
3. Checks / installs Node.js ≥ 18
4. Detects Unity Hub and installed editor versions
5. Runs `npm install` for the MCP server
6. Runs `npm run build` to compile TypeScript

---

### `configure-claude-desktop.sh`

Creates or updates the Claude Desktop configuration file so Claude can discover and launch the MCP server.

```bash
./scripts/configure-claude-desktop.sh [--server-port PORT]
```

| Flag | Default | Description |
|------|---------|-------------|
| `--server-port` | `8090` | TCP port the Unity bridge listens on |

**Config location:** `~/Library/Application Support/Claude/claude_desktop_config.json`

---

### `install-unity-package.sh`

Installs the Unity MCP Bridge editor package into a Unity project.

```bash
./scripts/install-unity-package.sh /path/to/UnityProject [--copy]
```

| Flag | Description |
|------|-------------|
| `--copy` | Copy files instead of creating a symlink |

By default, a **symlink** is created so the package stays in sync with the repository. Use `--copy` if you need a standalone copy.

---

### `start-server.sh`

Starts the MCP server (or runs in watch mode for development).

```bash
./scripts/start-server.sh [--port PORT] [--dev]
```

| Flag | Default | Description |
|------|---------|-------------|
| `--port` | `8090` | Override bridge port |
| `--dev` | — | Watch mode with auto-rebuild |

---

### `verify-installation.sh`

Runs a series of checks to confirm the installation is correct.

```bash
./scripts/verify-installation.sh [--unity-project /path]
```

**Checks performed:**
- Node.js ≥ 18 installed
- npm dependencies present
- MCP server compiled
- Claude Desktop config contains `unity-game-dev` entry
- Unity package installed (when `--unity-project` is given)
- Bridge port (8090) available

---

### `uninstall-mac.sh`

Removes MCP configuration and optionally cleans up build artifacts.

```bash
./scripts/uninstall-mac.sh [--remove-node-modules] [--unity-project /path]
```

| Flag | Description |
|------|-------------|
| `--remove-node-modules` | Also delete `node_modules/` and `dist/` |
| `--unity-project /path` | Remove the MCP Bridge package from a Unity project |

---

## Environment File (`.env`)

All scripts automatically load a `.env` file from the repository root if it exists. This lets you set defaults once instead of passing flags on every invocation.

```bash
cp .env.example .env   # create from template
```

### Supported Variables

| Variable | Default | Used by |
|----------|---------|---------|
| `UNITY_MCP_PORT` | `8090` | `configure-claude-desktop.sh`, `start-server.sh`, `verify-installation.sh` |
| `UNITY_PROJECT` | *(none)* | `install-unity-package.sh`, `verify-installation.sh`, `uninstall-mac.sh` |
| `NODE_PATH` | *(auto-detected)* | `configure-claude-desktop.sh` |

**Precedence:** CLI flags > `.env` values > built-in defaults.

Example `.env`:
```bash
UNITY_MCP_PORT=9090
UNITY_PROJECT=/Users/me/Projects/MyGame
```

With this file, you can simply run:
```bash
./scripts/install-unity-package.sh   # uses UNITY_PROJECT from .env
./scripts/start-server.sh            # uses port 9090 from .env
./scripts/start-server.sh --port 7777  # CLI flag overrides .env
```

> **Note:** `.env` is listed in `.gitignore` and will not be committed.

---

## Prerequisites

| Requirement | Minimum Version | Notes |
|-------------|----------------|-------|
| macOS | 12 Monterey+ | Apple Silicon and Intel supported |
| Homebrew | Latest | Auto-installed by `setup-mac.sh` |
| Node.js | 18.0.0 | Auto-installed via Homebrew |
| Unity Hub | Latest | Manual install required |
| Unity Editor | 2021.3 LTS+ | Install via Unity Hub |
| Claude Desktop | Latest | [Download](https://claude.ai/download) |

## Troubleshooting

**"This script is intended for macOS"** — These scripts are macOS-only. For Windows, refer to the manual installation guide in `unity-mcp-plugin/docs/installation.md`.

**"npm dependencies not installed"** — Run `./scripts/setup-mac.sh` to install everything.

**"Port 8090 is already in use"** — Another process is using the port. Either stop it or use `--port` to choose a different one.

**Claude doesn't see the Unity tools** — Restart Claude Desktop after running `configure-claude-desktop.sh`. Ensure Unity is open with the MCP Bridge package.
