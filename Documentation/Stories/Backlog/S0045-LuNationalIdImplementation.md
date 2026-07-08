# S0045: Implement LuNationalId Type for Luxembourg National Identification Number

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 11 points  
**Related:** S0038 (SePersonnummerTypeHierarchy), S0043 (ItCodiceFiscaleImplementation), S0044 (DeStuerIdImplementation), S0019-S0027 (Union migrations), E01-UnionMigration

---

## Overview

Implement a new strongly-typed business object representing a Luxembourg National Identification Number (Numéro d'identification nationale or Numéro de matricule). The Luxembourg national ID is a 13-digit numeric code issued by the Administration des Douanes et Accises (Luxembourg Customs and Excise Administration) to uniquely identify individuals for administrative, tax, and social security purposes.

Like the Italian Codice Fiscale (S0043) and German Steuer-IdNr (S0044), the Luxembourg National ID is a single, monolithic identifier type with no subtype hierarchy. However, it differs from these in that it encodes date of birth and gender (similar to Swedish and Norwegian identifiers), making it a hybrid implementation pattern.

The type will follow the union-based validation pattern established in S0019–S0027, with comprehensive validation of the numeric format, date of birth encoding, gender indicator, and check digit algorithm.

Implement as:
```csharp
public record LuNationalId : LuNationalIdBase
{
    // Single unrestricted type accepting all valid Luxembourg National ID values
    // Encodes date of birth and gender (unlike Italian/German types)
    // No subtype hierarchy (unlike Swedish/Norwegian types)
}
```

---

## Business Value

* **Administrative Compliance**: Supports Luxembourg administrative identification and processing, essential for businesses operating in or with Luxembourg entities.
* **Social Security Integration**: Aligns with Luxembourg social security and tax systems, critical for payroll and benefits processing.
* **Data Integrity**: Validates numeric format, date of birth encoding, gender indicator, and check digit, ensuring data accuracy.
* **International Expansion**: Enables the library to serve European business domains across Nordic, Southern, Central, and Western European countries.
* **Consistency**: Follows proven patterns from S0038, S0043, S0044, and earlier union migrations, maintaining architectural coherence.
* **Type Safety**: Compile-time enforcement via strongly-typed business object; eliminates string-based identity processing.
* **Validation Robustness**: Encapsulates Luxembourg national ID validation rules (format, date encoding, gender indicator, check digit).
* **Developer Clarity**: Self-documenting API; developers immediately recognize this as a Luxembourg national identifier.

---

## Requirements

### Functional Requirements

#### 1. Luxembourg National ID Format and Structure

The Luxembourg National Identification Number consists of 13 digits:

```
19670412 001 L V

19670412 = Date of birth (8 digits: YYYYMMDD, 1967-04-12)
001      = Serial number (3 digits: 001–999, for multiple individuals born on same day)
L        = Luhn check digit (calculated from first 11 digits: YYYYMMDDXXX)
V        = Verhoeff check digit (calculated from first 12 digits: YYYYMMDDXXXL)
```

**Detailed Breakdown:**

##### 1.1 Overall Format

- **Total Length**: 13 digits
- **Range**: 18000101001XX to 20991231999XX (where XX = check digits)
- **No Separators**: Format is strictly numeric with no hyphens, spaces, or other characters
- **No Gender Encoding**: Unlike Norwegian identifiers, day field is always 01–31; no gender encoding
- **Structure**: YYYYMMDDXXXLV (date-serial-luhn-verhoeff)

##### 1.2 Date of Birth Components

**Year (Characters 1–4)**:
- Four-digit year of birth (1800–2099)
- Full year representation; no century derivation required
- Valid range: 18000101 to 20991231

**Month (Characters 5–6)**:
- Two-digit month (01–12)
- Standard numeric: 01 = January, 02 = February, ..., 12 = December

**Day (Characters 7–8)**:
- Two-digit day component (01–31)
- Standard calendar days; no gender encoding (unlike Norwegian identifiers)
- **Valid Ranges**:
  - 01–31 (standard calendar days)
  - Day must be valid for the given month and year (accounting for leap years)

##### 1.3 Serial Number

**Serial Number (Characters 9–11)**:
- Three-digit code (001–999)
- Used to disambiguate multiple individuals born on the exact same date
- Sequential assignment in order of registration for same-day births
- Always 3 digits; 001 is the first individual born on a given date, 002 the second, etc.

##### 1.4 Check Digits

The Luxembourg National ID uses **two distinct check digit algorithms**:

###### Luhn Check Digit (Character 12)

1. **Extract the first 11 digits** (YYYYMMDDXXX)
2. **Apply Luhn algorithm**:
   - Double every second digit (starting from the right)
   - If doubling results in a value > 9, subtract 9
   - Sum all digits
   - Luhn digit = (10 - (sum mod 10)) mod 10
3. **Result**: Single digit (0–9)

###### Verhoeff Check Digit (Character 13)

1. **Extract the first 12 digits** (YYYYMMDDXXXL)
2. **Apply Verhoeff algorithm**:
   - Verhoeff is a stronger checksum algorithm than Luhn or modulus 11
   - Uses three mathematical tables: permutation, inverse, and multiplication tables
   - Calculates a single check digit (0–9) from the 12-digit sequence
3. **Result**: Single digit (0–9)

**Note**: The Verhoeff algorithm is more complex than Luhn but provides better error detection. Implementation should use established Verhoeff implementations or reference the algorithm definition carefully.

**Why Two Check Digits?**
- Luhn provides basic error detection (catches most single-digit errors and transpositions)
- Verhoeff provides enhanced error detection (catches all single-digit errors, all transpositions of adjacent digits, and other common patterns)
- Combined use provides very strong validation robustness

#### 2. LuNationalIdBase Type

Create an abstract record base class that encapsulates common functionality:

```csharp
public abstract record LuNationalIdBase
{
    // Common constants and static properties
    // Common validation logic and helper methods
    // Shared properties: Value, DateOfBirth, Gender
    // Union-based ValidationError and ValidationResult
}
```

**Responsibilities:**
- Store the normalized identifier value (13 digits as a string, without separators).
- Parse and store date of birth (extracted from characters 1–6).
- Parse and store gender (derived from day field encoding).
- Implement validation logic for format and check digit.
- Define the common `Value`, `DateOfBirth`, and `Gender` properties.
- Provide shared validation methods and error handling via discriminated unions.

**Design Constraints**:
- Should be `abstract`, not instantiable directly.
- Use the union-based validation pattern from S0019–S0027.
- Must be `record` type for value semantics and immutability.
- Should reside in `LuNationalIdBase.cs` (new file).

**Luxembourg-Specific Logic**:
- Modulo-97 or weighted modulus 11 check digit calculation (to be verified).
- Date of birth parsing from first 6 digits.
- Gender detection from day field (01–31 = male, 41–71 = female).
- Century derivation from two-digit year (using sliding window heuristic).

#### 3. LuNationalId Type (Unrestricted)

Implement a single, unrestricted type that accepts all valid Luxembourg National ID values:

```csharp
public record LuNationalId : LuNationalIdBase
{
    // Constructor: LuNationalId(String? value)
    // Static method: Create(String? value) -> CreateResult<LuNationalId, ValidationError>
    // Static method: Validate(String? value) -> ValidationResult

    // Properties:
    // - Value: String (13-digit normalized identifier)
    // - DateOfBirth: DateOnly (derived from characters 1-6)
    // - Gender: Gender.BinaryGender (derived from character 5-6 encoding)

    // Methods:
    // - ToString(): String (returns Value)
    // - implicit operator String(LuNationalId)
    // - explicit operator LuNationalId(String?)

    // JSON Converter: LuNationalIdJsonConverter
}
```

**Behavior**:
- Constructor throws `UKfValidationException<ValidationError>` if value is invalid.
- `Create()` method returns `CreateResult<LuNationalId, ValidationError>` for result-pattern usage.
- `Validate()` static method returns `ValidationResult` union (ValidValue or specific error).
- No subtype hierarchy; all valid Luxembourg National ID values are accepted equally.
- JSON serialization/deserialization supported via custom converter.
- `DateOfBirth` and `Gender` properties provide convenient access to decoded data (unlike German type).

#### 4. Validation Rules

When creating a new `LuNationalId`, the following validation rules are applied:

1. **Null/Empty**: Value may not be null, empty, or all whitespace characters.
2. **Length**: Value must be exactly 13 characters.
3. **Character Set**: All characters must be ASCII digits (0–9).
4. **Format**:
   - Characters 1–8: Digits (date of birth: YYYYMMDD)
   - Characters 9–11: Digits (serial number: 001–999)
   - Character 12: Digit (Luhn check digit)
   - Character 13: Digit (Verhoeff check digit)
5. **Date Validity**: Characters 1–8 must represent a valid calendar date:
   - Year: 1800–2099 (full 4-digit year)
   - Month: 01–12
   - Day: 01–31 (standard calendar days; no gender encoding)
   - Valid days for month (e.g., February 01–28/29, accounting for leap years)
   - Date must be in the past or present (no future dates)
6. **Serial Number Validity**: Characters 9–11 must be in range 001–999 (always 3 digits).
7. **Luhn Check Digit**: Character 12 must be a valid Luhn check digit calculated from the first 11 digits (YYYYMMDDXXX).
8. **Verhoeff Check Digit**: Character 13 must be a valid Verhoeff check digit calculated from the first 12 digits (YYYYMMDDXXXL).

#### 5. Error Handling

Define a discriminated union for validation errors:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidDateOfBirth,
   InvalidSerialNumber,
   InvalidLuhnCheckDigit,
   InvalidVerhoeffCheckDigit
) { }

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidDateOfBirth,
   InvalidSerialNumber,
   InvalidLuhnCheckDigit,
   InvalidVerhoeffCheckDigit
) { }
```

**Error Types**:
- **EmptyValue**: Input is null, empty, or all whitespace.
- **InvalidLength**: Input length is not 13 characters.
- **InvalidCharacter**: Input contains non-digit characters.
- **InvalidDateOfBirth**: Date components do not represent a valid calendar date.
- **InvalidSerialNumber**: Serial number is not in range 001–999.
- **InvalidLuhnCheckDigit**: Luhn check digit (character 12) does not match the calculated value.
- **InvalidVerhoeffCheckDigit**: Verhoeff check digit (character 13) does not match the calculated value.

### Acceptance Criteria

1. ✓ `LuNationalIdBase` abstract base class implemented with union-based validation.
2. ✓ `LuNationalId` type created; constructor and `Create()` method work correctly.
3. ✓ `Validate()` static method returns appropriate `ValidationResult` for all error conditions.
4. ✓ All validation rules (null/empty, length, format, date, serial number, Luhn check digit, Verhoeff check digit) are enforced.
5. ✓ `DateOfBirth` property correctly parses year, month, and day from characters 1–8.
6. ✓ `Gender` property **is not implemented** (Luxembourg National ID does not encode gender; day field is standard 01–31).
7. ✓ Serial number is validated to be in range 001–999 (characters 9–11).
8. ✓ Luhn check digit (character 12) is correctly calculated and validated from first 11 digits.
9. ✓ Verhoeff check digit (character 13) is correctly calculated and validated from first 12 digits.
10. ✓ Implicit and explicit conversion operators work correctly.
11. ✓ JSON serialization and deserialization work correctly (round-trip).
12. ✓ Unit tests achieve >95% code coverage and cover:
    - Valid values (various birth dates, serial numbers)
    - Invalid null/empty/whitespace
    - Invalid lengths
    - Invalid characters
    - Invalid dates (non-existent dates, invalid months, days out of range)
    - Invalid serial numbers (000, >999)
    - Invalid Luhn check digits
    - Invalid Verhoeff check digits
    - Date validation (leap years, month boundaries, valid date ranges)
    - JSON round-trips
    - Null handling in JSON
    - Error propagation in JSON deserialization
13. ✓ Reference documentation (`LuNationalId.md`) created with examples and validation rules.

---

## Implementation Notes

### Check Digit Algorithms

The Luxembourg National ID uses **two sequential check digit algorithms**, both industry-standard and well-documented:

#### Luhn Algorithm (Character 12)

The Luhn algorithm is one of the most widely-used checksums for numeric identifiers:

1. **Extract the first 11 digits** (YYYYMMDDXXX)
2. **Double every second digit from the right**:
   - Starting from the rightmost digit, double every other digit (positions 2, 4, 6, 8, 10)
3. **If doubling results in > 9, subtract 9**:
   - Example: 8 * 2 = 16 → 16 - 9 = 7
4. **Sum all digits** (both original and doubled)
5. **Calculate Luhn digit**: luhn_digit = (10 - (sum mod 10)) mod 10

**Example**:
```
YYYYMMDDXXX: 1967041200 1
Digits:      1 9 6 7 0 4 1 2 0 0 1
Positions:   1 2 3 4 5 6 7 8 9 10 11 (from left)
Double pos:  ✗ ✓ ✗ ✓ ✗ ✓ ✗ ✓ ✗ ✓ ✗
Doubled:     1 18 6 14 0 8 1 4 0 0 1
After -9:    1 9  6 5  0 8 1 4 0 0 1
Sum: 1+9+6+5+0+8+1+4+0+0+1 = 35
Luhn: (10 - (35 mod 10)) mod 10 = (10 - 5) mod 10 = 5
```

#### Verhoeff Algorithm (Character 13)

The Verhoeff algorithm is a stronger checksum that detects all single-digit errors and all adjacent transpositions:

1. **Extract the first 12 digits** (YYYYMMDDXXXL)
2. **Apply Verhoeff calculation using three mathematical tables**:
   - Permutation table (dihedral group D5 permutations)
   - Inverse table
   - Multiplication table (also from D5 group)
3. **Iterate through each digit, applying table lookups**:
   - Initialize p = identity permutation
   - Initialize inv = inverse of current permutation
   - For each digit d (left to right):
     - p = permutation[p[d]]
     - If position is even: inv = inv * multiplication_table[d]
4. **Calculate check digit**: verhoeff_digit = inverse[p]

**Note**: Verhoeff is complex; implementation should use an established, tested library or carefully reference the algorithm definition. Multiple reference implementations exist in various programming languages.

**Why Two Check Digits?**
- **Luhn** detects most common errors (single-digit changes, most transpositions)
- **Verhoeff** detects **all** single-digit errors and all adjacent transpositions
- Combined use provides extremely high error detection robustness (near-certainty of catching any single error or adjacent transposition)

### Date of Birth Parsing

The date of birth is straightforward (no century derivation required):

- Year (characters 1–4): Four-digit year (1800–2099), parsed directly
- Month (characters 5–6): Two-digit month (01–12), no encoding
- Day (characters 7–8): Two-digit day (01–31), standard calendar days

**Example**:
- ID: 19670412001LV
  - Year: 1967
  - Month: 04 (April)
  - Day: 12
  - DateOfBirth: 1967-04-12

### No Gender Encoding

Unlike Norwegian identifiers which encode gender via day field offset (41–71 for females), the Luxembourg National ID:
- Does **not encode gender**
- Day field is always 01–31 (standard calendar days)
- Gender information is not extractable from the ID itself

Therefore, the type:
- Does **not include** a `Gender` property
- Significantly simpler than Nordic types (no offset logic)

---

## User Stories / Related Issues

- **Related Backlog Stories**: S0038 (Swedish hierarchy), S0043 (Italian Codice Fiscale), S0044 (German Steuer-IdNr)
- **Related Union Migrations**: S0019–S0027 (earlier union-pattern implementations)
- **Inspiration**: Hybrid pattern combining date/gender encoding (Nordic) with single monolithic type (Italian/German).

---

## Testing Strategy

### Unit Test Coverage

Create `LuNationalIdTests.cs` with comprehensive test coverage:

#### Constructor Tests
- Valid Luxembourg National ID values (various birth dates, genders, sequential numbers)
- Invalid null/empty/whitespace input
- Invalid length (too short, too long)
- Invalid characters (non-digits)
- Invalid dates (non-existent dates, invalid months, days out of range)
- Invalid gender encoding (day field not 01–31 or 41–71)
- Invalid check digits

#### Validate() Method Tests
- Same invalid scenarios as constructor (returning ValidationResult instead of throwing)

#### Create() Method Tests
- Same valid and invalid scenarios (returning CreateResult instead of throwing or returning ValidValue)

#### Property Tests
- `DateOfBirth` property correctly parses year, month, day
  - Valid dates across multiple centuries (1900s, 2000s)
  - Leap year handling (Feb 29)
  - Month boundary dates (Jan 31, Apr 30, etc.)
- `Gender` property correctly derives gender from day encoding
  - Male: day 01–31
  - Female: day 41–71
- `Value` property returns 13-digit string

#### Conversion Operator Tests
- Implicit conversion to `String`
- Explicit conversion from `String`
- Round-trip: `String` → `LuNationalId` → `String`

#### Equality and Hash Code Tests
- Value equality (two instances with same National ID are equal)
- Different values are not equal
- Hash code consistency
- Reference equality vs. value equality

#### Century Derivation Tests
- Year 00–20 interpreted as 2000–2020 (or based on current date sliding window)
- Year 21–99 interpreted as 1921–1999
- Boundary cases (year 20 vs. 21, current year ± 1)

#### Gender Detection Tests
- Day 01–31 detected as male
- Day 41–71 detected as female
- Invalid day ranges rejected (32–40, 72–99)

#### Check Digit Tests
- Valid check digits for known Luxembourg National ID values
- Invalid check digits are rejected
- Deterministic check digit calculation

#### Date Boundary Tests
- Valid: 2000-02-29 (leap year)
- Invalid: 1900-02-29 (not a leap year, despite century year)
- Valid: 2020-02-29 (leap year)
- Valid/Invalid: Month boundaries (31 days, 30 days, 28/29 days)

### Test Data

Use known valid Luxembourg National ID values or construct test data based on the check digit algorithm. Example test data structure:

```csharp
public static TheoryData<String, Int32, Int32, Int32, Char> ValidLuNationalIdValues =>
[
    // (ID string, expected year, expected month, expected day, expected gender)
    ("6704120000011", 1967, 4, 12, 'M'),        // Male, 1967-04-12
    ("9205230000012", 1992, 5, 23, 'F'),        // Female, 1992-05-23
    ("8512010000013", 1985, 12, 1, 'M'),        // Male, 1985-12-01
    ("7103150000014", 1971, 3, 15, 'F'),        // Female, 1971-03-15 (day 55 = day 15 + 40)
    // Additional known valid IDs
];

