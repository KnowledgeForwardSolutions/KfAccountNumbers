# S0041: Implement NoFhnummer Type and Extend NoIdentityNumber Hierarchy

NOTE: AI generated story needs updating since is still considers H-nummer identification to be based on the day component and not the month component

**Status:** Backlog  
**Priority:** High  
**Effort:** 13 points  
**Related:** S0029 (NoFoedselsnummerUnionMigration), S0039 (NoFoedselsnummerTypeHierarchy), S0040 (NoHnummerImplementation), S0038 (SePersonnummerTypeHierarchy), S0019-S0027 (Union migrations), E01-UnionMigration

---

## Overview

Extend the Norwegian identity number system to support a fourth and fundamentally different category: FH-numbers (Felles Hjelpenummer / Shared Help Number), which are used for temporary patient identification in the Norwegian healthcare system. Unlike the previous three categories (fødselsnummer, D-nummer, H-nummer), FH-numbers have a different format, do not encode date of birth, and represent a different validation paradigm.

Implement a new `NoFhnummer` restricted type and extend the `NoIdentityNumberBase` hierarchy to accept and discriminate all four Norwegian identity categories:

1. **Fødselsnummer**: Birth number (day 01–31, date-based)
2. **D-nummer**: Foreign individual (day 41–71, date-based, +40 offset)
3. **H-nummer**: Temporary resident (day 81–99, date-based, +80 offset)
4. **FH-nummer**: Healthcare patient number (starts 8 or 9, NO date encoding, globally unique)

This creates a more complex type hierarchy that must accommodate two distinct validation paradigms: date-based Norwegian identity numbers and non-date-based healthcare identification:

```
NoIdentityNumberBase (abstract base)
├── NoIdentityNumber (unrestricted, all four categories)
├── NoFoedselsnummer (restricted to fødselsnummer, day 01–31)
├── NoDNummer (restricted to D-nummer, day 41–71)
├── NoHnummer (restricted to H-nummer, day 81–99)
└── NoFhnummer (restricted to FH-nummer, starts 8 or 9)
```

---

## Business Value

* **Healthcare System Integration**: Enables proper handling of FH-numbers used throughout the Norwegian healthcare system for patient identification when fødselsnummer/D-nummer/H-nummer are not available.
* **Type Safety**: Compile-time discrimination of FH-numbers from date-based types, ensuring healthcare systems don't accidentally treat FH-numbers as date-based identifiers.
* **Unique Identification**: Supports globally unique healthcare patient identification across all Norwegian institutions (vs. institution-specific H-numbers).
* **Compliance**: Ensures systems comply with Norwegian healthcare regulations requiring support for multiple patient identification types.
* **API Clarity**: Developers can see from the type signature whether code expects date-based identifiers or FH-numbers.
* **Validation Robustness**: Prevents date-validation logic from being mistakenly applied to non-date-based FH-numbers.
* **Complete Coverage**: Provides comprehensive support for all Norwegian identity/identification categories.
* **System Interoperability**: Enables seamless integration with Norwegian healthcare IT systems that use FH-numbers.

---

## Key Distinctions: FH-nummer vs. H-nummer

While both are used in healthcare settings, FH-numbers and H-numbers are fundamentally different:

| Aspect | H-nummer | FH-nummer |
|--------|----------|-----------|
| **Format** | DDMMYYIIICC (date-based) | Non-date format (starts 8 or 9) |
| **Date Encoding** | Yes, encodes birth date | **No**, no date information |
| **Scope** | Institution-specific | **Global (entire healthcare system)** |
| **Uniqueness** | May not be unique across institutions | Guaranteed globally unique |
| **Day Field** | 81–99 (original day 01–19) | N/A (no date component) |
| **Check Digits** | Weighted modulus 11 (C1, C2) | TBD (FH-specific algorithm) |
| **Use Case** | Temporary residents | Temporary patients (missing ID) |
| **Validation** | Date, individual number, century | Non-date validation |
| **First Digit** | 8 (day field starts 81–99) | **8 or 9** (identifier, not date) |

---

## Requirements

### Functional Requirements

#### 1. FH-Number Format and Structure

FH-numbers have a fundamentally different structure than date-based Norwegian identity numbers:

