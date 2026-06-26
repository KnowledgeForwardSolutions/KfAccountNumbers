# S0042: Migrate GbNationalInsuranceNumber to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 5 points  
**Related:** S0028 (SePersonnummer union migration), S0019 (GbNhsNumber), S0022 (GbPatientNumber), E01-UnionMigration

---

## Overview

Modernize the `GbNationalInsuranceNumber` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `GbNationalInsuranceNumber` with the new validation architecture introduced in the union-pattern migration initiative and improves type safety, error context, and API consistency across the codebase.

---

## Business Value

* **Type Safety**: Replace error enum values with discriminated union cases that can carry contextual error information (e.g., which character position, what invalid prefix or character was found, validation context).
* **Consistency**: Standardize the validation API across all identifier types (GB patient numbers, US identifiers, European identifiers, etc.).
* **Developer Experience**: Enable compiler-checked exhaustiveness on validation results and eliminate string-based error lookups.
* **Maintainability**: Reduce reliance on extension methods and message lookups; move validation context closer to the error definition.
* **Compatibility**: Align with the modern union-pattern approach used by `SePersonnummer`, `GbNhsNumber`, `UsSocialSecurityNumber`, `CaSocialInsuranceNumber`, `MxCurp`, and other recent implementations.

---

## Requirements

### Functional Requirements

