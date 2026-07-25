namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Strongly typed business object that represents a Swiss Social Security
///   Number (Sozialversicherungsnummer or Neue AVH Nummer).
/// </summary>
/// <remarks>
///   <para>
///      A Sozialversicherungsnummer is an 13-digit number structured as
///      756XXXXXXXXXY , with the following elements:
///      <list type="bullet">
///         <item>
///            <term>756</term>
///            <description>
///               Constant "756", the ISO 3166-1 code for Switzerland
///            </description>
///         </item>
///         <item>
///            <term>XXXXXXXXX</term>
///            <description>
///               Nine random digits.
///            </description>
///         </item>
///         <item>
///            <term>Y</term>
///            <description>
///               Check digit generated using the EAN-13 algorithm.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The 13 character value is often formatted for greater readability by
///      inserting a separator character, generally a period ('.'), at positions
///      3, 8 and 13 (zero-based), i.e. 756.XXXX.XXXX.XY
///   </para>
///   <para>
///      When creating a new <see cref="ChSozialversicherungsnummer"/>, the
///      following validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The string must be 13 characters long (unformatted) or 16
///               characters long (formatted for readability).
///            </description>
///         </item>
///         <item>
///            <description>
///               All non-separator characters must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing character must be a valid EAN-13 check digit.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the value has length 16, then characters at positions 3, 8
///               and 13 (zero-based) must not be ASCII digits ('0'-'9') and all
///               separator positions must be the same character.
///            </description>
///         </item>
///         <item>
///            <description>
///               The leading three characters must be "756".
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>7560850652826</term>
///            <description>
///               unformatted, check digit = 6
///            </description>
///         </item>
///         <item>
///            <term>756.8814.3009.98</term>
///            <description>
///               formatted, check digit = 8
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A Sozialversicherungsnummer does not encode any personal information.
///   </para>
///   <para>
///      See <see href="https://en.wikipedia.org/wiki/National_identification_number#Switzerland">Wikipedia - National identification number - Switzerland</see>
///      and <see href="https://de.wikipedia.org/wiki/Sozialversicherungsnummer#Versichertennummer">Wikipedia (German) - Sozialversicherungsnummer</see>
///      for more information.
///   </para>
/// </remarks>
public record ChSozialversicherungsnummer
{
}
