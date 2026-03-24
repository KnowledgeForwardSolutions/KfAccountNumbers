// Ignore Spelling: Kennitala

namespace KfAccountNumbers.Governmental.Europe;

public record IsKennitala
{
   /// <summary>
   ///   Represents the day offset used to distinguish personal (Einstaklingur)
   ///   kennitala values from company (Fyrirtaeki) kennitala values.
   /// </summary>
   /// <remarks>
   ///   For Icelandic kennitala numbers, a Fyrirtaeki kennitala is indicated by
   ///   adding 40 to the day component of the date of birth.
   /// </remarks>
   public const Int32 FyrirtaekiDayOffset = 40;

   /// <summary>
   ///   The latest year of birth supported by <see cref="IsKennitala"/>.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2099;

   /// <summary>
   ///   The earliest year of birth supported by <see cref="IsKennitala"/>.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1900;

   private const Int32 NoSeparatorLength = 10;
   private const Int32 SeparatorLength = 11;

   private const Int32 SeparatorOffset = 6;

   // These offsets measure from the right side of the value.
   private const Int32 CheckDigitOffset = 2;
   private const Int32 CenturyIndicatorOffset = 1;

   private static readonly Int32[] _weights = [2, 3, 4, 5, 6, 7, 2, 3];

   /// <summary>
   ///   Check the <paramref name="kennitala"/> to determine if it contains a
   ///   valid Icelandic kennitala number.
   /// </summary>
   /// <param name="kennitala">
   ///   String representation of an Icelandic kennitala number.
   /// </param>
   /// <returns>
   ///   A <see cref="IsKennitala"/> enumeration 
   ///   value that indicates if the <paramref name="kennitala"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static IsKennitalaValidationResult Validate(String? kennitala)
   {
      if (String.IsNullOrWhiteSpace(kennitala))
      {
         return IsKennitalaValidationResult.Empty;
      }
      else if (kennitala.Length is not NoSeparatorLength and not SeparatorLength)
      {
         return IsKennitalaValidationResult.InvalidLength;
      }

      // After performing basic checks, validate the check digits because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      IsKennitalaValidationResult validationResult = ValidateCheckDigit(kennitala);
      if (validationResult != IsKennitalaValidationResult.ValidationPassed)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }
      validationResult = ValidateCenturyIndicator(kennitala);
      if (validationResult != IsKennitalaValidationResult.ValidationPassed)
      {
         // Could be either InvalidCharacter or InvalidCentury.
         return validationResult;
      }

      throw new NotImplementedException();
   }

   private static IsKennitalaValidationResult ValidateCenturyIndicator(ReadOnlySpan<Char> kennitalia)
   {
      var num = kennitalia[^CenturyIndicatorOffset] - Chars.DigitZero;
      return (num < 0 || num > 9)
         ? IsKennitalaValidationResult.InvalidCharacter           // Check here because check digit doesn't cover century indicator
         : (num == 0 || num == 9)                                 // Valid century indicators are 0 and 9
            ? IsKennitalaValidationResult.ValidationPassed
            : IsKennitalaValidationResult.InvalidCentury;
   }

   private static IsKennitalaValidationResult ValidateCheckDigit(ReadOnlySpan<Char> kennitala)
   {
      var sum = 0;
      var weightIndex = 0;
      var isFormatted = kennitala.Length == SeparatorLength;
      for(var index = kennitala.Length - 3; index >= 0; index --)       // exclude both century indicator and check digit
      {
         if (isFormatted && index == SeparatorOffset)
         {
            continue;
         }

         var num = kennitala[index] - Chars.DigitZero;
         if (num < 0 || num > 9)
         {
            return IsKennitalaValidationResult.InvalidCharacter;
         }

         sum += num * _weights[weightIndex];
         weightIndex++;
      }

      var remainder = sum % 11;
      var calculatedCheckDigit = remainder == 0 ? 0 : 11 - remainder;

      var checkDigit = kennitala[^CheckDigitOffset] - Chars.DigitZero;
      if (checkDigit < 0 || checkDigit > 9)
      {
         return IsKennitalaValidationResult.InvalidCharacter;
      }

      return calculatedCheckDigit == checkDigit
         ? IsKennitalaValidationResult.ValidationPassed
         : IsKennitalaValidationResult.InvalidCheckDigit;
   }
}
