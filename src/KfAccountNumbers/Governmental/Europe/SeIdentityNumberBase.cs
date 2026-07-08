namespace KfAccountNumbers.Governmental.Europe;

#pragma warning disable IDE0250 // Make struct 'readonly'

/// <summary>
///   Abstract base class for either of the identifiers issued by the Swedish
///   Tax Agency, the personnummer issued to permanent residents of Sweden and
///   the samordningsnummer issued to temporary residents.
/// </summary>
public abstract record SeIdentityNumberBase
{
   /// <summary>
   ///   Discriminated union defining the types of Swedish identity numbers.
   /// </summary>
   public union IdentifierCategory(SeIdentifierType.Personnummer, SeIdentifierType.Samordningsnummer) { }

   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new Swedish identity numbers.
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
   ///   validating a Swedish identity numbers.
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
   ///   The latest year of birth supported by Swedish identity numbers.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2099;

   /// <summary>
   ///   The earliest year of birth supported by Swedish identity numbers.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1800;

   /// <summary>
   ///   Represents the day offset used to distinguish Swedish coordination
   ///   numbers (samordningsnummer) from personnummers.
   /// </summary>
   /// <remarks>
   ///   In Swedish personal identity numbers, a Samordningsnummer is indicated
   ///   by adding 60 to the day component of the date of birth.
   /// </remarks>
   public const Int32 SamordningsnummerDayOffset = 60;

   /// <summary>
   ///   The allowed length for a Swedish personal identity number with an
   ///   6-digit date of birth.
   /// </summary>
   public const Int32 ShortFormatLength = 11;

   /// <summary>
   ///   The allowed length for a Swedish personal identity number with an
   ///   8-digit date of birth.
   /// </summary>
   public const Int32 LongFormatLength = 13;

   /// <summary>
   ///   Identifies the location of the separator character, measured from the
   ///   end of the value.
   /// </summary>
   protected const Int32 SeparatorOffset = 5;

   /// <summary>
   ///   Identifies the location of the gender character, measured from the
   ///   end of the value.
   /// </summary>
   protected const Int32 GenderOffset = 2;

   /// <summary>
   ///   Century cutoff used when separator character is '+'.
   /// </summary>
   protected static readonly CenturyCutoff TwentithCenturyCutoff = new(currentCentury: 1900);

   private const Int32 InternalRepresentationLength = 12;      // YYYYMMDD + birth serial number + check digit

   /// <summary>
   ///   Defines how date offsets are applied when extracting the date of birth
   ///   from a value.
   /// </summary>
   protected enum DateOffsetMode
   {
      /// <summary>
      ///   Personnummers never adjust the date.
      /// </summary>
      Personummer = 0,

      /// <summary>
      ///   Samordningsnummers adjust the day by removing the +60 day offset.
      /// </summary>
      Samordningsnummer,

      /// <summary>
      ///   Date will be adjusted to remove an offset if necessary.
      /// </summary>
      Optional,
   }

   /// <summary>
   ///   Get the correct century cutoff, based on the separator character.
   /// </summary>
   /// <param name="value">
   ///   The value being processed.
   /// </param>
   /// <returns>
   ///   A <see cref="CenturyCutoff"/> object. When the separator character is a
   ///   dash, the cutoff's current century is assumed to be 2000. Otherwise,
   ///   the cutoff's current century is assumed to be 1900.
   /// </returns>
   protected static CenturyCutoff GetCenturyCutoff(ReadOnlySpan<Char> value)
      => value[^SeparatorOffset] == Chars.Dash
         ? CenturyCutoff.DefaultInstance
         : TwentithCenturyCutoff;

   /// <summary>
   ///   Given the internal representation of a Swedish identity number, extract
   ///   the date of birth from the first 8 digits.
   /// </summary>
   /// <param name="value">
   ///   Internal representation of a Swedish identity number in YYYYMMDDSSSC
   ///   format.
   /// </param>
   /// <param name="dateOffsetMode">
   ///   Defines how date offsets should be handled.
   /// </param>
   /// <returns>
   ///   The person's date of birth.
   /// </returns>
   protected static DateOnly GetDateOfBirth(
      String value,
      DateOffsetMode dateOffsetMode = DateOffsetMode.Optional)
   {
#pragma warning disable IDE0008 // Use explicit type
      var (year, month, day) = GetYearMonthDay(value, dateOffsetMode);
#pragma warning restore IDE0008 // Use explicit type

      return new DateOnly(year, month, day);
   }

