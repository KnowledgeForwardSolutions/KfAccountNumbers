namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Exception thrown when attempting to convert a string with an invalid US
///   Individual Taxpayer Identification Number to a
///   <see cref="UsIndividualTaxpayerIdentificationNumber"/>.
/// </summary>
/// <param name="validationResult">
///   Enum value that indicates the validation rule that was failed during the
///   conversion.
/// </param>
public class InvalidUsIndividualTaxpayerIdentificationNumberException(UsIndividualTaxpayerIdentificationNumberValidationResult validationResult)
   : InvalidAccountNumberException<UsIndividualTaxpayerIdentificationNumberValidationResult>(
      validationResult,
      validationResult.ToErrorDescription())
{
}
