namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a value contains an invalid
///   code intended to identify a geographical region (state, province,
///   department, town, municipality, etc.).
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that the value
///   contained an invalid location code.
/// </param>
/// <param name="LocationCode">
///   The invalid location code that was found in the input value.
/// </param>
public readonly record struct InvalidLocationCode(
   String Description,
   String LocationCode);
