# User Story: Refactor Belgian Identity Number Types Into a Type Hierarchy

## Summary

Restructure the Belgian identity number types to follow the same composite type pattern already established by the Swedish identity number types (`SeIdentityNumber`, `SePersonnummer`, `SeSamordningsnummer`). This eliminates duplication of common validation logic and provides a clearer API for consumers who may need to work with either rijksregisternummer or BIS-nummer types.

## Current State

**Single Monolithic Type:**
- `BeRijksregisternummer` — A single type that handles both:
  - Rijksregisternummer (residents of Belgium)
  - BIS-nummer (non-residents, 40 or 20 month offset)
- An `IdentifierType` property indicates which type is represented

**Issues:**
- ✗ All logic is tightly coupled in a single class
- ✗ Consumers must always check `IdentifierType` to understand the exact type
- ✗ Inconsistent with the Swedish type hierarchy pattern already in codebase
- ✗ Difficult to create specialized behavior for each type
- ✗ Code is harder to test in isolation for each type variant

## Desired State

**Composite Type Hierarchy:**
```
BeIdentityNumber (abstract base or composite type)
├── BeRijksregisternummer (resident number)
└── BeBisnummer (non-resident number, with 40 or 20 month offset)
```

**Benefits:**
- ✓ Follows established Swedish type pattern for consistency
- ✓ Clear type safety — consumers can work with specific types directly
- ✓ Each type can have specialized validation logic
- ✓ Easier to extend with type-specific properties or methods
- ✓ Better code organization and maintainability
- ✓ Improved discoverability for API consumers

## Reference Implementation Pattern

Study the Swedish implementation for guidance:

**SeIdentityNumber.cs** (Base/Composite Type)
- Represents either personnummer or samordningsnummer
- Has `IdentifierType` property to indicate which
- Common validation logic for both types
- Handles shared properties like `DateOfBirth`, `Gender`

**SePersonnummer.cs** (Specific Type)
- Inherits from or wraps SeIdentityNumber
- Specific validation for personnummer
- Example: day component is 01-31 (no offset)

**SeSamordningsnummer.cs** (Specific Type)
- Inherits from or wraps SeIdentityNumber
- Specific validation for samordningsnummer
- Specialized rule: day component is 61-91 (day + 60 offset)

**Applicable Pattern to Belgian Types:**
- `BeIdentityNumber` base type (like SeIdentityNumber)
- `BeRijksregisternummer` specific type (like SePersonnummer)
- `BeBisnummer` specific type (like SeSamordningsnummer)

## Acceptance Criteria

### 1. New Type Hierarchy Structure
- [ ] Create `BeIdentityNumber` abstract base class or factory type
- [ ] Refactor existing `BeRijksregisternummer` to inherit from or wrap `BeIdentityNumber`
- [ ] Create new `BeBisnummer` type inheriting from or wrapping `BeIdentityNumber`
- [ ] Both derived types share common validation and parsing logic from base
- [ ] `IdentifierType` enum exists and has `Rijksregisternummer` and Bisnummer values

### 2. API Contract Maintenance
- [ ] Existing `BeRijksregisternummer` public API remains backward compatible
- [ ] New consumers can use `BeBisnummer` directly
- [ ] New consumers can use `BeIdentityNumber` as common base type
- [ ] Implicit/explicit operators work for all three types
- [ ] Constructor and `Create()` factory method work for all types

### 3. Validation Logic Separation
- [ ] Common validation (length, characters, checksum, basic date validation) in base type
- [ ] Type-specific validation in each derived type:
  - **BeRijksregisternummer**: Month field is 01-12 (no offset)
  - **BeBisnummer**: Month field has 40 or 20 added to actual month (01-12 becomes 41-52 or 21-32)
- [ ] Month offset logic properly applied only to BIS-number validation

### 4. Properties and Methods
- [ ] All three types have:
  - `Value` property (the raw 11-digit number)
  - `DateOfBirth` property (derived date, with offsets applied as needed)
  - `Gender` property (derived from sequence number)
  - `IdentifierType` property (for base type)
- [ ] BeBisnummer-specific: Clear indication of which offset (40 or 20) is used
- [ ] All properties correctly interpret the encoded month/day

### 5. Constructors and Factory Methods
- [ ] `BeRijksregisternummer` constructor:
  - Accepts 11-digit string (unformatted) or 15-character string (formatted)
  - Validates as rijksregisternummer (no month offset)
  - Throws on invalid rijksregisternummer data

