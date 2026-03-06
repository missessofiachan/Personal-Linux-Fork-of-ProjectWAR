param(
    [string]$SourceCsvDir = 'C:\Users\Admin\Pictures\WAR\hash_workspace\extracted\data\data\gamedata',
    [string]$RepoCsvDir = 'Database\ability_parity\csv\gamedata',
    [string]$SchemaSqlPath = 'Database\ability_parity\mythic_csv.schema.sql',
    [string]$OutputSql = 'Database\ability_parity\work\mythic_csv_sync.generated.sql',
    [string]$MySqlExePath = 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe',
    [string]$MySqlHost = '127.0.0.1',
    [int]$MySqlPort = 3306,
    [string]$MySqlUser = 'root',
    [string]$MySqlPassword = 'password',
    [string]$WorldSchema = 'war_world_curated',
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

function SqlString {
    param([AllowNull()][string]$Value)

    if ($null -eq $Value)
        { return 'NULL' }

    $escaped = $Value.Replace('\', '\\').Replace("'", "''").Replace("`r", '').Replace("`n", '\n')
    return "'" + $escaped + "'"
}

function Parse-IntOrNull {
    param([AllowNull()][string]$Value)

    if ([string]::IsNullOrWhiteSpace($Value))
        { return $null }

    $trimmed = $Value.Trim()
    if ($trimmed -match '^-?\d+(\.\d+)?$')
    {
        try { return [int][double]$trimmed } catch { return $null }
    }

    return $null
}

function To-JsonArrayString {
    param([string[]]$Values)

    $sb = New-Object System.Text.StringBuilder
    [void]$sb.Append('[')

    for ($i = 0; $i -lt $Values.Length; $i++)
    {
        if ($i -gt 0) { [void]$sb.Append(',') }

        $v = if ($null -eq $Values[$i]) { '' } else { [string]$Values[$i] }
        $escaped = $v.Replace('\', '\\').Replace('"', '\"').Replace("`r", '\r').Replace("`n", '\n')
        [void]$sb.Append('"')
        [void]$sb.Append($escaped)
        [void]$sb.Append('"')
    }

    [void]$sb.Append(']')
    return $sb.ToString()
}

function Get-RowValueOrEmpty {
    param(
        [string[]]$Row,
        [int]$Index
    )

    if ($null -ne $Row -and $Row.Length -gt $Index -and $null -ne $Row[$Index])
    {
        return [string]$Row[$Index]
    }

    return ''
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

function Flush-InsertBatch {
    param(
        [Parameter(Mandatory = $true)][System.IO.StreamWriter]$Writer,
        [Parameter(Mandatory = $true)][string]$InsertPrefix,
        [Parameter(Mandatory = $true)][System.Collections.Generic.List[string]]$Rows
    )

    if ($Rows.Count -eq 0)
        { return }

    $Writer.WriteLine($InsertPrefix)
    for ($i = 0; $i -lt $Rows.Count; $i++)
    {
        $suffix = if ($i -lt $Rows.Count - 1) { ',' } else { ';' }
        $Writer.WriteLine($Rows[$i] + $suffix)
    }
    $Rows.Clear()
}

$csvFiles = @(
    'abilities.csv',
    'abilityline_to_bufftype.csv',
    'anim_abilities.csv',
    'effects.csv',
    'effectdef.csv',
    'effectlists.csv',
    'effectmisc.csv',
    'effectnifs.csv',
    'effectproj.csv',
    'effectvfx.csv',
    'flageffects.csv',
    'stateeffects.csv',
    'stepeffects.csv',
    'weaponeffects.csv',
    'petcommanddata.csv',
    'items.csv',
    'itemdata.csv'
)

$resolvedSourceDir = Resolve-ProjectPath -PathValue $SourceCsvDir
$resolvedRepoCsvDir = Resolve-ProjectPath -PathValue $RepoCsvDir
$resolvedSchemaSqlPath = Resolve-ProjectPath -PathValue $SchemaSqlPath
$resolvedOutputSql = Resolve-ProjectPath -PathValue $OutputSql
$resolvedMySqlExe = Resolve-ProjectPath -PathValue $MySqlExePath

if (-not (Test-Path -LiteralPath $resolvedSourceDir))
    { throw "SourceCsvDir not found: $resolvedSourceDir" }
if (-not (Test-Path -LiteralPath $resolvedSchemaSqlPath))
    { throw "Schema SQL not found: $resolvedSchemaSqlPath" }
if (-not (Test-Path -LiteralPath $resolvedMySqlExe))
    { throw "mysql executable not found: $resolvedMySqlExe" }

New-Item -ItemType Directory -Path $resolvedRepoCsvDir -Force | Out-Null
New-Item -ItemType Directory -Path (Split-Path -Path $resolvedOutputSql -Parent) -Force | Out-Null

foreach ($fileName in $csvFiles)
{
    $src = Join-Path $resolvedSourceDir $fileName
    if (-not (Test-Path -LiteralPath $src))
        { throw "Missing source CSV: $src" }

    $dst = Join-Path $resolvedRepoCsvDir $fileName
    Copy-Item -LiteralPath $src -Destination $dst -Force
}

Add-Type -AssemblyName Microsoft.VisualBasic

$schemaSql = Get-Content -LiteralPath $resolvedSchemaSqlPath -Raw
$writer = New-Object System.IO.StreamWriter($resolvedOutputSql, $false, [System.Text.Encoding]::UTF8)

$stats = [ordered]@{
    datasets = 0
    raw_rows = 0
    typed_abilities = 0
    typed_effects = 0
}

$rawBatchSize = 250
$typedBatchSize = 250

try
{
    $writer.WriteLine('-- Generated by Database/ability_parity/Sync-MythicAbilityCsvBundle.ps1')
    $writer.WriteLine("-- Source directory: $resolvedSourceDir")
    $writer.WriteLine("-- Repo CSV directory: $resolvedRepoCsvDir")
    $writer.WriteLine("USE $WorldSchema;")
    $writer.WriteLine()
    $writer.WriteLine($schemaSql.TrimEnd())
    $writer.WriteLine()
    $writer.WriteLine('START TRANSACTION;')
    $writer.WriteLine('DELETE FROM mythic_csv_raw_rows;')
    $writer.WriteLine('DELETE FROM mythic_csv_dataset_meta;')
    $writer.WriteLine('DELETE FROM mythic_csv_abilities;')
    $writer.WriteLine('DELETE FROM mythic_csv_effects;')
    $writer.WriteLine()

    $abilityInsertPrefix = 'REPLACE INTO mythic_csv_abilities (AbilityId,Name,Description,Notes,IconId,AnimationId,EffectAbilityId,EffectId,SourceRowIndex,RawJson) VALUES'
    $effectInsertPrefix = 'REPLACE INTO mythic_csv_effects (EffectId,Name,BuildUpEffectId,BuildUpId,ActivateEffectId,ActivateId,CastEffectId,CastId,ProjectileMainId,ProjectileId,ImpactEffectId,ImpactId,AoeEffectId,AoeId,ChannelEffectId,ChannelingId,VfxRefId,VfxId,AoeTarget,AoeEffectsPerSec,AoeEffectsPerSecond,AoeRadius,AoeDuration,AoeLocation,WeaponTrail,ProjectileOff,ProjectileOverride,SourceRowIndex,RawJson) VALUES'

    $abilityRows = New-Object 'System.Collections.Generic.List[string]'
    $effectRows = New-Object 'System.Collections.Generic.List[string]'

    foreach ($fileName in $csvFiles)
    {
        $datasetName = [System.IO.Path]::GetFileNameWithoutExtension($fileName)
        $repoFilePath = Join-Path $resolvedRepoCsvDir $fileName
        $relativePath = "Database/ability_parity/csv/gamedata/$fileName"
        $hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $repoFilePath).Hash.ToLowerInvariant()

        $parser = New-Object Microsoft.VisualBasic.FileIO.TextFieldParser($repoFilePath)
        $parser.SetDelimiters(',')
        $parser.HasFieldsEnclosedInQuotes = $true

        $headerRow = if (-not $parser.EndOfData) { $parser.ReadFields() } else { @() }
        $columnRow = if (-not $parser.EndOfData) { $parser.ReadFields() } else { @() }

        $headerText = ($headerRow -join ',')
        $columnText = ($columnRow -join ',')

        $writer.WriteLine(
            "INSERT INTO mythic_csv_dataset_meta (DatasetName,SourceRelativePath,Sha256,HeaderRowText,ColumnRowText,RowCount,ImportedAt,Notes) VALUES (" +
            "$(SqlString $datasetName),$(SqlString $relativePath),$(SqlString $hash),$(SqlString $headerText),$(SqlString $columnText),0,NOW(),'generated by Sync-MythicAbilityCsvBundle')" +
            ";"
        )

        $rawInsertPrefix = 'INSERT INTO mythic_csv_raw_rows (DatasetName,RowIndex,IdText,IdInt,PayloadJson) VALUES'
        $rawRows = New-Object 'System.Collections.Generic.List[string]'
        $rowIndex = 0

        while (-not $parser.EndOfData)
        {
            $row = $parser.ReadFields()
            if ($null -eq $row)
                { continue }

            $rowIndex++
            $idText = if ($row.Length -gt 0 -and $null -ne $row[0]) { [string]$row[0] } else { '' }
            $idText = $idText.Trim()
            $idInt = Parse-IntOrNull -Value $idText
            $idIntSql = if ($null -eq $idInt) { 'NULL' } else { [string]$idInt }

            $payloadJson = To-JsonArrayString -Values $row

            $rawRows.Add("($(SqlString $datasetName),$rowIndex,$(SqlString $idText),$idIntSql,$(SqlString $payloadJson))")
            if ($rawRows.Count -ge $rawBatchSize)
            {
                Flush-InsertBatch -Writer $writer -InsertPrefix $rawInsertPrefix -Rows $rawRows
            }

            if ($datasetName -eq 'abilities' -and $null -ne $idInt -and $idInt -ge 0)
            {
                $name = Get-RowValueOrEmpty -Row $row -Index 1
                $description = Get-RowValueOrEmpty -Row $row -Index 2
                $notes = Get-RowValueOrEmpty -Row $row -Index 3
                $iconId = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 4)
                $animationId = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 7)
                $effectAbilityId = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 8)

                $effectSql = if ($null -eq $effectAbilityId) { 'NULL' } else { [string]$effectAbilityId }
                $abilityRows.Add(
                    "($idInt,$(SqlString $name),$(SqlString $description),$(SqlString $notes)," +
                    "$(if ($null -eq $iconId) { 'NULL' } else { [string]$iconId })," +
                    "$(if ($null -eq $animationId) { 'NULL' } else { [string]$animationId })," +
                    "$effectSql,$effectSql," +
                    "$rowIndex,$(SqlString $payloadJson))"
                )

                if ($abilityRows.Count -ge $typedBatchSize)
                {
                    Flush-InsertBatch -Writer $writer -InsertPrefix $abilityInsertPrefix -Rows $abilityRows
                }

                $stats.typed_abilities++
            }
            elseif ($datasetName -eq 'effects' -and $null -ne $idInt -and $idInt -ge 0)
            {
                $name = Get-RowValueOrEmpty -Row $row -Index 1
                $buildUp = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 2)
                $activate = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 3)
                $cast = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 4)
                $projectileMain = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 5)
                $impact = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 6)
                $aoe = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 7)
                $channel = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 8)
                $vfx = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 9)
                $aoeTarget = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 10)
                $aoePerSec = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 11)
                $aoeRadius = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 12)
                $aoeDuration = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 13)
                $aoeLocation = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 14)
                $weaponTrail = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 15)
                $projectileOff = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 16)
                $projectileOverride = Parse-IntOrNull -Value (Get-RowValueOrEmpty -Row $row -Index 17)

                $buildUpSql = if ($null -eq $buildUp) { 'NULL' } else { [string]$buildUp }
                $activateSql = if ($null -eq $activate) { 'NULL' } else { [string]$activate }
                $castSql = if ($null -eq $cast) { 'NULL' } else { [string]$cast }
                $projectileMainSql = if ($null -eq $projectileMain) { 'NULL' } else { [string]$projectileMain }
                $impactSql = if ($null -eq $impact) { 'NULL' } else { [string]$impact }
                $aoeSql = if ($null -eq $aoe) { 'NULL' } else { [string]$aoe }
                $channelSql = if ($null -eq $channel) { 'NULL' } else { [string]$channel }
                $vfxSql = if ($null -eq $vfx) { 'NULL' } else { [string]$vfx }
                $aoePerSecSql = if ($null -eq $aoePerSec) { 'NULL' } else { [string]$aoePerSec }

                $effectRows.Add(
                    "($idInt,$(SqlString $name)," +
                    "$buildUpSql,$buildUpSql," +
                    "$activateSql,$activateSql," +
                    "$castSql,$castSql," +
                    "$projectileMainSql,$projectileMainSql," +
                    "$impactSql,$impactSql," +
                    "$aoeSql,$aoeSql," +
                    "$channelSql,$channelSql," +
                    "$vfxSql,$vfxSql," +
                    "$(if ($null -eq $aoeTarget) { 'NULL' } else { [string]$aoeTarget })," +
                    "$aoePerSecSql,$aoePerSecSql," +
                    "$(if ($null -eq $aoeRadius) { 'NULL' } else { [string]$aoeRadius })," +
                    "$(if ($null -eq $aoeDuration) { 'NULL' } else { [string]$aoeDuration })," +
                    "$(if ($null -eq $aoeLocation) { 'NULL' } else { [string]$aoeLocation })," +
                    "$(if ($null -eq $weaponTrail) { 'NULL' } else { [string]$weaponTrail })," +
                    "$(if ($null -eq $projectileOff) { 'NULL' } else { [string]$projectileOff })," +
                    "$(if ($null -eq $projectileOverride) { 'NULL' } else { [string]$projectileOverride })," +
                    "$rowIndex,$(SqlString $payloadJson))"
                )

                if ($effectRows.Count -ge $typedBatchSize)
                {
                    Flush-InsertBatch -Writer $writer -InsertPrefix $effectInsertPrefix -Rows $effectRows
                }

                $stats.typed_effects++
            }
        }

        $parser.Close()

        Flush-InsertBatch -Writer $writer -InsertPrefix $rawInsertPrefix -Rows $rawRows
        $writer.WriteLine("UPDATE mythic_csv_dataset_meta SET RowCount=$rowIndex, ImportedAt=NOW() WHERE DatasetName=$(SqlString $datasetName);")

        $stats.datasets++
        $stats.raw_rows += $rowIndex
    }

    Flush-InsertBatch -Writer $writer -InsertPrefix $abilityInsertPrefix -Rows $abilityRows
    Flush-InsertBatch -Writer $writer -InsertPrefix $effectInsertPrefix -Rows $effectRows

    $writer.WriteLine('COMMIT;')
}
finally
{
    $writer.Dispose()
}

Write-Host "Copied CSV bundle to: $resolvedRepoCsvDir"
Write-Host "Wrote SQL: $resolvedOutputSql"
Write-Host "Stats:"
$stats.GetEnumerator() | ForEach-Object { Write-Host ("  {0} = {1}" -f $_.Key, $_.Value) }

if ($Apply)
{
    Write-Host "Applying generated SQL to $WorldSchema..."
    Invoke-MySqlFile -ExecutablePath $resolvedMySqlExe -Server $MySqlHost -Port $MySqlPort -User $MySqlUser -Password $MySqlPassword -FilePath $resolvedOutputSql
    Write-Host 'Apply completed.'
}
