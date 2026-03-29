-- Repairs persisted bot appearance data for previously created BotAccount characters.
-- This backfills model ids, sex, and non-zero trait bytes so legacy bots render correctly.

UPDATE characters
SET
    Sex = CASE CareerLine
        WHEN 22 THEN 1
        ELSE 0
    END,
    ModelId = CASE CareerLine
        WHEN 1 THEN 16
        WHEN 2 THEN 18
        WHEN 3 THEN 22
        WHEN 4 THEN 20
        WHEN 5 THEN 12
        WHEN 6 THEN 13
        WHEN 7 THEN 14
        WHEN 8 THEN 15
        WHEN 9 THEN 34
        WHEN 10 THEN 30
        WHEN 11 THEN 32
        WHEN 12 THEN 36
        WHEN 13 THEN 24
        WHEN 14 THEN 25
        WHEN 15 THEN 26
        WHEN 16 THEN 28
        WHEN 17 THEN 48
        WHEN 18 THEN 50
        WHEN 19 THEN 46
        WHEN 20 THEN 44
        WHEN 21 THEN 39
        WHEN 22 THEN 43
        WHEN 23 THEN 38
        WHEN 24 THEN 41
        ELSE ModelId
    END,
    Traits = CASE
        WHEN CareerLine IN (1, 2, 3, 4) THEN UNHEX('0502060205050300')
        WHEN CareerLine IN (13, 14, 15, 16) THEN UNHEX('080D050606020300')
        ELSE UNHEX('0101010101010100')
    END
WHERE Name LIKE 'Bot_%'
  AND AccountId IN (
      SELECT AccountId
      FROM war_accounts.accounts
      WHERE LOWER(Username) = 'botaccount'
  );
