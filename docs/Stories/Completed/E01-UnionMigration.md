# E01-UnionMigration

Epic E01-UnionMigration defines the migration from the current enum based validation
to using the new union keyword being introduced in C# 15.

E01-UnionMigration will act as the main branch until C# 15 is released and then will be merged into main after.

## Migration Path

1. Update solution to use preview C# 15.
1. Introduce infrastructure for using unions. Create a new /Results folder in the main
 project and add new UCreateResult (U for union) and associated result structures. Result
 structures will include properties for Description and, where appropriate, Character and
 Position properties to identify specific characters found to be in error and the zero
 based position of the invalid character.
1. Introduce new GbUniquePatientIdentifier type that implements the new union pattern.
1. Update existing identifiers to use the new pattern.
