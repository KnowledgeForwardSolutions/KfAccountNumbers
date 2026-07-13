# S0009-NoFodselsnummer: Norwegian Fødselsnummer (Birth Number)

## As a
Developer integrating Norwegian identity validation into my application

## I want
A strongly typed business object for Norwegian fødselsnummer (birth number)

## So that
I can validate, store, and format Norwegian personal identification numbers with compile-time type safety and runtime validation

## Background

The Norwegian fødselsnummer (birth number) is an 11-digit personal identification number issued to all persons registered in Norway. It follows the format DDMMYY-IIIKK where:

- **DDMMYY**: Date of birth (day, month, year - 2 digits each)
- **-**: Separator (optional in formatted representations, generally a space ' ', if used)
- **III**: Individual number (3 digits)
- **KK**: Control digits (2 check digits)

### Structure Details

**Date Portion (DDMMYY):**
- DD: Day of birth (01-31)
- MM: Month of birth (01-12)
- YY: Year of birth (last 2 digits)

**Individual Number (III):**
- The individual number (positions 7-9) is used to make the fødselsnummer unique and to identify the century of birth:
  - 000-499: Born in 1900-1999
  - 500-749: Born in 1854-1899 or 2000-2039
  - 750-999: Born in 1940-1999 (not assigned after 2007)
  - 900-999: Born in 1940-1999 or 2000-2039
- The 9th digit (last digit of individual number) indicates gender:
  - Odd numbers: Male
  - Even numbers: Female

**Control Digits (KK):**
- Two independent check digits calculated using weighted modulo 11 algorithms
- First control digit (K1) at position 10
- Second control digit (K2) at position 11

### D-Number (D-nummer)
Norway also uses D-numbers for foreign individuals who do not have a fødselsnummer. A D-number is identical to a fødselsnummer except that the first digit is increased by 4 (e.g., day 01 becomes 41, day 15 becomes 55). This story should support validation of D-numbers as well.

### Validation Rules

A valid Norwegian fødselsnummer must meet the following criteria:

1. Must be exactly 11 digits in length (or 12 characters with separator)
2. All characters (excluding optional separator) must be ASCII digits (0-9)
3. The date portion must represent a valid calendar date (with adjustment for D-numbers)
4. The individual number must be within valid range (001-999)
5. The individual number must be consistent with the century indicated by the birth year
6. The first control digit (K1) must be correctly calculated using modulo 11 algorithm
7. The second control digit (K2) must be correctly calculated using modulo 11 algorithm
8. Must handle D-numbers (where day is 41-71 instead of 01-31)

### Formatting

Fødselsnummer are commonly formatted in two ways:
- Unformatted: 01010112345
- Formatted with dash: 010101-12345 (6 digits, dash, 5 digits)
- Formatted with space: 010101 12345 (6 digits, space, 5 digits)

## Acceptance Criteria

### 1. Namespace and Type Definition

**Given** the KfAccountNumbers library structure  
**When** implementing the Norwegian fødselsnummer  
**Then** the business object shall:
- Be defined in the `KfAccountNumbers.Governmental.Europe` namespace
- Be named `NoFodselsnummer`
- Be implemented as a C# `record` type
- Follow the same conventions as `CaSocialInsuranceNumber` and other governmental ID business objects
- Include JSON converter attribute `[JsonConverter(typeof(NoFodselsnummerJsonConverter))]`

### 2. Value Property

**Given** a valid `NoFodselsnummer` instance  
**When** accessing the `Value` property  
**Then** it shall:
- Return the unformatted 11-digit fødselsnummer as a `String`
- Not include any separator characters
- Be defined as `public String Value { get; init; }`

### 3. Constructor

**Given** a fødselsnummer string representation  
**When** creating a new `NoFodselsnummer` instance using the constructor  
**Then** it shall:
- Accept a `String?` parameter for the fødselsnummer value
- Validate the input using the `Validate` method
- Throw `KfValidationException<NoFodselsnummerValidationResult>` if validation fails, with the exception containing the `NoFodselsnummerValidationResult`
- Store the unformatted fødselsnummer value in the `Value` property
- Include comprehensive XML documentation

**Constructor signature:**
`public NoFodselsnummer(String? fodselsnummer)`


### 4. Create Method

