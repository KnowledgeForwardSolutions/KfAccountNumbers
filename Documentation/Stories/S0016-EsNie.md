# S0016-EsNie Business Object

## As a
Developer integrating Spanish governmental identification numbers into my application

## I want
A strongly typed business object that represents both Spanish DNI (Documento Nacional de Identidad) and NIE (Número de Identificación de Extranjero) numbers

## So that
I can validate, parse, and work with Spanish national identification numbers in a type-safe manner with comprehensive validation

## Acceptance Criteria

### Structure and Validation
- [ ] The `EsNif` type represents a Spanish NIF (Número de Identificación Fiscal) which encompasses both DNI and NIE
- [ ] The type includes an `IdentifierType` property that returns `EsIdentifierType` enumeration (Dni or Nie)
- [ ] DNI structure: 8 digits + 1 check letter (e.g., 12345678Z)
- [ ] NIE structure: 1 prefix letter (X, Y, or Z) + 7 digits + 1 check letter (e.g., X1234567L)
- [ ] Constructor accepts string representation and throws `KfValidationException<EsNifValidationResult>` if invalid
- [ ] Static `Validate` method returns `EsNifValidationResult` enumeration value
- [ ] Static `Create` method uses Result pattern returning `CreateResult<EsNif, EsNifValidationResult>`

### Validation Rules
- [ ] Value may not be null, empty, or all whitespace
- [ ] Value must be either 9 characters (unformatted) or 11 characters (formatted with separator)
- [ ] DNI: First 8 characters must be ASCII digits ('0'-'9')
- [ ] NIE: First character must be X, Y, or Z; next 7 characters must be ASCII digits
- [ ] Check letter must be valid according to modulus 23 algorithm
- [ ] Optional separator character (dash or space) may appear before the check letter
- [ ] If separator is present, it may be any non-alphanumeric character
- [ ] Check letter must be uppercase (A-Z, excluding Ñ, U, I, O for DNI; specific subset for NIE)

### Format Support
- [ ] Accept unformatted DNI: 12345678Z
- [ ] Accept formatted DNI: 12345678-Z or 12345678 Z
- [ ] Accept unformatted NIE: X1234567L
- [ ] Accept formatted NIE: X1234567-L or X1234567 L
- [ ] Case-insensitive for validation (convert to uppercase internally)
- [ ] `Format` method with optional mask parameter (default: "________-_")

### Properties
- [ ] `Value` property returns raw 9-character string (uppercase)
- [ ] `IdentifierType` property returns `EsIdentifierType` enumeration (Dni or Nie)
- [ ] `Number` property returns the numeric portion as string (8 digits for DNI, 7 for NIE)
- [ ] `CheckLetter` property returns the check letter character

### Check Letter Algorithm
- [ ] DNI: Calculate modulus 23 of the 8-digit number, map to letter using: TRWAGMYFPDXBNJZSQVHLCKE
- [ ] NIE: Convert prefix (X?0, Y?1, Z?2), concatenate with 7 digits, calculate modulus 23, map to letter
- [ ] Algorithm produces check letter from position in "TRWAGMYFPDXBNJZSQVHLCKE" string

### Operators and Methods
- [ ] Implicit conversion to string
- [ ] Explicit conversion from string
- [ ] `ToString` returns raw value (uppercase, no separator)
- [ ] Proper equality implementation (case-insensitive)
- [ ] JSON serialization/deserialization support via `EsNifJsonConverter`

### Special Cases
- [ ] Handle lowercase input by converting to uppercase
- [ ] NIE prefix letters X, Y, Z correspond to different issuance periods/categories
- [ ] DNI numbers starting with 0 are valid (leading zeros preserved)
- [ ] Historical note: Some very old DNI may have different formats (not supported)

### Test Coverage
- [ ] All validation rules with valid and invalid data
- [ ] Check letter algorithm for both DNI and NIE
- [ ] All 23 possible check letters (comprehensive coverage)
- [ ] Case-insensitive validation
- [ ] Both formatted and unformatted inputs
- [ ] Format and ToString methods
- [ ] IdentifierType property for both DNI and NIE
- [ ] Equality and hash code (case-insensitive)
- [ ] JSON serialization round-trip
- [ ] Conversion operators
- [ ] Create method Result pattern
- [ ] Error detection: single digit transcription, transposition, etc.

### Documentation
- [ ] XML documentation for all public members
- [ ] README.md section with:
  - Structure explanation for both DNI and NIE
  - Validation rules
  - Format examples
  - Modulus 23 check letter algorithm
  - Difference between DNI and NIE
  - Valid check letters and their positions
  - Example values for both types with descriptions
  - Wikipedia and official references
  - Historical context (DNI introduced 1944, NIE for foreigners)

### Performance
- [ ] Use `ReadOnlySpan<Char>` for validation and parsing
- [ ] Efficient check letter algorithm
- [ ] Single-pass validation where possible
- [ ] Minimal string allocations
- [ ] Case conversion only when necessary

## Notes
- NIF = Número de Identificación Fiscal (tax identification number, generic term)
- DNI = Documento Nacional de Identidad (for Spanish citizens)
- NIE = Número de Identificación de Extranjero (for foreign residents)
- DNI format: 8 digits + check letter
- NIE format: prefix (X/Y/Z) + 7 digits + check letter
- Check letter uses modulus 23 algorithm with specific letter sequence
- X prefix was original NIE prefix, Y added when X numbers exhausted, Z added later
- Both DNI and NIE use same check letter algorithm (NIE prefix converted to digit)
- Valid check letters: TRWAGMYFPDXBNJZSQVHLCKE (23 letters, no Ñ, U, I, O)
- Format may include optional separator (typically dash) before check letter
- Case-insensitive for input, but stored as uppercase
- See: https://en.wikipedia.org/wiki/National_identification_number#Spain
- See: https://es.wikipedia.org/wiki/N%C3%BAmero_de_identificaci%C3%B3n_fiscal

## Technical Details
- Namespace: `KfAccountNumbers.Governmental.Europe`
- Files:
  - `src/KfAccountNumbers/Governmental/Europe/EsNif.cs`
  - `src/KfAccountNumbers/Governmental/Europe/EsNifValidationResult.cs`
  - `src/KfAccountNumbers/Governmental/Europe/EsIdentifierType.cs`
  - `tests/KfAccountNumbers.Tests.Unit/Governmental/Europe/EsNifTests.cs`
- JSON converter: `EsNifJsonConverter`
- Target: .NET 10, C# 14.0
- Pattern similar to `IsKennitala` and `NoFoedselsnummer` (single class handling two identifier types)

## Example Values
### DNI Examples
- 12345678Z - Standard DNI format
- 12345678-Z - DNI with separator
- 00000000T - DNI with leading zeros (valid)
- 99999999R - High number DNI (valid)

### NIE Examples
- X1234567L - NIE with X prefix (original series)
- Y1234567Z - NIE with Y prefix (extended series)
- Z1234567R - NIE with Z prefix (latest series)
- X1234567-L - NIE with separator

### Invalid Examples
- 12345678A - Wrong check letter for DNI
- W1234567L - Invalid NIE prefix (only X, Y, Z allowed)
- 1234567Z - Too few digits for DNI
- X12345678L - Too many digits for NIE