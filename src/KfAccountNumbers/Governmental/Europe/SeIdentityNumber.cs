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
///               Samordningsnummer, date of birth 19890261 (actual date of
///               birth = 19890201), less than 100 years old, gender = female,
///               check digit  = 3.
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
///               All non-separator characters must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing character must be a valid Luhn algorithm check
///               digit, calculated from the YYMMDD digits of the date of birth
///               (if the value uses YYYYMMDD format, the leading two digits are
///               ignored) and the three digits of the birth serial number.
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
///               The leading 6 digits of an 11 character value or the leading
///               8 digits of a 13 character value must represent a valid date.
///               If the value is a samordningsnummer, the date of birth must be
///               valid after adjusting for the samordningsnummer day offset.
///               Note that the validation specifically does <b>NOT</b> check
///               for future dates, only that the date exists.
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
///      When determining if a date of birth is valid, values with six digit
///      dates of birth use the separator character to derive the full four
///      digit year. Year values between 00 and 49 are assumed to be 2000 to
///      2049 and year values between 50 and 99 are assumed to be 1950 to 1999.
///      If the separator character indicates that the person is at least 100
///      years of age, then 100 is subtracted from the year, resulting in 00 to
///      40 meaning 1900 to 1949 and 50 to 99 meaning 1850 to 1899.
///   </para>
///   <para>
///      The valid range for a date of birth is January 1, 1800 to
///      December 31, 2099. However, if a six digit date of birth is supplied
///      then the valid range will be between January 1, 1850 to December 31,
///      2049.
///   </para>
///   <para>
///      For samordningsnummer values, the value returned by the
///      <see cref="DateOfBirth"/> property is an actual date calculated by
///      subtracting 60 from the encoded date of birth.
///   </para>
///   <para>
///      Internally, <see cref="SeIdentityNumber"/> stores a 12-digit
///      representation consisting of the date of birth in YYYYMMDD format
///      followed by the birth serial number and check digit (no separator). The
///      <see cref="Value"/> property returns this internal representation.
///   </para>
///   <para>
///      When comparing <see cref="SeIdentityNumber"/> objects for equality, the
///      internal 12-digit representation is used. This means two objects
///      representing the same person will be considered equal regardless of
///      whether they were created from 11-character or 13-character input
///      strings.
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
///      for more details.
///   </para>
/// </remarks>
[JsonConverter(typeof(SeIdentityNumberJsonConverter))]
public record SeIdentityNumber : SeIdentityNumberBase
{
   /// <summary>
   ///   Initializes a new instance of the <see cref="SeIdentityNumber"/>
   ///   class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a samordningsnummer.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 11 or 13.
   ///   - or -
   ///   <paramref name="value"/> has a non-separator character that is not an
   ///   ASCII digit ('0'-9').
   ///   - or -
   ///   <paramref name="value"/> does not have a valid Luhn algorithm check
   ///   digit in the trailing (right-most) character position.
   ///   - or -
   ///   <paramref name="value"/> has a separator character that is not a dash
   ///   ('-') or a plus sign ('+'). The separator position is 6 (when length is
   ///   11) or 8 (when length is 13). The character positions are zero-based.
   ///   - or -
   ///   <paramref name="value"/> does not have start with a valid date of birth
   ///   (either YYMMDD format for values with length 11 or YYYYMMDD format for
   ///   values with length 13). If the value is a samordningsnummer, then the
   ///   samordningsnummer day offset is applied before checking if the date of
   ///   birth is valid.
   /// </exception>
   public SeIdentityNumber(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="SeIdentityNumber"/>
   ///   class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private SeIdentityNumber(String? value, ValidationMode validationMode)
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

      Value = GetNormalizedValue(value);
   }

   /// <summary>
   ///   Gets the person's date of birth, derived from the date of birth portion
   ///   of 13 character identity number (YYYYMMDD format) or from the date
   ///   portion of an 11 character identity number (YYMMDD format) and the
   ///   separator character.
   /// </summary>
   /// <remarks>
   ///   <para>
   ///      See the class comments for the rules for deriving the date of birth
   ///      for an 11 character identity number.
   ///   </para>
   ///   <para>
   ///      Note that the indicated date of birth may not be the person's exact
   ///      date of birth as it is possible that a day may run out of birth
   ///      serial numbers. In that case, a date close to the actual date of
   ///      birth is used instead.
   ///   </para>
   ///   <para>
   ///      Note also that samordningsnummer values encode the date of birth by
   ///      adding 60 to the day (i.e. "950123" encodes as "950183"). The
   ///      DateOfBirth property will automatically adjust the date of birth to
   ///      an actual date.
   ///   </para>
   /// </remarks>
   public DateOnly DateOfBirth => GetDateOfBirth(Value);

   /// <summary>
   ///   Gets the person's gender, as indicated by the third character of the
   ///   birth sequence number. Odd digits = Male; even digits = Female.
   /// </summary>
   public Gender.BinaryGender Gender
      => Value[^GenderOffset] % 2 == 0 ? default(Gender.Female) : default(Gender.Male);   // This works because the ASCII character values for digits have the same odd/even pattern

