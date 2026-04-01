# S0018-GbNationalInsuranceNumber Business Object

## As a
Developer integrating UK governmental identification numbers into my application

## I want
A strongly typed business object that represents a UK National Insurance Number (NINO)

## So that
I can validate, parse, and work with UK National Insurance numbers in a type-safe manner with comprehensive validation

## Acceptance Criteria

### Structure and Validation
- [ ] The `GbNino` type represents a UK National Insurance Number (NINO)
- [ ] The number is structured as PPNNNNNS where:
  - P = prefix letters (2 letters, specific restrictions apply)
  - N = 6 digits (0-9)
  - S = suffix letter (typically A, B, C, or D, but other letters may be valid)
- [ ] Total length is 9 characters (unformatted) or 11 characters (formatted with spaces)
- [ ] Constructor accepts string representation and throws `KfValidationException<GbNinoValidationResult>` if invalid
- [ ] Static `Validate` method returns `GbNinoValidationResult` enumeration value
- [ ] Static `Create` method uses Result pattern returning `CreateResult<GbNino, GbNinoValidationResult>`

### Validation Rules
- [ ] Value may not be null, empty, or all whitespace
- [ ] Value must be either 9 characters (unformatted) or 11 characters (formatted with spaces)
- [ ] First letter must not be D, F, I, Q, U, or V
- [ ] Second letter must not be D, F, I, O, Q, U, or V
- [ ] First two letters must not be: BG, GB, NK, KN, TN, NT, or ZZ
- [ ] First two letters must not be a valid temporary prefix (must verify against known administrative codes)
- [ ] Characters 3-8 (zero-based positions 2-7) must be ASCII digits ('0'-'9')
- [ ] Ninth character (suffix) must be an uppercase letter A-Z
- [ ] Suffix is typically A, B, C, or D, but other letters may be administratively valid
- [ ] If spaces present, must be at positions 2, 5, and 8 (zero-based) forming pattern: PP NN NN NN S
- [ ] Case-insensitive for validation (convert to uppercase internally)

### Format Support
- [ ] Accept unformatted: AB123456C
- [ ] Accept formatted with spaces: AB 12 34 56 C
- [ ] Accept lowercase input (normalize to uppercase)
- [ ] Accept mixed case (normalize to uppercase)
- [ ] `Format` method with optional mask parameter (default: "__ __ __ __ _")

### Properties
- [ ] `Value` property returns raw 9-character string (uppercase, no spaces)
- [ ] `Prefix` property returns the first two letters as string
- [ ] `Number` property returns the 6-digit numeric portion as string
- [ ] `Suffix` property returns the suffix letter as char

### No Check Digit or Intelligence
- [ ] NINO does not contain check digit
- [ ] NINO does not encode date of birth or other personal information
- [ ] Validation is purely format-based (letter restrictions, digit positions)
- [ ] No algorithmic validation beyond format rules

### Operators and Methods
- [ ] Implicit conversion to string
- [ ] Explicit conversion from string
- [ ] `ToString` returns raw value (uppercase, no spaces)
- [ ] Proper equality implementation (case-insensitive, space-insensitive)
- [ ] JSON serialization/deserialization support via `GbNinoJsonConverter`

### Special Cases
- [ ] Handle lowercase input by converting to uppercase
- [ ] Normalize spacing during validation
- [ ] Temporary NINOs may have different prefix patterns (document but may not validate)
- [ ] Administrative codes TN are reserved and invalid for individuals
- [ ] Leading zeros in the 6-digit number are valid and must be preserved

### Test Coverage
- [ ] All validation rules with valid and invalid data
- [ ] First letter restrictions (D, F, I, Q, U, V are invalid)
- [ ] Second letter restrictions (D, F, I, O, Q, U, V are invalid)
- [ ] Invalid prefix combinations (BG, GB, NK, KN, TN, NT, ZZ)
- [ ] All valid suffix letters (A, B, C, D at minimum)
- [ ] Case-insensitive validation
- [ ] Both formatted and unformatted inputs
- [ ] Format and ToString methods
- [ ] Space normalization
- [ ] Equality and hash code (case-insensitive, space-insensitive)
- [ ] JSON serialization round-trip
- [ ] Conversion operators
- [ ] Create method Result pattern
- [ ] Leading zeros preservation
- [ ] Comprehensive prefix letter combinations

