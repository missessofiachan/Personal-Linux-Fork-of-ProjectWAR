import mysql.connector

# Connect to the local MySQL database
conn = mysql.connector.connect(
    host="127.0.0.1",
    user="root",
    password="password"
)

cursor = conn.cursor(dictionary=True)

# 1. Fetch all items that exist in our target database to only update them
print("Fetching existing item IDs from war_world...")
cursor.execute("SELECT Entry FROM war_world.item_infos")
existing_entries = {row['Entry'] for row in cursor.fetchall()}

# 2. Fetch the corresponding item data from the 1.4.8 Londos database
print("Fetching 1.4.8 Item Truth from war_londo...")
cursor.execute("""
    SELECT ID, Name, Description, ItemTypeID, RaceMask, ModelID, SlotIndex, 
           Rarity, CareerMask, MinLevel, MinRenownLevel, SellPrice, 
           MaxStackCount, TalismanSlotCount, ObjectLevel, UniqueEquipped 
    FROM war_londo.Item
""")
londo_items = cursor.fetchall()

# 3. Fetch all stats from ItemStatistic and group them by ItemID
print("Fetching Item Statistics from war_londo...")
cursor.execute("SELECT ItemID, BonusTypeID, Value FROM war_londo.ItemStatistic")
stats_rows = cursor.fetchall()

# Group stats by item
item_stats_map = {}
for row in stats_rows:
    item_id = row['ItemID']
    if item_id not in item_stats_map:
        item_stats_map[item_id] = []
    
    # We only take up to 12 stats per the emulator's limitations
    if len(item_stats_map[item_id]) < 12:
        item_stats_map[item_id].append(f"{int(row['BonusTypeID'])}:{int(row['Value'])}")

sql_statements = []

print("Generating SQL updates...")
for item in londo_items:
    item_id = item['ID']
    if item_id not in existing_entries:
        continue # Skip items not currently in the emulator database

    # Format the stats string
    stats_list = item_stats_map.get(item_id, [])
    # Pad to 12 slots with 0:0
    while len(stats_list) < 12:
        stats_list.append("0:0")
    
    stats_string = ";".join(stats_list) + ";"
    
    # Clean strings
    name = str(item['Name'] or '').replace('\\', '\\\\').replace("'", "''").replace('\n', '\\n').replace('\r', '\\r')
    desc = str(item['Description'] or '').replace('\\', '\\\\').replace("'", "''").replace('\n', '\\n').replace('\r', '\\r')
    
    # Map fields, handle NULLs
    type_id = item['ItemTypeID'] or 0
    race_mask = item['RaceMask'] or 0
    model_id = item['ModelID'] or 0
    slot_id = item['SlotIndex'] or 0
    rarity = item['Rarity'] or 0
    career_mask = item['CareerMask'] or 0
    min_level = item['MinLevel'] or 0
    min_renown = item['MinRenownLevel'] or 0
    sell_price = item['SellPrice'] or 0
    max_stack = item['MaxStackCount'] or 1
    tali_slots = item['TalismanSlotCount'] or 0
    obj_level = item['ObjectLevel'] or 0
    unique_eq = item['UniqueEquipped'] or 0

    sql = (f"UPDATE item_infos SET "
           f"Name = '{name}', Description = '{desc}', Type = {type_id}, "
           f"Race = {race_mask}, ModelId = {model_id}, SlotId = {slot_id}, "
           f"Rarity = {rarity}, Career = {career_mask}, MinRank = {min_level}, "
           f"MinRenown = {min_renown}, SellPrice = {sell_price}, MaxStack = {max_stack}, "
           f"TalismanSlots = {tali_slots}, ObjectLevel = {obj_level}, "
           f"UniqueEquiped = {unique_eq}, Stats = '{stats_string}' "
           f"WHERE Entry = {item_id};")
    
    sql_statements.append(sql)

# Write to file
output_file = 'Database/updates/update_148_restore_item_stats.sql'
with open(output_file, 'w', encoding='utf-8') as f:
    f.write('-- ProjectWAR Database Restoration (Patch 1.4.8)\\n')
    f.write('-- Domain: Item Restoration (Stats, Requirements, Prices)\\n')
    f.write('-- Source: Londos Server v2 1.4.8 Parsed Dump\\n\\n')
    f.write('\\n'.join(sql_statements))

print(f"Generated {len(sql_statements)} SQL update statements in {output_file}")

conn.close()
