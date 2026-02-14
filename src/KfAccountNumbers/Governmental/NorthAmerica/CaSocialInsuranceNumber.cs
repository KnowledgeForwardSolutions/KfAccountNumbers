// Ignore Spelling: Json Kf Luhn

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
///      creating a new  <see cref="CaSocialInsuranceNumber"/>, after determining
///      that a value consists of 9 decimal digits, the following validation 
///      rules are applied:
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
   private const Int32 FormattedLength = 11;
   private const Int32 NonFormattedLength = 9;

   private const Int32 FirstSeparatorOffset = 3;
   private const Int32 SecondSeparatorOffset = 7;

   /// <summary>
   ///   Initialize a new <see cref="CaSocialInsuranceNumber"/>.
   /// </summary>
   /// <param name="sin">
   ///   The string representation of a Social Insurance Number.
   /// </param>
   /// <exception cref="KfValidationException{CaSocialInsuranceNumberValidationResult}">
   ///   <paramref name="sin"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="sin"/> does not have length of 9 or 11.
   ///   - or -
   ///   <paramref name="sin"/> contains a non-ASCII digit (not 0-9).
   ///   - or -
   ///   <paramref name="sin"/> is 11 characters in length and contains an 
   ///   invalid separator character.
   ///   - or -
   ///   <paramref name="sin"/> fails the Luhn check digit validation.
   ///   - or -
   ///   <paramref name="sin"/> contains an invalid province code (first digit
   ///   may not be zero or eight).
   /// </exception>
   public CaSocialInsuranceNumber(String? sin)
      : this(sin, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has already
   ///   been validated.
   /// </summary>
   private CaSocialInsuranceNumber(String? sin, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         CaSocialInsuranceNumberValidationResult validationResult = Validate(sin);
         if (validationResult != CaSocialInsuranceNumberValidationResult.ValidationPassed)
         {
            throw validationResult.ToValidationException();
         }
      }
      Value = GetValidatedSin(sin!);
   }

   /// <summary>
   ///   The raw SIN value.
   /// </summary>
   public String Value { get; private init; }

   public static implicit operator String(CaSocialInsuranceNumber sin)
      => sin?.Value ?? String.Empty;     // Handle null SIN object gracefully by returning empty string

   // Explicit conversion from String to avoid unintentional conversions that may throw exceptions.
   public static explicit operator CaSocialInsuranceNumber(String? sin) => new(sin);

   /// <summary>
   ///   Create a new <see cref="CaSocialInsuranceNumber"/>.
   /// </summary>
   /// <param name="sin">
   ///   String representation of a Social Insurance Number.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{CaSocialInsuranceNumber, CaSocialInsuranceNumberValidationResult}"/>.
   ///   Will contain the new <see cref="CaSocialInsuranceNumber"/> if 
   ///   <paramref name="sin"/> is valid or 
   ///   <see cref="CaSocialInsuranceNumberValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="sin"/> is 
   ///   invalid.
   /// </returns>
   public static CreateResult<CaSocialInsuranceNumber, CaSocialInsuranceNumberValidationResult> Create(String? sin)
   {
      CaSocialInsuranceNumberValidationResult validationResult = Validate(sin);
      return validationResult == CaSocialInsuranceNumberValidationResult.ValidationPassed
         ? new CaSocialInsuranceNumber(sin, validationMode: ValidationMode.BypassValidation)
         : validationResult;
   }

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
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="sin"/> to determine if it contains a valid
   ///   Canadian Social Insurance Number (SIN).
   /// </summary>
   /// <param name="sin">
   ///   String representation of a Social Insurance Number.
   /// </param>
   /// <returns>
   ///   A <see cref="CaSocialInsuranceNumberValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="sin"/> passed validation
   ///   or what validation error was encountered.
   /// </returns>
   public static CaSocialInsuranceNumberValidationResult Validate(String? sin)
   {
      // Basic checks for empty/null and length and formatting.
      if (String.IsNullOrWhiteSpace(sin))
      {
         return CaSocialInsuranceNumberValidationResult.Empty;
      }
      else if (sin.Length is not NonFormattedLength and not FormattedLength)
      {
         return CaSocialInsuranceNumberValidationResult.InvalidLength;
      }
      else if (IsFormattedSin(sin) && !ValidateEmbeddedSeparatorCharacters(sin))
      {
         return CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered;
      }

      // Validate the check digit and province code.
      var validCheckDigit = IsFormattedSin(sin)
         ? CheckDigitAlgorithms.Luhn.Validate(sin, CheckDigitMasks.CaSocialInsuranceNumberMask)
         : CheckDigitAlgorithms.Luhn.Validate(sin);
      if (!validCheckDigit)
      {
         // Either invalid check digit or invalid character encountered.
         return ValidateDigits(sin)
            ? CaSocialInsuranceNumberValidationResult.InvalidCheckDigit
            : CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered;
      }
      else if (!ValidateProvince(sin))
      {
         return CaSocialInsuranceNumberValidationResult.InvalidProvince;
      }

      return CaSocialInsuranceNumberValidationResult.ValidationPassed;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static void CopySection(
      ReadOnlySpan<Char> source,
      Span<Char> target,
      Int32 section)
   {
      var start = section * 3;
      var end = (section + 1) * 3;
      var formattedOffset = IsFormattedSin(source) ? section : 0;

      ReadOnlySpan<Char> sourceSpan = source[(start + formattedOffset)..(end + formattedOffset)];
      Span<Char> targetSpan = target[start..end];

      sourceSpan.CopyTo(targetSpan);
   }

   /// <summary>
   ///   Get an unformatted SIN value from a string that has passed validation.
   ///   If the source string is formatted, then create a new string by merging
   ///   all three SIN sections together without allocating intermediate 
   ///   Strings.
   /// </summary>
   private static String GetValidatedSin(String sin)
   {
      if (sin.Length == NonFormattedLength)
      {
         return sin;
      }

      var buffer = ArrayPool<Char>.Shared.Rent(NonFormattedLength);
      try
      {
         var span = new Span<Char>(buffer);

         CopySection(sin, span, 0);
         CopySection(sin, span, 1);
         CopySection(sin, span, 2);

         return span[..NonFormattedLength].ToString();
      }
      finally
      {
         ArrayPool<Char>.Shared.Return(buffer);
      }
   }

   private static Boolean IsFormattedSin(ReadOnlySpan<Char> sin) => sin.Length == FormattedLength;

   private static Boolean ValidateDigits(ReadOnlySpan<Char> sin)
   {
      var isFormatted = IsFormattedSin(sin);
      for (var index = 0; index < sin.Length; index++)
      {
         if (isFormatted && (index is FirstSeparatorOffset or SecondSeparatorOffset))
         {
            continue;
         }
         if (!sin[index].IsAsciiDigit())
         {
            return false;
         }
      }

      return true;
   }

   // A formatted SIN must contain the same separator character at the expected
   // offsets. And the separator character must be a non-digit character.
   private static Boolean ValidateEmbeddedSeparatorCharacters(ReadOnlySpan<Char> sin)
   {
      var firstSeparator = sin[FirstSeparatorOffset];
      var secondSeparator = sin[SecondSeparatorOffset];

      return firstSeparator == secondSeparator && !firstSeparator.IsAsciiDigit();
   }

   private static Boolean ValidateProvince(ReadOnlySpan<Char> sin)
      => sin[0] is not Chars.DigitZero and not Chars.DigitEight;
}

public class CaSocialInsuranceNumberJsonConverter : JsonConverter<CaSocialInsuranceNumber>
{
   public override CaSocialInsuranceNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var sinString = reader.GetString();
      return new CaSocialInsuranceNumber(sinString);
   }

   public override void Write(Utf8JsonWriter writer, CaSocialInsuranceNumber value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
