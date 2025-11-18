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
   {
      if (!ValidateSeparatorCharacter(separator))
      {
         throw new ArgumentOutOfRangeException(
            nameof(separator),
            separator,
            Messages.UsSsnInvalidCustomSeparatorCharacter);
      }

      // Basic checks for empty/null and length and formatting.
      if (String.IsNullOrWhiteSpace(sin))
      {
         return CaSocialInsuranceNumberValidationResult.Empty;
      }
      else if (sin.Length != NonFormattedLength && sin.Length != FormattedLength)
      {
         return CaSocialInsuranceNumberValidationResult.InvalidLength;
      }
      else if (IsFormattedSin(sin) && !ValidateEmbeddedSeparatorCharacters(sin, separator))
      {
         return CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered;
      }

      // Validate the check digit and province code. Note that the check digit
      // validation also checks that all non-separator characters are digits.
      var checkDigitResult = ValidateCheckDigit(sin);
      if (checkDigitResult != CaSocialInsuranceNumberValidationResult.ValidationPassed)
      {
         return checkDigitResult;
      }
      else if (!ValidateProvince(sin))
      {
         return CaSocialInsuranceNumberValidationResult.InvalidProvince;
      }

      return CaSocialInsuranceNumberValidationResult.ValidationPassed;
   }

   private static Boolean IsFormattedSin(ReadOnlySpan<Char> sin) => sin.Length == FormattedLength;

   /// <summary>
   ///   Validate the check digit of the SIN using the Luhn algorithm. As a side
   ///   effect, this method also validates that all non-separator characters in 
   ///   the SIN are digits.
   /// </summary>
   private static CaSocialInsuranceNumberValidationResult ValidateCheckDigit(String sin)
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
         if (digit < 0 || digit > 9)
         {
            return CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered;
         }
         sum += oddPosition
            ? digit > 4 ? (digit * 2) - 9 : digit * 2
            : digit;
         oddPosition = !oddPosition;
      }

      var checkDigit = (10 - (sum % 10)) % 10;
      var trailingDigit = sin[^1] - Chars.DigitZero;
      if (trailingDigit < 0 || trailingDigit > 9)
      {
         return CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered;
      }

      return checkDigit == trailingDigit
         ? CaSocialInsuranceNumberValidationResult.ValidationPassed
         : CaSocialInsuranceNumberValidationResult.InvalidCheckDigit; 
   }

   private static Boolean ValidateEmbeddedSeparatorCharacters(
         ReadOnlySpan<Char> sin,
         Char separator)
      // If SIN is formatted, must contain valid separator character between sections.
      => sin.Length == NonFormattedLength || (sin[FirstSeparatorOffset] == separator && sin[SecondSeparatorOffset] == separator);

   private static Boolean ValidateProvince(ReadOnlySpan<Char> sin)
      => sin[0] != Chars.DigitZero && sin[0] != Chars.DigitEight;

   // A separator character may be any character except ASCII digits (which are
   // valid SIN characters).
   private static Boolean ValidateSeparatorCharacter(Char separator)
      => !separator.IsAsciiDigit();
}
