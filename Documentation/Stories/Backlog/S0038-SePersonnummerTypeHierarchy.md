# S0038: Refactor SePersonnummer into Type Hierarchy with Restricted Types

**Status:** Backlog  
**Priority:** High  
**Effort:** 13 points  
**Related:** S0028 (SePersonnummerUnionMigration), S0019 (GbNhsNumber), S0022 (GbPatientNumber), S0023-S0027 (Union migrations), E01-UnionMigration

---

## Overview

Following the successful union-pattern migration of `SePersonnummer` in S0028, refactor the Swedish identifier type system into a three-tier hierarchy that provides type-safe, restricted access to each identifier category. Replace the current monolithic `SePersonnummer` type (which accepts both personnummer and samordningsnummer values) with:

1. **`SeIdentityNumber`**: Base implementation handling both personnummer and samordningsnummer (rename of current `SePersonnummer`)
2. **`SePersonnummer`**: Restricted type accepting only personnummer values
3. **`SeSamordningsnummer`**: Restricted type accepting only samordningsnummer values

All three types will share a common base class `SeIdentityNumberBase` following the pattern established by `GbPatientNumberBase`, `GbNhsNumber`, `GbChiNumber`, and `GbHcNumber`. This approach provides compile-time type safety, enables more specific business logic targeting, and improves API clarity.

NOTE: Look at creating a unit test base class that contains shared test data

---

## Business Value

* **Type Safety**: Compile-time enforcement that specific code paths receive only the intended identifier type, eliminating runtime category checks.
* **API Clarity**: Developers can immediately see from the type signature whether code expects any Swedish identity number or a specific category.
* **Domain Modeling**: Better represents the Swedish business domain, where personnummer and samordningsnummer have distinct issuance criteria and use cases.
* **Reduced Bugs**: Eliminates defensive coding patterns like `if (identifier.IdentifierType == SeIdentifierType.Personnummer)`, making intent explicit at the type level.
* **Pattern Consistency**: Aligns with proven patterns from the GB patient number hierarchy (`GbPatientNumberBase` with `GbNhsNumber`, `GbChiNumber`, `GbHcNumber`).
* **Backward Compatibility**: The base class `SeIdentityNumber` serves as a drop-in replacement for code that accepts both types, supporting gradual migration.
* **Extensibility**: The base class structure supports future Swedish identifiers (e.g., organizational numbers) using the same framework.

---

## Requirements

### Functional Requirements

#### 1. Base Class Design (`SeIdentityNumberBase`)

Create an abstract record base class that encapsulates all common functionality:

```csharp
public abstract record SeIdentityNumberBase
{
    // Common constants and static properties
    // Common validation logic and helper methods
    // Shared properties: Value, DateOfBirth, Gender, IdentifierType, Format()
    // Abstract or virtual methods for identifier-specific behavior
}
```

**Responsibilities:**
- Store the normalized identifier value (11 or 13 characters, unformatted)
- Store parsed date of birth, gender, and identifier type
- Implement shared validation logic (separator rules, Luhn check, date validation, birth serial number validation)
- Provide shared formatting and conversion methods
- Define the common `Value`, `DateOfBirth`, `Gender`, and `IdentifierType` properties
- Define abstract methods for identifier-specific validation (e.g., category discrimination)

**Design Constraints:**
- Should be `abstract`, not instantiable directly
- Use the union-based validation pattern from S0028
- Must be `record` type for value semantics and immutability
- Should reside in `SeIdentityNumberBase.cs` (new file)

#### 2. SeIdentityNumber Type (Rename Current SePersonnummer)

The current `SePersonnummer` implementation will be renamed to `SeIdentityNumber`:

```csharp
public record SeIdentityNumber : SeIdentityNumberBase
{
    // Accepts both Personnummer and Samordningsnummer
    // Equivalent to current SePersonnummer behavior
    // Constructor and Create() method unchanged in behavior
}
```

**Behavior:**
- Accepts any valid Swedish identity number (personnummer or samordningsnummer)
- `IdentifierType` property returns either `Personnummer` or `Samordningsnummer`
- All existing public API remains identical to current `SePersonnummer`
- Default choice for code that needs to work with both types

**Files:**
- Rename `SePersonnummer.cs` to `SeIdentityNumber.cs` (or keep both files with shared base class logic)

