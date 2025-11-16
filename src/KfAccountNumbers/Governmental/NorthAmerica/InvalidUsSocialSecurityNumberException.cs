namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Exception thrown when attempting to convert a string with an invalid US
///   Social Security number to a <see cref="UsSocialSecurityNumber"/>
/// </summary>
/// <param name="validationResult">
///   Enum value that indicates the validation rule that was failed during the
///   conversion.
/// </param>
public class InvalidUsSocialSecurityNumberException(UsSocialSecurityNumberValidationResult validationResult)
   : InvalidAccountNumberException<UsSocialSecurityNumberValidationResult>(
      validationResult, 
      validationResult.ToErrorDescription())
{
}
