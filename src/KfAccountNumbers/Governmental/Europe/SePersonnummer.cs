// Ignore Spelling: Json Personnummer Samordningsnummer

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents either of two identifiers
///   issued by the Swedish Tax Agency that have the same format and are used
///   for similar purposes. The first, the Personal Identity Number
///   (personnummer) is issued to persons born in Sweden or who are residents
///   of Sweden for 12 months or longer. The second, the coordination number
///   (samordningsnummer) is issued to persons who reside in Sweden for less
///   than a year. The <see cref="IdentifierType"/> property will indicate
///   the exact type of identifier represented by an instance of
///   <see cref="SePersonnummer"/>.
/// </summary>
/// <remarks>
///   <para>
///      Swedish personnummer and samordningsnummer values are both 11 or 13
///      character strings. The only difference between the two lengths are
///      the number of digits used to represent the date of birth, either
///      six or eight. The format of personnummer and samordningsnummer are
///      the same and consist of the following fields/sections:
///      <list type="bullet">
///         <item>
///            <description>
///               The date of birth, represented by either six or eight digits
///               (YYMMDD format or YYYYMMDD format). Originally six digits
///               were used but the eight digit format was introduced in 1997.
///            </description>
///         </item>
///         <item>
///            <description>
///               A separator character that separates the date of birth from
///               the remaining four digits. The separator character is
///               normally a dash ('-') but when a person turns 100 years old
///               the dash is replaced by a plus sign ('+').
///            </description>
///         </item>
///         <item>
///            <description>
///               A three digit birth serial number, issued serially as births
///               are recorded for a particular date. The last digit of the
///               birth serial number serves an additional purpose of indicating
///               the person's gender, with odd digits assigned to males and
///               even digits assigned to females.
///            </description>
///         </item>
///         <item>
///            <description>
///               A single check digit calculated using the Luhn algorithm
///               applied to the rightmost six digits of the date of birth and
///               to the birth serial number.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The only difference between a personnummer and a samordningsnummer is
///      that the samordningsnummer adds 60 to the day of a person's date of
///      birth (i.e. 950123 would become 950183).
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>890201-3286</term>
///            <description>
///               Personnummer, date of birth 890201, less than 100 years old,
///               gender = female, check digit = 6.
///            </description>
///         </item>
///         <item>
///            <term>19890201-3286</term>
///            <description>
///               Personnummer, date of birth 19890201, less than 100 years old,
///               gender = female, check digit = 6.
///            </description>
///         </item>
///         <item>
///            <term>811228+9874</term>
///            <description>
///               Personnummer, date of birth 811228, greater than 100 years old,
///               gender = male, check digit = 4.
///            </description>
///         </item>
///         <item>
///            <term>890261-3283</term>
///            <description>
///               Samordningsnummer, date of birth 890261 (actual date of birth
///               = 890201), less than 100 years old, gender = female, check
///               digit = 3.
///            </description>
///         </item>
///         <item>
///            <term>19890261-3283</term>
///            <description>
///               Samordningsnummer, date of birth 19890261 (actual date of birth
///               = 19890201), less than 100 years old, gender = female, check digit
///               = 3.
///            </description>
///         </item>
///         <item>
///            <term>811288+9871</term>
///            <description>
///               Samordningsnummer, date of birth 811288 (actual date of birth
///               = 811228), greater than 100 years old, gender = male, check
///               digit = 1.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      When creating a new <see cref="SePersonnummer"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The string must be either 11 or 13 characters long.
///            </description>
///         </item>
///         <item>
///            <description>
///               For 11-character strings, the first 6 characters must represent
///               a valid date in the format YYMMDD. For 13-character strings,
///               the first 8 characters must represent a valid date in the
///               format YYYYMMDD. Note that the validation specifically does
///               <b>NOT</b> check for future dates, only that the date exists.
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth must be followed by a valid separator
///               character. The separator must be either a dash (-) or a plus
///               sign (+).
///            </description>
///         </item>
///         <item>
///            <description>
///               The separator must be followed by a three digit birth serial
///               number.
///            </description>
///         </item>
///         <item>
///            <description>
///               The birth serial number must be followed by a  valid checksum
///               calculated using the Luhn algorithm based on the six digit
///               date of birth and the three-digit birth serial number. (The
///               leading two digits of an eight digit date of birth are
///               ignored.)
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Note that the encoded date of birth may not be the person's actual
///      date of birth. It is possible to run out of birth serial numbers for
///      a particular day and in this case a day close to the actual date of
///      birth is substituted in its stead.
///   </para>
///   <para>
///      When determining if a date of birth is valid, values with six digit
///      dates of birth use the separator character to derive the full four
///      digit year. Year values between 00 and 49 are assumed to be 2000 to
///      2049 and year values between 50 and 99 are assumed to be 1950 to 1999.
///      If the separator character indicates that the person is at least 100
///      years of age, then 100 is subtracted from the year, resulting in 00 to
///      40 meaning 1900 to 1949 and 50 to 99 meaning 1850 to 1899.
///   </para>
///   <para>
///      The valid range for a date of birth is January 1, 1800 to
///      December 31, 2099. However, if a six digit date of birth is supplied
///      then the valid range will be between January 1, 1850 to December 31, 2049.
///   </para>
///   <para>
///      For samordningsnummer values, the value returned by the
///      <see cref="DateOfBirth"/> property is an actual date calculated by
///      subtracting 60 from the encoded date of birth.
///   </para>
///   <para>
///      Internally, <see cref="SePersonnummer"/> stores a 12-digit representation
///      consisting of the date of birth in YYYYMMDD format followed by the birth
///      serial number and check digit (no separator). The <see cref="Value"/>
///      property returns this internal representation.
///   </para>
///   <para>
///      When comparing <see cref="SePersonnummer"/> objects for equality, the
///      internal 12-digit representation is used. This means two objects
///      representing the same person will be considered equal regardless of
///      whether they were created from 11-character or 13-character input strings.
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
///      for more details.
///   </para>
/// </remarks>
[JsonConverter(typeof(SePersonnummerJsonConverter))]
public record SePersonnummer
{
   /// <summary>
   ///   Discriminated union defining the types of identifier that
   ///   <see cref="SePersonnummer"/> can represent.
   /// </summary>
   public union IdentifierCategory(SeIdentifierType.Personnummer, SeIdentifierType.Samordningsnummer) { }

   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="SePersonnummer"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="SePersonnummer"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   The latest year of birth supported by <see cref="SePersonnummer"/>.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2099;

