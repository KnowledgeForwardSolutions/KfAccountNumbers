namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Strongly typed business object that represents a German identification
///   number (Steuerliche Identifikationsnummer or Steuer-IdNr).
/// </summary>
/// <remarks>
///   <para>
///      A Steuer-IdNr is an 11-digit number structured as DDDDDDDDDDC, with the
///      following elements:
///      <list type="bullet">
///         <item>
///            <term>DDDDDDDDDD</term>
///            <description>
///               Ten random digits.
///            </description>
///         </item>
///         <item>
///            <term>C</term>
///            <description>
///               Check digit generated using the ISO/IEC 7064, MOD 11,10
///               algorithm.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The 11 character value is sometimes formatted for greater readability
///      by inserting a separator character, generally a space, at positions 2,
///      6 and 10 (zero-based), i.e. DD DDD DDD DDC.
///   </para>
///   <para>
///      When creating a new <see cref="DeSteuerIdNr"/>, the following validation
///      rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The string must be either 11 or 14 characters long.
///            </description>
///         </item>
///         <item>
///            <description>
///               All non-separator characters must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing character must be a valid ISO/IEC 7064, MOD 11,10
///               check digit.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the value has length 14, then characters at positions 2, 6
///               and 10 (zero-based) must not be an ASCII digit ('0'-'9') and
///               all separator positions must be the same character
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>43957380212</term>
///            <description>
///               unformatted
///            </description>
///         </item>
///         <item>
///            <term>25 986 078 148</term>
///            <description>
///               formatted
///            </description>
///         </item>
///         <item>
///            <term>91 215 743 612</term>
///            <description>
///               formatted
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A Steuer-IdNr does not encode any personanl information.
///   </para>
///   <para>
///      See <see href="https://de.wikipedia.org/wiki/Steuerliche_Identifikationsnummer">Wikipedia (German) - Steuerliche Identifikationsnummer</see>
///      for more information.
///   </para>
/// </remarks>
public record DeSteuerIdNr
{
}
