# User Story: Refactor Spanish Identity Number Types Into a Type Hierarchy

## Summary

Restructure the Spanish identity number types to follow the same composite type pattern already established by the Belgian identity number types (`BeIdentityNumber`, `BeRijksregisternummer`, `BeBisnummer`) and Swedish identity number types (`SeIdentityNumber`, `SePersonnummer`, `SeSamordningsnummer`). This eliminates duplication of common validation logic and provides a clearer API for consumers who may need to work with either DNI (Documento Nacional de Identidad) or NIE (Número de Identificación de Extranjero) types.

## Current State

**Single Monolithic Type:**
- `EsNif` — A single type that handles both:
  - DNI (Documento Nacional de Identidad) - issued to Spanish citizens, starts with digit (0-9)
  - NIE (Número de Identificación de Extranjero) - issued to foreigners, starts with X, Y, or Z
- An `IdentifierType` property (discriminated union) indicates which type is represented

**Issues:**
- ✗ All logic is tightly coupled in a single class
- ✗ Consumers must always check `IdentifierType` to understand the exact type
- ✗ Inconsistent with the hierarchy pattern already established by Belgian and Swedish types
- ✗ Difficult to create specialized behavior for each type
- ✗ Code is harder to test in isolation for each type variant
- ✗ API discoverability is reduced when types are combined into a single class

## Desired State

**Composite Type Hierarchy:**
```
EsIdentityNumber (abstract base or composite type)
├── EsNif (citizen document number)
└── EsDni (foreigner identification number)
```

**Benefits:**
- ✓ Follows established Belgian and Swedish type patterns for consistency
- ✓ Clear type safety — consumers can work with specific types directly
- ✓ Each type can have specialized validation logic if needed
- ✓ Easier to extend with type-specific properties or methods
- ✓ Better code organization and maintainability
- ✓ Improved discoverability for API consumers
- ✓ Aligns with cultural naming (NIE developers may search for "NIE", "DNI", "EsDni", etc.)

## Reference Implementation Pattern

Study the existing implementations for guidance:

**BeIdentityNumber.cs** (Base/Composite Type)
- Represents either rijksregisternummer or BIS-nummer
- Has `IdentifierType` property to indicate which
- Common validation logic for both types
- Handles shared properties like `DateOfBirth`, `Gender`

**BeRijksregisternummer.cs** (Specific Type)
- Inherits from or wraps BeIdentityNumber
- Specific validation for rijksregisternummer (no month offset)
- Example: month component is 01-12 (no offset)

**BeBisnummer.cs** (Specific Type)
- Inherits from or wraps BeIdentityNumber
- Specific validation for BIS-nummer (month offset 40 or 20)
- Specialized rule: month component has 40 or 20 added

**SeIdentityNumber.cs** (Swedish Base/Composite Type)
- Represents either personnummer or samordningsnummer
- Has `IdentifierType` property to indicate which
- Common validation logic for both types

**SePersonnummer.cs** (Swedish Specific Type)
- Inherits from or wraps SeIdentityNumber
- Specific validation for personnummer

**SeSamordningsnummer.cs** (Swedish Specific Type)
- Inherits from or wraps SeIdentityNumber
- Specific validation for samordningsnummer

**Applicable Pattern to Spanish Types:**
- `EsIdentityNumber` base type (like BeIdentityNumber, SeIdentityNumber)
- `EsNif` specific type (like BeRijksregisternummer, SePersonnummer)
- `EsDni` specific type (like BeBisnummer, SeSamordningsnummer)

## Acceptance Criteria

### 1. New Type Hierarchy Structure
- [ ] Create `EsIdentityNumber` abstract base class or factory type
- [ ] Refactor existing `EsNif` to inherit from or wrap `EsIdentityNumber`
- [ ] Create new `EsDni` type inheriting from or wrapping `EsIdentityNumber`
- [ ] Both derived types share common validation and parsing logic from base
- [ ] `IdentifierType` enum/union exists and has `Nif` and `Dni` values (or equivalent)

