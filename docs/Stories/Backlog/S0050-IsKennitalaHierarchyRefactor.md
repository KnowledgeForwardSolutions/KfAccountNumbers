# User Story: Refactor Icelandic Identifier Types Into a Type Hierarchy

## Summary

Restructure the Icelandic identifier types to follow the same composite type pattern already established by the Belgian identity number types (`BeIdentityNumber`, `BeRijksregisternummer`, `BeBisnummer`), Swedish identity number types (`SeIdentityNumber`, `SePersonnummer`, `SeSamordningsnummer`), and Spanish identity number types (`EsIdentityNumber`, `EsNif`, `EsDni`). This eliminates duplication of common validation logic and provides a clearer API for consumers who may need to work with either personal (einstaklingur/person) or business (fyrirtæki/company) Icelandic identifiers.

## Current State

**Single Monolithic Type:**
- `IsKennitala` — A single type that handles both:
  - Einstaklingur (Person) - day component is 01-31
  - Fyrirtæki (Company) - day component has 40 added (41-71)
- An `IdentifierType` property (discriminated union) indicates which type is represented
- Uses weighted modulus 11 check digit algorithm
- Supports both 10-digit (unformatted) and 11-character (formatted with separator) representations

**Issues:**
- ✗ All logic is tightly coupled in a single class
- ✗ Consumers must always check `IdentifierType` to understand the exact type
- ✗ Inconsistent with the hierarchy pattern already established by Belgian, Swedish, and Spanish types
- ✗ Difficult to create specialized behavior for each type
- ✗ Code is harder to test in isolation for each type variant
- ✗ API discoverability is reduced when types are combined into a single class

## Desired State

**Composite Type Hierarchy:**
```
IsKennitala (abstract base or composite type)
├── IsEinstaklingurKennitala (person identifier)
└── IsFyrirtaekiKennitala (company identifier)
```

**Benefits:**
- ✓ Follows established Belgian, Swedish, and Spanish type patterns for consistency
- ✓ Clear type safety — consumers can work with specific types directly
- ✓ Each type can have specialized validation logic if needed
- ✓ Easier to extend with type-specific properties or methods
- ✓ Better code organization and maintainability
- ✓ Improved discoverability for API consumers
- ✓ Aligns with Icelandic naming conventions (developers familiar with Icelandic can recognize "Einstaklingur" and "Fyrirtæki")
- ✓ Clearer semantic separation: person vs. company identifiers

## Reference Implementation Pattern

Study the existing implementations for guidance:

**BeIdentityNumber.cs** (Base/Composite Type)
- Represents either rijksregisternummer or BIS-nummer
- Has `IdentifierType` property to indicate which
- Common validation logic for both types

**BeRijksregisternummer.cs** (Specific Type)
- Inherits from or wraps BeIdentityNumber
- Specific validation for rijksregisternummer (no month offset)

**BeBisnummer.cs** (Specific Type)
- Inherits from or wraps BeIdentityNumber
- Specific validation for BIS-nummer (month offset 40 or 20)

**SeIdentityNumber.cs** (Swedish Base/Composite Type)
- Represents either personnummer or samordningsnummer
- Has `IdentifierType` property to indicate which

**EsIdentityNumber.cs** (Spanish Base/Composite Type)
- Represents either Nif or Dni
- Has `IdentifierType` property to indicate which

**Applicable Pattern to Icelandic Types:**
- `IsKennitala` base type (like BeIdentityNumber, SeIdentityNumber, EsIdentityNumber)
- `IsEinstaklingurKennitala` specific type (like BeRijksregisternummer, SePersonnummer, EsNif)
- `IsFyrirtaekiKennitala` specific type (like BeBisnummer, SeSamordningsnummer, EsDni)

## Acceptance Criteria

