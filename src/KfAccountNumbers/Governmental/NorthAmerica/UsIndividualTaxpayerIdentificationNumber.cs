// Ignore Spelling: itin Json Kf

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
   private const Int32 FormattedLength = 11;
   private const Int32 UnformattedLength = 9;

   private const Int32 AreaRangeEnd = 3;                          // End indices are exclusive for use in range operator
   private const Int32 UnformattedGroupRangeStart = 3;
   private const Int32 UnformattedGroupRangeEnd = 5;
   private const Int32 UnformattedSerialNumberRangeStart = 5;
   private const Int32 FormattedGroupRangeStart = 4;
   private const Int32 FormattedGroupRangeEnd = 6;
   private const Int32 FormattedSerialNumberRangeStart = 7;

   private const Int32 GroupSeparatorOffset = 3;   // Offset of separator between area and group sections in formatted ITIN
   private const Int32 SerialSeparatorOffset = 6;  // Offset of separator between group and serial number sections in formatted ITIN

   /// <summary>
   ///   Initializes a new instance of the
   ///   <see cref="UsIndividualTaxpayerIdentificationNumber"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of an Individual Taxpayer Identification Number.
   /// </param>
   /// <exception cref="KfValidationException{UsIndividualTaxpayerIdentificationNumberValidationResult}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> does not have length of 9 or 11.
   ///   - or -
   ///   <paramref name="value"/> contains a non-ASCII digit (not 0-9).
   ///   - or -
   ///   <paramref name="value"/> is 11 characters in length and contains and 
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
   ///   <see cref="UsIndividualTaxpayerIdentificationNumber"/> class. Private
   ///   constructor that actually does the work. Supports bypassing validation
   ///   when creating a new instance from a value that has already been
   ///   validated.
   /// </summary>
   private UsIndividualTaxpayerIdentificationNumber(String? value, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         UsIndividualTaxpayerIdentificationNumberValidationResult validationResult = Validate(value);
         if (validationResult != UsIndividualTaxpayerIdentificationNumberValidationResult.ValidationPassed)
         {
            throw validationResult.ToValidationException();
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
   /// <param name="itin">
   ///   String representation of an Individual Taxpayer Identification Number.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{UsIndividualTaxpayerIdentificationNumber, UsIndividualTaxpayerIdentificationNumberValidationResult}"/>.
   ///   Will contain the new <see cref="UsIndividualTaxpayerIdentificationNumber"/> if
   ///   <paramref name="itin"/> is valid or
   ///   <see cref="UsIndividualTaxpayerIdentificationNumberValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="itin"/> is
   ///   invalid.
   /// </returns>
   public static CreateResult<UsIndividualTaxpayerIdentificationNumber, UsIndividualTaxpayerIdentificationNumberValidationResult> Create(String? itin)
      => Validate(itin) switch
      {
         UsIndividualTaxpayerIdentificationNumberValidationResult.ValidationPassed
            => new UsIndividualTaxpayerIdentificationNumber(itin, validationMode: ValidationMode.BypassValidation),
         var validationFailure => validationFailure,
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
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a valid
   ///   US Individual Taxpayer Identification Number (ITIN).
   /// </summary>
   /// <param name="value">
   ///   String representation of a Social Security Number.
   /// </param>
   /// <returns>
   ///   A <see cref="UsIndividualTaxpayerIdentificationNumberValidationResult"/> enumeration
   ///   value that indicates if the <paramref name="value"/> passed validation
   ///   or what validation error was encountered.
   /// </returns>
   public static UsIndividualTaxpayerIdentificationNumberValidationResult Validate(String? value)
   {
      // Preliminary checks for obviously incorrect values.
      if (String.IsNullOrWhiteSpace(value))
      {
         return UsIndividualTaxpayerIdentificationNumberValidationResult.Empty;
      }

      if (!ValidateLength(value))
      {
         return UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidLength;
      }

      if (value[0] != Chars.DigitNine)
      {
         return UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidAreaNumber;
      }

      if (IsFormatted(value) && !ValidateEmbeddedSeparatorCharacters(value))
      {
         return UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidSeparatorEncountered;
      }

      if (!ValidateAllDigits(value))
      {
         return UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidCharacterEncountered;
      }

      if (!ValidateGroupNumber(value))
      {
         return UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidGroupNumber;
      }

      return UsIndividualTaxpayerIdentificationNumberValidationResult.ValidationPassed;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static ReadOnlySpan<Char> GetAreaNumber(ReadOnlySpan<Char> value)
      => value[..AreaRangeEnd];

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static ReadOnlySpan<Char> GetGroupNumber(ReadOnlySpan<Char> value)
      => IsFormatted(value)
         ? value[FormattedGroupRangeStart..FormattedGroupRangeEnd]
         : value[UnformattedGroupRangeStart..UnformattedGroupRangeEnd];

   /// <summary>
   ///   Get an unformatted ITIN value from a string that has passed validation.
   ///   If the source string is formatted, then create a new string by merging
   ///   all three ITIN sections together without allocating intermediate
   ///   Strings.
   /// </summary>
   private static String GetRawValue(String value)
   {
      if (value.Length == UnformattedLength)
      {
         return value;
      }

      return String.Concat(
         GetAreaNumber(value),
         GetGroupNumber(value),
         GetSerialNumber(value));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static ReadOnlySpan<Char> GetSerialNumber(ReadOnlySpan<Char> value)
      => IsFormatted(value)
         ? value[FormattedSerialNumberRangeStart..]
         : value[UnformattedSerialNumberRangeStart..];

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> value) => value.Length == FormattedLength;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Int32 ParseGroupNumber(ReadOnlySpan<Char> groupSpan)
      => ((groupSpan[0] - Chars.DigitZero) * 10) + (groupSpan[1] - Chars.DigitZero);

   private static Boolean ValidateAllDigits(ReadOnlySpan<Char> value)
   {
      var length = value.Length;
      for (var index = 1; index < value.Length; index++)     // Can skip the first character since it is already validated to be '9'
      {
         if (length == FormattedLength && (index is GroupSeparatorOffset or SerialSeparatorOffset))
         {
            continue;  // Skip separator character positions in formatted ITIN
         }

         if (!value[index].IsAsciiDigit())
         {
            return false;
         }
      }

      return true;
   }

   // A formatted ITIN must contain the same separator character at the expected
   // offsets. And the separator character must be a non-digit character.
   private static Boolean ValidateEmbeddedSeparatorCharacters(ReadOnlySpan<Char> value)
   {
      var groupSeparator = value[GroupSeparatorOffset];
      var serialSeparator = value[SerialSeparatorOffset];

      return groupSeparator == serialSeparator && !groupSeparator.IsAsciiDigit();
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

   private static Boolean ValidateLength(ReadOnlySpan<Char> value)
      => value.Length is UnformattedLength or FormattedLength;
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