- [ ] `BeBisnummer` constructor:
  - Accepts 11-digit string (unformatted) or 15-character string (formatted)
  - Validates as BIS-nummer (validates with 40 or 20 month offset)
  - Throws on invalid BIS-nummer data

- [ ] `BeIdentityNumber.Create()` / factory method:
  - Accepts a potential identity number string
  - Automatically determines if it's rijksregisternummer or BIS-nummer
  - Returns appropriate type instance or validation error

### 6. Validation Rules by Type

**Common Rules (Both Types):**
- [ ] Value may not be null, empty, or all whitespace
- [ ] Value must be 11 characters (unformatted) or 15 characters (formatted)
- [ ] All non-separator characters must be ASCII digits ('0'-'9')
- [ ] Separator characters must not be ASCII digits ('0'-'9')
- [ ] Two trailing characters must be valid modulus 97 checksum
- [ ] Sequence number may not be 000 or 999
- [ ] Century of birth determined by checksum (1900-1999 or 2000-2099)

**BeRijksregisternummer-Specific:**
- [ ] Month field (after YY) must be 01-12 (no offset applied)
- [ ] Day field must be valid for the month/year (with special handling for incomplete dates)
- [ ] Validation fails if month field indicates BIS-nummer offset (41-52 or 21-32)

**BeBisnummer-Specific:**
- [ ] Month field must be 41-52 (offset +40) OR 21-32 (offset +20)
- [ ] After removing offset, month must be 01-12
- [ ] Day field must be valid for the adjusted month/year
- [ ] Validation fails if month field indicates rijksregisternummer (01-12)

### 7. Testing Coverage
- [ ] Unit tests for `BeIdentityNumber` base functionality
- [ ] Unit tests for `BeRijksregisternummer` with:
  - Valid rijksregisternummer values
  - Invalid BIS-nummer values (month offset present)
  - Edge cases (incomplete dates, zeros)
- [ ] Unit tests for `BeBisnummer` with:
  - Valid BIS-nummer values (both +40 and +20 offsets)
  - Invalid rijksregisternummer values
  - Correct DateOfBirth with offset applied
- [ ] Integration tests demonstrating type interoperability

### 8. Documentation Updates
- [ ] Update XML documentation on all three types
- [ ] Create or update `BeRijksregisternummer.md` reference documentation
- [ ] Create new `BeBisnummer.md` reference documentation
- [ ] Create new `BeIdentityNumber.md` reference documentation (composite type)
- [ ] Update README.md to reference the new types

### 9. Breaking Change Management
- [ ] Document that `BeRijksregisternummer` behavior remains unchanged
- [ ] New `BeBisnummer` type is purely additive
- [ ] Create migration guide if needed for any API adjustments
- [ ] Deprecation warnings if any properties/methods change

## Implementation Approach

### Phase 1: Create Base Type
1. Create `BeIdentityNumber.cs` as abstract base or sealed composite type
2. Extract common validation logic from `BeRijksregisternummer`
3. Define `BeIdentifierType` enum with `Rijksregisternummer` and `Bisnummer` values
4. Implement shared properties: `Value`, `DateOfBirth`, `Gender`

### Phase 2: Refactor Existing Type
1. Modify `BeRijksregisternummer.cs` to inherit/wrap `BeIdentityNumber`
2. Add validation specific to rijksregisternummer (reject month offsets)
3. Ensure backward compatibility with existing API
4. Update tests to verify behavior unchanged

### Phase 3: Create New Type
1. Create `BeBisnummer.cs` inheriting/wrapping `BeIdentityNumber`
2. Implement validation for BIS-nummer (month offset 40 or 20)
3. Implement `BisnummerOffset` property (returns 40 or 20)
4. Implement correct `DateOfBirth` calculation with offset removed
5. Comprehensive test coverage

### Phase 4: Factory and Common Methods
1. Implement `BeIdentityNumber.Create()` factory that auto-detects type
2. Implement `BeIdentityNumber.Validate()` that returns validation result
3. Implement implicit/explicit operators for all three types
4. Consider conversion methods between types if appropriate

### Phase 5: Testing
1. Create comprehensive test suite following existing patterns
2. Test all validation rules for both derived types
3. Test factory method auto-detection logic
4. Test backward compatibility of existing `BeRijksregisternummer`

### Phase 6: Documentation
1. Update XML documentation for all types
2. Create reference .md files
3. Update README.md
4. Create examples and usage guidance

## Design Decisions

### Structure Options

