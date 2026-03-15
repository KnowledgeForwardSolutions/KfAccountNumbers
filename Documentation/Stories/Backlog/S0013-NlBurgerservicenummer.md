# S0013-NlBurgerservicenummer Business Object

## Overview
Create a strongly typed business object to represent a Dutch Burgerservicenummer (BSN) issued by the Dutch government. The BSN is a national identification number assigned to all persons registered in the Dutch population register (Basisregistratie Personen, BRP). It replaced the older SOFI number (Sociaal-Fiscaalnummer) in 2007.

## Background
A Dutch BSN is a 9-digit identifier without any inherent formatting. The number consists of:
- **8 digits**: Sequential digits assigned by the government
- **1 digit**: Check digit calculated using the "11-proef" (modulo 11 check) algorithm

Unlike many other national identification numbers, the BSN does not encode:
- Date of birth
- Gender
- Place of birth
- Any other personal information

The BSN is purely a sequential identifier with a mathematically validated check digit to detect transcription errors.

## User Story
**As a** developer building an application that processes Dutch identification numbers  
**I want** a strongly typed business object that validates and parses BSN values  
**So that** I can ensure data integrity and prevent invalid BSN values from entering my system

## Acceptance Criteria

### 1. Constructor
- Accepts a string representation of a BSN
- Accepts either 9 digits without formatting or with optional space/dash separators
- Common formats: 123456782, 123 456 782, 123-456-782, 12345678-2
- Throws `KfValidationException<NlBurgerservicenummerValidationResult>` for invalid values
- Constructor is case-insensitive (not applicable for BSN, but consistent with other types)

### 2. Validation Rules
A valid BSN must meet all of the following rules:
- Must not be null, empty, or all whitespace
- Must contain exactly 9 digits (after removing any separator characters)
- All characters must be digits (0-9) or valid separator characters (space or dash)
- Must pass the "11-proef" (modulo 11 check) validation algorithm
- If separators are used:
  - Spaces or dashes are allowed
  - Separators must be consistent throughout (all spaces or all dashes)
  - Common patterns: XXX XXX XXX, XXX-XXX-XXX, XXXXXXXX-X

### 3. Properties
- `Value` (string): The raw BSN string as stored (digits only, no separators)
- `CheckDigit` (int): Returns the check digit (last digit)

### 4. Static Methods
- `Validate(string? bsn)`: Returns `NlBurgerservicenummerValidationResult` enum indicating validation status
- `Create(string? bsn)`: Returns `CreateResult<NlBurgerservicenummer, NlBurgerservicenummerValidationResult>`

### 5. Conversion Operators
- Implicit conversion from `NlBurgerservicenummer` to `string`
- Explicit conversion from `string` to `NlBurgerservicenummer`

### 6. Standard Object Methods
- `ToString()`: Returns the BSN value (digits only)
- `Equals()`: Value-based equality comparison
- `GetHashCode()`: Consistent with equality

### 7. JSON Serialization
- Custom JSON converter for seamless serialization/deserialization
- Serializes as a simple string value (digits only)
- Handles null values gracefully

## 11-Proef Validation Algorithm
The check digit is validated using the "11-proef" (modulo 11 check) algorithm:
1. Multiply each digit by its position weight: [9, 8, 7, 6, 5, 4, 3, 2, -1]
   - 1st digit × 9
   - 2nd digit × 8
   - 3rd digit × 7
   - 4th digit × 6
   - 5th digit × 5
   - 6th digit × 4
   - 7th digit × 3
   - 8th digit × 2
   - 9th digit × -1 (check digit)
2. Sum all products
3. Calculate: sum mod 11
4. If remainder is 0, the BSN is valid

Example validation for BSN 111222333:

(1×9) + (1×8) + (1×7) + (2×6) + (2×5) + (2×4) + (3×3) + (3×2) + (3×-1) = 9 + 8 + 7 + 12 + 10 + 8 + 9 + 6 + (-3) = 66 66 mod 11 = 0 ? Valid

## Example Values
Valid BSN:
- `111222333` - Basic valid BSN
- `123456782` - Another valid BSN
- `111 222 333` - Valid with space separators
- `123-456-782` - Valid with dash separators
- `12345678-2` - Valid with dash before check digit

