# S0033: Migrate NlBurgerservicenummer to Use Union Pattern

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 4 points  
**Related:** S0019 (GbNhsNumber), S0022 (GbPatientNumber), S0023-S0032 (Union migrations), E01-UnionMigration

---

## Overview

Modernize the `NlBurgerservicenummer` type to use C# 12 discriminated unions for validation results instead of the current enum-based pattern. This migration aligns `NlBurgerservicenummer` with the new validation architecture introduced in the union-pattern migration initiative and improves type safety, error context, and API consistency across the codebase.

---

## Business Value

* **Type Safety**: Replace error enum values with discriminated union cases that can carry contextual error information (e.g., which character position, what separator was found, validation context).
* **Consistency**: Standardize the validation API across all identifier types (GB patient numbers, US identifiers, European identifiers).
* **Developer Experience**: Enable compiler-checked exhaustiveness on validation results and eliminate string-based error lookups.
* **Maintainability**: Reduce reliance on extension methods and message lookups; move validation context closer to the error definition.
* **Compatibility**: Align with the modern union-pattern approach used by `GbNhsNumber`, `UsSocialSecurityNumber`, `CaSocialInsuranceNumber`, `MxCurp`, `SePersonnummer`, `NoFoedselsnummer`, `IsKennitala`, `DkPersonummer`, `FiHenkilotunnus`, and other recent implementations.

---

## Requirements

### Functional Requirements

