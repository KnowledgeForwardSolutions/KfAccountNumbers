// Ignore Spelling: Foedselsnummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible results returned when validating a Norwegian
///   fødselsnummer or D-nummer.
/// </summary>
public enum NoFoedselsnummerValidationResult
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
   ///   The value has incorrect length. Must be 11 or 12 characters in length.
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   The value contains a non-digit character in where a digit was expected.
   /// </summary>
   InvalidCharacter,

   /// <summary>
   ///   One or both of the two trailing (right-most) characters are not valid
   ///   weighted modulus 11 check digit characters.
   /// </summary>
   InvalidCheckDigits,

   /// <summary>
   ///   Twelve character value contains a digit character (0-9) in position 6
   ///   (zero-based). A valid separator may be any character other than a
   ///   digit.
   /// </summary>
   InvalidSeparator,

   /// <summary>
   ///   The date of birth specified by character positions 0-5 (zero-based) and
   ///   the century indicator in character position 6 or 7 (zero-based,
   ///   depending on the presence of a separator character) is not a valid date
   ///   in DDMMYY format.
   /// </summary>
   InvalidDateOfBirth,
}

/// <summary>
///   Additional methods for type <see cref="NoFoedselsnummerValidationResult"/>.
/// </summary>
public static class NoFoedselsnummerValidationResultExtensions
{
   public static String ToErrorDescription(this NoFoedselsnummerValidationResult errorType)
      => errorType switch
      {
         NoFoedselsnummerValidationResult.Empty => Messages.NoFoedselsnummerEmpty,
         NoFoedselsnummerValidationResult.InvalidLength => Messages.NoFoedselsnummerInvalidLength,
         NoFoedselsnummerValidationResult.InvalidCharacter => Messages.NoFoedselsnummerInvalidCharacter,
         NoFoedselsnummerValidationResult.InvalidCheckDigits => Messages.NoFoedselsnummerInvalidCheckDigits,
         NoFoedselsnummerValidationResult.InvalidSeparator => Messages.NoFoedselsnummerInvalidSeparator,
         NoFoedselsnummerValidationResult.InvalidDateOfBirth => Messages.NoFoedselsnummerInvalidDateOfBirth,
         NoFoedselsnummerValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };

   public static KfValidationException<NoFoedselsnummerValidationResult> ToValidationException(
      this NoFoedselsnummerValidationResult validationResult)
      => new(validationResult, validationResult.ToErrorDescription());
}
