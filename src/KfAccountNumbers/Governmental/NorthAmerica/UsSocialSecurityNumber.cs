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

   private const Int32 AreaRangeEnd = 3;                          // End indices are exclusive for use in range operator
   private const Int32 UnformattedGroupRangeStart = 3;
   private const Int32 UnformattedGroupRangeEnd = 5;
   private const Int32 UnformattedSerialNumberRangeStart = 5;
   private const Int32 FormattedGroupRangeStart = 4;
   private const Int32 FormattedGroupRangeEnd = 6;
   private const Int32 FormattedSerialNumberRangeStart = 7;

   private const Int32 GroupSeparatorOffset = 3;   // Offset of separator between area and group sections in formatted SSN
   private const Int32 SerialSeparatorOffset = 6;  // Offset of separator between group and serial number sections in formatted SSN

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
      // Preliminary checks for obviously incorrect values.
      ssn.ValidateNotNullOrWhiteSpace(Messages.UsSsnEmpty);
      if (!ValidateLength(ssn))
      {
         throw new ArgumentException(Messages.UsSsnInvalidLength, nameof(ssn));
      }
      if (IsFormattedSsn(ssn))
      {
         if (!ValidateSeparatorCharacter(separator))
         {
            throw new ArgumentOutOfRangeException(nameof(separator), separator, Messages.UsSsnInvalidSeparatorCharacter);
         }
         if (!ValidateEmbeddedSeparatorCharacters(ssn, separator, out var separatorCharMessage))
         {
            throw new ArgumentException(separatorCharMessage, nameof(ssn));
         }
      }
      if (!ValidateAllDigits(ssn, out var nonDigitMessage))
      {
         throw new ArgumentException(nonDigitMessage, nameof(ssn));
      }

      // Perform final checks to confirm that the candidate value confirms to SSN
      // rules.
      var areaNumber = GetAreaNumber(ssn);
      if (!ValidateAreaNumber(areaNumber))
      {
         throw new ArgumentException(Messages.UsSsnInvalidAreaNumber, nameof(ssn));
      }
      var groupNumber = GetGroupNumber(ssn);
      if (!ValidateGroupNumber(groupNumber))
      {
         throw new ArgumentException(Messages.UsSsnInvalidGroupNumber, nameof(ssn));
      }
      var serialNumber = GetSerialNumber(ssn);
      if (!ValidateSerialNumber(serialNumber))
      {
         throw new ArgumentException(Messages.UsSsnInvalidSerialNumber, nameof(ssn));
      }
      if (!ValidateNotAllIdenticalDigits(areaNumber, groupNumber, serialNumber))
      {
         throw new ArgumentException(Messages.UsSsnAllIdenticalDigits, nameof(ssn));
      }
      if (!ValidateNotConsecutiveRun(areaNumber, groupNumber, serialNumber))
      {
         throw new ArgumentException(Messages.UsSsnInvalidRun, nameof(ssn));
      }

      _ssn = GetValidatedSsn(areaNumber, groupNumber, serialNumber);
   }

   /// <summary>
   ///   Private constructor for use by <see cref="Create(String, Char)"/>
   ///   method.
   /// </summary>
   private UsSocialSecurityNumber(
      ReadOnlySpan<Char> areaNumber,
      ReadOnlySpan<Char> groupNumber,
      ReadOnlySpan<Char> serialNumber)
      => _ssn = GetValidatedSsn(areaNumber, groupNumber, serialNumber);

   public static implicit operator String(UsSocialSecurityNumber ssn) => ssn._ssn;

   public static implicit operator UsSocialSecurityNumber(String ssn) => new(ssn);

   /// <summary>
   ///   Create a new <see cref="UsSocialSecurityNumber"/>.
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
   /// <returns>
   ///   A <see cref="CreateResult{UsSocialSecurityNumber, UsSocialSecurityNumberErrorType}"/>.
   ///   Will contain the new <see cref="UsSocialSecurityNumber"/> if 
   ///   <paramref name="ssn"/> is valid or an error object if 
   ///   <paramref name="ssn"/> is invalid.
   /// </returns>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="separator"/> is an ASCII digit (0-9).
   /// </exception>
   public static CreateResult<UsSocialSecurityNumber, UsSocialSecurityNumberErrorType> Create(
      String ssn,
      Char separator = '-')
   {
      // Preliminary checks for obviously incorrect values.
      if (String.IsNullOrWhiteSpace(ssn))
      {
         return UsSocialSecurityNumberError.Empty;
      }
      if (!ValidateLength(ssn))
      {
         return UsSocialSecurityNumberError.InvalidLength;
      }
      if (IsFormattedSsn(ssn))
      {
         if (!ValidateSeparatorCharacter(separator))
         {
            throw new ArgumentOutOfRangeException(nameof(separator), separator, Messages.UsSsnInvalidSeparatorCharacter);
         }
         if (!ValidateEmbeddedSeparatorCharacters(ssn, separator, out var separatorCharMessage))
         {
            return UsSocialSecurityNumberError.InvalidSeparatorCharacter(separatorCharMessage!);
         }
      }
      if (!ValidateAllDigits(ssn, out var nonDigitMessage))
      {
         return UsSocialSecurityNumberError.InvalidCharacter(nonDigitMessage!);
      }

      // Perform final checks to confirm that the candidate value confirms to SSN
      // rules.
      var areaNumber = GetAreaNumber(ssn);
      if (!ValidateAreaNumber(areaNumber))
      {
         return UsSocialSecurityNumberError.InvalidAreaNumber;
      }
      var groupNumber = GetGroupNumber(ssn);
      if (!ValidateGroupNumber(groupNumber))
      {
         return UsSocialSecurityNumberError.InvalidGroupNumber;
      }
      var serialNumber = GetSerialNumber(ssn);
      if (!ValidateSerialNumber(serialNumber))
      {
         return UsSocialSecurityNumberError.InvalidSerialNumber;
      }
      if (!ValidateNotAllIdenticalDigits(areaNumber, groupNumber, serialNumber))
      {
         return UsSocialSecurityNumberError.AllIdenticalDigits;
      }
      if (!ValidateNotConsecutiveRun(areaNumber, groupNumber, serialNumber))
      {
         return UsSocialSecurityNumberError.InvalidRun;
      }

      return new UsSocialSecurityNumber(areaNumber, groupNumber, serialNumber);
   }

   /// <summary>
   ///   Format the SSN using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   The mask that specified the final output.
   /// </param>
   /// <returns>
   ///   A formatted Social Security Number.
   /// </returns>
   /// <exception cref="ArgumentNullException">
   ///   <paramref name="mask"/> is <see langword="null"/>.
   /// </exception>
   /// <exception cref="ArgumentException">
   ///   <paramref name="mask"/> is <see cref="String.Empty"/> or all whitespace
   ///   characters.
   /// </exception>
   /// <remarks>
   ///   <see cref="ExtensionMethods.FormatWithMask(String, String)"/> for more
   ///   details on creating a mask to format the SSN.
   /// </remarks>
   public String Format(String mask = "___-__-____") => _ssn.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the SSN.
   /// </summary>
   public override String ToString() => _ssn;

   private static ReadOnlySpan<Char> GetAreaNumber(ReadOnlySpan<Char> ssn)
      => ssn[..AreaRangeEnd];

   private static ReadOnlySpan<Char> GetGroupNumber(ReadOnlySpan<Char> ssn)
      => IsFormattedSsn(ssn)
         ? ssn[FormattedGroupRangeStart..FormattedGroupRangeEnd]
         : ssn[UnformattedGroupRangeStart..UnformattedGroupRangeEnd];

   private static ReadOnlySpan<Char> GetSerialNumber(ReadOnlySpan<Char> ssn)
      => IsFormattedSsn(ssn)
         ? ssn[FormattedSerialNumberRangeStart..]
         : ssn[UnformattedSerialNumberRangeStart..];

   /// <summary>
   ///   Merge all three SSN sections together without allocating intermediate
   ///   Strings.
   /// </summary>
   private static String GetValidatedSsn(
      ReadOnlySpan<Char> areaNumber,
      ReadOnlySpan<Char> groupNumber,
      ReadOnlySpan<Char> serialNumber)
   {
      var buffer = ArrayPool<Char>.Shared.Rent(NonFormattedLength);
      try
      {
         var span = new Span<Char>(buffer);
         areaNumber.CopyTo(span[..AreaRangeEnd]);
         groupNumber.CopyTo(span[UnformattedGroupRangeStart..UnformattedGroupRangeEnd]);
         serialNumber.CopyTo(span[UnformattedSerialNumberRangeStart..]);

         return span[..NonFormattedLength].ToString();
      }
      finally
      {
         ArrayPool<Char>.Shared.Return(buffer);
      }
   }

   private static Boolean IsFormattedSsn(ReadOnlySpan<Char> ssn) => ssn.Length == FormattedLength;

   private static Boolean ValidateAllDigits(String ssn, out String? message)
   {
      var index = 0;
      while (index < ssn.Length)
      {
         if (IsFormattedSsn(ssn) && (index == GroupSeparatorOffset || index == SerialSeparatorOffset))
         {
            index++;
         }

         if (!ssn[index].IsAsciiDigit())
         {
            message = String.Format(
              Messages.UsSsnInvalidCharacterEncountered,
              index,
              ssn[index]);
            return false;
         }

         index++;
      }

      message = default!;
      return true;
   }

   private static Boolean ValidateAreaNumber(ReadOnlySpan<Char> areaNumber)
   {
      const String InvalidArea000 = "000";
      const String InvalidArea666 = "666";

      return areaNumber[0] != Chars.DigitNine
         && !MemoryExtensions.Equals(areaNumber, InvalidArea000, StringComparison.Ordinal)
         && !MemoryExtensions.Equals(areaNumber, InvalidArea666, StringComparison.Ordinal);
   }

   private static Boolean ValidateEmbeddedSeparatorCharacters(
      String ssn,
      Char separator,
      out String? message)
   {
      if (IsFormattedSsn(ssn)
         && (ssn[GroupSeparatorOffset] != separator || ssn[SerialSeparatorOffset] != separator))
      {
         var offset = ssn[GroupSeparatorOffset] != separator
            ? GroupSeparatorOffset
            : SerialSeparatorOffset;
         message = String.Format(
            Messages.UsSsnInvalidSeparatorEncountered,
            offset,
            separator,
            ssn[offset]);
         return false;
      }

      message = default;
      return true;
   }

   private static Boolean ValidateGroupNumber(ReadOnlySpan<Char> groupNumber)
   {
      const String InvalidGroup00 = "00";

      return !MemoryExtensions.Equals(groupNumber, InvalidGroup00, StringComparison.Ordinal);
   }

   private static Boolean ValidateLength(String ssn)
      => ssn.Length == NonFormattedLength || ssn.Length == FormattedLength;

   private static Boolean ValidateNotAllIdenticalDigits(
      ReadOnlySpan<Char> areaNumber,
      ReadOnlySpan<Char> groupNumber,
      ReadOnlySpan<Char> serialNumber)
   {
      var initialChar = areaNumber[0];

      return !CheckSectionIdenticalDigits(areaNumber, initialChar)
         && !CheckSectionIdenticalDigits(groupNumber, initialChar)
         && !CheckSectionIdenticalDigits(serialNumber, initialChar);

      static Boolean CheckSectionIdenticalDigits(ReadOnlySpan<Char> span, Char initialChar)
      {
         for (var index = 0; index < span.Length; index++)
         {
            if (span[index] != initialChar)
            {
               return false;
            }
         }

         return true;
      }
   }

   private static Boolean ValidateNotConsecutiveRun(
      ReadOnlySpan<Char> areaNumber,
      ReadOnlySpan<Char> groupNumber,
      ReadOnlySpan<Char> serialNumber)
   {
      const String AreaRun = "123";
      const String GroupRun = "45";
      const String SerialRun = "6789";

      return !MemoryExtensions.Equals(areaNumber, AreaRun, StringComparison.Ordinal)
         && !MemoryExtensions.Equals(groupNumber, GroupRun, StringComparison.Ordinal)
         && !MemoryExtensions.Equals(serialNumber, SerialRun, StringComparison.Ordinal);
   }

   // A separator character may be any character except ASCII digits (which are
   // valid SSN characters).
   private static Boolean ValidateSeparatorCharacter(Char separator)
      => !separator.IsAsciiDigit();

   private static Boolean ValidateSerialNumber(ReadOnlySpan<Char> serialNumber)
   {
      const String InvalidSerial0000 = "0000";

      return !MemoryExtensions.Equals(serialNumber, InvalidSerial0000, StringComparison.Ordinal);
   }
}