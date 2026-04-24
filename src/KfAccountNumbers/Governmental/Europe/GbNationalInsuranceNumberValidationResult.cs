namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible results returned when validating a National Insurance
///   Number used in the UK.
/// </summary>
public enum GbNationalInsuranceNumberValidationResult
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
   ///   The value has incorrect length. Must be 8, 9, 11 or 13 characters in length.
   ///   (8 characters = unformatted, without suffix character, 9 characters = unformatted,
   ///   with suffix character, 11 characters = formatted, without suffix character,
   ///   13 characters = formatted, with suffix character)
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   The leading (left-most) two characters may not be BG, GB, NK, KN, TN, NT, or ZZ.
   /// </summary>
   InvalidPrefix,

   /// <summary>
   ///   The value has an invalid character in one or more locations. Character position 0
   ///   (zero-based) must contain A-C, E, G, H, J-P, R-T, W-Z. Character position 1
   ///   (zero-based) must contain A-C, E, G, H, J-N, P, R-T, W-Z. Character positions
   ///   2-7 (zero-based) must contain ASCII digits ('0'-'9'). Character position 8
   ///   (zero-based), if present, must contain A-D.
   /// </summary>
   InvalidCharacter,
}

/// <summary>
///   Additional methods for type <see cref="GbNationalInsuranceNumberValidationResult"/>.
/// </summary>
public static class GbNationalInsuranceNumberValidationResultExtensions
{
   public static String ToErrorDescription(this GbNationalInsuranceNumberValidationResult errorType)
      => errorType switch
      {
         GbNationalInsuranceNumberValidationResult.Empty => Messages.GbNationalInsuranceNumberEmpty,
         GbNationalInsuranceNumberValidationResult.InvalidLength => Messages.GbNationalInsuranceNumberInvalidLength,
         GbNationalInsuranceNumberValidationResult.InvalidPrefix => Messages.GbNationalInsuranceNumberInvalidPrefix,
         GbNationalInsuranceNumberValidationResult.InvalidCharacter => Messages.GbNationalInsuranceNumberInvalidCharacter,
         GbNationalInsuranceNumberValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };

   public static KfValidationException<GbNationalInsuranceNumberValidationResult> ToValidationException(
      this GbNationalInsuranceNumberValidationResult validationResult)
      => new(validationResult, validationResult.ToErrorDescription());
}
