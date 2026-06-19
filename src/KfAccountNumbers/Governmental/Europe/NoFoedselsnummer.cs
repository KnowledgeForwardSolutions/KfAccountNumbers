// Ignore Spelling: Foedselsnummer Json Nummer

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

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
///      Fødselsnummer and D-nummer are both 11-digit numbers formatted as
///      DDMMYYIIICC, with the following elements:
///      <list type="bullet">
///         <item>
///            <term>DDMMYY</term>
///            <description>
///               The person's date of birth in DDMMYY format. The
///               only difference between a fødselsnummer and a D-nummer is that
///               40 is added to the day component of the person's date of birth
///               (i.e. 130585 becomes 530585). Day values in the range 41-71
///               (inclusive)  are considered D-nummers.
///            </description>
///         </item>
///         <item>
///            <term>III</term>
///            <description>
///               Three digit individual number. All three digits of the
///               individual number are used to derive the century of the date
///               of birth and the last digit of the individual number indicates
///               the person's gender, with odd digits assigned to males and even
///               digits assigned to females. See below for details on the
///               derivation of the century of the date of birth.
///            </description>
///         </item>
///         <item>
///            <term>CC</term>
///            <description>
///               Two separate check digits calculated using a weighted
///               modulus 11 algorithm. The first check digit is calculated
///               for the first nine digits (date of birth and individual number)
///               and the second check digit is calculated for the date of birth,
///               individual number and first check digit.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The 11 character value is sometimes formatted for greater readability
///      by inserting a separator character, generally a space, between the date
///      of birth and the individual number, i.e. DDMMYY IIICC.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>13029597140</term>
///            <description>
///               fødselsnummer, date of birth February 13, 1995, gender = female,
///               check digits 40
///            </description>
///         </item>
///         <item>
///            <term>130295 97140</term>
///            <description>
///               fødselsnummer, date of birth February 13, 1995, gender = female,
///               check digits 40
///            </description>
///         </item>
///         <item>
///            <term>60055029566</term>
///            <description>
///               D-nummer, date of birth May 20, 1950, gender = male, check digits 66
///            </description>
///         </item>
///         <item>
///            <term>600550-29566</term>
///            <description>
///               D-nummer, date of birth May 20, 1950, gender = male, check digits 66
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
///               The string must be either 11 (without separator) or 12 characters
///               (with separator) long.
///            </description>
///         </item>
///         <item>
///            <description>
///               The first six characters must represent a valid date in DDMMYY
///               format (with century derived from the first individual number
///               digits). Note that the validation specifically does <b>NOT</b>
///               check for future dates, only that the date exists. See below
///               for details on the derivation of the century of the date of birth.
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
///      The century of the date of birth has somewhat complicated rules due to
///      several overlapping ranges of years. The rules used in <see cref="NoFoedselsnummer"/>
///      are taken from https://blog.variant.no/ssns-and-pattern-matching-in-c-9-498f96aa71d4.
///      Because of the overlapping ranges (the individual number 500  matches two
///      different rules), the rules must be evaluated in order to arrive at the
///      correct century.  The rules are:
///      <list type="bullet">
///         <item>
///            <term>Rule 1</term>
///            <description>
///               If the individual number is &gt;= 500 and &lt;= 749 AND the
///               two digit year is &gt;= 54 then the century = 1800.
///            </description>
///         </item>
///         <item>
///            <term>Rule 2</term>
///            <description>
///               If the individual number is &lt; 500 then the century = 1900.
///            </description>
///         </item>
///         <item>
///            <term>Rule 3</term>
///            <description>
///               If the individual number is &gt;= 900 AND the two digit year
///               is &gt;= 40 then the century = 1900.
///            </description>
///         </item>
///         <item>
///            <term>Rule 4</term>
///            <description>
///               If the individual number is &gt;= 500 AND the two digit year
///               is &lt;= 39 then the century =2000.
///            </description>
///         </item>
///         <item>
///            <term>Rule 5</term>
///            <description>
///               Otherwise invalid. Validation will return invalid date of birth.
///            </description>
///         </item>
///      </list>
///      According to these rules, the range of valid dates of birth are from
///      January 1, 1854 to December 31, 2039. A date of birth outside this range,
///      even if a valid date, will return invalid date of birth.
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/National_identity_number_(Norway) for more info.
///   </para>
/// </remarks>
[JsonConverter(typeof(NoFoedselsnummerJsonConverter))]
public record NoFoedselsnummer
{
   /// <summary>
   ///   Discriminated union defining the types of identifier that
   ///   <see cref="NoFoedselsnummer"/> can represent.
   /// </summary>
   public union IdentifierCategory(NoIdentifierType.Foedselsnummer, NoIdentifierType.DNummer) { }

   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="NoFoedselsnummer"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="NoFoedselsnummer"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   The name of the check digit algorithm used by fødselsnummers.
   /// </summary>
   public const String CheckDigitAlgorithmName = "Weighted Modulus 11";

