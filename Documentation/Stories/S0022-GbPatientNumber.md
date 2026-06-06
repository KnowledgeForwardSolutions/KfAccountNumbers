# S0022-GbPatientNumber Business Object

## As a
Developer building healthcare applications that operate across multiple UK regions

## I want
A unified strongly typed business object that can represent any UK public health service patient identifier (NHS Number, CHI Number, or H&C Number)

## So that
I can work with patient identifiers in a region-agnostic manner while preserving the ability to determine and convert to the specific regional type when needed

## Acceptance Criteria

### Structure and Validation
- [ ] The `GbPatientNumber` type represents a unified UK patient identifier
- [ ] Can represent any of the three UK healthcare identifiers:
  - **GbNhsNumber**: NHS Number for England, Wales, and Isle of Man
  - **GbChiNumber**: CHI Number for Scotland
  - **GbHcNumber**: H&C Number for Northern Ireland
- [ ] The number is a 10-digit identifier (with optional formatting)
- [ ] Constructor accepts string representation and throws `UKfValidationException<GbPatientNumber.ValidationError>` if invalid
- [ ] Static `Validate` method returns `GbPatientNumber.ValidationResult` union value
- [ ] Static `Create` method uses Result pattern returning `CreateResult<GbPatientNumber, GbPatientNumber.ValidationError>`

### Validation Rules
- [ ] Value may not be null, empty, or all whitespace
- [ ] Must be valid as at least one of: GbNhsNumber, GbChiNumber, or GbHcNumber (or a valid test number as allowed by GbNhsNumber and GbHcNumber)
- [ ] Validates using the most permissive rules that cover all three types
- [ ] Further validates date of birth if range indicates Scottish CHI number
- [ ] Length must be 10 digits or 11/12 characters with appropriate formatting
- [ ] All validation rules from constituent types apply as appropriate
- [ ] Check digit validation performed according to modulus 11 algorithm
- [ ] Date validation performed for CHI format (DDMMYY component)
- [ ] Test numbers in range 999 000 0000 to 999 999 9999 are valid (NHS/H&C test range)
- [ ] **CHI does not support test ranges** but valid CHI numbers are still accepted

### Identifier Type Detection
- [ ] `IdentifierType` property returns `GbPatientNumberIdentifierType` enum indicating the determined type
- [ ] Type determination based on range:
  - **CHI**: 010 000 000 to 311 299 999
  - **H&C**: 320 000 000 to 399 999 999
  - **NHS**: 400 000 000 to 499 999 999 or 600 000 000 to 799 999 999
  - **Test**: 900 000 000 to 999 999 999

### Format Support
- [ ] Accept unformatted: 1234567890
- [ ] Accept formatted with spaces: 123 456 7890
- [ ] `Format` method returns formatted version with optional mask parameter (default: "___ ___ ____")

### Properties
- [ ] `Value` property returns raw 10-character string (digits only, no spaces)
- [ ] `IdentifierType` property returns `GbPatientNumberIdentifierType` enum
- [ ] `Region` property returns string: "England/Wales/Isle of Man", "Scotland", or "Northern Ireland" based on type

### Conversion From Specific Types
- [ ] Implicit conversion from `GbNhsNumber` to `GbPatientNumber`
- [ ] Implicit conversion from `GbChiNumber` to `GbPatientNumber`
- [ ] Implicit conversion from `GbHcNumber` to `GbPatientNumber`
- [ ] Original type information preserved during conversion
- [ ] Conversion is lossless (all information retained)

### Conversion To Specific Types
- [ ] `ToGbNhsNumber()` method returns `Option<GbNhsNumber>`
  - Returns Some(GbNhsNumber) if IdentifierType is NHS
  - Returns None if IdentifierType is not NHS
- [ ] `ToGbChiNumber()` method returns `Option<GbChiNumber>`
  - Returns Some(GbChiNumber) if IdentifierType is CHI
  - Returns None if IdentifierType is not CHI
- [ ] `ToGbHcNumber()` method returns `Option<GbHcNumber>`
  - Returns Some(GbHcNumber) if IdentifierType is H&C
  - Returns None if IdentifierType is not H&C
