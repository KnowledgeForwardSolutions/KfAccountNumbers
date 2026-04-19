// Ignore Spelling: Nif

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a Spanish Número de Identificación
///   Fiscal (NIF). NIF may be either of two different values, a documento nacional
///   de identidad (DNI) issued to Spanish citizens or a número de identificación de
///   extranjero (NIE) issued to foreigners residing in Spain.
/// </summary>
/// <remarks>
///   <para>
///      DNI and NIE are both nine-digit numbers with similar, but slightly
///      different structures. A DNI has the structure DDDDDDDDC while a NIE
///      uses PDDDDDDDC, where:
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
///               is an alphabetic character representing the modulus 23 check digit
///               calculated from the previous eight digits.
///            </description>
///         </item>
///         <item>
///            <term>P</term>
///            <description>
///               is one of the letters X, Y or Z (when calculating the check digit,
///               X = 0, Y = 1 and Z = 2).
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The only difference between a DNI and a NIE is if the leading (left-most)
///      character is a digit or the letter X, Y or Z. Both values may be formatted
///      as a sequence of nine characters or may be formatted for greater readability
///      by using    separators. For a DNI, a separator (generally a dash '-') is
///      placed between the digits and the trailing alphabetic character. For a NIE,
///      separators are placed between the leading letter and the digits, and between
///      the digits and the trailing alphabetic character.
///   </para>
///   <para>
///      When creating a new <see cref="EsNif"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be 9 characters in length (without separators) or
///               10 characters (DNI with one separator) or 11 characters (NIE with
///               two separators).
///            </description>
///         </item>
///         <item>
///            <description>
///               All characters other than the leading and trailing characters
///               (and the optional separators) must be ASCII digits ('0'-'9'). The
///               leading character must be either an ASCII digit or X, Y, or Z.
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing character must be a valid modulus 23 check character.
///               Valid characters are "TRWAGMYFPDXBNJZSQVHLCKE" (where T represents a
///               remainder of 0 and E represents a remainder of 22).
///            </description>
///         </item>
///         <item>
///            <description>
///               The optional separator character(s), if included, may not be an ASCII
///               digit. Any non-digit character is allowed as a separator. For a DNI,
///               the separator must be in character position 8 (zero-based). For a NIE,
///               the separators must be in character positions 1 and 9 (zero-based) and
///               both separator characters must be the same.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>12345678Z</term>
///            <description>DNI</description>
///         </item>
///         <item>
///            <term>17110804680</term>
///            <term>50487563-X</term>
///            <description>DNE</description>
///         </item>
///         <item>
///            <term>X1234567L</term>
///            <description>NIF</description>
///         </item>
///         <item>
///            <term>Y-7654321-G</term>
///            <description>NIE</description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/National_Identity_Card_(Spain) and
///      https://es.wikipedia.org/wiki/N%C3%BAmero_de_identificaci%C3%B3n_fiscal (Spanish)
///      for more info.
///   </para>
/// </remarks>
public record EsNif
{
   private const Int32 UnformattedLength = 9;
   private const Int32 DniFormmattedLength = 10;
   private const Int32 NieFormattedLength = 11;

   private const String CheckCharacters = "TRWAGMYFPDXBNJZSQVHLCKE";
   private static readonly HashSet<Char> ValidCheckCharacters = CheckCharacters.ToHashSet();

   /// <summary>
   ///   Check the <paramref name="nif"/> to determine if it contains a
   ///   valid Spanish Número de Identificación Fiscal (NIF).
   /// </summary>
   /// <param name="nif">
   ///   String representation of a Spanish Número de Identificación Fiscal (NIF).
   /// </param>
   /// <returns>
   ///   A <see cref="EsNifValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="nif"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static EsNifValidationResult Validate(String? nif)
   {
      if (String.IsNullOrWhiteSpace(nif))
      {
         return EsNifValidationResult.Empty;
      }
      else if (!ValidateLength(nif))
      {
         return EsNifValidationResult.InvalidLength;
      }

      // After performing basic checks, validate the check digit because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      EsNifValidationResult validationResult = ValidateCheckDigit(nif);
      if (validationResult != EsNifValidationResult.ValidationPassed)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }
      //else if (!ValidateSeparators(nif))
      //{
      //   return EsNifValidationResult.InvalidSeparator;
      //}
      //else if (!ValidateSequenceNumber(nif))
      //{
      //   return EsNifValidationResult.InvalidSequenceNumber;
      //}
      //else if (!ValidateDateOfBirth(nif))
      //{
      //   return EsNifValidationResult.InvalidDateOfBirth;
      //}

      return EsNifValidationResult.ValidationPassed;
   }

   private static EsNifValidationResult ValidateCheckDigit(ReadOnlySpan<Char> nif)
   {
      // Process leading character outside main loop.
      var leadingCharacter = nif[0];
      var num = leadingCharacter.ToSingleDigit();
      if (!num.IsValidDigit())
      {
         // Handle possible NIE.
         num = leadingCharacter - Chars.UpperCaseX;
         if (num is < 0 or > 2)              // X = 0, Y = 1, Z = 2
         {
            return EsNifValidationResult.InvalidCharacter;
         }
      }
      var sum = num;

      // Handle inner digits.
      var start = nif.Length == NieFormattedLength ? 2 : 1;
      var end = nif.Length == NieFormattedLength ? 9 : 8;
      for(var index = start; index < end; index++)
      {
         sum *= 10;
         num = nif[index].ToSingleDigit();
         if (!num.IsValidDigit())
         {
            return EsNifValidationResult.InvalidCharacter;
         }

         sum += num;
      }

      var remainder = sum % 23;
      var checkCharacter = CheckCharacters[remainder];
      var trailingCharacter = nif[^1];
      if (trailingCharacter == checkCharacter)
      {
         return EsNifValidationResult.ValidationPassed;
      }

      // If check character doesn't match, check for character not in
      // set of valid check characters.
      return ValidCheckCharacters.Contains(trailingCharacter)
         ? EsNifValidationResult.InvalidCheckDigit
         : EsNifValidationResult.InvalidCharacter;
   }

   private static Boolean ValidateLength(ReadOnlySpan<Char> nif)
   {
      var isLeadingDigit = nif[0].IsAsciiDigit();

      return nif.Length == UnformattedLength
         || (isLeadingDigit && nif.Length == DniFormmattedLength)
         || (!isLeadingDigit && nif.Length == NieFormattedLength);
   }

}