**Given** a fødselsnummer string representation  
**When** calling the static `Create` method  
**Then** it shall:
- Accept a `String?` parameter for the fødselsnummer value
- Return a `CreateResult<NoFodselsnummer, NoFodselsnummerValidationResult>`
- Return a successful result containing the `NoFodselsnummer` instance if validation passes
- Return a failed result containing the `NoFodselsnummerValidationResult` if validation fails
- Not throw exceptions for invalid fødselsnummer values (only for invalid separator parameter)
- Include comprehensive XML documentation

**Method signature:**
`public static CreateResult<NoFodselsnummer, NoFodselsnummerValidationResult> Create( String? fodselsnummer)`


### 5. Validate Method

**Given** a fødselsnummer string representation  
**When** calling the static `Validate` method  
**Then** it shall:
- Accept a `String?` parameter for the fødselsnummer value
- Return a `NoFodselsnummerValidationResult` enumeration value
- Return `ValidationPassed` if the fødselsnummer is valid
- Return the specific validation error encountered if invalid
- Validate in the following order:
  1. Null, empty, or whitespace
  2. Length (11 unformatted or 12 formatted characters)
  3. Embedded separator character validity and position (if formatted)
  4. All characters are ASCII digits (excluding separator position)
  5. Date portion represents a valid calendar date (accounting for D-numbers)
  6. Individual number range (001-999)
  7. Century consistency with individual number
  8. First control digit (K1) using modulo 11 algorithm
  9. Second control digit (K2) using modulo 11 algorithm
- Include comprehensive XML documentation

**Method signature:**
`public static NoFodselsnummerValidationResult Validate( String? fodselsnummer)`


### 6. Implicit Conversion to String

**Given** a `NoFodselsnummer` instance  
**When** implicitly converting to `String`  
**Then** it shall:
- Return the `Value` property (unformatted fødselsnummer)
- Throw `ArgumentNullException` if the `NoFodselsnummer` instance is `null`

**Operator signature:**
`public static implicit operator String(NoFodselsnummer fodselsnummer)`


### 7. Implicit Conversion from String

**Given** a fødselsnummer string representation  
**When** implicitly converting from `String` to `NoFodselsnummer`  
**Then** it shall:
- Create a new `NoFodselsnummer` instance using the default separator (`-`)
- Behave identically to calling the constructor with default separator
- Throw the same exceptions as the constructor

**Operator signature:**
`public static implicit operator NoFodselsnummer(String? fodselsnummer)`


### 8. Format Method

**Given** a `NoFodselsnummer` instance  
**When** calling the `Format` method  
**Then** it shall:
- Accept a `String` parameter for the format mask
- Default mask shall be `"______ _____"` (6 digits, space, 5 digits)
- Return a formatted fødselsnummer string according to the mask
- Use the `FormatWithMask` extension method
- Throw `ArgumentNullException` if mask is `null`
- Throw `ArgumentException` if mask is empty or all whitespace
- Include comprehensive XML documentation

**Method signature:**
`public String Format(String mask = "______ _____")`


### 9. ToString Method

**Given** a `NoFodselsnummer` instance  
**When** calling `ToString()`  
**Then** it shall:
- Return the unformatted fødselsnummer value (same as `Value` property)
- Override the default `ToString()` method

### 10. IsDNumber Property

**Given** a `NoFodselsnummer` instance  
**When** accessing the `IsDNumber` property  
**Then** it shall:
- Return `true` if the fødselsnummer is a D-number (first digit is 4-7)
- Return `false` if the fødselsnummer is a standard fødselsnummer (first digit is 0-3)
- Be defined as a computed property

**Property signature:**
`public Boolean IsDNumber => Value[0] >= '4' && Value[0] <= '7';`

### 11. Gender Property

**Given** a `NoFodselsnummer` instance  
**When** accessing the `Gender` property  
**Then** it shall:
- Return an enum value indicating gender based on the 9th digit
- Return `NoFodselsnummerGender.Male` if the 9th digit is odd
- Return `NoFodselsnummerGender.Female` if the 9th digit is even
- Include comprehensive XML documentation

### 12. DateOfBirth Property

**Given** a `NoFodselsnummer` instance  
**When** accessing the `DateOfBirth` property  
**Then** it shall:
- Return a `DateOnly` value representing the birth date
- Correctly interpret the century based on the individual number
- Adjust for D-numbers (subtract 40 from day if D-number)
- Include comprehensive XML documentation, including notation that the date could be different from the person's actual date of birth and the possible reasons for this happening

