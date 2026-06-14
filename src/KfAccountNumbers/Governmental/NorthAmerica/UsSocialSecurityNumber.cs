// Ignore Spelling: Json Kf

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Strongly typed business object for a US Social Security Number (SSN).
/// </summary>
/// <remarks>
///   <para>
///      A valid US Social Security Number consists of nine decimal digits. The
///      nine digits are commonly separated into three groups, the first three
///      digits represent the area number, the next two digits represent the
///      group number and the final four digits represent the serial number.
///   </para>
///   <para>
///      Prior to 2011, the area number was tied to a geographic region in the
///      US, the group number represented a subgrouping within the area and the
///      serial number was a consecutive sequence within the group. However in
///      2011, the SSN generation process was randomized and the geographic
///      association was eliminated. The changes made in 2011 also started
///      issuing social security numbers from previously reserved blocks of area
///      numbers. The validation checks performed by
///      <see cref="UsSocialSecurityNumber"/> use the rules in effect at the
///      time of release (currently 2026) and therefore some SSN's that would
///      have been considered invalid prior to 2011 are considered valid by
///      <see cref="UsSocialSecurityNumber"/>.
///   </para>
///   <para>
///      Social Security Numbers are commonly formatted with dashes ('-') and
///      sometimes spaces separating the three groups. A
///      <see cref="UsSocialSecurityNumber"/> can be created from strings that
///      include or exclude the separator character, but if used, the same
///      character must be used to separate both the area/group numbers and the
///      group/serial numbers. The separator character may not be a decimal
///      digit (0-9).
///   </para>
///   <para>
///      Not all 9-digit numbers are valid Social Security Numbers. When
///      creating a new  <see cref="UsSocialSecurityNumber"/>, after determining
///      that a value consists of 9 decimal digits, the following validation
///      rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               An area number, group number or serial number that is all
///               zeros (0) is invalid.
///            </description>
///         </item>
///         <item>
///            <description>
///               The area number 666 is invalid.
///            </description>
///         </item>
///         <item>
///            <description>
///               Area numbers 900-999 are invalid because they are reserved for
///               Individual Taxpayer Identification Numbers.
///            </description>
///         </item>
///         <item>
///            <description>
///               The group number 00 is invalid.
///            </description>
///         </item>
///         <item>
///            <description>
///               The serial number 0000 is invalid.
///            </description>
///         </item>
///         <item>
///            <description>
///               A number that is 9 identical digits is invalid.
///            </description>
///         </item>
///         <item>
///            <description>
///               The run of 9 consecutive digits (123456789) is invalid.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      <see cref="UsSocialSecurityNumber"/> does not confirm the actual
///      validity of a number that meets all of the above rules. Confirmation of
///      the actual validity of a SSN requires use of the Social Security Number
///      Verification Service offered by the Social Security Administration.
///   </para>
///   <para>
///      See <see href="https://en.wikipedia.org/wiki/Social_Security_number">Wikipedia - Social Security Number</see>
///      for more details.
///   </para>
/// </remarks>
[JsonConverter(typeof(UsSocialSecurityNumberJsonConverter))]
public record UsSocialSecurityNumber
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="UsSocialSecurityNumber"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidSeparator,
      InvalidCharacter,
      UsTinInvalidAreaNumber,
      UsTinInvalidGroupNumber,
      UsSsnInvalidSerialNumber,
      UsSsnAllIdenticalDigits,
      UsSsnInvalidRun)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="UsSocialSecurityNumber"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidSeparator,
      InvalidCharacter,
      UsTinInvalidAreaNumber,
      UsTinInvalidGroupNumber,
      UsSsnInvalidSerialNumber,
      UsSsnAllIdenticalDigits,
      UsSsnInvalidRun)
   {
   }

   private const Int32 FormattedLength = 11;
   private const Int32 UnformattedLength = 9;

   private static readonly SegmentRange _area = new(0, 3);
   private static readonly SegmentRange _formattedGroup = new(4, 6);
   private static readonly SegmentRange _unformattedGroup = new(3, 5);
   private static readonly SegmentRange _formattedSerial = new(7, 11);
   private static readonly SegmentRange _unformattedSerial = new(5, 9);

   private const Int32 GroupSeparatorOffset = 3;   // Offset of separator between area and group sections in formatted SSN
   private const Int32 SerialSeparatorOffset = 6;  // Offset of separator between group and serial number sections in formatted SSN

   /// <summary>
   ///   Initializes a new instance of the <see cref="UsSocialSecurityNumber"/>
   ///   class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Social Security Number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
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
   ///   <paramref name="value"/> contains an invalid area number (000, 666 or
   ///   900-999).
   ///   - or -
   ///   <paramref name="value"/> contains an invalid group number (00).
   ///   - or -
   ///   <paramref name="value"/> contains an invalid serial number (0000).
   ///   - or -
   ///   <paramref name="value"/> contains nine identical digits.
   ///   - or -
   ///   <paramref name="value"/> contains a run of consecutive digits from 1 to 9.
   /// </exception>
   public UsSocialSecurityNumber(String? value)
      : this(value, validationMode: ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="UsSocialSecurityNumber"/>
   ///   class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private UsSocialSecurityNumber(String? value, ValidationMode validationMode)
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
               UsSsnInvalidSerialNumber invalidSerialNumber => new UKfValidationException<ValidationError>(invalidSerialNumber),
               UsSsnAllIdenticalDigits allIdenticalDigits => new UKfValidationException<ValidationError>(allIdenticalDigits),
               UsSsnInvalidRun invalidRun => new UKfValidationException<ValidationError>(invalidRun),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetRawValue(value!);
   }

   /// <summary>
   ///   Gets the raw SSN value.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="UsSocialSecurityNumber"/> to a <see cref="String"/>,
   ///   returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="UsSocialSecurityNumber"/> to convert.
   /// </param>
   public static implicit operator String(UsSocialSecurityNumber source)
      => source?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="UsSocialSecurityNumber"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Social Security Number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid SSN.
   /// </exception>
   public static explicit operator UsSocialSecurityNumber(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="UsSocialSecurityNumber"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Social Security Number.
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{UsSocialSecurityNumber, ValidationError}"/>. Will
   ///   contain the new <see cref="UsSocialSecurityNumber"/> if
   ///   <paramref name="value"/> is valid or a <see cref="ValidationError"/>
   ///   that identifies the validation rule that was failed if
   ///   <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<UsSocialSecurityNumber, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new UsSocialSecurityNumber(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         UsTinInvalidAreaNumber invalidAreaNumber => (ValidationError)invalidAreaNumber,
         UsTinInvalidGroupNumber invalidGroupNumber => (ValidationError)invalidGroupNumber,
         UsSsnInvalidSerialNumber invalidSerialNumber => (ValidationError)invalidSerialNumber,
         UsSsnAllIdenticalDigits allIdenticalDigits => (ValidationError)allIdenticalDigits,
         UsSsnInvalidRun invalidRun => (ValidationError)invalidRun,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the SSN using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   The mask that specified the final output.
   /// </param>
   /// <returns>
   ///   A formatted Social Security Number.
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
   ///   details on creating a mask to format the SSN.
   /// </remarks>
   public String Format(String mask = "___-__-____") => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the SSN.
   /// </summary>
   /// <returns>
   ///   The raw SSN, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a valid
   ///   US Social Security Number (SSN).
   /// </summary>
   /// <param name="value">
   ///   String representation of a Social Security Number.
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
            Messages.UsSsnInvalidLength,
            value.Length,
            GetInvalidLengthDefinitions());
      }

      if (!ValidateSeparators(value, out var invalidSeparatorPosition))
      {
         return new InvalidSeparator(
            Messages.UsSsnInvalidSeparator,
            value[invalidSeparatorPosition],
            invalidSeparatorPosition);
      }

      if (!ValidateAllDigits(value, out var invalidCharacterPosition))
      {
         return new InvalidCharacter(
            Messages.UsSsnInvalidCharacter,
            value[invalidCharacterPosition],
            invalidCharacterPosition);
      }

      // We know that the value contains 9 digits. Perform higher level checks
      // on the individual sections and the entire value.
      ReadOnlySpan<Char> areaNumber = GetAreaNumber(value);
      if (!ValidateAreaNumber(areaNumber))
      {
         return new UsTinInvalidAreaNumber(
            Messages.UsSsnInvalidAreaNumber,
            areaNumber.ToString());
      }

      ReadOnlySpan<Char> groupNumber = GetGroupNumber(value);
      if (!ValidateGroupNumber(groupNumber))
      {
         return new UsTinInvalidGroupNumber(
            Messages.UsSsnInvalidGroupNumber,
            groupNumber.ToString());
      }

      ReadOnlySpan<Char> serialNumber = GetSerialNumber(value);
      if (!ValidateSerialNumber(serialNumber))
      {
         return new UsSsnInvalidSerialNumber(
            Messages.UsSsnInvalidSerialNumber,
            serialNumber.ToString());
      }

      if (!ValidateNotAllIdenticalDigits(value))
      {
         return default(UsSsnAllIdenticalDigits);
      }

      if (!ValidateNotConsecutiveRun(areaNumber, groupNumber, serialNumber))
      {
         return default(UsSsnInvalidRun);
      }

      return default(ValidValue);
   }

   /// <summary>
   ///   Gets an array of details about valid lengths accepted for a US SSN.
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
   ///   Get an unformatted SSN value from a string that has passed validation.
   ///   If the source string is formatted, then create a new string by merging
   ///   all three SSN sections together without allocating intermediate
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

   private static Boolean ValidateAllDigits(
      ReadOnlySpan<Char> value,
      out Int32 invalidCharacterPosition)
   {
      invalidCharacterPosition = -1;
      var isFormatted = IsFormatted(value);
      for (var index = 0; index < value.Length; index++)
      {
         if (isFormatted && index is GroupSeparatorOffset or SerialSeparatorOffset)
         {
            continue;  // Skip separator character positions in formatted SSN
         }

         if (!value[index].IsAsciiDigit())
         {
            invalidCharacterPosition = index;
            return false;
         }
      }

      return true;
   }

   private static Boolean ValidateAreaNumber(ReadOnlySpan<Char> areaNumber)
   {
      const String invalidArea000 = "000";
      const String invalidArea666 = "666";

      return areaNumber[0] != Chars.DigitNine
             && !areaNumber.Equals(invalidArea000, StringComparison.Ordinal)
             && !areaNumber.Equals(invalidArea666, StringComparison.Ordinal);
   }

   private static Boolean ValidateGroupNumber(ReadOnlySpan<Char> groupNumber)
   {
      const String invalidGroup00 = "00";

      return !groupNumber.Equals(invalidGroup00, StringComparison.Ordinal);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidateLength(ReadOnlySpan<Char> value)
      => value.Length is UnformattedLength or FormattedLength;

   private static Boolean ValidateNotAllIdenticalDigits(ReadOnlySpan<Char> value)
   {
      var initialChar = value[0];
      var isFormatted = IsFormatted(value);
      for (var index = 0; index < value.Length; index++)
      {
         if (isFormatted && index is GroupSeparatorOffset or SerialSeparatorOffset)
         {
            continue;
         }

         // First character that is different from the initial character means
         // that not all digits are identical.
         if (value[index] != initialChar)
         {
            return true;
         }
      }

      return false;  // All 9 digits are identical.
   }

   private static Boolean ValidateNotConsecutiveRun(
      ReadOnlySpan<Char> areaNumber,
      ReadOnlySpan<Char> groupNumber,
      ReadOnlySpan<Char> serialNumber)
   {
      const String areaRun = "123";
      const String groupRun = "45";
      const String serialRun = "6789";

      // SSN is invalid only if it's exactly 123-45-6789
      if (!areaNumber.SequenceEqual(areaRun))
      {
         return true;  // Not the run - valid
      }

      if (!groupNumber.SequenceEqual(groupRun))
      {
         return true;  // Not the run - valid
      }

      if (!serialNumber.SequenceEqual(serialRun))
      {
         return true;  // Not the run - valid
      }

      return false;  // It's 123-45-6789 - invalid
   }

   // A formatted SSN must contain the same separator character at the expected
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

   private static Boolean ValidateSerialNumber(ReadOnlySpan<Char> serialNumber)
   {
      const String invalidSerial0000 = "0000";

      return !serialNumber.Equals(invalidSerial0000, StringComparison.Ordinal);
   }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class UsSocialSecurityNumberJsonConverter : JsonConverter<UsSocialSecurityNumber>
{
   public override UsSocialSecurityNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new UsSocialSecurityNumber(str);
   }

   public override void Write(Utf8JsonWriter writer, UsSocialSecurityNumber value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
