// Ignore Spelling: Fodselsnummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible results returned when validating a Norwegian
///   fødselsnummer or D-nummer.
/// </summary>
public enum NoFodselsnummerValidationResult
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
   ///   The date of birth specified by character positions 0-5 (zero-based) and
   ///   the century indicator in character position 6 or 7 (zero-based,
   ///   depending on the presence of a separator character) is not a valid date
   ///   in DDMMYY format.
   /// </summary>
   InvalidDateOfBirth,

   /// <summary>
   ///   Twelve character value contains a digit character (0-9) in position 6
   ///   (zero-based). A valid separator may be any character other than a
   ///   digit.
   /// </summary>
   InvalidSeparator,

   /// <summary>
   ///   Character positions 6-8 (zero-based) or character positions 7-9 (zero-
   ///   based), depending on the presence of a separator character, contain a
   ///   non-digit character.
   /// </summary>
   InvalidIndividualNumber,

   /// <summary>
   ///   One or both of the two trailing (right-most) characters are not valid
   ///   weighted modulus 11 check digit characters.
   /// </summary>
   InvalidCheckDigits,
}

/// <summary>
///   Additional methods for type <see cref="NoFodselsnummerValidationResult"/>.
/// </summary>
public static class NoFodselsnummerValidationResultResultExtensions
{
   public static String ToErrorDescription(this NoFodselsnummerValidationResult errorType)
      => errorType switch
      {
         NoFodselsnummerValidationResult.Empty => Messages.NoFodselsnummerEmpty,
         NoFodselsnummerValidationResult.InvalidLength => Messages.NoFodselsnummerInvalidLength,
         NoFodselsnummerValidationResult.InvalidDateOfBirth => Messages.NoFodselsnummerInvalidDateOfBirth,
         NoFodselsnummerValidationResult.InvalidSeparator => Messages. NoFodselsnummerInvalidSeparator,
         NoFodselsnummerValidationResult.InvalidIndividualNumber => Messages.NoFodselsnummerInvalidIndividualNumber,
         NoFodselsnummerValidationResult.InvalidCheckDigits => Messages.NoFodselsnummerInvalidCheckDigits,
         NoFodselsnummerValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };

   public static KfValidationException<NoFodselsnummerValidationResult> ToValidationException(
      this NoFodselsnummerValidationResult validationResult)
      => new(validationResult, validationResult.ToErrorDescription());
}
