# S0037: Migrate IePpsNumber to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 4 points  
**Related:** S0019 (GbNhsNumber), S0022 (GbPatientNumber), S0023-S0036 (Union migrations), E01-UnionMigration

---

## Overview

Modernize the `IePpsNumber` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `IePpsNumber` with the new validation architecture introduced in the union-pattern migration initiative and improves type safety, error context, and API consistency across the codebase.

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
Replace the current `IePpsNumberValidationResult` enum with:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigit)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigit)
{
}
```

#### 2. Update Constructor Exception Documentation
Change `KfValidationException<IePpsNumberValidationResult>` to `UKfValidationException<ValidationError>`.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types.
- Each validation failure should instantiate the appropriate union case with localized message via `Messages` class.

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<IePpsNumber, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing exception.
- Ensure private validation-bypass constructor remains functional.
- Ensure backward compatibility.

#### 5. Preserve Weighted Modulus 23 Check Character Validation
- Check character calculated from 7 digits + optional extension letter (if present)
- Valid check characters: A-W (23 characters, where W = 0, A = 1, B = 2, ..., V = 22)
- Weighted calculation using standard position weights
- Case-insensitive for letter characters (normalized to upper-case internally)

#### 6. Handle Optional Extension Letter
- Extension letter: positions indicate whether 8-char or 9-char format
- If 9-character format: last character must be A-I or W
- Extension letter is included in check character calculation
- Extension letter made permanent in 2013 to expand available PPS numbers

#### 7. Case Normalization
- Implementation is case-insensitive and normalizes to upper-case
- Accept both lower-case and upper-case alphabetic characters
- Store normalized (upper-case) internally

#### 8. Remove Old Extension Methods
- Delete the `IePpsNumberValidationResult` enum.
- Delete the `IePpsNumberValidationResultExtensions` class.

#### 9. JSON Serialization
- Verify that `IePpsNumberJsonConverter` continues to work with updated constructor.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- Public API surface must remain functionally equivalent.
- Exception types and messages consistent with current behavior.

#### 2. Code Organization
- Integrate union types directly into `IePpsNumber.cs`.

#### 3. Error Messaging
Parity between current and new messages:
- `Empty` → `Messages.IePpsNumberEmpty`
- `InvalidLength` → `Messages.IePpsNumberInvalidLength`
- `InvalidCharacter` → `Messages.IePpsNumberInvalidCharacter`
- `InvalidCheckDigit` → `Messages.IePpsNumberInvalidCheckDigit`

#### 4. Test Coverage
All existing tests in `IePpsNumberTests.cs` must continue to pass.

---

## Validation Rules

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace → `EmptyValue`.
2. **Length Validation**: Must be 8 or 9 characters in length → `InvalidLength`.
3. **Character Validation**:
   - Positions 0-6: must be ASCII digits (0-9)
   - Position 7: must be letter A-W (case-insensitive)
   - Position 8 (if present): must be letter A-I or W (case-insensitive) → `InvalidCharacter`.
4. **Check Character Validation**: 
   - Character at position 7 (zero-based) must be valid weighted modulus 23 check character
   - Valid characters: A-W (W = 0, A = 1, B = 2, ..., V = 22)
   - If 9-character format, extension letter participates in calculation → `InvalidCheckDigit`.

---

## Special Considerations

### Weighted Modulus 23 Check Character
- Uses position-weighted algorithm with modulus 23
- Check characters: `WABCDEFGHIJKLMNOPQRSTUV` (W=0, A=1, ..., V=22)
- For 9-character format, extension letter (position 8) participates in calculating check character

### Extension Letter (Position 8)
- Optional second letter (A-I or W)
- Made permanent in 2013 to allow expansion of available PPS numbers
- When present, included in check character calculation
- Different valid range (A-I or W) compared to check character (A-W)

### Case Insensitivity
- Implementation accepts both upper-case and lower-case
- All values normalized and stored as upper-case
- Provides user-friendly input handling

### Reference Implementations
- `GbNhsNumber.cs`: Union pattern template
- `IePpsNumber.cs`: 8 or 9 character format handling

---

## Files to Modify/Delete

### Modify
- `src/KfAccountNumbers/Governmental/Europe/IePpsNumber.cs`
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/IePpsNumberTests.cs`

### Delete
- `src/KfAccountNumbers/Governmental/Europe/IePpsNumberValidationResult.cs`

### Verify
- `src/KfAccountNumbers/Governmental/Europe/IePpsNumberJsonConverter.cs`
- `Messages` class

---

## Success Metrics

- All existing unit tests pass post-migration without scenario changes.
- Build succeeds with no compilation errors or warnings.
- Code review approves migration for consistency with union-pattern implementations.
- No performance regression detected.
- Both 8-character and 9-character formats continue to work correctly.
- Case-insensitive handling maintained (normalization to upper-case).
- Weighted modulus 23 check character validation works correctly for both formats.
- Extension letter (position 8) valid range (A-I or W) properly enforced.
