# Compile Fix & Login Diagnosis — 2026-03-29

## Scope
Follow-up session addressing one compile error in `MovementInterface.cs` and
diagnosing a login failure that turned out to be an antivirus interference issue.

---

## Fixed

### 1. CS1061 compile error — `Zone_Info` has no `CalculPin` (`MovementInterface.cs`)

`destZone.CalculPin(newWorldPosX, true)` and `destZone.CalculPin(newWorldPosY, false)`
were called where `destZone` is `Zone_Info`. `CalculPin` is a static utility on
`ZoneService`, not an instance method on `Zone_Info`.

**Fix:**
```csharp
// Before (CS1061):
ushort pinX = destZone.CalculPin(newWorldPosX, true);
ushort pinY = destZone.CalculPin(newWorldPosY, false);

// After:
ushort pinX = ZoneService.CalculPin(destZone, (int)newWorldPosX, true);
ushort pinY = ZoneService.CalculPin(destZone, (int)newWorldPosY, false);
```

Added `using WorldServer.Services.World;` to the file's imports.

**Files changed:**
- `WorldServer/World/Interfaces/MovementInterface.cs`

---

## Diagnosed (not a code bug)

### 2. Login failure — "Invalid User / Pass" for all accounts

**Symptom:** All accounts returned `LOGIN_INVALID_USERNAME_PASSWORD` regardless of
correct credentials.

**Root cause:** Antivirus was flagging and interfering with server processes
(AccountCacher / BCrypt operations), breaking the auth pipeline at runtime.

**Resolution:** Add server executables and their output directories to AV exclusion
list:
- `AccountCacher.exe` and its output folder
- `LauncherServer.exe` and its output folder
- `WorldServer.exe` and its output folder
- `BCrypt.Net-Next.dll` if targeted directly

**Auth code is correct.** The full pipeline was verified:
- New accounts: `BCrypt(SHA256(username_lower:password_lower))` stored, verified correctly.
- Old pre-c16044ff accounts (`BCrypt(plaintext)` + `Password=plaintext`): auto-migrated
  via `CheckPendingPassword` on first login after upgrade.
- Master-branch SHA256-only accounts: matched via legacy string-equals path then
  upgraded to BCrypt.
- If AV is not the issue and accounts still fail, use the AccountCacher console
  `reset <username> <plaintext_password>` to force-write a correct BCrypt hash.

---

## Build Status
`WorldServer.csproj` builds cleanly — zero errors, zero warnings.
