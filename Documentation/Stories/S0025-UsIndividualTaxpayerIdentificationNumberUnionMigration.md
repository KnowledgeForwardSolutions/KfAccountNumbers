# S0025: Migrate UsIndividualTaxpayerIdentificationNumber to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 5 points  
**Related:** S0019 (GbNhsNumber), S0020 (GbHcNumber), S0021 (GbChiNumber), S0022 (GbPatientNumber), S0023 (UsSocialSecurityNumber), S0024 (UsNationalProviderIdentifier), E01-UnionMigration

---

## Overview

Modernize the `UsIndividualTaxpayerIdentificationNumber` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `UsIndividualTaxpayerIdentificationNumber` with the new validation architecture introduced in all other identifier types (GB patient numbers, US SSN, and US NPI) and improves type safety, error context, and API consistency across the codebase.

---

## Business Value

* **Type Safety**: Replace error enum values with discriminated union cases that can carry contextual error information (e.g., which validation rule failed, detailed error messages).
* **Consistency**: Standardize the validation API across all identifier types in the codebase (GB patient numbers, US SSN, US NPI, and US ITIN).
* **Developer Experience**: Enable compiler-checked exhaustiveness on validation results and eliminate string-based error lookups.
* **Maintainability**: Reduce reliance on extension methods and message lookups; move validation context closer to the error definition.

---

## Requirements

### Functional Requirements

