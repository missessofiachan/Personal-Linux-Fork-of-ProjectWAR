import mysql.connector

conn = mysql.connector.connect(
    host="127.0.0.1",
    user="root",
    password="password"
)
cursor = conn.cursor(dictionary=True)

print("Restoring Domain 7: Ability Visual Effects & Name Cleaning...")

# Fetch EffectID from Londos ability table
cursor.execute("""
    SELECT w.Entry, l.Name, l.EffectID 
    FROM war_world.abilities w
    JOIN war_londo.ability l ON w.Entry = l.ID
""")
ability_updates = cursor.fetchall()

sql_updates = []
for row in ability_updates:
    # Clean name (remove ^M, ^f, etc.)
    name = str(row['Name'] or '').split('^')[0].strip().replace('\\', '\\\\').replace("'", "''")
    
    effect_id = row['EffectID'] or 0
    
    sql = f"UPDATE abilities SET Name = '{name}', EffectID = {effect_id} WHERE Entry = {row['Entry']};"
    sql_updates.append(sql)

# Write to file
output_file = 'Database/updates/update_148_restore_ability_visuals.sql'
with open(output_file, 'w', encoding='utf-8') as f:
    f.write('-- ProjectWAR Database Restoration (Patch 1.4.8)\n')
    f.write('-- Domain: Ability Visual Effects (EffectID) and Name Cleaning\n')
    f.write('-- Source: Londos Server v2 1.4.8 ability table\n\n')
    f.write('\n'.join(sql_updates))

print(f"Generated {len(sql_updates)} SQL update statements in {output_file}")
conn.close()
