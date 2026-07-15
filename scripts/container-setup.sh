#!/usr/bin/env bash
# ============================================================
# container-setup.sh
# Runs on the HOST via: podman exec -u root projectwar bash ...
# Installs Mono/MSBuild (using the official Mono repo) and
# builds ProjectWAR (skipping the native C++ WarZone project,
# whose .dll already exists in the tree).
# ============================================================
set -euo pipefail

PROJECT_ROOT="/run/media/system/NVME_GAME_1/GitHub/ProjectWAR"
MARKER="$PROJECT_ROOT/.distrobox_setup_done"

# ── already done? ────────────────────────────────────────────
if [[ -f "$MARKER" ]]; then
    echo "[setup] Packages already installed – skipping to build."
else
    echo "[setup] Adding Mono official repository …"
    DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
        gnupg ca-certificates apt-transport-https curl 2>&1 | tail -3

    curl -fsSL https://download.mono-project.com/repo/xamarin.gpg \
        | gpg --dearmor -o /usr/share/keyrings/mono-keyring.gpg

    echo 'deb [signed-by=/usr/share/keyrings/mono-keyring.gpg] https://download.mono-project.com/repo/ubuntu stable-focal main' \
        > /etc/apt/sources.list.d/mono-official-stable.list

    DEBIAN_FRONTEND=noninteractive apt-get update -qq 2>&1 | tail -3

    echo "[setup] Installing mono-complete, msbuild, nuget, tmux …"
    DEBIAN_FRONTEND=noninteractive apt-get install -y \
        mono-complete msbuild nuget tmux 2>&1 \
        | grep -E '(Setting up|E:|error)' || true

    # Trust Mono's SSL cert store
    cert-sync /etc/ssl/certs/ca-certificates.crt 2>/dev/null || true

    touch "$MARKER"
    echo "[setup] Packages installed."
fi

# Switch to the project user for building
BUILD_USER="${SUDO_USER:-sofia}"
cd "$PROJECT_ROOT"

echo "[setup] Restoring NuGet packages …"
su - "$BUILD_USER" -c "
    cd '$PROJECT_ROOT'
    nuget restore ProjectWAR.sln \
        -PackagesDirectory packages \
        -NonInteractive 2>&1 | tail -5
" || nuget restore ProjectWAR.sln -PackagesDirectory packages -NonInteractive 2>&1 | tail -5

echo "[setup] Building solution (excluding native C++ WarZone) …"
su - "$BUILD_USER" -c "
    cd '$PROJECT_ROOT'
    msbuild ProjectWAR.sln \
        /p:Configuration=Debug \
        /p:Platform=x64 \
        /t:Common,FrameWork,AccountCacher,LauncherServer,LobbyServer,WorldServer \
        /p:WarningLevel=0 \
        /verbosity:minimal 2>&1
" || msbuild ProjectWAR.sln \
        /p:Configuration=Debug \
        /p:Platform=x64 \
        /t:Common,FrameWork,AccountCacher,LauncherServer,LobbyServer,WorldServer \
        /p:WarningLevel=0 \
        /verbosity:minimal 2>&1

echo ""
echo "[setup] ✓ Build complete."
echo "[setup]   Output: $PROJECT_ROOT/bin/Debug/"
