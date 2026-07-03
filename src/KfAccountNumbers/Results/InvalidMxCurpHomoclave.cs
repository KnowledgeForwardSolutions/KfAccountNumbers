// Ignore Spelling: Curp Mx

namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that the homoclave character
///   located at character position 16 (zero-based) in the value is invalid.
///   Homoclave characters are alphanumeric characters (A-Z, 0-9) issued by
///   RENAPO (Registro Nacional de Población) to avoid duplicate CURP values.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that one or more
///   invalid characters were found in the input value.
/// </param>
/// <param name="Homoclave">
///   The invalid homoclave character that was found in the input value.
/// </param>
public readonly record struct InvalidMxCurpHomoclave(
   String Description,
   Char Homoclave);
