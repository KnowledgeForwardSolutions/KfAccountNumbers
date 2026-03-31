// Ignore Spelling: Json Personnummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a Danish Personnummer,
///   often informally called a CPR-nummer.
/// </summary>
/// <remarks>
///   <para>
///      A Danish personnummer is a ten-digit number structured as DDMMYYSSSS,
///      with the following elements:
///      <list type="bullet">
///         <item>
///            <term>DDMMYY</term>
///            <description>
///               The person's date of birth in DDMMYY format.
///            </description>
///         </item>
///         <item>
///            <term>SSSS</term>
///            <description>
///               A four digit sequence number used to differentiate between two
///               persons born on the same date. The sequence number also encodes
///               additional information. The first digit is used to indicate the
///               century of birth (see below) and the final digit indicates the
///               person's gender, with even numbers for females and odd numbers
///               for males.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A Danish personnummer may be formatted as a string of 10 consecutive digits
///      (DDMMYYSSSS) or as 11 characters with a dash ('-') as a separator character
///      separating the date of birth and the remaining four digits (DDMMYY-SSSS).
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>070761-4285</term>
///            <description>
///               Date of birth July 7, 1961, gender = male
///            </description>
///         </item>
///         <item>
///            <term>0102036234</term>
///            <description>
///               Date of birth February 1, 2003, gender = female
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      When creating a new <see cref="DkPersonnummer"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 10 characters (without separator) or 11
///               characters (with separator) in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               All characters (except the optional separator character) must be
///               ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The optional separator character, if included, must be a dash ('-').
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth, after deriving the century from the century
///               indicator must be a valid date between January 1, 1858 and
///               December 31, 2057.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The trailing (right-most) digit of the personnummer was originally a
///      modulus 11 check digit. However, in 2007 the use of the check digit was
///      discontinued since available numbers for several dates were exhausted
///      (especially January 1, which was often used in cases where immigrants
///      did not know their exact date of birth). <see cref="DkPersonnummer"/>
///      does not validate a check digit since it is not possible to determine
///      if the personnummer was issued pre- or post-2007.
///   </para>
///   <para>
///      The first digit following the date of birth is used to determine the
///      exact century of birth, but some digits can span more han one century.
///      The following rules are defined:
///      <list type="bullet">
///         <item>
///            <description>
///               If the century indicator = 0-3, then the century of birth = 1900
///            </description>
///         </item>
///         <item>
///            <description>
///               If the century indicator = 4 <b>AND</b> the two digit year of
///               birth is &le; 36 then the century of birth = 2000.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the century indicator = 4 <b>AND</b> the two digit year of
///               birth is &ge; 37 then the century of birth = 1900.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the century indicator = 5-8 <b>AND</b> the two digit year of
///               birth is &le; 57 then the century of birth = 2000.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the century indicator = 5-8 <b>AND</b> the two digit year of
///               birth is &ge; 58 then the century of birth = 1800.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the century indicator = 9 <b>AND</b> the two digit year of
///               birth is &le; 36 then the century of birth = 2000.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the century indicator = 9 <b>AND</b> the two digit year of
///               birth is &ge; 37 then the century of birth = 1900.
///            </description>
///         </item>
///      </list>
///      According to these rules, the valid range for a date of birth is
///      January 1, 1858 to December 31 2057.
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/Personal_identification_number_(Denmark)
///      and https://da.wikipedia.org/wiki/CPR-nummer for more info.
///   </para>
/// </remarks>
[JsonConverter(typeof(DkPersonnummerJsonConverter))]
public record DkPersonnummer
{
   /// <summary>
   ///   The latest year of birth supported by <see cref="DkPersonnummer"/>.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2057;

   /// <summary>
   ///   The earliest year of birth supported by <see cref="DkPersonnummer"/>.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1858;

   private const Int32 UnformattedLength = 10;
   private const Int32 FormattedLength = 11;

   private const Int32 SeparatorOffset = 6;

   // These constants are measured from the end of the value;
   private const Int32 CenturyIndicatorOffset = 4;
   private const Int32 GenderOffset = 1;

