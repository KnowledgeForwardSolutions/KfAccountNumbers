#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Strongly typed business object that represents a Belgian National Register
///   Number (Rijksregisternummer in Dutch, Numéro de registre national in
///   French), a unique identifier assigned to all persons (Belgian citizens and
///   foreign residents) who are registered in Belgium's National Register
///   (Rijksregister/Registre national).
/// </summary>
/// <remarks>
///   <para>
///      A rijksregisternummer is an 11-digit number, structured as YYMMDDXXXCC,
///      with the following elements:
///      <list type="bullet">
///         <item>
///            <term>YYMMDD</term>
///            <description>
///               The person's date of birth in YYMMDD format. The date of birth
///               may be unknown/incomplete and in that case zeros are used in
///               place of the unknown elements.
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
///               equal to the modulus 97 check sum calculated by first
///               prefixing YYMMDDXXX with the digit 2 (i.e. 2YYMMDDXXX) then
///               the person's century of birth is 2000-2099.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A Belgian rijksregisternummer may be formatted as a string of 11
///      consecutive digits (YYMMDDXXXCC) or as a 15 character string with
///      characters separating the individual elements. YY.MM.DD-XXX.CC is the
///      typical display format.
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
///               The value must be either 11 characters (without separators) or
///               15 characters (with separators) in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               All characters (except the optional separator characters) must
///               be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The separator characters, if included, must not be ASCII
///               digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The two trailing (right-most) characters must be a valid
///               modulus 97 check sum (taking into account the possibility of a
///               person born in the year 2000 or later).
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth, after deriving the century of birth from
///               the check sum, must be a valid date between January 1, 1900
///               and December 31,
///               2099.
///               <b>OR</b> the date of birth may use zeros to indicate that
///               some or all of the person's date of birth is unknown (see
///               below for more details).
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
///               If the person's date of birth is incomplete, then the two
///               digit year is used and zeros are used for month and day (for
///               example, 40.00.00-955.69).
///            </description>
///         </item>
///         <item>
///            <description>
///               If there are too many people with incomplete dates of birth
///               for a particular year than can be represented by a three digit
///               sequence number (i.e. more than 499 males with incomplete
///               dates of birth for the year 1940), then 01 is used for the day
///               of birth and the sequence number rolls over to 001
///               (ex. 40.00.01-001.33). (Note that
///               <see cref="BeRijksregisternummer"/> does not enforce an upper
///               limit on the day component in cases of rollover, though
///               multiple rollovers in a single year should be rare.)
///            </description>
///         </item>
///         <item>
///            <description>
///               If the person's date of birth is unknown, then the constant
///               00.00.01 is used.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>87092100294</term>
///            <description>
///               unformatted, date of birth September 21, 1987,
///               gender = female, check digit calculation
///               97 - (870921002 mod 97) = 97 - 3 = 94
///            </description>
///         </item>
///         <item>
///            <term>05.03.11-017.02</term>
///            <description>
///               formatted, date of birth March 11, 2005,
///               gender = male, check digit calculation 97 - (050311017 mod 97)
///               = 97 - 95 = 2
///            </description>
///         </item>
///         <item>
///            <term>55000007612</term>
///            <description>
///               unformatted, date of birth 1955, day/month unknown,
///               gender = female, check digit calculation
///               97 - (550000076 mod 97) = 97 - 85 = 12
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See <see href="https://fr.wikipedia.org/wiki/Num%C3%A9ro_de_registre_national">Wikipedia (French) - Numéro de registre national</see>
///      for more info.
///   </para>
/// </remarks>
[JsonConverter(typeof(BeRijksregisternummerJsonConverter))]
public record BeRijksregisternummer : BeIdentityNumberBase
{
   /// <summary>
   ///   Initializes a new instance of the <see cref="BeRijksregisternummer"/>
   ///   class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Belgian rijksregisternummer.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 11 (or 15 if separator
   ///   characters are used).
   ///   - or -
   ///   <paramref name="value"/> contains a non-digit character in
   ///   any position other than the separator locations.
   ///   - or -
   ///   <paramref name="value"/> has invalid modulus 97 check digit
   ///   characters in the trailing (right-most) character positions.
   ///   - or -
   ///   <paramref name="value"/> is 15 characters in length and has
   ///   an ASCII digit character ('0'-'9') in a separator location.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid sequence number.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid date of birth in
   ///   the leading (left-most) six digits.
   /// </exception>
   public BeRijksregisternummer(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="BeRijksregisternummer"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Belgian rijksregisternummer.
   /// </param>
   /// <param name="validationMode">
   ///   Indicates whether the <paramref name="value"/> requires validation.
   /// </param>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   internal BeRijksregisternummer(String? value, ValidationMode validationMode)
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
               InvalidSequenceNumber invalidSequenceNumber => new UKfValidationException<ValidationError>(invalidSequenceNumber),
               InvalidDateOfBirth invalidDateOfBirth => new UKfValidationException<ValidationError>(invalidDateOfBirth),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetNormalizedValue(value!);
   }

