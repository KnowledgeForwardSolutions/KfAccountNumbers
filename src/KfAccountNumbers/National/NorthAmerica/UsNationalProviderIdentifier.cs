// Ignore Spelling: Json Kf

#pragma warning disable IDE0250 // Make struct 'readonly'

namespace KfAccountNumbers.National.NorthAmerica;

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
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="UsNationalProviderIdentifier"/>.
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
   ///   validating a <see cref="UsNationalProviderIdentifier"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum)
   {
   }

   private const Int32 ValidLength = 10;

   /// <summary>
   ///   Initializes a new instance of the
   ///   <see cref="UsNationalProviderIdentifier"/> class.
   /// </summary>
   /// <param name="value">
   ///   The string representation of a US National Provider Identifier.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
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
   ///   <paramref name="value"/> is not a valid NPI.
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
   ///   A <see cref="CreateResult{UsNationalProviderIdentifier, ValidationError}"/>. Will
   ///   contain the new <see cref="UsNationalProviderIdentifier"/> if
   ///   <paramref name="value"/> is valid or a <see cref="ValidationError"/>
   ///   that identifies the validation rule that was failed if
   ///   <paramref name="value"/> is invalid.
   /// </returns>
   public static CreateResult<UsNationalProviderIdentifier, ValidationError> Create(
      String? value)
      => Validate(value) switch
      {
         ValidValue => new UsNationalProviderIdentifier(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

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
   ///   A <see cref="ValidationResult"/> union that indicates if the
   ///   <paramref name="value"/> passed validation or what validation error was
   ///   encountered.
   /// </returns>
   public static ValidationResult Validate(String? value)
   {
      // Basic checks for empty/null and length and formatting.
      if (String.IsNullOrWhiteSpace(value))
      {
         return default(EmptyValue);
      }
      else if (value.Length != ValidLength)
      {
         return new InvalidLength(
            Messages.UsNpiInvalidLength,
            value.Length,
            new ValidLengthDefinition(ValidLength, Messages.UsNpiValidLength));
      }

      // Validate the check digit (and by extension, that all characters are
      // digits).
      if (Algorithms.Npi.Validate(value))
      {
         return default(ValidValue);
      }

      // Invalid check digit could be due to either an invalid character or
      // an incorrect check digit. Check if all characters are digits to
      // determine which validation error to return.
      var invalidCharacterPosition = LocateInvalidCharacter(value);
      return invalidCharacterPosition == -1
         ? new InvalidChecksum(Messages.UsNpiInvalidCheckDigit, Algorithms.Luhn.AlgorithmName)
         : new InvalidCharacter(
            Messages.UsNpiInvalidCharacter,
            value[invalidCharacterPosition],
            invalidCharacterPosition);
   }

   // Return the zero-based index of the first non-digit character or -1 if no
   // non-digit characters found.
   private static Int32 LocateInvalidCharacter(ReadOnlySpan<Char> value)
   {
      for (var index = 0; index < value.Length; index++)
      {
         if (!Char.IsAsciiDigit(value[index]))
         {
            return index;
         }
      }

      return -1;
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
