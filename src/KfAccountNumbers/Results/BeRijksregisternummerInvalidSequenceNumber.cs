namespace KfAccountNumbers.Results;

/// <summary>
///   The BeRijksregisternummerInvalidSequenceNumber struct represents a
///   validation error indicating that a Belgian rijksregisternummer value
///   contains an invalid sequence number component.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that an invalid
///   sequence number was found in the input value.
/// </param>
/// <param name="SequenceNumber">
///   The invalid sequence number that was found in the input value.
/// </param>
public readonly record struct BeRijksregisternummerInvalidSequenceNumber(
   String Description,
   String SequenceNumber);