   /// <summary>
   ///   Extract just the day component of the <paramref name="value"/>'s date
   ///   of birth.
   /// </summary>
   /// <param name="value">
   ///   The value to process.
   /// </param>
   /// <returns>
   ///   The integer day component of the <paramref name="value"/>'s date of
   ///   birth.
   /// </returns>
   protected static Int32 GetDayOfBirth(ReadOnlySpan<Char> value)
   {
      var offset = value.Length == ShortFormatLength ? 4 : 6;

      return value[offset..].ParseTwoDigits();
   }

   /// <summary>
   ///   Given a validated identity number, get the internal representation
   ///   which converts six digit date of birth to eight digits and strips out the
   ///   separator character.
   /// </summary>
   /// <param name="value">
   ///   The validated identity number.
   /// </param>
   /// <returns>
   ///   The normalized identity number.
   /// </returns>
   protected static String GetNormalizedValue(String value)
   {
      // Handle value that is already normalized. This supports the
      // ToPersonnummer and ToSamordningsnummer methods of SeIdentityNumber.
      if (value.Length == InternalRepresentationLength)
      {
         return value;
      }

      var buffer = ArrayPool<Char>.Shared.Rent(InternalRepresentationLength);
      try
      {
         var span = new Span<Char>(buffer);
         ReadOnlySpan<Char> source;
         Int32 sourceOffset;

         // Copy 4-digit year.
         Span<Char> target = span[..4];
         if (value.Length == ShortFormatLength)
         {
            sourceOffset = 2;
            CenturyCutoff centuryCutoff = GetCenturyCutoff(value);
            var year = value.ParseTwoDigits();
            year = centuryCutoff.ToFourDigitYear(year);
            _ = year.TryFormat(target, out _, format: "D4", provider: CultureInfo.InvariantCulture);
         }
         else
         {
            sourceOffset = 4;
            source = value.AsSpan(..4);
            target = span[..4];
            source.CopyTo(target);
         }

         // Month & day.
         var end = sourceOffset + 4;
         source = value.AsSpan(sourceOffset..end);
         target = span[4..8];
         source.CopyTo(target);

         // Birth serial number and check digit.
         sourceOffset += 5;
         end += 5;
         source = value.AsSpan(sourceOffset..end);
         target = span[8..12];
         source.CopyTo(target);

         return span[..InternalRepresentationLength].ToString();
      }
      finally
      {
         ArrayPool<Char>.Shared.Return(buffer);
      }
   }

   /// <summary>
   ///   Extract the year, month and day elements of the person's date of birth.
   /// </summary>
   /// <param name="value">
   ///   The value being processed.
   /// </param>
   /// <param name="dateOffsetMode">
   ///   Defines how date offsets should be handled.
   /// </param>
   /// <returns>
   ///   The year, month and day of the person's date of birth.
   /// </returns>
   protected static (Int32 Year, Int32 Month, Int32 Day) GetYearMonthDay(
      ReadOnlySpan<Char> value,
      DateOffsetMode dateOffsetMode)
   {
      Int32 year;
      Int32 month;
      Int32 day;
      if (value.Length == ShortFormatLength)
      {
         // Refer to class XML comments for details of 2 digit year calculations.
         CenturyCutoff centuryCutoff = GetCenturyCutoff(value);
         year = value.ParseTwoDigits();
         year = centuryCutoff.ToFourDigitYear(year);
         month = value[2..].ParseTwoDigits();
         day = value[4..].ParseTwoDigits();
      }
      else
      {
         // This works for both 13 character values with separator and 12 character
         // all digit internal representation.
         year = value.ParseFourDigits();
         month = value[4..].ParseTwoDigits();
         day = value[6..].ParseTwoDigits();
      }

      // Handle date offset such as samordningsnummer.
#pragma warning disable IDE0072 // Add missing cases
      day = dateOffsetMode switch
      {
         DateOffsetMode.Samordningsnummer => day - SamordningsnummerDayOffset,
         DateOffsetMode.Optional => day > 31 ? day - SamordningsnummerDayOffset : day,
         _ => day,
      };
#pragma warning restore IDE0072 // Add missing cases

      return (year, month, day);
   }

   /// <summary>
   ///   Format an internal representation (YYYYMMDDSSSC) to a long format
   ///   value, including the correct separator for the person's age.
   /// </summary>
   /// <param name="value">
   ///   The internal representation of a Swedish identity number.
   /// </param>
   /// <param name="timeProvider">
   ///   Optional <see cref="TimeProvider"/> used to determine the current date,
   ///   which is then used to pick the correct separator character to use (
   ///   dash ('-') for persons less than 100 years old and plus ('+') for
   ///   persons 100 years or older. If <see langword="null"/>, then the
   ///   separator will default to dash ('-').
   /// </param>
   /// <returns>
   ///   The identity number reformatted to YYYYMMDD-SSSC format.
   /// </returns>
   protected static String InternalRepresentationToLongFormat(
      String value,
      TimeProvider? timeProvider)
   {
      var separator = timeProvider is not null
         ? GetCorrectSeparatorForAgeOfPerson(GetDateOfBirth(value), timeProvider)
         : Chars.Dash;

      return value[..8] + separator + value[^4..];
   }

