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

   private static readonly Int32[] _weights = [3, 2, 7, 6, 5, 4, 3, 2, 1];

   /// <summary>
   ///   The raw kennitala value.
   /// </summary>
   public String Value { get; private init; }

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
      else if (!ValidateSeparator(kennitala))
      {
         return IsKennitalaValidationResult.InvalidSeparator;
      }
      else if (!ValidateDateOfBirth(kennitala))
      {
         return IsKennitalaValidationResult.InvalidDateOfBirth;
      }

      return IsKennitalaValidationResult.ValidationPassed;
   }

   private static (Int32 day, Int32 month, Int32 year) GetDayMonthYear(ReadOnlySpan<Char> kennitala)
   {
      var day = kennitala.ParseTwoDigits();
      var month = kennitala[2..].ParseTwoDigits();
      var year = kennitala[4..].ParseTwoDigits();
      year += kennitala[^CenturyIndicatorOffset] == Chars.DigitNine ? 1900 : 2000;

      // Adjust day for possible Fyrirtaeki.
      if (day > FyrirtaekiDayOffset)
      {
         day -= FyrirtaekiDayOffset;
      }

      return (day, month, year);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> kennitala)
      => kennitala.Length == SeparatorLength;

   private static IsKennitalaValidationResult ValidateCenturyIndicator(ReadOnlySpan<Char> kennitala)
   {
      var ch = kennitala[^CenturyIndicatorOffset];
      if (!ch.IsAsciiDigit())                         // Check for ASCII digit because check digit validation doesn't evaluate century indicator
      {
         return IsKennitalaValidationResult.InvalidCharacter;
      }

      var num = kennitala[^CenturyIndicatorOffset] - Chars.DigitZero;
      return (num < 0 || num > 9)
         ? IsKennitalaValidationResult.InvalidCharacter           // Check here because check digit doesn't evaluate century indicator
         : (num == 0 || num == 9)                                 // Valid century indicators are 0 and 9
            ? IsKennitalaValidationResult.ValidationPassed
            : IsKennitalaValidationResult.InvalidCentury;
   }

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> foedselsnummer)
   {
#pragma warning disable IDE0008 // Use explicit type
      var (day, month, year) = GetDayMonthYear(foedselsnummer);
#pragma warning restore IDE0008 // Use explicit type

      if (month < 1 || month > 12)
      {
         return false;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }

   private static Boolean ValidateSeparator(ReadOnlySpan<Char> kennitala)
      => !IsFormatted(kennitala) || !kennitala[SeparatorOffset].IsAsciiDigit();

   private static IsKennitalaValidationResult ValidateCheckDigit(ReadOnlySpan<Char> kennitala)
   {
      // Note that while the documentation in the linked articles does not
      // explicitly state it, it appears that values that would result in a
      // check digit of 10 are not issued. This is consistent with other
      // modulus 11 check digit examples such as Norwegian fødselsnummer.

      var sum = 0;
      var weightIndex = 0;
      var isFormatted = IsFormatted(kennitala);
      var processLength = kennitala.Length - 1;
      for(var index = 0; index < processLength; index++)
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

      return (sum % 11) == 0
         ? IsKennitalaValidationResult.ValidationPassed
         : IsKennitalaValidationResult.InvalidCheckDigit;
   }
}
