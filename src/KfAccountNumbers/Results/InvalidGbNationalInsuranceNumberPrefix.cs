namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating the leading two characters of a
///   <see cref="GbNationalInsuranceNumber"/> are invalid.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that one or more
///   invalid characters were found in the input value.
/// </param>
/// <param name="Prefix">
///   The invalid prefix characters found in the value.
/// </param>
public readonly record struct InvalidGbNationalInsuranceNumberPrefix(
   String Description,
   String Prefix);
