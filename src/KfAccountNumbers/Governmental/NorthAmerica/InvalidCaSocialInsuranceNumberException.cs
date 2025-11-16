namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Exception thrown when attempting to convert a string with an invalid CA
///   Social Insurance number to a <see cref="CaSocialInsuranceNumber"/>.
/// </summary>
/// <param name="validationResult">
///   Enum value that indicates the validation rule that was failed during the
///   conversion.
/// </param>
public class InvalidCaSocialInsuranceNumberException(CaSocialInsuranceNumberValidationResult validationResult)
   : InvalidAccountNumberException<CaSocialInsuranceNumberValidationResult>(
      validationResult,
      validationResult.ToErrorDescription())
{
}
