// Ignore Spelling: Fi Henkilotunnus

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a Finnish Personal Identity
///   Code (henkilötunnus).
/// </summary>
/// <remarks>
///   <para>
///      A Danish personnummer is a ten-digit number structured as DDMMYYCZZZQ,
///      with the following elements:
///      <list type="bullet">
///         <item>
///            <term>DDMMYY</term>
///            <description>
///               The person's date of birth in DDMMYY format.
///            </description>
///         </item>
///         <item>
///            <term>C</term>
///            <description>
///               Century indicator, with + indicating 1800s, -, U, V, W, X or Y
///               indicating 1900s and A, B, C, D, E, F indicating 2000s.
///            </description>
///         </item>
///         <item>
///            <term>ZZZ</term>
///            <description>
///               Three digit individual number used to differentiate between two
///               persons born on the same date. The individual number also encodes
///               additional information. The person's gender is indicated with
///               even numbers for females and odd numbers for males. Individual
///               numbers between 002 and 899 indicate persons born in Finland or
///               permanent residents and numbers between 900 and 999 are reserved
///               for temporary identifiers. The individual number 001 is not
///               valid.
///            </description>
///         </item>
///         <item>
///            <term>Q</term>
///            <description>
///               Check character, calculated as modulus 31 of the date of birth
///               and individual number. Will be one of 31 alphanumeric characters,
///               "0123456789ABCDEFHJKLMNPRSTUVWXY" (the letters `G, I, O, Q and Z`
///               are excluded to avoid possible confusion with digit characters).
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>230526-034N</term>
///            <description>
///               Date of birth May 23, 1926, gender = female, permanent resident
///            </description>
///         </item>
///         <item>
///            <term>160117A275C</term>
///            <description>
///               Date of birth January 16, 2017, gender = male, permanent resident
///            </description>
///         </item>
///         <item>
///            <term>020508D929B</term>
///            <description>
///               Date of birth May 2, 2008, gender = male, temporary/test value
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      When creating a new <see cref="FiHenkilotunnus"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be 11 characters in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth and individual number elements (DDMMYY and
///               ZZZ elements) must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The century indicator must be +, -, U, V, W, X, Y, A, B, C, D, E or F.
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth, after deriving the century from the century
///               indicator must be a valid date between January 1, 1800 and
///               December 31, 2099.
///            </description>
///         </item>
///         <item>
///            <description>
///               The individual number must not be 001.
///            </description>
///         </item>
///         <item>
///            <description>
///               The check character must be a valid modulus 31 check character
///               calculated from the date of birth and the individual number.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/National_identification_number#Finland
///      for more info. Also see https://kenda.fi/tools/hetu/ for tools to generate
///      test henkilötunnus values.
///   </para>
/// </remarks>
public record FiHenkilotunnus
{
   /// <summary>
   ///   The latest year of birth supported by <see cref="FiHenkilotunnus"/>.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2099;

   /// <summary>
   ///   The earliest year of birth supported by <see cref="FiHenkilotunnus"/>.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1800;

   private const Int32 ValidLength = 11;
   private const Int32 CenturyIndicatorOffset = 6;
   private const Int32 IndividualNumberStartOffset = 7;
   private const Int32 IndividualNumberEndOffset = 10;      // Range has exclusive end offset
   private const Int32 GenderOffset = 9;
   private const Int32 CheckCharacterOffset = 10;

   private const String CheckCharacters = "0123456789ABCDEFHJKLMNPRSTUVWXY";

   /// <summary>
   ///   Check the <paramref name="henkilotunnus"/> to determine if it contains a
   ///   valid Finnish henkilotunnus.
   /// </summary>
   /// <param name="henkilotunnus">
   ///   String representation of a Finnish henkilotunnus.
   /// </param>
   /// <returns>
   ///   A <see cref="FiHenkilotunnusValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="henkilotunnus"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static FiHenkilotunnusValidationResult Validate(String? henkilotunnus)
   {
      if (String.IsNullOrWhiteSpace(henkilotunnus))
      {
         return FiHenkilotunnusValidationResult.Empty;
      }
      else if (henkilotunnus.Length != ValidLength)
      {
         return FiHenkilotunnusValidationResult.InvalidLength;
      }

      // After performing basic checks, validate the check digit because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      FiHenkilotunnusValidationResult validationResult = ValidateCheckDigit(henkilotunnus);
      if (validationResult != FiHenkilotunnusValidationResult.ValidationPassed)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }
      else if (!ValidateCenturyIndicator(henkilotunnus))
      {
         return FiHenkilotunnusValidationResult.InvalidCenturyIndicator;
      }
      else if (!ValidateIndividualNumber(henkilotunnus))
      {
         return FiHenkilotunnusValidationResult.InvalidIndividualNumber;
      }

      return FiHenkilotunnusValidationResult.ValidationPassed;
   }

   private static FiHenkilotunnusValidationResult ValidateCheckDigit(ReadOnlySpan<Char> henkilotunnus)
   {
      const Int32 processLength = 10;     // Exclude check digit from main process loop.

      var sum = 0;

      // Convert date of birth and individual number to an integer value.
      for (var index = 0; index < processLength; index++)
      {
         if (index == CenturyIndicatorOffset)
         {
            continue;
         }

         sum *= 10;
         var num = henkilotunnus[index] - Chars.DigitZero;
         if (num < 0 || num > 9)
         {
            return FiHenkilotunnusValidationResult.InvalidCharacter;
         }

         sum += num;
      }

      var checkDigit = sum % 31;
      var checkCharacter = CheckCharacters[checkDigit];

      return henkilotunnus[CheckCharacterOffset] == checkCharacter
         ? FiHenkilotunnusValidationResult.ValidationPassed
         : FiHenkilotunnusValidationResult.InvalidCheckDigit;
   }

   private static Boolean ValidateCenturyIndicator(ReadOnlySpan<Char> henkilotunnus)
      => henkilotunnus[CenturyIndicatorOffset] switch
      {
         Chars.Plus => true,
         Chars.Dash => true,
         Chars.UpperCaseA => true,
         Chars.UpperCaseB => true,
         Chars.UpperCaseC => true,
         Chars.UpperCaseD => true,
         Chars.UpperCaseE => true,
         Chars.UpperCaseF => true,
         Chars.UpperCaseU => true,
         Chars.UpperCaseV => true,
         Chars.UpperCaseW => true,
         Chars.UpperCaseX => true,
         Chars.UpperCaseY => true,
         _ => false
      };

   private static Boolean ValidateIndividualNumber(ReadOnlySpan<Char> henkilotunnus)
   {
      var d1 = henkilotunnus[IndividualNumberStartOffset] - Chars.DigitZero;
      var d2 = henkilotunnus[IndividualNumberStartOffset + 1] - Chars.DigitZero;
      var d3 = henkilotunnus[IndividualNumberStartOffset + 2] - Chars.DigitZero;

      var individualNumber = (d1 * 100) + (d2 * 10) + d3;

      return individualNumber != 1;
   }
}
