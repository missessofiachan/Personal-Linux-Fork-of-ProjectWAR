#!/usr/bin/env bash
# ============================================================
# distrobox-create.sh  (run this once on the HOST)
#
# What it does:
#   1. Creates a distrobox container called 'projectwar'
#      (Ubuntu 22.04 — best Mono .NET 4.8 support)
#   2. Runs container-setup.sh inside it to install Mono/MSBuild
#      and compile the solution
#   3. Starts MariaDB on the host and sets up the 3 databases
#   4. Launches all 4 game servers in a tmux session inside
#      the container
#
# Usage:
#   bash scripts/distrobox-create.sh          # full first-time setup
#   bash scripts/distrobox-create.sh --run    # start servers only
#   bash scripts/distrobox-create.sh --build  # rebuild only
# ============================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
CONTAINER_NAME="projectwar"
CONTAINER_IMAGE="ubuntu:22.04"

# ── Terminal Colors ──────────────────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
BOLD='\033[1m'
NC='\033[0m'

banner() {
    echo ""
    echo -e "${CYAN}${BOLD}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo -e "${CYAN}${BOLD}  ProjectWAR – Distrobox Setup Stack${NC}"
    echo -e "${CYAN}${BOLD}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo ""
}

info()  { echo -e "${GREEN}[host]${NC} $*"; }
warn()  { echo -e "${YELLOW}[host]${NC} $*"; }
error() { echo -e "${RED}[host]${NC} $*" >&2; }

# ── Parse Flags ──────────────────────────────────────────────
DO_CREATE=true
DO_BUILD=true
DO_DB=true
DO_RUN=true

for arg in "$@"; do
    case "$arg" in
        --run)
            DO_CREATE=false; DO_BUILD=false; DO_DB=true; DO_RUN=true
            ;;
        --build)
            DO_CREATE=false; DO_BUILD=true; DO_DB=false; DO_RUN=false
            ;;
        --db)
            DO_CREATE=false; DO_BUILD=false; DO_DB=true; DO_RUN=false
            ;;
        --help|-h)
            echo -e "${BOLD}Usage:${NC} $0 [--run | --build | --db]"
            echo "  (no flags) Full first-time configuration framework"
            echo "  --run      Fire up the host database and run game server runtime pipelines"
            echo "  --build    Perform targeted compilation tasks safely inside the runtime box"
            echo "  --db       Initialize schemas and database privileges on host"
            exit 0
            ;;
        *)
            warn "Unknown parameter passed: $arg. Proceeding with full configuration stack execution."
            ;;
    esac
done

banner

# ── Dependency Assertions ────────────────────────────────────
if ! command -v distrobox &>/dev/null; then
    error "distrobox command context not found. Please setup distrobox first:"
    error "  https://distrobox.it/#installation"
    exit 1
fi

# Ensure permissions match configuration expectations
chmod +x "$SCRIPT_DIR"/*.sh

# ── Step 1: Initialize Distrobox Container ───────────────────
if $DO_CREATE; then
    # Distrobox evaluates handles in lower-case context layers safely
    if distrobox list 2>/dev/null | grep -E -q "\b${CONTAINER_NAME}\b"; then
        warn "Container registry match for '$CONTAINER_NAME' already exists – skipping creation."
    else
        info "Creating distrobox container environment '$CONTAINER_NAME' (${CONTAINER_IMAGE}) ..."
        distrobox create \
            --name "$CONTAINER_NAME" \
            --image "$CONTAINER_IMAGE" \
            --volume "$PROJECT_ROOT:$PROJECT_ROOT" \
            --yes \
            --no-entry
        info "Container environment created successfully."
    fi
fi

# ── Step 2: Build Internals inside Container ─────────────────
if $DO_BUILD; then
    info "Running container-setup.sh compilation steps inside '$CONTAINER_NAME' ..."
    # We use --root option inside distrobox enter to cleanly run system setups
    distrobox enter "$CONTAINER_NAME" --root -- bash "$SCRIPT_DIR/container-setup.sh"
    info "Container infrastructure build verification step complete."
fi

# ── Step 3: Configure Host DB Access Layer ───────────────────
if $DO_DB; then
    info "Executing script-level MariaDB access validation tasks on host target ..."
    bash "$SCRIPT_DIR/db-setup.sh"
fi

# ── Step 4: Execute Process Matrix ───────────────────────────
if $DO_RUN; then
    info "Launching runtime tmux execution routines inside container stack '$CONTAINER_NAME' ..."
    distrobox enter "$CONTAINER_NAME" -- bash "$SCRIPT_DIR/run-servers.sh"

    echo ""
    echo -e "${GREEN}${BOLD}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo -e "${GREEN}${BOLD}  ✓  ProjectWAR Stack Initialized Successfully!${NC}"
    echo -e "${GREEN}${BOLD}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo ""
    echo -e "  ${BOLD}Attach to server consoles:${NC}"
    echo -e "    ${CYAN}distrobox enter $CONTAINER_NAME -- tmux attach -t projectwar${NC}"
    echo ""
    echo -e "  ${BOLD}Stop all active server targets cleanly:${NC}"
    echo -e "    ${CYAN}distrobox enter $CONTAINER_NAME -- tmux kill-session -t projectwar${NC}"
    echo ""
    echo -e "  ${BOLD}Hot-Restart runtime components directly:${NC}"
    echo -e "    ${CYAN}bash scripts/distrobox-create.sh --run${NC}"
    echo ""
fi