### 1. New Type Hierarchy Structure
- [ ] Create `IsKennitala` abstract base class or factory type (or rename existing to base if using inheritance)
- [ ] Create new `IsEinstaklingurKennitala` type inheriting from or wrapping `IsKennitala`
- [ ] Create new `IsFyrirtaekiKennitala` type inheriting from or wrapping `IsKennitala`
- [ ] Both derived types share common validation and parsing logic from base
- [ ] `IdentifierType` enum/union exists and has `Einstaklingur` and `Fyrirtaeki` values

### 2. API Contract Maintenance
- [ ] Existing `IsKennitala` public API remains backward compatible (or provides clear upgrade path)
- [ ] New consumers can use `IsEinstaklingurKennitala` directly
- [ ] New consumers can use `IsFyrirtaekiKennitala` directly
- [ ] New consumers can use `IsKennitala` as common base type
- [ ] Implicit/explicit operators work for all three types
- [ ] Constructor and `Create()` factory method work for all types

### 3. Validation Logic Separation
- [ ] Common validation (length, characters, checksum, century indicator, base date validation) in base type
- [ ] Type-specific validation in each derived type:
  - **IsEinstaklingurKennitala**: Day component must be 01-31 (no offset)
  - **IsFyrirtaekiKennitala**: Day component must be 41-71 (offset +40 applied)
- [ ] Day offset logic properly applied only to company identifier validation

### 4. Properties and Methods
- [ ] All three types have:
  - `Value` property (the raw 10-digit number)
  - `DateOfBirth` property (derived date, with day offset removed for company identifiers)
  - `IdentifierType` property (for base type)
  - `CheckDigitAlgorithmName` constant property (Weighted Modulus 11)
- [ ] `DateOfBirth` correctly interprets the day offset for company identifiers
- [ ] Constants preserved: `FyrirtaekiDayOffset`, `MinimumValidYearOfBirth`, `MaximumValidYearOfBirth`

### 5. Constructors and Factory Methods
- [ ] `IsEinstaklingurKennitala` constructor:
  - Accepts 10-digit string (unformatted) or 11-character string (formatted with 1 separator)
  - Validates as Einstaklingur (day component must be 01-31)
  - Throws on invalid Einstaklingur kennitala data

- [ ] `IsFyrirtaekiKennitala` constructor:
  - Accepts 10-digit string (unformatted) or 11-character string (formatted with 1 separator)
  - Validates as Fyrirtæki (day component must be 41-71)
  - Throws on invalid Fyrirtæki kennitala data

- [ ] `IsKennitala.Create()` / factory method:
  - Accepts a potential kennitala string
  - Automatically determines if it's Einstaklingur or Fyrirtæki based on day component
  - Returns appropriate type instance or validation error

### 6. Validation Rules by Type

**Common Rules (Both Types):**
- [ ] Value may not be null, empty, or all whitespace
- [ ] Value must be 10 characters (unformatted) or 11 characters (formatted with 1 separator)
- [ ] All non-separator characters must be ASCII digits ('0'-'9')
- [ ] Separator character, if included, must not be ASCII digit, must be at position 6 (0-based)
- [ ] Check digit (position 8, 0-based) must be valid using weighted modulus 11 algorithm
- [ ] Century indicator (position 9, 0-based) must be '9' (1900s) or '0' (2000s)
- [ ] Date of birth, after deriving century from century indicator and (if Fyrirtæki) subtracting day offset, must be valid

**IsEinstaklingurKennitala-Specific (Person):**
- [ ] Day component (positions 0-1, 0-based) must be 01-31 (no offset)
- [ ] Validation fails if day component is 41-71 (those indicate Fyrirtæki)

**IsFyrirtaekiKennitala-Specific (Company):**
- [ ] Day component (positions 0-1, 0-based) must be 41-71 (offset +40 applied)
- [ ] After subtracting offset (day - 40), resulting day must be 01-31
- [ ] Validation fails if day component is 01-31 (those indicate Einstaklingur)

