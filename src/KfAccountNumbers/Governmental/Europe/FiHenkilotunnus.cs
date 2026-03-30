// Ignore Spelling: Fi Henkilotunnus Json

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a Finnish Personal Identity
///   Code (henkilötunnus).
/// </summary>
/// <remarks>
///   <para>
///      A Finnish henkilötunnus is a ten-digit number structured as DDMMYYCZZZQ,
///      with the following elements:
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
///               Three digit individual number used to differentiate between two
///               persons born on the same date. The individual number also encodes
///               additional information. The person's gender is indicated with
///               even numbers for females and odd numbers for males. Individual
///               numbers between 002 and 899 indicate persons born in Finland or
///               permanent residents and numbers between 900 and 999 are reserved
///               for temporary identifiers. The individual number 001 is not
///               valid.
///            </description>
///         </item>
///         <item>
///            <term>Q</term>
///            <description>
///               Check character, calculated as modulus 31 of the date of birth
///               and individual number. Will be one of 31 alphanumeric characters,
///               "0123456789ABCDEFHJKLMNPRSTUVWXY" (the letters `G, I, O, Q and Z`
///               are excluded to avoid possible confusion with digit characters).
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
///               Date of birth May 23, 1926, gender = female, permanent resident
///            </description>
///         </item>
///         <item>
///            <term>160117A275C</term>
///            <description>
///               Date of birth January 16, 2017, gender = male, permanent resident
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
///               The century indicator must be +, -, U, V, W, X, Y, A, B, C, D, E or F.
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
///               The individual number must be &ge; 002.
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
///      See https://en.wikipedia.org/wiki/National_identification_number#Finland
///      for more info. Also see https://kenda.fi/tools/hetu/ for tools to generate
///      test henkilötunnus values.
///   </para>
/// </remarks>
[JsonConverter(typeof(FiHenkilotunnusJsonConverter))]
public record FiHenkilotunnus
{
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

   /// <summary>
   ///   Initialize a new instance of the <see cref="FiHenkilotunnus"/> class.
   /// </summary>
   /// <param name="henkilotunnus">
   ///   String representation of a Finnish henkilötunnus.
   /// </param>
   /// <exception cref="KfValidationException{FiHenkilotunnusValidationResult}">
   ///   <paramref name="henkilotunnus"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="henkilotunnus"/> is not length 11.
   ///   - or -
   ///   <paramref name="henkilotunnus"/> contains a non-digit character in
   ///   the date of birth and/or individual number element.
   ///   - or -
   ///   <paramref name="henkilotunnus"/> has an invalid modulus 31 check character
   ///   in the trailing (right-most) position.
   ///   - or -
   ///   <paramref name="henkilotunnus"/> has an invalid century indicator in
   ///   position 6 (zero-based). Valid century indicators are + (1800s),
   ///   -, U, V, W, X or Y (1900s) and A, B, C, D, E, F (2000s).
   ///   - or -
   ///   <paramref name="henkilotunnus"/> has an invalid individual number in
   ///   character positions 7-9 (zero-based). Valid individual numbers are
   ///   &ge; 002.
   ///   - or -
   ///   <paramref name="henkilotunnus"/> contains an invalid date of birth in
   ///   positions 0-5 (zero-based).
   /// </exception>
   public FiHenkilotunnus(String? henkilotunnus)
      : this(henkilotunnus, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has already
   ///   been validated.
   /// </summary>
   private FiHenkilotunnus(String? henkilotunnus, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         FiHenkilotunnusValidationResult validationResult = Validate(henkilotunnus);
         if (validationResult != FiHenkilotunnusValidationResult.ValidationPassed)
         {
            throw validationResult.ToValidationException();
         }
      }

      // Validation passed, just assign to Value property.
      Value = henkilotunnus!;
   }

