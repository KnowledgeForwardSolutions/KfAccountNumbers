# S0008-SePersonnummer: Swedish Personnummer (Personal Identity Number)

## As a
Developer integrating Swedish identity validation into my application

## I want
A strongly typed business object for Swedish Personnummer (personal identity number)

## So that
I can validate, store, and format Swedish personal identity numbers with compile-time type safety and runtime validation

## Background

The Swedish Personnummer is a 10 or 12-digit number assigned to Swedish residents for identification purposes. It follows the format YYMMDD-XXXX or YYYYMMDD-XXXX where:

- **YYMMDD** or **YYYYMMDD**: Date of birth (year, month, day)
- **-**: Separator (dash or plus sign for persons over 100 years old)
- **XXX**: Birth number (odd for males, even for females)
- **C**: Luhn check digit

The personnummer can be represented in two formats:
1. **10-digit format**: YYMMDD-XXXX (e.g., 900101-1234)
2. **12-digit format**: YYYYMMDD-XXXX (e.g., 19900101-1234)

For persons who are 100 years or older, the separator dash (-) is replaced with a plus sign (+).

### Validation Rules

A valid Swedish Personnummer must meet the following criteria:

1. Must be either 10 digits (plus separator) or 12 digits (plus separator) in length
2. The date portion must represent a valid calendar date
3. Must contain a valid separator character (dash or plus) at the correct position
4. The separator character may be a dash (-) or plus (+) only, and must not be a decimal digit (0-9)
5. All non-separator characters must be ASCII digits (0-9)
6. The last digit must be a valid Luhn check digit calculated from the preceding 9 digits
7. For 12-digit format, century must be 19 or 20
8. The birth number (XXX) must be within valid range (001-999)

### Formatting

Personnummer are commonly formatted with either a dash (-) or plus (+) separator:
- Standard format: 900101-1234 or 19900101-1234
- Century marker: A plus (+) indicates the person is 100+ years old

## Acceptance Criteria

### 1. Namespace and Type Definition

**Given** the KfAccountNumbers library structure  
**When** implementing the Swedish Personnummer  
**Then** the business object shall:
- Be defined in the `KfAccountNumbers.Governmental.Europe` namespace
- Be named `SePersonnummer`
- Be implemented as a C# `record` type
- Follow the same conventions as `CaSocialInsuranceNumber` and other governmental ID business objects

### 2. Value Property

**Given** a valid `SePersonnummer` instance  
**When** accessing the `Value` property  
**Then** it shall:
- Return the validated 10 or 12-digit personnummer as a `String`
- Be defined as `public String Value { get; init; }`
- Store the personnummer in the format it was originally provided (10 or 12 digits)

### 3. Constructor

**Given** a personnummer string representation  
**When** creating a new `SePersonnummer` instance using the constructor  
**Then** it shall:
- Accept a `String?` parameter for the personnummer value
- Validate the input using the `Validate` method
- Throw `KfValidationException<SePersonnummerValidationResult>` if validation fails, with the exception containing the `SePersonnummerValidationResult`
- Store the unformatted personnummer value in the `Value` property
- Include comprehensive XML documentation

**Constructor signature:**
`public SePersonnummer(String? personnummer)`


### 4. Create Method

**Given** a personnummer string representation  
**When** calling the static `Create` method  
**Then** it shall:
- Accept a `String?` parameter for the personnummer value
- Return a `CreateResult<SePersonnummer, SePersonnummerValidationResult>`
- Return a successful result containing the `SePersonnummer` instance if validation passes
- Return a failed result containing the `SePersonnummerValidationResult` if validation fails
- Not throw exceptions for invalid personnummer values (only for invalid separator parameter)
- Include comprehensive XML documentation

**Method signature:**
`public static CreateResult<SePersonnummer, SePersonnummerValidationResult> Create( String? personnummer)`


### 5. Validate Method

**Given** a personnummer string representation  
**When** calling the static `Validate` method  
**Then** it shall:
- Accept a `String?` parameter for the personnummer value
- Return a `SePersonnummerValidationResult` enumeration value
- Return `ValidationPassed` if the personnummer is valid
- Return the specific validation error encountered if invalid
- Validate in the following order:
  1. Null, empty, or whitespace
  2. Length (11 or 13 characters formatted, 10 or 12 unformatted)
  3. Embedded separator character validity and position
  4. All characters are ASCII digits (excluding separator position)
  5. Date portion represents a valid calendar date
  6. Century validity (for 12-digit format)
  7. Birth number range (001-999)
  8. Luhn check digit
- Include comprehensive XML documentation

**Method signature:**
`public static SePersonnummerValidationResult Validate( String? personnummer)`


### 6. Implicit Conversion to String

**Given** a `SePersonnummer` instance  
**When** implicitly converting to `String`  
**Then** it shall:
- Return the `Value` property (unformatted personnummer)
- String.Empty if the `SePersonnummer` instance is `null`

**Operator signature:**
`public static implicit operator String(SePersonnummer personnummer)`


### 7. Explicit Conversion from String

**Given** a personnummer string representation  
**When** explicitly converting from `String` to `SePersonnummer`  
**Then** it shall:
- Behave identically to calling the constructor
- Throw the same exceptions as the constructor

**Operator signature:**
`public static explicit operator SePersonnummer(String? personnummer)`

