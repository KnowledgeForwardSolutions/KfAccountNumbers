# S0040: Implement NoHnummer Type and Extend NoIdentityNumber Hierarchy

**Status:** Backlog  
**Priority:** High  
**Effort:** 8 points  
**Related:** S0029 (NoFoedselsnummerUnionMigration), S0039 (NoFoedselsnummerTypeHierarchy), S0038 (SePersonnummerTypeHierarchy), S0019-S0027 (Union migrations), E01-UnionMigration

---

## Overview

Extend the Norwegian identity number system to support a third category: H-numbers (householder numbers, Norwegian: "H-nummer"). Implement a new `NoHnummer` restricted type and extend the `NoIdentityNumberBase` hierarchy to accept and discriminate all three Norwegian identity categories:

1. **Fødselsnummer**: Birth number for Norwegian citizens and long-term residents (day 01–31)
2. **D-nummer**: Identifier for foreign individuals not eligible for fødselsnummer (day 41–71, +40 offset to day component of date)
3. **H-nummer**: Householder number for temporary Norwegian residents (month 41-52, +40 offset to month component of date)

This story builds upon the type hierarchy established in S0039 by adding a fourth type (`NoHnummer`) to the existing three-tier structure, creating a four-tier hierarchy:

```
NoIdentityNumberBase (abstract base)
├── NoIdentityNumber (unrestricted, all three categories)
├── NoFoedselsnummer (restricted to fødselsnummer, day 01–31)
├── NoDNummer (restricted to D-nummer, day 41–71)
└── NoHnummer (restricted to H-nummer, month 41-52)
```

---

## Business Value

* **Complete Coverage**: Supports all Norwegian identity number categories, not just the two most common ones.
* **Compliance**: Ensures systems can properly handle and validate H-numbers used by temporary residents and special populations.
* **Type Safety**: Extends compile-time type discrimination to include the third category, maintaining the type-safety benefits of the S0039 hierarchy.
* **API Consistency**: Provides parallel APIs for all three categories (fødselsnummer, D-nummer, H-nummer), improving developer experience.
* **Domain Clarity**: Better represents the complete Norwegian identity landscape, where H-numbers are issued by Skatteetaten (Norwegian Tax Authority) for specific use cases.
* **Validation Robustness**: Ensures values are categorized correctly and rejected if not matching the expected category.
* **Future-Proofing**: Demonstrates scalability of the base class pattern to accommodate additional identity types if needed.

---

## Requirements

### Functional Requirements

#### 1. H-Number Format and Structure

H-numbers follow the same format as fødselsnummer and D-nummer:

- **Format**: DDMMYYIIICC (11 digits total)
- **Day Component (DD)**: 01-31 (valid calendar days)
- **Month Component (MM)**: 41–52 (valid calendar months with +40 offset)
- **Year Component (YY)**: Two-digit year (century derived from individual number)
- **Individual Number (III)**: Three-digit birth serial number (001–999, never 000)
- **Check Digits (CC)**: Two weighted modulus 11 check digits (C1 and C2)

**Examples:**
```
H-number: 81020540987 (original birth day 01, month 02, year 05, individual 409, check 87)
H-number: 850203-40987 (with separator; original birth day 05, month 02, year 85)
```

#### 2. NoHnummer Type Implementation

Create a new restricted type that accepts only H-number values:

```csharp
public record NoHnummer : NoIdentityNumberBase
{
    // Accepts ONLY H-nummer values
    // Rejects Fødselsnummer (day field 01–31)
    // Rejects D-nummer (day field 41–71)
    // Throws or returns error if constructor receives other categories
}
```

**Behavior:**
- Constructor throws `UKfValidationException<ValidationError>` if value is not an H-number
- `IdentifierType` property always returns `Hnummer`
- Month field must be in range 41-52
- Used for APIs requiring H-numbers specifically
- Returns error: "Value is a valid [other category], not an H-nummer" if wrong category detected

**Validation Enhancement:**
- Add validation rule: "If identifier would be categorized as fødselsnummer or D-nummer, reject with error"
- Use existing union discriminator or add new case if needed