### 2. API Contract Maintenance
- [ ] Existing `EsNif` public API remains backward compatible
- [ ] New consumers can use `EsDni` directly
- [ ] New consumers can use `EsIdentityNumber` as common base type
- [ ] Implicit/explicit operators work for all three types
- [ ] Constructor and `Create()` factory method work for all types

### 3. Validation Logic Separation
- [ ] Common validation (length, characters, checksum) in base type
- [ ] Type-specific validation in each derived type:
  - **EsNif**: Leading character must be digit 0-9 (Spanish citizen)
  - **EsDni**: Leading character must be X, Y, or Z (Foreigner resident)
- [ ] Validation properly enforces type-specific structure

### 4. Properties and Methods
- [ ] All three types have:
  - `Value` property (the raw 9-character number)
  - `CheckDigitAlgorithmName` constant property (Modulus 23)
  - `IdentifierType` property (for base type)
- [ ] All properties correctly interpret the structure
- [ ] Check character validation remains consistent across all types

### 5. Constructors and Factory Methods
- [ ] `EsNif` constructor:
  - Accepts 9-character string (unformatted) or 10-character string (formatted with 1 separator)
  - Validates as Nif (leading character must be digit 0-9)
  - Throws on invalid Nif data

- [ ] `EsDni` constructor:
  - Accepts 9-character string (unformatted) or 11-character string (formatted with 2 separators)
  - Validates as Dni (leading character must be X, Y, or Z)
  - Throws on invalid Dni data

- [ ] `EsIdentityNumber.Create()` / factory method:
  - Accepts a potential identity number string
  - Automatically determines if it's Nif or Dni based on leading character
  - Returns appropriate type instance or validation error

### 6. Validation Rules by Type

**Common Rules (Both Types):**
- [ ] Value may not be null, empty, or all whitespace
- [ ] All characters (except optional separators) must be ASCII digits ('0'-'9') or terminal check character
- [ ] Middle 7 characters must be ASCII digits ('0'-'9')
- [ ] Trailing (right-most) character must be valid modulus 23 check character
- [ ] Valid check characters are "TRWAGMYFPDXBNJZSQVHLCKE" (T = remainder 0, E = remainder 22)
- [ ] Optional separator characters must not be ASCII digits

**EsNif-Specific (Citizen Number):**
- [ ] Leading character must be an ASCII digit (0-9)
- [ ] Value must be 9 characters (unformatted) or 10 characters (with 1 separator in position 8)
- [ ] If formatted (10 characters), separator must be at position 8 (0-based)
- [ ] Validation fails if leading character is X, Y, or Z (those indicate Dni)

**EsDni-Specific (Foreigner Number):**
- [ ] Leading character must be X, Y, or Z
- [ ] Value must be 9 characters (unformatted) or 11 characters (with 2 separators in positions 1 and 9)
- [ ] If formatted (11 characters), separators must be at positions 1 and 9 (0-based)
- [ ] Both separator characters must be the same (if formatted)
- [ ] Validation fails if leading character is a digit (those indicate Nif)

### 7. Testing Coverage
- [ ] Unit tests for `EsIdentityNumber` base functionality
- [ ] Unit tests for `EsNif` with:
  - Valid Nif values (digits 0-9 as leading character)
  - Invalid Dni values (X, Y, Z as leading character)
  - Valid formatted values (with separator at position 8)
  - Edge cases and all valid check characters
- [ ] Unit tests for `EsDni` with:
  - Valid Dni values (X, Y, Z as leading characters)
  - Invalid Nif values (digits as leading character)
  - Valid formatted values (with separators at positions 1 and 9)
  - Edge cases for all three leading characters (X, Y, Z)
- [ ] Integration tests demonstrating type interoperability
- [ ] Case-insensitivity tests (uppercase/lowercase normalization)

### 8. Documentation Updates
- [ ] Update XML documentation on all three types
- [ ] Create or update `EsNif.md` reference documentation
- [ ] Create new `EsDni.md` reference documentation
- [ ] Create new `EsIdentityNumber.md` reference documentation (composite type)
- [ ] Update README.md to reference the new types

### 9. Breaking Change Management
- [ ] Document that current `EsNif` behavior remains unchanged for existing consumers
- [ ] New `EsDni` type is purely additive
- [ ] Verify that discriminated union `IdentifierCategory` is handled properly in refactoring
- [ ] Create migration guide if any API adjustments are needed

