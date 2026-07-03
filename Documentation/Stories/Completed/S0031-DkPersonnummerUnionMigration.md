# S0031: Migrate DkPersonummer to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 5 points  
**Related:** S0019 (GbNhsNumber), S0022 (GbPatientNumber), S0023-S0030 (Union migrations), E01-UnionMigration

---

## Overview

Modernize the `DkPersonummer` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `DkPersonummer` with the new validation architecture introduced in the union-pattern migration initiative and improves type safety, error context, and API consistency across the codebase.

---

## Business Value

* **Type Safety**: Replace error enum values with discriminated union cases that can carry contextual error information (e.g., which character position, what separator was found, validation context).
* **Consistency**: Standardize the validation API across all identifier types (GB patient numbers, US identifiers, European identifiers).
* **Developer Experience**: Enable compiler-checked exhaustiveness on validation results and eliminate string-based error lookups.
* **Maintainability**: Reduce reliance on extension methods and message lookups; move validation context closer to the error definition.
* **Compatibility**: Align with the modern union-pattern approach used by `GbNhsNumber`, `UsSocialSecurityNumber`, `CaSocialInsuranceNumber`, `MxCurp`, `SePersonnummer`, `NoFoedselsnummer`, `IsKennitala`, and other recent implementations.

---

## Requirements

### Functional Requirements