### 7. Testing Coverage
- [ ] Unit tests for `IsKennitala` base functionality
- [ ] Unit tests for `IsEinstaklingurKennitala` with:
  - Valid Einstaklingur values (days 01-31)
  - Invalid Fyrirtæki values (days 41-71)
  - Valid formatted values (with separator at position 6)
  - All century indicators (9 and 0)
  - Edge cases (01/01/1900, 31/12/2099 boundaries)
- [ ] Unit tests for `IsFyrirtaekiKennitala` with:
  - Valid Fyrirtæki values (days 41-71)
  - Invalid Einstaklingur values (days 01-31)
  - Valid formatted values (with separator at position 6)
  - All century indicators (9 and 0)
  - Correct DateOfBirth with day offset applied and removed
  - Edge cases (01/01/1900, 31/12/2099 boundaries for underlying dates)
- [ ] Integration tests demonstrating type interoperability
- [ ] Tests for weighted modulus 11 check digit calculation

### 8. Documentation Updates
- [ ] Update XML documentation on all three types
- [ ] Create or update `IsKennitala.md` reference documentation (base/composite type)
- [ ] Create new `IsEinstaklingurKennitala.md` reference documentation
- [ ] Create new `IsFyrirtaekiKennitala.md` reference documentation
- [ ] Update README.md to reference the new types
- [ ] Provide examples showing how to distinguish between person and company identifiers

### 9. Breaking Change Management
- [ ] Document that current `IsKennitala` behavior remains unchanged for existing consumers
- [ ] New specific types (`IsEinstaklingurKennitala`, `IsFyrirtaekiKennitala`) are purely additive
- [ ] Determine migration strategy if API structure fundamentally shifts
- [ ] Create migration guide if any API adjustments are needed

## Implementation Approach

### Phase 1: Create Base Type
1. Create `IsKennitala` as abstract base or sealed composite type (or identify if refactoring existing)
2. Extract common validation logic
3. Define or preserve `IsIdentifierType` enum with `Einstaklingur` and `Fyrirtaeki` values
4. Implement shared properties: `Value`, `DateOfBirth`, `IdentifierType`, `CheckDigitAlgorithmName`
5. Preserve existing constants and unions

### Phase 2: Create Person Type
1. Create `IsEinstaklingurKennitala.cs` inheriting/wrapping `IsKennitala`
2. Implement validation specific to Einstaklingur (day 01-31 only)
3. Comprehensive test coverage including all day boundaries
4. Ensure `DateOfBirth` returns correct date without offset

### Phase 3: Create Company Type
1. Create `IsFyrirtaekiKennitala.cs` inheriting/wrapping `IsKennitala`
2. Implement validation for Fyrirtæki (day 41-71 only)
3. Implement correct day offset handling (subtract 40 to get actual day)
4. Implement `DateOfBirth` calculation with offset properly removed
5. Comprehensive test coverage

### Phase 4: Factory and Common Methods
1. Implement `IsKennitala.Create()` factory that auto-detects type based on day component
2. Implement `IsKennitala.Validate()` that returns validation result
3. Implement implicit/explicit operators for all three types
4. Consider conversion methods between types if appropriate

### Phase 5: Refactor Existing (if needed)
1. Determine if current `IsKennitala` class should remain as base or be replaced
2. Update serialization/JSON converters if needed
3. Update any dependent code or extensions
4. Ensure backward compatibility with existing tests

### Phase 6: Testing
1. Create comprehensive test suite following existing patterns
2. Test all validation rules for both derived types
3. Test factory method auto-detection logic
4. Test backward compatibility of existing `IsKennitala` usage
5. Test day offset calculation for company identifiers
6. Test weighted modulus 11 check digit across all types

### Phase 7: Documentation
1. Update XML documentation for all types with Icelandic naming clarification
2. Create reference .md files
3. Update README.md
4. Create examples showing person vs. company identification

## Design Decisions

### Structure Options

