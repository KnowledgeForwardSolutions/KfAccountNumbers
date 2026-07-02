// Ignore Spelling: Json

#pragma warning disable IDE0250 // Make struct 'readonly'

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents an Irish Personal Public
///   Service Number (PPS Number or PPSN). The PPS Number is Ireland's national
///   identification number.
/// </summary>
/// <remarks>
///   <para>
///      An Irish PPS Number consists of seven digits followed by an alphabetic
///      check character and sometimes one additional letter. The optional
///      second letter was made permanent in 2013 to allow for expansion of the
///      number of PPS numbers issued. A PPS Number is structured as DDDDDDDC or
///      DDDDDDDCE where:
///      <list type="bullet">
///         <item>
///            <term>D</term>
///            <description>
///               is a digit (0-9).
///            </description>
///         </item>
///         <item>
///            <term>C</term>
///            <description>
///               is an alphabetic character representing the weighted modulus
///               23 check character calculated from the previous seven digits
///               and the second letter, if present.
///            </description>
///         </item>
///         <item>
///            <term>E</term>
///            <description>
///               An optional letter in the range of A-I or W, made permanent in
///               2013 to expand the available number space for PPS number
///               issuance.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      An Irish PPS Number is typically displayed as a single string of eight
///      or nine characters, without any separator characters.
///   </para>
///   <para>
///      When creating a new <see cref="IePpsNumber"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 8 or 9 characters in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               The characters in positions 0-6 (zero-based) must be ASCII
///               digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The character in position 7 (zero-based) must be a valid
///               weighted-modulus 23 check character in the range of A-W.
///            </description>
///         </item>
///         <item>
///            <description>
///               The character in position 8 (zero-based), if present, must be
///               a letter in the range of A-I or W.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      <see cref="IePpsNumber"/> is case-insensitive and will accept both
///      upper-case and lower-case letters in the two trailing positions however
///      the value will be normalized to upper-case internally.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>1234567T</term>
///            <description>Check character = T</description>
///         </item>
///         <item>
///            <term>1234567FA</term>
///            <description>Check character = F, second letter = A</description>
///         </item>
///         <item>
///            <term>7654321PA</term>
///            <description>Check character = P, second letter = A</description>
///         </item>
///         <item>
///            <term>9876543QA</term>
///            <description>Check character = Q, second letter = A</description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/Personal_Public_Service_Number for more info.
///   </para>
/// </remarks>
[JsonConverter(typeof(IePpsNumberJsonConverter))]
public record IePpsNumber
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="IePpsNumber"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="IePpsNumber"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum)
   {
   }

   /// <summary>
   ///   The name of the check digit algorithm used by PPS numbers.
   /// </summary>
   public const String CheckDigitAlgorithmName = "Weighted Modulus 23";

   private const Int32 OriginalLength = 8;
   private const Int32 ExtendedLength = 9;

   private const Int32 CheckCharacterOffset = 7;
   private const Int32 TrailingCharacterOffset = 8;

   private const String CheckCharacters = "WABCDEFGHIJKLMNOPQRSTUV";    // Note leading W because W = 0 and A = 1, B = 2, ...

   /// <summary>
   ///   Initializes a new instance of the <see cref="IePpsNumber"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of an Irish Personal Public Service Number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 8 or length 9.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid character. The
   ///   characters in positions 0-6 (zero-based) must be ASCII digits ('0'-'9').
   ///   The character in position 7 (zero-based) must be an uppercase or
   ///   lowercase letter in the range A-W. The character in position 8
   ///   (zero-based), if present, must be a letter in the range of A-I or W.
   ///   - or -
   ///   <paramref name="value"/> has invalid modulus 23 check character
   ///   in the character position 7 (zero-based). Valid characters are
   ///   uppercase or lowercase letters in the range A-W.  (where W represents a
   ///   remainder of 0, A = remainder of 1, B = remainder of 2,...).
   /// </exception>
   public IePpsNumber(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="IePpsNumber"/> class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private IePpsNumber(String? value, ValidationMode validationMode)
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
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetRawValue(value!);
   }

   /// <summary>
   ///   Gets the raw PPS Number value, normalized to upper-case.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="IePpsNumber"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="IePpsNumber"/> to convert.
   /// </param>
   public static implicit operator String(IePpsNumber source)
      => source?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="IePpsNumber"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of an Irish Personal Public Service Number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid PPS number.
   /// </exception>
   public static explicit operator IePpsNumber(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="IePpsNumber"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of an Irish Personal Public Service Number.
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{IePpsNumber, ValidationError}"/>. Will
   ///   contain the new <see cref="IePpsNumber"/> if <paramref name="value"/>
   ///   is valid or a <see cref="ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<IePpsNumber, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new IePpsNumber(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Get a string representation of the PPS number.
   /// </summary>
   /// <returns>
   ///   The raw PPS number, normalized to upper-case.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Irish Personal Public Service Number.
   /// </summary>
   /// <param name="value">
   ///   String representation of an Irish Personal Public Service Number.
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
      else if (value.Length is not OriginalLength and not ExtendedLength)
      {
         return new InvalidLength(
            Messages.IePpsNumberInvalidLength,
            value.Length,
            GetValidLengthDefinitions());
      }

      // Validate check digit will validate for invalid characters during the
      // process of calculating the check digit. No further checks so simply
      // return the result of ValidateCheckDigit.
      return ValidateCheckDigit(value);
   }

   /// <summary>
   ///   Gets an array of details about valid lengths accepted for a PPS number.
   /// </summary>
   /// <returns>
   ///   An array of <see cref="ValidLengthDefinition"/>s.
   /// </returns>
   internal static ValidLengthDefinition[] GetValidLengthDefinitions()
      =>
      [
         new ValidLengthDefinition(OriginalLength, Messages.IePpsNumberWithoutSuffixLength),
         new ValidLengthDefinition(ExtendedLength, Messages.IePpsNumberWithSuffixLength),
      ];

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(
         Messages.IePpsNumberInvalidCharacter,
         value[position],
         position);

   // If already uppercase then return original value, otherwise normalize to uppercase.
   private static String GetRawValue(String value)
      => Char.IsLower(value[CheckCharacterOffset])
         || (value.Length == ExtendedLength && Char.IsLower(value[TrailingCharacterOffset]))
         ? value.ToUpperInvariant()
         : value;

   private static ValidationResult ValidateCheckDigit(ReadOnlySpan<Char> value)
   {
      var sum = 0;
      var weight = 8;

      // Process leading seven digits.
      for (var index = 0; index < CheckCharacterOffset; index++)
      {
         var num = value[index].ToSingleDigit();
         if (!num.IsValidDigit())
         {
            return GetInvalidCharacterResult(value, index);
         }

         sum += num * weight;
         weight--;
      }

      // Handle optional trailing character.
      if (value.Length == ExtendedLength)
      {
         var trailingCharacter = Char.ToUpper(value[TrailingCharacterOffset], CultureInfo.InvariantCulture);
         var trailingCharacterValue = trailingCharacter switch
         {
            >= Chars.UpperCaseA and <= Chars.UpperCaseI => trailingCharacter - Chars.UpperCaseA + 1,
            Chars.UpperCaseW => 0,
            _ => -1,
         };
         if (trailingCharacterValue < 0)
         {
            return GetInvalidCharacterResult(value, TrailingCharacterOffset);
         }

         sum += trailingCharacterValue * 9;        // Trailing character has fixed weight = 9
      }

      // Handle possible invalid check character.
      var checkCharacter = Char.ToUpper(value[CheckCharacterOffset], CultureInfo.InvariantCulture);
      if (checkCharacter is < Chars.UpperCaseA or > Chars.UpperCaseW)
      {
         return GetInvalidCharacterResult(value, CheckCharacterOffset);
      }

      var remainder = sum % 23;
      return checkCharacter == CheckCharacters[remainder]
         ? default(ValidValue)
         : new InvalidChecksum(
            Messages.IePpsNumberInvalidCheckDigit,
            CheckDigitAlgorithmName);
   }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class IePpsNumberJsonConverter : JsonConverter<IePpsNumber>
{
   public override IePpsNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new IePpsNumber(str);
   }

   public override void Write(Utf8JsonWriter writer, IePpsNumber value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
