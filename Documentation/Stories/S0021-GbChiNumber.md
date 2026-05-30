# S0021-GbChiNumber Business Object

## As a
Developer integrating Scottish National Health Service patient identification numbers into my application

## I want
A strongly typed business object that represents a Scottish CHI Number (Community Health Index Number)

## So that
I can validate, parse, and work with CHI numbers in a type-safe manner with comprehensive validation including date of birth extraction and check digit verification

## Acceptance Criteria

### Structure and Validation
- [ ] The `GbChiNumber` type represents a Scottish CHI Number (Community Health Index Number)
- [ ] The number is a 10-digit identifier in the format DDMMYYNNNNC where:
  - **DDMMYY**: Date of birth (day, month, year - 2 digits each)
  - **NNNN**: Four-digit sequence number (unique identifier for individuals born on the same date). Odd trailing digits indicate gender = male and even trailing digits indicate gender = female.
  - **C**: Check digit calculated using modulus 11 algorithm
- [ ] Total length is 10 digits (unformatted) or 12 characters (formatted with separator)
- [ ] Constructor accepts string representation and throws `UKfValidationException<GbChiNumber.ValidationError>` if invalid
- [ ] Static `Validate` method returns `GbChiNumber.ValidationResult` union value
- [ ] Static `Create` method uses Result pattern returning `CreateResult<GbChiNumber, GbChiNumber.ValidationError>`
- [ ] `GbChiNumber.ValidationError` is a union of the following types: `EmptyValue`, `InvalidLength`, `InvalidCharacter`, `InvalidCheckSum`, `InvalidSeparator`, `InvalidDateOfBirth` and `GbUniquePatientIdentifierInvalidRange`
- [ ] `GbChiNumber.ValidationResult` extends `GbChiNumber.ValidationError` with the type `ValidValue` to indicate a sucessful validation.

### Validation Rules
- [ ] Value may not be null, empty, or all whitespace
- [ ] Value must be either 10 digits or 11 characters with a dash separator in position 6
- [ ] Characters 0-1 (DD) must be digits 01-31
- [ ] Characters 2-3 (MM) must be digits 01-12
- [ ] Characters 4-5 (YY) must be digits 00-99
- [ ] If separator characers are present, must be at positions 3 and 7 (zero-based) forming pattern: NNN NNN NNNN
- [ ] If separator characters are present, they must not be ASCII digits ('0'-'9') and both separators must be the same character
- [ ] Characters 7-9 (NNNN first 3 digits) must be digits 000-999
- [ ] Character 10 (NNNN last digit/check digit C) must be a digit 0-9
- [ ] The date portion (DDMMYY) must represent a valid calendar date
- [ ] Date validation must account for leap years
- [ ] Future dates are allowed even though CHI numbers are assigned at birth/registration. This eliminates any requirement for GbChiNumber to be aware of the current date/time.
- [ ] Check digit (10th digit) must be valid according to modulus 11 algorithm
- [ ] The first nine digits must be in the allowed range: 010 000 000 to 311 299 999
- [ ] **Test numbers are NOT supported**: Numbers that would have invalid date components (e.g., day 90, month 00) are rejected

### Check Digit Algorithm (Modulus 11)
GbChisNumber should use the CheckDigits.Net Modulus11Decimal algorithm for validation of
the check digit. For reference, the check digit is calculated using the modulus 11 algorithm:
1. Multiply each of the first 9 digits by a weight (10 - position), where position is 0-based:
   - Digit 1 (position 0) × 10
   - Digit 2 (position 1) × 9
   - Digit 3 (position 2) × 8
   - Digit 4 (position 3) × 7
   - Digit 5 (position 4) × 6
   - Digit 6 (position 5) × 5
   - Digit 7 (position 6) × 4
   - Digit 8 (position 7) × 3
   - Digit 9 (position 8) × 2
2. Sum all the products
3. Calculate remainder: sum modulo 11
4. Subtract remainder from 11: check digit = 11 - remainder
5. If check digit is 11, use 0 instead
6. If check digit is 10, the number is **invalid** (no CHI number should have check digit 10)
7. The calculated check digit must match the 10th digit

### Format Support
- [ ] Accept unformatted: 0101901234
- [ ] Accept formatted with separator characters: 010 190 1234
- [ ] `Format` method with optional mask parameter (default: "___ ___ ____")

### Properties
- [ ] `Value` property returns the raw CHI number string as stored (10 digits or with dash if originally provided)
- [ ] `Gender` property returns a GbChiNumber.GenderUnion composed of types Male and Female. The ninth digit determines the gender with odd values = Male and even value = Female.

### Date of Birth Intelligence
- [ ] CHI number encodes actual date of birth in first 6 digits (DDMMYY)
- [ ] Constructor century determination logic uses the default century cutoff of 50
  - Years 00-49: Assumed to be 2000-2049 (current century)
  - Years 50-99: Assumed to be 1950-1999 (previous century)
