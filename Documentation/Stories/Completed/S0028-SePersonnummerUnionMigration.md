# S0028: Migrate SePersonnummer to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 5 points  
**Related:** S0019 (GbNhsNumber), S0022 (GbPatientNumber), S0023-S0027 (Union migrations), E01-UnionMigration

---

## Overview

Modernize the `SePersonnummer` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `SePersonnummer` with the new validation architecture introduced in the union-pattern migration initiative and improves type safety, error context, and API consistency across the codebase.

---

## Business Value

* **Type Safety**: Replace error enum values with discriminated union cases that can carry contextual error information (e.g., which character position, what separator was found, validation context).
* **Consistency**: Standardize the validation API across all identifier types (GB patient numbers, US identifiers, and European identifiers).
* **Developer Experience**: Enable compiler-checked exhaustiveness on validation results and eliminate string-based error lookups.
* **Maintainability**: Reduce reliance on extension methods and message lookups; move validation context closer to the error definition.
* **Compatibility**: Align with the modern union-pattern approach used by `GbNhsNumber`, `UsSocialSecurityNumber`, `CaSocialInsuranceNumber`, `MxCurp`, and other recent implementations.

---

## Requirements

### Functional Requirements

#### 1. Replace Enum with Union Pattern
Replace the current `SePersonnummerValidationResult` enum with discriminated unions mirroring the pattern established by GB patient number validation unions:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidSeparator,
   InvalidDateOfBirth,
   InvalidChecksum,
   InvalidBirthSerialNumber,
   SePersonnummerInvalidIdentifierType)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidSeparator,
   InvalidDateOfBirth,
   InvalidChecksum,
   InvalidBirthSerialNumber,
   SePersonnummerInvalidIdentifierType)
{
}
```

Each union case must be a record type (e.g., `record EmptyValue(String Message)`) with a `Message` property to carry localized error descriptions and optional context (position, character, expected value, etc.).

#### 2. Update Constructor Exception Documentation
Modify the constructor exception documentation to reference the new union types:
* Change `KfValidationException<SePersonnummerValidationResult>` to `UKfValidationException<ValidationError>`.
* Ensure all exception cases map to corresponding union cases.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types instead of enum values.
- Each validation failure should instantiate the appropriate union case with a localized message via `Messages` class.
- The success case should return `default(ValidValue)`.

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<SePersonnummer, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing `UKfValidationException<ValidationError>`.
- Ensure private validation-bypass constructor remains functional for post-validation object creation.
- Ensure backward compatibility (same behavioral contract; only the types change).

#### 5. Handle IdentifierType Property
- The `IdentifierType` property (which returns `SeIdentifierType` enum: `Personnummer` or `Samordningsnummer`) should remain unchanged.
- Validation logic must correctly distinguish between personnummer and samordningsnummer during all validation checks.

#### 6. Remove Old Extension Methods
- Delete or deprecate `SePersonnummerValidationResultExtensions.ToErrorDescription()` method (if exists).
- Delete the `SePersonnummerValidationResult` enum (after migration is complete).
- Optionally create a new file to hold the new union types and supporting methods if needed for organization.

#### 7. JSON Serialization
- Verify that `SePersonnummerJsonConverter` continues to work with the updated constructor.
- If needed, update the converter to handle internal validation-bypass logic correctly.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- The public API surface must remain functionally equivalent.
- Exception types and messages should be consistent with current behavior (though exception structure improves).
- Deprecated types (if any) should be marked with `[Obsolete]` attributes.
- The `IdentifierType` property and its return type (`SeIdentifierType` enum) remain unchanged.

#### 2. Code Organization
- Integrate union types directly into `SePersonnummer.cs` (similar to GB patient number implementations) or create a supporting file if the class becomes too large.
- Keep the main class file focused on business logic.
- Maintain consistent naming with other union-migrated types (e.g., `ValidationError`, `ValidationResult`).

#### 3. Error Messaging
- All error messages must be stored in the `Messages` class and referenceable via `Messages.SePersonnummer*` keys.
- Ensure parity between current error descriptions and new union case messages.

#### 4. Test Coverage
- All existing unit tests in `SePersonnummerTests.cs` must continue to pass.
- Test method signatures may be updated to work with union return types, but test scenarios must remain equivalent.
- Ensure all validation paths are still covered post-migration.

---

## Acceptance Criteria

### Definition of Done

- [ ] **Union types created**: `ValidationError` and `ValidationResult` discriminated unions defined as nested types within `SePersonnummer.cs`.
- [ ] **Constructor updated**: Exception documentation and validation logic updated to use `UKfValidationException<ValidationError>`.
- [ ] **Validate method refactored**: Returns `ValidationResult` union; all validation failures instantiate appropriate union cases.
- [ ] **Create method updated**: Signature returns `CreateResult<SePersonnummer, ValidationError>`.
- [ ] **IdentifierType property preserved**: `IdentifierType` property remains unchanged, returning `SeIdentifierType` enum correctly for both personnummer and samordningsnummer.
- [ ] **Private validation-bypass constructor updated**: Constructor with `ValidationMode` parameter continues to support validation bypass for post-validated instance creation.
- [ ] **All tests pass**: Existing unit tests in `SePersonnummerTests.cs` pass without modification (or with minimal adjustments to use new union API).
- [ ] **JSON converter verified**: `SePersonnummerJsonConverter` works correctly with updated constructor.
- [ ] **Old enum and extensions removed**: `SePersonnummerValidationResult` enum and related extensions deleted or deprecated.
- [ ] **Documentation updated**: XML comments in `SePersonnummer.cs` reflect new exception types and union patterns.
- [ ] **Error messages verified**: All required error messages present and correct in `Messages` class (see Validation Rules section).
- [ ] **Samordningsnummer validation verified**: Validation logic correctly handles both personnummer and samordningsnummer (includes 60-day offset validation).
- [ ] **Code review approved**: Migration code reviewed for consistency with other union-pattern implementations (GB patient numbers, US identifiers, CA SIN, MX CURP).

---

## Validation Rules

The following validation rules from the current implementation must be preserved and mapped to appropriate union cases:

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace → `EmptyValue`.
2. **Length Validation**: Value must be 10 or 12 characters in length (format: YYMMDD-XXXX or YYYYMMDD-XXXX) → `InvalidLength`.
3. **Separator Validation**: Separator must be either dash (`-`) or plus (`+`) at expected position, and different separators are not allowed → `InvalidSeparator`.
4. **Character Validation**: All non-separator characters must be ASCII digits (0-9) → `InvalidCharacter`.
5. **Date of Birth Validation**: First 6 or 8 digits must represent a valid calendar date in YYMMDD or YYYYMMDD format → `InvalidDateOfBirth`.
6. **Birth Serial Number Validation**: Middle 3 digits must be in valid range (001-999, not 000) → `InvalidBirthSerialNumber`.
7. **Samordningsnummer Offset**: For samordningsnummer, day offset (60 added to day field) must be valid after offset removal → `InvalidDateOfBirth`.
8. **Check Digit Validation**: Last digit must be a valid Luhn check digit calculated from the preceding characters, skipping the separator → `InvalidChecksum`.
9. **Identifier Type Validation**: Must be valid as either personnummer or samordningsnummer based on separator and date rules → `SePersonnummerInvalidIdentifierType` (if neither type can be determined).

---

## Implementation Notes

### Key Differences from Current Pattern

| Aspect | Current (Enum) | New (Union) |
|--------|----------------|------------|
| **Validation Return Type** | `SePersonnummerValidationResult` enum | `ValidationResult` discriminated union |
| **Error Context** | Implicit (via extension method lookup) | Explicit: each union case can carry `Message` property and context |
| **Exception Type** | `KfValidationException<SePersonnummerValidationResult>` | `UKfValidationException<ValidationError>` |
| **Message Resolution** | Runtime: `validationResult.ToErrorDescription()` | Build-time: included in union case definition |
| **Exhaustiveness** | Compiler warnings if switch incomplete | Compiler errors if union cases unhandled |
| **IdentifierType Property** | Returns `SeIdentifierType` enum (unchanged) | Returns `SeIdentifierType` enum (unchanged) |

### Reference Implementations

The following implementations serve as templates for this migration:

- **`GbNhsNumber.cs`**: GB patient number union pattern; establishes conventions for discriminated union structure.
- **`UsSocialSecurityNumber.cs`**: US identifier with similar validation complexity (formatter-specific ranges, multiple validation rules).
- **`CaSocialInsuranceNumber.cs`**: Canadian identifier with Luhn check digit handling and parsing logic.
- **`MxCurp.cs`**: Mexican identifier with century inference and complex date logic.

---

## Testing Strategy

- **Existing Test Suite**: All tests in `SePersonnummerTests.cs` should continue to exercise the same validation scenarios and test data.
- **Union Pattern Tests**: Add specific tests for union case exhaustiveness if not already covered by parametrized test data.
- **Samordningsnummer Coverage**: Ensure tests for both identifier types (personnummer and samordningsnummer) remain comprehensive, including the 60-day offset validation.
- **Backward Compatibility**: Verify that all public methods maintain their behavioral contracts.

---

## Definition of Ready

Before implementation:

- [ ] Current `SePersonnummer` implementation reviewed and validation rules documented.
- [ ] `SePersonnummerTests.cs` test coverage analyzed to ensure migration test scope.
- [ ] Union pattern from GB patient number implementations understood and agreed upon.
- [ ] Messages class updated with required keys (if not already present).
- [ ] Team consensus on `ValidationError` and `ValidationResult` union case definitions.

---

## Files to Modify/Create

### Modify
- `src/KfAccountNumbers/Governmental/Europe/SePersonnummer.cs` - Update to use union pattern, refactor validation logic, update constructor/Create method.
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/SePersonnummerTests.cs` - Update test helpers and assertions to work with union types (tests themselves remain largely unchanged).

### Delete (or Deprecate)
- `SePersonnummerValidationResult` enum definition (if in separate file) or inline enum (if in `SePersonnummer.cs`).
- Related extension methods (`SePersonnummerValidationResultExtensions`, etc.).

### Verify
- `src/KfAccountNumbers/Governmental/Europe/SePersonnummerJsonConverter.cs` - Ensure compatibility with updated constructor.
- `Messages` class - Verify all required error message keys exist.

---

## Success Metrics

- All existing unit tests pass post-migration without scenario changes.
- Build succeeds with no compilation errors or warnings related to this change.
- Code review approves migration for consistency with union-pattern implementations.
- No performance regression detected (Luhn check, date parsing, and other algorithms unchanged).
