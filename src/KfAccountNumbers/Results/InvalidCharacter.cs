namespace KfAccountNumbers.Results;

/// <summary>
///   The InvalidCharacter struct represents a validation error indicating that
///   one or more invalid characters were found in the input value.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that one or more
///   invalid characters were found in the input value.
/// </param>
/// <param name="Character">
///   The invalid character that was found in the input value.
/// </param>
/// <param name="Position">
///   The zero-based index of the invalid character in the input value.
/// </param>
public readonly record struct InvalidCharacter(
   String Description,
   Char Character,
   Int32 Position);
