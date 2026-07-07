// Ignore Spelling: Json Personnummer Samordningsnummer

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   <para>
///      Strongly typed business object that represents a Swedish Personal
///      Identity Number (personnummer) issued to persons born in Sweden or who
///      are residents of Sweden for 12 months or longer.
///   </para>
///   <para>
///      <b>Note:</b>See <see cref="SeSamordningsnummer"/> for a similar
///      identifier (samordningsnummer) issued to temporary residents of Sweden
///      and <see cref="SeIdentityNumber"/> for a compoosite type that can
///      represent either a personnummer or a samordningsnummer.
///   </para>
/// </summary>
/// <remarks>
///   <para>
///      Swedish personnummer values are both 11 or 13 character strings.
///      The only difference between the two lengths are the number of digits
///      used to represent the date of birth, either six or eight.
///      Personnummers are structured as YYMMDD-SSSC (or YYYYMMDD-SSSC for
///      eight-digit date of birth values) with the following elements.
///      <list type="bullet">
///         <item>
///            <term>YYMMDD</term>
///            <description>
///               The person's date of birth in YYMMDD format.
///            </description>
///         </item>
///         <item>
///            <term>YYYYMMDD</term>
///            <description>
///               The person's date of birth in YYYYMMDD format.
///            </description>
///         </item>
///         <item>
///            <term>-</term>
///            <description>
///               A separator character that separates the date of birth from
///               the remaining four digits. The separator character is
///               normally a dash ('-') but when a person turns 100 years old
///               the dash is replaced by a plus sign ('+').
///            </description>
///         </item>
///         <item>
///            <term>SSS</term>
///            <description>
///               A three digit birth serial number, issued serially as births
///               are recorded for a particular date. The last digit of the
///               birth serial number serves an additional purpose of indicating
///               the person's gender, with odd digits assigned to males and
///               even digits assigned to females.
///            </description>
///         </item>
///         <item>
///            <term>C</term>
///            <description>
///               A single check digit calculated using the Luhn algorithm
///               applied to the rightmost six digits of the date of birth and
///               to the three digits of the birth serial number.
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
///               Note that the validation specifically does <b>NOT</b> check
///               for future dates, only that the date exists.
///            </description>
///         </item>
///         <item>
///            <description>
///               Values in YYYYMMDD-SSSC format must have a year value between
///               1800 and 2049.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Personnummers are distinguished from other identity numbers by the fact
///      that the of the date of birth embeded in the value is not modified in
///      any way. In a samordningsnummer, 60 is added to the day, resulting in a
///      day between 61 and 91. Personnummers will always have a day between 01
///      and 31.
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
///      Internally, <see cref="SePersonnummer"/> stores a 12-digit
///      representation consisting of the date of birth in YYYYMMDD format
///      followed by the birth serial number and check digit (no separator). The
///      <see cref="Value"/> property returns this internal representation.
///   </para>
///   <para>
///      When comparing <see cref="SePersonnummer"/> objects for equality, the
///      internal 12-digit representation is used. This means two objects
///      representing the same person will be considered equal regardless of
///      whether they were created from 11-character or 13-character input
///      strings.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>890201-3286</term>
///            <description>
///               date of birth 890201, less than 100 years old, gender =
///               female, check digit = 6.
///            </description>
///         </item>
///         <item>
///            <term>19890201-3286</term>
///            <description>
///               date of birth 19890201, less than 100 years old, gender =
///               female, check digit = 6.
///            </description>
///         </item>
///         <item>
///            <term>811228+9874</term>
///            <description>
///               date of birth 811228, greater than 100 years old, gender =
///               male, check digit = 4.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
///      for more details.
///   </para>
/// </remarks>
[JsonConverter(typeof(SePersonnummerJsonConverter))]
public record SePersonnummer : SeIdentityNumberBase
{
   /// <summary>
   ///   Initializes a new instance of the <see cref="SePersonnummer"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a personnummer.
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
   ///   <paramref name="value"/> must have a valid date of birth between
   ///   01/01/1800 and 31/12/2049.
   /// </exception>
   /// <remarks>
   ///   The indices given in the exception description are all zero-based.
   /// </remarks>
   public SePersonnummer(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="SePersonnummer"/> class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private SePersonnummer(String? value, ValidationMode validationMode)
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
   ///   of 13 character personnummer (YYYYMMDD format) or from the date portion
   ///   of an 11 character personnummer (YYMMDD format) and the separator
   ///   character.
   /// </summary>
   /// <remarks>
   ///   <para>
   ///   See the class comments for the rules for deriving the date of birth
   ///   for an 11 character personnummer.
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
   public DateOnly DateOfBirth => GetDateOfBirth(Value);

   /// <summary>
   ///   Gets the person's gender, as indicated by the third character of the
   ///   birth sequence number. Odd digits = Male; even digits = Female.
   /// </summary>
   public Gender.BinaryGender Gender
      => Value[^GenderOffset] % 2 == 0 ? default(Gender.Female) : default(Gender.Male);   // This works because the ASCII character values for digits have the same odd/even pattern

   /// <summary>
   ///   Gets the raw personnummer value.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="SePersonnummer"/> to a <see cref="String"/>,
   ///   returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="SePersonnummer"/> to convert.
   /// </param>
   public static implicit operator String(SePersonnummer source)
      => source?.Value ?? String.Empty;     // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="SePersonnummer"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Swedish Personal Identity Number (Personnummer).
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid personnummer number.
   /// </exception>
   public static explicit operator SePersonnummer(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="SePersonnummer"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Swedish Personal Identity Number (Personnummer).
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{SePersonnummer, ValidationError}"/>. Will
   ///   contain the new <see cref="SePersonnummer"/> if <paramref name="value"/>
   ///   is valid or a <see cref="SeIdentityNumberBase.ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static CreateResult<SePersonnummer, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new SePersonnummer(value, ValidationMode.BypassValidation),
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
   ///   The personnummer formatted as a 13 character string.
   /// </returns>
   public String ToLongFormatValue(TimeProvider? timeProvider = null)
      => InternalRepresentationToLongFormat(Value, timeProvider);

   /// <summary>
   ///   Returns a string representation of the value in a short 11 character format,
   ///   combining the date of birth in YYMMDD format, a separator character, the
   ///   three digit birth serial number and the check digit.
   /// </summary>
   /// <param name="timeProvider">
   ///   Optional. <see cref="TimeProvider"/> instance used to determine the
   ///   exact age of the person. Persons 100 years or older will have a plus
   ///   ('+') as a separator; otherwise a dash ('-') is used as the separator.
   ///   If the <paramref name="timeProvider"/> is <see langword="null"/> then
   ///   the separator character will default to a dash ('-').
   /// </param>
   /// <returns>
   ///   The personnummer formatted as an 11 character string.
   /// </returns>
   public String ToShortFormatValue(TimeProvider? timeProvider = null)
      => InternalRepresentationToShortFormat(Value, timeProvider);

   /// <summary>
   ///   Get a string representation of the personnummer.
   /// </summary>
   /// <returns>
   ///   The personnummer formatted as a 13 character string.
   /// </returns>
   /// <remarks>
   ///   See <see cref="ToLongFormatValue"/> for additional details.
   /// </remarks>
   public override String ToString() => ToLongFormatValue();

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Swedish Personal Identity Number (Personnummer) value.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Swedish Personal Identity Number (Personnummer).
   /// </param>
   /// <returns>
   ///   A <see cref="SeIdentityNumberBase.ValidationResult"/> union that indicates if the
   ///   <paramref name="value"/> passed validation or what validation error was
   ///   encountered.
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

      if (!ValidateDateOfBirth(value, DateOffsetMode.Personummer))
      {
         return GetInvalidDateOfBirthResult(value);
      }

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.SePersonnummerInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.SePersonnummerInvalidCheckDigit, Algorithms.Luhn.AlgorithmName);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.SePersonnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(ShortFormatLength, Messages.SePersonnummerShortFormatLength),
            new ValidLengthDefinition(LongFormatLength, Messages.SePersonnummerLongFormatLength),
         ]);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(
         Messages.SePersonnummerInvalidDateOfBirth,
         value[..^SeparatorOffset],
         value.Length == ShortFormatLength ? DateFormatName.YYMMDD : DateFormatName.YYYYMMDD);

   private static InvalidSeparator GetInvalidSeparatorResult(ReadOnlySpan<Char> value)
      => new(
         Messages.SePersonnummerInvalidSeparator,
         value[^SeparatorOffset],
         value.Length - SeparatorOffset);
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class SePersonnummerJsonConverter : JsonConverter<SePersonnummer>
{
   public override SePersonnummer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new SePersonnummer(str);
   }

   public override void Write(Utf8JsonWriter writer, SePersonnummer value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.ToLongFormatValue());
}

internal class SePersonNumberShortFormatCheckDigitMask : ICheckDigitMask
{
   private static readonly Lazy<SePersonNumberShortFormatCheckDigitMask> _instance =
      new(() => new SePersonNumberShortFormatCheckDigitMask());

   public static SePersonNumberShortFormatCheckDigitMask Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index) => index == 6;

   public Boolean IncludeCharacter(Int32 index) => index != 6;
}

internal class SePersonNumberLongFormatCheckDigitMask : ICheckDigitMask
{
   private static readonly Lazy<SePersonNumberLongFormatCheckDigitMask> _instance =
      new(() => new SePersonNumberLongFormatCheckDigitMask());

   public static SePersonNumberLongFormatCheckDigitMask Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index)
      => index is 0 or 1 or 8;

   public Boolean IncludeCharacter(Int32 index)
      => index is not 0 and not 1 and not 8;
}