#### 3. Extend NoIdentifierType

Update the `NoIdentifierType` class to include H-number marker:

```csharp
public class NoIdentifierType
{
    /// <summary>
    ///   Personal identity number, issued to citizens or long-term residents of Norway.
    /// </summary>
    public struct Foedselsnummer { }

    /// <summary>
    ///   Identifier issued to foreign individuals not eligible for fødselsnummer.
    ///   Same format as a fødselsnummer, except 40 is added to the day component.
    /// </summary>
    public struct DNummer { }

    /// <summary>
    ///   Householder number issued to temporary Norwegian residents.
    ///   Same format as a fødselsnummer, except 80 is added to the day component.
    /// </summary>
    public struct Hnummer { }
}
```

**No breaking changes**: Addition of new marker struct is backward compatible.

#### 4. Extend NoIdentityNumber

Update the unrestricted `NoIdentityNumber` type to accept all three categories:

```csharp
public record NoIdentityNumber : NoIdentityNumberBase
{
    // Accepts all three: Fødselsnummer, D-nummer, and H-nummer
    // IdentifierType property returns appropriate category based on day field
    // Constructor and validation updated to support day 81–99 range
}
```

**Behavior:**
- Accept any valid Norwegian identity number (all three categories)
- `IdentifierType` property returns:
  - `Foedselsnummer` if day 01–31
  - `DNummer` if day 41–71
  - `Hnummer` if month 41-52
- Validation logic updated to accept all three ranges

#### 5. Day Field Validation Rules

Extend validation to support three distinct day field ranges:

**Current Validation (S0029/S0039):**
- Fødselsnummer: day 01–31
- D-nummer: day 41–71 (derived from original day 01–31 by adding 40)
- Invalid: day 32–40 and day 72–80 (gap ranges, never valid)

**Extended Validation (S0040):**
- Fødselsnummer: day 01–31
- D-nummer: day 41–71 (derived from original day 01–31 by adding 40)
- H-nummer: day 81–99 (derived from original day 01–19 by adding 80)
- Invalid: day 32–40 and day 72–80 (gap ranges, never valid)

**Day Field Discrimination Logic:**
```
if (day >= 1 && day <= 31)   → Fødselsnummer
else if (day >= 41 && day <= 71)  → D-nummer
else if (day >= 81 && day <= 99)  → H-nummer
else                         → Invalid (days 32–40 or 72–80)
```

#### 6. Validation Rules for H-Numbers

All validation rules remain the same as for fødselsnummer and D-nummer:

- **Null/Empty**: Value may not be null, empty, or all whitespace
- **Length**: 11 characters (unformatted) or 12 characters (with separator)
- **Separator**: Optional non-digit character at position 6
- **Characters**: All non-separator characters must be ASCII digits
- **Date Validity**: First 6 digits must represent valid calendar date (day 01–19 after offset removal)
- **Individual Number**: Middle 3 digits must be 001–999
- **Check Digits**: Last 2 digits must be valid weighted modulus 11 check digits

**H-Number Specific:**
- Original day must be 01–19 (after removing +80 offset, day becomes 01–19)
- If day > 99, treat as invalid (not an H-number)

#### 7. Century Derivation

Century derivation rules apply equally to H-numbers as to other Norwegian identity types:

The century is derived from the individual number using the same rules as fødselsnummer and D-nummer. H-numbers use identical logic:

- **Rule 1**: Individual number 500–749 and year ≥ 54 → 1800s
- **Rule 2**: Individual number < 500 (year not considered) → 1900s
- **Rule 3**: Individual number ≥ 900 and year ≥ 40 → 1900s
- **Rule 4**: Individual number ≥ 500 and year ≤ 39 → 2000s

No changes to century derivation; H-numbers follow identical rules.

#### 8. Conversion and Compatibility

Extend conversion operators to include H-numbers:

