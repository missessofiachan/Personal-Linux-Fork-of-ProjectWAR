-- Apply this after importing war_world if the Land of the Dead flight node appears in the
-- flight-master map but cannot be clicked.
--
-- Root cause:
--   The shipped zone_infos row for zone 191 stores Pairing = 100.
--   The flight-master packet sends zone_infos.Pairing directly to the client, but the client
--   expects the Land of the Dead pairing id (4). With Pairing = 100, the LOTD map node is not
--   treated as a valid clickable destination.

UPDATE zone_infos
SET Pairing = 4
WHERE ZoneId = 191
  AND Pairing <> 4;

SELECT ZoneId, Name, Pairing, Region, OffX, OffY
FROM zone_infos
WHERE ZoneId = 191;
