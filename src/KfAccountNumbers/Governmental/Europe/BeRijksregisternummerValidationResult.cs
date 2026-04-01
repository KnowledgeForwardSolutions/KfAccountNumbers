// Ignore Spelling: Rijksregisternummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible results returned when validating a Belgian
///   rijksregisternummer or BIS-nummer.
/// </summary>
public enum BeRijksregisternummerValidationResult
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
   ///   The value has incorrect length. Must be 11 or 15 characters in length.
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   The value contains a non-digit character where a digit was expected.
   /// </summary>
   InvalidCharacter,

   /// <summary>
   ///   Trailing (right-most) two characters are not a valid modulus 97 check
   ///   sum.
   /// </summary>
   InvalidCheckDigits,

   /// <summary>
   ///   15 character value contains a digit character (0-9) in a separator
   ///   location (positions 2, 5, 8, and 12, zero-based). A valid separator may
   ///   be any character other than a digit.
   /// </summary>
   InvalidSeparator,

   /// <summary>
   ///   The date of birth specified by leading (left-most) six digits and the
   ///   check digits is not a valid date in YYMMDD format.
   /// </summary>
   InvalidDateOfBirth,
}

/// <summary>
///   Additional methods for type <see cref="BeRijksregisternummerValidationResult"/>.
/// </summary>
public static class BeRijksregisternummerValidationResultExtensions
{
   public static String ToErrorDescription(this BeRijksregisternummerValidationResult errorType)
      => errorType switch
      {
         BeRijksregisternummerValidationResult.Empty => Messages.BeRijksregisternummerEmpty,
         BeRijksregisternummerValidationResult.InvalidLength => Messages.BeRijksregisternummerInvalidLength,
         BeRijksregisternummerValidationResult.InvalidCharacter => Messages.BeRijksregisternummerInvalidCharacter,
         BeRijksregisternummerValidationResult.InvalidCheckDigits => Messages.BeRijksregisternummerInvalidCheckDigits,
         BeRijksregisternummerValidationResult.InvalidSeparator => Messages.BeRijksregisternummerInvalidSeparator,
         BeRijksregisternummerValidationResult.InvalidDateOfBirth => Messages.BeRijksregisternummerInvalidDateOfBirth,
         BeRijksregisternummerValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };

   public static KfValidationException<BeRijksregisternummerValidationResult> ToValidationException(
      this BeRijksregisternummerValidationResult validationResult)
      => new(validationResult, validationResult.ToErrorDescription());
}