**Option A: Inheritance (Recommended - Following Established Pattern)**
```csharp
public abstract record IsKennitala { ... }
public record IsEinstaklingurKennitala : IsKennitala { ... }
public record IsFyrirtaekiKennitala : IsKennitala { ... }
```
- Pros: Type safety, follows established pattern (Belgian, Swedish, Spanish), clean polymorphism
- Cons: Requires refactoring of existing IsKennitala

**Option B: Composition with Factory**
```csharp
public record IsKennitala { 
    public IsIdentifierType Type { get; }
    public IsEinstaklingurKennitala? Person { get; }
    public IsFyrirtaekiKennitala? Company { get; }
}
```
- Pros: Easier backward compatibility, minimal refactoring
- Cons: Less type-safe, requires null checks, inconsistent with established patterns

**Recommendation:** Use Option A (Inheritance) for consistency with Belgian, Swedish, and Spanish types

### Base Type Public API
```csharp
public abstract record IsKennitala
{
    public string Value { get; }
    public DateOnly DateOfBirth { get; }
    public IsIdentifierType IdentifierType { get; }
    public const String CheckDigitAlgorithmName = "Weighted Modulus 11";
    public const Int32 FyrirtaekiDayOffset = 40;
    public const Int32 MinimumValidYearOfBirth = 1900;
    public const Int32 MaximumValidYearOfBirth = 2099;

    public static ValidationResult Validate(string? value);
    public static CreateResult<IsKennitala, ValidationError> Create(string? value);
    public static implicit operator string(IsKennitala source);
}
```

## Risk Analysis

| Risk | Probability | Impact | Mitigation |
| :--- | :--- | :--- | :--- |
| Breaking existing consumer code | **Medium** | High | Maintain backward compatibility; thorough testing; clear migration guide |
| Day offset calculation errors | **Low** | High | Extensive test coverage for boundary cases (01-31 and 41-71) |
| Century indicator handling | **Low** | Medium | Verify both '9' and '0' century indicators work correctly |
| Check digit calculation errors | **Low** | High | Validate weighted modulus 11 algorithm across all types |
| Auto-detection logic errors | **Low** | Medium | Clear tests for factory method day-based detection |
| Identifier type determination | **Low** | Medium | Test all boundary cases (30/31 transitions, 40/41 transitions) |

## Files to Be Modified
- `src/KfAccountNumbers/National/Europe/IsKennitala.cs` — Refactoring to hierarchy (or new base type)
- `src/KfAccountNumbers/National/Europe/IsIdentifierType.cs` — May need updates
- `README.md` — Add new types (IsEinstaklingurKennitala, IsFyrirtaekiKennitala)

## Files to Be Created
- `src/KfAccountNumbers/National/Europe/IsEinstaklingurKennitala.cs` — Person identifier type
- `src/KfAccountNumbers/National/Europe/IsFyrirtaekiKennitala.cs` — Company identifier type
- `docs/Reference/National/Europe/IsKennitala.md` — Documentation (base/composite type)
- `docs/Reference/National/Europe/IsEinstaklingurKennitala.md` — Documentation
- `docs/Reference/National/Europe/IsFyrirtaekiKennitala.md` — Documentation
- Unit test files for new types

## Definition of Done
- [ ] All acceptance criteria met and verified
- [ ] All unit tests passing (new and existing)
- [ ] Backward compatibility confirmed with existing code
- [ ] Code review completed
- [ ] Documentation complete and accurate
- [ ] All day boundary cases tested (01-31, 41-71)
- [ ] Both century indicators tested (9 and 0)
- [ ] No performance regressions
- [ ] Pull request merged to main branch

## Estimated Effort
- **Design & Planning:** 3-4 hours
- **Base Type Implementation/Refactoring:** 3-4 hours
- **Person Type Implementation:** 3-4 hours
- **Company Type Implementation:** 3-4 hours
- **Factory & Operators:** 2-3 hours
- **Testing:** 5-7 hours (including boundary cases and day offset logic)
- **Documentation:** 2-3 hours
- **Code Review & Refinement:** 2-3 hours
- **Total Estimated:** 23-32 hours

