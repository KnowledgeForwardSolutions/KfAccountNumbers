// Ignore Spelling: npi

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
/// </remarks>
public record struct UsNationalProviderIdentifier
{
   private const Int32 ValidLength = 10;

   /// <summary>
   ///   Initialize a new <see cref="UsNationalProviderIdentifier"/>.
   /// </summary>
   /// <param name="npi">
   ///   The string representation of a US National Provider Identifier.
   /// </param>
   /// <exception cref="InvalidUsNationalProviderIdentifierException">
   ///   <paramref name="npi"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="npi"/> does not have length of 10.
   ///   - or -
   ///   <paramref name="npi"/> contains a non-ASCII digit (not 0-9).
   ///   - or -
   ///   <paramref name="npi"/> fails the Luhn check digit validation (after
   ///   prefixing with "80840").
   /// </exception>
   public UsNationalProviderIdentifier(String? npi)
   {
      UsNationalProviderIdentifierValidationResult validationResult = Validate(npi);
      if (validationResult is not UsNationalProviderIdentifierValidationResult.ValidationPassed)
      {
         throw new InvalidUsNationalProviderIdentifierException(validationResult);
      }

      Value = npi!;
   }

   /// <summary>
   ///   The raw NPI value.
   /// </summary>
   public String Value { get; init; }

   public static implicit operator String(UsNationalProviderIdentifier npi)
      => npi.Value ?? throw new ArgumentNullException(nameof(npi), Messages.UsNationalProviderIdentifierInvalidDefaultConversionToString);

   public static implicit operator UsNationalProviderIdentifier(String? npi) => new(npi);

   /// <summary>
   ///   Check the <paramref name="npi"/> to determine if it contains any 
   ///   validation errors.
   /// </summary>
   /// <param name="npi">
   ///   String representation of a US National Provider Identifier.
   /// </param>
   /// <returns>
   ///   A <see cref="UsNationalProviderIdentifierValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="npi"/> passed validation
   ///   or what validation error was encountered.
   /// </returns>
   public static UsNationalProviderIdentifierValidationResult Validate (String? npi)
   {

      // Basic checks for empty/null and length and formatting.
      if (String.IsNullOrWhiteSpace(npi))
      {
         return UsNationalProviderIdentifierValidationResult.Empty;
      }
      else if (npi.Length != ValidLength)
      {
         return UsNationalProviderIdentifierValidationResult.InvalidLength;
      }

      // Validate the check digit (and by extension, that all characters are digits).
      if (!Algorithms.Npi.Validate(npi))
      {
         return ValidateDigits(npi)
            ? UsNationalProviderIdentifierValidationResult.InvalidCheckDigit
            : UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered;
      }

      return UsNationalProviderIdentifierValidationResult.ValidationPassed;
   }

   private static Boolean ValidateDigits(String npi)
   {
      for (var index = 0; index < npi.Length; index++)
      {
         if (!npi[index].IsAsciiDigit())
         {
            return false;
         }
      }
      return true;
   }

}