   /// <summary>
   ///   Initialize a new instance of the <see cref="DkPersonnummer"/> class.
   /// </summary>
   /// <param name="personnummer">
   ///   String representation of a Danish personnummer.
   /// </param>
   /// <exception cref="KfValidationException{DkPersonnummerValidationResult}">
   ///   <paramref name="personnummer"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="personnummer"/> is not length 10 (or 11 if a separator
   ///   character is used).
   ///   - or -
   ///   <paramref name="personnummer"/> contains a non-digit character in
   ///   any position other than the separator location.
   ///   - or -
   ///   <paramref name="personnummer"/> is 11 characters in length and has a
   ///   character other than dash ('-') as a separator.
   ///   - or -
   ///   <paramref name="personnummer"/> contains an invalid date of birth in
   ///   positions 0-5 (zero-based).
   /// </exception>
   public DkPersonnummer(String? personnummer)
      : this(personnummer, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has already
   ///   been validated.
   /// </summary>
   private DkPersonnummer(String? personnummer, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         DkPersonnummerValidationResult validationResult = Validate(personnummer);
         if (validationResult != DkPersonnummerValidationResult.ValidationPassed)
         {
            throw validationResult.ToValidationException();
         }
      }

      Value = GetRawValue(personnummer!);
   }

   /// <summary>
   ///   The person's date of birth, derived from the first six digits in DDMMYY
   ///   format and the exact century of birth derived from the century indicator.
   /// </summary>
   public DateOnly DateOfBirth
   {
      get
      {
#pragma warning disable IDE0008 // Use explicit type
         var (day, month, year) = GetDayMonthYear(Value);
#pragma warning restore IDE0008 // Use explicit type

         return new DateOnly(year, month, day);
      }
   }

   /// <summary>
   ///   The person's gender, as indicated by the trailing (right-most) digit.
   ///   Odd numbers = Male; even numbers = Female.
   /// </summary>
   public BinaryGender Gender => Value[^GenderOffset] % 2 == 0       // This works because the ASCII character values for digits have the same odd/even pattern
      ? BinaryGender.Female
      : BinaryGender.Male;

   /// <summary>
   ///   The raw personnummer value.
   /// </summary>
   public String Value { get; private init; }

   public static implicit operator String(DkPersonnummer personnummer)
      => personnummer?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   // Explicit conversion from String to avoid unintentional conversions that may throw exceptions.
   public static explicit operator DkPersonnummer(String? personnummer) => new(personnummer);

   /// <summary>
   ///   Create a new <see cref="DkPersonnummer"/> using the Result pattern.
   /// </summary>
   /// <param name="personnummer">
   ///   String representation of a Danish personnummer.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{DkPersonnummer, DkPersonnummerValidationResult}"/>.
   ///   Will contain the new <see cref="DkPersonnummer"/> if 
   ///   <paramref name="personnummer"/> is valid or an
   ///   <see cref="DkPersonnummerValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="personnummer"/> is 
   ///   invalid.
   /// </returns>
   public static CreateResult<DkPersonnummer, DkPersonnummerValidationResult> Create(String? personnummer)
   {
      DkPersonnummerValidationResult validationResult = Validate(personnummer);
      return validationResult == DkPersonnummerValidationResult.ValidationPassed
         ? new DkPersonnummer(personnummer, validationMode: ValidationMode.BypassValidation)
         : validationResult;
   }

