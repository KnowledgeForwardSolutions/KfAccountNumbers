namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible results returned when validating an Irish Personal
///   Public Service Number.
/// </summary>
public enum IePpsNumberValidationResult
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
   ///   The value has incorrect length. Must be 8 or 9 characters in length.
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   The value contains a character that was not expected. The leading (left-most)
   ///   seven characters must be ASCII digits ('0'-'9'), the eighth character must be
   ///   a letter in the range A-W and the optional ninth character must be a letter
   ///   in the range A-I or W.
   /// </summary>
   InvalidCharacter,

   /// <summary>
   ///   The eighth character is not a valid weighted modulus 23 check character.
   /// </summary>
   InvalidCheckDigit,
}

/// <summary>
///   Additional methods for type <see cref="IePpsNumberValidationResult"/>.
/// </summary>
public static class IePpsNumberValidationResultExtensions
{
   public static String ToErrorDescription(this IePpsNumberValidationResult errorType)
      => errorType switch
      {
         IePpsNumberValidationResult.Empty => Messages.IePpsNumberEmpty,
         IePpsNumberValidationResult.InvalidLength => Messages.IePpsNumberInvalidLength,
         IePpsNumberValidationResult.InvalidCharacter => Messages.IePpsNumberInvalidCharacter,
         IePpsNumberValidationResult.InvalidCheckDigit => Messages.IePpsNumberInvalidCheckDigit,
         IePpsNumberValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };

   public static KfValidationException<IePpsNumberValidationResult> ToValidationException(
      this IePpsNumberValidationResult validationResult)
      => new(validationResult, validationResult.ToErrorDescription());
}
