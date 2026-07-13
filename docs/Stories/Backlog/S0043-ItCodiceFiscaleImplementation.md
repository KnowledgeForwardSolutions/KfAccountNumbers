# S0043: Implement IrCodiceFiscale Type for Italian Tax Identification Number

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 13 points  
**Related:** S0038 (SePersonnummerTypeHierarchy), S0039 (NoFoedselsnummerTypeHierarchy), S0019-S0027 (Union migrations), E01-UnionMigration

---

## Overview

Implement a new strongly-typed business object representing an Italian Codice Fiscale (Tax Identification Number). The Codice Fiscale is a 16-character alphanumeric code issued by the Agenzia delle Entrate (Italian Revenue Agency) to uniquely identify individuals for tax and administrative purposes.

Unlike the Swedish (SePersonnummer, SeSamordningsnummer) and Norwegian (NoFoedselsnummer, NoDNummer) hierarchies which employ multiple restricted types, the Italian Codice Fiscale is a single, monolithic identifier type. The type will follow the union-based validation pattern established in S0019–S0027, with comprehensive validation of the deterministic portion (surname, first name, date of birth, gender, municipality) and the check digit algorithm.

Implement as:
```csharp
public record IrCodiceFiscale : IrCodiceFiscaleBase
{
    // Single unrestricted type accepting all valid Codice Fiscale values
    // No subtype hierarchy (unlike Swedish/Norwegian types)
}
```

---

## Business Value

* **Tax Compliance**: Supports Italian tax identification and administrative processing, essential for businesses operating in or with Italian entities.
* **Data Integrity**: Validates both the deterministic portion and the check digit, ensuring data accuracy before storage or transmission.
* **International Expansion**: Enables the library to serve European business domains beyond Nordic countries.
* **Consistency**: Follows proven patterns from S0038, S0039, and earlier union migrations, maintaining architectural coherence.
* **Type Safety**: Compile-time enforcement via strongly-typed business object; eliminates string-based identity processing.
* **Validation Robustness**: Encapsulates complex Italian fiscal code rules (surname/first name encoding, gender/date of birth mapping, check digit algorithm).
* **Developer Clarity**: Self-documenting API; developers immediately recognize this as an Italian tax identifier.

---

## Requirements

### Functional Requirements

#### 1. Codice Fiscale Format and Structure

The Codice Fiscale consists of 16 characters divided into six sections:

```
RSMGNN85M05F839K

RSM  = Surname (3 letters, encoded from surname)
GNN  = First Name (3 letters, encoded from first name)
85   = Year of Birth (2 digits, last two digits of year)
M    = Month of Birth (1 letter: A=Jan, B=Feb, ..., L=Dec)
05   = Day of Birth (2 digits, padded with leading zero)
F839 = Municipality Code (1 letter + 3 digits; or Z999 for foreign country)
K    = Check Digit (1 letter, calculated via modular algorithm)
```

**Detailed Breakdown:**

##### 1.1 Surname Section (Characters 1–3)

The first three characters are derived from the individual's surname:

- **Rule**: Consonants are taken in order; if fewer than 3 consonants, vowels (a, e, i, o, u) are taken in order.
- **If still fewer than 3**: Pad with 'X'.
- **Examples**:
  - Surname "Rossi" → consonants: R, S, S → `RSI`
  - Surname "Verdi" → consonants: V, R, D → `VRD`
  - Surname "Leo" → consonants: L; vowels: E, O → `LEO`
  - Surname "Ai" → consonants: (none); vowels: A, I → `AIX` (padded)

##### 1.2 First Name Section (Characters 4–6)

The next three characters are derived from the first name, using the same rule as the surname:

- **Rule**: If the first name contains 4 or more consonants, take consonants at positions 1, 3, and 4 (skipping position 2 to increase variation).
- **If fewer than 4 consonants or after skipping**: Take all consonants in order, then vowels.
- **If still fewer than 3**: Pad with 'X'.
- **Examples**:
  - First name "Giovanni" → consonants: G, V, N, N (4) → take positions 1, 3, 4 → `GVN`
  - First name "Maria" → consonants: M, R; vowels: A, I, A → `MRA`
  - First name "Pio" → consonants: P; vowels: I, O → `PIO`

