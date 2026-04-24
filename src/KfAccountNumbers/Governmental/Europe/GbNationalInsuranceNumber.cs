namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a National Insurance Number
///   (or NINO). While not defined as such, it effectively serves as national
///   identifier in the UK.
/// </summary>
/// <remarks>
///   <para>
///      A National Insurance Number consists of nine characters structured as
///      PPDDDDDDS, where:
///      <list type="bullet">
///         <item>
///            <term>PP</term>
///            <description>
///               is a two-letter prefix. See below for valid prefix characters.
///            </description>
///         </item>
///         <item>
///            <term>DDDDDD</term>
///            <description>
///               is a six-digit sequentially assigned number.
///            </description>
///         </item>
///         <item>
///            <term>S</term>
///            <description>
///               is a single suffix letter, either A, B, C, or D. The suffix
///               can be omitted if is unknown as the suffix does not contribute
///               to the uniqueness of the value.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A National Insurance Number is typically displayed as a single string
///      of nine characters but can be formatted for readability as groups of two
///      characters with a separator character, typically a space (i.e. PP DD DD DD S).
///      <see cref="GbNationalInsuranceNumber"/> is case-sensitive and requires
///      the prefix and suffix characters to be uppercase letters.
///   </para>
///   <para>
///      When creating a new <see cref="GbNationalInsuranceNumber"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must have length 8, 9, 11 or 13 characters. (8 characters =
///               unformatted, without suffix character, 9 characters = unformatted,
///               with suffix character, 11 characters = formatted, without suffix character,
///               13 characters = formatted, with suffix character)
///            </description>
///         </item>
///         <item>
///            <description>
///               The leading (left-most) two characters may not be BG, GB, NK, KN, TN, NT, or ZZ.
///            </description>
///         </item>
///         <item>
///            <description>
///               Character position 0 (zero-based) must be an uppercase letter, A-C, E, G, H, J-P, R-T, W-Z.
///               The letters D, F, I, Q, U and V are not allowed.
///            </description>
///         </item>
///         <item>
///            <description>
///               Character position 1 (zero-based) must be an uppercase letter, A-C, E, G, H, J-N, P, R-T, W-Z.
///               The letters D, F, I, O, Q, U and V are not allowed. (Note O is the only additional excluded character.)
///            </description>
///         </item>
///         <item>
///            <description>
///               Character positions 2-7 (zero-based) must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               Character position 8 (zero-based), if present, must be an uppercase letter, A-D.
///            </description>
///         </item>
///         <item>
///            <description>
///               Separator characters, if present, may not be ASCII digits ('0'-'9') or uppercase or lowercase letters (A-Z, a-z).
///            </description>
///         </item>
///         <item>
///            <description>
///               The same character must be used in every separator position.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Note that National Insurance Numbers do not include a check digit.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>AB123456C</term>
///            <description>unformatted, with suffix character</description>
///         </item>
///         <item>
///            <term>GG000123</term>
///            <description>unformatted, without suffix character</description>
///         </item>
///         <item>
///            <term>AB 12 34 56 C</term>
///            <description>formatted, with suffix character</description>
///         </item>
///         <item>
///            <term>GG 00 01 23</term>
///            <description>formatted, without suffix character</description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/National_Insurance_number for more info.
///   </para>
/// </remarks>
public record GbNationalInsuranceNumber
{
   private const Int32 UnformattedWithoutSuffixLength = 8;
   private const Int32 UnformattedWithSuffixLength = 9;
   private const Int32 FormattedWithoutSuffixLength = 11;
   private const Int32 FormattedWithSuffixLength = 13;

   private const Int32 Separator1Offset = 2;
   private const Int32 Separator2Offset = 5;
   private const Int32 Separator3Offset = 8;
   private const Int32 Separator4Offset = 11;

