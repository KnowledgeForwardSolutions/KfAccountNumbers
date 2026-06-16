namespace KfAccountNumbers.Results;

/// <summary>
///   The InvalidGender struct represents a validation error indicating that the
///   gender encoded in the value is invalid.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that one or more
///   invalid characters were found in the input value.
/// </param>
/// <param name="GenderCode">
///   The invalid gender code that was found in the input value.
/// </param>
public readonly record struct InvalidGender(
   String Description,
   String GenderCode);