```csharp
// NoFoedselsnummer <-> NoIdentityNumber (unchanged)
public static implicit operator NoIdentityNumber(NoFoedselsnummer foedselsnummer);
public static explicit operator NoFoedselsnummer(NoIdentityNumber identity);

// NoDNummer <-> NoIdentityNumber (unchanged)
public static implicit operator NoIdentityNumber(NoDNummer dnummer);
public static explicit operator NoDNummer(NoIdentityNumber identity);

// NoHnummer <-> NoIdentityNumber (new)
public static implicit operator NoIdentityNumber(NoHnummer hnummer);
public static explicit operator NoHnummer(NoIdentityNumber identity);

// No direct conversions between restricted types
```

**Rationale:**
- Implicit conversion from `NoHnummer` to `NoIdentityNumber` (always safe)
- Explicit conversion from `NoIdentityNumber` to `NoHnummer` (requires validation; may fail)
- No conversions between `NoFoedselsnummer`, `NoDNummer`, and `NoHnummer`

#### 9. JSON Serialization

Ensure H-number JSON serialization works correctly:

- **`NoHnummer`**: Uses derived converter (restricts to H-numbers during deserialization)
- **`NoIdentityNumber`**: Existing logic (accepts all three categories)
- **Validation**: Deserialization respects type restrictions (fail gracefully if wrong category)

**Implementation:**
- Create `NoHnummerJsonConverter.cs` for H-number specific serialization
- Update `NoIdentityNumberJsonConverter.cs` to handle all three categories
- All converters inherit from common base if using factory pattern

#### 10. Union Type Alignment

Ensure union validation types support H-number-specific errors:

**Option A: Reuse existing `InvalidIdentifierType` case**
- Use contextual error messages distinguishing between all three categories

**Option B: Add new union cases** (if granular reporting desired)
```csharp
public record IsFoedselsnummerRequired(String Message);
public record IsDNummerRequired(String Message);
public record IsHnummerRequired(String Message);
```

**Recommendation:** Option A (reuse existing case) unless more granular error reporting is desired.

---

### Non-Functional Requirements

#### 1. Backward Compatibility

**No Breaking Changes:**
- Adding `NoHnummer` type does not affect existing code using `NoFoedselsnummer` or `NoDNummer`
- Adding `Hnummer` marker to `NoIdentifierType` is backward compatible (new struct, no changes to existing ones)
- Extending `NoIdentityNumber` to accept H-numbers is backward compatible if validation is defensive

**Deprecation:**
- If S0039 has deprecated old `NoFoedselsnummer` name, continue deprecation strategy
- No new deprecations introduced in this story

#### 2. Code Organization

**File Structure Additions:**
```
src/KfAccountNumbers/Governmental/Europe/
  ├── NoHnummer.cs                     (New: H-number specific type)
  ├── NoHnummerJsonConverter.cs        (New: H-number JSON converter)
  ├── NoIdentifierType.cs              (Modify: add Hnummer marker)
  ├── NoIdentityNumber.cs              (Modify: update validation)
  ├── NoIdentityNumberBase.cs          (Modify if needed: day range validation)
  ├── NoIdentityNumberJsonConverter.cs (Modify: handle three categories)
  └── [other files unchanged]
```

**Minimal Changes Approach:**
- Leverages existing base class design from S0039
- Adds one new type file and one new converter file
- Minimal modifications to existing files

#### 3. Error Messaging

New error message keys:

- `NoHnummer_IsNotHnummer`: "The value is a valid [category], not an H-nummer."
- `NoIdentityNumber_InvalidDayRange`: (if needed) "Day field [value] is not valid for any Norwegian identity number."

Existing messages unchanged; new messages use consistent naming convention.

#### 4. Testing

**Test Coverage:**
- H-number specific tests in `NoHnummerTests.cs`
- Updated tests in `NoIdentityNumberTests.cs` to cover H-number discrimination
- Conversion operator tests including H-number conversions
- JSON round-trip tests for H-numbers
- Day field range validation tests (including gap ranges 32–40 and 72–80)

#### 5. Documentation Updates

- Update README.md to mention H-numbers alongside fødselsnummer and D-nummer
- Add H-number examples to documentation
- Document day field discrimination logic (01–31, 41–71, 81–99)
- Link to S0040 story

---

## Acceptance Criteria

### Definition of Done