   /// <summary>
   ///   The person's date of birth, derived from the first six digits in DDMMYY
   ///   format and the exact century of birth derived from the century indicator.
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
   ///   The person's gender, as indicated by the individual number element
   ///   (character positions 7-9, zero-based). Odd numbers = Male; even
   ///   numbers = Female.
   /// </summary>
   public BinaryGender Gender => Value[GenderOffset] % 2 == 0       // This works because the ASCII character values for digits have the same odd/even pattern
      ? BinaryGender.Female
      : BinaryGender.Male;

   /// <summary>
   ///   The type of henkilötunnus identifier represented by this instance,
   ///   indicating if this is identifier assigned to a permanent resident or
   ///   a temporary identifier.
   /// </summary>
   /// <remarks>
   ///   The individual number element (characters 7-9, zero-based) determine
   ///   the identifier type. 002-899 are issued to permanent residents and
   ///   900-999 are used for temporary identifiers.
   /// </remarks>
   public FiIdentifierType IdentifierType => Value[IndividualNumberStartOffset] == Chars.DigitNine
      ? FiIdentifierType.Temporary
      : FiIdentifierType.PermanentResident;

   /// <summary>
   ///   The validated henkilötunnus.
   /// </summary>
   public String Value { get; private init; }

   public static implicit operator String(FiHenkilotunnus henkilotunnus)
      => henkilotunnus?.Value ?? String.Empty;      // Handle null henkilotunnus object gracefully by returning empty string

   // Explicit conversion from String to avoid unintentional conversions that may throw exceptions.
   public static explicit operator FiHenkilotunnus(String? henkilotunnus) => new(henkilotunnus);

   /// <summary>
   ///   Create a new <see cref="FiHenkilotunnus"/> using the Result pattern.
   /// </summary>
   /// <param name="henkilotunnus">
   ///   String representation of a Finnish henkilötunnus.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{FiHenkilotunnus, FiHenkilotunnusValidationResult}"/>.
   ///   Will contain the new <see cref="FiHenkilotunnus"/> if 
   ///   <paramref name="henkilotunnus"/> is valid or an
   ///   <see cref="FiHenkilotunnusValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="henkilotunnus"/> is 
   ///   invalid.
   /// </returns>
   public static CreateResult<FiHenkilotunnus, FiHenkilotunnusValidationResult> Create(String? henkilotunnus)
   {
      FiHenkilotunnusValidationResult validationResult = Validate(henkilotunnus);
      return validationResult == FiHenkilotunnusValidationResult.ValidationPassed
         ? new FiHenkilotunnus(henkilotunnus, validationMode: ValidationMode.BypassValidation)
         : validationResult;
   }

   /// <summary>
   ///   Get a string representation of the henkilötunnus.
   /// </summary>
   /// <remarks>
   ///   Will return the validated henkilötunnus, the same as the
   ///   <see cref="Value"/> property.
   /// </remarks>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="henkilotunnus"/> to determine if it contains a
   ///   valid Finnish henkilötunnus.
   /// </summary>
   /// <param name="henkilotunnus">
   ///   String representation of a Finnish henkilötunnus.
   /// </param>
   /// <returns>
   ///   A <see cref="FiHenkilotunnusValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="henkilotunnus"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static FiHenkilotunnusValidationResult Validate(String? henkilotunnus)
   {
      if (String.IsNullOrWhiteSpace(henkilotunnus))
      {
         return FiHenkilotunnusValidationResult.Empty;
      }
      else if (henkilotunnus.Length != ValidLength)
      {
         return FiHenkilotunnusValidationResult.InvalidLength;
      }

      // After performing basic checks, validate the check digit because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      FiHenkilotunnusValidationResult validationResult = ValidateCheckDigit(henkilotunnus);
      if (validationResult != FiHenkilotunnusValidationResult.ValidationPassed)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }
      else if (!ValidateCenturyIndicator(henkilotunnus))
      {
         return FiHenkilotunnusValidationResult.InvalidCenturyIndicator;
      }
      else if (!ValidateIndividualNumber(henkilotunnus))
      {
         return FiHenkilotunnusValidationResult.InvalidIndividualNumber;
      }
      else if (!ValidateDateOfBirth(henkilotunnus))
      {
         return FiHenkilotunnusValidationResult.InvalidDateOfBirth;
      }

