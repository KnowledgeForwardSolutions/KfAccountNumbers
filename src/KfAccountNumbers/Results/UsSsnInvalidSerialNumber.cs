namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a US Social Security Number
///   (SSN) contains an invalid serial number component.
/// </summary>
/// <param name="Description">
///   Description of the validation error.
/// </param>
/// <param name="SerialNumber">
///   The invalid serial number.
/// </param>
public readonly record struct UsSsnInvalidSerialNumber(
   String Description,
   String SerialNumber);