- [ ] **NoHnummer type created**: New `NoHnummer` record type accepts only H-numbers (day 81–99).

- [ ] **Validation logic implemented**: Day field 81–99 recognized as H-number; other ranges correctly identified.

- [ ] **NoIdentifierType updated**: `Hnummer` marker struct added; existing markers unchanged.

- [ ] **NoIdentityNumber extended**: Accepts all three categories; `IdentifierType` property discriminates correctly.

- [ ] **Conversion operators added**: Implicit `NoHnummer` → `NoIdentityNumber`; explicit reverse.

- [ ] **JSON serialization working**: `NoHnummerJsonConverter` created; round-trip tests pass.

- [ ] **Error messages added**: `NoHnummer_IsNotHnummer` and other required keys in `Messages` class.

- [ ] **Day field validation tested**: Tests verify all ranges (01–31, 41–71, 81–99) and gap ranges (32–40, 72–80).

- [ ] **Century derivation verified**: H-numbers correctly derive century using existing rules.

- [ ] **Backward compatibility verified**: Existing code using `NoFoedselsnummer` and `NoDNummer` continues to work.

- [ ] **Test coverage complete**: `NoHnummerTests.cs` and updated `NoIdentityNumberTests.cs` cover all scenarios.

- [ ] **Documentation updated**: README and XML comments explain H-numbers and day field discrimination.

- [ ] **Build succeeds**: No compilation errors or warnings.

- [ ] **All tests pass**: Unit tests pass with no regressions.

- [ ] **Code review approved**: Architecture reviewed for consistency with S0039 pattern.

---

## Implementation Strategy

### Phase 1: Foundation (Extend Type Markers)

1. Add `Hnummer` struct to `NoIdentifierType`
2. Update `NoIdentityNumberBase` day field validation to support 81–99 range
3. Verify validation logic handles gap ranges (32–40, 72–80)

**Deliverable:** Updated `NoIdentifierType.cs` and modified validation in base class

### Phase 2: New NoHnummer Type

1. Create `NoHnummer.cs` inheriting from `NoIdentityNumberBase`
2. Implement H-number specific validation (day 81–99 only)
3. Add check: day field must be 81–99, reject if fødselsnummer or D-nummer

**Deliverable:** `NoHnummer.cs` with comprehensive implementation

### Phase 3: Extend NoIdentityNumber

1. Update `NoIdentityNumber.cs` to accept all three categories
2. Update `IdentifierType` discrimination logic to include H-numbers
3. Extend validation to support day range 81–99

**Deliverable:** Updated `NoIdentityNumber.cs` with three-category support

### Phase 4: JSON Serialization

1. Create `NoHnummerJsonConverter.cs` for H-number serialization
2. Update `NoIdentityNumberJsonConverter.cs` to handle three categories

**Deliverable:** Working JSON converters for all types

### Phase 5: Conversion Operators

1. Add `implicit operator` from `NoHnummer` to `NoIdentityNumber`
2. Add `explicit operator` from `NoIdentityNumber` to `NoHnummer`
3. Comprehensive tests for all conversion scenarios

**Deliverable:** Working conversion operators with full test coverage

### Phase 6: Testing and Validation

1. Create `NoHnummerTests.cs` with H-number specific tests
2. Update `NoIdentityNumberTests.cs` to cover H-number discrimination
3. Add tests for day field gap ranges (32–40, 72–80)
4. Add tests for century derivation with H-numbers
5. Add conversion operator tests

**Deliverable:** Comprehensive test coverage (95+ new tests)

### Phase 7: Documentation and Integration

1. Update README.md with H-number section
2. Add error messages to `Messages` class
3. Update XML comments in code
4. Final build validation

**Deliverable:** Documented, tested, and validated implementation

---

## Type Hierarchy Diagram

