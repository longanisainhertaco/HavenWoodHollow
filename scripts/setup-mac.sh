#!/usr/bin/env bash
# =============================================================================
# setup-mac.sh — One-step macOS setup for the HavenWoodHollow Unity MCP Plugin
# =============================================================================
# Usage: ./scripts/setup-mac.sh [--skip-brew] [--skip-unity-check]
#
# This script:
#   1. Verifies (and optionally installs) Homebrew
#   2. Verifies Node.js >= 18
#   3. Checks for Unity Hub / Unity Editor
#   4. Installs npm dependencies for the MCP server
#   5. Builds the MCP server
#   6. Prints next-step instructions
# =============================================================================
set -euo pipefail

# ── Colour helpers ───────────────────────────────────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Colour

info()    { printf "${BLUE}[INFO]${NC}  %s\n" "$*"; }
success() { printf "${GREEN}[OK]${NC}    %s\n" "$*"; }
warn()    { printf "${YELLOW}[WARN]${NC}  %s\n" "$*"; }
fail()    { printf "${RED}[FAIL]${NC}  %s\n" "$*"; }

# ── Parse flags ──────────────────────────────────────────────────────────────
SKIP_BREW=false
SKIP_UNITY=false

for arg in "$@"; do
  case "$arg" in
    --skip-brew)        SKIP_BREW=true  ;;
    --skip-unity-check) SKIP_UNITY=true ;;
    -h|--help)
      echo "Usage: $0 [--skip-brew] [--skip-unity-check]"
      echo "  --skip-brew          Skip Homebrew installation check"
      echo "  --skip-unity-check   Skip Unity Hub/Editor detection"
      exit 0
      ;;
    *)
      fail "Unknown option: $arg"
      exit 1
      ;;
  esac
done

# ── Resolve repo root (this script lives in <repo>/scripts/) ────────────────
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
SERVER_DIR="$REPO_ROOT/unity-mcp-plugin/server"

# ── Load .env if present (CLI flags above take precedence) ──────────────────
if [[ -f "$REPO_ROOT/.env" ]]; then
  # shellcheck source=/dev/null
  set -a; source "$REPO_ROOT/.env"; set +a
fi

echo ""
echo "============================================="
echo "  HavenWoodHollow — macOS Setup"
echo "============================================="
echo ""

# ── 1. macOS check ──────────────────────────────────────────────────────────
if [[ "$(uname)" != "Darwin" ]]; then
  fail "This script is intended for macOS. Detected: $(uname)"
  exit 1
fi
success "Running on macOS $(sw_vers -productVersion 2>/dev/null || echo '(unknown version)')"

# ── 2. Homebrew ──────────────────────────────────────────────────────────────
if [[ "$SKIP_BREW" == false ]]; then
  if command -v brew &>/dev/null; then
    success "Homebrew found: $(brew --version | head -1)"
  else
    info "Homebrew not found. Installing…"
    /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
    # Ensure brew is on PATH for Apple Silicon Macs
    if [[ -f /opt/homebrew/bin/brew ]]; then
      eval "$(/opt/homebrew/bin/brew shellenv)"
    fi
    success "Homebrew installed."
  fi
else
  info "Skipping Homebrew check (--skip-brew)."
fi

# ── 3. Node.js >= 18 ────────────────────────────────────────────────────────
REQUIRED_NODE_MAJOR=18

if command -v node &>/dev/null; then
  NODE_VERSION="$(node -v | sed 's/^v//')"
  NODE_MAJOR="${NODE_VERSION%%.*}"
  if (( NODE_MAJOR >= REQUIRED_NODE_MAJOR )); then
    success "Node.js $NODE_VERSION detected (>= $REQUIRED_NODE_MAJOR required)."
  else
    warn "Node.js $NODE_VERSION is too old (need >= $REQUIRED_NODE_MAJOR)."
    info "Installing latest LTS via Homebrew…"
    brew install node@22
    brew link --overwrite node@22
    success "Node.js $(node -v) installed."
  fi
else
  info "Node.js not found. Installing via Homebrew…"
  brew install node@22
  success "Node.js $(node -v) installed."
fi

# npm sanity check
if ! command -v npm &>/dev/null; then
  fail "npm not found after installing Node.js. Please check your PATH."
  exit 1
fi

# ── 4. Unity Hub / Unity Editor ─────────────────────────────────────────────
UNITY_HUB_MAC="/Applications/Unity Hub.app"
UNITY_EDITORS_DIR="/Applications/Unity/Hub/Editor"

if [[ "$SKIP_UNITY" == false ]]; then
  echo ""
  info "Checking for Unity installation…"

  if [[ -d "$UNITY_HUB_MAC" ]]; then
    success "Unity Hub found at $UNITY_HUB_MAC"
  else
    warn "Unity Hub not found at $UNITY_HUB_MAC"
    warn "Download Unity Hub from: https://unity.com/download"
  fi

  if [[ -d "$UNITY_EDITORS_DIR" ]]; then
    EDITORS_FOUND=$(ls -1 "$UNITY_EDITORS_DIR" 2>/dev/null || true)
    if [[ -n "$EDITORS_FOUND" ]]; then
      success "Unity Editor version(s) found:"
      echo "$EDITORS_FOUND" | while read -r ver; do
        echo "        • $ver"
      done
    else
      warn "Unity Hub Editor directory exists but no editors installed."
      warn "Open Unity Hub and install Unity 2021.3 LTS or later."
    fi
  else
    warn "No Unity Editor installations found."
    warn "Install Unity 2021.3 LTS (or later) via Unity Hub."
  fi
else
  info "Skipping Unity check (--skip-unity-check)."
fi

# ── 5. Install npm dependencies ─────────────────────────────────────────────
echo ""
info "Installing MCP server dependencies…"
cd "$SERVER_DIR"

if [[ ! -f "package.json" ]]; then
  fail "package.json not found in $SERVER_DIR."
  exit 1
fi

npm install --ignore-scripts
success "npm dependencies installed."

# ── 6. Build the MCP server ─────────────────────────────────────────────────
info "Building MCP server (TypeScript → JavaScript)…"
npm run build
success "MCP server built successfully."

# ── 7. Summary ───────────────────────────────────────────────────────────────
echo ""
echo "============================================="
echo "  ✅  Setup complete!"
echo "============================================="
echo ""
echo "Next steps:"
echo ""
echo "  1. Configure Claude Desktop:"
echo "       ./scripts/configure-claude-desktop.sh"
echo ""
echo "  2. Install the Unity package into your project:"
echo "       ./scripts/install-unity-package.sh /path/to/YourUnityProject"
echo ""
echo "  3. Open Unity, then start the MCP server:"
echo "       ./scripts/start-server.sh"
echo ""
echo "  4. Verify everything works:"
echo "       ./scripts/verify-installation.sh"
echo ""