## Implementation Approach

### Phase 1: Create Base Type
1. Create `EsIdentityNumber.cs` as abstract base or sealed composite type
2. Extract common validation logic from `EsNif`
3. Define or preserve `EsIdentifierType` enum with `Nif` and `Dni` values
4. Implement shared properties: `Value`, `CheckDigitAlgorithmName`
5. Preserve existing unions: `IdentifierCategory`, `ValidationError`, `ValidationResult`

### Phase 2: Refactor Existing Type
1. Modify `EsNif.cs` to inherit/wrap `EsIdentityNumber`
2. Add validation specific to Nif (leading digit 0-9 only)
3. Ensure backward compatibility with existing API
4. Update tests to verify behavior unchanged
5. Preserve case-insensitivity normalization

### Phase 3: Create New Type
1. Create `EsDni.cs` inheriting/wrapping `EsIdentityNumber`
2. Implement validation for Dni (leading X, Y, or Z only)
3. Implement correct separator handling (positions 1 and 9, matching separators)
4. Comprehensive test coverage including all leading characters
5. Case-insensitivity support consistent with EsNif

### Phase 4: Factory and Common Methods
1. Implement `EsIdentityNumber.Create()` factory that auto-detects type
2. Implement `EsIdentityNumber.Validate()` that returns validation result
3. Implement implicit/explicit operators for all three types
4. Consider conversion methods between types if appropriate

### Phase 5: Testing
1. Create comprehensive test suite following existing patterns
2. Test all validation rules for both derived types
3. Test factory method auto-detection logic
4. Test backward compatibility of existing `EsNif`
5. Test case normalization across all types
6. Test separator handling for both formatted variants

### Phase 6: Documentation
1. Update XML documentation for all types
2. Create reference .md files
3. Update README.md
4. Create examples and usage guidance

## Design Decisions

### Structure Options

**Option A: Inheritance (Recommended - Following Belgian and Swedish Pattern)**
```csharp
public abstract record EsIdentityNumber { ... }
public record EsNif : EsIdentityNumber { ... }
public record EsDni : EsIdentityNumber { ... }
```
- Pros: Type safety, follows established pattern, clean polymorphism
- Cons: Requires refactoring of existing EsNif

**Option B: Composition with Factory**
```csharp
public record EsIdentityNumber { 
    public EsIdentifierType Type { get; }
    public EsNif? Nif { get; }
    public EsDni? Dni { get; }
}
```
- Pros: Easier backward compatibility
- Cons: Less type-safe, requires null checks, inconsistent with existing patterns

**Recommendation:** Use Option A (Inheritance) for consistency with Belgian and Swedish types

### Base Type Public API
```csharp
public abstract record EsIdentityNumber
{
    public string Value { get; }
    public EsIdentifierType IdentifierType { get; }
    public const String CheckDigitAlgorithmName = "Modulus 23";

    public static ValidationResult Validate(string? value);
    public static CreateResult<EsIdentityNumber, ValidationError> Create(string? value);
    public static implicit operator string(EsIdentityNumber source);
}
```

### Naming Considerations

The naming intentionally follows cultural conventions:
- **EsNif** - For Spanish Citizens (NIF = Número de Identificación Fiscal)
  - More specific name helps developers find what they need
  - Aligns with Spanish internal naming conventions

- **EsDni** - For Foreign Residents (NIE = Número de Identificación de Extranjero, often called DNI informally)
  - Uses "Dni" to match the similar pattern to Norwegian "Dnummer"
  - Clear distinction from Nif for developers reading the code

## Risk Analysis

| Risk | Probability | Impact | Mitigation |
| :--- | :--- | :--- | :--- |
| Breaking existing consumer code | **Medium** | High | Maintain backward compatibility; thorough testing |
| Separator position handling errors | **Low** | Medium | Extensive test coverage for all separator combinations |
| Leading character validation errors | **Low** | Medium | Clear unit tests for leading character rules |
| Case normalization consistency | **Low** | Low | Verify uppercase normalization in both types |
| Difficulty determining type from string | **Low** | Medium | Clear factory method with auto-detection |
| Type naming confusion (Nif vs Dni) | **Low** | Low | Clear documentation and examples |

