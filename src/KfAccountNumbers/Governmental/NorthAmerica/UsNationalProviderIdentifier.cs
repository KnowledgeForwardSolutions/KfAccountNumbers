// Ignore Spelling: Json Kf npi

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Strongly typed business object for a US National Provider Identifier (NPI),
///   a unique 10-digit identification number assigned to healthcare providers
///   in the United States.
/// </summary>
/// <remarks>
///   <para>
///      A valid US National Provider Identifier (NPI) consists of exactly 10
///      decimal digits. The trailing (right-most) digit is a check digit
///      calculated using a slight variation of the Luhn algorithm (the value is
///      prefixed by the constant "80840" before applying the Luhn algorithm).
///   </para>
///   <para>
///      When validating an NPI, the following rules apply:
///      <list type="bullet">
///         <item>
///            <description>
///               The value must be exactly 10 characters in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               Each character must be a decimal digit (0-9).
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing (right-most) digit must be a valid check digit
///               according to the Luhn algorithm, with the value prefixed by
///               "80840" before applying the algorithm.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See <see href="https://en.wikipedia.org/wiki/National_Provider_Identifier">Wikipedia - National Provider Identifier</see>
///      for more details.
///   </para>
/// </remarks>
[JsonConverter(typeof(UsNationalProviderIdentifierJsonConverter))]
public record UsNationalProviderIdentifier
{
   private const Int32 ValidLength = 10;

   /// <summary>
   ///   Initializes a new instance of the
   ///   <see cref="UsNationalProviderIdentifier"/> class.
   /// </summary>
   /// <param name="value">
   ///   The string representation of a US National Provider Identifier.
   /// </param>
   /// <exception cref="KfValidationException{UsNationalProviderIdentifierValidationResult}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> does not have length of 10.
   ///   - or -
   ///   <paramref name="value"/> contains a non-ASCII digit (not 0-9).
   ///   - or -
   ///   <paramref name="value"/> fails the Luhn check digit validation (after
   ///   prefixing with "80840").
   /// </exception>
   public UsNationalProviderIdentifier(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the
   ///   <see cref="UsNationalProviderIdentifier"/> class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private UsNationalProviderIdentifier(String? value, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         UsNationalProviderIdentifierValidationResult validationResult = Validate(value);
         if (validationResult != UsNationalProviderIdentifierValidationResult.ValidationPassed)
         {
            throw validationResult.ToValidationException();
         }
      }

      Value = value!;
   }

   /// <summary>
   ///   Gets the raw NPI value.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="UsNationalProviderIdentifier"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="UsNationalProviderIdentifier"/> to convert.
   /// </param>
   public static implicit operator String(UsNationalProviderIdentifier source)
      => source?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a
   ///   <see cref="UsNationalProviderIdentifier"/>.
   /// </summary>
   /// <param name="value">
   ///   The string representation of a US National Provider Identifier.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid SSN.
   /// </exception>
   public static explicit operator UsNationalProviderIdentifier(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="UsNationalProviderIdentifier"/> using the
   ///   Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a US National Provider Identifier.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{UsNationalProviderIdentifier, UsNationalProviderIdentifierValidationResult}"/>.
   ///   Will contain the new <see cref="UsNationalProviderIdentifier"/> if
   ///   <paramref name="value"/> is valid or 
   ///   <see cref="UsNationalProviderIdentifierValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="value"/> is
   ///   invalid.
   /// </returns>
   public static CreateResult<UsNationalProviderIdentifier, UsNationalProviderIdentifierValidationResult> Create(
      String? value)
   {
      UsNationalProviderIdentifierValidationResult validationResult = Validate(value);

      return validationResult is UsNationalProviderIdentifierValidationResult.ValidationPassed
         ? new UsNationalProviderIdentifier(value, validationMode: ValidationMode.BypassValidation)
         : validationResult;
   }

   /// <summary>
   ///   Get a string representation of the NPI.
   /// </summary>
   /// <returns>
   ///   A string containing the NPI value.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a valid
   ///   US National Provider Identifier (NPI).
   /// </summary>
   /// <param name="value">
   ///   String representation of a US National Provider Identifier.
   /// </param>
   /// <returns>
   ///   A <see cref="UsNationalProviderIdentifierValidationResult"/> enumeration
   ///   value that indicates if the <paramref name="value"/> passed validation
   ///   or what validation error was encountered.
   /// </returns>
   public static UsNationalProviderIdentifierValidationResult Validate(String? value)
   {
      // Basic checks for empty/null and length and formatting.
      if (String.IsNullOrWhiteSpace(value))
      {
         return UsNationalProviderIdentifierValidationResult.Empty;
      }
      else if (value.Length != ValidLength)
      {
         return UsNationalProviderIdentifierValidationResult.InvalidLength;
      }

      // Validate the check digit (and by extension, that all characters are digits).
      if (Algorithms.Npi.Validate(value))
      {
         return UsNationalProviderIdentifierValidationResult.ValidationPassed;
      }

      // Invalid check digit could be due to either an invalid character or an incorrect
      // check digit. Check if all characters are digits to determine which validation
      // error to return.
      return ValidateDigits(value)
            ? UsNationalProviderIdentifierValidationResult.InvalidCheckDigit
            : UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered;
   }

   private static Boolean ValidateDigits(ReadOnlySpan<Char> value)
   {
      foreach (var ch in value)
      {
         if (!ch.IsAsciiDigit())
         {
            return false;
         }
      }

      return true;
   }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class UsNationalProviderIdentifierJsonConverter : JsonConverter<UsNationalProviderIdentifier>
{
   public override UsNationalProviderIdentifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new UsNationalProviderIdentifier(str);
   }

   public override void Write(Utf8JsonWriter writer, UsNationalProviderIdentifier value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
