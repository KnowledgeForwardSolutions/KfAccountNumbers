namespace KfAccountNumbers.Results;

/// <summary>
///   Represents an invalid length validation result with a description of the
///   error, the invalid length value, and the valid length definitions that
///   specify the expected length constraints.
/// </summary>
/// <param name="Description">
///   Description of the length validation error.
///   </param>
/// <param name="Length">
///   The actual length of the value that failed validation.
/// </param>
/// <param name="ValidLengths">
///   Valid length definitions that specify the expected length constraints.
/// </param>
public readonly record struct InvalidLength(
   String Description,
   Int32 Length,
   params ValidLengthDefinition[] ValidLengths);
