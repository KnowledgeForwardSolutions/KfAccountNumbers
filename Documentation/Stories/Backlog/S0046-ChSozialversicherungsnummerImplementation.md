# S0046: Implement ChSozialversicherungsnummer Type for Swiss Social Security Number

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 11 points  
**Related:** S0038 (SePersonnummerTypeHierarchy), S0043 (ItCodiceFiscaleImplementation), S0044 (DeStuerIdImplementation), S0045 (LuNationalIdImplementation), S0019-S0027 (Union migrations), E01-UnionMigration

---

## Overview

Implement a new strongly-typed business object representing a Swiss Sozialversicherungsnummer (Social Security Number, also known as Neue AHV Nummer or New AVS Number). The Sozialversicherungsnummer is a 13-digit numeric code issued by the Swiss State Secretariat for Economic Affairs (SECO) and the Central Compensation Office (ZAS) to uniquely identify individuals for social security, tax, and administrative purposes throughout Switzerland.

As of 2008, the modern Swiss Sozialversicherungsnummer uses a randomized format (unlike earlier date-based systems) with format 576.XXXX.XXXX.XY, where 576 is the ISO 3166-1 numeric code for Switzerland, XXXX.XXXX.X represents random digits for individual identification, and Y is an EAN-13 check digit. The type will follow the union-based validation pattern established in S0019–S0027, with comprehensive validation of the numeric format and EAN-13 check digit algorithm.

Implement as:
```csharp
public record ChSozialversicherungsnummer : ChSozialversicherungsnummerBase
{
    // Single unrestricted type accepting all valid Swiss Social Security Number values
    // Does not encode any personal data (date, gender, etc.)
    // Uses random digit sequence with EAN-13 check digit
    // No subtype hierarchy
}
```

---

## Business Value

* **Social Security Compliance**: Supports Swiss social security identification and administrative processing, essential for businesses operating in or with Swiss entities.
* **Tax Integration**: Aligns with Swiss tax systems and administrative identification, critical for payroll and benefits processing.
* **Data Integrity**: Validates numeric format and EAN-13 check digit, ensuring data accuracy and integrity.
* **International Expansion**: Enables the library to serve European business domains across Nordic, Southern, Central, and Western European countries.
* **Consistency**: Follows proven patterns from S0038, S0043, S0044, S0045, and earlier union migrations, maintaining architectural coherence.
* **Type Safety**: Compile-time enforcement via strongly-typed business object; eliminates string-based social security number processing.
* **Validation Robustness**: Encapsulates Swiss social security identifier validation rules (format, numeric structure, EAN-13 check digit).
* **Developer Clarity**: Self-documenting API; developers immediately recognize this as a Swiss social security identifier.

---

## Requirements

### Functional Requirements

#### 1. Swiss Sozialversicherungsnummer Format and Structure

The Swiss Sozialversicherungsnummer (Neue AHV Nummer, as of 2008) consists of 13 digits formatted as:

```
576.XXXX.XXXX.XY

576      = ISO 3166-1 numeric country code for Switzerland
XXXX     = Random digits (4 digits, first group)
XXXX     = Random digits (4 digits, second group)
X        = Random digit (1 digit, final individual identifier)
Y        = EAN-13 check digit (calculated over first 12 digits: 576XXXXXXXXX)
```

**Key Characteristics:**

- **No Personal Data Encoding**: Unlike earlier date-based systems, the modern format does not encode date of birth, gender, or any other personal information
- **Randomized Individual Identifiers**: Characters after 576 are random digits assigned sequentially to individuals
- **Country Code Prefix**: All modern Swiss identifiers begin with 576 (ISO 3166-1 numeric code for Switzerland)
- **Single Check Digit**: Uses EAN-13 check digit algorithm (industry-standard barcode check digit)
- **Display Format with Separators**: Often displayed with separators (576.XXXX.XXXX.XY) for readability
- **Internal Storage Format**: Stored internally as 13 contiguous digits (5760000000000 to 5769999999999 range, with valid EAN-13 check digits)

**Detailed Breakdown:**

##### 1.1 Overall Format

