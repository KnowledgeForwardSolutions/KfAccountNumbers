namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that the input value contains a
///   number that is outside the valid range for a GB patient number. The valid
///   range(s) depend on the health service that issued the number.
/// </summary>
/// <param name="Description">
///   Description of the specific range that was violated.
/// </param>
public readonly record struct GbPatientNumberInvalidRange(String Description);