#### 1. Replace Enum with Union Pattern
Replace the current `UsIndividualTaxpayerIdentificationNumberValidationResult` enum with a discriminated union mirroring the GB patient number and other identifier validation unions:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidSeparator,
   InvalidAreaNumber,
   InvalidGroupNumber)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidSeparator,
   InvalidAreaNumber,
   InvalidGroupNumber)
{
}
```

Each union case must be a record type (e.g., `record EmptyValue(String Message)`) with a `Message` property to carry localized error descriptions.

#### 2. Update Constructor Exception Documentation
Modify the constructor exception documentation to reference the new union types:
* Change `KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>` to `UKfValidationException<ValidationError>`.
* Ensure all exception cases map to corresponding union cases.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types instead of enum values.
- Each validation failure should instantiate the appropriate union case with a localized message via `Messages` class.
- The success case should return `default(ValidValue)`.

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<UsIndividualTaxpayerIdentificationNumber, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing `UKfValidationException<ValidationError>`.
- Maintain support for validation bypass via `ValidationMode.BypassValidation` for internal use (already in place).
- Ensure backward compatibility (same behavioral contract; only the types change).

#### 5. Remove Old Extension Methods
- Delete or deprecate `UsIndividualTaxpayerIdentificationNumberValidationResultExtensions` methods (`ToErrorDescription`, `ToValidationException`).
- Delete the `UsIndividualTaxpayerIdentificationNumberValidationResult` enum (after migration is complete).
- Create a new file `UsIndividualTaxpayerIdentificationNumberValidationError.cs` to hold the new union types and any supporting methods.

#### 6. JSON Serialization
- Verify that `UsIndividualTaxpayerIdentificationNumberJsonConverter` continues to work with the updated constructor.
- If needed, update the converter to handle internal validation-bypass logic correctly.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- The public API surface must remain functionally equivalent.
- Exception types and messages should be consistent with current behavior (though exception structure improves).
- Deprecated types (if any) should be marked with `[Obsolete]` attributes.

#### 2. Code Organization
- Move union types to a new file: `src/KfAccountNumbers/Governmental/NorthAmerica/UsIndividualTaxpayerIdentificationNumberValidationError.cs`.
- Keep the main class file `UsIndividualTaxpayerIdentificationNumber.cs` focused on business logic.
- Maintain consistent naming with other identifier types (e.g., `ValidationError`, `ValidationResult`).

#### 3. Error Messaging
- All error messages must be stored in the `Messages` class and reference-able via `Messages.UsItin*` keys.
- Ensure parity between current error descriptions and new union case messages.

---

## Acceptance Criteria

### Definition of Done

- [ ] **Union types created**: `ValidationError` and `ValidationResult` discriminated unions defined in `UsIndividualTaxpayerIdentificationNumberValidationError.cs`.
- [ ] **Constructor updated**: Exception documentation and validation logic updated to use `UKfValidationException<ValidationError>`.
- [ ] **Validate method refactored**: Returns `ValidationResult` union; all validation failures instantiate appropriate union cases.
- [ ] **Create method updated**: Signature returns `CreateResult<UsIndividualTaxpayerIdentificationNumber, ValidationError>`.
- [ ] **All tests pass**: Existing unit tests pass without modification (or with minimal adjustments to use new union API).
- [ ] **JSON converter verified**: `UsIndividualTaxpayerIdentificationNumberJsonConverter` works correctly with updated constructor.
- [ ] **Format method verified**: The `Format(String mask)` method continues to work correctly.
- [ ] **Old enum and extensions removed**: `UsIndividualTaxpayerIdentificationNumberValidationResult` enum and extensions deleted.
- [ ] **Documentation updated**: XML comments in `UsIndividualTaxpayerIdentificationNumber.cs` reflect new exception types.
- [ ] **Error messages verified**: All error messages present and correct in `Messages` class.
- [ ] **Code review approved**: Migration code reviewed for consistency with other identifier implementations.

---

## Validation Rules

The following validation rules from the current implementation must be preserved:

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace characters → `EmptyValue`.
2. **Length Validation**: Value must be 9 characters or 11 characters (with separators) → `InvalidLength`.
3. **Separator Validation**: If 11 characters, positions 3 and 6 must have identical non-digit separators → `InvalidSeparator`.
4. **Character Validation**: All non-separator characters must be ASCII digits ('0'-'9') → `InvalidCharacter`.
5. **Area Number Validation**: Area (first 3 digits) must start with digit 9 → `InvalidAreaNumber`.
6. **Group Number Validation**: Group (middle 2 digits) must be in ranges 50-65, 70-88, 90-92, or 94-99 → `InvalidGroupNumber`.

---

## Implementation Notes

### Key Differences from Current Pattern

| Aspect | Current (Enum) | New (Union) |
|--------|----------------|------------|
| **Validation Return Type** | `UsIndividualTaxpayerIdentificationNumberValidationResult` enum | `ValidationResult` discriminated union |
| **Error Context** | Implicit (via extension method lookup) | Explicit: each union case can carry `Message` property |
| **Exception Type** | `KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>` | `UKfValidationException<ValidationError>` |
| **Message Resolution** | Runtime: `validationResult.ToErrorDescription()` | Build-time: included in union case definition |
| **Exhaustiveness** | Compiler warnings if switch incomplete | Compiler errors if union cases unhandled |

### ITIN-Specific Considerations

The `UsIndividualTaxpayerIdentificationNumber` class includes several ITIN-specific features that must remain intact:

- **Formatting**: The `Format(String mask)` method uses the stored raw ITIN value to apply custom masks (e.g., `"___-__-____"`). This functionality is independent of validation and requires no changes.
- **Internal Helper Methods**: Helper methods like `GetAreaNumber`, `GetGroupNumber`, `GetSerialNumber`, `IsFormattedItin`, and `ValidateGroupNumber` must remain unchanged.
- **Range Validation**: The group number uses four separate valid ranges (50-65, 70-88, 90-92, 94-99); ensure this complex validation rule is preserved exactly during the migration.

### Inspiration / Reference Implementation

Refer to the following completed migrations for patterns and best practices:
- `src/KfAccountNumbers/Governmental/Europe/GbNhsNumber.cs` — constructor exception handling and `ValidationResult` union.
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsSocialSecurityNumber.cs` — parallel US identifier migration with separator validation (completed in S0023).
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsNationalProviderIdentifier.cs` — simpler US identifier migration (completed in S0024).

---

## Testing Strategy

1. **Unit Test Updates**: Existing unit tests for `UsIndividualTaxpayerIdentificationNumber` should pass without modification because the public behavior is identical.
2. **Validation Result Tests**: Add targeted tests to verify each union case is instantiated correctly with the appropriate message.
3. **Exception Mapping**: Verify that each `ValidationError` case correctly maps to an `UKfValidationException` with the right message and context.
4. **JSON Serialization Tests**: Ensure `UsIndividualTaxpayerIdentificationNumberJsonConverter` continues to work (constructor validation is internal).
5. **Format Method Tests**: Verify that the `Format(String mask)` method continues to work correctly with various masks.
6. **Group Number Range Tests**: Ensure all four valid group number ranges (50-65, 70-88, 90-92, 94-99) are correctly validated.

---

## Files to Create / Modify

### Files to Create
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsIndividualTaxpayerIdentificationNumberValidationError.cs` — New union type definitions.

