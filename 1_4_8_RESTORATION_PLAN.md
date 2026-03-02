# ProjectWAR Database Restoration Plan (Patch 1.4.8)

## 1. Overview and Objective
The core objective of this initiative is to restore the `ProjectWAR` emulator's database as closely as possible to the authentic state of the game during its final official patch (1.4.8). 

Over the years, the `master` branch accumulated thousands of custom, experimental, and non-canonical database edits. A "blanket merge" of these legacy SQL scripts would compromise the accuracy of the emulator. Instead, we will perform a highly precise, data-driven restoration using our primary "Sources of Truth".

## 2. Sources of Truth
We are exceptionally fortunate to possess the original, reverse-engineered client data and protocol documentation. These sources will dictate all database schema updates and record modifications.

### A. The Figleaf Database (`figleaf.db`)
*   **Location:** `C:\Users\Admin\Music\Warhammer\myps\assetdb\figleaf.db` and `C:\Users\Admin\Music\Warhammer\Warhammer\figleaf_zone_37_patched\figleaf.db`
*   **Description:** The intentionally obfuscated client-side SQLite database maintained by Mythic Entertainment. This database contains authoritative data on zones, patches, art assets, string references, and fixtures.
*   **Parsing Tools:** We have C# tools designed to decrypt and read this file located in `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\WarFigleaf`.

### B. MYP Extracted CSVs & Excel Data
*   **Location:** `C:\Users\Admin\Music\Warhammer\Warhammer\Warhammer Online` (Raw CSVs) & `C:\Users\Admin\Music\Warhammer\Warhammer\items` (Excel Docs)
*   **Description:** Raw comma-separated value files extracted from the game's encrypted `.myp` archives (e.g., `character meshes.csv`, `art switches.csv`). Additionally, exhaustive compiled spreadsheets document item stats, slots, and sets.
*   **Parsing Tools:** `WarClientTool` and `WarCoreTools_Myp` in the `WAR-RE-Toolkit` repository.

### C. Live Server Packet Captures & Protocol Documents
*   **Location:** `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit\RE_FINDINGS`
*   **Description:** PCAP analysis scripts, opcode references, and markdown files detailing the exact structure of server-to-client packets. These confirm how abilities, combat formulas, and movement were processed on the live 1.4.8 servers.

---

## 3. The Execution Workflow
To manage the complexity and avoid overwhelming the AI agents or the server's stability, we will divide the restoration into strict **Domains**. An AI Agent must follow this workflow for each domain.

### Step 1: Isolate the Domain
Pick a single, specific system to restore. Do not mix domains.
*   *Example Domains:* Item Statistics, Vendor Prices, Creature Spawns in Tier 1, Creature Abilities, Zone Keep Layouts, Quest Objectives.

### Step 2: Extract the 1.4.8 Truth
*   Navigate to the relevant `Source of Truth` (Figleaf DB, CSVs, or Packet Docs).
*   Use the available tools in the `WAR-RE-Toolkit` to parse or dump the raw data for the isolated domain.
*   Format the extracted data into a readable JSON or temporary CSV for the AI agent to analyze.

### Step 3: Audit Current Emulator State
*   Query the current `Restart` branch database (e.g., `war_world`) for the specific tables related to the domain.
*   Compare the emulator's data against the 1.4.8 Truth. Document the discrepancies (e.g., "Emulator gives Ironbreaker weapons +5 Strength; 1.4.8 Truth shows +2 Strength").

### Step 4: Scavenge from `master` (Optional but Recommended)
*   Search the `master` branch's legacy SQL scripts to see if any contributor already formatted an SQL query that aligns with the 1.4.8 Truth.
*   *Rule:* Only use legacy SQL scripts if they match the "Truth". If they deviate, discard them.

### Step 5: Generate & Apply Clean SQL
*   Create a brand new, meticulously documented SQL script in the `Database/updates/war_world/` folder (e.g., `update_148_restore_item_stats.sql`).
*   Apply the update to the local database.
*   Test the server to ensure the game client loads the updated data without crashing.

---

## 4. Recommended Starting Points for Agents

If you are an AI Agent booting up this task, start with the lowest-risk, highest-yield domains that don't interfere with the heavily refactored RvR codebase:

1.  **Item Restoration:**
    *   **Goal:** Restore base item stats, requirements, and vendor prices.
    *   **Data Source:** `C:\Users\Admin\Music\Warhammer\Warhammer\items\All_Items.xlsx` and `item Stats.xlsx`.
    *   **Emulator Target:** `item_infos` table.

2.  **Visuals & Art Mappings:**
    *   **Goal:** Ensure creatures and NPCs load with the correct visual models and meshes.
    *   **Data Source:** `character meshes.csv`, `character art.csv`, and `art switches.csv` in `C:\Users\Admin\Music\Warhammer\Warhammer\Warhammer Online`.
    *   **Emulator Target:** `creature_protos` table.

3.  **Abilities & Combat Formulas:**
    *   **Goal:** Restore spell coefficients, cast times, and cooldowns.
    *   **Data Source:** The parsed C# packets in `ProtocolServices` and the analysis in `RE_FINDINGS\combat_formulas.md`.
    *   **Emulator Target:** `abilities` and `ability_damage_heals` tables.

---

## 5. Agent Instructions for Resuming Work
If you are an AI agent picking up this task from a previous session:
1.  Read this `1_4_8_RESTORATION_PLAN.md` file in its entirety.
2.  Check the `change_log_gemini.md` (or equivalent) to see which Domains have already been processed.
3.  Select the next Domain from Section 4.
4.  Execute Steps 1 through 5 in Section 3 strictly. Do not proceed to Step 5 until Step 2 and 3 are validated.
5.  Document all findings and logic in your respective AI change log.