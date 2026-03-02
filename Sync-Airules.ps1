$sourceFile = "AI_Documentation\.cursorrules"
$targetFiles = @("AI_Documentation\.ai-rules", "AI_Documentation\CLAUDE.md", "AI_Documentation\openai-instructions.md")

if (-not (Test-Path $sourceFile)) {
    Write-Host "Source file $sourceFile not found!"
    exit 1
}

$content = Get-Content $sourceFile -Raw

foreach ($file in $targetFiles) {
    Set-Content -Path $file -Value $content
    Write-Host "Synced $file with $sourceFile"
}

Write-Host "All agent rule files have been successfully synchronized."
