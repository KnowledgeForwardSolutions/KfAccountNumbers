// Ignore Spelling: Json

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
///               can be omitted if it is unknown as the suffix does not contribute
///               to the uniqueness of the value.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A National Insurance Number is typically displayed as a single string
///      of nine characters but can be formatted for readability as groups of two
///      characters with a separator character, typically a space (i.e. PP DD DD DD S).
///      <see cref="GbNationalInsuranceNumber"/> is case-sensitive and requires
///      the prefix and suffix characters to be uppercase letters.
///   </para>
///   <para>
///      When creating a new <see cref="GbNationalInsuranceNumber"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must have length 8, 9, 11 or 13 characters. (8 characters =
///               unformatted, without suffix character, 9 characters = unformatted,
///               with suffix character, 11 characters = formatted, without suffix character,
///               13 characters = formatted, with suffix character)
///            </description>
///         </item>
///         <item>
///            <description>
///               The leading (left-most) two characters may not be BG, GB, NK, KN, TN, NT, or ZZ.
///            </description>
///         </item>
///         <item>
///            <description>
///               Character position 0 (zero-based) must be an uppercase letter, A-C, E, G, H, J-P, R-T, W-Z.
///               The letters D, F, I, Q, U and V are not allowed.
///            </description>
///         </item>
///         <item>
///            <description>
///               Character position 1 (zero-based) must be an uppercase letter, A-C, E, G, H, J-N, P, R-T, W-Z.
///               The letters D, F, I, O, Q, U and V are not allowed. (Note O is the only additional excluded character.)
///            </description>
///         </item>
///         <item>
///            <description>
///               Character positions 2-7 (zero-based) must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               Character position 8 (zero-based), if present, must be an uppercase letter, A-D.
///            </description>
///         </item>
///         <item>
///            <description>
///               Separator characters, if present, may not be ASCII digits ('0'-'9') or uppercase or lowercase letters (A-Z, a-z).
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
///      Also note that since suffix characters do not contribute to the uniqueness of National Insurance numbers, then
///      it is technically accurate to say that two values that differ only by one having a suffix character and the
///      other not should be considered equal. However, if <see cref="GbNationalInsuranceNumber"/> were to override the
///      normal record equality to support this case there would be other implications, such as hashing or equality where
///      two values have suffix character but only differ by suffix character. In the end, <see cref="GbNationalInsuranceNumber"/>
///      uses normal record equality and two values that differ only by the presence or absence of a suffix character
///      will still not be considered equal. But <see cref="GbNationalInsuranceNumber"/> does attempt to support this case by
///      including an `EqualsNonSuffix` method that performs an equality check only on the first eight characters (two
///      prefix characters and six digits) of both values.
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
   private const Int32 UnformattedWithoutSuffixLength = 8;
   private const Int32 UnformattedWithSuffixLength = 9;
   private const Int32 FormattedWithoutSuffixLength = 11;
   private const Int32 FormattedWithSuffixLength = 13;

   private const Int32 Separator1Offset = 2;
   private const Int32 Separator2Offset = 5;
   private const Int32 Separator3Offset = 8;
   private const Int32 Separator4Offset = 11;

   private static readonly HashSet<String>.AlternateLookup<ReadOnlySpan<Char>> InvalidPrefixes =
      new HashSet<String>() { "BG", "GB", "NK", "KN", "TN", "NT", "ZZ" }.GetAlternateLookup<ReadOnlySpan<Char>>();
   private static readonly HashSet<Char> AllowedPrefixFirstCharacters = "ABCEGHJKLMNOPRSTWXYZ".ToHashSet();
   private static readonly HashSet<Char> AllowedPrefixSecondCharacters = "ABCEGHJKLMNPRSTWXYZ".ToHashSet();

   /// <summary>
   ///   Initialize a new instance of the <see cref="GbNationalInsuranceNumber"/> class.
   /// </summary>
   /// <param name="nationalInsuranceNumber">
   ///   String representation of UK National Insurance Number.
   /// </param>
   /// <exception cref="KfValidationException{GbNationalInsuranceNumberValidationResult}">
   ///   <paramref name="nationalInsuranceNumber"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="nationalInsuranceNumber"/> is not length 8/9 (unformatted,
   ///   without/with suffix) or length 11/13 (formatted, without/with suffix).
   ///   - or -
   ///   <paramref name="nationalInsuranceNumber"/> contains an invalid two-character
   ///   prefix. BG, GB, NK, KN, TN, NT, or ZZ are not allowed prefixes.
   ///   - or -
   ///   <paramref name="nationalInsuranceNumber"/> has an invalid initial character.
   ///   Only A-C, E, G, H, J-P, R-T, W-Z are allowed as the first character.
   ///   - or -
   ///   <paramref name="nationalInsuranceNumber"/> has an invalid second character.
   ///   Only A-C, E, G, H, J-N, P, R-T, W-Z are allowed as the second character.
   ///   - or -
   ///   <paramref name="nationalInsuranceNumber"/> contains a character other than
   ///   an ASCII digit ('0'-'9') in character positions 2-7 (zero-based).
   ///   - or -
   ///   <paramref name="nationalInsuranceNumber"/> contains an invalid trailing
   ///   alphabetic character. If present, the trailing alphabetic character must
   ///   be A-D.
   /// </exception>
   public GbNationalInsuranceNumber(String? nationalInsuranceNumber)
      : this(nationalInsuranceNumber, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has already
   ///   been validated.
   /// </summary>
   private GbNationalInsuranceNumber(String? nationalInsuranceNumber, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         GbNationalInsuranceNumberValidationResult validationResult = Validate(nationalInsuranceNumber);
         if (validationResult != GbNationalInsuranceNumberValidationResult.ValidationPassed)
         {
            throw validationResult.ToValidationException();
         }
      }

      Value = GetRawValue(nationalInsuranceNumber!);
   }

   /// <summary>
   ///   The raw National Insurance Number value.
   /// </summary>
   public String Value { get; private init; }

   public static implicit operator String(GbNationalInsuranceNumber nationalInsuranceNumber)
      => nationalInsuranceNumber?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   // Explicit conversion from String to avoid unintentional conversions that may throw exceptions.
   public static explicit operator GbNationalInsuranceNumber(String? nationalInsuranceNumber) => new(nationalInsuranceNumber);

   /// <summary>
   ///   Create a new <see cref="GbNationalInsuranceNumber"/> using the Result pattern.
   /// </summary>
   /// <param name="nationalInsuranceNumber">
   ///   String representation of a UK National Insurance Number.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{GbNationalInsuranceNumber, GbNationalInsuranceNumberValidationResult}"/>.
   ///   Will contain the new <see cref="GbNationalInsuranceNumber"/> if 
   ///   <paramref name="nationalInsuranceNumber"/> is valid or an
   ///   <see cref="GbNationalInsuranceNumberValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="nationalInsuranceNumber"/> is 
   ///   invalid.
   /// </returns>
   public static CreateResult<GbNationalInsuranceNumber, GbNationalInsuranceNumberValidationResult> Create(String? nationalInsuranceNumber)
   {
      GbNationalInsuranceNumberValidationResult validationResult = Validate(nationalInsuranceNumber);
      return validationResult == GbNationalInsuranceNumberValidationResult.ValidationPassed
         ? new GbNationalInsuranceNumber(nationalInsuranceNumber, validationMode: ValidationMode.BypassValidation)
         : validationResult;
   }

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

      ReadOnlySpan<Char> span1 = this.Value.AsSpan(..UnformattedWithoutSuffixLength);
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
   /// <remarks>
   ///   Will return the raw National Insurance Number.
   /// </remarks>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="nationalInsuranceNumber"/> to determine if it contains a
   ///   valid UK National Insurance Number.
   /// </summary>
   /// <param name="nationalInsuranceNumber">
   ///   String representation of a UK National Insurance Number.
   /// </param>
   /// <returns>
   ///   A <see cref="GbNationalInsuranceNumberValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="nationalInsuranceNumber"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static GbNationalInsuranceNumberValidationResult Validate(String? nationalInsuranceNumber)
   {
      if (String.IsNullOrWhiteSpace(nationalInsuranceNumber))
      {
         return GbNationalInsuranceNumberValidationResult.Empty;
      }
      else if (!ValidateLength(nationalInsuranceNumber))
      {
         return GbNationalInsuranceNumberValidationResult.InvalidLength;
      }
      else if (!ValidatePrefix(nationalInsuranceNumber))
      {
         return GbNationalInsuranceNumberValidationResult.InvalidPrefix;
      }
      else if (!ValidatePrefixFirstCharacter(nationalInsuranceNumber)
         || !ValidatePrefixSecondCharacter(nationalInsuranceNumber)
         || !ValidateDigits(nationalInsuranceNumber)
         || !ValidateSuffixCharacter(nationalInsuranceNumber))
      {
         return GbNationalInsuranceNumberValidationResult.InvalidCharacter;
      }
      else if (!ValidateSeparators(nationalInsuranceNumber))
      {
         return GbNationalInsuranceNumberValidationResult.InvalidSeparator;
      }

      return GbNationalInsuranceNumberValidationResult.ValidationPassed;
   }

   private static String GetRawValue(String nationalInsuranceNumber)
   {
      var isFormatted = IsFormatted(nationalInsuranceNumber);
      if (!isFormatted)
      {
         return nationalInsuranceNumber;
      }

      var hasSuffix = HasSuffix(nationalInsuranceNumber);
      var finalLength = hasSuffix
         ? UnformattedWithSuffixLength
         : UnformattedWithoutSuffixLength;
      var buffer = ArrayPool<Char>.Shared.Rent(finalLength);
      try
      {
         ReadOnlySpan<Char> source = nationalInsuranceNumber.AsSpan();
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
   private static Boolean HasSuffix(ReadOnlySpan<Char> nationalInsuranceNumber)
      => nationalInsuranceNumber.Length is UnformattedWithSuffixLength or FormattedWithSuffixLength;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> nationalInsuranceNumber)
      => nationalInsuranceNumber.Length > UnformattedWithSuffixLength;

   private static Boolean ValidateDigits(ReadOnlySpan<Char> nationalInsuranceNumber)
   {
      var isFormatted = IsFormatted(nationalInsuranceNumber);
      var start = isFormatted ? 3 : 2;
      var end = nationalInsuranceNumber.Length switch
      {
         UnformattedWithSuffixLength => nationalInsuranceNumber.Length - 1,
         FormattedWithSuffixLength => nationalInsuranceNumber.Length - 2,
         _ => nationalInsuranceNumber.Length
      };

      for (var index = start; index < end; index++)
      {
         if (isFormatted
            && (index is Separator1Offset or Separator2Offset or Separator3Offset or Separator4Offset))
         {
            continue;
         }

         if (!nationalInsuranceNumber[index].IsAsciiDigit())
         {
            return false;
         }
      }

      return true;
   }

   private static Boolean ValidateLength(ReadOnlySpan<Char> nationalInsuranceNumber)
      => nationalInsuranceNumber.Length is UnformattedWithoutSuffixLength
         or UnformattedWithSuffixLength
         or FormattedWithoutSuffixLength
         or FormattedWithSuffixLength;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidatePrefix(ReadOnlySpan<Char> nationalInsuranceNumber)
      => !InvalidPrefixes.Contains(nationalInsuranceNumber[..2]);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidatePrefixFirstCharacter(ReadOnlySpan<Char> nationalInsuranceNumber)
      => AllowedPrefixFirstCharacters.Contains(nationalInsuranceNumber[0]);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidatePrefixSecondCharacter(ReadOnlySpan<Char> nationalInsuranceNumber)
      => AllowedPrefixSecondCharacters.Contains(nationalInsuranceNumber[1]);

   private static Boolean ValidateSeparators(ReadOnlySpan<Char> nationalInsuranceNumber)
   {
      if (nationalInsuranceNumber.Length < FormattedWithoutSuffixLength)
      {
         return true;
      }

      var ch = nationalInsuranceNumber[Separator1Offset];
      if (ch is >= Chars.DigitZero and Chars.DigitNine
         or >= Chars.UpperCaseA and Chars.UpperCaseZ
         or >= Chars.LowerCaseA and Chars.LowerCaseZ)
      {
         return false;
      }

      return ch == nationalInsuranceNumber[Separator2Offset]
         && ch == nationalInsuranceNumber[Separator3Offset]
         && (nationalInsuranceNumber.Length == FormattedWithoutSuffixLength
            || ch == nationalInsuranceNumber[Separator4Offset]);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidateSuffixCharacter(ReadOnlySpan<Char> nationalInsuranceNumber)
      => nationalInsuranceNumber.Length is UnformattedWithoutSuffixLength or FormattedWithoutSuffixLength
         || nationalInsuranceNumber[^1] is >= Chars.UpperCaseA and <= Chars.UpperCaseD;
}

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
