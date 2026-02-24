# Verify binary patch was applied to WAR.exe
$warPath = "C:\Users\Admin\Videos\Warhammer Online - Age of Reckoning\WAR.exe"
$bytes = [System.IO.File]::ReadAllBytes($warPath)

# Patch 1: encryptAddress = (0x00957FBE + 3) - 0x00400000 = 0x00557FC1
$p1 = 0x00557FC1
Write-Host "Patch 1 at 0x$($p1.ToString('X8')): byte = 0x$($bytes[$p1].ToString('X2')) (expected 0x01)"

# Patch 2: decryptAddress1 = 0x009580CB - 0x00400000 = 0x005580CB
$p2 = 0x005580CB
$expected2 = @(0x90, 0x90, 0x90, 0x90, 0x57, 0x8B, 0xF8, 0xEB, 0x32)
$actual2 = $bytes[$p2..($p2+8)]
Write-Host "Patch 2 at 0x$($p2.ToString('X8')): $($actual2 | ForEach-Object { '0x' + $_.ToString('X2') }) (expected: $($expected2 | ForEach-Object { '0x' + $_.ToString('X2') }))"

# Patch 3: decryptAddress2 = 0x0095814B - 0x00400000 = 0x0055814B
$p3 = 0x0055814B
$expected3 = @(0x90, 0x90, 0x90, 0x90, 0xEB, 0x08)
$actual3 = $bytes[$p3..($p3+5)]
Write-Host "Patch 3 at 0x$($p3.ToString('X3')): $($actual3 | ForEach-Object { '0x' + $_.ToString('X2') }) (expected: $($expected3 | ForEach-Object { '0x' + $_.ToString('X2') }))"

# Show bytes around crash offset 0x00556801
$crash = 0x00556801
Write-Host "`nBytes around crash offset 0x$($crash.ToString('X8')):"
$range = $bytes[($crash-8)..($crash+16)]
Write-Host ($range | ForEach-Object { $_.ToString('X2') }) -join ' '
