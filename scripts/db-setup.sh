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

# ── colours ─────────────────────────────────────────────────
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'; NC='\033[0m'
info()    { echo -e "${GREEN}[db]${NC} $*"; }
warn()    { echo -e "${YELLOW}[db]${NC} $*"; }
error()   { echo -e "${RED}[db]${NC} $*" >&2; }

# ── 1. start MariaDB ─────────────────────────────────────────
info "Starting MariaDB service …"
if ! sudo systemctl is-active --quiet mariadb; then
    sudo systemctl start mariadb
    sleep 2
fi

if ! sudo systemctl is-active --quiet mariadb; then
    error "MariaDB failed to start. Check: sudo systemctl status mariadb"
    exit 1
fi
info "MariaDB is running."

# ── 2. read password ─────────────────────────────────────────
# Pull the password directly from the World.xml config so we
# have a single source of truth.
CONFIG_XML="$PROJECT_ROOT/WorldServer/Configs/LocalDevelopment/World.xml"
DB_PASS=$(grep -oP '(?<=<Password>)[^<]+' "$CONFIG_XML" | head -1)
DB_USER=$(grep -oP '(?<=<Username>)[^<]+' "$CONFIG_XML" | head -1)

info "Using database user: $DB_USER"

# ── 3. check root access ─────────────────────────────────────
# Try passwordless (socket auth) first, then fall back to the
# password stored in the config.
MYSQL_OPTS=""
if sudo mariadb -u root -e "SELECT 1;" &>/dev/null; then
    MYSQL_CMD="sudo mariadb"
elif mariadb -u "$DB_USER" -p"$DB_PASS" -e "SELECT 1;" &>/dev/null; then
    MYSQL_CMD="mariadb -u $DB_USER -p$DB_PASS"
else
    warn "Could not authenticate automatically."
    warn "Please enter your MariaDB root password when prompted."
    MYSQL_CMD="sudo mariadb -u root -p"
fi

run_sql() { $MYSQL_CMD -e "$1" 2>/dev/null; }

# ── 4. create databases ──────────────────────────────────────
if [[ -f "$DB_MARKER" ]]; then
    warn "DB marker found – skipping database creation & import."
    warn "Delete '$DB_MARKER' to re-run the import."
    exit 0
fi

info "Creating databases …"
run_sql "CREATE DATABASE IF NOT EXISTS war_accounts CHARACTER SET utf8mb4;"
run_sql "CREATE DATABASE IF NOT EXISTS war_characters CHARACTER SET utf8mb4;"
run_sql "CREATE DATABASE IF NOT EXISTS war_world CHARACTER SET utf8mb4;"

# Ensure the configured user has access
run_sql "CREATE USER IF NOT EXISTS '$DB_USER'@'127.0.0.1' IDENTIFIED BY '$DB_PASS';" || true
run_sql "GRANT ALL PRIVILEGES ON war_accounts.*   TO '$DB_USER'@'127.0.0.1';" || true
run_sql "GRANT ALL PRIVILEGES ON war_characters.* TO '$DB_USER'@'127.0.0.1';" || true
run_sql "GRANT ALL PRIVILEGES ON war_world.*      TO '$DB_USER'@'127.0.0.1';" || true
run_sql "FLUSH PRIVILEGES;"

# ── 5. import SQL dumps ──────────────────────────────────────
import_sql_file() {
    local db="$1" file="$2"
    info "  Importing $(basename "$file") → $db …"
    $MYSQL_CMD "$db" < "$file"
}

# Map file-name prefixes/patterns to databases
for sql_file in "$IMPORT_DIR"/*.sql "$IMPORT_DIR"/**/*.sql 2>/dev/null; do
    [[ -f "$sql_file" ]] || continue
    fname=$(basename "$sql_file" .sql | tr '[:upper:]' '[:lower:]')
    case "$fname" in
        *account*)   import_sql_file war_accounts   "$sql_file" ;;
        *character*) import_sql_file war_characters "$sql_file" ;;
        *world*)     import_sql_file war_world      "$sql_file" ;;
        *)
            warn "  Unknown SQL file: $sql_file — skipping."
            warn "  Import it manually: mariadb <database> < $sql_file"
            ;;
    esac
done

touch "$DB_MARKER"
info "✓ Database setup complete."