#### 3. SePersonnummer Type (Restricted to Personnummer)

New type that accepts only personnummer values (day field 01–31):

```csharp
public record SePersonnummer : SeIdentityNumberBase
{
    // Accepts ONLY Personnummer values
    // Rejects Samordningsnummer (day field 41–71)
    // Throws or returns error if constructor receives Samordningsnummer
}
```

**Behavior:**
- Constructor throws `UKfValidationException<ValidationError>` if value is a valid samordningsnummer but not a personnummer
- Union `ValidationError` includes a specific case for samordningsnummer rejection (or reuses `InvalidIdentifierType`)
- `IdentifierType` property always returns `Personnummer`
- More specific type for APIs that require personnummer specifically

**Validation Enhancement:**
- Add validation rule: "If identifier would be categorized as Samordningsnummer, reject with error"
- Use existing union discriminator or add new case if needed

#### 4. SeSamordningsnummer Type (Restricted to Samordningsnummer)

New type that accepts only samordningsnummer values (day field 41–71 with +60 offset):

```csharp
public record SeSamordningsnummer : SeIdentityNumberBase
{
    // Accepts ONLY Samordningsnummer values
    // Rejects Personnummer (day field 01–31)
    // Throws or returns error if constructor receives Personnummer
}
```

**Behavior:**
- Constructor throws `UKfValidationException<ValidationError>` if value would be categorized as a personnummer
- Union `ValidationError` includes appropriate case for personnummer rejection
- `IdentifierType` property always returns `Samordningsnummer`
- More specific type for APIs requiring samordningsnummer specifically

**Validation Enhancement:**
- Add validation rule: "If identifier would be categorized as Personnummer, reject with error"
- Use existing union discriminator or add new case if needed

#### 5. Shared Validation Logic

Move all identifier-independent validation into `SeIdentityNumberBase`:

- **Null/Empty Validation**: `EmptyValue`
- **Length Validation**: Must be 11 or 13 characters (including separator) or 10 or 12 unformatted
- **Separator Validation**: Dash (`-`) or plus (`+`) at expected position
- **Character Validation**: All non-separator characters must be ASCII digits
- **Date Validation**: First 6 or 8 digits must represent a valid calendar date
- **Birth Serial Number Validation**: Middle 3 digits must be in range 001–999 (not 000)
- **Check Digit Validation**: Last digit must be valid Luhn checksum

#### 6. Identifier Category Discrimination

Implement category-specific logic in each derived type:

**`SeIdentityNumber`** (Base implementation):
- Accept both day ranges: 01–31 (personnummer) and 41–71 (samordningsnummer)
- Set `IdentifierType` based on day field value
- No rejection of either category

**`SePersonnummer`** (Restricted):
- Accept only day range 01–31
- Reject if day field is 41–71 (samordningsnummer day offset)
- Return error: "Value is a valid samordningsnummer, not a personnummer"
- Always set `IdentifierType` to `Personnummer`

**`SeSamordningsnummer`** (Restricted):
- Accept only day range 41–71 (samordningsnummer offset)
- Reject if day field is 01–31 (personnummer days)
- Return error: "Value is a valid personnummer, not a samordningsnummer"
- Always set `IdentifierType` to `Samordningsnummer`

#### 7. Union Type Enhancements

Extend the union types from S0028 to support identifier-specific rejection:

**Option A: Reuse existing `InvalidIdentifierType` case** (if it exists)
- Use existing union case with contextual message differentiating personnummer vs. samordningsnummer

**Option B: Add new union cases** (if more granularity needed)
```csharp
public record IsPersonnummerRequired(String Message);
public record IsSamordningsnummerRequired(String Message);
```

**Decision**: To be made during implementation. Recommendation is Option A (reuse existing case) unless more granular error reporting is desired.

#### 8. Conversion and Compatibility

Implement conversion operators where appropriate:

```csharp
// SePersonnummer <-> SeIdentityNumber
public static implicit operator SeIdentityNumber(SePersonnummer personnummer);
public static explicit operator SePersonnummer(SeIdentityNumber identity);

// SeSamordningsnummer <-> SeIdentityNumber
public static implicit operator SeIdentityNumber(SeSamordningsnummer samordning);
public static explicit operator SeSamordningsnummer(SeIdentityNumber identity);

// No direct conversion between SePersonnummer and SeSamordningsnummer (type-incompatible)
```

