#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.National.Europe;

/// <summary>
///   <para>
///      Strongly typed business object that represents a Fh-nummer (Felles
///      Hjelpenummer or Common Help Number). A Fh-nummer is similar to a
///      Norwegian H-nummer, an identifier issued to persons needing medical
///      assistance and who do not have a fødselsnummer or a D-nummer such as
///      tourists, newborns or persons with unknown identities.
///   </para>
///   <para>
///      Unlike H-nummers which are issued by a single organization and only
///      unique within that organization, Fh-nummers are issued by Norsk
///      Helsenett (the Norwegian Health Network) and are unique across the
///      entire Norwegian health system. Unlike other Norwegian identity numbers
///      (fødselsnummer, D-nummer and H-nummer), Fh-nummers do not encode
///      the person's date of birth or gender and consist of 9 random digits and
///      two check digits. Fh-nummers are distinguished by an initial digit = 8
///      or 9.
///   </para>
///   <para>
///      <b>Note:</b> See <see cref="NoFoedselsnummer"/> and
///      <see cref="NoDnummer"/> for a similar identifiers (fødselsnummer,
///      D-nummer) and <see cref="NoIdentityNumber"/> for a composite type that
///      can represent either a fødselsnummer, D-nummer or a H-nummer.
///   </para>
/// </summary>
/// <remarks>
///   <para>
///      A Fh-nummer is an 11-digit number structured as NNNNNNNNNCC, with the
///      following elements:
///      <list type="bullet">
///         <item>
///            <term>NNNNNNNNN</term>
///            <description>
///               9 random digits. The first digit must be 8 or 9 to distinguish
///               the value from other identifiers like fødselsnummer, etc.
///            </description>
///         </item>
///         <item>
///            <term>CC</term>
///            <description>
///               Two separate check digits calculated using a weighted
///               modulus 11 algorithm. The first check digit is calculated
///               for the first nine digits and the second check digit is
///               calculated for the first ten digits, includding the first
///               check digit.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The 11 character value is sometimes formatted for greater readability
///      by inserting a separator character, generally a space, between the date
///      of birth and the individual number, i.e. NNNNNN NNNCC.
///   </para>
///   <para>
///      When creating a new <see cref="NoFhnummer"/>, the following validation
///      rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The string must be either 11 or 12 characters long.
///            </description>
///         </item>
///         <item>
///            <description>
///               All non-separator characters must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing two characters must be valid weighted modulus 11
///               check digits.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the value has length 12, then the character at position 6
///               (zero-based) must not be an ASCII digit ('0'-'9')
///            </description>
///         </item>
///         <item>
///            <description>
///               The leading digit must be 8 or 9.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>98075450605</term>
///            <description>
///               unformatted, heck digits = 05
///            </description>
///         </item>
///         <item>
///            <term>87207009367</term>
///            <description>
///               unformatted, check digits = 67
///            </description>
///         </item>
///         <item>
///            <term>809390 27371</term>
///            <description>
///               formatted, check digits = 71
///            </description>
///         </item>
///      </list>
///   </para>
/// </remarks>
public record NoFhnummer : NoIdentityNumberBase
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new Norwegian Fh-nummer.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidPrefix)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating Norwegian Fh-nummers.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidPrefix)
   {
   }

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Norwegian H-nummer.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Norwegian H-nummer.
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
         return GetInvalidLengthResult(value);
      }

      // After performing basic checks, validate the check digits because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      if (!ValidateCheckDigits(value, out var invalidCharacterPosition))
      {
         return invalidCharacterPosition == -1
            ? GetInvalidChecksumResult()
            : GetInvalidCharacterResult(value, invalidCharacterPosition);
      }

      if (!ValidateSeparator(value))
      {
         return GetInvalidSeparatorResult(value);
      }

      if (value[0] is not Chars.DigitEight and not Chars.DigitNine)
      {
         return GetInvalidPrefixResult(value);
      }

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.NoFhnummerInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.NoFhnummerInvalidCheckDigits, CheckDigitAlgorithmName);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.NoFhnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(UnformattedLength, Messages.NoFhnummerUnformattedLength),
            new ValidLengthDefinition(FormattedLength, Messages.NoFhnummerFormattedLength),
         ]);

   private static InvalidPrefix GetInvalidPrefixResult(String value)
      => new(
         Messages.NoFhnummerInvalidPrefix,
         value[0].ToString());

   private static InvalidSeparator GetInvalidSeparatorResult(ReadOnlySpan<Char> value)
      => new(
         Messages.NoFhnummerInvalidSeparator,
         value[SeparatorOffset],
         SeparatorOffset);
}
