# S0011-DkPersonnummer Business Object

## Overview
Create a strongly typed business object to represent a Danish personnummer, also known as CPR-nummer (Central Person Register number), issued by the Danish Civil Registration System (Det Centrale Personregister). The CPR-nummer serves as a national identification number for all persons residing in Denmark.

## Background
A Danish CPR-nummer is a 10-digit identifier in the format DDMMYY-SSSC, where:
- **DDMMYY**: Date of birth (day, month, year - 2 digits each)
- **SSS**: Sequence number (odd = male, even = female)
- **C**: Check digit calculated using modulo 11 algorithm

The sequence number (SSS) serves dual purposes:
1. Distinguishes between individuals born on the same day
2. Encodes gender: odd final digit = male, even final digit = female
3. Indicates century of birth through specific ranges:
   - 0000-3999: Born 1900-1999
   - 4000-4999: Born 2000-2036 or 1900-1937
   - 5000-8999: Born 1858-1899 or 2000-2057
   - 9000-9999: Born 1937-1999

Note: The modulo 11 check digit validation was phased out for CPR numbers issued after October 1, 2007, but is still used for validation of older numbers.

## User Story
**As a** developer building an application that processes Danish identification numbers  
**I want** a strongly typed business object that validates and parses CPR-nummer values  
**So that** I can ensure data integrity and easily access CPR-nummer components including date of birth and gender

## Acceptance Criteria

### 1. Constructor
- Accepts a string representation of a CPR-nummer
- Accepts either 10 digits (DDMMYYSSSC) or 11 characters with separator (DDMMYY-SSSC)
- The separator character (if present) must be a dash (-)
- Throws `KfValidationException<DkPersonnummerValidationResult>` for invalid values
- Constructor is case-insensitive (not applicable for CPR-nummer, but consistent with other types)

### 2. Validation Rules
A valid CPR-nummer must meet all of the following rules:
- Must not be null, empty, or all whitespace
- Must be either 10 digits or 11 characters with a dash separator in position 6
- Characters 0-1 (DD) must be digits 01-31
- Characters 2-3 (MM) must be digits 01-12
- Characters 4-5 (YY) must be digits 00-99
- If present, character 6 must be a dash (-)
- Characters 7-9 (SSS) must be digits 000-999
- Character 10 (C) must be a digit 0-9
- The date portion (DDMMYY) combined with sequence number must represent a valid calendar date
- For CPR numbers with sequence 0000-3999: Check digit validation using modulo 11 algorithm
- For CPR numbers with sequence 4000-9999: Check digit validation is optional/relaxed (many modern CPR numbers do not follow modulo 11)
- Future dates are specifically **NOT** tested for to avoid issues requiring the class to be aware of current time

### 3. Properties
- `Value` (string): The raw CPR-nummer string as stored
- `DateOfBirth` (DateOnly): Returns the person's date of birth derived from the CPR-nummer
- `Gender` (BinaryGender): Returns Male or Female based on the last digit of the sequence number
- `SequenceNumber` (string): Returns the three-digit sequence number
- `CheckDigit` (int): Returns the check digit
- `HasValidCheckDigit` (bool): Returns true if the check digit passes modulo 11 validation (may be false for post-2007 CPR numbers)

### 4. Static Methods
- `Validate(string? personnummer)`: Returns `DkPersonnummerValidationResult` enum indicating validation status
- `Create(string? personnummer)`: Returns `CreateResult<DkPersonnummer, DkPersonnummerValidationResult>`

### 5. Conversion Operators
- Implicit conversion from `DkPersonnummer` to `string`
- Explicit conversion from `string` to `DkPersonnummer`

### 6. Standard Object Methods
- `ToString()`: Returns the CPR-nummer value
- `Equals()`: Value-based equality comparison
- `GetHashCode()`: Consistent with equality

### 7. Formatting
- `Format()` method that returns formatted version with dash (DDMMYY-SSSC)
- Support both formatted and unformatted input in constructor/validation

### 8. JSON Serialization
- Custom JSON converter for seamless serialization/deserialization
- Serializes as a simple string value
- Handles null values gracefully

## Check Digit Algorithm (Pre-2007)
The check digit is calculated using the modulo 11 algorithm with weights [4, 3, 2, 7, 6, 5, 4, 3, 2, 1]:
1. Multiply each of the 10 digits by its corresponding weight
2. Sum all products
3. Calculate remainder when sum is divided by 11
4. If remainder is 0, the check digit is valid
5. Note: Some valid modern CPR numbers may not satisfy this check

