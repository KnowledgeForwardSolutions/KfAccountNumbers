namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a US Taxpayer Identification
///   Number (TIN) such as a Social Security Number (SSN) or an Individual
///   Taxpayer Identification Number (ITIN) contains an invalid group number
///   component.
/// </summary>
/// <param name="Description">
///   Description of the validation error.
/// </param>
/// <param name="GroupNumber">
///   The invalid group number.
/// </param>
public readonly record struct InvalidUsTinGroupNumber(
   String Description,
   String GroupNumber);
