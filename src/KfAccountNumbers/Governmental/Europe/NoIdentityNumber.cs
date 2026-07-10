#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   <para>
///      Strongly typed business object that represents any of several different
///      Norwegian personal identity numbers: fødselsnummer, D-nummer, H-nummer
///      and Fh-nummer. These identifiers have similar format and validation
///      rules and are distinguished by the encoding of the person's date of
///      birth.The <see cref="IdentifierType"/> property will indicate the exact
///      type of identifier represented by an instance of
///      <see cref="NoIdentityNumber"/>.
///   </para>
///   <para>
///      A fødselsnummer is the Norwegian national identity number issued to
///      citizens and long-term residents of Norway.
///   </para>
///   <para>
///      A D-nummer serves many of the same purposes of a fødselsnummer, but is
///      issued to foreign individuals who are not eligible for a fødselsnummer.
///      A D-nummer is distinguished by a +40 offset added to the day component
///      of the date of birth (1-31 becomes 41-71).
///   </para>
///   <para>
///      A H-nummer is a temporary identifier issued by local health
///      organizations (such as a hospital) to persons needing medical
///      assistance and who do not have either a fødselsnummer or a D-nummer. A
///      H-nummer is unique only within the issuing organization. A H-nummer is
///      distinguished by a +40 offset added to the month component of the date
///      of birth (1-12 becomes 41-52).
///   </para>
///   <para>
///      A Fh-nummer is similar to a H-nummer, an identifier issued to persons
///      needing medical assistance and who do not have a fødselsnummer or a
///      D-nummer. However, while H-nummers are issued by a single organization
///      and only unique within that organization, Fh-nummers are issued by
///      Norsk Helsenett (the Norwegian Health Network) and are unique across
///      the entire Norwegian health system. Fh-nummers do not encode the
///      person's date of birth or gender and are distinguished by an initial
///      digit = 8 or 9.
///   </para>
///   <para>
///      See <see cref="NoFoedselsnummer"/> and <see cref="NoDnummer"/>
///      for types that represent specific identifiers.
///   </para>
/// </summary>
/// <remarks>
///   <para>
///      A fødselsnummer or a D-nummer is an 11-digit number structured as
///      DDMMYYIIICC, with the following elements:
///      <list type="bullet">
///         <item>
///            <term>DDMMYY</term>
///            <description>
///               The person's date of birth in DDMMYY format. Note that the
///               date of birth can be altered by adding different offsets to
///               the day or month to differentiate between different types of
///               identifiers as described above.
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
///      When creating a new <see cref="NoIdentityNumber"/>, the following
///      valiation rules are applied:
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
///               If the value is a fødselsnummer or D-nummer, the date of birth
///               (after adjusting for identifier specific offsets and after
///               determining the century from the individual number) must be a
///               valid date between 01/01/1854 and 31/12/2039. Note that the
///               validation specifically does <b>NOT</b> check for future
///               dates, only that the date exists.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>13029597140</term>
///            <description>
///               fødselsnummer, unformatted, date of birth = February 13, 1995,
///               gender = male, check digits = 40
///            </description>
///         </item>
///         <item>
///            <term>20050559433</term>
///            <description>
///               fødselsnummer, unformatted, date of birth = May 20, 2005,
///               gender = female, check digits = 33
///            </description>
///         </item>
///         <item>
///            <term>130682 27938</term>
///            <description>
///               fødselsnummer, formatted, date of birth = June 13, 1982,
///               gender = male, check digits = 38
///            </description>
///         </item>
///         <item>
///            <term>60055029566</term>
///            <description>
///               D-nummer, unformatted, date of birth = May 20, 1950, gender =
///               male, check digits = 66
///            </description>
///         </item>
///         <item>
///            <term>70100567871</term>
///            <description>
///               D-nummer, unformatted, date of birth = October 30, 2005,
///               gender = female, check digits = 71
///            </description>
///         </item>
///         <item>
///            <term>530295 34272</term>
///            <description>
///               D-nummer, formatted, date of birth = February 13, 1995,
///               gender = female, check digits = 72
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The century of birth is encoded in the individual number and when
///      combined with the 6-digit date of birth can determine an actual date of
///      birth. The rules for determining the century of birth vary by the type
///      of identifier.
///   </para>
///   <para>
///      Fødselsnummers have fairly complex rules for determining the century of
///      birth due to additional requirements being layered upon the individual
///      number element over time. The rules used by
///      <see cref="NoIdentityNumber"/> are taken from
///      https://blog.variant.no/ssns-and-pattern-matching-in-c-9-498f96aa71d4.
///      Because of the overlapping ranges (the individual number 500  matches
///      two different rules), the rules must be evaluated in order to arrive at
///      the correct century. The rules are:
///      <list type="bullet">
///         <item>
///            <term>Rule 1</term>
///            <description>
///               If the individual number is &gt;= 500 and &lt;= 749 AND the
///               two digit year is &gt;= 54 then the century = 1800.
///            </description>
///         </item>
///         <item>
///            <term>Rule 2</term>
///            <description>
///               If the individual number is &lt; 500 then the century = 1900.
///            </description>
///         </item>
///         <item>
///            <term>Rule 3</term>
///            <description>
///               If the individual number is &gt;= 900 AND the two digit year
///               is &gt;= 40 then the century = 1900.
///            </description>
///         </item>
///         <item>
///            <term>Rule 4</term>
///            <description>
///               If the individual number is &gt;= 500 AND the two digit year
///               is &lt;= 39 then the century =2000.
///            </description>
///         </item>
///         <item>
///            <term>Rule 5</term>
///            <description>
///               Otherwise invalid. Validation will return invalid date of
///               birth.
///            </description>
///         </item>
///      </list>
///      According to these rules, the range of valid dates of birth are from
///      January 1, 1854 to December 31, 2039. A date of birth outside this
///      range, even if a valid date, will return invalid date of birth.
///   </para>
///   <para>
///      For D-nummers, the first digit of the individual number indicates the
///      century of the person's birth (0-4 = 20th century or 1900-1999 and
///      5-9 = 21st century or 2000-2099).
///   </para>
///   <para>
///      See <see href="https://en.wikipedia.org/wiki/National_identity_number_(Norway)">Wikipedia - National_identity_number_(Norway)</see>
///      more info.
///   </para>
/// </remarks>
public record NoIdentityNumber : NoIdentityNumberBase
{
   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Norwegian identity number.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Norwegian identity number.
   /// </param>
   /// <returns>
   ///   A <see cref="NoIdentityNumberBase.ValidationResult"/> union that
   ///   indicates if the <paramref name="value"/> passed validation or what
   ///   validation error was encountered.
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

      if (!ValidateDateOfBirth(value, DateOffsetMode.Optional))
      {
         return GetInvalidDateOfBirthResult(value);
      }

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.NoIdentityNumberInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.NoIdentityNumberInvalidCheckDigits, CheckDigitAlgorithmName);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.NoIdentityNumberInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(UnformattedLength, Messages.NoIdentityNumberUnformattedLength),
            new ValidLengthDefinition(FormattedLength, Messages.NoIdentityNumberFormattedLength),
         ]);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(
         Messages.NoIdentityNumberInvalidDateOfBirth,
         value[..SeparatorOffset],
         DateFormatName.DDMMYY);

   private static InvalidSeparator GetInvalidSeparatorResult(ReadOnlySpan<Char> value)
      => new(
         Messages.NoIdentityNumberInvalidSeparator,
         value[SeparatorOffset],
         SeparatorOffset);
}