##### 1.3 Date of Birth Section (Characters 7–9)

Characters 7 and 8 represent the year and month; character 9 represents the day.

- **Characters 7–8 (Year)**: Two-digit year of birth (e.g., "85" for 1985 or 2085).
- **Character 9 (Month)**: Single letter representing birth month:
  - A = January, B = February, C = March, D = April, E = May, F = June
  - G = July, H = August, I = September, L = October, M = November, S = December
  - Note: No 'J' used (to avoid confusion with 'I'/'l').
- **Characters 10–11 (Day)**: Two-digit day of birth (01–31).

##### 1.4 Gender and Day Encoding (Character 10–11)

The day field encodes both day of birth and gender:

- **For Females**: Day is increased by 40 (e.g., day 05 becomes 45).
- **For Males**: Day remains as is (01–31).
- **Valid Ranges**:
  - Males: 01–31
  - Females: 41–71 (representing days 01–31)

##### 1.5 Municipality Code (Characters 12–14)

A 3-digit numeric code identifying the municipality of birth:

- **Italian Municipality**: 001–999 (ISTAT code)
- **Foreign Country**: "999" for individuals born outside Italy or in territories not under Italian jurisdiction.
- **Validation Rule**: No specific validation required beyond numeric format; codes are registered with Agenzia delle Entrate.

##### 1.6 Check Digit (Character 16)

A single alphabetic character calculated using a modulo-26 algorithm:

1. Assign each character a value: vowels (A, E, I, O, U) = 0; consonants (B, C, D, ..., Z) = 1–21.
2. For odd positions: use the assigned value directly.
3. For even positions: multiply by 2, then take modulo 26.
4. Sum all values from steps 2 and 3.
5. Divide the sum by 26 and take the remainder (0–25).
6. Convert the remainder to a letter: A=0, B=1, ..., Z=25.

**Character Value Table**:
```
A=0, B=1, C=2, D=3, E=4, F=5, G=6, H=7, I=8, L=9, M=10
N=11, O=12, P=13, Q=14, R=15, S=16, T=17, U=18, V=19, Z=20
W=21, X=22, Y=23, Z=24 (duplicate for compatibility)
```

**Example**:
```
Codice Fiscale: RSMGNN85M05F839K
Calculation of 'K':
Position: 1   2   3   4   5   6   7   8   9  10  11  12  13  14  15
Char:     R   S   M   G   N   N   8   5   M  0   5   F   8   3   9
Odd val:  15  16  10  6   11  11  ?   ?   ?  ?   ?   ?   ?   ?   ?
Even val: (apply x2 mod 26)

(Detailed calculation omitted for brevity; implementation follows standard algorithm)
```

#### 2. IrCodiceFiscaleBase Type

Create an abstract record base class that encapsulates common functionality:

```csharp
public abstract record IrCodiceFiscaleBase
{
    // Common constants and static properties
    // Common validation logic and helper methods
    // Shared properties: Value, DateOfBirth, Gender
    // Union-based ValidationError and ValidationResult
}
```

**Responsibilities:**
- Store the normalized identifier value (16 characters, uppercase alphanumeric).
- Parse and store date of birth, gender, and municipality code.
- Implement validation logic for all six sections (surname, first name, date of birth, gender, municipality, check digit).
- Define the common `Value`, `DateOfBirth`, `Gender`, and `Municipality` properties.
- Provide shared validation methods and error handling via discriminated unions.

**Design Constraints**:
- Should be `abstract`, not instantiable directly.
- Use the union-based validation pattern from S0019–S0027.
- Must be `record` type for value semantics and immutability.
- Should reside in `IrCodiceFiscaleBase.cs` (new file).

**Italian-Specific Logic**:
- Modulo-26 check digit calculation (not Luhn or weighted modulus 11).
- Character value mapping (vowels vs. consonants).
- Gender-based day encoding (male: 01–31, female: 41–71).
- Month-to-letter encoding (A–S, excluding J).
- Surname/first name consonant extraction algorithm.

