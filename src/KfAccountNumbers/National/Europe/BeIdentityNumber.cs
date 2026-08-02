#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.National.Europe;

/// <summary>
///   <para>
///      Strongly typed business object that represents a Belgian
///      rijksregisternummer or a Belgian BIS-nummer (for non-residents).
///   </para>
///   <para>
///      BIS-nummers apply an offset to the month portion of the date of birth
///      element to distinguish them from rijksregisternummers issued to Belgian
///      citizens and permanent residents.
///   </para>
/// </summary>
/// <remarks>
///   <para>
///      Rijksregisternummer and BIS-nummer both are 11-digit numbers,
///      structured as YYMMDDXXXCC, with the following elements.
///      <list type="bullet">
///         <item>
///            <term>YYMMDD</term>
///            <description>
///               6-digit date of birth in YYMMDD format. Note that for
///               BIS-nummers, the MM portion of the date of birth will be
///               either +20 (i.e. 21-32) or +40 (i.e. 41-52) to distinguish
///               from rijksregisternummer values. The date of birth may be
///               unknown/incomplete and in that case zeros are used in place of
///               the unknown elements.
///            </description>
///         </item>
///         <item>
///            <term>XXX</term>
///            <description>
///               3-digit sequence number used to distinguish between persons
///               born on the same date. The last digit indicates the person's
///               gender, with odd numbers = male and even numbers = female.
///               (See below for exception when value is a BIS-nummer and gender
///               is unknown.)
///            </description>
///         </item>
///         <item>
///            <term>CC</term>
///            <description>
///               Two digit modulus 97 check sum calculated for the YYMMDD and
///               XXX elements. The check sum is also used to indicate century
///               of birth. If CC is equal to the normal modulus 97 check sum
///               then the person's century of birth is 1900-1999. If CC is
///               equal to the modulus 97 check sum calculated by first
///               prefixing YYMMDDXXX with the digit 2 (i.e. 2YYMMDDXXX) then
///               the person's century of birth is 2000-2099.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A Belgian rijksregisternummer or BIS-nummer may be formatted as a
///      string of 11 consecutive digits (YYMMDDXXXCC) or as a 15 character
///      string with characters separating the individual elements.
///      YY.MM.DD-XXX.CC is the typical display format.
///   </para>
///   <para>
///      When creating a new <see cref="BeIdentityNumber"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 11 characters (without separators) or
///               15 characters (with separators) in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               All characters (except the optional separator characters) must
///               be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The separator characters, if included, must not be ASCII
///               digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The two trailing (right-most) characters must be a valid
///               modulus 97 check sum (taking into account the possibility of a
///               person born in the year 2000 or later).
///            </description>
///         </item>
///         <item>
///            <description>
///               The sequence number may not be 000 or 999.
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth, after deriving the century of birth from
///               the check sum and taking into account the BIS number offset,
///               must be a valid date between January 1, 1900 and December 31,
///               2099.
///               <b>OR</b> the date of birth may use zeros to indicate that
///               some or all of the person's date of birth is unknown (see
///               below for more details).
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The date of birth can be adjusted in a variety of ways:
///      <list type="bullet">
///         <item>
///            <description>
///               If the person's date of birth is incomplete, then the two
///               digit year is used and zeros are used for month and day (for
///               example, 40.00.00-955.69).
///            </description>
///         </item>
///         <item>
///            <description>
///               If there are too many people with incomplete dates of birth
///               for a particular year than can be represented by a three digit
///               sequence number (i.e. more than 499 males with incomplete
///               dates of birth for the year 1940), then 01 is used for the day
///               of birth and the sequence number rolls over to 001
///               (ex. 40.00.01-001.33). (Note that
///               <see cref="BeIdentityNumber"/> does not enforce an upper
///               limit on the day component in cases of rollover, though
///               multiple rollovers in a single year should be rare.)
///            </description>
///         </item>
///         <item>
///            <description>
///               If the person's date of birth is unknown, then the constant
///               00.00.01 is used.
///            </description>
///         </item>
///         <item>
///            <description>
///               As noted above, BIS-nummers apply an offset to the month
///               component of the date of birth to distinguish them from
///               rijksregisternummers. If the person's gender is known when the
///               BIS-nummer is issued, then <b>40</b> is added to the month;
///               Otherwise <b>20</b> is added to the month.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      For cases of a BIS-nummer for a person with an incomplete or unknown
///      date of birth, <see cref="BeIdentityNumber"/> stacks the appropriate
///      rules. For example, 87.40.00-023.47 would be the BIS number for a
///      person with an incomplete date of birth born in 1987.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>87092100294</term>
///            <description>
///               rijksregisternummer, unformatted, date of birth
///               September 21, 1987, gender = female, check digit calculation
///               97 - (870921002 mod 97) = 97 - 3 = 94
///            </description>
///         </item>
///         <item>
///            <term>05.03.11-017.02</term>
///            <description>
///               rijksregisternummer, formatted, date of birth March 11, 2005,
///               gender = male, check digit calculation 97 - (050311017 mod 97)
///               = 97 - 95 = 2
///            </description>
///         </item>
///         <item>
///            <term>455000007612</term>
///            <description>
///               rijksregisternummer, unformatted, date of birth 1955,
///               day/month unknown, gender = female, check digit calculation
///               97 - (550000076 mod 97) = 97 - 85 = 12
///            </description>
///         </item>
///         <item>
///            <term>17.51.08-046.40</term>
///            <description>
///               BIS-nummer, formatted, date of birth November 8, 1917,
///               gender = female, check digit calculation
///               97 - (175108046 mod 97) = 97 - 57 = 40
///            </description>
///         </item>
///         <item>
///            <term>09200000265</term>
///            <description>
///               BIS-nummer, unformatted, date of birth 2009, day/month
///               unknown, year of birth 2009, gender unknown, check digit
///               calculation 97 - (2092000002 mod 97) = 97 - 32 = 65
///            </description>
///         </item>
///         <item>
///            <term>20.47.00-033.66</term>
///            <description>
///              BIS-nummer, formatted, date of birth July 2020, day unknown,
///              gender = male, check digit calculation 97 - (204700033 mod 97)
///              = 97 - 31 = 66
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See <see href="https://fr.wikipedia.org/wiki/Num%C3%A9ro_de_registre_national">Wikipedia (French) - Numéro de registre national</see>
///      for more info.
///   </para>
/// </remarks>
public record BeIdentityNumber : BeIdentityNumberBase
{
   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Belgian identity number.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Belgian identity number.
   /// </param>
   /// <returns>
   ///   A <see cref="BeIdentityNumberBase.ValidationResult"/> union that
   ///   indicates if the <paramref name="value"/> passed validation or what
   ///   validation error was
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
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return invalidCharacterPosition == -1
            ? GetInvalidChecksumResult()
            : GetInvalidCharacterResult(value, invalidCharacterPosition);
      }

      if (!ValidateSeparators(value, out var invalidSeparatorPosition))
      {
         return GetInvalidSeparatorResult(value, invalidSeparatorPosition);
      }

      if (!ValidateSequenceNumber(value))
      {
         return GetInvalidSequenceNumberResult(value);
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
      => new(Messages.BeIdentityNumberInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.BeIdentityNumberInvalidCheckDigits, CheckDigitAlgorithmName);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.BeIdentityNumberInvalidDateOfBirth,
         IsFormatted(value) ? value[..8].ToString() : value[..6].ToString(),
         DateFormatName.YYMMDD);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.BeIdentityNumberInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(UnformattedLength, Messages.BeIdentityNumberUnformattedLength),
            new ValidLengthDefinition(FormattedLength, Messages.BeIdentityNumberFormattedLength),
         ]);

   private static InvalidSeparator GetInvalidSeparatorResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(
         Messages.BeIdentityNumberInvalidSeparator,
         value[position],
         position);

   private static InvalidSequenceNumber GetInvalidSequenceNumberResult(ReadOnlySpan<Char> value)
      => new(
         Messages.BeIdentityNumberInvalidSequenceNumber,
         IsFormatted(value) ? value[9..12].ToString() : value[6..9].ToString());
}
