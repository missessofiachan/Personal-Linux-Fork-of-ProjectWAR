import mysql.connector

conn = mysql.connector.connect(
    host="127.0.0.1",
    user="root",
    password="password"
)
cursor = conn.cursor(dictionary=True)

print("Fetching existing ability IDs from war_world...")
cursor.execute("SELECT Entry, Name FROM war_world.abilities")
existing_abilities = {row['Entry']: row['Name'] for row in cursor.fetchall()}

print("Fetching 1.4.8 Abilities from war_londo...")
cursor.execute("SELECT ID, Name, Casttime, Cooldown, AP, `Range` FROM war_londo.ability")
londo_abilities = cursor.fetchall()

sql_statements = []
print("Generating SQL updates for abilities...")

for ability in londo_abilities:
    ability_id = ability['ID']
    if ability_id not in existing_abilities:
        continue
    
    # We only update if it exists in both
    name = str(ability['Name'] or '').replace('\\', '\\\\').replace("'", "''")

    cast_time = ability['Casttime'] or 0
    cooldown = ability['Cooldown'] or 0
    ap_cost = ability['AP'] or 0
    range_val = ability['Range'] or 0

    # Ensure we don't accidentally put massive ranges if null or odd format
    if range_val > 65000:
        range_val = 0
    
    sql = (f"UPDATE abilities SET "
           f"Name = '{name}', CastTime = {cast_time}, Cooldown = {cooldown}, "
           f"ApCost = {ap_cost}, `Range` = {range_val} "
           f"WHERE Entry = {ability_id};")
    sql_statements.append(sql)

output_file = 'Database/updates/update_148_restore_abilities.sql'
with open(output_file, 'w', encoding='utf-8') as f:
    f.write('-- ProjectWAR Database Restoration (Patch 1.4.8)\\n')
    f.write('-- Domain: Abilities Restoration (Cast Time, Cooldowns, AP Cost, Range)\\n')
    f.write('-- Source: Londos Server v2 1.4.8 Parsed Dump\\n\\n')
    f.write('\\n'.join(sql_statements))

print(f"Generated {len(sql_statements)} SQL update statements in {output_file}")
conn.close()