- [ ] Date must be valid calendar date (handle leap years correctly)
- [ ] **No test number range**: Unlike NHS numbers, CHI does not have test ranges because date component must be valid

### Operators and Methods
- [ ] Implicit conversion to string
- [ ] Explicit conversion from string
- [ ] `ToString` returns the value as stored (with or without dash)
- [ ] Proper equality implementation (separator-insensitive: with/without dash are equal)
- [ ] JSON serialization/deserialization support via `GbChiNumberJsonConverter`
- [ ] `GetDateOfBirth` method returns a `DateOnly` value. Accepts an option `CenturyCutoff` object that is used to determine the exact century. The default century cutoff is 50.

### Special Cases
- [ ] Normalize separator characters during validation
- [ ] Reject numbers where calculated check digit would be 10
- [ ] Handle leap year validation correctly for all centuries
- [ ] Leading zeros in sequence number and date components are significant

### Test Coverage
- [ ] Standard number range (010 000 000 to 311 299 999)
- [ ] Valid CHI numbers with correct check digits from 1900s
- [ ] Valid CHI numbers with correct check digits from 2000s
- [ ] Invalid check digits
- [ ] Check digit edge case: calculated digit would be 11 (should use 0)
- [ ] Check digit edge case: calculated digit would be 10 (should be invalid number)
- [ ] Valid dates in different centuries (1950-1999, 2000-2049)
- [ ] Leap year dates (Feb 29 in leap years)
- [ ] Invalid leap year dates (Feb 29 in non-leap years)
- [ ] Invalid dates (day 32, month 13, day 00, month 00)
- [ ] Century boundary dates (1999/2000, 1949/1950)
- [ ] Both formatted (with dash) and unformatted inputs
- [ ] Format and ToString methods
- [ ] Separator character normalization
- [ ] Equality and hash code (separator-insensitive)
- [ ] JSON serialization round-trip
- [ ] Conversion operators
- [ ] Create method Result pattern
- [ ] Leading zeros preservation
- [ ] Null, empty, and whitespace inputs
- [ ] Invalid lengths
- [ ] Non-digit characters (except separator characters in correct position)
- [ ] Invalid separator position or character
- [ ] GetDateOfBirth extraction for various dates
- [ ] Gender property, odd and even digits

### Documentation
- [ ] XML documentation for all public members
- [ ] README.md section with:
  - Structure explanation (DDMMYYNNNNC format)
  - Date of birth encoding (first 6 digits)
  - Century determination logic (threshold approach)
  - Valid number range
  - Validation rules
  - Format examples (with and without separator characters)
  - Purpose (patient identification in Scotland)
  - Historical context (established with NHS Scotland)
  - Relationship to NHS Number (England/Wales), H&C Number (Northern Ireland)
  - Geographic coverage (Scotland only)
  - Date of birth intelligence encoded in number
  - Gender encoded in number
  - No test number ranges (unlike NHS Number)
  - Example values with descriptions
  - NHS Scotland and relevant references
  - Note about century determination threshold

### Performance
- [ ] Use `ReadOnlySpan<Char>` for validation and parsing
- [ ] Efficient check digit calculation
- [ ] Efficient date validation
- [ ] Single-pass validation where possible
- [ ] Minimal string allocations
- [ ] separator character normalization efficient

## Notes
- CHI Number = Community Health Index Number
- Used for patient identification in Scotland only
- Part of NHS Scotland (separate from NHS England/Wales)
- England, Wales, Isle of Man use NHS Number
- Northern Ireland uses H&C (Health and Care) Number
- Scotland uses CHI Number (this system)
- Format: DDMMYYNNNNC (date of birth + sequence + check digit)
- Date component encodes actual date of birth (unlike NHS Number)
- Check digit uses modulus 11 algorithm with weights [10,9,8,7,6,5,4,3,2]
- Check digit of 10 makes the CHI number invalid
- **No test number ranges**: Unlike NHS Number, CHI does not support test ranges because date component must be valid
  - NHS Number test range (000-009 million) would translate to invalid dates (day 00-09, month 00)
  - This is a key difference from NHS Number
- Century determination uses threshold approach (years 00-24 = 2000s, 25-99 = 1900s)
- Threshold may need periodic adjustment as current year advances
- Future dates are invalid (CHI assigned at birth/registration)
- Unique per patient within Scotland
- Leading zeros are significant and must be preserved
- Similar in structure to Danish personnummer (DkPersonnummer) with date component
- See: https://www.nhs.scot/ (NHS Scotland)
- See: https://www.isdscotland.org/Products-and-Services/eDRIS/CHI-Number/
- See: https://www.ndc.scot.nhs.uk/Dictionary-A-Z/Definitions/index.asp?ID=128
- See: https://digital.nhs.uk/services/demographics (UK-wide context)
- See: https://webarchive.nationalarchives.gov.uk/ukgwa/20231221081503/https://digital.nhs.uk/about-nhs-digital/contact-us/freedom-of-information/freedom-of-information-disclosure-log/december-2022/nic-690159-k8h4z

