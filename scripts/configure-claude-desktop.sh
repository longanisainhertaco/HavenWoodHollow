#!/usr/bin/env bash
# =============================================================================
# configure-claude-desktop.sh — Register the MCP server with Claude Desktop
# =============================================================================
# Usage: ./scripts/configure-claude-desktop.sh [--server-port PORT]
#
# Creates or updates:
#   ~/Library/Application Support/Claude/claude_desktop_config.json
# =============================================================================
set -euo pipefail

# ── Colour helpers ───────────────────────────────────────────────────────────
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'; BLUE='\033[0;34m'; NC='\033[0m'
info()    { printf "${BLUE}[INFO]${NC}  %s\n" "$*"; }
success() { printf "${GREEN}[OK]${NC}    %s\n" "$*"; }
warn()    { printf "${YELLOW}[WARN]${NC}  %s\n" "$*"; }
fail()    { printf "${RED}[FAIL]${NC}  %s\n" "$*"; }

# ── Parse arguments ─────────────────────────────────────────────────────────
SERVER_PORT=8090
while [[ $# -gt 0 ]]; do
  case "$1" in
    --server-port)  shift; SERVER_PORT="${1:-8090}" ;;
    --server-port=*) SERVER_PORT="${1#*=}" ;;
    -h|--help)
      echo "Usage: $0 [--server-port PORT]"
      echo "  --server-port PORT   MCP bridge port (default: 8090)"
      exit 0
      ;;
  esac
  shift
done

# ── Resolve paths ────────────────────────────────────────────────────────────
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
SERVER_ENTRY="$REPO_ROOT/unity-mcp-plugin/server/dist/index.js"
CLAUDE_CONFIG_DIR="$HOME/Library/Application Support/Claude"
CLAUDE_CONFIG="$CLAUDE_CONFIG_DIR/claude_desktop_config.json"

echo ""
echo "============================================="
echo "  Configure Claude Desktop for Unity MCP"
echo "============================================="
echo ""

# ── Check that the server has been built ─────────────────────────────────────
if [[ ! -f "$SERVER_ENTRY" ]]; then
  fail "MCP server has not been built yet."
  fail "Run ./scripts/setup-mac.sh first, or: cd unity-mcp-plugin/server && npm run build"
  exit 1
fi
success "MCP server build found at $SERVER_ENTRY"

# ── Check that Claude Desktop exists ────────────────────────────────────────
if [[ ! -d "/Applications/Claude.app" ]]; then
  warn "Claude Desktop not found in /Applications."
  warn "Download from: https://claude.ai/download"
  warn "Continuing to write config anyway…"
fi

# ── Create config directory if needed ────────────────────────────────────────
mkdir -p "$CLAUDE_CONFIG_DIR"

# ── Build the MCP server configuration block ────────────────────────────────
# We need to merge into any existing config. If the file doesn't exist or is
# empty, create a fresh one.  If it exists, we inject our server entry.

NODE_PATH="$(command -v node)"

NEW_SERVER_BLOCK=$(cat <<EOF
{
  "command": "$NODE_PATH",
  "args": ["$SERVER_ENTRY"],
  "env": {
    "UNITY_MCP_PORT": "$SERVER_PORT"
  }
}
EOF
)

if [[ -f "$CLAUDE_CONFIG" ]] && [[ -s "$CLAUDE_CONFIG" ]]; then
  info "Existing Claude config found. Merging unity-game-dev entry…"

  # Use a lightweight Python snippet to merge JSON safely
  python3 - "$CLAUDE_CONFIG" "$NEW_SERVER_BLOCK" <<'PYEOF'
import json, sys

config_path = sys.argv[1]
new_block = json.loads(sys.argv[2])

with open(config_path, "r") as f:
    config = json.load(f)

config.setdefault("mcpServers", {})
config["mcpServers"]["unity-game-dev"] = new_block

with open(config_path, "w") as f:
    json.dump(config, f, indent=2)

print("Merged successfully.")
PYEOF

else
  info "Creating new Claude Desktop config…"

  python3 - "$CLAUDE_CONFIG" "$NEW_SERVER_BLOCK" <<'PYEOF'
import json, sys

config_path = sys.argv[1]
new_block = json.loads(sys.argv[2])

config = {
    "mcpServers": {
        "unity-game-dev": new_block
    }
}

with open(config_path, "w") as f:
    json.dump(config, f, indent=2)

print("Created successfully.")
PYEOF
fi

success "Claude Desktop config written to:"
echo "        $CLAUDE_CONFIG"

echo ""
echo "============================================="
echo "  ✅  Claude Desktop configured!"
echo "============================================="
echo ""
echo "Next steps:"
echo "  1. Restart Claude Desktop (quit and reopen)"
echo "  2. Open your Unity project with the MCP Bridge package installed"
echo "  3. Claude will auto-connect when you use the Unity tools"
echo ""
