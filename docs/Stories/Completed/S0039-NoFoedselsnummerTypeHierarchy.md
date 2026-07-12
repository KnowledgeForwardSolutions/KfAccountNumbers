# S0039: Refactor NoFoedselsnummer into Type Hierarchy with Restricted Types

**Status:** Backlog  
**Priority:** High  
**Effort:** 13 points  
**Related:** S0029 (NoFoedselsnummerUnionMigration), S0038 (SePersonnummerTypeHierarchy), S0019 (GbNhsNumber), S0022 (GbPatientNumber), S0023-S0027 (Union migrations), E01-UnionMigration

---

## Overview

Following the successful union-pattern migration of `NoFoedselsnummer` in S0029 and the parallel Swedish identifier type hierarchy refactoring in S0038, refactor the Norwegian identifier type system into a three-tier hierarchy that provides type-safe, restricted access to each identifier category. Replace the current monolithic `NoFoedselsnummer` type (which accepts both fødselsnummer and D-nummer values) with:

1. **`NoIdentityNumber`**: Base implementation handling both fødselsnummer and D-nummer (rename of current `NoFoedselsnummer`)
2. **`NoFoedselsnummer`**: Restricted type accepting only fødselsnummer values
3. **`NoDNummer`**: Restricted type accepting only D-nummer values

All three types will share a common base class `NoIdentityNumberBase` following the pattern established by `GbPatientNumberBase`, `GbNhsNumber`, `GbChiNumber`, and `GbHcNumber`, as well as the pattern now defined in S0038 for Swedish identifiers. This approach provides compile-time type safety, enables more specific business logic targeting, and improves API clarity for Norwegian identity numbers.

---

## Business Value

* **Type Safety**: Compile-time enforcement that specific code paths receive only the intended identifier type, eliminating runtime category checks.
* **API Clarity**: Developers can immediately see from the type signature whether code expects any Norwegian identity number or a specific category (fødselsnummer vs. D-nummer).
* **Domain Modeling**: Better represents the Norwegian business domain, where fødselsnummer and D-nummer have distinct issuance criteria and eligibility requirements.
* **Reduced Bugs**: Eliminates defensive coding patterns like `if (identifier.IdentifierType == NoIdentifierType.Foedselsnummer)`, making intent explicit at the type level.
* **Pattern Consistency**: Aligns with proven patterns from the GB patient number hierarchy (`GbPatientNumberBase` with `GbNhsNumber`, `GbChiNumber`, `GbHcNumber`) and the Swedish hierarchy (S0038).
* **Backward Compatibility**: The base class `NoIdentityNumber` serves as a drop-in replacement for code that accepts both types, supporting gradual migration.
* **Extensibility**: The base class structure supports future Norwegian identifiers using the same framework.
* **Bilateral Development**: Enables parallel, non-conflicting development alongside Swedish identifier refactoring (S0038).

---

## Requirements

### Functional Requirements

#### 1. Base Class Design (`NoIdentityNumberBase`)

Create an abstract record base class that encapsulates all common functionality:

```csharp
public abstract record NoIdentityNumberBase
{
    // Common constants and static properties
    // Common validation logic and helper methods
    // Shared properties: Value, DateOfBirth, Gender, IdentifierType, Format()
    // Abstract or virtual methods for identifier-specific behavior
}
```

**Responsibilities:**
- Store the normalized identifier value (11 characters, unformatted)
- Store parsed date of birth (DDMMYY format), gender, individual number, and identifier type
- Implement shared validation logic (separator rules, weighted modulus 11 check, date validation, individual number validation)
- Provide shared formatting and conversion methods
- Define the common `Value`, `DateOfBirth`, `Gender`, `IdentifierType`, and `Format()` properties
- Define abstract/virtual methods for identifier-specific validation (e.g., category discrimination)

**Design Constraints:**
- Should be `abstract`, not instantiable directly
- Use the union-based validation pattern from S0029
- Must be `record` type for value semantics and immutability
- Should reside in `NoIdentityNumberBase.cs` (new file)

