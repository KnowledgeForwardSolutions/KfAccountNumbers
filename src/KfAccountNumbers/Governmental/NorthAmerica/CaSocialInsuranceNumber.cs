// Ignore Spelling: Json Kf Luhn

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Strongly typed business object for a CA Social Insurance Number (SIN).
/// </summary>
/// <remarks>
///   <para>
///      A valid Canadian Social Insurance Number (SIN) consists of nine decimal
///      digits, generally grouped in threes, e.g. 123-456-789.
///   </para>
///   <para>
///      The initial digit of the SIN generally indicates the province or
///      territory where the SIN was registered. However, some highly populated
///      provinces have needed to use multiple initial digits (ex. Ontario).
///   </para>
///   <para>
///      Social Insurance Numbers are commonly formatted with dashes ('-') and
///      sometimes spaces separating the three groups. A
///      <see cref="CaSocialInsuranceNumber"/> can be created from strings that
///      include or exclude the separator character, but if used, the same
///      character must be used to separate both the three groups of digits. The
///      separator character may not be a decimal digit (0-9).
///   </para>
///   <para>
///      Not all 9-digit numbers are valid Social Insurance Numbers. When
///      creating a new  <see cref="CaSocialInsuranceNumber"/>, after
///      determining that a value consists of 9 decimal digits, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The initial digit may not be 0 or 8. (8 is used for business
///               numbers assigned to business owners and corporations. 0 is
///               used for tax numbers assigned by the Canada Revenue Agency).
///            </description>
///         </item>
///         <item>
///            <description>
///               The last digit must be a valid Luhn check digit calculated
///               from the first eight digits.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See <see href="https://en.wikipedia.org/wiki/Social_Insurance_Number">Wikipedia - Social Insurance Number</see>
///      for more details.
///   </para>
/// </remarks>
[JsonConverter(typeof(CaSocialInsuranceNumberJsonConverter))]
public record CaSocialInsuranceNumber
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="CaSocialInsuranceNumber"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidSeparator,
      InvalidCharacter,
      InvalidChecksum,
      InvalidStateProvince)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="CaSocialInsuranceNumber"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidSeparator,
      InvalidCharacter,
      InvalidChecksum,
      InvalidStateProvince)
   {
   }

   private const Int32 FormattedLength = 11;
   private const Int32 UnformattedLength = 9;

   private static readonly SegmentRange _firstSection = new(0, 3);
   private static readonly SegmentRange _formattedSecondSection = new(4, 7);
   private static readonly SegmentRange _formattedThirdSection = new(8, 11);

   private const Int32 ProvinceOffset = 0;
   private const Int32 FirstSeparatorOffset = 3;
   private const Int32 SecondSeparatorOffset = 7;

   /// <summary>
   ///   Initializes a new instance of the <see cref="CaSocialInsuranceNumber"/>
   ///   class.
   /// </summary>
   /// <param name="value">
   ///   The string representation of a Social Insurance Number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> does not have length of 9 or 11.
   ///   - or -
   ///   <paramref name="value"/> contains a non-ASCII digit (not 0-9).
   ///   - or -
   ///   <paramref name="value"/> is 11 characters in length and contains an
   ///   invalid separator character.
   ///   - or -
   ///   <paramref name="value"/> fails the Luhn check digit validation.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid province code (first digit
   ///   may not be zero or eight).
   /// </exception>
   public CaSocialInsuranceNumber(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="CaSocialInsuranceNumber"/>
   ///   class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private CaSocialInsuranceNumber(String? value, ValidationMode validationMode)
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
               InvalidSeparator invalidSeparator => new UKfValidationException<ValidationError>(invalidSeparator),
               InvalidCharacter invalidCharacter => new UKfValidationException<ValidationError>(invalidCharacter),
               InvalidChecksum invalidChecksum => new UKfValidationException<ValidationError>(invalidChecksum),
               InvalidStateProvince invalidProvince => new UKfValidationException<ValidationError>(invalidProvince),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetRawValue(value!);
   }

   /// <summary>
   ///   Gets raw SIN value.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="CaSocialInsuranceNumber"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="CaSocialInsuranceNumber"/> to convert.
   /// </param>
   public static implicit operator String(CaSocialInsuranceNumber source)
      => source?.Value ?? String.Empty;     // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a
   ///   <see cref="CaSocialInsuranceNumber"/>.
   /// </summary>
   /// <param name="value">
   ///   The string representation of a Social Insurance Number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid SIN.
   /// </exception>
   public static explicit operator CaSocialInsuranceNumber(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="CaSocialInsuranceNumber"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Social Insurance Number.
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{CaSocialInsuranceNumber, ValidationError}"/>. Will
   ///   contain the new <see cref="CaSocialInsuranceNumber"/> if
   ///   <paramref name="value"/> is valid or a <see cref="ValidationError"/>
   ///   that identifies the validation rule that was failed if
   ///   <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<CaSocialInsuranceNumber, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new CaSocialInsuranceNumber(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidStateProvince invalidProvince => (ValidationError)invalidProvince,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the SIN using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   The mask that specified the final output.
   /// </param>
   /// <returns>
   ///   A formatted Social Insurance Number.
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
   ///   details on creating a mask to format the SIN.
   /// </remarks>
   public String Format(String mask = "___-___-___") => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the SIN.
   /// </summary>
   /// <returns>
   ///   The raw SIN, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a valid
   ///   Canadian Social Insurance Number (SIN).
   /// </summary>
   /// <param name="value">
   ///   String representation of a Social Insurance Number.
   /// </param>
   /// <returns>
   ///   A <see cref="ValidationResult"/> union that indicates if the
   ///   <paramref name="value"/> passed validation or what validation error was
   ///   encountered.
   /// </returns>
   public static ValidationResult Validate(String? value)
   {
      // Basic checks for empty/null and length and formatting.
      if (String.IsNullOrWhiteSpace(value))
      {
         return default(EmptyValue);
      }

      if (!ValidateLength(value))
      {
         return new InvalidLength(
            Messages.CaSinInvalidLength,
            value.Length,
            GetValidLengthDefinitions());
      }

      if (!ValidateSeparators(value, out var invalidSeparatorPosition))
      {
         return new InvalidSeparator(
            Messages.CaSinInvalidSeparator,
            value[invalidSeparatorPosition],
            invalidSeparatorPosition);
      }

      // Validate the check digit and province code.
      var validCheckDigit = IsFormatted(value)
         ? MaskedAlgorithms.Luhn.Validate(value, CaSocialInsuranceNumberMask.Instance)
         : Algorithms.Luhn.Validate(value);
      if (!validCheckDigit)
      {
         // Either invalid check digit or invalid character encountered.
         var invalidCharacterOffset = LocateInvalidCharacter(value);
         return invalidCharacterOffset == -1
            ? new InvalidChecksum(Messages.CaSinInvalidCheckDigit, Algorithms.Luhn.AlgorithmName)
            : new InvalidCharacter(
               Messages.CaSinInvalidCharacter,
               value[invalidCharacterOffset],
               invalidCharacterOffset);
      }

      if (!ValidateProvince(value))
      {
         return new InvalidStateProvince(
            Messages.CaSinInvalidProvince,
            value[ProvinceOffset].ToString());
      }

      return default(ValidValue);
   }

   /// <summary>
   ///   Gets an array of details about valid lengths accepted for a CA SIN.
   /// </summary>
   /// <returns>
   ///   An array of <see cref="ValidLengthDefinition"/>s.
   /// </returns>
   internal static ValidLengthDefinition[] GetValidLengthDefinitions()
      =>
      [
         new ValidLengthDefinition(UnformattedLength, Messages.CaSinUnformattedLength),
         new ValidLengthDefinition(FormattedLength, Messages.CaSinFormattedLength),
      ];

   /// <summary>
   ///   Get an unformatted SIN value from a string that has passed validation.
   ///   If the source string is formatted, then create a new string by merging
   ///   all three SIN sections together without allocating intermediate
   ///   Strings.
   /// </summary>
   private static String GetRawValue(String value)
      => value.Length == UnformattedLength
         ? value
         : String.Concat(
            _firstSection.Extract(value),
            _formattedSecondSection.Extract(value),
            _formattedThirdSection.Extract(value));

   private static Boolean IsFormatted(ReadOnlySpan<Char> value) => value.Length == FormattedLength;

   // Return the zero-based index of the first non-digit character (excluding
   // separators) or -1 if no non-digit characters found.
   private static Int32 LocateInvalidCharacter(ReadOnlySpan<Char> value)
   {
      var isFormatted = IsFormatted(value);
      for (var index = 0; index < value.Length; index++)
      {
         if (isFormatted && index is FirstSeparatorOffset or SecondSeparatorOffset)
         {
            continue;
         }

         if (!Char.IsAsciiDigit(value[index]))
         {
            return index;
         }
      }

      return -1;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidateLength(ReadOnlySpan<Char> value)
      => value.Length is UnformattedLength or FormattedLength;

   private static Boolean ValidateProvince(ReadOnlySpan<Char> value)
      => value[ProvinceOffset] is not Chars.DigitZero and not Chars.DigitEight;

   // A formatted SIN must contain the same separator character at the expected
   // offsets. And the separator character must be a non-digit character.
   private static Boolean ValidateSeparators(
      ReadOnlySpan<Char> value,
      out Int32 invalidSeparatorPosition)
   {
      invalidSeparatorPosition = -1;
      if (value.Length == UnformattedLength)
      {
         return true;
      }

      var groupSeparator = value[FirstSeparatorOffset];
      if (groupSeparator.IsAsciiDigit())
      {
         invalidSeparatorPosition = FirstSeparatorOffset;
         return false;
      }
      else if (value[SecondSeparatorOffset] != groupSeparator)
      {
         invalidSeparatorPosition = SecondSeparatorOffset;
         return false;
      }

      return true;
   }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class CaSocialInsuranceNumberJsonConverter : JsonConverter<CaSocialInsuranceNumber>
{
   public override CaSocialInsuranceNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new CaSocialInsuranceNumber(str);
   }

   public override void Write(Utf8JsonWriter writer, CaSocialInsuranceNumber value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}

/// <summary>
///   Mask that breaks a Canadian Social Insurance Number into groups of three
///   digits with separator characters between the groups.
/// </summary>
internal class CaSocialInsuranceNumberMask : ICheckDigitMask
{
   private static readonly Lazy<CaSocialInsuranceNumberMask> _instance =
      new(() => new CaSocialInsuranceNumberMask());

   public static CaSocialInsuranceNumberMask Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index) => index is 3 or 7;

   public Boolean IncludeCharacter(Int32 index) => index is not 3 and not 7;
}
