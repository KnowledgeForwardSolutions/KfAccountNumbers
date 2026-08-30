# S0052 - Identifier Type Enum Refactor and JSON Converter Nullability

## Overview

This story addresses two related cleanup tasks in the codebase:

1. **Refactor `IdentifierType` classes to standard enums:** Several national identifier types use non-enum class-based structures (e.g., `BeIdentifierType`, `SeIdentifierType`, `NoIdentifierType`, etc.) to represent discriminated union-like behavior for identifier categories. These should be converted back to standard C# enums as a pragmatic interim solution. Once C# introduces the `closed` keyword (allowing sealed union discriminants), these enums can be migrated to that more expressive form.

2. **Update JSON converters to support nullability:** Several identity number JSON converter classes (e.g., `BeBisnummerJsonConverter`, `BeRijksregisternummerJsonConverter`) currently use `null!` to suppress nullability warnings when deserializing null JSON tokens. These should be updated to return `null` with proper nullable return types, following the pattern established by `BeIdentityNumberJsonConverter`.

## Acceptance Criteria

### Refactor IdentifierType Classes to Enums

- [ ] Identify all `IdentifierType` class definitions across the codebase:
  - `BeIdentifierType.cs`
  - `SeIdentifierType.cs`
  - `NoIdentifierType.cs`
  - `EsIdentifierType.cs` (if it exists)
  - `IsIdentifierType.cs` (if it exists)
  - Any other similar class-based discriminant types

- [ ] For each identified class, create a corresponding enum with members matching the nested struct names:
  - Example: `BeIdentifierType` class with `Rijksregisternummer` and `BisNummer` structs → `BeIdentifierType` enum with `Rijksregisternummer` and `BisNummer` members

- [ ] Update all usages of the nested structs to use enum values instead:
  - Replace `default(BeIdentifierType.Rijksregisternummer)` with `BeIdentifierType.Rijksregisternummer`
  - Replace `default(BeIdentifierType.BisNummer)` with `BeIdentifierType.BisNummer`
  - Update any `IdentifierCategory` union type definitions that reference the struct types

- [ ] Update XML documentation on enum values to match the original struct documentation

- [ ] Add a comment to the enum indicating that it will be converted to a closed enum once C# supports the `closed` keyword

### Update JSON Converters for Nullability

- [ ] Identify all JSON converter classes that currently use `null!` to suppress nullability warnings:
  - `BeBisnummerJsonConverter`
  - `BeRijksregisternummerJsonConverter`
  - Any other similar converter classes in the national identifier types

- [ ] For each identified converter, update the `Read()` method to:
  - Change return type from non-nullable to nullable (e.g., `BeBisnummer` → `BeBisnummer?`)
  - Return `null` instead of `null!` when the JSON token is `null`
  - Follow the pattern from `BeIdentityNumberJsonConverter`

- [ ] Verify that the change does not break existing JSON serialization/deserialization tests

- [ ] Update any existing tests that may rely on the old `null!` behavior

### Testing

- [ ] Ensure all existing unit tests pass after the refactoring
- [ ] Verify that `IdentifierType` properties continue to work correctly across all affected types
- [ ] Confirm that JSON round-trip serialization/deserialization tests pass for null values
- [ ] Run full test suite to catch any regressions

## Technical Notes

### IdentifierType Refactoring Pattern

**Before:**
```csharp
public class BeIdentifierType
{
   public struct Rijksregisternummer { }
   public struct BisNummer { }
}

// Usage:
public IdentifierCategory IdentifierType
   => isBisnummer 
      ? default(BeIdentifierType.BisNummer)
      : default(BeIdentifierType.Rijksregisternummer);
```

**After:**
```csharp
/// <summary>
///   Defines the possible types of identifiers that can be represented with a
///   <see cref="BeIdentityNumber"/> or <see cref="BeRijksregisternummer"/> object.
/// </summary>
/// <remarks>
///   This enum will be converted to a closed enum once C# supports the 'closed' keyword.
/// </remarks>
public enum BeIdentifierType
{
   /// <summary>
   ///   Personal identity number, issued to a person registered in Belgium's
   ///   National Register.
   /// </summary>
   Rijksregisternummer,

   /// <summary>
   ///   Identifier assigned to a person who does not have a rijksregisternummer,
   ///   but who still needs an identifier for tax or other purposes.
   /// </summary>
   BisNummer,
}

// Usage:
public IdentifierCategory IdentifierType
   => isBisnummer 
      ? BeIdentifierType.BisNummer
      : BeIdentifierType.Rijksregisternummer;
```