- [ ] Use Option<T> pattern
- [ ] `ToGbNhsNumber()`, `ToGbChiNumber()` and `ToGbHcNumber()` return None if the value is a test number

### Type Determination Examples
```
NHS Number:    450 557 7104 → IdentifierType.Nhs
CHI Number:    010 190 1234 → IdentifierType.Chi
H&C Number:    320 123 4560 → IdentifierType.Hc
Test Number:   900 000 0000 → IdentifierType.Nhs (test range)
```

### Operators and Methods
- [ ] Implicit conversion from GbNhsNumber to GbPatientNumber
- [ ] Implicit conversion from GbChiNumber to GbPatientNumber
- [ ] Implicit conversion from GbHcNumber to GbPatientNumber
- [ ] Explicit conversion from string to GbPatientNumber
- [ ] Implicit conversion from GbPatientNumber to string
- [ ] `ToGbNhsNumber()` returns Option<GbNhsNumber>
- [ ] `ToGbChiNumber()` returns Option<GbChiNumber>
- [ ] `ToGbHcNumber()` returns Option<GbHcNumber>
- [ ] `ToString` returns the value as stored
- [ ] Proper equality implementation (format-insensitive)
- [ ] JSON serialization/deserialization support via `GbPatientNumberJsonConverter`

### JSON Serialization
- [ ] Custom JSON converter `GbPatientNumberJsonConverter`
  ```json
  {
    "value": "4505577104",
  }
  ```
- [ ] Handles null values gracefully
- [ ] Round-trip preserves value

### Test Coverage
- [ ] Valid NHS Numbers and conversion to GbPatientNumber
- [ ] Valid CHI Numbers and conversion to GbPatientNumber
- [ ] Valid H&C Numbers and conversion to GbPatientNumber
- [ ] Implicit conversions from specific types to GbPatientNumber
- [ ] ToGbNhsNumber() returning Some for NHS numbers, None for others
- [ ] ToGbChiNumber() returning Some for CHI numbers, None for others
- [ ] ToGbHcNumber() returning Some for H&C numbers, None for others
- [ ] IdentifierType property correctly identifying each type
- [ ] Test numbers (900 million, 999 million) for NHS/H&C
- [ ] CHI numbers with valid dates
- [ ] Format variations (spaces, dashes, unformatted)
- [ ] Equality across different format styles
- [ ] JSON serialization round-trip preserving type information
- [ ] Conversion operators
- [ ] Create method Result pattern
- [ ] Region property values
- [ ] All validation failure paths
- [ ] Edge cases for type determination

### Documentation
- [ ] XML documentation for all public members
- [ ] README.md section with:
  - Purpose: unified patient identifier across UK regions
  - Structure explanation (union of three types)
  - Conversion examples (from/to specific types)
  - Format examples for each type
  - Test number support (NHS/H&C only, not CHI)
  - Usage scenarios (when to use GbPatientNumber vs specific types)
  - Relationship to constituent types
  - Geographic coverage (all UK regions)
  - Validation rules (union of all three types)
  - JSON serialization format
  - Example values showing each type
  - When to use unified vs specific types (design guidance)

### Performance
- [ ] Use `ReadOnlySpan<Char>` for validation and parsing
- [ ] Efficient type determination (minimize redundant validation)
- [ ] Lazy evaluation of IdentifierType property if expensive
- [ ] Cache type determination result
- [ ] Minimal string allocations
- [ ] Efficient conversion operations

## Notes
- **Union Type Pattern**: GbPatientNumber is a discriminated union of three distinct types
- **Type Discrimination**: Automatically determines which specific type ranges of valid values for each system
- **Regional Coverage**:
  - NHS Number: England, Wales, Isle of Man
  - CHI Number: Scotland
  - H&C Number: Northern Ireland
- **Test Numbers**: Supported for NHS and H&C (000-009 million, 999 million ranges)
- **CHI Special Case**: CHI embeds date of birth and gender, no test ranges
- **Conversion Safety**: Use Option<T> pattern for safe conversions to specific types
- **Use Cases**:
  - Cross-border healthcare (patient moves between UK regions)
  - National health data aggregation
  - Systems that process data from multiple UK regions
  - APIs that accept patient IDs regardless of region
