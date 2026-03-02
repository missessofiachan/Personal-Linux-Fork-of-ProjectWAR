# ProjectWAR Systemic Divergence & Mapping Report

This document tracks fundamental differences in architecture, logic, and data representation between the authentic 1.4.8 Retail (Mythic) design and the ProjectWAR Emulator implementation.

## 1. Positional & Spatial Logic

| Feature | 1.4.8 Retail (Truth) | Emulator (ProjectWAR) | Mapping/Translation Logic |
| :--- | :--- | :--- | :--- |
| **Heading / Orientation** | Radians (Double: 0 to ~6.28) | Integer (Short: 0 to 4095) | `EmuHeading = (RetailHeading * 4096 / (2 * PI))`. |
| **World Coordinates** | Continuous World Space (Int) | World Space & Zone Pins | `WorldX = (ZoneOffX << 12) + PinX`. |
| **Zone Offsets** | Fixed 4096-unit grid increments | Used for World-to-Pin mapping | Offsets are critical; mapping them incorrectly "breaks" spawns (NPCs falling through floor). |

## 2. Ability & Combat Systems

| Feature | 1.4.8 Retail (Truth) | Emulator (ProjectWAR) | Mapping/Translation Logic |
| :--- | :--- | :--- | :--- |
| **Damage/Heal Formulas** | `MythicComponentData` JSON strings containing coefficients and variables. | Flat integer columns (`MinDamage`, `MaxDamage`) in `ability_damage_heals`. | **Significant Divergence:** The emulator cannot natively process the Mythic formula engine. Truth JSON must be pre-parsed into static base values. |
| **Visual Effects** | `EffectID` linked to client-side assets. | `EffectID` columns in `abilities`. | Direct mapping, but name strings in Emulator often contain legacy metadata (`^M`, `^f`) that must be stripped for client compatibility. |
| **Ability Constraints** | Complex `AbilityRequirement` binary logic. | Simplified `CantCrit`, `MinRange`, `Range` columns. | High-fidelity mapping requires translating Truth logic flags into the emulator's specific boolean/integer constraints. |

## 3. Item & Character Templates

| Feature | 1.4.8 Retail (Truth) | Emulator (ProjectWAR) | Mapping/Translation Logic |
| :--- | :--- | :--- | :--- |
| **Item Statistics** | 12-slot serialized `StatID:Value;` pairs. | Serialized `Stats` string in `item_infos`. | Identical format, but requires StatID alignment (e.g., 16 = Toughness). |
| **Career Stats** | Binary `UserData` blobs in `player` table. | Flat `StatValue` records in `characterinfo_stats`. | **Current Blocker:** The emulator's stat growth logic is hardcoded or column-based, making direct translation of the Truth binary blobs difficult. |
| **Creature Models** | `SourceIndex` in monster CSVs. | `Model1` / `Model2` in `creature_protos`. | Maps directly to client-side art indices. |

## 4. NPC & Merchant Interaction

| Feature | 1.4.8 Retail (Truth) | Emulator (ProjectWAR) | Mapping/Translation Logic |
| :--- | :--- | :--- | :--- |
| **Monster Faction Mapping** | Faction IDs usually raw (e.g. 128+) in dumps. | `FactionId = Faction/8`. Realm determined by `FactionId` ranges (Order: 8-15, Destro: 16-23). | **Divergence:** Applying raw 1.4.8 dumps (like Londos) directly to `creature_protos.Faction` results in misaligned Realms (e.g., High Elves becoming Destruction) because the emulator logic expects specific bit-shifted ID ranges. |
| **NPC Scale** | Scale represented as 100 for normal. | Scale represented as 100 for normal. | **Divergence:** Some RE dumps use 50 as a base scale or normalized value, which appears as half-size in the ProjectWAR client. |