   /// <summary>
   ///   The earliest year of birth supported by <see cref="SePersonnummer"/>.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1800;

   /// <summary>
   ///   Represents the day offset used to distinguish Swedish coordination numbers
   ///   (samordningsnummer) from personnummers.
   /// </summary>
   /// <remarks>
   ///   In Swedish personal identity numbers, a Samordningsnummer is indicated by
   ///   adding 60 to the day component of the date of birth.
   /// </remarks>
   public const Int32 SamordningsnummerDayOffset = 60;

   private const Int32 InternalRepresentationLength = 12;      // YYYYMMDD + birth serial number + check digit
   private const Int32 ShortFormatLength = 11;
   private const Int32 LongFormatLength = 13;

   // These offsets are measured from the end of the string instead of the start
   // because the date of birth has variable length.
   private const Int32 SeparatorOffset = 5;
   private const Int32 GenderOffset = 2;

   /// <summary>
   ///   Initializes a new instance of the <see cref="SePersonnummer"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a personnummer.
   /// </param>
   /// <exception cref="KfValidationException{SePersonnummerValidationResult}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 11 or 13.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid date of birth in
   ///   positions 0-5 (11 character values) or positions 0-7 (13 character
   ///   values).
   ///   - or -
   ///   <paramref name="value"/> contains an invalid separator character
   ///   in position 6 (11 character values) or position 8 (13 character values).
   ///   Valid separator characters are dash ('-') and plus ('+').
   ///   - or -
   ///   <paramref name="value"/> contains an invalid birth serial number
   ///   (i.e. one or more non-digit characters) in positions 7-9 (11 character
   ///   values) or positions 9-11 (13 character values).
   ///   - or -
   ///   <paramref name="value"/> contains an invalid check digit in
   ///   position 10 (11 character values) or position 12 (13 character values).
   ///   The check digit is calculated using the Luhn algorithm based on the six
   ///   digit date of birth and the three-digit birth serial number. (The
   ///   leading two digits of an eight digit date of birth are ignored.)
   /// </exception>
   /// <remarks>
   ///   The indices given in the exception description are all zero-based.
   /// </remarks>
   public SePersonnummer(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="SePersonnummer"/> class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private SePersonnummer(String? value, ValidationMode validationMode)
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
               InvalidDateOfBirth invalidDateOfBirth => new UKfValidationException<ValidationError>(invalidDateOfBirth),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetInternalRepresentation(value);
   }

