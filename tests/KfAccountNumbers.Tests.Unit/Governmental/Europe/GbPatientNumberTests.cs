namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class GbPatientNumberTests
{
   private const String ValidUnformattedChiNumber = "0101000006";
   private const String ValidFormattedChiNumber = "010100 0006";
   private const String AltValidUnformattedChiNumber = "3112999991";
   private const String AltValidFormattedChiNumber = "311299-9991";

   private const String ValidUnformattedHcNumber = "3200000007";
   private const String ValidFormattedHcNumber = "320 000 0007";
   private const String AltValidUnformattedHcNumber = "3999999993";
   private const String AltValidFormattedHcNumber = "399-999-9993";

   private const String ValidUnformattedNhsNumberBlock1 = "4000000004";
   private const String ValidFormattedNhsNumberBlock1 = "400 000 0004";
   private const String AltValidUnformattedNhsNumberBlock1 = "4999999994";
   private const String AltValidFormattedNhsNumberBlock1 = "499-999-9994";

   private const String ValidUnformattedNhsNumberBlock2 = "6000000006";
   private const String ValidFormattedNhsNumberBlock2 = "600 000 0006";
   private const String AltValidUnformattedNhsNumberBlock2 = "7999999997";
   private const String AltValidFormattedNhsNumberBlock2 = "799-999-9997";

   private const String ValidUnformattedTestNumber = "9000000009";
   private const String ValidFormattedTestNumber = "900 000 0009";
   private const String AltValidUnformattedTestNumber = "9999999980";
   private const String AltValidFormattedTestNumber = "999-999-9980";

   private const String UnformattedChiNumberModulus11CheckDigitZeroValue = "0101000200";     // Edge case, modulus 11 with remainder 0 should result in 0 check digit
   private const String FormattedChiNumberModulus11CheckDigitZeroValue = "010100 0200";

   private const String UnformattedHcNumberModulus11CheckDigitZeroValue = "3200000120";      // Edge case, modulus 11 with remainder 0 should result in 0 check digit
   private const String FormattedHcNumberModulus11CheckDigitZeroValue = "320 000 0120";

   private const String UnformattedNhsNumberModulus11CheckDigitZeroValue = "4000000020";     // Edge case, modulus 11 with remainder 0 should result in 0 check digit
   private const String FormattedNhsNumberModulus11CheckDigitZeroValue = "400 000 0020";

   private static Char GetCheckDigit(String value)
   {
      var str = value.Length switch
      {
         GbPatientNumberBase.UnformattedLength - 1 => value,
         GbPatientNumberBase.ChiFormattedLength - 1 => value[..6] + value[7..],
         GbPatientNumberBase.NhsFormattedLength - 1 => value[..3] + value[4..7] + value[8..],
      };
      return Algorithms.Modulus11Decimal.TryCalculateCheckDigit(str, out var ch)
         ? ch
         : throw new InvalidOperationException("Can't generate valid check digit");
   }

   private static String GetChiNumberWithValidCheckDigit(
      String dateOfBirth = "030988",
      Int32 random = 0,
      Char gender = '1',
      String separator = "")
   {
      for (var index = 0; index < 15; index++)
      {
         var str = $"{dateOfBirth}{random:D2}{gender}";
         if (Algorithms.Modulus11Decimal.TryCalculateCheckDigit(str, out var checkDigit))
         {
            return $"{str[..6]}{separator}{str[6..]}{checkDigit}";
         }

         random++;
      }

      throw new InvalidOperationException("Can't generate valid check digit");
   }

   private static String GetRawValue(String value)
      => value.Length switch
      {
         GbPatientNumberBase.UnformattedLength => value,
         GbPatientNumberBase.ChiFormattedLength => $"{value[..6]}{value[7..]}",
         GbPatientNumberBase.NhsFormattedLength => $"{value[..3]}{value[4..7]}{value[8..]}",
      };

   public static TheoryData<String> ValidValues =>
   [
      ValidUnformattedChiNumber,
      ValidFormattedChiNumber,
      AltValidUnformattedChiNumber,
      AltValidFormattedChiNumber,

      ValidUnformattedHcNumber,
      ValidFormattedHcNumber,
      AltValidUnformattedHcNumber,
      AltValidFormattedHcNumber,

      ValidUnformattedNhsNumberBlock1,
      ValidFormattedNhsNumberBlock1,
      AltValidUnformattedNhsNumberBlock1,
      AltValidFormattedNhsNumberBlock1,

      ValidUnformattedNhsNumberBlock2,
      ValidFormattedNhsNumberBlock2,
      AltValidUnformattedNhsNumberBlock2,
      AltValidFormattedNhsNumberBlock2,

      ValidUnformattedTestNumber,
      ValidFormattedTestNumber,
      AltValidUnformattedTestNumber,
      AltValidFormattedTestNumber,

      UnformattedChiNumberModulus11CheckDigitZeroValue,
      FormattedChiNumberModulus11CheckDigitZeroValue,

      UnformattedHcNumberModulus11CheckDigitZeroValue,
      FormattedHcNumberModulus11CheckDigitZeroValue,

      UnformattedNhsNumberModulus11CheckDigitZeroValue,
      FormattedNhsNumberModulus11CheckDigitZeroValue,
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "123456789",            // Length 9, too short
      "123-456-78901",        // Length 13, too long,
      new String('1', 100),   // Very long value
   ];

   public static TheoryData<String, Int32> InvalidCharacterData = new()
   {
      { ".000000004", 0 },          // Non-digit character '.'
      { "4 00000004", 1 },          // Non-digit character ' '
      { "40A0000004", 2 },          // Non-digit character 'A'
      { "400Z000004", 3 },          // Non-digit character 'Z'
      { "4000^00004", 4 },          // Non-digit character '^'
      { "40000a0004", 5 },          // Non-digit character 'a'
      { "400000z004", 6 },          // Non-digit character 'z'
      { "4000000~04", 7 },          // Non-digit character '~'
      { "40000000\u21534", 8 },     // Non-digit character Unicode fraction 1/3
      { "400000000\u00D6", 9 },     // Invalid character unicode O with umlaut
      // same, with single separator
      { ".10100 0006", 0 },         // Non-digit character '.'
      { "0 0100 0006", 1 },         // Non-digit character ' '
      { "01A100 0006", 2 },         // Non-digit character 'A'
      { "010Z00 0006", 3 },         // Non-digit character 'Z'
      { "0101^0 0006", 4 },         // Non-digit character '^'
      { "01010a-0006", 5 },         // Non-digit character 'a'
      { "010100-z006", 7 },         // Non-digit character 'z'
      { "010100-0~06", 8 },         // Non-digit character '~'
      { "010100-00\u21536", 9 },    // Non-digit character Unicode fraction 1/3
      { "010100-000\u00D6", 10 },   // Invalid character unicode O with umlaut
      // same, with double separator
      { ".00 000 0004", 0 },        // Non-digit character '.'
      { "4 0 000 0004", 1 },        // Non-digit character ' '
      { "40A 000 0004", 2 },        // Non-digit character 'A'
      { "400 Z00 0004", 4 },        // Non-digit character 'Z'
      { "400 0^0 0004", 5 },        // Non-digit character '^'
      { "400-00a-0004", 6 },        // Non-digit character 'a'
      { "400-000-z004", 8 },        // Non-digit character 'z'
      { "400-000-0~04", 9 },        // Non-digit character '~'
      { "400-000-00\u21534", 10 },  // Non-digit character Unicode fraction 1/3
      { "400-000-000\u00D6", 11 },  // Invalid character unicode O with umlaut
   };

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "4000900004",        // 4000000004 with single digit transcription error, 0 -> 9
      "6111999998",        // 6112999998 with single digit transcription error, 2 -> 1
      "7441050604",        // 7441005604 with two digit transposition error 05 -> 50
      "4934765919",        // 9434765919 with two digit transposition error 94 -> 49
      "4947687882",        // 4967487882 with jump transposition error 674 -> 476
      "9434761959",        // 9434765919 with jump transposition error 591 -> 195
      "7515568242",        // 7514468242 with twin error 44 -> 55
      "9990099980",        // 9999999980 with twin error 99 -> 00
      "4000000110",        // 4000000110 is invalid because check digit would be 10

      "400090 0004",       // 4000000004 with single digit transcription error, 0 -> 9
      "611199 9998",       // 6112999998 with single digit transcription error, 2 -> 1
      "744105 0604",       // 7441005604 with two digit transposition error 05 -> 50
      "493476 5919",       // 9434765919 with two digit transposition error 94 -> 49
      "494768 7882",       // 4967487882 with jump transposition error 674 -> 476
      "943476 1959",       // 9434765919 with jump transposition error 591 -> 195
      "751556 8242",       // 7514468242 with twin error 44 -> 55
      "999009 9980",       // 9999999980 with twin error 99 -> 00
      "400000 0110",       // 4000000110 is invalid because check digit would be 10

      "400 090 0004",      // 4000000004 with single digit transcription error, 0 -> 9
      "611 199 9998",      // 6112999998 with single digit transcription error, 2 -> 1
      "744 105 0604",      // 7441005604 with two digit transposition error 05 -> 50
      "493 476 5919",      // 9434765919 with two digit transposition error 94 -> 49
      "494 768 7882",      // 4967487882 with jump transposition error 674 -> 476
      "943 476 1959",      // 9434765919 with jump transposition error 591 -> 195
      "751 556 8242",      // 7514468242 with twin error 44 -> 55
      "999 009 9980",      // 9999999980 with twin error 99 -> 00
      "400 000 0110",      // 4000000110 is invalid because check digit would be 10
   ];

   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      // NHS format first separator position
      { "4000000 0004", 3 },
      { "4001000 0004", 3 },
      { "4002000 0004", 3 },
      { "4003000 0004", 3 },
      { "4004000 0004", 3 },
      { "4005000 0004", 3 },
      { "4006000 0004", 3 },
      { "4007000 0004", 3 },
      { "4008000 0004", 3 },
      { "4009000 0004", 3 },

      // NHS format second separator position
      { "400 00000004", 7 },
      { "400 00010004", 7 },
      { "400 00020004", 7 },
      { "400 00030004", 7 },
      { "400 00040004", 7 },
      { "400 00050004", 7 },
      { "400 00060004", 7 },
      { "400 00070004", 7 },
      { "400 00080004", 7 },
      { "400 00090004", 7 },

      // NHS format mixed separators
      { "400 000-0004", 7 },
      { "400-000 0004", 7 },

      // CHI format single separator
      { "40000000004", 6 },
      { "40000010004", 6 },
      { "40000020004", 6 },
      { "40000030004", 6 },
      { "40000040004", 6 },
      { "40000050004", 6 },
      { "40000060004", 6 },
      { "40000070004", 6 },
      { "40000080004", 6 },
      { "40000090004", 6 },
   };

   public static TheoryData<String> InvalidRangeValues =>
   [
      // Values do not include check digit that must be calculated in the test

      // Values below CHI number block
      "000000001",
      "009999999",

      // Gap between CHI number block and HC number block
      "311300000",
      "319999999",

      // Gap in NHS numbers
      "500000000",
      "599999999",

      // Gap between NHS numbers and test numbers
      "800000000",
      "899999999",

      // Values below CHI number block
      "000 000 001",
      "009 999 999",

      // Gap between CHI number block and HC number block
      "311 300 000",
      "319 999 999",

      // Gap in NHS numbers
      "500 000 000",
      "599 999 999",

      // Gap between NHS numbers and test numbers
      "800 000 000",
      "899 999 999",
   ];

   public static TheoryData<String> InvalidLengthForRangeValues =>
   [
      // CHI number block
      "010 000 000",
      "311 299 999",

      // HC number block
      "320000 000",
      "399999 999",

      // First NHS number block
      "400000 000",
      "499999 999",

      // Second NHS number block
      "600000 000",
      "799999 999",

      // Test numbers
      "900000 000",
      "999999 999",
   ];

   public static TheoryData<String, String> InvalidChiNumberDateOfBirthValues = new()
   {
      // Note - certain combinations are commented out because they will fail range
      // validation before reaching date of birth validation. They are left here to
      // show that those cases have been considered.

      // Unformatted
      { "110004", String.Empty },   // month = 0
      // { "311304", String.Empty },   // month = 13, will fail invalid range instead
      // { "000104", String.Empty },   // day = 0, will fail invalid range instead
      // { "320104", String.Empty },   // Invalid day of month for January, any year, will be considered a valid H & C number instead
      { "290201", String.Empty },   // Invalid day of for February, non-leap year
      { "300204", String.Empty },   // Invalid day of for February, leap year
      { "300200", String.Empty },   // Invalid day of for February, leap year (2000 is leap-year)
      // { "320304", String.Empty },   // Invalid day of for March, any year, will be considered a valid H & C number instead
      { "310404", String.Empty },   // Invalid day of for April, any year
      // { "320504", String.Empty },   // Invalid day of for May, any year, will be considered a valid H & C number instead
      { "310604", String.Empty },   // Invalid day of for June, any year
      // { "320704", String.Empty },   // Invalid day of for July, any year, will be considered a valid H & C number instead
      // { "320804", String.Empty },   // Invalid day of for August, any year, will be considered a valid H & C number instead
      { "310904", String.Empty },   // Invalid day of for September, any year
      // { "321004", String.Empty },   // Invalid day of for October, any year, will be considered a valid H & C number instead
      { "311104", String.Empty },   // Invalid day of for November, any year
      // { "321204", String.Empty },   // Invalid day of for December, any year, will be considered a valid H & C number instead

      // Formatted
      { "110004", " " },            // month = 0
      // { "311304", " " },            // month = 13, will fail invalid range instead
      // { "000104", " " },            // day = 0, will fail invalid range instead
      // { "320104", " " },            // Invalid day of month for January, any year, will be considered a valid H & C number instead
      { "290201", " " },            // Invalid day of for February, non-leap year
      { "300204", " " },            // Invalid day of for February, leap year
      { "300200", " " },            // Invalid day of for February, leap year (2000 is leap-year)
      // { "320304", " " },            // Invalid day of for March, any year, will be considered a valid H & C number instead
      { "310404", " " },            // Invalid day of for April, any year
      // { "320504", "-" },            // Invalid day of for May, any year, will be considered a valid H & C number instead
      { "310604", "-" },            // Invalid day of for June, any year
      // { "320704", "-" },            // Invalid day of for July, any year, will be considered a valid H & C number instead
      // { "320804", "-" },            // Invalid day of for August, any year, will be considered a valid H & C number instead
      { "310904", "-" },            // Invalid day of for September, any year
      // { "321004", "-" },            // Invalid day of for October, any year, will be considered a valid H & C number instead
      { "311104", "-" },            // Invalid day of for November, any year
      // { "321204", "-" },            // Invalid day of for December, any year, will be considered a valid H & C number instead
   };

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbPatientNumber_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = new GbPatientNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbPatientNumber_Constructor_ShouldThrowValidationError_WhenValueIsEmpty(String value)
   {
      // Arrange.
      GbPatientNumber.ValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbPatientNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbPatientNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      GbPatientNumber.ValidationError expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.GetGbPatientNumberValidLengthDefinitions());

      // Act/assert.
      FluentActions
         .Invoking(() => new GbPatientNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Value.Should().BeEquivalentTo(expected.Value);              // Issue with FluentAssertions resolving nested types, so compare Value to Value
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterData))]
   public void GbPatientNumber_Constructor_ShouldThrowValidationError_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      GbPatientNumber.ValidationError expected = new InvalidCharacter(
         Messages.GbPatientNumberInvalidCharacter,
         value[position],
         position);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbPatientNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void GbPatientNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      GbPatientNumber.ValidationError expected = new InvalidChecksum(
         Messages.GbPatientNumberInvalidCheckDigit,
         Algorithms.Modulus11Decimal.AlgorithmName);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbPatientNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbPatientNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      GbPatientNumber.ValidationError expected = new InvalidSeparator(
         Messages.GbPatientNumberInvalidSeparator,
         value[position],
         position);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbPatientNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRangeValues))]
   public void GbPatientNumber_Constructor_ShouldThrowValidationError_WhenValueIsOutsideOfValidRanges(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbPatientNumber.ValidationError expected = new GbPatientNumberInvalidRange(Messages.GbPatientNumberInvalidRange);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbPatientNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthForRangeValues))]
   public void GbPatientNumber_Constructor_ShouldThrowValidationError_WhenLengthIsInvalidForRange(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbPatientNumber.ValidationError expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.GetGbPatientNumberValidLengthDefinitions());

      // Act/assert.
      FluentActions
         .Invoking(() => new GbPatientNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Value.Should().BeEquivalentTo(expected.Value);              // Issue with FluentAssertions resolving nested types, so compare Value to Value
   }

   [Theory]
   [MemberData(nameof(InvalidChiNumberDateOfBirthValues))]
   public void GbPatientNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String separator)
   {
      // Arrange.
      var value = GetChiNumberWithValidCheckDigit(dateOfBirth, separator: separator);
      var invalidDateOfBirth = value[..6];
      GbPatientNumber.ValidationError expected = new InvalidDateOfBirth(
         Messages.GbChiNumberInvalidDateOfBirth,
         invalidDateOfBirth,
         GbPatientNumberBase.ChiNumberDateFormat);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbPatientNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbPatientNumber_Validate_ShouldReturnValidValue_WhenValueIsValid(String value)
   {
      // Arrange.
      GbPatientNumber.ValidationResult expected = default(ValidValue);

      // Act.
      var result = GbPatientNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbPatientNumber_Validate_ShouldReturnEmptyValue_WhenValueIsEmpty(String value)
   {
      // Arrange.
      GbPatientNumber.ValidationResult expected = default(EmptyValue);

      // Act.
      var result = GbPatientNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbPatientNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      GbPatientNumber.ValidationResult expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.GetGbPatientNumberValidLengthDefinitions());

      // Act.
      var result = GbPatientNumber.Validate(value);

      // Assert.
      result.Value.Should().BeEquivalentTo(expected.Value);       // Issue with FluentAssertions resolving nested types, so compare Value to Value
      // result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterData))]
   public void GbPatientNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      GbPatientNumber.ValidationResult expected = new InvalidCharacter(
         Messages.GbPatientNumberInvalidCharacter,
         value[position],
         position);

      // Act.
      var result = GbPatientNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void GbPatientNumber_Validate_ShouldReturnInvalidChecksum_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      GbPatientNumber.ValidationResult expected = new InvalidChecksum(
         Messages.GbPatientNumberInvalidCheckDigit,
         Algorithms.Modulus11Decimal.AlgorithmName);

      // Act.
      var result = GbPatientNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbPatientNumber_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      GbPatientNumber.ValidationResult expected = new InvalidSeparator(
         Messages.GbPatientNumberInvalidSeparator,
         value[position],
         position);

      // Act.
      var result = GbPatientNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRangeValues))]
   public void GbPatientNumber_Validate_ShouldReturnInvalidRange_WhenValueIsOutsideOfValidRanges(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbPatientNumber.ValidationResult expected = new GbPatientNumberInvalidRange(Messages.GbPatientNumberInvalidRange);

      // Act.
      var result = GbPatientNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthForRangeValues))]
   public void GbPatientNumber_Validate_ShouldReturnInvalidLength_WhenLengthIsInvalidForRange(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbPatientNumber.ValidationResult expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.GetGbPatientNumberValidLengthDefinitions());

      // Act.
      var result = GbPatientNumber.Validate(value);

      // Assert.
      result.Value.Should().BeEquivalentTo(expected.Value);       // Issue with FluentAssertions resolving nested types, so compare Value to Value
      // result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidChiNumberDateOfBirthValues))]
   public void GbPatientNumber_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String separator)
   {
      // Arrange.
      var value = GetChiNumberWithValidCheckDigit(dateOfBirth, separator: separator);
      var invalidDateOfBirth = value[..6];
      GbPatientNumber.ValidationResult expected = new InvalidDateOfBirth(
         Messages.GbChiNumberInvalidDateOfBirth,
         invalidDateOfBirth,
         GbPatientNumberBase.ChiNumberDateFormat);

      // Act.
      var result = GbPatientNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion
}