```
                    NoIdentityNumberBase (abstract record)
                            ▲
                ┌───────────┼──────────┬────────────┐
                │           │          │            │
          NoIdentityNumber   │          │            │
      (all three categories) │          │            │
                        NoFoedselsnummer NoDNummer  NoHnummer
                    (day 01-31)    (day 41-71)    (day 81-99)

Conversion Operators:
- NoFoedselsnummer → NoIdentityNumber (implicit)
- NoDNummer → NoIdentityNumber (implicit)
- NoHnummer → NoIdentityNumber (implicit)
- NoIdentityNumber → NoFoedselsnummer (explicit, may fail)
- NoIdentityNumber → NoDNummer (explicit, may fail)
- NoIdentityNumber → NoHnummer (explicit, may fail)
- No direct conversions between restricted types
```

---

## Day Field Reference

Quick reference for day field discrimination:

| Range | Offset | Original Day | Category | Notes |
|-------|--------|--------------|----------|-------|
| 01–31 | +0 | 01–31 | Fødselsnummer | Birth number |
| 32–40 | N/A | N/A | INVALID | Gap range (never valid) |
| 41–71 | +40 | 01–31 | D-nummer | Foreign individual |
| 72–80 | N/A | N/A | INVALID | Gap range (never valid) |
| 81–99 | +80 | 01–19 | H-nummer | Temporary resident |

**Validation Logic:**
```csharp
switch (dayField)
{
    case >= 1 and <= 31:
        return IdentifierType.Foedselsnummer;
    case >= 41 and <= 71:
        return IdentifierType.DNummer;
    case >= 81 and <= 99:
        return IdentifierType.Hnummer;
    default:
        return InvalidDayRange; // 32–40 or 72–80
}
```

---

## Migration Guide for Existing Code

### Before (If Using Current Code)

```csharp
// Current code only accepts fødselsnummer and D-nummer
public void ProcessIdentity(NoIdentityNumber identity)
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

### After (With H-Number Support)

**Option A: Ignore H-Numbers (No Code Change)**
```csharp
// Code continues to work; ignores new category
public void ProcessIdentity(NoIdentityNumber identity)
{
    if (identity.IdentifierType == NoIdentifierType.Foedselsnummer)
    {
        SendToFoedselsnummerSystem(identity.Value);
    }
    else if (identity.IdentifierType == NoIdentifierType.DNummer)
    {
        SendToDNummerSystem(identity.Value);
    }
    // Implicitly ignores H-numbers
}
```

**Option B: Handle H-Numbers**
```csharp
// Updated to explicitly handle H-numbers
public void ProcessIdentity(NoIdentityNumber identity)
{
    _ = identity.IdentifierType switch
    {
        default(NoIdentifierType.Foedselsnummer) => SendToFoedselsnummerSystem(identity.Value),
        default(NoIdentifierType.DNummer) => SendToDNummerSystem(identity.Value),
        default(NoIdentifierType.Hnummer) => SendToHnummerSystem(identity.Value),
        _ => throw new ArgumentException("Unknown Norwegian identity type"),
    };
}
```

**Option C: Type-Specific APIs**
```csharp
// Use restricted types for specificity
public void ProcessFoedselsnummer(NoFoedselsnummer foedselsnummer)
    => SendToFoedselsnummerSystem(foedselsnummer.Value);

public void ProcessDNummer(NoDNummer dnummer)
    => SendToDNummerSystem(dnummer.Value);

public void ProcessHnummer(NoHnummer hnummer)
    => SendToHnummerSystem(hnummer.Value);