## Files to Be Modified
- `src/KfAccountNumbers/National/Europe/EsNif.cs` — Refactoring to hierarchy
- `src/KfAccountNumbers/National/Europe/EsIdentifierType.cs` — May need updates
- `README.md` — Add new types (EsDni, EsIdentityNumber)

## Files to Be Created
- `src/KfAccountNumbers/National/Europe/EsIdentityNumber.cs` — Base type
- `src/KfAccountNumbers/National/Europe/EsDni.cs` — New specific type
- `docs/Reference/National/Europe/EsIdentityNumber.md` — Documentation
- `docs/Reference/National/Europe/EsDni.md` — Documentation
- Unit test files for new types

## Definition of Done
- [ ] All acceptance criteria met and verified
- [ ] All unit tests passing (new and existing)
- [ ] Backward compatibility confirmed with existing code
- [ ] Code review completed
- [ ] Documentation complete and accurate
- [ ] No performance regressions
- [ ] Pull request merged to main branch

## Estimated Effort
- **Design & Planning:** 3-4 hours
- **Base Type Implementation:** 3-4 hours
- **Refactor Existing Type:** 2-3 hours
- **New EsDni Type:** 3-4 hours
- **Factory & Operators:** 2-3 hours
- **Testing:** 4-6 hours
- **Documentation:** 2-3 hours
- **Code Review & Refinement:** 2-3 hours
- **Total Estimated:** 21-30 hours

## Priority
Medium-High — Architectural improvement and consistency with existing patterns

## Related User Stories
- S0048-BeRijksregisternummerHierarchyRefactor (Belgian hierarchy refactor)
- S0038 (Swedish hierarchy - SeIdentityNumber)
- S0047-Documentation_Expansion (create EsIdentityNumber.md, EsDni.md)

## Technical Notes

### Leading Character Determination

| Type | Leading Character | Format Length | Separator Positions |
| :--- | :--- | :--- | :--- |
| EsNif | 0-9 (digit) | 9 or 10 | Position 8 (optional) |
| EsDni | X, Y, or Z | 9 or 11 | Positions 1 and 9 (optional, must match) |

### Check Character Calculation

Both types use the same modulus 23 algorithm:
1. Remove leading character (digit or X/Y/Z)
2. Convert leading character: digit = as-is, X=0, Y=1, Z=2
3. Calculate modulus 23 of the numeric value
4. Look up remainder in "TRWAGMYFPDXBNJZSQVHLCKE"

### Case-Insensitivity

The type maintains case-insensitivity:
- Input: lowercase letters (x, y, z) → converted to uppercase (X, Y, Z)
- Input: uppercase letters → kept as uppercase
- Equality comparisons use normalized uppercase values

## Success Metrics
- ✓ All existing tests continue to pass
- ✓ New type hierarchy covers all validation rules
- ✓ Consumer code using EsNif requires no changes
- ✓ New consumers can use EsDni and EsIdentityNumber
- ✓ Code follows Belgian and Swedish type patterns for consistency
- ✓ Documentation is clear and comprehensive
- ✓ All leading character variants are tested (0-9 for Nif, X/Y/Z for Dni)

## References & Related Items
- `src/KfAccountNumbers/National/Europe/BeIdentityNumber.cs` — Reference implementation pattern (Belgian)
- `src/KfAccountNumbers/National/Europe/BeRijksregisternummer.cs` — Similar specific type implementation
- `src/KfAccountNumbers/National/Europe/BeBisnummer.cs` — Similar specific type implementation
- `src/KfAccountNumbers/National/Europe/SeIdentityNumber.cs` — Reference implementation pattern (Swedish)
- `src/KfAccountNumbers/National/Europe/SePersonnummer.cs` — Specific type example
- `src/KfAccountNumbers/National/Europe/SeSamordningsnummer.cs` — Specific type example
- `docs/Reference/National/Europe/EsNif.md` — Existing reference documentation
- Existing EsNif unit tests — Baseline for coverage
