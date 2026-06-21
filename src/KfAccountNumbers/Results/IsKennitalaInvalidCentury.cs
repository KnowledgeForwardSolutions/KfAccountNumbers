namespace KfAccountNumbers.Results;

/// <summary>
///   The IsKennitalaInvalidCentury struct represents a validation error
///   indicating that an Icelandic kennitala contains an invalid century
///   indicator character.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating thatan invalid century
///   indicator character was found in the input value.
/// </param>
/// <param name="CenturyChar">
///   The invalid century indicator character that was found in the input value.
/// </param>
public readonly record struct IsKennitalaInvalidCentury(
   String Description,
   Char CenturyChar);
