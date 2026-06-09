namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that the input value contains an
///   optional separator character that is invalid. Generally indicates that a
///   character indistinguishable from a valid character was found in a
///   separator position, or that the formatted value uses different characters
///   in the two separator positions.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that one or more
///   invalid separator were found in the input value.
/// </param>
/// <param name="Separator">
///   The invalid separator character that was found in the input value.
/// </param>
/// <param name="Position">
///   The zero-based index of the invalid separator character in the input
///   value.
/// </param>
public readonly record struct InvalidSeparator(
   String Description,
   Char Separator,
   Int32 Position);