Invalid examples:
- `111222334` - Invalid check digit (fails 11-proef)
- `12345678` - Invalid length (too short)
- `1234567890` - Invalid length (too long)
- `12345678a` - Invalid character (not a digit)
- `123 456-782` - Invalid mixed separators
- `000000000` - All zeros (technically valid by algorithm but typically reserved/not issued)
- `123456789` - Invalid check digit

## Implementation Notes
1. Place in namespace: `KfAccountNumbers.Governmental.Europe`
2. Use ISO country code: `Nl` (Netherlands)
3. Follow the same patterns as `SePersonnummer`, `MxCurp`, and other identity types
4. Create corresponding enum: `NlBurgerservicenummerValidationResult`
5. Implement full unit test coverage similar to `SePersonnummerTests`
6. Support multiple input formats (with/without separators)
7. Store value without separators (digits only)
8. Do NOT implement a `Format()` method - BSN has no official formatting standard
9. Handle separator characters during parsing/validation
10. The 11-proef algorithm weight for the check digit is negative (-1)

## Special Considerations
- **No Personal Information**: Unlike most other national ID numbers, BSN contains no encoded information
  - No date of birth property
  - No gender property
  - No place of birth or other metadata
  - It is purely a validated sequential identifier
- **Separator Variations**: While no official format exists, common patterns include:
  - No separators (most common): 123456782
  - Space separators: 123 456 782
  - Dash separators: 123-456-782, 12345678-2
  - Accept all variations during input
  - Store without separators
  - Do not provide a Format() method (no official standard)
- **11-Proef Algorithm**: The check digit uses a negative weight (-1)
  - This is different from most check digit algorithms
  - Ensure correct implementation with the negative multiplier
  - Test thoroughly with known valid/invalid BSNs
- **Reserved Numbers**: Some BSN ranges may be reserved or have special meanings
  - 000000000 may be technically valid but not issued
  - Do not implement special validation for reserved ranges (treat as valid if passes 11-proef)
  - Document that validation checks format only, not actual issuance status
- **Historic SOFI Numbers**: BSN replaced SOFI in 2007
  - BSN and SOFI use the same format and validation
  - This class validates the format, not the historical context
  - Some older systems may still use "SOFI" terminology

## References
- [Wikipedia - Burgerservicenummer](https://en.wikipedia.org/wiki/National_identification_number#Netherlands)
- [Wikipedia - Burgerservicenummer (Dutch)](https://nl.wikipedia.org/wiki/Burgerservicenummer)
- [Dutch Government - BSN Information](https://www.rijksoverheid.nl/onderwerpen/privacy-en-persoonsgegevens/vraag-en-antwoord/wat-is-het-burgerservicenummer-bsn)

## Testing Requirements
Create comprehensive unit tests covering:
- Valid BSN values with correct check digits
- All validation failure paths
- Format variations (no separators, spaces, dashes, mixed patterns)
- Invalid check digits (including common transcription errors)
- Edge cases (all zeros, all nines, repeating patterns)
- Property value extraction (Value, CheckDigit)
- Conversion operators
- Equality semantics
- JSON serialization/deserialization
- 11-proef algorithm validation accuracy
- Separator handling and normalization
- Single-digit transcription errors (should be detected)
- Adjacent digit transposition errors (may or may not be detected)
- Jump transposition errors (should be detected in most cases)

## Documentation Requirements
- Complete XML documentation comments
- Update README.md with NlBurgerservicenummer section
- Include example usage
- Document the 11-proef check digit algorithm clearly
- Emphasize that BSN contains no personal information (contrast with other identity types)
- Note the separator handling approach (accept variations, store without)
- Explain that validation is format-only, not issuance verification
- Provide clear examples with and without separators
- Document the negative weight in the 11-proef algorithm

## Implementation Phases
1. **Phase 1**: Basic structure, validation rules (excluding check digit), properties
2. **Phase 2**: 11-proef check digit validation with comprehensive tests
3. **Phase 3**: Separator handling (parsing and normalization)
4. **Phase 4**: JSON serialization, conversion operators, full test coverage
5. **Phase 5**: Documentation, README updates, code review

## Notable Differences from Other Identity Types
- **No encoded information**: BSN is unique in that it encodes nothing but a sequential number
- **No standard formatting**: Unlike SSN (XXX-XX-XXXX) or personnummer (YYMMDD-XXXX), BSN has no official format
- **Negative weight in check digit**: The -1 multiplier for the check digit is unusual
- **Simpler validation**: No date validation, gender validation, or century determination logic
- **Format method**: Do NOT implement - there is no standard format to return