import mysql.connector

conn = mysql.connector.connect(
    host="127.0.0.1",
    user="root",
    password="password"
)
cursor = conn.cursor(dictionary=True)

# 1. Fetch valid Item Sets in emulator DB
print("Fetching existing item sets from war_world...")
cursor.execute("SELECT Entry FROM war_world.item_sets")
existing_entries = {row['Entry'] for row in cursor.fetchall()}

# 2. Fetch all sets from Londos DB
print("Fetching 1.4.8 Item Sets from war_londo...")
cursor.execute("SELECT ID, Name FROM war_londo.ItemSet")
londo_sets = {row['ID']: str(row['Name'] or '') for row in cursor.fetchall()}

# 3. Fetch Items mapping to ItemSets
print("Fetching Items to build ItemsString...")
cursor.execute("SELECT ID, Name, ItemSetID FROM war_londo.Item WHERE ItemSetID > 0")
items = cursor.fetchall()
set_items_map = {}
for item in items:
    set_id = item['ItemSetID']
    if set_id not in set_items_map:
        set_items_map[set_id] = []
    item_name = str(item['Name'] or '').replace(":", "").replace("|", "")
    set_items_map[set_id].append(f"{item['ID']}:{item_name}")

# 4. Fetch Bonuses to build BonusString
print("Fetching Bonuses to build BonusString...")
cursor.execute("SELECT ItemSetID, AbilityID, BonusType, Value1, Value2, PieceCount FROM war_londo.ItemSetBonus")
bonuses = cursor.fetchall()
set_bonus_map = {}
for bonus in bonuses:
    set_id = bonus['ItemSetID']
    if set_id not in set_bonus_map:
        set_bonus_map[set_id] = []
    
    piece_count = bonus['PieceCount'] or 0
    ability_id = bonus['AbilityID']
    bonus_type = bonus['BonusType']
    value1 = bonus['Value1'] or 0
    value2 = bonus['Value2'] or 0
    
    if ability_id and ability_id > 0:
        bonus_id = 80 + piece_count
        set_bonus_map[set_id].append(f"{bonus_id}:{ability_id}")
    elif bonus_type and bonus_type > 0:
        bonus_id = 30 + piece_count + 2
        set_bonus_map[set_id].append(f"{bonus_id}:{bonus_type},{value1},{value2}")

sql_statements = []
print("Generating SQL updates...")

for set_id in existing_entries:
    if set_id not in londo_sets:
        continue
    
    name = londo_sets[set_id].replace('\\', '\\\\').replace("'", "''").replace('\n', '\\n')
    
    items_list = set_items_map.get(set_id, [])
    items_string = "|".join(items_list) + "|" if items_list else ""
    items_string = items_string.replace('\\', '\\\\').replace("'", "''").replace('\n', '\\n')
    
    bonus_list = set_bonus_map.get(set_id, [])
    bonus_string = "|".join(bonus_list) + "|" if bonus_list else ""
    bonus_string = bonus_string.replace('\\', '\\\\').replace("'", "''").replace('\n', '\\n')
    
    # We only update if we actually have valid items or bonuses from the truth DB
    if items_string or bonus_string:
        sql = (f"UPDATE item_sets SET "
               f"Name = '{name}', ItemsString = '{items_string}', BonusString = '{bonus_string}' "
               f"WHERE Entry = {set_id};")
        sql_statements.append(sql)

output_file = 'Database/updates/update_148_restore_item_sets.sql'
with open(output_file, 'w', encoding='utf-8') as f:
    f.write('-- ProjectWAR Database Restoration (Patch 1.4.8)\\n')
    f.write('-- Domain: Item Sets Restoration (ItemsString, BonusString)\\n')
    f.write('-- Source: Londos Server v2 1.4.8 Parsed Dump\\n\\n')
    f.write('\\n'.join(sql_statements))

print(f"Generated {len(sql_statements)} SQL update statements in {output_file}")
conn.close()
