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
   protected static String GetNormalizedValue(ReadOnlySpan<Char> value)
   {
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
            source = value[..4];
            target = span[..4];
            source.CopyTo(target);
         }

         // Month & day.
         var end = sourceOffset + 4;
         source = value[sourceOffset..end];
         target = span[4..8];
         source.CopyTo(target);

         // Birth serial number and check digit.
         sourceOffset += 5;
         end += 5;
         source = value[sourceOffset..end];
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
   /// <returns>
   ///   The year, month and day of the person's date of birth.
   /// </returns>
   protected static (Int32 Year, Int32 Month, Int32 Day) GetYearMonthDay(ReadOnlySpan<Char> value)
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

      // Handle samordningsnummer, which adds 60 to the date of birth.
      if (day > SamordningsnummerDayOffset)
      {
         day -= SamordningsnummerDayOffset;
      }

      return (year, month, day);
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
         ? SePersonNumberShortFormatCheckDigitMask.Instance
         : SePersonNumberLongFormatCheckDigitMask.Instance;
      return MaskedAlgorithms.Luhn.Validate(value, checkDigitMask);
   }

   /// <summary>
   ///   Determine if the <paramref name="value"/> contains a valid date of
   ///   birth.
   /// </summary>
   /// <param name="value">
   ///   The value to check.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if <paramref name="value"/> has a valid date of
   ///   birth; otherwise <see langword="false"/>.
   /// </returns>
   protected static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> value)
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
}
