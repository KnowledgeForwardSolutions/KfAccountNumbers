# S0006-UsIndividualTaxpayerIdentificationNumber: Strongly Typed US Individual Taxpayer Identification Number (ITIN)

## Title
Create a strongly typed .NET object for US Individual Taxpayer Identification Number (ITIN)

## User Story
As a **.NET developer working with US tax systems**,  
I want a **strongly typed object to represent an Individual Taxpayer Identification Number (ITIN)**,  
so that **ITIN values are validated, consistent, and safely handled across the application, with clear differentiation from Social Security Numbers**.

## Background
The Individual Taxpayer Identification Number (ITIN) is a 9-digit tax processing number issued by the Internal Revenue Service (IRS). ITINs are used by individuals who have a federal tax filing or reporting requirement but are not eligible for a Social Security Number (SSN). This includes nonresident aliens, resident aliens, dependents, and spouses of U.S. citizens/residents.

While ITINs follow a similar XXX-XX-XXXX format as SSNs, they have distinct validation rules that prevent confusion between the two identifier types. ITINs are for tax purposes only and cannot be used for employment authorization or to qualify for Social Security benefits.

Treating ITINs as plain strings or integers increases the risk of:
- Accepting invalid ITIN values
- Confusing ITINs with SSNs
- Inconsistent validation logic throughout the codebase
- Security issues when handling sensitive tax identifiers

## Acceptance Criteria

- The ITIN is represented as a **dedicated .NET type**, not a primitive (`string`, `int`, or `long`)
- The object enforces the following validation rules:
  - Must be exactly **9 digits** (unformatted) or **11 characters** with separators (formatted as XXX-XX-XXXX)
  - The first digit must be **9** (distinguishes from valid SSNs)
  - The middle section (4th and 5th digits) must be in specific ranges:
    - **50-65**, **70-88**, **90-92**, or **94-99**
    - This explicitly excludes ranges used for SSNs and prevents overlap
  - Separators (if present) must be consistent and must be the same non-digit character at positions 3 and 6
  - Default separator is **dash ('-')** but other non-digit separators should be accepted
- Invalid ITIN values cannot be instantiated
- The object is **immutable** after creation
- The object supports:
  - Equality comparison
  - String conversion (e.g., `ToString()`)
  - Formatting with custom separators
  - Implicit conversion to/from string
- The object provides a **static `Validate` method** that returns validation results without throwing
- The object provides a **static `Create` method** using the result pattern (returns success or validation failure)
- The object can be safely used in:
  - Entity models
  - DTOs
  - Serialization/deserialization scenarios (JSON)

## Technical Notes

- Implement as a **`record`** for value semantics and immutability
- Follow the **ValidationMode pattern** established in `UsSocialSecurityNumber` and `MxCurp`:
  - Public constructor: `ValidationMode.ValidationRequired`
  - Private constructor: `ValidationMode.BypassValidation` (for `Create` method)
- Provide explicit creation patterns:
  - Constructor with validation (throws `InvalidUsIndividualTaxpayerIdentificationNumberException`)
  - Static `Create` method that returns `CreateResult<UsIndividualTaxpayerIdentificationNumber, UsIndividualTaxpayerIdentificationNumberValidationResult>`
  - Static `Validate` method that returns `UsIndividualTaxpayerIdentificationNumberValidationResult`
- Validation errors should provide meaningful, specific feedback via validation result enum
- Store value as **unformatted 9-digit string** internally (similar to SSN pattern)
- Support formatted (XXX-XX-XXXX) and unformatted (XXXXXXXXX) input
- Include **`[JsonConverter]`** attribute with custom converter for JSON serialization
- Provide **`Format` method** with default mask "___-__-____"
- Implement:
  - Implicit conversion operators to/from `String`
- Use **`ReadOnlySpan<Char>`** for efficient string operations (like `UsSocialSecurityNumber`)
- Use **`ArrayPool<Char>`** when removing separators (like `UsSocialSecurityNumber`)
- Consider **`[MethodImpl(MethodImplOptions.AggressiveInlining)]`** for hot-path helper methods

### ITIN Middle Section Validation Details
The middle section (positions 4-5, zero-based) must fall into these ranges:
- **50-65**: Inclusive (16 values)
- **70-88**: Inclusive (19 values)  
- **90-92**: Inclusive (3 values)
- **94-99**: Inclusive (6 values)

