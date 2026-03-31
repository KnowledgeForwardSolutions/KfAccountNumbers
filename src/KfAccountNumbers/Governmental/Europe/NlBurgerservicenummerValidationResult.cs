// Ignore Spelling: Burgerservicenummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible results returned when validating Dutch burgerservicenummer.
/// </summary>
public enum NlBurgerservicenummerValidationResult
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
   ///   The value has incorrect length. Must be 9 or 11 characters in length.
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   The value contains a non-digit character where a digit was expected.
   /// </summary>
   InvalidCharacter,

   /// <summary>
   ///   Trailing (right-most) character is not a valid modulus 11 (11-proef) check digit.
   /// </summary>
   InvalidCheckDigit,

   /// <summary>
   ///   Eleven character value contains a digit character (0-9) in a separator
   ///   location (positions 4 and 7, zero-based). A valid separator may be any
   ///   character other than a digit.
   ///   - or -
   ///   Two different non-digit characters were used as separator characters.
   ///   The same separator character must be used in both locations.
   /// </summary>
   InvalidSeparator,
}

/// <summary>
///   Additional methods for type <see cref="NlBurgerservicenummerValidationResult"/>.
/// </summary>
public static class NlBurgerservicenummerValidationResultExtensions
{
   public static String ToErrorDescription(this NlBurgerservicenummerValidationResult errorType)
      => errorType switch
      {
         NlBurgerservicenummerValidationResult.Empty => Messages.NlBurgerservicenummerEmpty,
         NlBurgerservicenummerValidationResult.InvalidLength => Messages.NlBurgerservicenummerInvalidLength,
         NlBurgerservicenummerValidationResult.InvalidCharacter => Messages.NlBurgerservicenummerInvalidCharacter,
         NlBurgerservicenummerValidationResult.InvalidSeparator => Messages.NlBurgerservicenummerInvalidSeparator,
         NlBurgerservicenummerValidationResult.InvalidCheckDigit => Messages.NlBurgerservicenummerInvalidCheckDigit,
         NlBurgerservicenummerValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };

   public static KfValidationException<NlBurgerservicenummerValidationResult> ToValidationException(
      this NlBurgerservicenummerValidationResult validationResult)
      => new(validationResult, validationResult.ToErrorDescription());
}