**Norwegian-Specific Logic:**
- Weighted modulus 11 check digit calculation (different from Swedish Luhn algorithm)
- Two check digits (C1 and C2) with distinct weight arrays
- Century derivation from individual number (based on two-digit year and individual number range)
- Day field discrimination: 01–31 (fødselsnummer) vs. 41–71 (D-nummer, with +40 offset)

#### 2. NoIdentityNumber Type (Rename Current NoFoedselsnummer)

The current `NoFoedselsnummer` implementation will be renamed to `NoIdentityNumber`:

```csharp
public record NoIdentityNumber : NoIdentityNumberBase
{
    // Accepts both Foedselsnummer and DNummer
    // Equivalent to current NoFoedselsnummer behavior
    // Constructor and Create() method unchanged in behavior
}
```

**Behavior:**
- Accepts any valid Norwegian identity number (fødselsnummer or D-nummer)
- `IdentifierType` property returns either `Foedselsnummer` or `DNummer`
- All existing public API remains identical to current `NoFoedselsnummer`
- Default choice for code that needs to work with both types
- Day field range: 01–71 (both categories)

**Files:**
- Rename `NoFoedselsnummer.cs` to `NoIdentityNumber.cs` (or keep both files with shared base class logic)

#### 3. NoFoedselsnummer Type (Restricted to Fødselsnummer)

New type that accepts only fødselsnummer values (day field 01–31):

```csharp
public record NoFoedselsnummer : NoIdentityNumberBase
{
    // Accepts ONLY Fødselsnummer values
    // Rejects D-nummer (day field 41–71)
    // Throws or returns error if constructor receives D-nummer
}
```

**Behavior:**
- Constructor throws `UKfValidationException<ValidationError>` if value is a valid D-nummer but not a fødselsnummer
- Union `ValidationError` includes a specific case for D-nummer rejection (or reuses `InvalidIdentifierType`)
- `IdentifierType` property always returns `Foedselsnummer`
- More specific type for APIs that require fødselsnummer specifically
- Day field range: 01–31 only

**Validation Enhancement:**
- Add validation rule: "If identifier would be categorized as D-nummer, reject with error"
- Use existing union discriminator or add new case if needed

#### 4. NoDNummer Type (Restricted to D-Nummer)

New type that accepts only D-nummer values (day field 41–71):

```csharp
public record NoDNummer : NoIdentityNumberBase
{
    // Accepts ONLY D-nummer values
    // Rejects Fødselsnummer (day field 01–31)
    // Throws or returns error if constructor receives Fødselsnummer
}
```

**Behavior:**
- Constructor throws `UKfValidationException<ValidationError>` if value would be categorized as a fødselsnummer
- Union `ValidationError` includes appropriate case for fødselsnummer rejection
- `IdentifierType` property always returns `DNummer`
- More specific type for APIs requiring D-nummer specifically
- Day field range: 41–71 only

**Validation Enhancement:**
- Add validation rule: "If identifier would be categorized as Fødselsnummer, reject with error"
- Use existing union discriminator or add new case if needed

#### 5. Shared Validation Logic

Move all identifier-independent validation into `NoIdentityNumberBase`:

- **Null/Empty Validation**: `EmptyValue`
- **Length Validation**: Must be 11 characters (unformatted) or 12 characters (with separator)
- **Separator Validation**: Non-digit character at position 6 (between DDMMYY and IIICC)
- **Character Validation**: All non-separator characters must be ASCII digits
- **Date Validation**: First 6 digits must represent a valid calendar date (DDMMYY format)
- **Individual Number Validation**: Middle 3 digits must be in range 001–999
- **Check Digit Validation**: Last 2 digits must be valid weighted modulus 11 check digits

#### 6. Identifier Category Discrimination

Implement category-specific logic in each derived type:

