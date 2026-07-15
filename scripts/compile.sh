#!/usr/bin/env bash
# ==============================================================================
# compile.sh
# Compiles the ProjectWAR codebase inside the distrobox container.
#
# Usage:
#   bash scripts/compile.sh            # Standard fast incremental compilation
#   bash scripts/compile.sh --clean    # Wipe artifacts and perform clean build
#   bash scripts/compile.sh --restore  # Restore NuGet dependencies before building
#   bash scripts/compile.sh --run      # Compile and immediately hot-restart servers
# ==============================================================================
set -euo pipefail

# ── Dynamic Path Resolution ──────────────────────────────────
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
if [[ "$(basename "$SCRIPT_DIR")" == "scripts" ]]; then
    PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
else
    PROJECT_ROOT="$SCRIPT_DIR"
fi

CONTAINER_NAME="projectwar"

# ── Terminal Colors & Formatting ─────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
BOLD='\033[1m'
NC='\033[0m'

info()    { echo -e "${CYAN}[build]${NC} $*"; }
warn()    { echo -e "${YELLOW}[build]${NC} $*"; }
error()   { echo -e "${RED}[build]${NC} $*" >&2; }
success() { echo -e "\n${GREEN}[build]${NC} $*"; }

# ── 1. Parse Arguments & Flags ───────────────────────────────
CLEAN_BUILD=false
DO_RESTORE=false
RUN_AFTER_BUILD=false

while [[ $# -gt 0 ]]; do
    case "$1" in
        -c|--clean)
            CLEAN_BUILD=true
            shift
            ;;
        -r|--restore)
            DO_RESTORE=true
            shift
            ;;
        --run)
            RUN_AFTER_BUILD=true
            shift
            ;;
        -h|--help)
            echo -e "${BOLD}Usage:${NC} $0 [options]"
            echo ""
            echo "Options:"
            echo "  -c, --clean    Perform a clean build by invoking MSBuild target cleaning first."
            echo "  -r, --restore  Restore NuGet package dependencies inside the box before compiling."
            echo "  --run          Automatically execute start-projectwar.sh upon a successful build."
            echo "  -h, --help     Display this help interface."
            exit 0
            ;;
        *)
            warn "Unknown parameter passed: $1. Skipping configuration."
            shift
            ;;
    esac
done

echo -e "${BLUE}=== ProjectWAR Engine Build Matrix ===${NC}"

# ── 2. Determine Parallel Build Job Factor ───────────────────
if [[ -n "${JOBS:-}" ]]; then
    NUM_JOBS="$JOBS"
elif command -v nproc &>/dev/null; then
    NUM_JOBS=$(nproc)
elif command -v sysctl &>/dev/null; then
    NUM_JOBS=$(sysctl -n hw.ncpu)
else
    NUM_JOBS=2
fi

# ── 3. Core Pre-flight Validation ────────────────────────────
# Pre-verify the container configuration exists before initializing build routines
if ! command -v distrobox &>/dev/null; then
    error "Distrobox toolchain missing from the active environment path."
    exit 1
fi

if ! distrobox list 2>/dev/null | grep -E -q "\b${CONTAINER_NAME}\b"; then
    error "Target container instance '$CONTAINER_NAME' is absent. Please run distrobox-create.sh first."
    exit 1
fi

# Start the compilation stop-watch timer
START_TIME=$(date +%s)

# ── 4. Clean Build Routine (If Requested) ────────────────────
if $CLEAN_BUILD; then
    info "Wiping existing compilation artifacts via MSBuild target cleaning ..."
    CLEAN_CMD="cd '$PROJECT_ROOT' && msbuild ProjectWAR.sln /t:Clean /nologo /v:m"
    if ! distrobox enter "$CONTAINER_NAME" --no-tty -- bash -c "$CLEAN_CMD"; then
        warn "Clean target operations completed with non-critical notices."
    fi
fi

# ── 5. NuGet Package Restoration ─────────────────────────────
if $DO_RESTORE; then
    info "Running NuGet package dependency sync routines ..."
    RESTORE_CMD="cd '$PROJECT_ROOT' && nuget restore ProjectWAR.sln -PackagesDirectory packages -NonInteractive"
    if ! distrobox enter "$CONTAINER_NAME" --no-tty -- bash -c "$RESTORE_CMD"; then
        error "NuGet environment restoration failed. Review downstream dependencies."
        exit 1
    fi
fi

# ── 6. Execute Multi-Processor MSBuild Pipeline ──────────────
info "Compiling ProjectWAR C# stack targets using ${NUM_JOBS} parallel workers ..."

# Explicitly targets our refined list of C# projects to cleanly skip native C++ components
BUILD_CMD="cd '$PROJECT_ROOT' && msbuild ProjectWAR.sln \
    /p:Configuration=Debug \
    /p:Platform=x64 \
    /t:Common,FrameWork,AccountCacher,LauncherServer,LobbyServer,WorldServer \
    /p:MaxCpuCount=${NUM_JOBS} \
    /nologo \
    /v:m"

if distrobox enter "$CONTAINER_NAME" --no-tty -- bash -c "$BUILD_CMD"; then
    END_TIME=$(date +%s)
    ELAPSED=$((END_TIME - START_TIME))
    
    success "✓ Build finished successfully in ${ELAPSED}s!"
    info "Binary output verified inside: ${YELLOW}$PROJECT_ROOT/bin/Debug/${NC}"
else
    error "❌ Codebase compilation matrix failed. Review errors generated above."
    exit 1
fi

# ── 7. Optional Post-Build Hot-Restart ───────────────────────
if $RUN_AFTER_BUILD; then
    LAUNCHER_SCRIPT="$PROJECT_ROOT/start-projectwar.sh"
    if [[ -f "$LAUNCHER_SCRIPT" ]]; then
        success "Triggering automatic application stack hot-restart ..."
        cd "$PROJECT_ROOT"
        bash start-projectwar.sh
    else
        error "Orchestration script missing at: $LAUNCHER_SCRIPT. Manual boot required."
        exit 1
    fi
else
    echo -e "  Run ${CYAN}bash start-projectwar.sh${NC} to apply your new changes and refresh your active runtime servers.\n"
fi