- **Design Trade-offs**:
  - Flexibility: Can represent any UK patient ID
  - Type safety: Can convert to specific type when needed
  - Validation: Must validate against multiple formats
  - Performance: Type determination adds overhead
- See previous stories: S0019-GbNhsNumber, S0020-GbHcNumber, S0021-GbChiNumber

## Technical Details
- Namespace: `KfAccountNumbers.Governmental.Europe`
- Files:
  - `src/KfAccountNumbers/Governmental/Europe/GbPatientNumber.cs`
  - `src/KfAccountNumbers/Governmental/Europe/GbPatientNumberIdentifierType.cs` (enum)
  - `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/GbPatientNumberTests.cs`
- JSON converter: `GbPatientNumberJsonConverter`
- Target: .NET 11, C# 15.0
- External library: CheckDigits.Net 3.1.0
- Pattern: Combines validation rules for GbNhsNumber, GbChiNumber, GbHcNumber
- Depends on: GbNhsNumber, GbChiNumber, GbHcNumber types
- ISO 3166-1 alpha-2 code: GB (Great Britain / UK)

## GbPatientNumberIdentifierType Enum
```csharp
public enum GbPatientNumberIdentifierType
{
    /// <summary>
    /// NHS Number for England, Wales, and Isle of Man
    /// </summary>
    Nhs,

    /// <summary>
    /// CHI (Community Health Index) Number for Scotland
    /// </summary>
    Chi,

    /// <summary>
    /// H&C (Health and Care) Number for Northern Ireland
    /// </summary>
    Hc,

    /// <summary>
    /// Test number
    /// </summary>
    Test
}
```

## Example Values

### NHS Number Examples
```csharp
var nhs = new GbNhsNumber("450 557 7104");
GbPatientNumber patient = nhs; // Implicit conversion
patient.IdentifierType; // Returns GbPatientNumberIdentifierType.Nhs
patient.Region; // Returns "England/Wales/Isle of Man"
patient.ToGbNhsNumber(); // Returns Some(nhs)
patient.ToGbChiNumber(); // Returns None
patient.ToGbHcNumber(); // Returns None
```

### CHI Number Examples
```csharp
var chi = new GbChiNumber("010190-1234");
GbPatientNumber patient = chi; // Implicit conversion
patient.IdentifierType; // Returns GbPatientNumberIdentifierType.Chi
patient.Region; // Returns "Scotland"
patient.ToGbNhsNumber(); // Returns None (has date component, not NHS)
patient.ToGbChiNumber(); // Returns Some(chi)
patient.ToGbHcNumber(); // Returns None
```

### H&C Number Examples
```csharp
var hc = new GbHcNumber("320 123 4560");
GbPatientNumber patient = hc; // Implicit conversion
patient.IdentifierType; // Returns GbPatientNumberIdentifierType.Hc
patient.Region; // Returns "Northern Ireland"
patient.ToGbNhsNumber(); // Returns None (has H&C prefix)
patient.ToGbChiNumber(); // Returns None (invalid date component)
patient.ToGbHcNumber(); // Returns Some(hc)
```

### Test Number Examples
```csharp
// NHS test number
var test = new GbPatientNumber("900 000 0000");
test.IdentifierType; // Returns GbPatientNumberIdentifierType.Test
test.Region; // Returns "Test"

// H&C test number (if in appropriate range)
var hcTest = new GbPatientNumber("320 000 0000");
hcTest.IdentifierType; // Returns GbPatientNumberIdentifierType.Hc
hcTest.Region; // Returns "Northern Ireland"
```

### Direct String Construction
```csharp
// Construct directly from string
var patient1 = new GbPatientNumber("450 557 7104"); // NHS
var patient2 = new GbPatientNumber("010 190 1234"); // CHI
var patient3 = new GbPatientNumber("320 123 4560"); // H&C
var patient4 = new GbPatientNumber("900 000 0000"); // Test

// Type is automatically determined
patient1.IdentifierType; // Nhs
patient2.IdentifierType; // Chi
patient3.IdentifierType; // Hc
patient4.IdentifierType; // Test
```