**`NoIdentityNumber`** (Base implementation):
- Accept both day ranges: 01–31 (fødselsnummer) and 41–71 (D-nummer)
- Set `IdentifierType` based on day field value
- No rejection of either category

**`NoFoedselsnummer`** (Restricted):
- Accept only day range 01–31
- Reject if day field is 41–71 (D-nummer day offset)
- Return error: "Value is a valid D-nummer, not a fødselsnummer"
- Always set `IdentifierType` to `Foedselsnummer`

**`NoDNummer`** (Restricted):
- Accept only day range 41–71 (D-nummer offset)
- Reject if day field is 01–31 (fødselsnummer days)
- Return error: "Value is a valid fødselsnummer, not a D-nummer"
- Always set `IdentifierType` to `DNummer`

#### 7. Union Type Enhancements

Extend the union types from S0029 to support identifier-specific rejection:

**Option A: Reuse existing `InvalidIdentifierType` case** (if it exists)
- Use existing union case with contextual message differentiating fødselsnummer vs. D-nummer

**Option B: Add new union cases** (if more granularity needed)
```csharp
public record IsFoedselsnummerRequired(String Message);
public record IsDNummerRequired(String Message);
```

**Decision**: To be made during implementation. Recommendation is Option A (reuse existing case) unless more granular error reporting is desired.

#### 8. Conversion and Compatibility

Implement conversion operators where appropriate:

```csharp
// NoFoedselsnummer <-> NoIdentityNumber
public static implicit operator NoIdentityNumber(NoFoedselsnummer foedselsnummer);
public static explicit operator NoFoedselsnummer(NoIdentityNumber identity);

// NoDNummer <-> NoIdentityNumber
public static implicit operator NoIdentityNumber(NoDNummer dnummer);
public static explicit operator NoDNummer(NoIdentityNumber identity);

// No direct conversion between NoFoedselsnummer and NoDNummer (type-incompatible)
```

**Rationale:**
- Implicit conversion from restricted to base (always safe; restricted type is a strict subset)
- Explicit conversion from base to restricted (requires validation; may fail)
- No direct conversion between restricted types (ensures type safety)

#### 9. NoIdentifierType Alignment

The existing `NoIdentifierType` markers are already defined:

- Current: `NoIdentifierType.Foedselsnummer` and `NoIdentifierType.DNummer`
- No API changes required; marker types remain as-is
- May add documentation linking markers to restricted type usage

#### 10. JSON Serialization

Ensure each type has appropriate JSON converter:

- **`NoIdentityNumber`**: Uses existing logic (accepts both types)
- **`NoFoedselsnummer`**: Uses derived converter (restricts to fødselsnummer during deserialization)
- **`NoDNummer`**: Uses derived converter (restricts to D-nummer during deserialization)

**Implementation Strategy:**
- Create a single converter base or factory that handles common serialization
- Each type can override or extend converter as needed
- Ensure deserialization respects type restrictions (fail gracefully if wrong category)

---

### Non-Functional Requirements

#### 1. Backward Compatibility

**Migration Path:**
- `NoIdentityNumber` is the primary implementation (contains all current logic from `NoFoedselsnummer`)
- Existing code that uses `NoFoedselsnummer` continues to work if `NoFoedselsnummer` becomes an alias or derives from `NoIdentityNumberBase`
- Deprecation strategy (to be finalized during sprint planning):
  - **Option A**: Mark old `NoFoedselsnummer` name as `[Obsolete]` after a grace period
  - **Option B**: Create compiler-version-specific aliases for backward compatibility
  - **Option C**: Use a major version bump to cleanly break

**Recommended:** Option A for gradual deprecation over 2–3 releases

#### 2. Code Organization

