# S0029: Migrate NoFoedselsnummer to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 5 points  
**Related:** S0019 (GbNhsNumber), S0022 (GbPatientNumber), S0023-S0028 (Union migrations), E01-UnionMigration

---

## Overview

Modernize the `NoFoedselsnummer` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `NoFoedselsnummer` with the new validation architecture introduced in the union-pattern migration initiative and improves type safety, error context, and API consistency across the codebase.

---

## Business Value

* **Type Safety**: Replace error enum values with discriminated union cases that can carry contextual error information (e.g., which character position, what separator was found, validation context).
* **Consistency**: Standardize the validation API across all identifier types (GB patient numbers, US identifiers, European identifiers).
* **Developer Experience**: Enable compiler-checked exhaustiveness on validation results and eliminate string-based error lookups.
* **Maintainability**: Reduce reliance on extension methods and message lookups; move validation context closer to the error definition.
* **Compatibility**: Align with the modern union-pattern approach used by `GbNhsNumber`, `UsSocialSecurityNumber`, `CaSocialInsuranceNumber`, `MxCurp`, `SePersonnummer`, and other recent implementations.

---

## Requirements

### Functional Requirements

#### 1. Replace Enum with Union Pattern
Replace the current `NoFoedselsnummerValidationResult` enum with discriminated unions mirroring the pattern established by GB patient number validation unions:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidSeparator,
   InvalidDateOfBirth,
   InvalidChecksum)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidSeparator,
   InvalidDateOfBirth,
   InvalidChecksum)
{
}
```

Each union case must be a record type (e.g., `record EmptyValue(String Message)`) with a `Message` property to carry localized error descriptions. Some union cases may carry additional context such as:
- `InvalidCharacter`: position and character encountered
- `InvalidSeparator`: position and character encountered
- `InvalidDateOfBirth`: date string, format name

#### 2. Update Constructor Exception Documentation
Modify the constructor exception documentation to reference the new union types:
* Change `KfValidationException<NoFoedselsnummerValidationResult>` to `UKfValidationException<ValidationError>`.
* Ensure all exception cases map to corresponding union cases.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types instead of enum values.
- Each validation failure should instantiate the appropriate union case with a localized message via `Messages` class.
- The success case should return `default(ValidValue)`.

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<NoFoedselsnummer, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing `UKfValidationException<ValidationError>`.
- Ensure private validation-bypass constructor remains functional for post-validation object creation.
- Ensure backward compatibility (same behavioral contract; only the types change).

#### 5. Handle IdentifierType Property
- The `IdentifierType` property (which returns `NoIdentifierType` enum: `Foedselsnummer` or `DNummer`) should remain unchanged.
- Validation logic must correctly distinguish between foedselsnummer and D-nummer during all validation checks.
- The 40-day offset logic for D-nummer (day component 41-71) must be preserved in validation.

#### 6. Preserve Date of Birth Century Inference
- The complex century inference logic (Rules 1-5 based on individual number ranges and two-digit year) must be preserved exactly as-is.
- Valid date range (January 1, 1854 to December 31, 2039) must be maintained.
- Leap-year and month-boundary validation must continue to work correctly.

#### 7. Remove Old Extension Methods
- Delete the `NoFoedselsnummerValidationResult` enum (after migration is complete).
- Delete the `NoFoedselsnummerValidationResultExtensions` class (after migration is complete).
- Optionally create a new supporting file if needed for organization.

#### 8. JSON Serialization
- Verify that `NoFoedselsnummerJsonConverter` continues to work with the updated constructor.
- If needed, update the converter to handle internal validation-bypass logic correctly.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- The public API surface must remain functionally equivalent.
- Exception types and messages should be consistent with current behavior (though exception structure improves).
- Deprecated types (if any) should be marked with `[Obsolete]` attributes.
- The `IdentifierType` property and its return type (`NoIdentifierType` enum) remain unchanged.

#### 2. Code Organization
- Integrate union types directly into `NoFoedselsnummer.cs` (similar to GB patient number implementations).
- Keep the main class file focused on business logic.
- Maintain consistent naming with other union-migrated types (e.g., `ValidationError`, `ValidationResult`).

#### 3. Error Messaging
- All error messages must be stored in the `Messages` class and referenceable via `Messages.NoFoedselsnummer*` keys.
- Ensure parity between current error descriptions and new union case messages:
  - `Empty` → `Messages.NoFoedselsnummerEmpty`
  - `InvalidLength` → `Messages.NoFoedselsnummerInvalidLength`
  - `InvalidCharacter` → `Messages.NoFoedselsnummerInvalidCharacter`
  - `InvalidCheckDigits` → `Messages.NoFoedselsnummerInvalidCheckDigits` (maps to `InvalidChecksum`)
  - `InvalidSeparator` → `Messages.NoFoedselsnummerInvalidSeparator`
  - `InvalidDateOfBirth` → `Messages.NoFoedselsnummerInvalidDateOfBirth`

#### 4. Test Coverage
- All existing unit tests in `NoFoedselsnummerTests.cs` must continue to pass.
- Test method signatures may be updated to work with union return types, but test scenarios must remain equivalent.
- Ensure all validation paths are still covered post-migration, including:
  - Both foedselsnummer and D-nummer validation
  - All five century inference rules
  - Century derivation edge cases
  - Weighted modulus 11 check digit validation (both C1 and C2 digits)

---

## Acceptance Criteria

### Definition of Done

- [ ] **Union types created**: `ValidationError` and `ValidationResult` discriminated unions defined as nested types within `NoFoedselsnummer.cs`.
- [ ] **Constructor updated**: Exception documentation and validation logic updated to use `UKfValidationException<ValidationError>`.
- [ ] **Validate method refactored**: Returns `ValidationResult` union; all validation failures instantiate appropriate union cases.
- [ ] **Create method updated**: Signature returns `CreateResult<NoFoedselsnummer, ValidationError>`.
- [ ] **IdentifierType property preserved**: `IdentifierType` property remains unchanged, returning `NoIdentifierType` enum correctly for both foedselsnummer and D-nummer.
- [ ] **D-nummer offset handling verified**: Validation correctly handles 40-day offset (day range 41-71 indicates D-nummer).
- [ ] **Century inference rules preserved**: All five century derivation rules work correctly with new union pattern.
- [ ] **Private validation-bypass constructor updated**: Constructor with `ValidationMode` parameter continues to support validation bypass for post-validated instance creation.
- [ ] **All tests pass**: Existing unit tests in `NoFoedselsnummerTests.cs` pass without modification (or with minimal adjustments to use new union API).
- [ ] **JSON converter verified**: `NoFoedselsnummerJsonConverter` works correctly with updated constructor.
- [ ] **Old enum and extensions removed**: `NoFoedselsnummerValidationResult` enum and `NoFoedselsnummerValidationResultExtensions` class deleted.
- [ ] **Documentation updated**: XML comments in `NoFoedselsnummer.cs` reflect new exception types and union patterns.
- [ ] **Error messages verified**: All required error messages present and correct in `Messages` class.
- [ ] **Weighted modulus 11 verified**: Both C1 and C2 check digit validation works correctly with new pattern.
- [ ] **Code review approved**: Migration code reviewed for consistency with other union-pattern implementations (GB patient numbers, US identifiers, CA SIN, MX CURP, SE Personnummer).

---

## Validation Rules

The following validation rules from the current implementation must be preserved and mapped to appropriate union cases:

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace → `EmptyValue`.
2. **Length Validation**: Value must be 11 or 12 characters in length → `InvalidLength`.
3. **Separator Validation**: If 12 characters, position 6 must not be a digit character (0-9) → `InvalidSeparator`.
4. **Character Validation**: All non-separator characters must be ASCII digits (0-9) → `InvalidCharacter`.
5. **Date of Birth Validation**: First 6 digits must represent a valid date in DDMMYY format, with century derived from individual number using five rules → `InvalidDateOfBirth`.
6. **D-nummer Offset Handling**: If day component is 41-71 (D-nummer offset), subtract 40 from day before date validation → date must be valid after offset.
7. **Check Digit Validation**: 
   - Positions 9-10 (zero-based) must be valid weighted modulus 11 check digits (C1 and C2) → `InvalidChecksum`.
   - C1 check digit: calculated from positions 0-8 (excluding separator)
   - C2 check digit: calculated from positions 0-9 (excluding separator)

### Century Derivation Rules (Must be evaluated in order)

1. **Rule 1**: If individual number ≥ 500 AND ≤ 749 AND two-digit year ≥ 54 → century = 1800
2. **Rule 2**: If individual number < 500 → century = 1900
3. **Rule 3**: If individual number ≥ 900 AND two-digit year ≥ 40 → century = 1900
4. **Rule 4**: If individual number ≥ 500 AND two-digit year ≤ 39 → century = 2000
5. **Rule 5**: Otherwise invalid date of birth

Valid date range: January 1, 1854 to December 31, 2039.

---

## Implementation Notes

### Key Differences from Current Pattern

| Aspect | Current (Enum) | New (Union) |
|--------|----------------|------------|
| **Validation Return Type** | `NoFoedselsnummerValidationResult` enum | `ValidationResult` discriminated union |
| **Error Context** | Implicit (via extension method lookup) | Explicit: each union case can carry `Message` property and context |
| **Exception Type** | `KfValidationException<NoFoedselsnummerValidationResult>` | `UKfValidationException<ValidationError>` |
| **Message Resolution** | Runtime: enum methods mapped in extension | Build-time: included in union case definition |
| **Exhaustiveness** | Compiler warnings if switch incomplete | Compiler errors if union cases unhandled |
| **IdentifierType Property** | Returns `NoIdentifierType` enum (unchanged) | Returns `NoIdentifierType` enum (unchanged) |
| **Check Digit Algorithm** | Weighted modulus 11 (unchanged) | Weighted modulus 11 (unchanged) |

### Reference Implementations

The following implementations serve as templates for this migration:

- **`GbNhsNumber.cs`**: GB patient number union pattern; establishes conventions for discriminated union structure.
- **`UsSocialSecurityNumber.cs`**: US identifier with similar validation complexity and multiple validation rules.
- **`CaSocialInsuranceNumber.cs`**: Canadian identifier with Luhn check digit handling and formatted/unformatted parsing.
- **`MxCurp.cs`**: Mexican identifier with complex date logic and century inference.
- **`SePersonnummer.cs`**: Swedish identifier with similar structure (11-12 characters, separator, check digits, binary identifier type).

### Special Considerations

**Century Derivation Complexity**: The `NoFoedselsnummer` class has more complex century derivation rules than most other identifier types. This logic must be preserved exactly as-is. The rules must be evaluated in the specific order (1-5) to handle overlapping ranges correctly, especially for individual numbers in the 500-749 range.

**Check Digit Algorithm**: The weighted modulus 11 algorithm uses two separate calculation weights (`_c1Weights` and `_c2Weights`). Both must continue to work correctly post-migration.

**D-nummer Offset**: The 40-day offset (day range 41-71) that distinguishes D-nummer from foedselsnummer must be properly handled in the validation logic. The offset is removed before date validation, but the `IdentifierType` property must correctly identify the type.

---

## Testing Strategy

- **Existing Test Suite**: All tests in `NoFoedselsnummerTests.cs` should continue to exercise the same validation scenarios and test data.
- **Century Rules Coverage**: Ensure tests covering all five century derivation rules continue to pass, especially edge cases at year/individual-number boundaries.
- **D-nummer Coverage**: Ensure tests for D-nummer (day offset 40-71) validation remain comprehensive.
- **Check Digit Coverage**: Ensure both C1 and C2 check digit validation tests pass.
- **Union Pattern Tests**: Add specific tests for union case exhaustiveness if not already covered by parametrized test data.
- **Backward Compatibility**: Verify that all public methods maintain their behavioral contracts.

---

## Definition of Ready

Before implementation:

- [ ] Current `NoFoedselsnummer` implementation reviewed and validation rules documented.
- [ ] `NoFoedselsnummerTests.cs` test coverage analyzed to ensure migration test scope.
- [ ] Century derivation rules (Rules 1-5) understood and documented.
- [ ] Union pattern from GB patient number implementations understood and agreed upon.
- [ ] Messages class reviewed for required keys (all should already exist).
- [ ] Team consensus on `ValidationError` and `ValidationResult` union case definitions.

---

## Files to Modify/Delete

### Modify
- `src/KfAccountNumbers/Governmental/Europe/NoFoedselsnummer.cs` - Update to use union pattern, refactor validation logic, update constructor/Create method.
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/NoFoedselsnummerTests.cs` - Update test helpers and assertions to work with union types (tests themselves remain largely unchanged).

### Delete
- `src/KfAccountNumbers/Governmental/Europe/NoFoedselsnummerValidationResult.cs` - Contains enum and extensions (no longer needed post-migration).

### Verify
- `src/KfAccountNumbers/Governmental/Europe/NoFoedselsnummerJsonConverter.cs` - Ensure compatibility with updated constructor.
- `Messages` class - Verify all required error message keys exist.

---

## Success Metrics

- All existing unit tests pass post-migration without scenario changes.
- Build succeeds with no compilation errors or warnings related to this change.
- Code review approves migration for consistency with union-pattern implementations.
- No performance regression detected (check digit validation, date parsing, and century derivation logic unchanged).
- All century derivation edge cases continue to work correctly.
