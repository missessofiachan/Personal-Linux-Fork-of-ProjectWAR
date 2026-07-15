#!/usr/bin/env bash
# ============================================================
# container-setup.sh
# Runs INSIDE the projectwar distrobox container via the setup script.
# Installs Mono/MSBuild (using the official Mono repo) and
# builds ProjectWAR (skipping the native C++ WarZone project,
# whose .dll already exists in the tree).
# ============================================================
set -euo pipefail

# Dynamically locate the project root relative to the script location
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
MARKER="$PROJECT_ROOT/.distrobox_setup_done"

# ── Terminal Colors ──────────────────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

info()  { echo -e "${GREEN}[setup]${NC} $*"; }
warn()  { echo -e "${YELLOW}[setup]${NC} $*"; }
error() { echo -e "${RED}[setup]${NC} $*" >&2; }

# ── 1. Package Installation (Requires Root Context) ──────────
if [[ -f "$MARKER" ]]; then
    info "Packages already installed – skipping directly to build step."
else
    # Verify we are running as root before running package updates
    if [[ "$EUID" -ne 0 ]]; then
        error "Package installation must be executed in a root context."
        error "Please run this via distrobox-create.sh or 'distrobox enter projectwar --root'"
        exit 1
    fi

    info "Adding official Mono repository mappings ..."
    DEBIAN_FRONTEND=noninteractive apt-get update -qq
    DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
        gnupg ca-certificates apt-transport-https curl 2>&1 | tail -n 3

    mkdir -p /etc/apt/keyrings
    curl -fsSL https://download.mono-project.com/repo/xamarin.gpg \
        | gpg --dearmor -o /etc/apt/keyrings/mono-keyring.gpg

    echo 'deb [signed-by=/etc/apt/keyrings/mono-keyring.gpg] https://download.mono-project.com/repo/ubuntu stable-focal main' \
        > /etc/apt/sources.list.d/mono-official-stable.list

    DEBIAN_FRONTEND=noninteractive apt-get update -qq 2>&1 | tail -n 3

    info "Installing mono-complete, msbuild, nuget, tmux, and mimalloc dependencies ..."
    DEBIAN_FRONTEND=noninteractive apt-get install -y \
        mono-complete msbuild nuget tmux libmimalloc2.0 libmimalloc-dev 2>&1 \
        | grep -E '(Setting up|E:|error)' || true

    # Trust Mono's SSL cert store explicitly
    cert-sync /etc/ssl/certs/ca-certificates.crt 2>/dev/null || true

    touch "$MARKER"
    info "System packages installed successfully."
fi

# ── 2. Determine Build User context ──────────────────────────
# Find the actual non-root user calling the environment to avoid building as root
if [[ "${SUDO_USER:-}" != "" ]]; then
    BUILD_USER="$SUDO_USER"
elif [[ "${USER:-}" != "root" ]]; then
    BUILD_USER="$USER"
else
    # Distrobox shares your host home environment pathing
    BUILD_USER=$(stat -c '%U' "$PROJECT_ROOT")
fi

info "Executing compilation tasks as user: ${CYAN}$BUILD_USER${NC}"

# ── 3. Restore and Compile ───────────────────────────────────
# Helper to execute shell tasks securely under the regular user profile
run_as_user() {
    local cmd="$1"
    su - "$BUILD_USER" -c "cd '$PROJECT_ROOT' && $cmd"
}

info "Restoring NuGet package dependencies ..."
run_as_user "nuget restore ProjectWAR.sln -PackagesDirectory packages -NonInteractive" 2>&1 | tail -n 5

info "Building solution via MSBuild (excluding native C++ WarZone component) ..."
BUILD_CMD="msbuild ProjectWAR.sln \
    /p:Configuration=Debug \
    /p:Platform=x64 \
    /t:Common,FrameWork,AccountCacher,LauncherServer,LobbyServer,WorldServer \
    /p:WarningLevel=0 \
    /verbosity:minimal"

if run_as_user "$BUILD_CMD"; then
    echo ""
    info "✓ Build completed successfully."
    info "  Binary Output Location: ${CYAN}$PROJECT_ROOT/bin/Debug/${NC}"
    echo ""
else
    echo ""
    error "❌ MSBuild pipeline execution failed. Review errors shown above."
    exit 1
fi