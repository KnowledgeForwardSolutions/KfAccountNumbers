#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.National.Europe;

/// <summary>
///   <para>
///      Strongly typed business object that represents a Fh-nummer (Felles
///      Hjelpenummer or Common Help Number). A Fh-nummer is similar to a
///      Norwegian H-nummer, an identifier issued to persons needing medical
///      assistance and who do not have a fødselsnummer or a D-nummer such as
///      tourists, newborns or persons with unknown identities.
///   </para>
///   <para>
///      Unlike H-nummers which are issued by a single organization and only
///      unique within that organization, Fh-nummers are issued by Norsk
///      Helsenett (the Norwegian Health Network) and are unique across the
///      entire Norwegian health system. Unlike other Norwegian identity numbers
///      (fødselsnummer, D-nummer and H-nummer), Fh-nummers do not encode
///      the person's date of birth or gender and consist of 9 random digits and
///      two check digits. Fh-nummers are distinguished by an initial digit = 8
///      or 9.
///   </para>
///   <para>
///      <b>Note:</b> See <see cref="NoFoedselsnummer"/> and
///      <see cref="NoDnummer"/> for a similar identifiers (fødselsnummer,
///      D-nummer) and <see cref="NoIdentityNumber"/> for a composite type that
///      can represent either a fødselsnummer, D-nummer or a H-nummer.
///   </para>
/// </summary>
/// <remarks>
///   <para>
///      A Fh-nummer is an 11-digit number structured as NNNNNNNNNCC, with the
///      following elements:
///      <list type="bullet">
///         <item>
///            <term>NNNNNNNNN</term>
///            <description>
///               9 random digits. The first digit must be 8 or 9 to distinguish
///               the value from other identifiers like fødselsnummer, etc.
///            </description>
///         </item>
///         <item>
///            <term>CC</term>
///            <description>
///               Two separate check digits calculated using a weighted
///               modulus 11 algorithm. The first check digit is calculated
///               for the first nine digits and the second check digit is
///               calculated for the first ten digits, includding the first
///               check digit.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The 11 character value is sometimes formatted for greater readability
///      by inserting a separator character, generally a space, between the date
///      of birth and the individual number, i.e. NNNNNN NNNCC.
///   </para>
///   <para>
///      When creating a new <see cref="NoFhnummer"/>, the following validation
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
///               If the value has length 12, then the character at position 6
///               (zero-based) must not be an ASCII digit ('0'-'9')
///            </description>
///         </item>
///         <item>
///            <description>
///               The leading digit must be 8 or 9.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>98075450605</term>
///            <description>
///               unformatted, heck digits = 05
///            </description>
///         </item>
///         <item>
///            <term>87207009367</term>
///            <description>
///               unformatted, check digits = 67
///            </description>
///         </item>
///         <item>
///            <term>809390 27371</term>
///            <description>
///               formatted, check digits = 71
///            </description>
///         </item>
///      </list>
///   </para>
/// </remarks>
[JsonConverter(typeof(NoFhnummerJsonConverter))]
public record NoFhnummer : NoIdentityNumberBase
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new Norwegian Fh-nummer.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidPrefix)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating Norwegian Fh-nummers.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidPrefix)
   {
   }

   /// <summary>
   ///   Initializes a new instance of the <see cref="NoFhnummer"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Fh-nummer.
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
   ///   <paramref name="value"/> starts with a digit other than 8 or 9.
   /// </exception>
   public NoFhnummer(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="NoFhnummer"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Fh-nummer.
   /// </param>
   /// <param name="validationMode">
   ///   Indicates whether the <paramref name="value"/> requires validation.
   /// </param>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   internal NoFhnummer(String? value, ValidationMode validationMode)
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
               InvalidPrefix invalidPrefix => new UKfValidationException<ValidationError>(invalidPrefix),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetNormalizedValue(value!);
   }

   /// <summary>
   ///   Gets a string representation of the H-nummer.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="NoFhnummer"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="NoFhnummer"/> to convert.
   /// </param>
   public static implicit operator String(NoFhnummer source)
      => source?.Value ?? String.Empty;     // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="NoFhnummer"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Norwegian Fh-nummer.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid Fh-nummer.
   /// </exception>
   public static explicit operator NoFhnummer(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="NoFhnummer"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Norwegian Fh-nummer.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{NoFhnummer, ValidationError}"/>. Will
   ///   contain the new <see cref="NoFhnummer"/> if <paramref name="value"/>
   ///   is valid or a <see cref="ValidationError"/> that
   ///   identifies the validation rule that was failed if
   ///   <paramref name="value"/> is invalid.
   /// </returns>
   public static CreateResult<NoFhnummer, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new NoFhnummer(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         InvalidPrefix invalidPrefix => (ValidationError)invalidPrefix,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the Fh-nummer using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then the default mask
   ///   <see cref="NoIdentityNumberBase.DefaultFormatMask"/> will be used
   ///   instead.
   /// </param>
   /// <returns>
   ///   A formatted Fh-nummer.
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
   ///   details on creating a mask to format the Fh-nummer.
   /// </remarks>
   public String Format(String mask = DefaultFormatMask) => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the Fh-nummer.
   /// </summary>
   /// <returns>
   ///   The raw Fh-nummer, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Norwegian H-nummer.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Norwegian H-nummer.
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

      if (value[0] is not Chars.DigitEight and not Chars.DigitNine)
      {
         return GetInvalidPrefixResult(value);
      }

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.NoFhnummerInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.NoFhnummerInvalidCheckDigits, CheckDigitAlgorithmName);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.NoFhnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(UnformattedLength, Messages.NoFhnummerUnformattedLength),
            new ValidLengthDefinition(FormattedLength, Messages.NoFhnummerFormattedLength),
         ]);

   private static InvalidPrefix GetInvalidPrefixResult(String value)
      => new(
         Messages.NoFhnummerInvalidPrefix,
         value[0].ToString());

   private static InvalidSeparator GetInvalidSeparatorResult(ReadOnlySpan<Char> value)
      => new(
         Messages.NoFhnummerInvalidSeparator,
         value[SeparatorOffset],
         SeparatorOffset);
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class NoFhnummerJsonConverter : JsonConverter<NoFhnummer>
{
   public override NoFhnummer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new NoFhnummer(str);
   }

   public override void Write(Utf8JsonWriter writer, NoFhnummer value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
