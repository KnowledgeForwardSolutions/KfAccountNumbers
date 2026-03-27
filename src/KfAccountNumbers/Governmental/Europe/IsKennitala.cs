// Ignore Spelling: Fyrirtaeki Json Kennitala

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents an Icelandic national
///   identity number (kennitala). A kennitala may be issued to an individual
///   or to a company. The <see cref="IdentifierType"/> property returns an
///   <see cref="IsIdentifierType"/> value that indicates if the kennitala number
///   is assigned to an individual (Einstaklingur) or to a company (Fyrirtæki).
/// </summary>
/// <remarks>
///   <para>
///      A kennitala is a ten-digit number structured as DDMMYYRRPC, with the
///      following elements:
///      <list type="bullet">
///         <item>
///            <term>DDMMYY</term>
///            <description>
///               the person's date of birth (for individuals) or the date of
///               registration (for companies) in DDMMYY format. The only difference
///               between an Einstaklingur kennitala and a Fyrirtæki kennitala is
///               that 40 is added to the day component of the date of birth for
///               the Fyrirtæki kennitala (i.e. 130585 becomes 530585). Day values
///               in the range 41-71 (inclusive) indicate a Fyrirtæki kennitala.
///            </description>
///         </item>
///         <item>
///            <term>RR</term>
///            <description>
///               Two random digits used to differentiate between two persons born
///               on the same date.
///            </description>
///         </item>
///         <item>
///            <term>P</term>
///            <description>
///               A check digit calculated for the DDMMYYRR digits using a weighted
///               modulus 11 algorithm.
///            </description>
///         </item>
///         <item>
///            <term>C</term>
///            <description>
///               A single digit indicating the century of birth. Valid digits are
///               9 (1900s) and 0 (2000s).
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A kennitala may be formatted as a string of 10 consecutive digits (DDMMYYRRPC)
///      or as 11 characters with a separator character, generally a dash ('-'),
///      separating the date of birth and the remaining four digits (DDMMYY-RRPC).
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>1205854369</term>
///            <description>
///               Einstaklingur, date of birth May 12, 1985, check digit = 6
///            </description>
///         </item>
///         <item>
///            <term>120585-4369</term>
///            <description>
///               Einstaklingur, date of birth May 12, 1985, check digit = 6
///            </description>
///         </item>
///         <item>
///            <term>5311073810</term>
///            <description>
///               Fyrirtæki, date of registration November 13, 2007, check digit 1
///            </description>
///         </item>
///         <item>
///            <term>531107 3810</term>
///            <description>
///               Fyrirtæki, date of registration November 13, 2007, check digit 1
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      When creating a new <see cref="IsKennitala"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 10 characters (without separator) or 11
///               characters (with separator) in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               All characters (except the optional separator character) must be
///               ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The check digit must match the digit calculated using a weighted
///               modulus 11 algorithm.
///            </description>
///         </item>
///         <item>
///            <description>
///               The optional separator character, if included, may not be an ASCII
///               digit. Any non-digit character is allowed as a separator.
///            </description>
///         </item>
///         <item>
///            <description>
///               The century indicator must be the ASCII character nine ('9') or
///               the ASCII character zero ('0').
///            </description>
///         </item>
///         <item>
///            <description>
///               The date of birth, after deriving the century from the century
///               indicator (and if the value is a Fyrirtæki kennitala, after
///               subtracting the Fyrirtæki kennitala offset) must be a valid date
///               between January 1, 1900 and December 31, 2099.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/Icelandic_identification_number and
///      https://kennitala.com/ for more info.
///   </para>
/// </remarks>
[JsonConverter(typeof(IsKennitalaJsonConverter))]
public record IsKennitala
{
   /// <summary>
   ///   Represents the day offset used to distinguish personal (Einstaklingur)
   ///   kennitala values from company (Fyrirtaeki) kennitala values.
   /// </summary>
   /// <remarks>
   ///   For Icelandic kennitala numbers, a Fyrirtaeki kennitala is indicated by
   ///   adding 40 to the day component of the date of birth.
   /// </remarks>
   public const Int32 FyrirtaekiDayOffset = 40;

   /// <summary>
   ///   The latest year of birth supported by <see cref="IsKennitala"/>.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2099;

   /// <summary>
   ///   The earliest year of birth supported by <see cref="IsKennitala"/>.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1900;

   private const Int32 UnformattedLength = 10;
   private const Int32 FormattedLength = 11;

   private const Int32 SeparatorOffset = 6;

   // These offsets measure from the right side of the value.
   private const Int32 CenturyIndicatorOffset = 1;

   // Fyrirtæki adds 40 to the day portion of date of birth.
   private const Int32 FyrirtaekiMinimumDay = 41;
   private const Int32 FyrirtaekiMaximumDay = 71;

