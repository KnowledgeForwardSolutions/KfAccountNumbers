namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that value includes a sequence
///   number element (such as a number to distinguish between persons born on
///   same date) contains an invalid sequence number.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that an invalid
///   sequence number was found in the input value.
/// </param>
/// <param name="SequenceNumber">
///   The invalid sequence number that was found in the input value.
/// </param>
public readonly record struct InvalidSequenceNumber(
   String Description,
   String SequenceNumber);