#### 1. Replace Enum with Union Pattern
Replace the current `NlBurgerservicenummerValidationResult` enum with discriminated unions mirroring the pattern established by GB patient number validation unions:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigit,
   InvalidSeparator)
{
}

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigit,
   InvalidSeparator)
{
}
```

Each union case must be a record type (e.g., `record EmptyValue(String Message)`) with a `Message` property to carry localized error descriptions. Some union cases may carry additional context such as:
- `InvalidCharacter`: position and character encountered
- `InvalidSeparator`: position and separator information (including mismatch between two separator positions)

#### 2. Update Constructor Exception Documentation
Modify the constructor exception documentation to reference the new union types:
* Change `KfValidationException<NlBurgerservicenummerValidationResult>` to `UKfValidationException<ValidationError>`.
* Ensure all exception cases map to corresponding union cases.

#### 3. Refactor Validation Logic
- Refactor the `Validate` method to return `ValidationResult` union types instead of enum values.
- Each validation failure should instantiate the appropriate union case with a localized message via `Messages` class.
- The success case should return `default(ValidValue)`.

#### 4. Update Create and Constructor Methods
- Update the `Create` method signature to return `CreateResult<NlBurgerservicenummer, ValidationError>`.
- Modify the constructor to check `validationResult.Value is not ValidValue` before throwing `UKfValidationException<ValidationError>`.
- Ensure private validation-bypass constructor remains functional for post-validation object creation.
- Ensure backward compatibility (same behavioral contract; only the types change).

#### 5. Preserve Variant Modulus 11 Check Digit Validation
- The variant modulus 11 check digit algorithm must be preserved exactly as-is.
- The variant uses a weight of -1 for the check digit instead of the standard weight of 1.
- The check digit is calculated from the first 8 digits (excluding separator).

#### 6. Preserve Separator Validation Rules
- Separators are allowed only at positions 4 and 7 (zero-based, counting all characters).
- Both separator positions must use the same character (separators must match).
- Separators must NOT be ASCII digits (0-9); any non-digit character is allowed.
- Common separator characters are dash ('-') and dot ('.'), but others are valid.

#### 7. Handle Formatted and Unformatted Parsing
- Support parsing of both 9-character unformatted numbers and 11-character formatted numbers (with 2 separator characters).
- Separators are optional; the underlying value should be stored without separators.

#### 8. Remove Old Extension Methods
- Delete the `NlBurgerservicenummerValidationResult` enum (after migration is complete).
- Delete the `NlBurgerservicenummerValidationResultExtensions` class (after migration is complete).
- Optionally create a new supporting file if needed for organization.

#### 9. JSON Serialization
- Verify that `NlBurgerservicenummerJsonConverter` continues to work with the updated constructor.
- If needed, update the converter to handle internal validation-bypass logic correctly.

---

### Non-Functional Requirements

#### 1. Backward Compatibility
- The public API surface must remain functionally equivalent.
- Exception types and messages should be consistent with current behavior (though exception structure improves).
- Deprecated types (if any) should be marked with `[Obsolete]` attributes.
- No identifier type enum exists for Dutch burgerservicenummer (simple numeric identifier).

#### 2. Code Organization
- Integrate union types directly into `NlBurgerservicenummer.cs` (similar to GB patient number implementations).
- Keep the main class file focused on business logic.
- Maintain consistent naming with other union-migrated types (e.g., `ValidationError`, `ValidationResult`).

#### 3. Error Messaging
- All error messages must be stored in the `Messages` class and referenceable via `Messages.NlBurgerservicenummer*` keys.
- Ensure parity between current error descriptions and new union case messages:
  - `Empty` → `Messages.NlBurgerservicenummerEmpty`
  - `InvalidLength` → `Messages.NlBurgerservicenummerInvalidLength`
  - `InvalidCharacter` → `Messages.NlBurgerservicenummerInvalidCharacter`
  - `InvalidCheckDigit` → `Messages.NlBurgerservicenummerInvalidCheckDigit`
  - `InvalidSeparator` → `Messages.NlBurgerservicenummerInvalidSeparator`

#### 4. Test Coverage
- All existing unit tests in `NlBurgerservicenummerTests.cs` must continue to pass.
- Test method signatures may be updated to work with union return types, but test scenarios must remain equivalent.
- Ensure all validation paths are still covered post-migration, including:
  - Formatted (11-character) and unformatted (9-character) parsing
  - Variant modulus 11 check digit validation
  - Separator position and matching validation
  - Separation character flexibility (any non-digit allowed)

---

## Acceptance Criteria

### Definition of Done

- [ ] **Union types created**: `ValidationError` and `ValidationResult` discriminated unions defined as nested types within `NlBurgerservicenummer.cs`.
- [ ] **Constructor updated**: Exception documentation and validation logic updated to use `UKfValidationException<ValidationError>`.
- [ ] **Validate method refactored**: Returns `ValidationResult` union; all validation failures instantiate appropriate union cases.
- [ ] **Create method updated**: Signature returns `CreateResult<NlBurgerservicenummer, ValidationError>`.
- [ ] **Check digit validation preserved**: Variant modulus 11 algorithm works correctly with new union pattern.
- [ ] **Separator validation verified**: 
  - Separators at positions 4 and 7 only
  - Both separators must match
  - Any non-digit character allowed as separator
- [ ] **Formatted/unformatted parsing verified**: Both 9 and 11 character inputs parse correctly.
- [ ] **Private validation-bypass constructor updated**: Constructor with `ValidationMode` parameter continues to support validation bypass for post-validated instance creation.
- [ ] **All tests pass**: Existing unit tests in `NlBurgerservicenummerTests.cs` pass without modification (or with minimal adjustments to use new union API).
- [ ] **JSON converter verified**: `NlBurgerservicenummerJsonConverter` works correctly with updated constructor.
- [ ] **Old enum and extensions removed**: `NlBurgerservicenummerValidationResult` enum and `NlBurgerservicenummerValidationResultExtensions` class deleted.
- [ ] **Documentation updated**: XML comments in `NlBurgerservicenummer.cs` reflect new exception types and union patterns.
- [ ] **Error messages verified**: All required error messages present and correct in `Messages` class.
- [ ] **Code review approved**: Migration code reviewed for consistency with other union-pattern implementations (GB patient numbers, US identifiers, European identifiers).

---

## Validation Rules

The following validation rules from the current implementation must be preserved and mapped to appropriate union cases:

1. **Empty/Null Check**: Value may not be `null`, empty, or all whitespace → `EmptyValue`.
2. **Length Validation**: Value must be 9 or 11 characters in length → `InvalidLength`.
3. **Character Validation**: All non-separator characters must be ASCII digits (0-9) → `InvalidCharacter`.
4. **Separator Validation (11-character format only)**:
   - Positions 4 and 7 (zero-based) must contain separator characters if format is 11 characters
   - Separators must NOT be ASCII digits (0-9)
   - Both separator positions must use the same separator character → `InvalidSeparator`.
5. **Check Digit Validation**: Position 8 (the 9th character, rightmost after formatting removal) must be a valid check digit according to the variant modulus 11 algorithm → `InvalidCheckDigit`.

### Variant Modulus 11 Algorithm

The variant modulus 11 algorithm used for burgerservicenummer:
- Assigns a weight of -1 to the check digit (instead of the standard +1)
- Calculates the check digit from the first 8 digits
- All weights and calculations otherwise follow standard modulus 11 conventions

---

## Implementation Notes

### Key Differences from Current Pattern

| Aspect | Current (Enum) | New (Union) |
|--------|----------------|------------|
| **Validation Return Type** | `NlBurgerservicenummerValidationResult` enum | `ValidationResult` discriminated union |
| **Error Context** | Implicit (via extension method lookup) | Explicit: each union case can carry `Message` property and context |
| **Exception Type** | `KfValidationException<NlBurgerservicenummerValidationResult>` | `UKfValidationException<ValidationError>` |
| **Message Resolution** | Runtime: enum methods mapped in extension | Build-time: included in union case definition |
| **Exhaustiveness** | Compiler warnings if switch incomplete | Compiler errors if union cases unhandled |
| **Check Digit Algorithm** | Variant modulus 11 (unchanged) | Variant modulus 11 (unchanged) |

### Reference Implementations

The following implementations serve as templates for this migration:

- **`GbNhsNumber.cs`**: GB patient number union pattern; establishes conventions for discriminated union structure.
- **`UsSocialSecurityNumber.cs`**: US identifier with similar validation complexity.
- **`CaSocialInsuranceNumber.cs`**: Canadian identifier with formatted/unformatted parsing and check digit handling.
- **`SePersonnummer.cs`**: Swedish identifier with similar structure and separator flexibility.

### Special Considerations

**Variant Modulus 11**: The Dutch burgerservicenummer uses a variant of the modulus 11 algorithm with a check digit weight of -1 instead of +1. This non-standard variation must be preserved exactly.

**Separator Flexibility**: Unlike Danish personnummer which requires a dash, Dutch burgerservicenummer allows ANY non-digit character as separator. This flexibility should be maintained. Common separators are dash ('-') and dot ('.'), but the implementation should accept others.

**Separator Consistency**: Both separator positions (4 and 7 in 11-character format) must use the same separator character. This consistency check must be preserved.

**No Personal Information Embedded**: Unlike many other European identifiers, the Dutch burgerservicenummer contains no date of birth or personal information other than the trailing check digit. The validation is primarily about check digit correctness and format compliance.

---

## Testing Strategy

- **Existing Test Suite**: All tests in `NlBurgerservicenummerTests.cs` should continue to exercise the same validation scenarios and test data.
- **Format Coverage**: Ensure both 9-character (unformatted) and 11-character (formatted) parsing pass.
- **Separator Character Coverage**: Ensure tests using different separator characters (dash, dot, etc.) pass.
- **Separator Mismatch Detection**: Ensure mismatched separators (different characters in positions 4 and 7) are correctly rejected.
- **Check Digit Coverage**: Ensure variant modulus 11 check digit validation works correctly.
- **Union Pattern Tests**: Add specific tests for union case exhaustiveness if not already covered by parametrized test data.
- **Backward Compatibility**: Verify that all public methods maintain their behavioral contracts.

---

## Definition of Ready

Before implementation:

- [ ] Current `NlBurgerservicenummer` implementation reviewed and validation rules documented.
- [ ] `NlBurgerservicenummerTests.cs` test coverage analyzed to ensure migration test scope.
- [ ] Variant modulus 11 check digit algorithm understood and documented.
- [ ] Separator validation rules (position, matching, non-digit requirement) understood and documented.
- [ ] Formatted (11-char) and unformatted (9-char) parsing behavior understood and documented.
- [ ] Union pattern from GB patient number implementations understood and agreed upon.
- [ ] Messages class reviewed for required keys (all should already exist).
- [ ] Team consensus on `ValidationError` and `ValidationResult` union case definitions.

---

## Files to Modify/Delete

### Modify
- `src/KfAccountNumbers/Governmental/Europe/NlBurgerservicenummer.cs` - Update to use union pattern, refactor validation logic, update constructor/Create method.
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/NlBurgerservicenummerTests.cs` - Update test helpers and assertions to work with union types (tests themselves remain largely unchanged).

### Delete
- `src/KfAccountNumbers/Governmental/Europe/NlBurgerservicenummerValidationResult.cs` - Contains enum and extensions (no longer needed post-migration).

### Verify
- `src/KfAccountNumbers/Governmental/Europe/NlBurgerservicenummerJsonConverter.cs` - Ensure compatibility with updated constructor.
- `Messages` class - Verify all required error message keys exist.

---

## Success Metrics

- All existing unit tests pass post-migration without scenario changes.
- Build succeeds with no compilation errors or warnings related to this change.
- Code review approves migration for consistency with union-pattern implementations.
- No performance regression detected (variant modulus 11 check digit validation unchanged).
- Separator validation and format parsing continue to work correctly with various separator characters.
