# S0044: Implement DeSteuerId Type for German Tax Identification Number

**Status:** Backlog  
**Priority:** Medium  
**Effort:** 11 points  
**Related:** S0038 (SePersonnummerTypeHierarchy), S0043 (ItCodiceFiscaleImplementation), S0019-S0027 (Union migrations), E01-UnionMigration

---

## Overview

Implement a new strongly-typed business object representing a German Steuer-IdNr (Tax Identification Number, officially Steuerliche Identifikationsnummer or IdNr). The Steuer-IdNr is an 11-digit numeric code issued by the Bundeszentralamt für Steuern (German Federal Tax Office) to uniquely identify individuals for tax purposes within Germany.

Like the Italian Codice Fiscale (S0043), the German Steuer-IdNr is a single, monolithic identifier type with no subtype hierarchy. The type will follow the union-based validation pattern established in S0019–S0027, with comprehensive validation of the numeric format and the check digit algorithm.

Implement as:
```csharp
public record DeSteuerId : DeStuerIdBase
{
    // Single unrestricted type accepting all valid Steuer-IdNr values
    // No subtype hierarchy (unlike Swedish/Norwegian types)
}
```

---

## Business Value

* **Tax Compliance**: Supports German tax identification and administrative processing, essential for businesses operating in or with German entities.
* **Data Integrity**: Validates the numeric format and the check digit, ensuring data accuracy before storage or transmission.
* **International Expansion**: Enables the library to serve European business domains across Nordic, Southern, and Central European countries.
* **Consistency**: Follows proven patterns from S0038, S0043, and earlier union migrations, maintaining architectural coherence.
* **Type Safety**: Compile-time enforcement via strongly-typed business object; eliminates string-based identity processing.
* **Validation Robustness**: Encapsulates German tax identifier validation rules (format, check digit algorithm, leading zeros).
* **Developer Clarity**: Self-documenting API; developers immediately recognize this as a German tax identifier.

---

## Requirements

### Functional Requirements

#### 1. Steuer-IdNr Format and Structure

The German Steuer-IdNr (Tax Identification Number) consists of 11 digits:

```
08 152 034 261

08      = Sequential assignment digits (first two digits of unique sequence)
152     = Middle digits (continuation of unique sequence)
034     = Prefix/check area digits (part of sequence)
261     = Check digits (C1 and C2, plus one additional digit for total of 11)
```

**Detailed Breakdown:**

##### 1.1 Overall Format

- **Total Length**: 11 digits
- **Range**: 10,000,000,000 to 99,999,999,999
- **Leading Zero Handling**: Valid IDs may start with 0; represented as 11 digits with leading zeros if necessary
- **No Separators**: Format is strictly numeric with no hyphens, spaces, or other characters

##### 1.2 Sequential Structure

The 11 digits represent a sequential assignment number with an embedded check digit:

```
Position: 1  2  3  4  5  6  7  8  9  10 11
Content:  X₁ X₂ X₃ X₄ X₅ X₆ X₇ X₈ X₉ C₁ C₂
```

Where:
- **X₁–X₉**: Nine-digit sequential base number (001,000,000 to 999,999,999)
- **C₁–C₂**: Two-digit check digit pair calculated using weighted modulus algorithm

##### 1.3 Sequential Number Range

Valid sequential numbers are in the range **1,000,000 to 999,999,999** (9 digits, no leading zeros in the conceptual nine-digit number, but may be represented with leading zeros when constructing the full 11-digit form).

**Valid IDs**:
- 08152034261 (valid format, specific ID)
- 01234567891 (valid format with leading zeros in representation)

**Invalid IDs**:
- 00000000000 (all zeros)
- 12345678901 (valid format, but check digits must be validated)

##### 1.4 Check Digit Algorithm

The German Steuer-IdNr uses a **weighted modulus 11** check digit algorithm, similar to Norwegian identifiers but with different weights:

1. **Extract the first nine digits** (X₁–X₉).
2. **Apply weights** to each digit in reverse order:
   - Weight sequence: 10, 9, 8, 7, 6, 5, 4, 3, 2 (applied to positions 9, 8, 7, 6, 5, 4, 3, 2, 1 respectively)
   - For digit at position i (1-indexed), weight = (11 - i)
3. **Calculate products**: Multiply each digit by its weight.
4. **Sum the products**: Add all weighted products together.
5. **Calculate remainder**: remainder = sum mod 11
6. **First check digit (C₁)**: 11 - remainder (if remainder ≠ 0); 0 if remainder = 0
7. **Second check digit (C₂)**: Apply the same algorithm to the first 10 digits (X₁–X₉ + C₁)

**Detailed Example**:
```
Steuer-IdNr: 08 152 034 261

First nine digits: 0, 8, 1, 5, 2, 0, 3, 4 (positions 1-8, we'll recalculate for position 9)
Actually: 081520342 (positions 1-9)

Position: 1  2  3  4  5  6  7  8  9
Digit:    0  8  1  5  2  0  3  4  2
Weight:   10 9  8  7  6  5  4  3  2

Products: 0  72 8  35 12 0  12 12 4
Sum: 0 + 72 + 8 + 35 + 12 + 0 + 12 + 12 + 4 = 155

Remainder: 155 mod 11 = 1
C₁: 11 - 1 = 10 → This is invalid (must be single digit 0-9)
    When result is 10, use 0; when result is 11, use 1
    Actually, standard algorithm: if remainder = 0, C₁ = 0; else C₁ = 11 - remainder
    In this case, C₁ = 11 - 1 = 10, which exceeds 9, so special handling applies

For German Steuer-IdNr, the algorithm ensures C₁ is always 0-9 through modular arithmetic.
```

**Alternative (More Common) Algorithm Description**:
The check digit is calculated as follows:
1. Take the first 10 digits (X₁–X₉ + C₁).
2. Apply weights 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 to each digit.
3. Sum the weighted products.
4. The check digit is (11 - (sum mod 11)) mod 11.

(Implementation should verify against published test cases.)

#### 2. DeStuerIdBase Type

Create an abstract record base class that encapsulates common functionality:

```csharp
public abstract record DeStuerIdBase
{
    // Common constants and static properties
    // Common validation logic and helper methods
    // Shared properties: Value
    // Union-based ValidationError and ValidationResult
}
```

**Responsibilities:**
- Store the normalized identifier value (11 digits as a string, with leading zeros if applicable).
- Implement validation logic for format and check digit.
- Define the common `Value` property.
- Provide shared validation methods and error handling via discriminated unions.

**Design Constraints**:
- Should be `abstract`, not instantiable directly.
- Use the union-based validation pattern from S0019–S0027.
- Must be `record` type for value semantics and immutability.
- Should reside in `DeStuerIdBase.cs` (new file).

**German-Specific Logic**:
- Weighted modulus 11 check digit calculation (similar to Norwegian, but with different weight sequence).
- Numeric-only validation.
- Leading zero preservation (11 digits always).
- No date of birth or gender extraction (unlike Swedish/Norwegian identifiers).

#### 3. DeSteuerId Type (Unrestricted)

Implement a single, unrestricted type that accepts all valid Steuer-IdNr values:

```csharp
public record DeSteuerId : DeStuerIdBase
{
    // Constructor: DeSteuerId(String? value)
    // Static method: Create(String? value) -> CreateResult<DeSteuerId, ValidationError>
    // Static method: Validate(String? value) -> ValidationResult

    // Properties:
    // - Value: String (11-digit normalized identifier, with leading zeros)

    // Methods:
    // - ToString(): String (returns Value)
    // - implicit operator String(DeSteuerId)
    // - explicit operator DeSteuerId(String?)

    // JSON Converter: DeStuerIdJsonConverter
}
```