**Rationale:**
- Implicit conversion from restricted to base (always safe; restricted type is a strict subset)
- Explicit conversion from base to restricted (requires validation; may fail)
- No direct conversion between restricted types (ensures type safety)

#### 9. SeIdentifierType Alignment

Update `SeIdentifierType` markers as needed:

- Current: `SeIdentifierType.Personnummer` and `SeIdentifierType.Samordningsnummer`
- No API changes required; marker types remain as-is
- May add documentation linking markers to restricted type usage

#### 10. JSON Serialization

Ensure each type has appropriate JSON converter:

- **`SeIdentityNumber`**: Uses existing logic (accepts both types)
- **`SePersonnummer`**: Uses derived converter (restricts to personnummer during deserialization)
- **`SeSamordningsnummer`**: Uses derived converter (restricts to samordningsnummer during deserialization)

**Implementation Strategy:**
- Create a single converter base or factory that handles common serialization
- Each type can override or extend converter as needed
- Ensure deserialization respects type restrictions (fail gracefully if wrong category)

---

### Non-Functional Requirements

#### 1. Backward Compatibility

**Migration Path:**
- `SeIdentityNumber` is the primary implementation (contains all current logic from `SePersonnummer`)
- Existing code that uses `SePersonnummer` continues to work if `SePersonnummer` becomes an alias or derives from `SeIdentityNumberBase`
- Deprecation strategy (to be finalized during sprint planning):
  - **Option A**: Mark old `SePersonnummer` name as `[Obsolete]` after a grace period
  - **Option B**: Create compiler-version-specific aliases for backward compatibility
  - **Option C**: Use a major version bump to cleanly break

**Recommended:** Option A for gradual deprecation over 2–3 releases

#### 2. Code Organization

**File Structure:**
```
src/KfAccountNumbers/Governmental/Europe/
  ├── SeIdentityNumberBase.cs          (New: abstract base class)
  ├── SeIdentityNumber.cs              (Renamed: current SePersonnummer logic)
  ├── SePersonnummer.cs                (New: restricted to personnummer)
  ├── SeSamordningsnummer.cs           (New: restricted to samordningsnummer)
  ├── SeIdentifierType.cs              (Existing: type markers)
  ├── SeIdentityNumberJsonConverter.cs (New/Renamed: base converter)
  ├── SePersonnummerJsonConverter.cs   (Renamed: may become wrapper)
  └── SeSamordningsnummerJsonConverter.cs (New: if needed)
```

**Alternative (Minimal Files):**
- Keep all types in `SeIdentityNumberBase.cs` (similar to some GB implementations)
- Keep JSON converters in single file with factory/dispatch pattern

**Decision:** Separate files recommended for clarity and single-responsibility principle

#### 3. Naming Conventions

- **Base class**: `SeIdentityNumberBase` (prefix `Se` + suffix `Base`)
- **Unrestricted type**: `SeIdentityNumber` (handles both)
- **Restricted types**: `SePersonnummer`, `SeSamordningsnummer` (name indicates specific category)
- **Consistency**: Follow GB naming (`GbPatientNumberBase`, `GbNhsNumber`, `GbChiNumber`)

#### 4. Error Messaging

All error messages stored in `Messages` class with new keys:

- `SeIdentityNumber_*` (base class errors)
- `SePersonnummer_*` (personnummer-specific errors, including "is samordningsnummer" rejection)
- `SeSamordningsnummer_*` (samordningsnummer-specific errors, including "is personnummer" rejection)

Example messages:
- `SePersonnummer_IsNotPersonnummer`: "The value is a valid samordningsnummer, not a personnummer."
- `SeSamordningsnummer_IsNotSamordningsnummer`: "The value is a valid personnummer, not a samordningsnummer."

#### 5. Test Organization

**Test Files:**
```
tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/
  ├── SeIdentityNumberTests.cs         (Renamed: covers base class and unrestricted logic)
  ├── SePersonnummerTests.cs           (New: tests personnummer-specific restrictions)
  ├── SeSamordningsnummerTests.cs      (New: tests samordningsnummer-specific restrictions)
```

**Alternative (Minimal Tests):**
- Keep all tests in single expanded `SeIdentityNumberTests.cs` file with region organization
- Use parameterized tests to avoid duplication

