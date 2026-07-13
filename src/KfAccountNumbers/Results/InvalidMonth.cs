namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a value contains an invalid
///   month component.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that an invalid
///   month element was found in the input value.
/// </param>
/// <param name="Month">
///   The invalid month element that was found in the input value.
/// </param>
public readonly record struct InvalidMonth(
   String Description,
   String Month);