**Behavior**:
- Constructor throws `UKfValidationException<ValidationError>` if value is invalid.
- `Create()` method returns `CreateResult<DeSteuerId, ValidationError>` for result-pattern usage.
- `Validate()` static method returns `ValidationResult` union (ValidValue or specific error).
- No subtype hierarchy; all valid Steuer-IdNr values are accepted equally.
- JSON serialization/deserialization supported via custom converter.
- Input may be provided as 11-digit string or as numeric value (with leading zeros normalized).

#### 4. Validation Rules

When creating a new `DeSteuerId`, the following validation rules are applied:

1. **Null/Empty**: Value may not be null, empty, or all whitespace characters.
2. **Length**: Value must be exactly 11 characters (digits, possibly with leading zeros).
3. **Character Set**: All characters must be ASCII digits (0–9).
4. **Numeric Range**: When interpreted as a number, must be in range 10,000,000,001 to 99,999,999,999 (i.e., not all zeros, not below 10 billion).
5. **Check Digit Validity**: The last two digits must form a valid check digit pair calculated from the first nine digits using the weighted modulus 11 algorithm.
6. **Leading Zero Handling**: Input with fewer than 11 characters should be zero-padded on the left to exactly 11 digits.

#### 5. Error Handling

Define a discriminated union for validation errors:

```csharp
public union ValidationError(
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigit
) { }

public union ValidationResult(
   ValidValue,
   EmptyValue,
   InvalidLength,
   InvalidCharacter,
   InvalidCheckDigit
) { }
```

**Error Types**:
- **EmptyValue**: Input is null, empty, or all whitespace.
- **InvalidLength**: Input length is not 11 characters (after normalization and zero-padding).
- **InvalidCharacter**: Input contains non-digit characters.
- **InvalidCheckDigit**: The check digit pair does not match the calculated value, or the numeric value is all zeros (which is invalid).

### Acceptance Criteria

1. ✓ `DeStuerIdBase` abstract base class implemented with union-based validation.
2. ✓ `DeSteuerId` type created; constructor and `Create()` method work correctly.
3. ✓ `Validate()` static method returns appropriate `ValidationResult` for all error conditions.
4. ✓ All validation rules (null/empty, length, numeric format, check digit) are enforced.
5. ✓ Leading zeros are preserved; 11-digit format is always maintained.
6. ✓ Check digit algorithm correctly validates known Steuer-IdNr values.
7. ✓ All zeros (00000000000) and values below 10,000,000,001 are rejected.
8. ✓ Implicit and explicit conversion operators work correctly.
9. ✓ JSON serialization and deserialization work correctly (round-trip, with leading zeros preserved).
10. ✓ Unit tests achieve >95% code coverage and cover:
    - Valid values (various numeric ranges, leading zeros)
    - Invalid null/empty/whitespace
    - Invalid lengths
    - Invalid characters
    - Invalid check digits
    - All zeros (00000000000)
    - Values below minimum range
    - JSON round-trips
    - Null handling in JSON
    - Error propagation in JSON deserialization
11. ✓ Reference documentation (`DeSteuerId.md`) created with examples and validation rules.

---

## Implementation Notes

### Check Digit Algorithm Verification

The weighted modulus 11 algorithm for German Steuer-IdNr differs from Norwegian (which uses two check digits calculated sequentially) and Swedish (which uses Luhn). The algorithm must be verified against published test cases from the Bundeszentralamt für Steuern or published references.

**Key Difference from Norwegian Weighted Modulus 11**:
- Norwegian uses sequential calculation (C1, then C2 based on C1)
- German uses a single pair of check digits (C1, C2) where both may be derived differently

Implementation should reference authoritative sources such as:
- German tax authority documentation
- Published test cases
- Academic or reference implementations

### Leading Zero Handling

