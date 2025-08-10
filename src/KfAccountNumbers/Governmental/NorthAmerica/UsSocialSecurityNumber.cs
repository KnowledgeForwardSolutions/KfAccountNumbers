// Ignore Spelling: ssn

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Strongly typed business object for a US Social Security Number.
/// </summary>
/// <remarks>
///   <para>
///      A valid US Social Security Number consists of nine decimal digits. The 
///      nine digits are commonly separated into three groups, the first three
///      digits represent the area number, the next two digits represent the
///      group number and the final four digits represent the serial number.
///   </para>
///   <para>
///      Prior to 2011, the area number was tied to a geographic region in the
///      US, the group number represented a subgrouping within the area and the
///      serial number was a consecutive sequence within the group. However in 
///      2011, the SSN generation process was randomized and the geographic 
///      association was eliminated. The changes made in 2011 also started
///      issuing social security numbers from previously reserved blocks of area
///      numbers. The validation checks performed by 
///      <see cref="UsSocialSecurityNumber"/> use the rules in effect at the
///      time of release (currently 2025) and therefore some SSN's that would 
///      have been considered invalid prior to 2011 are considered valid by
///      <see cref="UsSocialSecurityNumber"/>.
///   </para>
///   <para>
///      Social Security Numbers are commonly formatted with dashes ('-') and
///      sometimes spaces separating the three groups. A 
///      <see cref="UsSocialSecurityNumber"/> can be created from strings that 
///      include or exclude the separator character, but if used, the same 
///      character must be used to separate both the area/group numbers and the 
///      group/serial numbers. The separator character may not be a decimal 
///      digit (0-9).
///   </para>
///   <para>
///      Not all 9 digit numbers are valid Social Security Numbers. When 
///      creating a new  <see cref="UsSocialSecurityNumber"/>, after determining
///      that a value consists of 9 decimal digits, the following validation 
///      rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               An area number, group number or serial number that is all 
///               zeros (0) is invalid.
///            </description>
///         </item>
///         <item>
///            <description>
///               The area number 666 is never used.
///            </description>
///         </item>
///         <item>
///            <description>
///               Area numbers 900-999 are invalid because they are reserved for
///               Individual Taxpayer Identification Numbers.
///            </description>
///         </item>
///         <item>
///            <description>
///               A number that is 9 identical digits is invalid.
///            </description>
///         </item>
///         <item>
///            <description>
///               The run of 9 consecutive digits (123456789) is invalid.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      <see cref="UsSocialSecurityNumber"/> does not confirm the actual 
///      validity of a number that meets all of the above rules. Confirmation of
///      the actual validity of a SSN requires use of the Social Security Number
///      Verification Service offered by the Social Security Administration.
///   </para>
///   <para>
///      See <see href="https://en.wikipedia.org/wiki/Social_Security_number">Wikipedia - Social Security Number</see>
///      for more details.
///   </para>
/// </remarks>
public record UsSocialSecurityNumber
{
   private readonly String _ssn;

   private const Int32 FormattedLength = 11;
   private const Int32 NonFormattedLength = 9;

   /// <summary>
   ///   Initialize a new <see cref="UsSocialSecurityNumber"/>.
   /// </summary>
   /// <param name="ssn">
   ///   String representation of a Social Security Number.
   /// </param>
   /// <param name="separator">
   ///   Optional. If the <paramref name="ssn"/> is 11 characters in length, 
   ///   then <paramref name="separator"/> identifies the character used to
   ///   separate the different sections of the SSN. This parameter is ignored 
   ///   if the <paramref name="ssn"/> is 9 characters in length. Defaults to '-'.
   /// </param>
   /// <exception cref="ArgumentNullException">
   ///   <paramref name="ssn"/> is <see langword="null"/>.
   /// </exception>
   /// <exception cref="ArgumentException">
   ///   <paramref name="ssn"/> is <see cref="String.Empty"/> or all whitespace
   ///   characters.
   ///   - or -
   ///   <paramref name="ssn"/> does not have length of 9 or 11.
   ///   - or -
   ///   <paramref name="ssn"/> contains a non-ASCII digit (not 0-9).
   ///   - or -
   ///   <paramref name="ssn"/> is 11 characters in length and contains an 
   ///   invalid separator character.
   ///   - or -
   ///   <paramref name="ssn"/> contains an invalid area number (000, 666 or 
   ///   900-999).
   ///   - or -
   ///   <paramref name="ssn"/> contains an invalid group number (00).
   ///   - or -
   ///   <paramref name="ssn"/> contains an invalid serial number (0000).
   ///   - or -
   ///   <paramref name="ssn"/> contains nine identical digits.
   ///   - or -
   ///   <paramref name="ssn"/> contains a run of consecutive digits from 1 to 9.
   /// </exception>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="separator"/> is an ASCII digit (0-9).
   /// </exception>
   public UsSocialSecurityNumber(String ssn, Char separator = '-')
   {
      _ = ssn.RequiresNotNullOrWhiteSpace(Messages.UsSsnEmpty);
      if (!ValidateLength(ssn, out var message))
      {
         throw new ArgumentException(message, nameof(ssn));
      }
      if (separator.IsAsciiDigit())
      {
         throw new ArgumentOutOfRangeException(
            nameof(separator), 
            separator,
            Messages.UsSsnInvalidSeparatorCharacter);
      }

      if (!TryGetSsnValue(ssn, separator, out var candidateValue, out message))
      {
         throw new ArgumentException(message, nameof(ssn));
      }

      // Perform final checks to confirm that the candidate value confirms to SSN
      // rules.
      if (!ValidateAreaNumber(candidateValue, out  message))
      {
         throw new ArgumentException(message, nameof(ssn));
      }
      if (!ValidateGroupNumber(candidateValue, out message))
      {
         throw new ArgumentException(message, nameof(ssn));
      }
      if (!ValidateSerialNumber(candidateValue, out message))
      {
         throw new ArgumentException(message, nameof(ssn));
      }
      if (!ValidateNotAllIdenticalDigits(candidateValue, out message))
      {
         throw new ArgumentException(message, nameof(ssn));
      }
      if (!ValidateNotConsecutiveRun(candidateValue, out message))
      {
         throw new ArgumentException(message, nameof(ssn));
      }

      _ssn = candidateValue;
   }

