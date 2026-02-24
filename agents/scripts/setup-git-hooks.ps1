param()

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path ".").Path
$hooksPath = Join-Path $repoRoot ".githooks"

if (-not (Test-Path $hooksPath)) {
    New-Item -ItemType Directory -Path $hooksPath | Out-Null
}

git config core.hooksPath ".githooks"

Write-Host "Configured git hooks path to .githooks"
Write-Host "Active guard: .githooks/pre-push -> agents/scripts/pre-push-guard.ps1"