Unlike Swedish and Norwegian identifiers which may have variable-length representations (6 vs. 8 digits for date of birth), the German Steuer-IdNr is always exactly 11 digits. The type should:
- Accept input with or without leading zeros
- Normalize to exactly 11 digits (zero-pad if necessary)
- Store and return the 11-digit form consistently
- Preserve leading zeros in JSON serialization

**Example**:
- Input: "8152034261" (10 digits) → Normalized: "08152034261" (11 digits)
- Input: "08152034261" (11 digits) → Normalized: "08152034261" (11 digits)
- Input: "008152034261" (12 digits) → Invalid (too long)

### No Demographic Information Extraction

Unlike Swedish (Personnummer) and Norwegian (Fødselsnummer) identifiers which encode date of birth and gender, the German Steuer-IdNr is purely sequential. The type:
- Does **not** extract date of birth
- Does **not** extract gender
- Does **not** extract any demographic information
- Provides only the `Value` property and validation

This simplifies the implementation significantly.

### Scope: Single Type, No Subtype Hierarchy

Like the Italian Codice Fiscale (S0043), the German Steuer-IdNr is a single identity category. There are no equivalent types like:
- Swedish: Personnummer vs. Samordningsnummer
- Norwegian: Fødselsnummer vs. D-nummer vs. H-nummer

Therefore:
- **No subtype hierarchy**
- **Single type**: `DeSteuerId`
- **No type discrimination logic**

---

## User Stories / Related Issues

- **Related Backlog Stories**: S0038 (Swedish hierarchy), S0043 (Italian Codice Fiscale), S0039 (Norwegian hierarchy)
- **Related Union Migrations**: S0019–S0027 (earlier union-pattern implementations)
- **Inspiration**: Follows proven patterns from European tax identifiers.

---

## Testing Strategy

### Unit Test Coverage

Create `DeStuerIdTests.cs` with comprehensive test coverage:

#### Constructor Tests
- Valid Steuer-IdNr values (various numeric values, with and without leading zeros)
- Invalid null/empty/whitespace input
- Invalid length (too short, too long)
- Invalid characters (non-digits)
- Invalid check digits
- All zeros (00000000000)
- Values below minimum range (1–9999999999)

#### Validate() Method Tests
- Same invalid scenarios as constructor (returning ValidationResult instead of throwing)

#### Create() Method Tests
- Same valid and invalid scenarios (returning CreateResult instead of throwing or returning ValidValue)

#### Property Tests
- `Value` property returns correctly formatted 11-digit string with leading zeros

#### Conversion Operator Tests
- Implicit conversion to `String`
- Explicit conversion from `String`
- Round-trip: `String` → `DeSteuerId` → `String`

#### Leading Zero Tests
- Input "8152034261" (10 digits) normalized to "08152034261"
- Input "08152034261" (11 digits) remains "08152034261"
- Leading zeros preserved in Value property
- Leading zeros preserved in JSON serialization

#### Equality and Hash Code Tests
- Value equality (two instances with same Steuer-IdNr are equal)
- Leading zero variants are equal ("8152034261" and "08152034261" represent the same ID)
- Different values are not equal
- Hash code consistency
- Reference equality vs. value equality

#### JSON Serialization Tests
- Round-trip serialization/deserialization
- Leading zeros preserved in JSON strings
- Null value serialization
- Complex object serialization (DeSteuerId as a property of another class)
- Deserialization error handling (invalid Steuer-IdNr in JSON throws validation exception)

#### Check Digit Tests
- Valid check digits for known Steuer-IdNr values (use published test cases)
- Invalid check digits are rejected
- Deterministic check digit calculation

### Test Data

Use published test cases from German tax authority sources if available, or construct known valid IDs based on the check digit algorithm. Example test data structure:

```csharp
public static TheoryData<String> ValidStuerIdValues =>
[
    "08152034261",      // Known valid ID
    "00000000191",      // Valid format with leading zeros
    "99999999958",      // Large valid ID
    // Additional known valid IDs
];

public static TheoryData<String> InvalidCheckDigitValues =>
[
    "08152034260",      // Last digit incorrect
    "08152034262",      // Last digit incorrect
    "08152034200",      // Check digits incorrect
];
```

