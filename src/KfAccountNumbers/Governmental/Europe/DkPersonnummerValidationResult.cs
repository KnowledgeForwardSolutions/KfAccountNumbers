// Ignore Spelling: Personnummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible results returned when validating Danish personnummer.
/// </summary>
public enum DkPersonnummerValidationResult
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
   ///   The value contains a non-digit character where a digit was expected.
   /// </summary>
   InvalidCharacter,

   /// <summary>
   ///   Eleven character value contains an invalid separator at character
   ///   position six (zero based). The separator character, if included, must
   ///   be a dash ('-').
   /// </summary>
   InvalidSeparator,

   /// <summary>
   ///   The date of birth specified by character positions 0-5 (zero-based) and
   ///   the century indicator (the first digit following the date of birth) is
   ///   not a valid date in DDMMYY format.
   /// </summary>
   InvalidDateOfBirth,
}

/// <summary>
///   Additional methods for type <see cref="DkPersonnummerValidationResult"/>.
/// </summary>
public static class DkPersonnummerValidationResultExtensions
{
   public static String ToErrorDescription(this DkPersonnummerValidationResult errorType)
      => errorType switch
      {
         DkPersonnummerValidationResult.Empty => Messages.DkPersonnummerEmpty,
         DkPersonnummerValidationResult.InvalidLength => Messages.DkPersonnummerInvalidLength,
         DkPersonnummerValidationResult.InvalidCharacter => Messages.DkPersonnummerInvalidCharacter,
         DkPersonnummerValidationResult.InvalidSeparator => Messages.DkPersonnummerInvalidSeparator,
         DkPersonnummerValidationResult.InvalidDateOfBirth => Messages.DkPersonnummerInvalidDateOfBirth,
         DkPersonnummerValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };

   public static KfValidationException<DkPersonnummerValidationResult> ToValidationException(
      this DkPersonnummerValidationResult validationResult)
      => new(validationResult, validationResult.ToErrorDescription());
}
