# S0019-GbNhsNumber Business Object

## As a
Developer integrating UK National Health Service patient identification numbers into my application

## I want
A strongly typed business object that represents a UK NHS Number (National Health Service number for England, Wales, and the Isle of Man)

## So that
I can validate, parse, and work with NHS numbers in a type-safe manner with comprehensive validation including check digit verification

## Acceptance Criteria

### Structure and Validation
- [ ] The `GbNhsNumber` type represents a UK NHS Number
- [ ] The number is a 10-digit identifier where:
  - First 9 digits = unique identifier
  - 10th digit = check digit calculated using modulus 11 algorithm
- [ ] Total length is 10 digits (unformatted) or 12 characters (formatted with spaces)
- [ ] Constructor accepts string representation and throws `UKfValidationException<GbNhsNumber.ValidationError>` if invalid
- [ ] Static `Validate` method returns `GbNhsNumber.ValidationResult` enumeration value
- [ ] Static `Create` method uses Result pattern returning `CreateResult<GbNhsNumber, GbNhsNumber.ValidationError>`
- [ ] `GbNhsNumber.ValidationError` is a union of the following types: `EmptyValue`, `InvalidLength`, `InvalidCharacter`, `InvalidCheckSum`, `InvalidSeparator` and `GbUniquePatientIdentifierInvalidRange`
- [ ] `GbNhsNumber.ValidationResult` extends `GbNhsNumber.ValidationError` with the type `ValidValue` to indicate a sucessful validation.

### Validation Rules
- [ ] Value may not be null, empty, or all whitespace
- [ ] Value must be either 10 characters (unformatted) or 12 characters (formatted with separator characters)
- [ ] All 10 characters must be ASCII digits ('0'-'9')
- [ ] If separator characters present, must be at positions 3 and 7 (zero-based) forming pattern: NNN NNN NNNN
- [ ] If separator characters are present, they must not be ASCII digits ('0'-'9') and both separators must be the same character
- [ ] Check digit (10th digit) must be valid according to modulus 11 algorithm
- [ ] The first nine digits must be in one of the allowed ranges: 400 000 000 to 499 999 999 or 600 000 000 to 799 999 999 or 900 000 000 to 999 999 999 (900 series block reserved for testing purposes)

### Check Digit Algorithm (Modulus 11)
GbNhsNumber should use the CheckDigits.Net Modulus11Decimal algorithm for validation of
the check digit. For reference, the check digit is calculated as follows:
1. Multiply each of the first 9 digits by a weight (11 - position), where position is 1-based:
   - Digit 1 × 10
   - Digit 2 × 9
   - Digit 3 × 8
   - Digit 4 × 7
   - Digit 5 × 6
   - Digit 6 × 5
   - Digit 7 × 4
   - Digit 8 × 3
   - Digit 9 × 2
2. Sum all the products
3. Calculate remainder: sum modulo 11
4. Subtract remainder from 11: check digit = 11 - remainder
5. If check digit is 11, use 0 instead
6. If check digit is 10, the number is **invalid** (no NHS number should have check digit 10)
7. The calculated check digit must match the 10th digit

### Format Support
- [ ] Accept unformatted: 1234567881
- [ ] Accept formatted with spaces: 123 456 7881
- [ ] `Format` method with optional mask parameter (default: "___ ___ ____")

### Properties
- [ ] `Value` property returns raw 10-character string (digits only, no spaces)
- [ ] `IdentifierType` property returns a union of NhsNumber and TestNumber

### No Embedded Intelligence
- [ ] NHS number does not encode date of birth
- [ ] NHS number does not encode gender
- [ ] NHS number does not encode geographic information
- [ ] Only validation intelligence is the check digit and valid range

### Operators and Methods
- [ ] Implicit conversion to string
- [ ] Explicit conversion from string
- [ ] `ToString` returns raw value (10 digits, no spaces)
- [ ] Proper equality implementation (space-insensitive)
- [ ] JSON serialization/deserialization support via `GbNhsNumberJsonConverter`

### Special Cases
- [ ] Normalize separators during validation
- [ ] Reject numbers where calculated check digit would be 10
- [ ] Test numbers (900 million range) should validate but be identifiable via property

### Test Coverage
- [ ] Valid NHS numbers with correct check digits
- [ ] Invalid check digits
- [ ] Check digit edge case: calculated digit would be 11 (should use 0)
- [ ] Check digit edge case: calculated digit would be 10 (should be invalid number)
- [ ] Standard number range (400 000 000 to 499 999 999 or 600 000 000 to 799 999 999)
- [ ] Test number range (900 000 0000 to 999 999 9999)
- [ ] Both formatted and unformatted inputs
- [ ] Format and ToString methods
- [ ] Separator normalization
- [ ] Invalid separator characters
- [ ] Equality and hash code (space-insensitive)
- [ ] JSON serialization round-trip
- [ ] Conversion operators
- [ ] Create method Result pattern
- [ ] Null, empty, and whitespace inputs
- [ ] Invalid lengths
- [ ] Non-digit characters

