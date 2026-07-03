namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating the leading two characters of a
///   GB National Insurance Number are invalid.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that the input value
///   contained a disallowed National Insurance Number prefix.
/// </param>
/// <param name="Prefix">
///   The invalid prefix characters found in the value.
/// </param>
public readonly record struct InvalidGbNationalInsuranceNumberPrefix(
   String Description,
   String Prefix);
