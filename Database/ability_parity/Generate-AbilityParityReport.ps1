param(
    [string]$ClientAbilitiesCsv = 'C:\Users\Admin\Pictures\WAR\hash_workspace\extracted\data\data\gamedata\abilities.csv',
    [string]$LondoAbilitySql = 'C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\Database Tables\Londos Server v2\War_Ability.sql',
    [string]$WorldArchive = 'Database\Database.7z',
    [string]$WorldExtractDir = 'Database\ability_parity\work',
    [string]$OutputDir = 'Database\ability_parity\reports',
    [string]$AbilityMgrPath = 'WorldServer\World\Abilities\AbilityMgr.cs',
    [int]$PreviewCount = 200
)

$ErrorActionPreference = 'Stop'

function Resolve-ProjectPath {
    param([Parameter(Mandatory = $true)][string]$PathValue)

    if ([System.IO.Path]::IsPathRooted($PathValue)) {
        return $PathValue
    }

    return [System.IO.Path]::GetFullPath((Join-Path (Get-Location) $PathValue))
}

function Ensure-Directory {
    param([Parameter(Mandatory = $true)][string]$PathValue)

    if (-not (Test-Path -LiteralPath $PathValue)) {
        New-Item -ItemType Directory -Path $PathValue | Out-Null
    }
}

function New-IntSet {
    return ,([System.Collections.Generic.HashSet[int]]::new())
}

function Get-ClientAbilityIds {
    param([Parameter(Mandatory = $true)][string]$CsvPath)

    try {
        if (-not (Test-Path -LiteralPath $CsvPath)) {
            throw "Client CSV path not found: $CsvPath"
        }

        $lines = Get-Content -LiteralPath $CsvPath
        $unique = @{}
        $lineIndex = 0
        foreach ($line in $lines) {
            $lineIndex++
            if ($lineIndex -le 2) {
                continue
            }

            if ([string]::IsNullOrWhiteSpace($line)) {
                continue
            }

            $match = [System.Text.RegularExpressions.Regex]::Match($line, '^\s*(\d+)\s*,')
            if (-not $match.Success) {
                continue
            }

            $parsed = 0
            if ([int]::TryParse($match.Groups[1].Value, [ref]$parsed)) {
                $unique[$parsed] = $true
            }
        }

        $ids = New-IntSet
        foreach ($key in $unique.Keys) {
            $null = $ids.Add([int]$key)
        }

        return ,$ids
    }
    catch {
        $line = $_.InvocationInfo.ScriptLineNumber
        $text = $_.Exception.Message
        throw "Get-ClientAbilityIds failed at script line ${line}: $text"
    }
}

function Get-FirstColumnIdsFromSqlText {
    param(
        [Parameter(Mandatory = $true)][string]$SqlText,
        [Parameter(Mandatory = $true)][string]$TableName
    )

    $needle = "INSERT INTO ``{0}`` VALUES " -f $TableName
    $ids = New-IntSet
    $cursor = 0

    while ($true) {
        $idx = $SqlText.IndexOf($needle, $cursor, [System.StringComparison]::OrdinalIgnoreCase)
        if ($idx -lt 0) {
            break
        }

        $start = $idx + $needle.Length
        $end = $SqlText.IndexOf(';', $start)
        if ($end -lt 0) {
            break
        }

        $segment = $SqlText.Substring($start, $end - $start)
        foreach ($match in [System.Text.RegularExpressions.Regex]::Matches($segment, '\((\d+),')) {
            $parsed = 0
            if ([int]::TryParse($match.Groups[1].Value, [ref]$parsed)) {
                $null = $ids.Add($parsed)
            }
        }

        $cursor = $end + 1
    }

    return ,$ids
}

function Get-OnlyInA {
    param(
        [Parameter(Mandatory = $true)][System.Collections.Generic.HashSet[int]]$A,
        [Parameter(Mandatory = $true)][System.Collections.Generic.HashSet[int]]$B
    )

    $result = [System.Collections.Generic.List[int]]::new()
    foreach ($id in $A) {
        if (-not $B.Contains($id)) {
            $result.Add($id)
        }
    }

    return @($result | Sort-Object)
}

function Get-IntersectionCount {
    param(
        [Parameter(Mandatory = $true)][System.Collections.Generic.HashSet[int]]$A,
        [Parameter(Mandatory = $true)][System.Collections.Generic.HashSet[int]]$B
    )

    $count = 0
    foreach ($id in $A) {
        if ($B.Contains($id)) {
            $count++
        }
    }

    return $count
}

function Get-SevenZipPath {
    $cmd = Get-Command 7z -ErrorAction SilentlyContinue
    if ($cmd) {
        return $cmd.Source
    }

    $default = 'C:\Program Files\7-Zip\7z.exe'
    if (Test-Path -LiteralPath $default) {
        return $default
    }

    throw '7z executable was not found.'
}