## Technical Details
- Namespace: `KfAccountNumbers.Governmental.Europe`
- Files:
  - `src/KfAccountNumbers/Governmental/Europe/GbChiNumber.cs`
  - `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/GbChiNumberTests.cs`
- JSON converter: `GbChiNumberJsonConverter`
- Target: .NET 11, C# 15.0
- External library: CheckDigits.Net 3.1.0
- Pattern similar to `DkPersonnummer` (has date component) and `SePersonnummer` (Nordic structure)
- Also similar to `GbNhsNumber` for check digit algorithm
- ISO 3166-1 alpha-2 code: GB (Great Britain, commonly used for UK including Scotland)

## Example Values

### Standard CHI Numbers (Valid)
- 3112999991 - Born December 31, 1999, gender = male
- 311 299 9991 - Same, formatted with spaces
- 3112992345 - Born December 31, 1999, gender = female
- 311 299 2345 - Same, formatted
- 0101000123 - Born January 1, 2000, gender = female
- 010 100 0123 - Same, formatted
- 1506245678 - Born June 15, 2024, gender = male
- 150 624 5678 - Same, formatted

### Edge Case: Check Digit Would Be 11 (Use 0)
- 0101901230 - Check digit calculation yields 11, so use 0
- 010190-1230 - Same, formatted

### Leap Year Dates (Valid)
- 2902001234 - Born February 29, 2000 (leap year)
- 290200-1234 - Same, formatted
- 2902961234 - Born February 29, 1996 (leap year)
- 290296-1234 - Same, formatted

### Century Boundary Examples
- 3112991234 - Born December 31, 1999 (20th century)
- 311299-1234 - Same, formatted
- 0101001234 - Born January 1, 2000 (21st century)
- 010100-1234 - Same, formatted
- 0101241234 - Born January 1, 2024 (21st century)
- 010124-1234 - Same, formatted
- 0101251234 - Born January 1, 1925 (20th century, below threshold)
- 010125-1234 - Same, formatted

