# S0014-BeNationalRegisterNumber Business Object

## Overview
Create a strongly typed business object to represent a Belgian National Register Number (Rijksregisternummer in Dutch, Numéro de registre national in French) issued by the Belgian government. The National Register Number is a unique identifier assigned to all persons registered in Belgium's National Register (Rijksregister/Registre national), including Belgian citizens and foreign residents.

## Background
A Belgian National Register Number is an 11-digit identifier in the format YY.MM.DD-XXX.CC, where:
- **YY.MM.DD**: Date of birth (year, month, day - 2 digits each)
- **XXX**: Serial number (001-998 for births before 2000, with specific encoding for post-2000 births)
- **CC**: Check digits (2 digits) calculated using modulo 97 algorithm

The serial number serves multiple purposes:
1. Distinguishes between individuals born on the same day
2. Encodes gender: odd = male, even = female (with special handling for post-2000 births)
3. For persons born from 2000 onwards, an implicit '2' is prepended during validation

The format uses dots and a dash as separators: YY.MM.DD-XXX.CC

## User Story
**As a** developer building an application that processes Belgian identification numbers  
**I want** a strongly typed business object that validates and parses National Register Numbers  
**So that** I can ensure data integrity and easily access components including date of birth and gender

## Acceptance Criteria

### 1. Constructor
- Accepts a string representation of a National Register Number
- Accepts either 11 digits (YYMMDDXXXCC) or formatted with separators (YY.MM.DD-XXX.CC)
- Alternative formats: YY.MM.DD-XXXCC, YYMMDD-XXX.CC, YYMMDD-XXXCC (with or without internal dots/dashes)
- Throws `KfValidationException<BeNationalRegisterNumberValidationResult>` for invalid values
- Constructor is case-insensitive (not applicable for digits, but consistent with other types)

### 2. Validation Rules
A valid National Register Number must meet all of the following rules:
- Must not be null, empty, or all whitespace
- Must contain exactly 11 digits (after removing separator characters)
- Characters 0-1 (YY) must be digits 00-99
- Characters 2-3 (MM) must be digits 01-12
- Characters 4-5 (DD) must be digits 01-31
- Characters 6-8 (XXX) must be digits 001-998 (000 and 999 are not used)
- Characters 9-10 (CC) must be digits 01-97
- The date portion (YY.MM.DD) must represent a valid calendar date
- The check digits must be correctly calculated using modulo 97 algorithm
- If separators are used, they must be dots (.) or dashes (-) in appropriate positions
- Future dates are specifically **NOT** tested for to avoid issues requiring the class to be aware of current time

### 3. Properties
- `Value` (string): The raw National Register Number string as stored (digits only)
- `DateOfBirth` (DateOnly): Returns the person's date of birth derived from the number
- `Gender` (BinaryGender): Returns Male or Female based on serial number parity
- `SerialNumber` (string): Returns the three-digit serial number
- `CheckDigits` (string): Returns the two-digit check value

### 4. Static Methods
- `Validate(string? nationalRegisterNumber)`: Returns `BeNationalRegisterNumberValidationResult` enum indicating validation status
- `Create(string? nationalRegisterNumber)`: Returns `CreateResult<BeNationalRegisterNumber, BeNationalRegisterNumberValidationResult>`

### 5. Conversion Operators
- Implicit conversion from `BeNationalRegisterNumber` to `string`
- Explicit conversion from `string` to `BeNationalRegisterNumber`

### 6. Standard Object Methods
- `ToString()`: Returns the National Register Number value (digits only)
- `Equals()`: Value-based equality comparison
- `GetHashCode()`: Consistent with equality

### 7. Formatting
- `Format()` method that returns formatted version with separators (YY.MM.DD-XXX.CC)
- Support both formatted and unformatted input in constructor/validation

### 8. JSON Serialization
- Custom JSON converter for seamless serialization/deserialization
- Serializes as a simple string value (digits only)
- Handles null values gracefully

## Check Digits Algorithm (Modulo 97)
The check digits are calculated using the modulo 97 algorithm:

