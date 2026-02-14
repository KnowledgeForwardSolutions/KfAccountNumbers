// Ignore Spelling: itin Json Kf

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
   private const Int32 NonFormattedLength = 9;

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
   ///   Initialize a new <see cref="UsIndividualTaxpayerIdentificationNumber"/>.
   /// </summary>
   /// <param name="itin">
   ///   String representation of an Individual Taxpayer Identification Number.
   /// </param>
   /// <exception cref="KfValidationException{UsIndividualTaxpayerIdentificationNumberValidationResult}">
   ///   <paramref name="itin"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="itin"/> does not have length of 9 or 11.
   ///   - or -
   ///   <paramref name="itin"/> contains a non-ASCII digit (not 0-9).
   ///   - or -
   ///   <paramref name="itin"/> is 11 characters in length and contains and 
   ///   separator characters that are not identical or are ASCII digits (0-9).
   ///   - or -
   ///   <paramref name="itin"/> contains an invalid area number (000-899).
   ///   - or -
   ///   <paramref name="itin"/> contains an invalid group number (not in the
   ///   ranges 50-65, 70-88, 90-92 or 94-99).
   /// </exception>
   public UsIndividualTaxpayerIdentificationNumber(String? itin)
      : this(itin, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has already
   ///   been validated.
   /// </summary>
   private UsIndividualTaxpayerIdentificationNumber(String? itin, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         UsIndividualTaxpayerIdentificationNumberValidationResult validationResult = Validate(itin);
         if (validationResult != UsIndividualTaxpayerIdentificationNumberValidationResult.ValidationPassed)
         {
            throw validationResult.ToValidationException();
         }
      }

      Value = GetValidatedItin(itin!);
   }

   /// <summary>
   ///   The raw ITIN value.
   /// </summary>
   public String Value { get; private init; }

   public static implicit operator String(UsIndividualTaxpayerIdentificationNumber itin)
      => itin?.Value ?? String.Empty;     // Handle null ITIN object gracefully by returning empty string

   // Explicit conversion from String to avoid unintentional conversions that may throw exceptions.
   public static explicit operator UsIndividualTaxpayerIdentificationNumber(String? itin) => new(itin);

   /// <summary>
   ///   Create a new <see cref="UsIndividualTaxpayerIdentificationNumber"/>.
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
         var validationFailure => validationFailure
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
   ///   Check the <paramref name="itin"/> to determine if it contains a valid
   ///   US Individual Taxpayer Identification Number (ITIN).
   /// </summary>
   /// <param name="itin">
   ///   String representation of a Social Security Number.
   /// </param>
   /// <returns>
   ///   A <see cref="UsIndividualTaxpayerIdentificationNumberValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="itin"/> passed validation
   ///   or what validation error was encountered.
   /// </returns>
   public static UsIndividualTaxpayerIdentificationNumberValidationResult Validate(String? itin)
   {
      // Preliminary checks for obviously incorrect values.
      if (String.IsNullOrWhiteSpace(itin))
      {
         return UsIndividualTaxpayerIdentificationNumberValidationResult.Empty;
      }
      if (!ValidateLength(itin))
      {
         return UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidLength;
      }
      if (itin[0] != Chars.DigitNine)
      {
         return UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidAreaNumber;
      }
      if (IsFormattedItin(itin) && !ValidateEmbeddedSeparatorCharacters(itin))
      {
         return UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidSeparatorEncountered;
      }
      if (!ValidateAllDigits(itin))
      {
         return UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidCharacterEncountered;
      }
      if (!ValidateGroupNumber(itin))
      {
         return UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidGroupNumber;
      }

      return UsIndividualTaxpayerIdentificationNumberValidationResult.ValidationPassed;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static ReadOnlySpan<Char> GetAreaNumber(ReadOnlySpan<Char> itin)
      => itin[..AreaRangeEnd];

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static ReadOnlySpan<Char> GetGroupNumber(ReadOnlySpan<Char> itin)
      => IsFormattedItin(itin)
         ? itin[FormattedGroupRangeStart..FormattedGroupRangeEnd]
         : itin[UnformattedGroupRangeStart..UnformattedGroupRangeEnd];

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static ReadOnlySpan<Char> GetSerialNumber(ReadOnlySpan<Char> itin)
      => IsFormattedItin(itin)
         ? itin[FormattedSerialNumberRangeStart..]
         : itin[UnformattedSerialNumberRangeStart..];

   /// <summary>
   ///   Get an unformatted ITIN value from a string that has passed validation.
   ///   If the source string is formatted, then create a new string by merging
   ///   all three ITIN sections together without allocating intermediate 
   ///   Strings.
   /// </summary>
   private static String GetValidatedItin(String itin)
   {
      if (itin.Length == NonFormattedLength)
      {
         return itin;
      }

      var buffer = ArrayPool<Char>.Shared.Rent(NonFormattedLength);
      try
      {
         var span = new Span<Char>(buffer);
         ReadOnlySpan<Char> areaNumber = GetAreaNumber(itin);
         ReadOnlySpan<Char> groupNumber = GetGroupNumber(itin);
         ReadOnlySpan<Char> serialNumber = GetSerialNumber(itin);
         areaNumber.CopyTo(span[..AreaRangeEnd]);
         groupNumber.CopyTo(span[UnformattedGroupRangeStart..UnformattedGroupRangeEnd]);
         serialNumber.CopyTo(span[UnformattedSerialNumberRangeStart..]);

         return span[..NonFormattedLength].ToString();
      }
      finally
      {
         ArrayPool<Char>.Shared.Return(buffer);
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormattedItin(ReadOnlySpan<Char> itin) => itin.Length == FormattedLength;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Int32 ParseGroupNumber(ReadOnlySpan<Char> groupSpan)
      => ((groupSpan[0] - Chars.DigitZero) * 10) + (groupSpan[1] - Chars.DigitZero);

   private static Boolean ValidateAllDigits(ReadOnlySpan<Char> itin)
   {
      var length = itin.Length;
      for (var index = 1; index < itin.Length; index++)     // Can skip the first character since it is already validated to be '9'
      {
         if (length == FormattedLength && (index is GroupSeparatorOffset or SerialSeparatorOffset))
         {
            continue;  // Skip separator character positions in formatted ITIN
         }

         if (!itin[index].IsAsciiDigit())
         {
            return false;
         }
      }

      return true;
   }

   // A formatted ITIN must contain the same separator character at the expected
   // offsets. And the separator character must be a non-digit character.
   private static Boolean ValidateEmbeddedSeparatorCharacters(ReadOnlySpan<Char> itin)
   {
      var groupSeparator = itin[GroupSeparatorOffset];
      var serialSeparator = itin[SerialSeparatorOffset];

      return groupSeparator == serialSeparator && !groupSeparator.IsAsciiDigit();
   }

   private static Boolean ValidateGroupNumber(ReadOnlySpan<Char> itin)
   {
      ReadOnlySpan<Char> groupSpan = GetGroupNumber(itin);
      var groupNumber = ParseGroupNumber(groupSpan);
      return groupNumber is
            (>= 50 and <= 65) or
            (>= 70 and <= 88) or
            (>= 90 and <= 92) or
            (>= 94 and <= 99);
   }

   private static Boolean ValidateLength(ReadOnlySpan<Char> itin)
      => itin.Length is NonFormattedLength or FormattedLength;
}

public class UsIndividualTaxpayerIdentificationNumberJsonConverter : JsonConverter<UsIndividualTaxpayerIdentificationNumber>
{
   public override UsIndividualTaxpayerIdentificationNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var itinString = reader.GetString();
      return new UsIndividualTaxpayerIdentificationNumber(itinString);
   }

   public override void Write(Utf8JsonWriter writer, UsIndividualTaxpayerIdentificationNumber value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
