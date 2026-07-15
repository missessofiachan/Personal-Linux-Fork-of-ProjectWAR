#!/usr/bin/env bash
# ============================================================
# run-servers.sh
# Runs INSIDE the projectwar distrobox container.
# Launches all 4 ProjectWAR servers in a tmux session so each
# has its own window (tab). Attach with:
#   distrobox enter projectwar -- tmux attach -t projectwar
# ============================================================
set -euo pipefail

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BIN_DIR="$PROJECT_ROOT/bin/Debug"
IMPORT_DIR="$PROJECT_ROOT/ImportIntoProject"
CONFIG_SRC="$PROJECT_ROOT/WorldServer/Configs/LocalDevelopment"
SESSION="projectwar"

# ── Terminal Colors & Formatting ─────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
BOLD='\033[1m'
NC='\033[0m'

info()  { echo -e "${GREEN}[run]${NC} $*"; }
warn()  { echo -e "${YELLOW}[run]${NC} $*"; }
error() { echo -e "${RED}[run]${NC} $*" >&2; }

# ── 1. Pre-flight Dependency Checks ──────────────────────────
for cmd in tmux mono; do
    if ! command -v "$cmd" &> /dev/null; then
        error "Required runtime executable '$cmd' is missing from the container environment."
        exit 1
    fi
done

if [[ ! -d "$BIN_DIR" ]]; then
    error "Build compilation directory not found at: $BIN_DIR"
    error "Please run your compilation framework inside the container first."
    exit 1
fi

for exe in AccountCacher LauncherServer LobbyServer WorldServer; do
    if [[ ! -f "$BIN_DIR/${exe}.exe" ]]; then
        error "Missing required assembly: $BIN_DIR/${exe}.exe — did the build phase succeed?"
        exit 1
    fi
done

# ── 2. Sync Game-Data Assets into Target Binaries ────────────
info "Synchronizing game world assets into $BIN_DIR ..."

sync_data() {
    local target_name="$1"
    local src="$IMPORT_DIR/$target_name"
    local dst="$BIN_DIR/$target_name"

    if [[ -d "$src" ]]; then
        mkdir -p "$dst"
        if command -v rsync &> /dev/null; then
            rsync -a --delete "${src}/" "${dst}/"
        else
            cp -r "${src}/"* "${dst}/" 2>/dev/null || true
        fi
    else
        warn "Data source target absent: $src (skipping asset injection)"
    fi
}

sync_data "zones"
sync_data "los"
sync_data "Configs"

# ── 3. Copy LocalDevelopment Environment Settings ───────────
info "Refreshing target system configuration files ..."
if [[ -d "$CONFIG_SRC" ]]; then
    # Explicitly ensure the inner Configs directory exists
    mkdir -p "$BIN_DIR/Configs"
    
    # Loop through all 5 required configuration files
    for conf_file in Account.xml Lobby.xml World.xml Launcher.xml mythloginserviceconfig.xml; do
        if [[ -f "$CONFIG_SRC/$conf_file" ]]; then
            # Mirror to both locations to satisfy absolute and relative paths safely
            cp "$CONFIG_SRC/$conf_file" "$BIN_DIR/"
            cp "$CONFIG_SRC/$conf_file" "$BIN_DIR/Configs/"
        else
            warn "Config file missing from development source tree: $conf_file"
        fi
    done
else
    warn "Configuration source path missing: $CONFIG_SRC"
fi

# ── 4. Dynamic Performance Allocator Discovery ───────────────
MIMALLOC_PATH=$(find /usr/lib /usr/local/lib /lib -name "libmimalloc.so" -print -quit 2>/dev/null || true)
if [[ -n "$MIMALLOC_PATH" ]]; then
    info "Found mimalloc optimization layer at $MIMALLOC_PATH. Enabling LD_PRELOAD."
    MIMALLOC_PREFIX="LD_PRELOAD=$MIMALLOC_PATH "
else
    MIMALLOC_PREFIX=""
fi

# ── 5. Manage Active Tmux Session Life Cycle ─────────────────
# Force-initialize background tmux daemon by spawning a temporary dummy session
unset TMUX
TMUX='' tmux -L war_servers -f /dev/null new-session -d -s "init_daemon" "sleep 1" 2>/dev/null || true
sleep 1

if TMUX='' tmux -L war_servers -f /dev/null has-session -t "$SESSION" 2>/dev/null; then
    info "Terminating stale '$SESSION' tmux session instances ..."
    TMUX='' tmux -L war_servers -f /dev/null kill-session -t "$SESSION"
fi
TMUX='' tmux -L war_servers -f /dev/null start-server 2>/dev/null || true

start_server() {
    local window_name="$1"
    local exe_name="$2"
    local is_first="${3:-false}"
    
    local cmd="cd \"$BIN_DIR\" && ${MIMALLOC_PREFIX}mono \"${exe_name}.exe\"; echo -e '\n\033[0;31m--- ${exe_name} dropped offline or exited ---\033[0m'; read -p 'Press Enter to close window...'"

    if [[ "$is_first" == "true" ]]; then
        TMUX='' tmux -L war_servers -f /dev/null new-session -d -s "$SESSION" -n "$window_name"
        sleep 0.5
        TMUX='' tmux -L war_servers -f /dev/null send-keys -t "$SESSION:$window_name" "cd '$BIN_DIR' && ${MIMALLOC_PREFIX}mono ${exe_name}.exe" C-m
    else
        TMUX='' tmux -L war_servers -f /dev/null new-window -t "$SESSION" -n "$window_name"
        sleep 0.5
        TMUX='' tmux -L war_servers -f /dev/null send-keys -t "$SESSION:$window_name" "cd '$BIN_DIR' && ${MIMALLOC_PREFIX}mono ${exe_name}.exe" C-m
    fi
    info "Successfully initialized $exe_name inside window: $window_name"
}

info "Spawning fresh tmux environment session '$SESSION' ..."

# Start AccountCacher first (remaining servers depend entirely on its RPC socket initialization)
start_server "AccountCacher" "AccountCacher" true
sleep 2 

# Spin up secondary application environments
start_server "LauncherServer" "LauncherServer" false
start_server "LobbyServer"    "LobbyServer" false
start_server "WorldServer"    "WorldServer" false

# Force view focus back onto the first window tab by default
TMUX='' tmux -L war_servers -f /dev/null select-window -t "$SESSION:0" 2>/dev/null || true

info "✓ All 4 ProjectWAR server services successfully initialized inside tmux container space."
echo ""
echo -e "  ${YELLOW}${BOLD}Useful commands inside container:${NC}"
echo -e "    Attach to console:  ${CYAN}tmux -L war_servers attach -t $SESSION${NC}"
echo -e "    Switch windows:     ${CYAN}Ctrl-b, then window number (0-3)${NC}"
echo -e "    Detach context:     ${CYAN}Ctrl-b d${NC}"
echo -e "    Kill entire stack:  ${CYAN}tmux -L war_servers kill-session -t $SESSION${NC}"
echo ""
echo -e "  ${YELLOW}${BOLD}From your Host Terminal run:${NC}"
echo -e "    ${CYAN}distrobox enter projectwar -- tmux -L war_servers attach -t $SESSION${NC}"
echo ""

