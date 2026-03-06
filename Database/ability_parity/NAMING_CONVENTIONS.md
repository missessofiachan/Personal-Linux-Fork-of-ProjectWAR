# Ability Naming Conventions

This workstream uses a strict source-priority model for naming.

## Source Priority

1. Mythic client CSV naming/semantics (primary).
2. Londo SQL naming/semantics (fallback when CSV naming is ambiguous or missing).
3. Legacy ProjectWAR names only for compatibility bridges.

## Database Naming Rules

- Keep the existing staging table prefixes:
  - `mythic_csv_*` for client CSV staging.
  - `mythic_bin_*` for imported Mythic/Londo graph tables.
  - `mythic_src_*` for mirrored legacy gameplay tables.
- For migration safety, canonical CSV-first columns may coexist with legacy compatibility columns.
  - Example: `EffectId` (canonical) and `EffectAbilityId` (legacy compatibility) are both populated.
- For typed columns in `mythic_csv_*`, use names that describe CSV meaning first.
  - Example: `EffectAbilityId` is the `abilities.csv` effect-link field.
- For imported Londo graph tables, keep Londo table/column intent when CSV meaning is not directly available.
  - Example: `AbilityRequirmentBin` spelling is preserved in fallback table mapping because that is the source table name.

## Code Naming Rules

- C# model/property names must mirror the database schema because `DataObject` binding is name-based.
- Runtime resolver/translator code should use CSV-first terms for ability/effect links.
- When a behavior concept is only known from Londo SQL, use Londo term in method/class naming, and add a brief comment.
- Avoid introducing new pseudo names when a CSV/Londo name already exists.

## Current Canonical Examples

- CSV-first:
  - `MythicCsvAbility.EffectAbilityId`
  - `MythicCsvEffect.ActivateEffectId`
  - `MythicCsvEffect.ProjectileMainId`
- Londo fallback:
  - `LondoAbilityRequirementBinRow` mapped to `AbilityRequirmentBin`
  - `LondoAbilityComponentLinkRow` mapped to `AbilityComponentXComponent`

## Refactor Guidance

- New DB columns/models:
  - pick CSV semantics first;
  - if uncertain, cross-check `WAR-RE-Toolkit` Londo SQL schema and use those names.
- Existing legacy names should be migrated only with compatibility-safe changes (schema + model + loader updates in one pass).
