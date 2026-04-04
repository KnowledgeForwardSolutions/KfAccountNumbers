// Ignore Spelling: Rijksregisternummer Nummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a Belgian rijksregisternummer
///   or a Belgian BIS-nummer (for non-residents).
/// </summary>
/// <remarks>
///   <para>
///      Rijksregisternummer and BIS-nummer both are 11-digit numbers, structured
///      as YYMMDDXXXCC, with the following elements.
///      <list type="bullet">
///         <item>
///            <term>YYMMDD</term>
///            <description>
///               The person's date of birth in YYMMDD format. A BIS number is
///               differentiated from a rijksregisternummer by the addition of a
///               constant value (40 or 20, see below) to the month component of
///               the date of birth.
///            </description>
///         </item>
///         <item>
///            <term>XXX</term>
///            <description>
///               Three digit sequence number used to differentiate between
///               persons born on the same date. The sequence number also
///               indicates gender with odd numbers for males and even numbers
///               for females.
///            </description>
///         </item>
///         <item>
///            <term>CC</term>
///            <description>
///               Two digit modulus 97 check sum calculated for the YYMMDD and
///               XXX elements. The check sum is also used to indicate century
///               of birth. If CC is equal to the normal modulus 97 check sum
///               then the persons' century of birth is 1900-1999. If CC is
///               equal to the modulus 97 check sum calculated by first prefixing
///               YYMMDDXXX with the digit 2 (i.e. 2YYMMDDXXX) then the person's
///               century of birth is 2000-2099.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A Belgian rijksregisternummer may be formatted as a string of 11 consecutive
///      digits (YYMMCCXXXCC) or as a 15 character string with characters separating
///      the individual elements. YY.MM.DD-XXX.CC is the typical display format.
///   </para>
///   <para>
///      When creating a new <see cref="BeRijksregisternummer"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 11 characters (without separators) or 15
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
///               The separator characters, if included, must not be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The two trailing (right-most) characters must be a valid modulus 97
///               check sum (taking into account the possibility of a person born in
///               the year 2000 or later).
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth, after deriving the century of birth from the
///               check sum and taking into account the BIS number offset, must
///               be a valid date between January 1, 1900 and December 31, 2099.
///               <b>OR</b> the date of birth may use zeros to indicate that some
///               or all of the person's date of birth is unknown (see below for
///               more details).
///            </description>
///         </item>
///         <item>
///            <description>
///               The sequence number may not be 000 or 999.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The date of birth can be adjusted in a variety of ways:
///      <list type="bullet">
///         <item>
///            <description>
///               If the person's date of birth is incomplete, then the two digit
///               year  is used and zeros are used for month and year (for example,
///               40.00.00-955.69).
///            </description>
///         </item>
///         <item>
///            <description>
///               If there are too many people with incomplete dates of birth for
///               a particular year than can be represented by a three digit sequence
///               number (i.e. more than 499 males with incomplete dates of birth for
///               the year 1940), then 01 is used for the day of birth and the sequence
///               number rolls over to 001 (ex. 40.00.01-001.33)
///            </description>
///         </item>
///         <item>
///            <description>
///               If the person's date of birth is unknown, then the constant 00.00.01 is used.
///            </description>
///         </item>
///         <item>
///            <description>
///               As noted above, if the value is a BIS number then 40 is added to the month
///               component of the date of birth.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the value is a BIS number <b>AND</b> the person's gender is unknown
///               at the time the number is issued then <b>20</b> is added to the month
///               component of the date of birth.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      For cases of a BIS number for a person with an incomplete or unknown date
///      of birth, <see cref="BeRijksregisternummer"/> stacks the appropriate rules.
///      For example, 87.40.00-023.47 would be the BIS number for a person with an
///      incomplete date of birth born in 1987.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>85.07.30-033.28</term>
///            <description>
///               Rijksregisternummer, date of birth July 30, 1985, gender = male,
///               check digit calculation 97 - (850730033 mod 97) = 97 - 69 = 28
///            </description>
///         </item>
///         <item>
///            <term>17110804680</term>
///            <description>
///               Rijksregisternummer, date of birth November 11, 2017, gender = female,
///               check digit calculation 97 - (2171108046 mod 97) = 97 - 17 = 80
///            </description>
///         </item>
///         <item>
///            <term>40 00 00 955-79</term>
///            <description>
///               Rijksregisternummer, date of birth incomplete, year of birth = 1940,
///               gender = male, check digit calculation 97 - (400000955 mod 97) = 97 - 18 = 79
///            </description>
///         </item>
///         <item>
///            <term>00 00 01 003-64</term>
///            <description>
///               Rijksregisternummer, date of birth unknown, gender = male, check
///               digit calculation 97 - (000001003 mod 97) = 97 - 33 = 64
///            </description>
///         </item>
///         <item>
///            <term>17.51.08-046.40</term>
///            <description>
///               BIS number, date of birth November 11, 1917, gender = female, check
///               digit calculation 97 - (175108046 mod 97) = 97 - 57 = 40
///            </description>
///         </item>
///         <item>
///            <term>09 20 00 002 65</term>
///            <description>
///              BIS number, date of birth incomplete, year of birth 2009, gender unknown,
///              check digit calculation 97 - (2092000002 mod 97) = 97 - 32 = 65
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://fr.wikipedia.org/wiki/Num%C3%A9ro_de_registre_national (French) for more info.
///   </para>
/// </remarks>
public record BeRijksregisternummer
{
   /// <summary>
   ///   Represents the month offset used to distinguish BIS-nummers from
   ///   rijksregisternummers when the person's gender is known.
   /// </summary>
   /// <remarks>
   ///   In Belgian identity numbers, a BIS-nummer is indicated by
   ///   adding a constant to the month component of the date of birth.
   /// </remarks>
   public const Int32 BisNummerMonthOffset = 40;

