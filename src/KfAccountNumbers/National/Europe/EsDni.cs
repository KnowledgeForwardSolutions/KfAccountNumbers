namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Strongly typed business object that represents a Spanish national
///   identifier, the Documento Nacional de Identidad (DNI), which is issued to
///   Spanish citizens.
/// </summary>
/// <remarks>
///   <para>
///      A Spanish DNI is a nine-character value with the structure DDDDDDDDC,
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
///               is an alphabetic character representing the modulus 23 check
///               digit calculated from the previous eight digits.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A DNI may be formatted for readability by including a separator
///      character between the leading eight digits and the trailing alphabetic
///      check character. The separator character (generally a dash '-') may not
///      be an ASCII digit ('0'-'9') or an upper-case or lower-case alphabetic
///      character ('A'-'Z', 'a'-'z').
///   </para>
///   <para>
///      When creating a new <see cref="EsDni"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be 9 characters in length (without separator)
///               or 10 characters (with separator).
///            </description>
///         </item>
///         <item>
///            <description>
///               The leading eight characters must all be ASCII digits
///               ('0'-'9') and the trailing character must be an upper-case or
///               lower-case alphabetic character from the subset allowed by the
///               Modulus 23 algorithm (TRWAGMYFPDXBNJZSQVHLCKE).
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing character must be a valid modulus 23 check
///               character. Valid characters are "TRWAGMYFPDXBNJZSQVHLCKE"
///               (where T represents a remainder of 0 and E represents a
///               remainder of 22).
///            </description>
///         </item>
///         <item>
///            <description>
///               If the length equals the formatted length (10 characters), the
///               eighth character (zero-based) is considered a separator
///               character and may not be an ASCII digit ('0'-'9') or an
///               upper-case or lower-case alphabetic character ('A'-'Z',
///               'a'-'z').
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      <see cref="EsDni"/> is case-insensitive for validation and parsing
///      purposes. The EsDni constructor, Create method and explicit string to
///      EsDni operator will normalize any lowercase letters to uppercase.
///      Equality and inequality comparisons between instances of EsDni will
///      compare the normalized uppercase versions of the value.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>12345678Z</term>
///            <description>unformatted</description>
///         </item>
///         <item>
///            <term>50487563-X</term>
///            <description>formatted</description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/National_Identity_Card_(Spain) and
///      https://es.wikipedia.org/wiki/N%C3%BAmero_de_identificaci%C3%B3n_fiscal (Spanish)
///      for more info.
///   </para>
/// </remarks>
public record EsDni : EsIdentityNumberBase
{
   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Documento Nacional de Identidad (DNI).
   /// </summary>
   /// <param name="value">
   ///   String representation of a Spanish NDocumento Nacional de Identidad
   ///   (DNI).
   /// </param>
   /// <returns>
   ///   A <see cref="EsIdentityNumberBase.ValidationResult"/> union that
   ///   indicates if the <paramref name="value"/> passed validation or what
   ///   validation error was encountered.
   /// </returns>
   public static ValidationResult Validate(String? value)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         return default(EmptyValue);
      }

      if (!ValidateLength(value))
      {
         return GetInvalidLengthResult(value);
      }

      // After performing basic checks, validate the check digit because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      if (!ValidateCheckDigit(
         value,
         AllowedIdentifierType.Dni,
         out var invalidCharacterPosition))
      {
         return invalidCharacterPosition == -1
            ? GetInvalidChecksumResult()
            : GetInvalidCharacterResult(value, invalidCharacterPosition);
      }

      if (!ValidateSeparators(value, out var invalidSeparatorPosition))
      {
         return GetInvalidSeparatorResult(value, invalidSeparatorPosition);
      }

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.EsDniInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.EsDniInvalidCheckDigit, EsIdentityNumberBase.CheckDigitAlgorithmName);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.EsDniInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(UnformattedLength, Messages.EsDniUnformattedLength),
            new ValidLengthDefinition(DniFormattedLength, Messages.EsDniFormattedLength),
         ]);

   private static InvalidSeparator GetInvalidSeparatorResult(
      ReadOnlySpan<Char> value,
      Int32 invalidSeparatorOffset)
      => new(
         Messages.EsDniInvalidSeparator,
         value[invalidSeparatorOffset],
         invalidSeparatorOffset);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean ValidateLength(ReadOnlySpan<Char> value)
      => value.Length is UnformattedLength or DniFormattedLength;
}
