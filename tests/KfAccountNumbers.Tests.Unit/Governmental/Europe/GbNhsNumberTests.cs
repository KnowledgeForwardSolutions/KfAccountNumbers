namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class GbNhsNumberTests
{
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

   private static Char GetCheckDigit(String value)
   {
      var str = value.Length == GbPatientNumberBase.UnformattedLength - 1
         ? value
         : value[..3] + value[4..7] + value[8..];
      return Algorithms.Modulus11Decimal.TryCalculateCheckDigit(str, out var ch)
         ? ch
         : throw new InvalidOperationException("Can't generate valid check digit");
   }

   private static String GetRawValue(String value)
      => value.Length == GbPatientNumberBase.UnformattedLength
         ? value
         : $"{value[..3]}{value[4..7]}{value[8..]}";

   public static TheoryData<String> ValidValues =>
   [
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
      AltValidFormattedTestNumber
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "123456789",            // Length 9, too short
      "12345678901",          // Length 11, too long
      "123 456 789",          // Length 11, too long for unformatted, too short for formatted
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

      "400 090 0004",      // 4000000004 with single digit transcription error, 0 -> 9
      "611 199 9998",      // 6112999998 with single digit transcription error, 2 -> 1
      "744 105 0604",      // 7441005604 with two digit transposition error 05 -> 50
      "493 476 5919",      // 9434765919 with two digit transposition error 94 -> 49
      "494 768 7882",      // 4967487882 with jump transposition error 674 -> 476
      "943 476 1959",      // 9434765919 with jump transposition error 591 -> 195
      "751 556 8242",      // 7514468242 with twin error 44 -> 55
      "999 009 9980",      // 9999999980 with twin error 99 -> 00
   ];

   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      // First separator position
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

      // Second separator position
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

      // Mixed separators
      { "400 000-0004", 7 },
      { "400-000 0004", 7 },
   };

   public static TheoryData<String> InvalidRangeValues =>
   [
      // Values do not include check digit that must be calculated in the test

      // Values below CHI number block
      "000000001",
      "009999999",

      // CHI number block
      "311300000",
      "319999999",

      // HC number block
      "320000000",
      "399999999",

      // Gap in NHS numbers
      "500000000",
      "599999999",

      // Gap between NHS numbers and test numbers
      "800000000",
      "899999999",

      // Values below CHI number block
      "000 000 001",
      "009 999 999",

      // CHI number block
      "311 300 000",
      "319 999 999",

      // HC number block
      "320 000 000",
      "399 999 999",

      // Gap in NHS numbers
      "500 000 000",
      "599 999 999",

      // Gap between NHS numbers and test numbers
      "800 000 000",
      "899 999 999",
   ];

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbNhsNumber_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = new GbNhsNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbNhsNumber_Constructor_ShouldThrowValidationError_WhenValueIsEmpty(String value)
   {
      // Arrange.
      GbNhsNumber.ValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNhsNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbNhsNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbNhsNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      GbNhsNumber.ValidationError expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.ValidLengthDefinitions);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNhsNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbNhsNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterData))]
   public void GbNhsNumber_Constructor_ShouldThrowValidationError_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      GbNhsNumber.ValidationError expected = new InvalidCharacter(
         Messages.GbPatientNumberInvalidCharacter,
         value[position],
         position);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNhsNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbNhsNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void GbNhsNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      GbNhsNumber.ValidationError expected = new InvalidChecksum(
         Messages.GbPatientNumberInvalidCheckDigit,
         Algorithms.Modulus11Decimal.AlgorithmName);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNhsNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbNhsNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbNhsNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      GbNhsNumber.ValidationError expected = new InvalidSeparator(
         Messages.GbPatientNumberInvalidSeparator,
         value[position],
         position);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNhsNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbNhsNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRangeValues))]
   public void GbNhsNumber_Constructor_ShouldThrowValidationError_WhenValueIsOutsideOfValidRanges(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbNhsNumber.ValidationError expected = new GbPatientNumberInvalidRange(Messages.GbNhsNumberInvalidRange);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNhsNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbNhsNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region IdentifierType Property Tests
   // ==========================================================================
   // ==========================================================================

   public static TheoryData<String, GbNhsNumber.IdentifierCategory> IdentifierTypeTestData = new()
   {
      { ValidUnformattedNhsNumberBlock1, default(GbHealthService.Nhs) },
      { ValidUnformattedNhsNumberBlock2, default(GbHealthService.Nhs) },
      { ValidUnformattedTestNumber, default(GbHealthService.Test) },
      { ValidFormattedNhsNumberBlock1, default(GbHealthService.Nhs) },
      { ValidFormattedNhsNumberBlock2, default(GbHealthService.Nhs) },
      { ValidFormattedTestNumber, default(GbHealthService.Test) },
   };

   [Theory]
   [MemberData(nameof(IdentifierTypeTestData))]
   public void GbNhsNumber_IdentifierType_ShouldReturnExpectedIdentifierType(
      String value,
      GbNhsNumber.IdentifierCategory expected)
   {
      // Arrange.
      var sut = new GbNhsNumber(value);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbNhsNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedNhsNumberBlock1;
      var sut = new GbNhsNumber(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void GbNhsNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedNhsNumberBlock1;
      var sut = new GbNhsNumber(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void GbNhsNumber_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      GbNhsNumber sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void GbNhsNumber_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      GbNhsNumber sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbNhsNumber_ExplicitCastToBeGbNhsNumber_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new GbNhsNumber(value);

      // Act.
      var sut = (GbNhsNumber)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbNhsNumber_ExplicitCastToBeGbNhsNumber_ShouldThrowValidationError_WhenValueIsEmpty(String value)
   {
      // Arrange.
      GbNhsNumber.ValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbNhsNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbNhsNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbNhsNumber_ExplicitCastToBeGbNhsNumber_ShouldThrowValidationError_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      GbNhsNumber.ValidationError expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.ValidLengthDefinitions);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbNhsNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbNhsNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterData))]
   public void GbNhsNumber_ExplicitCastToBeGbNhsNumber_ShouldThrowValidationError_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      GbNhsNumber.ValidationError expected = new InvalidCharacter(
         Messages.GbPatientNumberInvalidCharacter,
         value[position],
         position);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbNhsNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbNhsNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void GbNhsNumber_ExplicitCastToBeGbNhsNumber_ShouldThrowValidationError_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      GbNhsNumber.ValidationError expected = new InvalidChecksum(
         Messages.GbPatientNumberInvalidCheckDigit,
         Algorithms.Modulus11Decimal.AlgorithmName);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbNhsNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbNhsNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbNhsNumber_ExplicitCastToBeGbNhsNumber_ShouldThrowValidationError_WhenValueHasInvalidCSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      GbNhsNumber.ValidationError expected = new InvalidSeparator(
         Messages.GbPatientNumberInvalidSeparator,
         value[position],
         position);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbNhsNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbNhsNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRangeValues))]
   public void GbNhsNumber_ExplicitCastToBeGbNhsNumber_ShouldThrowValidationError_WhenValueIsOutsideOfValidRanges(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbNhsNumber.ValidationError expected = new GbPatientNumberInvalidRange(Messages.GbNhsNumberInvalidRange);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbNhsNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbNhsNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbNhsNumber_Value_ShouldReturnValidIdentifier(String value)
   {
      // Arrange.
      var sut = new GbNhsNumber(value);
      var expected = GetRawValue(value);

      // Act/ssert.
      sut.Value.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbNhsNumber_Validate_ShouldReturnValidValue_WhenValueIsValid(String value)
   {
      // Arrange.
      GbNhsNumber.ValidationResult expected = default(ValidValue);

      // Act.
      var result = GbNhsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbNhsNumber_Validate_ShouldReturnEmptyValue_WhenValueIsEmpty(String value)
   {
      // Arrange.
      GbNhsNumber.ValidationResult expected = default(EmptyValue);

      // Act.
      var result = GbNhsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbNhsNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      GbNhsNumber.ValidationResult expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.ValidLengthDefinitions);

      // Act.
      var result = GbNhsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterData))]
   public void GbNhsNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      GbNhsNumber.ValidationResult expected = new InvalidCharacter(
         Messages.GbPatientNumberInvalidCharacter,
         value[position],
         position);

      // Act.
      var result = GbNhsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void GbNhsNumber_Validate_ShouldReturnInvalidChecksum_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      GbNhsNumber.ValidationResult expected = new InvalidChecksum(
         Messages.GbPatientNumberInvalidCheckDigit,
         Algorithms.Modulus11Decimal.AlgorithmName);

      // Act.
      var result = GbNhsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbNhsNumber_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidCSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      GbNhsNumber.ValidationResult expected = new InvalidSeparator(
         Messages.GbPatientNumberInvalidSeparator,
         value[position],
         position);

      // Act.
      var result = GbNhsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRangeValues))]
   public void GbNhsNumber_Validate_ShouldReturnInvalidRange_WhenValueIsOutsideOfValidRanges(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbNhsNumber.ValidationResult expected = new GbPatientNumberInvalidRange(Messages.GbNhsNumberInvalidRange);

      // Act.
      var result = GbNhsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion
}
