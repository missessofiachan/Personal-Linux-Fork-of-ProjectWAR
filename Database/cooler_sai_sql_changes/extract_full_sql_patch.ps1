param(
  [Parameter(Mandatory=$true)]
  [string]$CommitHash,
  [string]$OutputPath
)

if (-not $OutputPath) {
  $OutputPath = Join-Path $PSScriptRoot ("sql_patches/manual_{0}.patch" -f $CommitHash.Substring(0,[Math]::Min(8,$CommitHash.Length)))
}

$dir = Split-Path -Parent $OutputPath
if (-not (Test-Path $dir)) {
  New-Item -ItemType Directory -Path $dir | Out-Null
}

git show --pretty=fuller $CommitHash -- '*.sql' | Set-Content -Path $OutputPath -Encoding UTF8
Write-Output "Wrote $OutputPath"