   /// <summary>
   ///   Represents the day offset used to distinguish D-nummers from fødselsnummers.
   /// </summary>
   /// <remarks>
   ///   In Norwegian identity numbers, a D-nummer is indicated by
   ///   adding 40 to the day component of the date of birth.
   /// </remarks>
   public const Int32 DNummerDayOffset = 40;

   /// <summary>
   ///   The latest year of birth supported by <see cref="NoFoedselsnummer"/>.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2039;

   /// <summary>
   ///   The earliest year of birth supported by <see cref="NoFoedselsnummer"/>.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1854;

   private const Int32 UnformattedLength = 11;
   private const Int32 FormattedLength = 12;

   private const Int32 SeparatorOffset = 6;

   // Offsets measured from end of value to avoid needing to account for the
   // presence or absence of a separator.
   private const Int32 IndividualNumberOffset = 5;
   private const Int32 GenderOffset = 3;              // Gender indicated by odd/even-ness of individual number, but only need to examine the last digit

   // D-nummer adds 40 to the day portion of date of birth.
   private const Int32 DNummerMinimumDay = 41;
   private const Int32 DNummerMaximumDay = 71;

   private static readonly Int32[] _c1Weights = [3, 7, 6, 1, 8, 9, 4, 5, 2, 1, 0];
   private static readonly Int32[] _c2Weights = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2, 1];

   /// <summary>
   ///   Initializes a new instance of the <see cref="NoFoedselsnummer"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a fødselsnummer.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 11 (or 12 if a separator
   ///   character is used).
   ///   - or -
   ///   <paramref name="value"/> contains a non-digit character in
   ///   any position other than the separator location.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid weighted modulus
   ///   11 check digit in one or both trailing positions.
   ///   - or -
   ///   <paramref name="value"/> contains a digit character in position
   ///   6 (zero-based). Valid separator characters are any non-digit character,
   ///   though space (' ') and dash ('-') are the most common values.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid date of birth in
   ///   positions 0-5 (zero-based).
   /// </exception>
   public NoFoedselsnummer(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="NoFoedselsnummer"/> class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private NoFoedselsnummer(String? value, ValidationMode validationMode)
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
               InvalidChecksum invalidChecksum => new UKfValidationException<ValidationError>(invalidChecksum),
               InvalidSeparator invalidSeparator => new UKfValidationException<ValidationError>(invalidSeparator),
               InvalidDateOfBirth invalidDateOfBirth => new UKfValidationException<ValidationError>(invalidDateOfBirth),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetRawFoedselsnummer(value!);
   }

   /// <summary>
   ///   Gets the person's date of birth, derived from the first six digits in
   ///   DDMMYY format and the exact century of birth derived from the
   ///   individual number.
   /// </summary>
   /// <remarks>
   ///   Note that D-nummer values add 40 to the leading two digits (the DD
   ///   portion of the DDMMYY date of birth). The date of birth property
   ///   automatically adjusts for this offset.
   /// </remarks>
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
   ///   Gets the person's gender, as indicated by the individual number. Odd
   ///   numbers = Male; even numbers = Female.
   /// </summary>
   public BinaryGender Gender => Value[^GenderOffset] % 2 == 0 // This works because the ASCII character values for digits have the same odd/even pattern
      ? BinaryGender.Female
      : BinaryGender.Male;

   /// <summary>
   ///   Gets the type of Norwegian identifier represented by this instance,
   ///   indicating if this is a fødselsnummer or a D-nummer.
   /// </summary>
   /// <remarks>
   ///   The first two digits of the value determine the type of identifier.
   ///   D-nummers add 40 to the first two digits of the date of birth (DDMMYY
   ///   format) so any day of birth between 41 and 71 is considered a D-nummer.
   /// </remarks>
   public IdentifierCategory IdentifierType
      => Value.AsSpan().ParseTwoDigits() is >= DNummerMinimumDay and <= DNummerMaximumDay
         ? default(NoIdentifierType.DNummer)
         : default(NoIdentifierType.Foedselsnummer);

   /// <summary>
   ///   Gets a string representation of the fødselsnummer.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="NoFoedselsnummer"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="NoFoedselsnummer"/> to convert.
   /// </param>
   public static implicit operator String(NoFoedselsnummer source)
      => source?.Value ?? String.Empty;     // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="NoFoedselsnummer"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Norwegian National Identity Number (fødselsnummer).
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid fødselsnummer.
   /// </exception>
   public static explicit operator NoFoedselsnummer(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="NoFoedselsnummer"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Norwegian National Identity Number (fødselsnummer).
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{NoFoedselsnummer, ValidationError}"/>. Will
   ///   contain the new <see cref="NoFoedselsnummer"/> if <paramref name="value"/>
   ///   is valid or a <see cref="ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<NoFoedselsnummer, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new NoFoedselsnummer(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         InvalidDateOfBirth invalidDateOfBirth => (ValidationError)invalidDateOfBirth,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the fødselsnummer using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then the default mask "______ _____" will be used instead.
   /// </param>
   /// <returns>
   ///   A formatted fødselsnummer.
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
   ///   details on creating a mask to format the fødselsnummer.
   /// </remarks>
   public String Format(String mask = "______ _____") => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the fødselsnummer.
   /// </summary>
   /// <returns>
   ///   The raw fødselsnummer, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Norwegian national identity number (fødselsnummer) value.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Norwegian national identity number (fødselsnummer).
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
            Messages.NoFoedselsnummerInvalidLength,
            value.Length,
            GetValidLengthDefinitions());
      }

      // After performing basic checks, validate the check digits because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      ValidationResult validationResult = ValidateCheckDigits(value);
      if (validationResult is not ValidValue)
      {
         // Could be either InvalidCharacter or InvalidCheckDigits.
         return validationResult;
      }

      if (!ValidateSeparator(value))
      {
         return new InvalidSeparator(
            Messages.NoFoedselsnummerInvalidSeparator,
            value[SeparatorOffset],
            SeparatorOffset);
      }

      if (!ValidateDateOfBirth(value))
      {
         return new InvalidDateOfBirth(
            Messages.NoFoedselsnummerInvalidDateOfBirth,
            value[..SeparatorOffset],
            DateFormatName.DDMMYY);
      }

      return default(ValidValue);
   }

   /// <summary>
   ///   Gets an array of details about valid lengths accepted for a
   ///   fødselsnummer.
   /// </summary>
   /// <returns>
   ///   An array of <see cref="ValidLengthDefinition"/>s.
   /// </returns>
   internal static ValidLengthDefinition[] GetValidLengthDefinitions()
      =>
      [
         new ValidLengthDefinition(UnformattedLength, Messages.NoFoedselsnummerUnformattedLength),
         new ValidLengthDefinition(FormattedLength, Messages.NoFoedselsnummerFormattedLength),
      ];

   private static (Int32 Day, Int32 Month, Int32 Year) GetDayMonthYear(ReadOnlySpan<Char> value)
   {
      var day = value.ParseTwoDigits();
      var month = value[2..].ParseTwoDigits();
      var year = value[4..].ParseTwoDigits();

      // Adjust day for possible D-nummer.
      if (day is >= DNummerMinimumDay and <= DNummerMaximumDay)
      {
         day -= DNummerDayOffset;
      }

      // Adjust the year according to the value of the individual number.
      // See https://blog.variant.no/ssns-and-pattern-matching-in-c-9-498f96aa71d4
      // for description of the rules used.
      var individualNumber = value[^IndividualNumberOffset..].ParseThreeDigits();
#pragma warning disable format
      year += (individualNumber, year) switch
      {
         // Rule 1. 500–749: 1854–1899
         (>= 500 and <= 749, >= 54) => 1800,

         // Rule 2. 000–499: 1900–1999
         (< 500, _) => 1900,

         // Rule 3. 900–999: 1940–1999
         (>= 900, >= 40) => 1900,

         // Rule 4. 500–999: 2000–2039
         (>= 500, <= 39) => 2000,

         // No rule
         (_, _) => 0,
      };
#pragma warning restore format

      return (day, month, year);
   }

   private static String GetRawFoedselsnummer(String value)
      => value.Length == UnformattedLength
         ? value
         : String.Concat(
            value.AsSpan(0, SeparatorOffset),
            value.AsSpan(SeparatorOffset + 1));

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> value)
      => value.Length == FormattedLength;

   private static ValidationResult ValidateCheckDigits(ReadOnlySpan<Char> value)
   {
      // Calcuate weighted sums for both check digits in a single pass. Final
      // c1 weight is zero so that it the final digit is excluded from c1 sum.
      var isFormatted = IsFormatted(value);
      var c1Sum = 0;
      var c2Sum = 0;
      var weightIndex = 0;
      var processLength = value.Length;
      for (var charIndex = 0; charIndex < processLength; charIndex++)
      {
         if (isFormatted && charIndex == SeparatorOffset)
         {
            continue;
         }

         var num = value[charIndex] - Chars.DigitZero;
         if (num is < 0 or > 9)
         {
            return new InvalidCharacter(
               Messages.NoFoedselsnummerInvalidCharacter,
               value[charIndex],
               charIndex);
         }

         c1Sum += num * _c1Weights[weightIndex];
         c2Sum += num * _c2Weights[weightIndex];
         weightIndex++;
      }

      // Both weighted sums must be multiples of 11 for the check digits to be valid.
      return (c1Sum % 11) == 0 && (c2Sum % 11) == 0
         ? default(ValidValue)
         : new InvalidChecksum(
            Messages.NoFoedselsnummerInvalidCheckDigits,
            CheckDigitAlgorithmName);
   }

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> value)
   {
#pragma warning disable IDE0008 // Use explicit type
      var (day, month, year) = GetDayMonthYear(value);
#pragma warning restore IDE0008 // Use explicit type

      if (year is < MinimumValidYearOfBirth or > MaximumValidYearOfBirth)
      {
         return false;
      }

      if (month is < 1 or > 12)
      {
         return false;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }

   private static Boolean ValidateSeparator(ReadOnlySpan<Char> value)
      => !IsFormatted(value) || !value[SeparatorOffset].IsAsciiDigit();
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class NoFoedselsnummerJsonConverter : JsonConverter<NoFoedselsnummer>
{
   public override NoFoedselsnummer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new NoFoedselsnummer(str);
   }

   public override void Write(Utf8JsonWriter writer, NoFoedselsnummer value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
