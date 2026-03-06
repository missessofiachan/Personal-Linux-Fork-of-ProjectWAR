param(
    [string]$CsvPath = 'Database\ability_parity\csv\gamedata\abilities.csv',
    [string]$MySqlExePath = 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe',
    [string]$MySqlHost = '127.0.0.1',
    [int]$MySqlPort = 3306,
    [string]$MySqlUser = 'root',
    [string]$MySqlPassword = 'password',
    [string]$WorldSchema = 'war_world_curated',
    [string]$SchemaSqlPath = 'Database\ability_parity\ability_entry_resolver.schema.sql',
    [string]$OutputSql = 'Database\ability_parity\work\ability_entry_resolver.generated.sql',
    [switch]$Apply
)

Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'

function Resolve-ProjectPath {
    param([Parameter(Mandatory = $true)][string]$PathValue)

    if ([string]::IsNullOrWhiteSpace($PathValue))
        { throw "Path cannot be empty." }

    if ([System.IO.Path]::IsPathRooted($PathValue))
        { return [System.IO.Path]::GetFullPath($PathValue) }

    return [System.IO.Path]::GetFullPath((Join-Path (Get-Location).Path $PathValue))
}

function Invoke-MySqlQuery {
    param(
        [Parameter(Mandatory = $true)][string]$ExecutablePath,
        [Parameter(Mandatory = $true)][string]$Server,
        [Parameter(Mandatory = $true)][int]$Port,
        [Parameter(Mandatory = $true)][string]$User,
        [Parameter(Mandatory = $true)][string]$Password,
        [Parameter(Mandatory = $true)][string]$Query,
        [switch]$TabSeparated
    )

    $args = @(
        "--host=$Server",
        "--port=$Port",
        "--user=$User"
    )

    if ($TabSeparated)
    {
        $args += @('--batch', '--raw', '--skip-column-names')
    }

    $args += @('--execute', $Query)

    $oldMySqlPwd = $env:MYSQL_PWD
    $env:MYSQL_PWD = $Password
    try
    {
        $result = & $ExecutablePath @args 2>&1
    }
    finally
    {
        if ($null -eq $oldMySqlPwd)
        {
            Remove-Item Env:MYSQL_PWD -ErrorAction SilentlyContinue
        }
        else
        {
            $env:MYSQL_PWD = $oldMySqlPwd
        }
    }
    if ($LASTEXITCODE -ne 0)
    {
        throw "mysql query failed: $Query`n$result"
    }

    return $result
}

function Invoke-MySqlFile {
    param(
        [Parameter(Mandatory = $true)][string]$ExecutablePath,
        [Parameter(Mandatory = $true)][string]$Server,
        [Parameter(Mandatory = $true)][int]$Port,
        [Parameter(Mandatory = $true)][string]$User,
        [Parameter(Mandatory = $true)][string]$Password,
        [Parameter(Mandatory = $true)][string]$FilePath
    )

    $command = "set MYSQL_PWD=$Password&&`"$ExecutablePath`" --host=$Server --port=$Port --user=$User < `"$FilePath`""
    $result = cmd /c $command 2>&1
    if ($LASTEXITCODE -ne 0)
    {
        throw "mysql file import failed: $FilePath`n$result"
    }
}

function SqlString {
    param([AllowNull()][string]$Value)

    if ($null -eq $Value)
        { return 'NULL' }

    return "'" + $Value.Replace("'", "''") + "'"
}

$resolvedCsvPath = Resolve-ProjectPath -PathValue $CsvPath
$resolvedMySqlExe = Resolve-ProjectPath -PathValue $MySqlExePath
$resolvedSchemaSqlPath = Resolve-ProjectPath -PathValue $SchemaSqlPath
$resolvedOutputSql = Resolve-ProjectPath -PathValue $OutputSql

if (-not (Test-Path -LiteralPath $resolvedMySqlExe))
    { throw "mysql executable not found: $resolvedMySqlExe" }
