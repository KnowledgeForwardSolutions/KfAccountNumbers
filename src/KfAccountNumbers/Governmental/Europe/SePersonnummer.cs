// Ignore Spelling: Json Personnummer Samordningsnummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents either of two identifiers
///   issued by the Swedish Tax Agency that have the same format and are used
///   for similar purposes. The first, the Personal Identity Number
///   (personnummer) is issued to persons born in Sweden or who are residents
///   of Sweden for 12 months or longer. The second, the coordination number
///   (samordningsnummer) is issued to persons who reside in Sweden for less
///   than a year. The <see cref="IdentifierType"/> property will indicate
///   the exact type of identifier represented by an instance of
///   <see cref="SePersonnummer"/>.
/// </summary>
/// <remarks>
///   <para>
///      Swedish personnummer and samordningsnummer values are both 11 or 13
///      character strings. The only difference between the two lengths are
///      the number of digits used to represent the date of birth, either
///      six or eight. The format of personnummer and samordningsnummer are
///      the same and consist of the following fields/sections:
///      <list type="bullet">
///         <item>
///            <description>
///               The date of birth, represented by either six or eight digits
///               (YYMMDD format or YYYYMMDD format). Originally six digits
///               were used but the eight digit format was introduced in 1997.
///            </description>
///         </item>
///         <item>
///            <description>
///               A separator character that separates the date of birth from
///               the remaining four digits. The separator character is
///               normally a dash ('-') but when a person turns 100 years old
///               the dash is replaced by a plus sign ('+').
///            </description>
///         </item>
///         <item>
///            <description>
///               A three digit birth serial number, issued serially as births
///               are recorded for a particular date. The last digit of the
///               birth serial number serves an additional purpose of indicating
///               the person's gender, with odd digits assigned to males and
///               even digits assigned to females.
///            </description>
///         </item>
///         <item>
///            <description>
///               A single check digit calculated using the Luhn algorithm
///               applied to the rightmost six digits of the date of birth and
///               to the birth serial number.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The only difference between a personnummer and a samordningsnummer is
///      that the samordningsnummer adds 60 to the day of a person's date of
///      birth (i.e. 950123 would become 950183).
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>890201-3286</term>
///            <description>
///               Personnummer, date of birth 890201, less than 100 years old,
///               gender = female, check digit = 6.
///            </description>
///         </item>
///         <item>
///            <term>19890201-3286</term>
///            <description>
///               Personnummer, date of birth 19890201, less than 100 years old,
///               gender = female, check digit = 6.
///            </description>
///         </item>
///         <item>
///            <term>811228+9874</term>
///            <description>
///               Personnummer, date of birth 811228, greater than 100 years old,
///               gender = male, check digit = 4.
///            </description>
///         </item>
///         <item>
///            <term>890261-3283</term>
///            <description>
///               Samordningsnummer, date of birth 890261 (actual date of birth
///               = 890201), less than 100 years old, gender = female, check
///               digit = 3.
///            </description>
///         </item>
///         <item>
///            <term>19890261-3283</term>
///            <description>
///               Samordningsnummer, date of birth 19890261 (actual date of birth
///               = 19890201), less than 100 years old, gender = female, check digit
///               = 3.
///            </description>
///         </item>
///         <item>
///            <term>811288+9871</term>
///            <description>
///               Samordningsnummer, date of birth 811288 (actual date of birth
///               = 811228), greater than 100 years old, gender = male, check
///               digit = 1.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      When creating a new <see cref="SePersonnummer"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The string must be either 11 or 13 characters long.
///            </description>
///         </item>
///         <item>
///            <description>
///               For 11-character strings, the first 6 characters must represent
///               a valid date in the format YYMMDD. For 13-character strings,
///               the first 8 characters must represent a valid date in the
///               format YYYYMMDD. Note that the validation specifically does
///               <b>NOT</b> check for future dates, only that the date exists.
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth must be followed by a valid separator
///               character. The separator must be either a dash (-) or a plus
///               sign (+).
///            </description>
///         </item>
///         <item>
///            <description>
///               The separator must be followed by a three digit birth serial
///               number.
///            </description>
///         </item>
///         <item>
///            <description>
///               The birth serial number must be followed by a  valid checksum
///               calculated using the Luhn algorithm based on the six digit
///               date of birth and the three-digit birth serial number. (The
///               leading two digits of an eight digit date of birth are
///               ignored.)
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Note that the encoded date of birth may not be the person's actual
///      date of birth. It is possible to run out of birth serial numbers for
///      a particular day and in this case a day close to the actual date of
///      birth is substituted in its stead.
///   </para>
///   <para>
///      When validating the date of birth, 13 character strings only use the
///      initial eight characters in YYYYMMDD format to determine the date.
///      For 11 character strings the initial six characters in YYMMDD format
///      plus the separator character are used to determine the date. Two digit
///      years are assumed to be between 1950 and 2049. If the separator
///      character indicates that the person is 100 years of age or older, then
///      the year is assumed to be between 1850 and 1949.
///   </para>
///   <para>
///      The valid range for a date of birth is January 1, 1800 to
///      December 31, 2099. However, if a six digit date if birth is supplied
///      then the valid range will be between January 1, 1850 to December 31, 2049.
///   </para>
///   <para>
///      For samordningsnummer values, the value returned by the
///      <see cref="DateOfBirth"/> property is an actual date calculated by
///      subtracting 60 from the encoded date of birth.
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
///      for more details.
///   </para>
/// </remarks>
[JsonConverter(typeof(SePersonnummerJsonConverter))]
public record SePersonnummer
{
   private const Int32 ShortFormatLength = 11;
   private const Int32 LongFormatLength = 13;