**Excluded ranges** (reserved for SSNs or other purposes):
- 00-49: Reserved for SSNs
- 66-69: Reserved for SSNs
- 89: Reserved
- 93: Reserved

## Definition of Done

- **Code complete**:
  - `UsIndividualTaxpayerIdentificationNumber` record implemented
  - `UsIndividualTaxpayerIdentificationNumberValidationResult` enum defined
  - `InvalidUsIndividualTaxpayerIdentificationNumberException` implemented
  - JSON converter implemented and tested
  - All validation rules correctly implemented
  - `ValidationMode` pattern correctly implemented
  - `Format` method implemented
  - Implicit conversion operators implemented
- **Unit tests complete** covering:
  - Valid ITINs (formatted and unformatted)
  - All invalid scenarios (corresponding to validation result enum values):
    - Empty/null values
    - Invalid length
    - First digit not 9
    - Invalid middle section values (00-49, 66-69, 89, 93)
    - Invalid separator characters
    - Invalid characters (non-digits where digits expected)
  - Boundary values for middle section ranges
  - Constructor (throwing behavior)
  - `Create` method (result pattern)
  - `Validate` method (all validation results)
  - Implicit conversions (to/from string)
  - Equality operators
  - `GetHashCode` consistency
  - `ToString` method
  - `Format` method (default and custom masks)
  - JSON serialization/deserialization (round-trip, case handling, error scenarios)
  - Edge cases (separators, formatting, etc.)
- **Documentation complete**:
  - XML comments on all public members
  - Comprehensive `<remarks>` section explaining:
    - ITIN format and structure
    - Difference from SSN
    - Valid middle section ranges
    - IRS issuance (not SSA)
  - Inline comments for complex validation logic
  - README.md updated with ITIN section
- **Code review** completed and approved
- **No compiler warnings** in implementation or tests
- **All tests passing** (100% success rate)
- **Consistent with existing patterns**:
  - Matches `UsSocialSecurityNumber` architecture
  - Matches `MxCurp` ValidationMode pattern
  - Matches JSON converter pattern from both SSN and CURP

## Out of Scope

- IRS validation service integration (actual ITIN verification with IRS systems)
- ITIN expiration date tracking (ITINs can expire if not used)
- ITIN application process (W-7 form handling)
- Distinction between individual/business ITINs
- Historical ITIN format changes (pre-1996 format differences)
- ITIN renewal workflow
- Employer Identification Number (EIN) support
- Adoption Taxpayer Identification Number (ATIN) support
- Preparer Tax Identification Number (PTIN) support
- Database persistence concerns
- Encryption/security mechanisms (beyond immutability)
- Audit logging
- User interface components
- Validation against actual IRS ITIN databases

## References

- [IRS - Individual Taxpayer Identification Number](https://www.irs.gov/individuals/individual-taxpayer-identification-number)
- [IRS Publication 1915 - Understanding Your IRS Individual Taxpayer Identification Number](https://www.irs.gov/pub/irs-pdf/p1915.pdf)
- [Wikipedia - Individual Taxpayer Identification Number](https://en.wikipedia.org/wiki/Individual_Taxpayer_Identification_Number)
- IRS ITIN Fact Sheet: Valid middle section ranges documented in IRS guidance

## Notes

### Key Differences from SSN
1. **First digit**: Always 9 (SSN never starts with 9 for valid numbers post-2011 randomization)
2. **Middle section**: Restricted ranges (50-65, 70-88, 90-92, 94-99) vs SSN's broader range (01-99 excluding 00)
3. **Purpose**: Tax filing only vs SSN's employment and benefits eligibility
4. **Issuing authority**: IRS vs Social Security Administration
5. **Expiration**: ITINs can expire if unused; SSNs never expire
6. **No geographic significance**: Unlike pre-2011 SSNs

### Implementation Considerations
- Should validate that first digit is 9 **before** checking middle section (fail fast)
- Middle section validation is the key distinguishing feature
- Consider providing a helper method to check if a 9-digit number is ITIN vs SSN-ITIN-reserved
- Error messages should clearly indicate which validation rule failed
- Test data should include boundary values: 50, 65, 70, 88, 90, 92, 94, 99
- Invalid values to test: 00, 49, 66, 69, 89, 93