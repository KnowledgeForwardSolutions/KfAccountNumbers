using LocalCreateResult = KfAccountNumbers.Results.UCreateResult<
   KfAccountNumbers.Governmental.Europe.GbPatientNumber,
   KfAccountNumbers.Governmental.Europe.GbPatientNumber.ValidationError>;

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
         GbPatientNumberBase.GetAllValidLengthDefinitions());

      // Act/assert.
      FluentActions
         .Invoking(() => new GbPatientNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<GbPatientNumber.ValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
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
         GbPatientNumberBase.GetAllValidLengthDefinitions());

      // Act/assert.
      FluentActions
         .Invoking(() => new GbPatientNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<GbPatientNumber.ValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
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

   #region IdentifierType Property Tests
   // ==========================================================================
   // ==========================================================================

   public static TheoryData<String, GbPatientNumber.IdentifierCategory> IdentifierTypeTestData = new()
   {
      { ValidUnformattedChiNumber, default(GbHealthService.Chi) },
      { ValidUnformattedHcNumber, default(GbHealthService.Hc) },
      { ValidUnformattedNhsNumberBlock1, default(GbHealthService.Nhs) },
      { ValidUnformattedNhsNumberBlock2, default(GbHealthService.Nhs) },
      { ValidFormattedTestNumber, default(GbHealthService.Test) },
      { ValidFormattedChiNumber, default(GbHealthService.Chi) },
      { ValidUnformattedHcNumber, default(GbHealthService.Hc) },
      { ValidFormattedNhsNumberBlock1, default(GbHealthService.Nhs) },
      { ValidFormattedNhsNumberBlock2, default(GbHealthService.Nhs) },
      { ValidFormattedTestNumber, default(GbHealthService.Test) },
   };

   [Theory]
   [MemberData(nameof(IdentifierTypeTestData))]
   public void GbPatientNumber_IdentifierType_ShouldReturnExpectedIdentifierType(
      String value,
      GbPatientNumber.IdentifierCategory expected)
   {
      // Arrange.
      var sut = new GbPatientNumber(value);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbPatientNumber_Value_ShouldReturnValidIdentifier(String value)
   {
      // Arrange.
      var sut = new GbPatientNumber(value);
      var expected = GetRawValue(value);

      // Act/assert.
      sut.Value.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbPatientNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedNhsNumberBlock1;
      var sut = new GbPatientNumber(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void GbPatientNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedNhsNumberBlock1;
      var sut = new GbPatientNumber(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void GbPatientNumber_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      GbPatientNumber sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void GbPatientNumber_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      GbPatientNumber sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbPatientNumber_ExplicitCastToBeGbPatientNumber_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new GbPatientNumber(value);

      // Act.
      var sut = (GbPatientNumber)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbPatientNumber_ExplicitCastToBeGbPatientNumber_ShouldThrowValidationError_WhenValueIsEmpty(String value)
   {
      // Arrange.
      GbPatientNumber.ValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbPatientNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbPatientNumber_ExplicitCastToBeGbPatientNumber_ShouldThrowValidationError_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      GbPatientNumber.ValidationError expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.GetAllValidLengthDefinitions());

      // Act/assert.
      FluentActions
         .Invoking(() => (GbPatientNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<GbPatientNumber.ValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterData))]
   public void GbPatientNumber_ExplicitCastToBeGbPatientNumber_ShouldThrowValidationError_WhenValueHasNonDigitCharacter(
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
         .Invoking(() => (GbPatientNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void GbPatientNumber_ExplicitCastToBeGbPatientNumber_ShouldThrowValidationError_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      GbPatientNumber.ValidationError expected = new InvalidChecksum(
         Messages.GbPatientNumberInvalidCheckDigit,
         Algorithms.Modulus11Decimal.AlgorithmName);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbPatientNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbPatientNumber_ExplicitCastToBeGbPatientNumber_ShouldThrowValidationError_WhenValueHasInvalidSeparator(
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
         .Invoking(() => (GbPatientNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRangeValues))]
   public void GbPatientNumber_ExplicitCastToBeGbPatientNumber_ShouldThrowValidationError_WhenValueIsOutsideOfValidRanges(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbPatientNumber.ValidationError expected = new GbPatientNumberInvalidRange(Messages.GbPatientNumberInvalidRange);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbPatientNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthForRangeValues))]
   public void GbPatientNumber_ExplicitCastToBeGbPatientNumber_ShouldThrowValidationError_WhenLengthIsInvalidForRange(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbPatientNumber.ValidationError expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.GetAllValidLengthDefinitions());

      // Act/assert.
      FluentActions
         .Invoking(() => (GbPatientNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<GbPatientNumber.ValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidChiNumberDateOfBirthValues))]
   public void GbPatientNumber_ExplicitCastToBeGbPatientNumber_ShouldThrowValidationError_WhenValueHasInvalidDateOfBirth(
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
         .Invoking(() => (GbPatientNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbPatientNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidFormattedNhsNumberBlock1);
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock1);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void GbPatientNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidFormattedNhsNumberBlock1);
      var sut2 = new GbPatientNumber(ValidFormattedTestNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void GbPatientNumber_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 12 character versions for same person should still be equal.
      var sut1 = new GbPatientNumber(ValidUnformattedNhsNumberBlock1);
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock1);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void GbPatientNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidFormattedNhsNumberBlock1);
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock1.Replace(' ', '.'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void GbPatientNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidFormattedNhsNumberBlock1.Replace(' ', 'A'));
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock1.Replace(' ', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbPatientNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidUnformattedNhsNumberBlock1);
      var sut2 = new GbPatientNumber(ValidFormattedTestNumber);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void GbPatientNumber_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 12 character versions for same person should still be equal.
      var sut1 = new GbPatientNumber(ValidUnformattedNhsNumberBlock1);
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock1);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void GbPatientNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidFormattedNhsNumberBlock1);
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock1);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void GbPatientNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidFormattedNhsNumberBlock1);
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock1.Replace(' ', '.'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void GbPatientNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidFormattedNhsNumberBlock1.Replace(' ', 'A'));
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock1.Replace(' ', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbPatientNumber_Create_ShouldReturnNewInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new GbPatientNumber(value);

      // Act.
      var result = GbPatientNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbPatientNumber_Create_ShouldReturnEmptyValue_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (GbPatientNumber.ValidationError)default(EmptyValue);

      // Act.
      var result = GbPatientNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbPatientNumber_Create_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      GbPatientNumber.ValidationError expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.GetAllValidLengthDefinitions());

      // Act.
      var result = GbPatientNumber.Create(value);

      // Assert.
      result.TryGetValue(out GbPatientNumber.ValidationError error).Should().BeTrue();    // Necessary to get around some issued with FluentAssertions and nested types
      error.Value.Should().BeEquivalentTo(expected.Value);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterData))]
   public void GbPatientNumber_Create_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (GbPatientNumber.ValidationError)new InvalidCharacter(
         Messages.GbPatientNumberInvalidCharacter,
         value[position],
         position);

      // Act.
      var result = GbPatientNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void GbPatientNumber_Create_ShouldReturnInvalidChecksum_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalCreateResult expected = (GbPatientNumber.ValidationError)new InvalidChecksum(
         Messages.GbPatientNumberInvalidCheckDigit,
         Algorithms.Modulus11Decimal.AlgorithmName);

      // Act.
      var result = GbPatientNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbPatientNumber_Create_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (GbPatientNumber.ValidationError)new InvalidSeparator(
         Messages.GbPatientNumberInvalidSeparator,
         value[position],
         position);

      // Act.
      var result = GbPatientNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRangeValues))]
   public void GbPatientNumber_Create_ShouldReturnInvalidRange_WhenValueIsOutsideOfValidRanges(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      LocalCreateResult expected = (GbPatientNumber.ValidationError)new GbPatientNumberInvalidRange(
         Messages.GbPatientNumberInvalidRange);

      // Act.
      var result = GbPatientNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthForRangeValues))]
   public void GbPatientNumber_Create_ShouldReturnInvalidLength_WhenLengthIsInvalidForRange(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbPatientNumber.ValidationError expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.GetAllValidLengthDefinitions());

      // Act.
      var result = GbPatientNumber.Create(value);

      // Assert.
      result.TryGetValue(out GbPatientNumber.ValidationError error).Should().BeTrue();    // Necessary to get around some issued with FluentAssertions and nested types
      error.Value.Should().BeEquivalentTo(expected.Value);
   }

   [Theory]
   [MemberData(nameof(InvalidChiNumberDateOfBirthValues))]
   public void GbPatientNumber_Create_ShouldReturnInvalidRange_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String separator)
   {
      // Arrange.
      var value = GetChiNumberWithValidCheckDigit(dateOfBirth, separator: separator);
      var invalidDateOfBirth = value[..6];
      LocalCreateResult expected = (GbPatientNumber.ValidationError)new InvalidDateOfBirth(
         Messages.GbChiNumberInvalidDateOfBirth,
         invalidDateOfBirth,
         GbPatientNumberBase.ChiNumberDateFormat);

      // Act.
      var result = GbPatientNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbPatientNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidFormattedNhsNumberBlock2);
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock2);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void GbPatientNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(AltValidUnformattedNhsNumberBlock1);
      var sut2 = new GbPatientNumber(ValidUnformattedTestNumber);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void GbPatientNumber_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 12 character versions for same person should still be equal.
      var sut1 = new GbPatientNumber(ValidUnformattedNhsNumberBlock2);
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock2);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void GbPatientNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidFormattedNhsNumberBlock2);
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock2.Replace(' ', '.'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void GbPatientNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidFormattedNhsNumberBlock2.Replace(' ', 'A'));
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock2.Replace(' ', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidUnformattedChiNumber, ValidFormattedChiNumber)]
   [InlineData(ValidUnformattedHcNumber, ValidFormattedHcNumber)]
   [InlineData(ValidUnformattedNhsNumberBlock1, ValidFormattedNhsNumberBlock1)]
   [InlineData(ValidUnformattedTestNumber, ValidFormattedTestNumber)]
   public void GbPatientNumber_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed(
      String value,
      String expected)
   {
      // Arrange.
      var sut = new GbPatientNumber(value);

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void GbPatientNumber_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new GbPatientNumber(ValidUnformattedNhsNumberBlock1);
      var mask = "__________";
      var expected = ValidUnformattedNhsNumberBlock1;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Theory]
   [InlineData("")]
   [InlineData("\t")]
   public void GbPatientNumber_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new GbPatientNumber(ValidUnformattedNhsNumberBlock1);
      var expectedMessage = Messages.FormatMaskEmpty + "*";

      // Act/assert.
      FluentActions
         .Invoking(() => _ = sut.Format(mask))
         .Should()
         .ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(mask))
         .WithMessage(expectedMessage);
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbPatientNumber_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidUnformattedNhsNumberBlock2);
      var sut2 = new GbPatientNumber(ValidUnformattedNhsNumberBlock2);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void GbPatientNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidUnformattedNhsNumberBlock2);
      var sut2 = new GbPatientNumber(ValidUnformattedTestNumber);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void GbPatientNumber_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 12 character versions for same person should still be equal.
      var sut1 = new GbPatientNumber(ValidUnformattedNhsNumberBlock2);
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock2);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void GbPatientNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidFormattedNhsNumberBlock2);
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock2.Replace(' ', '.'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void GbPatientNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidFormattedNhsNumberBlock2.Replace(' ', 'A'));
      var sut2 = new GbPatientNumber(ValidFormattedNhsNumberBlock2.Replace(' ', 'a'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   #endregion

   #region ReferenceEquals Method Tests
   // ==========================================================================
   // ==========================================================================

   // GbPatientNumber does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void GbPatientNumber_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new GbPatientNumber(ValidUnformattedTestNumber);
      var sut2 = new GbPatientNumber(ValidUnformattedTestNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToGbChiNumber Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbPatientNumber_ToGbChiNumber_ShouldReturnExpectedResult_WhenValueIsChiNumber()
   {
      // Arrange.
      var sut = new GbPatientNumber(ValidUnformattedChiNumber);
      var expected = new GbChiNumber(ValidUnformattedChiNumber);

      // Act.
      KfOption<GbChiNumber> result = sut.ToGbChiNumber();

      // Assert.
      result.Value.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData(ValidUnformattedHcNumber)]
   [InlineData(ValidUnformattedNhsNumberBlock1)]
   [InlineData(ValidUnformattedTestNumber)]
   public void GbPatientNumber_ToGbChiNumber_ShouldReturnExpectedResult_WhenValueIsNotChiNumber(String value)
   {
      // Arrange.
      var sut = new GbPatientNumber(value);
      var expected = default(None);

      // Act.
      KfOption<GbChiNumber> result = sut.ToGbChiNumber();

      // Assert.
      result.Value.Should().Be(expected);
   }

   #endregion

   #region ToGbHcNumber Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbPatientNumber_ToGbHcNumber_ShouldReturnExpectedResult_WhenValueIsHcNumber()
   {
      // Arrange.
      var sut = new GbPatientNumber(ValidUnformattedHcNumber);
      var expected = new GbHcNumber(ValidUnformattedHcNumber);

      // Act.
      KfOption<GbHcNumber> result = sut.ToGbHcNumber();

      // Assert.
      result.Value.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData(ValidUnformattedChiNumber)]
   [InlineData(ValidUnformattedNhsNumberBlock1)]
   [InlineData(ValidUnformattedTestNumber)]
   public void GbPatientNumber_ToGbHcNumber_ShouldReturnExpectedResult_WhenValueIsNotHcNumber(String value)
   {
      // Arrange.
      var sut = new GbPatientNumber(value);
      var expected = default(None);

      // Act.
      KfOption<GbHcNumber> result = sut.ToGbHcNumber();

      // Assert.
      result.Value.Should().Be(expected);
   }

   #endregion

   #region ToGbNhsNumber Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbPatientNumber_ToGbNhsNumber_ShouldReturnExpectedResult_WhenValueIsNhsNumber()
   {
      // Arrange.
      var sut = new GbPatientNumber(ValidUnformattedNhsNumberBlock1);
      var expected = new GbNhsNumber(ValidUnformattedNhsNumberBlock1);

      // Act.
      KfOption<GbNhsNumber> result = sut.ToGbNhsNumber();

      // Assert.
      result.Value.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData(ValidUnformattedChiNumber)]
   [InlineData(ValidUnformattedHcNumber)]
   [InlineData(ValidUnformattedTestNumber)]
   public void GbPatientNumber_ToGbNhsNumber_ShouldReturnExpectedResult_WhenValueIsNotNhsNumber(String value)
   {
      // Arrange.
      var sut = new GbPatientNumber(value);
      var expected = default(None);

      // Act.
      KfOption<GbNhsNumber> result = sut.ToGbNhsNumber();

      // Assert.
      result.Value.Should().Be(expected);
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbPatientNumber_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new GbPatientNumber(value);
      var expected = GetRawValue(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
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
         GbPatientNumberBase.GetAllValidLengthDefinitions());

      // Act.
      var result = GbPatientNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<GbPatientNumber.ValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
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
         GbPatientNumberBase.GetAllValidLengthDefinitions());

      // Act.
      var result = GbPatientNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<GbPatientNumber.ValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
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

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbPatientNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new GbPatientNumber(ValidUnformattedNhsNumberBlock2);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<GbPatientNumber>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void GbPatientNumber_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new GbPatientNumber(AltValidFormattedNhsNumberBlock1);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public GbPatientNumber PatientNumber { get; set; } = null!;
   }

   [Fact]
   public void GbPatientNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { PatientNumber = new GbPatientNumber(AltValidFormattedTestNumber) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void GbPatientNumber_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"PatientNumber\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void GbPatientNumber_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"PatientNumber\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.PatientNumber.Should().BeNull();
   }

   [Fact]
   public void GbPatientNumber_JsonDeserialization_ShouldThrowKfValidationException_WhenValueIsInvalid()
   {
      // Arrange.
      var json = "{\"PatientNumber\":\"123-456-78901\"}";  // Invalid length
      GbPatientNumber.ValidationError expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         13,
         GbPatientNumberBase.GetAllValidLengthDefinitions());

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<UKfValidationException<GbPatientNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<GbPatientNumber.ValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   #endregion
}
