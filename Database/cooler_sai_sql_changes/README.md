# cooler-SAI SQL change archive

This folder is a **manual review archive only**. Do not import it as a migration set.

## What is included
- `commits.csv`: all SQL/Database commits by `cooler-SAI` from commit range `1980c8733975f8898aab1410a96b606d1aa1ea4a..master`.
- `sql_file_index.csv`: per-commit SQL file-level change index (`A/M/D/R...` status + path).
- `commit_summaries/*.txt`: commit metadata + SQL/Database name-status file list.
- `sql_patches.7z`: compressed SQL-only patch exports for commits where patch size was considered manageable.
- `summary_only_commits.csv`: commits intentionally exported as summary only.
- `extract_full_sql_patch.ps1`: helper to export a full SQL patch for any commit hash when needed.

## Export policy used
- Full SQL patch exported when `(insertions + deletions) <= 75000` and at least one `.sql` file changed.
- Otherwise commit is `summary_only` and listed in `summary_only_commits.csv`.

## Notes
- This archive preserves reviewability, not execution order safety.
- Several commits include bulk SQL dumps, legacy packs, or large file moves.
- Validate table/schema compatibility before cherry-picking any SQL into `war_world_curated`.
- To inspect shipped patches, extract `sql_patches.7z` first.