**Recommended:** Separate test files with focused coverage

#### 6. Documentation Updates

- Update README.md to document the new type hierarchy
- Add examples showing when to use `SeIdentityNumber` vs. `SePersonnummer` vs. `SeSamordningsnummer`
- Link to this story (S0038) and prior migration story (S0028)
- Document conversion operators and implicit/explicit rules
- Provide migration guide for existing code

---

## Acceptance Criteria

### Definition of Done

- [ ] **Base class created**: `SeIdentityNumberBase` abstract record implemented with all shared validation logic, properties, and methods.

- [ ] **SeIdentityNumber type created**: Current `SePersonnummer` logic migrated to new `SeIdentityNumber` type; accepts both personnummer and samordningsnummer.

- [ ] **SePersonnummer restricted type created**: New `SePersonnummer` type accepts only personnummer values (day 01–31); rejects samordningsnummer with appropriate error.

- [ ] **SeSamordningsnummer type created**: New `SeSamordningsnummer` type accepts only samordningsnummer values (day 41–71); rejects personnummer with appropriate error.

- [ ] **Validation logic preserved**: All validation rules from union migration (S0028) maintained; identifier-specific restrictions added.

- [ ] **Conversion operators implemented**: Implicit from restricted to base; explicit from base to restricted; no direct restricted-to-restricted conversions.

- [ ] **Union types aligned**: `ValidationError` and `ValidationResult` unions updated to support identifier-specific rejection cases (or appropriately reused).

- [ ] **JSON serialization updated**: All three types have working JSON converters; deserialization respects type restrictions.

- [ ] **Error messages added**: All required message keys created in `Messages` class with appropriate localization.

- [ ] **SeIdentifierType alignment verified**: Type markers (`Personnummer`, `Samordningsnummer`) work correctly with all three types.

- [ ] **Existing tests updated**: All tests in `SePersonnummerTests.cs` pass with new type hierarchy; minimal changes to test code required.

- [ ] **New test coverage added**: Separate test files for `SePersonnummer` and `SeSamordningsnummer` covering type-specific restrictions and error cases.

- [ ] **Backward compatibility maintained**: Code using old `SePersonnummer` API continues to work (either via alias, inheritance, or deprecation strategy).

- [ ] **Documentation updated**: README.md, XML comments, and migration guide document new type hierarchy with usage examples.

- [ ] **Code review approved**: Architecture reviewed for consistency with GB patient number hierarchy and other patterns in codebase.

- [ ] **Build succeeds**: No compilation errors or warnings; all tests pass.

- [ ] **No performance regression**: Validation performance equivalent to or better than pre-refactoring.

---

## Implementation Strategy

### Phase 1: Foundation (Base Class)

1. Extract common logic into `SeIdentityNumberBase` abstract record
2. Implement shared validation methods and properties
3. Define abstract/virtual methods for derived types to override
4. Ensure union types from S0028 are available for inheritance

**Deliverable:** `SeIdentityNumberBase.cs` with comprehensive implementation

### Phase 2: Unrestricted Type (SeIdentityNumber)

1. Create `SeIdentityNumber.cs` by refactoring current `SePersonnummer` implementation
2. Inherit from `SeIdentityNumberBase`
3. Implement category discrimination logic (both types accepted)
4. Verify all current validation logic works unchanged
5. Create corresponding JSON converter if needed

**Deliverable:** `SeIdentityNumber.cs` with all current `SePersonnummer` behavior

### Phase 3: Restricted Types (SePersonnummer and SeSamordningsnummer)

1. Create `SePersonnummer.cs` inheriting from `SeIdentityNumberBase`
   - Implement personnummer-only validation
   - Add check: day field must be 01–31 (not 41–71)
   - Return appropriate error if samordningsnummer detected

2. Create `SeSamordningsnummer.cs` inheriting from `SeIdentityNumberBase`
   - Implement samordningsnummer-only validation
   - Add check: day field must be 41–71 (not 01–31)
   - Return appropriate error if personnummer detected

3. Add JSON converters for both types

**Deliverables:** `SePersonnummer.cs`, `SeSamordningsnummer.cs`, JSON converters

### Phase 4: Conversion and Interoperability

1. Implement conversion operators (implicit from restricted to base; explicit from base to restricted)
2. Add documentation for conversion semantics
3. Ensure serialization/deserialization respects type boundaries

