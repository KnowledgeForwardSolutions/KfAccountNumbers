// Ignore Spelling: Json

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression
#pragma warning disable SA1516 // Elements should be separated by blank line

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a National Insurance Number
///   (or NINO). While not defined as such, it effectively serves as national
///   identifier in the UK.
/// </summary>
/// <remarks>
///   <para>
///      A National Insurance Number consists of nine characters structured as
///      PPDDDDDDS, where:
///      <list type="bullet">
///         <item>
///            <term>PP</term>
///            <description>
///               is a two-letter prefix. See below for valid prefix characters.
///            </description>
///         </item>
///         <item>
///            <term>DDDDDD</term>
///            <description>
///               is a six-digit sequentially assigned number.
///            </description>
///         </item>
///         <item>
///            <term>S</term>
///            <description>
///               is a single suffix letter, either A, B, C, or D. The suffix
///               can be omitted if it is unknown as the suffix does not
///               contribute to the uniqueness of the value.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A National Insurance Number is typically displayed as a single string
///      of nine characters but can be formatted for readability as groups of
///      two characters with a separator character, typically a space
///      (i.e. PP DD DD DD S). <see cref="GbNationalInsuranceNumber"/> is
///      case-sensitive and requires the prefix and suffix characters to be
///      uppercase letters.
///   </para>
///   <para>
///      When creating a new <see cref="GbNationalInsuranceNumber"/>, the
///      following validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must have length 8, 9, 11 or 13 characters.
///               (8 characters = unformatted, without suffix character,
///               9 characters = unformatted, with suffix character,
///               11 characters = formatted, without suffix character,
///               13 characters = formatted, with suffix character)
///            </description>
///         </item>
///         <item>
///            <description>
///               The leading (left-most) two characters may not be BG, GB, NK,
///               KN, TN, NT, or ZZ.
///            </description>
///         </item>
///         <item>
///            <description>
///               Character position 0 (zero-based) must be an uppercase letter,
///               A-C, E, G, H, J-P, R-T, W-Z. The letters D, F, I, Q, U and V
///               are not allowed.
///            </description>
///         </item>
///         <item>
///            <description>
///               Character position 1 (zero-based) must be an uppercase letter,
///               A-C, E, G, H, J-N, P, R-T, W-Z. The letters D, F, I, O, Q, U
///               and V are not allowed. (Note O allowed in character position 0
///               but not in character position 1).
///            </description>
///         </item>
///         <item>
///            <description>
///               Character positions 2-7 (zero-based) must be ASCII digits
///               ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               Character position 8 (zero-based), if present, must be an
///               uppercase letter, A-D.
///            </description>
///         </item>
///         <item>
///            <description>
///               Separator characters, if present, may not be ASCII digits
///               ('0'-'9') or uppercase or lowercase letters (A-Z, a-z).
///            </description>
///         </item>
///         <item>
///            <description>
///               The same character must be used in every separator position.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Note that National Insurance Numbers do not include a check digit.
///   </para>
///   <para>
///      Also note that since suffix characters do not contribute to the
///      uniqueness of National Insurance numbers, then it is technically
///      accurate to say that two values that differ only by one having a suffix
///      character and the other not should be considered equal. However, if
///      <see cref="GbNationalInsuranceNumber"/> were to override the normal
///      record equality to support this case there would be other implications,
///      such as hashing or equality where two values have suffix character but
///      only differ by suffix character. In the end,
///      <see cref="GbNationalInsuranceNumber"/> uses normal record equality and
///      two values that differ only by the presence or absence of a suffix
///      character will still not be considered equal. But
///      <see cref="GbNationalInsuranceNumber"/> does attempt to support this
///      case by including an `EqualsNonSuffix` method that performs an equality
///      check only on the first eight characters (two prefix characters and six
///      digits) of both values.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>AB123456C</term>
///            <description>unformatted, with suffix character</description>
///         </item>
///         <item>
///            <term>GG000123</term>
///            <description>unformatted, without suffix character</description>
///         </item>
///         <item>
///            <term>AB 12 34 56 C</term>
///            <description>formatted, with suffix character</description>
///         </item>
///         <item>
///            <term>GG 00 01 23</term>
///            <description>formatted, without suffix character</description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/National_Insurance_number for more info.
///   </para>
/// </remarks>
[JsonConverter(typeof(GbNationalInsuranceNumberJsonConverter))]
public record GbNationalInsuranceNumber
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="GbNationalInsuranceNumber"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidGbNationalInsuranceNumberPrefix,
      InvalidCharacter,
      InvalidSeparator)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="GbNationalInsuranceNumber"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidGbNationalInsuranceNumberPrefix,
      InvalidCharacter,
      InvalidSeparator)
   {
   }

   private const Int32 UnformattedWithoutSuffixLength = 8;
   private const Int32 UnformattedWithSuffixLength = 9;
   private const Int32 FormattedWithoutSuffixLength = 11;
   private const Int32 FormattedWithSuffixLength = 13;

   private const Int32 Separator1Offset = 2;
   private const Int32 Separator2Offset = 5;
   private const Int32 Separator3Offset = 8;
   private const Int32 Separator4Offset = 11;

   private static readonly HashSet<String>.AlternateLookup<ReadOnlySpan<Char>> _invalidPrefixes =
      new HashSet<String>(new CaseInsensitiveSpanComparer()) { "BG", "GB", "NK", "KN", "TN", "NT", "ZZ" }.GetAlternateLookup<ReadOnlySpan<Char>>();
   private static readonly HashSet<Char> _allowedPrefixFirstCharacters = [.. "ABCEGHJKLMNOPRSTWXYZ"];
   private static readonly HashSet<Char> _allowedPrefixSecondCharacters = [.. "ABCEGHJKLMNPRSTWXYZ"];

   /// <summary>
   ///   Initializes a new instance of the
   ///   <see cref="GbNationalInsuranceNumber"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of UK National Insurance Number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 8/9 (unformatted, without/with
   ///   suffix) or length 11/13 (formatted, without/with suffix).
   ///   - or -
   ///   <paramref name="value"/> contains an invalid two-character prefix. BG,
   ///   GB, NK, KN, TN, NT, or ZZ are not allowed prefixes.
   ///   - or -
   ///   <paramref name="value"/> has an invalid initial character. Only A-C, E,
   ///   G, H, J-P, R-T, W-Z are allowed as the first character.
   ///   - or -
   ///   <paramref name="value"/> has an invalid second character. Only A-C, E,
   ///   G, H, J-N, P, R-T, W-Z are allowed as the second character.
   ///   - or -
   ///   <paramref name="value"/> contains a character other than an ASCII digit
   ///   ('0'-'9') in character positions 2-7 (zero-based).
   ///   - or -
   ///   <paramref name="value"/> contains an invalid trailing alphabetic
   ///   character. If present, the trailing alphabetic character must be A-D.
   ///   - or -
   ///   <paramref name="value"/> is formatted (has length 11 or 13) and has an
   ///   ASCII digit ('0'-'9') or an alphabetic character (A-Z) in a separator
   ///   location (character positions 2, 5, 8 and 11, zero-based). In addition,
   ///   each separator location must contain the same character.
   /// </exception>
   public GbNationalInsuranceNumber(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the
   ///   <see cref="GbNationalInsuranceNumber"/> class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private GbNationalInsuranceNumber(String? value, ValidationMode validationMode)
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
               InvalidGbNationalInsuranceNumberPrefix invalidPrefix => new UKfValidationException<ValidationError>(invalidPrefix),
               InvalidCharacter invalidCharacter => new UKfValidationException<ValidationError>(invalidCharacter),
               InvalidSeparator invalidSeparator => new UKfValidationException<ValidationError>(invalidSeparator),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetRawValue(value!);
   }

   /// <summary>
   ///   Gets the raw National Insurance Number value.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="GbNationalInsuranceNumber"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="GbNationalInsuranceNumber"/> to convert.
   /// </param>
   public static implicit operator String(GbNationalInsuranceNumber source)
      => source?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="GbNationalInsuranceNumber"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a UK National Insurance Number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid PPS number.
   /// </exception>
   public static explicit operator GbNationalInsuranceNumber(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="GbNationalInsuranceNumber"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a UK National Insurance Number.
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{GbNationalInsuranceNumber, ValidationError}"/>. Will
   ///   contain the new <see cref="GbNationalInsuranceNumber"/> if <paramref name="value"/>
   ///   is valid or a <see cref="ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<GbNationalInsuranceNumber, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new GbNationalInsuranceNumber(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidGbNationalInsuranceNumberPrefix invalidPrefix => (ValidationError)invalidPrefix,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Determines whether the current National Insurance number is equal to
   ///   another, ignoring the suffix (if it exists).
   /// </summary>
   /// <remarks>
   ///   <para>
   ///      The comparison does not consider the suffix portion of the National
   ///      Insurance number.
   ///   </para>
   ///   <para>
   ///      The alphabetic suffix of a National Insurance Number is a historical
   ///      artifact and does not contribute to the uniqueness of the value so
   ///      it can be safely dropped from the value if it is unknown. This
   ///      method exists to support cases where values that may not have suffix
   ///      characters need to be compared for equality without having to alter
   ///      the base value type equality of the <see cref="GbNationalInsuranceNumber"/>
   ///      type.
   ///   </para>
   /// </remarks>
   /// <param name="other">
   ///   The National Insurance number to compare with the current instance.
   ///   Can be <see langword="null"/>.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if the main parts of the two National Insurance
   ///   numbers are equal, ignoring the suffix; otherwise, <see langword="false"/>.
   /// </returns>
   public Boolean EqualsNonSuffix(GbNationalInsuranceNumber? other)
   {
      if (other is null)
      {
         return false;
      }

      if (ReferenceEquals(this, other))
      {
         return true;
      }

      ReadOnlySpan<Char> span1 = Value.AsSpan(..UnformattedWithoutSuffixLength);
      ReadOnlySpan<Char> span2 = other.Value.AsSpan(..UnformattedWithoutSuffixLength);

      return span1.Equals(span2, StringComparison.Ordinal);
   }

   /// <summary>
   ///   Format the national insurance number using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then the default mask "__ __ __ __ _" (or "__ __ __ __" for values
   ///   without suffixes) will be used instead.
   /// </param>
   /// <returns>
   ///   A formatted national insurance number.
   /// </returns>
   /// <exception cref="ArgumentException">
   ///   <paramref name="mask"/> is <see cref="String.Empty"/> or all whitespace
   ///   characters.
   /// </exception>
   /// <remarks>
   ///   <see cref="ExtensionMethods.FormatWithMask(String, String)"/> for more
   ///   details on creating a mask to format the national insurance number.
   /// </remarks>
   public String Format(String? mask = null)
   {
      mask ??= HasSuffix(Value)
         ? "__ __ __ __ _"
         : "__ __ __ __";

      return Value.FormatWithMask(mask);
   }

   /// <summary>
   ///   Get a string representation of the National Insurance Number.
   /// </summary>
   /// <returns>
   ///   The raw National Insurance Number, normalized to upper-case.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid UK National Insurance Number.
   /// </summary>
   /// <param name="value">
   ///   String representation of a UK National Insurance Number.
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
         return new InvalidLength(
            Messages.GbNationalInsuranceNumberInvalidLength,
            value.Length,
            GetValidLengthDefinitions());
      }

      if (!ValidatePrefix(value))
      {
         return new InvalidGbNationalInsuranceNumberPrefix(
            Messages.GbNationalInsuranceNumberInvalidPrefix,
            value[..2]);
      }

      if (!ValidatePrefixFirstCharacter(value))
      {
         return GetInvalidCharacterResult(value, 0);
      }

      if (!ValidatePrefixSecondCharacter(value))
      {
         return GetInvalidCharacterResult(value, 1);
      }

      if (!ValidateDigits(value, out var invalidCharacterPosition))
      {
         return GetInvalidCharacterResult(value, invalidCharacterPosition);
      }

      if (!ValidateSuffixCharacter(value))
      {
         return GetInvalidCharacterResult(value, value.Length - 1);
      }

      if (!ValidateSeparators(value, out invalidCharacterPosition))
      {
         return new InvalidSeparator(
            Messages.GbNationalInsuranceNumberInvalidSeparator,
            value[invalidCharacterPosition],
            invalidCharacterPosition);
      }

      return default(ValidValue);
   }

   /// <summary>
   ///   Gets an array of details about valid lengths accepted for a National
   ///   Insurance Number.
   /// </summary>
   /// <returns>
   ///   An array of <see cref="ValidLengthDefinition"/>s.
   /// </returns>
   internal static ValidLengthDefinition[] GetValidLengthDefinitions()
      =>
      [
         new ValidLengthDefinition(UnformattedWithoutSuffixLength, Messages.GbNationalInsuranceNumberUnformattedNoSuffixLength),
         new ValidLengthDefinition(UnformattedWithSuffixLength, Messages.GbNationalInsuranceNumberUnformattedWithSuffixLength),
         new ValidLengthDefinition(FormattedWithoutSuffixLength, Messages.GbNationalInsuranceNumberFormattedNoSuffixLength),
         new ValidLengthDefinition(FormattedWithSuffixLength, Messages.GbNationalInsuranceNumberFormattedWithSuffixLength),
      ];

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(
         Messages.GbNationalInsuranceNumberInvalidCharacter,
         value[position],
         position);

   private static String GetRawValue(String value)
   {
      var isFormatted = IsFormatted(value);
      if (!isFormatted)
      {
         return value;
      }

      var hasSuffix = HasSuffix(value);
      var finalLength = hasSuffix
         ? UnformattedWithSuffixLength
         : UnformattedWithoutSuffixLength;
      var buffer = ArrayPool<Char>.Shared.Rent(finalLength);
      try
      {
         ReadOnlySpan<Char> source = value.AsSpan();
         var span = new Span<Char>(buffer);

         ReadOnlySpan<Int32> segmentLengths = hasSuffix
            ? [2, 2, 2, 2, 1]
            : [2, 2, 2, 2];
         var sourceOffset = 0;
         var targetOffset = 0;
         foreach (var length in segmentLengths)
         {
            ReadOnlySpan<Char> sourceSpan = source[sourceOffset..(sourceOffset + length)];
            Span<Char> targetSpan = span[targetOffset..(targetOffset + length)];

            sourceSpan.CopyTo(targetSpan);

            sourceOffset += length + 1;
            targetOffset += length;
         }

         return span[..finalLength].ToString();
      }
      finally
      {
         ArrayPool<Char>.Shared.Return(buffer);
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean HasSuffix(ReadOnlySpan<Char> value)
      => value.Length is UnformattedWithSuffixLength or FormattedWithSuffixLength;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> value)
      => value.Length > UnformattedWithSuffixLength;

   private static Boolean ValidateDigits(
      ReadOnlySpan<Char> value,
      out Int32 invalidCharacterPosition)
   {
      invalidCharacterPosition = -1;
      var isFormatted = IsFormatted(value);
      var start = isFormatted ? 3 : 2;
      var end = value.Length switch
      {
         UnformattedWithSuffixLength => value.Length - 1,
         FormattedWithSuffixLength => value.Length - 2,
         _ => value.Length,
      };

      for (var index = start; index < end; index++)
      {
         if (isFormatted
            && (index is Separator1Offset or Separator2Offset or Separator3Offset or Separator4Offset))
         {
            continue;
         }

         if (!value[index].IsAsciiDigit())
         {
            invalidCharacterPosition = index;
            return false;
         }
      }

      return true;
   }

   private static Boolean ValidateLength(ReadOnlySpan<Char> value)
      => value.Length is UnformattedWithoutSuffixLength
         or UnformattedWithSuffixLength
         or FormattedWithoutSuffixLength
         or FormattedWithSuffixLength;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidatePrefix(ReadOnlySpan<Char> value)
      => !_invalidPrefixes.Contains(value[..2]);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidatePrefixFirstCharacter(ReadOnlySpan<Char> value)
      => _allowedPrefixFirstCharacters.Contains(value[0]);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidatePrefixSecondCharacter(ReadOnlySpan<Char> value)
      => _allowedPrefixSecondCharacters.Contains(value[1]);

   private static Boolean ValidateSeparators(
      ReadOnlySpan<Char> value,
      out Int32 invalidCharacterOffset)
   {
      invalidCharacterOffset = -1;
      if (value.Length < FormattedWithoutSuffixLength)
      {
         return true;
      }

      var initialSeparator = value[Separator1Offset];
      if (initialSeparator is (>= Chars.DigitZero and <= Chars.DigitNine)
         or (>= Chars.UpperCaseA and <= Chars.UpperCaseZ)
         or (>= Chars.LowerCaseA and <= Chars.LowerCaseZ))
      {
         invalidCharacterOffset = Separator1Offset;
         return false;
      }

      if (value[Separator2Offset] != initialSeparator)
      {
         invalidCharacterOffset = Separator2Offset;
         return false;
      }

      if (value[Separator3Offset] != initialSeparator)
      {
         invalidCharacterOffset = Separator3Offset;
         return false;
      }

      if (value.Length == FormattedWithSuffixLength
         && value[Separator4Offset] != initialSeparator)
      {
         invalidCharacterOffset = Separator4Offset;
         return false;
      }

      return true;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidateSuffixCharacter(ReadOnlySpan<Char> value)
      => value.Length is UnformattedWithoutSuffixLength or FormattedWithoutSuffixLength
         || value[^1] is >= Chars.UpperCaseA and <= Chars.UpperCaseD;
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class GbNationalInsuranceNumberJsonConverter : JsonConverter<GbNationalInsuranceNumber>
{
   public override GbNationalInsuranceNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new GbNationalInsuranceNumber(str);
   }

   public override void Write(Utf8JsonWriter writer, GbNationalInsuranceNumber value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
