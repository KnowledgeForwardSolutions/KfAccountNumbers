using LocalCreateResult = KfAccountNumbers.Results.UCreateResult<
   KfAccountNumbers.Governmental.Europe.GbHcNumber,
   KfAccountNumbers.Governmental.Europe.GbHcNumber.ValidationError>;

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class GbHcNumberTests
{
   private const String ValidUnformattedHcNumber = "3200000007";
   private const String ValidFormattedHcNumber = "320 000 0007";
   private const String AltValidUnformattedHcNumber = "3999999993";
   private const String AltValidFormattedHcNumber = "399-999-9993";

   private const String ValidUnformattedTestNumber = "9000000009";
   private const String ValidFormattedTestNumber = "900 000 0009";
   private const String AltValidUnformattedTestNumber = "9999999980";
   private const String AltValidFormattedTestNumber = "999-999-9980";

   private const String UnformattedModulus11CheckDigitZeroValue = "3200000120";     // Edge case, modulus 11 with remainder 0 should result in 0 check digit
   private const String FormattedModulus11CheckDigitZeroValue = "320 000 0120";

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
      ValidUnformattedHcNumber,
      ValidFormattedHcNumber,
      AltValidUnformattedHcNumber,
      AltValidFormattedHcNumber,

      ValidUnformattedTestNumber,
      ValidFormattedTestNumber,
      AltValidUnformattedTestNumber,
      AltValidFormattedTestNumber,

      UnformattedModulus11CheckDigitZeroValue,
      FormattedModulus11CheckDigitZeroValue,
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
      { ".200000007", 0 },          // Non-digit character '.'
      { "3 00000007", 1 },          // Non-digit character ' '
      { "32A0000007", 2 },          // Non-digit character 'A'
      { "320Z000007", 3 },          // Non-digit character 'Z'
      { "3200^00007", 4 },          // Non-digit character '^'
      { "32000a0007", 5 },          // Non-digit character 'a'
      { "320000z007", 6 },          // Non-digit character 'z'
      { "3200000~07", 7 },          // Non-digit character '~'
      { "32000000\u21537", 8 },     // Non-digit character Unicode fraction 1/3
      { "320000000\u00D6", 9 },     // Invalid character unicode O with umlaut

      { ".20 000 0007", 0 },        // Non-digit character '.'
      { "3 0 000 0007", 1 },        // Non-digit character ' '
      { "32A 000 0007", 2 },        // Non-digit character 'A'
      { "320 Z00 0007", 4 },        // Non-digit character 'Z'
      { "320 0^0 0007", 5 },        // Non-digit character '^'
      { "320-00a-0007", 6 },        // Non-digit character 'a'
      { "320-000-z007", 8 },        // Non-digit character 'z'
      { "320-000-0~07", 9 },        // Non-digit character '~'
      { "320-000-00\u21537", 10 },  // Non-digit character Unicode fraction 1/3
      { "320-000-000\u00D6", 11 },  // Invalid character unicode O with umlaut
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
      // First separator position
      { "3200000 0007", 3 },
      { "3201000 0007", 3 },
      { "3202000 0007", 3 },
      { "3203000 0007", 3 },
      { "3204000 0007", 3 },
      { "3205000 0007", 3 },
      { "3206000 0007", 3 },
      { "3207000 0007", 3 },
      { "3208000 0007", 3 },
      { "3209000 0007", 3 },

      // Second separator position
      { "320 00000007", 7 },
      { "320 00010007", 7 },
      { "320 00020007", 7 },
      { "320 00030007", 7 },
      { "320 00040007", 7 },
      { "320 00050007", 7 },
      { "320 00060007", 7 },
      { "320 00070007", 7 },
      { "320 00080007", 7 },
      { "320 00090007", 7 },

      // Mixed separators
      { "320 000-0007", 7 },
      { "320-000 0007", 7 },
   };

   public static TheoryData<String> InvalidRangeValues =>
   [
      // Values do not include check digit that must be calculated in the test

      // Values below CHI number block
      "000000001",
      "009999999",

      // CHI number block
      "010000000",
      "311299999",

      // Gap between CHI number block and HC number block
      "311300000",
      "319999999",

      // First NHS number block
      "400000000",
      "499999999",

      // Gap in NHS numbers
      "500000000",
      "599999999",

      // Second NHS number block
      "600000000",
      "799999999",

      // Gap between NHS numbers and test numbers
      "800000000",
      "899999999",

      // Values below CHI number block
      "000 000 001",
      "009 999 999",

      // CHI number block
      "010 000 000",
      "311 299 999",

      // Gap between CHI number block and HC number block
      "311 300 000",
      "319 999 999",

      // First NHS number block
      "400 000 000",
      "499 999 999",

      // Gap in NHS numbers
      "500 000 000",

      // Second NHS number block
      "600 000 000",
      "799 999 999",

      // Gap between NHS numbers and test numbers
      "800 000 000",
      "899 999 999",
   ];

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbHcNumber_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = new GbHcNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbHcNumber_Constructor_ShouldThrowValidationError_WhenValueIsEmpty(String value)
   {
      // Arrange.
      GbHcNumber.ValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbHcNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbHcNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbHcNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      GbHcNumber.ValidationError expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.ValidLengthDefinitions);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbHcNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbHcNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterData))]
   public void GbHcNumber_Constructor_ShouldThrowValidationError_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      GbHcNumber.ValidationError expected = new InvalidCharacter(
         Messages.GbPatientNumberInvalidCharacter,
         value[position],
         position);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbHcNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbHcNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void GbHcNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      GbHcNumber.ValidationError expected = new InvalidChecksum(
         Messages.GbPatientNumberInvalidCheckDigit,
         Algorithms.Modulus11Decimal.AlgorithmName);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbHcNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbHcNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbHcNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      GbHcNumber.ValidationError expected = new InvalidSeparator(
         Messages.GbPatientNumberInvalidSeparator,
         value[position],
         position);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbHcNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbHcNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRangeValues))]
   public void GbHcNumber_Constructor_ShouldThrowValidationError_WhenValueIsOutsideOfValidRanges(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbHcNumber.ValidationError expected = new GbPatientNumberInvalidRange(Messages.GbHcNumberInvalidRange);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbHcNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbHcNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbHcNumber_Validate_ShouldReturnValidValue_WhenValueIsValid(String value)
   {
      // Arrange.
      GbHcNumber.ValidationResult expected = default(ValidValue);

      // Act.
      var result = GbHcNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbHcNumber_Validate_ShouldReturnEmptyValue_WhenValueIsEmpty(String value)
   {
      // Arrange.
      GbHcNumber.ValidationResult expected = default(EmptyValue);

      // Act.
      var result = GbHcNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbHcNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      GbHcNumber.ValidationResult expected = new InvalidLength(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.ValidLengthDefinitions);

      // Act.
      var result = GbHcNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterData))]
   public void GbHcNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      GbHcNumber.ValidationResult expected = new InvalidCharacter(
         Messages.GbPatientNumberInvalidCharacter,
         value[position],
         position);

      // Act.
      var result = GbHcNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void GbHcNumber_Validate_ShouldReturnInvalidChecksum_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      GbHcNumber.ValidationResult expected = new InvalidChecksum(
         Messages.GbPatientNumberInvalidCheckDigit,
         Algorithms.Modulus11Decimal.AlgorithmName);

      // Act.
      var result = GbHcNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbHcNumber_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      GbHcNumber.ValidationResult expected = new InvalidSeparator(
         Messages.GbPatientNumberInvalidSeparator,
         value[position],
         position);

      // Act.
      var result = GbHcNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRangeValues))]
   public void GbHcNumber_Validate_ShouldReturnInvalidRange_WhenValueIsOutsideOfValidRanges(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbHcNumber.ValidationResult expected = new GbPatientNumberInvalidRange(Messages.GbHcNumberInvalidRange);

      // Act.
      var result = GbHcNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion
}