### Conversion Examples
```csharp
var patient = new GbPatientNumber("450 557 7104");

// Try to convert to specific type
var nhsOption = patient.ToGbNhsNumber();
if (nhsOption.IsSome)
{
    GbNhsNumber nhs = nhsOption.Value;
    // Use NHS-specific functionality
}
```

## Implementation Notes

### Type Determination Algorithm
1. Use published valid ranges for each health service. See: https://webarchive.nationalarchives.gov.uk/ukgwa/20231221081503/https://digital.nhs.uk/about-nhs-digital/contact-us/freedom-of-information/freedom-of-information-disclosure-log/december-2022/nic-690159-k8h4z

### Validation Strategy
- **Permissive Validation**: Accept if valid as ANY of the three types
- **Type Determination**: Separate concern from validation
- **Validation Order**: 
  1. Basic format (length, digits)
  2. Type-specific validation (date for CHI, check digit for all)
  3. Type determination (using validation rules)

### Storage Strategy
- Store the original string value
- Conversions create new instances on demand

### Conversion Implementation
```csharp
public Option<GbNhsNumber> ToGbNhsNumber()
   => IdentifierType == GbPatientNumberIdentifierType.Nhs
   ? GbNhsNumber.Create(Value)
   : return Option.None<GbNhsNumber>();
```

### Equality Implementation
- Two GbPatientNumber instances are equal if their UnformattedValue is the same
- Format-insensitive (spaces/dashes ignored)
- Type-insensitive for equality (same number is same number regardless of which service)
- Hash code based on UnformattedValue

## Special Considerations

### Future-Proofing
- **Extensibility**: Enum allows adding new types if new UK regions get different systems
- **Validation Evolution**: Each constituent type can evolve independently
- **API Stability**: GbPatientNumber API remains stable even as constituent types change

### When to Use GbPatientNumber vs Specific Types
**Use GbPatientNumber when:**
- Building cross-region applications
- Accepting patient IDs from unknown regions
- Aggregating national health data
- Need flexibility for patient movement between regions
- API endpoints serving multiple regions

**Use Specific Types when:**
- Region is known and fixed
- Need region-specific functionality (e.g., DateOfBirth or Gender for CHI)
- Building region-specific applications
- Maximum type safety required
- Performance critical (avoid type determination overhead)

### Relationship to Constituent Types
- **Composition**: GbPatientNumber wraps/represents GbNhsNumber, GbChiNumber, or GbHcNumber
- **Not Inheritance**: Not a base class, but a combination type
- **Conversion**: Lossless conversion to/from specific types
- **Independence**: Each specific type can be used standalone
- **Interoperability**: Systems can choose appropriate level of abstraction