**File Structure:**
```
src/KfAccountNumbers/Governmental/Europe/
  ├── NoIdentityNumberBase.cs          (New: abstract base class)
  ├── NoIdentityNumber.cs              (Renamed: current NoFoedselsnummer logic)
  ├── NoFoedselsnummer.cs              (New: restricted to fødselsnummer)
  ├── NoDNummer.cs                     (New: restricted to D-nummer)
  ├── NoIdentifierType.cs              (Existing: type markers)
  ├── NoIdentityNumberJsonConverter.cs (New/Renamed: base converter)
  ├── NoFoedselsnummerJsonConverter.cs (Renamed: may become wrapper)
  └── NoDNummerJsonConverter.cs        (New: if needed)
```

**Alternative (Minimal Files):**
- Keep all types in `NoIdentityNumberBase.cs` (similar to some GB implementations)
- Keep JSON converters in single file with factory/dispatch pattern

**Decision:** Separate files recommended for clarity and single-responsibility principle

#### 3. Naming Conventions

- **Base class**: `NoIdentityNumberBase` (prefix `No` + suffix `Base`)
- **Unrestricted type**: `NoIdentityNumber` (handles both)
- **Restricted types**: `NoFoedselsnummer`, `NoDNummer` (names indicate specific category)
- **Consistency**: Follow GB naming (`GbPatientNumberBase`, `GbNhsNumber`, `GbChiNumber`) and Swedish naming (S0038)

#### 4. Error Messaging

All error messages stored in `Messages` class with new keys:

- `NoIdentityNumber_*` (base class errors)
- `NoFoedselsnummer_*` (fødselsnummer-specific errors, including "is D-nummer" rejection)
- `NoDNummer_*` (D-nummer-specific errors, including "is fødselsnummer" rejection)

Example messages:
- `NoFoedselsnummer_IsNotFoedselsnummer`: "The value is a valid D-nummer, not a fødselsnummer."
- `NoDNummer_IsNotDNummer`: "The value is a valid fødselsnummer, not a D-nummer."

#### 5. Test Organization

**Test Files:**
```
tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/
  ├── NoIdentityNumberTests.cs         (Renamed: covers base class and unrestricted logic)
  ├── NoFoedselsnummerTests.cs         (New: tests fødselsnummer-specific restrictions)
  ├── NoDNummerTests.cs                (New: tests D-nummer-specific restrictions)
```

**Alternative (Minimal Tests):**
- Keep all tests in single expanded `NoIdentityNumberTests.cs` file with region organization
- Use parameterized tests to avoid duplication

**Recommended:** Separate test files with focused coverage

#### 6. Documentation Updates

- Update README.md to document the new type hierarchy
- Add examples showing when to use `NoIdentityNumber` vs. `NoFoedselsnummer` vs. `NoDNummer`
- Link to this story (S0039), prior migration story (S0029), and parallel Swedish story (S0038)
- Document conversion operators and implicit/explicit rules
- Provide migration guide for existing code

---

## Acceptance Criteria

### Definition of Done

- [ ] **Base class created**: `NoIdentityNumberBase` abstract record implemented with all shared validation logic, properties, and methods.

- [ ] **NoIdentityNumber type created**: Current `NoFoedselsnummer` logic migrated to new `NoIdentityNumber` type; accepts both fødselsnummer and D-nummer.

- [ ] **NoFoedselsnummer restricted type created**: New `NoFoedselsnummer` type accepts only fødselsnummer values (day 01–31); rejects D-nummer with appropriate error.

- [ ] **NoDNummer restricted type created**: New `NoDNummer` type accepts only D-nummer values (day 41–71); rejects fødselsnummer with appropriate error.

- [ ] **Validation logic preserved**: All validation rules from union migration (S0029) maintained; identifier-specific restrictions added.

- [ ] **Check digit validation**: Weighted modulus 11 validation with two check digits (C1 and C2) correctly implemented and preserved.

- [ ] **Conversion operators implemented**: Implicit from restricted to base; explicit from base to restricted; no direct restricted-to-restricted conversions.

- [ ] **Union types aligned**: `ValidationError` and `ValidationResult` unions updated to support identifier-specific rejection cases (or appropriately reused).

