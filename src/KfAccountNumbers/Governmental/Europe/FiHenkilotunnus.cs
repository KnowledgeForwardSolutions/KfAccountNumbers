// Ignore Spelling: Fi Henkilotunnus

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a Finnish Personal Identity
///   Code (henkilötunnus).
/// </summary>
/// <remarks>
///   <para>
///      A Danish personnummer is a ten-digit number structured as DDMMYYCZZZQ,
///      with the following elements:
///      <list type="bullet">
///         <item>
///            <term>DDMMYY</term>
///            <description>
///               The person's date of birth in DDMMYY format.
///            </description>
///         </item>
///         <item>
///            <term>C</term>
///            <description>
///               Century indicator, with + indicating 1800s, -, U, V, W, X or Y
///               indicating 1900s and A, B, C, D, E, F indicating 2000s.
///            </description>
///         </item>
///         <item>
///            <term>ZZZ</term>
///            <description>
///               Three digit individual number used to differentiate between two
///               persons born on the same date. The individual number also encodes
///               additional information. The person's gender is indicated with
///               even numbers for females and odd numbers for males. Individual
///               numbers between 002 and 899 indicate persons born in Finland or
///               permanent residents and numbers between 900 and 999 are reserved
///               for temporary identifiers. The individual number 001 is not
///               valid.
///            </description>
///         </item>
///         <item>
///            <term>Q</term>
///            <description>
///               Check character, calculated as modulus 31 of the date of birth
///               and individual number. Will be one of 31 alphanumeric characters,
///               "0123456789ABCDEFHJKLMNPRSTUVWXY" (the letters `G, I, O, Q and Z`
///               are excluded to avoid possible confusion with digit characters).
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>230526-034N</term>
///            <description>
///               Date of birth May 23, 1926, gender = female, permanent resident
///            </description>
///         </item>
///         <item>
///            <term>160117A275C</term>
///            <description>
///               Date of birth January 16, 2017, gender = male, permanent resident
///            </description>
///         </item>
///         <item>
///            <term>020508D929B</term>
///            <description>
///               Date of birth May 2, 2008, gender = male, temporary/test value
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      When creating a new <see cref="FiHenkilotunnus"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be 11 characters in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth and individual number elements (DDMMYY and
///               ZZZ elements) must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The century indicator must be +, -, U, V, W, X, Y, A, B, C, D, E or F.
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth, after deriving the century from the century
///               indicator must be a valid date between January 1, 1800 and
///               December 31, 2099.
///            </description>
///         </item>
///         <item>
///            <description>
///               The individual number must not be 001.
///            </description>
///         </item>
///         <item>
///            <description>
///               The check character must be a valid modulus 31 check character
///               calculated from the date of birth and the individual number.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/National_identification_number#Finland
///      for more info. Also see https://kenda.fi/tools/hetu/ for tools to generate
///      test henkilötunnus values.
///   </para>
/// </remarks>
public record FiHenkilotunnus
{
}
