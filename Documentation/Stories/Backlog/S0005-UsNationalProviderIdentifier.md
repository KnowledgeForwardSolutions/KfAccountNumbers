# S0005-UsNationalProviderIdentifier: Strongly Typed National Provider Identifier (NPI)

## Title
Create a strongly typed .NET object for US National Provider Identifier (NPI)

## User Story
As a **.NET developer**,  
I want a **strongly typed object to represent a US National Provider Identifier (NPI)**,  
so that **NPI values are validated, consistent, and safely handled across the application**.

## Background
The National Provider Identifier (NPI) is a 10-digit numeric identifier used to uniquely identify healthcare providers in the United States. Treating NPIs as plain strings or integers increases the risk of invalid values, formatting errors, and inconsistent validation logic throughout the codebase.

## Acceptance Criteria

- The NPI is represented as a **dedicated .NET type**, not a primitive (`string`, `int`, or `long`)
- The object enforces the following validation rules:
  - Must be exactly **10 digits**
  - Must contain **numeric characters only**
  - Must pass the **NPI Luhn check digit algorithm**
- Invalid NPI values cannot be instantiated
- The object is **immutable** after creation
- The object supports:
  - Equality comparison
  - String conversion (e.g., `ToString()`)
- The object can be safely used in:
  - Entity models
  - DTOs
  - Serialization/deserialization scenarios

## Technical Notes

- Implement as a `readonly struct`, `sealed class` or `record`
- Provide a clear and explicit creation pattern:
  - Constructor with validation **or**
  - Static factory method (e.g., `Npi.Create(string value)`)
- Validation errors should provide meaningful feedback
- NPI Luhn checkdigit validation via **CheckDigits.Net*
- Override:
  - `Equals`
  - `GetHashCode`
  - Equality operators (`==`, `!=`) if applicable
- Consider implementing:
  - `IEquatable<T>`
  - Custom JSON converter (if required by the application)

## Definition of Done

- Unit tests cover:
  - Valid NPIs
  - Invalid length
  - Non-numeric values
  - Invalid check digits
- Object is documented with XML comments
- No direct usage of primitive types for NPI elsewhere in the codebase
- Code review completed and approved

## Out of Scope

- Provider lookup or external NPI registry integration
- Persistence or database-specific concerns