#### 3. IrCodiceFiscale Type (Unrestricted)

Implement a single, unrestricted type that accepts all valid Codice Fiscale values:

```csharp
public record IrCodiceFiscale : IrCodiceFiscaleBase
{
    // Constructor: IrCodiceFiscale(String? value)
    // Static method: Create(String? value) -> CreateResult<IrCodiceFiscale, ValidationError>
    // Static method: Validate(String? value) -> ValidationResult

    // Properties:
    // - Value: String (16-character normalized identifier)
    // - DateOfBirth: DateOnly (derived from characters 7-11)
    // - Gender: Gender.BinaryGender (derived from character 11 encoding)
    // - Municipality: String (3-digit code from characters 12-14)

    // Methods:
    // - ToString(): String (returns Value in uppercase)
    // - implicit operator String(IrCodiceFiscale)
    // - explicit operator IrCodiceFiscale(String?)

    // JSON Converter: IrCodiceFiscaleJsonConverter
}
```

**Behavior**:
- Constructor throws `UKfValidationException<ValidationError>` if value is invalid.
- `Create()` method returns `CreateResult<IrCodiceFiscale, ValidationError>` for result-pattern usage.
- `Validate()` static method returns `ValidationResult` union (ValidValue or specific error).
- No subtype hierarchy; all valid Codice Fiscale values are accepted equally.
- JSON serialization/deserialization supported via custom converter.

#### 4. Validation Rules

When creating a new `IrCodiceFiscale`, the following validation rules are applied:

1. **Null/Empty**: Value may not be null, empty, or all whitespace characters.
2. **Length**: Value must be exactly 16 characters.
3. **Character Set**: All characters must be uppercase ASCII letters or digits.
4. **Format**:
   - Characters 1–3: Letters (surname)
   - Characters 4–6: Letters (first name)
   - Characters 7–8: Digits (year)
   - Character 9: Letter (month: A–S, excluding J)
   - Characters 10–11: Digits (day)
   - Character 12–14: Digits (municipality code)
   - Character 15: Unused (reserved)
   - Character 16: Letter (check digit)
5. **Date Validity**: Characters 7–11 must represent a valid calendar date:
   - Year: 00–99 (interpreted relative to current date; typically 00–current_year%100)
   - Month: A–S (January–December, excluding J)
   - Day: For males (01–31); for females (41–71, representing 01–31)
   - Valid days for month (e.g., February 02 through 29, accounting for leap years)
6. **Gender Encoding**: Character 11 must correctly indicate gender via the day encoding:
   - Male: 01–31
   - Female: 41–71
7. **Check Digit**: Character 16 must be a valid check digit calculated from characters 1–15 using the modulo-26 algorithm.
8. **Municipality Code**: Characters 12–14 must be numeric (001–999); no specific validation against registered codes required at the type level.

#### 5. Error Handling