   /// <summary>
   ///   Represents the month offset used to distinguish BIS-nummers from
   ///   rijksregisternummers when the person's gender is unknown.
   /// </summary>
   /// <remarks>
   ///   In Belgian identity numbers, a BIS-nummer is indicated by
   ///   adding a constant to the month component of the date of birth.
   /// </remarks>
   public const Int32 BisNummerUnknownGenderMonthOffset = 20;

   /// <summary>
   ///   The latest year of birth supported by <see cref="BeRijksregisternummer"/>.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2099;

   /// <summary>
   ///   The earliest year of birth supported by <see cref="BeRijksregisternummer"/>.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1900;

   private const Int32 UnformattedLength = 11;
   private const Int32 FormattedLength = 15;

   private const Int32 Separator1Offset = 2;
   private const Int32 Separator2Offset = 5;
   private const Int32 Separator3Offset = 8;
   private const Int32 Separator4Offset = 12;

   /// <summary>
   ///   The raw burgerservicenummer value.
   /// </summary>
   public String Value { get; private init; }

   // These items are measured from the end of the value.
   private const Int32 CheckDigit1Offset = 2;
   private const Int32 CheckDigit2Offset = 1;

   /// <summary>
   ///   Check the <paramref name="rijksregisternummer"/> to determine if it contains a
   ///   valid Belgian rijksregisternummer.
   /// </summary>
   /// <param name="rijksregisternummer">
   ///   String representation of a Belgian rijksregisternummer.
   /// </param>
   /// <returns>
   ///   A <see cref="BeRijksregisternummerValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="rijksregisternummer"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static BeRijksregisternummerValidationResult Validate(String? rijksregisternummer)
   {
      if (String.IsNullOrWhiteSpace(rijksregisternummer))
      {
         return BeRijksregisternummerValidationResult.Empty;
      }
      else if (rijksregisternummer.Length is not UnformattedLength and not FormattedLength)
      {
         return BeRijksregisternummerValidationResult.InvalidLength;
      }

      // After performing basic checks, validate the check digits because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      BeRijksregisternummerValidationResult validationResult = ValidateCheckDigits(rijksregisternummer);
      if (validationResult != BeRijksregisternummerValidationResult.ValidationPassed)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }
      else if (!ValidateSeparators(rijksregisternummer))
      {
         return BeRijksregisternummerValidationResult.InvalidSeparator;
      }
      else if (!ValidateDateOfBirth(rijksregisternummer))
      {
         return BeRijksregisternummerValidationResult.InvalidDateOfBirth;
      }