- **First Digit**: 8 or 9 (distinguishes FH-number from date-based types)
- **No Date Encoding**: Unlike fødselsnummer, D-nummer, and H-nummer, FH-numbers do NOT contain encoded date of birth
- **Format**: To be specified (likely different from DDMMYYIIICC)
- **Length**: TBD (may differ from 11 characters)
- **Check Digit Algorithm**: TBD (may differ from weighted modulus 11)
- **Uniqueness**: Globally unique across entire Norwegian healthcare system

**Critical Distinction:**
When analyzing an 11-digit number starting with 8 or 9:
- If day field (second and third digits) represent 81–99 → Likely H-nummer (DDMMYY... format)
- If starting digit is 8 or 9 but NOT in date-based format → Likely FH-nummer (different structure)

**Examples (Placeholder - To Be Confirmed):**
```
FH-nummer: 8XXXXXXXXXX (starts with 8, but not date-based format)
FH-nummer: 9XXXXXXXXXX (starts with 9, but not date-based format)
```

**Implementation Note:**
The exact format specification must be obtained from Norwegian healthcare authorities or official documentation before implementation.

#### 2. NoFhnummer Type Implementation

Create a new restricted type that accepts only FH-number values:

```csharp
public record NoFhnummer : NoIdentityNumberBase
{
    // Accepts ONLY FH-nummer values
    // Rejects all date-based types (Fødselsnummer, D-nummer, H-nummer)
    // Throws or returns error if constructor receives other categories
}
```

