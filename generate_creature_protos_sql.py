import mysql.connector

conn = mysql.connector.connect(
    host="127.0.0.1",
    user="root",
    password="password"
)
cursor = conn.cursor(dictionary=True)

print("Restoring Domain 8: Creature Prototype Restoration...")

# Fetch monster data from Londos
cursor.execute("""
    SELECT w.Entry, l.Name, l.Level, l.Scale, l.Faction 
    FROM war_world.creature_protos w
    JOIN war_londo.monster l ON w.Entry = l.ID
""")
creature_updates = cursor.fetchall()

sql_updates = []
for row in creature_updates:
    # Clean name
    name = str(row['Name'] or '').split('^')[0].strip().replace('\\', '\\\\').replace("'", "''")
    level = row['Level'] or 1
    scale = row['Scale'] or 50
    faction = row['Faction'] or 0
    
    sql = (f"UPDATE creature_protos SET "
           f"Name = '{name}', MinLevel = {level}, MaxLevel = {level}, "
           f"Faction = {faction}, MinScale = {scale}, MaxScale = {scale} "
           f"WHERE Entry = {row['Entry']};")
    sql_updates.append(sql)

# Write to file
output_file = 'Database/updates/update_148_restore_creature_protos.sql'
with open(output_file, 'w', encoding='utf-8') as f:
    f.write('-- ProjectWAR Database Restoration (Patch 1.4.8)\n')
    f.write('-- Domain: Creature Prototype Restoration (Level, Scale, Faction, Name)\n')
    f.write('-- Source: Londos Server v2 1.4.8 monster table\n\n')
    f.write('\n'.join(sql_updates))

print(f"Generated {len(sql_updates)} SQL update statements in {output_file}")
conn.close()
