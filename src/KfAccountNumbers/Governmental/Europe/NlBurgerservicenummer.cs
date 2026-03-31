// Ignore Spelling: Burgerservicenummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a Dutch Burgerservicenummer
///   or BSN.
/// </summary>
/// <remarks>
///   <para>
///      A burgerservicenummer is a nine-digit number with no intelligent or
///      encoded attributes other than a trailing check digit calculated using
///      a variation of the modulus 11 algorithm (the 11-proef algorithm). The
///      number is typically displayed as nine consecutive digits or formatted
///      as NNNN-NN-NNN.
///   </para>
///   <para>
///      When creating a new <see cref="NlBurgerservicenummer"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 9 characters (without separators) or 11
///               characters (with separators) in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               All characters (except the optional separator characters) must be
///               ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               If the length of the value is 11 characters, then character
///               positions 4 and 7 (zero-based) must be valid separator characters.
///               Valid separator characters are any non-ASCII digit characters.
///               The same character must be used for both separator characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing (right-most) character position must be a valid
///               check digit calculated using the 11-proef variation of the
///               modulus 11 algorithm.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://nl.wikipedia.org/wiki/Burgerservicenummer for more info.
///   </para>
/// </remarks>
public record NlBurgerservicenummer
{
   private const Int32 UnformattedLength = 9;
   private const Int32 FormattedLength = 11;

   private const Int32 FirstSeparatorOffset = 4;
   private const Int32 SecondSeparatorOffset = 7;

   private const Int32 CheckDigitOffset = 1;    // Measured from end of value

   /// <summary>
   ///   Check the <paramref name="burgerservicenummer"/> to determine if it contains a
   ///   valid Dutch burgerservicenummer.
   /// </summary>
   /// <param name="burgerservicenummer">
   ///   String representation of a Dutch burgerservicenummer.
   /// </param>
   /// <returns>
   ///   A <see cref="NlBurgerservicenummerValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="burgerservicenummer"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static NlBurgerservicenummerValidationResult Validate(String? burgerservicenummer)
   {
      if (String.IsNullOrWhiteSpace(burgerservicenummer))
      {
         return NlBurgerservicenummerValidationResult.Empty;
      }
      else if (burgerservicenummer.Length is not UnformattedLength and not FormattedLength)
      {
         return NlBurgerservicenummerValidationResult.InvalidLength;
      }

      // After performing basic checks, validate the check digit because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      NlBurgerservicenummerValidationResult validationResult = ValidateCheckDigit(burgerservicenummer);
      if (validationResult != NlBurgerservicenummerValidationResult.ValidationPassed)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }
      else if (!ValidateSeparator(burgerservicenummer))
      {
         return NlBurgerservicenummerValidationResult.InvalidSeparator;
      }

      return NlBurgerservicenummerValidationResult.ValidationPassed;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> burgerservicenummer)
      => burgerservicenummer.Length == FormattedLength;

   private static NlBurgerservicenummerValidationResult ValidateCheckDigit(ReadOnlySpan<Char> burgerservicenummer)
   {
      var sum = 0;
      var weight = 9;
      var isFormatted = IsFormatted(burgerservicenummer);
      var processLength = burgerservicenummer.Length - 1;      // Handle check digit separately

      for (var index = 0; index < processLength; index ++)
      {
         if (isFormatted && (index == FirstSeparatorOffset || index == SecondSeparatorOffset))
         {
            continue;
         }

         var num = burgerservicenummer[index] - Chars.DigitZero;
         if (num < 0 || num > 9)
         {
            return NlBurgerservicenummerValidationResult.InvalidCharacter;
         }

         sum += (num * weight);
         weight--;
      }

      var checkDigit = burgerservicenummer[^CheckDigitOffset] - Chars.DigitZero;
      if (checkDigit < 0 || checkDigit > 9)
      {
         return NlBurgerservicenummerValidationResult.InvalidCharacter;
      }

      sum -= checkDigit;

      return sum % 11 == 0
         ? NlBurgerservicenummerValidationResult.ValidationPassed
         : NlBurgerservicenummerValidationResult.InvalidCheckDigit;
   }

   private static Boolean ValidateSeparator(ReadOnlySpan<Char> burgerservicenummer)
   {
      if (burgerservicenummer.Length == UnformattedLength)
      {
         return true;
      }

      var s1 = burgerservicenummer[FirstSeparatorOffset];
      var s2 = burgerservicenummer[SecondSeparatorOffset];

      return s1 == s2 && !s1.IsAsciiDigit();
   }
}
