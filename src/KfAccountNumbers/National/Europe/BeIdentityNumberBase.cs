#pragma warning disable IDE0250 // Make struct 'readonly'

namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Abstract base class for Belgian personal identity numbers.
/// </summary>
public abstract record BeIdentityNumberBase
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new Belgian identity number.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidSequenceNumber,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a Belgian identity number.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidSequenceNumber,
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
   ///   The name of the check digit algorithm used by rijksregisternummer and
   ///   BIS-nummer values.
   /// </summary>
   public const String CheckDigitAlgorithmName = "Modulus 97";

   /// <summary>
   ///   The default format to use when formatting Belgian identity numbers.
   /// </summary>
   public const String DefaultFormatMask = "__.__.__-___.__";

   /// <summary>
   ///   The latest year of birth supported by rijksregisternummer and
   ///   BIS-nummer values.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2099;

   /// <summary>
   ///   The earliest year of birth supported by rijksregisternummer and
   ///   BIS-nummer values.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1900;

   /// <summary>
   ///   The allowed length of a rijksregisternummer or BIS-nummer that does not
   ///   contain additional formatting characters.
   /// </summary>
   public const Int32 UnformattedLength = 11;

   /// <summary>
   ///   The allowed length of a rijksregisternummer or BIS-nummer that contains
   ///   additional characters to format it for readability.
   /// </summary>
   public const Int32 FormattedLength = 15;

   /// <summary>
   ///   Index (measured from the end of the value) of the gender indicator.
   /// </summary>
   protected const Int32 GenderOffset = 3;

   private const Int32 Separator1Offset = 2;
   private const Int32 Separator2Offset = 5;
   private const Int32 Separator3Offset = 8;
   private const Int32 Separator4Offset = 12;

   private const Int32 MinValidSequence = 1;
   private const Int32 MaxValidSequence = 998;

   private static readonly Int32[] _separatorOffsets =
   [
      Separator1Offset,
      Separator2Offset,
      Separator3Offset,
      Separator4Offset
   ];

   // These items are measured from the end of the value.
   private const Int32 CheckDigit1Offset = 2;
   private const Int32 CheckDigit2Offset = 1;

   /// <summary>
   ///   Defines how date offsets are applied when extracting the date of birth
   ///   from a value.
   /// </summary>
   protected enum DateOffsetMode
   {
      /// <summary>
      ///   Rijksregisternummers never adjust the date.
      /// </summary>
      Rijksregisternummer = 0,

      /// <summary>
      ///   Bisnummers adjust the day by removing the +20/+40 month offset.
      /// </summary>
      Bisnummer,

      /// <summary>
      ///   Date will be adjusted to remove an offset if necessary.
      /// </summary>
      Optional,
   }

   /// <summary>
   ///   Given a validated identity number, get the internal representation
   ///   which strips out any separator characters.
   /// </summary>
   /// <param name="value">
   ///   The validated identity number.
   /// </param>
   /// <returns>
   ///   The normalized identity number.
   /// </returns>
   protected static String GetNormalizedValue(String value)
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
      var fieldWidth = value.Length == UnformattedLength ? 2 : 3;
      var year = value.ParseTwoDigits();

      var fieldStart = fieldWidth;
      var month = value[fieldStart..].ParseTwoDigits();

      fieldStart += fieldWidth;
      var day = value[fieldStart..].ParseTwoDigits();

      fieldStart += fieldWidth;
      var sequenceNumber = value[fieldStart..].ParseThreeDigits();

      // Apply BIS-nummer offsets if necessary.
      var effectiveMonth = (dateOffsetMode, month) switch
      {
         // Bisnummer always applies an offset.
         (DateOffsetMode.Bisnummer, _) => month >= BisNummerMonthOffset
            ? month - BisNummerMonthOffset
            : month - BisNummerUnknownGenderMonthOffset,

         // Optional may apply an offset.
         (DateOffsetMode.Optional, > BisNummerMonthOffset) => month - BisNummerMonthOffset,
         (DateOffsetMode.Optional, > BisNummerUnknownGenderMonthOffset) => month - BisNummerUnknownGenderMonthOffset,

         // Otherwise leave unchanged.
         _ => month,
      };

      // Add the century to the year.
      // Already parsed the individual elements, combine to use in checksum calculation.
      var total = sequenceNumber + (day * 1000) + (month * 100000) + (year * 10000000);
      var checksum = value[^2..].ParseTwoDigits();
      var century = (97 - (total % 97)) == checksum
         ? 1900
         : 2000;

      year += century;

      return (year, effectiveMonth, day);
   }

   /// <summary>
   ///   Determine if the value contains format characters.
   /// </summary>
   /// <param name="value">
   ///   The value to check.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if the value contains format characters;
   ///   otherwise <see langword="false"/>.
   /// </returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   protected static Boolean IsFormatted(ReadOnlySpan<Char> value)
      => value.Length == FormattedLength;

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
            invalidCharacterOffset = index;
            return false;
         }

         sum += num;
      }

      var c1 = value[^CheckDigit1Offset].ToSingleDigit();
      if (!c1.IsValidDigit())
      {
         invalidCharacterOffset = value.Length - CheckDigit1Offset;
         return false;
      }

      var c2 = value[^CheckDigit2Offset].ToSingleDigit();
      if (!c2.IsValidDigit())
      {
         invalidCharacterOffset = value.Length - CheckDigit2Offset;
         return false;
      }

      var checkSum = (c1 * 10) + c2;
      invalidCharacterOffset = -1;

      // Check for persons born 1900-1999.
      var remainder = 97 - (sum % 97);
      if (remainder == checkSum)
      {
         return true;
      }

      // Then for persons born 2000-2099;
      var longRemainder = 97 - ((2000000000L + sum) % 97);           // Long int to handle possible int overflow
      return longRemainder == checkSum;
   }

   /// <summary>
   ///   Determine if <paramref name="value"/> has a valid date of birth.
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
#pragma warning disable IDE0008 // Use explicit type
      var (year, month, day) = GetYearMonthDay(value, dateOffsetMode);
