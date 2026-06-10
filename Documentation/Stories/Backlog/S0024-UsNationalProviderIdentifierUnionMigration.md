# S0024: Migrate UsNationalProviderIdentifier to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 5 points  
**Related:** S0019 (GbNhsNumber), S0020 (GbHcNumber), S0021 (GbChiNumber), S0022 (GbPatientNumber), S0023 (UsSocialSecurityNumber), E01-UnionMigration

---

## Overview

Modernize the `UsNationalProviderIdentifier` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `UsNationalProviderIdentifier` with the new validation architecture introduced in the GB patient number types and improves type safety, error context, and API consistency across the codebase.

---

## Business Value

* **Type Safety**: Replace error enum values with discriminated union cases that can carry contextual error information (e.g., which validation rule failed, detailed error messages).
* **Consistency**: Standardize the validation API across all identifier types (US NPI, US SSN, and GB patient numbers).
* **Developer Experience**: Enable compiler-checked exhaustiveness on validation results and eliminate string-based error lookups.
* **Maintainability**: Reduce reliance on extension methods and message lookups; move validation context closer to the error definition.

---

## Requirements

### Functional Requirements

#### 1. Replace Enum with Union Pattern
Replace the current `UsNationalProviderIdentifierValidationResult` enum with a discriminated union mirroring the GB patient number validation unions:

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

Each union case must be a record type (e.g., `record EmptyValue(String Message)`) with a `Message` property to carry localized error descriptions.

#### 2. Update Constructor Exception Documentation
Modify the constructor exception documentation to reference the new union types:
* Change `KfValidationException<UsNationalProviderIdentifierValidationResult>` to `UKfValidationException<ValidationError>`.
* Ensure all exception cases map to corresponding union cases.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types instead of enum values.
- Each validation failure should instantiate the appropriate union case with a localized message via `Messages` class.
- The success case should return `default(ValidValue)`.

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<UsNationalProviderIdentifier, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing `UKfValidationException<ValidationError>`.
- Maintain support for validation bypass via `ValidationMode.BypassValidation` for internal use.
- Ensure backward compatibility (same behavioral contract; only the types change).

#### 5. Remove Old Extension Methods
- Delete or deprecate `UsNationalProviderIdentifierValidationResultExtensions` methods (`ToErrorDescription`, `ToValidationException`).
- Delete the `UsNationalProviderIdentifierValidationResult` enum (after migration is complete).
- Create a new file `UsNationalProviderIdentifierValidationError.cs` to hold the new union types and any supporting methods.

#### 6. JSON Serialization
- Verify that `UsNationalProviderIdentifierJsonConverter` continues to work with the updated constructor.
- If needed, update the converter to handle internal validation-bypass logic correctly.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- The public API surface must remain functionally equivalent.
- Exception types and messages should be consistent with current behavior (though exception structure improves).
- Deprecated types (if any) should be marked with `[Obsolete]` attributes.

#### 2. Code Organization
- Move union types to a new file: `src/KfAccountNumbers/Governmental/NorthAmerica/UsNationalProviderIdentifierValidationError.cs`.
- Keep the main class file `UsNationalProviderIdentifier.cs` focused on business logic.
- Maintain consistent naming with GB patient number and US SSN types (e.g., `ValidationError`, `ValidationResult`).

#### 3. Error Messaging
- All error messages must be stored in the `Messages` class and reference-able via `Messages.UsNpi*` keys.
- Ensure parity between current error descriptions and new union case messages.

---

## Acceptance Criteria

### Definition of Done

- [ ] **Union types created**: `ValidationError` and `ValidationResult` discriminated unions defined in `UsNationalProviderIdentifierValidationError.cs`.
- [ ] **Constructor updated**: Exception documentation and validation logic updated to use `UKfValidationException<ValidationError>`.
- [ ] **Validate method refactored**: Returns `ValidationResult` union; all validation failures instantiate appropriate union cases.
- [ ] **Create method updated**: Signature returns `CreateResult<UsNationalProviderIdentifier, ValidationError>`.
- [ ] **All tests pass**: Existing unit tests pass without modification (or with minimal adjustments to use new union API).
- [ ] **JSON converter verified**: `UsNationalProviderIdentifierJsonConverter` works correctly with updated constructor.
- [ ] **Old enum and extensions removed**: `UsNationalProviderIdentifierValidationResult` enum and extensions deleted.
- [ ] **Documentation updated**: XML comments in `UsNationalProviderIdentifier.cs` reflect new exception types.
- [ ] **Error messages verified**: All error messages present and correct in `Messages` class.
- [ ] **Code review approved**: Migration code reviewed for consistency with GB patient number and US SSN implementations.

---

## Validation Rules

The following validation rules from the current implementation must be preserved:

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace characters → `EmptyValue`.
2. **Length Validation**: Value must be exactly 10 characters in length → `InvalidLength`.
3. **Character Validation**: All characters must be ASCII digits ('0'-'9') → `InvalidCharacter`.
4. **Check Digit Validation**: The trailing (right-most) digit must be a valid check digit according to the Luhn algorithm prefixed with "80840" → `InvalidCheckDigit`.

---

## Implementation Notes

### Key Differences from Current Pattern

