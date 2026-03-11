// Ignore Spelling: Personnummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object for a Swedish Personal Identity Number
///   (Personnummer).
/// </summary>
/// <remarks>
///   <para>
///      A valid Swedish Personal Identity Number (Personnummer) is a string
///      that is 11 or 13 characters long. The first 6 or 8 characters represent
///      the date of birth in the format YYMMDD or YYYYMMDD, followed by a
///      hyphen or plus sign, and then a three-digit birth serial number and a
///      single digit that is a checksum calculated using the Luhn algorithm.
///      An odd birth serial number indicates a male, while an even birth serial
///      number indicates a female. The hyphen indicates that the person is under
///      100 years old, while the plus sign indicates that the person is 100
///      years old or older.
///   </para>
///   <para>
///      Not all combinations of digits are valid, as the date of birth must be
///      a valid date and the checksum must be correct according to the Luhn
///      algorithm. When creating a new <see cref="SePersonnummer"/>, the
///      following validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The string must be either 11 or 13 characters long.
///            </description>
///         </item>
///         <item>
///            <description>
///               For 11-character strings, the first 6 characters must represent
///               a valid date in the format YYMMDD. For 13-character strings,
///               the first 8 characters must represent a valid date in the format
///               YYYYMMDD.
///            </description>
///         </item>
///         <item>
///            <description>
///               A valid separator must be present in position 6 (zero based,
///               for YYMMDD format) or position 8 (zero based, for YYYYMMDD
///               format). The separator must be either a hyphen (-) or a plus
///               sign (+).
///            </description>
///         </item>
///         <item>
///            <description>
///               Character positions 7 to 9 (zero based, YYMMDD format) or
///               positions 9 to 11 (zero based, YYYYMMDD format) must be
///               digits representing the birth serial number.
///            </description>
///         </item>
///         <item>
///            <description>
///               Character position 10 (zero based, YYMMDD format) or position
///               12 (zero based, YYYYMMDD format) must be a valid checksum
///               calculated using the Luhn algorithm based on the preceding
///               digits.
///            </description>
///         </item>
///      </list>
///   </para>
/// </remarks>
public record SePersonnummer
{
}
