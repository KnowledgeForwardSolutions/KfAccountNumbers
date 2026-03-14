// Ignore Spelling: Personnummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible results returned when validating a Swedish Personal
///   Identity Number (Personnummer).
/// </summary>
public enum SePersonnummerValidationResult
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
   ///   The value has incorrect length. Must be 11 or 13 characters in length.
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   Character positions 0-5 (zero-based, for YYMMDD format) are not a
   ///   valid 6 digit date of birth or character positions 0-7 (zero-based,
   ///   for YYYYMMDD format) are not a valid 8 digit date of birth.
   /// </summary>
   InvalidDateOfBirth,

   /// <summary>
   ///   Character position 6 (zero-based, for YYMMDD format) or position 8
   ///   (zero-based, for YYYYMMDD format) is not a valid separator. Must be a
   ///   hyphen (-) or a plus sign (+).
   /// </summary>
   InvalidSeparator,

   /// <summary>
   ///   Character positions 7-9 (zero-based, for YYMMDD format) or positions
   ///   9-11 (zero-based, for YYYYMMDD format) contain a non-digit character.
   /// </summary>
   InvalidBirthSerialNumber,

   /// <summary>
   ///   Character position 10 (zero-based, for YYMMDD format) or position 12
   ///   (zero-based, for YYYYMMDD format) is not a valid check digit character
   ///   according to the Luhn algorithm.
   /// </summary>
   InvalidCheckDigit
}

/// <summary>
///   Additional methods for type <see cref="SePersonnummerValidationResult"/>.
/// </summary>
public static class SePersonnummerValidationResultExtensions
{
   public static String ToErrorDescription(this SePersonnummerValidationResult errorType)
      => errorType switch
      {
         SePersonnummerValidationResult.Empty => Messages.SePersonnummerEmpty,
         SePersonnummerValidationResult.InvalidLength => Messages.SePersonnummerInvalidLength,
         SePersonnummerValidationResult.InvalidDateOfBirth => Messages.SePersonnummerInvalidDateOfBirth,
         SePersonnummerValidationResult.InvalidSeparator => Messages.SePersonnummerInvalidSeparator,
         SePersonnummerValidationResult.InvalidBirthSerialNumber => Messages.SePersonnummerInvalidBirthSerialNumber,
         SePersonnummerValidationResult.InvalidCheckDigit => Messages.SePersonnummerInvalidCheckDigit,
         SePersonnummerValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };

   public static KfValidationException<SePersonnummerValidationResult> ToValidationException(
      this SePersonnummerValidationResult validationResult)
      => new(validationResult, validationResult.ToErrorDescription());
}
