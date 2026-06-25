namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a CA Social Insurance
///   Number contains an invalid province identifier as the leading digit.
/// </summary>
/// <param name="Description">
///   Description of the validation error.
/// </param>
/// <param name="ProvinceCode">
///   The invalid province indicator.
/// </param>
public readonly record struct InvalidCaSinProvince(
   String Description,
   Char ProvinceCode);
