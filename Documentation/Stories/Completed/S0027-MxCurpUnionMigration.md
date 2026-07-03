# S0027: Migrate MxCurp to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 6 points  
**Related:** S0019 (GbNhsNumber), S0020 (GbHcNumber), S0021 (GbChiNumber), S0022 (GbPatientNumber), S0023 (UsSocialSecurityNumber), S0024 (UsNationalProviderIdentifier), S0025 (UsIndividualTaxpayerIdentificationNumber), S0026 (CaSocialInsuranceNumber), E01-UnionMigration

---

## Overview

Modernize the `MxCurp` (Mexican Clave Única de Registro de Población) type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `MxCurp` with the new validation architecture introduced in all other identifier types and improves type safety, error context, and API consistency across the codebase.

---

## Business Value

* **Type Safety**: Replace error enum values with discriminated union cases that can carry contextual error information (e.g., which validation rule failed, detailed error messages).
* **Consistency**: Standardize the validation API across all identifier types in the codebase (GB patient numbers, US identifiers, CA SIN, and MX CURP).
* **Developer Experience**: Enable compiler-checked exhaustiveness on validation results and eliminate string-based error lookups.
* **Maintainability**: Reduce reliance on extension methods and message lookups; move validation context closer to the error definition.

---

## Requirements

### Functional Requirements

