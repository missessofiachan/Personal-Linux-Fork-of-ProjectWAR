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

RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'
CYAN='\033[0;36m'; BOLD='\033[1m'; NC='\033[0m'

banner() {
    echo ""
    echo -e "${CYAN}${BOLD}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo -e "${CYAN}${BOLD}  ProjectWAR – Distrobox Setup${NC}"
    echo -e "${CYAN}${BOLD}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo ""
}

info()  { echo -e "${GREEN}[host]${NC} $*"; }
warn()  { echo -e "${YELLOW}[host]${NC} $*"; }
error() { echo -e "${RED}[host]${NC} $*" >&2; }

# ── parse flags ──────────────────────────────────────────────
DO_CREATE=true
DO_BUILD=true
DO_DB=true
DO_RUN=true

for arg in "$@"; do
    case "$arg" in
        --run)   DO_CREATE=false; DO_BUILD=false; DO_DB=true  ;;
        --build) DO_CREATE=false; DO_BUILD=true;  DO_DB=false; DO_RUN=false ;;
        --db)    DO_CREATE=false; DO_BUILD=false; DO_DB=true;  DO_RUN=false ;;
        --help|-h)
            echo "Usage: $0 [--run|--build|--db]"
            echo "  (no flag)  Full first-time setup"
            echo "  --run      Start services only (skips build)"
            echo "  --build    Rebuild inside container only"
            echo "  --db       Set up / reset the database only"
            exit 0 ;;
    esac
done

banner

# ── require distrobox ────────────────────────────────────────
if ! command -v distrobox &>/dev/null; then
    error "distrobox not found. Install it first:"
    error "  https://distrobox.it/#installation"
    exit 1
fi

# ── make scripts executable ──────────────────────────────────
chmod +x "$SCRIPT_DIR"/*.sh

# ── step 1: create the container ─────────────────────────────
if $DO_CREATE; then
    if distrobox list 2>/dev/null | grep -q "^$CONTAINER_NAME\b"; then
        warn "Container '$CONTAINER_NAME' already exists – skipping creation."
    else
        info "Creating distrobox container '$CONTAINER_NAME' (${CONTAINER_IMAGE}) …"
        distrobox create \
            --name "$CONTAINER_NAME" \
            --image "$CONTAINER_IMAGE" \
            --volume "$PROJECT_ROOT:$PROJECT_ROOT" \
            --yes \
            --no-entry
        info "Container created."
    fi
fi

# ── step 2: build inside the container ───────────────────────
if $DO_BUILD; then
    info "Running container-setup.sh inside '$CONTAINER_NAME' (as root) …"
    podman exec -u root "$CONTAINER_NAME" \
        bash "$SCRIPT_DIR/container-setup.sh"
    info "Build step complete."
fi

# ── step 3: set up the host database ─────────────────────────
if $DO_DB; then
    info "Setting up MariaDB on the host …"
    bash "$SCRIPT_DIR/db-setup.sh"
fi

# ── step 4: launch the servers ───────────────────────────────
if $DO_RUN; then
    info "Launching game servers in container '$CONTAINER_NAME' …"
    podman exec "$CONTAINER_NAME" \
        bash "$SCRIPT_DIR/run-servers.sh"

    echo ""
    echo -e "${GREEN}${BOLD}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo -e "${GREEN}${BOLD}  ✓  ProjectWAR servers are running!${NC}"
    echo -e "${GREEN}${BOLD}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo ""
    echo -e "  ${BOLD}Attach to server consoles:${NC}"
    echo -e "    distrobox enter projectwar -- tmux attach -t projectwar"
    echo ""
    echo -e "  ${BOLD}Stop all servers:${NC}"
    echo -e "    distrobox enter projectwar --no-tty -- tmux kill-session -t projectwar"
    echo ""
    echo -e "  ${BOLD}Restart servers after a rebuild:${NC}"
    echo -e "    bash scripts/distrobox-create.sh --run"
    echo ""
fi
