import mysql.connector
import math

conn = mysql.connector.connect(
    host="127.0.0.1",
    user="root",
    password="password"
)
cursor = conn.cursor(dictionary=True)

print("Restoring Domain 14: Zone Respawn Points (with Offsets)...")

# Fetch zone offsets
cursor.execute("SELECT ZoneId, OffX, OffY FROM war_world.zone_infos")
offsets = {row['ZoneId']: (row['OffX'], row['OffY']) for row in cursor.fetchall()}

# Fetch truth respawns
cursor.execute("SELECT ID, ZoneID, X, Y, Z, Heading, Realm FROM war_londo.zonespawnpoint")
truth_respawns = cursor.fetchall()

sql_statements = []
print(f"Processing {len(truth_respawns)} truth respawn points...")

for tr in truth_respawns:
    rid = tr['ID']
    zid = tr['ZoneID']
    tx_world = int(tr['X'])
    ty_world = int(tr['Y'])
    tz_world = int(tr['Z'])
    th = int(tr['Heading'] * 4096 / (2 * math.pi))
    realm = tr['Realm']
    
    if zid not in offsets:
        print(f"Warning: Zone {zid} not found in offsets. Skipping ID {rid}.")
        continue
        
    off_x, off_y = offsets[zid]
    
    # Calculate relative pins
    pin_x = tx_world - (off_x * 4096)
    pin_y = ty_world - (off_y * 4096)
    pin_z = tz_world # Z usually doesn't have a large offset in the same way, or it's just raw
    
    # Safety check for smallint range
    if not (0 <= pin_x <= 65535) or not (0 <= pin_y <= 65535):
        # Some zones might have different coordinate systems (like cities or instances)
        # We'll use raw values if they fit, or log the error
        pass

    sql = (f"REPLACE INTO zone_respawns (RespawnID, ZoneID, Realm, PinX, PinY, PinZ, WorldO, InZoneID) "
           f"VALUES ({rid}, {zid}, {realm}, {pin_x}, {pin_y}, {pin_z}, {th}, {zid});")
    sql_statements.append(sql)

output_file = 'Database/updates/update_148_restore_zone_respawns.sql'
with open(output_file, 'w', encoding='utf-8') as f:
    f.write('-- ProjectWAR Database Restoration (Patch 1.4.8)\n')
    f.write('-- Domain: Zone Respawn Points\n')
    f.write('-- Source: Londos Server v2 1.4.8 zonespawnpoint table with Zone Offsets\n\n')
    f.write('\n'.join(sql_statements))

print(f"Generated {len(sql_statements)} SQL update statements in {output_file}")
conn.close()