   // These offsets are measured from the end of the string instead of the start
   // because the date of birth has variable length.
   private const Int32 DateOfBirthOffset = 5;                  // Range end index is exclusive so -1 from expected offset from end
   private const Int32 SeparatorOffset = 5;
   private const Int32 GenderOffset = 2;

   private const Int32 CenturyCutoff = 49;                     // Values < 50 considered 2000's, >= 50 considered 1900's
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
   ///   positions 0-5 (11 character values) or positions 0-7 (13 character
   ///   values).
   ///   - or -
   ///   <paramref name="personnummer"/> contains an invalid separator character
   ///   in position 6 (11 character values) or position 8 (13 character values).
   ///   Valid separator characters are dash ('-') and plus ('+').
   ///   - or -
   ///   <paramref name="personnummer"/> contains an invalid birth serial number
   ///   (i.e. one or more non-digit characters) in positions 7-9 (11 character
   ///   values) or positions 9-11 (13 character values).
   ///   - or -
   ///   <paramref name="personnummer"/> contains an invalid check digit in 
   ///   position 10 (11 character values) or position 12 (13 character values).
   ///   The check digit is calculated using the Luhn algorithm based on the six
   ///   digit date of birth and the three-digit birth serial number. (The
   ///   leading two digits of an eight digit date of birth are ignored.)
   /// </exception>
   /// <remarks>
   ///   The indices given in the exception description are all zero-based.
   /// </remarks>
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
#pragma warning disable IDE0008 // Use explicit type
         var (year, month, day) = GetYearMonthDay(Value);
#pragma warning restore IDE0008 // Use explicit type