```

---

## Success Metrics

- [ ] All existing tests pass without regression
- [ ] 50+ new tests added for H-number functionality
- [ ] Conversion operators thoroughly tested (implicit/explicit, success/failure)
- [ ] JSON round-trip serialization passes for H-numbers
- [ ] Day field discrimination logic verified for all ranges (01–31, 41–71, 81–99) and gap ranges
- [ ] Documentation clearly explains H-numbers and day field logic
- [ ] Code review approves consistency with S0039 pattern
- [ ] Build passes without warnings or errors
- [ ] Performance benchmarks show no regression
- [ ] Team confirms H-number format and usage rules are correctly implemented

---

## Risk Assessment

### High Risks

1. **Day Field Validation Complexity**: Three distinct ranges plus two gap ranges is complex
   - **Mitigation**: Comprehensive parametrized tests for all ranges; clear validation logic

### Medium Risks

2. **Integration with S0039**: Coordinates with parallel S0039 implementation
   - **Mitigation**: Either build on top of S0039 or implement together

3. **Validation Rule Verification**: Must ensure H-number rules are correctly understood
   - **Mitigation**: Reference official Norwegian Tax Authority documentation; thorough validation tests

### Low Risks

4. **Backward Compatibility**: Adding new type should not break existing code
   - **Mitigation**: Careful design; comprehensive integration tests

---

## Definition of Ready

Before implementation:

- [ ] S0029 (NoFoedselsnummerUnionMigration) successfully deployed
- [ ] S0039 (NoFoedselsnummerTypeHierarchy) completed or in progress
- [ ] Norwegian H-number format and rules verified with official sources
- [ ] Day field discrimination ranges confirmed (01–31, 41–71, 81–99)
- [ ] Team consensus on validation logic and error messages
- [ ] Test infrastructure ready for new test file
- [ ] Messages class confirmed available for new keys
- [ ] Documentation template finalized

---

## Dependencies

- **S0029**: NoFoedselsnummerUnionMigration (provides union types)
- **S0039**: NoFoedselsnummerTypeHierarchy (provides base class structure)
- **Messages class**: For error message keys
- **NoIdentifierType.cs**: Existing file to extend

---

## Stakeholders

- **Developers**: Need clear API for H-number handling
- **Product Owner**: Value of H-number support for Norwegian market
- **QA**: Responsible for comprehensive day field validation testing
- **DevOps**: Release coordination with S0039
- **Documentation**: README and inline documentation updates

---

## Files to Create/Modify

### Create (New Files)

- `src/KfAccountNumbers/Governmental/Europe/NoHnummer.cs` – H-number restricted type
- `src/KfAccountNumbers/Governmental/Europe/NoHnummerJsonConverter.cs` – H-number JSON converter
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/NoHnummerTests.cs` – H-number specific tests

### Modify (Existing Files)

- `src/KfAccountNumbers/Governmental/Europe/NoIdentifierType.cs` – Add `Hnummer` marker struct
- `src/KfAccountNumbers/Governmental/Europe/NoIdentityNumber.cs` – Support H-number category
- `src/KfAccountNumbers/Governmental/Europe/NoIdentityNumberBase.cs` – Extended day range validation
- `src/KfAccountNumbers/Governmental/Europe/NoIdentityNumberJsonConverter.cs` – Support three categories
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/NoIdentityNumberTests.cs` – Add H-number discrimination tests
- `README.md` – Document H-numbers
- `Messages` class – Add H-number error message keys

### No Deletion

- Existing files remain; this is purely additive/extension

---

## Implementation Notes

### Day Field Validation

The key to this implementation is robust day field validation:

1. Extract day field from DDMMYY component
2. Check against three valid ranges: 01–31, 41–71, 81–99
3. If outside all ranges (32–40 or 72–80), reject as invalid
4. Discriminate based on which range matched

### Original Day Recovery for H-Numbers

When validating H-numbers, the original birth day must be recovered:

- H-number day field: 81–99
- Original day: 81 - 80 = 01, 99 - 80 = 19
- Valid original day range: 01–19

Example: H-number 850305-40987 represents original birthday 1985-03-05 (day 05, 05 + 80 = 85)

### Alignment with SePersonnummer

The Swedish SePersonnummer type (S0038) only has two categories (personnummer and samordningsnummer). Norwegian identifiers are more complex with three categories (fødselsnummer, D-nummer, H-nummer). This design accommodates the Norwegian complexity while maintaining parallel structure.

---

## Notes

- This story extends S0039 by adding one more type to the hierarchy
- The base class design from S0039 handles the complexity of three categories
- Adding H-numbers is straightforward given the S0039 foundation
- Consider implementing S0039 and S0040 together for efficiency
- Day field gap ranges (32–40, 72–80) are never valid and must be rejected
- H-numbers represent original birth days 01–19 (offset by +80)
- All three categories use identical weighted modulus 11 check digit algorithm
- All three categories use identical century derivation rules
- Backward compatibility is maintained; existing code continues to work
