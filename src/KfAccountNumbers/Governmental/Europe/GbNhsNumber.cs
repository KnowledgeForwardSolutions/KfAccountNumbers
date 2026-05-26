namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents the identifier used by the
///   National Health Service (NHS) of England, Wales and the Isle of Man.
/// </summary>
/// <remarks>
///   <para>
///      A NHS Number consists of 10 digits, structured as NNNNNNNNNC, where
///      <list type="bullet">
///         <item>
///            <term>NNNNNNNNN</term>
///            <description>
///               A unique nine-digit number assigned by the NHS.
///            </description>
///         </item>
///         <item>
///            <term>C</term>
///            <description>
///               A modulus 11 check digit calculated from the preceding nine
///               digits.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      NHS Numbers can be displayed as a string of 10 digits or formatted for
///      readability as three groups of digits in a '3 3 4' pattern
///      (e.g. "123 456 7890"). The optional separator characters can be any
///      character that is not an ASCII digit ('0' - '9'), but both separator
///      characters must be the same. The typical separator character is a space (' ').
///   </para>
///   <para>
///      Each of the public health services in Great Britain (NHS, Scottish CHI
///      and Northern Ireland H&amp;C) are allocated separate blocks of 10-digit
///      numbers so it is possible to determine what service issued the number by
///      comparing the number to a list of valid ranges for each service. For NHS,
///      the valid ranges are 400 000 000 to 499 999 999 and 600 000 000 to
///      799 999 999 (excluding the trailing check digit). <see cref="GbNhsNumber"/>
///      also allows a range of numbers from 900 000 000 to 999 999 999 which are
///      reserved for test purposes and not issued to the public.
///   </para>
///   <para>
///      When creating a new <see cref="GbNhsNumber"/>, the following validation
///      rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 10 characters (without separators) or
///               12 characters (with separators) in length.
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
///               The trailing (right-most) digit must be a valid Modulus 11
///               check digit.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the value is 12 characters long, character positions 3 and 7
///               (zero-based) must not be ASCII digits ('0' - '9'). The same
///               character must be used in each separator position.
///            </description>
///         </item>
///         <item>
///            <description>
///               The first nine digits must fall in one of the following ranges:
///               400 000 000 to 499 999 999, 600 000 000 to 799 999 999, or
///               900 000 000 to 999 999 999.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The Modulus 11 check digit algorithm used by NHS numbers can generate a
///      check value of 10 which can not be encoded as a single decimal digit.
///      The National Health Service and other issuing authorities avoid this
///      issue by not issuing any number that would result in a check value of
///      10. This means that approximately 9.09% of all possible values are never
///      issued.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>4000000004</term>
///            <description>
///               Standard NHS number without formatting.
///            </description>
///         </item>
///         <item>
///            <term>799 999 9997</term>
///            <description>
///               Standard NHS number, formatted for readability.
///            </description>
///         </item>
///         <item>
///            <term>9000000009</term>
///            <description>
///               Test number without formatting.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/NHS_number,
///      https://www.datadictionary.nhs.uk/attributes/nhs_number.html
///      and https://webarchive.nationalarchives.gov.uk/ukgwa/20231221081503/https://digital.nhs.uk/about-nhs-digital/contact-us/freedom-of-information/freedom-of-information-disclosure-log/december-2022/nic-690159-k8h4z
///      for more info.
///   </para>
/// </remarks>
public record GbNhsNumber
{
}
