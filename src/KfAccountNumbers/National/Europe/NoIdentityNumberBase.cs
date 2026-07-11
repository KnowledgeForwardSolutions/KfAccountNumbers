namespace KfAccountNumbers.National.Europe;

#pragma warning disable IDE0250 // Make struct 'readonly'

/// <summary>
///   Abstract base class for Norwegian personal identity numbers.
/// </summary>
public abstract record NoIdentityNumberBase
{
   /// <summary>
   ///   Discriminated union defining the types of identifier that Norwegian
   ///   identity number types in this hierarchy can represent.
   /// </summary>
   public union IdentifierCategory(NoIdentifierType.Foedselsnummer, NoIdentifierType.DNummer) { }

   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new Norwegian identity number types in this
   ///   hierarchy.
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
   ///   validating Norwegian identity number types in this hierarchy.
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
   ///   The name of the check digit algorithm used by Norwegian identity
   ///   numbers.
   /// </summary>
   public const String CheckDigitAlgorithmName = "Weighted Modulus 11";

   /// <summary>
   ///   Represents the day offset used to distinguish a D-nummer from a
   ///   fødselsnummer.
   /// </summary>
   /// <remarks>
   ///   In Norwegian identity numbers, a D-nummer is indicated by
   ///   adding 40 to the day component of the date of birth.
   /// </remarks>
   public const Int32 DNummerDayOffset = 40;

   /// <summary>
   ///   The latest year of birth supported by Norwegian identity numbers.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2039;

   /// <summary>
   ///   The earliest year of birth supported by Norwegian identity numbers.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1854;

   /// <summary>
   ///   The valid length of an unformatted Norwegian identity number.
   /// </summary>
   public const Int32 UnformattedLength = 11;

   /// <summary>
   ///   The valid length of a formatted Norwegian identity number.
   /// </summary>
   public const Int32 FormattedLength = 12;

   /// <summary>
   ///   The default mask used to format Norwegian identity numbers.
   /// </summary>
   public const String DefaultFormatMask = "______ _____";

   /// <summary>
   ///   Identifies the location of the gender character, measured from the
   ///   end of the value.
   /// </summary>
   protected const Int32 GenderOffset = 3;              // Gender indicated by odd/even-ness of individual number, but only need to examine the last digit

   /// <summary>
   ///   Zero-based offset of the separator character.
   /// </summary>
   protected const Int32 SeparatorOffset = 6;

   // Offsets measured from end of value to avoid needing to account for the
   // presence or absence of a separator.
   private const Int32 IndividualNumberOffset = 5;