   /// <summary>
   ///   Format the personnummer using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then the default mask "______-____" will be used instead.
   /// </param>
   /// <returns>
   ///   A formatted personnummer.
   /// </returns>
   /// <exception cref="ArgumentNullException">
   ///   <paramref name="mask"/> is <see langword="null"/>.
   /// </exception>
   /// <exception cref="ArgumentException">
   ///   <paramref name="mask"/> is <see cref="String.Empty"/> or all whitespace
   ///   characters.
   /// </exception>
   /// <remarks>
   ///   <see cref="ExtensionMethods.FormatWithMask(String, String)"/> for more
   ///   details on creating a mask to format the personnummer.
   /// </remarks>
   public String Format(String mask = "______-____") => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the personnummer.
   /// </summary>
   /// <remarks>
   ///   Will return the raw personnummer, without a separator character.
   /// </remarks>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="personnummer"/> to determine if it contains a
   ///   valid Danish personnummer.
   /// </summary>
   /// <param name="personnummer">
   ///   String representation of a Danish personnummer.
   /// </param>
   /// <returns>
   ///   A <see cref="DkPersonnummerValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="personnummer"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static DkPersonnummerValidationResult Validate(String? personnummer)
   {
      if (String.IsNullOrWhiteSpace(personnummer))
      {
         return DkPersonnummerValidationResult.Empty;
      }
      else if (personnummer.Length is not UnformattedLength and not FormattedLength)
      {
         return DkPersonnummerValidationResult.InvalidLength;
      }
      else if (!ValidateAllDigits(personnummer))
      {
         return DkPersonnummerValidationResult.InvalidCharacter;
      }
      else if (!ValidateSeparator(personnummer))
      {
         return DkPersonnummerValidationResult.InvalidSeparator;
      }
      else if (!ValidateDateOfBirth(personnummer))
      {
         return DkPersonnummerValidationResult.InvalidDateOfBirth;
      }

      return DkPersonnummerValidationResult.ValidationPassed;
   }

   private static (Int32 day, Int32 month, Int32 year) GetDayMonthYear(ReadOnlySpan<Char> personnummer)
   {
      var day = personnummer.ParseTwoDigits();
      var month = personnummer[2..].ParseTwoDigits();
      var year = personnummer[4..].ParseTwoDigits();

      // Adjust the year according to the value of the century indicator.
      var centuryIndicator = personnummer[^CenturyIndicatorOffset] - Chars.DigitZero;

      year += (centuryIndicator, year) switch
      {
         // Rule 1. Century indicator = 0-3 => 1900
         ( >= 0 and <= 3, _) => 1900,
         // Rule 2. Century indicator = 4, year <= 36 => 2000
         (4, <= 36) => 2000,
         // Rule 3. Century indicator = 4, year >= 37 => 1900
         (4, >= 37) => 1900,
         // Rule 4. Century indicator = 5-8, year <= 57 => 2000
         ( >= 5 and <= 8, <= 57) => 2000,
         // Rule 5. Century indicator = 5-8, year >= 58 => 1800
         ( >= 5 and <= 8, >= 58) => 1800,
         // Rule 6. Century indicator = 9, year <= 36 => 2000
         (9, <= 36) => 2000,
         // Rule 7. Century indicator = 9, year >= 37 => 1900
         (9, >= 37) => 1900,
         _ => 0
      };

      return (day, month, year);
   }

   private static String GetRawValue(String personnummer)
      => personnummer.Length == UnformattedLength
         ? personnummer
         : String.Concat(
            personnummer.AsSpan(0, SeparatorOffset),
            personnummer.AsSpan(SeparatorOffset + 1));

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> personnummer)
      => personnummer.Length == FormattedLength;

   private static Boolean ValidateAllDigits(ReadOnlySpan<Char> personnummer)
   {
      var isFormatted = IsFormatted(personnummer);
      var processLength = personnummer.Length;
      for (var index = 0; index < processLength; index++)
      {
         if (isFormatted && index == SeparatorOffset)
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

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> personnummer)
   {
#pragma warning disable IDE0008 // Use explicit type
      var (day, month, year) = GetDayMonthYear(personnummer);
#pragma warning restore IDE0008 // Use explicit type

      if (year < MinimumValidYearOfBirth || year > MaximumValidYearOfBirth)
      {
         // Should be impossible to ever reach this point, but return false
         // out of abundance of caution and to avoid throwing an exception.
         return false;
      }

      if (month < 1 || month > 12)
      {
         return false;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }

   private static Boolean ValidateSeparator(ReadOnlySpan<Char> personnummer)
      => personnummer.Length == UnformattedLength || personnummer[SeparatorOffset] == Chars.Dash;
}

public class DkPersonnummerJsonConverter : JsonConverter<DkPersonnummer>
{
   public override DkPersonnummer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new DkPersonnummer(str);
   }

   public override void Write(Utf8JsonWriter writer, DkPersonnummer value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
