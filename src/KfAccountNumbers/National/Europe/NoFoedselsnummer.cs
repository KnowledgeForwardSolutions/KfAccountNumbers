// Ignore Spelling: Foedselsnummer Json Nummer

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.National.Europe;

/// <summary>
///   <para>
///      Strongly typed business object that represents a Norwegian national
///      identity number (fødselsnummer).
///   </para>
///   <para>
///      <b>Note:</b> See <see cref="NoDnummer"/> for a similar
///      identifier (D-nummer) issued to temporary residents of Norway and
///      <see cref="NoIdentityNumber"/> for a composite type that can represent
///      either a fødselsnummer or a D-nummer.
///   </para>
/// </summary>
/// <remarks>
///   <para>
///      A fødselsnummer is an 11-digit number structured as DDMMYYIIICC, with
///      the following elements:
///      <list type="bullet">
///         <item>
///            <term>DDMMYY</term>
///            <description>
///               The person's date of birth in DDMMYY format.
///            </description>
///         </item>
///         <item>
///            <term>III</term>
///            <description>
///               Three-digit individual number. Also used to determine the
///               century of the person's birth (see below for the rules to
///               derive the century of birth). The last digit indicates the
///               person's gender, with odd digits assigned to males and even
///               digits assigned to females.
///            </description>
///         </item>
///         <item>
///            <term>CC</term>
///            <description>
///               Two separate check digits calculated using a weighted
///               modulus 11 algorithm. The first check digit is calculated
///               for the first nine digits (date of birth and individual
///               number) and the second check digit is calculated for the date
///               of birth, individual number and first check digit.
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
///               All non-separator characters must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing two characters must be valid weighted modulus 11
///               check digits.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the value has length 12, then the character at position 6
///               (zero-based) must not be an ASCII digit ('0'-'9')
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth (after determining the century from the
///               individual number) must be a valid date between 01/01/1854 and
///               31/12/2039. Note that the validation specifically does
///               <b>NOT</b> check for future dates, only that the date exists.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>13029597140</term>
///            <description>
///               unformatted, date of birth = February 13, 1995, gender = male,
///               check digits = 40
///            </description>
///         </item>
///         <item>
///            <term>20050559433</term>
///            <description>
///               unformatted, date of birth = May 20, 2005, gender = female,
///               check digits = 33
///            </description>
///         </item>
///         <item>
///            <term>130682 27938</term>
///            <description>
///               formatted, date of birth = June 13, 1982, gender = male, check
///               digits = 38
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The century of the date of birth has somewhat complicated rules due to
///      several overlapping ranges of years. The rules used in
///      <see cref="NoFoedselsnummer"/> are taken from this
///      <see href="https://blog.variant.no/ssns-and-pattern-matching-in-c-9-498f96aa71d4">article</see>
///      published on Medium.com. Because of the overlapping ranges (the
///      individual number 500 matches two different rules), the rules must be
///      evaluated in order to arrive at the correct century.  The rules are:
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
///               is &lt;= 39 then the century = 2000.
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
///      See <see href="https://en.wikipedia.org/wiki/National_identity_number_(Norway)">Wikipedia - National_identity_number_(Norway)</see>
///      for more information.
///   </para>
/// </remarks>
[JsonConverter(typeof(NoFoedselsnummerJsonConverter))]
public record NoFoedselsnummer : NoIdentityNumberBase
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new Norwegian fødselsnummer.
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
   ///   validating Norwegian fødselsnummers.
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
   /// <param name="value">
   ///   String representation of a fødselsnummer.
   /// </param>
   /// <param name="validationMode">
   ///   Indicates whether the <paramref name="value"/> requires validation.
   /// </param>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   internal NoFoedselsnummer(String? value, ValidationMode validationMode)
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

      Value = GetNormalizedValue(value!);
   }

   /// <summary>
   ///   Gets the person's date of birth, derived from the first six digits in
   ///   DDMMYY format and the exact century of birth derived from the
   ///   individual number.
   /// </summary>
   public DateOnly DateOfBirth
   {
      get
      {
#pragma warning disable IDE0008 // Use explicit type
         var (day, month, year) = GetDayMonthYear(Value, DateOffsetMode.Fodselsnummer);
#pragma warning restore IDE0008 // Use explicit type

         return new DateOnly(year, month, day);
      }
   }

   /// <summary>
   ///   Gets the person's gender, as indicated by the individual number. Odd
   ///   numbers = Male; even numbers = Female.
   /// </summary>
   public Gender.BinaryGender Gender
      => Value[^GenderOffset] % 2 == 0 ? default(Gender.Female) : default(Gender.Male);   // This works because the ASCII character values for digits have the same odd/even pattern

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
   ///   String representation of a Norwegian National Identity Number
   ///   (fødselsnummer).
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid fødselsnummer.
   /// </exception>
   public static explicit operator NoFoedselsnummer(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="NoFoedselsnummer"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Norwegian National Identity Number
   ///   (fødselsnummer).
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{NoFoedselsnummer, ValidationError}"/>. Will
   ///   contain the new <see cref="NoFoedselsnummer"/> if
   ///   <paramref name="value"/> is valid or a <see cref="ValidationError"/>
   ///   that identifies the validation rule that was failed if
   ///   <paramref name="value"/> is invalid.
   /// </returns>
   public static CreateResult<NoFoedselsnummer, ValidationError> Create(String? value)
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
   ///   then the default mask
   ///   <see cref="NoIdentityNumberBase.DefaultFormatMask"/> will be used
   ///   instead.
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
   public String Format(String mask = DefaultFormatMask) => Value.FormatWithMask(mask);

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
   ///   String representation of a Norwegian national identity number
   ///   (fødselsnummer).
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
         return GetInvalidLengthResult(value);
      }

      // After performing basic checks, validate the check digits because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      if (!ValidateCheckDigits(value, out var invalidCharacterPosition))
      {
         return invalidCharacterPosition == -1
            ? GetInvalidChecksumResult()
            : GetInvalidCharacterResult(value, invalidCharacterPosition);
      }

      if (!ValidateSeparator(value))
      {
         return GetInvalidSeparatorResult(value);
      }

      if (!ValidateDateOfBirth(value, DateOffsetMode.Fodselsnummer))
      {
         return GetInvalidDateOfBirthResult(value);
      }

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.NoFoedselsnummerInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.NoFoedselsnummerInvalidCheckDigits, CheckDigitAlgorithmName);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(
         Messages.NoFoedselsnummerInvalidDateOfBirth,
         value[..SeparatorOffset],
         DateFormatName.DDMMYY);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.NoFoedselsnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(UnformattedLength, Messages.NoFoedselsnummerUnformattedLength),
            new ValidLengthDefinition(FormattedLength, Messages.NoFoedselsnummerFormattedLength),
         ]);

   private static InvalidSeparator GetInvalidSeparatorResult(ReadOnlySpan<Char> value)
      => new(
         Messages.NoFoedselsnummerInvalidSeparator,
         value[SeparatorOffset],
         SeparatorOffset);
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
