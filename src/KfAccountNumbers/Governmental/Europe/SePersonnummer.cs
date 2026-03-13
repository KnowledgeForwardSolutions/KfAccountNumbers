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
///               date of birth and the three-digit birth serial number. (The
///               leading two digits of an eight digit date of birth are
///               ignored.)
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

   private const Int32 SamordningsnummerDayOffset = 60;

   /// <summary>
   ///   Initialize a new instance of the <see cref="SePersonnummer"/> class.
   /// </summary>
   /// <param name="personnummer">
   ///   String representation of a personnummer.
   /// </param>
   /// <exception cref="KfValidationException{SePersonnummerValidationResult}">
   ///   <paramref name="personnummer"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="personnummer"/> is not length 11 or 13.
   ///   - or -
   ///   <paramref name="personnummer"/> contains an invalid date of birth in
   ///   positions 0-5 (zero-based, YYMMDD format) or positions 0-7 (zero-based,
   ///   YYYYMMDD format).
   ///   - or -
   ///   <paramref name="personnummer"/> contains an invalid separator character
   ///   in position 6 (zero-based, YYMMDD format) or position 8 (zero-based,
   ///   YYYYMMDD format). Valid separator characters are dash ('-') and plus
   ///   ('+').
   ///   - or -
   ///   <paramref name="personnummer"/> contains an invalid birth serial number
   ///   (i.e. one or more non-digit characters) in positions 7-9 (zero-based,
   ///   YYMMDD format) or positions 9-11 (zero-based, YYYYMMDD format).
   ///   - or -
   ///   <paramref name="personnummer"/> contains an invalid check digit in 
   ///   position 10 (zero-based, YYMMDD format) or position 12 (zero-based,
   ///   YYYYMMDD format). The check digit is calculated using the Luhn algorithm
   ///   based on the six digit date of birth and the three-digit birth serial
   ///   number. (The leading two digits of an eight digit date of birth are
   ///   ignored.)
   /// </exception>
   public SePersonnummer(String? personnummer)
      : this(personnummer, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has already
   ///   been validated.
   /// </summary>
   private SePersonnummer(String? personnummer, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         SePersonnummerValidationResult validationResult = Validate(personnummer);
         if (validationResult != SePersonnummerValidationResult.ValidationPassed)
         {
            throw validationResult.ToValidationException();
         }
      }

      Value = personnummer!;
   }

   /// <summary>
   ///   The person's date of birth, derived from the date of birth portion of
   ///   the personnummer (either the first six digits for 11 character values
   ///   or the first eight digits for 13 character values).
   /// </summary>
   /// <remarks>
   ///   <para>
   ///   An 11 character value (i.e. YYMMDD format) assumes that the year of
   ///   birth will be between 1900 and 1999.
   ///   </para>
   ///   <para>
   ///   Note that the indicated date of birth may not be the person's exact
   ///   date of birth as it is possible that a day may run out of birth
   ///   serial numbers. In that case, a date close to the actual date of
   ///   birth is used instead.
   ///   </para>
   ///   <para>
   ///   Note also that samordningsnummer values encode the date of birth by
   ///   adding 60 to the day (i.e. "950123" encodes as "950183"). If the
   ///   personnummer is actually a samordningsnummer, then 60 will be
   ///   subtracted from the day to get the actual numeric day.
   ///   </para>
   /// </remarks>
   public DateOnly DateOfBirth
   {
      get
      {
         var (year, month, day) = GetYearMonthDay(Value);

         // Handle samordningsnummer, which adds 60 to the date of birth.
         if (day > SamordningsnummerDayOffset)
         {
            day -= SamordningsnummerDayOffset;
         }

         return new DateOnly(year, month, day);
      }
   }

   /// <summary>
   ///   The person's gender, as indicated by the third character of the birth
   ///   sequence number. Odd digits = Female; even digits = Male.
   /// </summary>
   public BinaryGender Gender => Value[^2] % 2 == 0
      ? BinaryGender.Female
      : BinaryGender.Male;

   /// <summary>
   ///   The raw personnummer value.
   /// </summary>
   public String Value { get; private init; }

   public static implicit operator String(SePersonnummer personnummer)
      => personnummer?.Value ?? String.Empty;     // Handle null personnummer object gracefully by returning empty string

   // Explicit conversion from String to avoid unintentional conversions that may throw exceptions.
   public static explicit operator SePersonnummer(String personnummer) => new(personnummer);

   /// <summary>
   ///   Get a string representation of the personnummer.
   /// </summary>
   public override String ToString() => Value;

   /// <summary>
   ///   Create a new <see cref="SePersonnummer"/>.
   /// </summary>
   /// <param name="personnummer">
   ///   String representation of a Swedish Personal Identity Number (Personnummer).
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{SePersonnummer, SePersonnummerValidationResult}"/>.
   ///   Will contain the new <see cref="SePersonnummerValidationResult"/> if 
   ///   <paramref name="personnummer"/> is valid or 
   ///   <see cref="SePersonnummerValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="personnummer"/> is 
   ///   invalid.
   /// </returns>
   public static CreateResult<SePersonnummer, SePersonnummerValidationResult> Create(String? personnummer)
   {
      SePersonnummerValidationResult validationResult = Validate(personnummer);
      return validationResult == SePersonnummerValidationResult.ValidationPassed
         ? new SePersonnummer(personnummer, validationMode: ValidationMode.BypassValidation)
         : validationResult;
   }

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
         ? personnummer[7..10]
         : personnummer[9..12];

   private static ReadOnlySpan<Char> GetDateOfBirth(ReadOnlySpan<Char> personnummer)
      => personnummer.Length == ShortFormatLength
         ? personnummer[..6]
         : personnummer[..8];

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
         year = 1900 + personnummer.ParseTwoDigits();
         month = personnummer[2..].ParseTwoDigits();
         day = personnummer[4..].ParseTwoDigits();
      }
      else
      {
         var century = personnummer.ParseTwoDigits() * 100;
         year = century + personnummer[2..].ParseTwoDigits();
         month = personnummer[4..].ParseTwoDigits();
         day = personnummer[6..].ParseTwoDigits();
      }

      return (year, month, day);
   }
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
#pragma warning disable IDE0008 // Use explicit type
      var (year, month, day) = GetYearMonthDay(personnummer);
#pragma warning restore IDE0008 // Use explicit type

      if (year < 1900 || year > 2099)
      {
         return false;
      }
      else if (month < 1 || month > 12)
      {
         return false;
      }

      // Handle samordningsnummer, which adds 60 to the date of birth.
      if (day > SamordningsnummerDayOffset)
      {
         day -= SamordningsnummerDayOffset;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }
}