## Priority
Medium-High — Architectural improvement and consistency with established type patterns

## Related User Stories
- S0048-BeRijksregisternummerHierarchyRefactor (Belgian hierarchy refactor)
- S0049-EsIdentityNumberHierarchyRefactor (Spanish hierarchy refactor)
- S0038 (Swedish hierarchy - SeIdentityNumber)
- S0047-Documentation_Expansion (create IsKennitala.md, IsEinstaklingurKennitala.md, IsFyrirtaekiKennitala.md)

## Technical Notes

### Day Component Determination

| Type | Day Range | Offset Applied | Example |
| :--- | :--- | :--- | :--- |
| IsEinstaklingurKennitala | 01-31 | None | Day 15 = "15" (person born on 15th) |
| IsFyrirtaekiKennitala | 41-71 | +40 | Day 55 = "15" (company registered on 15th, stored as 41-71 range) |

Given DDMMYY format:
- "150185" → day=15, month=01, year=85 → Einstaklingur (person born Jan 15, 1985)
- "550185" → day=55, month=01, year=85 → Fyrirtæki (company registered Jan 15, 1985)

### Weighted Modulus 11 Check Digit Algorithm

The check digit is calculated using a weighted modulus 11 algorithm with weights [3, 2, 7, 6, 5, 4, 3, 2, 1] applied to the first 9 digits:

1. Multiply each of the first 9 digits by their corresponding weight
2. Sum all products
3. Calculate sum modulo 11
4. Subtract from 11 to get check digit
5. If result is 10, the number is invalid (no valid check digit exists)

### Century Indicator

The last digit (position 9, 0-based) indicates century of birth:
- '9' = 1900s (years 1900-1999)
- '0' = 2000s (years 2000-2099)

### Formatter and Separator

Typical format: "DDMMYY-RRPC" where:
- DD = day (01-31 for persons, 41-71 for companies)
- MM = month (01-12)
- YY = year within century (00-99)
- RRPC = (separator)-RRPC where RR = random digits, P = check digit, C = century indicator
- Separator is optional, commonly '-' or space ' '

## Success Metrics
- ✓ All existing tests continue to pass
- ✓ New type hierarchy covers all validation rules
- ✓ Consumer code using IsKennitala requires no changes (or has clear migration path)
- ✓ New consumers can use IsEinstaklingurKennitala and IsFyrirtaekiKennitala
- ✓ Code follows Belgian, Swedish, and Spanish type patterns for consistency
- ✓ Documentation is clear and comprehensive
- ✓ All day boundary cases properly validated (01, 31, 40, 41, 71, 72)
- ✓ All century indicators work correctly (9 and 0)

## References & Related Items
- `src/KfAccountNumbers/National/Europe/BeIdentityNumber.cs` — Reference implementation pattern (Belgian)
- `src/KfAccountNumbers/National/Europe/BeRijksregisternummer.cs` — Similar specific type implementation
- `src/KfAccountNumbers/National/Europe/BeBisnummer.cs` — Similar specific type with offset implementation
- `src/KfAccountNumbers/National/Europe/SeIdentityNumber.cs` — Reference implementation pattern (Swedish)
- `src/KfAccountNumbers/National/Europe/EsIdentityNumber.cs` — Reference implementation pattern (Spanish)
- `docs/Reference/National/Europe/BeRijksregisternummer.md` — Similar reference documentation format
- `docs/Reference/National/Europe/IsKennitala.md` — Existing reference documentation
- https://en.wikipedia.org/wiki/Icelandic_identification_number — Official reference
- https://kennitala.com/ — Icelandic Kennitala information
- Existing IsKennitala unit tests — Baseline for coverage