- **Total Length**: 13 digits (always)
- **Valid Range**: 5760000000000 to 5769999999999 (Switzerland country code 576 with any 9 random digits and valid EAN-13 check digit)
- **Storage Format**: Strictly numeric, no separators (5760000000000)
- **Display Format**: May include separators (576.0000.0000.0Y) for readability
- **Structure**: 576 + 9 random digits + 1 EAN-13 check digit

##### 1.2 Country Code Prefix

**Prefix (Characters 1–3)**:
- Always **576** (ISO 3166-1 numeric code for Switzerland)
- No variation; essential identifier that the number belongs to Swiss social security system
- First 3 digits of the 13-digit format

##### 1.3 Random Individual Identifier

**Random Digits (Characters 4–12)**:
- Nine random digits (0–9)
- No inherent structure or pattern
- Assigned sequentially as individuals register with Swiss social security system
- Provides a large namespace (10^9 possible values, sufficient for Swiss population and foreign workers)
- Cannot be reverse-engineered to determine personal information

**Range**: 000000000 to 999999999 (represented as 9 digits in positions 4–12)

##### 1.4 EAN-13 Check Digit

The Swiss Sozialversicherungsnummer uses the **EAN-13 check digit algorithm** calculated over the first 12 digits (576 + 9 random digits):

###### EAN-13 Algorithm

1. **Extract the first 12 digits** (576XXXXXXXXX)
2. **Assign alternating weights**: 1, 3, 1, 3, 1, 3, 1, 3, 1, 3, 1, 3 (starting from the left with weight 1)
3. **Calculate weighted sum**: Multiply each digit by its weight and sum all products
4. **Calculate check digit**: check_digit = (10 - (sum mod 10)) mod 10

**Detailed Example**:
```
Swiss ID (first 12 digits): 576 0000 0000 0
Digits:      5 7 6 0 0 0 0 0 0 0 0 0
Weights:     1 3 1 3 1 3 1 3 1 3 1 3
Products:    5 21 6 0 0 0 0 0 0 0 0 0
Sum: 5 + 21 + 6 + 0 + 0 + 0 + 0 + 0 + 0 + 0 + 0 + 0 = 32

Check digit: (10 - (32 mod 10)) mod 10 = (10 - 2) mod 10 = 8

Complete ID: 5760000000008
```

**Formula**:
```
weighted_sum = Σ(digit[i] × weight[i]) for i = 1 to 12, where weight[i] = 1 if i is odd, 3 if i is even
check_digit = (10 - (weighted_sum mod 10)) mod 10
```

#### 2. ChSozialversicherungsnummerBase Type

Create an abstract record base class that encapsulates common functionality:

```csharp
public abstract record ChSozialversicherungsnummerBase
{
    // Common constants and static properties
    // Common validation logic and helper methods
    // Shared properties: Value
    // Union-based ValidationError and ValidationResult
    // NOTE: No DateOfBirth or Gender properties (no personal data encoded)
}
```

**Responsibilities:**

- Hold the internal 13-digit string representation
- Provide `Value` property (read-only, returns full 13-digit string)
- Provide static `Validate()` method for validation-only scenarios (returns `ValidationResult` union)
- Provide static `Create()` method for optional creation (returns `CreateResult` union)
- Encapsulate EAN-13 check digit validation logic
- Support implicit/explicit conversions to/from `String`
- Implement value semantics (equality, hash code based on the 13-digit value)
- Provide custom JSON converter for serialization/deserialization

#### 3. ChSozialversicherungsnummer Type

Create a public record type derived from `ChSozialversicherungsnummerBase`:

```csharp
public record ChSozialversicherungsnummer : ChSozialversicherungsnummerBase
{
    // No additional functionality beyond base
    // Single unrestricted type for all valid Swiss Social Security Numbers
}
```

**Constructor**:

```csharp
public ChSozialversicherungsnummer(String value)
{
    // Validate using validation logic from base
    // Throw InvalidOperationException if validation fails
    // (or similar exception strategy used by other types in the library)
}
```

**Factory Methods**:

