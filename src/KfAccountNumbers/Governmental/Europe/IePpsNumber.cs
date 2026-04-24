// Ignore Spelling: Json

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents an Irish Personal Public
///   Service Number (PPS Number or PPSN). The PPS Number is Ireland's national
///   identification number.
/// </summary>
/// <remarks>
///   <para>
///      An Irish PPS Number consists of seven digits followed by an alphabetic
///      check character and sometimes one additional letter. The optional second
///      letter was made permanent in 2013 to allow for expansion of the number of
///      PPS numbers issued. A PPS Number is structured as DDDDDDDC or DDDDDDDCE
///      where:
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
///               is an alphabetic character representing the weighted modulus 23
///               check character calculated from the previous seven digits and
///               the second letter, if present.
///            </description>
///         </item>
///         <item>
///            <term>E</term>
///            <description>
///               An optional letter in the range of A-I or W, made permanent in
///               2013 to expand the available number space for PPS number issuance.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      An Irish PPS Number is typically displayed as a single string of eight or
///      nine characters, without any separator characters.
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
///               The characters in positions 0-6 (zero-based) must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The character in position 7 (zero-based) must be a valid weighted modulus 23
///               check character in the range of A-W.
///            </description>
///         </item>
///         <item>
///            <description>
///               The character in position 8 (zero-based), if present, must be a letter in
///               the range of A-I or W.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      <see cref="IePpsNumber"/> is case-insensitive and will accept both upper-case and
///      lower-case letters in the two trailing positions however the value will be
///      normalized to upper-case internally.
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
   private const Int32 OriginalLength = 8;
   private const Int32 ExtendedLength = 9;

   private const Int32 CheckCharacterOffset = 7;
   private const Int32 TrailingCharacterOffset = 8;

   private const String CheckCharacters = "WABCDEFGHIJKLMNOPQRSTUV";    // Note leading W because W = 0 and A = 1, B = 2, ...

   /// <summary>
   ///   Initialize a new instance of the <see cref="IePpsNumber"/> class.
   /// </summary>
   /// <param name="ppsNumber">
   ///   String representation of an Irish Personal Public Service Number.
   /// </param>
   /// <exception cref="KfValidationException{IePpsNumberValidationResult}">
   ///   <paramref name="ppsNumber"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="ppsNumber"/> is not length 8 or length 9.
   ///   - or -
   ///   <paramref name="ppsNumber"/> contains an invalid character. The characters
   ///   in positions 0-6 (zero-based) must be ASCII digits ('0'-'9'). The character
   ///   in position 7 (zero-based) must be an uppercase or lowercase letter in the
   ///   range A-W. The character in position 8 (zero-based), if present, must be a
   ///   letter in the range of A-I or W.
   ///   - or -
   ///   <paramref name="ppsNumber"/> has invalid modulus 23 check character
   ///   in the character position 7 (zero-based). Valid characters are 
   ///   uppercase or lowercase letters in the range A-W.  (where W represents a
   ///   remainder of 0, A = remainder of 1, B = remainder of 2,...). 
   /// </exception>
   public IePpsNumber(String? ppsNumber)
      : this(ppsNumber, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has already
   ///   been validated.
   /// </summary>
   private IePpsNumber(String? ppsNumber, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         IePpsNumberValidationResult validationResult = Validate(ppsNumber);
         if (validationResult != IePpsNumberValidationResult.ValidationPassed)
         {
            throw validationResult.ToValidationException();
         }
      }

      Value = GetRawValue(ppsNumber!);
   }

   /// <summary>
   ///   The raw PPS Number value, normalized to upper-case.
   /// </summary>
   public String Value { get; private init; }

   public static implicit operator String(IePpsNumber ppsNumber)
      => ppsNumber?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   // Explicit conversion from String to avoid unintentional conversions that may throw exceptions.
   public static explicit operator IePpsNumber(String? ppsNumber) => new(ppsNumber);

   /// <summary>
   ///   Create a new <see cref="IePpsNumber"/> using the Result pattern.
   /// </summary>
   /// <param name="ppsNumber">
   ///   String representation of an Irish Personal Public Service Number.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{IePpsNumber, IePpsNumberValidationResult}"/>.
   ///   Will contain the new <see cref="IePpsNumber"/> if 
   ///   <paramref name="ppsNumber"/> is valid or an
   ///   <see cref="IePpsNumberValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="ppsNumber"/> is 
   ///   invalid.
   /// </returns>
   public static CreateResult<IePpsNumber, IePpsNumberValidationResult> Create(String? ppsNumber)
   {
      IePpsNumberValidationResult validationResult = Validate(ppsNumber);
      return validationResult == IePpsNumberValidationResult.ValidationPassed
         ? new IePpsNumber(ppsNumber, validationMode: ValidationMode.BypassValidation)
         : validationResult;
   }

   /// <summary>
   ///   Get a string representation of the PPS number.
   /// </summary>
   /// <remarks>
   ///   Will return the raw PPS number, normalized to upper-case.
   /// </remarks>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="ppsNumber"/> to determine if it contains a
   ///   valid Irish Personal Public Service Number.
   /// </summary>
   /// <param name="ppsNumber">
   ///   String representation of an Irish Personal Public Service Number.
   /// </param>
   /// <returns>
   ///   A <see cref="IePpsNumberValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="ppsNumber"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static IePpsNumberValidationResult Validate(String? ppsNumber)
   {
      if (String.IsNullOrWhiteSpace(ppsNumber))
      {
         return IePpsNumberValidationResult.Empty;
      }
      else if (ppsNumber.Length != OriginalLength && ppsNumber.Length != ExtendedLength)
      {
         return IePpsNumberValidationResult.InvalidLength;
      }

      // Validate check digit will validate for invalid characters during the
      // process of calculating the check digit. No further checks so simply
      // return the result of ValidateCheckDigit.
      return ValidateCheckDigit(ppsNumber);
   }

   // If already uppercase then return original value, otherwise normalize to uppercase.
   private static String GetRawValue(String ppsNumber)
      => Char.IsLower(ppsNumber[CheckCharacterOffset])
         || (ppsNumber.Length == ExtendedLength && Char.IsLower(ppsNumber[TrailingCharacterOffset]))
         ? ppsNumber.ToUpperInvariant()
         : ppsNumber;

   private static IePpsNumberValidationResult ValidateCheckDigit(ReadOnlySpan<Char> ppsNumber)
   {
      var sum = 0;
      var weight = 8;

      // Process leading seven digits.
      for (var index = 0; index < CheckCharacterOffset; index ++)
      {
         var num = ppsNumber[index].ToSingleDigit();
         if (!num.IsValidDigit())
         {
            return IePpsNumberValidationResult.InvalidCharacter;
         }

         sum += (num * weight);
         weight--;
      }

      // Handle optional trailing character.
      if (ppsNumber.Length == ExtendedLength)
      {
         var trailingCharacter = Char.ToUpper(ppsNumber[TrailingCharacterOffset], CultureInfo.InvariantCulture);
         var trailingCharacterValue = trailingCharacter switch
         {
            >= Chars.UpperCaseA and <= Chars.UpperCaseI => (trailingCharacter - Chars.UpperCaseA) + 1,
            Chars.UpperCaseW => 0,
            _ => -1
         };
         if (trailingCharacterValue < 0)
         {
            return IePpsNumberValidationResult.InvalidCharacter;
         }

         sum += trailingCharacterValue * 9;        // Trailing character has fixed weight = 9
      }

      // Handle possible invalid check character.
      var checkCharacter = Char.ToUpper(ppsNumber[CheckCharacterOffset], CultureInfo.InvariantCulture);
      if (checkCharacter is < Chars.UpperCaseA or > Chars.UpperCaseW)
      {
         return IePpsNumberValidationResult.InvalidCharacter;
      }

      var remainder = sum % 23;
      return checkCharacter == CheckCharacters[remainder]
         ? IePpsNumberValidationResult.ValidationPassed
         : IePpsNumberValidationResult.InvalidCheckDigit;
   }
}

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
