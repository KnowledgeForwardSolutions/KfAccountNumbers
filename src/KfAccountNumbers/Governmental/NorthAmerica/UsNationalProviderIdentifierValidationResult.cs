namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Defines the possible results returned when validating a US National
///   Provider Identifier (NPI).
/// </summary>
public enum UsNationalProviderIdentifierValidationResult
{
   /// <summary>
   ///   The value does not contain any validation errors.
   /// </summary>
   ValidationPassed = 1,

   /// <summary>
   ///   NPI value is <see langword="null"/>, <see cref="String.Empty"/> or all
   ///   whitespace characters.
   /// </summary>
   Empty,

   /// <summary>
   ///   NPI value has incorrect length. Must be exactly 10 characters in length.
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   NPI value contains a non digit character.
   /// </summary>
   InvalidCharacterEncountered,

   /// <summary>
   ///   NPI check digit is invalid.
   /// </summary>
   InvalidCheckDigit,
}

/// <summary>
///   Additional methods for type <see cref="UsNationalProviderIdentifierValidationResult"/>.
/// </summary>
public static class UsNationalProviderIdentifierValidationResultExtensions
{
   public static String ToErrorDescription(this UsNationalProviderIdentifierValidationResult errorType)
      => errorType switch
      {
         UsNationalProviderIdentifierValidationResult.Empty => Messages.UsNpiEmpty,
         UsNationalProviderIdentifierValidationResult.InvalidLength => Messages.UsNpiInvalidLength,
         UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered => Messages.UsNpiInvalidCharacterEncountered,
         UsNationalProviderIdentifierValidationResult.InvalidCheckDigit => Messages.UsNpiInvalidCheckDigit,
         UsNationalProviderIdentifierValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };
}