   /// <summary>
   ///   Gets the person's date of birth, derived from the date of birth portion
   ///   of 13 character personnummer (YYYYMMDD format) or from the date portion
   ///   of an 11 character personnummer (YYMMDD format) and the separator
   ///   character.
   /// </summary>
   /// <remarks>
   ///   <para>
   ///   See the class comments for the rules for deriving the date of birth
   ///   for an 11 character personnummer.
   ///   </para>
   ///   <para>
   ///   Note that the indicated date of birth may not be the person's exact
   ///   date of birth as it is possible that a day may run out of birth
   ///   serial numbers. In that case, a date close to the actual date of
   ///   birth is used instead.
   ///   </para>
   ///   <para>
   ///   Note also that samordningsnummer values encode the date of birth by
   ///   adding 60 to the day (i.e. "950123" encodes as "950183"). If the
   ///   personnummer is actually a samordningsnummer, then 60 will be
   ///   subtracted from the day to get the actual numeric day.
   ///   </para>
   /// </remarks>
   public DateOnly DateOfBirth
   {
      get
      {
#pragma warning disable IDE0008 // Use explicit type
         var (year, month, day) = GetYearMonthDay(Value);
#pragma warning restore IDE0008 // Use explicit type

         return new DateOnly(year, month, day);
      }
   }

   /// <summary>
   ///   Gets the person's gender, as indicated by the third character of the
   ///   birth sequence number. Odd digits = Male; even digits = Female.
   /// </summary>
   public BinaryGender Gender => Value[^GenderOffset] % 2 == 0 // This works because the ASCII character values for digits have the same odd/even pattern
      ? BinaryGender.Female
      : BinaryGender.Male;

   /// <summary>
   ///   Gets the type of Swedish identifier represented by this instance,
   ///   indicating whether it is a personal identity number (personnummer) or a
   ///   coordination number (samordningsnummer).
   /// </summary>
   /// <remarks>
   ///   A personnummer will have a date of birth with a day component in the
   ///   normal range (1-31) while a samordningsnummer will add 60 to the day
   ///   component of the day of birth resulting in values from 61-91.
   /// </remarks>
   public IdentifierCategory IdentifierType
      => Value.AsSpan(6..8).ParseTwoDigits() > SamordningsnummerDayOffset
         ? default(SeIdentifierType.Samordningsnummer)
         : default(SeIdentifierType.Personnummer);

