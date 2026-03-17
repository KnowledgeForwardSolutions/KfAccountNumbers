// Ignore Spelling: Foedselsnummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a Norwegian national
///   identity number. Like a number of other countries, Norway has two
///   different identity numbers with identical format, the fødselsnummer
///   (birth number), which is issued to citizens and long-term residents
///   of Norway and the D-nummer, which is issued to foreign individuals
///   who are not eligible for a fødselsnummer.
/// </summary>
/// <remarks>
///   <para>
///      Fødselsnummer and D-nummer are both 11 digit numbers formatted as
///      DDMMYYIIICC, with the following elements:
///      <list type="bullet">
///         <item>
///            <description>
///               DDMMYY - the person's date of birth in DDMMYY format. The
///               only difference between a fødselsnummer and a D-nummer is that
///               4 is added to the first digit of the person's date of birth
///               (i.e. 130585 becomes 530485).
///            </description>
///         </item>
///         <item>
///            <description>
///               III - three digit individual number. The first digit indicates
///               the person's the century of birth and the last digit indicates
///               the person's gender, with odd digits assigned to males and even
///               digits assigned to females.
///            </description>
///         </item>
///         <item>
///            <description>
///               CC - two separate check digits calculated using a weighted
///               modulus 11 algorithm. The first check digit is calculated
///               for the first nine digits (date of birth and identity digits)
///               and the second check digit is calculated for the preceding
///               ten digits.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The 11 character value is sometimes formatted for greater readability
///      by inserting a separator character, generally a space, between the date
///      of birth and the identity digits, i.e. DDMMYY IIICC.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>010289158CC</term>
///            <description>
///               fødselsnummer, date of birth February 1, 1989, gender = female,
///               check digits CC
///            </description>
///         </item>
///         <item>
///            <term>010289 158CC</term>
///            <description>
///               fødselsnummer, date of birth February 1, 1989, gender = female,
///               check digits CC
///            </description>
///         </item>
///         <item>
///            <term>521050035CC</term>
///            <description>
///               D-nummer, date of birth October 12, 1950, gender = male, check
///               digits CC
///            </description>
///         </item>
///         <item>
///            <term>521050-035CC</term>
///            <description>
///               D-nummer, date of birth October 12, 1950, gender = male, check
///               digits CC
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      When creating a new <see cref="NoFoedselsnummer"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The string must be either 11 or 12 characters long.
///            </description>
///         </item>
///         <item>
///            <description>
///               The first six characters must represent a valid date in DDYYMM
///               format (with century specified by the first individual number
///               digit). Note that the validation specifically does <b>NOT</b>
///               check for future dates, only that the date exists. See the
///               linked Wikipedia article for the definition of the century
///               indicator.
///            </description>
///         </item>
///         <item>
///            <description>
///               The separator character (if used) must not be a digit character.
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth (or the separator character, if used) must
///               be followed by a three digit individual number. All three
///               characters must be ASCII digits (0-9).
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing two characters (right-most) must be valid
///               weighted modulus 11 check digit characters.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/National_identity_number_(Norway) for more info.
///   </para>
/// </remarks>
public class NoFoedselsnummer
{
   private const Int32 NoSeparatorLength = 11;
   private const Int32 SeparatorLength = 12;

   private const Int32 SeparatorOffset = 6;

   private static readonly Int32[] _c1Weights = [3, 7, 6, 1, 8, 9, 4, 5, 2, 1, 0];
   private static readonly Int32[] _c2Weights = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2, 1];

   /// <summary>
   ///   Check the <paramref name="foedselsnummer"/> to determine if it contains a
   ///   valid Norwegien national identity number (fødselsnummer) value.
   /// </summary>
   /// <param name="foedselsnummer">
   ///   String representation of a Norwegien national identity number (fødselsnummer).
   /// </param>
   /// <returns>
   ///   A <see cref="NoFoedselsnummerValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="foedselsnummer"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static NoFoedselsnummerValidationResult Validate(String? foedselsnummer)
   {
      if (String.IsNullOrWhiteSpace(foedselsnummer))
      {
         return NoFoedselsnummerValidationResult.Empty;
      }
      else if (foedselsnummer.Length is not NoSeparatorLength and not SeparatorLength)
      {
         return NoFoedselsnummerValidationResult.InvalidLength;
      }
      NoFoedselsnummerValidationResult validationResult = ValidateCheckDigits(foedselsnummer);
      if (validationResult != NoFoedselsnummerValidationResult.ValidationPassed)
      {
         // Could be either InvalidCharacter or InvalidCheckDigits.
         return validationResult;
      }

      throw new NotImplementedException();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> foedselsnummer)
      => foedselsnummer.Length == SeparatorLength;

   private static NoFoedselsnummerValidationResult ValidateCheckDigits(ReadOnlySpan<Char> foedselsnummer)
   {
      var isFormatted = IsFormatted(foedselsnummer);
      var c1Sum = 0;
      var c2Sum = 0;
      var weightIndex = 0;
      var processLength = foedselsnummer.Length;
      for(var charIndex = 0; charIndex < processLength; charIndex ++)
      {
         if (isFormatted && charIndex == SeparatorOffset)
         {
            continue;
         }

         var num = foedselsnummer[charIndex] - Chars.DigitZero;
         if (num < 0 || num > 9)
         {
            return NoFoedselsnummerValidationResult.InvalidCharacter;
         }

         c1Sum += num * _c1Weights[weightIndex];
         c2Sum += num * _c2Weights[weightIndex];
         weightIndex ++;
      }

      return (c1Sum % 11) == 0 && (c2Sum % 11) == 0
         ? NoFoedselsnummerValidationResult.ValidationPassed
         : NoFoedselsnummerValidationResult.InvalidCheckDigits;
   }
}
