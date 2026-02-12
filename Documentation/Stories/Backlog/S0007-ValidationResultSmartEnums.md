# S0007-ValidationResultSmartEnums: Refactor Validation Result Enums to Smart Enum Pattern

## Title
Convert ValidationResult enums to smart enum classes with shared base class for improved extensibility and maintainability

## User Story
As a **.NET developer maintaining the KfAccountNumbers library**,  
I want **validation result enums refactored to smart enum classes with a common base**,  
so that **I can add rich behavior to validation results, improve error messaging, eliminate code duplication, and enable polymorphic validation result handling across different account number types**.

## Background
Currently, each account number type has its own validation result enum:
- `CaSocialInsuranceNumberValidationResult`
- `UsSocialSecurityNumberValidationResult`
- `MxCurpValidationResult`
- `UsIndividualTaxpayerIdentificationNumberValidationResult`

These enums have several limitations:
1. **Code duplication**: Each enum has a `ToErrorDescription()` extension method with switch expressions
2. **Limited functionality**: Enums can't have methods, properties, or inherit behavior
3. **No polymorphism**: Can't treat different validation results uniformly
4. **Maintenance burden**: Adding new validation types requires duplicating patterns
5. **Testing complexity**: Each extension method needs separate testing
6. **No intellisense for descriptions**: Error messages are in Messages resource file, requiring lookup

The **smart enum pattern** (enumeration classes) provides:
- Type-safe enumeration with class benefits
- Rich behavior through methods and properties
- Inheritance hierarchy for shared functionality
- Singleton instances (like enum values)
- Better extensibility without breaking changes
- Self-documenting code with error messages embedded
- Easier unit testing

This refactoring will improve code quality while maintaining backward compatibility through implicit conversions.

## Acceptance Criteria

### Core Smart Enum Infrastructure
- A **base `ValidationResult` abstract class** is created with:
  - `Name` property (e.g., "ValidationPassed", "Empty", "InvalidLength")
  - `ErrorMessage` property (user-friendly description)
  - `IsSuccess` boolean property (true only for ValidationPassed)
  - `Severity` property (Success, Warning, Error)
  - Protected constructor to prevent external instantiation
  - `Equals` and `GetHashCode` based on name (for singleton semantics)
  - `ToString()` returning the name
  - Implicit conversion operator to `String` (returns ErrorMessage)
  - Static `FromName` method for lookup by name
- Each account number type has a **derived smart enum class**:
  - `CaSocialInsuranceNumberValidationResult : ValidationResult`
  - `UsSocialSecurityNumberValidationResult : ValidationResult`
  - `MxCurpValidationResult : ValidationResult`
  - `UsIndividualTaxpayerIdentificationNumberValidationResult : ValidationResult`
- Smart enum instances are **static readonly singleton fields**:

```csharp
public static readonly CaSocialInsuranceNumberValidationResult ValidationPassed = new("ValidationPassed", "Validation passed.", isSuccess: true); public static readonly CaSocialInsuranceNumberValidationResult Empty = new("Empty", "SIN value is null, empty or all whitespace."); // ... etc
```

- All singleton instances are exposed as **public static readonly fields** on the derived class
- A **static `All` property** returns `IReadOnlyList<T>` of all instances for enumeration
- A **static `FromName` method** returns the instance with the matching name (or null)

- Original enum types are **marked `[Obsolete]`** with migration guidance but not removed initially
- All existing code continues to work without modification
- Exception classes continue to work with both enum and smart enum

### Integration with Existing Code
- All `Validate` methods return the smart enum type (conversion operators handle enum returns)
- All exception constructors accept the smart enum type (conversion operators handle enum inputs)
- `CreateResult<T, TValidationResult>` works with smart enum types
- Messages resource file entries can be gradually migrated into smart enum definitions

### Testing
- Base `ValidationResult` class has comprehensive unit tests
- Each derived smart enum class has tests verifying:
- All expected instances exist
- Singleton behavior (same instance returned)
- Correct `IsSuccess` values
- Correct `ErrorMessage` values
- `FromName` lookup works
- `All` property contains all instances
- Equality semantics
- Implicit conversions to/from enum
- Integration tests verify existing validation still works

## Technical Notes

### Base ValidationResult Implementation

/// <summary> 
///   Base class for validation result smart enums across all account number types. 
/// </summary> public abstract class ValidationResult : IEquatable<ValidationResult> 
{ 
  /// <summary> 
  ///   The unique name of this validation result. 
  /// </summary> 
  public String Name { get; }
  
  /// <summary>
  ///   The user-friendly error message describing this validation result.
  /// </summary>
  public String ErrorMessage { get; }

  /// <summary>
  ///   Indicates whether this represents a successful validation.
  /// </summary>
  public Boolean IsSuccess { get; }

  /// <summary>
  ///   The severity level of this validation result.
  /// </summary>
  public ValidationResultSeverity Severity { get; }

protected ValidationResult(
    String name, 
    String errorMessage, 
    Boolean isSuccess = false,
    ValidationResultSeverity severity = ValidationResultSeverity.Error)
{
    Name = name ?? throw new ArgumentNullException(nameof(name));
    ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
    IsSuccess = isSuccess;
    Severity = isSuccess ? ValidationResultSeverity.Success : severity;
}

public virtual Boolean Equals(ValidationResult? other)
    => other is not null && Name.Equals(other.Name, StringComparison.Ordinal);

public override Boolean Equals(Object? obj)
    => Equals(obj as ValidationResult);

public override Int32 GetHashCode()
    => Name.GetHashCode(StringComparison.Ordinal);

public override String ToString() => Name;

public static implicit operator String(ValidationResult result)
    => result?.ErrorMessage ?? String.Empty;
}


