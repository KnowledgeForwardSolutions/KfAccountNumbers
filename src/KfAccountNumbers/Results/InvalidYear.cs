namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a value contains an invalid
///   year component.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that an invalid
///   year element was found in the input value.
/// </param>
/// <param name="Year">
///   The invalid year element that was found in the input value.
/// </param>
public readonly record struct InvalidYear(
   String Description,
   String Year);
