namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a value contains one or more
///   leading characters.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that an invalid prefix
///   was found in the input value.
/// </param>
/// <param name="Prefix">
///   The invalid leading character(s) found in the input value.
/// </param>
public readonly record struct InvalidPrefix(
   String Description,
   String Prefix);
