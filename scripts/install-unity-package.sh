#!/usr/bin/env bash
# =============================================================================
# install-unity-package.sh — Install the MCP Bridge into a Unity project
# =============================================================================
# Usage: ./scripts/install-unity-package.sh /path/to/UnityProject [--copy]
#
# By default, creates a symbolic link so the package stays in sync with the
# repo.  Pass --copy to do a full copy instead.
# =============================================================================
set -euo pipefail

# ── Colour helpers ───────────────────────────────────────────────────────────
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'; BLUE='\033[0;34m'; NC='\033[0m'
info()    { printf "${BLUE}[INFO]${NC}  %s\n" "$*"; }
success() { printf "${GREEN}[OK]${NC}    %s\n" "$*"; }
warn()    { printf "${YELLOW}[WARN]${NC}  %s\n" "$*"; }
fail()    { printf "${RED}[FAIL]${NC}  %s\n" "$*"; }

# ── Parse arguments ─────────────────────────────────────────────────────────
USE_COPY=false
UNITY_PROJECT=""

for arg in "$@"; do
  case "$arg" in
    --copy) USE_COPY=true ;;
    -h|--help)
      echo "Usage: $0 /path/to/UnityProject [--copy]"
      echo ""
      echo "  /path/to/UnityProject   Root of the target Unity project"
      echo "  --copy                  Copy files instead of symlinking"
      exit 0
      ;;
    *)
      if [[ -z "$UNITY_PROJECT" ]]; then
        UNITY_PROJECT="$arg"
      else
        fail "Unknown option: $arg"
        exit 1
      fi
      ;;
  esac
done

if [[ -z "$UNITY_PROJECT" ]]; then
  fail "Usage: $0 /path/to/UnityProject [--copy]"
  exit 1
fi

# ── Resolve paths ────────────────────────────────────────────────────────────
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
PACKAGE_SRC="$REPO_ROOT/unity-mcp-plugin/unity-package"
PACKAGES_DIR="$UNITY_PROJECT/Packages"
DEST="$PACKAGES_DIR/com.havenwoodhollow.mcp-bridge"

echo ""
echo "============================================="
echo "  Install Unity MCP Bridge Package"
echo "============================================="
echo ""

# ── Validate source ─────────────────────────────────────────────────────────
if [[ ! -d "$PACKAGE_SRC" ]]; then
  fail "Unity package source not found at $PACKAGE_SRC"
  exit 1
fi

if [[ ! -f "$PACKAGE_SRC/package.json" ]]; then
  fail "package.json missing in $PACKAGE_SRC"
  exit 1
fi

# ── Validate target Unity project ───────────────────────────────────────────
if [[ ! -d "$UNITY_PROJECT" ]]; then
  fail "Unity project directory does not exist: $UNITY_PROJECT"
  exit 1
fi

if [[ ! -d "$UNITY_PROJECT/Assets" ]]; then
  fail "No Assets/ folder found — is this a Unity project? $UNITY_PROJECT"
  exit 1
fi

# Create Packages dir if it doesn't exist
mkdir -p "$PACKAGES_DIR"

# ── Handle existing installation ─────────────────────────────────────────────
if [[ -e "$DEST" || -L "$DEST" ]]; then
  warn "Existing installation found at $DEST"
  read -rp "  Remove and reinstall? [y/N] " answer
  case "$answer" in
    [yY]|[yY][eE][sS])
      rm -rf "$DEST"
      info "Removed previous installation."
      ;;
    *)
      info "Keeping existing installation. Exiting."
      exit 0
      ;;
  esac
fi

# ── Install ──────────────────────────────────────────────────────────────────
if [[ "$USE_COPY" == true ]]; then
  info "Copying package to $DEST …"
  cp -R "$PACKAGE_SRC" "$DEST"
  success "Package copied."
else
  info "Symlinking package to $DEST …"
  ln -s "$PACKAGE_SRC" "$DEST"
  success "Symlink created: $DEST → $PACKAGE_SRC"
fi

echo ""
echo "============================================="
echo "  ✅  Unity package installed!"
echo "============================================="
echo ""
echo "Next steps:"
echo "  1. Open (or restart) Unity for this project"
echo "  2. Unity will detect the new package automatically"
echo "  3. The MCP Bridge editor window will appear under:"
echo "       Window → HavenWoodHollow → MCP Bridge"
echo ""
