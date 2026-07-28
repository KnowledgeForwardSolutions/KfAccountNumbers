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
   ///   Determine the <paramref name="value"/> has valid separator characters
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

}