if (-not (Test-Path -LiteralPath $resolvedCsvPath))
    { throw "abilities.csv not found: $resolvedCsvPath" }
if (-not (Test-Path -LiteralPath $resolvedSchemaSqlPath))
    { throw "schema SQL not found: $resolvedSchemaSqlPath" }

Add-Type -AssemblyName Microsoft.VisualBasic

$csvEffects = New-Object 'System.Collections.Generic.Dictionary[int,int]'
$parser = New-Object Microsoft.VisualBasic.FileIO.TextFieldParser($resolvedCsvPath)
$parser.SetDelimiters(',')
$parser.HasFieldsEnclosedInQuotes = $true

# Skip metadata and header rows.
[void]$parser.ReadFields()
[void]$parser.ReadFields()

while (-not $parser.EndOfData)
{
    $row = $parser.ReadFields()
    if ($null -eq $row -or $row.Length -lt 9)
        { continue }

    $idText = if ($row.Length -gt 0 -and $null -ne $row[0]) { [string]$row[0] } else { '' }
    $idText = $idText.Trim()
    if ($idText -notmatch '^\d+$')
        { continue }

    $effectText = if ($row.Length -gt 8 -and $null -ne $row[8]) { [string]$row[8] } else { '' }
    $effectText = $effectText.Trim()
    $effectValue = 0
    if ($effectText -match '^-?\d+(\.\d+)?$')
    {
        $effectValue = [int][double]$effectText
    }

    $csvEffects[[int]$idText] = $effectValue
}

$parser.Close()

$worldAbilityEntries = New-Object 'System.Collections.Generic.HashSet[int]'
Invoke-MySqlQuery `
    -ExecutablePath $resolvedMySqlExe `
    -Server $MySqlHost `
    -Port $MySqlPort `
    -User $MySqlUser `
    -Password $MySqlPassword `
    -Query "SELECT Entry FROM $WorldSchema.abilities;" `
    -TabSeparated | ForEach-Object {
        if ($_ -match '^\d+$')
        {
            [void]$worldAbilityEntries.Add([int]$_)
        }
    }

$worldBuffEntries = New-Object 'System.Collections.Generic.HashSet[int]'
Invoke-MySqlQuery `
    -ExecutablePath $resolvedMySqlExe `
    -Server $MySqlHost `
    -Port $MySqlPort `
    -User $MySqlUser `
    -Password $MySqlPassword `
    -Query "SELECT Entry FROM $WorldSchema.buff_infos;" `
    -TabSeparated | ForEach-Object {
        if ($_ -match '^\d+$')
        {
            [void]$worldBuffEntries.Add([int]$_)
        }
    }

