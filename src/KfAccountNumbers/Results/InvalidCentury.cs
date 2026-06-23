namespace KfAccountNumbers.Results;

/// <summary>
///   The InvalidCentury struct represents a validation error indicating that a
///   value contains an invalid century indicator character.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that an invalid
///   century indicator character was found in the input value.
/// </param>
/// <param name="CenturyChar">
///   The invalid century indicator character that was found in the input value.
/// </param>
public readonly record struct InvalidCentury(
   String Description,
   Char CenturyChar);
