// Ignore Spelling: Curp Mx

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Exception thrown when attempting to convert a string with an invalid 
///   Mexican CURP to a <see cref="MxCurp"/>
/// </summary>
/// <param name="validationResult">
///   Enum value that indicates the validation rule that was failed during the
///   conversion.
/// </param>
public class InvalidMxCurpException(MxCurpValidationResult validationResult)
   : InvalidAccountNumberException<MxCurpValidationResult>(
      validationResult,
      validationResult.ToErrorDescription())
{
}