### 8. SePersonnummerValidationResult Enumeration

**Given** the need to communicate validation errors  
**Then** a `SePersonnummerValidationResult` enumeration shall be defined with the following members:
```
public enum SePersonnummerValidationResult { 
  /// <summary> 
  ///   The value does not contain any validation errors. 
  /// </summary> 
  ValidationPassed = 1,
  
  /// <summary> 
  ///   Personnummer value is <see langword="null"/>, <see cref="String.Empty"/> 
  ///   or all whitespace characters. 
  /// </summary> 
  Empty,
  
  /// <summary> 
  ///   Personnummer value has incorrect length. Must be either 11 characters for short format, 
  ///   or 13 characters for long format. 
  /// </summary> 
  InvalidLength,
  
  /// <summary> 
  ///   Personnummer value contains an unexpected character in a separator 
  ///   character location. 
  /// </summary> 
  /// <remarks> 
  ///   A formatted personnummer should have a valid separator character (dash 
  ///   or plus) at position 6 (for 10-digit format) or position 8 (for 12-digit 
  ///   format). The separator character may not be a decimal digit (0-9). 
  /// </remarks> 
  InvalidSeparatorEncountered,
  
  /// <summary> 
  ///   Personnummer value contains a non-digit character in a position that 
  ///   should contain a digit. 
  /// </summary> 
  InvalidCharacterEncountered,
  
  /// <summary> 
  ///   The date portion of the personnummer does not represent a valid 
  ///   calendar date. 
  /// </summary> 
  InvalidDate,
  
  /// <summary> 
  ///   For 12-digit format, the century must be 19 or 20. 
  /// </summary> 
  InvalidCentury,
  
  /// <summary> 
  ///   The birth number (positions 7-9 in 10-digit format or 9-11 in 12-digit 
  ///   format) must be in the range 001-999. 
  /// </summary> 
  InvalidBirthNumber,
  
  /// <summary> 
  ///   Personnummer check digit is invalid per Luhn algorithm validation. 
  /// </summary> 
  InvalidCheckDigit, 
}
```

### 9. XML Documentation

**Given** all public members of `SePersonnummer`  
**Then** they shall include:
- Comprehensive `<summary>` tags
- `<param>` tags for all parameters
- `<returns>` tags for all return values
- `<exception>` tags for all thrown exceptions
- `<remarks>` sections where appropriate explaining Swedish personnummer rules and formatting
- `<example>` tags demonstrating usage (optional but recommended)

### 10. Unit Tests

**Given** the `SePersonnummer` implementation  
**Then** comprehensive unit tests shall be created that:
- Test all validation scenarios in `SePersonnummerValidationResult`
- Test constructor success and failure cases
- Test `Create` method success and failure cases
- Test `Validate` method with all possible validation results
- Test implicit conversions to and from `String`
- Test `Format` method with various masks
- Test edge cases (century boundaries, leap years, separator variations)
- Test both 10-digit and 12-digit formats
- Test plus (+) separator for persons over 100 years old
- Follow naming conventions of existing test classes

## Technical Notes

1. **Date Validation**: The implementation should validate that YYMMDD or YYYYMMDD represents a valid calendar date, including leap year considerations.

2. **Century Handling**: For 10-digit format (YY), the century is presumed to be 1900 (confirm this assumption - it is based upon the fact that the 12 digit format was introduced in 1997, presumably as part of Y2K preparations). Alternatively, the 12-digit format should be preferred when century is ambiguous.

3. **Separator Logic**: The separator can be either dash (-) for persons under 100 or plus (+) for persons 100 years or older. The validation should accept both.

4. **Luhn Algorithm**: Use the existing `CheckDigitAlgorithms.Luhn.Validate` method from the CheckDigits.Net library.

5. **Birth Number Range**: The birth number (XXX) typically ranges from 001 to 999, where odd numbers are assigned to males and even to females.

6. **Coordination Numbers**: Consider whether to support Swedish coordination numbers (samordningsnummer), which use dates 61-91 for day portion. This may require additional validation result values if supported.

## Definition of Done

- [ ] `SePersonnummer` record class implemented in `KfAccountNumbers.Governmental.Europe` namespace
- [ ] `SePersonnummerValidationResult` enumeration implemented
- [ ] `InvalidSePersonnummerException` class implemented
- [ ] All public members include comprehensive XML documentation
- [ ] Constructor validates and throws appropriate exceptions
- [ ] `Create` method returns `CreateResult` without throwing exceptions
- [ ] `Validate` method returns appropriate validation result
- [ ] Implicit conversions to/from `String` implemented
- [ ] `Format` method implemented with mask support
- [ ] `ToString` override implemented
- [ ] All constants defined
- [ ] Performance optimizations using spans and pooled arrays
- [ ] Unit tests achieve >95% code coverage
- [ ] Unit tests cover all validation scenarios
- [ ] Unit tests cover both 10-digit and 12-digit formats
- [ ] Unit tests cover dash and plus separators
- [ ] Code follows existing conventions and style
- [ ] Code review completed
- [ ] Documentation updated (README, if applicable)

## References

- Swedish Tax Agency (Skatteverket) Personnummer specification
- ISO/IEC 7064 MOD 10 (Luhn algorithm)
- Existing `CaSocialInsuranceNumber` implementation as reference
- CheckDigits.Net library for Luhn validation