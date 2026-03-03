# Failure Log - Ability Purchase Fix

## Attempt 1: Indexing & Database Column (Feb 2026)
- **Problem**: Purchasing an ability often granted a random different ability or failed entirely past level 15.
- **Root cause 1**: The server was using a filtered list of "purchasable" abilities to resolve the client's index. When a player learned an ability, the list shifted, causing the indices to mismatch.
- **Root cause 2**: `PurchasedAbilities` column was missing.
- **Why it failed**: The "Stable Indexing" fix was a workaround that didn't address the true source of desync. It failed to account for:
    - **Hidden Categories**: The trainer list included Tactics (Cat 1, 3, 5) and Morale (Cat 4) which the client hides from the trainer UI, causing an inherent index shift.
    - **Bolstering**: Level 16+ players in T1 zones (bolstered to 15) were failing rank-based validation because the server was checking `AdjustedLevel` instead of `Level`.

## Attempt 2: Category Filtering & Auto-Grants (Mar 2026)
- **Problem**: Level 15+ wall remains. Passive abilities (like pet skills) still appear in the trainer.
- **Root cause (Theory)**: `Category == 0` was insufficient to filter out all unwanted abilities, or the client-side trainer logic uses a different field. Bolstering bypass and auto-grants did not resolve the core issue.
- **Status**: **FAILED**. User reports no improvement.

## Attempt 4: Multi-Category Indexing (Mar 2026)
- **Problem**: Desync remains; user clicks slot 22/21 and server resolves to wrong entries (Git Em!, Don't Eat Me).
- **Finding**: Logged output showed the client sent `PackageID=1` for both slot 22 and slot 21. This suggests that for the Career Trainer, `F_BUY_CAREER_PACKAGE` does not use a 1-based index in the way Mastery does, or the client is using a different packet entirely for trainer purchases.
- **Status**: **FAILED**. The hypothesis that partitioned categories would solve the index shift was debunked by the client's invariant `PackageID` behavior.

## Final Summary of Failures
1. **Stable Indexing (Attempt 1)**: Failed to account for hidden client-side categorization.
2. **Category/Bolstering Fix (Attempt 2)**: Failed to correctly sync the trainer list and allowed passives to leak through.
3. **Tab-Based Indexing Partition (Attempt 3/4)**: Attempted to align the server's categories with the client's tabs, but the client returned invalid/static indices (`PackageID=1`), rendering index-based resolution impossible.

**Current State**: All attempts failed. Source code reverted to original state to allow for a fresh start with a different approach (possibly packet sniffing or alternative Opcode research).