```csharp
public static ChSozialversicherungsnummer? Create(String? value);
public static ValidationResult Validate(String? value);
```

#### 4. Validation Rules

When creating a new `ChSozialversicherungsnummer`, the following validation rules are applied:

1. **Null/Empty**: Value may not be null, empty, or all whitespace characters.
2. **Length**: Value must be exactly 13 characters.
3. **Character Set**: All characters must be ASCII digits (0–9).
4. **Format**:
   - Characters 1–3: Digits "576" (Swiss country code prefix; must be exactly 576)
   - Characters 4–12: Digits (9 random individual identifier digits)
   - Character 13: Digit (EAN-13 check digit)
5. **Country Code Prefix**: Characters 1–3 must be exactly "576" (ISO 3166-1 numeric code for Switzerland).
6. **Random Digits**: Characters 4–12 are not validated for any pattern; any digit 0–9 is acceptable.
7. **EAN-13 Check Digit**: Character 13 must be a valid EAN-13 check digit calculated from the first 12 digits (576 + 9 random digits).

#### 5. Error Handling

Define a discriminated union for validation errors:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCountryCode,
   InvalidCheckDigit
) { }

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCountryCode,
   InvalidCheckDigit
) { }
```

**Error Types**:
- **EmptyValue**: Input is null, empty, or all whitespace.
- **InvalidLength**: Input length is not 13 characters.
- **InvalidCharacter**: Input contains non-digit characters.
- **InvalidCountryCode**: First 3 digits are not "576" (Swiss country code).
- **InvalidCheckDigit**: EAN-13 check digit (character 13) does not match the calculated value.

---

## Acceptance Criteria

1. ✓ `ChSozialversicherungsnummerBase` abstract base class implemented with union-based validation.
2. ✓ `ChSozialversicherungsnummer` type created; constructor and `Create()` method work correctly.
3. ✓ `Validate()` static method returns appropriate `ValidationResult` for all error conditions.
4. ✓ All validation rules (null/empty, length, format, country code prefix, EAN-13 check digit) are enforced.
5. ✓ Country code prefix (characters 1–3) must be exactly "576" (Swiss identifier).
6. ✓ Random identifier digits (characters 4–12) are accepted for any digit value 0–9 (no pattern validation).
7. ✓ EAN-13 check digit (character 13) is correctly calculated and validated from first 12 digits.
8. ✓ Implicit and explicit conversion operators work correctly.
9. ✓ JSON serialization and deserialization work correctly (round-trip).
10. ✓ Unit tests achieve >95% code coverage and cover:
    - Valid values with various random digit combinations
    - Invalid null/empty/whitespace
    - Invalid lengths
    - Invalid characters
    - Invalid country code (not "576")
    - Invalid EAN-13 check digits
    - Boundary cases (all zeros except check digit, all nines, etc.)
    - JSON round-trips
    - Null handling in JSON
    - Error propagation in JSON deserialization
11. ✓ Reference documentation (`ChSozialversicherungsnummer.md`) created with examples and validation rules.

---

## Implementation Notes

### EAN-13 Check Digit Algorithm

The EAN-13 algorithm is a standard barcode check digit used in retail and many identification systems:

1. **Extract the first 12 digits** (576XXXXXXXXX)
2. **Assign alternating weights**: 1, 3, 1, 3, 1, 3, 1, 3, 1, 3, 1, 3 (starting from the left)
3. **Calculate weighted sum**: weighted_sum = Σ(digit[i] × weight[i])
4. **Calculate check digit**: check_digit = (10 - (weighted_sum mod 10)) mod 10

**Example Implementation**:
```
Swiss ID (first 12 digits): 576 0000 0000 0
Digits:      5 7 6 0 0 0 0 0 0 0 0 0 (positions 1-12)
Weights:     1 3 1 3 1 3 1 3 1 3 1 3
Products:    5 21 6 0 0 0 0 0 0 0 0 0
Sum: 5 + 21 + 6 = 32

