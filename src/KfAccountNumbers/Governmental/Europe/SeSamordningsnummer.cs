namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a coordination number
///   (samordningsnummer) issued by the Swedish Tax Agency to temporary
///   residents (persons who reside in Sweden for less than 12 months).
/// </summary>
/// <remarks>
///   <para>
///      Swedish samordningsnummer values are both 11 or 13 character strings.
///      The only difference between the two lengths are the number of digits
///      used to represent the date of birth, either six or eight.
///      Samordningsnummers are structured as YYMMDD-SSSC (or YYYYMMDD-SSSC for
///      eight-digit date of birth values) with the following elements.
///      <list type="bullet">
///         <item>
///            <term>YYMMDD</term>
///            <description>
///               The person's date of birth in YYMMDD format.
///            </description>
///         </item>
///         <item>
///            <term>YYYYMMDD</term>
///            <description>
///               The person's date of birth in YYYYMMDD format.
///            </description>
///         </item>
///         <item>
///            <term>-</term>
///            <description>
///               A separator character that separates the date of birth from
///               the remaining four digits. The separator character is
///               normally a dash ('-') but when a person turns 100 years old
///               the dash is replaced by a plus sign ('+').
///            </description>
///         </item>
///         <item>
///            <term>SSS</term>
///            <description>
///               A three digit birth serial number, issued serially as births
///               are recorded for a particular date. The last digit of the
///               birth serial number serves an additional purpose of indicating
///               the person's gender, with odd digits assigned to males and
///               even digits assigned to females.
///            </description>
///         </item>
///         <item>
///            <term>C</term>
///            <description>
///               A single check digit calculated using the Luhn algorithm
///               applied to the rightmost six digits of the date of birth and
///               to the three digits of the birth serial number.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      When creating a new <see cref="SeSamordningsnummer"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The string must be either 11 or 13 characters long.
///            </description>
///         </item>
///         <item>
///            <description>
///               For 11-character strings, the first 6 characters must represent
///               a valid date in the format YYMMDD. For 13-character strings,
///               the first 8 characters must represent a valid date in the
///               format YYYYMMDD. Note that the validation specifically does
///               <b>NOT</b> check for future dates, only that the date exists.
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth must be followed by a valid separator
///               character. The separator must be either a dash (-) or a plus
///               sign (+).
///            </description>
///         </item>
///         <item>
///            <description>
///               The separator must be followed by a three digit birth serial
///               number.
///            </description>
///         </item>
///         <item>
///            <description>
///               The birth serial number must be followed by a  valid checksum
///               calculated using the Luhn algorithm based on the six digit
///               date of birth and the three-digit birth serial number. (The
///               leading two digits of an eight digit date of birth are
///               ignored.)
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Samordningsnummers are distinguished from other identity numbers by an
///      offset added to the day component of the date of birth. In a
///      samordningsnummer, 60 is added to the day, resulting in a day between
///      61 and 91. The date of birth property takes the offset into account and
///      returns the actual date of birth.
///   </para>
///   <para>
///      Note that the encoded date of birth may not be the person's actual
///      date of birth. It is possible to run out of birth serial numbers for
///      a particular day and in this case a day close to the actual date of
///      birth is substituted in its stead.
///   </para>
///   <para>
///      When determining if a date of birth is valid, values with six digit
///      dates of birth use the separator character to derive the full four
///      digit year. Year values between 00 and 49 are assumed to be 2000 to
///      2049 and year values between 50 and 99 are assumed to be 1950 to 1999.
///      If the separator character indicates that the person is at least 100
///      years of age, then 100 is subtracted from the year, resulting in 00 to
///      40 meaning 1900 to 1949 and 50 to 99 meaning 1850 to 1899.
///   </para>
///   <para>
///      Internally, <see cref="SeSamordningsnummer"/> stores a 12-digit
///      representation consisting of the date of birth in YYYYMMDD format
///      followed by the birth serial number and check digit (no separator). The
///      <see cref="Value"/> property returns this internal representation.
///   </para>
///   <para>
///      When comparing <see cref="SeSamordningsnummer"/> objects for equality,
///      the internal 12-digit representation is used. This means two objects
///      representing the same person will be considered equal regardless of
///      whether they were created from 11-character or 13-character input strings.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>890261-3283</term>
///            <description>
///               Samordningsnummer, date of birth 890261 (actual date of birth
///               = 890201), less than 100 years old, gender = female, check
///               digit = 3.
///            </description>
///         </item>
///         <item>
///            <term>19890261-3283</term>
///            <description>
///               Samordningsnummer, date of birth 19890261 (actual date of birth
///               = 19890201), less than 100 years old, gender = female, check digit
///               = 3.
///            </description>
///         </item>
///         <item>
///            <term>811288+9871</term>
///            <description>
///               Samordningsnummer, date of birth 811288 (actual date of birth
///               = 811228), greater than 100 years old, gender = male, check
///               digit = 1.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
///      for more details.
///   </para>
/// </remarks>
public record SeSamordningsnummer : SeIdentityNumberBase
{
}
