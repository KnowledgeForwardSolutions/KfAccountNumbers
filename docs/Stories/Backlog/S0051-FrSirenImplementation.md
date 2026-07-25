# User Story: Implement FrSiren Type for French Business Registration Number

## Summary

Implement a new strongly-typed business object representing a French SIREN (Système d'Identification du Répertoire des Entreprises - Business Registration Identification System) number. The SIREN is a 9-digit identifier issued by INSEE (Institut National de la Statistique et des Études Économiques - National Institute for Statistics and Economic Studies) to uniquely identify any business entity registered in France, including sole proprietorships, partnerships, and corporations.

The SIREN is a fundamental identifier for French business registration, tax administration, and statistical purposes. Unlike personal identifiers, SIREN numbers do not encode personal information and use the Luhn algorithm for check digit validation. The type will follow the union-based validation pattern established in previous stories (similar to ChSozialversicherungsnummer), with comprehensive validation of the 9-digit numeric format and Luhn check digit algorithm.

Implement as:
```csharp
public record FrSiren
{
    // Single unrestricted type accepting all valid French SIREN values
    // Does not encode any personal data
    // Uses Luhn algorithm for check digit validation
    // No subtype hierarchy
}
```

---

## Business Value

* **French Business Compliance**: Supports French business identification and administrative processing, essential for companies operating in or with French business entities.
* **Tax Integration**: Aligns with French tax systems (DGFIP - Direction Générale des Finances Publiques) and INSEE administrative identification.
* **Statistical Reporting**: Enables integration with French statistical databases maintained by INSEE.
* **Data Integrity**: Validates numeric format and Luhn check digit, ensuring data accuracy and integrity.
* **European Expansion**: Enables the library to serve business domains across European countries with business identifiers.
* **Type Safety**: Compile-time enforcement via strongly-typed business object; eliminates string-based SIREN processing.
* **Validation Robustness**: Encapsulates SIREN validation rules (format, numeric structure, Luhn check digit algorithm).
* **Developer Clarity**: Self-documenting API; developers immediately recognize this as a French business identifier.

---

## Requirements

### Functional Requirements

#### 1. French SIREN Format and Structure

The French SIREN number consists of 9 digits with the following structure:

```
NNNNNNNNNC
```

Where:
- `NNNNNNNNN` - First 8 digits (business identifier portion)
- `C` - Check digit calculated using the Luhn algorithm on the first 8 digits

**Format Variants:**
- **Unformatted**: 9 consecutive digits (e.g., "123456789")
- **Formatted**: 3 groups of 3 digits separated by spaces: "NNN NNN NNN" (e.g., "123 456 789")

**Additional Context:**
- SIREN numbers are always numeric (no letters)
- No personal information is encoded
- Valid SIREN numbers range from 000000001 to 999999999
- All-zero SIREN (000000000) is invalid
- SIREN is typically paired with SIRET (Système d'Identification du Répertoire des Établissements - establishment number) which appends 5 additional digits for location/establishment identification

---

### Validation Rules

#### 1. Core Validation Rules (All SIREN Values)

- [ ] Value may not be null, empty, or all whitespace characters
- [ ] Value must be exactly 9 characters (unformatted) or 11 characters (formatted with 2 separators)
- [ ] All non-separator characters must be ASCII digits ('0'-'9')
- [ ] All digits cannot be zero (000000000 is invalid)
- [ ] Separator characters (if formatted) must not be ASCII digits
- [ ] Separator characters (if formatted) must be at positions 3 and 6 (0-based), and both must be the same character
- [ ] The last digit must be a valid Luhn check digit calculated from the first 8 digits

#### 2. Luhn Algorithm Details

The Luhn algorithm, also known as the modulus 10 algorithm, is applied to validate the check digit:

1. Starting from the second-to-last digit, double every second digit
2. If the result of doubling is greater than 9, subtract 9
3. Sum all the digits
4. The total modulo 10 must equal 0

For SIREN:
- Digits 1-8 (positions 0-7) are processed
- Digit 9 (position 8) is the check digit
- The check digit is calculated so that the sum of all 9 digits (with processing) modulo 10 equals 0

---

### Example SIREN Values

| SIREN (Unformatted) | SIREN (Formatted) | Type | Notes |
| :--- | :--- | :--- | :--- |
| 732789823 | 732 789 823 | Valid | Real-world example (Disney France) |
| 732041045 | 732 041 045 | Valid | Real-world example |
| 402110707 | 402 110 707 | Valid | Large corporation |
| 123456785 | 123 456 785 | Valid | Example with valid check digit |
| 000000000 | 000 000 000 | Invalid | All zeros |
| 123456789 | 123 456 789 | Invalid | Invalid Luhn check digit (should be 785) |

---

## Acceptance Criteria

### 1. Type Structure and Basic Properties
- [ ] Create `FrSiren` public record type in `src/KfAccountNumbers/National/Europe/FrSiren.cs`
- [ ] Type accepts 9-digit numeric identifier with Luhn check digit validation
- [ ] `Value` property stores normalized 9-digit unformatted string
- [ ] Type supports both formatted (11 chars with spaces) and unformatted (9 chars) input

### 2. Union Types for Validation
- [ ] `ValidationError` discriminated union with cases:
  - EmptyValue
  - InvalidLength
  - InvalidCharacter
  - InvalidChecksum
  - InvalidSeparator
- [ ] `ValidationResult` discriminated union with cases:
  - ValidValue
  - EmptyValue
  - InvalidLength
  - InvalidCharacter
  - InvalidChecksum
  - InvalidSeparator

### 3. Public Constants
- [ ] `UnformattedLength = 9` - standard 9-digit SIREN
- [ ] `FormattedLength = 11` - SIREN with 2 separator characters
- [ ] `CheckDigitAlgorithmName = "Luhn"` - algorithm name
- [ ] `DefaultFormatMask = "_ _ _"` or similar - default formatting mask
- [ ] Internal constants for separator positions (3, 6)

### 4. Constructors and Factory Methods
- [ ] Primary constructor accepting `String?` value with automatic validation
- [ ] Private constructor accepting `ValidationMode` parameter to bypass validation
- [ ] `static CreateResult<FrSiren, ValidationError> Create(String? value)` - Result pattern factory
- [ ] `static ValidationResult Validate(String? value)` - Validation without exception throwing
- [ ] All constructors properly handle null, empty, and whitespace input

### 5. Operators and Conversions
- [ ] Implicit operator: `FrSiren` to `String` (returns `Value` or empty string if null)
- [ ] Explicit operator: `String?` to `FrSiren` (invokes constructor, may throw)

### 6. Format and Display Methods
- [ ] `String Format(String mask = DefaultFormatMask)` - Format using mask
- [ ] `override String ToString()` - Returns unformatted 9-digit value
- [ ] Support for multiple formatting masks

### 7. Validation Implementation
- [ ] Null/empty/whitespace check (returns EmptyValue)
- [ ] Length check - exactly 9 or 11 characters (returns InvalidLength)
- [ ] Separator validation if formatted:
  - Separators at positions 3 and 6
  - Separators are not digits
  - Both separators are the same character
  - Returns InvalidSeparator if violated
- [ ] Character validation:
  - All non-separator positions must be digits
  - Returns InvalidCharacter with position info if violated
- [ ] All-zeros check:
  - Value cannot be "000000000"
  - Returns InvalidChecksum if all zeros
- [ ] Luhn check digit validation:
  - Last digit (position 8) must be valid Luhn check
  - Returns InvalidChecksum if fails
- [ ] Strategic validation order:
  1. Null/empty check (fastest)
  2. Length check
  3. Character/separator validation
  4. all-zeros check
  5. Luhn check digit (most expensive, last)

### 8. JSON Serialization
- [ ] Create `FrSirenJsonConverter : JsonConverter<FrSiren>`
- [ ] Deserialize from string, apply validation
- [ ] Serialize to unformatted 9-digit string
- [ ] Handle null values gracefully

### 9. Equality and GetHashCode
- [ ] Implement value-based equality (two FrSiren instances equal if Value is equal)
- [ ] Record type handles this automatically
- [ ] Formatted and unformatted versions of same SIREN are equal

### 10. Formatting Support
- [ ] Support custom formatting masks
- [ ] Default mask produces "NNN NNN NNN" format
- [ ] Document mask syntax compatibility with existing `FormatWithMask` extension

---

## Testing Requirements

### 1. Valid SIREN Values
- [ ] Valid unformatted values (9 digits)
- [ ] Valid formatted values (11 characters with space or other separators)
- [ ] Real-world SIREN numbers from INSEE database
- [ ] Edge cases: small numbers, large numbers, all same digit patterns

### 2. Invalid Length
- [ ] Too short: 8 digits
- [ ] Too long: 10 digits
- [ ] Formatted values with wrong separator positions
- [ ] Extreme cases: 1 digit, 100 digits

### 3. Invalid Characters
- [ ] Non-digit characters in unformatted value
- [ ] Unicode edge cases (fractions, accented characters, Tamil digits)
- [ ] Letters (A-Z, a-z) in digit positions
- [ ] Special characters (punctuation) in digit positions
- [ ] Separate tests for each position in the 9-digit value

### 4. Invalid Separators (Formatted Values)
- [ ] Digits at separator positions (3 and 6)
- [ ] Separator at wrong positions
- [ ] Mismatched separators (different characters)
- [ ] Multiple different separator characters

### 5. Invalid Check Digits
- [ ] Single digit transcription errors
- [ ] Digit transposition errors
- [ ] Complete check digit replacement
- [ ] All detectable and undetectable Luhn errors

### 6. All-Zeros Validation
- [ ] "000000000" is invalid
- [ ] "000000001" through "000000009" are valid (if Luhn passes)
- [ ] "000 000 000" formatted is invalid

### 7. Operators and Conversions
- [ ] Implicit conversion to string
- [ ] Explicit conversion from string (valid)
- [ ] Explicit conversion from string (invalid, throws)
- [ ] Null handling in conversions

### 8. Format Method
- [ ] Default mask produces expected output
- [ ] Custom masks work correctly
- [ ] Invalid masks throw appropriate exceptions

### 9. Equality Methods
- [ ] Equal instances return true
- [ ] Different instances return false
- [ ] Formatted vs unformatted same value returns true
- [ ] Comparison to null returns false
- [ ] Comparison to different types returns false
- [ ] Hash codes equal for equal values

### 10. JSON Serialization
- [ ] Deserialize valid SIREN strings
- [ ] Serialize instances to strings
- [ ] Handle null in JSON
- [ ] Invalid SIRENs in JSON throw or return error appropriately

---

## Implementation Approach

### Phase 1: Type Definition and Constants
1. Create `src/KfAccountNumbers/National/Europe/FrSiren.cs`
2. Define `ValidationError` and `ValidationResult` discriminated unions
3. Define public and internal constants
4. Define `Value` property

### Phase 2: Constructors and Validation Pipeline
1. Implement private validation pipeline methods
2. Implement `Validate()` static method with strategic validation order
3. Implement constructors with proper exception mapping
4. Add `ValidationMode` parameter support for bypass validation

### Phase 3: Factory Methods and Operators
1. Implement `Create()` factory method using Result pattern
2. Implement implicit/explicit operators
3. Implement `ToString()` override

### Phase 4: Formatting and Display
1. Implement `Format()` method
2. Create `FrSirenNumberCheckDigitMask` for masked validation
3. Test with various mask patterns

### Phase 5: JSON Support
1. Create `FrSirenJsonConverter : JsonConverter<FrSiren>`
2. Implement Read/Write methods
3. Handle null and invalid values

### Phase 6: Comprehensive Testing
1. Create `tests/KfAccountNumbers.Tests.Unit/National/Europe/FrSirenTests.cs`
2. Organize tests by category (constructor, Create, Validate, Format, Equals, JSON)
3. Use MemberData for data-driven tests
4. Test all validation error paths

### Phase 7: Documentation
1. Create `docs/Reference/National/Europe/FrSiren.md`
2. Document format, validation rules, examples
3. Update README.md to reference FrSiren
4. Add XML documentation to type

---

## Design Considerations

### Algorithm Selection: Luhn vs. Modulus 97
- **Decision**: Use Luhn (Modulus 10) algorithm
- **Rationale**: Official INSEE SIREN validation uses Luhn algorithm
- **Implementation**: Use existing `Algorithms.Modulus10` from codebase if available, otherwise implement

### Input Normalization
- Strip leading/trailing whitespace
- Accept both formatted and unformatted input
- Normalize to unformatted 9-digit representation for storage
- Support multiple separator characters (space, dash, period, etc.)

### Edge Cases
- All-zeros validation separate from Luhn check
- Zero-prefixed numbers valid (e.g., "007654321" if Luhn passes)
- Single digit to 9-digit numbers all potentially valid

### Formatting
- Default format: "NNN NNN NNN" (spaces)
- Support custom separators via mask
- Follow existing FormatWithMask extension pattern

---

## Risk Analysis

| Risk | Probability | Impact | Mitigation |
| :--- | :--- | :--- | :--- |
| Luhn algorithm implementation error | **Low** | High | Reuse existing Modulus10 implementation; extensive test coverage |
| All-zeros check confusion | **Low** | Medium | Clear separation from Luhn check; explicit tests |
| Separator position errors | **Low** | Medium | Comprehensive formatted value tests; boundary testing |
| International character handling | **Low** | Low | ASCII-only requirement clearly documented |
| Real-world SIREN validation failures | **Low** | High | Validate against INSEE database samples; user feedback |

---

## Files to Be Created

- `src/KfAccountNumbers/National/Europe/FrSiren.cs` — Main type implementation
- `src/KfAccountNumbers/National/Europe/FrSirenJsonConverter.cs` — JSON converter (or inline)
- `tests/KfAccountNumbers.Tests.Unit/National/Europe/FrSirenTests.cs` — Comprehensive test suite
- `docs/Reference/National/Europe/FrSiren.md` — Reference documentation

## Files to Be Modified

- `README.md` — Add FrSiren to type listing
- `src/KfAccountNumbers/National/Europe/ChSozialversicherungsnummer.cs` — Reference pattern (read-only)

## Definition of Done

- [ ] Type implementation complete and compiles
- [ ] All acceptance criteria validated
- [ ] All unit tests passing (>95% coverage)
- [ ] JSON serialization working correctly
- [ ] Formatting with masks working
- [ ] Code review completed
- [ ] Documentation complete and accurate
- [ ] Real-world SIREN examples validated
- [ ] No performance regressions
- [ ] Pull request merged to main branch

## Estimated Effort

- **Research & Planning**: 2-3 hours
- **Type Implementation**: 4-5 hours
- **Validation Pipeline**: 3-4 hours
- **JSON Converter**: 1-2 hours
- **Formatting Support**: 1-2 hours
- **Testing**: 6-8 hours (comprehensive test data)
- **Documentation**: 2-3 hours
- **Code Review & Refinement**: 2-3 hours
- **Total Estimated**: 21-30 hours

## Priority

Medium — Extends library capability for French business domain, follows established patterns

## Related Issues/PRs

- Builds on pattern established by ChSozialversicherungsnummer (S0046)
- References Luhn algorithm documentation from existing implementations
- S0047-Documentation_Expansion (create FrSiren.md)

## Technical Notes

### Luhn Algorithm Implementation Example

```csharp
private static Boolean ValidateLuhn(ReadOnlySpan<Char> digits)
{
   Int32 sum = 0;
   Boolean isSecondDigit = false;

   // Process digits from right to left (excluding check digit)
   for (Int32 i = digits.Length - 2; i >= 0; i--)
   {
      Int32 digit = digits[i] - '0';

      if (isSecondDigit)
      {
         digit *= 2;
         if (digit > 9)
         {
            digit -= 9;
         }
      }

      sum += digit;
      isSecondDigit = !isSecondDigit;
   }

   // Last digit is check digit
   Int32 checkDigit = digits[^1] - '0';
   return (sum + checkDigit) % 10 == 0;
}
```

### Separator Position Validation

For formatted (11-character) SIREN:
- Position 3 (0-based): separator (non-digit)
- Position 6 (0-based): separator (non-digit)
- Both separators must be identical
- All other positions must be digits

Example: "732 789 823"
- Positions: 0=7, 1=3, 2=2, **3= **, 4=7, 5=8, 6=9, **6= **, 7=8, 8=2, 9=3

### SIREN Components

Full SIREN structure in administrative systems:
- **SIREN**: 9 digits (business identifier)
- **SIRET**: SIREN + 5 digits for establishment number (14 digits total)

This implementation focuses only on SIREN (9 digits), not SIRET.

---

## References & Resources

- **Official INSEE Documentation**: https://www.insee.fr/en/information/2028129 (SIREN/SIRET)
- **SIREN Validation Algorithm**: https://www.insee.fr/en/statistiques/1417286 (French source)
- **Luhn Algorithm**: https://en.wikipedia.org/wiki/Luhn_algorithm
- **French Business Registration**: https://www.infogreffe.fr/ (Greffe du Tribunal de Commerce)

## Success Metrics

- ✓ All acceptance criteria implemented and verified
- ✓ 100% code coverage for validation paths
- ✓ All real-world SIREN examples validate correctly
- ✓ Matches performance characteristics of similar types
- ✓ Follows established codebase patterns
- ✓ Documentation clear and comprehensive
- ✓ Zero validation false positives/negatives