   private static readonly Int32[] _c1Weights = [3, 7, 6, 1, 8, 9, 4, 5, 2, 1, 0];
   private static readonly Int32[] _c2Weights = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2, 1];

   /// <summary>
   ///   Defines how date offsets are applied when extracting the date of birth
   ///   from a value.
   /// </summary>
   protected enum DateOffsetMode
   {
      /// <summary>
      ///   Fødselsnummers never adjust the date.
      /// </summary>
      Fodselsnummer = 0,

      /// <summary>
      ///   D-nummers adjust the day by removing the +40 day offset.
      /// </summary>
      Dnummer,

      /// <summary>
      ///   H-nummers adjust the month by removing the +40 month offset.
      /// </summary>
      Hnummer,

      /// <summary>
      ///   Date will be adjusted to remove an offset if necessary.
      /// </summary>
      Optional,
   }

   /// <summary>
   ///   Extract the day, month and year elements of the person's date of birth.
   /// </summary>
   /// <param name="value">
   ///   The value being processed.
   /// </param>
   /// <param name="dateOffsetMode">
   ///   Defines how date offsets should be handled.
   /// </param>
   /// <returns>
   ///   The day, month and year of the person's date of birth.
   /// </returns>
   protected static (Int32 Day, Int32 Month, Int32 Year) GetDayMonthYear(
      ReadOnlySpan<Char> value,
      DateOffsetMode dateOffsetMode)
   {
      var baseDay = value.ParseTwoDigits();
      var baseMonth = value[2..].ParseTwoDigits();
      var baseYear = value[4..].ParseTwoDigits();

      // Handle possible day/month offsets.
      #pragma warning disable IDE0072 // Add missing cases
      var day = dateOffsetMode switch
      {
         DateOffsetMode.Dnummer => baseDay - DNummerDayOffset,
            DateOffsetMode.Optional => baseDay > 31 ? baseDay - DNummerDayOffset : baseDay,
         _ => baseDay,
      };

      var month = dateOffsetMode switch
      {
         DateOffsetMode.Hnummer => baseMonth - DNummerDayOffset,
         DateOffsetMode.Optional => baseMonth > 12 ? baseMonth - DNummerDayOffset : baseMonth,
         _ => baseMonth,
      };
#pragma warning restore IDE0072 // Add missing cases

      // Adjust the year according to the value of the individual number.
      var individualNumber = value[^IndividualNumberOffset..].ParseThreeDigits();
      var century = baseDay <= 31
         ? GetFodselsnummerBirthCentury(baseYear, individualNumber)
         : GetDnummerBirthCentury(individualNumber);
      var year = baseYear + century;

      return (day, month, year);
   }

   /// <summary>
   ///   Given a validated identity number, get the normalized representation
   ///   which  strips out the separator character.
   /// </summary>
   /// <param name="value">
   ///   The validated identity number.
   /// </param>
   /// <returns>
   ///   The normalized identity number.
   /// </returns>
   protected static String GetNormalizedValue(String value)
      => value.Length == UnformattedLength
         ? value
         : String.Concat(
            value.AsSpan(0, SeparatorOffset),
            value.AsSpan(SeparatorOffset + 1));

   /// <summary>
   ///   Determine the <paramref name="value"/> has valid check digits.
   /// </summary>
   /// <param name="value">
   ///   The value to check.
   /// </param>
   /// <param name="invalidCharacterOffset">
   ///   Out parameter that identifies the zero-based offset of the first
   ///   invalid character found, or -1 if no invalid characters were found.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if the value has valid check digits; otherwise
   ///   <see langword="false"/>.
   /// </returns>
   protected static Boolean ValidateCheckDigits(
      ReadOnlySpan<Char> value,
      out Int32 invalidCharacterOffset)
   {
      invalidCharacterOffset = -1;

      // Calculate weighted sums for both check digits in a single pass. Final
      // c1 weight is zero so that it the final digit is excluded from c1 sum.
      var isFormatted = IsFormatted(value);
      var c1Sum = 0;
      var c2Sum = 0;
      var weightIndex = 0;
      var processLength = value.Length;
      for (var charIndex = 0; charIndex < processLength; charIndex++)
      {
         if (isFormatted && charIndex == SeparatorOffset)
         {
            continue;
         }

         var num = value[charIndex] - Chars.DigitZero;
         if (num is < 0 or > 9)
         {
            invalidCharacterOffset = charIndex;
            return false;
         }

         c1Sum += num * _c1Weights[weightIndex];
         c2Sum += num * _c2Weights[weightIndex];
         weightIndex++;
      }

      // Both weighted sums must be multiples of 11 for the check digits to be valid.
      return (c1Sum % 11) == 0 && (c2Sum % 11) == 0;
   }

   /// <summary>
   ///   Determine if the <paramref name="value"/> contains a valid date of
   ///   birth.
   /// </summary>
   /// <param name="value">
   ///   The value to check.
   /// </param>
   /// <param name="dateOffsetMode">
   ///   Defines how date offsets should be handled.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if <paramref name="value"/> has a valid date of
   ///   birth; otherwise <see langword="false"/>.
   /// </returns>
   protected static Boolean ValidateDateOfBirth(
      ReadOnlySpan<Char> value,
      DateOffsetMode dateOffsetMode)
   {
      // Manual validation is faster than using DateTime.TryParseExact.
#pragma warning disable IDE0008 // Use explicit type
      var (day, month, year) = GetDayMonthYear(value, dateOffsetMode);
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

   /// <summary>
   ///   Determine if the <paramref name="value"/> has a valid separator
   ///   character.
   /// </summary>
   /// <param name="value">
   ///   The value to check.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if the value is unformatted or is formatted and
   ///   has a valid separator character; otherwise <see langword="false"/>.
   /// </returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   protected static Boolean ValidateSeparator(ReadOnlySpan<Char> value)
      => !IsFormatted(value) || !value[SeparatorOffset].IsAsciiDigit();

   // Per Wikipedia (https://en.wikipedia.org/wiki/National_identity_number_(Norway))
   // D-nummer has much simpler rules for birth century than fødselsnummer.
   private static Int32 GetDnummerBirthCentury(Int32 individualNumber)
      => individualNumber < 500 ? 1900 : 2000;

   // See https://blog.variant.no/ssns-and-pattern-matching-in-c-9-498f96aa71d4
   // for details for how fødselsnummer birth century is derived.
#pragma warning disable format
   private static Int32 GetFodselsnummerBirthCentury(
      Int32 year,
      Int32 individualNumber)
      => (individualNumber, year) switch
      {
         // Rule 1. 500–749: 1854–1899
         (>= 500 and <= 749, >= 54) => 1800,

         // Rule 2. 000–499: 1900–1999
         (< 500, _) => 1900,

         // Rule 3. 900–999: 1940–1999
         (>= 900, >= 40) => 1900,

         // Rule 4. 500–999: 2000–2039
         (>= 500, <= 39) => 2000,

         // No rule
         (_, _) => 0,
      };
#pragma warning restore format

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> value)
      => value.Length == FormattedLength;
}