   private static readonly Int32[] _weights = [3, 2, 7, 6, 5, 4, 3, 2, 1];

   /// <summary>
   ///   Initialize a new instance of the <see cref="IsKennitala"/> class.
   /// </summary>
   /// <param name="kennitala">
   ///   String representation of a kennitala.
   /// </param>
   /// <exception cref="KfValidationException{IsKennitalaValidationResult}">
   ///   <paramref name="kennitala"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="kennitala"/> is not length 10 (or 11 if a separator
   ///   character is used).
   ///   - or -
   ///   <paramref name="kennitala"/> contains a non-digit character in
   ///   any position other than the separator location.
   ///   - or -
   ///   <paramref name="kennitala"/> contains an invalid weighted modulus
   ///   11 check digit in the trailing (right-most) character position.
   ///   - or -
   ///   <paramref name="kennitala"/> contains a digit character in position
   ///   6 (zero-based). Valid separator characters are any non-digit character,
   ///   though space (' ') and dash ('-') are the most common values.
   ///   - or -
   ///   <paramref name="kennitala"/> contains an invalid century indicator
   ///   in the trailing (right-most) position. Valid century indicators are
   ///   '9' (1900's) and '0' (2000's).
   ///   - or -
   ///   <paramref name="kennitala"/> contains an invalid date of birth in
   ///   positions 0-5 (zero-based).
   /// </exception>
   public IsKennitala(String? kennitala)
      : this(kennitala, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has already
   ///   been validated.
   /// </summary>
   private IsKennitala(String? kennitala, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         IsKennitalaValidationResult validationResult = Validate(kennitala);
         if (validationResult != IsKennitalaValidationResult.ValidationPassed)
         {
            throw validationResult.ToValidationException();
         }
      }

      Value = GetRawValue(kennitala!);
   }

   /// <summary>
   ///   The person's date of birth, derived from the first six digits in DDMMYY
   ///   format and the exact century of birth derived from the century indicator.
   /// </summary>
   /// <remarks>
   ///   Note that fyrirtaeki kennitala values add 40 to the leading two digits
   ///   (the DD portion of the DDMMYY date of birth). The date of birth property
   ///   automatically adjusts for this offset.
   /// </remarks>
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
   ///   The type of kennitala identifier represented by this instance,
   ///   indicating if this is an Einstaklingur or a Fyrirtaeki.
   /// </summary>
   /// <remarks>
   ///   The first two digits of the value determine the type of identifier.
   ///   Fyrirtaekis add 40 to the first two digits of the date of birth (DDMMYY
   ///   format) so any day of birth between 41 and 71 is considered a Fyrirtaeki.
   /// </remarks>
   public IsIdentifierType IdentifierType
      => Value.AsSpan().ParseTwoDigits() is >= FyrirtaekiMinimumDay and <= FyrirtaekiMaximumDay
         ? IsIdentifierType.Fyrirtaeki
         : IsIdentifierType.Einstaklingur;

   /// <summary>
   ///   The raw kennitala value.
   /// </summary>
   public String Value { get; private init; }

   public static implicit operator String(IsKennitala kennitala)
      => kennitala?.Value ?? String.Empty;      // Handle null kennitala object gracefully by returning empty string

   // Explicit conversion from String to avoid unintentional conversions that may throw exceptions.
   public static explicit operator IsKennitala(String? kennitala) => new(kennitala);

   /// <summary>
   ///   Create a new <see cref="IsKennitala"/> using the Result pattern.
   /// </summary>
   /// <param name="kennitala">
   ///   String representation of an Icelandic kennitala.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{IsKennitala, IsKennitalaValidationResult}"/>.
   ///   Will contain the new <see cref="IsKennitala"/> if 
   ///   <paramref name="kennitala"/> is valid or an
   ///   <see cref="IsKennitalaValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="kennitala"/> is 
   ///   invalid.
   /// </returns>
   public static CreateResult<IsKennitala, IsKennitalaValidationResult> Create(String? kennitala)
   {
      IsKennitalaValidationResult validationResult = Validate(kennitala);
      return validationResult == IsKennitalaValidationResult.ValidationPassed
         ? new IsKennitala(kennitala, validationMode: ValidationMode.BypassValidation)
         : validationResult;
   }

