# S0010-IsKennitala Business Object

## Overview
Create a strongly typed business object to represent an Icelandic kennitala (Icelandic identification number) issued by Registers Iceland (Þjóðskrá Íslands). The kennitala serves as a national identification number for both individuals and legal entities (companies, organizations).

## Background
An Icelandic kennitala is a 10-digit identifier in the format DDMMYY-RRCC, where:
- **DDMMYY**: Date of birth (day, month, year - 2 digits each)
- **RR**: Random/sequence digits (originally random, now sequential)
- **C**: Century indicator (9 = 1900s, 0 = 2000s, 8 = 1800s)
- **C**: Check digit calculated using modulo 11 algorithm

The first digit distinguishes between individuals (0-3) and legal entities (4-7). For legal entities, the date portion does not represent an actual date but rather the registration date or other administrative date.

## User Story
**As a** developer building an application that processes Icelandic identification numbers  
**I want** a strongly typed business object that validates and parses kennitala values  
**So that** I can ensure data integrity and easily access kennitala components

## Acceptance Criteria

### 1. Constructor
- Accepts a string representation of a kennitala
- Accepts either 10 digits (DDMMYYRRCC) or 11 characters with separator (DDMMYY-RRCC)
- The separator character (if present) must be a dash (-)
- Throws `KfValidationException<IsKennitalaValidationResult>` for invalid values
- Constructor is case-insensitive (not applicable for kennitala, but consistent with other types)

### 2. Validation Rules
A valid kennitala must meet all of the following rules:
- Must not be null, empty, or all whitespace
- Must be either 10 digits or 11 characters with a dash separator in position 6
- Characters 0-1 (DD) must be digits 01-71 (01-31 for individuals, 41-71 for legal entities)
- Characters 2-3 (MM) must be digits 01-12
- Characters 4-5 (YY) must be digits 00-99
- If present, character 6 must be a dash (-)
- Characters 7-8 (RR) must be digits 00-99
- Character 9 (first C) must be a valid century indicator (8, 9, or 0)
- Character 10 (second C) must be a valid check digit calculated using modulo 11 algorithm
- For individual kennitala (DD 01-31), the date portion must represent a valid calendar date
- For legal entity kennitala (DD 41-71), date validation is relaxed (registration dates)

### 3. Properties
- `Value` (string): The raw kennitala string as stored
- `DateOfBirth` (DateOnly?): Returns the date of birth for individual kennitala, or null for legal entities
- `IsLegalEntity` (bool): Returns true if the kennitala represents a legal entity (DD >= 41)
- `CenturyIndicator` (int): Returns the century indicator digit (8, 9, or 0)
- `RandomDigits` (string): Returns the two-digit random/sequence number
- `CheckDigit` (int): Returns the check digit

### 4. Static Methods
- `Validate(string? kennitala)`: Returns `IsKennitalaValidationResult` enum indicating validation status
- `Create(string? kennitala)`: Returns `CreateResult<IsKennitala, IsKennitalaValidationResult>`

### 5. Conversion Operators
- Implicit conversion from `IsKennitala` to `string`
- Explicit conversion from `string` to `IsKennitala`

### 6. Standard Object Methods
- `ToString()`: Returns the kennitala value
- `Equals()`: Value-based equality comparison
- `GetHashCode()`: Consistent with equality

### 7. JSON Serialization
- Custom JSON converter for seamless serialization/deserialization
- Serializes as a simple string value
- Handles null values gracefully

## Check Digit Algorithm
The check digit is calculated using the modulo 11 algorithm with weights [3, 2, 7, 6, 5, 4, 3, 2]:
1. Multiply each of the first 8 digits by its corresponding weight
2. Sum all products
3. Calculate remainder when sum is divided by 11
4. Check digit = 11 - remainder (if result is 11, check digit is 0)

## Example Values
Valid individual kennitala:
- `120174-3389` - Born January 12, 1974 (century indicator 9 = 1900s)
- `1201743389` - Same, without separator
- `010130-2989` - Born January 1, 2030 (century indicator 0 = 2000s)

Valid legal entity kennitala:
- `450169-3339` - Legal entity registered 1969
- `4501693339` - Same, without separator

Invalid examples:
- `000174-3389` - Invalid day (00)
- `320174-3389` - Invalid day (32) for individual
- `120074-3389` - Invalid month (00)
- `121374-3389` - Invalid month (13)
- `120174-3388` - Invalid check digit
- `120174-3387` - Invalid century indicator (7)

## Implementation Notes
1. Place in namespace: `KfAccountNumbers.Governmental.Europe`
2. Use ISO country code: `Is` (Iceland)
3. Follow the same patterns as `SePersonnummer`, `MxCurp`, and `UsSocialSecurityNumber`
4. Create corresponding enum: `IsKennitalaValidationResult`
5. Implement full unit test coverage similar to `SePersonnummerTests`
6. Support both formatted (with dash) and unformatted input
7. Store value in unformatted or formatted form (decide on consistency)
8. Provide a `Format()` method that returns the formatted version (DDMMYY-RRCC)

## References
- [Wikipedia - Kennitala](https://en.wikipedia.org/wiki/Kennitala)
- [Registers Iceland (Þjóðskrá Íslands)](https://www.skra.is/)

## Testing Requirements
Create comprehensive unit tests covering:
- Valid individual kennitala (various dates, centuries)
- Valid legal entity kennitala
- All validation failure paths
- Boundary dates (leap years, century changes)
- Format variations (with/without separator)
- Property value extraction
- Conversion operators
- Equality semantics
- JSON serialization/deserialization
- Check digit validation (including undetectable errors)

## Documentation Requirements
- Complete XML documentation comments
- Update README.md with IsKennitala section
- Include example usage
- Document the check digit algorithm
- Note differences between individual and legal entity kennitala