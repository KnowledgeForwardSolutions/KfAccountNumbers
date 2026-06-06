using Microsoft.Win32.SafeHandles;

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Abstract base class for any of the patient identifiers issued by the
///   UK public health services (NHS, CHI, and H&amp;C).
/// </summary>
public abstract record GbPatientNumberBase
{
   /// <summary>
   ///   The six-digit date format used by CHI numbers.
   /// </summary>
   public const String ChiNumberDateFormat = "DDMMYY";

   /// <summary>
   ///   The default format mask that will render the patient number as a
   ///   "3 3 4" value.
   /// </summary>
   public const String DefaultFormatMask = "___ ___ ____";

   /// <summary>
   ///   The length of a valid patient number that is not formatted for readability.
   /// </summary>
   internal const Int32 UnformattedLength = 10;

   /// <summary>
   ///   The length of a valid patient number that is formatted for readability.
   /// </summary>
   internal const Int32 FormattedLength = 12;

   /// <summary>
   ///   The zero-based offset of the first separator character in a formatted patient number.
   /// </summary>
   internal const Int32 SeparatorPosition1 = 3;

   /// <summary>
   ///   The zero-based offset of the second separator character in a formatted patient number.
   /// </summary>
   internal const Int32 SeparatorPosition2 = 7;

   /// <summary>
   ///   The zero-based offset for the digit that encodes gender for CHI numbers.
   /// </summary>
   internal const Int32 GenderOffset = 8;

   /// <summary>
   ///   Predefined array used when reporting invalid length values.
   /// </summary>
   internal static readonly ValidLengthDefinition[] ValidLengthDefinitions =
   [
      new ValidLengthDefinition(UnformattedLength, Messages.GbPatientNumberUnformattedLength),
      new ValidLengthDefinition(FormattedLength, Messages.GbPatientNumberFormattedLength)
   ];

   /// <summary>
   ///   Specifies which block of identifiers a particular number is a member of.
   /// </summary>
   protected enum IdentifierRangeCategory
   {
      /// <summary>
      ///   Value is not a member of valid block of identifiers.
      /// </summary>
      Invalid = 0,

      /// <summary>
      ///   Scottish CHI block: 010 000 000 to 311 299 999.
      /// </summary>
      Chi,

      /// <summary>
      ///   Northern Ireland H&amp;C block: 320 000 000 to 399 999 999.
      /// </summary>
      Hc,

      /// <summary>
      ///   National Health Service block: 400 000 000 to 499 999 999 or
      ///   600 000 000 to 799 999 999.
      /// </summary>
      Nhs,

      /// <summary>
      ///   Reserved test block: 900 000 000 to 999 999 999.
      /// </summary>
      Test,
   }

   /// <summary>
   ///   Extract the two-digit day, month and year elements from a formatted or
   ///   unformatted patient number <paramref name="value"/>.
   /// </summary>
   /// <param name="value">
   ///   The patient number containing the day, month and year values.
   /// </param>
   /// <returns>
   ///   A tuple containing the two-digit day, month and year elements contained
   ///   in the first six digits of the <paramref name="value"/>.
   /// </returns>
   protected static (Int32 Day, Int32 Month, Int32 Year) GetDayMonthYear(ReadOnlySpan<Char> value)
   {
      var day = value.ParseTwoDigits();
      Int32 month, year;
      if (IsFormatted(value))
      {
         // When formatted, the month value is split across the trailing
         // character of the first group of three digits and the first character
         // of the second group of three digits.
         month = (value[2].ToSingleDigit() * 10) + value[4].ToSingleDigit();
         year = value[5..].ParseTwoDigits();
      }
      else
      {
         month = value[2..].ParseTwoDigits();
         year = value[4..].ParseTwoDigits();
      }

      return (day, month, year);
   }

   /// <summary>
   ///   Determines the identifier category based on the first four digits of
   ///   the identifier.
   /// </summary>
   /// <param name="value">
   ///   The identifier value to categorize.
   /// </param>
   /// <returns>
   ///   The identifier range category corresponding to the numeric range.
   /// </returns>
   /// <remarks>
   ///   Assumes that the value has already been validated to only contain
   ///   digit characters (excluding optional separators).
   /// </remarks>
   protected static IdentifierRangeCategory GetIdentifierCategory(ReadOnlySpan<Char> value)
   {
      var fourthDigitOffset = IsFormatted(value) ? 4 : 3;
      var num = (value.ParseThreeDigits() * 10) + value[fourthDigitOffset].ToSingleDigit();

      return num switch
      {
         >= 0100 and <= 3112 => IdentifierRangeCategory.Chi,
         >= 3200 and <= 3999 => IdentifierRangeCategory.Hc,
         (>= 4000 and <= 4999) or (>= 6000 and <= 7999) => IdentifierRangeCategory.Nhs,
         >= 9000 => IdentifierRangeCategory.Test,
         _ => IdentifierRangeCategory.Invalid,
      };
   }