- [ ] **JSON serialization updated**: All three types have working JSON converters; deserialization respects type restrictions.

- [ ] **Error messages added**: All required message keys created in `Messages` class with appropriate localization.

- [ ] **NoIdentifierType alignment verified**: Type markers (`Foedselsnummer`, `DNummer`) work correctly with all three types.

- [ ] **Existing tests updated**: All tests in `NoFoedselsnummerTests.cs` pass with new type hierarchy; minimal changes to test code required.

- [ ] **New test coverage added**: Separate test files for `NoFoedselsnummer` and `NoDNummer` covering type-specific restrictions and error cases.

- [ ] **Backward compatibility maintained**: Code using old `NoFoedselsnummer` API continues to work (either via alias, inheritance, or deprecation strategy).

- [ ] **Documentation updated**: README.md, XML comments, and migration guide document new type hierarchy with usage examples.

- [ ] **Code review approved**: Architecture reviewed for consistency with GB patient number hierarchy, Swedish identifier hierarchy (S0038), and other patterns in codebase.

- [ ] **Build succeeds**: No compilation errors or warnings; all tests pass.

- [ ] **No performance regression**: Validation performance equivalent to or better than pre-refactoring.

---

## Implementation Strategy

### Phase 1: Foundation (Base Class)

1. Extract common logic into `NoIdentityNumberBase` abstract record
2. Implement shared validation methods and properties
3. Define abstract/virtual methods for derived types to override
4. Ensure union types from S0029 are available for inheritance
5. Implement weighted modulus 11 check digit calculation (two check digits)

**Deliverable:** `NoIdentityNumberBase.cs` with comprehensive implementation

### Phase 2: Unrestricted Type (NoIdentityNumber)

1. Create `NoIdentityNumber.cs` by refactoring current `NoFoedselsnummer` implementation
2. Inherit from `NoIdentityNumberBase`
3. Implement category discrimination logic (both types accepted)
4. Verify all current validation logic works unchanged
5. Create corresponding JSON converter if needed

**Deliverable:** `NoIdentityNumber.cs` with all current `NoFoedselsnummer` behavior

### Phase 3: Restricted Types (NoFoedselsnummer and NoDNummer)

1. Create `NoFoedselsnummer.cs` inheriting from `NoIdentityNumberBase`
   - Implement fødselsnummer-only validation
   - Add check: day field must be 01–31 (not 41–71)
   - Return appropriate error if D-nummer detected

2. Create `NoDNummer.cs` inheriting from `NoIdentityNumberBase`
   - Implement D-nummer-only validation
   - Add check: day field must be 41–71 (not 01–31)
   - Return appropriate error if fødselsnummer detected

3. Add JSON converters for both types

**Deliverables:** `NoFoedselsnummer.cs`, `NoDNummer.cs`, JSON converters

### Phase 4: Conversion and Interoperability

1. Implement conversion operators (implicit from restricted to base; explicit from base to restricted)
2. Add documentation for conversion semantics
3. Ensure serialization/deserialization respects type boundaries

**Deliverable:** Conversion operators with comprehensive tests

### Phase 5: Testing and Validation

1. Create test files: `NoFoedselsnummerTests.cs`, `NoDNummerTests.cs`
2. Migrate existing `NoFoedselsnummerTests.cs` to `NoIdentityNumberTests.cs`
3. Add tests for type-specific restrictions
4. Add tests for conversion operators
5. Ensure all union validation cases are exercised
6. Verify weighted modulus 11 check digit validation

**Deliverable:** Three comprehensive test files with 100% validation coverage

### Phase 6: Documentation and Migration

1. Update README.md with new type hierarchy
2. Add usage examples for each type
3. Provide migration guide for code using old `NoFoedselsnummer`
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
                    NoIdentityNumberBase (abstract record)
                            ▲
                ┌───────────┼───────────┐
                │           │           │
          NoIdentityNumber   │           │
      (both types accepted)   │           │
                        NoFoedselsnummer  NoDNummer
                    (day 01-31 only)     (day 41-71 only)