**Deliverable:** Conversion operators with comprehensive tests

### Phase 5: Testing and Validation

1. Create test files: `SePersonnummerTests.cs`, `SeSamordningsnummerTests.cs`
2. Migrate existing `SePersonnummerTests.cs` to `SeIdentityNumberTests.cs`
3. Add tests for type-specific restrictions
4. Add tests for conversion operators
5. Ensure all union validation cases are exercised

**Deliverable:** Three comprehensive test files with 100% validation coverage

### Phase 6: Documentation and Migration

1. Update README.md with new type hierarchy
2. Add usage examples for each type
3. Provide migration guide for code using old `SePersonnummer`
4. Update error messages in `Messages` class
5. Add XML comments documenting type differences

**Deliverable:** Updated documentation, migration guide, error messages

### Phase 7: Review and Release

1. Code review for architectural consistency
2. Performance testing and verification
3. Final build validation
4. Merge and release coordination

**Deliverable:** Approved, tested, and released code

---

## Type Hierarchy Diagram

```
                    SeIdentityNumberBase (abstract record)
                            ▲
                ┌───────────┼───────────┐
                │           │           │
          SeIdentityNumber  │           │
      (both types accepted)  │           │
                        SePersonnummer  SeSamordningsnummer
                    (day 01-31 only)    (day 41-71 only)

Conversion Operators:
- SePersonnummer → SeIdentityNumber (implicit)
- SeSamordningsnummer → SeIdentityNumber (implicit)
- SeIdentityNumber → SePersonnummer (explicit, may fail)
- SeIdentityNumber → SeSamordningsnummer (explicit, may fail)
- No direct SePersonnummer ↔ SeSamordningsnummer
```

---

## Design Pattern Reference

This refactoring closely follows the GB patient number hierarchy established in prior work:

| GB Pattern | Swedish Pattern | Purpose |
|-----------|-----------------|---------|
| `GbPatientNumberBase` | `SeIdentityNumberBase` | Abstract base with shared logic |
| `GbNhsNumber` | `SeIdentityNumber` | Unrestricted (both categories) |
| `GbChiNumber` | `SePersonnummer` | Restricted (Scottish CHI) |
| `GbHcNumber` | `SeSamordningsnummer` | Restricted (Northern Ireland H&C) |

**Key Similarities:**
- Abstract base class encapsulates common validation and formatting
- Multiple derived types each representing a distinct category or service
- Conversion operators enable safe type interoperability
- Each type includes category-specific discrimination logic
- Separate test files verify type-specific behavior

**Reference Files:**
- `src/KfAccountNumbers/Governmental/Europe/GbPatientNumberBase.cs` (475 lines)
- `src/KfAccountNumbers/Governmental/Europe/GbNhsNumber.cs` (404 lines)
- `src/KfAccountNumbers/Governmental/Europe/GbChiNumber.cs` (similar)
- `src/KfAccountNumbers/Governmental/Europe/GbHcNumber.cs` (similar)

---

## Validation Rules Summary

All validation rules from S0028 (union migration) preserved and extended:

### Shared Rules (All Types)

1. **Null/Empty**: Value may not be null, empty, or all whitespace → `EmptyValue`
2. **Length**: Value must be 10 or 12 characters (unformatted or formatted) → `InvalidLength`
3. **Separator**: Dash (`-`) or plus (`+`) at expected position → `InvalidSeparator`
4. **Characters**: All non-separator characters must be ASCII digits → `InvalidCharacter`
5. **Date**: First 6 or 8 digits must represent valid calendar date → `InvalidDateOfBirth`
6. **Birth Serial**: Middle 3 digits must be 001–999 (not 000) → `InvalidBirthSerialNumber`
7. **Checksum**: Last digit must be valid Luhn check digit → `InvalidChecksum`

### SeIdentityNumber (Unrestricted)

8. **Both categories accepted**: Day field may be 01–31 (personnummer) or 41–71 (samordningsnummer)
9. **IdentifierType determined by day field**: If day ≤ 31 → Personnummer; if day ≥ 41 → Samordningsnummer

### SePersonnummer (Restricted to Personnummer)

8. **Personnummer only**: Day field must be 01–31 (not 41–71)
   - Reject if day field indicates samordningsnummer (41–71)
   - Error: "Value is a valid samordningsnummer, not a personnummer" → `InvalidIdentifierType`