function Get-HardcodedAbilityMgrConstants {
    param(
        [Parameter(Mandatory = $true)][string]$AbilityMgrSourcePath,
        [Parameter(Mandatory = $true)][System.Collections.Generic.HashSet[int]]$WorldAbilityIds
    )

    $content = Get-Content -LiteralPath $AbilityMgrSourcePath -Raw
    $missing = [System.Collections.Generic.List[object]]::new()
    $regex = [System.Text.RegularExpressions.Regex]::new('const\s+ushort\s+(\w+)\s*=\s*(\d+)\s*;')
    $matches = $regex.Matches($content)

    foreach ($match in $matches) {
        $name = $match.Groups[1].Value
        if (-not $name.EndsWith('Entry', [System.StringComparison]::Ordinal)) {
            continue
        }

        $id = [int]$match.Groups[2].Value
        if (-not $WorldAbilityIds.Contains($id)) {
            $missing.Add([ordered]@{
                    Name = $name
                    Entry = $id
                })
        }
    }

    return $missing
}

function Take-Preview {
    param(
        [Parameter(Mandatory = $true)][System.Array]$Values,
        [Parameter(Mandatory = $true)][int]$Count
    )

    if ($Values.Count -le $Count) {
        return $Values
    }

    return $Values[0..($Count - 1)]
}

$resolvedClientCsv = Resolve-ProjectPath -PathValue $ClientAbilitiesCsv
$resolvedLondoSql = Resolve-ProjectPath -PathValue $LondoAbilitySql
$resolvedWorldArchive = Resolve-ProjectPath -PathValue $WorldArchive
$resolvedWorldExtractDir = Resolve-ProjectPath -PathValue $WorldExtractDir
$resolvedOutputDir = Resolve-ProjectPath -PathValue $OutputDir
$resolvedAbilityMgr = Resolve-ProjectPath -PathValue $AbilityMgrPath

Ensure-Directory -PathValue $resolvedWorldExtractDir
Ensure-Directory -PathValue $resolvedOutputDir

$sevenZip = Get-SevenZipPath
& $sevenZip e -y ("-o{0}" -f $resolvedWorldExtractDir) $resolvedWorldArchive 'war_world.sql' | Out-Null

$worldSqlPath = Join-Path $resolvedWorldExtractDir 'war_world.sql'
if (-not (Test-Path -LiteralPath $worldSqlPath)) {
    throw "Failed to extract war_world.sql from $resolvedWorldArchive"
}

$clientIds = Get-ClientAbilityIds -CsvPath $resolvedClientCsv
$londoSqlText = Get-Content -LiteralPath $resolvedLondoSql -Raw
$worldSqlText = Get-Content -LiteralPath $worldSqlPath -Raw

$londoAbilityIds = Get-FirstColumnIdsFromSqlText -SqlText $londoSqlText -TableName 'Ability'
$worldAbilityIds = Get-FirstColumnIdsFromSqlText -SqlText $worldSqlText -TableName 'abilities'
$worldAbilityCommandEntryIds = Get-FirstColumnIdsFromSqlText -SqlText $worldSqlText -TableName 'ability_commands'
$worldBuffInfoIds = Get-FirstColumnIdsFromSqlText -SqlText $worldSqlText -TableName 'buff_infos'
$worldBuffCommandEntryIds = Get-FirstColumnIdsFromSqlText -SqlText $worldSqlText -TableName 'buff_commands'

$clientMissingInWorld = Get-OnlyInA -A $clientIds -B $worldAbilityIds
$worldMissingInClient = Get-OnlyInA -A $worldAbilityIds -B $clientIds
$londoMissingInWorld = Get-OnlyInA -A $londoAbilityIds -B $worldAbilityIds
$worldMissingInLondo = Get-OnlyInA -A $worldAbilityIds -B $londoAbilityIds
$worldAbilitiesWithoutCommands = Get-OnlyInA -A $worldAbilityIds -B $worldAbilityCommandEntryIds
$hardcodedMissing = Get-HardcodedAbilityMgrConstants -AbilityMgrSourcePath $resolvedAbilityMgr -WorldAbilityIds $worldAbilityIds

$clientWorldOverlap = Get-IntersectionCount -A $clientIds -B $worldAbilityIds
$londoWorldOverlap = Get-IntersectionCount -A $londoAbilityIds -B $worldAbilityIds

$reportDateUtc = [DateTime]::UtcNow.ToString('yyyy-MM-ddTHH:mm:ssZ')
$baseName = [DateTime]::UtcNow.ToString('yyyyMMdd_HHmmss')
$jsonPath = Join-Path $resolvedOutputDir ("ability_parity_{0}.json" -f $baseName)
$mdPath = Join-Path $resolvedOutputDir ("ability_parity_{0}.md" -f $baseName)

