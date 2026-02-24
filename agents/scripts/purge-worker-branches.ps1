param(
    [Parameter(Mandatory = $true)]
    [string]$TaskId,

    [string]$BaseBranch = "Restart",

    [switch]$DeleteRemote
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($TaskId)) {
    throw "TaskId cannot be empty."
}

$consolidatorBranch = "agent/$TaskId/consolidator"
$workerBranches = @(
    "agent/$TaskId/khorne",
    "agent/$TaskId/tzeentch",
    "agent/$TaskId/slaanesh",
    "agent/$TaskId/nurgle",
    "agent/$TaskId/claude"
)

git rev-parse --verify $BaseBranch 1>$null 2>$null
if ($LASTEXITCODE -ne 0) {
    throw "Base branch '$BaseBranch' was not found."
}

git rev-parse --verify $consolidatorBranch 1>$null 2>$null
if ($LASTEXITCODE -ne 0) {
    throw "Consolidator branch '$consolidatorBranch' was not found locally."
}

git merge-base --is-ancestor $consolidatorBranch $BaseBranch
if ($LASTEXITCODE -ne 0) {
    throw "Consolidator branch '$consolidatorBranch' is not merged into '$BaseBranch'. Purge blocked."
}

$currentBranch = (git rev-parse --abbrev-ref HEAD).Trim()
$deletedLocal = @()
$missingLocal = @()

foreach ($branch in $workerBranches) {
    if ($branch -eq $currentBranch) {
        throw "Cannot delete current branch '$branch'. Switch branches and retry."
    }

    git show-ref --verify --quiet "refs/heads/$branch"
    if ($LASTEXITCODE -ne 0) {
        $missingLocal += $branch
        continue
    }

    git branch -D $branch 1>$null
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to delete local branch '$branch'."
    }

    $deletedLocal += $branch
}

Write-Host "Base branch: $BaseBranch"
Write-Host "Consolidator branch: $consolidatorBranch (merged)"
Write-Host ""
Write-Host "Deleted local worker branches:"
if ($deletedLocal.Count -eq 0) {
    Write-Host "  (none)"
}
else {
    $deletedLocal | ForEach-Object { Write-Host "  $_" }
}

Write-Host ""
Write-Host "Missing local worker branches:"
if ($missingLocal.Count -eq 0) {
    Write-Host "  (none)"
}
else {
    $missingLocal | ForEach-Object { Write-Host "  $_" }
}

if ($DeleteRemote) {
    git remote get-url origin 1>$null 2>$null
    if ($LASTEXITCODE -ne 0) {
        throw "Remote 'origin' is not configured. Cannot delete remote branches."
    }

    Write-Host ""
    Write-Host "Deleting remote worker branches from origin..."
    git push origin --delete @workerBranches
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to delete one or more remote worker branches."
    }
}