   /// <summary>
   ///   Gets the raw personnummer value.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="SePersonnummer"/> to a <see cref="String"/>,
   ///   returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="SePersonnummer"/> to convert.
   /// </param>
   public static implicit operator String(SePersonnummer source)
      => source?.Value ?? String.Empty;     // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="SePersonnummer"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Swedish Personal Identity Number (Personnummer).
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid NHS number.
   /// </exception>
   public static explicit operator SePersonnummer(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="SePersonnummer"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Swedish Personal Identity Number (Personnummer).
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{SePersonnummer, ValidationError}"/>. Will
   ///   contain the new <see cref="SePersonnummer"/> if <paramref name="value"/>
   ///   is valid or a <see cref="ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<SePersonnummer, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new SePersonnummer(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         InvalidDateOfBirth invalidDateOfBirth => (ValidationError)invalidDateOfBirth,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Returns a string representation of the value in a long 13 character
   ///   format, combining the date of birth in YYYYMMDD format, a separator
   ///   character, the three digit birth serial number and the check digit.
   /// </summary>
   /// <param name="timeProvider">
   ///   Optional. <see cref="TimeProvider"/> instance used to determine the
   ///   exact age of the person. Persons 100 years or older will have a plus
   ///   ('+') as a separator; otherwise a dash ('-') is used as the separator.
   ///   If the <paramref name="timeProvider"/> is <see langword="null"/> then
   ///   the separator character will default to a dash ('-').
   /// </param>
   /// <returns>
   ///   The personummer formatted as a 13 character string.
   /// </returns>
   public String ToLongFormatValue(TimeProvider? timeProvider = null)
   {
      var separator = timeProvider is not null
         ? GetCorrectSeparatorForAgeOfPerson(timeProvider)
         : Chars.Dash;

      return Value[..8] + separator + Value[^4..];
   }

   /// <summary>
   ///   Returns a string representation of the value in a short 11 character format,
   ///   combining the date of birth in YYMMDD format, a separator character, the
   ///   three digit birth serial number and the check digit.
   /// </summary>
   /// <param name="timeProvider">
   ///   Optional. <see cref="TimeProvider"/> instance used to determine the
   ///   exact age of the person. Persons 100 years or older will have a plus
   ///   ('+') as a separator; otherwise a dash ('-') is used as the separator.
   ///   If the <paramref name="timeProvider"/> is <see langword="null"/> then
   ///   the separator character will default to a dash ('-').
   /// </param>
   /// <returns>
   ///   The personummer formatted as an 11 character string.
   /// </returns>
   public String ToShortFormatValue(TimeProvider? timeProvider = null)
   {
      var separator = timeProvider is not null
            ? GetCorrectSeparatorForAgeOfPerson(timeProvider)
            : Chars.Dash;

      return Value[2..8] + separator + Value[^4..];
   }

   /// <summary>
   ///   Get a string representation of the personnummer.
   /// </summary>
   /// <returns>
   ///   The personnummer formatted as a 13 character string.
   /// </returns>
   /// <remarks>
   ///   See <see cref="ToLongFormatValue"/> for additional details.
   /// </remarks>
   public override String ToString() => ToLongFormatValue();

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Swedish Personal Identity Number (Personnummer) value.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Swedish Personal Identity Number (Personnummer).
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

      if (value.Length is not ShortFormatLength and not LongFormatLength)
      {
         return new InvalidLength(
            Messages.SePersonnummerInvalidLength,
            value.Length,
            GetValidLengthDefinitions());
      }

      // After performing basic checks, validate the check digit because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      Int32 invalidCharacterPosition;
      if (!ValidateCheckDigit(value))
      {
         invalidCharacterPosition = LocateInvalidCharacter(value);
         return invalidCharacterPosition == -1
            ? new InvalidChecksum(Messages.SePersonnummerInvalidCheckDigit, Algorithms.Luhn.AlgorithmName)
            : new InvalidCharacter(
               Messages.SePersonnummerInvalidCharacter,
               value[invalidCharacterPosition],
               invalidCharacterPosition);
      }

      if (value.Length == LongFormatLength)
      {
         // Check digit does not consider leading two digits for 13 character
         // strings. So validate here.
         invalidCharacterPosition = !value[0].IsAsciiDigit()
            ? 0
            : !value[1].IsAsciiDigit() ? 1 : -1;
         if (invalidCharacterPosition != -1)
         {
            return new InvalidCharacter(
               Messages.SePersonnummerInvalidCharacter,
               value[invalidCharacterPosition],
               invalidCharacterPosition);
         }
      }

      if (GetSeparator(value) is not Chars.Dash and not Chars.Plus)
      {
         return new InvalidSeparator(
            Messages.SePersonnummerInvalidSeparator,
            value[^SeparatorOffset],
            value.Length - SeparatorOffset);
      }

      if (!ValidateDateOfBirth(value))
      {
         return new InvalidDateOfBirth(
            Messages.SePersonnummerInvalidDateOfBirth,
            value[..^SeparatorOffset],
            value.Length == ShortFormatLength ? DateFormatName.YYMMDD : DateFormatName.YYYYMMDD);
      }

      return default(ValidValue);
   }

   /// <summary>
   ///   Gets an array of details about valid lengths accepted for a
   ///   personnummer.
   /// </summary>
   /// <returns>
   ///   An array of <see cref="ValidLengthDefinition"/>s.
   /// </returns>
   internal static ValidLengthDefinition[] GetValidLengthDefinitions()
      =>
      [
         new ValidLengthDefinition(ShortFormatLength, Messages.SePersonnummerShortFormatLength),
         new ValidLengthDefinition(LongFormatLength, Messages.SePersonnummerLongFormatLength),
      ];

   private Char GetCorrectSeparatorForAgeOfPerson(TimeProvider timeProvider)
   {
      DateOnly dateOfBirth = DateOfBirth;
      DateTime today = timeProvider.GetLocalNow().Date;
      var age = today.Year - dateOfBirth.Year;
      if (today.Month < dateOfBirth.Month ||
         (today.Month == dateOfBirth.Month && today.Day < dateOfBirth.Day))
      {
         age--;
      }

      return age >= 100 ? Chars.Plus : Chars.Dash;
   }

