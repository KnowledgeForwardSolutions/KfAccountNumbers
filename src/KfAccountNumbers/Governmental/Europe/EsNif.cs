// Ignore Spelling: Nif

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a Spanish documento nacional
///   de identidad (DNI) or número de identificación de extranjero (NIE).
/// </summary>
/// <remarks>
///   <para>
///      DNI and NIE are both nine-digit numbers with similar, but slightly
///      different structures. A DNI has the structure DDDDDDDDC while a NIE
///      uses PDDDDDDDC, where:
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
///               is an alphabetic character representing the modulus 23 check digit
///               calculated from the previous eight digits.
///            </description>
///         </item>
///         <item>
///            <term>P</term>
///            <description>
///               is one of the letters X, Y or Z (when calculating the check digit,
///               X = 0, Y = 1 and Z = 2).
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The only difference between a DNI and a NIE is if the leading (left-most)
///      character is a digit or the letter X, Y or Z. Both values may be formatted
///      as a sequence of nine characters or may be formatted for greater readability
///      by using    separators. For a DNI, a separator (generally a dash '-') is
///      placed between the digits and the trailing alphabetic character. For a NIE,
///      separators are placed between the leading letter and the digits, and between
///      the digits and the trailing alphabetic character.
///   </para>
///   <para>
///      When creating a new <see cref="EsNif"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be 9 characters in length (without separators) or
///               10 characters (DNI with one separator) or 11 characters (NIE with
///               two separators).
///            </description>
///         </item>
///         <item>
///            <description>
///               All characters other than the leading and trailing characters
///               (and the optional separators) must be ASCII digits ('0'-'9'). The
///               leading character must be either an ASCII digit or X, Y, or Z.
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing character must be a valid modulus 23 check character.
///               Valid characters are "TRWAGMYFPDXBNJZSQVHLCKE" (where T represents a
///               remainder of 0 and E represents a remainder of 22).
///            </description>
///         </item>
///         <item>
///            <description>
///               The optional separator character(s), if included, may not be an ASCII
///               digit. Any non-digit character is allowed as a separator. For a DNI,
///               the separator must be in character position 8 (zero-based). For a NIE,
///               the separators must be in character positions 1 and 9 (zero-based) and
///               both separator characters must be the same.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>12345678Z</term>
///            <description>DNI</description>
///         </item>
///         <item>
///            <term>17110804680</term>
///            <term>50487563-X</term>
///            <description>DNE</description>
///         </item>
///         <item>
///            <term>X1234567L</term>
///            <description>NIF</description>
///         </item>
///         <item>
///            <term>Y-7654321-G</term>
///            <description>NIE</description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/National_Identity_Card_(Spain) and
///      https://es.wikipedia.org/wiki/N%C3%BAmero_de_identificaci%C3%B3n_fiscal (Spanish)
///      for more info.
///   </para>
/// </remarks>
public record EsNif
{
}