Conversion Operators:
- NoFoedselsnummer → NoIdentityNumber (implicit)
- NoDNummer → NoIdentityNumber (implicit)
- NoIdentityNumber → NoFoedselsnummer (explicit, may fail)
- NoIdentityNumber → NoDNummer (explicit, may fail)
- No direct NoFoedselsnummer ↔ NoDNummer
```

---

## Design Pattern Reference

This refactoring closely follows the pattern established in S0038 (Swedish identifiers) and the GB patient number hierarchy:

| GB Pattern | Swedish Pattern (S0038) | Norwegian Pattern | Purpose |
|-----------|------------------------|-------------------|---------|
| `GbPatientNumberBase` | `SeIdentityNumberBase` | `NoIdentityNumberBase` | Abstract base with shared logic |
| `GbNhsNumber` | `SeIdentityNumber` | `NoIdentityNumber` | Unrestricted (both categories) |
| `GbChiNumber` | `SePersonnummer` | `NoFoedselsnummer` | Restricted (first category) |
| `GbHcNumber` | `SeSamordningsnummer` | `NoDNummer` | Restricted (second category) |

**Key Similarities:**
- Abstract base class encapsulates common validation and formatting
- Multiple derived types each representing a distinct category
- Conversion operators enable safe type interoperability
- Each type includes category-specific discrimination logic
- Separate test files verify type-specific behavior

**Reference Files:**
- `src/KfAccountNumbers/Governmental/Europe/GbPatientNumberBase.cs` (475 lines)
- `src/KfAccountNumbers/Governmental/Europe/GbNhsNumber.cs` (404 lines)
- `src/KfAccountNumbers/Governmental/Europe/GbChiNumber.cs` (similar)
- `src/KfAccountNumbers/Governmental/Europe/GbHcNumber.cs` (similar)
- `Documentation/Stories/Backlog/S0038-SePersonnummerTypeHierarchy.md` (template)

---

## Validation Rules Summary

All validation rules from S0029 (union migration) preserved and extended:

### Shared Rules (All Types)

1. **Null/Empty**: Value may not be null, empty, or all whitespace → `EmptyValue`
2. **Length**: Value must be 11 or 12 characters (unformatted or formatted) → `InvalidLength`
3. **Separator**: Non-digit character at position 6 (between DDMMYY and IIICC) → `InvalidSeparator`
4. **Characters**: All non-separator characters must be ASCII digits → `InvalidCharacter`
5. **Date**: First 6 digits must represent valid calendar date (DDMMYY format) → `InvalidDateOfBirth`
6. **Individual Number**: Middle 3 digits must be 001–999 (not 000) → `InvalidBirthSerialNumber`
7. **Check Digits**: Last 2 digits must be valid weighted modulus 11 check digits → `InvalidChecksum`

### NoIdentityNumber (Unrestricted)

8. **Both categories accepted**: Day field may be 01–31 (fødselsnummer) or 41–71 (D-nummer)
9. **IdentifierType determined by day field**: If day ≤ 31 → Foedselsnummer; if day ≥ 41 → DNummer

### NoFoedselsnummer (Restricted to Fødselsnummer)

8. **Fødselsnummer only**: Day field must be 01–31 (not 41–71)
   - Reject if day field indicates D-nummer (41–71)
   - Error: "Value is a valid D-nummer, not a fødselsnummer" → `InvalidIdentifierType`

### NoDNummer (Restricted to D-Nummer)

8. **D-nummer only**: Day field must be 41–71 (not 01–31)
   - Reject if day field indicates fødselsnummer (01–31)
   - Error: "Value is a valid fødselsnummer, not a D-nummer" → `InvalidIdentifierType`

---

## Migration Guide for Existing Code

### Before (Current NoFoedselsnummer)

```csharp
// Current code using NoFoedselsnummer (accepts both types)
public void ProcessIdentity(NoFoedselsnummer identity)
{
    if (identity.IdentifierType == NoIdentifierType.Foedselsnummer)
    {
        SendToFoedselsnummerSystem(identity.Value);
    }
    else if (identity.IdentifierType == NoIdentifierType.DNummer)
    {
        SendToDNummerSystem(identity.Value);
    }
}
```

### After (Using Type-Specific API)

**Option A: Gradual Migration (Using NoIdentityNumber Base)**
```csharp
// Refactored to accept base type (still works with all identities)
public void ProcessIdentity(NoIdentityNumber identity)
{
    // Logic remains the same; type system now enforces the hierarchy
}
```

**Option B: Type-Safe Branch (Specific Types)**
```csharp
// New approach: separate methods for each type
public void ProcessIdentity(NoFoedselsnummer foedselsnummer)
{
    SendToFoedselsnummerSystem(foedselsnummer.Value);
}