   public static implicit operator String(UsSocialSecurityNumber ssn) => ssn._ssn;

   private static Boolean IsFormattedSsn(String ssn) => ssn.Length == FormattedLength;

   private static Boolean IsSeparatorPosition(Int32 position)
   {
      const Int32 AreaSeparatorOffset = 3;
      const Int32 GroupSeparatorOffset = 6;

      return position == AreaSeparatorOffset || position == GroupSeparatorOffset;
   }

   private static Boolean TryGetSsnValue(
      String ssn, 
      Char separator,
      out String value,
      out String errorMessage)
   {

      value = String.Empty;
      errorMessage = String.Empty;

      var digits = new Char[NonFormattedLength];
      var inputOffset = 0;
      var resultOffset = 0;

      while(inputOffset < ssn.Length)
      {
         var currentChar = ssn[inputOffset];
         if (IsFormattedSsn(ssn) && IsSeparatorPosition(inputOffset))
         {
            // Validate that the separator is expected character, but do not
            // include separator in array of valid digits.
            if (currentChar != separator)
            {
               errorMessage = String.Format(
                  Messages.UsSsnInvalidSeparatorEncountered,
                  inputOffset,
                  separator,
                  currentChar);
               return false;
            }
         }
         else
         {
            if (!currentChar.IsAsciiDigit())
            {
               errorMessage = String.Format(
                  Messages.UsSsnInvalidCharacterEncountered,
                  inputOffset,
                  currentChar);
               return false;
            }

            digits[resultOffset] = currentChar;
            resultOffset++;
         }
         inputOffset++;
      }

      value = new String(digits);
      return true;
   }

   private static Boolean ValidateAreaNumber(
      ReadOnlySpan<Char> span,
      out String errorMessage)
   {
      const Int32 AreaLength = 3;
      const String InvalidArea000 = "000";
      const String InvalidArea666 = "666";

      if (span[0] == Chars.DigitNine
         || MemoryExtensions.Equals(span[..AreaLength], InvalidArea000.AsSpan(), StringComparison.Ordinal)
         || MemoryExtensions.Equals(span[..AreaLength], InvalidArea666.AsSpan(), StringComparison.Ordinal))
      {
         errorMessage = Messages.UsSsnInvalidAreaNumber;
         return false;
      }

      errorMessage = String.Empty;
      return true;
   }

   private static Boolean ValidateGroupNumber(
      ReadOnlySpan<Char> span,
      out String errorMessage)
   {
      const Int32 GroupStart = 3;
      const Int32 GroupEnd = 5;     // Last char in group number is actually index position 4, but ranges are exclusive of end index
      const String InvalidGroup00 = "00";

      if (MemoryExtensions.Equals(span[GroupStart..GroupEnd], InvalidGroup00.AsSpan(), StringComparison.Ordinal))
      {
         errorMessage = Messages.UsSsnInvalidGroupNumber;
         return false;
      }

      errorMessage = String.Empty;
      return true;
   }

   private static Boolean ValidateLength(String ssn, out String errorMessage)
   {
      if (ssn?.Length == NonFormattedLength || ssn?.Length == FormattedLength)
      {
         errorMessage = String.Empty;
         return true;
      }

      errorMessage = Messages.UsSsnInvalidLength;
      return false;
   }

   private static Boolean ValidateNotAllIdenticalDigits(String ssn, out String errorMessage)
   {
      var firstChar = ssn[0];
      for (var index = 1; index < ssn.Length; index++)
      {
         if (ssn[index] != firstChar)
         {
            errorMessage = String.Empty;
            return true;
         }
      }

      errorMessage = Messages.UsSsnAllIdenticalDigits;
      return false;
   }

   private static Boolean ValidateNotConsecutiveRun(
      String ssn,
      out String errorMessage)
   {
      const String InvalidConsecutiveRun = "123456789";

      if (String.Equals(ssn, InvalidConsecutiveRun, StringComparison.Ordinal))
      {
         errorMessage = Messages.UsSsnInvalidRun;
         return false;
      }

      errorMessage = String.Empty;
      return true;
   }

   private static Boolean ValidateSerialNumber(
      ReadOnlySpan<Char> span,
      out String errorMessage)
   {
      const Int32 SerialStart = 5;
      const String InvalidSerial0000 = "0000";

      if (MemoryExtensions.Equals(span[SerialStart..], InvalidSerial0000.AsSpan(), StringComparison.Ordinal))
      {
         errorMessage = Messages.UsSsnInvalidSerialNumber;
         return false;
      }

      errorMessage = String.Empty;
      return true;
   }

}