**For persons born before 2000:**
1. Take YYMMDDXXX as a 9-digit number
2. Calculate: check_digits = 97 - (YYMMDDXXX mod 97)
3. If result is less than 10, prepend '0' to make it 2 digits

**For persons born from 2000 onwards:**
1. Prepend '2' to YYMMDDXXX to form 2YYMMDDXXX (10-digit number)
2. Calculate: check_digits = 97 - (2YYMMDDXXX mod 97)
3. If result is less than 10, prepend '0' to make it 2 digits

**Validation Logic:**
- First attempt validation assuming birth before 2000 (9-digit calculation)
- If that fails, attempt validation assuming birth from 2000 onwards (10-digit calculation)
- If YY = 00 and YY = 23 (or current year - 2000), try 2000+ interpretation
- If YY = 24 (or some threshold), assume 1900s interpretation

## Century Determination Logic
Determining the century (1900s vs 2000s) requires checking both interpretations:
1. Calculate check digits for 1900s interpretation (9 digits)
2. Calculate check digits for 2000s interpretation (10 digits with '2' prefix)
3. If the provided check digits match the 2000s calculation AND YY could reasonably be post-2000, use 2000s
4. Otherwise, use 1900s interpretation
5. Consider validation ambiguity: some numbers could be valid in both centuries

## Gender Encoding
Gender is determined by the serial number parity:
- **Odd serial number (XXX)**: Male (001, 003, 005, ..., 997)
- **Even serial number (XXX)**: Female (002, 004, 006, ..., 998)
- Note: The year 2000+ adjustment does not affect gender determination

## Example Values
Valid National Register Numbers (pre-2000 births):
- `85.07.30-033.54` - Born July 30, 1985, male (odd serial 033)
- `850730-033.54` - Same, alternative format
- `85073003354` - Same, no separators
- `92.12.31-002.61` - Born December 31, 1992, female (even serial 002)

Valid National Register Numbers (post-2000 births):
- `00.01.01-001.62` - Born January 1, 2000, male (check digits calculated with '2' prefix)
- `15.05.20-123.45` - Born May 20, 2015, male (odd serial 123)
- `23.06.15-456.78` - Born June 15, 2023, female (even serial 456)

Invalid examples:
- `85.00.30-033.54` - Invalid month (00)
- `85.13.30-033.54` - Invalid month (13)
- `85.07.00-033.54` - Invalid day (00)
- `85.07.32-033.54` - Invalid day (32)
- `85.07.30-000.54` - Invalid serial (000 not used)
- `85.07.30-999.54` - Invalid serial (999 not used)
- `85.07.30-033.97` - Invalid check digits (exceeds 96)
- `85.07.30-033.53` - Invalid check digits (incorrect calculation)
- `85.02.29-033.54` - Invalid date (Feb 29 in non-leap year 1985)
- `850730033` - Invalid length (too short)
- `8507300335412` - Invalid length (too long)

## Implementation Notes
1. Place in namespace: `KfAccountNumbers.Governmental.Europe`
2. Use ISO country code: `Be` (Belgium)
3. Follow the same patterns as `SePersonnummer`, `DkPersonnummer`, and `FiHenkilotunnus`
4. Create corresponding enum: `BeNationalRegisterNumberValidationResult`
5. Implement full unit test coverage similar to `SePersonnummerTests`
6. Support both formatted (with separators) and unformatted input
7. Store value without separators (digits only)
8. Implement `Format()` method to return YY.MM.DD-XXX.CC format
9. Handle leap year validation correctly for both 1900s and 2000s
10. Century determination is complex due to year 2000 transition - implement carefully
11. Check digit calculation differs for pre-2000 vs post-2000 births
12. Serial numbers 000 and 999 are reserved and not issued

## Special Considerations
- **Year 2000 Transition**: The validation algorithm changes for persons born from 2000 onwards
  - Pre-2000: 9-digit number for modulo 97 calculation
  - Post-2000: 10-digit number (with '2' prefix) for modulo 97 calculation
  - This creates potential ambiguity for interpreting the century
  - Implement validation that tries both interpretations
  - Consider that some YY values could be valid in both centuries with different check digits
