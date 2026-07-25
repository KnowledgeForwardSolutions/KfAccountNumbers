# User Story: Expand Reference Documentation and Update README

## Summary
Create comprehensive reference documentation (.md files) for all account number types in KfAccountNumbers that currently lack individual documentation files. Update README.md to reference these new documentation files and replace verbose type descriptions with concise links.

## Acceptance Criteria

### 1. Documentation Files Created
Create individual reference documentation (.md files) for the following account number types that currently lack documentation:

**Europe - Missing Documentation (as of current state):**
- [ ] BeRijksregisternummer
- [ ] DkPersonnummer
- [ ] FiHenkilotunnus
- [ ] FrInseeNumber
- [ ] GbChiNumber
- [ ] GbHcNumber
- [ ] GbNhsNumber
- [ ] GbPatientNumber
- [ ] GbNationalInsuranceNumber
- [ ] IePpsNumber
- [ ] IsKennitala
- [ ] NlBurgerservicenummer

**North America - Missing Documentation:**
- [ ] CaSocialInsuranceNumber
- [ ] MxCurp
- [ ] UsIndividualTaxpayerIdentificationNumber
- [ ] UsNationalProviderIdentifier
- [ ] UsSocialSecurityNumber

**Files Already Documented (Reference Format):**
- DeSteuerIdNr.md ✓
- EsNif.md ✓
- ItCodiceFiscale.md ✓
- LuMatricule.md ✓
- NoIdentityNumber, NoFoedselsnummer, NoHnummer, NoDnummer, NoFhnummer ✓
- SeIdentityNumber, SePersonnummer, SeSamordningsnummer ✓

### 2. Documentation Format Standards
Each .md file **MUST** follow the standard template used in existing documentation files (LuMatricule.md, NoHnummer.md, DeSteuerIdNr.md, etc.) and include:

**Required Sections:**
- [ ] Header with type name
- [ ] Overview/description section
- [ ] Information table with:
  - Class name
  - Is composite (Yes/No)
  - Length (unformatted and/or formatted if applicable)
  - Check digit algorithm(s)
  - Allowed characters
  - Allowed separator characters (if applicable)
  - Structure definition with detailed element breakdown
  - Example values with annotations

- [ ] Validation rules section with table containing:
  - Rule number
  - Rule description
  - Error result type

- [ ] Additional properties section (if applicable):
  - Properties like DateOfBirth, Gender, etc.

- [ ] Notes section:
  - Important clarifications and special behaviors

- [ ] References section:
  - Links to authoritative sources

### 3. README.md Updates
- [ ] Create a new "Reference Documentation" or "Type Documentation" section in README.md
- [ ] For each type, replace verbose multi-line descriptions with a brief link:
  ```markdown
  - [TypeName](docs/Reference/National/[Region]/[TypeName].md)
  ```
- [ ] Keep brief single-line summary descriptions (max 1-2 sentences) before the link for quick context
- [ ] Remove extensive implementation details from README.md body
- [ ] Maintain the namespace hierarchy structure but use links instead of inline documentation

**Example Format:**
```markdown
### Europe
- [DeSteuerIdNr](docs/Reference/National/Europe/DeSteuerIdNr.md) — German tax identification number
- [LuMatricule](docs/Reference/National/Europe/LuMatricule.md) — Luxembourg national identification number
```

### 4. Directory Structure
- [ ] Create `docs/Reference/National/NorthAmerica/` directory (if needed)
- [ ] Organize all documentation files by region:
  ```
  docs/Reference/National/
    ├── Europe/
    │   ├── BeRijksregisternummer.md
    │   ├── DeSteuerIdNr.md
    │   └── [other Europe types]
    └── NorthAmerica/
        ├── CaSocialInsuranceNumber.md
        ├── MxCurp.md
        └── [other NorthAmerica types]
  ```

## Implementation Notes

### Data Extraction
- [ ] Extract structure, validation rules, and properties from source code comments and implementation
- [ ] Reference official government documentation and standards
- [ ] Include check digit algorithm details where applicable
- [ ] Include example values with detailed annotations explaining each component

### Consistency
- [ ] All documentation files must follow the exact same format and style
- [ ] Use the same terminology and phrasing conventions across all files
- [ ] Ensure validation rule numbering aligns with implementation
- [ ] Verify links in references work correctly

### Quality Assurance
- [ ] Grammar and spelling check all documentation
- [ ] Validate that examples in documentation match test cases
- [ ] Ensure technical accuracy of all algorithm descriptions
- [ ] Cross-reference structure definitions with source code

## Files to Be Modified
- `README.md` — Update namespace hierarchy section to use links instead of inline descriptions
- Created: Multiple new .md files in `docs/Reference/National/Europe/` and `docs/Reference/National/NorthAmerica/`

## Files to Be Created
- `docs/Reference/National/Europe/BeRijksregisternummer.md`
- `docs/Reference/National/Europe/DkPersonnummer.md`
- `docs/Reference/National/Europe/FiHenkilotunnus.md`
- `docs/Reference/National/Europe/FrInseeNumber.md`
- `docs/Reference/National/Europe/GbChiNumber.md`
- `docs/Reference/National/Europe/GbHcNumber.md`
- `docs/Reference/National/Europe/GbNhsNumber.md`
- `docs/Reference/National/Europe/GbPatientNumber.md`
- `docs/Reference/National/Europe/GbNationalInsuranceNumber.md`
- `docs/Reference/National/Europe/IePpsNumber.md`
- `docs/Reference/National/Europe/IsKennitala.md`
- `docs/Reference/National/Europe/NlBurgerservicenummer.md`
- `docs/Reference/National/NorthAmerica/CaSocialInsuranceNumber.md`
- `docs/Reference/National/NorthAmerica/MxCurp.md`
- `docs/Reference/National/NorthAmerica/UsIndividualTaxpayerIdentificationNumber.md`
- `docs/Reference/National/NorthAmerica/UsNationalProviderIdentifier.md`
- `docs/Reference/National/NorthAmerica/UsSocialSecurityNumber.md`

## Definition of Done
- [ ] All 17 missing reference documentation files are created
- [ ] Each file follows the standard template format
- [ ] All documentation is technically accurate
- [ ] All examples are validated against test data
- [ ] README.md is updated with links to documentation
- [ ] Verbose inline descriptions are removed from README.md
- [ ] All markdown files pass grammar and spell checks
- [ ] All external reference links are verified as active
- [ ] Directory structure is organized and consistent
- [ ] Pull request is reviewed and approved
- [ ] Documentation is merged to main branch

## Estimated Effort
- **Discovery & Planning:** 2-4 hours
- **Documentation Creation (per type):** 45-90 minutes
- **README.md Updates:** 1-2 hours
- **Quality Assurance & Review:** 2-3 hours
- **Total Estimated:** 25-40 hours

## Priority
High — Improved developer experience and API discoverability

## Related Issues/PRs
- Builds on existing documentation pattern established by DeSteuerIdNr.md, LuMatricule.md, NoHnummer.md

## Notes
- Follow existing documentation style established in LuMatricule.md and NoHnummer.md as templates
- Consider creating a documentation template file to ensure consistency
- Use existing test files as reference for valid examples and validation rules
- Ensure all algorithm descriptions are accurate and sufficient for developers to understand implementation
