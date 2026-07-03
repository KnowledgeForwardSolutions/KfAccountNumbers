# S0032: Migrate FiHenkilotunnus to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 5 points  
**Related:** S0019 (GbNhsNumber), S0022 (GbPatientNumber), S0023-S0031 (Union migrations), E01-UnionMigration

---

## Overview

Modernize the `FiHenkilotunnus` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `FiHenkilotunnus` with the new validation architecture introduced in the union-pattern migration initiative and improves type safety, error context, and API consistency across the codebase.

---

## Business Value

* **Type Safety**: Replace error enum values with discriminated union cases that can carry contextual error information (e.g., which character position, what century indicator was encountered, validation context).
* **Consistency**: Standardize the validation API across all identifier types (GB patient numbers, US identifiers, European identifiers).
* **Developer Experience**: Enable compiler-checked exhaustiveness on validation results and eliminate string-based error lookups.
* **Maintainability**: Reduce reliance on extension methods and message lookups; move validation context closer to the error definition.
* **Compatibility**: Align with the modern union-pattern approach used by `GbNhsNumber`, `UsSocialSecurityNumber`, `CaSocialInsuranceNumber`, `MxCurp`, `SePersonnummer`, `NoFoedselsnummer`, `IsKennitala`, `DkPersonummer`, and other recent implementations.

---

## Requirements

### Functional Requirements

