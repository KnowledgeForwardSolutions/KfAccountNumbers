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

      // Validate the check digit and province code.
      var validCheckDigit = IsFormattedSin(sin)
         ? CheckDigitAlgorithms.Luhn.Validate(sin, CheckDigitMasks.CaSocialInsuranceNumberMask)
         : CheckDigitAlgorithms.Luhn.Validate(sin);
      if (!validCheckDigit)
      {
         // Either invalid check digit or invalid character encountered.
         return ValidateDigits(sin)
            ? CaSocialInsuranceNumberValidationResult.InvalidCheckDigit
            : CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered;
      }
      else if (!ValidateProvince(sin))
      {
         return CaSocialInsuranceNumberValidationResult.InvalidProvince;
      }

      return CaSocialInsuranceNumberValidationResult.ValidationPassed;
   }

   private static Boolean IsFormattedSin(ReadOnlySpan<Char> sin) => sin.Length == FormattedLength;

   private static Boolean ValidateDigits(String sin)
   {
      var isFormatted = IsFormattedSin(sin);
      for (var index = 0; index < sin.Length; index++)
      {
         if (isFormatted && (index == FirstSeparatorOffset || index == SecondSeparatorOffset))
         {
            continue;
         }
         if (!sin[index].IsAsciiDigit())
         {
            return false;
         }
      }
      return true;
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