### 13. NoFodselsnummerValidationResult Enumeration

**Given** the need to communicate validation errors  
**Then** a `NoFodselsnummerValidationResult` enumeration shall be defined with the following members:
```csharp
public enum NoFodselsnummerValidationResult 
{ 
  /// <summary> 
  ///   The value does not contain any validation errors. 
  /// </summary> 
  ValidationPassed = 1,
  
  /// <summary> 
  ///   Fødselsnummer value is <see langword="null"/>, <see cref="String.Empty"/> 
  ///   or all whitespace characters. 
  /// </summary> 
  Empty,
  
  /// <summary> 
  ///   Fødselsnummer value has incorrect length. Must be either 11 characters 
  ///   (digits only) or 12 characters (11 digits with separator). 
  /// </summary> 
  InvalidLength,
  
  /// <summary> 
  ///   Fødselsnummer value contains an unexpected character in a separator 
  ///   character location. 
  /// </summary> 
  /// <remarks> 
  ///   A formatted fødselsnummer should have a valid separator character 
  ///   (typically dash or space) at position 6. The separator character may 
  ///   not be a decimal digit (0-9). 
  /// </remarks> 
  InvalidSeparatorEncountered,
  
  /// <summary> 
  ///   Fødselsnummer value contains a non-digit character in a position that 
  ///   should contain a digit. 
  /// </summary> 
  InvalidCharacterEncountered,
  
  /// <summary> 
  ///   The date portion of the fødselsnummer does not represent a valid 
  ///   calendar date (after adjusting for D-numbers). 
  /// </summary> 
  InvalidDate,
  
  /// <summary> 
  ///   The individual number (positions 7-9) must be in the range 000-999. 
  ///   Note: In practice, 000 is not used, so valid range is 001-999. 
  /// </summary> 
  InvalidIndividualNumber,
  
  /// <summary> 
  ///   The individual number is inconsistent with the century indicated by 
  ///   the birth year. 
  /// </summary> 
  /// <remarks> 
  ///   <para> 
  ///      The individual number ranges determine the century: 
  ///      - 000-499: Born in 1900-1999 
  ///      - 500-749: Born in 1854-1899 or 2000-2039 
  ///      - 750-999: Born in 1940-1999 (not assigned after 2007) 
  ///      - 900-999: Born in 1940-1999 or 2000-2039 
  ///   </para> 
  /// </remarks> 
  InvalidCenturyCombination,
  
  /// <summary> 
  ///   The first control digit (K1) at position 10 is invalid according to 
  ///   the weighted modulo 11 algorithm. 
  /// </summary> 
  InvalidFirstControlDigit,
  
  /// <summary> 
  ///   The second control digit (K2) at position 11 is invalid according to 
  ///   the weighted modulo 11 algorithm. 
  /// </summary> 
  InvalidSecondControlDigit, 
  }
```

### 14. XML Documentation

**Given** all public members of `NoFodselsnummer`  
**Then** they shall include:
- Comprehensive `<summary>` tags
- `<param>` tags for all parameters
- `<returns>` tags for all return values
- `<exception>` tags for all thrown exceptions
- `<remarks>` sections explaining Norwegian fødselsnummer rules, D-numbers, control digit algorithms
- `<example>` tags demonstrating usage (optional but recommended)

### 15. Control Digit Algorithm Implementation

**Given** the need to validate control digits  
**Then** the implementation shall:
- Implement private static methods for calculating K1 and K2
- Use the standard Norwegian modulo 11 algorithm with correct weights
- K1 weights: [3, 7, 6, 1, 8, 9, 4, 5, 2] for positions 1-9
- K2 weights: [5, 4, 3, 2, 7, 6, 5, 4, 3, 2] for positions 1-10
- Handle modulo result of 11 (invalid fødselsnummer, should fail validation)
- Handle modulo result of 10 (represented as 0 in the fødselsnummer)

### 16. Performance Considerations

**Given** the validation and parsing logic  
**Then** the implementation shall:
- Use `Span<char>` and `ReadOnlySpan<char>` where appropriate to minimize allocations
- Use `stackalloc` for small temporary buffers (digit arrays, weight arrays)
- Consider using `ArrayPool<char>` for larger temporary buffers if needed
- Perform single-pass validation where possible
- Follow the performance patterns established in `CaSocialInsuranceNumber`
- Cache the separator offset and control digit positions as constants

