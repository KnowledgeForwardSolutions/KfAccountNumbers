// Ignore Spelling: Fi Henkilotunnus Json

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a Finnish Personal Identity
///   Code (henkilötunnus).
/// </summary>
/// <remarks>
///   <para>
///      A Finnish henkilötunnus is an eleven-digit number structured as
///      DDMMYYCZZZQ, with the following elements:
///      <list type="bullet">
///         <item>
///            <term>DDMMYY</term>
///            <description>
///               The person's date of birth in DDMMYY format.
///            </description>
///         </item>
///         <item>
///            <term>C</term>
///            <description>
///               Century indicator, with + indicating 1800s, -, U, V, W, X or Y
///               indicating 1900s and A, B, C, D, E, F indicating 2000s.
///            </description>
///         </item>
///         <item>
///            <term>ZZZ</term>
///            <description>
///               Three digit individual number used to differentiate between
///               two persons born on the same date. The individual number also
///               encodes additional information. The person's gender is
///               indicated with even numbers for females and odd numbers for
///               males. Individual numbers between 002 and 899 indicate persons
///               born in Finland or permanent residents and numbers between 900
///               and 999 are reserved for temporary identifiers. Individual
///               numbers less than 002 are not valid.
///            </description>
///         </item>
///         <item>
///            <term>Q</term>
///            <description>
///               Check character, calculated as modulus 31 of the digits of the
///               date of birth and individual number. (The century indicator is
///               excluded from the calculation.) Will be one of 31 alphanumeric
///               characters, "0123456789ABCDEFHJKLMNPRSTUVWXY" (the letters
///               `G, I, O, Q and Z` are excluded to avoid possible confusion
///               with digit characters).
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>230526-034N</term>
///            <description>
///               Date of birth May 23, 1926, gender = female, permanent
///               resident
///            </description>
///         </item>
///         <item>
///            <term>160117A275C</term>
///            <description>
///               Date of birth January 16, 2017, gender = male, permanent
///               resident
///            </description>
///         </item>
///         <item>
///            <term>020508D929B</term>
///            <description>
///               Date of birth May 2, 2008, gender = male, temporary/test value
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      When creating a new <see cref="FiHenkilotunnus"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be 11 characters in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth and individual number elements (DDMMYY and
///               ZZZ elements) must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The century indicator must be +, -, U, V, W, X, Y, A, B, C, D,
///               E or F.
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth, after deriving the century from the century
///               indicator must be a valid date between January 1, 1800 and
///               December 31, 2099.
///            </description>
///         </item>
///         <item>
///            <description>
///               The individual number must be &gt;= 002.
///            </description>
///         </item>
///         <item>
///            <description>
///               The check character must be a valid modulus 31 check character
///               calculated from the date of birth and the individual number.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      <see cref="FiHenkilotunnus"/> is case-insensitive for validation and
///      parsing purposes. The FiHenkilotunnus constructor, Create method and
///      explicit string to FiHenkilotunnus operator will normalize any
///      lowercase letters to uppercase. Equality and inequality comparisons
///      between instances of FiHenkilotunnus will compare the normalized
///      uppercase versions of the value.
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/National_identification_number#Finland
///      for more info. Also see https://kenda.fi/tools/hetu/ for tools to generate
///      test henkilötunnus values.
///   </para>
/// </remarks>
[JsonConverter(typeof(FiHenkilotunnusJsonConverter))]
public record FiHenkilotunnus
{
   /// <summary>
   ///   Discriminated union defining the types of identifier that
   ///   <see cref="FiHenkilotunnus"/> can represent.
   /// </summary>
   public union IdentifierCategory(FiIdentifierType.PermanentResident, FiIdentifierType.Temporary) { }

   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="FiHenkilotunnus"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidCentury,
      InvalidFiHenkilotunnusIndividualNumber,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="FiHenkilotunnus"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidCentury,
      InvalidFiHenkilotunnusIndividualNumber,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   The name of the check digit algorithm used by henkilötunnus values.
   /// </summary>
   public const String CheckDigitAlgorithmName = "Modulus 31";

   /// <summary>
   ///   The latest year of birth supported by <see cref="FiHenkilotunnus"/>.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2099;

   /// <summary>
   ///   The earliest year of birth supported by <see cref="FiHenkilotunnus"/>.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1800;

   private const Int32 ValidLength = 11;
   private const Int32 CenturyIndicatorOffset = 6;
   private const Int32 IndividualNumberStartOffset = 7;
   private const Int32 GenderOffset = 9;
   private const Int32 CheckCharacterOffset = 10;

   private const String CheckCharacters = "0123456789ABCDEFHJKLMNPRSTUVWXY";

   private static readonly SegmentRange _individualNumber =
      new(IndividualNumberStartOffset, CheckCharacterOffset);

   /// <summary>
   ///   Initializes a new instance of the <see cref="FiHenkilotunnus"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Finnish henkilötunnus.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or
   ///   all whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 11.
   ///   - or -
   ///   <paramref name="value"/> contains a non-digit character in
   ///   the date of birth and/or individual number element.
   ///   - or -
   ///   <paramref name="value"/> has an invalid modulus 31 check
   ///   character in the trailing (right-most) position.
   ///   - or -
   ///   <paramref name="value"/> has an invalid century indicator in
   ///   position 6 (zero-based). Valid century indicators are + (1800s),
   ///   -, U, V, W, X or Y (1900s) and A, B, C, D, E, F (2000s).
   ///   - or -
   ///   <paramref name="value"/> has an invalid individual number in
   ///   character positions 7-9 (zero-based). Valid individual numbers are
   ///   &gt;= 002.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid date of birth in
   ///   positions 0-5 (zero-based).
   /// </exception>
   public FiHenkilotunnus(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="FiHenkilotunnus"/> class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private FiHenkilotunnus(String? value, ValidationMode validationMode)
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
               InvalidChecksum invalidCheckDigit => new UKfValidationException<ValidationError>(invalidCheckDigit),
               InvalidCentury invalidCentury => new UKfValidationException<ValidationError>(invalidCentury),
               InvalidFiHenkilotunnusIndividualNumber invalidIndividualNumber => new UKfValidationException<ValidationError>(invalidIndividualNumber),
               InvalidDateOfBirth invalidDateOfBirth => new UKfValidationException<ValidationError>(invalidDateOfBirth),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      // Validation passed, just assign to Value property, normalizing to
      // uppercase if necessary.
      Value = value!.ToUpperInvariant();
   }

   /// <summary>
   ///   Gets the person's date of birth, derived from the first six digits in
   ///   DDMMYY format and the exact century of birth derived from the century
   ///   indicator.
   /// </summary>
   public DateOnly DateOfBirth
   {
      get
      {
#pragma warning disable IDE0008 // Use explicit type
         var (day, month, year) = GetDayMonthYear(Value);
#pragma warning restore IDE0008 // Use explicit type

         return new DateOnly(year, month, day);
      }
   }

   /// <summary>
   ///   Gets the person's gender, as indicated by the individual number element
   ///   (character positions 7-9, zero-based). Even numbers = Female; odd
   ///   numbers = Male.
   /// </summary>
   public Gender.BinaryGender Gender
      => Value[GenderOffset] % 2 == 0 ? default(Gender.Female) : default(Gender.Male);    // This works because the ASCII character values for digits have the same odd/even pattern

   /// <summary>
   ///   Gets the type of henkilötunnus identifier represented by this instance,
   ///   indicating if this is identifier assigned to a permanent resident or
   ///   a temporary identifier.
   /// </summary>
   /// <remarks>
   ///   The individual number element (characters 7-9, zero-based) determine
   ///   the identifier type. 002-899 are issued to permanent residents and
   ///   900-999 are used for temporary identifiers.
   /// </remarks>
   public IdentifierCategory IdentifierType => Value[IndividualNumberStartOffset] == Chars.DigitNine
      ? default(FiIdentifierType.Temporary)
      : default(FiIdentifierType.PermanentResident);

   /// <summary>
   ///   Gets the validated henkilötunnus.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="FiHenkilotunnus"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="FiHenkilotunnus"/> to convert.
   /// </param>
   public static implicit operator String(FiHenkilotunnus source)
      => source?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a
   ///   <see cref="FiHenkilotunnus"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Finnish henkilötunnus.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid henkilötunnus.
   /// </exception>
   public static explicit operator FiHenkilotunnus(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="FiHenkilotunnus"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Finnish henkilötunnus.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{FiHenkilotunnus, ValidationError}"/>. Will
   ///   contain the new <see cref="FiHenkilotunnus"/> if <paramref name="value"/>
   ///   is valid or a <see cref="ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static CreateResult<FiHenkilotunnus, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new FiHenkilotunnus(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidCheckDigit => (ValidationError)invalidCheckDigit,
         InvalidCentury invalidCentury => (ValidationError)invalidCentury,
         InvalidFiHenkilotunnusIndividualNumber invalidIndividualNumber => (ValidationError)invalidIndividualNumber,
         InvalidDateOfBirth invalidDateOfBirth => (ValidationError)invalidDateOfBirth,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Get a string representation of the henkilötunnus.
   /// </summary>
   /// <returns>
   ///   The validated henkilötunnus.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains
   ///   a valid Finnish henkilötunnus.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Finnish henkilötunnus.
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

      if (value.Length != ValidLength)
      {
         return new InvalidLength(
            Messages.FiHenkilotunnusInvalidLength,
            value.Length,
            new ValidLengthDefinition(ValidLength, Messages.FiHenkilotunnusValidLength));
      }

      // After performing basic checks, validate the check digit because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      ValidationResult validationResult = ValidateCheckDigit(value);
      if (validationResult is not ValidValue)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }

      if (!ValidateCenturyIndicator(value))
      {
         return new InvalidCentury(
            Messages.FiHenkilotunnusInvalidCenturyIndicator,
            value[CenturyIndicatorOffset].ToString());
      }

      if (!ValidateIndividualNumber(value))
      {
         return new InvalidFiHenkilotunnusIndividualNumber(
            Messages.FiHenkilotunnusInvalidIndividualNumber,
            _individualNumber.Extract(value).ToString());
      }

      if (!ValidateDateOfBirth(value))
      {
         return new InvalidDateOfBirth(
            Messages.FiHenkilotunnusInvalidDateOfBirth,
            value[..CenturyIndicatorOffset],
            DateFormatName.DDMMYY);
      }

      return default(ValidValue);
   }

   private static (Int32 Day, Int32 Month, Int32 Year) GetDayMonthYear(ReadOnlySpan<Char> value)
   {
      var day = value.ParseTwoDigits();
      var month = value[2..].ParseTwoDigits();
      var year = value[4..].ParseTwoDigits();

      // Adjust the year according to the value of the century indicator.
      var centuryIndicator = value[CenturyIndicatorOffset];
      year += centuryIndicator switch
      {
         Chars.Plus => 1800,
         Chars.Dash => 1900,
         (>= Chars.UpperCaseA and <= Chars.UpperCaseF) or (>= Chars.LowerCaseA and <= Chars.LowerCaseF) => 2000,
         (>= Chars.UpperCaseU and <= Chars.UpperCaseY) or (>= Chars.LowerCaseU and <= Chars.LowerCaseY) => 1900,
         _ => 0,
      };

      return (day, month, year);
   }

   private static ValidationResult ValidateCheckDigit(ReadOnlySpan<Char> value)
   {
      const Int32 processLength = 10;     // Exclude check digit from main process loop.

      var sum = 0;

      for (var index = 0; index < processLength; index++)
      {
         // Century indicator is ignored for check digit calculation.
         if (index == CenturyIndicatorOffset)
         {
            continue;
         }

         sum *= 10;
         var num = value[index] - Chars.DigitZero;
         if (num is < 0 or > 9)
         {
            return new InvalidCharacter(
               Messages.FiHenkilotunnusInvalidCharacter,
               value[index],
               index);
         }

         sum += num;
      }

      var checkDigit = sum % 31;
      var checkCharacter = value[CheckCharacterOffset];
      if (Char.IsLower(checkCharacter))
      {
         checkCharacter = Char.ToUpper(checkCharacter, CultureInfo.InvariantCulture);
      }

      return checkCharacter == CheckCharacters[checkDigit]
         ? default(ValidValue)
         : new InvalidChecksum(
            Messages.FiHenkilotunnusInvalidCheckDigit,
            CheckDigitAlgorithmName);
   }

   private static Boolean ValidateCenturyIndicator(ReadOnlySpan<Char> value)
      => value[CenturyIndicatorOffset] switch
      {
         Chars.Plus or Chars.Dash => true,
         (>= Chars.UpperCaseA and <= Chars.UpperCaseF) or (>= Chars.LowerCaseA and <= Chars.LowerCaseF) => true,
         (>= Chars.UpperCaseU and <= Chars.UpperCaseY) or (>= Chars.LowerCaseU and <= Chars.LowerCaseY) => true,
         _ => false,
      };

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> value)
   {
#pragma warning disable IDE0008 // Use explicit type
      var (day, month, year) = GetDayMonthYear(value);
#pragma warning restore IDE0008 // Use explicit type

      if (year is < MinimumValidYearOfBirth or > MaximumValidYearOfBirth)
      {
         // Should be impossible to ever reach this point because century indicator
         // has been validated, but return false out of abundance of caution and
         // to avoid throwing an exception.
         return false;
      }

      if (month is < 1 or > 12)
      {
         return false;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }

   private static Boolean ValidateIndividualNumber(ReadOnlySpan<Char> value)
   {
      var individualNumber = value[IndividualNumberStartOffset..].ParseThreeDigits();

      return individualNumber >= 2;
   }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class FiHenkilotunnusJsonConverter : JsonConverter<FiHenkilotunnus>
{
   public override FiHenkilotunnus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new FiHenkilotunnus(str);
   }

   public override void Write(Utf8JsonWriter writer, FiHenkilotunnus value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