$worldCommandEntries = New-Object 'System.Collections.Generic.HashSet[int]'
Invoke-MySqlQuery `
    -ExecutablePath $resolvedMySqlExe `
    -Server $MySqlHost `
    -Port $MySqlPort `
    -User $MySqlUser `
    -Password $MySqlPassword `
    -Query "SELECT DISTINCT Entry FROM $WorldSchema.ability_commands;" `
    -TabSeparated | ForEach-Object {
        if ($_ -match '^\d+$')
        {
            [void]$worldCommandEntries.Add([int]$_)
        }
    }

$stats = [ordered]@{
    csv_rows = 0
    invalid_csv_effect = 0
    source_has_commands = 0
    target_has_commands = 0
    target_missing_commands = 0
    not_in_world = 0
    out_of_range = 0
    selected = 0
}

$resolverRows = @{}

foreach ($csvRow in $csvEffects.GetEnumerator())
{
    $stats.csv_rows++
    $sourceEntry = [int]$csvRow.Key
    $csvEffectEntry = [int]$csvRow.Value

    if ($csvEffectEntry -le 0 -or $csvEffectEntry -eq $sourceEntry)
    {
        $stats.invalid_csv_effect++
        continue
    }

    if ($sourceEntry -gt [UInt16]::MaxValue -or $csvEffectEntry -gt [UInt16]::MaxValue)
    {
        $stats.out_of_range++
        continue
    }

    if (-not $worldAbilityEntries.Contains($sourceEntry) -or -not $worldAbilityEntries.Contains($csvEffectEntry))
    {
        $stats.not_in_world++
        continue
    }

    if ($worldCommandEntries.Contains($sourceEntry))
    {
        $stats.source_has_commands++
        continue
    }

    $targetHasCommands = $worldCommandEntries.Contains($csvEffectEntry)
    if ($targetHasCommands)
    {
        $stats.target_has_commands++
    }
    else
    {
        $stats.target_missing_commands++
    }

    $canonicalBuffEntry = if ($worldBuffEntries.Contains($csvEffectEntry)) { $csvEffectEntry } else { 0 }
    $targetCommandFlag = if ($targetHasCommands) { 1 } else { 0 }
    $notes = "csv_effect=$csvEffectEntry;source_has_world_commands=0;target_has_world_commands=$targetCommandFlag"

    $resolverRows[$sourceEntry] = [pscustomobject]@{
        SourceEntry = [UInt16]$sourceEntry
        CanonicalAbilityEntry = [UInt16]$csvEffectEntry
        CanonicalBuffEntry = [UInt16]$canonicalBuffEntry
        ResolutionSource = 'csv_effect'
        Enabled = 1
        Notes = $notes
    }
}

$orderedRows = @(
    $resolverRows.GetEnumerator() `
        | Sort-Object -Property Name `
        | ForEach-Object { $_.Value }
)

$stats.selected = $orderedRows.Count

$schemaSql = Get-Content -LiteralPath $resolvedSchemaSqlPath -Raw
$sb = New-Object System.Text.StringBuilder

[void]$sb.AppendLine('-- Generated by Database/ability_parity/Build-AbilityEntryResolver.ps1')
[void]$sb.AppendLine("-- Source CSV: $resolvedCsvPath")
[void]$sb.AppendLine("-- Selected mappings: $($stats.selected)")
[void]$sb.AppendLine()
[void]$sb.AppendLine("USE $WorldSchema;")
[void]$sb.AppendLine()
[void]$sb.AppendLine($schemaSql.TrimEnd())
[void]$sb.AppendLine()
[void]$sb.AppendLine("DELETE FROM ability_entry_resolver WHERE ResolutionSource = 'csv_effect';")

if ($orderedRows.Count -gt 0)
{
    [void]$sb.AppendLine('INSERT INTO ability_entry_resolver (SourceEntry,CanonicalAbilityEntry,CanonicalBuffEntry,ResolutionSource,Enabled,Notes) VALUES')
    for ($i = 0; $i -lt $orderedRows.Count; $i++)
    {
        $row = $orderedRows[$i]
        $suffix = if ($i -lt $orderedRows.Count - 1) { ',' } else { ';' }
        [void]$sb.AppendLine(
            "($($row.SourceEntry),$($row.CanonicalAbilityEntry),$($row.CanonicalBuffEntry),$(SqlString $row.ResolutionSource),$($row.Enabled),$(SqlString $row.Notes))$suffix"
        )
    }
}

$outputDir = Split-Path -Path $resolvedOutputSql -Parent
if (-not [string]::IsNullOrWhiteSpace($outputDir))
{
    New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
}

[System.IO.File]::WriteAllText($resolvedOutputSql, $sb.ToString(), [System.Text.Encoding]::UTF8)

Write-Host "Wrote resolver SQL: $resolvedOutputSql"
Write-Host "Stats:"
$stats.GetEnumerator() | ForEach-Object { Write-Host ("  {0} = {1}" -f $_.Key, $_.Value) }

if ($Apply)
{
    Write-Host "Applying resolver SQL to $WorldSchema..."
    Invoke-MySqlFile -ExecutablePath $resolvedMySqlExe -Server $MySqlHost -Port $MySqlPort -User $MySqlUser -Password $MySqlPassword -FilePath $resolvedOutputSql
    Write-Host "Apply completed."
}
