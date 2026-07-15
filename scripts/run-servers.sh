#!/usr/bin/env bash
# ============================================================
# run-servers.sh
# Runs INSIDE the projectwar distrobox container.
# Launches all 4 ProjectWAR servers in a tmux session so each
# has its own visible pane.  Attach with:
#   distrobox enter projectwar -- tmux attach -t projectwar
# ============================================================
set -euo pipefail

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BIN_DIR="$PROJECT_ROOT/bin/Debug"
IMPORT_DIR="$PROJECT_ROOT/ImportIntoProject"
SESSION="projectwar"

RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'; NC='\033[0m'
info()  { echo -e "${GREEN}[run]${NC} $*"; }
warn()  { echo -e "${YELLOW}[run]${NC} $*"; }
error() { echo -e "${RED}[run]${NC} $*" >&2; }

# ── sanity checks ────────────────────────────────────────────
if [[ ! -d "$BIN_DIR" ]]; then
    error "Build output not found at $BIN_DIR"
    error "Run: distrobox enter projectwar -- $PROJECT_ROOT/scripts/container-setup.sh"
    exit 1
fi

for exe in AccountCacher LauncherServer LobbyServer WorldServer; do
    if [[ ! -f "$BIN_DIR/${exe}.exe" ]]; then
        error "Missing: $BIN_DIR/${exe}.exe  — did the build succeed?"
        exit 1
    fi
done

# ── copy game-data assets into bin/ ─────────────────────────
# The WorldServer looks for zones/ and los/ relative to its
# working directory, which is the bin/Debug folder.
info "Syncing game data into $BIN_DIR …"
if [[ -d "$IMPORT_DIR/zones" ]]; then
    rsync -a --delete "$IMPORT_DIR/zones" "$BIN_DIR/" 2>/dev/null \
        || cp -r "$IMPORT_DIR/zones" "$BIN_DIR/"
fi
if [[ -d "$IMPORT_DIR/los" ]]; then
    rsync -a --delete "$IMPORT_DIR/los" "$BIN_DIR/" 2>/dev/null \
        || cp -r "$IMPORT_DIR/los" "$BIN_DIR/"
fi
if [[ -d "$IMPORT_DIR/Configs" ]]; then
    rsync -a "$IMPORT_DIR/Configs" "$BIN_DIR/" 2>/dev/null \
        || cp -r "$IMPORT_DIR/Configs" "$BIN_DIR/"
fi

# ── copy LocalDevelopment configs next to each binary ────────
CONFIG_SRC="$PROJECT_ROOT/WorldServer/Configs/LocalDevelopment"
info "Copying server configs …"
for conf_file in Account.xml Lobby.xml World.xml Launcher.xml; do
    [[ -f "$CONFIG_SRC/$conf_file" ]] && cp "$CONFIG_SRC/$conf_file" "$BIN_DIR/" || true
done

# ── kill existing session if present ────────────────────────
tmux kill-session -t "$SESSION" 2>/dev/null || true

# ── helper: start a server in a named tmux window ────────────
start_server() {
    local window="$1" exe="$2"
    local prefix=""
    if [[ -f "/usr/lib/x86_64-linux-gnu/libmimalloc.so" ]]; then
        prefix="LD_PRELOAD=/usr/lib/x86_64-linux-gnu/libmimalloc.so "
    fi
    local cmd="cd $BIN_DIR && ${prefix}mono ${exe}.exe; echo '--- ${exe} exited ---'; read"
    if [[ "$window" == "AccountCacher" ]]; then
        # First window — new session
        tmux new-session  -d -s "$SESSION" -n "$window" "bash -c '$cmd'"
    else
        tmux new-window   -t "$SESSION"    -n "$window" "bash -c '$cmd'"
    fi
}

info "Starting tmux session '$SESSION' …"

# Start AccountCacher first (other servers depend on its RPC)
start_server "AccountCacher"  "AccountCacher"
sleep 1

# LauncherServer — manages the game launcher / patch notes
start_server "LauncherServer" "LauncherServer"

# LobbyServer — character select / realm list
start_server "LobbyServer"    "LobbyServer"

# WorldServer — the game world itself
start_server "WorldServer"    "WorldServer"

echo ""
info "✓ All 4 servers launched in tmux session '$SESSION'."
info ""
info "  Useful commands:"
info "    Attach to console:  tmux attach -t $SESSION"
info "    Switch panes:       Ctrl-b, then window number (0-3)"
info "    Detach:             Ctrl-b d"
info "    Kill everything:    tmux kill-session -t $SESSION"
info ""
info "  From the host run:"
info "    distrobox enter projectwar -- tmux attach -t $SESSION"