   /// <summary>
   ///   Gets the normalized identity number value.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="SeIdentityNumber"/> to a <see cref="String"/>,
   ///   returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="SeIdentityNumber"/> to convert.
   /// </param>
   public static implicit operator String(SeIdentityNumber source)
      => source?.Value ?? String.Empty;     // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="SeIdentityNumber"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Swedish identity number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid Swedish identity number.
   /// </exception>
   public static explicit operator SeIdentityNumber(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="SeIdentityNumber"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Swedish identity number.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{SeIdentityNumber, ValidationError}"/>. Will
   ///   contain the new <see cref="SeIdentityNumber"/> if <paramref name="value"/>
   ///   is valid or a <see cref="SeIdentityNumberBase.ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static CreateResult<SeIdentityNumber, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new SeIdentityNumber(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         InvalidDateOfBirth invalidDateOfBirth => (ValidationError)invalidDateOfBirth,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Returns a string representation of the value in a long 13 character
   ///   format, combining the date of birth in YYYYMMDD format, a separator
   ///   character, the three digit birth serial number and the check digit.
   /// </summary>
   /// <param name="timeProvider">
   ///   Optional. <see cref="TimeProvider"/> instance used to determine the
   ///   exact age of the person. Persons 100 years or older will have a plus
   ///   ('+') as a separator; otherwise a dash ('-') is used as the separator.
   ///   If the <paramref name="timeProvider"/> is <see langword="null"/> then
   ///   the separator character will default to a dash ('-').
   /// </param>
   /// <returns>
   ///   The identity number formatted as a 13 character string.
   /// </returns>
   public String ToLongFormatValue(TimeProvider? timeProvider = null)
      => InternalRepresentationToLongFormat(Value, timeProvider);

   /// <summary>
   ///   Returns a string representation of the value in a short 11 character
   ///   format, combining the date of birth in YYMMDD format, a separator
   ///   character, the three digit birth serial number and the check digit.
   /// </summary>
   /// <param name="timeProvider">
   ///   Optional. <see cref="TimeProvider"/> instance used to determine the
   ///   exact age of the person. Persons 100 years or older will have a plus
   ///   ('+') as a separator; otherwise a dash ('-') is used as the separator.
   ///   If the <paramref name="timeProvider"/> is <see langword="null"/> then
   ///   the separator character will default to a dash ('-').
   /// </param>
   /// <returns>
   ///   The identity number formatted as an 11 character string.
   /// </returns>
   public String ToShortFormatValue(TimeProvider? timeProvider = null)
      => InternalRepresentationToShortFormat(Value, timeProvider);

   /// <summary>
   ///   Get a string representation of the identity number.
   /// </summary>
   /// <returns>
   ///   The identity number formatted as an 11 character string.
   /// </returns>
   /// <remarks>
   ///   See <see cref="ToLongFormatValue"/> for additional details.
   /// </remarks>
   public override String ToString() => InternalRepresentationToLongFormat(Value, null);

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Swedish identity number.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Swedish identity number.
   /// </param>
   /// <returns>
   ///   A <see cref="SeIdentityNumberBase.ValidationResult"/> union that
   ///   indicates if the <paramref name="value"/> passed validation or what
   ///   validation error was encountered.
   /// </returns>
   public static ValidationResult Validate(String? value)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         return default(EmptyValue);
      }

      if (!ValidateLength(value))
      {
         return GetInvalidLengthResult(value);
      }

      // After performing basic checks, validate the check digit because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      Int32 invalidCharacterPosition;
      if (!ValidateCheckDigit(value))
      {
         invalidCharacterPosition = LocateInvalidCharacter(value);
         return invalidCharacterPosition == -1
            ? GetInvalidChecksumResult()
            : GetInvalidCharacterResult(value, invalidCharacterPosition);
      }

      if (value.Length == LongFormatLength)
      {
         // Check digit does not consider leading two digits for 13 character
         // strings. So validate here.
         invalidCharacterPosition = !value[0].IsAsciiDigit()
            ? 0
            : !value[1].IsAsciiDigit() ? 1 : -1;
         if (invalidCharacterPosition != -1)
         {
            return GetInvalidCharacterResult(value, invalidCharacterPosition);
         }
      }

      if (value[^SeparatorOffset] is not Chars.Dash and not Chars.Plus)
      {
         return GetInvalidSeparatorResult(value);
      }

      if (!ValidateDateOfBirth(value))
      {
         return GetInvalidDateOfBirthResult(value);
      }

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.SeIdentityNumberInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.SeIdentityNumberInvalidCheckDigit, Algorithms.Luhn.AlgorithmName);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.SeIdentityNumberInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(ShortFormatLength, Messages.SeIdentityNumberShortFormatLength),
            new ValidLengthDefinition(LongFormatLength, Messages.SeIdentityNumberLongFormatLength),
         ]);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(
         Messages.SeIdentityNumberInvalidDateOfBirth,
         value[..^SeparatorOffset],
         value.Length == ShortFormatLength ? DateFormatName.YYMMDD : DateFormatName.YYYYMMDD);

   private static InvalidSeparator GetInvalidSeparatorResult(ReadOnlySpan<Char> value)
      => new(
         Messages.SeIdentityNumberInvalidSeparator,
         value[^SeparatorOffset],
         value.Length - SeparatorOffset);
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class SeIdentityNumberJsonConverter : JsonConverter<SeIdentityNumber>
{
   public override SeIdentityNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new SeIdentityNumber(str);
   }

   public override void Write(Utf8JsonWriter writer, SeIdentityNumber value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.ToLongFormatValue());
}