### Documentation
- [ ] XML documentation for all public members
- [ ] README.md section with:
  - Structure explanation (PP-NNNNNN-S format)
  - Validation rules (letter restrictions, invalid prefixes)
  - Format examples (with and without spaces)
  - No check digit or embedded intelligence
  - Purpose (National Insurance contributions, taxation, benefits)
  - Historical context (introduced 1948 with creation of welfare state)
  - Prefix letter restrictions and rationale
  - Invalid prefix combinations and reasons
  - Suffix letter meanings (if any, or note administrative use)
  - Temporary NINO format (if different)
  - Example values with descriptions
  - Gov.uk and Wikipedia references
  - Note about no personal information encoded

### Performance
- [ ] Use `ReadOnlySpan<Char>` for validation and parsing
- [ ] Efficient prefix validation (hash set or switch expression)
- [ ] Single-pass validation where possible
- [ ] Minimal string allocations
- [ ] Case conversion only when necessary
- [ ] Space normalization efficient

## Notes
- NINO = National Insurance Number
- Introduced in 1948 with the creation of the UK welfare state
- Used for National Insurance contributions, taxation, and benefits
- Format: 2 letters + 6 digits + 1 suffix letter
- No embedded personal information (unlike some other national IDs)
- No check digit algorithm
- First letter restrictions: Cannot be D, F, I, Q, U, or V
- Second letter restrictions: Cannot be D, F, I, O, Q, U, or V
- Invalid prefixes: BG, GB, NK, KN, TN, NT, ZZ (administrative or confusion reasons)
- Suffix typically A, B, C, or D (originally indicated contribution category)
- Formatted with spaces: AB 12 34 56 C
- Case-insensitive but typically displayed in uppercase
- Never contains personal information like date of birth
- Unique per individual and not reused after death
- Temporary NINOs may exist with different formats (TN prefix) but are not standard
- See: https://en.wikipedia.org/wiki/National_Insurance_number
- See: https://www.gov.uk/national-insurance/your-national-insurance-number
- See: https://www.gov.uk/hmrc-internal-manuals/national-insurance-manual/nim39110

## Technical Details
- Namespace: `KfAccountNumbers.Governmental.Europe`
- Files:
  - `src/KfAccountNumbers/Governmental/Europe/GbNino.cs`
  - `src/KfAccountNumbers/Governmental/Europe/GbNinoValidationResult.cs`
  - `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/GbNinoTests.cs`
- JSON converter: `GbNinoJsonConverter`
- Target: .NET 10, C# 14.0
- Pattern similar to `NlBurgerservicenummer` (simple format without check digit or embedded data)
- ISO 3166-1 alpha-2 code: GB (Great Britain, commonly used for UK)

## Example Values
### Standard Format (Unformatted)
- AB123456C - Standard NINO
- QQ123456C - Valid NINO (QQ is valid prefix)
- AA000000A - NINO with leading zeros (valid)
- ZY999999D - High number NINO (valid)

### Formatted with Spaces
- AB 12 34 56 C - Standard formatted NINO
- QQ 12 34 56 C - Formatted NINO
- AA 00 00 00 A - Formatted with leading zeros
- ZY 99 99 99 D - Formatted high number

### Mixed Case (Should Normalize)
- ab123456c - Lowercase (normalize to AB123456C)
- Ab 12 34 56 C - Mixed case (normalize to AB 12 34 56 C)

### Invalid Examples
- DB123456C - Invalid first letter D
- AD123456C - Invalid second letter D
- AI123456C - Invalid second letter I
- AO123456C - Invalid second letter O
- BG123456C - Invalid prefix combination BG
- GB123456C - Invalid prefix combination GB
- NK123456C - Invalid prefix combination NK
- KN123456C - Invalid prefix combination KN
- TN123456C - Invalid prefix combination TN (administrative use)
- NT123456C - Invalid prefix combination NT
- ZZ123456C - Invalid prefix combination ZZ
- AB12345C - Too few digits (5 instead of 6)
- AB1234567C - Too many digits (7 instead of 6)
- ABCDEFGHC - Letters where digits expected
- AB123456 - Missing suffix letter
- AB1234561 - Digit where suffix letter expected

## Additional Considerations
### Suffix Letter Meanings (Historical)
- A = Standard contributions
- B = Married woman paying reduced contributions (historical)
- C = Deferred contributions or other special cases
- D = Special cases or administrative use
- Other letters may exist for specific administrative purposes
- Modern NINOs may not strictly follow these categories
- Validation should accept A-Z for suffix to accommodate administrative variations

### Validation Strategy
- [ ] Use HashSet<string> for invalid prefix combinations (BG, GB, NK, KN, TN, NT, ZZ)
- [ ] Use switch expression or character range checks for individual letter restrictions
- [ ] Document rationale for each restriction in code comments
- [ ] Consider future-proofing for potential format changes