### Invalid Examples - Date Validation
- 0001901234 - Invalid day (00)
- 3201901234 - Invalid day (32)
- 0100901234 - Invalid month (00)
- 0113901234 - Invalid month (13)
- 2902901234 - Invalid date (Feb 29 in non-leap year 1990)
- 3102901234 - Invalid date (Feb 31 doesn't exist)
- 3111901234 - Invalid date (Nov 31 doesn't exist)

### Invalid Examples - Check Digit
- 0101901235 - Invalid check digit (wrong digit)
- 010190-1235 - Invalid check digit (formatted)
- (Check digit 10 example) - Invalid because check digit cannot be 10

### Invalid Examples - Format
- 010190123 - Too short (9 digits instead of 10)
- 01019012345 - Too long (11 digits instead of 10)
- 010190*1234 - Invalid separator (not a dash)
- 01-0190-1234 - Invalid separator position
- 0101-901234 - Invalid separator position
- 010A901234 - Non-digit character in date
- 010190123A - Non-digit character in sequence/check digit

### Invalid Examples - Future Dates
- 0101501234 - Future date (year 2050, assuming current year < 2050)
- 010150-1234 - Same, formatted

### Invalid Examples - No Test Number Support
- Unlike NHS Number, CHI does NOT support test numbers in 900 range because:
  - Day 90, month 00 would be invalid date
  - Day 00, month 90 would be invalid date
  - Any combination creating invalid date is rejected
- 9001001234 - Invalid (day 90, month 01 = invalid day)
- 0090001234 - Invalid (day 00, month 90 = invalid month)
- 0009001234 - Invalid (day 00, month 09 = invalid day)

## Implementation Notes
1. Place in namespace: `KfAccountNumbers.Governmental.Europe`
2. Use ISO country code: `Gb` (Great Britain/UK)
3. Follow the same patterns as `DkPersonnummer` and `SePersonnummer` (have date components)
4. Create corresponding enum: `GbChiNumberValidationResult`
5. Implement full unit test coverage similar to other European identifiers with date components
6. Support both formatted and unformatted input
7. Store value in the normalized format
8. The check digit validation is mandatory - no valid CHI number has invalid check digit
9. Date validation is mandatory - must be valid calendar date
10. No Future date validation - allow dates in the future
11. Century determination uses threshold approach (may need documentation about future maintenance)
12. Preserve leading zeros - they are significant in date components
13. Document that this is for Scotland only (not England, Wales, Northern Ireland, Isle of Man)
14. Comprehensive test coverage for date validation including leap years and century boundaries
15. **No test number support** - explicitly reject invalid date components

## Special Considerations

### Date of Birth Component
- **Embedded Intelligence**: Unlike NHS Number and H&C Number, CHI encodes actual date of birth
  - First 6 digits are DDMMYY format
  - Must be valid calendar date
  - Must not be in future
  - This is similar to Nordic personnummer systems (Swedish, Norwegian, Danish, etc.)
- **Century Determination**: Uses threshold approach
  - Years 00-49: Interpreted as 2000-2049
  - Years 50-99: Interpreted as 1950-1999
  - This threshold will need periodic review/adjustment
  - Document clearly that threshold is based on current common practice
  - Consider making threshold configurable or documenting maintenance needs
- **Leap Year Handling**: Must correctly validate February 29
  - Valid in leap years (2000, 2004, 2020, 2024, etc.)
  - Invalid in non-leap years (1900, 2001, 2100, etc.)
  - Comprehensive leap year logic required

### No Test Number Support
- **Key Difference from NHS Number**: CHI does NOT support test number ranges
  - NHS Number reserves 900-999 million range for testing
  - This would translate to invalid dates in CHI (day 00-09, month 00, etc.)
  - CHI numbers must always have valid date components
  - This is a design consequence of embedding date of birth
- **Rationale**: 
  - Date validation takes precedence
  - Test scenarios must use valid (possibly synthetic) dates
  - No special test ranges can be carved out without violating date rules
- **Documentation**: Clearly document this difference from NHS Number to avoid confusion

### Check Digit Algorithm
- **Same as NHS Number**: Uses modulus 11 with same edge cases
  - Check digit 10 = invalid number
  - Check digit 11 = use 0 instead
  - Can potentially share check digit validation logic with GbNhsNumber and GbHcNumber
- **Weights**: [10,9,8,7,6,5,4,3,2] applied to first 9 digits

### Geographic Scope
- **Scotland Only**: Only valid for Scotland
  - England, Wales, Isle of Man use NHS Number
  - Northern Ireland uses H&C Number  
  - Scotland uses CHI Number (this system)
  - Document this clearly to avoid confusion
- **NHS Scotland**: Part of NHS Scotland infrastructure
  - Separate health service from NHS England
  - Different IT systems and databases
  - CHI is the primary patient identifier

## References
- [NHS Scotland](https://www.nhs.scot/)
- [ISD Scotland - CHI Number](https://www.isdscotland.org/Products-and-Services/eDRIS/CHI-Number/)
- [NHS Scotland Data Dictionary - CHI Number](https://www.ndc.scot.nhs.uk/Dictionary-A-Z/Definitions/index.asp?ID=128)
- [NHS Digital - Demographics Service](https://digital.nhs.uk/services/demographics) (UK-wide context)
- [Wikipedia - NHS number](https://en.wikipedia.org/wiki/NHS_number) (includes mention of CHI)
- [Scotland's National Health Service](https://www.gov.scot/policies/health-and-social-care/)

## Implementation Phases
1. **Phase 1**: Basic structure, length validation, digit validation, separator handling
2. **Phase 2**: Date parsing and validation (without century determination)
3. **Phase 3**: Century determination logic (threshold-based)
4. **Phase 4**: Future date validation (time-dependent)
5. **Phase 5**: Leap year validation (comprehensive)
6. **Phase 6**: Check digit algorithm implementation (can share with GbNhsNumber)
7. **Phase 7**: Properties (`DateOfBirth`, `SequenceNumber`, `CheckDigit`, `UnformattedValue`)
8. **Phase 8**: Format support, conversion operators, equality
9. **Phase 9**: JSON serialization support
10. **Phase 10**: Full test coverage including all date edge cases
11. **Phase 11**: Documentation, README updates, code review

## Relationship to Other UK Identifiers
This CHI Number completes the comprehensive coverage of UK governmental identification numbers:
- **GbNino** (S0018): UK National Insurance Number - for taxation and benefits across UK
- **GbNhsNumber** (S0019): NHS Number - for England, Wales, Isle of Man healthcare
- **GbHcNumber** (S0020): H&C Number - for Northern Ireland healthcare
- **GbChiNumber** (S0021): CHI Number - for Scotland healthcare (this story)

Key differences:
- **GbNino**: No check digit, no embedded intelligence, format-based validation only
- **GbNhsNumber**: Check digit, no embedded intelligence, test ranges supported
- **GbHcNumber**: Check digit, no embedded intelligence, prefix-based classification
- **GbChiNumber**: Check digit, **embedded date of birth**, no test ranges (this story)

Together these provide complete coverage of UK governmental identification numbers.
