#!/usr/bin/env bash
# =============================================================================
# uninstall-mac.sh — Remove HavenWoodHollow MCP configuration from macOS
# =============================================================================
# Usage: ./scripts/uninstall-mac.sh [--remove-node-modules] [--unity-project /path]
#
# This script:
#   1. Removes the MCP server entry from Claude Desktop config
#   2. Optionally removes the Unity package from a project
#   3. Optionally cleans node_modules and build artifacts
# =============================================================================
set -euo pipefail

# ── Colour helpers ───────────────────────────────────────────────────────────
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'; BLUE='\033[0;34m'; NC='\033[0m'
info()    { printf "${BLUE}[INFO]${NC}  %s\n" "$*"; }
success() { printf "${GREEN}[OK]${NC}    %s\n" "$*"; }
warn()    { printf "${YELLOW}[WARN]${NC}  %s\n" "$*"; }
fail()    { printf "${RED}[FAIL]${NC}  %s\n" "$*"; }

# ── Parse arguments ─────────────────────────────────────────────────────────
REMOVE_NODE_MODULES=false
UNITY_PROJECT=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --remove-node-modules) REMOVE_NODE_MODULES=true ;;
    --unity-project)       shift; UNITY_PROJECT="${1:-}" ;;
    --unity-project=*)     UNITY_PROJECT="${1#*=}" ;;
    -h|--help)
      echo "Usage: $0 [--remove-node-modules] [--unity-project /path]"
      echo "  --remove-node-modules     Remove node_modules and dist/"
      echo "  --unity-project /path     Remove MCP package from Unity project"
      exit 0
      ;;
    *)
      fail "Unknown option: $1"
      exit 1
      ;;
  esac
  shift
done

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
SERVER_DIR="$REPO_ROOT/unity-mcp-plugin/server"
CLAUDE_CONFIG="$HOME/Library/Application Support/Claude/claude_desktop_config.json"

echo ""
echo "============================================="
echo "  HavenWoodHollow — Uninstall"
echo "============================================="
echo ""

# ── 1. Remove from Claude Desktop config ────────────────────────────────────
if [[ -f "$CLAUDE_CONFIG" ]]; then
  info "Removing unity-game-dev from Claude Desktop config…"

  python3 - "$CLAUDE_CONFIG" <<'PYEOF'
import json, sys

config_path = sys.argv[1]
with open(config_path, "r") as f:
    config = json.load(f)

servers = config.get("mcpServers", {})
if "unity-game-dev" in servers:
    del servers["unity-game-dev"]
    with open(config_path, "w") as f:
        json.dump(config, f, indent=2)
    print("Removed unity-game-dev entry.")
else:
    print("No unity-game-dev entry found — nothing to remove.")
PYEOF

  success "Claude Desktop config updated."
else
  info "No Claude Desktop config found — skipping."
fi

# ── 2. Remove Unity package (if specified) ───────────────────────────────────
if [[ -n "$UNITY_PROJECT" ]]; then
  PKG_PATH="$UNITY_PROJECT/Packages/com.havenwoodhollow.mcp-bridge"
  if [[ -e "$PKG_PATH" || -L "$PKG_PATH" ]]; then
    info "Removing MCP Bridge package from $UNITY_PROJECT …"
    rm -rf "$PKG_PATH"
    success "Unity package removed."
  else
    info "MCP Bridge package not found in $UNITY_PROJECT — skipping."
  fi
fi

# ── 3. Clean build artifacts ─────────────────────────────────────────────────
if [[ "$REMOVE_NODE_MODULES" == true ]]; then
  info "Removing node_modules…"
  rm -rf "$SERVER_DIR/node_modules"
  info "Removing dist/…"
  rm -rf "$SERVER_DIR/dist"
  success "Build artifacts cleaned."
fi

echo ""
echo "============================================="
echo "  ✅  Uninstall complete"
echo "============================================="
echo ""
echo "Note: The repository source code has not been removed."
echo "To fully remove, delete the repository directory."
echo ""
