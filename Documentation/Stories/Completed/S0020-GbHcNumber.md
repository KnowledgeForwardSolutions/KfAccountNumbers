# S0020-GbHcNumber Business Object

## As a
Developer integrating Northern Ireland Health and Care Service patient identification numbers into my application

## I want
A strongly typed business object that represents a Northern Ireland Health and Care Number (H&C Number)

## So that
I can validate, parse, and work with H&C numbers in a type-safe manner with comprehensive validation including check digit verification

## Acceptance Criteria

### Structure and Validation
- [x] The `GbHcNumber` type represents a Northern Ireland Health and Care Number (H&C Number)
- [x] The number is a 10-digit identifier where:
  - First 9 digits = unique identifier
  - 10th digit = check digit calculated using modulus 11 algorithm
- [x] Total length is 10 digits (unformatted) or 12 characters (formatted with spaces)
- [x] Constructor accepts string representation and throws `UKfValidationException<GbHcNumber.ValidationError>` if invalid
- [x] Static `Validate` method returns `GbHcNumber.ValidationResult` union value
- [x] Static `Create` method uses Result pattern returning `CreateResult<GbHcNumber, GbHcNumber.ValidationError>`
- [x] `GbHcNumber.ValidationError` is a union of the following types: `EmptyValue`, `InvalidLength`, `InvalidCharacter`, `InvalidCheckSum`, `InvalidSeparator` and `GbUniquePatientIdentifierInvalidRange`
- [x] `GbHcNumber.ValidationResult` extends `GbHcNumber.ValidationError` with the type `ValidValue` to indicate a sucessful validation.

### Validation Rules
- [x] Value may not be null, empty, or all whitespace
- [x] Value must be either 10 characters (unformatted) or 12 characters (formatted with separator characters)
- [x] All 10 characters must be ASCII digits ('0'-'9')
- [x] If separator characers are present, must be at positions 3 and 7 (zero-based) forming pattern: NNN NNN NNNN
- [x] If separator characters are present, they must not be ASCII digits ('0'-'9') and both separators must be the same character
- [x] Check digit (10th digit) must be valid according to modulus 11 algorithm
- [x] The first nine digits must be in one of the allowed ranges: 320 000 000 to 399 999 999 or 900 000 000 to 999 999 999 (900 series block reserved for testing purposes)

### Check Digit Algorithm (Modulus 11)
GbHcNumber should use the CheckDigits.Net Modulus11Decimal algorithm for validation of
the check digit. For reference, the check digit is calculated using the modulus 11 algorithm:
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
6. If check digit is 10, the number is **invalid** (no H&C number should have check digit 10)
7. The calculated check digit must match the 10th digit

### Format Support
- [x] Accept unformatted: 3200000007
- [x] Accept formatted with separator characters: 320 000 0007
- [x] `Format` method with optional mask parameter (default: "___ ___ ____")

### Properties
- [x] `Value` property returns raw 10-character string (digits only, no spaces)
- [x] `IdentifierType` property returns a union of HcNumber and TestNumber

### No Embedded Intelligence
- [x] H&C number does not encode date of birth
- [x] H&C number does not encode gender
- [x] H&C number does not encode specific geographic information beyond Northern Ireland
- [x] Only validation intelligence is the check digit and valid range

### Operators and Methods
- [x] Implicit conversion to string
- [x] Explicit conversion from string
- [x] `ToString` returns raw value (10 digits, no spaces)
- [x] Proper equality implementation (space-insensitive)
- [x] JSON serialization/deserialization support via `GbHcNumberJsonConverter`

### Special Cases
- [x] Normalize separator characters during validation
- [x] Reject numbers where calculated check digit would be 10
- [x] Standard numbers (320 000 000 to 399 999 999) should be identifiable via property
- [x] Test numbers (900 000 000 to 399 999 999) should be identifiable via property

### Test Coverage
- [x] Valid H&C numbers with correct check digits
- [x] Invalid check digits
- [x] Check digit edge case: calculated digit would be 11 (should use 0)
- [x] Check digit edge case: calculated digit would be 10 (should be invalid number)
- [x] Standard number range (320 000 000 to 399 999 999)
- [x] Test number range (900 000 0000 to 999 999 9999)
- [x] Both formatted and unformatted inputs
- [x] Format and ToString methods
- [x] Separator character normalization
- [x] Invalid separator characters
- [x] Equality and hash code (space-insensitive)
- [x] JSON serialization round-trip
- [x] Conversion operators
- [x] Create method Result pattern
- [x] Null, empty, and whitespace inputs
- [x] Invalid lengths
- [x] Non-digit characters

### Documentation
- [x] XML documentation for all public members
- [x] README.md section with:
  - Structure explanation (9-digit identifier + check digit)
  - Validation rules
  - Format examples (with and without spaces)
  - Valid number ranges (standard and test)
  - Purpose (patient identification in Northern Ireland)
  - Historical context (replaced previous systems, aligned with NHS model)
  - Relationship to NHS Number (England/Wales) and CHI Number (Scotland)
  - Geographic coverage (Northern Ireland only)
  - No personal information encoded
  - Example values with descriptions
  - HSC Northern Ireland and relevant references

### Performance
- [x] Use `ReadOnlySpan<Char>` for validation and parsing
- [x] Efficient check digit calculation
- [x] Single-pass validation where possible
- [x] Minimal string allocations
- [x] Space normalization efficient

