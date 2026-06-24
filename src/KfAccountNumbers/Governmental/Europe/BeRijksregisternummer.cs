// Ignore Spelling: Json Nummer Rijksregisternummer

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a Belgian
///   rijksregisternummer or a Belgian BIS-nummer (for non-residents).
/// </summary>
/// <remarks>
///   <para>
///      Rijksregisternummer and BIS-nummer both are 11-digit numbers,
///      structured as YYMMDDXXXCC, with the following elements.
///      <list type="bullet">
///         <item>
///            <term>YYMMDD</term>
///            <description>
///               The person's date of birth in YYMMDD format. A BIS number is
///               differentiated from a rijksregisternummer by the addition of a
///               constant value (40 or 20, see below) to the month component of
///               the date of birth.
///            </description>
///         </item>
///         <item>
///            <term>XXX</term>
///            <description>
///               Three digit sequence number used to differentiate between
///               persons born on the same date. The sequence number also
///               indicates gender with odd numbers for males and even numbers
///               for females.
///            </description>
///         </item>
///         <item>
///            <term>CC</term>
///            <description>
///               Two digit modulus 97 check sum calculated for the YYMMDD and
///               XXX elements. The check sum is also used to indicate century
///               of birth. If CC is equal to the normal modulus 97 check sum
///               then the persons' century of birth is 1900-1999. If CC is
///               equal to the modulus 97 check sum calculated by first
///               prefixing YYMMDDXXX with the digit 2 (i.e. 2YYMMDDXXX) then
///               the person's century of birth is 2000-2099.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A Belgian rijksregisternummer may be formatted as a string of 11
///      consecutive digits (YYMMDDXXXCC) or as a 15 character string with
///      characters separating the individual elements. YY.MM.DD-XXX.CC is the
///      typical display format.
///   </para>
///   <para>
///      When creating a new <see cref="BeRijksregisternummer"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 11 characters (without separators) or
///               15 characters (with separators) in length.
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
///               The separator characters, if included, must not be ASCII
///               digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The two trailing (right-most) characters must be a valid
///               modulus 97 check sum (taking into account the possibility of a
///               person born in the year 2000 or later).
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth, after deriving the century of birth from
///               the check sum and taking into account the BIS number offset,
///               must be a valid date between January 1, 1900 and December 31,
///               2099.
///               <b>OR</b> the date of birth may use zeros to indicate that
///               some or all of the person's date of birth is unknown (see
///               below for more details).
///            </description>
///         </item>
///         <item>
///            <description>
///               The sequence number may not be 000 or 999.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The date of birth can be adjusted in a variety of ways:
///      <list type="bullet">
///         <item>
///            <description>
///               If the person's date of birth is incomplete, then the two
///               digit year is used and zeros are used for month and day (for
///               example, 40.00.00-955.69).
///            </description>
///         </item>
///         <item>
///            <description>
///               If there are too many people with incomplete dates of birth
///               for a particular year than can be represented by a three digit
///               sequence number (i.e. more than 499 males with incomplete
///               dates of birth for the year 1940), then 01 is used for the day
///               of birth and the sequence number rolls over to 001
///               (ex. 40.00.01-001.33). (Note that
///               <see cref="BeRijksregisternummer"/> does not enforce an upper
///               limit on the day component in cases of rollover, though
///               multiple rollovers in a single year should be rare.)
///            </description>
///         </item>
///         <item>
///            <description>
///               If the person's date of birth is unknown, then the constant
///               00.00.01 is used.
///            </description>
///         </item>
///         <item>
///            <description>
///               As noted above, if the value is a BIS number then 40 is added
///               to the month component of the date of birth.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the value is a BIS number <b>AND</b> the person's gender is
///               unknown at the time the number is issued then <b>20</b> is
///               added to the month component of the date of birth.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      For cases of a BIS number for a person with an incomplete or unknown
///      date of birth, <see cref="BeRijksregisternummer"/> stacks the
///      appropriate rules. For example, 87.40.00-023.47 would be the BIS number
///      for a person with an incomplete date of birth born in 1987.
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>85.07.30-033.28</term>
///            <description>
///               Rijksregisternummer, date of birth July 30, 1985, gender = male,
///               check digit calculation 97 - (850730033 mod 97) = 97 - 69 = 28
///            </description>
///         </item>
///         <item>
///            <term>17110804680</term>
///            <description>
///               Rijksregisternummer, date of birth November 11, 2017, gender = female,
///               check digit calculation 97 - (2171108046 mod 97) = 97 - 17 = 80
///            </description>
///         </item>
///         <item>
///            <term>40 00 00 955-79</term>
///            <description>
///               Rijksregisternummer, date of birth incomplete, year of birth = 1940,
///               gender = male, check digit calculation 97 - (400000955 mod 97) = 97 - 18 = 79
///            </description>
///         </item>
///         <item>
///            <term>00 00 01 003-64</term>
///            <description>
///               Rijksregisternummer, date of birth unknown, gender = male,
///               check digit calculation 97 - (000001003 mod 97) = 97 - 33 = 64
///            </description>
///         </item>
///         <item>
///            <term>17.51.08-046.40</term>
///            <description>
///               BIS number, date of birth November 11, 1917, gender = female,
///               check digit calculation 97 - (175108046 mod 97) = 97 - 57 = 40
///            </description>
///         </item>
///         <item>
///            <term>09 20 00 002 65</term>
///            <description>
///              BIS number, date of birth incomplete, year of birth 2009, gender unknown,
///              check digit calculation 97 - (2092000002 mod 97) = 97 - 32 = 65
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://fr.wikipedia.org/wiki/Num%C3%A9ro_de_registre_national (French) for more info.
///   </para>
/// </remarks>
[JsonConverter(typeof(BeRijksregisternummerJsonConverter))]
public record BeRijksregisternummer
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="BeRijksregisternummer"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      BeRijksregisternummerInvalidSequenceNumber,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="BeRijksregisternummer"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      BeRijksregisternummerInvalidSequenceNumber,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   Represents the month offset used to distinguish BIS-nummers from
   ///   rijksregisternummers when the person's gender is known.
   /// </summary>
   /// <remarks>
   ///   In Belgian identity numbers, a BIS-nummer is indicated by
   ///   adding a constant to the month component of the date of birth.
   /// </remarks>
   public const Int32 BisNummerMonthOffset = 40;

   /// <summary>
   ///   Represents the month offset used to distinguish BIS-nummers from
   ///   rijksregisternummers when the person's gender is unknown.
   /// </summary>
   /// <remarks>
   ///   In Belgian identity numbers, a BIS-nummer is indicated by
   ///   adding a constant to the month component of the date of birth.
   /// </remarks>
   public const Int32 BisNummerUnknownGenderMonthOffset = 20;

   /// <summary>
   ///   The name of the check digit algorithm used by rijksregisternummer
   ///   values.
   /// </summary>
   public const String CheckDigitAlgorithmName = "Modulus 97";

   /// <summary>
   ///   The latest year of birth supported by
   ///   <see cref="BeRijksregisternummer"/>.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2099;

   /// <summary>
   ///   The earliest year of birth supported by
   ///   <see cref="BeRijksregisternummer"/>.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1900;

   private const Int32 UnformattedLength = 11;
   private const Int32 FormattedLength = 15;

   private const Int32 Separator1Offset = 2;
   private const Int32 Separator2Offset = 5;
   private const Int32 Separator3Offset = 8;
   private const Int32 Separator4Offset = 12;

   private static readonly Int32[] _separatorOffsets =
   [
      Separator1Offset,
      Separator2Offset,
      Separator3Offset,
      Separator4Offset
   ];

   // These items are measured from the end of the value.
   private const Int32 GenderOffset = 3;
   private const Int32 CheckDigit1Offset = 2;
   private const Int32 CheckDigit2Offset = 1;

   /// <summary>
   ///   Initializes a new instance of the <see cref="BeRijksregisternummer"/>
   ///   class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Belgian rijksregisternummer.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 11 (or 15 if separator
   ///   characters are used).
   ///   - or -
   ///   <paramref name="value"/> contains a non-digit character in
   ///   any position other than the separator locations.
   ///   - or -
   ///   <paramref name="value"/> has invalid modulus 97 check digit
   ///   characters in the trailing (right-most) character positions.
   ///   - or -
   ///   <paramref name="value"/> is 15 characters in length and has
   ///   an ASCII digit character ('0'-'9') in a separator location.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid date of birth in
   ///   the leading (left-most) six digits.
   /// </exception>
   public BeRijksregisternummer(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="BeRijksregisternummer"/>
   ///   class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private BeRijksregisternummer(String? value, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         ValidationResult validationResult = Validate(value);
         if (validationResult.Value is not ValidValue)
         {
            throw validationResult switch
            {
               EmptyValue emptyValue => new UKfValidationException<ValidationError>(emptyValue),
               InvalidLength invalidLength => new UKfValidationException<ValidationError>(invalidLength),
               InvalidCharacter invalidCharacter => new UKfValidationException<ValidationError>(invalidCharacter),
               InvalidChecksum invalidChecksum => new UKfValidationException<ValidationError>(invalidChecksum),
               InvalidSeparator invalidSeparator => new UKfValidationException<ValidationError>(invalidSeparator),
               BeRijksregisternummerInvalidSequenceNumber invalidSequenceNumber => new UKfValidationException<ValidationError>(invalidSequenceNumber),
               InvalidDateOfBirth invalidDateOfBirth => new UKfValidationException<ValidationError>(invalidDateOfBirth),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetRawValue(value!);
   }

   /// <summary>
   ///   Gets the person's date of birth, derived from the first six digits in
   ///   YYMMDD format and the exact century of birth derived from the check
   ///   digits.
   /// </summary>
   public DateResult DateOfBirth
   {
      get
      {
#pragma warning disable IDE0008 // Use explicit type
         var (year, month, day) = GetYearMonthDay(Value);
#pragma warning restore IDE0008 // Use explicit type

#pragma warning disable format
         return (year, month, day) switch
         {
            (> 0, > 0, > 0) => new DateResult(year, month, day),
            (> 0, 0, _) => new DateResult(year),
            _ => new DateResult(),
         };
#pragma warning restore format
      }
   }

   /// <summary>
   ///   Gets the person's gender, as indicated by the sequence number (and in
   ///   the case of a BIS-nummer, the month offset).
   /// </summary>
   public BinaryOrUnknownGender Gender
   {
      get
      {
         ReadOnlySpan<Char> span = Value.AsSpan();

         // Check for BIS-nummer with unknown gender.
         var num = span[2..].ParseTwoDigits();
         if (num is >= 20 and <= 32)
         {
            return BinaryOrUnknownGender.Unknown;
         }

         return span[^GenderOffset] % 2 == 0 // This works because the ASCII character values for digits have the same odd/even pattern
            ? BinaryOrUnknownGender.Female
            : BinaryOrUnknownGender.Male;
      }
   }

   /// <summary>
   ///   Gets the type of Belgian identifier represented by the current value.
   /// </summary>
   /// <remarks>
   ///   The month component of the date of birth determines the identifier type.
   ///   BIS-nummers add an offset (either 20 or 40) to the month so month values
   ///   greater than 12 indicate that the identifier is a BIS-nummer.
   /// </remarks>
   public BeIdentifierType IdentifierType
      => Value.AsSpan(2..).ParseTwoDigits() > 12
         ? BeIdentifierType.BisNummer
         : BeIdentifierType.Rijksregisternummer;

   /// <summary>
   ///   Gets the raw rijksregisternummer value.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="BeRijksregisternummer"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="BeRijksregisternummer"/> to convert.
   /// </param>
   public static implicit operator String(BeRijksregisternummer source)
      => source?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="BeRijksregisternummer"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Belgian rijksregisternummer.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid rijksregisternummer.
   /// </exception>
   public static explicit operator BeRijksregisternummer(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="BeRijksregisternummer"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Belgian rijksregisternummer.
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{BeRijksregisternummer, ValidationError}"/>. Will
   ///   contain the new <see cref="BeRijksregisternummer"/> if <paramref name="value"/>
   ///   is valid or a <see cref="ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<BeRijksregisternummer, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new BeRijksregisternummer(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         BeRijksregisternummerInvalidSequenceNumber invalidSequenceNumber => (ValidationError)invalidSequenceNumber,
         InvalidDateOfBirth invalidDateOfBirth => (ValidationError)invalidDateOfBirth,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the rijksregisternummer using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then the default mask "__.__.__-___.__" will be used instead.
   /// </param>
   /// <returns>
   ///   A formatted rijksregisternummer.
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
   ///   details on creating a mask to format the rijksregisternummer.
   /// </remarks>
   public String Format(String mask = "__.__.__-___.__") => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the rijksregisternummer.
   /// </summary>
   /// <returns>
   ///   The raw burgerservicenummer, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Belgian rijksregisternummer.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Belgian rijksregisternummer.
   /// </param>
   /// <returns>
   ///   A <see cref="ValidationResult"/> union that indicates if the
   ///   <paramref name="value"/> passed validation or what validation error was
   ///   encountered.
   /// </returns>
   public static ValidationResult Validate(String? value)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         return default(EmptyValue);
      }

      if (value.Length is not UnformattedLength and not FormattedLength)
      {
         return new InvalidLength(
            Messages.BeRijksregisternummerInvalidLength,
            value.Length,
            GetValidLengthDefinitions());
      }

      // After performing basic checks, validate the check digits because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      ValidationResult validationResult = ValidateCheckDigits(value);
      if (validationResult is not ValidValue)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }

      var isFormatted = IsFormatted(value);
      if (!ValidateSeparators(value, out var invalidSeparatorPosition))
      {
         return new InvalidSeparator(
            Messages.BeRijksregisternummerInvalidSeparator,
            value[invalidSeparatorPosition],
            invalidSeparatorPosition);
      }

      if (!ValidateSequenceNumber(value))
      {
         return new BeRijksregisternummerInvalidSequenceNumber(
            Messages.BeRijksregisternummerInvalidSequenceNumber,
            isFormatted ? value[9..12] : value[6..9]);
      }

      if (!ValidateDateOfBirth(value))
      {
         var dateOfBirthLength = isFormatted ? 8 : 6;
         return new InvalidDateOfBirth(
            Messages.BeRijksregisternummerInvalidDateOfBirth,
            value[..dateOfBirthLength],
            DateFormatName.YYMMDD);
      }

      return default(ValidValue);
   }

   /// <summary>
   ///   Gets an array of details about valid lengths accepted for a
   ///   rijksregisternummer.
   /// </summary>
   /// <returns>
   ///   An array of <see cref="ValidLengthDefinition"/>s.
   /// </returns>
   internal static ValidLengthDefinition[] GetValidLengthDefinitions()
      =>
      [
         new ValidLengthDefinition(UnformattedLength, Messages.BeRijksregisternummerUnformattedLength),
         new ValidLengthDefinition(FormattedLength, Messages.BeRijksregisternummerFormattedLength),
      ];

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(
         Messages.BeRijksregisternummerInvalidCharacter,
         value[position],
         position);

   private static String GetRawValue(String value)
   {
      if (value.Length == UnformattedLength)
      {
         return value;
      }

      var buffer = ArrayPool<Char>.Shared.Rent(UnformattedLength);
      try
      {
         ReadOnlySpan<Char> source = value.AsSpan();
         var span = new Span<Char>(buffer);

         ReadOnlySpan<Int32> segmentLengths = [2, 2, 2, 3, 2];
         var sourceOffset = 0;
         var targetOffset = 0;
         foreach (var length in segmentLengths)
         {
            ReadOnlySpan<Char> sourceSpan = source[sourceOffset..(sourceOffset + length)];
            Span<Char> targetSpan = span[targetOffset..(targetOffset + length)];

            sourceSpan.CopyTo(targetSpan);

            sourceOffset += length + 1;
            targetOffset += length;
         }

         return span[..UnformattedLength].ToString();
      }
      finally
      {
         ArrayPool<Char>.Shared.Return(buffer);
      }
   }

   private static (Int32 Year, Int32 Month, Int32 Day) GetYearMonthDay(ReadOnlySpan<Char> value)
   {
      var fieldWidth = value.Length == UnformattedLength ? 2 : 3;
      var year = value.ParseTwoDigits();

      var fieldStart = fieldWidth;
      var month = value[fieldStart..].ParseTwoDigits();

      fieldStart += fieldWidth;
      var day = value[fieldStart..].ParseTwoDigits();

      fieldStart += fieldWidth;
      var sequenceNumber = value[fieldStart..].ParseThreeDigits();

      // Apply BIS-nummer offsets if necessary.
      var effectiveMonth = month switch
      {
         >= BisNummerMonthOffset => month - BisNummerMonthOffset,
         >= BisNummerUnknownGenderMonthOffset => month - BisNummerUnknownGenderMonthOffset,
         _ => month,
      };

      // Add the century to the year if the date is not incomplete.
      if (year > 0 || effectiveMonth > 0)
      {
         // Already parsed the individual elements, combine to use in checksum calculation.
         var total = sequenceNumber + (day * 1000) + (month * 100000) + (year * 10000000);
         var checksum = value[^2..].ParseTwoDigits();
         var century = (97 - (total % 97)) == checksum
            ? 1900
            : 2000;

         year += century;
      }

      return (year, effectiveMonth, day);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> value)
      => value.Length == FormattedLength;

   private static ValidationResult ValidateCheckDigits(ReadOnlySpan<Char> value)
   {
      var processLength = value.Length - 2;      // Exclude check digits from main loop
      var isFormatted = IsFormatted(value);

      var sum = 0;
      for (var index = 0; index < processLength; index++)
      {
         if (isFormatted &&
             (index is Separator1Offset or Separator2Offset or Separator3Offset or Separator4Offset))
         {
            continue;
         }

         sum *= 10;
         var num = value[index].ToSingleDigit();
         if (!num.IsValidDigit())
         {
            return GetInvalidCharacterResult(value, index);
         }

         sum += num;
      }

      var c1 = value[^CheckDigit1Offset].ToSingleDigit();
      if (!c1.IsValidDigit())
      {
         return GetInvalidCharacterResult(value, value.Length - CheckDigit1Offset);
      }

      var c2 = value[^CheckDigit2Offset].ToSingleDigit();
      if (!c2.IsValidDigit())
      {
         return GetInvalidCharacterResult(value, value.Length - CheckDigit2Offset);
      }

      var checkSum = (c1 * 10) + c2;

      // Check for persons born 1900-1999.
      var remainder = 97 - (sum % 97);
      if (remainder == checkSum)
      {
         return default(ValidValue);
      }

      // Then for persons born 2000-2099;
      var longRemainder = 97 - ((2000000000L + sum) % 97);           // Long int to handle possible int overflow
      return longRemainder == checkSum
         ? default(ValidValue)
         : new InvalidChecksum(
            Messages.BeRijksregisternummerInvalidCheckDigits,
            CheckDigitAlgorithmName);
   }

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> value)
   {
#pragma warning disable IDE0008 // Use explicit type
      var (year, month, day) = GetYearMonthDay(value);
#pragma warning restore IDE0008 // Use explicit type

      // Allow zero for incomplete dates of birth.
      if ((year > 0 && month == 0)                    // Incomplete date of birth
         || (year == 0 && month == 0 && day > 0))     // Unknown date of birth
      {
         return true;
      }

      if (year is < MinimumValidYearOfBirth or > MaximumValidYearOfBirth)
      {
         // Should be impossible to ever reach this point because of the check
         // digit calcuations, but return false out of abundance of caution and
         // to avoid throwing an exception.
         return false;
      }

      if (month is < 1 or > 12)
      {
         return false;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }

   private static Boolean ValidateSeparators(
      ReadOnlySpan<Char> value,
      out Int32 invalidSeparatorPosition)
   {
      invalidSeparatorPosition = -1;
      if (value.Length == UnformattedLength)
      {
         return true;
      }

      foreach (var offset in _separatorOffsets)
      {
         if (value[offset].IsAsciiDigit())
         {
            invalidSeparatorPosition = offset;
            return false;
         }
      }

      return true;
   }

   private static Boolean ValidateSequenceNumber(ReadOnlySpan<Char> value)
   {
      var offsetFromEnd = IsFormatted(value) ? 6 : 5;
      var sequenceNumber = value[^offsetFromEnd..].ParseThreeDigits();

      return sequenceNumber is not 0 and not 999;
   }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class BeRijksregisternummerJsonConverter : JsonConverter<BeRijksregisternummer>
{
   public override BeRijksregisternummer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new BeRijksregisternummer(str);
   }

   public override void Write(Utf8JsonWriter writer, BeRijksregisternummer value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
