namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents the identifier used by the
///   National Health Service (NHS) of England, Wales and the Isle of Man.
/// </summary>
/// <remarks>
///   <para>
///      A NHS Number consists of 10 digits, structured as NNNNNNNNNC, where:
///      <list type="bullet">
///         <item>
///            <term>NNNNNNNNN</term>
///            <description>
///               A unique nine-digit number assigned by the NHS.
///            </description>
///         </item>
///         <item>
///            <term>C</term>
///            <description>
///               A modulus 11 check digit calculated from the preceding nine
///               digits.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      NHS Numbers can be displayed as a string of 10 digits or formatted for
///      readability as three groups of digits in a '3 3 4' pattern
///      (e.g. "123 456 7890"). The optional separator characters can be any
///      character that is not an ASCII digit ('0' - '9'), but both separator
///      characters must be the same. The typical separator character is a space (' ').
///   </para>
///   <para>
///      Each of the public health services in Great Britain (NHS, Scottish CHI
///      and Northern Ireland H&amp;C) are allocated separate blocks of 10-digit
///      numbers so it is possible to determine what service issued the number by
///      comparing the number to a list of valid ranges for each service. For NHS,
///      the valid ranges are 400 000 000 to 499 999 999 and 600 000 000 to
///      799 999 999 (excluding the trailing check digit). <see cref="GbNhsNumber"/>
///      also allows a range of numbers from 900 000 000 to 999 999 999 which are
///      reserved for test purposes and not issued to the public.
///   </para>
///   <para>
///      When creating a new <see cref="GbNhsNumber"/>, the following validation
///      rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 10 characters (without separators) or
///               12 characters (with separators) in length.
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
///               The trailing (right-most) digit must be a valid Modulus 11
///               check digit.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the value is 12 characters long, character positions 3 and 7
///               (zero-based) must not be ASCII digits ('0' - '9'). The same
///               character must be used in each separator position.
///            </description>
///         </item>
///         <item>
///            <description>
///               The first nine digits must fall in one of the following ranges:
///               400 000 000 to 499 999 999, 600 000 000 to 799 999 999, or
///               900 000 000 to 999 999 999.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The Modulus 11 check digit algorithm used by NHS numbers can generate a
///      check value of 10 which can not be encoded as a single decimal digit.
///      The National Health Service and other issuing authorities avoid this
///      issue by not issuing any number that would result in a check value of
///      10. This means that approximately 9.09% of all possible values are never
///      issued.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>4000000004</term>
///            <description>
///               Standard NHS number without formatting.
///            </description>
///         </item>
///         <item>
///            <term>799 999 9997</term>
///            <description>
///               Standard NHS number, formatted for readability.
///            </description>
///         </item>
///         <item>
///            <term>9000000009</term>
///            <description>
///               Test number without formatting.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/NHS_number,
///      https://www.datadictionary.nhs.uk/attributes/nhs_number.html
///      and https://webarchive.nationalarchives.gov.uk/ukgwa/20231221081503/https://digital.nhs.uk/about-nhs-digital/contact-us/freedom-of-information/freedom-of-information-disclosure-log/december-2022/nic-690159-k8h4z
///      for more info.
///   </para>
///   <para>
///      Also see <see cref="GbChiNumber"/>, <see cref="GbHcNumber"/> and
///      <see cref="GbPatientNumber"/> for associated patient identifier business
///      objects.
///   </para>
/// </remarks>
[JsonConverter(typeof(GbNhsNumberJsonConverter))]
public record GbNhsNumber : GbPatientNumberBase
{
   /// <summary>
   ///   Discriminated union defining the types of identifier that
   ///   <see cref="GbNhsNumber"/> can represent. Either a NHS number or a test
   ///   number.
   /// </summary>
   public union IdentifierCategory(GbHealthService.Nhs, GbHealthService.Test) { }

   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="GbNhsNumber"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      GbPatientNumberInvalidRange)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="GbNhsNumber"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      GbPatientNumberInvalidRange)
   {
   }

   /// <summary>
   ///   Initializes a new instance of the <see cref="GbNhsNumber"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a UK National Health Service number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 10 (or 12 if separator
   ///   characters are used).
   ///   - or -
   ///   <paramref name="value"/> contains a non-digit character in any
   ///   position other than the separator locations.
   ///   - or -
   ///   <paramref name="value"/> has invalid modulus 11 check digit character
   ///   in the trailing (right-most) character position.
   ///   - or -
   ///   <paramref name="value"/> is 12 characters in length and has an ASCII
   ///   digit character ('0'-'9') in a separator location or uses a different
   ///   character in each separator location.
   ///   - or -
   ///   The first nine digits of <paramref name="value"/> are not in one of the
   ///   valid ranges for a NHS number (400 000 000 to 499 999 999, 600 000 000
   ///   to 799 999 999, or 900 000 000 to 999 999 999).
   /// </exception>
   public GbNhsNumber(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="GbNhsNumber"/> class.
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has already
   ///   been validated.
   /// </summary>
   private GbNhsNumber(String? value, ValidationMode validationMode)
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
               GbPatientNumberInvalidRange invalidRange => new UKfValidationException<ValidationError>(invalidRange),
               _ => new SwitchExpressionException("This branch should never be reached"),
            };
         }
      }

      Value = GetRawValue(value!);
   }

   /// <summary>
   ///   Gets the specific type of identifier that this instance represents.
   /// </summary>
   public IdentifierCategory IdentifierType
   {
      get => GetIdentifierCategory(Value) switch
      {
         IdentifierRangeCategory.Nhs => default(GbHealthService.Nhs),
         IdentifierRangeCategory.Test => default(GbHealthService.Test),
         _ => throw new SwitchExpressionException("Validation should ensure that this branch is never taken"),
      };
   }

   /// <summary>
   ///   Gets the NHS number, without any formatting.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="GbNhsNumber"/> to a <see cref="String"/>,
   ///   returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="GbNhsNumber"/> to convert.
   /// </param>
   public static implicit operator String(GbNhsNumber source)
      => source?.Value ?? String.Empty;         // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="GbNhsNumber"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a UK National Health Service number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid NHS number.
   /// </exception>
   public static explicit operator GbNhsNumber(String value) => new(value);

   /// <summary>
   ///   Create a new <see cref="GbNhsNumber"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a UK National Health Service number.
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{GbNhsNumber, ValidationError}"/>. Will
   ///   contain the new <see cref="GbNhsNumber"/> if <paramref name="value"/>
   ///   is valid or a <see cref="ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<GbNhsNumber, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new GbNhsNumber(value!, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         GbPatientNumberInvalidRange invalidRange => (ValidationError)invalidRange,
         _ => throw new SwitchExpressionException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the NHS number using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then the default mask "___ ___ ____" will be used instead.
   /// </param>
   /// <returns>
   ///   A formatted UK National Health Service number.
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
   ///   details on creating a mask to format the NHS number.
   /// </remarks>
   public String Format(String mask = DefaultFormatMask) => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the NHS number.
   /// </summary>
   /// <returns>
   ///   The raw NHS number, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a valid
   ///   NHS number.
   /// </summary>
   /// <param name="value">
   ///   String representation of a NHS number.
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

      if (!ValidateLength(value))
      {
         return GetInvalidLengthResult(value.Length);
      }

      if (!ValidateCheckDigit(value, out var invalidCharacterPosition))
      {
         return invalidCharacterPosition == -1
            ? GetInvalidChecksumResult()
            : GetInvalidCharacterResult(value, invalidCharacterPosition);
      }

      if (!ValidateSeparators(value, out var invalidSeparatorPosition))
      {
         return GetInvalidSeparatorResult(value, invalidSeparatorPosition);
      }

      return GetIdentifierCategory(value) is not IdentifierRangeCategory.Nhs and not IdentifierRangeCategory.Test
         ? new GbPatientNumberInvalidRange(Messages.GbNhsNumberInvalidRange)
         : default(ValidValue);
   }
}

/// <summary>
///   Support serialization and deserialization of <see cref="GbNhsNumber"/>.
/// </summary>
public class GbNhsNumberJsonConverter : JsonConverter<GbNhsNumber>
{
   /// <summary>
   ///   Reads and converts the JSON to <see cref="GbNhsNumber"/>.
   /// </summary>
   /// <param name="reader">
   ///   The reader.
   /// </param>
   /// <param name="typeToConvert">
   ///   The type to convert.
   /// </param>
   /// <param name="options">
   ///   An object that specifies serialization options to use.
   /// </param>
   /// <returns>
   ///   The converted value.
   /// </returns>
   public override GbNhsNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new GbNhsNumber(str);
   }

   /// <summary>
   ///   Write a specified value as JSON.
   /// </summary>
   /// <param name="writer">
   ///   The writer to write to.
   /// </param>
   /// <param name="value">
   ///   The value to convert to JSON.
   /// </param>
   /// <param name="options">
   ///   An object that specifies serialization options to use.
   /// </param>
   public override void Write(Utf8JsonWriter writer, GbNhsNumber value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