### Documentation
- [ ] XML documentation for all public members
- [ ] README.md section with:
  - Structure explanation (9-digit identifier + check digit)
  - Validation rules
  - Format examples (with and without spaces)
  - Valid number ranges (standard and test)
  - Purpose (NHS patient identification in England, Wales, Isle of Man)
  - Historical context (introduced 1996, replaced older systems)
  - Check digit calculation example with step-by-step walkthrough
  - Note about Scotland using CHI number instead
  - Note about Northern Ireland using H&C number instead
  - Geographic coverage (England, Wales, Isle of Man only)
  - No personal information encoded
  - Example values with descriptions
  - NHS Digital and Wikipedia references

### Performance
- [ ] Use `ReadOnlySpan<Char>` for validation and parsing
- [ ] Efficient check digit calculation
- [ ] Single-pass validation where possible
- [ ] Minimal string allocations
- [ ] Separator normalization efficient

## Notes
- NHS Number = National Health Service Number
- Introduced in 1996 to replace previous patient identification systems
- Used for patient identification in England, Wales, and the Isle of Man
- Scotland uses CHI (Community Health Index) number instead
- Northern Ireland uses Health and Care Number (H&C Number) instead
- Format: 9 digits + 1 check digit
- Standard display format with spaces: NNN NNN NNNN
- Check digit uses modulus 11 algorithm with weights [10,9,8,7,6,5,4,3,2]
- Check digit of 10 makes the NHS number invalid
- Numbers 000 000 0000 to 009 999 9999 reserved for testing
- Numbers 999 000 0000 to 999 999 9999 reserved for temporary registrations
- No embedded personal information (unlike some other national IDs)
- Unique per patient and not reused
- Leading zeros are significant and must be preserved
- See: https://en.wikipedia.org/wiki/NHS_number
- See: https://www.nhs.uk/using-the-nhs/about-the-nhs/what-is-an-nhs-number/
- See: https://www.datadictionary.nhs.uk/attributes/nhs_number.html
- See: https://digital.nhs.uk/services/nhs-number (NHS Digital)
- See: https://webarchive.nationalarchives.gov.uk/ukgwa/20231221081503/https://digital.nhs.uk/about-nhs-digital/contact-us/freedom-of-information/freedom-of-information-disclosure-log/december-2022/nic-690159-k8h4z

## Technical Details
- Namespace: `KfAccountNumbers.Governmental.Europe`
- Files:
  - `src/KfAccountNumbers/Governmental/Europe/GbNhsNumber.cs`
  - `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/GbNhsNumberTests.cs`
- JSON converter: `GbNhsNumberJsonConverter`
- Target: .NET 11, C# 15.0
- Pattern similar to `GbNino` for structure, but with check digit like `NlBurgerservicenummer`
- ISO 3166-1 alpha-2 code: GB (Great Britain, commonly used for UK)

## Example Values

### Standard NHS Numbers (Valid)
- 4000000004 - Minimum standard NHS number with valid check digit
- 400 000 0004 - Same, formatted with spaces
- 7999999997 - Maximum valid NHS number with valid check digit
- 799 999 9997 - Same, formatted

### Test Numbers (Valid but Special)
- 9000000009 - Minimum test number with valid check digit
- 900 000 0009 - Same, formatted
- 9999999980 - Maximum test number with valid check digit
- 999 999 9980 - Same, formatted

### Invalid Examples
- 450557710**5** - Invalid check digit (should be 4)
- 450 557 710**3** - Invalid check digit (should be 4)
- 450557710 - Too short (9 digits instead of 10)
- 45055771041 - Too long (11 digits instead of 10)
- 450A557104 - Non-digit character
- 4505577104X - Non-digit character
- 4505 577104 - Invalid space positions
- (Check digit 10 example if one exists) - Invalid because check digit cannot be 10

## Implementation Notes
1. Place in namespace: `KfAccountNumbers.Governmental.Europe`
2. Use ISO country code: `Gb` (Great Britain/UK)
3. Follow the same patterns as `NlBurgerservicenummer` (also has check digit) and `GbNino` (same country)
4. Create corresponding error union: `GbNhsNumber.ValidationError`
1. Create corresponding validtion union `GbNhsNumber.ValidationResult`
5. Implement full unit test coverage similar to other European national identifiers
6. Support both formatted (with separator characters) and unformatted input
7. Store value in unformatted 10-digit format
8. The check digit validation is mandatory - no valid NHS number has invalid check digit
9. Implement special range detection (test) with IdentifierType property
11. Document that this is for England, Wales, and Isle of Man only (not Scotland or Northern Ireland)
12. Comprehensive test coverage for check digit algorithm including edge cases

## Special Considerations
- **Check Digit 10**: If the modulus 11 algorithm produces check digit 10, the number is invalid
  - No valid NHS number can have check digit 10
  - This is different from some other modulus 11 systems that substitute a letter
- **Check Digit 11**: If algorithm produces 11, use 0 as the check digit
- **Geographic Scope**: Only valid for England, Wales, and Isle of Man
  - Scotland uses CHI (Community Health Index) number - different format
  - Northern Ireland uses H&C (Health and Care) Number - different format
  - Document this clearly to avoid confusion
- **Special Ranges**: 
  - Test numbers (900 000 000 to 999 999 999) are valid but should be identifiable
- **No Intelligence**: Unlike some national IDs, NHS numbers contain no encoded personal information
  - No date of birth
  - No gender indicator
  - No geographic information
  - Only the check digit provides validation intelligence
