// Ignore Spelling: Kennitala

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible results returned when validating an Icelandic
///   kennitala number.
/// </summary>
public enum IsKennitalaValidationResult
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
   ///   The value has incorrect length. Must be 10 or 11 characters in length.
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   The value contains a non-digit character in where a digit was expected.
   /// </summary>
   InvalidCharacter,

   /// <summary>
   ///   The check digit in the second position from the right of the number is
   ///   not a valid weighted modulus 11 check digit character.
   /// </summary>
   InvalidCheckDigit,

   /// <summary>
   ///   Eleven character value contains a digit character (0-9) in position 6
   ///   (zero-based). A valid separator may be any character other than a
   ///   digit.
   /// </summary>
   InvalidSeparator,

   /// <summary>
   ///   Trailing (right-most) character is not a valid century indicator. Valid
   ///   century indicator characters are '9' and '0'.
   /// </summary>
   InvalidCentury,

   /// <summary>
   ///   The date of birth specified by character positions 0-5 (zero-based) and
   ///   the century indicator in the right-most character position is not a valid
   ///   date in DDMMYY format.
   /// </summary>
   InvalidDateOfBirth,
}

/// <summary>
///   Additional methods for type <see cref="IsKennitalaValidationResult"/>.
/// </summary>
public static class IsKennitalaValidationResultExtensions
{
   public static String ToErrorDescription(this IsKennitalaValidationResult errorType)
      => errorType switch
      {
         IsKennitalaValidationResult.Empty => Messages.IsKennitalaEmpty,
         IsKennitalaValidationResult.InvalidLength => Messages.IsKennitalaInvalidLength,
         IsKennitalaValidationResult.InvalidCharacter => Messages.IsKennitalaInvalidCharacter,
         IsKennitalaValidationResult.InvalidCheckDigit => Messages.IsKennitalaInvalidCheckDigit,
         IsKennitalaValidationResult.InvalidSeparator => Messages.IsKennitalaInvalidSeparator,
         IsKennitalaValidationResult.InvalidCentury => Messages.IsKennitalaInvalidCentury,
         IsKennitalaValidationResult.InvalidDateOfBirth => Messages.IsKennitalaInvalidDateOfBirth,
         IsKennitalaValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };

   public static KfValidationException<IsKennitalaValidationResult> ToValidationException(
      this IsKennitalaValidationResult validationResult)
      => new(validationResult, validationResult.ToErrorDescription());
}
