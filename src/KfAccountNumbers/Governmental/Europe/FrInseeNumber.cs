// Ignore Spelling: Insee

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a French INSEE number.
/// </summary>
/// <remarks>
///   <para>
///      A French INSEE number is a 15-digit number structured as SYYMMLLOOOKKKCC
///      with the following elements:
///      <list type="bullet">
///         <item>
///            <term>S</term>
///            <description>
///               The person's gender, where 1 = male and 2 = female. Temporary
///               INSEE numbers use 7 = male and 8 = female instead.
///            </description>
///         </item>
///         <item>
///            <term>YY</term>
///            <description>
///               The person's two-digit year of birth.
///            </description>
///         </item>
///         <item>
///            <term>MM</term>
///            <description>
///               The person's two-digit month of birth. See below for values
///               used for persons with unknown or incomplete date of birth
///               documentation.
///            </description>
///         </item>
///         <item>
///            <term>LLOO</term>
///            <description>
///               Five-digit INSEE COG (Code officiel géographique) identifying
///               the person's department and commune of birth. (Exception: LL
///               may be "2A" or "2B" for the two departments in Corsica).
///            </description>
///         </item>
///         <item>
///            <term>KKK</term>
///            <description>
///               Three digits used to distinguish between people born in the same
///               year/month/department/commune.
///            </description>
///         </item>
///         <item>
///            <term>CC</term>
///            <description>
///               Two-digit modulus 97 check sum calculated for the preceding 13 digits.
///               When calculating the checksum, department code "2A" is replaced by 19,
///               and department code "2B" is replaced by 18.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      An INSEE number may be formatted as 15 consecutive digits or as 21 characters
///      with spaces separating the different elements, i.e. "S YY MM LL OOO KKK CC".
///   </para>
///   <para>
///      When creating a new <see cref="FrInseeNumber"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 15 characters (without separators) or 21
///               characters (with separators) in length.
///            </description>
///         </item>
///         <item>
///            <description>
///              All characters (except the optional separator characters or Corsican
///              department codes) must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The two trailing (right-most) characters must be a valid modulus 97
///               check sum.
///            </description>
///         </item>
///         <item>
///            <description>
///               The separator characters (if used) may not be ASCII digits ('0'-'9').
///               All separator characters must be the same character.
///            </description>
///         </item>
///         <item>
///            <description>
///               The leading gender indicator (S) must be 1, 2, 7 or 8.
///            </description>
///         </item>
///         <item>
///            <description>
///               The month element (MM) must be a number between 01 and 12 (for known
///               dates) or 20-42, 50-99 (for persons with unknown or incomplete date
///               of birth documentation).
///            </description>
///         </item>
///         <item>
///            <description>
///               The COG element (LLOOO) must start with a valid department code, or
///               99 for persons born abroad.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>188121884813261</term>
///            <description>
///               gender = male, year of birth = 88, month of birth = 12, department = 18 (Cher)
///            </description>
///         </item>
///         <item>
///            <term>255102445387796</term>
///            <description>
///               gender = female, year of birth = 55, month of birth = 10, department = 24 (Dordogne)
///            </description>
///         </item>
///         <item>
///            <term>112072A28806058</term>
///            <description>
///               gender = male, year of birth = 12, month of birth = 07, department = 2A (Corse-du-Sud)
///            </description>
///         </item>
///         <item>
///            <term>821099901013371</term>
///            <description>
///               temporary INSEE, gender = female, year of birth = 21, month of birth = 09, department = 99 (born abroad)
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/INSEE_code and
///      https://fr.wikipedia.org/wiki/Num%C3%A9ro_de_s%C3%A9curit%C3%A9_sociale_en_France (French) for more info.
///   </para>
/// </remarks>
public record FrInseeNumber
{
}
