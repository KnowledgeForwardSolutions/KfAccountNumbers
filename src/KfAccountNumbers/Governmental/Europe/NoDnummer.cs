namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   <para>
///      Strongly typed business object that represents a D-nummer, a temporary
///      Norwegian personal identity number issued to foreign individuals who
///      are not eligible for a permanent identity number (fødselsnummer).
///   </para>
///   <para>
///      <b>Note:</b>See <see cref="NoFoedselsnummer"/> for a similar
///      identifier (fødselsnummer) issued to permanent residents of Norway and
///      <see cref="NoIdentityNumber"/> for a composite type that can represent
///      either a fødselsnummer or a D-nummer.
///   </para>
/// </summary>
/// <remarks>
///   <para>
///      A D-nummer is an 11-digit number structured as DDMMYYIIICC, with the
///      following elements:
///      <list type="bullet">
///         <item>
///            <term>DDMMYY</term>
///            <description>
///               The person's date of birth in DDMMYY format. The <b>DD</b>
///               portion of the date of birth is offset by 40 (i.e. 1-31
///               becomes 41-71) to distinguish D-nummers from fødselsnummers.
///            </description>
///         </item>
///         <item>
///            <term>III</term>
///            <description>
///               Three-digit individual number. The first digit indicates the
///               person's century of birth, with 0-4 = 20th century or
///               1900-1999 and 5-9 = 21st century or 2000-2099. The last digit
///               indicates the person's gender, with odd digits assigned to
///               males and even digits assigned to females.
///            </description>
///         </item>
///         <item>
///            <term>CC</term>
///            <description>
///               Two separate check digits calculated using a weighted
///               modulus 11 algorithm. The first check digit is calculated
///               for the first nine digits (date of birth and individual
///               number) and the second check digit is calculated for the date
///               of birth, individual number and first check digit.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The 11 character value is sometimes formatted for greater readability
///      by inserting a separator character, generally a space, between the date
///      of birth and the individual number, i.e. DDMMYY IIICC.
///   </para>
///   <para>
///      When creating a new <see cref="NoDnummer"/>, the following valiation
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
///               If the value has length 12, then character position 6
///               (zero-based) must not be an ASCII digit ('0'-'9')
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth (after adjusting for  the +40 D-nummer day
///               offset and after determining the century from the individual
///               number) must be a valid date between 01/01/1854 and
///               31/12/2039. Note that the validation specifically does
///               <b>NOT</b> check for future dates, only that the date exists.
///            </description>
///         </item>
///      </list>
///      Note that the <b>DD</b> portion of the date of birth will be 61-91
///      because samordningsnummer offsets the day of birth by +60 to
///      distinguish from personnummer values.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>60055029566</term>
///            <description>
///               unformatted, date of birth = 600550 (actual date of birth =
///               May 20, 1950), gender = male, check digits = 66
///            </description>
///         </item>
///         <item>
///            <term>70100567871</term>
///            <description>
///               unformatted, date of birth = 701005 (actual date of birth =
///               October 30, 2005), gender = female, check digits = 71
///            </description>
///         </item>
///         <item>
///            <term>530295 34272</term>
///            <description>
///               formatted, date of birth = 530295 (actual date of birth =
///               February 13, 1995), gender = female, check digits = 72
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/National_identity_number_(Norway) for more info.
///   </para>
/// </remarks>
public record NoDnummer : NoIdentityNumberBase
{

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Norwegian D-nummer.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Norwegian D-nummer.
   /// </param>
   /// <returns>
   ///   A <see cref="NoIdentityNumberBase.ValidationResult"/> union that indicates if the
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

      if (!ValidateDateOfBirth(value, DateOffsetMode.Dnummer))
      {
         return GetInvalidDateOfBirthResult(value);
      }

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.NoDnummerInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.NoDnummerInvalidCheckDigits, CheckDigitAlgorithmName);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.NoDnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(UnformattedLength, Messages.NoDnummerUnformattedLength),
            new ValidLengthDefinition(FormattedLength, Messages.NoDnummerFormattedLength),
         ]);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(
         Messages.NoDnummerInvalidDateOfBirth,
         value[..SeparatorOffset],
         DateFormatName.DDMMYY);

   private static InvalidSeparator GetInvalidSeparatorResult(ReadOnlySpan<Char> value)
      => new(
         Messages.NoDnummerInvalidSeparator,
         value[SeparatorOffset],
         SeparatorOffset);
}