**Option A: Inheritance (Recommended - Following Swedish Pattern)**
```csharp
public abstract record BeIdentityNumber { ... }
public record BeRijksregisternummer : BeIdentityNumber { ... }
public record BeBisnummer : BeIdentityNumber { ... }
```
- Pros: Type safety, follows SeIdentityNumber pattern, clean polymorphism
- Cons: Requires refactoring of existing BeRijksregisternummer

**Option B: Composition with Factory**
```csharp
public record BeIdentityNumber { 
    public BeIdentifierType Type { get; }
    public BeRijksregisternummer? Rijksregisternummer { get; }
    public BeBisnummer? Bisnummer { get; }
}
```
- Pros: Easier backward compatibility
- Cons: Less type-safe, requires null checks, inconsistent with Swedish pattern

**Recommendation:** Use Option A (Inheritance) for consistency with Swedish types

### Base Type Public API
```csharp
public abstract record BeIdentityNumber
{
    public string Value { get; }
    public DateOnly DateOfBirth { get; }
    public Gender Gender { get; }
    public BeIdentifierType IdentifierType { get; }

    public static ValidationResult Validate(string? value);
    public static CreateResult<BeIdentityNumber, ValidationError> Create(string? value);
    public static implicit operator string(BeIdentityNumber source);
}
```

## Risk Analysis

| Risk | Probability | Impact | Mitigation |
| :--- | :--- | :--- | :--- |
| Breaking existing consumer code | **Medium** | High | Maintain backward compatibility; thorough testing |
| Month offset calculation errors | **Medium** | High | Extensive test coverage for both offset types |
| Difficulty determining type from string | **Low** | Medium | Clear factory method with auto-detection |
| Performance impact from inheritance | **Low** | Low | Use `record` types; inline where possible |
| Documentation complexity | **Low** | Medium | Clear examples for each type |

## Files to Be Modified
- `src/KfAccountNumbers/National/Europe/BeRijksregisternummer.cs` — Refactoring to hierarchy
- `src/KfAccountNumbers/National/Europe/BeIdentifierType.cs` — May need updates
- `README.md` — Add new types (BeBisnummer, BeIdentityNumber)

## Files to Be Created
- `src/KfAccountNumbers/National/Europe/BeIdentityNumber.cs` — Base type
- `src/KfAccountNumbers/National/Europe/BeBisnummer.cs` — New specific type
- `docs/Reference/National/Europe/BeIdentityNumber.md` — Documentation
- `docs/Reference/National/Europe/BeBisnummer.md` — Documentation
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
- **New BiBsnummer Type:** 3-4 hours
- **Factory & Operators:** 2-3 hours
- **Testing:** 4-6 hours
- **Documentation:** 2-3 hours
- **Code Review & Refinement:** 2-3 hours
- **Total Estimated:** 21-30 hours

## Priority
Medium-High — Architectural improvement and consistency with existing patterns

## Related User Stories
- S0046-DocumentationExpansion (create BeIdentityNumber.md, BeBisnummer.md)
- Any refactoring of SeIdentityNumber hierarchy if applicable

## Technical Notes

### Month Offset Handling
| Type | Actual Month | Encoded Month | Formula |
| :--- | :--- | :--- | :--- |
| Rijksregisternummer | 1-12 | 1-12 | None |
| BIS-nummer (gender known) | 1-12 | 41-52 | encoded - 40 |
| BIS-nummer (gender unknown) | 1-12 | 21-32 | encoded - 20 |

The validation logic must correctly interpret encoded months and reverse-calculate actual birth dates.

### Test Data Requirements
Gather or create test cases for:
- Valid rijksregisternummer values (all months, various genders)
- Valid BIS-nummer values (both +40 and +20 variants)
- Edge cases (incomplete dates, century boundaries)
- Invalid values that mix types (e.g., rijksregisternummer format with BIS month offset)

## Success Metrics
- ✓ All existing tests continue to pass
- ✓ New type hierarchy covers all validation rules
- ✓ Consumer code using BeRijksregisternummer requires no changes
- ✓ New consumers can use BeBisnummer and BeIdentityNumber
- ✓ Code follows SeIdentityNumber pattern for consistency
- ✓ Documentation is clear and comprehensive

## References & Related Items
- `src/KfAccountNumbers/National/Europe/SeIdentityNumber.cs` — Reference implementation pattern
- `src/KfAccountNumbers/National/Europe/SePersonnummer.cs` — Specific type example
- `src/KfAccountNumbers/National/Europe/SeSamordningsnummer.cs` — Specific type example
- Existing BeRijksregisternummer unit tests — Baseline for coverage