#### 1. Replace Enum with Union Pattern
Replace the current `MxCurpValidationResult` enum with a discriminated union mirroring the established validation unions:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidAlphabeticCharacter,
   InvalidDateOfBirth,
   InvalidGender,
   InvalidState,
   InvalidHomoclave,
   InvalidCheckDigit)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidAlphabeticCharacter,
   InvalidDateOfBirth,
   InvalidGender,
   InvalidState,
   InvalidHomoclave,
   InvalidCheckDigit)
{
}
```

Each union case must be a record type (e.g., `record EmptyValue(String Message)`) with a `Message` property to carry localized error descriptions.

#### 2. Update Constructor Exception Documentation
Modify the constructor exception documentation to reference the new union types:
* Change `KfValidationException<MxCurpValidationResult>` to `UKfValidationException<ValidationError>`.
* Ensure all exception cases map to corresponding union cases.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types instead of enum values.
- Each validation failure should instantiate the appropriate union case with a localized message via `Messages` class.
- The success case should return `default(ValidValue)`.
- Preserve the case-insensitive validation (the result is normalized to upper-case in the `Value` property).

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<MxCurp, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing `UKfValidationException<ValidationError>`.
- Maintain support for validation bypass via `ValidationMode.BypassValidation` for internal use (already in place).
- Ensure backward compatibility (same behavioral contract; only the types change).

#### 5. Remove Old Extension Methods
- Delete or deprecate `MxCurpValidationResultExtensions` methods (`ToErrorDescription`, `ToValidationException`).
- Delete the `MxCurpValidationResult` enum (after migration is complete).
- Create a new file `MxCurpValidationError.cs` to hold the new union types and any supporting methods.

#### 6. JSON Serialization
- Verify that `MxCurpJsonConverter` continues to work with the updated constructor.
- If needed, update the converter to handle internal validation-bypass logic correctly.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- The public API surface must remain functionally equivalent.
- Exception types and messages should be consistent with current behavior (though exception structure improves).
- Deprecated types (if any) should be marked with `[Obsolete]` attributes.

#### 2. Code Organization
- Move union types to a new file: `src/KfAccountNumbers/Governmental/NorthAmerica/MxCurpValidationError.cs`.
- Keep the main class file `MxCurp.cs` focused on business logic.
- Maintain consistent naming with other identifier types (e.g., `ValidationError`, `ValidationResult`).

#### 3. Error Messaging
- All error messages must be stored in the `Messages` class and reference-able via `Messages.MxCurp*` keys.
- Ensure parity between current error descriptions and new union case messages.

---

## Acceptance Criteria

### Definition of Done

- [ ] **Union types created**: `ValidationError` and `ValidationResult` discriminated unions defined in `MxCurpValidationError.cs`.
- [ ] **Constructor updated**: Exception documentation and validation logic updated to use `UKfValidationException<ValidationError>`.
- [ ] **Validate method refactored**: Returns `ValidationResult` union; all validation failures instantiate appropriate union cases.
- [ ] **Create method updated**: Signature returns `CreateResult<MxCurp, ValidationError>`.
- [ ] **All tests pass**: Existing unit tests pass without modification (or with minimal adjustments to use new union API).
- [ ] **JSON converter verified**: `MxCurpJsonConverter` works correctly with updated constructor.
- [ ] **Case-insensitive validation verified**: Validation remains case-insensitive; normalization to upper-case continues to work.
- [ ] **Old enum and extensions removed**: `MxCurpValidationResult` enum and extensions deleted.
- [ ] **Documentation updated**: XML comments in `MxCurp.cs` reflect new exception types.
- [ ] **Error messages verified**: All error messages present and correct in `Messages` class.
- [ ] **Date-of-birth and gender extraction verified**: Properties `DateOfBirth`, `GenderCode`, and `StateCode` continue to work correctly.
- [ ] **Code review approved**: Migration code reviewed for consistency with other identifier implementations.

---

## Validation Rules

The following validation rules from the current implementation must be preserved:

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace characters → `EmptyValue`.
2. **Length Validation**: Value must be exactly 18 characters in length → `InvalidLength`.
3. **Alphabetic Character Validation**: Positions 0-3 and 13-15 (zero-based) must contain alphabetic characters → `InvalidAlphabeticCharacter`.
4. **Date of Birth Validation**: Positions 4-9 (zero-based) must contain a valid 6-digit date in YYMMDD format → `InvalidDateOfBirth`.
5. **Gender Code Validation**: Position 10 (zero-based) must be H (male), M (female), or X (non-binary) → `InvalidGender`.
6. **State Code Validation**: Positions 11-12 (zero-based) must contain a valid Mexican state code → `InvalidState`.
7. **Homoclave Validation**: Position 16 (zero-based) must be an alphanumeric character (A-Z, 0-9) → `InvalidHomoclave`.
8. **Check Digit Validation**: Position 17 (zero-based) must be a digit character (0-9) → `InvalidCheckDigit`.

---

## Implementation Notes

### Key Differences from Current Pattern

| Aspect | Current (Enum) | New (Union) |
|--------|----------------|------------|
| **Validation Return Type** | `MxCurpValidationResult` enum | `ValidationResult` discriminated union |
| **Error Context** | Implicit (via extension method lookup) | Explicit: each union case can carry `Message` property |
| **Exception Type** | `KfValidationException<MxCurpValidationResult>` | `UKfValidationException<ValidationError>` |
| **Message Resolution** | Runtime: `validationResult.ToErrorDescription()` | Build-time: included in union case definition |
| **Exhaustiveness** | Compiler warnings if switch incomplete | Compiler errors if union cases unhandled |

### CURP-Specific Considerations

The `MxCurp` class includes several CURP-specific features that must remain intact:

- **Case-Insensitive Validation**: The validation logic works with case-insensitive input, but the stored `Value` property is always normalized to upper-case. This behavior must be preserved exactly.
- **Complex Date-of-Birth Logic**: The `DateOfBirth` property uses the homoclave character (position 16) to infer the century of birth:
  - Digit homoclave (0-9) → birth in 1900-1999 century
  - Alphabetic homoclave (A-Z) → birth in 2000-2099 century
  - Leap year validation depends on the inferred century (1900 is not a leap year; 2000 is)
- **Derived Properties**: The `DateOfBirth`, `GenderCode`, and `StateCode` properties extract and derive data from the CURP. These must continue to work correctly.
- **Validation Order**: The validation checks multiple positions in sequence; this order should be preserved for consistent error reporting.
- **Undocumented Algorithm**: The check digit algorithm used by RENAPO is not published; validation only confirms the check digit is a digit character.

### Inspiration / Reference Implementation

Refer to the following completed migrations for patterns and best practices:
- `src/KfAccountNumbers/Governmental/Europe/GbNhsNumber.cs` — constructor exception handling and `ValidationResult` union.
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsSocialSecurityNumber.cs` — US identifier with case handling considerations.
- `src/KfAccountNumbers/Governmental/NorthAmerica/CaSocialInsuranceNumber.cs` — identifier with date and check digit considerations (completed in S0026).

---

## Testing Strategy

1. **Unit Test Updates**: Existing unit tests for `MxCurp` should pass without modification because the public behavior is identical.
2. **Validation Result Tests**: Add targeted tests to verify each union case is instantiated correctly with the appropriate message.
3. **Exception Mapping**: Verify that each `ValidationError` case correctly maps to an `UKfValidationException` with the right message and context.
4. **JSON Serialization Tests**: Ensure `MxCurpJsonConverter` continues to work (constructor validation is internal).
5. **Case Sensitivity Tests**: Verify that validation remains case-insensitive and that the resulting `Value` is always upper-case.
6. **Date-of-Birth Tests**: Ensure `DateOfBirth` property correctly interprets the homoclave character to determine century, including leap year boundary cases.
7. **Gender and State Tests**: Verify that `GenderCode` and `StateCode` properties continue to extract correct values.
8. **Leap Year Boundary Tests**: Test the special leap year logic for century-dependent date validation (1900 vs. 2000).

---

## Files to Create / Modify

### Files to Create
- `src/KfAccountNumbers/Governmental/NorthAmerica/MxCurpValidationError.cs` — New union type definitions.

