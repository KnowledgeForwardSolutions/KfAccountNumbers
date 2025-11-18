// Ignore Spelling: Luhn

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Strongly typed business object for a CA Social Insurance Number (SIN).
/// </summary>
public record CaSocialInsuranceNumber
{
   public const Char DefaultSeparator = Chars.Dash;

   private const Int32 FormattedLength = 11;
   private const Int32 NonFormattedLength = 9;

   private const Int32 FirstSeparatorOffset = 3;
   private const Int32 SecondSeparatorOffset = 7;

   /// <summary>
   ///   Check the <paramref name="sin"/> to determine if it contains any 
   ///   validation errors.
   /// </summary>
   /// <param name="sin">
   ///   String representation of a Social Insurance Number.
   /// </param>
   /// <param name="separator">
   ///   Optional. If the <paramref name="sin"/> is 11 characters in length, 
   ///   then <paramref name="separator"/> identifies the character used to
   ///   separate the different sections of the SSN. This parameter is ignored 
   ///   if the <paramref name="sin"/> is 9 characters in length. Defaults to '-'.
   /// </param>
   /// <returns>
   ///   A <see cref="CaSocialInsuranceNumberValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="sin"/> passed validation
   ///   or what validation error was encountered.
   /// </returns>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="separator"/> is an ASCII digit (0-9).
   /// </exception>
   public static CaSocialInsuranceNumberValidationResult Validate(
      String? sin,
      Char separator = DefaultSeparator)
      => !ValidateSeparatorCharacter(separator)
         ? throw new ArgumentOutOfRangeException(nameof(separator), separator, Messages.UsSsnInvalidCustomSeparatorCharacter)
         : ValidateSin(sin, separator);

   private static Boolean IsFormattedSin(ReadOnlySpan<Char> sin) => sin.Length == FormattedLength;

   private static Boolean ValidateAllDigits(ReadOnlySpan<Char> sin)
   {
      var index = 0;
      while (index < sin.Length)
      {
         if (IsFormattedSin(sin) && (index == FirstSeparatorOffset || index == SecondSeparatorOffset))
         {
            index++;
         }

         if (!sin[index].IsAsciiDigit())
         {
            return false;
         }

         index++;
      }

      return true;
   }

   private static Boolean ValidateEmbeddedSeparatorCharacters(
         ReadOnlySpan<Char> sin,
         Char separator)
      // If SIN is formatted, must contain valid separator character between sections.
      => sin.Length == NonFormattedLength || (sin[FirstSeparatorOffset] == separator && sin[SecondSeparatorOffset] == separator);

   private static Boolean ValidateLuhnCheckDigit(ReadOnlySpan<Char> sin)
   {
      var sum = 0;
      var oddPosition = true;
      for (var index = sin.Length - 2; index >= 0; index--)
      {
         if (IsFormattedSin(sin) && (index == FirstSeparatorOffset || index == SecondSeparatorOffset))
         {
            continue;
         }
         var digit = sin[index] - Chars.DigitZero;
         sum += oddPosition
            ? digit > 4 ? (digit * 2) - 9 : digit * 2
            : digit;
         oddPosition = !oddPosition;
      }

      var checkDigit = (10 - (sum % 10)) % 10;

      return checkDigit == (sin[^1] - Chars.DigitZero);
   }

   private static Boolean ValidateProvince(ReadOnlySpan<Char> sin)
      => sin[0] != Chars.DigitZero && sin[0] != Chars.DigitEight;

   // A separator character may be any character except ASCII digits (which are
   // valid SIN characters).
   private static Boolean ValidateSeparatorCharacter(Char separator)
      => !separator.IsAsciiDigit();

   private static Boolean ValidateLength(ReadOnlySpan<Char> sin)
      => sin.Length == NonFormattedLength || sin.Length == FormattedLength;

   private static CaSocialInsuranceNumberValidationResult ValidateSin(
      ReadOnlySpan<Char> sin,
      Char separator)
   {
      // Preliminary checks for obviously incorrect values.
      if (sin.IsEmpty || sin.IsWhiteSpace())
      {
         return CaSocialInsuranceNumberValidationResult.Empty;
      }
      if (!ValidateLength(sin))
      {
         return CaSocialInsuranceNumberValidationResult.InvalidLength;
      }
      if (IsFormattedSin(sin) && !ValidateEmbeddedSeparatorCharacters(sin, separator))
      {
         return CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered;
      }
      if (!ValidateAllDigits(sin))
      {
         return CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered;
      }
      if (!ValidateProvince(sin))
      {
         return CaSocialInsuranceNumberValidationResult.InvalidProvince;
      }
      if (!ValidateLuhnCheckDigit(sin))
      {
         return CaSocialInsuranceNumberValidationResult.InvalidCheckDigit;
      }

      return CaSocialInsuranceNumberValidationResult.ValidationPassed;
   }
}
