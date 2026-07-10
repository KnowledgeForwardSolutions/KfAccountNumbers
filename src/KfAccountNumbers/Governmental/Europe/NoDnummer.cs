#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   <para>
///      Strongly typed business object that represents a D-nummer, a temporary
///      Norwegian personal identity number issued to foreign individuals who
///      are not eligible for a permanent identity number (fødselsnummer).
///   </para>
///   <para>
///      <b>Note:</b> See <see cref="NoFoedselsnummer"/> for a similar
///      identifier (fødselsnummer) issued to permanent residents of Norway and
///      <see cref="NoIdentityNumber"/> for a composite type that can represent
///      either a fødselsnummer or a D-nummer.
///   </para>
/// </summary>
/// <remarks>
///   <para>
///      A D-nummer is an 11-digit number structured as DDMMYYIIICC, with the
///      following elements:
///      <list type="bullet">
///         <item>
///            <term>DDMMYY</term>
///            <description>
///               The person's date of birth in DDMMYY format. The <b>DD</b>
///               portion of the date of birth is offset by 40 (i.e. 1-31
///               becomes 41-71) to distinguish D-nummers from fødselsnummers.
///            </description>
///         </item>
///         <item>
///            <term>III</term>
///            <description>
///               Three-digit individual number. The first digit indicates the
///               person's century of birth, with 0-4 = 20th century or
///               1900-1999 and 5-9 = 21st century or 2000-2099. The last digit
///               indicates the person's gender, with odd digits assigned to
///               males and even digits assigned to females.
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
///      When creating a new <see cref="NoDnummer"/>, the following validation
///      rules are applied:
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
///               If the value has length 12, then character position 6
///               (zero-based) must not be an ASCII digit ('0'-'9')
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth (after adjusting for the +40 D-nummer day
///               offset and after determining the century from the individual
///               number) must be a valid date between 01/01/1854 and
///               31/12/2039. Note that the validation specifically does
///               <b>NOT</b> check for future dates, only that the date exists.
///            </description>
///         </item>
///      </list>
///      Note that the <b>DD</b> portion of the date of birth will be 41-71
///      because D-nummer offsets the day of birth by +40 to
///      distinguish from foedselsnummer values.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>60055029566</term>
///            <description>
///               unformatted, date of birth = May 20, 1950), gender = male,
///               check digits = 66
///            </description>
///         </item>
///         <item>
///            <term>70100567871</term>
///            <description>
///               unformatted, date of birth = October 30, 2005), gender =
///               female, check digits = 71
///            </description>
///         </item>
///         <item>
///            <term>530295 34272</term>
///            <description>
///               formatted, date of birth = February 13, 1995), gender =
///               female, check digits = 72
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Note that fødselsnummers use the individual number to determine the
///      century of birth, but the rules are more complicated. Refer to the
///      fødselsnummer documentation for more detail.
///   </para>
///   <para>
///      See <see href="https://en.wikipedia.org/wiki/National_identity_number_(Norway)">Wikipedia - National_identity_number_(Norway)</see>
///      for more information.
///   </para>
/// </remarks>
[JsonConverter(typeof(NoDnummerJsonConverter))]
public record NoDnummer : NoIdentityNumberBase
{
   /// <summary>
   ///   Initializes a new instance of the <see cref="NoDnummer"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a D-nummer.
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
   public NoDnummer(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="NoDnummer"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a D-nummer.
   /// </param>
   /// <param name="validationMode">
   ///   Indicates whether the <paramref name="value"/> requires validation.
   /// </param>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private NoDnummer(String? value, ValidationMode validationMode)
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
         var (day, month, year) = GetDayMonthYear(Value, DateOffsetMode.Dnummer);
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
   ///   Gets a string representation of the D-nummer.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="NoDnummer"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="NoDnummer"/> to convert.
   /// </param>
   public static implicit operator String(NoDnummer source)
      => source?.Value ?? String.Empty;     // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="NoDnummer"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Norwegian D-nummer.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid D-nummer.
   /// </exception>
   public static explicit operator NoDnummer(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="NoDnummer"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Norwegian D-nummer.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{NoDnummer, ValidationError}"/>. Will
   ///   contain the new <see cref="NoDnummer"/> if <paramref name="value"/>
   ///   is valid or a <see cref="NoIdentityNumberBase.ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static CreateResult<NoDnummer, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new NoDnummer(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         InvalidDateOfBirth invalidDateOfBirth => (ValidationError)invalidDateOfBirth,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the D-nummer using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then the default mask
   ///   <see cref="NoIdentityNumberBase.DefaultFormatMask"/> will be used
   ///   instead.
   /// </param>
   /// <returns>
   ///   A formatted D-nummer.
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
   ///   details on creating a mask to format the D-nummer.
   /// </remarks>
   public String Format(String mask = DefaultFormatMask) => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the D-nummer.
   /// </summary>
   /// <returns>
   ///   The raw D-nummer, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Norwegian D-nummer.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Norwegian D-nummer.
   /// </param>
   /// <returns>
   ///   A <see cref="NoIdentityNumberBase.ValidationResult"/> union that indicates if the
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

      if (!ValidateDateOfBirth(value, DateOffsetMode.Dnummer))
      {
         return GetInvalidDateOfBirthResult(value);
      }

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.NoDnummerInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.NoDnummerInvalidCheckDigits, CheckDigitAlgorithmName);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.NoDnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(UnformattedLength, Messages.NoDnummerUnformattedLength),
            new ValidLengthDefinition(FormattedLength, Messages.NoDnummerFormattedLength),
         ]);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(
         Messages.NoDnummerInvalidDateOfBirth,
         value[..SeparatorOffset],
         DateFormatName.DDMMYY);

   private static InvalidSeparator GetInvalidSeparatorResult(ReadOnlySpan<Char> value)
      => new(
         Messages.NoDnummerInvalidSeparator,
         value[SeparatorOffset],
         SeparatorOffset);
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class NoDnummerJsonConverter : JsonConverter<NoDnummer>
{
   public override NoDnummer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new NoDnummer(str);
   }

   public override void Write(Utf8JsonWriter writer, NoDnummer value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