Check digit: (10 - (32 mod 10)) mod 10 = (10 - 2) mod 10 = 8
Complete ID: 5760000000008
```

### Modern Format (Post-2008)

The Swiss Sozialversicherungsnummer was redesigned in 2008 to address limitations of the previous date-based system:

**Previous System (Before 2008)**:
- Encoded date of birth and gender (similar to Nordic identifiers)
- Limited namespace for individuals
- Personal data embedded in the identifier

**Modern System (2008 and Later)**:
- Random individual identifier digits
- No personal data encoding
- Significantly larger namespace
- Improved privacy (no embedded data)
- All Swiss Social Security identifiers now use format 576.XXXX.XXXX.XY

### No Personal Data Extraction

Unlike Luxembourg (S0045) or Nordic types (S0038, S0039):
- **No DateOfBirth property**: Modern format does not encode dates
- **No Gender property**: Modern format does not encode gender
- **No sequence-based extraction**: Format is purely random for individual identification
- Simple identifier-only type with minimal interpretation

### Display Format vs. Storage Format

The Swiss Sozialversicherungsnummer may be:
- **Displayed with separators**: 576.XXXX.XXXX.XY (for readability and form presentation)
- **Stored internally**: 5760000000008 (13 contiguous digits for validation and processing)

**Implementation Consideration**: The type should accept and normalize both formats:
- Input "576.1234.5678.9" should normalize to "5761234567899" (or similar, based on check digit)
- Output `Value` property returns canonical 13-digit format
- Display formatting (with separators) may be handled by ToString() override or format specifiers

### Swiss Social Security System Context

The Swiss social security system includes:
- **AHV** (Alters- und Hinterlassenenversicherung / Old-Age and Survivors' Insurance)
- **IV** (Invalidenversicherung / Disability Insurance)
- **EL** (Ergänzungsleistungen / Supplementary Benefits)
- **FAK** (Familienausgleichskassen / Family Compensation Funds)
- **ALV** (Arbeitslosenversicherung / Unemployment Insurance)

The Sozialversicherungsnummer serves as the unified identifier across these social insurance branches.

---

## Deliverables

1. **Source Code**:
   - `src/KfAccountNumbers/Governmental/Europe/ChSozialversicherungsnummerBase.cs` (abstract base class)
   - `src/KfAccountNumbers/Governmental/Europe/ChSozialversicherungsnummer.cs` (main type and JSON converter)

2. **Unit Tests**:
   - `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/ChSozialversicherungsnummerTests.cs` (comprehensive test suite)

3. **Documentation**:
   - `Documentation/Reference/National/Europe/ChSozialversicherungsnummer.md` (API reference with examples)

4. **Definition of Done**:
   - All acceptance criteria met
   - Unit tests pass with >95% code coverage
   - Code review approval (verify supplementary digit algorithm against authoritative sources)
   - Integration with existing library (registered in any identifier registry/factory if applicable)
   - Documentation complete and reviewed

---

## Assumptions & Constraints

- **Naming Convention**: Type name follows pattern `ChSozialversicherungsnummer` (country code "Ch" for Switzerland, type name "Sozialversicherungsnummer" for social security number).
- **No External Dependencies**: Implementation uses only .NET standard library and existing library abstractions.
- **Numeric Format Only**: Swiss Social Security Number is strictly numeric; no letters or special characters in canonical storage format.
- **Country Code Prefix**: All valid modern Swiss identifiers begin with "576" (ISO 3166-1 numeric code).
- **No Personal Data**: Modern format (since 2008) does not encode date of birth, gender, or any other personal information.
- **Random Individual Identifiers**: Characters 4–12 are random digits with no pattern validation.
- **Single Type, No Hierarchy**: No subtype discrimination required; single monolithic type.
- **EAN-13 Check Digit Algorithm**: Well-established industry standard; implementation uses standard algorithm.
- **Display Format Normalization**: Type should accept separators in input (576.XXXX.XXXX.XY) and normalize to canonical 13-digit format (5760000000000 range).
- **No Century Derivation**: No year parsing required (modern format has no date encoding).

---

## References

- Swiss State Secretariat for Economic Affairs (SECO): [Social Security Identification Number (AHV-Nr)](https://www.seco.admin.ch/)
- Central Compensation Office (ZAS): [Neue AHV Nummer](https://www.zas.admin.ch/)
- ISO 3166-1: [Country Codes](https://www.iso.org/iso-3166-1-numeric.html) (Switzerland = 576)
- EAN-13 Barcode Check Digit: [Industry Standard Documentation](https://en.wikipedia.org/wiki/International_Article_Number)
- S0045: Luxembourg National ID Implementation (similar format with check digit)
- S0044: German Steuer-IdNr Implementation (numeric check-digit pattern)
- S0043: Italian Codice Fiscale Implementation (single-type pattern)
- S0038: Swedish Personnummer Type Hierarchy (date-based pattern, for comparison)
- Union Migration Pattern: S0019–S0027

---

## Historical Context & Related Work

This story extends the library's coverage of European national identifiers. The Swiss Sozialversicherungsnummer represents the Alpine/Western region:

| Country | Type | Status | Story | Key Features |
|---------|------|--------|-------|--------------|
| Sweden | Personnummer / Samordningsnummer | Completed | S0038 | Date + Gender; Subtype hierarchy |
| Norway | Fødselsnummer / D-nummer / H-nummer | Backlog | S0039–S0041 | Date + Gender; Multiple categories |
| Italy | Codice Fiscale | Backlog | S0043 | Name/date encoding; Modulo-26 check |
| Germany | Steuer-IdNr | Backlog | S0044 | Sequential numeric; Weighted mod 11 |
| Luxembourg | National ID | Backlog | S0045 | Date + Gender; Luhn + Verhoeff |
| **Switzerland** | **Sozialversicherungsnummer** | **This Story** | **S0046** | **Random Identifier; EAN-13 Check** |

**Key Distinction**: Unlike previous identifiers, the Swiss Sozialversicherungsnummer is a **modern random identifier without any personal data encoding**, making it a simpler validation-only type focused on format and check digit.

Additional European identifiers that may be implemented in future stories:
- France: Numéro de sécurité sociale (Social Security Number) - with date of birth and gender encoding
- Spain: NIF (Número de Identidad Fiscal) - tax identifier
- Netherlands: BSN (Burgerservicenummer) - citizen service number with date encoding
- Belgium: Numéro de registre national - with date of birth and gender encoding
- Ireland: Personal Public Service Number (PPSN) - tax and social security identifier
- Austria: Sozialversicherungsnummer - similar structure to Swiss model
- Czech Republic: Rodné číslo - with date encoding
- Poland: PESEL (Powszechny Elektroniczny System Ewidencji Ludności) - with date encoding

---

## Notes for Implementation Team

1. **Algorithm Verification**: The EAN-13 check digit algorithm is well-established and widely implemented. Implementation can proceed directly without additional research.

2. **Format Normalization**: The type should gracefully handle both display format (with separators: 576.1234.5678.9) and canonical format (13 contiguous digits: 5761234567899). Consider a normalization step in the constructor that:
   - Removes all non-digit characters
   - Validates length and format
   - Recalculates check digit if needed

3. **Simplicity**: This type is significantly simpler than date-based types (Luxembourg S0045, Nordic S0038–S0041) because:
   - No date parsing required
   - No complex validation logic beyond check digit
   - No personal data extraction
   - Purely format and check-digit validation

4. **Pattern Consistency**: Despite simplicity, maintain consistency with existing library types:
   - Abstract base class (`ChSozialversicherungsnummerBase`)
   - Union-based validation
   - JSON converter
   - Implicit/explicit conversion operators

5. **Testing Strategy**: Use known valid Swiss Social Security Numbers if available from public test data; alternatively, construct test data by:
   - Using 576 prefix
   - Generating random 9-digit sequences
   - Calculating EAN-13 check digit for each

6. **Integration Considerations**: If the library maintains an identifier factory or registry, register `ChSozialversicherungsnummer` alongside other European identifiers.

7. **Privacy Implications**: Document in reference material that the modern Swiss Sozialversicherungsnummer does **not** encode personal data, improving privacy compared to previous systems. This is a design advantage compared to Nordic and other date-based identifiers.

```