   /// <summary>
   ///   Gets the person's date of birth, derived from the first six digits in
   ///   YYMMDD format and the exact century of birth derived from the check
   ///   digits.
   /// </summary>
   public DateResult DateOfBirth
   {
      get
      {
#pragma warning disable IDE0008 // Use explicit type
         var (year, month, day) = GetYearMonthDay(Value, DateOffsetMode.Rijksregisternummer);
         #pragma warning restore IDE0008 // Use explicit type

         return new DateResult(
            year > 0 ? year : null,
            month > 0 ? month : null,
            day > 0 ? day : null);
      }
   }

   /// <summary>
   ///   Gets the person's gender, as indicated by the sequence number.
   /// </summary>
   /// <remarks>
   ///   Note that gender is always known for rijksregisternummers. (Unlike
   ///   BIS-nummers which allow gender to be unknown.)
   /// </remarks>
   public Gender.BinaryGender Gender
      => Value[^GenderOffset] % 2 == 0 ? default(Gender.Female) : default(Gender.Male);   // This works because the ASCII character values for digits have the same odd/even pattern

   /// <summary>
   ///   Gets the normalized rijksregisternummer value (without separator
   ///   characters).
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="BeRijksregisternummer"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="BeRijksregisternummer"/> to convert.
   /// </param>
   public static implicit operator String(BeRijksregisternummer source)
      => source?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="BeRijksregisternummer"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Belgian rijksregisternummer.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid rijksregisternummer.
   /// </exception>
   public static explicit operator BeRijksregisternummer(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="BeRijksregisternummer"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Belgian rijksregisternummer.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{BeRijksregisternummer, ValidationError}"/>. Will
   ///   contain the new <see cref="BeRijksregisternummer"/> if <paramref name="value"/>
   ///   is valid or a <see cref="BeIdentityNumberBase.ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static CreateResult<BeRijksregisternummer, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new BeRijksregisternummer(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         InvalidSequenceNumber invalidSequenceNumber => (ValidationError)invalidSequenceNumber,
         InvalidDateOfBirth invalidDateOfBirth => (ValidationError)invalidDateOfBirth,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the rijksregisternummer using the supplied
   ///   <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then <see cref="BeIdentityNumberBase.DefaultFormatMask"/> will be used
   ///   instead.
   /// </param>
   /// <returns>
   ///   A formatted Belgian rijksregisternummer.
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
   ///   details on creating a mask to format the rijksregisternummer.
   /// </remarks>
   public String Format(String mask = DefaultFormatMask) => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the rijksregisternummer.
   /// </summary>
   /// <returns>
   ///   The normalized rijksregisternummer, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Belgian rijksregisternummer.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Belgian rijksregisternummer.
   /// </param>
   /// <returns>
   ///   A <see cref="BeIdentityNumberBase.ValidationResult"/> union that
   ///   indicates if the <paramref name="value"/> passed validation or what
   ///   validation error was encountered.
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
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return invalidCharacterPosition == -1
            ? GetInvalidChecksumResult()
            : GetInvalidCharacterResult(value, invalidCharacterPosition);
      }

      if (!ValidateSeparators(value, out var invalidSeparatorPosition))
      {
         return GetInvalidSeparatorResult(value, invalidSeparatorPosition);
      }

      if (!ValidateSequenceNumber(value))
      {
         return GetInvalidSequenceNumberResult(value);
      }

      if (!ValidateDateOfBirth(value, DateOffsetMode.Rijksregisternummer))
      {
         return GetInvalidDateOfBirthResult(value);
      }

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.BeRijksregisternummerInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.BeRijksregisternummerInvalidCheckDigits, CheckDigitAlgorithmName);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.BeRijksregisternummerInvalidDateOfBirth,
         IsFormatted(value) ? value[..8].ToString() : value[..6].ToString(),
         DateFormatName.YYMMDD);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.BeRijksregisternummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(UnformattedLength, Messages.BeRijksregisternummerUnformattedLength),
            new ValidLengthDefinition(FormattedLength, Messages.BeRijksregisternummerFormattedLength),
         ]);

   private static InvalidSeparator GetInvalidSeparatorResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(
         Messages.BeRijksregisternummerInvalidSeparator,
         value[position],
         position);

   private static InvalidSequenceNumber GetInvalidSequenceNumberResult(ReadOnlySpan<Char> value)
      => new(
         Messages.BeRijksregisternummerInvalidSequenceNumber,
         IsFormatted(value) ? value[9..12].ToString() : value[6..9].ToString());
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class BeRijksregisternummerJsonConverter : JsonConverter<BeRijksregisternummer>
{
   public override BeRijksregisternummer? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null;
      }

      var str = reader.GetString();
      return new BeRijksregisternummer(str);
   }

   public override void Write(Utf8JsonWriter writer, BeRijksregisternummer value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