#### 1. Replace Enum with Union Pattern
Replace the current `GbNationalInsuranceNumberValidationResult` enum with discriminated unions mirroring the pattern established by other validation union implementations:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidPrefix,
   InvalidCharacter,
   InvalidSeparator)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidPrefix,
   InvalidCharacter,
   InvalidSeparator)
{
}
```

Each union case must be a record type (e.g., `record EmptyValue(String Message)`) with a `Message` property to carry localized error descriptions and optional context (position, character, expected value, prefix, etc.).

#### 2. Update Constructor Exception Documentation
Modify the constructor exception documentation to reference the new union types:
* Change `KfValidationException<GbNationalInsuranceNumberValidationResult>` to `UKfValidationException<ValidationError>`.
* Ensure all exception cases map to corresponding union cases.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types instead of enum values.
- Each validation failure should instantiate the appropriate union case with a localized message via `Messages` class.
- The success case should return `default(ValidValue)`.
- Map current enum values to new union cases:
  * `ValidationPassed` → `ValidValue`
  * `Empty` → `EmptyValue`
  * `InvalidLength` → `InvalidLength` (with length information)
  * `InvalidPrefix` → `InvalidPrefix` (with prefix value)
  * `InvalidCharacter` → `InvalidCharacter` (with character position and character)
  * `InvalidSeparator` → `InvalidSeparator` (with separator position and separator character)

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<GbNationalInsuranceNumber, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing `UKfValidationException<ValidationError>`.
- Ensure private validation-bypass constructor remains functional for post-validation object creation.
- Ensure backward compatibility (same behavioral contract; only the types change).

#### 5. Remove Old Enum and Extensions
- Delete or deprecate `GbNationalInsuranceNumberValidationResult` enum (currently in separate file).
- Delete the `GbNationalInsuranceNumberValidationResult.cs` file.
- Delete or deprecate any extension methods like `GbNationalInsuranceNumberValidationResultExtensions.ToErrorDescription()` (if exists).
- Update any internal usages of the old enum throughout the codebase.

#### 6. JSON Serialization
- Verify that `GbNationalInsuranceNumberJsonConverter` continues to work with the updated constructor.
- If needed, update the converter to handle internal validation-bypass logic correctly.

#### 7. Update EqualsNonSuffix Method
- The `EqualsNonSuffix()` method (which performs equality check only on first 8 characters) should remain unchanged.
- Ensure the method continues to work correctly with the updated validation logic.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- The public API surface must remain functionally equivalent.
- Exception types and messages should be consistent with current behavior (though exception structure improves).
- Deprecated types (if any) should be marked with `[Obsolete]` attributes.
- The `EqualsNonSuffix()` method and its behavior remain unchanged.

#### 2. Code Organization
- Integrate union types directly into `GbNationalInsuranceNumber.cs` (similar to SePersonnummer implementations).
- Keep the main class file focused on business logic.
- Maintain consistent naming with other union-migrated types (e.g., `ValidationError`, `ValidationResult`).
- Delete the separate `GbNationalInsuranceNumberValidationResult.cs` file after migration.

#### 3. Error Messaging
- All error messages must be stored in the `Messages` class and referenceable via `Messages.GbNationalInsuranceNumber*` keys.
- Ensure parity between current error descriptions and new union case messages.
- Required message keys:
  * `GbNationalInsuranceNumberEmpty`
  * `GbNationalInsuranceNumberInvalidLength`
  * `GbNationalInsuranceNumberInvalidPrefix`
  * `GbNationalInsuranceNumberInvalidCharacter`
  * `GbNationalInsuranceNumberInvalidSeparator`

#### 4. Test Coverage
- All existing unit tests in `GbNationalInsuranceNumberTests.cs` must continue to pass.
- Test method signatures may be updated to work with union return types, but test scenarios must remain equivalent.
- Ensure all validation paths are still covered post-migration, including:
  * All invalid prefix combinations
  * Character validation for all positions (prefix characters, digits, suffix)
  * Separator validation for all positions (unformatted and formatted lengths)
  * Suffix presence/absence scenarios

---

## Acceptance Criteria

### Definition of Done

- [ ] **Union types created**: `ValidationError` and `ValidationResult` discriminated unions defined as nested types within `GbNationalInsuranceNumber.cs`.
- [ ] **Constructor updated**: Exception documentation and validation logic updated to use `UKfValidationException<ValidationError>`.
- [ ] **Validate method refactored**: Returns `ValidationResult` union; all validation failures instantiate appropriate union cases.
- [ ] **Create method updated**: Signature returns `CreateResult<GbNationalInsuranceNumber, ValidationError>`.
- [ ] **Private validation-bypass constructor updated**: Constructor with `ValidationMode` parameter continues to support validation bypass for post-validated instance creation.
- [ ] **All tests pass**: Existing unit tests in `GbNationalInsuranceNumberTests.cs` pass without modification (or with minimal adjustments to use new union API).
- [ ] **JSON converter verified**: `GbNationalInsuranceNumberJsonConverter` works correctly with updated constructor.
- [ ] **Old enum file deleted**: `GbNationalInsuranceNumberValidationResult.cs` deleted.
- [ ] **Old enum and extensions removed**: All internal usages of `GbNationalInsuranceNumberValidationResult` enum replaced with union types.
- [ ] **Documentation updated**: XML comments in `GbNationalInsuranceNumber.cs` reflect new exception types and union patterns.
- [ ] **Error messages verified**: All required error messages present and correct in `Messages` class (see Validation Rules section).
- [ ] **EqualsNonSuffix method verified**: Suffix-agnostic equality method continues to work correctly with updated implementation.
- [ ] **Code review approved**: Migration code reviewed for consistency with other union-pattern implementations (SePersonnummer, GB patient numbers, US identifiers, etc.).
- [ ] **Build succeeds**: No compilation errors or warnings related to this change.

---

## Validation Rules

The following validation rules from the current implementation must be preserved and mapped to appropriate union cases:

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace → `EmptyValue`.
2. **Length Validation**: Value must be 8, 9, 11, or 13 characters in length:
   * 8 = unformatted without suffix
   * 9 = unformatted with suffix
   * 11 = formatted without suffix
   * 13 = formatted with suffix
   → `InvalidLength` (with actual length).
3. **Prefix Validation**: First two characters must not be BG, GB, NK, KN, TN, NT, or ZZ → `InvalidPrefix` (with prefix value).
4. **First Character Validation**: Character position 0 must be A-C, E, G, H, J-P, R-T, W-Z (not D, F, I, Q, U, V) → `InvalidCharacter` (with position and character).
5. **Second Character Validation**: Character position 1 must be A-C, E, G, H, J-N, P, R-T, W-Z (not D, F, I, O, Q, U, V) → `InvalidCharacter` (with position and character).
6. **Digit Validation**: Character positions 2-7 must be ASCII digits (0-9) → `InvalidCharacter` (with position and character).
7. **Suffix Validation**: Character position 8 (if present) must be A-D → `InvalidCharacter` (with position and character).
8. **Separator Validation**: For formatted values (length 11 or 13), separator characters must appear at positions 2, 5, 8, and optionally 11 (zero-based); must not be ASCII digits or letters; all separators must be the same character → `InvalidSeparator` (with position and separator character).

---

## Implementation Notes

### Key Differences from Current Pattern

| Aspect | Current (Enum) | New (Union) |
|--------|----------------|------------|
| **Validation Return Type** | `GbNationalInsuranceNumberValidationResult` enum | `ValidationResult` discriminated union |
| **Error Context** | Implicit (via extension method lookup) | Explicit: each union case can carry `Message` property and context |
| **Exception Type** | `KfValidationException<GbNationalInsuranceNumberValidationResult>` | `UKfValidationException<ValidationError>` |
| **Message Resolution** | Runtime: `validationResult.ToErrorDescription()` | Build-time: included in union case definition |
| **Exhaustiveness** | Compiler warnings if switch incomplete | Compiler errors if union cases unhandled |
| **File Organization** | Separate `GbNationalInsuranceNumberValidationResult.cs` | Integrated into `GbNationalInsuranceNumber.cs` |

### Reference Implementations

The following implementations serve as templates for this migration:

- **`SePersonnummer.cs`** (S0028): Swedish identifier with similar prefix/character/separator validation plus date/checksum logic.
- **`GbNhsNumber.cs`**: GB patient number union pattern; establishes conventions for discriminated union structure.
- **`UsSocialSecurityNumber.cs`**: US identifier with multiple validation rules and context-carrying union cases.
- **`FrInseeNumber.cs`**: French identifier with case-insensitive handling and complex validation rules.

---

## Testing Strategy

- **Existing Test Suite**: All tests in `GbNationalInsuranceNumberTests.cs` should continue to exercise the same validation scenarios and test data.
- **Union Pattern Tests**: Add specific tests for union case exhaustiveness if not already covered by parametrized test data.
- **Suffix Handling Coverage**: Ensure tests for both suffix-present and suffix-absent scenarios remain comprehensive.
- **EqualsNonSuffix Tests**: Verify that suffix-agnostic equality tests continue to work correctly with updated implementation.
- **Backward Compatibility**: Verify that all public methods maintain their behavioral contracts.

---

## Definition of Ready

Before implementation:

- [ ] Current `GbNationalInsuranceNumber` implementation reviewed and validation rules documented.
- [ ] `GbNationalInsuranceNumberTests.cs` test coverage analyzed to ensure migration test scope.
- [ ] Union pattern from SePersonnummer and other implementations understood and agreed upon.
- [ ] Messages class updated with required keys (if not already present).
- [ ] Team consensus on `ValidationError` and `ValidationResult` union case definitions.
- [ ] Identified all internal usages of `GbNationalInsuranceNumberValidationResult` enum to be replaced.

---

## Files to Modify/Create

### Modify
- `src/KfAccountNumbers/Governmental/Europe/GbNationalInsuranceNumber.cs` - Update to use union pattern, refactor validation logic, update constructor/Create method.
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/GbNationalInsuranceNumberTests.cs` - Update test helpers and assertions to work with union types (tests themselves remain largely unchanged).

