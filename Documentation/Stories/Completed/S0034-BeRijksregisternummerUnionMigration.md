# S0034: Migrate BeRijksregisternummer to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 6 points  
**Related:** S0019 (GbNhsNumber), S0022 (GbPatientNumber), S0023-S0033 (Union migrations), E01-UnionMigration

---

## Overview

Modernize the `BeRijksregisternummer` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `BeRijksregisternummer` with the new validation architecture introduced in the union-pattern migration initiative and improves type safety, error context, and API consistency across the codebase.

---

## Business Value

* **Type Safety**: Replace error enum values with discriminated union cases that can carry contextual error information (e.g., which character position, what separator was found, validation context).
* **Consistency**: Standardize the validation API across all identifier types (GB patient numbers, US identifiers, European identifiers).
* **Developer Experience**: Enable compiler-checked exhaustiveness on validation results and eliminate string-based error lookups.
* **Maintainability**: Reduce reliance on extension methods and message lookups; move validation context closer to the error definition.
* **Compatibility**: Align with the modern union-pattern approach used by `GbNhsNumber`, `UsSocialSecurityNumber`, `CaSocialInsuranceNumber`, `MxCurp`, `SePersonnummer`, `NoFoedselsnummer`, `IsKennitala`, `DkPersonummer`, `FiHenkilotunnus`, `NlBurgerservicenummer`, and other recent implementations.

---

## Requirements

### Functional Requirements