**Behavior:**
- Constructor throws `UKfValidationException<ValidationError>` if value is not an FH-number
- `IdentifierType` property always returns `Fhnummer`
- First digit must be 8 or 9
- Used for APIs requiring FH-numbers specifically
- Returns error: "Value is a valid [other category], not an FH-nummer" if wrong category detected
- No date-of-birth validation (FH-numbers don't encode dates)
- Different validation rules than date-based types

**Validation Enhancement:**
- Add validation rule: "If identifier appears to be date-based (fødselsnummer, D-nummer, or H-nummer), reject with error"
- Use appropriate union discriminator or new case

#### 3. First-Digit Discrimination

Implement logic to distinguish FH-numbers from date-based types by first digit and format:

```csharp
// Pseudo-logic for discrimination
if (value[0] is '8' or '9')
{
    if (IsDateBasedFormat(value))
    {
        // If it's in DDMMYY... format, check day field
        int day = int.Parse(value[0..2]);
        if (day is >= 1 and <= 31)      → Fødselsnummer (day field is DD)
        if (day is >= 41 and <= 71)     → D-nummer (day field is DD + 40)
        if (day is >= 81 and <= 99)     → H-nummer (day field is DD + 80)
    }
    else
    {
        // Non-date-based format → Fhnummer
        → Fhnummer
    }
}
```

**Critical Challenge:**
The first digit alone cannot distinguish between H-numbers and FH-numbers because:
- H-number day field starts with 8 (days 81–99)
- FH-number also starts with 8 or 9

**Solution:**
Format validation must determine if the value follows DDMMYYIIICC (date-based) or FH-number format (non-date-based).

#### 4. Extend NoIdentifierType

Update the `NoIdentifierType` class to include FH-number marker:

```csharp
public class NoIdentifierType
{
    /// <summary>
    ///   Personal identity number, issued to citizens or long-term residents of Norway.
    /// </summary>
    public struct Foedselsnummer { }

    /// <summary>
    ///   Identifier issued to foreign individuals not eligible for fødselsnummer.
    /// </summary>
    public struct DNummer { }

    /// <summary>
    ///   Householder number issued to temporary Norwegian residents.
    /// </summary>
    public struct Hnummer { }

    /// <summary>
    ///   Felles Hjelpenummer (Shared Help Number) for healthcare patient identification.
    ///   Globally unique across entire Norwegian healthcare system.
    ///   Does not encode date of birth.
    /// </summary>
    public struct Fhnummer { }
}
```

**No Breaking Changes**: Addition of new marker struct is backward compatible.

#### 5. Extend NoIdentityNumber

Update the unrestricted `NoIdentityNumber` type to accept all four categories:

```csharp
public record NoIdentityNumber : NoIdentityNumberBase
{
    // Accepts all four: Fødselsnummer, D-nummer, H-nummer, and FH-nummer
    // IdentifierType property returns appropriate category based on value structure
    // Constructor and validation updated to support FH-numbers
}
```

**Behavior:**
- Accept any valid Norwegian identity number (all four categories)
- `IdentifierType` property returns:
  - `Foedselsnummer` if date-based format with day 01–31
  - `DNummer` if date-based format with day 41–71
  - `Hnummer` if date-based format with day 81–99
  - `Fhnummer` if non-date-based format starting with 8 or 9
- Validation logic updated to attempt date-based validation first, then fall back to FH-number validation

#### 6. Validation Rules for FH-Numbers

FH-numbers have fundamentally different validation rules than date-based types:

**NOT Applied to FH-Numbers:**
- ❌ Date validation (no DDMMYY component)
- ❌ Century derivation (no date of birth)
- ❌ Day field discrimination (no day component)
- ❌ Weighted modulus 11 with date-based check digits

**Applied to FH-Numbers:**
- ✅ Null/Empty validation
- ✅ Length validation (exact length TBD)
- ✅ First digit check (8 or 9)
- ✅ Character validation (all digits or format-specific rules)
- ✅ Check digit validation (algorithm TBD)
- ✅ Possible range validation (if alphanumeric or mixed format)

**Implementation Note:**
Exact validation rules must be obtained from official FH-number specification before implementation.

#### 7. Architectural Consideration: Dual Validation Paradigm

This story introduces an architectural challenge: the base class must accommodate two distinct validation paradigms:

**Date-Based Types:**
- Fødselsnummer, D-nummer, H-nummer all use date-based validation
- Inherit from `NoIdentityNumberBase` and use date parsing logic
- Weighted modulus 11 check digits

**Non-Date-Based Types:**
- FH-nummer uses different validation
- Inherits from `NoIdentityNumberBase` but bypasses date logic
- Different check digit algorithm (TBD)

**Design Options:**

**Option A: Keep Single Base Class**
```csharp
public abstract record NoIdentityNumberBase
{
    // Contains BOTH date-based and non-date-based validation logic
    // Each derived type calls appropriate validation path
    // Inherits some unused properties (e.g., DateOfBirth for FH-nummer)
}
```
Pros: Simpler hierarchy; all types in one place
Cons: Base class contains unrelated logic; properties meaningless for FH-nummer

**Option B: Create Intermediate Base Classes**
```csharp
public abstract record NoIdentityNumberBase { /* Common */ }
public abstract record NoDateBasedIdentityBase : NoIdentityNumberBase { /* Date logic */ }
public abstract record NoHealthcareIdentityBase : NoIdentityNumberBase { /* Healthcare logic */ }

// Derived types inherit from appropriate base
public record NoFoedselsnummer : NoDateBasedIdentityBase { }
public record NoFhnummer : NoHealthcareIdentityBase { }
```
Pros: Cleaner separation; no mixed concerns
Cons: More complex hierarchy; more files

**Option C: Composition Over Inheritance**
```csharp
public abstract record NoIdentityNumberBase { /* Core */ }
public sealed record NoFhnummer : NoIdentityNumberBase
{
    private NoFhnummerValidator _validator; // Composed validator
}
```
Pros: Complete separation of concerns
Cons: More boilerplate; less reuse

**Recommendation:**
Option B (intermediate base classes) balances clarity, maintainability, and pattern consistency. This story should document the chosen approach and provide rationale.

#### 8. Conversion and Compatibility

Implement conversion operators for FH-numbers:

```csharp
// NoFhnummer <-> NoIdentityNumber
public static implicit operator NoIdentityNumber(NoFhnummer fhnummer);
public static explicit operator NoFhnummer(NoIdentityNumber identity);

// All existing conversions unchanged
```

**Rationale:**
- Implicit conversion from `NoFhnummer` to `NoIdentityNumber` (always safe)
- Explicit conversion from `NoIdentityNumber` to `NoFhnummer` (requires validation; may fail)
- No conversions between date-based types and FH-number (fundamentally different)

#### 9. JSON Serialization

Ensure FH-number JSON serialization works correctly:

- **`NoFhnummer`**: Uses derived converter (restricts to FH-numbers during deserialization)
- **`NoIdentityNumber`**: Existing logic (accepts all four categories)
- **Validation**: Deserialization respects type restrictions and validation paradigm

**Implementation:**
- Create `NoFhnummerJsonConverter.cs` for FH-number specific serialization
- Update `NoIdentityNumberJsonConverter.cs` to handle all four categories
- Use format validation to discriminate during deserialization

#### 10. Union Type Alignment

Ensure union validation types support FH-number specific errors:

```csharp
// New or reused union cases for FH-number rejection
public record IsNotFhnummer(String Message);         // OR
public record IsFhnummerRequired(String Message);    // OR
public record InvalidFhnummerFormat(String Message);
```

**Recommendation:** Add specific cases for healthcare vs. date-based discrimination errors.

---

### Non-Functional Requirements

#### 1. Backward Compatibility

**No Breaking Changes:**
- Adding `NoFhnummer` type does not affect existing code
- Adding `Fhnummer` marker to `NoIdentifierType` is backward compatible
- Extending `NoIdentityNumber` to accept FH-numbers is backward compatible if validation is defensive

#### 2. Code Organization

**File Structure Additions:**
```
src/KfAccountNumbers/Governmental/Europe/
  ├── NoFhnummer.cs                      (New: FH-number specific type)
  ├── NoFhnummerJsonConverter.cs         (New: FH-number JSON converter)
  ├── [New intermediate base classes if using Option B]
  ├── NoIdentifierType.cs                (Modify: add Fhnummer marker)
  ├── NoIdentityNumber.cs                (Modify: support FH-numbers)
  ├── NoIdentityNumberBase.cs            (Modify: dual validation paradigm)
  ├── NoIdentityNumberJsonConverter.cs   (Modify: handle four categories)
  └── [other files]
```

#### 3. Error Messaging

New error message keys:

- `NoFhnummer_IsNotFhnummer`: "The value is a valid [category], not an FH-nummer."
- `NoFhnummer_InvalidFormat`: "The value does not match FH-nummer format."
- `NoIdentityNumber_InvalidDayRange`: (if needed) "Day field is invalid."
- `NoIdentityNumber_NotDateBasedFormat`: (if needed) "Value is not in date-based format."

#### 4. Testing

**Test Coverage:**
- FH-number specific tests in `NoFhnummerTests.cs`
- Updated tests in `NoIdentityNumberTests.cs` covering FH-number discrimination
- Format validation tests (distinguishing H-number from FH-number with first digit 8)
- Conversion operator tests including FH-number conversions
- JSON round-trip tests for FH-numbers

**Critical Test Scenarios:**
- Values starting with 8 or 9 that are date-based (H-numbers) vs. non-date-based (FH-numbers)
- No date-of-birth parsing for FH-numbers
- Proper rejection of date validation for FH-numbers

#### 5. Documentation Updates

- Update README.md to mention FH-numbers and healthcare identification
- Add FH-number examples to documentation
- Document first-digit discrimination logic and format-based discrimination
- Explain why FH-numbers are fundamentally different from other types
- Link to S0041 story and provide healthcare context

---

## Acceptance Criteria

### Definition of Done

- [ ] **NoFhnummer type created**: New `NoFhnummer` record type accepts only FH-numbers (starts 8 or 9, non-date format).

- [ ] **Format specification confirmed**: Exact FH-number format, length, and check digit algorithm documented from official sources.

- [ ] **First-digit discrimination implemented**: Code correctly distinguishes FH-numbers from H-numbers by analyzing format, not just first digit.

- [ ] **NoIdentifierType updated**: `Fhnummer` marker struct added; all existing markers unchanged.

- [ ] **NoIdentityNumber extended**: Accepts all four categories; `IdentifierType` property discriminates correctly.

- [ ] **Validation rules implemented**: FH-numbers validated without date parsing; date-based validation skipped for FH-numbers.

- [ ] **Architectural pattern chosen**: Intermediate base classes (or other approach) documented and implemented.

- [ ] **Conversion operators added**: Implicit `NoFhnummer` → `NoIdentityNumber`; explicit reverse.

- [ ] **JSON serialization working**: `NoFhnummerJsonConverter` created; round-trip tests pass.

- [ ] **Error messages added**: FH-number specific message keys in `Messages` class.

- [ ] **Format-based discrimination verified**: Tests confirm H-number (day 81–99 in DDMMYY format) correctly distinguished from FH-number (format check).

- [ ] **No date validation for FH-numbers**: Tests verify DateOfBirth, gender derivation, and century logic are not applied to FH-numbers.

- [ ] **Backward compatibility verified**: Existing code using other types continues to work.

- [ ] **Test coverage complete**: `NoFhnummerTests.cs` and updated `NoIdentityNumberTests.cs` cover all scenarios.

- [ ] **Documentation updated**: README and XML comments explain FH-numbers and distinction from H-numbers.

- [ ] **Build succeeds**: No compilation errors or warnings.

- [ ] **All tests pass**: Unit tests pass with no regressions.

- [ ] **Code review approved**: Architecture reviewed for handling dual validation paradigms.

---

## Implementation Strategy

### Phase 1: Format Specification and Validation Strategy

1. Research and document exact FH-number format (length, structure, check digit)
2. Design validation strategy distinguishing FH-numbers from H-numbers
3. Determine intermediate base class approach (Option A, B, or C)
4. Document first-digit discrimination algorithm

**Deliverable:** Design document with format spec and validation strategy

### Phase 2: Foundation (Type Markers and Validation)

1. Add `Fhnummer` struct to `NoIdentifierType`
2. Implement format validation distinguishing date-based from non-date-based
3. Implement intermediate base classes if using Option B

**Deliverable:** Updated `NoIdentifierType.cs` and base class hierarchy

### Phase 3: New NoFhnummer Type

1. Create `NoFhnummer.cs` inheriting from appropriate base
2. Implement FH-number specific validation (no date parsing)
3. Add check: starts with 8 or 9, matches FH-number format
4. Ensure DateOfBirth and Gender properties are handled appropriately

**Deliverable:** `NoFhnummer.cs` with comprehensive implementation

### Phase 4: Extend NoIdentityNumber

1. Update `NoIdentityNumber.cs` to accept all four categories
2. Update `IdentifierType` discrimination logic
3. Extend validation to handle format-based discrimination

**Deliverable:** Updated `NoIdentityNumber.cs` with four-category support

### Phase 5: JSON Serialization

1. Create `NoFhnummerJsonConverter.cs`
2. Update `NoIdentityNumberJsonConverter.cs` to handle all four categories
3. Implement format-based discrimination during deserialization

**Deliverable:** Working JSON converters

### Phase 6: Conversion Operators

1. Add `implicit operator` from `NoFhnummer` to `NoIdentityNumber`
2. Add `explicit operator` from `NoIdentityNumber` to `NoFhnummer`
3. Comprehensive tests for all conversion scenarios

**Deliverable:** Working conversion operators

### Phase 7: Testing and Validation

1. Create `NoFhnummerTests.cs` with FH-number specific tests
2. Update `NoIdentityNumberTests.cs` to cover FH-number discrimination
3. Add critical tests for H-number (day 81–99) vs. FH-number (format) distinction
4. Verify no date validation applied to FH-numbers

**Deliverable:** Comprehensive test coverage (100+ tests)

### Phase 8: Documentation and Integration

1. Update README.md with FH-number section and healthcare context
2. Add error messages to `Messages` class
3. Update XML comments explaining healthcare paradigm
4. Final build validation

**Deliverable:** Documented, tested, and validated implementation

---

## Type Hierarchy Diagram

```
                            NoIdentityNumberBase (abstract)
                                      ▲
                  ┌───────────────────┼───────────────────┐
                  │                   │                   │
        [Intermediate bases if Option B]
        NoDateBasedIdentityBase   NoHealthcareIdentityBase
                  ▲                       ▲
        ┌─────────┼─────────┐            │
        │         │         │            │
   NoFoeds.    NoDNum.    NoHnum.    NoFhnumber
   Fødselsnummer D-nummer  H-nummer  FH-nummer
   (date 01-31) (date 41-71) (date 81-99) (healthcare)

Unrestricted: NoIdentityNumber (all four)
```

---

## FH-Number vs. H-Number Quick Reference

| Aspect | H-nummer | FH-nummer |
|--------|----------|-----------|
| First digit(s) | 8 (days 81–99) | 8 or 9 (no date) |
| Format | DDMMYYIIICC (date) | Non-date format |
| Date encoding | YES | **NO** |
| Validation | Date + individual + century | Format + check digit |
| Scope | Institution-specific | **Global (all healthcare)** |
| Example discrimination | If DDMMYY format & day 81–99 | If NOT date format & starts 8/9 |

---

## Validation Logic Pseudo-Code

```csharp
// Discriminate Norwegian identity types
public static NoIdentifierType Discriminate(String value)
{
    // Step 1: Check for date-based format (DDMMYYIIICC pattern)
    if (IsDateBasedFormat(value))
    {
        int day = ExtractDay(value);

        if (day is >= 1 and <= 31)
            return Foedselsnummer;
        else if (day is >= 41 and <= 71)
            return DNummer;
        else if (day is >= 81 and <= 99)
            return Hnummer;
        else
            return InvalidDayRange; // 32–40 or 72–80
    }

    // Step 2: Check for FH-number format
    else if (IsFhnummerFormat(value))
    {
        if (StartsWithDigit(value, '8', '9'))
            return Fhnummer;
    }

    // Step 3: No valid format matched
    return InvalidFormat;
}

// Format validators
bool IsDateBasedFormat(String value)
    => value.Length is 11 or 12
    && AllDigitsExceptSeparator(value)
    && ValidCalendarDate(ExtractDate(value));

bool IsFhnummerFormat(String value)
    => value.Length is 11 or 12 // or TBD length
    && (value[0] is '8' or '9')
    && !IsDateBasedFormat(value); // Explicitly exclude date-based
```

---

## Migration Guide for Existing Code

### Before

```csharp
public void ProcessIdentity(NoIdentityNumber identity)
{
    if (identity.IdentifierType == NoIdentifierType.Foedselsnummer)
        SendToFoedselsnummerSystem(identity.Value);
    else if (identity.IdentifierType == NoIdentifierType.DNummer)
        SendToDNummerSystem(identity.Value);
    else if (identity.IdentifierType == NoIdentifierType.Hnummer)
        SendToHnummerSystem(identity.Value);
}
```

### After (Updated to Handle FH-Numbers)

```csharp
public void ProcessIdentity(NoIdentityNumber identity)
{
    _ = identity.IdentifierType switch
    {
        default(NoIdentifierType.Foedselsnummer) => SendToFoedselsnummerSystem(identity.Value),
        default(NoIdentifierType.DNummer) => SendToDNummerSystem(identity.Value),
        default(NoIdentifierType.Hnummer) => SendToHnummerSystem(identity.Value),
        default(NoIdentifierType.Fhnummer) => SendToHealthcareSystem(identity.Value),
        _ => throw new ArgumentException("Unknown Norwegian identity type"),
    };
}
```

---

## Success Metrics

- [ ] All existing tests pass without regression
- [ ] 60+ new tests added for FH-number functionality
- [ ] Format-based discrimination tests verify H-number/FH-number distinction
- [ ] Tests confirm DateOfBirth and Gender properties handled correctly for FH-numbers
- [ ] Conversion operators thoroughly tested
- [ ] JSON round-trip serialization passes for all four types
- [ ] Documentation clearly explains healthcare paradigm and format-based discrimination
- [ ] Code review approves dual validation paradigm architecture
- [ ] Build passes without warnings or errors
- [ ] Performance benchmarks show no regression
- [ ] Team confirms FH-number format and validation rules are correctly implemented

---

## Risk Assessment

### High Risks

1. **Format Specification Uncertainty**: Exact FH-number format must be confirmed before implementation
   - **Mitigation**: Research official Norwegian healthcare documentation; defer implementation until spec confirmed

2. **Architectural Dual Paradigm**: Supporting both date-based and non-date-based validation in same hierarchy
   - **Mitigation**: Design intermediate base classes; separate concerns clearly; thorough design review

3. **H-Number vs. FH-Number Discrimination**: Both can start with 8; distinction requires format analysis, not just first digit
   - **Mitigation**: Implement robust format validation; comprehensive test coverage for edge cases

### Medium Risks

4. **Integration with S0040**: Coordinates with H-number implementation (S0040)
   - **Mitigation**: Implement together or after S0040 is stable

5. **DateOfBirth Property Semantics**: FH-numbers don't have birth date; property becomes null or unused
   - **Mitigation**: Document semantics; consider property design in intermediate base classes

### Low Risks

6. **Backward Compatibility**: Adding new type should not break existing code
   - **Mitigation**: Careful design; comprehensive integration tests

---

## Definition of Ready

Before implementation:

- [ ] S0029, S0039, S0040 completed or nearing completion
- [ ] Official FH-number format specification obtained and documented
- [ ] FH-number check digit algorithm confirmed
- [ ] Team consensus on intermediate base class approach
- [ ] Format validation algorithm designed and reviewed
- [ ] H-number vs. FH-number discrimination algorithm designed
- [ ] Test infrastructure ready
- [ ] Messages class confirmed available for new keys
- [ ] Documentation template finalized

---

## Dependencies

- **S0029**: NoFoedselsnummerUnionMigration (union types)
- **S0039**: NoFoedselsnummerTypeHierarchy (base class)
- **S0040**: NoHnummerImplementation (context for distinction)
- **Messages class**: For error messages
- **NoIdentifierType.cs**: Existing type markers
- **Official FH-Number Spec**: From Norwegian healthcare authorities

---

## Stakeholders

- **Developers**: Need clear API for healthcare patient identification
- **Healthcare IT Teams**: Value of FH-number support
- **Product Owner**: Business value of healthcare compliance
- **QA**: Responsible for format validation testing
- **DevOps**: Release coordination with S0040
- **Documentation**: README and inline documentation updates

---

## Files to Create/Modify

### Create (New Files)

- `src/KfAccountNumbers/Governmental/Europe/NoFhnummer.cs` – FH-number type
- `src/KfAccountNumbers/Governmental/Europe/NoFhnummerJsonConverter.cs` – JSON converter
- `[Intermediate base classes if using Option B]`
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/NoFhnummerTests.cs` – FH-number tests

### Modify (Existing Files)

- `src/KfAccountNumbers/Governmental/Europe/NoIdentifierType.cs` – Add `Fhnummer` marker
- `src/KfAccountNumbers/Governmental/Europe/NoIdentityNumber.cs` – Support four categories
- `src/KfAccountNumbers/Governmental/Europe/NoIdentityNumberBase.cs` – Dual validation paradigm
- `src/KfAccountNumbers/Governmental/Europe/NoIdentityNumberJsonConverter.cs` – Support four categories
- `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/NoIdentityNumberTests.cs` – Add FH-number tests
- `README.md` – Document FH-numbers and healthcare context
- `Messages` class – Add FH-number error keys

---

## Implementation Notes

### Critical Challenge: First Digit Discrimination

The primary challenge is that both H-numbers and FH-numbers can start with 8:
- H-number: Day field 81–99 (so first digit is 8, second digit is 1–9)
- FH-number: Starts with 8 or 9 (but entire value is not DDMMYYIIICC format)

**Solution:** Format validation must come first:
1. Attempt to parse as date-based (DDMMYYIIICC)
2. If successful and date valid → check day field (01–31, 41–71, or 81–99)
3. If not date-based but starts 8 or 9 → FH-number

### Architectural Decision: Handling DateOfBirth

FH-numbers don't encode date of birth, so the inherited `DateOfBirth` property is problematic:

**Options:**
- Property returns `null` for FH-numbers
- Property throws `NotSupportedException` for FH-numbers
- Intermediate base class hierarchy prevents property existence in FH-numbers

**Recommendation:** Intermediate base class hierarchy (Option B) avoids exposing inapplicable properties.

### Healthcare System Context

FH-numbers are essential for Norwegian healthcare IT systems because:
- Patients may not have fødselsnummer (e.g., foreign temporary workers, asylum seekers)
- D-nummer may not be issued or accessible
- H-number is institution-specific (different in each healthcare institution)
- FH-nummer provides unique identification across entire healthcare system
- Healthcare systems must handle all categories for patient identification

---

## Notes

- This story represents significant architectural expansion beyond initial design
- Support for both date-based and non-date-based identification types requires careful design
- FH-numbers are qualitatively different from other Norwegian identity types
- Intermediate base classes may be necessary for clean implementation
- This story should be prioritized after S0039 and S0040 are stable
- Research official FH-number specifications before implementation details finalized
- Consider whether other Norwegian non-date-based identification types may be needed in future
