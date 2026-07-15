#!/usr/bin/env bash
# ==============================================================================
# start-projectwar.sh (Double-click or run this script to start everything)
#
# What it does:
#   1. Starts and configures the host database safely.
#   2. Validates and awakens the distrobox container environment.
#   3. Launches all 4 game servers in a detached tmux session inside the container.
#   4. Sends a graphical system notification on completion.
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

# ── Terminal Colors ──────────────────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
BOLD='\033[1m'
NC='\033[0m'

info()  { echo -e "${GREEN}[launcher]${NC} $*"; }
warn()  { echo -e "${YELLOW}[launcher]${NC} $*"; }
error() { echo -e "${RED}[launcher]${NC} $*" >&2; }

# Change working directory to project root to guarantee execution safety
cd "$PROJECT_ROOT"

# ── 1. Initialize Host Database Stack ────────────────────────
info "Verifying database service infrastructure ..."
if [[ -f "$PROJECT_ROOT/scripts/db-setup.sh" ]]; then
    # If executed via GUI launcher without an interactive terminal, try elevating via graphical prompt
    if ! systemctl is-active --quiet mariadb && ! tty -s &>/dev/null; then
        if command -v pkexec &>/dev/null; then
            info "Graphical environment detected. Requesting authentication via pkexec ..."
            pkexec bash "$PROJECT_ROOT/scripts/db-setup.sh"
        else
            bash "$PROJECT_ROOT/scripts/db-setup.sh"
        fi
    else
        bash "$PROJECT_ROOT/scripts/db-setup.sh"
    fi
else
    warn "Database configuration script missing from tree. Falling back to direct service startup."
    if ! systemctl is-active --quiet mariadb; then
        if command -v pkexec &>/dev/null; then pkexec systemctl start mariadb; else sudo systemctl start mariadb; fi
    fi
fi

# ── 2. Run Container and Launch Services ─────────────────────
info "Ensuring distrobox container environment '$CONTAINER_NAME' is active ..."
# distrobox enter cleanly handles boot procedures implicitly if the container is stopped
distrobox enter "$CONTAINER_NAME" --no-tty -- echo "Container environment validated."

info "Spawning game server runtime pipelines inside tmux ..."
distrobox enter "$CONTAINER_NAME" --no-tty -- bash "$PROJECT_ROOT/scripts/run-servers.sh"

# ── 3. Desktop Notification Hook ─────────────────────────────
if command -v notify-send &>/dev/null; then
    notify-send "ProjectWAR Server Engine" "Database instances and all 4 game servers initialized successfully!" -i input-gaming -t 5000
fi

echo ""
echo -e "${GREEN}${BOLD}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${GREEN}${BOLD}  ✓  All core services successfully deployed!${NC}"
echo -e "${GREEN}${BOLD}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo ""
echo -e "  To inspect the running servers, open a terminal and run:"
echo -e "    ${CYAN}distrobox enter $CONTAINER_NAME -- tmux attach -t projectwar${NC}"
echo ""

