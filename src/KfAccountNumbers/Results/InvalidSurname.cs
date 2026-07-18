namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a value contains an invalid
///   surname (or characters derived from the surname).
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that the value
///   contained an invalid surname (or characters derived from the surname).
/// </param>
/// <param name="Surname">
///   The invalid surname element that was found in the input value.
/// </param>
public readonly record struct InvalidSurname(
   String Description,
   String Surname);