#### 1. Replace Enum with Union Pattern
Replace the current `DkPersonnummerValidationResult` enum with discriminated unions mirroring the pattern established by GB patient number validation unions:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidSeparator,
   InvalidDateOfBirth)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidSeparator,
   InvalidDateOfBirth)
{
}
```

Each union case must be a record type (e.g., `record EmptyValue(String Message)`) with a `Message` property to carry localized error descriptions. Some union cases may carry additional context such as:
- `InvalidCharacter`: position and character encountered
- `InvalidSeparator`: position and character encountered (note: Danish uses dash '-' as separator)

#### 2. Update Constructor Exception Documentation
Modify the constructor exception documentation to reference the new union types:
* Change `KfValidationException<DkPersonnummerValidationResult>` to `UKfValidationException<ValidationError>`.
* Ensure all exception cases map to corresponding union cases.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types instead of enum values.
- Each validation failure should instantiate the appropriate union case with a localized message via `Messages` class.
- The success case should return `default(ValidValue)`.

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<DkPersonummer, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing `UKfValidationException<ValidationError>`.
- Ensure private validation-bypass constructor remains functional for post-validation object creation.
- Ensure backward compatibility (same behavioral contract; only the types change).

#### 5. Preserve Century Derivation Logic
- The century derivation rules (based on century indicator digit and two-digit year) must be preserved exactly as-is.
- Valid date range (January 1, 1858 to December 31, 2057) must be maintained.
- The seven century derivation rules must be evaluated in their specified order.

#### 6. Handle Check Digit Discontinuation
- The trailing digit was originally a modulus 11 check digit, but its use was discontinued in 2007.
- The current implementation does **NOT** validate a check digit since it cannot determine pre/post-2007 status.
- This behavior must remain unchanged post-migration.

#### 7. Preserve Separator Validation
- Only a dash ('-') character is allowed as separator (position 6 in 11-character format).
- Unlike some other identifiers, any non-digit separator is **NOT** allowed; specifically, it must be a dash.

#### 8. Remove Old Extension Methods
- Delete the `DkPersonnummerValidationResult` enum (after migration is complete).
- Delete the `DkPersonnummerValidationResultExtensions` class (after migration is complete).
- Optionally create a new supporting file if needed for organization.

#### 9. JSON Serialization
- Verify that `DkPersonnummerJsonConverter` continues to work with the updated constructor.
- If needed, update the converter to handle internal validation-bypass logic correctly.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- The public API surface must remain functionally equivalent.
- Exception types and messages should be consistent with current behavior (though exception structure improves).
- Deprecated types (if any) should be marked with `[Obsolete]` attributes.
- No IdentifierType property exists for Danish personnummer (simpler than Icelandic or Norwegian types).

#### 2. Code Organization
- Integrate union types directly into `DkPersonummer.cs` (similar to GB patient number implementations).
- Keep the main class file focused on business logic.
- Maintain consistent naming with other union-migrated types (e.g., `ValidationError`, `ValidationResult`).

#### 3. Error Messaging
- All error messages must be stored in the `Messages` class and referenceable via `Messages.DkPersonummer*` keys.
- Ensure parity between current error descriptions and new union case messages:
  - `Empty` → `Messages.DkPersonnummerEmpty`
  - `InvalidLength` → `Messages.DkPersonnummerInvalidLength`
  - `InvalidCharacter` → `Messages.DkPersonnummerInvalidCharacter`
  - `InvalidSeparator` → `Messages.DkPersonnummerInvalidSeparator`
  - `InvalidDateOfBirth` → `Messages.DkPersonnummerInvalidDateOfBirth`

#### 4. Test Coverage
- All existing unit tests in `DkPersonnummerTests.cs` must continue to pass.
- Test method signatures may be updated to work with union return types, but test scenarios must remain equivalent.
- Ensure all validation paths are still covered post-migration, including:
  - All seven century derivation rules
  - Century derivation edge cases (e.g., year boundaries at condition thresholds)
  - Separator validation (dash only, not any non-digit character)
  - No check digit validation (intentional discontinuation)

---

## Acceptance Criteria

### Definition of Done

- [ ] **Union types created**: `ValidationError` and `ValidationResult` discriminated unions defined as nested types within `DkPersonummer.cs`.
- [ ] **Constructor updated**: Exception documentation and validation logic updated to use `UKfValidationException<ValidationError>`.
- [ ] **Validate method refactored**: Returns `ValidationResult` union; all validation failures instantiate appropriate union cases.
- [ ] **Create method updated**: Signature returns `CreateResult<DkPersonummer, ValidationError>`.
- [ ] **Century derivation rules preserved**: All seven century derivation rules work correctly with new union pattern.
- [ ] **No check digit validation**: The trailing digit is NOT validated (intentional, post-2007 policy).
- [ ] **Separator validation strict**: Only dash ('-') is allowed as separator; other non-digit characters are rejected.
- [ ] **Private validation-bypass constructor updated**: Constructor with `ValidationMode` parameter continues to support validation bypass for post-validated instance creation.
- [ ] **All tests pass**: Existing unit tests in `DkPersonnummerTests.cs` pass without modification (or with minimal adjustments to use new union API).
- [ ] **JSON converter verified**: `DkPersonnummerJsonConverter` works correctly with updated constructor.
- [ ] **Old enum and extensions removed**: `DkPersonnummerValidationResult` enum and `DkPersonnummerValidationResultExtensions` class deleted.
- [ ] **Documentation updated**: XML comments in `DkPersonummer.cs` reflect new exception types and union patterns.
- [ ] **Error messages verified**: All required error messages present and correct in `Messages` class.
- [ ] **Code review approved**: Migration code reviewed for consistency with other union-pattern implementations (GB patient numbers, US identifiers, European identifiers).

---

## Validation Rules

The following validation rules from the current implementation must be preserved and mapped to appropriate union cases:

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace → `EmptyValue`.
2. **Length Validation**: Value must be 10 or 11 characters in length → `InvalidLength`.
3. **Separator Validation**: If 11 characters, position 6 must be a dash ('-') character ONLY → `InvalidSeparator`.
4. **Character Validation**: All non-separator characters must be ASCII digits (0-9) → `InvalidCharacter`.
5. **Date of Birth Validation**: 
   - First 6 digits must represent a valid date in DDMMYY format
   - Century derived from first digit following date of birth (century indicator)
   - Valid date range: January 1, 1858 to December 31, 2057 → `InvalidDateOfBirth`.
6. **No Check Digit Validation**: The trailing digit is intentionally NOT validated (discontinued in 2007, cannot determine pre/post status).

### Century Derivation Rules (Must be evaluated in order)

1. **Rule 1**: If century indicator = 0-3 → century = 1900
2. **Rule 2**: If century indicator = 4 AND two-digit year ≤ 36 → century = 2000
3. **Rule 3**: If century indicator = 4 AND two-digit year ≥ 37 → century = 1900
4. **Rule 4**: If century indicator = 5-8 AND two-digit year ≤ 57 → century = 2000
5. **Rule 5**: If century indicator = 5-8 AND two-digit year ≥ 58 → century = 1800
6. **Rule 6**: If century indicator = 9 AND two-digit year ≤ 36 → century = 2000
7. **Rule 7**: If century indicator = 9 AND two-digit year ≥ 37 → century = 1900

Valid date range per rules: January 1, 1858 to December 31, 2057.

---

## Implementation Notes

### Key Differences from Current Pattern

| Aspect | Current (Enum) | New (Union) |
|--------|----------------|------------|
| **Validation Return Type** | `DkPersonnummerValidationResult` enum | `ValidationResult` discriminated union |
| **Error Context** | Implicit (via extension method lookup) | Explicit: each union case can carry `Message` property and context |
| **Exception Type** | `KfValidationException<DkPersonnummerValidationResult>` | `UKfValidationException<ValidationError>` |
| **Message Resolution** | Runtime: enum methods mapped in extension | Build-time: included in union case definition |
| **Exhaustiveness** | Compiler warnings if switch incomplete | Compiler errors if union cases unhandled |
| **Check Digit Algorithm** | NOT validated (intentional) | NOT validated (intentional, unchanged) |

### Reference Implementations

The following implementations serve as templates for this migration:

- **`GbNhsNumber.cs`**: GB patient number union pattern; establishes conventions for discriminated union structure.
- **`UsSocialSecurityNumber.cs`**: US identifier with similar validation complexity.
- **`CaSocialInsuranceNumber.cs`**: Canadian identifier with formatted/unformatted parsing.
- **`MxCurp.cs`**: Mexican identifier with complex date logic.
- **`SePersonnummer.cs`**: Swedish identifier with similar structure (10-11 characters, separator, binary identifier type).
- **`NoFoedselsnummer.cs`**: Norwegian identifier with similar structure.
- **`IsKennitala.cs`**: Icelandic identifier with similar century derivation complexity.

### Special Considerations

**Century Derivation Complexity**: The `DkPersonummer` class has seven century derivation rules with overlapping ranges (especially around the '4' and '9' century indicators). The rules must be evaluated in the specific order listed above to handle these overlaps correctly.

**Strict Separator Validation**: Unlike some other identifier types that allow any non-digit character as a separator, Danish personnummer requires specifically a dash ('-') character. This stricter validation must be preserved.

**No Check Digit Validation**: The trailing digit was originally a modulus 11 check digit but was discontinued in 2007. Since it is impossible to determine pre/post-2007 status, the current implementation intentionally does NOT validate this digit. This behavior must remain unchanged.

**Valid Date Range**: The date range (1858-2057) is wider than the century indicator ranges alone would suggest. The rules must be applied carefully to achieve this specific range.

---

## Testing Strategy

- **Existing Test Suite**: All tests in `DkPersonnummerTests.cs` should continue to exercise the same validation scenarios and test data.
- **Century Rules Coverage**: Ensure tests covering all seven century derivation rules continue to pass, especially edge cases at year/century-indicator boundaries.
- **Century Range Verification**: Ensure tests verify the boundaries (1858 and 2057) correctly.
- **Separator Handling**: Ensure tests for both formatted (with dash separator) and unformatted (without separator) parsing pass.
- **No Check Digit Tests**: Verify that invalid check digit values are still accepted (i.e., no check digit validation occurs).
- **Union Pattern Tests**: Add specific tests for union case exhaustiveness if not already covered by parametrized test data.
- **Backward Compatibility**: Verify that all public methods maintain their behavioral contracts.

---

## Definition of Ready

Before implementation:

- [ ] Current `DkPersonummer` implementation reviewed and validation rules documented.
- [ ] `DkPersonnummerTests.cs` test coverage analyzed to ensure migration test scope.
- [ ] All seven century derivation rules understood and documented.
- [ ] Dash-only separator requirement confirmed and understood.
- [ ] Check digit discontinuation policy understood (NOT validated in this implementation).
- [ ] Union pattern from GB patient number implementations understood and agreed upon.
- [ ] Messages class reviewed for required keys (all should already exist).
- [ ] Team consensus on `ValidationError` and `ValidationResult` union case definitions.

---

## Files to Modify/Delete

### Modify
- `src/KfAccountNumbers/Governmental/Europe/DkPersonummer.cs` - Update to use union pattern, refactor validation logic, update constructor/Create method.
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/DkPersonnummerTests.cs` - Update test helpers and assertions to work with union types (tests themselves remain largely unchanged).

### Delete
- `src/KfAccountNumbers/Governmental/Europe/DkPersonnummerValidationResult.cs` - Contains enum and extensions (no longer needed post-migration).

### Verify
- `src/KfAccountNumbers/Governmental/Europe/DkPersonnummerJsonConverter.cs` - Ensure compatibility with updated constructor.
- `Messages` class - Verify all required error message keys exist.

---

## Success Metrics

- All existing unit tests pass post-migration without scenario changes.
- Build succeeds with no compilation errors or warnings related to this change.
- Code review approves migration for consistency with union-pattern implementations.
- No performance regression detected (date parsing logic unchanged).
- All seven century derivation rules and their edge cases continue to work correctly.
- Century indicator boundary conditions behave as expected (1858-2057 range maintained).