### 17. Unit Tests

**Given** the `NoFodselsnummer` implementation  
**Then** comprehensive unit tests shall be created that:
- Test all validation scenarios in `NoFodselsnummerValidationResult`
- Test constructor success and failure cases
- Test `Create` method success and failure cases
- Test `Validate` method with all possible validation results
- Test implicit conversions to and from `String`
- Test `Format` method with various masks
- Test `IsDNumber` property for both standard and D-numbers
- Test `GetGender` method for both male and female cases
- Test `GetBirthDate` method including century interpretation
- Test edge cases (leap years, century boundaries, D-numbers)
- Test valid fødselsnummer from different time periods (1800s, 1900s, 2000s)
- Test both control digits with known valid/invalid examples
- Follow naming conventions of existing test classes
- Achieve >95% code coverage

## Technical Notes

1. **Control Digit Algorithms**: The Norwegian fødselsnummer uses two independent modulo 11 check digits with specific weight sequences. These must be implemented according to the official specification.

2. **Century Determination**: The century must be correctly determined based on the combination of the year digits (YY) and the individual number range. This requires careful logic to handle all valid combinations.

3. **D-Number Handling**: D-numbers add 40 to the day portion. The validation must recognize this pattern and adjust the date validation accordingly. For example, day 41 represents day 01 for a D-number.

4. **Date Validation**: Must validate that DDMMYY represents a valid calendar date, including proper handling of leap years. For D-numbers, subtract 40 from DD before date validation.

5. **Modulo 11 Edge Cases**: When the modulo 11 calculation results in 11, the fødselsnummer is considered invalid (these numbers are not issued). When it results in 10, the check digit is represented as 0.

6. **Individual Number Zero**: While the range 000-999 is technically possible, 000 is not used in practice. Consider whether to reject this explicitly or allow it for completeness.

7. **Historical Context**: Individual number ranges 750-999 (for 1940-1999) are no longer assigned after 2007, but existing fødselsnummer in this range remain valid.

8. **Performance**: Given the multiple validation steps and control digit calculations, optimize for single-pass validation where possible and use spans to avoid allocations.

## Definition of Done

- [ ] `NoFodselsnummer` record class implemented in `KfAccountNumbers.Governmental.Europe` namespace
- [ ] `NoFodselsnummerValidationResult` enumeration implemented
- [ ] `Gender` enumeration implemented
- [ ] `InvalidNoFodselsnummerException` class implemented
- [ ] `NoFodselsnummerJsonConverter` class implemented
- [ ] All public members include comprehensive XML documentation
- [ ] Constructor validates and throws appropriate exceptions
- [ ] `Create` method returns `CreateResult` without throwing exceptions
- [ ] `Validate` method returns appropriate validation result
- [ ] Both control digit validation methods implemented correctly
- [ ] Implicit conversions to/from `String` implemented
- [ ] `Format` method implemented with mask support
- [ ] `ToString` override implemented
- [ ] `IsDNumber` property implemented
- [ ] `Gender` property implemented
- [ ] `DateOfBirth` property implemented with correct century logic
- [ ] Performance optimizations using spans and stackalloc
- [ ] Unit tests achieve >95% code coverage
- [ ] Unit tests cover all validation scenarios
- [ ] Unit tests cover both standard fødselsnummer and D-numbers
- [ ] Unit tests verify control digit calculations with known examples
- [ ] Unit tests verify century determination logic
- [ ] Unit tests verify date extraction and gender determination
- [ ] Code follows existing conventions and style
- [ ] Code review completed
- [ ] Documentation updated (README, if applicable)

## References

- Norwegian Tax Administration (Skatteetaten) fødselsnummer specification
- [Fødselsnummer on Wikipedia](https://en.wikipedia.org/wiki/National_identification_number#Norway)
- [D-number specification](https://www.skatteetaten.no/en/person/foreign/norwegian-identification-number/d-number/)
- Modulo 11 check digit algorithm for Norwegian personal numbers
- Existing `CaSocialInsuranceNumber` implementation as reference
- ISO/IEC 7064 check digit algorithms