## Notes
- H&C Number = Health and Care Number
- Used for patient identification in Northern Ireland only
- Introduced to align with similar systems in rest of UK
- England/Wales uses NHS Number
- Scotland uses CHI (Community Health Index) Number
- Northern Ireland uses H&C Number (this system)
- Format: 9 digits + 1 check digit
- Standard display format with spaces: NNN NNN NNNN
- Check digit uses modulus 11 algorithm with weights [10,9,8,7,6,5,4,3,2]
- Check digit of 10 makes the H&C number invalid
- Other prefixes may be valid for special administrative purposes
- No embedded personal information (unlike some other national IDs)
- Unique per patient and not reused
- Leading zeros are significant and must be preserved
- Algorithmically very similar to NHS Number but with different prefix conventions
- See: https://www.health-ni.gov.uk/ (HSC Northern Ireland)
- See: https://www.nidirect.gov.uk/articles/health-and-care-number
- See: Health and Social Care Board Northern Ireland documentation
- See: https://webarchive.nationalarchives.gov.uk/ukgwa/20231221081503/https://digital.nhs.uk/about-nhs-digital/contact-us/freedom-of-information/freedom-of-information-disclosure-log/december-2022/nic-690159-k8h4z

## Technical Details
- Namespace: `KfAccountNumbers.Governmental.Europe`
- Files:
  - `src/KfAccountNumbers/Governmental/Europe/GbHcNumber.cs`
  - `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/GbHcNumberTests.cs`
- JSON converter: `GbHcNumberJsonConverter`
- Target: .NET 11, C# 15.0
- External library: CheckDigits.Net 3.1.0
- Pattern similar to `GbNhsNumber` (same check digit algorithm and structure)
- ISO 3166-1 alpha-2 code: GB (Great Britain, commonly used for UK including Northern Ireland)

## Example Values

### Standard H&C Number
- 3200000007 - Standard H&C number
- 320 000 0007 - Same, formatted with spaces
- 3999999993 - Another valid standard H&C number
- 399 999 9993 - Same, formatted

### Test Numbers (Valid but Special)
- 9000000009 - Minimum test number with valid check digit
- 900 000 0009 - Same, formatted
- 9999999980 - Maximum test number with valid check digit
- 999 999 9980 - Same, formatted

### Invalid Examples
- 3201234561 - Invalid check digit (wrong digit)
- 320 123 4561 - Invalid check digit (formatted)
- 320123456 - Too short (9 digits instead of 10)
- 32012345601 - Too long (11 digits instead of 10)
- 320A234560 - Non-digit character
- 3201234560X - Non-digit character
- 3201 234560 - Invalid space positions
- 320-123-4560 - Invalid separator (dashes not allowed)
- (Check digit 10 example) - Invalid because check digit cannot be 10

## Implementation Notes
1. Place in namespace: `KfAccountNumbers.Governmental.Europe`
2. Use ISO country code: `Gb` (Great Britain/UK)
3. Follow the same patterns as `GbNhsNumber` (same check digit algorithm and structure)
4. Create corresponding enum: `GbHcNumberValidationResult`
6. Implement full unit test coverage similar to `GbNhsNumber`
7. Support both formatted (with spaces) and unformatted input
8. Store value in unformatted 10-digit format
9. The check digit validation is mandatory - no valid H&C number has invalid check digit
12. Document that this is for Northern Ireland only (not England, Wales, Scotland, Isle of Man)
13. Comprehensive test coverage for check digit algorithm including edge cases
14. Note similarity to NHS Number but with different prefix conventions

## Special Considerations
- **Check Digit 10**: If the modulus 11 algorithm produces check digit 10, the number is invalid
  - No valid H&C number can have check digit 10
  - Same rule as NHS Number
- **Check Digit 11**: If algorithm produces 11, use 0 as the check digit
- **Geographic Scope**: Only valid for Northern Ireland
  - England, Wales, Isle of Man use NHS Number
  - Scotland uses CHI Number
  - Northern Ireland uses H&C Number (this system)
  - Document this clearly to avoid confusion
- **No Intelligence**: Like NHS Number, H&C numbers contain no encoded personal information
  - No date of birth
  - No gender indicator
  - No specific geographic information (beyond Northern Ireland)
  - Only the check digit provides validation intelligence
- **Algorithm Similarity**: Uses same modulus 11 algorithm as NHS Number
  - Weights: [10,9,8,7,6,5,4,3,2]
  - Same edge cases (10 invalid, 11 becomes 0)
  - Can potentially share check digit validation logic with GbNhsNumber

## References
- [NI Direct - Health and Care Number](https://www.nidirect.gov.uk/articles/health-and-care-number)
- [Health and Social Care in Northern Ireland](https://www.health-ni.gov.uk/)
- [HSC Business Services Organisation](https://hscbusiness.hscni.net/)
- [Wikipedia - NHS number](https://en.wikipedia.org/wiki/NHS_number) (includes H&C Number context)
- [Patient and Client Council NI](https://www.patientclientcouncil.hscni.net/)

## Implementation Phases
1. **Phase 1**: Basic structure, length validation, digit validation
2. **Phase 2**: Check digit algorithm implementation (can share with GbNhsNumber if appropriate)
3. **Phase 3**: Prefix detection and RegistrationType enum implementation
4. **Phase 4**: Format support (with spaces), space normalization
6. **Phase 6**: Conversion operators, equality
7. **Phase 7**: JSON serialization support
8. **Phase 8**: Full test coverage including all edge cases and prefixes
9. **Phase 9**: Documentation, README updates, code review

## Relationship to Other UK Identifiers
This H&C Number complements the existing UK identifier types in this repository:
- **GbNino** (S0018): UK National Insurance Number - for taxation and benefits
- **GbNhsNumber** (S0019): NHS Number - for England, Wales, Isle of Man healthcare
- **GbHcNumber** (S0020): H&C Number - for Northern Ireland healthcare (this story)
- Future: **GbChiNumber**: CHI Number - for Scotland healthcare (separate story needed)

Together these provide comprehensive coverage of UK governmental identification numbers.
