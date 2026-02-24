param(
    [Parameter(Mandatory = $true)]
    [string]$TaskId,

    [string]$BaseBranch = "Restart",

    [switch]$SkipClaude,

    [switch]$SkipConsolidator
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($TaskId)) {
    throw "TaskId cannot be empty."
}

$agents = @("khorne", "tzeentch", "slaanesh", "nurgle")

if (-not $SkipClaude) {
    $agents += "claude"
}

if (-not $SkipConsolidator) {
    $agents += "consolidator"
}

git rev-parse --verify $BaseBranch 1>$null 2>$null
if ($LASTEXITCODE -ne 0) {
    throw "Base branch '$BaseBranch' was not found."
}

$created = @()
$alreadyExists = @()

foreach ($agent in $agents) {
    $branchName = "agent/$TaskId/$agent"

    git show-ref --verify --quiet "refs/heads/$branchName"
    if ($LASTEXITCODE -eq 0) {
        $alreadyExists += $branchName
        continue
    }

    git branch $branchName $BaseBranch 1>$null
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to create branch '$branchName'."
    }

    $created += $branchName
}

Write-Host "Base branch: $BaseBranch"
Write-Host ""
Write-Host "Created branches:"
if ($created.Count -eq 0) {
    Write-Host "  (none)"
}
else {
    $created | ForEach-Object { Write-Host "  $_" }
}

Write-Host ""
Write-Host "Already existing branches:"
if ($alreadyExists.Count -eq 0) {
    Write-Host "  (none)"
}
else {
    $alreadyExists | ForEach-Object { Write-Host "  $_" }
}
