// Ignore Spelling: Insee

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible results returned when validating a French INSEE number.
/// </summary>
public enum FrInseeNumberValidationResult
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
   ///   The value has incorrect length. Must be 15 or 21 characters in length.
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
   ///   21 character value contains a digit character (0-9) in a separator
   ///   location (positions 1, 4, 7, 10, 15 and 18, zero-based). A valid separator
   ///   may be any character other than a digit and the same character must be
   ///   used in all locations.
   /// </summary>
   InvalidSeparator,

   /// <summary>
   ///   Leading (left-most) digit is not a valid gender value. Valid values are
   ///   1 (male) and 2 (female) or 7 (male) and 8 (female) for temporary
   ///   INSEE numbers.
   /// </summary>
   InvalidGender,

   /// <summary>
   ///   The two-digit month (character positions 3-4 in values without separators,
   ///   or character positions 5-6 in values with separators, all zero-based)
   ///   is invalid. Valid month values are between 01 and 12 (for known dates)
   ///   or 20-42, 50-99 (for persons with unknown or incomplete date of birth
   ///   documentation).
   /// </summary>
   InvalidMonth,

   /// <summary>
   ///   The two or three character department code (character positions 5-7 in
   ///   values without separators or character positions 8-10 in values with
   ///   separators, all zero-based) are not a value INSEE department code.
   /// </summary>
   InvalidDepartment,
}

/// <summary>
///   Additional methods for type <see cref="FrInseeNumberValidationResult"/>.
/// </summary>
public static class FrInseeNumberValidationResultExtensions
{
   public static String ToErrorDescription(this FrInseeNumberValidationResult errorType)
      => errorType switch
      {
         FrInseeNumberValidationResult.Empty => Messages.FrInseeNumberEmpty,
         FrInseeNumberValidationResult.InvalidLength => Messages.FrInseeNumberInvalidLength,
         FrInseeNumberValidationResult.InvalidCharacter => Messages.FrInseeNumberInvalidCharacter,
         FrInseeNumberValidationResult.InvalidCheckDigits => Messages.FrInseeNumberInvalidCheckDigits,
         FrInseeNumberValidationResult.InvalidSeparator => Messages.FrInseeNumberInvalidSeparator,
         FrInseeNumberValidationResult.InvalidGender => Messages.FrInseeNumberInvalidGender,
         FrInseeNumberValidationResult.InvalidMonth => Messages.MonthOutOfRange,
         FrInseeNumberValidationResult.InvalidDepartment => Messages.FrInseeNumberInvalidDepartment,
         FrInseeNumberValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };

   public static KfValidationException<FrInseeNumberValidationResult> ToValidationException(
      this FrInseeNumberValidationResult validationResult)
      => new(validationResult, validationResult.ToErrorDescription());
}
