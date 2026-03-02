import mysql.connector
import json

conn = mysql.connector.connect(
    host="127.0.0.1",
    user="root",
    password="password"
)
cursor = conn.cursor(dictionary=True)

print("Restoring Domain 6: Ability Damage & Heal Components (The Hard Part)...")

# Fetch abilities with MythicComponentData
cursor.execute("SELECT ID, Name, MythicComponentData FROM war_londo.ability WHERE MythicComponentData IS NOT NULL AND MythicComponentData != ''")
londo_abilities = cursor.fetchall()

# Fetch existing emulator components to avoid orphans
cursor.execute("SELECT Entry, `Index` FROM war_world.ability_damage_heals")
existing_components = {(row['Entry'], row['Index']) for row in cursor.fetchall()}

sql_updates = []
print("Parsing MythicComponentData and mapping to ability_damage_heals...")

for ability in londo_abilities:
    ability_id = ability['ID']
    try:
        components = json.loads(ability['MythicComponentData'])
    except:
        continue
        
    for comp in components:
        # Type 1 = Damage, Type 2 = Heal
        if comp.get('Type') not in [1, 2]:
            continue
            
        try:
            data = json.loads(comp.get('Data', '{}'))
            idx = data.get('Index', 0)
            values = data.get('Values', [0])
            base_val = values[0] if values else 0
            
            # Check if this (Entry, Index) exists in emulator
            if (ability_id, idx) in existing_components:
                # We update the MinDamage
                sql = f"UPDATE ability_damage_heals SET MinDamage = {int(base_val)} WHERE Entry = {ability_id} AND `Index` = {idx};"
                sql_updates.append(sql)
        except:
            continue

# Write to file
output_file = 'Database/updates/update_148_restore_ability_values.sql'
with open(output_file, 'w', encoding='utf-8') as f:
    f.write('-- ProjectWAR Database Restoration (Patch 1.4.8)\\n')
    f.write('-- Domain: Ability Damage & Heal Base Values\\n')
    f.write('-- Source: Londos Server v2 1.4.8 MythicComponentData (JSON)\\n\\n')
    f.write('\\n'.join(sql_updates))

print(f"Generated {len(sql_updates)} SQL update statements in {output_file}")
conn.close()
