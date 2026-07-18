namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a value contains an invalid
///   given name (or characters derived from the given name[s]).
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that the value
///   contained an invalid given name (or characters derived from the given
///   name[s]).
/// </param>
/// <param name="GivenName">
///   The invalid given name element that was found in the input value.
/// </param>
public readonly record struct InvalidGivenName(
   String Description,
   String GivenName);
