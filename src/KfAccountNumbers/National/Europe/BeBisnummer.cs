#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.National.Europe;

/// <summary>
///   <para>
///      Strongly typed business object that represents a Belgian BIS-nummer
///      (or numéro bis), the identifier issued to non-residents and individuals
///      who are not recorded in Belgium's National Register
///      (Rijksregister/Registre national).
///   </para>
///   <para>
///      BIS-nummers apply an offset to the month portion of the date of birth
///      element to distinguish them from rijksregisternummers issued to Belgian
///      citizens and permanent residents.
///   </para>
/// </summary>
/// <remarks>
///   <para>
///      A Belgian BIS-nummer is an 11-digit number, structured as YYMMDDXXXCC,
///      with the following elements:
///      <list type="bullet">
///         <item>
///            <term>YYMMDD</term>
///            <description>
///               6-digit date of birth in YYMMDD format. Note that the MM
///               portion of the date of birth will be 21-32 or 41-52 because
///               BIS-nummers offset the month of birth by either +20 or +40 to
///               distinguish from rijksregisternummer values. The date of birth
///               may be unknown/incomplete and in that case zeros are used in
///               place of the unknown elements.
///            </description>
///         </item>
///         <item>
///            <term>XXX</term>
///            <description>
///               3-digit sequence number used to distinguish between persons
///               born on the same date. The last digit indicates the person's
///               gender, with odd numbers = male and even numbers = female.
///               (See below for exception when gender is unknown.)
///            </description>
///         </item>
///         <item>
///            <term>CC</term>
///            <description>
///               Two digit modulus 97 check sum calculated for the YYMMDD and
///               XXX elements. The check sum is also used to indicate century
///               of birth. If CC is equal to the normal modulus 97 check sum
///               then the person's century of birth is 1900-1999. If CC is
///               equal to the modulus 97 check sum calculated by first
///               prefixing YYMMDDXXX with the digit 2 (i.e. 2YYMMDDXXX) then
///               the person's century of birth is 2000-2099.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A Belgian BIS-nummer may be formatted as a string of 11 consecutive
///      digits (YYMMDDXXXCC) or as a 15 character string with characters
///      separating the individual elements. YY.MM.DD-XXX.CC is the typical
///      display format.
///   </para>
///   <para>
///      When creating a new <see cref="BeBisnummer"/>, the following validation
///      rules are applied:
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
///               The sequence number may not be 000 or 999.
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth, after deriving the century of birth from
///               the check sum and taking into account the BIS number offset,
///               must be a valid date between January 1, 1900 and December 31,
///               2099.
///               <b>OR</b> the date of birth may use zeros to indicate that
///               some or all of the person's date of birth is unknown (see
///               below for more details).
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
///               <see cref="BeBisnummer"/> does not enforce an upper
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
///         <item>
///            <description>
///               As noted above, BIS-nummers apply an offset to the month
///               component of the date of birth to distinguish them from
///               rijksregisternummers. If the person's gender is known when the
///               BIS-nummer is issued, then <b>40</b> is added to the month;
///               Otherwise <b>20</b> is added to the month.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      For cases of a person with an incomplete or unknown date of birth,
///      <see cref="BeBisnummer"/> stacks the appropriate rules. For example,
///      87.40.00-023.47 would be the BIS number for a person with an incomplete
///      date of birth born in 1987.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>17.51.08-046.40</term>
///            <description>
///               formatted, date of birth November 8, 1917, gender = female,
///               check digit calculation 97 - (175108046 mod 97) = 97 - 57 = 40
///            </description>
///         </item>
///         <item>
///            <term>09200000265</term>
///            <description>
///              unformatted, date of birth incomplete, year of birth 2009,
///              gender unknown, check digit calculation 97 - (2092000002 mod
///              97) = 97 - 32 = 65
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See <see href="https://fr.wikipedia.org/wiki/Num%C3%A9ro_de_registre_national">Wikipedia (French) - Numéro de registre national</see>
///      for more info.
///   </para>
/// </remarks>
[JsonConverter(typeof(BeBisnummerJsonConverter))]
public record BeBisnummer : BeIdentityNumberBase
{
   /// <summary>
   ///   Initializes a new instance of the <see cref="BeBisnummer"/>
   ///   class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a BIS-nummer.
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
   public BeBisnummer(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="BeBisnummer"/>
   ///   class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private BeBisnummer(String? value, ValidationMode validationMode)
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
         var (year, month, day) = GetYearMonthDay(Value, DateOffsetMode.Bisnummer);
#pragma warning restore IDE0008 // Use explicit type

         return new DateResult(
            year > 0 ? year : null,
            month > 0 ? month : null,
            day > 0 ? day : null);
      }
   }

   /// <summary>
   ///   Gets an <see cref="KfOption{TS}"/> that indicates the
   ///   person's gender, as indicated by the sequence number (and the month
   ///   offset). May be <see cref="None"/> in the case of a BIS-nummer with an
   ///   unknown gender.
   /// </summary>
   public KfOption<Gender.BinaryGender> Gender
   {
      get
      {
         ReadOnlySpan<Char> span = Value.AsSpan();

         // Check for BIS-nummer with unknown gender.
         var num = span[2..].ParseTwoDigits();
         if (num is >= 20 and <= 32)
         {
            return default(None);
         }

         Gender.BinaryGender gender = Value[^GenderOffset] % 2 == 0 ? default(Gender.Female) : default(Gender.Male);   // This works because the ASCII character values for digits have the same odd/even pattern
         return gender;
      }
   }

   /// <summary>
   ///   Gets the normalized BIS-nummer value (without separator characters).
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="BeBisnummer"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="BeBisnummer"/> to convert.
   /// </param>
   public static implicit operator String(BeBisnummer source)
      => source?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="BeBisnummer"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Belgian BIS-nummer.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid BIS-nummer.
   /// </exception>
   public static explicit operator BeBisnummer(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="BeBisnummer"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Belgian BIS-nummer.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{BeBisnummer, ValidationError}"/>. Will
   ///   contain the new <see cref="BeBisnummer"/> if <paramref name="value"/>
   ///   is valid or a <see cref="BeIdentityNumberBase.ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static CreateResult<BeBisnummer, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new BeBisnummer(value, ValidationMode.BypassValidation),
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
   ///   Format the BIS-nummer using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then <see cref="BeIdentityNumberBase.DefaultFormatMask"/> will be used
   ///   instead.
   /// </param>
   /// <returns>
   ///   A formatted Belgian BIS-nummer.
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
   ///   details on creating a mask to format the BIS-nummer.
   /// </remarks>
   public String Format(String mask = DefaultFormatMask) => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the BIS-nummer.
   /// </summary>
   /// <returns>
   ///   The normalized BIS-nummer, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Belgian BIS-nummer.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Belgian BIS-nummer.
   /// </param>
   /// <returns>
   ///   A <see cref="BeIdentityNumberBase.ValidationResult"/> union that
   ///   indicates if the <paramref name="value"/> passed validation or what
   ///   validation error was
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

      if (!ValidateDateOfBirth(value, DateOffsetMode.Bisnummer))
      {
         return GetInvalidDateOfBirthResult(value);
      }

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.BeBisnummerInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.BeBisnummerInvalidCheckDigits, CheckDigitAlgorithmName);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.BeBisnummerInvalidDateOfBirth,
         IsFormatted(value) ? value[..8].ToString() : value[..6].ToString(),
         DateFormatName.YYMMDD);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.BeBisnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(UnformattedLength, Messages.BeBisnummerUnformattedLength),
            new ValidLengthDefinition(FormattedLength, Messages.BeBisnummerFormattedLength),
         ]);

   private static InvalidSeparator GetInvalidSeparatorResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(
         Messages.BeBisnummerInvalidSeparator,
         value[position],
         position);

   private static InvalidSequenceNumber GetInvalidSequenceNumberResult(ReadOnlySpan<Char> value)
      => new(
         Messages.BeBisnummerInvalidSequenceNumber,
         IsFormatted(value) ? value[9..12].ToString() : value[6..9].ToString());
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class BeBisnummerJsonConverter : JsonConverter<BeBisnummer>
{
   public override BeBisnummer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new BeBisnummer(str);
   }

   public override void Write(Utf8JsonWriter writer, BeBisnummer value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