         return new DateOnly(year, month, day);
      }
   }

   /// <summary>
   ///   The person's gender, as indicated by the third character of the birth
   ///   sequence number. Odd digits = Male; even digits = Female.
   /// </summary>
   public BinaryGender Gender => Value[^GenderOffset] % 2 == 0       // This works because the ASCII character values for digits have the same odd/even pattern
      ? BinaryGender.Female
      : BinaryGender.Male;

   /// <summary>
   ///   The type of Swedish identifier represented by this instance, indicating
   ///   whether it is a personal identity number (personnummer) or a coordination
   ///   number (samordningsnummer).
   /// </summary>
   /// <remarks>
   ///   A personnummer will have a date of birth with a day component in the normal
   ///   range (1-31) while a samordningsnummer will add 60 to the day component
   ///   of the day of birth resulting in values from 61-91.
   /// </remarks>
   public SeIdentifierType IdentifierType
   {
      get
      {
         var day = Value.Length == ShortFormatLength 
            ? Value.AsSpan()[4..6].ParseTwoDigits() 
            : Value.AsSpan()[6..8].ParseTwoDigits();
         return (day > SamordningsnummerDayOffset)
            ? SeIdentifierType.Samordningsnummer
            : SeIdentifierType.Personnummer;
      }
   }

   /// <summary>
   ///   Indicates if the person is 100 years of age or older as indicated by
   ///   the separator value. A dash ('-') separator indicates a person less
   ///   than 100 years of age, while a plus sign ('+') indicates a person 100
   ///   years of age or older.
   /// </summary>
   public Boolean IsCentenarian => Value[^SeparatorOffset] == Chars.Plus;

   /// <summary>
   ///   The raw personnummer value.
   /// </summary>
   public String Value { get; private init; }

   public static implicit operator String(SePersonnummer personnummer)
      => personnummer?.Value ?? String.Empty;     // Handle null personnummer object gracefully by returning empty string

   // Explicit conversion from String to avoid unintentional conversions that may throw exceptions.
   public static explicit operator SePersonnummer(String? personnummer) => new(personnummer);

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
   public static SePersonnummerValidationResult Validate(String? personnummer)
   {
      if (String.IsNullOrWhiteSpace(personnummer))
      {
         return SePersonnummerValidationResult.Empty;
      }
      else if (personnummer.Length is not ShortFormatLength and not LongFormatLength)
      {
         return SePersonnummerValidationResult.InvalidLength;
      }

      // After performing basic checks, validate the check digit because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      if (!ValidateCheckDigit(personnummer))
      {
         return ValidateAllDigits(personnummer)
            ? SePersonnummerValidationResult.InvalidCheckDigit
            : SePersonnummerValidationResult.InvalidCharacter;
      }
      else if (personnummer.Length == LongFormatLength
               && (!personnummer[0].IsAsciiDigit() || !personnummer[1].IsAsciiDigit()))
      {
         // Check digit does not consider leading two digits for 13 character strings.
         // So validate here.
         return SePersonnummerValidationResult.InvalidCharacter;
      }
      else if (GetSeparator(personnummer) is not Chars.Dash and not Chars.Plus)
      {
         return SePersonnummerValidationResult.InvalidSeparator;
      }
      else if (!ValidateDateOfBirth(personnummer))
      {
         return SePersonnummerValidationResult.InvalidDateOfBirth;
      }

      return SePersonnummerValidationResult.ValidationPassed;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static ReadOnlySpan<Char> GetDateOfBirth(ReadOnlySpan<Char> personnummer)
      => personnummer[..^DateOfBirthOffset];

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Char GetSeparator(ReadOnlySpan<Char> personnummer)
      => personnummer[^SeparatorOffset];

   private static (Int32 year, Int32 month, Int32 day) GetYearMonthDay(ReadOnlySpan<Char> personnummer)
   {
      Int32 year;
      Int32 month;
      Int32 day;
      if (personnummer.Length == ShortFormatLength)
      {
         // Refer to class XML comments for details of 2 digit year calculations.
         year = personnummer.ParseTwoDigits();
         year += year > CenturyCutoff ? 1900 : 2000;
         if (GetSeparator(personnummer) == Chars.Plus)
         {
            year -= 100;
         }
         month = personnummer[2..].ParseTwoDigits();
         day = personnummer[4..].ParseTwoDigits();
      }
      else
      {
         // This works for both 13 character values with separator and 12 character
         // all digit internal representation.
         var century = personnummer.ParseTwoDigits() * 100;
         year = century + personnummer[2..].ParseTwoDigits();
         month = personnummer[4..].ParseTwoDigits();
         day = personnummer[6..].ParseTwoDigits();
      }

      // Handle samordningsnummer, which adds 60 to the date of birth.
      if (day > SamordningsnummerDayOffset)
      {
         day -= SamordningsnummerDayOffset;
      }

      return (year, month, day);
   }

   /// <summary>
   ///   If <see cref="ValidateCheckDigit(String)"/> returns false, determine
   ///   if the reason was an invalid character or an invalid check digit.
   /// </summary>
   private static Boolean ValidateAllDigits(ReadOnlySpan<Char> personnummer)
   {
      const Int32 yymmddLength = 6;
      const Int32 yyyymmddLength = 8;

      var processLength = personnummer.Length;
      var separatorIndex = processLength == ShortFormatLength
         ? yymmddLength
         : yyyymmddLength;

      for(var index = 0; index < processLength; index ++)
      {
         if (index == separatorIndex)
         {
            continue;
         }

         if (!personnummer[index].IsAsciiDigit())
         {
            return false;
         }
      }


      return true;
   }

   private static Boolean ValidateCheckDigit(String personnummer)
   {
      ICheckDigitMask checkDigitMask = personnummer.Length == ShortFormatLength
         ? SePersonNumberShortFormatCheckDigitMask.Instance
         : SePersonNumberLongFormatCheckDigitMask.Instance;
      return CheckDigitAlgorithms.Luhn.Validate(personnummer, checkDigitMask);
   }

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> personnummer)
   {
      const Int32 minimumValidYear = 1800;
      const Int32 maximumValidYear = 2099;

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

      if (year < minimumValidYear || year > maximumValidYear)
      {
         return false;
      }
      else if (month is < 1 or > 12)
      {
         return false;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }
}

public class SePersonnummerJsonConverter : JsonConverter<SePersonnummer>
{
   public override SePersonnummer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var personnummerString = reader.GetString();
      return new SePersonnummer(personnummerString);
   }

   public override void Write(Utf8JsonWriter writer, SePersonnummer value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