#### 1. Replace Enum with Union Pattern
Replace the current `FiHenkilotunnusValidationResult` enum with discriminated unions mirroring the pattern established by GB patient number validation unions:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCenturyIndicator,
   InvalidCheckDigit,
   InvalidIndividualNumber,
   InvalidDateOfBirth)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCenturyIndicator,
   InvalidCheckDigit,
   InvalidIndividualNumber,
   InvalidDateOfBirth)
{
}
```

Each union case must be a record type (e.g., `record EmptyValue(String Message)`) with a `Message` property to carry localized error descriptions. Some union cases may carry additional context such as:
- `InvalidCharacter`: position and character encountered
- `InvalidCenturyIndicator`: the century indicator character found
- `InvalidIndividualNumber`: the three-digit individual number value

#### 2. Update Constructor Exception Documentation
Modify the constructor exception documentation to reference the new union types:
* Change `KfValidationException<FiHenkilotunnusValidationResult>` to `UKfValidationException<ValidationError>`.
* Ensure all exception cases map to corresponding union cases.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types instead of enum values.
- Each validation failure should instantiate the appropriate union case with a localized message via `Messages` class.
- The success case should return `default(ValidValue)`.

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<FiHenkilotunnus, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing `UKfValidationException<ValidationError>`.
- Ensure private validation-bypass constructor remains functional for post-validation object creation.
- Ensure backward compatibility (same behavioral contract; only the types change).

#### 5. Preserve Modulus 31 Check Character Validation
- The check character algorithm (modulus 31) must be preserved exactly as-is.
- The 31 valid check characters are: `0123456789ABCDEFHJKLMNPRSTUVWXY` (excluding G, I, O, Q, Z).
- The check character is calculated from the first 8 digits and the century indicator.

#### 6. Preserve Century Indicator Validation
- The century indicator must be one of 12 valid characters: `+`, `-`, `U`, `V`, `W`, `X`, `Y`, `A`, `B`, `C`, `D`, `E`, `F`.
- Valid date range (January 1, 1800 to December 31, 2099) must be maintained.
- Century mapping: `+` = 1800s, `-/U/V/W/X/Y` = 1900s, `A/B/C/D/E/F` = 2000s.

#### 7. Preserve Individual Number Validation
- Individual number must be ≥ 002 (i.e., not 000 or 001).
- Individual numbers 002-899 indicate persons born in Finland or permanent residents.
- Individual numbers 900-999 are reserved for temporary identifiers/test values.
- Gender is encoded in the last digit: even = female, odd = male.

#### 8. Remove Old Extension Methods
- Delete the `FiHenkilotunnusValidationResult` enum (after migration is complete).
- Delete the `FiHenkilotunnusValidationResultExtensions` class (after migration is complete).
- Optionally create a new supporting file if needed for organization.

#### 9. JSON Serialization
- Verify that `FiHenkilotunnusJsonConverter` continues to work with the updated constructor.
- If needed, update the converter to handle internal validation-bypass logic correctly.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- The public API surface must remain functionally equivalent.
- Exception types and messages should be consistent with current behavior (though exception structure improves).
- Deprecated types (if any) should be marked with `[Obsolete]` attributes.
- No identifier type enum exists for Finnish henkilotunnus (simpler than Icelandic or Belgian types).

#### 2. Code Organization
- Integrate union types directly into `FiHenkilotunnus.cs` (similar to GB patient number implementations).
- Keep the main class file focused on business logic.
- Maintain consistent naming with other union-migrated types (e.g., `ValidationError`, `ValidationResult`).

#### 3. Error Messaging
- All error messages must be stored in the `Messages` class and referenceable via `Messages.FiHenkilotunnus*` keys.
- Ensure parity between current error descriptions and new union case messages:
  - `Empty` → `Messages.FiHenkilotunnusEmpty`
  - `InvalidLength` → `Messages.FiHenkilotunnusInvalidLength`
  - `InvalidCharacter` → `Messages.FiHenkilotunnusInvalidCharacter`
  - `InvalidCenturyIndicator` → `Messages.FiHenkilotunnusInvalidCenturyIndicator`
  - `InvalidCheckDigit` → `Messages.FiHenkilotunnusInvalidCheckDigit`
  - `InvalidIndividualNumber` → `Messages.FiHenkilotunnusInvalidIndividualNumber`
  - `InvalidDateOfBirth` → `Messages.FiHenkilotunnusInvalidDateOfBirth`

#### 4. Test Coverage
- All existing unit tests in `FiHenkilotunnusTests.cs` must continue to pass.
- Test method signatures may be updated to work with union return types, but test scenarios must remain equivalent.
- Ensure all validation paths are still covered post-migration, including:
  - Century indicator validation for all 12 valid characters
  - Individual number boundary conditions (002 minimum, 999 maximum)
  - Modulus 31 check character validation
  - Gender encoding (even/odd validation)
  - Date validity across all three centuries (1800s, 1900s, 2000s)

---

## Acceptance Criteria

### Definition of Done

- [ ] **Union types created**: `ValidationError` and `ValidationResult` discriminated unions defined as nested types within `FiHenkilotunnus.cs`.
- [ ] **Constructor updated**: Exception documentation and validation logic updated to use `UKfValidationException<ValidationError>`.
- [ ] **Validate method refactored**: Returns `ValidationResult` union; all validation failures instantiate appropriate union cases.
- [ ] **Create method updated**: Signature returns `CreateResult<FiHenkilotunnus, ValidationError>`.
- [ ] **Century indicator validation preserved**: All 12 valid century indicators (`+`, `-`, `U`, `V`, `W`, `X`, `Y`, `A`, `B`, `C`, `D`, `E`, `F`) work correctly.
- [ ] **Individual number validation verified**: Minimum (002) and maximum (999) boundary checks work correctly.
- [ ] **Check character validation preserved**: Modulus 31 algorithm and 31-character set (`0123456789ABCDEFHJKLMNPRSTUVWXY`) work correctly.
- [ ] **Gender encoding preserved**: Odd/even check in last digit still functions correctly.
- [ ] **Private validation-bypass constructor updated**: Constructor with `ValidationMode` parameter continues to support validation bypass for post-validated instance creation.
- [ ] **All tests pass**: Existing unit tests in `FiHenkilotunnusTests.cs` pass without modification (or with minimal adjustments to use new union API).
- [ ] **JSON converter verified**: `FiHenkilotunnusJsonConverter` works correctly with updated constructor.
- [ ] **Old enum and extensions removed**: `FiHenkilotunnusValidationResult` enum and `FiHenkilotunnusValidationResultExtensions` class deleted.
- [ ] **Documentation updated**: XML comments in `FiHenkilotunnus.cs` reflect new exception types and union patterns.
- [ ] **Error messages verified**: All required error messages present and correct in `Messages` class.
- [ ] **Code review approved**: Migration code reviewed for consistency with other union-pattern implementations (GB patient numbers, US identifiers, European identifiers).

---

## Validation Rules

The following validation rules from the current implementation must be preserved and mapped to appropriate union cases:

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace → `EmptyValue`.
2. **Length Validation**: Value must be 11 characters in length → `InvalidLength`.
3. **Character Validation**: Date of birth (positions 0-5) and individual number (positions 7-9) must be ASCII digits (0-9) → `InvalidCharacter`.
4. **Century Indicator Validation**: Character at position 6 must be one of: `+`, `-`, `U`, `V`, `W`, `X`, `Y`, `A`, `B`, `C`, `D`, `E`, `F` → `InvalidCenturyIndicator`.
5. **Individual Number Range**: Three-digit individual number (positions 7-9) must be ≥ 002 (not 000 or 001) → `InvalidIndividualNumber`.
6. **Check Character Validation**: Character at position 10 (rightmost) must be a valid modulus 31 check character → `InvalidCheckDigit`.
7. **Date of Birth Validation**: 
   - First 6 digits must represent a valid date in DDMMYY format
   - Century derived from century indicator at position 6
   - Valid date range: January 1, 1800 to December 31, 2099 → `InvalidDateOfBirth`.

### Century Mapping

- `+` → 1800s
- `-`, `U`, `V`, `W`, `X`, `Y` → 1900s
- `A`, `B`, `C`, `D`, `E`, `F` → 2000s

---

## Implementation Notes

### Key Differences from Current Pattern

| Aspect | Current (Enum) | New (Union) |
|--------|----------------|------------|
| **Validation Return Type** | `FiHenkilotunnusValidationResult` enum | `ValidationResult` discriminated union |
| **Error Context** | Implicit (via extension method lookup) | Explicit: each union case can carry `Message` property and context |
| **Exception Type** | `KfValidationException<FiHenkilotunnusValidationResult>` | `UKfValidationException<ValidationError>` |
| **Message Resolution** | Runtime: enum methods mapped in extension | Build-time: included in union case definition |
| **Exhaustiveness** | Compiler warnings if switch incomplete | Compiler errors if union cases unhandled |
| **Check Character Algorithm** | Modulus 31 (unchanged) | Modulus 31 (unchanged) |

### Reference Implementations

The following implementations serve as templates for this migration:

- **`GbNhsNumber.cs`**: GB patient number union pattern; establishes conventions for discriminated union structure.
- **`UsSocialSecurityNumber.cs`**: US identifier with similar validation complexity.
- **`MxCurp.cs`**: Mexican identifier with complex date logic and check character validation.
- **`SePersonnummer.cs`**: Swedish identifier with similar structure.
- **`IsKennitala.cs`**: Icelandic identifier with similar century indicator mapping.

### Special Considerations

**Modulus 31 Check Character**: The Finnish henkilotunnus uses modulus 31, not modulus 11. The 31 valid check characters exclude `G`, `I`, `O`, `Q`, and `Z` to avoid confusion with digit characters. This algorithm must be preserved exactly.

**Century Indicator Range**: The century indicator has 12 valid values spanning three different centuries. The mapping must be correct to derive the full birth year from the two-digit year in the identifier.

**Individual Number Significance**: Unlike some other identifiers where the individual number is purely sequential, the Finnish individual number contains semantic meaning (gender encoding, residency status through the 002-899 vs 900-999 range split).

---

## Testing Strategy

- **Existing Test Suite**: All tests in `FiHenkilotunnusTests.cs` should continue to exercise the same validation scenarios and test data.
- **Century Indicator Coverage**: Ensure tests for all 12 valid century indicator characters pass.
- **Individual Number Boundaries**: Ensure tests for minimum (002) and maximum (999) values pass, and that 000/001 are rejected.
- **Check Character Coverage**: Ensure modulus 31 algorithm and all 31 valid check characters work correctly.
- **Gender Encoding**: Ensure both odd and even individual number endings work correctly.
- **Date Range Coverage**: Ensure dates in all three centuries (1800s, 1900s, 2000s) validate correctly.
- **Union Pattern Tests**: Add specific tests for union case exhaustiveness if not already covered by parametrized test data.
- **Backward Compatibility**: Verify that all public methods maintain their behavioral contracts.

---

## Definition of Ready

Before implementation:

- [ ] Current `FiHenkilotunnus` implementation reviewed and validation rules documented.
- [ ] `FiHenkilotunnusTests.cs` test coverage analyzed to ensure migration test scope.
- [ ] All 12 century indicator characters and their century mappings understood and documented.
- [ ] Individual number validation rules (002+ minimum, gender encoding) understood and documented.
- [ ] Modulus 31 check character algorithm and 31-character set understood and documented.
- [ ] Union pattern from GB patient number implementations understood and agreed upon.
- [ ] Messages class reviewed for required keys (all should already exist).
- [ ] Team consensus on `ValidationError` and `ValidationResult` union case definitions.

---

## Files to Modify/Delete

### Modify
- `src/KfAccountNumbers/Governmental/Europe/FiHenkilotunnus.cs` - Update to use union pattern, refactor validation logic, update constructor/Create method.
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/FiHenkilotunnusTests.cs` - Update test helpers and assertions to work with union types (tests themselves remain largely unchanged).

### Delete
- `src/KfAccountNumbers/Governmental/Europe/FiHenkilotunnusValidationResult.cs` - Contains enum and extensions (no longer needed post-migration).

### Verify
- `src/KfAccountNumbers/Governmental/Europe/FiHenkilotunnusJsonConverter.cs` - Ensure compatibility with updated constructor.
- `Messages` class - Verify all required error message keys exist.

---

## Success Metrics

- All existing unit tests pass post-migration without scenario changes.
- Build succeeds with no compilation errors or warnings related to this change.
- Code review approves migration for consistency with union-pattern implementations.
- No performance regression detected (modulus 31 check character validation and date parsing logic unchanged).
- All century indicator mappings and individual number boundary conditions continue to work correctly.
