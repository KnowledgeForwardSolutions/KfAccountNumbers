# S0035: Migrate FrInseeNumber to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 6 points  
**Related:** S0019 (GbNhsNumber), S0022 (GbPatientNumber), S0023-S0034 (Union migrations), E01-UnionMigration

---

## Overview

Modernize the `FrInseeNumber` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `FrInseeNumber` with the new validation architecture introduced in the union-pattern migration initiative and improves type safety, error context, and API consistency across the codebase.

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
Replace the current `FrInseeNumberValidationResult` enum with:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigits,
   InvalidSeparator,
   InvalidGender,
   InvalidMonth,
   InvalidDepartment)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigits,
   InvalidSeparator,
   InvalidGender,
   InvalidMonth,
   InvalidDepartment)
{
}
```

#### 2. Update Constructor Exception Documentation
Change `KfValidationException<FrInseeNumberValidationResult>` to `UKfValidationException<ValidationError>`.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types.
- Each validation failure should instantiate the appropriate union case with localized message via `Messages` class.
- The success case should return `default(ValidValue)`.

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<FrInseeNumber, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing exception.
- Ensure private validation-bypass constructor remains functional.
- Ensure backward compatibility.

#### 5. Preserve Complex Validation Rules
- Gender indicator: must be 1, 2, 7, or 8
- Month: must be 01-12 (known dates) or 13, 20-42, 50-99 (unknown/incomplete dates)
- Department codes: 01-95 standard, plus 97 (Monaco), 98 (special), 99 (foreign), 2A/2B (Corsica)
- For Corsica departments: 2A → 19, 2B → 18 in checksum calculation
- Modulus 97 check digit validation
- Corsica department codes must be uppercase
- Separators at positions 1, 4, 7, 10, 14, 18 (zero-based) in 21-character format

#### 6. Handle Formatted and Unformatted Parsing
- Support both 15-character unformatted and 21-character formatted (with spaces) input
- Format: `S YY MM LL OOO KKK CC`
- All separator characters must be the same (typically spaces)
- Separators must not be ASCII digits

#### 7. Remove Old Extension Methods
- Delete the `FrInseeNumberValidationResult` enum.
- Delete the `FrInseeNumberValidationResultExtensions` class.

#### 8. JSON Serialization
- Verify that `FrInseeNumberJsonConverter` continues to work with updated constructor.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- Public API surface must remain functionally equivalent.
- Exception types and messages consistent with current behavior.

#### 2. Code Organization
- Integrate union types directly into `FrInseeNumber.cs`.

#### 3. Error Messaging
Parity between current and new messages:
- `Empty` → `Messages.FrInseeNumberEmpty`
- `InvalidLength` → `Messages.FrInseeNumberInvalidLength`
- `InvalidCharacter` → `Messages.FrInseeNumberInvalidCharacter`
- `InvalidCheckDigits` → `Messages.FrInseeNumberInvalidCheckDigits`
- `InvalidSeparator` → `Messages.FrInseeNumberInvalidSeparator`
- `InvalidGender` → `Messages.FrInseeNumberInvalidGender`
- `InvalidMonth` → `Messages.FrInseeNumberInvalidMonth`
- `InvalidDepartment` → `Messages.FrInseeNumberInvalidDepartment`

#### 4. Test Coverage
All existing tests in `FrInseeNumberTests.cs` must continue to pass.

---

## Validation Rules

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace → `EmptyValue`.
2. **Length Validation**: Value must be 15 or 21 characters → `InvalidLength`.
3. **Character Validation**: All non-separator/non-Corsica characters must be digits → `InvalidCharacter`.
4. **Gender Validation**: Position 0 must be 1, 2, 7, or 8 → `InvalidGender`.
5. **Month Validation**: Positions 3-4 must be 01-12 or 13, 20-42, 50-99 → `InvalidMonth`.
6. **Department Validation**: Positions 5-7 (or with separators) must be valid department code → `InvalidDepartment`.
7. **Check Digit Validation**: Last 2 digits must be valid modulus 97 check sum → `InvalidCheckDigits`.
8. **Separator Validation**: In 21-char format, must be non-digit and consistent → `InvalidSeparator`.

---

## Files to Modify/Delete

### Modify
- `src/KfAccountNumbers/Governmental/Europe/FrInseeNumber.cs`
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/FrInseeNumberTests.cs`

### Delete
- `src/KfAccountNumbers/Governmental/Europe/FrInseeNumberValidationResult.cs`

### Verify
- `src/KfAccountNumbers/Governmental/Europe/FrInseeNumberJsonConverter.cs`
- `Messages` class

---

## Success Metrics

- All existing unit tests pass post-migration without scenario changes.
- Build succeeds with no compilation errors or warnings.
- Code review approves migration for consistency with union-pattern implementations.
- No performance regression detected.
- All special department codes (2A, 2B, 97, 98, 99) continue to work correctly.