### Delete
- `src/KfAccountNumbers/Governmental/Europe/GbNationalInsuranceNumberValidationResult.cs` - Delete after migration complete.

### Verify
- `src/KfAccountNumbers/Governmental/Europe/GbNationalInsuranceNumberJsonConverter.cs` - Ensure compatibility with updated constructor.
- `src/KfAccountNumbers/Messages.cs` (or equivalent) - Verify all required error message keys exist.
- Any other files that import or reference `GbNationalInsuranceNumberValidationResult`.

---

## Success Metrics

- All existing unit tests pass post-migration without scenario changes.
- Build succeeds with no compilation errors or warnings related to this change.
- Code review approves migration for consistency with union-pattern implementations.
- No performance regression detected (validation logic and algorithms unchanged).
- All internal usages of old enum successfully replaced with union types.
- Suffix-agnostic equality (`EqualsNonSuffix`) continues to work correctly.

---

## Effort Breakdown (5 points)

- **Analysis & Planning** (0.5 points): Review current implementation, identify all usages, plan migration approach.
- **Union Types Definition** (0.5 points): Define validation union types with appropriate record cases and properties.
- **Validation Logic Refactoring** (1.5 points): Update `Validate` method to return union types; update constructor and `Create` method.
- **Test Updates & Verification** (1.5 points): Update test helpers; verify all tests pass; test union pattern exhaustiveness.
- **Cleanup & Documentation** (1 point): Delete old enum file; update XML comments; remove old extensions; final review.

---

## Risks & Mitigations

| Risk | Mitigation |
|------|-----------|
| Missing internal usages of old enum | Perform comprehensive codebase search for all `GbNationalInsuranceNumberValidationResult` references before deletion |
| Test failures due to union pattern differences | Run full test suite during migration; update test helpers incrementally |
| JSON converter compatibility issues | Test JSON serialization/deserialization thoroughly before marking complete |
| Message key mismatches | Verify all required message keys exist in `Messages` class before implementation |
| Performance regression | Profile validation logic before/after migration to ensure no performance impact |

