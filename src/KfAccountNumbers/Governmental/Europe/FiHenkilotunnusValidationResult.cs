// Ignore Spelling: Fi Henkilotunnus

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible results returned when validating a Finnish
///   henkilötunnus.
/// </summary>
public enum FiHenkilotunnusValidationResult
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
   ///   The value has incorrect length. Must be 11 characters in length.
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   The value contains a non-digit character where a digit was expected.
   /// </summary>
   InvalidCharacter,

   /// <summary>
   ///   Trailing (right-most) character position is not a valid modulus 31
   ///   check character.
   /// </summary>
   InvalidCheckDigit,

   /// <summary>
   ///   Century indicator (character position 6, zero-based) is not valid.
   ///   Valid century indicators are + (1800s), -, U, V, W, X or Y (1900s)
   ///   and A, B, C, D, E, F (2000s).
   /// </summary>
   InvalidCenturyIndicator,

   /// <summary>
   ///   The individual number (character positions 7-9, zero based) are "001",
   ///   which below the minimum range for valid individual numbers ("002-999").
   /// </summary>
   InvalidIndividualNumber,

   /// <summary>
   ///   The date of birth specified by character positions 0-5 (zero-based) and
   ///   the century indicator is not a valid date in DDMMYY format.
   /// </summary>
   InvalidDateOfBirth,
}

/// <summary>
///   Additional methods for type <see cref="FiHenkilotunnusValidationResult"/>.
/// </summary>
public static class FiHenkilotunnusValidationResultExtensions
{
   public static String ToErrorDescription(this FiHenkilotunnusValidationResult errorType)
      => errorType switch
      {
         FiHenkilotunnusValidationResult.Empty => Messages.FiHenkilotunnusEmpty,
         FiHenkilotunnusValidationResult.InvalidLength => Messages.FiHenkilotunnusInvalidLength,
         FiHenkilotunnusValidationResult.InvalidCharacter => Messages.FiHenkilotunnusInvalidCharacter,
         FiHenkilotunnusValidationResult.InvalidCheckDigit => Messages.FiHenkilotunnusInvalidCheckDigit,
         FiHenkilotunnusValidationResult.InvalidCenturyIndicator => Messages.FiHenkilotunnusInvalidCenturyIndicator,
         FiHenkilotunnusValidationResult.InvalidIndividualNumber => Messages.FiHenkilotunnusInvalidIndividualNumber,
         FiHenkilotunnusValidationResult.InvalidDateOfBirth => Messages.FiHenkilotunnusInvalidDateOfBirth,
         FiHenkilotunnusValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };

   public static KfValidationException<FiHenkilotunnusValidationResult> ToValidationException(
      this FiHenkilotunnusValidationResult validationResult)
      => new(validationResult, validationResult.ToErrorDescription());
}