| Aspect | Current (Enum) | New (Union) |
|--------|----------------|------------|
| **Validation Return Type** | `UsNationalProviderIdentifierValidationResult` enum | `ValidationResult` discriminated union |
| **Error Context** | Implicit (via extension method lookup) | Explicit: each union case can carry `Message` property |
| **Exception Type** | `KfValidationException<UsNationalProviderIdentifierValidationResult>` | `UKfValidationException<ValidationError>` |
| **Message Resolution** | Runtime: `validationResult.ToErrorDescription()` | Build-time: included in union case definition |
| **Exhaustiveness** | Compiler warnings if switch incomplete | Compiler errors if union cases unhandled |

### Inspiration / Reference Implementation

Refer to the following completed migrations for patterns and best practices:
- `src/KfAccountNumbers/Governmental/Europe/GbNhsNumber.cs` — constructor exception handling and `ValidationResult` union.
- `src/KfAccountNumbers/Governmental/Europe/GbChiNumber.cs` — `ValidationError` union with multiple cases.
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsSocialSecurityNumber.cs` — parallel US identifier migration (completed in S0023).

### Algorithm Reference

The NPI validation uses a variant of the Luhn algorithm:
- Input: 10-digit NPI number
- Process: Prefix "80840" to the first 9 digits, then apply standard Luhn check digit algorithm
- The result should match the trailing (10th) digit
- Implemented in: `Algorithms.Npi.Validate(npi)`

---

## Testing Strategy

1. **Unit Test Updates**: Existing unit tests for `UsNationalProviderIdentifier` should pass without modification because the public behavior is identical.
2. **Validation Result Tests**: Add targeted tests to verify each union case is instantiated correctly with the appropriate message.
3. **Exception Mapping**: Verify that each `ValidationError` case correctly maps to an `UKfValidationException` with the right message and context.
4. **JSON Serialization Tests**: Ensure `UsNationalProviderIdentifierJsonConverter` continues to work (constructor validation is internal).
5. **Algorithm Tests**: Verify that the Luhn algorithm validation remains unchanged and produces correct results.

---

## Files to Create / Modify

### Files to Create
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsNationalProviderIdentifierValidationError.cs` — New union type definitions.

### Files to Modify
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsNationalProviderIdentifier.cs` — Refactor constructor, `Validate`, and `Create` methods.
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsNationalProviderIdentifierJsonConverter.cs` — Verify compatibility (may not need changes).
- `tests/KfAccountNumbers.Tests.Unit/Governmental/NorthAmerica/UsNationalProviderIdentifierTests.cs` — Update assertions if needed (optional if behavior is identical).

### Files to Delete
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsNationalProviderIdentifierValidationResult.cs` — Old enum and extensions.

### Files to Review (No Changes Expected)
- `src/KfAccountNumbers/Messages.cs` — Verify all `UsNpi*` message keys exist; add any missing keys.
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsNationalProviderIdentifierJsonConverter.cs` — Ensure it works with updated constructor.

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|-----------|
| **Breaking Change in Public API** | Low | High | Maintain same exception types; only internal representation changes. Use `CreateResult` pattern for non-throwing API. |
| **Incomplete Union Case Handling** | Medium | Medium | Thorough code review against GB patient number and US SSN implementations; compiler warnings catch most cases. |
| **Missing Error Messages** | Low | Medium | Audit `Messages` class for all `UsNpi*` keys before merge; run with culture-specific tests. |
| **JSON Serialization Regression** | Low | High | Run existing JSON serialization tests; verify converter works with new exception types. |
| **Algorithm Validation Regression** | Low | High | Ensure `Algorithms.Npi.Validate()` is called correctly; run all existing algorithm tests. |
| **Test Coverage Gaps** | Medium | Medium | Add new tests for each union case; verify exception mapping; use code coverage tools. |

---

## Dependencies

- **Completed**: C# 12 discriminated unions available (language feature).
- **Prerequisite**: E01-UnionMigration backlog item or equivalent environment setup.
- **Related Stories**: S0019–S0023 (GB patient numbers and US SSN using union pattern).
- **Utility**: `Algorithms.Npi` — NPI validation algorithm (must remain unchanged).

---

## Definition of Done (Story Level)

1. **Implementation Complete**: All code changes applied and passing local build.
2. **Tests Green**: All existing and new unit tests passing.
3. **Code Review**: Peer review completed; linting and static analysis clean.
4. **Documentation**: XML comments updated; README or release notes mention migration if needed.
5. **Backwards Compatibility Verified**: Functional behavior unchanged from consumer perspective.
6. **Ready for Merge**: Approved for merge to main branch.

---

## Estimated Effort

**5 story points** (Medium complexity):
- ~1 point: Create new union types and validation error file.
- ~2 points: Refactor `Validate`, `Create`, and constructor; update exception handling; verify algorithm integration.
- ~1 point: Verify JSON serialization; update tests.
- ~0.5 point: Clean up old enum; review documentation.
- ~0.5 point: Code review and final validation.

**Actual effort may vary** based on test complexity, JSON converter integration, and algorithm validation requirements.

---

## Additional Context

The migration of identifier validation to discriminated unions is part of a broader modernization effort (E01-UnionMigration) to improve type safety and API consistency. This story provides a concrete application of the union pattern to the `UsNationalProviderIdentifier` class, complementing the parallel migration of `UsSocialSecurityNumber` (S0023) and building on the established patterns from GB patient numbers (S0019–S0022).

The NPI validation algorithm remains unchanged; only the validation result representation changes from enum to union, enabling richer error context and better compile-time type checking.

See the related stories (S0019–S0023) for completed examples of this pattern in action.
