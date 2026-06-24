// Ignore Spelling: Burgerservicenummer Json

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a Dutch Burgerservicenummer
///   or BSN.
/// </summary>
/// <remarks>
///   <para>
///      A burgerservicenummer is a nine-digit number without embedded personal
///      information or attributes other than a trailing check digit calculated
///      using a variation of the modulus 11 algorithm. The number is typically
///      displayed as nine consecutive digits or formatted as NNNN-NN-NNN.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>123456782</term>
///            <description>
///               Nine digits, no formatting characters
///            </description>
///         </item>
///         <item>
///            <term>1112-22-333</term>
///            <description>
///               Nine digits with formatting characters
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      When creating a new <see cref="NlBurgerservicenummer"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 9 characters (without separators) or
///               11 characters (with separators) in length.
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
///               If the length of the value is 11 characters, then character
///               positions 4 and 7 (zero-based) must be valid separator
///               characters. Valid separator characters are any non-ASCII digit
///               characters ('0'-'9'). The same character must be used for both
///               separator characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing (right-most) character position must be a valid
///               check digit according to the variant modulus 11 algorithm.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The variant modulus 11 algorithm used for burgerservicenummer assigns a
///      weight of -1 to the check digit instead of the weight of 1 that is
///      normally used for modulus 11 check digits.
///   </para>
///   <para>
///      See https://nl.wikipedia.org/wiki/Burgerservicenummer (Dutch) for more
///      info.
///   </para>
/// </remarks>
[JsonConverter(typeof(NlBurgerservicenummerJsonConverter))]
public record NlBurgerservicenummer
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="NlBurgerservicenummer"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="NlBurgerservicenummer"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator)
   {
   }

   /// <summary>
   ///   The name of the check digit algorithm used by burgerservicenummer values.
   /// </summary>
   public const String CheckDigitAlgorithmName = "11-proef";

   private const Int32 UnformattedLength = 9;
   private const Int32 FormattedLength = 11;

   private const Int32 FirstSeparatorOffset = 4;
   private const Int32 SecondSeparatorOffset = 7;

   private const Int32 CheckDigitOffset = 1;    // Measured from end of value

   /// <summary>
   ///   Initializes a new instance of the <see cref="NlBurgerservicenummer"/>
   ///   class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Dutch burgerservicenummer.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty
   ///   or all whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 9 (or 11 if a
   ///   separator character is used).
   ///   - or -
   ///   <paramref name="value"/> contains a non-digit character
   ///   in any position other than the separator locations.
   ///   - or -
   ///   <paramref name="value"/> is 11 characters in length and
   ///   has an ASCII digit ('0'-'9') in a separator location
   ///   - or -
   ///   <paramref name="value"/> is 11 characters in length and
   ///   has two different separator characters.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid modulus 11
   ///   (11-proef) check digit in the trailing (right-most) character position.
   /// </exception>
   public NlBurgerservicenummer(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="NlBurgerservicenummer"/>
   ///   class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private NlBurgerservicenummer(String? value, ValidationMode validationMode)
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
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetRawValue(value!);
   }

   /// <summary>
   ///   Gets the raw burgerservicenummer value.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="NlBurgerservicenummer"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="NlBurgerservicenummer"/> to convert.
   /// </param>
   public static implicit operator String(NlBurgerservicenummer source)
      => source?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="NlBurgerservicenummer"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Dutch burgerservicenummer.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid burgerservicenummer.
   /// </exception>
   public static explicit operator NlBurgerservicenummer(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="NlBurgerservicenummer"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Dutch burgerservicenummer.
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{NlBurgerservicenummer, ValidationError}"/>. Will
   ///   contain the new <see cref="NlBurgerservicenummer"/> if <paramref name="value"/>
   ///   is valid or a <see cref="ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<NlBurgerservicenummer, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new NlBurgerservicenummer(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the burgerservicenummer using the supplied
   ///   <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then the default mask "____-__-___" will be used instead.
   /// </param>
   /// <returns>
   ///   A formatted burgerservicenummer.
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
   ///   details on creating a mask to format the burgerservicenummer.
   /// </remarks>
   public String Format(String mask = "____-__-___") => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the burgerservicenummer.
   /// </summary>
   /// <returns>
   ///   The raw burgerservicenummer, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it
   ///   contains a valid Dutch burgerservicenummer.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Dutch burgerservicenummer.
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
         return new InvalidLength(
            Messages.NlBurgerservicenummerInvalidLength,
            value.Length,
            GetValidLengthDefinitions());
      }

      // After performing basic checks, validate the check digit because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      ValidationResult validationResult = ValidateCheckDigit(value);
      if (validationResult is not ValidValue)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }

      if (!ValidateSeparators(value, out var invalidSeparatorPosition))
      {
         return new InvalidSeparator(
            Messages.NlBurgerservicenummerInvalidSeparator,
            value[invalidSeparatorPosition],
            invalidSeparatorPosition);
      }

      return default(ValidValue);
   }

   /// <summary>
   ///   Gets an array of details about valid lengths accepted for a
   ///   burgerservicenummer.
   /// </summary>
   /// <returns>
   ///   An array of <see cref="ValidLengthDefinition"/>s.
   /// </returns>
   internal static ValidLengthDefinition[] GetValidLengthDefinitions()
      =>
      [
         new ValidLengthDefinition(UnformattedLength, Messages.NlBurgerservicenummerUnformattedLength),
         new ValidLengthDefinition(FormattedLength, Messages.NlBurgerservicenummerFormattedLength),
      ];

   private static String GetRawValue(String value)
      => value.Length == UnformattedLength
         ? value
         : String.Concat(
            value.AsSpan(0, FirstSeparatorOffset),
            value.AsSpan(FirstSeparatorOffset + 1, 2),
            value.AsSpan(SecondSeparatorOffset + 1));

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> value)
      => value.Length == FormattedLength;

   private static ValidationResult ValidateCheckDigit(ReadOnlySpan<Char> value)
   {
      var sum = 0;
      var weight = 9;      // Weights applied left to right: 9, 8, 7, 6, 5, 4, 3, 2
      var isFormatted = IsFormatted(value);
      var processLength = value.Length - 1;      // Handle check digit separately

      for (var index = 0; index < processLength; index++)
      {
         if (isFormatted && (index is FirstSeparatorOffset or SecondSeparatorOffset))
         {
            continue;
         }

         var num = value[index] - Chars.DigitZero;
         if (!num.IsValidDigit())
         {
            return new InvalidCharacter(
               Messages.NlBurgerservicenummerInvalidCharacter,
               value[index],
               index);
         }

         sum += num * weight;
         weight--;
      }

      var checkDigit = value[^CheckDigitOffset] - Chars.DigitZero;
      if (!checkDigit.IsValidDigit())
      {
         return new InvalidCharacter(
            Messages.NlBurgerservicenummerInvalidCharacter,
            value[^CheckDigitOffset],
            value.Length - CheckDigitOffset);
      }

      sum -= checkDigit;      // Check digit weight = -1

      return sum % 11 == 0
         ? default(ValidValue)
         : new InvalidChecksum(
            Messages.NlBurgerservicenummerInvalidCheckDigit,
            CheckDigitAlgorithmName);
   }

   // A formatted burgerservicenummer must contain the same separator character
   // at the expected offsets. And the separator character must be a non-digit
   // character.
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
public class NlBurgerservicenummerJsonConverter : JsonConverter<NlBurgerservicenummer>
{
   public override NlBurgerservicenummer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new NlBurgerservicenummer(str);
   }

   public override void Write(Utf8JsonWriter writer, NlBurgerservicenummer value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