- **Century Ambiguity**: Numbers with YY values like 00-23 could be either 1900 or 2000
  - Use the check digit calculation to determine which century is correct
  - First try the 2000s interpretation (if YY is reasonably post-2000)
  - Fall back to 1900s interpretation if 2000s validation fails
  - Document this ambiguity clearly
- **Gender Encoding**: Based on serial number parity, not a specific digit
  - Odd serial = male
  - Even serial = female
  - Independent of century calculation
- **Reserved Serial Numbers**: 000 and 999 are not used
  - Validate that serial is in range 001-998
- **Check Digit Range**: Check digits range from 01-97
  - This is a result of the modulo 97 calculation
  - Validate that check digits match the calculated modulo 97 result

- **Format Flexibility**: Accept various separator patterns during input
  - YY.MM.DD-XXX.CC (official format)
  - YYMMDD-XXXCC, YY.MM.DD-XXXCC, etc.
  - No separators at all
  - Normalize to digits-only for storage
  - Provide Format() method for official format

## References
- [Wikipedia - National identification number (Belgium)](https://en.wikipedia.org/wiki/National_identification_number#Belgium)
- [Belgian Government - National Register](https://www.ibz.rrn.fgov.be/en/)
- [Wikipedia - Rijksregisternummer (Dutch)](https://nl.wikipedia.org/wiki/Rijksregisternummer)

## Testing Requirements
Create comprehensive unit tests covering:
- Valid National Register Numbers from 1900s (9-digit check calculation)
- Valid National Register Numbers from 2000s (10-digit check calculation)
- All validation failure paths
- Boundary dates (leap years, century transition, year 2000)
- Ambiguous years (00-23 could be either century)
- Format variations (with/without separators, various separator patterns)
- Gender determination (odd/even serial numbers)
- Serial number boundary cases (001, 002, 998, and invalid 000, 999)
- Check digit boundary cases (00, 96, invalid 97-99)
- Property value extraction
- Conversion operators
- Equality semantics
- JSON serialization/deserialization
- Check digit calculation accuracy for both pre-2000 and post-2000
- Century determination logic
- Date validation including leap years in both centuries

## Documentation Requirements
- Complete XML documentation comments
- Update README.md with BeNationalRegisterNumber section
- Include example usage
- Document the modulo 97 check digit algorithm for both pre-2000 and post-2000 births
- Document the century determination logic and potential ambiguity
- Explain the gender encoding based on serial number parity
- Note the reserved serial numbers (000, 999)
- Note the check digit range limit (00-96)
- Provide clear examples for both 1900s and 2000s births
- Document the official format (YY.MM.DD-XXX.CC) and accepted variations

## Implementation Phases
1. **Phase 1**: Basic structure, validation rules (excluding check digits), properties
2. **Phase 2**: Date validation with leap year handling for both centuries
3. **Phase 3**: Check digit validation using modulo 97 algorithm (both 9-digit and 10-digit)
4. **Phase 4**: Century determination logic with comprehensive tests
5. **Phase 5**: Gender determination, serial number validation
6. **Phase 6**: Format() method, separator handling during parsing
7. **Phase 7**: JSON serialization, conversion operators, full test coverage
8. **Phase 8**: Documentation, README updates, code review

## Notable Differences from Other Identity Types
- **Check Digits (plural)**: Uses 2 check digits instead of 1, providing stronger error detection
- **Year 2000 Algorithm Change**: The validation algorithm changes based on birth year
- **Century Ambiguity**: Unlike other systems, determining the century requires check digit validation
- **Gender Encoding**: Similar to Finnish and Danish systems (parity of serial number)
- **Reserved Numbers**: Serial 000 and 999 are explicitly not used
- **Check Digit Range**: Limited to 00-96 (never 97-99) due to modulo 97 calculation
- **Multiple Format Variations**: More flexible separator acceptance than some other systems