## References
- [NHS Number](https://www.nhs.uk/using-the-nhs/about-the-nhs/what-is-an-nhs-number/) (England/Wales/Isle of Man)
- [CHI Number](https://www.isdscotland.org/Products-and-Services/eDRIS/CHI-Number/) (Scotland)
- [H&C Number](https://www.nidirect.gov.uk/articles/health-and-care-number) (Northern Ireland)
- [NHS Digital - Demographics](https://digital.nhs.uk/services/demographics) (UK-wide)
- Previous stories: S0019-GbNhsNumber, S0020-GbHcNumber, S0021-GbChiNumber
- See: https://webarchive.nationalarchives.gov.uk/ukgwa/20231221081503/https://digital.nhs.uk/about-nhs-digital/contact-us/freedom-of-information/freedom-of-information-disclosure-log/december-2022/nic-690159-k8h4z

## Testing Requirements

### Core Functionality Tests
- [ ] Construct GbPatientNumber from valid NHS number string
- [ ] Construct GbPatientNumber from valid CHI number string
- [ ] Construct GbPatientNumber from valid H&C number string
- [ ] Reject invalid strings (fail validation for all three types)

### Conversion From Specific Types
- [ ] Implicit conversion from GbNhsNumber preserves value
- [ ] Implicit conversion from GbChiNumber preserves value
- [ ] Implicit conversion from GbHcNumber preserves value
- [ ] IdentifierType correct after conversion from each type

### Conversion To Specific Types
- [ ] ToGbNhsNumber() returns Some for NHS numbers
- [ ] ToGbNhsNumber() returns None for CHI numbers
- [ ] ToGbNhsNumber() returns None for H&C numbers
- [ ] ToGbChiNumber() returns Some for CHI numbers
- [ ] ToGbChiNumber() returns None for NHS numbers
- [ ] ToGbChiNumber() returns None for H&C numbers
- [ ] ToGbHcNumber() returns Some for H&C numbers
- [ ] ToGbHcNumber() returns None for NHS numbers
- [ ] ToGbHcNumber() returns None for CHI numbers
- [ ] TryGet methods work correctly for each type

### Type Determination
- [ ] NHS numbers identified as IdentifierType.Nhs
- [ ] CHI numbers identified as IdentifierType.Chi
- [ ] H&C numbers identified as IdentifierType.Hc
- [ ] Test numbers (900 million) identified correctly
- [ ] Type determination is consistent and deterministic

### Properties
- [ ] Value property returns normalized string
- [ ] IdentifierType property correct for each type
- [ ] Region property returns correct region name

### Format Support
- [ ] Accept unformatted 10-digit strings
- [ ] Accept format with separator characters (NNN NNN NNNN)
- [ ] `Format` method with optional mask parameter (default: "___ ___ ____")
- [ ] Equality works across different format styles

### Equality and Hash Code
- [ ] Equal numbers with different formats are equal
- [ ] Hash codes consistent for equal numbers
- [ ] Different numbers are not equal
- [ ] Equality is format-insensitive
- [ ] Equality compares normalized Value

### JSON Serialization
- [ ] Serialize NHS number and deserialize correctly
- [ ] Serialize CHI number and deserialize correctly
- [ ] Serialize H&C number and deserialize correctly
- [ ] Type information preserved (if using object format)
- [ ] Type inferred correctly (if using string format)
- [ ] Null handling
- [ ] Round-trip preserves value and type

### Integration Tests
- [ ] Convert NHS → GbPatientNumber → NHS (round-trip)
- [ ] Convert CHI → GbPatientNumber → CHI (round-trip)
- [ ] Convert H&C → GbPatientNumber → H&C (round-trip)
- [ ] Cross-type conversions fail as expected
- [ ] Integration with all three specific types

## Documentation Requirements

### Code Examples in Documentation
Include comprehensive examples:
```csharp
// Converting from specific types
var nhs = new GbNhsNumber("450 557 7104");
GbPatientNumber patient = nhs; // Implicit

// Determining type
Console.WriteLine(patient.IdentifierType); // Nhs
Console.WriteLine(patient.Region); // England/Wales/Isle of Man

// Converting back to specific type
var nhsOption = patient.ToGbNhsNumber();
if (nhsOption.IsSome)
{
    GbNhsNumber original = nhsOption.Value;
}

// Working with multiple regions
void ProcessPatient(GbPatientNumber patient)
{
    switch (patient.IdentifierType)
    {
        case GbPatientNumberIdentifierType.Nhs:
            // Handle NHS patient
            break;
        case GbPatientNumberIdentifierType.Chi:
            // Handle Scottish patient
            if (patient.TryGetChiNumber(out var chi))
            {
                // Access CHI-specific features like DateOfBirth
                Console.WriteLine(chi.DateOfBirth);
            }
            break;
        case GbPatientNumberIdentifierType.Hc:
            // Handle Northern Ireland patient
            break;
    }
}
```

## Implementation Phases

### Phase 1: Core Structure
- [ ] Create GbPatientNumber class
- [ ] Create GbPatientNumberValidationResult enum
- [ ] Create GbPatientNumberIdentifierType enum
- [ ] Basic constructor and validation
- [ ] Value and UnformattedValue properties

### Phase 2: Type Determination
- [ ] Implement type determination algorithm
- [ ] CHI detection (date validation)
- [ ] H&C detection (prefix check)
- [ ] NHS detection (catch-all)
- [ ] IdentifierType property
- [ ] Region property
- [ ] IsTestNumber property

### Phase 3: Conversions From Specific Types
- [ ] Implicit conversion from GbNhsNumber
- [ ] Implicit conversion from GbChiNumber
- [ ] Implicit conversion from GbHcNumber
- [ ] Preserve type information during conversion
- [ ] Unit tests for all conversions

### Phase 4: Conversions To Specific Types
- [ ] Implement Option<T> type (or use existing library)
- [ ] ToGbNhsNumber() method
- [ ] ToGbChiNumber() method
- [ ] ToGbHcNumber() method
- [ ] TryGetNhsNumber() method (alternative pattern)
- [ ] TryGetChiNumber() method
- [ ] TryGetHcNumber() method
- [ ] Unit tests for all conversion methods

### Phase 5: Format and Validation
- [ ] Support multiple format styles (spaces, dash, unformatted)
- [ ] Format() method (type-appropriate formatting)
- [ ] Check digit validation (shared with constituent types)
- [ ] Test number range support
- [ ] Comprehensive validation tests

### Phase 6: Operators and Methods
- [ ] ToString() method
- [ ] Equality implementation
- [ ] GetHashCode() implementation
- [ ] Explicit string conversion operator
- [ ] Implicit string conversion operator
- [ ] Static Validate and Create methods

### Phase 7: JSON Serialization
- [ ] GbPatientNumberJsonConverter implementation
- [ ] Decide on serialization format (string vs object)
- [ ] Deserialization with type inference
- [ ] Null handling
- [ ] Round-trip tests

### Phase 8: Testing and Documentation
- [ ] Complete unit test coverage (all scenarios)
- [ ] Integration tests with specific types
- [ ] XML documentation for all members
- [ ] README.md section with examples
- [ ] Code examples and usage guidance

### Phase 9: Review and Polish
- [ ] Performance optimization (lazy type determination, caching)
- [ ] Code review
- [ ] Documentation review
- [ ] Edge case verification
- [ ] Final integration testing

## Relationship to Other Stories

This story builds upon and unifies three previously defined types:
- **S0019-GbNhsNumber**: Base type for NHS Number (England/Wales/Isle of Man)
- **S0020-GbHcNumber**: Base type for H&C Number (Northern Ireland)
- **S0021-GbChiNumber**: Base type for CHI Number (Scotland)

GbPatientNumber is a higher-level abstraction that:
- Accepts any of the three types
- Provides unified interface for cross-region operations
- Enables safe conversions between unified and specific types
- Preserves full type safety through Option<T> pattern

**Implementation Order**: Must implement S0019, S0020, and S0021 before S0022.

## Design Rationale

### Why a Union Type?
- **Real-world need**: Healthcare systems must handle patients moving between UK regions
- **API simplification**: Single patient ID type for UK-wide APIs
- **Type safety**: Conversions use Option<T> to prevent invalid assumptions
- **Flexibility**: Can work with any UK patient ID without knowing region upfront

### Why Not Just Use String?
- **Validation**: Ensures ID is valid for at least one UK health service
- **Type safety**: Compiler prevents passing arbitrary strings
- **Discoverability**: API surface shows available operations
- **Correctness**: Check digit validation enforced

### Why Implicit Conversions From Specific Types?
- **Convenience**: Natural upgrade path from specific to general
- **Safety**: Direction is safe (specific → general loses no information)
- **Ergonomics**: Minimal code changes to accept GbPatientNumber in APIs

### Why Option<T> for Conversions To Specific Types?
- **Safety**: Forces caller to handle case where conversion is invalid
- **Explicitness**: Makes potential failure visible in type system
- **Functional style**: Aligns with modern .NET patterns (Result<T>, Option<T>)

## Open Questions / Decisions Needed

### 3. TryGet vs ToType Methods
- Provide both patterns?
- Choose one pattern for consistency?
- TryGet more familiar to C# developers
- ToType with Option<T> more functional/modern

**Recommendations:**
3. Provide both TryGet and ToType methods (flexibility)
