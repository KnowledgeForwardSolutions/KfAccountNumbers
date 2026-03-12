// Ignore Spelling: Personnummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object for a Swedish Personal Identity Number
///   (personnummer). Also supports Swedish coordination numbers
///   (samordningsnummer) for persons who are not eligible for a personnummer
///   (non-residents, foreign employees, etc.).
/// </summary>
/// <remarks>
///   <para>
///      A valid Swedish Personal Identity Number (personnummer) is a string
///      that is 11 or 13 characters long. The first 6 or 8 characters represent
///      the date of birth in the format YYMMDD or YYYYMMDD, followed by a
///      hyphen or plus sign, and then a three-digit birth serial number and a
///      single digit that is a checksum calculated using the Luhn algorithm.
///      An odd birth serial number indicates a male, while an even birth serial
///      number indicates a female. The hyphen indicates that the person is under
///      100 years old, while the plus sign indicates that the person is 100
///      years old or older.
///   </para>
///   <para>
///      A valid Swedish coordination umber (samordningsnummer) uses the same
///      format as a personnummer, but in the date of birth section, 60 is added
///      to the day. For example a date of birth Jan, 23, 1995 would be "950123"
///      for a YYMMDD format personnummer, but would be "951283" for a YYMMDD
///      format samordningsnummer.
///   </para>
///   <para>
///      Not all combinations of digits are valid, as the date of birth must be
///      a valid date and the checksum must be correct according to the Luhn
///      algorithm. When creating a new <see cref="SePersonnummer"/>, the
///      following validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The string must be either 11 or 13 characters long.
///            </description>
///         </item>
///         <item>
///            <description>
///               For 11-character strings, the first 6 characters must represent
///               a valid date in the format YYMMDD. For 13-character strings,
///               the first 8 characters must represent a valid date in the format
///               YYYYMMDD.
///            </description>
///         </item>
///         <item>
///            <description>
///               A valid separator must be present in position 6 (zero based,
///               for YYMMDD format) or position 8 (zero based, for YYYYMMDD
///               format). The separator must be either a hyphen (-) or a plus
///               sign (+).
///            </description>
///         </item>
///         <item>
///            <description>
///               Character positions 7 to 9 (zero based, YYMMDD format) or
///               positions 9 to 11 (zero based, YYYYMMDD format) must be
///               digits representing the birth serial number.
///            </description>
///         </item>
///         <item>
///            <description>
///               Character position 10 (zero based, YYMMDD format) or position
///               12 (zero based, YYYYMMDD format) must be a valid checksum
///               calculated using the Luhn algorithm based on the six digit
///               date of birth and the three-digit birth serial number.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      When determining if a date of birth is valid, YYMMDD format dates are
///      assumed to be in the 20th century (1900-1999). The reason for this
///      assumption is that the YYYYMMDD format was introduced in 1997,
///      presumably as part of Y2K preparations. The practical impact of this
///      assumption is that the YYMMDD format date "000229" will always be
///      considered invalid because 1900 is not a leap year. (The opposite
///      would be true if "00" represented the year 2000, which is a leap
///      year because of the century divisible by 400 rule for leap years).
///   </para>
/// </remarks>
public record SePersonnummer
{
   private const Int32 ShortFormatLength = 11;
   private const Int32 LongFormatLength = 13;

   private const Int32 SeparatorOffsetSixDigitDateOfBirthLength = 6;
   private const Int32 SeparatorOffsetEightDigitDateOfBirthLength = 8;

