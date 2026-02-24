param(
    [Parameter(Mandatory = $true)]
    [string]$TaskId
)

$ErrorActionPreference = "Stop"

function Test-RegexInFile {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,
        [Parameter(Mandatory = $true)]
        [string]$Pattern
    )

    if (-not (Test-Path $Path)) {
        return $false
    }

    $content = Get-Content $Path -Raw
    return [regex]::IsMatch($content, $Pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
}

$taskDir = Join-Path "agents/tasks" $TaskId
$submissionsDir = Join-Path $taskDir "submissions"
$adminDir = Join-Path $taskDir "admin"

if (-not (Test-Path $taskDir)) {
    throw "Task folder '$taskDir' was not found."
}

$requiredCoderFiles = @(
    (Join-Path $submissionsDir "khorne.md")
    (Join-Path $submissionsDir "tzeentch.md")
    (Join-Path $submissionsDir "slaanesh.md")
    (Join-Path $submissionsDir "nurgle.md")
)

$claudeFile = Join-Path $submissionsDir "claude.md"
$waiverFile = Join-Path $taskDir "waiver.md"
$qaFile = Join-Path $adminDir "qa-review.md"
$releaseFile = Join-Path $adminDir "release-signoff.md"

$failures = @()

foreach ($file in $requiredCoderFiles) {
    if (-not (Test-RegexInFile -Path $file -Pattern 'Status:\s*`?Complete`?')) {
        $failures += "Coder submission not complete: $file"
    }
}

$claudeComplete = Test-RegexInFile -Path $claudeFile -Pattern 'Status:\s*`?Complete`?'
$claudeWaived = Test-RegexInFile -Path $waiverFile -Pattern 'Claude Waived:\s*`?true`?'

if (-not $claudeComplete -and -not $claudeWaived) {
    $failures += "Claude gate failed: no complete submission and no recorded waiver."
}

if (-not (Test-RegexInFile -Path $qaFile -Pattern 'Gate Status:\s*`?PASS`?')) {
    $failures += "QA gate failed: '$qaFile' does not contain Gate Status PASS."
}

if (-not (Test-RegexInFile -Path $releaseFile -Pattern 'Status:\s*`?APPROVED`?')) {
    $failures += "Release gate failed: '$releaseFile' does not contain Status APPROVED."
}

Write-Host "Pre-merge gate check for task: $TaskId"
Write-Host ""

if ($failures.Count -eq 0) {
    Write-Host "Result: PASS"
    exit 0
}

Write-Host "Result: FAIL"
Write-Host ""
Write-Host "Blocking issues:"
$failures | ForEach-Object { Write-Host "  - $_" }
exit 1
