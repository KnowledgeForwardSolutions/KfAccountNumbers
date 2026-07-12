# S0036: Migrate EsNif to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 5 points  
**Related:** S0019 (GbNhsNumber), S0022 (GbPatientNumber), S0023-S0035 (Union migrations), E01-UnionMigration

---

## Overview

Modernize the `EsNif` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `EsNif` with the new validation architecture introduced in the union-pattern migration initiative and improves type safety, error context, and API consistency across the codebase.

---

## Business Value

* **Type Safety**: Replace error enum values with discriminated union cases that can carry contextual error information.
* **Consistency**: Standardize the validation API across all identifier types.
* **Developer Experience**: Enable compiler-checked exhaustiveness on validation results.
* **Maintainability**: Reduce reliance on extension methods and message lookups.
* **Compatibility**: Align with the modern union-pattern approach across all identifiers.

---

## Requirements

### Functional Requirements

#### 1. Replace Enum with Union Pattern
Replace the current `EsNifValidationResult` enum with:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigit,
   InvalidSeparator)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigit,
   InvalidSeparator)
{
}
```

#### 2. Update Constructor Exception Documentation
Change `KfValidationException<EsNifValidationResult>` to `UKfValidationException<ValidationError>`.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types.
- Each validation failure should instantiate the appropriate union case with localized message via `Messages` class.

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<EsNif, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing exception.
- Ensure private validation-bypass constructor remains functional.
- Ensure backward compatibility.

#### 5. Handle IdentifierType Property
- The `IdentifierType` property (returns `EsIdentifierType` enum: `Dni` or `Nie`) should remain unchanged.
- Validation logic must correctly distinguish between DNI (Spanish citizens) and NIE (foreign residents).
- DNI format: DDDDDDDDC (9 chars unformatted, 10 chars with separator D-C)
- NIE format: PDDDDDDDC (9 chars unformatted, 11 chars with separators P-D-C where P is X/Y/Z)

#### 6. Preserve Modulus 23 Check Character Validation
- Must validate using the 23-character set: `TRWAGMYFPDXBNJZSQVHLCKE`
- For NIE, leading letter (X/Y/Z) maps to (0/1/2) before calculation
- Check character is calculated from the numeric representation of the first 8 digits

#### 7. Handle Formatted and Unformatted Parsing
- Support both 9-character unformatted and formatted input
- DNI formatted: 8 digits + separator + check character (10 total)
- NIE formatted: X-7 digits + separator + check character (11 total)
- Separators can be any non-digit character (typically dash "-")
- Separators must NOT be digits
- Case-sensitive: alphabetic characters must be UPPERCASE

#### 8. Remove Old Extension Methods
- Delete the `EsNifValidationResult` enum.
- Delete the `EsNifValidationResultExtensions` class.

#### 9. JSON Serialization
- Verify that `EsNifJsonConverter` continues to work with updated constructor.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- Public API surface must remain functionally equivalent.
- Exception types and messages consistent with current behavior.

#### 2. Code Organization
- Integrate union types directly into `EsNif.cs`.

#### 3. Error Messaging
Parity between current and new messages:
- `Empty` → `Messages.EsNifEmpty`
- `InvalidLength` → `Messages.EsNifInvalidLength`
- `InvalidCharacter` → `Messages.EsNifInvalidCharacter`
- `InvalidCheckDigit` → `Messages.EsNifInvalidCheckDigit`
- `InvalidSeparator` → `Messages.EsNifInvalidSeparator`

#### 4. Test Coverage
All existing tests in `EsNifTests.cs` must continue to pass.

---

## Validation Rules

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace → `EmptyValue`.
2. **Length Validation**: 
   - 9 characters (unformatted)
   - 10 characters (DNI with 1 separator)
   - 11 characters (NIE with 2 separators) → `InvalidLength`.
3. **Character Validation**:
   - DNI: 8 leading digits + 1 check character
   - NIE: 1 leading (X/Y/Z) + 7 digits + 1 check character
   - All characters case-sensitive (upper-case required) → `InvalidCharacter`.
4. **Separator Validation (formatted only)**:
   - DNI: separator at position 8 (zero-based), must be non-digit
   - NIE: separators at positions 1 and 9 (zero-based), must be same character and non-digit → `InvalidSeparator`.
5. **Check Character Validation**: 
   - Last character must be valid modulus 23 check character from `TRWAGMYFPDXBNJZSQVHLCKE`
   - X/Y/Z (X=0, Y=1, Z=2) are converted before calculation → `InvalidCheckDigit`.

---

## Implementation Notes

### Key IdentifierType Handling
- If leading character is digit → DNI
- If leading character is X/Y/Z → NIE
- The implementation must detect and preserve this distinction through validation

### Modulus 23 Check Characters
- `TRWAGMYFPDXBNJZSQVHLCKE` (23 characters for remainders 0-22)
- Position in string = remainder value

### Reference Implementations
- `GbNhsNumber.cs`: Union pattern template
- `IsKennitala.cs`: Similar IdentifierType property handling

---

## Files to Modify/Delete

### Modify
- `src/KfAccountNumbers/Governmental/Europe/EsNif.cs`
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/EsNifTests.cs`

### Delete
- `src/KfAccountNumbers/Governmental/Europe/EsNifValidationResult.cs`

### Verify
- `src/KfAccountNumbers/Governmental/Europe/EsNifJsonConverter.cs`
- `Messages` class

---

## Success Metrics

- All existing unit tests pass post-migration without scenario changes.
- Build succeeds with no compilation errors or warnings.
- Code review approves migration for consistency with union-pattern implementations.
- No performance regression detected.
- Both DNI and NIE formats (including formatted variants) continue to work correctly.
- Case-sensitivity enforcement (upper-case alphabetic characters) maintained.
- IdentifierType property correctly distinguishes DNI from NIE.