### Files to Modify
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsIndividualTaxpayerIdentificationNumber.cs` — Refactor constructor, `Validate`, and `Create` methods.
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsIndividualTaxpayerIdentificationNumberJsonConverter.cs` — Verify compatibility (may not need changes).
- `tests/KfAccountNumbers.Tests.Unit/Governmental/NorthAmerica/UsIndividualTaxpayerIdentificationNumberTests.cs` — Update assertions if needed (optional if behavior is identical).

### Files to Delete
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsIndividualTaxpayerIdentificationNumberValidationResult.cs` — Old enum and extensions.

### Files to Review (No Changes Expected)
- `src/KfAccountNumbers/Messages.cs` — Verify all `UsItin*` message keys exist; add any missing keys.
- `src/KfAccountNumbers/Governmental/NorthAmerica/UsIndividualTaxpayerIdentificationNumberJsonConverter.cs` — Ensure it works with updated constructor.

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|-----------|
| **Breaking Change in Public API** | Low | High | Maintain same exception types; only internal representation changes. Use `CreateResult` pattern for non-throwing API. |
| **Incomplete Union Case Handling** | Medium | Medium | Thorough code review against other identifier implementations; compiler warnings catch most cases. |
| **Missing Error Messages** | Low | Medium | Audit `Messages` class for all `UsItin*` keys before merge; run with culture-specific tests. |
| **JSON Serialization Regression** | Low | High | Run existing JSON serialization tests; verify converter works with new exception types. |
| **Format Method Regression** | Low | Medium | Ensure `Format` method continues to work; run all mask-formatting tests. |
| **Group Number Range Validation Error** | Low | High | Double-check that all four valid ranges (50-65, 70-88, 90-92, 94-99) are preserved; add specific unit tests for boundary cases. |
| **Test Coverage Gaps** | Medium | Medium | Add new tests for each union case; verify exception mapping; use code coverage tools. |

---

## Dependencies

- **Completed**: C# 12 discriminated unions available (language feature).
- **Prerequisite**: E01-UnionMigration backlog item or equivalent environment setup.
- **Related Stories**: S0019–S0024 (GB patient numbers, US SSN, and US NPI using union pattern).

---

## Definition of Done (Story Level)

1. **Implementation Complete**: All code changes applied and passing local build.
2. **Tests Green**: All existing and new unit tests passing, including group number range tests.
3. **Code Review**: Peer review completed; linting and static analysis clean.
4. **Documentation**: XML comments updated; README or release notes mention migration if needed.
5. **Backwards Compatibility Verified**: Functional behavior unchanged from consumer perspective, including formatting.
6. **Ready for Merge**: Approved for merge to main branch.

---

## Estimated Effort

**5 story points** (Medium complexity):
- ~1 point: Create new union types and validation error file.
- ~2 points: Refactor `Validate`, `Create`, and constructor; update exception handling; preserve critical validation logic (especially group number ranges).
- ~1 point: Verify JSON serialization and formatting; update tests.
- ~0.5 point: Clean up old enum; review documentation.
- ~0.5 point: Code review and final validation.

**Actual effort may vary** based on test complexity, group number range validation specificity, and formatting method integration.

---

## Additional Context

The migration of identifier validation to discriminated unions is part of a broader modernization effort (E01-UnionMigration) to improve type safety and API consistency. This story provides a concrete application of the union pattern to the `UsIndividualTaxpayerIdentificationNumber` class, completing the modernization of all core US identifier types (SSN, NPI, and ITIN).

The ITIN-specific characteristics—particularly the complex group number range validation (four separate ranges) and the flexible formatting support—require careful attention during implementation to ensure all existing functionality is preserved.

See the related stories (S0019–S0024) for completed examples of this pattern in action.