   /// <summary>
   ///   Given a validated personnummer, get the internal representation which
   ///   converts six digit date of birth to eight digits and strips out the
   ///   separator character.
   /// </summary>
   private static String GetInternalRepresentation(ReadOnlySpan<Char> value)
   {
      var buffer = ArrayPool<Char>.Shared.Rent(InternalRepresentationLength);
      try
      {
#pragma warning disable IDE0008 // Use explicit type
         var (year, month, day) = GetYearMonthDay(value, applySamordningsnummerOffset: false);
#pragma warning restore IDE0008 // Use explicit type

         var span = new Span<Char>(buffer);

         Span<Char> work = span[..4];
         _ = year.TryFormat(work, out _, format: "D4", provider: CultureInfo.InvariantCulture);
         work = span[4..6];
         _ = month.TryFormat(work, out _, format: "D2", provider: CultureInfo.InvariantCulture);
         work = span[6..8];
         _ = day.TryFormat(work, out _, format: "D2", provider: CultureInfo.InvariantCulture);

         work = span[8..12];
         ReadOnlySpan<Char> remainder = value[^4..];
         remainder.CopyTo(work);

         return span[..InternalRepresentationLength].ToString();
      }
      finally
      {
         ArrayPool<Char>.Shared.Return(buffer);
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Char GetSeparator(ReadOnlySpan<Char> value)
      => value[^SeparatorOffset];

   private static (Int32 Year, Int32 Month, Int32 Day) GetYearMonthDay(
      ReadOnlySpan<Char> value,
      Boolean applySamordningsnummerOffset = true)
   {
      Int32 year;
      Int32 month;
      Int32 day;
      if (value.Length == ShortFormatLength)
      {
         // Refer to class XML comments for details of 2 digit year calculations.
         CenturyCutoff centuryCutoff = CenturyCutoff.DefaultInstance;
         year = value.ParseTwoDigits();
         year = centuryCutoff.ToFourDigitYear(year);
         if (GetSeparator(value) == Chars.Plus)
         {
            year -= 100;
         }

         month = value[2..].ParseTwoDigits();
         day = value[4..].ParseTwoDigits();
      }
      else
      {
         // This works for both 13 character values with separator and 12 character
         // all digit internal representation.
         var century = value.ParseTwoDigits() * 100;
         year = century + value[2..].ParseTwoDigits();
         month = value[4..].ParseTwoDigits();
         day = value[6..].ParseTwoDigits();
      }

      // Handle samordningsnummer, which adds 60 to the date of birth.
      if (applySamordningsnummerOffset && day > SamordningsnummerDayOffset)
      {
         day -= SamordningsnummerDayOffset;
      }

      return (year, month, day);
   }

   /// <summary>
   ///   If <see cref="ValidateCheckDigit(String)"/> returns false, determine
   ///   if the reason was an invalid character or an invalid check digit.
   /// </summary>
   /// <returns>
   ///   The zero-based index of the first non-digit character or -1 if no
   ///   non-digit characters found.
   /// </returns>
   private static Int32 LocateInvalidCharacter(ReadOnlySpan<Char> value)
   {
      var processLength = value.Length;
      var separatorIndex = processLength - SeparatorOffset;          // SeparatorOffset measures from end of value because date of birth has variable length

      for (var index = 0; index < processLength; index++)
      {
         if (index == separatorIndex)
         {
            continue;
         }

         if (!value[index].IsAsciiDigit())
         {
            return index;
         }
      }

      return -1;
   }

   private static Boolean ValidateCheckDigit(String value)
   {
      ICheckDigitMask checkDigitMask = value.Length == ShortFormatLength
         ? SePersonNumberShortFormatCheckDigitMask.Instance
         : SePersonNumberLongFormatCheckDigitMask.Instance;
      return CheckDigitAlgorithms.Luhn.Validate(value, checkDigitMask);
   }

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> value)
   {
      // Manual validation is faster than using DateTime.TryParseExact.
#pragma warning disable IDE0008 // Use explicit type
      var (year, month, day) = GetYearMonthDay(value);
#pragma warning restore IDE0008 // Use explicit type

      if (year is < MinimumValidYearOfBirth or > MaximumValidYearOfBirth)
      {
         return false;
      }
      else if (month is < 1 or > 12)
      {
         return false;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class SePersonnummerJsonConverter : JsonConverter<SePersonnummer>
{
   public override SePersonnummer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new SePersonnummer(str);
   }

   public override void Write(Utf8JsonWriter writer, SePersonnummer value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.ToLongFormatValue());
}

internal class SePersonNumberShortFormatCheckDigitMask : ICheckDigitMask
{
   private static readonly Lazy<SePersonNumberShortFormatCheckDigitMask> _instance =
      new(() => new SePersonNumberShortFormatCheckDigitMask());

   public static SePersonNumberShortFormatCheckDigitMask Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index) => index == 6;

   public Boolean IncludeCharacter(Int32 index) => index != 6;
}

internal class SePersonNumberLongFormatCheckDigitMask : ICheckDigitMask
{
   private static readonly Lazy<SePersonNumberLongFormatCheckDigitMask> _instance =
      new(() => new SePersonNumberLongFormatCheckDigitMask());

   public static SePersonNumberLongFormatCheckDigitMask Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index)
      => index is 0 or 1 or 8;

   public Boolean IncludeCharacter(Int32 index)
      => index is not 0 and not 1 and not 8;
}