public static TheoryData<String> InvalidCheckDigitValues =>
[
    "6704120000010",  // Last digit incorrect
    "6704120000012",  // Last digit incorrect
    "6704120000000",  // Check digit incorrect
];
```

---

## Deliverables

1. **Source Code**:
   - `src/KfAccountNumbers/Governmental/Europe/LuNationalIdBase.cs` (abstract base class)
   - `src/KfAccountNumbers/Governmental/Europe/LuNationalId.cs` (main type and JSON converter)

2. **Unit Tests**:
   - `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/LuNationalIdTests.cs` (comprehensive test suite)

3. **Documentation**:
   - `Documentation/Reference/National/Europe/LuNationalId.md` (API reference with examples)

4. **Definition of Done**:
   - All acceptance criteria met
   - Unit tests pass with >95% code coverage
   - Code review approval (verify check digit algorithm against authoritative sources)
   - Integration with existing library (registered in any identifier registry/factory if applicable)
   - Documentation complete and reviewed

---

## Assumptions & Constraints

- **Naming Convention**: Type name follows pattern `LuNationalId` (country code "Lu" for Luxembourg, type name "NationalId" for national identification).
- **No External Dependencies**: Implementation uses only .NET standard library and existing library abstractions.
- **Numeric Format Only**: Luxembourg National ID is strictly numeric; no letters or special characters.
- **Date of Birth Extraction**: Unlike German Steuer-IdNr, this type includes date of birth and gender extraction (similar to Nordic types).
- **Single Type, No Hierarchy**: Unlike Swedish/Norwegian types, no subtype discrimination is required; single monolithic type.
- **Check Digit Algorithm**: Implementation should verify against published test cases; initial implementation may require verification spike if algorithm is not readily available.
- **Century Derivation**: Uses sliding window heuristic (e.g., current_year ± 1–2 years to determine century); standard practice for two-digit year encoding.
- **Gender Encoding**: Follows Norwegian convention: 01–31 (male), 41–71 (female).

---

## References

- Luxembourg Administration: [National Identification Number](https://guichet.public.lu/)
- Wikipedia: [National identification number](https://en.wikipedia.org/wiki/National_identification_number)
- S0043: Italian Codice Fiscale Implementation
- S0044: German Steuer-IdNr Implementation
- S0038: Swedish Personnummer Type Hierarchy (date of birth extraction pattern)
- S0039: Norwegian Fødselsnummer Type Hierarchy (gender encoding pattern)
- Union Migration Pattern: S0019–S0027

---

## Historical Context & Related Work

This story extends the library's coverage of European national identifiers. Prior implementations include:

| Country | Type | Status | Story | Key Features |
|---------|------|--------|-------|--------------|
| Sweden | Personnummer / Samordningsnummer | Completed | S0038 | Date + Gender; Subtype hierarchy |
| Norway | Fødselsnummer / D-nummer / H-nummer | Backlog | S0039–S0041 | Date + Gender; Multiple categories |
| Italy | Codice Fiscale | Backlog | S0043 | Name/date encoding; Modulo-26 check |
| Germany | Steuer-IdNr | Backlog | S0044 | Sequential numeric; Weighted mod 11 |
| Luxembourg | National ID | **This Story** | S0045 | Date + Gender; Single type |

Additional European identifiers that may be implemented in future stories:
- France: Numéro de sécurité sociale (Social Security Number) - with date of birth and gender encoding
- Spain: NIF (Número de Identidad Fiscal) - tax identifier
- Netherlands: BSN (Burgerservicenummer) - citizen service number with date encoding
- Belgium: Numéro de registre national - with date of birth and gender encoding
- Ireland: Personal Public Service Number (PPSN) - tax and social security identifier
- UK: National Insurance Number (already implemented as GbNationalInsuranceNumber)

---

## Notes for Implementation Team

1. **Algorithm Verification**: Before implementation begins, verify the check digit algorithm against published test cases or Luxembourg Authority documentation. Consider opening a technical spike (E-number) if algorithm clarification is needed.

2. **Century Derivation**: Implement and test the sliding window heuristic for century determination. Ensure consistency with other Nordic identifier types (Swedish, Norwegian) that use similar heuristics.

3. **Gender Encoding Pattern**: This type demonstrates a third gender-encoding pattern:
   - Swedish: Last digit of birth sequence number (odd/even)
   - Norwegian: Day field with +40 offset for females
   - Luxembourg: Day field with +40 offset for females (same as Norwegian)

   Consistent implementation across these types strengthens the library's coherence.

4. **Hybrid Implementation Pattern**: This type represents a hybrid approach:
   - **Date/Gender Extraction**: Like Nordic types (S0038, S0039)
   - **Single Type, No Hierarchy**: Like Italian/German types (S0043, S0044)

   This hybrid pattern may be useful for other European identifiers (Netherlands, Belgium, France).

5. **Future European Identifiers**: After completing this story, the library will have strong coverage across Western and Central Europe:
   - Nordic: Sweden, Norway
   - Southern: Italy
   - Central: Germany
   - Western: Luxembourg

   Future expansion into France, Spain, Netherlands, and Belgium would complete coverage of major European economies.

