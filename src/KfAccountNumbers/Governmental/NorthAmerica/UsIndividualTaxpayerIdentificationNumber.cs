// Ignore Spelling: Json Kf

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Represents a United States Individual Taxpayer Identification Number (ITIN).
/// </summary>
/// <remarks>
///   <para>
///      An ITIN is a tax processing number issued by the Internal Revenue Service
///      (IRS) for individuals who are required to have a U.S. taxpayer
///      identification number but are not eligible to obtain a Social Security
///      Number (SSN).
///   </para>
///   <para>
///      A valid US Individual Taxpayer Identification Number consists of nine
///      decimal digits. The nine digits are commonly separated into three groups,
///      the first three digits represent the area number, the next two digits
///      represent the group number and the final four digits represent the serial
///      number.
///   </para>
///   <para>
///      Individual Taxpayer Identification Numbers are commonly formatted with
///      dashes ('-') and sometimes spaces separating the three groups. A
///      <see cref="UsIndividualTaxpayerIdentificationNumber"/> can be created
///      from strings that include or exclude the separator character, but if
///      used, the same character must be used to separate both the area/group
///      numbers and the group/serial numbers. The separator character may not
///      be a decimal digit (0-9).
///   </para>
///   <para>
///      Not all 9-digit numbers are valid Individual Taxpayer Identification Numbers.
///      When creating a new  <see cref="UsIndividualTaxpayerIdentificationNumber"/>,
///      after determining that a value consists of 9 decimal digits, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               An area number must start with the digit 9.
///            </description>
///         </item>
///         <item>
///            <description>
///               The group number must be in the range 50-65, 70-88, 90-92 or 94-99.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See <see href="https://en.wikipedia.org/wiki/Individual_Taxpayer_Identification_Number">Wikipedia - Individual Taxpayer Identification Number</see>
///      for more details.
///   </para>
/// </remarks>
[JsonConverter(typeof(UsIndividualTaxpayerIdentificationNumberJsonConverter))]
public record UsIndividualTaxpayerIdentificationNumber
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="UsIndividualTaxpayerIdentificationNumber"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidSeparator,
      InvalidCharacter,
      UsTinInvalidAreaNumber,
      UsTinInvalidGroupNumber)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="UsIndividualTaxpayerIdentificationNumber"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidSeparator,
      InvalidCharacter,
      UsTinInvalidAreaNumber,
      UsTinInvalidGroupNumber)
   {
   }

   private const Int32 FormattedLength = 11;
   private const Int32 UnformattedLength = 9;

   private static readonly SegmentRange _area = new(0, 3);
   private static readonly SegmentRange _formattedGroup = new(4, 6);
   private static readonly SegmentRange _unformattedGroup = new(3, 5);
   private static readonly SegmentRange _formattedSerial = new(7, 11);
   private static readonly SegmentRange _unformattedSerial = new(5, 9);

   private const Int32 GroupSeparatorOffset = 3;   // Offset of separator between area and group sections in formatted ITIN
   private const Int32 SerialSeparatorOffset = 6;  // Offset of separator between group and serial number sections in formatted ITIN

   /// <summary>
   ///   Initializes a new instance of the
   ///   <see cref="UsIndividualTaxpayerIdentificationNumber"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of an Individual Taxpayer Identification Number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> does not have length of 9 or 11.
   ///   - or -
   ///   <paramref name="value"/> contains a non-ASCII digit (not 0-9).
   ///   - or -
   ///   <paramref name="value"/> is 11 characters in length and contains
   ///   separator characters that are not identical or are ASCII digits (0-9).
   ///   - or -
   ///   <paramref name="value"/> contains an invalid area number (000-899).
   ///   - or -
   ///   <paramref name="value"/> contains an invalid group number (not in the
   ///   ranges 50-65, 70-88, 90-92 or 94-99).
   /// </exception>
   public UsIndividualTaxpayerIdentificationNumber(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the
   ///   <see cref="UsIndividualTaxpayerIdentificationNumber"/> class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private UsIndividualTaxpayerIdentificationNumber(String? value, ValidationMode validationMode)
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
               UsTinInvalidAreaNumber invalidAreaNumber => new UKfValidationException<ValidationError>(invalidAreaNumber),
               UsTinInvalidGroupNumber invalidGroupNumber => new UKfValidationException<ValidationError>(invalidGroupNumber),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetRawValue(value!);
   }

   /// <summary>
   ///   Gets the raw ITIN value.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="UsIndividualTaxpayerIdentificationNumber"/> to a <see cref="String"/>,
   ///   returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="UsIndividualTaxpayerIdentificationNumber"/> to convert.
   /// </param>
   public static implicit operator String(UsIndividualTaxpayerIdentificationNumber source)
      => source?.Value ?? String.Empty;     // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="UsIndividualTaxpayerIdentificationNumber"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of an Individual Taxpayer Identification Number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid SSN.
   /// </exception>
   public static explicit operator UsIndividualTaxpayerIdentificationNumber(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="UsIndividualTaxpayerIdentificationNumber"/>
   ///   using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of an Individual Taxpayer Identification Number.
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{UsIndividualTaxpayerIdentificationNumber, ValidationError}"/>.
   ///   Will contain the new <see cref="UsIndividualTaxpayerIdentificationNumber"/>
   ///   if <paramref name="value"/> is valid or a <see cref="ValidationError"/>
   ///   that identifies the validation rule that was failed if
   ///   <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<UsIndividualTaxpayerIdentificationNumber, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new UsIndividualTaxpayerIdentificationNumber(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         UsTinInvalidAreaNumber invalidAreaNumber => (ValidationError)invalidAreaNumber,
         UsTinInvalidGroupNumber invalidGroupNumber => (ValidationError)invalidGroupNumber,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the ITIN using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   The mask that specified the final output.
   /// </param>
   /// <returns>
   ///   A formatted Individual Taxpayer Identification Number.
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
   ///   details on creating a mask to format the ITIN.
   /// </remarks>
   public String Format(String mask = "___-__-____") => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the ITIN.
   /// </summary>
   /// <returns>
   ///   The raw ITIN, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a valid
   ///   US Individual Taxpayer Identification Number (ITIN).
   /// </summary>
   /// <param name="value">
   ///   String representation of an Individual Taxpayer Identification Number.
   /// </param>
   /// <returns>
   ///   A <see cref="ValidationResult"/> union that indicates if the
   ///   <paramref name="value"/> passed validation or what validation error was
   ///   encountered.
   /// </returns>
   public static ValidationResult Validate(String? value)
   {
      // Preliminary checks for obviously incorrect values.
      if (String.IsNullOrWhiteSpace(value))
      {
         return default(EmptyValue);
      }

      if (!ValidateLength(value))
      {
         return new InvalidLength(
            Messages.UsItinInvalidLength,
            value.Length,
            GetInvalidLengthDefinitions());
      }

      if (value[0] != Chars.DigitNine)
      {
         return new UsTinInvalidAreaNumber(
            Messages.UsItinInvalidAreaNumber,
            GetAreaNumber(value).ToString());
      }

      if (!ValidateSeparators(value, out var invalidSeparatorPosition))
      {
         return new InvalidSeparator(
            Messages.UsItinInvalidSeparator,
            value[invalidSeparatorPosition],
            invalidSeparatorPosition);
      }

      if (!ValidateAllDigits(value, out var invalidCharacterPosition))
      {
         return new InvalidCharacter(
            Messages.UsItinInvalidCharacter,
            value[invalidCharacterPosition],
            invalidCharacterPosition);
      }

      if (!ValidateGroupNumber(value))
      {
         return new UsTinInvalidGroupNumber(
            Messages.UsItinInvalidGroupNumber,
            GetGroupNumber(value).ToString());
      }

      return default(ValidValue);
   }

   /// <summary>
   ///   Gets an array of details about valid lengths accepted for a US ITIN.
   /// </summary>
   /// <returns>
   ///   An array of <see cref="ValidLengthDefinition"/>s.
   /// </returns>
   internal static ValidLengthDefinition[] GetInvalidLengthDefinitions()
      =>
      [
         new ValidLengthDefinition(UnformattedLength, Messages.UsTinUnformattedLength),
         new ValidLengthDefinition(FormattedLength, Messages.UsTinFormattedLength),
      ];

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static ReadOnlySpan<Char> GetAreaNumber(ReadOnlySpan<Char> value)
      => _area.Extract(value);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static ReadOnlySpan<Char> GetGroupNumber(ReadOnlySpan<Char> value)
      => IsFormatted(value)
         ? _formattedGroup.Extract(value)
         : _unformattedGroup.Extract(value);

   /// <summary>
   ///   Get an unformatted ITIN value from a string that has passed validation.
   ///   If the source string is formatted, then create a new string by merging
   ///   all three ITIN sections together without allocating intermediate
   ///   Strings.
   /// </summary>
   private static String GetRawValue(String value)
      => value.Length == UnformattedLength
         ? value
         : String.Concat(
            GetAreaNumber(value),
            GetGroupNumber(value),
            GetSerialNumber(value));

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static ReadOnlySpan<Char> GetSerialNumber(ReadOnlySpan<Char> value)
      => IsFormatted(value)
         ? _formattedSerial.Extract(value)
         : _unformattedSerial.Extract(value);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> value) => value.Length == FormattedLength;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Int32 ParseGroupNumber(ReadOnlySpan<Char> groupSpan)
      => ((groupSpan[0] - Chars.DigitZero) * 10) + (groupSpan[1] - Chars.DigitZero);

   private static Boolean ValidateAllDigits(
      ReadOnlySpan<Char> value,
      out Int32 invalidCharacterPosition)
   {
      invalidCharacterPosition = -1;
      var isFormatted = IsFormatted(value);
      for (var index = 1; index < value.Length; index++) // Can skip the first character since it is already validated to be '9'
      {
         if (isFormatted && index is GroupSeparatorOffset or SerialSeparatorOffset)
         {
            continue;  // Skip separator character positions in formatted ITIN
         }

         if (!value[index].IsAsciiDigit())
         {
            invalidCharacterPosition = index;
            return false;
         }
      }

      return true;
   }

   private static Boolean ValidateGroupNumber(ReadOnlySpan<Char> value)
   {
      ReadOnlySpan<Char> groupSpan = GetGroupNumber(value);
      var groupNumber = ParseGroupNumber(groupSpan);
      return groupNumber is
            (>= 50 and <= 65) or
            (>= 70 and <= 88) or
            (>= 90 and <= 92) or
            (>= 94 and <= 99);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidateLength(ReadOnlySpan<Char> value)
      => value.Length is UnformattedLength or FormattedLength;

   // A formatted ITIN must contain the same separator character at the expected
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

      var groupSeparator = value[GroupSeparatorOffset];
      if (groupSeparator.IsAsciiDigit())
      {
         invalidSeparatorPosition = GroupSeparatorOffset;
         return false;
      }
      else if (value[SerialSeparatorOffset] != groupSeparator)
      {
         invalidSeparatorPosition = SerialSeparatorOffset;
         return false;
      }

      return true;
   }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class UsIndividualTaxpayerIdentificationNumberJsonConverter : JsonConverter<UsIndividualTaxpayerIdentificationNumber>
{
   public override UsIndividualTaxpayerIdentificationNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new UsIndividualTaxpayerIdentificationNumber(str);
   }

   public override void Write(Utf8JsonWriter writer, UsIndividualTaxpayerIdentificationNumber value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