### Files to Modify
- `src/KfAccountNumbers/Governmental/NorthAmerica/MxCurp.cs` — Refactor constructor, `Validate`, and `Create` methods.
- `src/KfAccountNumbers/Governmental/NorthAmerica/MxCurpJsonConverter.cs` — Verify compatibility (may not need changes).
- `tests/KfAccountNumbers.Tests.Unit/Governmental/NorthAmerica/MxCurpTests.cs` — Update assertions if needed (optional if behavior is identical).

### Files to Delete
- `src/KfAccountNumbers/Governmental/NorthAmerica/MxCurpValidationResult.cs` — Old enum and extensions.

### Files to Review (No Changes Expected)
- `src/KfAccountNumbers/Messages.cs` — Verify all `MxCurp*` message keys exist; add any missing keys.
- `src/KfAccountNumbers/Governmental/NorthAmerica/MxCurpJsonConverter.cs` — Ensure it works with updated constructor.

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|-----------|
| **Breaking Change in Public API** | Low | High | Maintain same exception types; only internal representation changes. Use `CreateResult` pattern for non-throwing API. |
| **Incomplete Union Case Handling** | Medium | Medium | Thorough code review against other identifier implementations; compiler warnings catch most cases. |
| **Missing Error Messages** | Low | Medium | Audit `Messages` class for all `MxCurp*` keys before merge; run with culture-specific tests. |
| **JSON Serialization Regression** | Low | High | Run existing JSON serialization tests; verify converter works with new exception types. |
| **Case Sensitivity Regression** | Low | High | Verify validation remains case-insensitive; ensure `Value` normalization to upper-case continues. |
| **Date-of-Birth Logic Regression** | Low | High | Double-check homoclave-dependent century inference; test all leap year boundary cases (especially 1900 non-leap and 2000 leap). |
| **Property Extraction Regression** | Low | Medium | Ensure `DateOfBirth`, `GenderCode`, and `StateCode` properties extract correct values. |
| **Test Coverage Gaps** | Medium | Medium | Add new tests for each union case; verify exception mapping; test century-dependent leap year rules; use code coverage tools. |

---

## Dependencies

- **Completed**: C# 12 discriminated unions available (language feature).
- **Prerequisite**: E01-UnionMigration backlog item or equivalent environment setup.
- **Related Stories**: S0019–S0026 (GB patient numbers and North American identifiers using union pattern).

---

## Definition of Done (Story Level)

1. **Implementation Complete**: All code changes applied and passing local build.
2. **Tests Green**: All existing and new unit tests passing, including date-of-birth and case-sensitivity tests.
3. **Code Review**: Peer review completed; linting and static analysis clean.
4. **Documentation**: XML comments updated; README or release notes mention migration if needed.
5. **Backwards Compatibility Verified**: Functional behavior unchanged from consumer perspective, including case handling and date derivation.
6. **Date Logic Verified**: Century inference via homoclave character works correctly for both 1900s and 2000s birth dates.
7. **Ready for Merge**: Approved for merge to main branch.

---

## Estimated Effort

**6 story points** (Medium-to-high complexity):
- ~1 point: Create new union types and validation error file.
- ~2.5 points: Refactor `Validate`, `Create`, and constructor; update exception handling; preserve critical validation logic (especially case-insensitive handling and complex date-of-birth validation).
- ~1.5 points: Verify JSON serialization; verify derived property extraction (`DateOfBirth`, `GenderCode`, `StateCode`); update tests; test leap year rules.
- ~0.5 point: Clean up old enum; review documentation.
- ~0.5 point: Code review and final validation.

**Actual effort may vary** based on test complexity, derived property extraction verification, and the need to extensively test century-dependent date validation logic.

---

## Additional Context

The migration of identifier validation to discriminated unions is part of a broader modernization effort (E01-UnionMigration) to improve type safety and API consistency. This story provides a concrete application of the union pattern to the `MxCurp` class, completing the modernization of all core North American identifier types.

The CURP validation is particularly complex due to:
- **Multi-Component Structure**: The 18-character identifier contains distinct components (initials, date, gender, state, internal consonants, homoclave, check digit) with different validation rules.
- **Homoclave-Dependent Logic**: The century-of-birth inference depends on whether the homoclave is numeric (1900s) or alphabetic (2000s), which affects leap year validation.
- **Case-Insensitive Input with Upper-Case Output**: The input is case-insensitive but the stored value is always upper-case.
- **Undocumented Algorithm**: The RENAPO check digit algorithm is not published, so only format validation is performed.

These characteristics require careful attention during implementation to ensure all existing functionality is preserved, especially the critical date-of-birth derivation logic and case handling.

See the related stories (S0019–S0026) for completed examples of this pattern in action.
