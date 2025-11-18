// Ignore Spelling: ssn
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Strongly typed business object for a US Social Security Number (SSN).
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
///      Not all 9-digit numbers are valid Social Security Numbers. When 
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
///               The area number 666 is invalid.
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
///               The group number 00 is invalid.
///            </description>
///         </item>
///         <item>
///            <description>
///               The serial number 0000 is invalid.
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
   public const Char DefaultSeparator = Chars.Dash;

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
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="separator"/> is an ASCII digit (0-9).
   /// </exception>
   /// <exception cref="InvalidUsSocialSecurityNumberException">
   ///   <paramref name="ssn"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
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
   public UsSocialSecurityNumber(String? ssn, Char separator = DefaultSeparator)
      : this(ssn, separator, validationRequired: true)
   {
      // NOP - all work done in private constructor.
   }

   /// <summary>
   ///   Private constructor that performs the actual work. Supports bypassing
   ///   validation when invoked by Create method.
   /// </summary>
   private UsSocialSecurityNumber(
      String? ssn,
      Char separator,
      Boolean validationRequired)
   {
      if (validationRequired)
      {
         if (!ValidateSeparatorCharacter(separator))
         {
            throw new ArgumentOutOfRangeException(nameof(separator), separator, Messages.UsSsnInvalidCustomSeparatorCharacter);
         }

         var validationResult = ValidateSsn(ssn, separator);
         if (validationResult != UsSocialSecurityNumberValidationResult.ValidationPassed)
         {
            throw new InvalidUsSocialSecurityNumberException(validationResult);
         }
      }

      Value = GetValidatedSsn(ssn!);
   }

   /// <summary>
   ///   The raw SSN value.
   /// </summary>
   public String Value { get; init; }

   public static implicit operator String(UsSocialSecurityNumber ssn) 
      => ssn?.Value ?? throw new ArgumentNullException(nameof(ssn), Messages.UsSsnInvalidNullConversionToString);

   public static implicit operator UsSocialSecurityNumber(String? ssn) => new(ssn, DefaultSeparator);

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
   ///   A <see cref="CreateResult{UsSocialSecurityNumber, UsSocialSecurityNumberValidationResult}"/>.
   ///   Will contain the new <see cref="UsSocialSecurityNumber"/> if 
   ///   <paramref name="ssn"/> is valid or 
   ///   <see cref="UsSocialSecurityNumberValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="ssn"/> is 
   ///   invalid.
   /// </returns>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="separator"/> is an ASCII digit (0-9).
   /// </exception>
   public static CreateResult<UsSocialSecurityNumber, UsSocialSecurityNumberValidationResult> Create(
      String? ssn,
      Char separator = DefaultSeparator)
   {
      if (!ValidateSeparatorCharacter(separator))
      {
         throw new ArgumentOutOfRangeException(nameof(separator), separator, Messages.UsSsnInvalidCustomSeparatorCharacter);
      }

      var validationResult = ValidateSsn(ssn, separator);
      return validationResult == UsSocialSecurityNumberValidationResult.ValidationPassed
         ? new UsSocialSecurityNumber(ssn, separator, validationRequired: false)
         : validationResult;
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
   public String Format(String mask = "___-__-____") => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the SSN.
   /// </summary>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="ssn"/> to determine if it contains any 
   ///   validation errors.
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
   ///   A <see cref="UsSocialSecurityNumberValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="ssn"/> passed validation
   ///   or what validation error was encountered.
   /// </returns>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="separator"/> is an ASCII digit (0-9).
   /// </exception>
   public static UsSocialSecurityNumberValidationResult Validate(
      String? ssn,
      Char separator = DefaultSeparator)
      => !ValidateSeparatorCharacter(separator)
         ? throw new ArgumentOutOfRangeException(nameof(separator), separator, Messages.UsSsnInvalidCustomSeparatorCharacter)
         : ValidateSsn(ssn, separator);

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
   ///   Get an unformatted SSN value from a string that has passed validation.
   ///   If the source string is formatted, then create a new string by merging
   ///   all three SSN sections together without allocating intermediate 
   ///   Strings.
   /// </summary>
   private static String GetValidatedSsn(String ssn)
   {
      if (ssn.Length == NonFormattedLength)
      {
         return ssn;
      }

      var buffer = ArrayPool<Char>.Shared.Rent(NonFormattedLength);
      try
      {
         var span = new Span<Char>(buffer);
         var areaNumber = GetAreaNumber(ssn);
         var groupNumber = GetGroupNumber(ssn);
         var serialNumber = GetSerialNumber(ssn);
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

   private static Boolean ValidateAllDigits(ReadOnlySpan<Char> ssn)
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
            return false;
         }

         index++;
      }

      return true;
   }

   private static Boolean ValidateAreaNumber(ReadOnlySpan<Char> areaNumber)
   {
      const String InvalidArea000 = "000";
      const String InvalidArea666 = "666";

      return areaNumber[0] != Chars.DigitNine
         && !areaNumber.Equals(InvalidArea000, StringComparison.Ordinal)
         && !areaNumber.Equals(InvalidArea666, StringComparison.Ordinal);
   }

   private static Boolean ValidateEmbeddedSeparatorCharacters(
      ReadOnlySpan<Char> ssn,
      Char separator)
      // If ssn is formatted, must contain valid separator character between area, group and serial number sections.
      => ssn.Length == NonFormattedLength || (ssn[GroupSeparatorOffset] == separator && ssn[SerialSeparatorOffset] == separator);

   private static Boolean ValidateGroupNumber(ReadOnlySpan<Char> groupNumber)
   {
      const String InvalidGroup00 = "00";

      return !groupNumber.Equals(InvalidGroup00, StringComparison.Ordinal);
   }

   private static Boolean ValidateLength(ReadOnlySpan<Char> ssn)
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
         foreach (var ch in span)
         {
            if (ch != initialChar)
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

      return !areaNumber.Equals(AreaRun, StringComparison.Ordinal)
         && !groupNumber.Equals(GroupRun, StringComparison.Ordinal)
         && !serialNumber.Equals(SerialRun, StringComparison.Ordinal);
   }

   // A separator character may be any character except ASCII digits (which are
   // valid SSN characters).
   private static Boolean ValidateSeparatorCharacter(Char separator)
      => !separator.IsAsciiDigit();

   private static Boolean ValidateSerialNumber(ReadOnlySpan<Char> serialNumber)
   {
      const String InvalidSerial0000 = "0000";

      return !serialNumber.Equals(InvalidSerial0000, StringComparison.Ordinal);
   }

   private static UsSocialSecurityNumberValidationResult ValidateSsn(
      ReadOnlySpan<Char> ssn,
      Char separator)
   {
      // Preliminary checks for obviously incorrect values.
      if (ssn.IsEmpty || ssn.IsWhiteSpace())
      {
         return UsSocialSecurityNumberValidationResult.Empty;
      }
      if (!ValidateLength(ssn))
      {
         return UsSocialSecurityNumberValidationResult.InvalidLength;
      }
      if (IsFormattedSsn(ssn) && !ValidateEmbeddedSeparatorCharacters(ssn, separator))
      {
         return UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered;
      }
      if (!ValidateAllDigits(ssn))
      {
         return UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered;
      }

      // We know that the value contains 9 digits. Perform higher level checks
      // on the individual sections and the entire value.
      var areaNumber = GetAreaNumber(ssn);
      if (!ValidateAreaNumber(areaNumber))
      {
         return UsSocialSecurityNumberValidationResult.InvalidAreaNumber;
      }
      var groupNumber = GetGroupNumber(ssn);
      if (!ValidateGroupNumber(groupNumber))
      {
         return UsSocialSecurityNumberValidationResult.InvalidGroupNumber;
      }
      var serialNumber = GetSerialNumber(ssn);
      if (!ValidateSerialNumber(serialNumber))
      {
         return UsSocialSecurityNumberValidationResult.InvalidSerialNumber;
      }
      if (!ValidateNotAllIdenticalDigits(areaNumber, groupNumber, serialNumber))
      {
         return UsSocialSecurityNumberValidationResult.AllIdenticalDigits;
      }
      if (!ValidateNotConsecutiveRun(areaNumber, groupNumber, serialNumber))
      {
         return UsSocialSecurityNumberValidationResult.InvalidRun;
      }

      return UsSocialSecurityNumberValidationResult.ValidationPassed;
   }
}
