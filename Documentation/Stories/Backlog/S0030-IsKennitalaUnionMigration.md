# S0030: Migrate IsKennitala to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 5 points  
**Related:** S0019 (GbNhsNumber), S0022 (GbPatientNumber), S0023-S0029 (Union migrations), E01-UnionMigration

---

## Overview

Modernize the `IsKennitala` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `IsKennitala` with the new validation architecture introduced in the union-pattern migration initiative and improves type safety, error context, and API consistency across the codebase.

---

## Business Value

* **Type Safety**: Replace error enum values with discriminated union cases that can carry contextual error information (e.g., which character position, what separator was found, validation context).
* **Consistency**: Standardize the validation API across all identifier types (GB patient numbers, US identifiers, European identifiers).
* **Developer Experience**: Enable compiler-checked exhaustiveness on validation results and eliminate string-based error lookups.
* **Maintainability**: Reduce reliance on extension methods and message lookups; move validation context closer to the error definition.
* **Compatibility**: Align with the modern union-pattern approach used by `GbNhsNumber`, `UsSocialSecurityNumber`, `CaSocialInsuranceNumber`, `MxCurp`, `SePersonnummer`, `NoFoedselsnummer`, and other recent implementations.

---

## Requirements

### Functional Requirements

#### 1. Replace Enum with Union Pattern
Replace the current `IsKennitalaValidationResult` enum with discriminated unions mirroring the pattern established by GB patient number validation unions:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigit,
   InvalidSeparator,
   InvalidCentury,
   InvalidDateOfBirth)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigit,
   InvalidSeparator,
   InvalidCentury,
   InvalidDateOfBirth)
{
}
```

Each union case must be a record type (e.g., `record EmptyValue(String Message)`) with a `Message` property to carry localized error descriptions. Some union cases may carry additional context such as:
- `InvalidCharacter`: position and character encountered
- `InvalidSeparator`: position and character encountered
- `InvalidCentury`: the century indicator found

#### 2. Update Constructor Exception Documentation
Modify the constructor exception documentation to reference the new union types:
* Change `KfValidationException<IsKennitalaValidationResult>` to `UKfValidationException<ValidationError>`.
* Ensure all exception cases map to corresponding union cases.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types instead of enum values.
- Each validation failure should instantiate the appropriate union case with a localized message via `Messages` class.
- The success case should return `default(ValidValue)`.

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<IsKennitala, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing `UKfValidationException<ValidationError>`.
- Ensure private validation-bypass constructor remains functional for post-validation object creation.
- Ensure backward compatibility (same behavioral contract; only the types change).

#### 5. Handle IdentifierType Property
- The `IdentifierType` property (which returns `IsIdentifierType` enum: `Einstaklingur` or `Fyrirtaeki`) should remain unchanged.
- Validation logic must correctly distinguish between personal (Einstaklingur) and company (Fyrirtaeki) identifiers during all validation checks.
- The 40-day offset logic for Fyrirtaeki (day component 41-71) must be preserved in validation.

#### 6. Preserve Weighted Modulus 11 Check Digit Validation
- The check digit algorithm (weighted modulus 11) must be preserved exactly as-is.
- The check digit is calculated from the first 8 digits (DDMMYYRR), excluding the separator if present.
- The check digit must be the 9th character (position 8) or 10th character (position 9) depending on separator presence.

#### 7. Preserve Century Indicator Validation
- The century indicator must be in the rightmost position (character '9' for 1900s or '0' for 2000s).
- Valid date range (January 1, 1900 to December 31, 2099) must be maintained.
- The 40-day Fyrirtaeki offset must be subtracted before validating the date.

#### 8. Remove Old Extension Methods
- Delete the `IsKennitalaValidationResult` enum (after migration is complete).
- Delete the `IsKennitalaValidationResultExtensions` class (after migration is complete).
- Optionally create a new supporting file if needed for organization.

#### 9. JSON Serialization
- Verify that `IsKennitalaJsonConverter` continues to work with the updated constructor.
- If needed, update the converter to handle internal validation-bypass logic correctly.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- The public API surface must remain functionally equivalent.
- Exception types and messages should be consistent with current behavior (though exception structure improves).
- Deprecated types (if any) should be marked with `[Obsolete]` attributes.
- The `IdentifierType` property and its return type (`IsIdentifierType` enum) remain unchanged.

#### 2. Code Organization
- Integrate union types directly into `IsKennitala.cs` (similar to GB patient number implementations).
- Keep the main class file focused on business logic.
- Maintain consistent naming with other union-migrated types (e.g., `ValidationError`, `ValidationResult`).

#### 3. Error Messaging
- All error messages must be stored in the `Messages` class and referenceable via `Messages.IsKennitala*` keys.
- Ensure parity between current error descriptions and new union case messages:
  - `Empty` → `Messages.IsKennitalaEmpty`
  - `InvalidLength` → `Messages.IsKennitalaInvalidLength`
  - `InvalidCharacter` → `Messages.IsKennitalaInvalidCharacter`
  - `InvalidCheckDigit` → `Messages.IsKennitalaInvalidCheckDigit`
  - `InvalidSeparator` → `Messages.IsKennitalaInvalidSeparator`
  - `InvalidCentury` → `Messages.IsKennitalaInvalidCentury`
  - `InvalidDateOfBirth` → `Messages.IsKennitalaInvalidDateOfBirth`

#### 4. Test Coverage
- All existing unit tests in `IsKennitalaTests.cs` must continue to pass.
- Test method signatures may be updated to work with union return types, but test scenarios must remain equivalent.
- Ensure all validation paths are still covered post-migration, including:
  - Both Einstaklingur (personal) and Fyrirtaeki (company) validation
  - Separator validation (both with and without separator)
  - Weighted modulus 11 check digit validation
  - Century indicator validation

---

## Acceptance Criteria

### Definition of Done

- [ ] **Union types created**: `ValidationError` and `ValidationResult` discriminated unions defined as nested types within `IsKennitala.cs`.
- [ ] **Constructor updated**: Exception documentation and validation logic updated to use `UKfValidationException<ValidationError>`.
- [ ] **Validate method refactored**: Returns `ValidationResult` union; all validation failures instantiate appropriate union cases.
- [ ] **Create method updated**: Signature returns `CreateResult<IsKennitala, ValidationError>`.
- [ ] **IdentifierType property preserved**: `IdentifierType` property remains unchanged, returning `IsIdentifierType` enum correctly for both Einstaklingur and Fyrirtaeki.
- [ ] **Fyrirtaeki offset handling verified**: Validation correctly handles 40-day offset (day range 41-71 indicates Fyrirtaeki).
- [ ] **Check digit validation preserved**: Weighted modulus 11 algorithm works correctly with new union pattern.
- [ ] **Century indicator validation verified**: Century indicator ('9' or '0') is correctly validated in rightmost position.
- [ ] **Private validation-bypass constructor updated**: Constructor with `ValidationMode` parameter continues to support validation bypass for post-validated instance creation.
- [ ] **All tests pass**: Existing unit tests in `IsKennitalaTests.cs` pass without modification (or with minimal adjustments to use new union API).
- [ ] **JSON converter verified**: `IsKennitalaJsonConverter` works correctly with updated constructor.
- [ ] **Old enum and extensions removed**: `IsKennitalaValidationResult` enum and `IsKennitalaValidationResultExtensions` class deleted.
- [ ] **Documentation updated**: XML comments in `IsKennitala.cs` reflect new exception types and union patterns.
- [ ] **Error messages verified**: All required error messages present and correct in `Messages` class.
- [ ] **Code review approved**: Migration code reviewed for consistency with other union-pattern implementations (GB patient numbers, US identifiers, European identifiers).

---

## Validation Rules

The following validation rules from the current implementation must be preserved and mapped to appropriate union cases:

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace → `EmptyValue`.
2. **Length Validation**: Value must be 10 or 11 characters in length → `InvalidLength`.
3. **Separator Validation**: If 11 characters, position 6 must not be a digit character (0-9) → `InvalidSeparator`.
4. **Character Validation**: All non-separator characters must be ASCII digits (0-9) → `InvalidCharacter`.
5. **Century Indicator Validation**: Rightmost character must be '9' (1900s) or '0' (2000s) → `InvalidCentury`.
6. **Check Digit Validation**: Character at position 8 (zero-based, excluding separator) must be a valid weighted modulus 11 check digit → `InvalidCheckDigit`.
7. **Date of Birth Validation**: 
   - First 6 digits must represent a valid date in DDMMYY format
   - If Fyrirtaeki (day 41-71), subtract 40 from day before validation
   - Valid date range: January 1, 1900 to December 31, 2099 → `InvalidDateOfBirth`.
8. **Fyrirtaeki Offset Handling**: If day component is 41-71 (Fyrirtaeki offset), subtract 40 from day before date validation → date must be valid after offset.

---

## Implementation Notes

### Key Differences from Current Pattern

| Aspect | Current (Enum) | New (Union) |
|--------|----------------|------------|
| **Validation Return Type** | `IsKennitalaValidationResult` enum | `ValidationResult` discriminated union |
| **Error Context** | Implicit (via extension method lookup) | Explicit: each union case can carry `Message` property and context |
| **Exception Type** | `KfValidationException<IsKennitalaValidationResult>` | `UKfValidationException<ValidationError>` |
| **Message Resolution** | Runtime: enum methods mapped in extension | Build-time: included in union case definition |
| **Exhaustiveness** | Compiler warnings if switch incomplete | Compiler errors if union cases unhandled |
| **IdentifierType Property** | Returns `IsIdentifierType` enum (unchanged) | Returns `IsIdentifierType` enum (unchanged) |
| **Check Digit Algorithm** | Weighted modulus 11 (unchanged) | Weighted modulus 11 (unchanged) |

### Reference Implementations

The following implementations serve as templates for this migration:

- **`GbNhsNumber.cs`**: GB patient number union pattern; establishes conventions for discriminated union structure.
- **`UsSocialSecurityNumber.cs`**: US identifier with similar validation complexity.
- **`CaSocialInsuranceNumber.cs`**: Canadian identifier with check digit handling and formatted/unformatted parsing.
- **`MxCurp.cs`**: Mexican identifier with complex date logic.
- **`SePersonnummer.cs`**: Swedish identifier with similar structure (11-12 characters, separator, check digits, binary identifier type).
- **`NoFoedselsnummer.cs`**: Norwegian identifier with similar structure and 40-day offset logic for D-nummer.

### Special Considerations

**Fyrirtaeki Offset Handling**: The 40-day offset (day range 41-71) that distinguishes Fyrirtaeki (company) from Einstaklingur (personal) must be properly handled in the validation logic. The offset is removed before date validation, but the `IdentifierType` property must correctly identify the type.

**Check Digit Position**: The weighted modulus 11 check digit is in the position before the century indicator. Its exact position depends on whether a separator is present (position 8 without separator, position 9 with separator at position 6).

**Valid Date Range**: The date range (1900-2099) is fixed and encoded in the century indicator ('9' for 1900s, '0' for 2000s), unlike some other identifiers with variable century derivation rules.

---

## Testing Strategy

- **Existing Test Suite**: All tests in `IsKennitalaTests.cs` should continue to exercise the same validation scenarios and test data.
- **Fyrirtaeki Coverage**: Ensure tests for Fyrirtaeki (40-day offset) validation remain comprehensive.
- **Check Digit Coverage**: Ensure weighted modulus 11 check digit validation tests pass.
- **Century Indicator Coverage**: Ensure tests for both '9' (1900s) and '0' (2000s) century indicators pass.
- **Separator Handling**: Ensure tests for both formatted (with separator) and unformatted (without separator) parsing pass.
- **Union Pattern Tests**: Add specific tests for union case exhaustiveness if not already covered by parametrized test data.
- **Backward Compatibility**: Verify that all public methods maintain their behavioral contracts.

---

## Definition of Ready

Before implementation:

- [ ] Current `IsKennitala` implementation reviewed and validation rules documented.
- [ ] `IsKennitalaTests.cs` test coverage analyzed to ensure migration test scope.
- [ ] Fyrirtaeki offset logic (40-day addition) understood and documented.
- [ ] Union pattern from GB patient number implementations understood and agreed upon.
- [ ] Messages class reviewed for required keys (all should already exist).
- [ ] Team consensus on `ValidationError` and `ValidationResult` union case definitions.

---

## Files to Modify/Delete

### Modify
- `src/KfAccountNumbers/Governmental/Europe/IsKennitala.cs` - Update to use union pattern, refactor validation logic, update constructor/Create method.
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/IsKennitalaTests.cs` - Update test helpers and assertions to work with union types (tests themselves remain largely unchanged).

### Delete
- `src/KfAccountNumbers/Governmental/Europe/IsKennitalaValidationResult.cs` - Contains enum and extensions (no longer needed post-migration).

### Verify
- `src/KfAccountNumbers/Governmental/Europe/IsKennitalaJsonConverter.cs` - Ensure compatibility with updated constructor.
- `Messages` class - Verify all required error message keys exist.

---

## Success Metrics

- All existing unit tests pass post-migration without scenario changes.
- Build succeeds with no compilation errors or warnings related to this change.
- Code review approves migration for consistency with union-pattern implementations.
- No performance regression detected (check digit validation and date parsing logic unchanged).
- All Fyrirtaeki offset edge cases continue to work correctly.
