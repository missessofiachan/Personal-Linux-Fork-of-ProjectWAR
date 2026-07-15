#!/usr/bin/env bash
# ============================================================
# db-setup.sh
# Runs on the HOST.
# Starts MariaDB, creates the 3 ProjectWAR databases, and
# imports any .sql dump files found in ImportIntoProject/.
# ============================================================
set -euo pipefail

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
IMPORT_DIR="$PROJECT_ROOT/ImportIntoProject"
DB_MARKER="$PROJECT_ROOT/.db_setup_done"

# ── Terminal Colors ──────────────────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

info()    { echo -e "${GREEN}[db]${NC} $*"; }
warn()    { echo -e "${YELLOW}[db]${NC} $*"; }
error()   { echo -e "${RED}[db]${NC} $*" >&2; }

# ── 1. Start MariaDB ─────────────────────────────────────────
info "Starting MariaDB service ..."
if ! sudo systemctl is-active --quiet mariadb; then
    sudo systemctl start mariadb
    sleep 2
fi

if ! sudo systemctl is-active --quiet mariadb; then
    error "MariaDB failed to start. Check: sudo systemctl status mariadb"
    exit 1
fi
info "MariaDB is running."

# ── 2. Read Credentials from Config ──────────────────────────
CONFIG_XML="$PROJECT_ROOT/WorldServer/Configs/LocalDevelopment/World.xml"

if [[ ! -f "$CONFIG_XML" ]]; then
    error "Configuration file not found: $CONFIG_XML"
    exit 1
fi

# Multi-strategy parsing (PCRE grep with a sed fallback)
DB_PASS=$(grep -oP '(?<=<Password>)[^<]+' "$CONFIG_XML" | head -1 || sed -n 's/.*<Password>\([^<]*\)<\/Password>.*/\1/p' "$CONFIG_XML" | head -1)
DB_USER=$(grep -oP '(?<=<Username>)[^<]+' "$CONFIG_XML" | head -1 || sed -n 's/.*<Username>\([^<]*\)<\/Username>.*/\1/p' "$CONFIG_XML" | head -1)

if [[ -z "$DB_USER" || -z "$DB_PASS" ]]; then
    error "Failed to parse database username or password from $CONFIG_XML"
    exit 1
fi

info "Using database user: ${CYAN}$DB_USER${NC}"

# ── 3. Check Access & Setup Connection Client ─────────────────
# Test multiple configurations to auto-authenticate cleanly
if sudo mariadb -u root -e "SELECT 1;" &>/dev/null; then
    MYSQL_CMD="sudo mariadb"
elif mariadb -u "$DB_USER" -p"$DB_PASS" -e "SELECT 1;" &>/dev/null; then
    MYSQL_CMD="mariadb -u $DB_USER -p$DB_PASS"
else
    warn "Could not authenticate automatically."
    warn "Please enter your MariaDB root password when prompted."
    MYSQL_CMD="sudo mariadb -u root -p"
fi

run_sql() { 
    $MYSQL_CMD -e "$1"
}

# ── 4. Create Databases and Users ────────────────────────────
if [[ -f "$DB_MARKER" ]]; then
    warn "DB marker found – skipping database creation & import."
    warn "Delete '$DB_MARKER' to re-run the full import process."
    exit 0
fi

info "Creating databases ..."
run_sql "CREATE DATABASE IF NOT EXISTS war_accounts CHARACTER SET utf8mb4;"
run_sql "CREATE DATABASE IF NOT EXISTS war_characters CHARACTER SET utf8mb4;"
run_sql "CREATE DATABASE IF NOT EXISTS war_world CHARACTER SET utf8mb4;"

# Ensure user access works both from host ('localhost') and from the container namespace ('%')
info "Configuring privileges for container & host contexts ..."
for host_scope in 'localhost' '%'; do
    run_sql "CREATE USER IF NOT EXISTS '$DB_USER'@'$host_scope' IDENTIFIED BY '$DB_PASS';"
    run_sql "GRANT ALL PRIVILEGES ON war_accounts.*   TO '$DB_USER'@'$host_scope';"
    run_sql "GRANT ALL PRIVILEGES ON war_characters.* TO '$DB_USER'@'$host_scope';"
    run_sql "GRANT ALL PRIVILEGES ON war_world.*      TO '$DB_USER'@'$host_scope';"
done
run_sql "FLUSH PRIVILEGES;"

# ── 5. Import SQL Dumps ──────────────────────────────────────
import_sql_file() {
    local db="$1" file="$2"
    info "  Importing $(basename "$file") → ${CYAN}$db${NC} ..."
    $MYSQL_CMD "$db" < "$file"
}

if [[ -d "$IMPORT_DIR" ]]; then
    info "Scanning for database schemas in $IMPORT_DIR ..."
    
    # Safely handle nested trees and unexpected spacing using a read loop
    while IFS= read -r -d '' sql_file; do
        fname=$(basename "$sql_file" .sql | tr '[:upper:]' '[:lower:]')
        case "$fname" in
            *account*)   import_sql_file war_accounts   "$sql_file" ;;
            *character*) import_sql_file war_characters "$sql_file" ;;
            *world*)     import_sql_file war_world      "$sql_file" ;;
            *)
                warn "  Unknown SQL file mapping: $sql_file — skipping."
                warn "  Import manually if needed: mariadb <database> < \"$sql_file\""
                ;;
        esac
    done < <(find "$IMPORT_DIR" -type f -name "*.sql" -print0 2>/dev/null)
else
    warn "Import directory not found at $IMPORT_DIR. No schemas populated."
fi

touch "$DB_MARKER"
info "✓ Database setup complete."