### SeSamordningsnummer (Restricted to Samordningsnummer)

8. **Samordningsnummer only**: Day field must be 41–71 after 60-day offset removal (original day 01–31)
   - Reject if day field indicates personnummer (01–31)
   - Error: "Value is a valid personnummer, not a samordningsnummer" → `InvalidIdentifierType`

---

## Migration Guide for Existing Code

### Before (Current SePersonnummer)

```csharp
// Current code using SePersonnummer (accepts both types)
public void ProcessIdentity(SePersonnummer identity)
{
    if (identity.IdentifierType == SeIdentifierType.Personnummer)
    {
        SendToPersonnummerSystem(identity.Value);
    }
    else if (identity.IdentifierType == SeIdentifierType.Samordningsnummer)
    {
        SendToSamordningsnummerSystem(identity.Value);
    }
}
```

### After (Using Type-Specific API)

**Option A: Gradual Migration (Using SeIdentityNumber Base)**
```csharp
// Refactored to accept base type (still works with all identities)
public void ProcessIdentity(SeIdentityNumber identity)
{
    // Logic remains the same; type system now enforces the hierarchy
}
```

**Option B: Type-Safe Branch (Specific Types)**
```csharp
// New approach: separate methods for each type
public void ProcessIdentity(SePersonnummer personnummer)
{
    SendToPersonnummerSystem(personnummer.Value);
}

public void ProcessIdentity(SeSamordningsnummer samordningsnummer)
{
    SendToSamordningsnummerSystem(samordningsnummer.Value);
}
```

**Option C: Pattern Matching (Best of Both Worlds)**
```csharp
// Type-safe pattern matching on base type
public void ProcessIdentity(SeIdentityNumber identity)
{
    _ = identity switch
    {
        SePersonnummer p => SendToPersonnummerSystem(p.Value),
        SeSamordningsnummer s => SendToSamordningsnummerSystem(s.Value),
        _ => throw new ArgumentException("Unknown Swedish identity type"),
    };
}
```

### Constructor Usage

**Before:**
```csharp
try
{
    var identity = new SePersonnummer("890201-3286");
    // Works for both personnummer and samordningsnummer
}
catch (UKfValidationException<SePersonnummer.ValidationError> ex)
{
    // Handle error
}
```

**After – Option A (Base Type, Most Compatible):**
```csharp
try
{
    var identity = new SeIdentityNumber("890201-3286");
    // Works for both (same behavior)
}
catch (UKfValidationException<SeIdentityNumber.ValidationError> ex)
{
    // Handle error
}
```

**After – Option B (Restricted Type, Type-Safe):**
```csharp
try
{
    var identity = new SePersonnummer("890201-3286");
    // Throws if value is samordningsnummer
}
catch (UKfValidationException<SePersonnummer.ValidationError> ex)
{
    // Handle error (now guaranteed to be personnummer or invalid)
}
```

**After – Option C (Explicit Conversion):**
```csharp
var identity = new SeIdentityNumber("890201-3286");
var personnummer = (SePersonnummer)identity;
// Throws if identity is samordningsnummer
```

---

## Success Metrics

- [ ] All existing unit tests pass without scenario changes (only API adjustments)
- [ ] New type restrictions validated by passing 95+ new test cases across both restricted types
- [ ] Conversion operators thoroughly tested (implicit/explicit, success and failure cases)
- [ ] JSON serialization round-trips succeed for all three types
- [ ] Performance benchmarks show no regression vs. pre-refactoring
- [ ] Code review feedback incorporated; architectural consistency approved
- [ ] README.md and inline documentation clearly explain type hierarchy and usage
- [ ] Team agreement on backward compatibility strategy (deprecation timeline, alias approach, etc.)
- [ ] Build pipeline passes without warnings or errors
- [ ] Released version marked with appropriate semver (likely minor version bump for new API, or major if breaking)

---

## Risk Assessment

### High Risks

1. **Breaking Change Risk**: Renaming `SePersonnummer` to `SeIdentityNumber` breaks existing code
   - **Mitigation**: Provide clear migration path; consider deprecation grace period or compiler aliases

2. **Complexity of Base Class**: Extracting shared logic without introducing subtle bugs
   - **Mitigation**: Comprehensive test suite with 1:1 coverage of current tests plus new restrictions

