namespace KfAccountNumbers.Results;

/// <summary>
///   The ValidLengthDefinition struct describes a valid length for an input
///   value.
/// </summary>
/// <param name="Length">
///   The valid length for the input value.
/// </param>
/// <param name="Description">
///   A description of the valid length.
/// </param>
/// <remarks>
///   Note that some values may have multiple valid lengths, so this struct can
///   be used to represent one of those valid lengths along with a description.
///   For example, a value may allow an unformatted length of 10 characters or a
///   formatted length of 14 characters, and this struct can be used to
///   represent each of those valid lengths along with a description of what
///   they represent (e.g., "Unformatted Length" and "Formatted Length").
/// </remarks>
public record struct ValidLengthDefinition(Int32 Length, String Description)
{
}
