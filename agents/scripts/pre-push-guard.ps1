param(
    [string]$RemoteName,
    [string]$RemoteUrl
)

$ErrorActionPreference = "Stop"

$protectedRefs = @(
    "refs/heads/Restart",
    "refs/heads/master",
    "refs/heads/main"
)

function Is-AllZeroSha {
    param([string]$Sha)
    return $Sha -match '^[0]+$'
}

function Get-FirstParentCommitsInRange {
    param(
        [Parameter(Mandatory = $true)]
        [string]$LocalSha,
        [Parameter(Mandatory = $true)]
        [string]$RemoteSha
    )

    if (Is-AllZeroSha $RemoteSha) {
        # New protected branch push: inspect local first-parent history.
        return git rev-list --first-parent $LocalSha
    }

    return git rev-list --first-parent "$RemoteSha..$LocalSha"
}

function Commit-TouchesNonWorkflowFiles {
    param(
        [Parameter(Mandatory = $true)]
        [string]$CommitSha
    )

    $changedFiles = git diff-tree --no-commit-id --name-only -r $CommitSha
    if (-not $changedFiles) {
        return $false
    }

    foreach ($file in $changedFiles) {
        if ($file -notmatch '^(agents/|AGENTS\.md$|\.githooks/)') {
            return $true
        }
    }

    return $false
}

function Commit-MessageHasRequiredToken {
    param(
        [Parameter(Mandatory = $true)]
        [string]$CommitSha
    )

    $subject = git log -1 --pretty=%s $CommitSha
    $body = git log -1 --pretty=%b $CommitSha
    $fullMessage = "$subject`n$body"

    if ($fullMessage -match 'TASK-\d{4}' -or $fullMessage -match 'DIRECTOR-OVERRIDE') {
        return $true
    }

    return $false
}

$updates = @()
while (($line = [Console]::In.ReadLine()) -ne $null) {
    if ($line.Trim().Length -gt 0) {
        $updates += $line
    }
}

if ($updates.Count -eq 0) {
    exit 0
}

$violations = @()

foreach ($update in $updates) {
    $parts = $update -split ' '
    if ($parts.Count -lt 4) {
        continue
    }

    $localRef = $parts[0]
    $localSha = $parts[1]
    $remoteRef = $parts[2]
    $remoteSha = $parts[3]

    if ($protectedRefs -notcontains $remoteRef) {
        continue
    }

    # Branch deletion or null update.
    if (Is-AllZeroSha $localSha) {
        continue
    }

    $commits = Get-FirstParentCommitsInRange -LocalSha $localSha -RemoteSha $remoteSha
    if (-not $commits) {
        continue
    }

    foreach ($commit in $commits) {
        if (-not (Commit-TouchesNonWorkflowFiles -CommitSha $commit)) {
            continue
        }

        if (-not (Commit-MessageHasRequiredToken -CommitSha $commit)) {
            $subject = git log -1 --pretty=%s $commit
            $violations += "Protected push policy failure on ${remoteRef}: commit $commit missing TASK-#### or DIRECTOR-OVERRIDE token. Subject: $subject"
        }
    }
}

if ($violations.Count -eq 0) {
    exit 0
}

Write-Host ""
Write-Host "Push blocked by ProjectWAR protected-branch workflow policy." -ForegroundColor Red
Write-Host "For code changes on Restart/master/main, include TASK-#### in commit messages." -ForegroundColor Red
Write-Host "For explicit emergency authorization, include DIRECTOR-OVERRIDE." -ForegroundColor Red
Write-Host ""
$violations | ForEach-Object { Write-Host " - $_" -ForegroundColor Red }
Write-Host ""
Write-Host "Fix and retry push." -ForegroundColor Red
exit 1