   /// <summary>
   ///   Format the kennitala using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then the default mask "______-____" will be used instead.
   /// </param>
   /// <returns>
   ///   A formatted kennitala.
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
   ///   details on creating a mask to format the kennitala.
   /// </remarks>
   public String Format(String mask = "______-____") => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the kennitala.
   /// </summary>
   /// <remarks>
   ///   Will return the raw kennitala, without a separator character.
   /// </remarks>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="kennitala"/> to determine if it contains a
   ///   valid Icelandic kennitala number.
   /// </summary>
   /// <param name="kennitala">
   ///   String representation of an Icelandic kennitala number.
   /// </param>
   /// <returns>
   ///   A <see cref="IsKennitalaValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="kennitala"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static IsKennitalaValidationResult Validate(String? kennitala)
   {
      if (String.IsNullOrWhiteSpace(kennitala))
      {
         return IsKennitalaValidationResult.Empty;
      }
      else if (kennitala.Length is not UnformattedLength and not FormattedLength)
      {
         return IsKennitalaValidationResult.InvalidLength;
      }

      // After performing basic checks, validate the check digits because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      IsKennitalaValidationResult validationResult = ValidateCheckDigit(kennitala);
      if (validationResult != IsKennitalaValidationResult.ValidationPassed)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }
      validationResult = ValidateCenturyIndicator(kennitala);
      if (validationResult != IsKennitalaValidationResult.ValidationPassed)
      {
         // Could be either InvalidCharacter or InvalidCentury.
         return validationResult;
      }
      else if (!ValidateSeparator(kennitala))
      {
         return IsKennitalaValidationResult.InvalidSeparator;
      }
      else if (!ValidateDateOfBirth(kennitala))
      {
         return IsKennitalaValidationResult.InvalidDateOfBirth;
      }

      return IsKennitalaValidationResult.ValidationPassed;
   }

   private static (Int32 day, Int32 month, Int32 year) GetDayMonthYear(ReadOnlySpan<Char> kennitala)
   {
      var day = kennitala.ParseTwoDigits();
      var month = kennitala[2..].ParseTwoDigits();
      var year = kennitala[4..].ParseTwoDigits();
      year += kennitala[^CenturyIndicatorOffset] == Chars.DigitNine ? 1900 : 2000;

      // Adjust day for possible Fyrirtaeki.
      if (day > FyrirtaekiDayOffset)
      {
         day -= FyrirtaekiDayOffset;
      }

      return (day, month, year);
   }

   private static String GetRawValue(String kennitala)
      => kennitala.Length == UnformattedLength
         ? kennitala
         : String.Concat(
            kennitala.AsSpan(0, SeparatorOffset),
            kennitala.AsSpan(SeparatorOffset + 1));

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> kennitala)
      => kennitala.Length == FormattedLength;

   private static IsKennitalaValidationResult ValidateCenturyIndicator(ReadOnlySpan<Char> kennitala)
   {
      var ch = kennitala[^CenturyIndicatorOffset];
      if (!ch.IsAsciiDigit())                         // Check for ASCII digit because check digit validation doesn't evaluate century indicator
      {
         return IsKennitalaValidationResult.InvalidCharacter;
      }

      return ch is Chars.DigitZero or Chars.DigitNine
         ? IsKennitalaValidationResult.ValidationPassed
         : IsKennitalaValidationResult.InvalidCentury;
   }

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> kennitala)
   {
#pragma warning disable IDE0008 // Use explicit type
      var (day, month, year) = GetDayMonthYear(kennitala);
#pragma warning restore IDE0008 // Use explicit type

      // No need to validate year because validation has already confirmed that
      // the kennitala is all digits and that the century indicator is valid.

      if (month < 1 || month > 12)
      {
         return false;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }

   private static Boolean ValidateSeparator(ReadOnlySpan<Char> kennitala)
      => !IsFormatted(kennitala) || !kennitala[SeparatorOffset].IsAsciiDigit();

   private static IsKennitalaValidationResult ValidateCheckDigit(ReadOnlySpan<Char> kennitala)
   {
      // Note that while the documentation in the linked articles does not
      // explicitly state it, it appears that values that would result in a
      // check digit of 10 are not issued. This is consistent with other
      // modulus 11 check digit examples such as Norwegian fødselsnummer.

      var sum = 0;
      var weightIndex = 0;
      var isFormatted = IsFormatted(kennitala);
      var processLength = kennitala.Length - 1;
      for (var index = 0; index < processLength; index++)
      {
         if (isFormatted && index == SeparatorOffset)
         {
            continue;
         }

         var num = kennitala[index] - Chars.DigitZero;
         if (num < 0 || num > 9)
         {
            return IsKennitalaValidationResult.InvalidCharacter;
         }

         sum += num * _weights[weightIndex];
         weightIndex++;
      }

      return (sum % 11) == 0
         ? IsKennitalaValidationResult.ValidationPassed
         : IsKennitalaValidationResult.InvalidCheckDigit;
   }
}

public class IsKennitalaJsonConverter : JsonConverter<IsKennitala>
{
   public override IsKennitala Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var kennitalaString = reader.GetString();
      return new IsKennitala(kennitalaString);
   }

   public override void Write(Utf8JsonWriter writer, IsKennitala value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