### Derived Smart Enum Pattern

/// <summary> 
///   Validation results for Canadian Social Insurance Numbers. 
/// </summary> public sealed class CaSocialInsuranceNumberValidationResult : ValidationResult 
{ 
  // Singleton instances 
  public static readonly CaSocialInsuranceNumberValidationResult ValidationPassed =     new("ValidationPassed",     "The value does not contain any validation errors.", isSuccess: true);

  public static readonly CaSocialInsuranceNumberValidationResult Empty = 
    new("Empty", "SIN value is null, empty or all whitespace characters.");

  public static readonly CaSocialInsuranceNumberValidationResult InvalidLength = 
    new("InvalidLength", "SIN value has incorrect length. Must be exactly 9 or 11 characters in length.");

// ... other instances

// Collection of all instances
private static readonly Lazy<IReadOnlyList<CaSocialInsuranceNumberValidationResult>> _all = new(() => new[]
{
    ValidationPassed,
    Empty,
    InvalidLength,
    // ... etc
});

public static IReadOnlyList<CaSocialInsuranceNumberValidationResult> All => _all.Value;

// Private constructor
private CaSocialInsuranceNumberValidationResult(
    String name, 
    String errorMessage, 
    Boolean isSuccess = false,
    ValidationResultSeverity severity = ValidationResultSeverity.Error)
    : base(name, errorMessage, isSuccess, severity)
{
}

// Lookup by name
public static CaSocialInsuranceNumberValidationResult? FromName(String name)
    => All.FirstOrDefault(r => r.Name.Equals(name, StringComparison.Ordinal));

// Backward compatibility - conversion to legacy enum
public static implicit operator CaSocialInsuranceNumberValidationResultEnum(CaSocialInsuranceNumberValidationResult result)
    => Enum.Parse<CaSocialInsuranceNumberValidationResultEnum>(result.Name);

// Backward compatibility - conversion from legacy enum
public static implicit operator CaSocialInsuranceNumberValidationResult(CaSocialInsuranceNumberValidationResultEnum enumValue)
    => FromName(enumValue.ToString()) 
        ?? throw new ArgumentException($"Unknown enum value: {enumValue}", nameof(enumValue));
}



### Benefits Over Current Approach
- **Centralized error messages**: No more searching Messages.resx
- **Type safety with flexibility**: Can add methods without breaking changes
- **Polymorphic handling**: `ValidationResult` base enables generic validation handling
- **Better IDE experience**: IntelliSense shows error message with each value
- **Easier testing**: Test instances directly, not extension methods
- **Extensibility**: Can add methods like `TryParse`, `IsTransient`, etc.
- **Documentation**: Error messages embedded in code, not separate resource file
- **No switch statements**: Logic embedded in the type hierarchy

### Performance Considerations
- Smart enums are singleton instances (no allocation per use)
- Implicit conversions have negligible overhead
- Equality comparisons are reference checks (singleton pattern)
- `FromName` can be optimized with `Dictionary<String, T>` if needed
- No performance regression compared to enum + extension method

## Definition of Done

- **Code complete**:
  - Base `ValidationResult` class implemented and tested
  - `ValidationResultSeverity` enum created
  - All four account number validation result smart enums implemented
  - XML documentation complete on all public members
- **Unit tests complete** covering:
  - Base `ValidationResult` class:
    - Constructor validation
    - Equality semantics
    - ToString behavior
    - Implicit string conversion
  - Each smart enum class:
    - All expected instances exist and are singletons
    - Correct `IsSuccess` values
    - Correct `ErrorMessage` values
    - Correct `Severity` values
    - `All` property completeness
    - `FromName` lookup (valid and invalid names)
    - Equality between instances
    - GetHashCode consistency
    - Implicit conversions to/from legacy enum
  - Integration tests:
    - Existing validation methods work unchanged
    - `CreateResult<T, TValidationResult>` works with smart enums
- **All existing tests pass** 
- **Code review** completed and approved
- **Documentation updated**:
  - README.md updated to reference smart enums
  - Migration guide for consumers (if library is public)
  - Inline comments for complex conversion logic
- **No compiler warnings** (except expected obsolete warnings if using old enums)
- **Performance benchmarks** confirm no regression

## Out of Scope

- **Changing exception class names** or structure
- **Adding new validation types** (can be done after refactoring)
- **Localization/internationalization** of error messages (future enhancement)
- **Custom error message templates** (future enhancement)
- **Serialization-specific behavior** (JSON converters continue to work)
- **Database persistence changes** (smart enums work like enums for EF Core)
- **API breaking changes** (all existing code must continue working)

## References

- [Smart Enum Pattern - Ardalis](https://github.com/ardalis/SmartEnum)
- [Enumeration Classes - Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types)
- [Smart Enums in C# - Steve Smith](https://ardalis.com/enum-alternatives-in-c/)
- [Value Objects - DDD Reference](https://www.domainlanguage.com/wp-content/uploads/2016/05/DDD_Reference_2015-03.pdf)

## Notes

### Advantages of Smart Enum Pattern

1. **Encapsulation**: Error messages live with validation results, not in separate files
2. **Extensibility**: Can add methods without breaking changes
3. **Type Safety**: Compiler prevents invalid instances
4. **Singleton Pattern**: Each instance is unique (like enum values)
5. **Polymorphism**: Can treat all validation results uniformly via base class
6. **Self-Documenting**: Error message visible in code, not hidden in resources
7. **Testability**: Test instances directly, not through extension methods
8. **Maintainability**: Single location for name, message, and behavior

