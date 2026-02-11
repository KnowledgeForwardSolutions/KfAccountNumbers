namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Defines the possible results returned when validating a US Individual
///   Taxpayer Identification Number (ITIN).
/// </summary>
public enum UsIndividualTaxpayerIdentificationNumberValidationResult
{
   /// <summary>
   ///   The value does not contain any validation errors.
   /// </summary>
   ValidationPassed = 1,

   /// <summary>
   ///   ITIN value is <see langword="null"/>, <see cref="String.Empty"/> or all
   ///   whitespace characters.
   /// </summary>
   Empty,

   /// <summary>
   ///   ITIN value has incorrect length. Must be either 9 characters (digits 
   ///   only) or 11 characters (9 digits with two separator characters).
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   ITIN value contains an unexpected character in a separator character
   ///   location. 
   /// </summary>
   /// <remarks>
   ///   A ITIN with length 11 should have valid separator character between the 
   ///   area, group and serial number sections. When using the default
   ///   separator, the expected separator character is a dash ('-'). When using
   ///   a custom separator, the expected separator character is the custom
   ///   separator.
   /// </remarks>
   InvalidSeparatorEncountered,

   /// <summary>
   ///   ITIN value contains a non digit character.
   /// </summary>
   InvalidCharacterEncountered,

   /// <summary>
   ///   ITIN area number segment is invalid (may not be 000-899).
   /// </summary>
   InvalidAreaNumber,

   /// <summary>
   ///   ITIN group number segment is invalid (may not be 00-49, 66-69, 89 or 93).
   /// </summary>
   InvalidGroupNumber,
}

/// <summary>
///   Additional methods for type <see cref="UsSocialSecurityNumberValidationResult"/>.
/// </summary>
public static class UsIndividualTaxpayerIdentificationNumberValidationResultExtensions
{
   public static String ToErrorDescription(this UsIndividualTaxpayerIdentificationNumberValidationResult errorType)
      => errorType switch
      {
         UsIndividualTaxpayerIdentificationNumberValidationResult.Empty => Messages.UsItinEmpty,
         UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidLength => Messages.UsItinInvalidLength,
         UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidSeparatorEncountered => Messages.UsItinInvalidSeparatorEncountered,
         UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidCharacterEncountered => Messages.UsItinInvalidCharacterEncountered,
         UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidAreaNumber => Messages.UsItinInvalidAreaNumber,
         UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidGroupNumber => Messages.UsItinInvalidGroupNumber,
         UsIndividualTaxpayerIdentificationNumberValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };
}