### JSON Converter Nullability Pattern

**Before:**
```csharp
public class BeBisnummerJsonConverter : JsonConverter<BeBisnummer>
{
   public override BeBisnummer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;  // Suppresses nullability warning, but returns non-null in reality
      }

      var str = reader.GetString();
      return new BeBisnummer(str);
   }

   public override void Write(Utf8JsonWriter writer, BeBisnummer value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
```

**After (matching `BeIdentityNumberJsonConverter` pattern):**
```csharp
public class BeBisnummerJsonConverter : JsonConverter<BeBisnummer>
{
   public override BeBisnummer? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null;  // Properly nullable return type
      }

      var str = reader.GetString();
      return new BeBisnummer(str);
   }

   public override void Write(Utf8JsonWriter writer, BeBisnummer value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
```

## Implementation Scope

### Files to Modify

**IdentifierType Enum Conversions:**
- `src/KfAccountNumbers/National/Europe/BeIdentifierType.cs`
- `src/KfAccountNumbers/National/Europe/SeIdentifierType.cs`
- `src/KfAccountNumbers/National/Europe/NoIdentifierType.cs`
- `src/KfAccountNumbers/National/Europe/EsIdentifierType.cs`
- `src/KfAccountNumbers/National/Europe/IsIdentifierType.cs`
- `src/KfAccountNumbers/National/Europe/FiIdentifierType.cs`
- `src/KfAccountNumbers/National/Europe/FrIdentifierType.cs`
- Any other similar type files in the codebase

**JSON Converter Nullable Refactoring:**
- `src/KfAccountNumbers/National/Europe/BeBisnummerJsonConverter` (in `BeBisnummer.cs`)
- `src/KfAccountNumbers/National/Europe/BeRijksregisternummerJsonConverter` (in `BeRijksregisternummer.cs`)
- Any other identifier type JSON converters that need the nullability fix

**Test Files:**
- `tests/KfAccountNumbers.Tests.Unit/National/Europe/BeBisnummerTests.cs`
- `tests/KfAccountNumbers.Tests.Unit/National/Europe/BeRijksregisternummerTests.cs`
- `tests/KfAccountNumbers.Tests.Unit/National/Europe/BeIdentityNumberTests.cs`
- Related test files for other identifier types

## Risk Analysis

### Low Risk

- **Enum conversion:** The change from nested structs to enum values is a mechanical refactoring that does not change the logical behavior. Compiler will catch most issues at build time.
- **Test coverage:** Comprehensive existing tests for all affected types ensure regressions are caught.

### Mitigation

- **Incremental approach:** Consider refactoring one identifier type family (e.g., Belgian types) and running tests before moving to others.
- **JSON converter updates:** These changes maintain backward-compatible serialization behavior while improving null safety.

## Definition of Done

- [ ] All identified `IdentifierType` classes have been converted to enums
- [ ] All usages of nested struct defaults have been updated to enum values
- [ ] XML documentation has been preserved and remains accurate
- [ ] All JSON converter `Read()` methods support nullable returns
- [ ] No `null!` suppression operators remain in JSON converters for null handling
- [ ] All unit tests pass (full test suite)
- [ ] No compiler warnings or errors
- [ ] Code review completed
- [ ] Changes merged to main branch

## References

- **Belgian identifier hierarchy:** `docs/Stories/Backlog/S0048-BeRijksregisternummerHierarchyRefactor.md`
- **Swedish identifier hierarchy:** Related enum patterns in `SeIdentifierType.cs`
- **C# closed enum proposal:** Future C# language feature (TBD version)
- **BeIdentityNumberJsonConverter pattern:** `src/KfAccountNumbers/National/Europe/BeIdentityNumber.cs` lines 535–552

## Story Points Estimate

**8 points** (Medium effort)

- Enum conversions across 6–8 files: 3 points
- JSON converter nullability updates: 2 points
- Test verification and cleanup: 2 points
- Integration testing and review: 1 point