      return FiHenkilotunnusValidationResult.ValidationPassed;
   }

   private static (Int32 day, Int32 month, Int32 year) GetDayMonthYear(ReadOnlySpan<Char> henkilotunnus)
   {
      var day = henkilotunnus.ParseTwoDigits();
      var month = henkilotunnus[2..].ParseTwoDigits();
      var year = henkilotunnus[4..].ParseTwoDigits();

      // Adjust the year according to the value of the century indicator.
      var centuryIndicator = henkilotunnus[CenturyIndicatorOffset];

      year += centuryIndicator switch
      {
         Chars.Plus => 1800,
         Chars.Dash => 1900,
         Chars.UpperCaseA => 2000,
         Chars.UpperCaseB => 2000,
         Chars.UpperCaseC => 2000,
         Chars.UpperCaseD => 2000,
         Chars.UpperCaseE => 2000,
         Chars.UpperCaseF => 2000,
         Chars.UpperCaseU => 1900,
         Chars.UpperCaseV => 1900,
         Chars.UpperCaseW => 1900,
         Chars.UpperCaseX => 1900,
         Chars.UpperCaseY => 1900,
         _ => 0
      };

      return (day, month, year);
   }

   private static FiHenkilotunnusValidationResult ValidateCheckDigit(ReadOnlySpan<Char> henkilotunnus)
   {
      const Int32 processLength = 10;     // Exclude check digit from main process loop.

      var sum = 0;

      // Convert date of birth and individual number to an integer value.
      for (var index = 0; index < processLength; index++)
      {
         if (index == CenturyIndicatorOffset)
         {
            continue;
         }

         sum *= 10;
         var num = henkilotunnus[index] - Chars.DigitZero;
         if (num < 0 || num > 9)
         {
            return FiHenkilotunnusValidationResult.InvalidCharacter;
         }

         sum += num;
      }

      var checkDigit = sum % 31;
      var checkCharacter = CheckCharacters[checkDigit];

      return henkilotunnus[CheckCharacterOffset] == checkCharacter
         ? FiHenkilotunnusValidationResult.ValidationPassed
         : FiHenkilotunnusValidationResult.InvalidCheckDigit;
   }

   private static Boolean ValidateCenturyIndicator(ReadOnlySpan<Char> henkilotunnus)
      => henkilotunnus[CenturyIndicatorOffset] switch
      {
         Chars.Plus => true,
         Chars.Dash => true,
         Chars.UpperCaseA => true,
         Chars.UpperCaseB => true,
         Chars.UpperCaseC => true,
         Chars.UpperCaseD => true,
         Chars.UpperCaseE => true,
         Chars.UpperCaseF => true,
         Chars.UpperCaseU => true,
         Chars.UpperCaseV => true,
         Chars.UpperCaseW => true,
         Chars.UpperCaseX => true,
         Chars.UpperCaseY => true,
         _ => false
      };

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> personnummer)
   {
#pragma warning disable IDE0008 // Use explicit type
      var (day, month, year) = GetDayMonthYear(personnummer);
#pragma warning restore IDE0008 // Use explicit type

      if (year < MinimumValidYearOfBirth || year > MaximumValidYearOfBirth)
      {
         // Should be impossible to ever reach this point because century indicator
         // has been validated, but return false out of abundance of caution and
         // to avoid throwing an exception.
         return false;
      }

      if (month < 1 || month > 12)
      {
         return false;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }

   private static Boolean ValidateIndividualNumber(ReadOnlySpan<Char> henkilotunnus)
   {
      var d1 = henkilotunnus[IndividualNumberStartOffset] - Chars.DigitZero;
      var d2 = henkilotunnus[IndividualNumberStartOffset + 1] - Chars.DigitZero;
      var d3 = henkilotunnus[IndividualNumberStartOffset + 2] - Chars.DigitZero;

      var individualNumber = (d1 * 100) + (d2 * 10) + d3;

      return individualNumber >= 2;
   }
}

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
