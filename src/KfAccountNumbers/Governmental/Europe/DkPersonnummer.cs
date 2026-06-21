// Ignore Spelling: Json Personnummer

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

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
///               persons born on the same date. The sequence number also
///               encodes additional information. The first digit is used to
///               indicate the century of birth (see below) and the final digit
///               indicates the person's gender, with even numbers for females
///               and odd numbers for males.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A Danish personnummer may be formatted as a string of 10 consecutive
///      digits (DDMMYYSSSS) or as 11 characters with a dash ('-') as a
///      separator character separating the date of birth and the remaining four
///      digits (DDMMYY-SSSS).
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
///               The value must be either 10 characters (without separator) or
///               11 characters (with separator) in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               All characters (except the optional separator character) must
///               be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The optional separator character, if included, must be a dash
///               ('-').
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
///               If the century indicator = 0-3, then the century of birth =
///               1900
///            </description>
///         </item>
///         <item>
///            <description>
///               If the century indicator = 4 <b>AND</b> the two digit year of
///               birth is &lt;= 36 then the century of birth = 2000.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the century indicator = 4 <b>AND</b> the two digit year of
///               birth is &gt;= 37 then the century of birth = 1900.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the century indicator = 5-8 <b>AND</b> the two digit year
///               of birth is &lt;= 57 then the century of birth = 2000.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the century indicator = 5-8 <b>AND</b> the two digit year
///               of birth is &gt;= 58 then the century of birth = 1800.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the century indicator = 9 <b>AND</b> the two digit year of
///               birth is &lt;= 36 then the century of birth = 2000.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the century indicator = 9 <b>AND</b> the two digit year of
///               birth is &gt;= 37 then the century of birth = 1900.
///            </description>
///         </item>
///      </list>
///      According to these rules, the valid range for a date of birth is
///      January 1, 1858 to December 31, 2057.
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
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="DkPersonnummer"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidSeparator,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="DkPersonnummer"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidSeparator,
      InvalidDateOfBirth)
   {
   }

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
   ///   Initializes a new instance of the <see cref="DkPersonnummer"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Danish personnummer.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 10 (or 11 if a separator
   ///   character is used).
   ///   - or -
   ///   <paramref name="value"/> contains a non-digit character in
   ///   any position other than the separator location.
   ///   - or -
   ///   <paramref name="value"/> is 11 characters in length and has a
   ///   character other than dash ('-') as a separator.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid date of birth in
   ///   positions 0-5 (zero-based).
   /// </exception>
   public DkPersonnummer(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="DkPersonnummer"/> class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private DkPersonnummer(String? value, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         ValidationResult validationResult = Validate(value);
         if (validationResult.Value is not ValidValue)
         {
            throw validationResult switch
            {
               EmptyValue emptyValue => new UKfValidationException<ValidationError>(emptyValue),
               InvalidLength invalidLength => new UKfValidationException<ValidationError>(invalidLength),
               InvalidCharacter invalidCharacter => new UKfValidationException<ValidationError>(invalidCharacter),
               InvalidSeparator invalidSeparator => new UKfValidationException<ValidationError>(invalidSeparator),
               InvalidDateOfBirth invalidDateOfBirth => new UKfValidationException<ValidationError>(invalidDateOfBirth),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetRawValue(value!);
   }

   /// <summary>
   ///   Gets the person's date of birth, derived from the first six digits in
   ///   DDMMYY format and the exact century of birth derived from the century
   ///   indicator.
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
   ///   Gets the person's gender, as indicated by the trailing (right-most)
   ///   digit. Odd numbers = Male; even numbers = Female.
   /// </summary>
   public Gender.BinaryGender Gender
      => Value[^GenderOffset] % 2 == 0 ? default(Gender.Female) : default(Gender.Male);   // This works because the ASCII character values for digits have the same odd/even pattern

   /// <summary>
   ///   Gets the raw personnummer value.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="DkPersonnummer"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="DkPersonnummer"/> to convert.
   /// </param>
   public static implicit operator String(DkPersonnummer source)
      => source?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a
   ///   <see cref="DkPersonnummer"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a personnummer.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid personnummer.
   /// </exception>
   public static explicit operator DkPersonnummer(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="DkPersonnummer"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Danish personnummer.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{DkPersonnummer, DkPersonnummerValidationResult}"/>.
   ///   Will contain the new <see cref="DkPersonnummer"/> if
   ///   <paramref name="value"/> is valid or an
   ///   <see cref="DkPersonnummerValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="value"/> is
   ///   invalid.
   /// </returns>
   public static UCreateResult<DkPersonnummer, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new DkPersonnummer(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         InvalidDateOfBirth invalidDateOfBirth => (ValidationError)invalidDateOfBirth,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

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
   /// <returns>
   ///   The raw personnummer, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Danish personnummer.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Danish personnummer.
   /// </param>
   /// <returns>
   ///   A <see cref="ValidationResult"/> union that indicates if the
   ///   <paramref name="value"/> passed validation or what validation error was
   ///   encountered.
   /// </returns>
   public static ValidationResult Validate(String? value)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         return default(EmptyValue);
      }

      if (value.Length is not UnformattedLength and not FormattedLength)
      {
         return new InvalidLength(
            Messages.DkPersonnummerInvalidLength,
            value.Length,
            GetValidLengthDefinitions());
      }

      if (!ValidateAllDigits(value, out var invalidCharacterPosition))
      {
         return new InvalidCharacter(
            Messages.DkPersonnummerInvalidCharacter,
            value[invalidCharacterPosition],
            invalidCharacterPosition);
      }

      if (!ValidateSeparator(value))
      {
         return new InvalidSeparator(
            Messages.DkPersonnummerInvalidSeparator,
            value[SeparatorOffset],
            SeparatorOffset);
      }

      if (!ValidateDateOfBirth(value))
      {
         return new InvalidDateOfBirth(
            Messages.DkPersonnummerInvalidDateOfBirth,
            value[..SeparatorOffset],
            DateFormatName.DDMMYY);
      }

      return default(ValidValue);
   }

   /// <summary>
   ///   Gets an array of details about valid lengths accepted for a
   ///   personnummer.
   /// </summary>
   /// <returns>
   ///   An array of <see cref="ValidLengthDefinition"/>s.
   /// </returns>
   internal static ValidLengthDefinition[] GetValidLengthDefinitions()
      =>
      [
         new ValidLengthDefinition(UnformattedLength, Messages.DkPersonnummerUnformattedLength),
         new ValidLengthDefinition(FormattedLength, Messages.DkPersonnummerFormattedLength),
      ];

   private static (Int32 Day, Int32 Month, Int32 Year) GetDayMonthYear(ReadOnlySpan<Char> value)
   {
      var day = value.ParseTwoDigits();
      var month = value[2..].ParseTwoDigits();
      var year = value[4..].ParseTwoDigits();

      // Adjust the year according to the value of the century indicator.
      var centuryIndicator = value[^CenturyIndicatorOffset] - Chars.DigitZero;

#pragma warning disable format
      year += (centuryIndicator, year) switch
      {
         // Rule 1. Century indicator = 0-3 => 1900
         (>= 0 and <= 3, _) => 1900,

         // Rule 2. Century indicator = 4, year <= 36 => 2000
         (4, <= 36) => 2000,

         // Rule 3. Century indicator = 4, year >= 37 => 1900
         (4, >= 37) => 1900,

         // Rule 4. Century indicator = 5-8, year <= 57 => 2000
         (>= 5 and <= 8, <= 57) => 2000,

         // Rule 5. Century indicator = 5-8, year >= 58 => 1800
         (>= 5 and <= 8, >= 58) => 1800,

         // Rule 6. Century indicator = 9, year <= 36 => 2000
         (9, <= 36) => 2000,

         // Rule 7. Century indicator = 9, year >= 37 => 1900
         (9, >= 37) => 1900,

         _ => 0,
      };
#pragma warning restore format

      return (day, month, year);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static String GetRawValue(String value)
      => value.Length == UnformattedLength
         ? value
         : String.Concat(
            value.AsSpan(0, SeparatorOffset),
            value.AsSpan(SeparatorOffset + 1));

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> value)
      => value.Length == FormattedLength;

   private static Boolean ValidateAllDigits(
      ReadOnlySpan<Char> value,
      out Int32 invalidCharacterPosition)
   {
      var isFormatted = IsFormatted(value);
      var processLength = value.Length;
      for (var index = 0; index < processLength; index++)
      {
         if (isFormatted && index == SeparatorOffset)
         {
            continue;
         }

         if (!value[index].IsAsciiDigit())
         {
            invalidCharacterPosition = index;
            return false;
         }
      }

      invalidCharacterPosition = -1;
      return true;
   }

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> value)
   {
#pragma warning disable IDE0008 // Use explicit type
      var (day, month, year) = GetDayMonthYear(value);
#pragma warning restore IDE0008 // Use explicit type

      if (year is < MinimumValidYearOfBirth or > MaximumValidYearOfBirth)
      {
         // Should be impossible to ever reach this point, but return false
         // out of abundance of caution and to avoid throwing an exception.
         return false;
      }

      if (month is < 1 or > 12)
      {
         return false;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidateSeparator(ReadOnlySpan<Char> value)
      => value.Length == UnformattedLength || value[SeparatorOffset] == Chars.Dash;
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
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
