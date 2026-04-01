# S0019-GbNhsNumber Business Object

## As a
Developer integrating UK healthcare identification numbers into my application

## I want
A strongly typed business object that represents a UK NHS Number

## So that
I can validate, parse, and work with UK National Health Service numbers in a type-safe manner with comprehensive validation including check digit verification

## Acceptance Criteria

### Structure and Validation
- [ ] The `GbNhsNumber` type represents a UK NHS Number
- [ ] The number is structured as 10 digits (NNNNNNNNNN) where:
  - First 9 digits are the identifier
  - 10th digit is a check digit calculated using modulus 11 algorithm
- [ ] Constructor accepts string representation and throws `KfValidationException<GbNhsNumberValidationResult>` if invalid
- [ ] Static `Validate` method returns `GbNhsNumberValidationResult` enumeration value
- [ ] Static `Create` method uses Result pattern returning `CreateResult<GbNhsNumber, GbNhsNumberValidationResult>`

### Validation Rules
- [ ] Value may not be null, empty, or all whitespace
- [ ] Value must be either 10 characters (unformatted) or 12 characters (formatted with spaces/dashes)
- [ ] All digits must be ASCII digits ('0'-'9')
- [ ] If separators present, must be at positions 3 and 6 (zero-based), forming pattern: NNN-NNN-NNNN
- [ ] Separator character can be dash, space, or any non-digit character (must be consistent)
- [ ] Check digit must be valid according to modulus 11 algorithm
- [ ] Numbers with calculated check digit of 10 are invalid (rejected by the algorithm)

### Format Support
- [ ] Accept unformatted: 1234567881
- [ ] Accept formatted with dashes: 123-456-7881
- [ ] Accept formatted with spaces: 123 456 7881
- [ ] Accept other non-digit separators: 123.456.7881
- [ ] `Format` method with optional mask parameter (default: "___-___-____")

### Properties
- [ ] `Value` property returns raw 10-character string (digits only, no separators)
- [ ] `CheckDigit` property returns the check digit (10th digit) as int

### Check Digit Algorithm
- [ ] Use weighted modulus 11 algorithm
- [ ] Weights applied left to right: 10, 9, 8, 7, 6, 5, 4, 3, 2 for the first 9 digits
- [ ] Sum = (d1 × 10) + (d2 × 9) + (d3 × 8) + (d4 × 7) + (d5 × 6) + (d6 × 5) + (d7 × 4) + (d8 × 3) + (d9 × 2)
- [ ] Remainder = Sum mod 11
- [ ] Check digit = 11 - Remainder
- [ ] If check digit = 11, then check digit = 0
- [ ] If check digit = 10, the number is invalid and should be rejected
- [ ] Validate that the 10th digit matches the calculated check digit

### Operators and Methods
- [ ] Implicit conversion to string
- [ ] Explicit conversion from string
- [ ] `ToString` returns raw value (10 digits, no separators)
- [ ] Proper equality implementation
- [ ] JSON serialization/deserialization support via `GbNhsNumberJsonConverter`

### Special Cases
- [ ] Handle various separator characters (-, space, etc.) during validation
- [ ] Numbers with check digit 10 are never valid (algorithm constraint)
- [ ] Leading zeros are valid and must be preserved
- [ ] No embedded personal information (purely sequential)
- [ ] Old-style NHS numbers (9 digits) are not supported

### Test Coverage
- [ ] All validation rules with valid and invalid data
- [ ] Check digit algorithm verification
- [ ] Test cases where calculated check digit would be 10 (should be rejected)
- [ ] Test case where calculated check digit is 11 (should become 0)
- [ ] Both formatted and unformatted inputs
- [ ] Various separator characters (dash, space, period, etc.)
- [ ] Format and ToString methods
- [ ] CheckDigit property
- [ ] Equality and hash code
- [ ] JSON serialization round-trip
- [ ] Conversion operators
- [ ] Create method Result pattern
- [ ] Error detection: single digit transcription, transposition, etc.
- [ ] Leading zeros preservation
- [ ] Comprehensive check digit validation (all possible valid check digits 0-9)

### Documentation
- [ ] XML documentation for all public members
- [ ] README.md section with:
  - Structure explanation (10 digits with check digit)
  - Validation rules
  - Format examples (with various separators)
  - Modulus 11 check digit algorithm with weights
  - Special case: check digit 10 makes number invalid
  - Special case: check digit 11 becomes 0
  - Historical context (introduced 1996 with NHS Wide Clearing Service)
  - Purpose (patient identification in NHS)
  - No embedded personal information
  - Difference from old 9-digit format (not supported)
  - Example values with descriptions
  - NHS Digital and Wikipedia references
  - Note about sequential allocation (no intelligence)