   /// <summary>
   ///   Create an invalid character result for a GB patient number.
   /// </summary>
   /// <param name="value">
   ///   The value containing the invalid character.
   /// </param>
   /// <param name="position">
   ///   The zero-based offset of the invalid character.
   /// </param>
   /// <returns>
   ///   A <see cref="InvalidCharacter"/> result containing the error message
   ///   and validation details.
   /// </returns>
   protected static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(Messages.GbPatientNumberInvalidCharacter, value[position], position);

   /// <summary>
   ///   Create an invalid checksum result for a GB patient number.
   /// </summary>
   /// <returns>
   ///   A <see cref="InvalidChecksum"/> result containing the error message
   ///   and validation details.
   /// </returns>
   protected static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.GbPatientNumberInvalidCheckDigit, Algorithms.Modulus11Decimal.AlgorithmName);

   /// <summary>
   ///   Create a new invalid date of birth result for a GB patient number.
   /// </summary>
   /// <param name="value">
   ///   The value containing the invalid date of birth.
   /// </param>
   /// <param name="description">
   ///   Message describing the validation failure.
   /// </param>
   /// <returns>
   ///   A <see cref="InvalidDateOfBirth"/> result containing the error message
   ///   and validation details.
   /// </returns>
   protected static InvalidDateOfBirth GetInvalidDateOfBirthResult(
      String value,
      String description)
      => new(description, value[..(IsFormatted(value) ? 7 : 6)], ChiNumberDateFormat);

   /// <summary>
   ///   Creates an invalid length result for a GB patient number.
   /// </summary>
   /// <param name="length">
   ///   The invalid length value.
   /// </param>
   /// <returns>
   ///   An <see cref="InvalidLength"/> result containing the error message and
   ///   validation details.
   /// </returns>
   protected static InvalidLength GetInvalidLengthResult(Int32 length)
      => new(Messages.GbPatientNumberInvalidLength, length, ValidLengthDefinitions);

   /// <summary>
   ///   Create an invalid separator result for a GB patient number.
   /// </summary>
   /// <param name="value">
   ///   The value containing the invalid separator.
   /// </param>
   /// <param name="position">
   ///   The zero-based offset of the invalid separator.
   /// </param>
   /// <returns>
   ///   A <see cref="InvalidSeparator"/> result containing the error message
   ///   and validation details.
   /// </returns>
   protected static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(Messages.GbPatientNumberInvalidSeparator, value[position], position);

   /// <summary>
   ///   Get the raw patient number value, stripped of any formatting.
   /// </summary>
   /// <param name="value">
   ///   The original patient number value.
   /// </param>
   /// <returns>
   ///   The raw patient number value, stripped of any formatting.
   /// </returns>
   protected static String GetRawValue(String value)
      => value.Length == UnformattedLength
         ? value
         : String.Concat(value.AsSpan(..3), value.AsSpan(4..7), value.AsSpan(8..));

   /// <summary>
   ///   Determines whether the value contains format characters (based on
   ///   the length of the value).
   /// </summary>
   /// <param name="value">
   ///   The value to check.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if the value length equals the formatted length;
   ///   otherwise, <see langword="false"/>.
   /// </returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   protected static Boolean IsFormatted(ReadOnlySpan<Char> value)
      => value.Length == FormattedLength;

   /// <summary>
   ///   Validates whether <paramref name="value"/> contains a valid Modulus 11
   ///   check digit.
   /// </summary>
   /// <param name="value">
   ///   The value to validate.
   /// </param>
   /// <param name="invalidCharacterPosition">
   ///   When this method returns, contains the zero-based position of the first
   ///   invalid character, or -1 if all characters are valid.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if the check digit is valid; otherwise, <see langword="false"/>.
   /// </returns>
   protected static Boolean ValidateCheckDigit(
      String value,
      out Int32 invalidCharacterPosition)
   {
      var validCheckDigit = IsFormatted(value)
         ? MaskedAlgorithms.Modulus11Decimal.Validate(value, GbPatientNumberMask.Instance)
         : Algorithms.Modulus11Decimal.Validate(value);

      // Modulus11Decimal.Validate returns false for an invalid check digit OR
      // if an invalid character was encountered (because a valid check digit
      // can not be calculated for an invalid character). Because we don't know
      // what caused the failure, we make a second pass to check for invalid
      // characters, but only if Modulus11Decimal.Validate fails. This optimizes
      // the success path to make the fewest passes through the value.
      invalidCharacterPosition = validCheckDigit
         ? -1
         : LocateInvalidCharacter(value);

      return validCheckDigit;
   }

   /// <summary>
   ///   Validates that the value date of birth component represents a valid
   ///   date.
   /// </summary>
   /// <param name="value">
   ///   The character span to validate.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if the value's date of birth component represents
   ///   a valid date; otherwise <see langword="false"/>.
   /// </returns>
   protected static Boolean ValidateChiNumberDateOfBirth(ReadOnlySpan<Char> value)
   {
#pragma warning disable IDE0008 // Use explicit type
      var (day, month, year) = GetDayMonthYear(value);
#pragma warning restore IDE0008 // Use explicit type

      if (month is < 1 or > 12)
      {
         return false;
      }

      // Treat YY as 20YY for date validation: within 00-99, the only leap-year
      // difference between 19YY and 20YY is YY=00 (1900 is not a leap year;
      // 2000 is). Since the CenturyCutoff helper treats 00 = 2000, we do the
      // same here to keep this check consistent with GetDateOfBirth.
      year += 2000;

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }

   /// <summary>
   ///   Validates whether the span length matches the expected unformatted or
   ///   formatted length.
   /// </summary>
   /// <param name="value">
   ///   The character span to validate.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if the length is valid; otherwise, <see langword="false"/>.
   /// </returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   protected static Boolean ValidateLength(ReadOnlySpan<Char> value)
      => value.Length is UnformattedLength or FormattedLength;

   /// <summary>
   ///   Confirm that <paramref name="value"/> does not contain invalid
   ///   separator characters.
   /// </summary>
   /// <param name="value">
   ///   The character span to validate.
   /// </param>
   /// <param name="invalidSeparatorPosition">
   ///   When this method returns, contains the zero-based position of the first
   ///   invalid separator, or -1 if all separators are valid.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if the value does not contain invalid separator
   ///   characters; otherwise, <see langword="false"/>.
   /// </returns>
   protected static Boolean ValidateSeparators(
      ReadOnlySpan<Char> value,
      out Int32 invalidSeparatorPosition)
   {
      invalidSeparatorPosition = -1;
      if (!IsFormatted(value))
      {
         return true;
      }

      var separator1 = value[SeparatorPosition1];
      if (Char.IsAsciiDigit(separator1))
      {
         invalidSeparatorPosition = SeparatorPosition1;
         return false;
      }
      else if (value[SeparatorPosition2] != separator1)
      {
         invalidSeparatorPosition = SeparatorPosition2;
         return false;
      }

      return true;
   }

   // Return the zero-based index of the first non-digit character (excluding
   // separators) or -1 if no non-digit characters found.
   private static Int32 LocateInvalidCharacter(ReadOnlySpan<Char> value)
   {
      var isFormatted = IsFormatted(value);

      for (var index = 0; index < value.Length; index++)
      {
         if (isFormatted && index is SeparatorPosition1 or SeparatorPosition2)
         {
            continue;
         }

         if (!Char.IsAsciiDigit(value[index]))
         {
            return index;
         }
      }

      return -1;
   }

   /// <summary>
   ///   <see cref="ICheckDigitMask"/> used when validating the
   ///   check digit of a formatted patient number.
   /// </summary>
   protected class GbPatientNumberMask : ICheckDigitMask
   {
      private static readonly Lazy<GbPatientNumberMask> _instance =
         new(() => new GbPatientNumberMask());

      /// <summary>
      ///   Gets the only instance of <see cref="GbPatientNumberMask"/>.
      /// </summary>
      public static GbPatientNumberMask Instance => _instance.Value;

      /// <inheritdoc/>
      public Boolean ExcludeCharacter(Int32 index)
         => index is SeparatorPosition1 or SeparatorPosition2;

      /// <inheritdoc/>
      public Boolean IncludeCharacter(Int32 index)
         => index is not SeparatorPosition1 and not SeparatorPosition2;
   }
}
