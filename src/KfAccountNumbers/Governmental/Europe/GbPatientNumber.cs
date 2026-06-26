#pragma warning disable IDE0250 // Make struct 'readonly'

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that can represent any of the UK public
///   health service patient identifiers, the NHS number used by the National
///   Health Service (NHS) of England, Wales and the Isle of Man, the CHI number
///   used by the Scottish Community Health Index (CHI) and the H&amp;C number
///   used by Northern Ireland Health and Care.
/// </summary>
/// <remarks>
///   <para>
///      A GB Patient Number consists of 10 digits, with a structure that varies
///      slightly, depending on the type of number. NHS numbers and H&amp;C
///      numbers are structured as NNNNNNNNNC,
///      where:
///      <list type="bullet">
///         <item>
///            <term>NNNNNNNNN</term>
///            <description>
///               A unique nine-digit number assigned by the issuing health
///               service.
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
///      CHI numbers are structured as DDMMYYNNGC, where:
///      <list type="bullet">
///         <item>
///            <term>DDMMYY</term>
///            <description>
///               The patient date of birth encoded in DDMMYY format.
///            </description>
///         </item>
///         <item>
///            <term>NNG</term>
///            <description>
///               Three digits used to differentiate between two persons born on
///               the same day. The third digit (G) also indicates the person's
///               gender, where odd numbers = male and even numbers = female.
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
///      Scottish CHI numbers are the only UK patient number that encodes
///      patient data in the number.
///   </para>
///   <para>
///      All three types (NHS, H&amp;C and CHI) can be displayed as a string of
///      10 digits or formatted for readability by including separator
///      characters. NHS numbers and H&amp;C numbers are typically formatted as
///      three groups of digits in a '3 3 4' pattern (e.g. "123 456 7890"). CHI
///      numbers are typically formatted as two groups of digits, six digits for
///      the date of birth and the four remaining digits. The optional separator
///      characters can be any character that is not an ASCII digit ('0' - '9'),
///      but NHS and H&amp;C numbers must use the same character in both
///      separator character locations. The typical separator character is a
///      space (' ').
///   </para>
///   <para>
///      Each of the public health services (NHS, Scottish CHI and Northern
///      Ireland H&amp;C) are allocated separate blocks of 10-digit numbers
///      so it is possible to determine what service issued the number by
///      comparing the number to a list of valid ranges for each service. For
///      NHS, the valid ranges are 400 000 000 to 499 999 999 and 600 000 000 to
///      799 999 999 (excluding the trailing check digit). For H&amp;C, the
///      valid ranges are 320 000 000 to 399 999 999 (excluding the trailing
///      check digit). For CHI, the valid range is 010 000 000 to 311 299 999
///      (excluding the trailing check digit). The two leading digits of the
///      valid range for CHI numbers correspond to the digits allowed by a valid
///      date in DDMMYY format (i.e. 01-31). An additional range of numbers from
///      900 000 000 to 999 999 999 are reserved for test purposes and are not
///      issued to the public.
///   </para>
///   <para>
///      GbPatientNumber includes an <see cref="IdentifierType"/> property which
///      indicates the exact type of identifier, based on the ranges listed
///      above. GbPatientNumber also includes methods to convert to the specific
///      identifier type (<see cref="ToGbNhsNumber"/>,
///      <see cref="ToGbChiNumber"/> and <see cref="ToGbHcNumber"/>).
///   </para>
///   <para>
///      When creating a new <see cref="GbPatientNumber"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 10 characters (without separators) or
///               11 characters (a CHI number with one separator) or 12
///               characters (NHS and H&amp;C numbers with 2 separators) in
///               length.
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
///               If the value is 11 characters long, character positions 6
///               (zero-based) must not be an ASCII digit ('0' - '9').
///            </description>
///         </item>
///         <item>
///            <description>
///               If the value is 12 characters long, character positions 3 and
///               7 (zero-based) must not be ASCII digits ('0' - '9'). The same
///               character must be used in each separator position.
///            </description>
///         </item>
///         <item>
///            <description>
///               The first nine digits must fall in one of the following
///               ranges: 010 000 000 to 311 299 999, 320 000 000 to 399 999 999,
///               400 000 000 to 499 999 999, 600 000 000 to 799 999 999, or
///               900 000 000 to 999 999 999.
///            </description>
///         </item>
///         <item>
///            <description>
///               The length of the input value must be appropriate for the
///               identifier type, as determined by the range that the value
///               falls into. NHS, H&amp;C and test numbers may have lengths of
///               10 or 12 characters (two separators for length 12) while CHI
///               numbers may have lengths of 10 or 11 characters (a single
///               separator for length 11).
///            </description>
///         </item>
///         <item>
///            <description>
///               For CHI number (as determined by the range that the value
///               falls into), the first six digits must represent a valid date
///               of birth in the format DDMMYY. When determining the validity
///               of the date of birth, the default century cutoff of 50 is used,
///               which results in year 00-49 = 2000-2049 and 50-99 = 1950-1999.
///               The means that year 00 is considered a leap year which allows
///               the date of birth 290200.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The Modulus 11 check digit algorithm used by the National Health
///      Service and other issuing authorities generate a check value of 10
///      which can not be encoded as a single decimal digit. NHS, H&amp;C and
///      CHI avoid this issue by not issuing any number that would result in a
///      check value of 10. This means that approximately 9.09% of all possible
///      values are never issued.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>3112999991</term>
///            <description>
///               CHI number without formatting. Date of birth December 31,
///               1999, gender = male.
///            </description>
///         </item>
///         <item>
///            <term>311299 9991</term>
///            <description>
///               The same, but formatted for readability.
///            </description>
///         </item>
///         <item>
///            <term>3200000007</term>
///            <description>
///               H&amp;C number without formatting.
///            </description>
///         </item>
///         <item>
///            <term>320 000 0007</term>
///            <description>
///               The same, but formatted for readability.
///            </description>
///         </item>
///         <item>
///            <term>4000000004</term>
///            <description>
///               NHS number without formatting.
///            </description>
///         </item>
///         <item>
///            <term>799 999 9997</term>
///            <description>
///               The same, but formatted for readability.
///            </description>
///         </item>
///         <item>
///            <term>9000000009</term>
///            <description>
///               Test number without formatting.
///            </description>
///         </item>
///         <item>
///            <term>900 000 0009</term>
///            <description>
///               The same, but formatted for readability.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/NHS_number,
///      https://www.datadictionary.nhs.uk/attributes/nhs_number.html
///      https://en.wikipedia.org/wiki/National_Health_Service_Central_Register#Community_Health_Index,
///      https://www.datadictionary.nhs.uk/attributes/community_health_index_number.html?hl=chi%2Cnumber,
///      https://www.datadictionary.nhs.uk/attributes/health_and_care_number.html?hl=number,
///      and https://webarchive.nationalarchives.gov.uk/ukgwa/20231221081503/https://digital.nhs.uk/about-nhs-digital/contact-us/freedom-of-information/freedom-of-information-disclosure-log/december-2022/nic-690159-k8h4z
///      for more info.
///   </para>
///   <para>
///      Also see <see cref="GbChiNumber"/>, <see cref="GbHcNumber"/> and
///      <see cref="GbNhsNumber"/> for associated patient identifier business
///      objects.
///   </para>
/// </remarks>
[JsonConverter(typeof(GbPatientNumberJsonConverter))]
public record GbPatientNumber : GbPatientNumberBase
{
   /// <summary>
   ///   Discriminated union defining the types of identifier that
   ///   <see cref="GbPatientNumber"/> can represent.
   /// </summary>
   public union IdentifierCategory(
      GbHealthService.Chi,
      GbHealthService.Hc,
      GbHealthService.Nhs,
      GbHealthService.Test)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="GbPatientNumber"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidGbPatientNumberRange,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="GbPatientNumber"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidGbPatientNumberRange,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   Initializes a new instance of the <see cref="GbPatientNumber"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a GB patient number, either a NHS number,
   ///   H&amp;C number, CHI number or a test number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 10 (or 11/12 if separator
   ///   characters are used).
   ///   - or -
   ///   <paramref name="value"/> contains a non-digit character in any
   ///   position other than the separator locations.
   ///   - or -
   ///   <paramref name="value"/> has invalid modulus 11 check digit character
   ///   in the trailing (right-most) character position.
   ///   - or -
   ///   <paramref name="value"/> is greater than 10 characters in length and
   ///   has an ASCII digit character ('0'-'9') in a separator location or uses
   ///   a different character in each separator location.
   ///   - or -
   ///   The first nine digits of <paramref name="value"/> are not in one of the
   ///   valid ranges for a CHI number (010 000 000 to 311 299 999), H&amp;C
   ///   number (320 000 000 to 399 999 999), NHS number (400 000 000 to
   ///   499 999 999, 600 000 000 to 799 999 999), or test numbers 900 000 000
   ///   to 999 999 999.
   ///   - or -
   ///   <paramref name="value"/> is greater than 10 characters in length and
   ///   the length does not match the expected length as determined by the
   ///   range that the number falls into (11 for CHI numbers, 12 for other than
   ///   CHI numbers).
   ///   - or -
   ///   <paramref name="value"/> is a CHI number (as determined by the range
   ///   that the number falls into) and the first six digits are not a valid
   ///   date in DDMMYY format.
   /// </exception>
   public GbPatientNumber(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="GbPatientNumber"/> class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private GbPatientNumber(String? value, ValidationMode validationMode)
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
               InvalidGbPatientNumberRange invalidRange => new UKfValidationException<ValidationError>(invalidRange),
               InvalidDateOfBirth invalidDateOfBirth => new UKfValidationException<ValidationError>(invalidDateOfBirth),
               _ => new UnreachableException("This branch should never be reached"),
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
         IdentifierRangeCategory.Chi => default(GbHealthService.Chi),
         IdentifierRangeCategory.Hc => default(GbHealthService.Hc),
         IdentifierRangeCategory.Nhs => default(GbHealthService.Nhs),
         IdentifierRangeCategory.Test => default(GbHealthService.Test),
         IdentifierRangeCategory.Invalid => throw new UnreachableException("Validation should ensure that this branch is never taken"),
         _ => throw new UnreachableException("Validation should ensure that this branch is never taken"),
      };
   }

   /// <summary>
   ///   Gets the patient number, without any formatting.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="GbPatientNumber"/> to a <see cref="String"/>,
   ///   returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="GbPatientNumber"/> to convert.
   /// </param>
   public static implicit operator String(GbPatientNumber source)
      => source?.Value ?? String.Empty;         // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="GbPatientNumber"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a GB patient number, either a NHS number,
   ///   H&amp;C number, CHI number or a test number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid GB patient number.
   /// </exception>
   public static explicit operator GbPatientNumber(String value) => new(value);

   /// <summary>
   ///   Create a new <see cref="GbPatientNumber"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a GB patient number, either a NHS number,
   ///   H&amp;C number, CHI number or a test number.
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{GbPatientNumber, ValidationError}"/>. Will
   ///   contain the new <see cref="GbPatientNumber"/> if <paramref name="value"/>
   ///   is valid or a <see cref="ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<GbPatientNumber, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new GbPatientNumber(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         InvalidGbPatientNumberRange invalidRange => (ValidationError)invalidRange,
         InvalidDateOfBirth invalidDateOfBirth => (ValidationError)invalidDateOfBirth,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the patient number using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then a default mask chosen by the <see cref="IdentifierType"/> will be
   ///   used instead.
   /// </param>
   /// <returns>
   ///   A formatted patient number.
   /// </returns>
   /// <exception cref="ArgumentException">
   ///   <paramref name="mask"/> is <see cref="String.Empty"/> or all whitespace
   ///   characters.
   /// </exception>
   /// <remarks>
   ///   <see cref="ExtensionMethods.FormatWithMask(String, String)"/> for more
   ///   details on creating a mask to format the NHS number.
   /// </remarks>
   public String Format(String? mask = null)
   {
      mask ??= IdentifierType is GbHealthService.Chi
         ? DefaultChiFormatMask
         : DefaultNhsFormatMask;

      return Value.FormatWithMask(mask);
   }

   /// <summary>
   ///   Convert this instance to a <see cref="GbChiNumber"/>.
   /// </summary>
   /// <returns>
   ///   An <see cref="KfOption{GbChiNumber}"/> instance that will contain the
   ///   <see cref="GbChiNumber"/> if this patient number is a CHI number;
   ///   otherwise <see cref="None"/> to indicate that this is not a CHI number.
   /// </returns>
   public KfOption<GbChiNumber> ToGbChiNumber()
      => IdentifierType is GbHealthService.Chi
      ? new GbChiNumber(Value, ValidationMode.BypassValidation)
      : default(None);

   /// <summary>
   ///   Convert this instance to a <see cref="GbHcNumber"/>.
   /// </summary>
   /// <returns>
   ///   An <see cref="KfOption{GbHcNumber}"/> instance that will contain the
   ///   <see cref="GbHcNumber"/> if this patient number is a H&amp;C number;
   ///   otherwise <see cref="None"/> to indicate that this is not a H&amp;C
   ///   number or test number.
   /// </returns>
   /// <remarks>
   ///   <see cref="GbHcNumber"/> allows numbers from the test block so test
   ///   numbers can be converted to <see cref="GbHcNumber"/>.
   /// </remarks>
   public KfOption<GbHcNumber> ToGbHcNumber()
      => IdentifierType is GbHealthService.Hc or GbHealthService.Test
         ? new GbHcNumber(Value, ValidationMode.BypassValidation)
         : default(None);

   /// <summary>
   ///   Convert this instance to a <see cref="GbNhsNumber"/>.
   /// </summary>
   /// <returns>
   ///   An <see cref="KfOption{GbNhsNumber}"/> instance that will contain the
   ///   <see cref="GbNhsNumber"/> if this patient number is a NHS number;
   ///   otherwise <see cref="None"/> to indicate that this is not a NHS number
   ///   or test number.
   /// </returns>
   /// <remarks>
   ///   <see cref="GbNhsNumber"/> allows numbers from the test block so test
   ///   numbers can be converted to <see cref="GbNhsNumber"/>.
   /// </remarks>
   public KfOption<GbNhsNumber> ToGbNhsNumber()
      => IdentifierType is GbHealthService.Nhs or GbHealthService.Test
         ? new GbNhsNumber(Value, ValidationMode.BypassValidation)
         : default(None);

   /// <summary>
   ///   Get a string representation of the patient number.
   /// </summary>
   /// <returns>
   ///   The raw patient number, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a valid
   ///   UK public health service patient number.
   /// </summary>
   /// <param name="value">
   ///   String representation of a UK public health service patient number.
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
         return GetInvalidLengthResult(value);
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

      IdentifierRangeCategory identifierCategory = GetIdentifierCategory(value);
      if (identifierCategory is IdentifierRangeCategory.Invalid)
      {
         return new InvalidGbPatientNumberRange(Messages.GbPatientNumberInvalidRange);
      }

      // Secondary length validation for formatted values. Formatted length must
      // match the length allowed for the specific category.
      if ((value.Length == ChiFormattedLength && identifierCategory is not IdentifierRangeCategory.Chi)
         || (value.Length == NhsFormattedLength && identifierCategory is IdentifierRangeCategory.Chi))
      {
         return GetInvalidLengthResult(value);
      }

#pragma warning disable IDE0046 // Convert to conditional expression
      if (identifierCategory is IdentifierRangeCategory.Chi && !ValidateChiNumberDateOfBirth(value))
      {
         return GetInvalidDateOfBirthResult(value, Messages.GbChiNumberInvalidDateOfBirth);
      }
#pragma warning restore IDE0046 // Convert to conditional expression

      return default(ValidValue);
   }

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(Messages.GbPatientNumberInvalidLength, value.Length, GetAllValidLengthDefinitions());

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidateLength(ReadOnlySpan<Char> value)
      => value.Length is UnformattedLength or ChiFormattedLength or NhsFormattedLength;
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class GbPatientNumberJsonConverter : JsonConverter<GbPatientNumber>
{
   public override GbPatientNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new GbPatientNumber(str);
   }

   public override void Write(Utf8JsonWriter writer, GbPatientNumber value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
