namespace KfAccountNumbers.Results;

/// <summary>
///   The InvalidStateProvince struct represents a validation error indicating
///   that the state/province encoded in the value is invalid.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that one or more
///   invalid characters were found in the input value.
/// </param>
/// <param name="StateProvinceCode">
///   The invalid state/province code that was found in the input value.
/// </param>
public readonly record struct InvalidStateProvince(
   String Description,
   String StateProvinceCode);
