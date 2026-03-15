# S0012-FiHenkilotunnus Business Object

## Overview
Create a strongly typed business object to represent a Finnish henkilötunnus (personal identity code) issued by the Digital and Population Data Services Agency (Digi- ja väestötietovirasto, DVV). The henkilötunnus serves as a national identification number for all persons residing in Finland and is assigned at birth or upon registration in the Finnish population information system.

## Background
A Finnish henkilötunnus is an 11-character identifier in the format DDMMYYCZNNC, where:
- **DDMMYY**: Date of birth (day, month, year - 2 digits each)
- **C**: Century character (+ = 1800s, - = 1900s, A = 2000s, B = 2100s, etc.)
- **Z**: Reserved separator (always the century character, no other character allowed)
- **NN**: Individual number (002-899 for persons born in Finland, 900-999 for temporary codes)
- **C**: Check character calculated using modulo 31 algorithm

The individual number serves multiple purposes:
1. Distinguishes between individuals born on the same day
2. Encodes gender: odd = male, even = female
3. Range 002-899: Permanent identity codes for persons born in Finland
4. Range 900-999: Temporary identity codes (e.g., for asylum seekers)
5. Numbers 000-001 are not used

## User Story
**As a** developer building an application that processes Finnish identification numbers  
**I want** a strongly typed business object that validates and parses henkilötunnus values  
**So that** I can ensure data integrity and easily access henkilötunnus components including date of birth, gender, and identity type

## Acceptance Criteria

### 1. Constructor
- Accepts a string representation of a henkilötunnus
- Must be exactly 11 characters in the format DDMMYYCZNNC (no format variations accepted)
- The century character (position 6, zero-based) must be one of: +, -, A, B, C, D, E, F
- Throws `KfValidationException<FiHenkilotunnusValidationResult>` for invalid values
- Constructor is case-insensitive (check character may be uppercase or lowercase)
- Normalizes check character to uppercase

### 2. Validation Rules
A valid henkilötunnus must meet all of the following rules:
- Must not be null, empty, or all whitespace
- Must be exactly 11 characters long
- Characters 0-1 (DD) must be digits 01-31
- Characters 2-3 (MM) must be digits 01-12
- Characters 4-5 (YY) must be digits 00-99
- Character 6 must be a valid century character: +, -, A, B, C, D, E, F
  - \+ = 1800-1899
  - \- = 1900-1999
  - A = 2000-2099
  - B = 2100-2199
  - C = 2200-2299
  - D = 2300-2399
  - E = 2400-2499
  - F = 2500-2599
- Characters 7-9 (NNN) must be digits 002-999
  - 000-001: Not used
  - 002-899: Permanent codes
  - 900-999: Temporary codes
- Character 10 must be a valid check character: 0-9, A-Y (excluding G, I, O, Q, Z)
- The date portion (DDMMYY) must represent a valid calendar date
- The check character must be correctly calculated using the modulo 31 algorithm
- Future dates are specifically **NOT** tested for to avoid issues requiring the class to be aware of current time

### 3. Properties
- `Value` (string): The raw henkilötunnus string as stored (with uppercase check character)
- `DateOfBirth` (DateOnly): Returns the person's date of birth derived from the henkilötunnus
- `Gender` (BinaryGender): Returns Male or Female based on odd/even individual number
- `CenturyCharacter` (char): Returns the century character (+, -, A, B, C, D, E, F)
- `IndividualNumber` (string): Returns the three-digit individual number
- `CheckCharacter` (char): Returns the check character
- `IsTemporary` (bool): Returns true if individual number is in range 900-999

### 4. Static Methods
- `Validate(string? henkilotunnus)`: Returns `FiHenkilotunnusValidationResult` enum indicating validation status
- `Create(string? henkilotunnus)`: Returns `CreateResult<FiHenkilotunnus, FiHenkilotunnusValidationResult>`

### 5. Conversion Operators
- Implicit conversion from `FiHenkilotunnus` to `string`
- Explicit conversion from `string` to `FiHenkilotunnus`

### 6. Standard Object Methods
- `ToString()`: Returns the henkilötunnus value
- `Equals()`: Value-based equality comparison (case-insensitive for check character)
- `GetHashCode()`: Consistent with equality

### 7. JSON Serialization
- Custom JSON converter for seamless serialization/deserialization
- Serializes as a simple string value
- Handles null values gracefully

## Check Character Algorithm
The check character is calculated using the modulo 31 algorithm:
1. Take DDMMYYNNN as a 9-digit number
2. Calculate: remainder = (DDMMYYNNN) mod 31
3. Map remainder to check character using: "0123456789ABCDEFHJKLMNPRSTUVWXY"
   - 0 ? '0', 1 ? '1', ..., 9 ? '9'
   - 10 ? 'A', 11 ? 'B', ..., 30 ? 'Y'
   - Note: Letters G, I, O, Q, Z are excluded from the character set

## Century Character Mapping
| Character | Century    | Years       |
|-----------|------------|-------------|
| +         | 19th       | 1800-1899   |
| -         | 20th       | 1900-1999   |
| A         | 21st       | 2000-2099   |
| B         | 22nd       | 2100-2199   |
| C         | 23rd       | 2200-2299   |
| D         | 24th       | 2300-2399   |
| E         | 25th       | 2400-2499   |
| F         | 26th       | 2500-2599   |