#pragma warning restore IDE0008 // Use explicit type

      if (year is < MinimumValidYearOfBirth or > MaximumValidYearOfBirth)
      {
         // Should be impossible to ever reach this point because of the check
         // digit calcuations, but return false out of abundance of caution and
         // to avoid throwing an exception.
         return false;
      }

      if (month is < 0 or > 12)
      {
         return false;
      }

#pragma warning disable format
      var maxDay = (year, month) switch
      {
         (> 0, > 0) => DateTime.DaysInMonth(year, month),
         (0, > 0) => DateTime.DaysInMonth(2000, month),        // Year unknown, assume leap year
         _ => 31,
      };
#pragma warning restore format
      if (day > maxDay)
      {
         return false;
      }

      // Final sanity check. Must have at least one non-zero element. Even an
      // unknown date of birth will default to YYMMDD of 000001.
      return year % 100 != 0 || month != 0 || day != 0;
   }

   /// <summary>
   ///   Determine if <paramref name="value"/> has valid separator characters.
   /// </summary>
   /// <param name="value">
   ///   The value to check.
   /// </param>
   /// <param name="invalidSeparatorPosition">
   ///   Out parameter that identifies the zero-based offset of the first
   ///   invalid separator found, or -1 if no invalid separator characters were
   ///   found.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if the value has valid separator characters;
   ///   otherwise <see langword="false"/>.
   /// </returns>
   protected static Boolean ValidateSeparators(
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

   /// <summary>
   ///   Determine if <paramref name="value"/> has a valid sequence number.
   /// </summary>
   /// <param name="value">
   ///   The value to check.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if <paramref name="value"/> has a valid sequence
   ///   number; otherwise <see langword="false"/>.
   /// </returns>
   protected static Boolean ValidateSequenceNumber(ReadOnlySpan<Char> value)
   {
      var offsetFromEnd = IsFormatted(value) ? 6 : 5;
      var sequenceNumber = value[^offsetFromEnd..].ParseThreeDigits();

      return sequenceNumber is >= MinValidSequence and <= MaxValidSequence;
   }
}
