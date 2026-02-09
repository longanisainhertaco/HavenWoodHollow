#!/usr/bin/env bash
# =============================================================================
# verify-installation.sh — Verify the HavenWoodHollow MCP setup is correct
# =============================================================================
# Usage: ./scripts/verify-installation.sh [--unity-project /path]
#
# Checks:
#   1. Node.js version
#   2. npm dependencies installed
#   3. MCP server built
#   4. Claude Desktop config present
#   5. Unity package installed (if --unity-project given)
#   6. Unity bridge port availability
# =============================================================================
set -euo pipefail

# ── Colour helpers ───────────────────────────────────────────────────────────
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'; BLUE='\033[0;34m'; NC='\033[0m'
info()    { printf "${BLUE}[INFO]${NC}  %s\n" "$*"; }
success() { printf "${GREEN}[  ✓ ]${NC}  %s\n" "$*"; }
warn()    { printf "${YELLOW}[WARN]${NC}  %s\n" "$*"; }
fail()    { printf "${RED}[  ✗ ]${NC}  %s\n" "$*"; }

UNITY_PROJECT=""
ISSUES=0

for arg in "$@"; do
  case "$arg" in
    --unity-project)  shift; UNITY_PROJECT="${1:-}" ;;
    --unity-project=*) UNITY_PROJECT="${arg#*=}" ;;
    -h|--help)
      echo "Usage: $0 [--unity-project /path]"
      exit 0
      ;;
  esac
done

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
SERVER_DIR="$REPO_ROOT/unity-mcp-plugin/server"
CLAUDE_CONFIG="$HOME/Library/Application Support/Claude/claude_desktop_config.json"

echo ""
echo "============================================="
echo "  HavenWoodHollow — Installation Verification"
echo "============================================="
echo ""

# ── 1. Node.js ───────────────────────────────────────────────────────────────
if command -v node &>/dev/null; then
  NODE_VERSION="$(node -v | sed 's/^v//')"
  NODE_MAJOR="${NODE_VERSION%%.*}"
  if (( NODE_MAJOR >= 18 )); then
    success "Node.js $NODE_VERSION (>= 18 required)"
  else
    fail "Node.js $NODE_VERSION is too old (need >= 18)"
    ((ISSUES++))
  fi
else
  fail "Node.js is not installed"
  ((ISSUES++))
fi

# ── 2. npm dependencies ─────────────────────────────────────────────────────
if [[ -d "$SERVER_DIR/node_modules" ]]; then
  success "npm dependencies installed"
else
  fail "npm dependencies not installed (run ./scripts/setup-mac.sh)"
  ((ISSUES++))
fi

# ── 3. MCP server built ─────────────────────────────────────────────────────
if [[ -f "$SERVER_DIR/dist/index.js" ]]; then
  success "MCP server built (dist/index.js exists)"
else
  fail "MCP server not built (run ./scripts/setup-mac.sh)"
  ((ISSUES++))
fi

# ── 4. Claude Desktop config ────────────────────────────────────────────────
if [[ -f "$CLAUDE_CONFIG" ]]; then
  if python3 -c "
import json, sys
with open('$CLAUDE_CONFIG') as f:
    cfg = json.load(f)
if 'unity-game-dev' in cfg.get('mcpServers', {}):
    sys.exit(0)
else:
    sys.exit(1)
" 2>/dev/null; then
    success "Claude Desktop config has unity-game-dev entry"
  else
    fail "Claude Desktop config exists but missing unity-game-dev entry"
    ((ISSUES++))
  fi
else
  fail "Claude Desktop config not found (run ./scripts/configure-claude-desktop.sh)"
  ((ISSUES++))
fi

# ── 5. Unity package (optional) ─────────────────────────────────────────────
if [[ -n "$UNITY_PROJECT" ]]; then
  PKG_PATH="$UNITY_PROJECT/Packages/com.havenwoodhollow.mcp-bridge"
  if [[ -e "$PKG_PATH" || -L "$PKG_PATH" ]]; then
    if [[ -f "$PKG_PATH/package.json" ]]; then
      success "Unity MCP Bridge package installed in project"
    else
      fail "Unity package path exists but package.json is missing"
      ((ISSUES++))
    fi
  else
    fail "Unity MCP Bridge package not installed in $UNITY_PROJECT"
    ((ISSUES++))
  fi
else
  info "No --unity-project specified; skipping Unity package check."
fi

# ── 6. Port availability ────────────────────────────────────────────────────
PORT=8090
if command -v lsof &>/dev/null; then
  if lsof -i :"$PORT" -sTCP:LISTEN &>/dev/null; then
    warn "Port $PORT is already in use (MCP server may already be running)"
  else
    success "Port $PORT is available"
  fi
else
  info "lsof not available — skipping port check."
fi

# ── Summary ──────────────────────────────────────────────────────────────────
echo ""
if (( ISSUES == 0 )); then
  echo "============================================="
  echo "  ✅  All checks passed!"
  echo "============================================="
else
  echo "============================================="
  printf "  ❌  %d issue(s) found — see above\n" "$ISSUES"
  echo "============================================="
fi
echo ""