---

## Deliverables

1. **Source Code**:
   - `src/KfAccountNumbers/Governmental/Europe/DeStuerIdBase.cs` (abstract base class)
   - `src/KfAccountNumbers/Governmental/Europe/DeSteuerId.cs` (main type and JSON converter)

2. **Unit Tests**:
   - `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/DeStuerIdTests.cs` (comprehensive test suite)

3. **Documentation**:
   - `Documentation/Reference/National/Europe/DeSteuerId.md` (API reference with examples)

4. **Definition of Done**:
   - All acceptance criteria met
   - Unit tests pass with >95% code coverage
   - Code review approval (verify check digit algorithm against authoritative sources)
   - Integration with existing library (registered in any identifier registry/factory if applicable)
   - Documentation complete and reviewed

---

## Assumptions & Constraints

- **Naming Convention**: Type name follows pattern `DeSteuerId` (country code "De" for Germany, type name "SteuerId" for tax identifier).
- **No External Dependencies**: Implementation uses only .NET standard library and existing library abstractions.
- **Numeric Format Only**: Steuer-IdNr is strictly numeric; no letters or special characters.
- **No Demographic Data**: Unlike Nordic identifiers, the Steuer-IdNr does not encode date of birth or gender; no extraction logic required.
- **Check Digit Algorithm**: Implementation should verify against published test cases; initial implementation may require verification spike if authoritative algorithm is not readily available.
- **Leading Zero Preservation**: The 11-digit format is maintained throughout; leading zeros are not stripped.
- **Range Validation**: All-zeros (00000000000) and values below 10,000,000,001 are invalid; implementation enforces this.

---

## References

- Bundeszentralamt für Steuern: [Steuerliche Identifikationsnummer](https://www.bzst.bund.de/)
- Wikipedia: [Steuerliche Identifikationsnummer](https://en.wikipedia.org/wiki/Steuerliche_Identifikationsnummer)
- S0043: Italian Codice Fiscale Implementation
- S0038: Swedish Personnummer Type Hierarchy
- S0039: Norwegian Fødselsnummer Type Hierarchy
- Union Migration Pattern: S0019–S0027

---

## Historical Context & Related Work

This story extends the library's coverage of European tax identifiers. Prior implementations include:

| Country | Type | Status | Story |
|---------|------|--------|-------|
| Sweden | Personnummer / Samordningsnummer | Completed | S0038 |
| Norway | Fødselsnummer / D-nummer / H-nummer | Backlog | S0039–S0041 |
| Italy | Codice Fiscale | Backlog | S0043 |
| Germany | Steuer-IdNr | **This Story** | S0044 |

Additional European identifiers that may be implemented in future stories:
- France: Numéro de sécurité sociale (Social Security Number)
- Spain: NIF (Número de Identidad Fiscal)
- Netherlands: BSN (Burgerservicenummer)
- Belgium: Numéro de registre national
- Ireland: Personal Public Service Number (PPSN)
- UK: National Insurance Number (already implemented as GbNationalInsuranceNumber)

---

## Notes for Implementation Team

1. **Algorithm Verification**: Before implementation begins, verify the check digit algorithm against published test cases. Consider opening a technical spike (E-number) if algorithm clarification is needed.

2. **Leading Zero Handling**: Ensure consistent handling of leading zeros throughout the API (JSON serialization, ToString, Value property, comparison).

3. **Similarity to Norwegian**: The German Steuer-IdNr validation is simpler than Norwegian identifiers because:
   - No date of birth or gender extraction
   - No category discrimination (single type, no subtype hierarchy)
   - Only numeric format (no need for separate masks like Nordic types)

4. **Future European Identifiers**: This implementation establishes another European identifier type, reinforcing the library's role as a comprehensive European business object library. Consider planning for French, Spanish, and other European identifiers.

