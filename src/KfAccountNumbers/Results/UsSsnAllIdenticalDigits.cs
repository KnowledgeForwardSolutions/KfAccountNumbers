namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a US Social Security Number
///   (SSN) is composed of nine identical digits.
/// </summary>
/// <param name="Description">
///   Description of the validation error.
/// </param>
public readonly record struct UsSsnAllIdenticalDigits(String Description);
