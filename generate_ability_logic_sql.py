import mysql.connector

conn = mysql.connector.connect(
    host="127.0.0.1",
    user="root",
    password="password"
)
cursor = conn.cursor(dictionary=True)

# Domain 5: Advanced Ability Logic Restoration
print("Restoring Domain 5: Advanced Ability Logic...")

# 1. Update Ranges in 'abilities' table
print("Updating MinRange and Range in abilities...")
cursor.execute("""
    SELECT w.Entry, l.RangeMin, l.RangeMax 
    FROM war_world.abilities w
    JOIN war_londo.abilitybin l ON w.Entry = l.ID
""")
range_updates = cursor.fetchall()

sql_ranges = []
for row in range_updates:
    min_r = row['RangeMin'] or 0
    max_r = row['RangeMax'] or 0
    # Cap values to table constraints
    if min_r > 255: min_r = 255
    if max_r > 65535: max_r = 65535
    
    sql_ranges.append(f"UPDATE abilities SET MinRange = {min_r}, `Range` = {max_r} WHERE Entry = {row['Entry']};")

# 2. Update NoCrits in 'ability_damage_heals' table
print("Updating NoCrits in ability_damage_heals...")
cursor.execute("""
    SELECT w.Entry, l.CantCrit 
    FROM war_world.abilities w
    JOIN war_londo.ability l ON w.Entry = l.ID
    WHERE l.CantCrit = 1
""")
crit_updates = cursor.fetchall()

sql_crits = []
for row in crit_updates:
    sql_crits.append(f"UPDATE ability_damage_heals SET NoCrits = 1 WHERE Entry = {row['Entry']};")

# Write to file
output_file = 'Database/updates/update_148_restore_ability_logic.sql'
with open(output_file, 'w', encoding='utf-8') as f:
    f.write('-- ProjectWAR Database Restoration (Patch 1.4.8)\\n')
    f.write('-- Domain: Advanced Ability Logic (Ranges, Crit Constraints)\\n')
    f.write('-- Source: Londos Server v2 1.4.8 Parsed Dump\\n\\n')
    f.write('\\n'.join(sql_ranges))
    f.write('\\n\\n')
    f.write('\\n'.join(sql_crits))

print(f"Generated {len(sql_ranges) + len(sql_crits)} SQL update statements in {output_file}")
conn.close()
