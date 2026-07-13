# S0017-IePpsn Business Object

## As a
Developer integrating Irish governmental identification numbers into my application

## I want
A strongly typed business object that represents an Irish Personal Public Service Number (PPSN)

## So that
I can validate, parse, and work with Irish social security and tax identification numbers in a type-safe manner with comprehensive validation

## Acceptance Criteria

### Structure and Validation
- [ ] The `IePpsn` type represents an Irish Personal Public Service Number (PPSN)
- [ ] The number is structured as NNNNNNNC or NNNNNNNCA where:
  - N = 7 digits (0-9)
  - C = check character (letter A-Z, excluding certain letters)
  - A = optional second letter (historically for married women, less common today)
- [ ] Constructor accepts string representation and throws `KfValidationException<IePpsnValidationResult>` if invalid
- [ ] Static `Validate` method returns `IePpsnValidationResult` enumeration value
- [ ] Static `Create` method uses Result pattern returning `CreateResult<IePpsn, IePpsnValidationResult>`

### Validation Rules
- [ ] Value may not be null, empty, or all whitespace
- [ ] Value must be either 8 characters (7 digits + 1 letter) or 9 characters (7 digits + 2 letters)
- [ ] May optionally include spaces for formatting (e.g., "1234567 W" or "1234567 WA")
- [ ] If spaces present, must be at position 7 (zero-based) or positions 7 and 8
- [ ] First 7 characters must be ASCII digits ('0'-'9')
- [ ] Check character (8th character) must be uppercase letter A-Z
- [ ] Optional second letter (9th character) must be uppercase letter A-Z
- [ ] Check character must match calculated value using weighted modulus 23 algorithm
- [ ] Case-insensitive for validation (convert to uppercase internally)

### Format Support
- [ ] Accept unformatted 8 characters: 1234567W
- [ ] Accept unformatted 9 characters: 1234567WA
- [ ] Accept formatted with space: 1234567 W or 1234567 WA
- [ ] Accept formatted with multiple spaces: 1234567  W (normalize to single space)
- [ ] `Format` method with optional mask parameter (default: "_______ _" or "_______ __" based on length)

### Properties
- [ ] `Value` property returns raw string (7 digits + letters, uppercase, no spaces)
- [ ] `Number` property returns the 7-digit numeric portion as string
- [ ] `CheckCharacter` property returns the check character (first letter)
- [ ] `Suffix` property returns optional second letter (null if not present)
- [ ] `HasSuffix` property returns bool indicating if second letter is present

### Check Character Algorithm
- [ ] Use weighted modulus 23 algorithm
- [ ] Weights applied right to left: 8, 7, 6, 5, 4, 3, 2 for the 7 digits
- [ ] Sum = (d1 × 8) + (d2 × 7) + (d3 × 6) + (d4 × 5) + (d5 × 4) + (d6 × 3) + (d7 × 2)
- [ ] Remainder = Sum mod 23
- [ ] If remainder = 0, check character = 'W'
- [ ] Otherwise, map remainder (1-22) to letters: ABCDEFGHIJKLMNOPQRSTUVWXYZ (excluding W)
- [ ] **Alternative mapping**: Use alphabet excluding W: A=1, B=2, ..., V=22, skip W, continue with X-Z
- [ ] **Note**: Verify exact algorithm from official Irish sources

### Operators and Methods
- [ ] Implicit conversion to string
- [ ] Explicit conversion from string
- [ ] `ToString` returns raw value (uppercase, no spaces)
- [ ] Proper equality implementation (case-insensitive, space-insensitive)
- [ ] JSON serialization/deserialization support via `IePpsnJsonConverter`

### Special Cases
- [ ] Handle lowercase input by converting to uppercase
- [ ] Normalize multiple spaces to single space during validation
- [ ] Second letter (suffix) is optional and primarily historical
- [ ] Leading zeros in the 7-digit number are valid and must be preserved
- [ ] Old RSI numbers (pre-1998) are not supported

### Test Coverage
- [ ] All validation rules with valid and invalid data
- [ ] Check character algorithm verification
- [ ] Test all possible check characters (comprehensive coverage)
- [ ] 8-character and 9-character formats
- [ ] Case-insensitive validation
- [ ] Both formatted and unformatted inputs
- [ ] Format and ToString methods
- [ ] HasSuffix and Suffix properties
- [ ] Equality and hash code (case-insensitive, space-insensitive)
- [ ] JSON serialization round-trip
- [ ] Conversion operators
- [ ] Create method Result pattern
- [ ] Error detection: single digit transcription, transposition, etc.
- [ ] Leading zeros preservation

### Documentation
- [ ] XML documentation for all public members
- [ ] README.md section with:
  - Structure explanation (8 vs 9 character formats)
  - Validation rules
  - Format examples
  - Check character algorithm with weights
  - Historical context (replaced RSI in 1998)
  - Purpose (social welfare, taxation, public services)
  - Second letter significance (historical usage)
  - Example values with descriptions
  - Wikipedia and official references (Revenue.ie, gov.ie)
  - Note about old RSI numbers not being supported

### Performance
- [ ] Use `ReadOnlySpan<Char>` for validation and parsing
- [ ] Efficient check character algorithm
- [ ] Single-pass validation where possible
- [ ] Minimal string allocations
- [ ] Case conversion only when necessary
- [ ] Space normalization efficient

## Notes
- PPSN = Personal Public Service Number
- Introduced in 1998 to replace old Revenue and Social Insurance (RSI) number
- Used for social welfare, taxation, healthcare, and other public services
- Format: 7 digits + 1 check character, or 7 digits + check character + suffix letter
- Check character calculated using weighted modulus 23 algorithm
- Second letter historically used for married women, less common in modern numbers
- All Irish residents, workers, and benefit claimants receive a PPSN
- Numbers are unique and never reused
- Case-insensitive but typically displayed in uppercase
- May be displayed with or without space separator
- Old RSI numbers had different format and are not supported by this implementation
- See: https://en.wikipedia.org/wiki/Personal_Public_Service_Number
- See: https://www.gov.ie/en/service/12e6de-get-a-personal-public-service-number-ppsn/
- See: https://www.revenue.ie/en/personal-tax-credits-reliefs-and-exemptions/ppsn/index.aspx

## Technical Details
- Namespace: `KfAccountNumbers.Governmental.Europe`
- Files:
  - `src/KfAccountNumbers/Governmental/Europe/IePpsn.cs`
  - `src/KfAccountNumbers/Governmental/Europe/IePpsnValidationResult.cs`
  - `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/IePpsnTests.cs`
- JSON converter: `IePpsnJsonConverter`
- Target: .NET 10, C# 14.0
- Pattern similar to `NlBurgerservicenummer` (simple format with check digit)

## Example Values
### Standard 8-Character Format
- 1234567W - PPSN with check character W (sum divisible by 23)
- 1234567T - Valid PPSN example
- 0000000W - Valid PPSN with leading zeros
- 6335435E - Real-world example format

### 9-Character Format with Suffix
- 1234567WA - PPSN with check character and suffix
- 6335435EA - Example with suffix letter

### Formatted Examples
- 1234567 W - Standard format with space
- 1234567 WA - Format with suffix and space
- 0000000 W - Leading zeros with space

### Invalid Examples
- 123456W - Too few digits (only 6)
- 12345678W - Too many digits (8 instead of 7)
- 1234567X - Invalid check character for this number
- ABCDEFGW - Letters where digits expected
- 1234567w - Lowercase (should be rejected or normalized to uppercase)
- 12345671 - Digit where check character expected