   private static HashSet<String>.AlternateLookup<ReadOnlySpan<Char>> _invalidPrefixes =
      new HashSet<String>() { "BG", "GB", "NK", "KN", "TN", "NT", "ZZ" }.GetAlternateLookup<ReadOnlySpan<Char>>();
   private static HashSet<Char> _validPrefixFirstCharacters = "ABCEGHJKLMNOPRSTWXYZ".ToHashSet();
   private static HashSet<Char> _validPrefixSecondCharacters = "ABCEGHJKLMNPRSTWXYZ".ToHashSet();

   /// <summary>
   ///   Check the <paramref name="nationaInsuranceNumber"/> to determine if it contains a
   ///   valid UK National Insurance Number.
   /// </summary>
   /// <param name="nationaInsuranceNumber">
   ///   String representation of a UK National Insurance Number.
   /// </param>
   /// <returns>
   ///   A <see cref="GbNationalInsuranceNumberValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="nationaInsuranceNumber"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static GbNationalInsuranceNumberValidationResult Validate(String? nationaInsuranceNumber)
   {
      if (String.IsNullOrWhiteSpace(nationaInsuranceNumber))
      {
         return GbNationalInsuranceNumberValidationResult.Empty;
      }
      else if (!ValidateLength(nationaInsuranceNumber))
      {
         return GbNationalInsuranceNumberValidationResult.InvalidLength;
      }
      else if (!ValidatePrefix(nationaInsuranceNumber))
      {
         return GbNationalInsuranceNumberValidationResult.InvalidPrefix;
      }
      else if (!ValidatePrefixFirstCharacter(nationaInsuranceNumber)
         || !ValidatePrefixSecondCharacter(nationaInsuranceNumber)
         || !ValidateDigits(nationaInsuranceNumber)
         || !ValidateSuffixCharacter(nationaInsuranceNumber))
      {
         return GbNationalInsuranceNumberValidationResult.InvalidCharacter;
      }

      return GbNationalInsuranceNumberValidationResult.ValidationPassed;
   }

   private static Boolean ValidateDigits(ReadOnlySpan<Char> nationalInsuranceNumber)
   {
      var isFormatted = nationalInsuranceNumber.Length > UnformattedWithSuffixLength;
      var start = isFormatted ? 3 : 2;
      var end = nationalInsuranceNumber.Length switch
      {
         UnformattedWithSuffixLength => nationalInsuranceNumber.Length - 1,
         FormattedWithSuffixLength => nationalInsuranceNumber.Length - 2,
         _ => nationalInsuranceNumber.Length
      };

      for (var index = start; index < end; index++)
      {
         if (isFormatted
            && (index is Separator1Offset or Separator2Offset or Separator3Offset or Separator4Offset))
         {
            continue;
         }

         if (!nationalInsuranceNumber[index].IsAsciiDigit())
         {
            return false;
         }
      }

      return true;
   }

   private static Boolean ValidateLength(ReadOnlySpan<Char> nationalInsuranceNumber)
      => nationalInsuranceNumber.Length is UnformattedWithoutSuffixLength
         or UnformattedWithSuffixLength
         or FormattedWithoutSuffixLength
         or FormattedWithSuffixLength;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidatePrefix(ReadOnlySpan<Char> nationalInsuranceNumber)
      => !_invalidPrefixes.Contains(nationalInsuranceNumber[..2]);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidatePrefixFirstCharacter(ReadOnlySpan<Char> nationalInsuranceNumber)
      => _validPrefixFirstCharacters.Contains(nationalInsuranceNumber[0]);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidatePrefixSecondCharacter(ReadOnlySpan<Char> nationalInsuranceNumber)
      => _validPrefixSecondCharacters.Contains(nationalInsuranceNumber[1]);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidateSuffixCharacter(ReadOnlySpan<Char> nationalInsuranceNumber)
      => (nationalInsuranceNumber.Length is UnformattedWithoutSuffixLength or FormattedWithoutSuffixLength)
         || nationalInsuranceNumber[^1] is >= Chars.UpperCaseA and <= Chars.UpperCaseD;

}
