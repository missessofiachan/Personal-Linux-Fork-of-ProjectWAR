$file = "C:\Users\Admin\Videos\Warhammer Online - Age of Reckoning\data.myp"
$info = Get-Item $file
Write-Host "Size: $($info.Length) bytes"
Write-Host "LastModified: $($info.LastWriteTime)"

# Compare with backup in bin/Release
$server = "C:\Users\Admin\source\repos\Shmerrick\ProjectWAR\bin\Release\data.myp"
if (Test-Path $server) {
    $serverInfo = Get-Item $server
    Write-Host "Server data.myp: $($serverInfo.Length) bytes, modified $($serverInfo.LastWriteTime)"
}

# First 4 bytes should be MYP magic
$bytes = [System.IO.File]::ReadAllBytes($file)
$magic = [System.Text.Encoding]::ASCII.GetString($bytes[0..3])
Write-Host "MYP magic: $magic"
