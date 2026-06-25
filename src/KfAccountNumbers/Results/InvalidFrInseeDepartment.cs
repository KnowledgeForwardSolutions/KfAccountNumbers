namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a French INSEE number
///   contains an invalid department code.
/// </summary>
/// <param name="Description">
///   Description of the validation error.
/// </param>
/// <param name="Department">
///   The invalid department code.
/// </param>
public readonly record struct InvalidFrInseeDepartment(
   String Description,
   String Department);