      return BeRijksregisternummerValidationResult.ValidationPassed;
   }

   private static (Int32 year, Int32 month, Int32 day) GetYearMonthDay(ReadOnlySpan<Char> rijksregisternummer)
   {
      var fieldWidth = rijksregisternummer.Length == UnformattedLength ? 2 : 3;
      var year = rijksregisternummer.ParseTwoDigits();

      var fieldStart = fieldWidth;
      var month = rijksregisternummer[fieldStart..].ParseTwoDigits();

      fieldStart += fieldWidth;
      var day = rijksregisternummer[fieldStart..].ParseTwoDigits();

      fieldStart += fieldWidth;
      var sequenceNumber = rijksregisternummer[fieldStart..].ParseThreeDigits();

      // Already parsed the individual elements, combine to use in checksum calculation.
      var total = sequenceNumber + (day * 1000) + (month * 10000) + (year * 10000000);
      var checksum = rijksregisternummer[^2..].ParseTwoDigits();
      var century = (97 - (total % 97)) == checksum
         ? 1900
         : 2000;

      // Apply BIS-nummer offsets if necessary.
      month = month switch
      {
         >= BisNummerMonthOffset => month - BisNummerMonthOffset,
         >= BisNummerUnknownGenderMonthOffset => month - BisNummerUnknownGenderMonthOffset,
         _ => month
      };

      return (century + year, month, day);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> rijksregisternummer)
      => rijksregisternummer.Length == FormattedLength;

   private static BeRijksregisternummerValidationResult ValidateCheckDigits(ReadOnlySpan<Char> rijksregisternummer)
   {
      var processLength = rijksregisternummer.Length - 2;      // Exclude check digits from main loop
      var isFormatted = IsFormatted(rijksregisternummer);

      var sum = 0;
      for (var index = 0; index < processLength; index++)
      {
         if (isFormatted &&
            (index is Separator1Offset or Separator2Offset or Separator3Offset or Separator4Offset))
         {
            continue;
         }

         sum *= 10;
         var num = rijksregisternummer[index] - Chars.DigitZero;
         if (num < 0 || num > 9)
         {
            return BeRijksregisternummerValidationResult.InvalidCharacter;
         }

         sum += num;
      }

      var c1 = rijksregisternummer[^CheckDigit1Offset] - Chars.DigitZero;
      var c2 = rijksregisternummer[^CheckDigit2Offset] - Chars.DigitZero;
      if (c1 < 0 || c1 > 9 || c2 < 0 || c2 > 9)
      {
         return BeRijksregisternummerValidationResult.InvalidCharacter;
      }
      var checkSum = (c1 * 10) + c2;

      // Check for persons born 1900-1999.
      var remainder = 97 - (sum % 97);
      if (remainder == checkSum)
      {
         return BeRijksregisternummerValidationResult.ValidationPassed;
      }

      // Then for persons born 2000-2099;
      var longRemainder = 97 - ((2000000000L + sum) % 97);           // Long int to handle possible int overflow
      return longRemainder == checkSum
         ? BeRijksregisternummerValidationResult.ValidationPassed
         : BeRijksregisternummerValidationResult.InvalidCheckDigits;
   }

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> rijksregisternummer)
   {
#pragma warning disable IDE0008 // Use explicit type
      var (year, month, day) = GetYearMonthDay(rijksregisternummer);
#pragma warning restore IDE0008 // Use explicit type

      if (year < MinimumValidYearOfBirth || year > MaximumValidYearOfBirth)
      {
         // Should be impossible to ever reach this point because of the check
         // digit calcuations, but return false out of abundance of caution and
         // to avoid throwing an exception.
         return false;
      }

      // Allow zero for incomplete dates of birth.
      if (month == 0 || day == 0)
      {
         return true;
      }

      if (month < 1 || month > 12)
      {
         return false;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }

   private static Boolean ValidateSeparators(ReadOnlySpan<Char> rijksregisternummer)
      => rijksregisternummer.Length == UnformattedLength
         || (  !rijksregisternummer[Separator1Offset].IsAsciiDigit()
               && !rijksregisternummer[Separator2Offset].IsAsciiDigit()
               && !rijksregisternummer[Separator3Offset].IsAsciiDigit()
               && !rijksregisternummer[Separator4Offset].IsAsciiDigit()
            );
         
}
