#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Strongly typed business object that represents an Italian tax identifier
///   assigned to individuals by the Italian tax office, the Agenzia delle
///   Entrate.
/// </summary>
/// <remarks>
///   <para>
///      A Codice Fiscale is a 16-character value structured as
///      SSSGGGYYMDDLLLLC, with the following elements:
///      <list type="bullet">
///         <item>
///            <term>SSS</term>
///            <description>
///               Three characters from the person's surname.
///            </description>
///         </item>
///         <item>
///            <term>GGG</term>
///            <description>
///               Three characters from the person's given name.
///            </description>
///         </item>
///         <item>
///            <term>YY</term>
///            <description>
///               Two-digit year of birth.
///            </description>
///         </item>
///         <item>
///            <term>M</term>
///            <description>
///               Month of birth encoded as a single alphabetic character.
///            </description>
///         </item>
///         <item>
///            <term>DD</term>
///            <description>
///               Two-digit day of birth. Also encodes the person's gender,
///               where 0-31 is used for males and 41-71 (day +40) is used for
///               females.
///            </description>
///         </item>
///         <item>
///            <term>LLLL</term>
///            <description>
///               Four-character Belfiore code that indicates the person's
///               comune (town) of birth. The code consists of one alphabetic
///               character followed by three digits. Persons who were born in a
///               foreign country will have a code starting with Z and a
///               three-digit code indicating the country of birth.
///            </description>
///         </item>
///         <item>
///            <term>C</term>
///            <description>
///               Modulus 26 check character.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      In cases where the personal details for two different individuals
///      result in the same tax code (known as "omocodia" or "same code"), one
///      or more digits of the value are replaced with letter equivalents until
///      a unique code is generated. See below.
///   </para>
///   <para>
///      When creating a new <see cref="ItCodiceFiscale"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The string must be 16 characters long.
///            </description>
///         </item>
///         <item>
///            <description>
///               All characters must be upper-case or lower-case letters
///               ('A'-'Z', 'a'-'z') or ASCII digits ('0'-'9')
///            </description>
///         </item>
///         <item>
///            <description>
///               The check character, position 15 (zero-based) must be a valid
///               upper-case or lower-case alphabetic ('A'-'Z', 'a'-'z') modulus
///               26 check character
///            </description>
///         </item>
///         <item>
///            <description>
///               The surename characters, positions 0-2 (zero-based), must be
///               upper-case or lower-case alphabetic characters ('A'-'Z',
///               'a'-'z')
///            </description>
///         </item>
///         <item>
///            <description>
///               The given name characters, positions 3-5 (zero-based), must be
///               upper-case or lower-case alphabetic characters ('A'-'Z',
///               'a'-'z')
///            </description>
///         </item>
///         <item>
///            <description>
///               The year of birth, character positions 6-7 (zero-based), must
///               be ASCII digits ('0'-'9') or the equivalent Omocodia letter
///               (see below)
///            </description>
///         </item>
///         <item>
///            <description>
///               The month of birth, character position 8 (zero-based) must be
///               an upper-case or lower-case alphabetic character. Valid month
///               characters are "ABCDEHLMPRST", where 'A' = January and 'T' =
///               December
///            </description>
///         </item>
///         <item>
///            <description>
///               The day of birth, character positions 9-10 (zero-based) must
///               be two ASCII digits ('0'-'9') the equivalent Omocodia letter
///               (see below). The integer value must be bettween 01-31 for
///               males and 61-91 for females. The integer value must also be
///               valid for the year/month
///            </description>
///         </item>
///         <item>
///            <description>
///               The comune of birth, character positions 11-14 (zero-based)
///               must be an upper-case or lower-case alphabetic character
///               ('A'-'Z', 'a'-'z') followed by three ASCII digits ('0'-'9')
///               the equivalent Omocodia letter (see below)
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>MRTMTT91D08F205J</term>
///            <description>
///               Matteo Moretti (male), born in Milan on 8 April 1991 (example
///               from wikipedia): surname characters = MRT, given name
///               characters = MTT, year of birth = 91, month of birth = D, day
///               of birth = 01, gender = male, comune of birth = F205, check
///               character = J
///            </description>
///         </item>
///         <item>
///            <term>MLLSNT82P65Z404U</term>
///            <description>
///               Samantha Miller (female), born in the USA on 25 September
///               1982, living in Italy (example from wikipedia): surname
///               characters = MLL, given name characters = SNT, year of birth =
///               82, month of birth = P, day of birth = 65 (actual day of
///               birth = 25), gender = female, comune of birth = Z404, check
///               character = U
///            </description>
///         </item>
///         <item>
///            <term>RSSNTN86H08G2NST</term>
///            <description>
///               example of omocodia with the two right-most digits replaced by
///               letter equivalents: surname characters = RSS, given name
///               characters = NTN, year of birth = 86, month of birth = H,
///               day of birth = 08, gender = male, comune of birth = G2NS
///               (actual comune of birth = G226), check character = T
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      <see cref="ItCodiceFiscale"/> is case-insensitive for validation and
///      parsing purposes. The ItCodiceFiscale constructor, Create method and
///      explicit string to ItCodiceFiscale operator will normalize any
///      lowercase letters to uppercase. Equality and inequality comparisons
///      between instances of ItCodiceFiscale will compare the normalized
///      uppercase versions of the value.
///   </para>
///   <para>
///      <see cref="ItCodiceFiscale"/> does not validate the comune of birth
///      (the Belfiore code) against a comprehensive list of valid values
///      (because the list of values exceeds 8000 entries). ItCodiceFiscale only
///      validates the format, one alphabetic character followed by three digit
///      characters.
///   </para>
///   <para>
///      While rare, it is possible that two different people can generate the
///      same tax code, a situation referred to as "omocodia" (or "same code").
///      The Agenzia delle Entrate handles omocodia by progressively replacing
///      digits with a letter equivalent, starting at the right-most digit and
///      progressing leftward until a unique code is generated. After a unique
///      code is generated, the check character is calculated for the unique
///      code. The letter substitutions are "LMNPQRSTUV", where 0 = L and 9 = V.
///      See<see href="https://codicefiscale.expert/en/omocodia">CodiceFiscale.expert</see>
///      for a full description of the handling of omocodia.
///   </para>
///   <para>
///      If a value contains an omocidia substitution, the substituted letters
///      are converted to the equivalent digits before validation or retrieving
///      the BirthYear, Gender or Belfiore code.
///   </para>
///   <para>
///      ItCodiceFiscale uses a custom modulus 26 check digit algorithm. Refer
///      to the English Wikipedia article below for a full description of the
///      algorithm. The custom algorithm has some weaknesses described in the
///      Italian Wikipedia article below. For example, the algorithm cannot two
///      character jump transpositions (i.e. DEF ⇒ FED) nor transpositions of
///      the characters WY (i.e. WY ⇒ YW).
///   </para>
///   <para>
///      See <see href="https://en.wikipedia.org/wiki/Italian_fiscal_code">Wikipedia - Italian fiscal code</see>
///      and <see href="https://it.wikipedia.org/wiki/Codice_fiscale">Wikipedia (Italian) - Codice fiscale</see>
///      for more information.
///   </para>
/// </remarks>
public record ItCodiceFiscale
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new Italian codice fiscale.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSurname,
      InvalidGivenName,
      InvalidYear,
      InvalidMonth,
      InvalidDay,
      InvalidLocationCode)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating Italian codice fiscale values.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSurname,
      InvalidGivenName,
      InvalidYear,
      InvalidMonth,
      InvalidDay,
      InvalidLocationCode)
   {
   }

   /// <summary>
   ///   The name of the check digit algorithm used by Italian codice fiscale
   ///   values for individuals.
   /// </summary>
   public const String CheckDigitAlgorithmName = "Modulus 26";

   /// <summary>
   ///   The length of a valid codice fiscale for an individual.
   /// </summary>
   public const Int32 IndividualLength = 16;

   private static readonly Int32[] _evenCharacterMap = [.. Enumerable.Range(Chars.DigitZero, Chars.UpperCaseZ - Chars.DigitZero + 1).Select(ch => MapEvenCharacter((Char)ch))];

   private static readonly Int32[] _oddCharacterMap = [.. Enumerable.Range(Chars.DigitZero, Chars.UpperCaseZ - Chars.DigitZero + 1).Select(ch => MapOddCharacter((Char)ch))];

   private static readonly SegmentRange _surnameRange = new(0, 3);
   private static readonly SegmentRange _givenNameRange = new(3, 6);
   private static readonly SegmentRange _yearRange = new(6, 8);
   private const Int32 MonthOffset = 8;
   private static readonly SegmentRange _dayRange = new(9, 11);
   private static readonly SegmentRange _comuneRange = new(11, 15);

   // Female gender is inticated by the day of birth incremented by +60.
   private const Int32 FemaleGenderDayOffset = 60;

   private const Int32 FemaleMinDay = 61;
   private const Int32 FemaleMaxDay = 91;
   private const Int32 MaleMinDay = 1;
   private const Int32 MaleMaxDay = 31;

   /// <summary>
   ///   Initializes a new instance of the <see cref="ItCodiceFiscale"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of an Italian codice fiscale.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 16.
   ///   - or -
   ///   <paramref name="value"/> contains a character other than upper-case or
   ///   lower-case letters ('A'-'Z', 'a'-'z') or ASCII digits ('0'-'9').
   ///   - or -
   ///   <paramref name="value"/> has an invalid modulus 26 check character in
   ///   the trailing (right-most) character position.
   ///   - or -
   ///   <paramref name="value"/> contains digit characters ('0'-'9') in the
   ///   surename positions (0-2, zero-based).
   ///   - or -
   ///   <paramref name="value"/> contains digit characters ('0'-'9') in the
   ///   given name positions (3-5, zero-based).
   ///   - or -
   ///   <paramref name="value"/> contains non-digit characters ('0'-'9') or
   ///   invalid omocodia substitution characters in the year of birth positions
   ///   (6-7, zero-based).
   ///   - or -
   ///   <paramref name="value"/> contains an invalid month of birth in
   ///   character position 8 (zero-based).
   ///   - or -
   ///   <paramref name="value"/> contains non-digit characters ('0'-'9') or
   ///   invalid omocodia substitution characters in the year of birth positions
   ///   (9-10, zero-based).
   ///   - or -
   ///   <paramref name="value"/> contains an invalid comune of birth in
   ///   positions (11-14, zero-based). A valid comune Belfiore code must be a
   ///   single upper-case or lower-case letter ('A'-'Z', 'a'-'z') followed by
   ///   three ASCII digit characters ('0'-'9') or their valid omocodia
   ///   substitution characters.
   /// </exception>
   public ItCodiceFiscale(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="ItCodiceFiscale"/> class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private ItCodiceFiscale(String? value, ValidationMode validationMode)
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
               InvalidSurname invalidSurname => new UKfValidationException<ValidationError>(invalidSurname),
               InvalidGivenName invalidGivenName => new UKfValidationException<ValidationError>(invalidGivenName),
               InvalidYear invalidYear => new UKfValidationException<ValidationError>(invalidYear),
               InvalidMonth invalidMonth => new UKfValidationException<ValidationError>(invalidMonth),
               InvalidDay invalidDay => new UKfValidationException<ValidationError>(invalidDay),
               InvalidLocationCode invalidComune => new UKfValidationException<ValidationError>(invalidComune),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = value!.ToUpperInvariant();
   }

   /// <summary>
   ///   Gets the 4-character Belfiore code that identifies the person's comune
   ///   of birth.
   /// </summary>
   public String BelfioreCode => _comuneRange.Extract(Value).ToString();

   /// <summary>
   ///   Gets the person's gender, as indicated by the day of birth.
   /// </summary>
   /// <remarks>
   ///   Day of birth 1-31 = male, 61-91 = female.
   /// </remarks>
   public Gender.BinaryGender Gender
      => GetTwoDigitInteger(_dayRange.Extract(Value)) switch
      {
         >= MaleMinDay and <= MaleMaxDay => default(Gender.Male),
         >= FemaleMinDay and <= FemaleMaxDay => default(Gender.Female),
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Gets a string representation of the codice fiscale.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid Italian codice fiscale.
   /// </summary>
   /// <param name="value">
   ///   String representation of an Italian codice fiscale.
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

      if (value.Length != IndividualLength)
      {
         return GetInvalidLengthResult(value);
      }

      // After performing basic checks, validate the check character because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      ValidationResult validationResult = ValidateCheckCharacter(value);
      if (validationResult is not ValidValue)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }

      if (!_surnameRange.ValidateAllAsciiLetters(value, out _))
      {
         return GetInvalidSurnameResult(value);
      }

      if (!_givenNameRange.ValidateAllAsciiLetters(value, out _))
      {
         return GetInvalidGivenNameResult(value);
      }

      validationResult = ValidateDateOfBirth(value);
      if (validationResult is not ValidValue)
      {
         // Could be either InvalidYear, InvalidMonth or InvalidDay.
         return validationResult;
      }

      if (!ValidateComune(value))
      {
         return GetInvalidLocationCodeResult(value);
      }

      return default(ValidValue);
   }

   /// <summary>
   ///   Map a character located at an even index (one-based) to its integer
   ///   equivalent for the purposes of calculating the check digit.
   /// </summary>
   /// <param name="ch">
   ///   The character to map.
   /// </param>
   /// <returns>
   ///   The character's integer equivalent for calculating the check digit.
   /// </returns>
   internal static Int32 MapEvenCharacter(Char ch)
      => ch switch
      {
         var d when d is >= Chars.DigitZero and <= Chars.DigitNine => d - Chars.DigitZero,
         var c when c is >= Chars.UpperCaseA and <= Chars.UpperCaseZ => c - Chars.UpperCaseA,
         _ => -1,
      };

   /// <summary>
   ///   Map a character located at an odd index (one-based) to its integer
   ///   equivalent for the purposes of calculating the check digit.
   /// </summary>
   /// <param name="ch">
   ///   The character to map.
   /// </param>
   /// <returns>
   ///   The character's integer equivalent for calculating the check digit.
   /// </returns>
   internal static Int32 MapOddCharacter(Char ch)
      => ch switch
      {
         // Map from https://en.wikipedia.org/wiki/Italian_fiscal_code
         '0' => 1,
         '1' => 0,
         '2' => 5,
         '3' => 7,
         '4' => 9,
         '5' => 13,
         '6' => 15,
         '7' => 17,
         '8' => 19,
         '9' => 21,
         'A' => 1,
         'B' => 0,
         'C' => 5,
         'D' => 7,
         'E' => 9,
         'F' => 13,
         'G' => 15,
         'H' => 17,
         'I' => 19,
         'J' => 21,
         'K' => 2,
         'L' => 4,
         'M' => 18,
         'N' => 20,
         'O' => 11,
         'P' => 3,
         'Q' => 6,
         'R' => 8,
         'S' => 12,
         'T' => 14,
         'U' => 16,
         'V' => 10,
         'W' => 22,
         'X' => 25,
         'Y' => 24,
         'Z' => 23,
         _ => -1,
      };

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.ItCodiceFiscaleInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.ItCodiceFiscaleInvalidCheckCharacter, CheckDigitAlgorithmName);

   private static InvalidDay GetInvalidDayResult(ReadOnlySpan<Char> value)
      => new(Messages.ItCodiceFiscaleInvalidDay, _dayRange.Extract(value).ToString());

   private static InvalidGivenName GetInvalidGivenNameResult(ReadOnlySpan<Char> value)
      => new(Messages.ItCodiceFiscaleInvalidGivenName, _givenNameRange.Extract(value).ToString());

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.ItCodiceFiscaleInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(IndividualLength, Messages.ItCodiceFiscaleLength),
         ]);

   private static InvalidLocationCode GetInvalidLocationCodeResult(ReadOnlySpan<Char> value)
      => new(Messages.ItCodiceFiscaleInvalidComune, _comuneRange.Extract(value).ToString());

   private static InvalidMonth GetInvalidMonthResult(ReadOnlySpan<Char> value)
      => new(Messages.ItCodiceFiscaleInvalidMonth, value[MonthOffset].ToString());

   private static InvalidSurname GetInvalidSurnameResult(ReadOnlySpan<Char> value)
      => new(Messages.ItCodiceFiscaleInvalidSurname, _surnameRange.Extract(value).ToString());

   private static InvalidYear GetInvalidYearResult(ReadOnlySpan<Char> value)
      => new(Messages.ItCodiceFiscaleInvalidYear, _yearRange.Extract(value).ToString());

   private static Int32 GetMonth(Char ch)
      => ch switch
      {
         >= Chars.UpperCaseA and <= Chars.UpperCaseE => ch - Chars.UpperCaseA + 1,
         Chars.UpperCaseH => 6,
         >= Chars.UpperCaseL and <= Chars.UpperCaseM => ch - Chars.UpperCaseL + 7,
         Chars.UpperCaseP => 9,
         >= Chars.UpperCaseR and <= Chars.UpperCaseT => ch - Chars.UpperCaseR + 10,
         >= Chars.LowerCaseA and <= Chars.LowerCaseE => ch - Chars.LowerCaseA + 1,
         Chars.LowerCaseH => 6,
         >= Chars.LowerCaseL and <= Chars.LowerCaseM => ch - Chars.LowerCaseL + 7,
         Chars.LowerCaseP => 9,
         >= Chars.LowerCaseR and <= Chars.LowerCaseT => ch - Chars.LowerCaseR + 10,
         _ => -1,
      };

   /// <summary>
   ///   Translate a character to its integer equivalent, taking into account
   ///   possible substitution for omocodia.
   /// </summary>
   /// <param name="ch">
   ///   The character to evaluate.
   /// </param>
   /// <returns>
   ///   An integer value between 0 and 9 or -1 if the character is not a valid
   ///   omocodia substitution.
   /// </returns>
   private static Int32 GetOmocodiaDigit(Char ch)
      => ch switch
      {
         >= Chars.DigitZero and <= Chars.DigitNine => ch.ToSingleDigit(),
         >= Chars.UpperCaseL and <= Chars.UpperCaseN => ch - Chars.UpperCaseL,
         >= Chars.UpperCaseP and <= Chars.UpperCaseV => ch - Chars.UpperCaseP + 3,
         >= Chars.LowerCaseL and <= Chars.LowerCaseN => ch - Chars.LowerCaseL,
         >= Chars.LowerCaseP and <= Chars.LowerCaseV => ch - Chars.LowerCaseP + 3,
         _ => -1,
      };

   private static Int32 GetTwoDigitInteger(ReadOnlySpan<Char> span)
   {
      var d1 = GetOmocodiaDigit(span[0]);
      var d2 = GetOmocodiaDigit(span[1]);

      return d1 > -1 && d2 > -1
         ? (d1 * 10) + d2
         : -1;
   }

   private static (Int32 Year, Int32 Month, Int32 Day) GetYearMonthDay(ReadOnlySpan<Char> value)
   {
      var year = GetTwoDigitInteger(_yearRange.Extract(value));
      var month = GetMonth(value[MonthOffset]);
      var day = GetTwoDigitInteger(_dayRange.Extract(value));

      return (year, month, day);
   }

   private static ValidationResult ValidateCheckCharacter(ReadOnlySpan<Char> value)
   {
      var processLength = value.Length - 1;
      var sum = 0;
      var isOdd = true;

      for (var index = 0; index < processLength; index++)
      {
         var ch = Char.ToUpperInvariant(value[index]);
         var num = ch is >= Chars.DigitZero and <= Chars.UpperCaseZ
            ? isOdd ? _oddCharacterMap[ch - Chars.DigitZero] : _evenCharacterMap[ch - Chars.DigitZero]
            : -1;
         if (num == -1)
         {
            return GetInvalidCharacterResult(value, index);
         }

         sum += num;
         isOdd = !isOdd;
      }

      var remainder = sum % 26;
      var checkCharacter = Char.ToUpperInvariant(value[^1]);
      var checkCharacterValue = checkCharacter is >= Chars.DigitZero and <= Chars.UpperCaseZ
         ? _evenCharacterMap[checkCharacter - Chars.DigitZero]
         : -1;
      if (checkCharacterValue == -1)
      {
         return GetInvalidCharacterResult(value, value.Length - 1);
      }

      return checkCharacterValue == remainder
         ? default(ValidValue)
         : GetInvalidChecksumResult();
   }

   // Only validate format of the comune (town of birth) component
   private static Boolean ValidateComune(ReadOnlySpan<Char> value)
   {
      ReadOnlySpan<Char> span = _comuneRange.Extract(value);

      return span[0].IsAsciiLetter()
             && GetOmocodiaDigit(span[1]) != -1
             && GetOmocodiaDigit(span[2]) != -1
             && GetOmocodiaDigit(span[3]) != -1;
   }

   private static ValidationResult ValidateDateOfBirth(ReadOnlySpan<Char> value)
   {
#pragma warning disable IDE0008 // Use explicit type
      var (year, month, day) = GetYearMonthDay(value);
#pragma warning restore IDE0008 // Use explicit type

      if (year == -1)
      {
         return GetInvalidYearResult(value);
      }

      if (month == -1)
      {
         return GetInvalidMonthResult(value);
      }

      // Basic check for day in valid range.
      if (day is not ((>= MaleMinDay and <= MaleMaxDay) or (>= FemaleMinDay and <= FemaleMaxDay)))
      {
         return GetInvalidDayResult(value);
      }

      // Also check that day is valid for the month.
      year += 2000;
      var daysInMonth = DateTime.DaysInMonth(year, month);
      if (day > 31)
      {
         day -= FemaleGenderDayOffset;
      }

      if (day > daysInMonth)
      {
         return GetInvalidDayResult(value);
      }

      return default(ValidValue);
   }
}
