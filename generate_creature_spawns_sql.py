import mysql.connector
import math

conn = mysql.connector.connect(
    host="127.0.0.1",
    user="root",
    password="password"
)
cursor = conn.cursor(dictionary=True)

print("Restoring Domain 11: Creature Spawns (High-Fidelity Placement)...")

# Fetch truth spawns
cursor.execute("SELECT ID, Name, ZoneID, X, Y, Z, Heading FROM war_londo.monster WHERE X != 0 OR Y != 0")
truth_monsters = cursor.fetchall()

sql_statements = []
print(f"Processing {len(truth_monsters)} truth spawns...")

for tm in truth_monsters:
    # Clean the name for matching (Emulator names often have ^M or ^f)
    name_clean = str(tm['Name']).split('^')[0].strip().replace("'", "''")
    zone_id = tm['ZoneID']
    tx = int(tm['X'])
    ty = int(tm['Y'])
    tz = int(tm['Z'])
    th = int(tm['Heading'] * 4096 / (2 * math.pi))
    
    # Try to find a matching spawn in the emulator
    # Strategy: Match by ZoneID and name (LIKE) and coordinate proximity
    # We use a 1000 unit (approx 10 meter) tolerance for proximity
    query = f"""
        SELECT guid FROM war_world.creature_spawns w
        JOIN war_world.creature_protos p ON w.Entry = p.Entry
        WHERE w.ZoneId = {zone_id} 
        AND p.Name LIKE '{name_clean}%'
        AND abs(w.WorldX - {tx}) < 1000
        AND abs(w.WorldY - {ty}) < 1000
        LIMIT 1
    """
    cursor.execute(query)
    match = cursor.fetchone()
    
    if match:
        guid = match['guid']
        sql = (f"UPDATE creature_spawns SET "
               f"WorldX = {tx}, WorldY = {ty}, WorldZ = {tz}, WorldO = {th} "
               f"WHERE guid = {guid};")
        sql_statements.append(sql)

output_file = 'Database/updates/update_148_restore_creature_spawns.sql'
with open(output_file, 'w', encoding='utf-8') as f:
    f.write('-- ProjectWAR Database Restoration (Patch 1.4.8)\n')
    f.write('-- Domain: Creature Spawns (High-Fidelity Placement)\n')
    f.write('-- Source: Londos Server v2 1.4.8 monster table mapping by proximity\n\n')
    f.write('\n'.join(sql_statements))

print(f"Generated {len(sql_statements)} SQL update statements in {output_file}")
conn.close()
