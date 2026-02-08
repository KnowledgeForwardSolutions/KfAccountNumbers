namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Exception thrown when attempting to convert a string with an invalid US
///   National Provider Identifier to a <see cref="UsNationalProviderIdentifier"/>.
/// </summary>
/// <param name="validationResult">
///   Enum value that indicates the validation rule that was failed during the
///   conversion.
/// </param>
public class InvalidUsNationalProviderIdentifierException(UsNationalProviderIdentifierValidationResult validationResult)
   : InvalidAccountNumberException<UsNationalProviderIdentifierValidationResult>(
      validationResult,
      validationResult.ToErrorDescription())
{
}