## Example Values
Valid henkilötunnus:
- `010190-1234` - Born January 1, 1990, male (odd individual number 123)
- `311299-100P` - Born December 31, 1999, female (even individual number 100)
- `010100A1234` - Born January 1, 2000 (century A = 2000s), male
- `150585+024W` - Born May 15, 1885 (century + = 1800s), female
- `010190-900X` - Temporary code, male (individual number 900)

Valid henkilötunnus with different check characters:
- `131052-308T` - Example from Wikipedia
- `131052A308T` - Same person born in 2052

Invalid examples:
- `000190-1234` - Invalid day (00)
- `320190-1234` - Invalid day (32)
- `010090-1234` - Invalid month (00)
- `011390-1234` - Invalid month (13)
- `290290-1234` - Invalid date (Feb 29 in non-leap year 1990)
- `010190-001X` - Invalid individual number (001 not used)
- `010190-000X` - Invalid individual number (000 not used)
- `010190X1234` - Invalid century character (X)
- `010190-123G` - Invalid check character (G not in character set)
- `010190-1235` - Invalid check character (incorrect calculation)
- `010190-123` - Invalid length (too short)
- `010190 1234` - Invalid separator (space instead of century character)

## Implementation Notes
1. Place in namespace: `KfAccountNumbers.Governmental.Europe`
2. Use ISO country code: `Fi` (Finland)
3. Follow the same patterns as `SePersonnummer`, `MxCurp`, and `DkPersonnummer`
4. Create corresponding enum: `FiHenkilotunnusValidationResult`
5. Implement full unit test coverage similar to `SePersonnummerTests`
6. No format variations - henkilötunnus has a single fixed format (no separators, no alternatives)
7. Store value with normalized check character (uppercase)
8. The century character is both a separator and an encoded value - do not allow other separators
9. Gender determination: individual number mod 2 (odd = male, even = female)
10. Handle leap year validation correctly for all centuries
11. Implement check character calculation exactly according to modulo 31 algorithm
12. Validate check character mapping excludes G, I, O, Q, Z

## Special Considerations
- **No Format Variations**: Unlike some other identity numbers, the henkilötunnus has exactly one format
  - Always 11 characters
  - Always DDMMYYCZNNC format
  - No dashes, spaces, or other separators beyond the century character
- **Century Character**: Serves dual purpose as separator and century indicator
  - Must validate that it's in the correct position (index 6)
  - Must validate that it's a recognized century character
- **Check Character Set**: The modulo 31 remainder maps to a specific 31-character alphabet
  - Excludes letters G, I, O, Q, Z to avoid confusion with similar-looking characters
  - Must implement exact mapping: "0123456789ABCDEFHJKLMNPRSTUVWXY"
- **Individual Number Ranges**:
  - 000-001: Never used
  - 002-899: Permanent codes (most common)
  - 900-999: Temporary codes (less common)
  - Provide `IsTemporary` property to distinguish
- **Gender Encoding**: Based on individual number parity, not a specific digit
  - Odd individual number (e.g., 123, 905) = male
  - Even individual number (e.g., 124, 900) = female

## References
- [Wikipedia - National identification number (Finland)](https://en.wikipedia.org/wiki/National_identification_number#Finland)
- [Digital and Population Data Services Agency (DVV)](https://dvv.fi/en/personal-identity-code)

## Testing Requirements
Create comprehensive unit tests covering:
- Valid henkilötunnus from different centuries (1800s, 1900s, 2000s, future centuries)
- Valid henkilötunnus with permanent codes (002-899)
- Valid henkilötunnus with temporary codes (900-999)
- All validation failure paths
- Boundary dates (leap years, century transitions, end of months)
- Century character variations (+, -, A, B, C, D, E, F)
- Gender determination (odd/even individual numbers)
- Individual number boundary cases (001, 002, 899, 900, 999)
- Property value extraction
- Conversion operators
- Equality semantics (case-insensitive for check character)
- JSON serialization/deserialization
- Check character validation for all 31 possible values
- Check character calculation accuracy
- Invalid check characters (G, I, O, Q, Z and others)
- Case normalization (lowercase check character ? uppercase)

## Documentation Requirements
- Complete XML documentation comments
- Update README.md with FiHenkilotunnus section
- Include example usage
- Document the check character algorithm and the 31-character mapping
- Document the century character encoding system
- Explain the individual number ranges and their meanings
- Note that the format has no variations (no optional separators)
- Provide clear examples for all centuries
- Explain gender encoding based on individual number parity

## Implementation Phases
1. **Phase 1**: Basic structure, validation rules (excluding check character), properties
2. **Phase 2**: Century determination logic with comprehensive tests
3. **Phase 3**: Check character validation using modulo 31 algorithm
4. **Phase 4**: Gender determination, temporary code detection
5. **Phase 5**: JSON serialization, conversion operators, full test coverage
6. **Phase 6**: Documentation, README updates, code review