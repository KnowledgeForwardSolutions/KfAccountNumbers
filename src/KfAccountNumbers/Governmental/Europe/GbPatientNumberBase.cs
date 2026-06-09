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
   ///   "DDMMYY NNNN" value.
   /// </summary>
   public const String DefaultChiFormatMask = "______ ____";

   /// <summary>
   ///   The default format mask that will render the patient number as a
   ///   "3 3 4" value.
   /// </summary>
   public const String DefaultNhsFormatMask = "___ ___ ____";

   /// <summary>
   ///   The length of a valid patient number that is not formatted for
   ///   readability.
   /// </summary>
   internal const Int32 UnformattedLength = 10;

   /// <summary>
   ///   The length of a valid patient number that is formatted for readability
   ///   using a 'DDMMYY NNNC' pattern.
   /// </summary>
   internal const Int32 ChiFormattedLength = 11;

   /// <summary>
   ///   The length of a valid patient number that is formatted for readability
   ///   using '3 3 4' pattern.
   /// </summary>
   internal const Int32 NhsFormattedLength = 12;

   /// <summary>
   ///   The zero-based offset of the separator character in a formatted CHI
   ///   number.
   /// </summary>
   internal const Int32 ChiSeparatorPosition = 6;

   /// <summary>
   ///   The zero-based offset of the first separator character in a formatted
   ///   NHS, H&amp;C or test patient number.
   /// </summary>
   internal const Int32 NhsSeparatorPosition1 = 3;

   /// <summary>
   ///   The zero-based offset of the second separator character in a formatted
   ///   NHS, H&amp;C or test patient number.
   /// </summary>
   internal const Int32 NhsSeparatorPosition2 = 7;

   /// <summary>
   ///   The zero-based offset for the digit that encodes gender for CHI numbers.
   /// </summary>
   internal const Int32 GenderOffset = 8;

   /// <summary>
   ///   Details for the length of a valid formatted CHI patient number.
   /// </summary>
   internal static readonly ValidLengthDefinition ChiLengthDefinition =
      new(ChiFormattedLength, Messages.GbPatientNumberSingleSeparatorFormattedLength);

   /// <summary>
   ///   Details for the length of a valid formatted NHS, H&amp;C or test
   ///   patient number.
   /// </summary>
   internal static readonly ValidLengthDefinition NhsLengthDefinition =
      new(NhsFormattedLength, Messages.GbPatientNumberDoubleSeparatorFormattedLength);

   /// <summary>
   ///   Details for the length of a valid unformatted GB patient number.
   /// </summary>
   internal static readonly ValidLengthDefinition UnformattedLengthDefinition =
      new(UnformattedLength, Messages.GbPatientNumberUnformattedLength);

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
   ///   Gets an array of details about valid lengths accepted for a GB patient
   ///   number.
   /// </summary>
   /// <returns>
   ///   An array of <see cref="ValidLengthDefinition"/>s.
   /// </returns>
   internal static ValidLengthDefinition[] GetAllValidLengthDefinitions() =>
   [
      UnformattedLengthDefinition,
      ChiLengthDefinition,
      NhsLengthDefinition,
   ];

   /// <summary>
   ///   Gets an array of details about valid lengths accepted for a CHI number.
   /// </summary>
   /// <returns>
   ///   An array of <see cref="ValidLengthDefinition"/>s.
   /// </returns>
   internal static ValidLengthDefinition[] GetChiValidLengthDefinitions() =>
   [
      UnformattedLengthDefinition,
      ChiLengthDefinition,
   ];

   /// <summary>
   ///   Gets an array of details about valid lengths accepted for a NHS number,
   ///   H&amp;C number or test number.
   /// </summary>
   /// <returns>
   ///   An array of <see cref="ValidLengthDefinition"/>s.
   /// </returns>
   internal static ValidLengthDefinition[] GetNhsValidLengthDefinitions() =>
   [
      UnformattedLengthDefinition,
      NhsLengthDefinition,
   ];

   /// <summary>
   ///   Extract the two-digit day, month and year elements from a formatted or
   ///   unformatted CHI number <paramref name="value"/>.
   /// </summary>
   /// <param name="value">
   ///   The CHI number containing the day, month and year values.
   /// </param>
   /// <returns>
   ///   A tuple containing the two-digit day, month and year elements contained
   ///   in the first six digits of the <paramref name="value"/>.
   /// </returns>
   /// <remarks>
   ///   We don't need to worry about possible formatting because the six digits
   ///   in CHI number date of birth all preceed the separator character.
   /// </remarks>
   protected static (Int32 Day, Int32 Month, Int32 Year) GetDayMonthYear(ReadOnlySpan<Char> value)
   {
      var day = value.ParseTwoDigits();
      var month = value[2..].ParseTwoDigits();
      var year = value[4..].ParseTwoDigits();

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
      var fourthDigitOffset = value.Length == NhsFormattedLength ? 4 : 3;
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
      => new(description, value[..6], ChiNumberDateFormat);

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
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
#pragma warning disable IDE0072 // Add missing cases
      => value.Length switch
      {
         UnformattedLength => value,
         ChiFormattedLength => String.Concat(value.AsSpan(..6), value.AsSpan(7..)),
         NhsFormattedLength => String.Concat(value.AsSpan(..3), value.AsSpan(4..7), value.AsSpan(8..)),
      };
#pragma warning restore IDE0072 // Add missing cases
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

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
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
#pragma warning disable IDE0072 // Add missing cases
      var validCheckDigit = value.Length switch
      {
         UnformattedLength => Algorithms.Modulus11Decimal.Validate(value),
         ChiFormattedLength => MaskedAlgorithms.Modulus11Decimal.Validate(value, ChiNumberMask.Instance),
         NhsFormattedLength => MaskedAlgorithms.Modulus11Decimal.Validate(value, NhsNumberMask.Instance),
      };
#pragma warning restore IDE0072 // Add missing cases
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

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
      if (value.Length == UnformattedLength)
      {
         return true;
      }

      if (value.Length == ChiFormattedLength)
      {
         invalidSeparatorPosition = ChiSeparatorPosition;
         return !Char.IsAsciiDigit(value[ChiSeparatorPosition]);
      }

      var separator1 = value[NhsSeparatorPosition1];
      if (Char.IsAsciiDigit(separator1))
      {
         invalidSeparatorPosition = NhsSeparatorPosition1;
         return false;
      }
      else if (value[NhsSeparatorPosition2] != separator1)
      {
         invalidSeparatorPosition = NhsSeparatorPosition2;
         return false;
      }

      return true;
   }

   // Return the zero-based index of the first non-digit character (excluding
   // separators) or -1 if no non-digit characters found.
   private static Int32 LocateInvalidCharacter(ReadOnlySpan<Char> value)
   {
      for (var index = 0; index < value.Length; index++)
      {
         if ((value.Length == ChiFormattedLength && index == ChiSeparatorPosition)
            || (value.Length == NhsFormattedLength && index is NhsSeparatorPosition1 or NhsSeparatorPosition2))
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
   ///   check digit of a formatted CHI number.
   /// </summary>
   protected class ChiNumberMask : ICheckDigitMask
   {
      private static readonly Lazy<ChiNumberMask> _instance =
         new(() => new ChiNumberMask());

      /// <summary>
      ///   Gets the only instance of <see cref="ChiNumberMask"/>.
      /// </summary>
      public static ChiNumberMask Instance => _instance.Value;

      /// <inheritdoc/>
      public Boolean ExcludeCharacter(Int32 index)
         => index == ChiSeparatorPosition;

      /// <inheritdoc/>
      public Boolean IncludeCharacter(Int32 index)
         => index != ChiSeparatorPosition;
   }

   /// <summary>
   ///   <see cref="ICheckDigitMask"/> used when validating the
   ///   check digit of a formatted NHS, H&amp;C or test patient number.
   /// </summary>
   protected class NhsNumberMask : ICheckDigitMask
   {
      private static readonly Lazy<NhsNumberMask> _instance =
         new(() => new NhsNumberMask());

      /// <summary>
      ///   Gets the only instance of <see cref="NhsNumberMask"/>.
      /// </summary>
      public static NhsNumberMask Instance => _instance.Value;

      /// <inheritdoc/>
      public Boolean ExcludeCharacter(Int32 index)
         => index is NhsSeparatorPosition1 or NhsSeparatorPosition2;

      /// <inheritdoc/>
      public Boolean IncludeCharacter(Int32 index)
         => index is not NhsSeparatorPosition1 and not NhsSeparatorPosition2;
   }
}