### Medium Risks

3. **Performance Impact**: Inheritance hierarchy may add indirection
   - **Mitigation**: Benchmark and optimize; likely negligible for record types

4. **JSON Serialization Complexity**: Three types with validation restrictions requires careful converter design
   - **Mitigation**: Base converter with derived specialization; thorough round-trip testing

### Low Risks

5. **Documentation Gaps**: Developers unfamiliar with type hierarchy may misuse API
   - **Mitigation**: Comprehensive README updates, examples, XML comments, and migration guide

---

## Definition of Ready

Before implementation:

- [ ] S0028 (SePersonnummerUnionMigration) successfully completed and deployed
- [ ] Current `SePersonnummer.cs` implementation fully reviewed and validated
- [ ] GB patient number base class (`GbPatientNumberBase`, `GbNhsNumber`, `GbChiNumber`) examined and understood
- [ ] Union types from S0028 available and stable
- [ ] Team consensus on type hierarchy design and naming conventions
- [ ] Backward compatibility strategy agreed upon (deprecation timeline, alias approach, etc.)
- [ ] Test infrastructure reviewed and ready for new test files
- [ ] Messages class confirmed available for new error message keys
- [ ] Documentation template (README style, XML comment format) finalized

---

## Dependencies

- **S0028**: SePersonnummerUnionMigration (must be completed first; provides union types)
- **S0019**, **S0022-S0027**: Previous union migrations (reference implementations)
- **GB Patient Number Stories** (S0019, S0020, S0021): Base class pattern reference
- **Messages class**: For error message keys
- **SeIdentifierType.cs**: Type marker definitions (may need minor updates)

---

## Stakeholders

- **Developers**: Need clear API and migration guidance
- **Product Owner**: Concerned with business value (type safety, domain clarity)
- **QA**: Responsible for comprehensive test coverage
- **DevOps**: Release planning and version management
- **Documentation**: README and developer guide updates

---

## Files to Create/Modify

### Create (New Files)

- `src/KfAccountNumbers/Governmental/Europe/SeIdentityNumberBase.cs` – Abstract base class
- `src/KfAccountNumbers/Governmental/Europe/SeIdentityNumber.cs` – Renamed current implementation
- `src/KfAccountNumbers/Governmental/Europe/SePersonnummer.cs` – Restricted to personnummer
- `src/KfAccountNumbers/Governmental/Europe/SeSamordningsnummer.cs` – Restricted to samordningsnummer
- `src/KfAccountNumbers/Governmental/Europe/SeIdentityNumberJsonConverter.cs` – Base converter
- `src/KfAccountNumbers/Governmental/Europe/SePersonnummerJsonConverter.cs` – Derived/specialized converter
- `src/KfAccountNumbers/Governmental/Europe/SeSamordningsnummerJsonConverter.cs` – Derived/specialized converter
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/SeIdentityNumberTests.cs` – Base/unrestricted tests
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/SePersonnummerTests.cs` – Personnummer-specific tests
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/SeSamordningsnummerTests.cs` – Samordningsnummer-specific tests

### Modify (Existing Files)

- `src/KfAccountNumbers/Governmental/Europe/SeIdentifierType.cs` – Minor updates if needed
- `README.md` – Add section on Swedish identity types and type hierarchy
- `Messages` class – Add new error message keys for type-specific restrictions
- `KfAccountNumbers.csproj` – Verify no changes needed

### Delete (If Applicable)

- Old `SePersonnummer.cs` (after migration to `SeIdentityNumber.cs`)
- Old `SePersonnummerTests.cs` (after migration to `SeIdentityNumberTests.cs`)
- Old JSON converter (if fully refactored into base class or new naming)

---

## Notes

- This story builds directly on S0028 (SePersonnummerUnionMigration), so union types must be stable
- The GB patient number hierarchy is a proven pattern; this story applies the same principles to Swedish identifiers
- The type hierarchy enables gradual migration: code using `SeIdentityNumber` can coexist with code using specific types
- Conversion operators ensure safe interoperability between types
- Testing strategy should verify both that restrictions work (e.g., `SePersonnummer` rejects samordningsnummer) and that base behavior is preserved
- Consider releasing as a minor version (new API, non-breaking if `SeIdentityNumber` is default choice) or major version (if `SePersonnummer` name change is treated as breaking)
