#pragma warning disable IDE0250 // Make struct 'readonly'

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents the identifier used by the
///   Scottish Community Health Index (CHI).
/// </summary>
/// <remarks>
///   <para>
///      A CHI Number consists of 10 digits, structured as DDMMYYNNGC, where:
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
///   </para>
///   <para>
///      CHI Numbers can be displayed as a string of 10 digits or formatted for
///      readability as three groups of digits in a '3 3 4' pattern
///      (e.g. "123 456 7890"). The optional separator characters can be any
///      character that is not an ASCII digit ('0' - '9'), but both separator
///      characters must be the same. The typical separator character is a space (' ').
///   </para>
///   <para>
///      Each of the public health services in the United Kingdom (NHS, Scottish CHI
///      and Northern Ireland H&amp;C) are allocated separate blocks of 10-digit
///      numbers so it is possible to determine what service issued the number by
///      comparing the number to a list of valid ranges for each service. For CHI,
///      the valid range is 010 000 000 to 311 299 999 (excluding the trailing check
///      digit). Unlike <see cref="GbChiNumber"/> and <see cref="GbHcNumber"/>,
///      <see cref="GbChiNumber"/> does not allow test numbers in the range of
///      900 000 000 to 999 999 999 because those numbers would not contain a valid
///      date of birth.
///   </para>
///   <para>
///      When creating a new <see cref="GbChiNumber"/>, the following validation
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
///               The first nine digits must fall in the following range:
///               010 000 000 to 311 299 999.
///            </description>
///         </item>
///         <item>
///            <description>
///               The first six digits must represent a valid date in DDMMYY format.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The Modulus 11 check digit algorithm used by CHI numbers can generate a
///      check value of 10 which can not be encoded as a single decimal digit.
///      Community Health Index and other issuing authorities avoid this
///      issue by not issuing any number that would result in a check value of
///      10. This means that approximately 9.09% of all possible values are never
///      issued.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>3112999991</term>
///            <description>
///               CHI number without formatting. Date of birth December 31, 1999,
///               gender = male.
///            </description>
///         </item>
///         <item>
///            <term>311 299 9991</term>
///            <description>
///               The same, but with format characters.
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
///      Also see <see cref="GbHcNumber"/>, <see cref="GbChiNumber"/> and
///      <see cref="GbPatientNumber"/> for associated patient identifier business
///      objects.
///   </para>
/// </remarks>
public record class GbChiNumber : GbPatientNumberBase
{

   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="GbChiNumber"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      GbPatientNumberInvalidRange,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="GbChiNumber"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      GbPatientNumberInvalidRange,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   Initializes a new instance of the <see cref="GbChiNumber"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Scottish CHI number.
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
   ///   The first nine digits of <paramref name="value"/> are not in the range
   ///   of 010 000 000 to 311 299 999.
   ///   - or -
   ///   The first six digits of <paramref name="value"/> do not represent a
   ///   valid date in DDMMYY format.
   /// </exception>
   public GbChiNumber(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="GbChiNumber"/> class.
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has already
   ///   been validated.
   /// </summary>
   private GbChiNumber(String? value, ValidationMode validationMode)
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
               InvalidDateOfBirth invalidDateOfBirth => new UKfValidationException<ValidationError>(invalidDateOfBirth),
               _ => new SwitchExpressionException("This branch should never be reached"),
            };
         }
      }

      Value = GetRawValue(value!);
   }

   /// <summary>
   ///   Gets the binary gender extracted from the CHI number.
   /// </summary>
   public Gender.BinaryGender Gender
      => Value[GenderOffset] % 2 == 0 ? default(Gender.Female) : default(Gender.Male);    // This works because the ASCII character values for digits have the same odd/even pattern

   /// <summary>
   ///   Gets the CHI number, without any formatting.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="GbChiNumber"/> to a <see cref="String"/>,
   ///   returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="GbChiNumber"/> to convert.
   /// </param>
   public static implicit operator String(GbChiNumber source)
      => source?.Value ?? String.Empty;         // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="GbChiNumber"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Scottish CHI number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid CHI number.
   /// </exception>
   public static explicit operator GbChiNumber(String value) => new(value);

   /// <summary>
   ///   Create a new <see cref="GbChiNumber"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Scottish CHI number.
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{GbChiNumber, ValidationError}"/>. Will
   ///   contain the new <see cref="GbChiNumber"/> if <paramref name="value"/>
   ///   is valid or a <see cref="ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<GbChiNumber, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new GbChiNumber(value!, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         GbPatientNumberInvalidRange invalidRange => (ValidationError)invalidRange,
         InvalidDateOfBirth invalidDateOfBirth => (ValidationError)invalidDateOfBirth,
         _ => throw new SwitchExpressionException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the CHI number using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then the default mask "___ ___ ____" will be used instead.
   /// </param>
   /// <returns>
   ///   A formatted Scottish CHI number.
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
   ///   details on creating a mask to format the CHI number.
   /// </remarks>
   public String Format(String mask = DefaultFormatMask) => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the CHI number.
   /// </summary>
   /// <returns>
   ///   The raw CHI number, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a valid
   ///   CHI number.
   /// </summary>
   /// <param name="value">
   ///   String representation of a CHI number.
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

      if (GetIdentifierCategory(value) is not IdentifierRangeCategory.Chi)
      {
         return new GbPatientNumberInvalidRange(Messages.GbChiNumberInvalidRange);
      }

      return ValidateChiNumberDateOfBirth(value)
         ? default(ValidValue)
         : GetInvalidDateOfBirthResult(value, Messages.GbChiNumberInvalidDateOfBirth);
   }
}