   /// <summary>
   ///   Check the <paramref name="personnummer"/> to determine if it contains a
   ///   valid Swedish Personal Identity Number (Personnummer) value.
   /// </summary>
   /// <param name="personnummer">
   ///   String representation of a Swedish Personal Identity Number (Personnummer).
   /// </param>
   /// <returns>
   ///   A <see cref="SePersonnummerValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="personnummer"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   /// <remarks>
   ///   Validation is case-insensitive.
   /// </remarks>
   public static SePersonnummerValidationResult Validate(String? personnummer)
   {
      if (String.IsNullOrWhiteSpace(personnummer))
      {
         return SePersonnummerValidationResult.Empty;
      }
      else if (personnummer.Length != ShortFormatLength
            && personnummer.Length != LongFormatLength)
      {
         return SePersonnummerValidationResult.InvalidLength;
      }
      else if (GetSeparator(personnummer) != Chars.Dash
            && GetSeparator(personnummer) != Chars.Plus)
      {
         return SePersonnummerValidationResult.InvalidSeparator;
      }
      else if (!ValidateDateOfBirth(personnummer))
      {
         return SePersonnummerValidationResult.InvalidDateOfBirth;
      }
      else if (!ValidateBirthSerialNumber(personnummer))
      {
         return SePersonnummerValidationResult.InvalidBirthSerialNumber;
      }
      else if (!ValidateCheckDigit(personnummer))
      {
         return SePersonnummerValidationResult.InvalidCheckDigit;
      }

      return SePersonnummerValidationResult.ValidationPassed;
   }

   private static ReadOnlySpan<Char> GetBirthSerialNumber(ReadOnlySpan<Char> personnummer)
      => personnummer.Length == ShortFormatLength
         ? personnummer[7.. 10]
         : personnummer[9.. 12];

   private static ReadOnlySpan<Char> GetDateOfBirth(ReadOnlySpan<Char> personnummer)
      => personnummer.Length == ShortFormatLength
         ? personnummer[.. 6]
         : personnummer[.. 8];

   private static Char GetSeparator(ReadOnlySpan<Char> personnummer)
      => personnummer.Length == ShortFormatLength
         ? personnummer[SeparatorOffsetSixDigitDateOfBirthLength]
         : personnummer[SeparatorOffsetEightDigitDateOfBirthLength];

   private static (Int32 year, Int32 month, Int32 day) GetYearMonthDay(ReadOnlySpan<Char> personnummer)
   {
      Int32 year;
      Int32 month;
      Int32 day;
      if (personnummer.Length == ShortFormatLength)
      {
         // Assume that short format personnummer values are for people born in
         // the 20th century, as long format personnummer was introduced in 1997,
         // presumably as part of Y2K preparations.
         year = 1900 + ParseTwoDigits(personnummer[0], personnummer[1]);
         month = ParseTwoDigits(personnummer[2], personnummer[3]);
         day = ParseTwoDigits(personnummer[4], personnummer[5]);
      }
      else
      {
         var century = ParseTwoDigits(personnummer[0], personnummer[1]) * 100;
         year = century + ParseTwoDigits(personnummer[2], personnummer[3]);
         month = ParseTwoDigits(personnummer[4], personnummer[5]);
         day = ParseTwoDigits(personnummer[6], personnummer[7]);
      }

      return (year, month, day);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Int32 ParseTwoDigits(Char tens, Char ones)
      => ((tens - Chars.DigitZero) * 10) + (ones - Chars.DigitZero);

   private static Boolean ValidateBirthSerialNumber(ReadOnlySpan<Char> personnummer)
   {
      ReadOnlySpan<Char> birthSerialNumberSpan = GetBirthSerialNumber(personnummer);
      foreach (var ch in birthSerialNumberSpan)
      {
         if (!Char.IsAsciiDigit(ch))
         {
            return false;
         }
      }

      return true;
   }

   private static Boolean ValidateCheckDigit(String personnummer)
   {
      ICheckDigitMask checkDigitMask = personnummer.Length == ShortFormatLength
         ? SePersonNumberShortFormatCheckDigitMasks.Instance
         : SePersonNumberLongFormatCheckDigitMasks.Instance;
      return CheckDigitAlgorithms.Luhn.Validate(personnummer, checkDigitMask);
   }

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> personnummer)
   {
      ReadOnlySpan<Char> dateOfBirthSpan = GetDateOfBirth(personnummer);
      foreach (var ch in dateOfBirthSpan)
      {
         if (!Char.IsAsciiDigit(ch))
         {
            return false;
         }
      }

      // Manual validation is faster than using DateTime.TryParseExact.
      var (year, month, day) = GetYearMonthDay(personnummer);

      if (year < 1900 || year > 2099)
      {
         return false;
      }
      else if (month < 1 || month > 12)
      {
         return false;
      }

      // Handle samordningsnummer, which adds 60 to the date of birth.
      if (day > 60)
      {
         day -= 60;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }
}