$summary = [ordered]@{
    generated_utc = $reportDateUtc
    inputs        = [ordered]@{
        client_abilities_csv = $resolvedClientCsv
        londo_ability_sql    = $resolvedLondoSql
        world_archive        = $resolvedWorldArchive
        world_sql            = $worldSqlPath
        ability_mgr          = $resolvedAbilityMgr
    }
    counts        = [ordered]@{
        client_abilities                       = $clientIds.Count
        londo_abilities                        = $londoAbilityIds.Count
        world_abilities                        = $worldAbilityIds.Count
        world_ability_commands_distinct_entry  = $worldAbilityCommandEntryIds.Count
        world_buff_infos                       = $worldBuffInfoIds.Count
        world_buff_commands_distinct_entry     = $worldBuffCommandEntryIds.Count
        overlap_client_world                   = $clientWorldOverlap
        overlap_londo_world                    = $londoWorldOverlap
        client_missing_in_world                = $clientMissingInWorld.Count
        world_missing_in_client                = $worldMissingInClient.Count
        londo_missing_in_world                 = $londoMissingInWorld.Count
        world_missing_in_londo                 = $worldMissingInLondo.Count
        world_abilities_without_commands       = $worldAbilitiesWithoutCommands.Count
        hardcoded_abilitymgr_constants_missing = $hardcodedMissing.Count
    }
    preview       = [ordered]@{
        client_missing_in_world                = Take-Preview -Values $clientMissingInWorld -Count $PreviewCount
        world_missing_in_client                = Take-Preview -Values $worldMissingInClient -Count $PreviewCount
        londo_missing_in_world                 = Take-Preview -Values $londoMissingInWorld -Count $PreviewCount
        world_missing_in_londo                 = Take-Preview -Values $worldMissingInLondo -Count $PreviewCount
        world_abilities_without_commands       = Take-Preview -Values $worldAbilitiesWithoutCommands -Count $PreviewCount
        hardcoded_abilitymgr_constants_missing = Take-Preview -Values $hardcodedMissing -Count $PreviewCount
    }
}

$summary | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $jsonPath -Encoding UTF8

$md = @()
$md += "# Ability Parity Baseline"
$md += ""
$md += "- Generated (UTC): $reportDateUtc"
$md += "- Client CSV: $resolvedClientCsv"
$md += "- Londo SQL: $resolvedLondoSql"
$md += "- World SQL: $worldSqlPath"
$md += ""
$md += "## Counts"
$md += ""
$md += "- Client abilities: $($clientIds.Count)"
$md += "- Londo abilities: $($londoAbilityIds.Count)"
$md += "- World abilities: $($worldAbilityIds.Count)"
$md += "- World ability command entries: $($worldAbilityCommandEntryIds.Count)"
$md += "- World buff infos: $($worldBuffInfoIds.Count)"
$md += "- World buff command entries: $($worldBuffCommandEntryIds.Count)"
$md += "- Client/World overlap: $clientWorldOverlap"
$md += "- Londo/World overlap: $londoWorldOverlap"
$md += "- Client missing in World: $($clientMissingInWorld.Count)"
$md += "- World missing in Client: $($worldMissingInClient.Count)"
$md += "- Londo missing in World: $($londoMissingInWorld.Count)"
$md += "- World missing in Londo: $($worldMissingInLondo.Count)"
$md += "- World abilities without ability_commands: $($worldAbilitiesWithoutCommands.Count)"
$md += "- AbilityMgr hardcoded constants missing in World DB: $($hardcodedMissing.Count)"
$md += ""
$md += "## Preview (first $PreviewCount each)"
$md += ""
$md += "### Client missing in World"
$md += ($summary.preview.client_missing_in_world -join ', ')
$md += ""
$md += "### Londo missing in World"
$md += ($summary.preview.londo_missing_in_world -join ', ')
$md += ""
$md += "### World missing in Client"
$md += ($summary.preview.world_missing_in_client -join ', ')
$md += ""
$md += "### World abilities without ability_commands"
$md += ($summary.preview.world_abilities_without_commands -join ', ')
$md += ""
$md += "### AbilityMgr constants missing in World DB"
$md += (($summary.preview.hardcoded_abilitymgr_constants_missing | ForEach-Object { "$($_.Name)=$($_.Entry)" }) -join ', ')
$md += ""
$md += "## Output Files"
$md += ""
$md += "- JSON: $jsonPath"
$md += "- Markdown: $mdPath"

$md -join [Environment]::NewLine | Set-Content -LiteralPath $mdPath -Encoding UTF8

Write-Host "Ability parity report generated:"
Write-Host "  $jsonPath"
Write-Host "  $mdPath"