Define a discriminated union for validation errors:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidFormat,
   InvalidDateOfBirth,
   InvalidGenderEncoding,
   InvalidCheckDigit
) { }

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidFormat,
   InvalidDateOfBirth,
   InvalidGenderEncoding,
   InvalidCheckDigit
) { }
```

**Error Types**:
- **EmptyValue**: Input is null, empty, or all whitespace.
- **InvalidLength**: Input length is not 16 characters.
- **InvalidCharacter**: Input contains non-ASCII or non-alphanumeric characters.
- **InvalidFormat**: Input does not match the expected section format (e.g., month letter not A–S).
- **InvalidDateOfBirth**: Date components do not represent a valid calendar date.
- **InvalidGenderEncoding**: Day field does not correctly encode gender (male 01–31, female 41–71).
- **InvalidCheckDigit**: Check digit does not match the calculated value.

### Acceptance Criteria

1. ✓ `IrCodiceFiscaleBase` abstract base class implemented with union-based validation.
2. ✓ `IrCodiceFiscale` type created; constructor and `Create()` method work correctly.
3. ✓ `Validate()` static method returns appropriate `ValidationResult` for all error conditions.
4. ✓ All validation rules (null/empty, length, format, date, gender encoding, check digit) are enforced.
5. ✓ `DateOfBirth` property correctly parses year, month, and day.
6. ✓ `Gender` property correctly derives gender from day field encoding.
7. ✓ `Municipality` property returns 3-digit code.
8. ✓ Implicit and explicit conversion operators work correctly.
9. ✓ JSON serialization and deserialization work correctly (round-trip).
10. ✓ Unit tests achieve >95% code coverage and cover:
    - Valid values (various surnames, first names, dates, genders, municipalities)
    - Invalid null/empty/whitespace
    - Invalid lengths
    - Invalid characters
    - Invalid format sections
    - Invalid dates (non-existent dates, invalid months, days out of range)
    - Invalid gender encoding
    - Invalid check digits
    - JSON round-trips
    - Null handling in JSON
    - Error propagation in JSON deserialization
11. ✓ Reference documentation (`IrCodiceFiscale.md`) created with examples and validation rules.

---

## Implementation Notes

### Check Digit Algorithm

The check digit calculation is **not** the Luhn algorithm (used for Swedish identifiers) or weighted modulus 11 (used for Norwegian identifiers). Instead, it uses a **modulo-26** algorithm:

1. Each character (1–15) is assigned a numeric value based on the character-value table.
2. Odd-positioned characters (1, 3, 5, ..., 15) contribute their value directly.
3. Even-positioned characters (2, 4, 6, ..., 14) contribute (value * 2) mod 26.
4. Sum all contributions.
5. Remainder = (sum mod 26).
6. Check digit = remainder converted to letter (A=0, B=1, ..., Z=25).

This algorithm is **deterministic** and **self-correcting** (check digit is always uniquely determined by the first 15 characters).

### Date of Birth Calculation

The date of birth is derived from:
- Year (characters 7–8): Two-digit year. Century determination uses current date context (00–current_year%100 typically refers to 20xx; higher values typically refer to 19xx).
- Month (character 9): Single letter (A–S) decoded to numeric month (1–12).
- Day (characters 10–11): Two-digit day, with gender adjustment (subtract 40 for females to recover original day 01–31).

### No Surname/First Name Encoding Validation

The implementation **does not validate** that characters 1–6 correctly encode the individual's surname and first name. Validation is limited to:
- Character set (uppercase letters only).
- Modulo-26 check digit algorithm.

Encoding validation would require access to the individual's legal name and would be impractical at the type level. Agenzia delle Entrate maintains a registry for full validation; this type focuses on format and check digit correctness.

### Foreign Country Handling

For individuals born outside Italy:
- Characters 12–14 are set to "999" (standard code for foreign country).
- Date of birth and gender are still encoded normally.
- Validation accepts "999" as a valid municipality code.

### Scope: Single Type, No Subtype Hierarchy

Unlike Swedish (SePersonnummer/SeSamordningsnummer) and Norwegian (NoFoedselsnummer/NoDNummer/NoHnummer) identifiers, which encode **two or three distinct identity categories** with different eligibility rules, the Italian Codice Fiscale is a **single identity category** issued to all individuals for tax purposes. Therefore:
- **No subtype hierarchy** (no "restricted" types like SePersonnummer vs. SeSamordningsnummer).
- **Single type**: `IrCodiceFiscale`.
- Possible future extension: If Italian law introduces additional identity categories (similar to Norwegian D-numbers or H-numbers), a hierarchy could be refactored at that time.

---

## User Stories / Related Issues

- **Related Backlog Stories**: S0038 (Swedish hierarchy), S0039 (Norwegian hierarchy), S0040 (Norwegian H-number)
- **Related Union Migrations**: S0019–S0027 (earlier union-pattern implementations)
- **Inspiration**: Follows proven patterns from GB identifiers (GbPatientNumber, GbNhsNumber, GbChiNumber) and Nordic identifiers.

---

## Testing Strategy

### Unit Test Coverage

Create `IrCodiceFiscaleTests.cs` with comprehensive test coverage:

#### Constructor Tests
- Valid Codice Fiscale values (various surnames, first names, dates, genders, municipalities)
- Invalid null/empty/whitespace input
- Invalid length (too short, too long)
- Invalid characters (non-ASCII, non-alphanumeric)
- Invalid format (wrong character type for each section)
- Invalid dates (non-existent dates, invalid months, days out of range)
- Invalid gender encoding (day field not 01–31 or 41–71)
- Invalid check digits

#### Validate() Method Tests
- Same invalid scenarios as constructor (returning ValidationResult instead of throwing)

#### Create() Method Tests
- Same valid and invalid scenarios (returning CreateResult instead of throwing or returning ValidValue)

#### Property Tests
- `DateOfBirth` property correctly parses year, month, day
- `Gender` property correctly derives gender from day encoding
- `Municipality` property returns correct 3-digit code
- `Value` property returns 16-character normalized string

#### Conversion Operator Tests
- Implicit conversion to `String`
- Explicit conversion from `String`
- Round-trip: `String` → `IrCodiceFiscale` → `String`

#### Equality and Hash Code Tests
- Value equality (two instances with same Codice Fiscale are equal)
- Different values are not equal
- Hash code consistency
- Reference equality vs. value equality

#### JSON Serialization Tests
- Round-trip serialization/deserialization
- Null value serialization
- Complex object serialization (IrCodiceFiscale as a property of another class)
- Deserialization error handling (invalid Codice Fiscale in JSON throws validation exception)

#### Check Digit Tests
- Valid check digits for known Codice Fiscale values
- Invalid check digits are rejected
- Deterministic check digit calculation (same input always produces same digit)

### Test Data

Use a set of valid and invalid test Codice Fiscale values. Example valid values (may be randomized or verified against public sources):
```
RSMGNN85M05F839K  (male, born 1985-05-05, municipality F839)
VRDELC90L15D612A  (female, born 1990-12-15, municipality D612)
```

Invalid test values:
```
RSMGNN85M05F839J  (check digit incorrect, month has 'J' which is invalid)
RSMGNN85Q05F839K  (month 'Q' is invalid, valid months are A–S excluding J)
RSMGNN85M45F839K  (invalid day field '45' for male, only 01–31 valid; female would be 41–71)
```

---

## Deliverables

1. **Source Code**:
   - `src/KfAccountNumbers/Governmental/Europe/IrCodiceFiscaleBase.cs` (abstract base class)
   - `src/KfAccountNumbers/Governmental/Europe/IrCodiceFiscale.cs` (main type and JSON converter)

2. **Unit Tests**:
   - `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/IrCodiceFiscaleTests.cs` (comprehensive test suite)

3. **Documentation**:
   - `Documentation/Reference/National/Europe/IrCodiceFiscale.md` (API reference with examples)

4. **Definition of Done**:
   - All acceptance criteria met
   - Unit tests pass with >95% code coverage
   - Code review approval
   - Integration with existing library (registered in any identifier registry/factory if applicable)
   - Documentation complete and reviewed

---

## Assumptions & Constraints

- **Naming Convention**: Type name follows pattern `IrCodiceFiscale` (country code "It" for Italy, type name "CodiceFiscale").
- **No External Dependencies**: Implementation uses only .NET standard library and existing library abstractions (no external validation libraries).
- **ASCII Uppercase**: Implementation stores and validates only uppercase ASCII letters and digits; lowercase input is converted or rejected.
- **No Legal Name Validation**: Does not validate that surname/first name encoding matches legal records (scope limited to format and check digit).
- **No Registry Lookup**: Does not validate against the Agenzia delle Entrate registry; format and algorithm validation only.
- **Century Derivation**: Uses standard heuristic (two-digit year 00–current%100 → 20xx; higher values → 19xx). This is standard practice and matches most library conventions.

---

## References

- Wikipedia: [Italian fiscal identification number](https://en.wikipedia.org/wiki/Italian_fiscal_identification_number)
- Agenzia delle Entrate: [Codice Fiscale](https://www.agenziaentrate.gov.it/)
- S0038: Swedish Personnummer Type Hierarchy
- S0039: Norwegian Fødselsnummer Type Hierarchy
- S0040: Norwegian H-number Implementation
- Union Migration Pattern: S0019–S0027

