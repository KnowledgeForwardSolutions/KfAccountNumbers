// Ignore Spelling: Nif

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible results returned when validating a Spanish documento
///   nacional de identidad (DNI) or número de identificación de extranjero (NIE).
/// </summary>
public enum EsNifValidationResult
{
   /// <summary>
   ///   The value does not contain any validation errors.
   /// </summary>
   ValidationPassed = 1,

   /// <summary>
   ///   The value is <see langword="null"/>, <see cref="String.Empty"/> or all
   ///   whitespace characters.
   /// </summary>
   Empty,

   /// <summary>
   ///   The value has incorrect length. Must be 9 characters in length (without
   ///   separators or 10 characters in length (DNI with separator) or 11
   ///   characters in length (NIE with two separator characters).
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   The value contains a character that was not expected. For DNI, the
   ///   leading (left-most) eight characters must be ASCII digits ('0'-'9'). For
   ///   NIE, the leading character must be X, Y or Z and the next seven non-separator
   ///   characters must be ASCII digits.
   /// </summary>
   InvalidCharacter,

   /// <summary>
   ///   The trailing (right-most) character is not a valid modulus 23 check character.
   /// </summary>
   InvalidCheckDigit,

   /// <summary>
   ///   A 10-character DNI contains an ASCII digit ('0'-'9') in character position
   ///   8 (zero-based) or an 11-character NIE contains an ASCII digit in character
   ///   position 1 or 9 (zero-based) or an 11-character NIE uses two different
   ///   separator characters.
   /// </summary>
   InvalidSeparator,
}

/// <summary>
///   Additional methods for type <see cref="EsNifValidationResult"/>.
/// </summary>
public static class EsNifValidationResultExtensions
{
   public static String ToErrorDescription(this EsNifValidationResult errorType)
      => errorType switch
      {
         EsNifValidationResult.Empty => Messages.EsNifEmpty,
         EsNifValidationResult.InvalidLength => Messages.EsNifInvalidLength,
         EsNifValidationResult.InvalidCharacter => Messages.EsNifInvalidCharacter,
         EsNifValidationResult.InvalidCheckDigit => Messages.EsNifInvalidCheckDigit,
         EsNifValidationResult.InvalidSeparator => Messages.EsNifInvalidSeparator,
         EsNifValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };

   public static KfValidationException<EsNifValidationResult> ToValidationException(
      this EsNifValidationResult validationResult)
      => new(validationResult, validationResult.ToErrorDescription());
}