#### 1. Replace Enum with Union Pattern
Replace the current `BeRijksregisternummerValidationResult` enum with discriminated unions mirroring the pattern established by GB patient number validation unions:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigits,
   InvalidSeparator,
   InvalidSequenceNumber,
   InvalidDateOfBirth)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigits,
   InvalidSeparator,
   InvalidSequenceNumber,
   InvalidDateOfBirth)
{
}
```

Each union case must be a record type (e.g., `record EmptyValue(String Message)`) with a `Message` property to carry localized error descriptions. Some union cases may carry additional context such as:
- `InvalidCharacter`: position and character encountered
- `InvalidSeparator`: position and separator information
- `InvalidSequenceNumber`: the sequence number value found (000 or 999)

#### 2. Update Constructor Exception Documentation
Modify the constructor exception documentation to reference the new union types:
* Change `KfValidationException<BeRijksregisternummerValidationResult>` to `UKfValidationException<ValidationError>`.
* Ensure all exception cases map to corresponding union cases.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types instead of enum values.
- Each validation failure should instantiate the appropriate union case with a localized message via `Messages` class.
- The success case should return `default(ValidValue)`.

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<BeRijksregisternummer, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing `UKfValidationException<ValidationError>`.
- Ensure private validation-bypass constructor remains functional for post-validation object creation.
- Ensure backward compatibility (same behavioral contract; only the types change).

#### 5. Handle IdentifierType Property
- The `IdentifierType` property (which returns `BeIdentifierType` enum: `Rijksregisternummer` or `BisNummer`) should remain unchanged.
- Validation logic must correctly distinguish between regular rijksregisternummer and BIS-nummer during all validation checks.
- BIS-nummer is identified by 40 or 20 added to the month component (month 41-60 for regular BIS, month 61-80 for gender-unknown BIS).

#### 6. Preserve Modulus 97 Check Digit Validation
- The modulus 97 check digit algorithm must be preserved exactly as-is.
- Check digits are calculated from YYMMDDXXX (date and sequence number).
- For 1900s births, the check digit is calculated as `97 - (YYMMDDXXX mod 97)`.
- For 2000s births, the check digit is calculated as `97 - ((2YYMMDDXXX) mod 97)` where the number is prefixed with 2.

#### 7. Preserve Complex Date of Birth Handling
- Support incomplete and unknown date of birth scenarios through special formatting:
  - Incomplete dates use zeros (e.g., YY.00.00 for year known only)
  - Unknown date of birth uses 00.00.01
  - Dates can "roll over" with 01 used for day when exceeding capacity (rare)
- Date validation must accept these special cases while still validating calendar dates when fully specified.

#### 8. Preserve Sequence Number Validation
- Sequence number must NOT be 000 or 999.
- Gender is encoded in the last digit: odd = male, even = female.
- Sequence numbers 001-499 for males, 500-998 for females (typically).

#### 9. Preserve BIS-Nummer Offset Handling
- BIS-nummer adds 40 to the month component (months 41-60 for known gender, months 61-80 for unknown gender).
- The offset must be subtracted before date validation to verify date validity.
- The `IdentifierType` property must correctly identify BIS-nummer vs rijksregisternummer.

#### 10. Handle Formatted and Unformatted Parsing
- Support parsing of both 11-character unformatted numbers and 15-character formatted numbers (with separators).
- Format: YY.MM.DD-XXX.CC (typical) or YYMMDDXXXCC (without separators).
- Separators are optional; the underlying value should be stored without separators.

#### 11. Remove Old Extension Methods
- Delete the `BeRijksregisternummerValidationResult` enum (after migration is complete).
- Delete the `BeRijksregisternummerValidationResultExtensions` class (after migration is complete).
- Optionally create a new supporting file if needed for organization.

#### 12. JSON Serialization
- Verify that `BeRijksregisternummerJsonConverter` continues to work with the updated constructor.
- If needed, update the converter to handle internal validation-bypass logic correctly.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- The public API surface must remain functionally equivalent.
- Exception types and messages should be consistent with current behavior (though exception structure improves).
- Deprecated types (if any) should be marked with `[Obsolete]` attributes.
- The `IdentifierType` property and its return type (`BeIdentifierType` enum) remain unchanged.

#### 2. Code Organization
- Integrate union types directly into `BeRijksregisternummer.cs` (similar to GB patient number implementations).
- Keep the main class file focused on business logic.
- Maintain consistent naming with other union-migrated types (e.g., `ValidationError`, `ValidationResult`).

#### 3. Error Messaging
- All error messages must be stored in the `Messages` class and referenceable via `Messages.BeRijksregisternummer*` keys.
- Ensure parity between current error descriptions and new union case messages:
  - `Empty` → `Messages.BeRijksregisternummerEmpty`
  - `InvalidLength` → `Messages.BeRijksregisternummerInvalidLength`
  - `InvalidCharacter` → `Messages.BeRijksregisternummerInvalidCharacter`
  - `InvalidCheckDigits` → `Messages.BeRijksregisternummerInvalidCheckDigits`
  - `InvalidSeparator` → `Messages.BeRijksregisternummerInvalidSeparator`
  - `InvalidSequenceNumber` → `Messages.BeRijksregisternummerInvalidSequenceNumber`
  - `InvalidDateOfBirth` → `Messages.BeRijksregisternummerInvalidDateOfBirth`

#### 4. Test Coverage
- All existing unit tests in `BeRijksregisternummerTests.cs` must continue to pass.
- Test method signatures may be updated to work with union return types, but test scenarios must remain equivalent.
- Ensure all validation paths are still covered post-migration, including:
  - Both regular rijksregisternummer and BIS-nummer validation
  - Both 1900s and 2000s century detection via modulus 97
  - Incomplete and unknown date of birth scenarios
  - Sequence number boundary conditions (001-499 for males, 500-998 for females)
  - Formatted (15-character) and unformatted (11-character) parsing

---

## Acceptance Criteria

### Definition of Done

- [ ] **Union types created**: `ValidationError` and `ValidationResult` discriminated unions defined as nested types within `BeRijksregisternummer.cs`.
- [ ] **Constructor updated**: Exception documentation and validation logic updated to use `UKfValidationException<ValidationError>`.
- [ ] **Validate method refactored**: Returns `ValidationResult` union; all validation failures instantiate appropriate union cases.
- [ ] **Create method updated**: Signature returns `CreateResult<BeRijksregisternummer, ValidationError>`.
- [ ] **IdentifierType property preserved**: `IdentifierType` property remains unchanged, returning `BeIdentifierType` enum correctly for both Rijksregisternummer and BisNummer.
- [ ] **BIS-nummer offset handling verified**: Validation correctly handles 40-day month offset (months 41-60) and 20-day offset for unknown gender (months 61-80).
- [ ] **Modulus 97 check digit validation preserved**: Both 1900s and 2000s century detection methods work correctly.
- [ ] **Complex date handling verified**: Incomplete dates (with zeros), unknown dates (00.00.01), and date rollover (rare) scenarios work correctly.
- [ ] **Sequence number validation verified**: 000 and 999 are rejected; valid range 001-998 accepted.
- [ ] **Private validation-bypass constructor updated**: Constructor with `ValidationMode` parameter continues to support validation bypass for post-validated instance creation.
- [ ] **All tests pass**: Existing unit tests in `BeRijksregisternummerTests.cs` pass without modification (or with minimal adjustments to use new union API).
- [ ] **JSON converter verified**: `BeRijksregisternummerJsonConverter` works correctly with updated constructor.
- [ ] **Old enum and extensions removed**: `BeRijksregisternummerValidationResult` enum and `BeRijksregisternummerValidationResultExtensions` class deleted.
- [ ] **Documentation updated**: XML comments in `BeRijksregisternummer.cs` reflect new exception types and union patterns.
- [ ] **Error messages verified**: All required error messages present and correct in `Messages` class.
- [ ] **Code review approved**: Migration code reviewed for consistency with other union-pattern implementations (GB patient numbers, US identifiers, European identifiers).

---

## Validation Rules

The following validation rules from the current implementation must be preserved and mapped to appropriate union cases:

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace → `EmptyValue`.
2. **Length Validation**: Value must be 11 or 15 characters in length → `InvalidLength`.
3. **Character Validation**: All non-separator characters must be ASCII digits (0-9) → `InvalidCharacter`.
4. **Separator Validation (15-character format only)**:
   - Positions 2, 5, 8, 12 (zero-based) must contain non-digit separator characters
   - Separator positions must not be ASCII digits (0-9) → `InvalidSeparator`.
5. **Sequence Number Validation**: Positions 6-8 (three-digit sequence) must NOT be 000 or 999 → `InvalidSequenceNumber`.
6. **Check Digit Validation**: Positions 9-10 (two-digit check sum) must be valid modulus 97 check digits → `InvalidCheckDigits`.
7. **Date of Birth Validation**:
   - First 6 digits (YYMMDD) represent date of birth
   - BIS-nummer has 40 or 20 added to month (detect by month value 41-80)
   - Subtract BIS offset before validation if applicable
   - Allow special incomplete/unknown date patterns (zeros for unknown components)
   - Valid complete dates: January 1, 1900 to December 31, 2099 → `InvalidDateOfBirth`.

### Check Digit Calculation Rules

**For 1900s births** (CC = 97 - (YYMMDDXXX mod 97)):
- Take the 9-digit date and sequence number
- Calculate modulus 97
- Subtract from 97 to get check digits

**For 2000s births** (CC = 97 - ((2YYMMDDXXX) mod 97)):
- Prefix the 9-digit date and sequence number with 2 (making it 10 digits)
- Calculate modulus 97 of this 10-digit number
- Subtract from 97 to get check digits

---

## Implementation Notes

### Key Differences from Current Pattern

| Aspect | Current (Enum) | New (Union) |
|--------|----------------|------------|
| **Validation Return Type** | `BeRijksregisternummerValidationResult` enum | `ValidationResult` discriminated union |
| **Error Context** | Implicit (via extension method lookup) | Explicit: each union case can carry `Message` property and context |
| **Exception Type** | `KfValidationException<BeRijksregisternummerValidationResult>` | `UKfValidationException<ValidationError>` |
| **Message Resolution** | Runtime: enum methods mapped in extension | Build-time: included in union case definition |
| **Exhaustiveness** | Compiler warnings if switch incomplete | Compiler errors if union cases unhandled |
| **IdentifierType Property** | Returns `BeIdentifierType` enum (unchanged) | Returns `BeIdentifierType` enum (unchanged) |
| **Check Digit Algorithm** | Modulus 97 (unchanged) | Modulus 97 (unchanged) |

### Reference Implementations

The following implementations serve as templates for this migration:

- **`GbNhsNumber.cs`**: GB patient number union pattern; establishes conventions for discriminated union structure.
- **`UsSocialSecurityNumber.cs`**: US identifier with similar validation complexity.
- **`CaSocialInsuranceNumber.cs`**: Canadian identifier with check digit handling and formatted/unformatted parsing.
- **`IsKennitala.cs`**: Icelandic identifier with similar 40-day offset logic and IdentifierType property.
- **`NoFoedselsnummer.cs`**: Norwegian identifier with similar structure and offset handling.

### Special Considerations

**Dual Check Digit Method**: The Belgian rijksregisternummer uses modulus 97 check digits but with two different calculation methods depending on century of birth. Both methods must be attempted during validation (try 1900s calculation first, if that fails try 2000s calculation).

**Complex Date Handling**: The date of birth can be partially unknown or completely unknown, and the handling is complex with multiple special cases. Zero padding is used to indicate unknown date components, and special values like 00.00.01 indicate completely unknown dates. This sophistication must be preserved.

**BIS-Nummer Offset Stacking**: BIS-nummer can stack multiple offsets:
- 40 added to month for non-residents
- Additional 20 added if gender is unknown (making total 60)
This creates the ranges 41-60 (known gender) and 61-80 (unknown gender) for BIS-nummer identification.

**IdentifierType Property**: Unlike some other identifiers where the type is determined purely by range checking, the Belgian identifier's type affects validation significantly (month offset interpretation). The property must continue to work correctly post-migration.

**Formatted Display**: The typical display format YY.MM.DD-XXX.CC has separators at positions 2, 5, 8, and 12 (zero-based). This must be supported for both input parsing and output formatting.

---

## Testing Strategy

- **Existing Test Suite**: All tests in `BeRijksregisternummerTests.cs` should continue to exercise the same validation scenarios and test data.
- **Century Detection Coverage**: Ensure tests for both 1900s and 2000s century detection (via dual modulus 97 calculation) pass.
- **BIS-Nummer Coverage**: Ensure tests for both regular rijksregisternummer and BIS-nummer (month offsets 40 and 20) pass.
- **Date Validation Coverage**: Ensure tests for complete dates, incomplete dates (with zeros), unknown dates, and date rollover scenarios pass.
- **Sequence Number Boundaries**: Ensure 000 and 999 are rejected, and 001-998 are accepted.
- **Format Coverage**: Ensure both 11-character (unformatted) and 15-character (formatted) parsing pass.
- **Union Pattern Tests**: Add specific tests for union case exhaustiveness if not already covered by parametrized test data.
- **Backward Compatibility**: Verify that all public methods maintain their behavioral contracts.

---

## Definition of Ready

Before implementation:

- [ ] Current `BeRijksregisternummer` implementation reviewed and validation rules documented.
- [ ] `BeRijksregisternummerTests.cs` test coverage analyzed to ensure migration test scope.
- [ ] Modulus 97 check digit algorithms (both 1900s and 2000s) understood and documented.
- [ ] BIS-nummer offset rules (40 for known gender, 20 additional for unknown) understood and documented.
- [ ] Complex date handling rules (incomplete, unknown, rollover) understood and documented.
- [ ] Sequence number ranges and validation understood and documented.
- [ ] Union pattern from GB patient number implementations understood and agreed upon.
- [ ] Messages class reviewed for required keys (all should already exist).
- [ ] Team consensus on `ValidationError` and `ValidationResult` union case definitions.

---

## Files to Modify/Delete

### Modify
- `src/KfAccountNumbers/Governmental/Europe/BeRijksregisternummer.cs` - Update to use union pattern, refactor validation logic, update constructor/Create method.
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/BeRijksregisternummerTests.cs` - Update test helpers and assertions to work with union types (tests themselves remain largely unchanged).

### Delete
- `src/KfAccountNumbers/Governmental/Europe/BeRijksregisternummerValidationResult.cs` - Contains enum and extensions (no longer needed post-migration).

### Verify
- `src/KfAccountNumbers/Governmental/Europe/BeRijksregisternummerJsonConverter.cs` - Ensure compatibility with updated constructor.
- `Messages` class - Verify all required error message keys exist.

---

## Success Metrics

- All existing unit tests pass post-migration without scenario changes.
- Build succeeds with no compilation errors or warnings related to this change.
- Code review approves migration for consistency with union-pattern implementations.
- No performance regression detected (modulus 97 check digit validation and date parsing logic unchanged).
- All century detection methods, BIS-nummer offset handling, and complex date scenarios continue to work correctly.
- IdentifierType property correctly distinguishes between Rijksregisternummer and BisNummer in all validation scenarios.
