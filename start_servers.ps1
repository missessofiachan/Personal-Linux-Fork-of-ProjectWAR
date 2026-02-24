$binDir = "C:\Users\Admin\source\repos\Shmerrick\ProjectWAR\bin\Release"

Write-Host "Starting AccountCacher..."
Start-Process -FilePath "$binDir\AccountCacher.exe" -WorkingDirectory $binDir -WindowStyle Minimized
Start-Sleep -Seconds 3

Write-Host "Starting LauncherServer..."
Start-Process -FilePath "$binDir\LauncherServer.exe" -WorkingDirectory $binDir -WindowStyle Minimized
Start-Sleep -Seconds 2

Write-Host "Starting LobbyServer..."
Start-Process -FilePath "$binDir\LobbyServer.exe" -WorkingDirectory $binDir -WindowStyle Minimized
Start-Sleep -Seconds 2

Write-Host "Starting WorldServer..."
Start-Process -FilePath "$binDir\WorldServer.exe" -WorkingDirectory $binDir -WindowStyle Minimized
Start-Sleep -Seconds 3

Write-Host "All servers launched."
Get-Process | Where-Object { $_.Name -match 'AccountCacher|LauncherServer|LobbyServer|WorldServer' } | Select-Object Name, Id | Format-Table -AutoSize
