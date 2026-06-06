namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation result indicating that the provided value contains
///   an embedded date of birth that is invalid, along with a description of the
///   error.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that the date of
///   birth embedded in the value is invalid.
/// </param>
/// <param name="DateOfBirth">
///   The embedded date of birth value that failed validation, represented as a
///   string.
/// </param>
/// <param name="DateFormat">
///   The date format, i.e. DDMMYY or YYYYMMDD, etc.
/// </param>
public record struct InvalidDateOfBirth(
   String Description,
   String DateOfBirth,
   String DateFormat)
{
}
