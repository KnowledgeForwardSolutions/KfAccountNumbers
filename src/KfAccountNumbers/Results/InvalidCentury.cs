namespace KfAccountNumbers.Results;

/// <summary>
///   The InvalidCentury struct represents a validation error indicating that a
///   value contains an invalid century indicator.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that an invalid
///   century indicator was found in the input value.
/// </param>
/// <param name="Century">
///   The invalid century indicator that was found in the input value.
/// </param>
public readonly record struct InvalidCentury(
   String Description,
   String Century);