## Century Determination Logic
The century is determined by the sequence number (SSS):
- **0000-3999**: Born 1900-1999
- **4000-4999**: 
  - If YY is 00-36: Born 2000-2036
  - If YY is 37-99: Born 1937-1999
- **5000-8999**:
  - If YY is 00-57: Born 2000-2057
  - If YY is 58-99: Born 1858-1899
- **9000-9999**: Born 1937-1999

## Example Values
Valid CPR-nummer (with modulo 11 check digit):
- `010190-1234` - Born January 1, 1990, male (odd sequence ending)
- `0101901234` - Same, without separator
- `311299-1008` - Born December 31, 1999, female (even sequence ending)
- `010100-4000` - Born January 1, 2000 (sequence 4000 indicates 2000s)

Valid CPR-nummer (post-2007, may not have valid modulo 11):
- `010115-5555` - Born January 1, 2015, male (sequence may not pass modulo 11)

Invalid examples:
- `000190-1234` - Invalid day (00)
- `320190-1234` - Invalid day (32)
- `010090-1234` - Invalid month (00)
- `011390-1234` - Invalid month (13)
- `290290-1234` - Invalid date (Feb 29 in non-leap year 1990)
- `010190-123` - Invalid length (missing check digit)
- `010190*1234` - Invalid separator

## Implementation Notes
1. Place in namespace: `KfAccountNumbers.Governmental.Europe`
2. Use ISO country code: `Dk` (Denmark)
3. Follow the same patterns as `SePersonnummer`, `MxCurp`, and `UsSocialSecurityNumber`
4. Create corresponding enum: `DkPersonnummerValidationResult`
5. Implement full unit test coverage similar to `SePersonnummerTests`
6. Support both formatted (with dash) and unformatted input
7. Store value in the format provided by user (maintain formatting consistency)
8. The `HasValidCheckDigit` property should be computed, not stored
9. Century determination is complex - implement carefully with extensive test coverage
10. Handle leap year validation correctly for all centuries
11. Consider that check digit validation failure should not necessarily fail overall validation (post-2007 numbers)

## Special Considerations
- **Check Digit Transition**: CPR numbers issued after October 1, 2007, may not satisfy modulo 11 validation
  - Validation should accept CPR numbers regardless of check digit validity
  - Provide `HasValidCheckDigit` property to inform users about check digit status
  - Document this clearly in XML comments
- **Century Ambiguity**: The sequence number ranges create complex century determination logic
  - Implement comprehensive tests for all century/sequence combinations
  - Document edge cases (e.g., 1937, 1999, 2000, 2036, etc.)
- **Gender Encoding**: Unlike Swedish personnummer, gender is determined by the last digit of the entire 10-digit number (not just birth serial number)

## References
- [Wikipedia - Personal identification number (Denmark)](https://en.wikipedia.org/wiki/Personal_identification_number_(Denmark))
- [CPR Office (CPR-kontoret)](https://www.cpr.dk/)

## Testing Requirements
Create comprehensive unit tests covering:
- Valid CPR-nummer from different centuries (1800s, 1900s, 2000s)
- Valid CPR-nummer with and without valid check digits (pre/post 2007)
- All validation failure paths
- Boundary dates (leap years, century transitions, year 1937, 1999, 2000, 2036, 2057)
- Format variations (with/without separator)
- Gender determination (odd/even last digit)
- Century determination for all sequence ranges
- Property value extraction
- Conversion operators
- Equality semantics
- JSON serialization/deserialization
- Check digit validation (including acknowledgment that post-2007 may fail)
- Edge cases for sequence number ranges and their century mappings

## Documentation Requirements
- Complete XML documentation comments
- Update README.md with DkPersonnummer section
- Include example usage
- Document the check digit algorithm and its optional nature post-2007
- Document the complex century determination logic
- Explain the gender encoding mechanism
- Note the transition in 2007 regarding check digit validation
- Provide clear examples for all century/sequence combinations

## Implementation Phases
1. **Phase 1**: Basic structure, validation rules (excluding check digit), properties
2. **Phase 2**: Century determination logic with comprehensive tests
3. **Phase 3**: Check digit validation with `HasValidCheckDigit` property
4. **Phase 4**: JSON serialization, conversion operators, full test coverage
5. **Phase 5**: Documentation, README updates, code review