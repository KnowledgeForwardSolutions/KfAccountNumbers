namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Defines the possible results returned when validating a Canadian Social 
///   Insurance Number.
/// </summary>
public enum CaSocialInsuranceNumberValidationResult
{
   /// <summary>
   ///   The value does not contain any validation errors.
   /// </summary>
   ValidationPassed = 1,

   /// <summary>
   ///   SIN value is <see langword="null"/>, <see cref="String.Empty"/> or all
   ///   whitespace characters.
   /// </summary>
   Empty,

   /// <summary>
   ///   SIN value has incorrect length. Must be either 9 characters (digits 
   ///   only) or 11 characters (9 digits with two separator characters).
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   SIN value contains an unexpected character in a separator character
   ///   location. 
   /// </summary>
   /// <remarks>
   ///   A SIN with length 11 should have valid separator character between the 
   ///   three groups of digits. When using the default separator, the expected 
   ///   separator character is a dash ('-'). When using a custom separator, the 
   ///   expected separator character is the custom
   ///   separator.
   /// </remarks>
   InvalidSeparatorEncountered,

   /// <summary>
   ///   SIN value contains a non digit character.
   /// </summary>
   InvalidCharacterEncountered,

   /// <summary>
   ///   SIN check digit is invalid.
   /// </summary>
   InvalidCheckDigit,
}

/// <summary>
///   Additional methods for type <see cref="CaSocialInsuranceNumberValidationResult"/>.
/// </summary>
public static class CaSocialInsuranceNumberValidationResultExtensions
{
   public static String ToErrorDescription(this CaSocialInsuranceNumberValidationResult errorType)
      => errorType switch
      {
         CaSocialInsuranceNumberValidationResult.Empty => Messages.CaSinEmpty,
         CaSocialInsuranceNumberValidationResult.InvalidLength => Messages.CaSinInvalidLength,
         CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered => Messages.CaSinInvalidSeparatorEncountered,
         CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered => Messages.CaSinInvalidCharacterEncountered,
         CaSocialInsuranceNumberValidationResult.InvalidCheckDigit => Messages.CaSinInvalidCheckDigit,
         CaSocialInsuranceNumberValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };
}