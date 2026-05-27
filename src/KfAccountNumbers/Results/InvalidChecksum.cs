namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that the input value contains an
///   invalid check digit or checksum.
/// </summary>
/// <remarks>
///   The exact checksum validation rules depend on the specific type of
///   identifier being validated. Common checksum algorithms include the Luhn
///   algorithm, Modulo 10, Modulo 11, MOD 97-10, and others.
/// </remarks>
/// <param name="Description">
///   Description of the checksum validation error.
///   </param>
/// <param name="AlgorithmName">
///   Name of the algorithm used for checksum validation.
///   </param>
public record struct InvalidChecksum(
   String Description,
   String AlgorithmName)
{
}