   /// <summary>
   ///   Format an internal representation (YYYYMMDDSSSC) to a short format
   ///   value, including the correct separator for the person's age.
   /// </summary>
   /// <param name="value">
   ///   The internal representation of a Swedish identity number.
   /// </param>
   /// <param name="timeProvider">
   ///   Optional <see cref="TimeProvider"/> used to determine the current date,
   ///   which is then used to pick the correct separator character to use (
   ///   dash ('-') for persons less than 100 years old and plus ('+') for
   ///   persons 100 years or older. If <see langword="null"/>, then the
   ///   separator will default to dash ('-').
   /// </param>
   /// <returns>
   ///   The identity number reformatted to YYMMDD-SSSC format.
   /// </returns>
   protected static String InternalRepresentationToShortFormat(
      String value,
      TimeProvider? timeProvider)
   {
      var separator = timeProvider is not null
         ? GetCorrectSeparatorForAgeOfPerson(GetDateOfBirth(value), timeProvider)
         : Chars.Dash;

      return value[2..8] + separator + value[^4..];
   }

   /// <summary>
   ///   If <see cref="ValidateCheckDigit(String)"/> returns false, determine
   ///   if the reason was an invalid character or an invalid check digit.
   /// </summary>
   /// <param name="value">
   ///   The value to check.
   /// </param>
   /// <returns>
   ///   The zero-based index of the first non-digit character or -1 if no
   ///   non-digit characters found.
   /// </returns>
   protected static Int32 LocateInvalidCharacter(ReadOnlySpan<Char> value)
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

   /// <summary>
   ///   Determine the <paramref name="value"/> has a valid check digit.
   /// </summary>
   /// <param name="value">
   ///   The value to check.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if <paramref name="value"/> has a valid check
   ///   digit; otherwise <see langword="false"/>.
   /// </returns>
   protected static Boolean ValidateCheckDigit(String value)
   {
      ICheckDigitMask checkDigitMask = value.Length == ShortFormatLength
         ? SeIdentityNumberShortFormatCheckDigitMask.Instance
         : SeIdentityNumberLongFormatCheckDigitMask.Instance;
      return MaskedAlgorithms.Luhn.Validate(value, checkDigitMask);
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
      var (year, month, day) = GetYearMonthDay(value, dateOffsetMode);
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
   ///   Determine if the <paramref name="value"/> has a valid length.
   /// </summary>
   /// <param name="value">
   ///   The value to check.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if <paramref name="value"/> has a valid length;
   ///   otherwise <see langword="false"/>.
   /// </returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   protected static Boolean ValidateLength(ReadOnlySpan<Char> value)
      => value.Length is ShortFormatLength or LongFormatLength;

   private static Char GetCorrectSeparatorForAgeOfPerson(
      DateOnly dateOfBirth,
      TimeProvider timeProvider)
   {
      DateTime today = timeProvider.GetLocalNow().Date;
      var age = today.Year - dateOfBirth.Year;
      if (today.Month < dateOfBirth.Month ||
          (today.Month == dateOfBirth.Month && today.Day < dateOfBirth.Day))
      {
         age--;
      }

      return age >= 100 ? Chars.Plus : Chars.Dash;
   }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
internal class SeIdentityNumberShortFormatCheckDigitMask : ICheckDigitMask
{
   private static readonly Lazy<SeIdentityNumberShortFormatCheckDigitMask> _instance =
      new(() => new SeIdentityNumberShortFormatCheckDigitMask());

   public static SeIdentityNumberShortFormatCheckDigitMask Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index) => index == 6;

   public Boolean IncludeCharacter(Int32 index) => index != 6;
}

internal class SeIdentityNumberLongFormatCheckDigitMask : ICheckDigitMask
{
   private static readonly Lazy<SeIdentityNumberLongFormatCheckDigitMask> _instance =
      new(() => new SeIdentityNumberLongFormatCheckDigitMask());

   public static SeIdentityNumberLongFormatCheckDigitMask Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index)
      => index is 0 or 1 or 8;

   public Boolean IncludeCharacter(Int32 index)
      => index is not 0 and not 1 and not 8;
}
