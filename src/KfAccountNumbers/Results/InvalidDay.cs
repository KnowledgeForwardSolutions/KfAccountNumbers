namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a value contains an invalid
///   day component.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that an invalid
///   day element was found in the input value.
/// </param>
/// <param name="Day">
///   The invalid day element that was found in the input value.
/// </param>
public readonly record struct InvalidDay(
   String Description,
   String Day);