public void ProcessIdentity(NoDNummer dnummer)
{
    SendToDNummerSystem(dnummer.Value);
}
```

**Option C: Pattern Matching (Best of Both Worlds)**
```csharp
// Type-safe pattern matching on base type
public void ProcessIdentity(NoIdentityNumber identity)
{
    _ = identity switch
    {
        NoFoedselsnummer f => SendToFoedselsnummerSystem(f.Value),
        NoDNummer d => SendToDNummerSystem(d.Value),
        _ => throw new ArgumentException("Unknown Norwegian identity type"),
    };
}
```

### Constructor Usage

**Before:**
```csharp
try
{
    var identity = new NoFoedselsnummer("13029597140");
    // Works for both fødselsnummer and D-nummer
}
catch (UKfValidationException<NoFoedselsnummer.ValidationError> ex)
{
    // Handle error
}
```

**After – Option A (Base Type, Most Compatible):**
```csharp
try
{
    var identity = new NoIdentityNumber("13029597140");
    // Works for both (same behavior)
}
catch (UKfValidationException<NoIdentityNumber.ValidationError> ex)
{
    // Handle error
}
```

**After – Option B (Restricted Type, Type-Safe):**
```csharp
try
{
    var identity = new NoFoedselsnummer("13029597140");
    // Throws if value is D-nummer
}
catch (UKfValidationException<NoFoedselsnummer.ValidationError> ex)
{
    // Handle error (now guaranteed to be fødselsnummer or invalid)
}
```

**After – Option C (Explicit Conversion):**
```csharp
var identity = new NoIdentityNumber("13029597140");
var foedselsnummer = (NoFoedselsnummer)identity;
// Throws if identity is D-nummer
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

1. **Breaking Change Risk**: Renaming `NoFoedselsnummer` to `NoIdentityNumber` breaks existing code
   - **Mitigation**: Provide clear migration path; consider deprecation grace period or compiler aliases

2. **Complexity of Base Class**: Extracting shared logic without introducing subtle bugs
   - **Mitigation**: Comprehensive test suite with 1:1 coverage of current tests plus new restrictions

### Medium Risks

3. **Performance Impact**: Inheritance hierarchy may add indirection
   - **Mitigation**: Benchmark and optimize; likely negligible for record types

4. **JSON Serialization Complexity**: Three types with validation restrictions requires careful converter design
   - **Mitigation**: Base converter with derived specialization; thorough round-trip testing

5. **Check Digit Implementation**: Weighted modulus 11 with two check digits is complex; extraction must preserve behavior
   - **Mitigation**: Detailed review of check digit logic; extensive parametrized tests

### Low Risks

6. **Documentation Gaps**: Developers unfamiliar with type hierarchy may misuse API
   - **Mitigation**: Comprehensive README updates, examples, XML comments, and migration guide

---

## Definition of Ready

Before implementation:

- [ ] S0029 (NoFoedselsnummerUnionMigration) successfully completed and deployed
- [ ] S0038 (SePersonnummerTypeHierarchy) completed or nearing completion (for reference)
- [ ] Current `NoFoedselsnummer.cs` implementation fully reviewed and validated
- [ ] GB patient number base class (`GbPatientNumberBase`, `GbNhsNumber`, `GbChiNumber`) examined and understood
- [ ] Union types from S0029 available and stable
- [ ] Team consensus on type hierarchy design and naming conventions
- [ ] Backward compatibility strategy agreed upon (deprecation timeline, alias approach, etc.)
- [ ] Test infrastructure reviewed and ready for new test files
- [ ] Messages class confirmed available for new error message keys
- [ ] Documentation template (README style, XML comment format) finalized

---

## Dependencies

- **S0029**: NoFoedselsnummerUnionMigration (must be completed first; provides union types)
- **S0038**: SePersonnummerTypeHierarchy (parallel effort; reference for pattern consistency)
- **S0019**, **S0022-S0027**: Previous union migrations (reference implementations)
- **GB Patient Number Stories** (S0019, S0020, S0021): Base class pattern reference
- **Messages class**: For error message keys
- **NoIdentifierType.cs**: Type marker definitions (already exists; no changes needed)

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

- `src/KfAccountNumbers/Governmental/Europe/NoIdentityNumberBase.cs` – Abstract base class
- `src/KfAccountNumbers/Governmental/Europe/NoIdentityNumber.cs` – Renamed current implementation
- `src/KfAccountNumbers/Governmental/Europe/NoFoedselsnummer.cs` – Restricted to fødselsnummer
- `src/KfAccountNumbers/Governmental/Europe/NoDNummer.cs` – Restricted to D-nummer
- `src/KfAccountNumbers/Governmental/Europe/NoIdentityNumberJsonConverter.cs` – Base converter
- `src/KfAccountNumbers/Governmental/Europe/NoFoedselsnummerJsonConverter.cs` – Derived/specialized converter
- `src/KfAccountNumbers/Governmental/Europe/NoDNummerJsonConverter.cs` – Derived/specialized converter
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/NoIdentityNumberTests.cs` – Base/unrestricted tests
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/NoFoedselsnummerTests.cs` – Fødselsnummer-specific tests
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/NoDNummerTests.cs` – D-nummer-specific tests

### Modify (Existing Files)

- `src/KfAccountNumbers/Governmental/Europe/NoIdentifierType.cs` – Minor updates if needed
- `README.md` – Add section on Norwegian identity types and type hierarchy
- `Messages` class – Add new error message keys for type-specific restrictions
- `KfAccountNumbers.csproj` – Verify no changes needed

### Delete (If Applicable)

- Old `NoFoedselsnummer.cs` (after migration to `NoIdentityNumber.cs`)
- Old `NoFoedselsnummerTests.cs` (after migration to `NoIdentityNumberTests.cs`)
- Old JSON converter (if fully refactored into base class or new naming)

---

## Notes

- This story builds directly on S0029 (NoFoedselsnummerUnionMigration), so union types must be stable
- S0038 (Swedish identifiers) establishes pattern precedent; maintain consistency while adapting for Norwegian context
- The GB patient number hierarchy is a proven pattern; this story applies the same principles to Norwegian identifiers
- The type hierarchy enables gradual migration: code using `NoIdentityNumber` can coexist with code using specific types
- Conversion operators ensure safe interoperability between types
- Testing strategy should verify both that restrictions work (e.g., `NoFoedselsnummer` rejects D-nummer) and that base behavior is preserved
- Weighted modulus 11 check digit logic is complex; pay special attention to C1 and C2 calculation during extraction
- Consider releasing as a minor version (new API, non-breaking if `NoIdentityNumber` is default choice) or major version (if `NoFoedselsnummer` name change is treated as breaking)
- Coordinate with S0038 release to maintain parallel evolution of identifier type systems