### Performance
- [ ] Use `ReadOnlySpan<Char>` for validation and parsing
- [ ] Efficient check digit algorithm
- [ ] Single-pass validation where possible
- [ ] Minimal string allocations
- [ ] Separator normalization efficient

## Notes
- NHS Number = National Health Service Number
- Introduced in 1996 as part of NHS Wide Clearing Service
- Used to uniquely identify patients in the NHS in England, Wales, and Isle of Man
- Scotland uses CHI (Community Health Index) number instead (different format)
- Northern Ireland uses H&C number (different format)
- Format: 10 digits with modulus 11 check digit
- Commonly formatted as: NNN-NNN-NNNN or NNN NNN NNNN
- Check digit algorithm: weights 10, 9, 8, 7, 6, 5, 4, 3, 2
- Numbers where check digit calculates to 10 are invalid and discarded
- Numbers where check digit calculates to 11 have check digit 0
- No embedded personal information (sequential allocation)
- Replaced older 9-digit local NHS numbers
- Universal across NHS in England and Wales (Isle of Man uses same system)
- Leading zeros are significant and must be preserved
- See: https://en.wikipedia.org/wiki/NHS_number
- See: https://www.nhs.uk/using-the-nhs/about-the-nhs/what-is-an-nhs-number/
- See: https://www.datadictionary.nhs.uk/attributes/nhs_number.html
- See: https://digital.nhs.uk/services/demographics

## Technical Details
- Namespace: `KfAccountNumbers.Governmental.Europe`
- Files:
  - `src/KfAccountNumbers/Governmental/Europe/GbNhsNumber.cs`
  - `src/KfAccountNumbers/Governmental/Europe/GbNhsNumberValidationResult.cs`
  - `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/GbNhsNumberTests.cs`
- JSON converter: `GbNhsNumberJsonConverter`
- Target: .NET 10, C# 14.0
- Pattern similar to `CaSocialInsuranceNumber` and `UsSocialSecurityNumber` (format with check digit)
- ISO 3166-1 alpha-2 code: GB (Great Britain, commonly used for UK)

## Example Values
### Standard Format (Unformatted)
- 4505577104 - Valid NHS number (check digit 4)
- 9434765919 - Valid NHS number (check digit 9)
- 5301194917 - Valid NHS number (check digit 7)
- 9999999999 - May be used for testing/training (verify if valid check digit)

### Formatted with Dashes
- 450-557-7104 - Standard formatted NHS number
- 943-476-5919 - Formatted NHS number
- 530-119-4917 - Formatted NHS number

### Formatted with Spaces
- 450 557 7104 - Space-separated NHS number
- 943 476 5919 - Space-separated NHS number
- 530 119 4917 - Space-separated NHS number

### With Leading Zeros
- 0123456789 - Valid if check digit matches (preserve leading zero)
- 000-000-0008 - Valid if check digit 8 is correct

### Invalid Examples
- 123456789X - Non-digit character 'X'
- 123456789 - Too few digits (9 instead of 10)
- 12345678901 - Too many digits (11 instead of 10)
- 4505577105 - Invalid check digit (should be 4, not 5)
- 1234567890 - Invalid if check digit 0 doesn't match calculation
- XXXXXXXXXX - All non-digits
- 450-55-77104 - Incorrect separator positions
- 450-557-710 - Missing last digit

## Additional Considerations
### Check Digit Edge Cases
- [ ] When Sum mod 11 = 1, check digit = 11 - 1 = 10 (INVALID, reject number)
- [ ] When Sum mod 11 = 0, check digit = 11 - 0 = 11, which becomes 0
- [ ] Test comprehensive set covering all possible remainders (0-10)
- [ ] Document that check digit 10 means the number cannot exist

### Algorithm Example

Example: 450-557-7104 Digits:     4    5    0    5    5    7    7    1    0    [4] Weights:   10    9    8    7    6    5    4    3    2 Products:  40 + 45 +  0 + 35 + 30 + 35 + 28 +  3 +  0 = 216 216 mod 11 = 7 Check digit = 11 - 7 = 4 

### Validation Strategy
- [ ] Single-pass validation combining length, character, and check digit checks
- [ ] Early exit on invalid characters or length
- [ ] Check digit validation as final step
- [ ] Clear error messages for each validation failure type
- [ ] Document in comments that check digit 10 causes rejection