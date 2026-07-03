# S0026: Migrate CaSocialInsuranceNumber to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 5 points  
**Related:** S0019 (GbNhsNumber), S0020 (GbHcNumber), S0021 (GbChiNumber), S0022 (GbPatientNumber), S0023 (UsSocialSecurityNumber), S0024 (UsNationalProviderIdentifier), S0025 (UsIndividualTaxpayerIdentificationNumber), E01-UnionMigration

---

## Overview

Modernize the `CaSocialInsuranceNumber` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `CaSocialInsuranceNumber` with the new validation architecture introduced in all other identifier types (GB patient numbers and other US identifiers) and improves type safety, error context, and API consistency across the codebase.

---

## Business Value

* **Type Safety**: Replace error enum values with discriminated union cases that can carry contextual error information (e.g., which validation rule failed, detailed error messages).
* **Consistency**: Standardize the validation API across all identifier types in the codebase (GB patient numbers, US SSN, US NPI, US ITIN, and CA SIN).
* **Developer Experience**: Enable compiler-checked exhaustiveness on validation results and eliminate string-based error lookups.
* **Maintainability**: Reduce reliance on extension methods and message lookups; move validation context closer to the error definition.

---

## Requirements

### Functional Requirements

#### 1. Replace Enum with Union Pattern
Replace the current `CaSocialInsuranceNumberValidationResult` enum with a discriminated union mirroring the established validation unions:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidSeparator,
   InvalidProvince,
   InvalidCheckDigit)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidSeparator,
   InvalidProvince,
   InvalidCheckDigit)
{
}
```

Each union case must be a record type (e.g., `record EmptyValue(String Message)`) with a `Message` property to carry localized error descriptions.

#### 2. Update Constructor Exception Documentation
Modify the constructor exception documentation to reference the new union types:
* Change `KfValidationException<CaSocialInsuranceNumberValidationResult>` to `UKfValidationException<ValidationError>`.
* Ensure all exception cases map to corresponding union cases.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types instead of enum values.
- Each validation failure should instantiate the appropriate union case with a localized message via `Messages` class.
- The success case should return `default(ValidValue)`.

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<CaSocialInsuranceNumber, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing `UKfValidationException<ValidationError>`.
- Maintain support for validation bypass via `ValidationMode.BypassValidation` for internal use (already in place).
- Ensure backward compatibility (same behavioral contract; only the types change).

#### 5. Remove Old Extension Methods
- Delete or deprecate `CaSocialInsuranceNumberValidationResultExtensions` methods (`ToErrorDescription`, `ToValidationException`).
- Delete the `CaSocialInsuranceNumberValidationResult` enum (after migration is complete).
- Create a new file `CaSocialInsuranceNumberValidationError.cs` to hold the new union types and any supporting methods.

#### 6. JSON Serialization
- Verify that `CaSocialInsuranceNumberJsonConverter` continues to work with the updated constructor.
- If needed, update the converter to handle internal validation-bypass logic correctly.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- The public API surface must remain functionally equivalent.
- Exception types and messages should be consistent with current behavior (though exception structure improves).
- Deprecated types (if any) should be marked with `[Obsolete]` attributes.

#### 2. Code Organization
- Move union types to a new file: `src/KfAccountNumbers/Governmental/NorthAmerica/CaSocialInsuranceNumberValidationError.cs`.
- Keep the main class file `CaSocialInsuranceNumber.cs` focused on business logic.
- Maintain consistent naming with other identifier types (e.g., `ValidationError`, `ValidationResult`).

#### 3. Error Messaging
- All error messages must be stored in the `Messages` class and reference-able via `Messages.CaSin*` keys.
- Ensure parity between current error descriptions and new union case messages.

---

## Acceptance Criteria

### Definition of Done

- [ ] **Union types created**: `ValidationError` and `ValidationResult` discriminated unions defined in `CaSocialInsuranceNumberValidationError.cs`.
- [ ] **Constructor updated**: Exception documentation and validation logic updated to use `UKfValidationException<ValidationError>`.
- [ ] **Validate method refactored**: Returns `ValidationResult` union; all validation failures instantiate appropriate union cases.
- [ ] **Create method updated**: Signature returns `CreateResult<CaSocialInsuranceNumber, ValidationError>`.
- [ ] **All tests pass**: Existing unit tests pass without modification (or with minimal adjustments to use new union API).
- [ ] **JSON converter verified**: `CaSocialInsuranceNumberJsonConverter` works correctly with updated constructor.
- [ ] **Format method verified**: The `Format(String mask)` method continues to work correctly.
- [ ] **Old enum and extensions removed**: `CaSocialInsuranceNumberValidationResult` enum and extensions deleted.
- [ ] **Documentation updated**: XML comments in `CaSocialInsuranceNumber.cs` reflect new exception types.
- [ ] **Error messages verified**: All error messages present and correct in `Messages` class.
- [ ] **Check digit algorithm verified**: Luhn check digit validation (with optional mask support) remains unchanged.
- [ ] **Code review approved**: Migration code reviewed for consistency with other identifier implementations.

---

## Validation Rules

The following validation rules from the current implementation must be preserved:

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace characters → `EmptyValue`.
2. **Length Validation**: Value must be 9 characters or 11 characters (with separators) → `InvalidLength`.
3. **Separator Validation**: If 11 characters, positions 3 and 7 must have identical non-digit separators → `InvalidSeparator`.
4. **Character Validation**: All non-separator characters must be ASCII digits ('0'-'9') → `InvalidCharacter`.
5. **Province Code Validation**: First digit may not be 0 or 8 → `InvalidProvince`.
6. **Check Digit Validation**: The trailing (right-most) digit must be a valid Luhn check digit calculated from the first eight digits → `InvalidCheckDigit`.

---

## Implementation Notes

### Key Differences from Current Pattern

| Aspect | Current (Enum) | New (Union) |
|--------|----------------|------------|
| **Validation Return Type** | `CaSocialInsuranceNumberValidationResult` enum | `ValidationResult` discriminated union |
| **Error Context** | Implicit (via extension method lookup) | Explicit: each union case can carry `Message` property |
| **Exception Type** | `KfValidationException<CaSocialInsuranceNumberValidationResult>` | `UKfValidationException<ValidationError>` |
| **Message Resolution** | Runtime: `validationResult.ToErrorDescription()` | Build-time: included in union case definition |
| **Exhaustiveness** | Compiler warnings if switch incomplete | Compiler errors if union cases unhandled |

### Canadian SIN-Specific Considerations

The `CaSocialInsuranceNumber` class includes several SIN-specific features that must remain intact:

- **Formatting**: The `Format(String mask)` method uses the stored raw SIN value to apply custom masks (e.g., `"___-___-___"`). This functionality is independent of validation and requires no changes.
- **Luhn Algorithm with Mask Support**: The validation uses `CheckDigitAlgorithms.Luhn.Validate()` with optional `CaSocialInsuranceNumberMask.Instance` parameter for formatted SINs. This complex integration must be preserved exactly.
- **Efficient Unformatting**: The `GetValidatedSin` method uses `ArrayPool<Char>` for efficient memory management when converting formatted to unformatted SINs. This optimization must remain unchanged.
- **Province Code Mapping**: The first digit indicates the province/territory where the SIN was registered (excluding 0 and 8). This validation logic must be preserved.

### Inspiration / Reference Implementation

Refer to the following completed migrations for patterns and best practices:
- `src/KfAccountNumbers/Governmental/Europe/GbNhsNumber.cs` — constructor exception handling and `ValidationResult` union.
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsSocialSecurityNumber.cs` — US identifier with separator validation (completed in S0023).
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsIndividualTaxpayerIdentificationNumber.cs` — US identifier with formatter method (completed in S0025).

---

## Testing Strategy

1. **Unit Test Updates**: Existing unit tests for `CaSocialInsuranceNumber` should pass without modification because the public behavior is identical.
2. **Validation Result Tests**: Add targeted tests to verify each union case is instantiated correctly with the appropriate message.
3. **Exception Mapping**: Verify that each `ValidationError` case correctly maps to an `UKfValidationException` with the right message and context.
4. **JSON Serialization Tests**: Ensure `CaSocialInsuranceNumberJsonConverter` continues to work (constructor validation is internal).
5. **Format Method Tests**: Verify that the `Format(String mask)` method continues to work correctly with various masks.
6. **Check Digit Algorithm Tests**: Ensure Luhn check digit validation (both with and without mask) produces correct results.
7. **Province Code Tests**: Verify that province code validation correctly rejects 0 and 8 as first digits.
8. **Memory Efficiency Tests**: Verify that `ArrayPool<Char>` usage in `GetValidatedSin` continues to work correctly.

---

## Files to Create / Modify

### Files to Create
- `src/KfAccountNumbers/Governmental/NorthAmerica/CaSocialInsuranceNumberValidationError.cs` — New union type definitions.

### Files to Modify
- `src/KfAccountNumbers/Governmental/NorthAmerica/CaSocialInsuranceNumber.cs` — Refactor constructor, `Validate`, and `Create` methods.
- `src/KfAccountNumbers/Governmental/NorthAmerica/CaSocialInsuranceNumberJsonConverter.cs` — Verify compatibility (may not need changes).
- `tests/KfAccountNumbers.Tests.Unit/Governmental/NorthAmerica/CaSocialInsuranceNumberTests.cs` — Update assertions if needed (optional if behavior is identical).

### Files to Delete
- `src/KfAccountNumbers/Governmental/NorthAmerica/CaSocialInsuranceNumberValidationResult.cs` — Old enum and extensions.

### Files to Review (No Changes Expected)
- `src/KfAccountNumbers/Messages.cs` — Verify all `CaSin*` message keys exist; add any missing keys.
- `src/KfAccountNumbers/Governmental/NorthAmerica/CaSocialInsuranceNumberJsonConverter.cs` — Ensure it works with updated constructor.
- `src/KfAccountNumbers/Governmental/NorthAmerica/CaSocialInsuranceNumberMask.cs` — Mask helper class (used by Luhn algorithm).

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|-----------|
| **Breaking Change in Public API** | Low | High | Maintain same exception types; only internal representation changes. Use `CreateResult` pattern for non-throwing API. |
| **Incomplete Union Case Handling** | Medium | Medium | Thorough code review against other identifier implementations; compiler warnings catch most cases. |
| **Missing Error Messages** | Low | Medium | Audit `Messages` class for all `CaSin*` keys before merge; run with culture-specific tests. |
| **JSON Serialization Regression** | Low | High | Run existing JSON serialization tests; verify converter works with new exception types. |
| **Format Method Regression** | Low | Medium | Ensure `Format` method continues to work; run all mask-formatting tests. |
| **Luhn Algorithm Regression** | Low | High | Double-check that Luhn validation with and without mask works correctly; run all check digit tests. |
| **Memory Management Regression** | Low | Medium | Verify that `ArrayPool<Char>` usage in `GetValidatedSin` continues to work; monitor memory performance. |
| **Province Code Validation Error** | Low | Medium | Ensure first digit validation correctly rejects 0 and 8; add boundary tests. |
| **Test Coverage Gaps** | Medium | Medium | Add new tests for each union case; verify exception mapping; use code coverage tools. |

---

## Dependencies

- **Completed**: C# 12 discriminated unions available (language feature).
- **Prerequisite**: E01-UnionMigration backlog item or equivalent environment setup.
- **Related Stories**: S0019–S0025 (GB patient numbers and all US identifiers using union pattern).
- **Utility**: `CheckDigitAlgorithms.Luhn` — Luhn check digit algorithm with optional mask support (must remain unchanged).

---

## Definition of Done (Story Level)

1. **Implementation Complete**: All code changes applied and passing local build.
2. **Tests Green**: All existing and new unit tests passing, including check digit and province code tests.
3. **Code Review**: Peer review completed; linting and static analysis clean.
4. **Documentation**: XML comments updated; README or release notes mention migration if needed.
5. **Backwards Compatibility Verified**: Functional behavior unchanged from consumer perspective, including formatting and check digit validation.
6. **Memory Management Verified**: No performance regressions; `ArrayPool<Char>` usage remains efficient.
7. **Ready for Merge**: Approved for merge to main branch.

---

## Estimated Effort

**5 story points** (Medium complexity):
- ~1 point: Create new union types and validation error file.
- ~2 points: Refactor `Validate`, `Create`, and constructor; update exception handling; preserve critical validation logic (especially Luhn with mask support and province code validation).
- ~1 point: Verify JSON serialization and formatting; update tests; verify check digit algorithm integration.
- ~0.5 point: Clean up old enum; review documentation; verify memory management.
- ~0.5 point: Code review and final validation.

**Actual effort may vary** based on test complexity, Luhn algorithm integration subtleties, and mask support verification requirements.

---

## Additional Context

The migration of identifier validation to discriminated unions is part of a broader modernization effort (E01-UnionMigration) to improve type safety and API consistency. This story provides a concrete application of the union pattern to the `CaSocialInsuranceNumber` class, completing the modernization of all core North American identifier types (US SSN, US NPI, US ITIN, and CA SIN).

The Canadian SIN validation is particularly complex due to:
- **Province Code System**: The first digit encodes the province/territory where the SIN was registered.
- **Luhn Algorithm with Mask Support**: The check digit validation supports both formatted and unformatted input via optional mask parameter.
- **Efficient Memory Handling**: The class uses array pooling to efficiently convert between formatted and unformatted SINs without unnecessary string allocations.

These characteristics require careful attention during implementation to ensure all existing functionality is preserved and performance characteristics are maintained.

See the related stories (S0019–S0025) for completed examples of this pattern in action.
