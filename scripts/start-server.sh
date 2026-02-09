#!/usr/bin/env bash
# =============================================================================
# start-server.sh — Start the Unity MCP server
# =============================================================================
# Usage: ./scripts/start-server.sh [--port PORT] [--dev]
#
# Options:
#   --port PORT   Override the default bridge port (8090)
#   --dev         Run in watch mode (auto-rebuild on changes)
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
DEV_MODE=false

while [[ $# -gt 0 ]]; do
  case "$1" in
    --port)   shift; SERVER_PORT="${1:-8090}" ;;
    --port=*) SERVER_PORT="${1#*=}" ;;
    --dev)    DEV_MODE=true ;;
    -h|--help)
      echo "Usage: $0 [--port PORT] [--dev]"
      echo "  --port PORT   Bridge port (default: 8090)"
      echo "  --dev         Watch mode with auto-rebuild"
      exit 0
      ;;
    *)
      fail "Unknown option: $1"
      exit 1
      ;;
  esac
  shift
done

# ── Resolve paths ────────────────────────────────────────────────────────────
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
SERVER_DIR="$REPO_ROOT/unity-mcp-plugin/server"

echo ""
echo "============================================="
echo "  Unity MCP Server"
echo "============================================="
echo ""

# ── Preflight checks ────────────────────────────────────────────────────────
if [[ ! -d "$SERVER_DIR/node_modules" ]]; then
  fail "Dependencies not installed. Run ./scripts/setup-mac.sh first."
  exit 1
fi

if [[ "$DEV_MODE" == false ]] && [[ ! -f "$SERVER_DIR/dist/index.js" ]]; then
  fail "Server not built. Run ./scripts/setup-mac.sh or: cd unity-mcp-plugin/server && npm run build"
  exit 1
fi

cd "$SERVER_DIR"
export UNITY_MCP_PORT="$SERVER_PORT"

if [[ "$DEV_MODE" == true ]]; then
  info "Starting in development mode (watch + auto-rebuild)…"
  info "Port: $SERVER_PORT"
  echo ""
  npm run dev
else
  info "Starting MCP server…"
  info "Port: $SERVER_PORT"
  echo ""
  npm start
fi
