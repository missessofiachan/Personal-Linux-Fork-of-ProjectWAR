import mysql.connector

conn = mysql.connector.connect(
    host="127.0.0.1",
    user="root",
    password="password"
)
cursor = conn.cursor(dictionary=True)

print("Restoring Domain 10: NPC Equipment (creature_items)...")

# Fetch all monster items from Londos
# We map MonsterID -> Entry, SlotIndex1 -> SlotId, ModelID -> ModelId, Dye1 -> PrimaryColor, Dye2 -> SecondaryColor
cursor.execute("SELECT MonsterID, SlotIndex1, ModelID, Dye1, Dye2 FROM war_londo.monsteritem")
londo_items = cursor.fetchall()

# Fetch existing emulator entries to ensure we are updating/replacing valid NPCs
cursor.execute("SELECT Entry FROM war_world.creature_protos")
existing_npcs = {row['Entry'] for row in cursor.fetchall()}

sql_statements = []
print(f"Processing {len(londo_items)} truth records...")

for item in londo_items:
    npc_id = item['MonsterID']
    slot_id = item['SlotIndex1']
    model_id = item['ModelID']
    dye1 = item['Dye1'] or 0
    dye2 = item['Dye2'] or 0
    
    # Optional: Filter to only NPCs that exist in emulator
    if npc_id not in existing_npcs:
        continue
        
    # We use REPLACE INTO to overwrite existing slot equipment for this NPC
    sql = (f"REPLACE INTO creature_items (Entry, SlotId, ModelId, EffectId, PrimaryColor, SecondaryColor) "
           f"VALUES ({npc_id}, {slot_id}, {model_id}, 0, {dye1}, {dye2});")
    sql_statements.append(sql)

output_file = 'Database/updates/update_148_restore_creature_items.sql'
with open(output_file, 'w', encoding='utf-8') as f:
    f.write('-- ProjectWAR Database Restoration (Patch 1.4.8)\n')
    f.write('-- Domain: NPC Equipment (creature_items)\n')
    f.write('-- Source: Londos Server v2 1.4.8 monsteritem table\n\n')
    f.write('\n'.join(sql_statements))

print(f"Generated {len(sql_statements)} SQL update statements in {output_file}")
conn.close()
