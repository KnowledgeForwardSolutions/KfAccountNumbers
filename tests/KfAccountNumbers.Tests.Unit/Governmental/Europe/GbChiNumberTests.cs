using LocalCreateResult = KfAccountNumbers.Results.UCreateResult<
   KfAccountNumbers.Governmental.Europe.GbChiNumber,
   KfAccountNumbers.Governmental.Europe.GbChiNumber.ValidationError>;

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class GbChiNumberTests
{
   private const String ValidUnformattedChiNumber = "0101000006";
   private const String ValidFormattedChiNumber = "010100 0006";
   private const String AltValidUnformattedChiNumber = "3112999991";
   private const String AltValidFormattedChiNumber = "311299-9991";

   private const String UnformattedModulus11CheckDigitZeroValue = "0101000200";     // Edge case, modulus 11 with remainder 0 should result in 0 check digit
   private const String FormattedModulus11CheckDigitZeroValue = "010100 0200";

   private static Char GetCheckDigit(String value)
   {
      var str = value.Length == GbPatientNumberBase.UnformattedLength - 1
         ? value
         : value[..6] + value[7..];
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
      => value.Length == GbPatientNumberBase.UnformattedLength
         ? value
         : $"{value[..6]}{value[7..]}";

   public static TheoryData<String> ValidValues =>
   [
      ValidUnformattedChiNumber,
      ValidFormattedChiNumber,
      AltValidUnformattedChiNumber,
      AltValidFormattedChiNumber,

      UnformattedModulus11CheckDigitZeroValue,
      FormattedModulus11CheckDigitZeroValue,
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "123456789",            // Length 9, too short
      "123456 78901",         // Length 12, too long
      "123456 789012",        // Length 13, too long,
      new String('1', 100),   // Very long value
   ];

   public static TheoryData<String, Int32> InvalidCharacterData = new()
   {
      // Unformatted.
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

      // Formatted.
      { ".00000 0004", 0 },         // Non-digit character '.'
      { "4 0000 0004", 1 },         // Non-digit character ' '
      { "40A000 0004", 2 },         // Non-digit character 'A'
      { "400Z00 0004", 3 },         // Non-digit character 'Z'
      { "4000^0 0004", 4 },         // Non-digit character '^'
      { "40000a-0004", 5 },         // Non-digit character 'a'
      { "400000-z004", 7 },         // Non-digit character 'z'
      { "400000-0~04", 8 },         // Non-digit character '~'
      { "400000-00\u21534", 9 },    // Non-digit character Unicode fraction 1/3
      { "400000-000\u00D6", 10 },   // Invalid character unicode O with umlaut
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
   ];

   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      { "01010000006", 6 },
      { "01010010006", 6 },
      { "01010020006", 6 },
      { "01010030006", 6 },
      { "01010040006", 6 },
      { "01010050006", 6 },
      { "01010060006", 6 },
      { "01010070006", 6 },
      { "01010080006", 6 },
      { "01010090006", 6 },
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

      // HC number block
      "320000000",
      "399999999",

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

      // Test numbers
      "900000000",
      "999999999",

      // Values below CHI number block
      "000000 001",
      "009999 999",

      // Gap between CHI number block and HC number block
      "311300 000",
      "319999 999",

      // HC number block
      "320000 000",
      "399999 999",

      // First NHS number block
      "400000 000",
      "499999 999",

      // Gap in NHS numbers
      "500000 000",
      "599999 999",

      // Second NHS number block
      "600000 000",
      "799999 999",

      // Gap between NHS numbers and test numbers
      "800000 000",
      "899999 999",

      // Test numbers
      "900000 000",
      "999999 999",
   ];

   public static TheoryData<String, String> InvalidChiNumberDateOfBirthValues = new()
   {
      // Note - certain combinations are commented out because they will fail CHI number range
      // validation before reaching date of birth validation. They are left here to show that
      // those cases have been considered.

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

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.GbPatientNumberInvalidLength,
         value.Length,
         GbPatientNumberBase.GetChiValidLengthDefinitions());

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.GbPatientNumberInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.GbPatientNumberInvalidCheckDigit,
         Algorithms.Modulus11Decimal.AlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(
         Messages.GbPatientNumberInvalidSeparator,
         value[position],
         position);

   private static GbPatientNumberInvalidRange GetInvalidRangeResult()
      => new(Messages.GbChiNumberInvalidRange);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(Messages.GbChiNumberInvalidDateOfBirth, value[..6], DateFormatName.DDMMYY);

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbChiNumber_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = new GbChiNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbChiNumber_Constructor_ShouldThrowValidationError_WhenValueIsEmpty(String value)
   {
      // Arrange.
      GbChiNumber.ValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbChiNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbChiNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      GbChiNumber.ValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbChiNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<GbChiNumber.ValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterData))]
   public void GbChiNumber_Constructor_ShouldThrowValidationError_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      GbChiNumber.ValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbChiNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void GbChiNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      GbChiNumber.ValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new GbChiNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbChiNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      GbChiNumber.ValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbChiNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRangeValues))]
   public void GbChiNumber_Constructor_ShouldThrowValidationError_WhenValueIsOutsideOfValidRanges(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbChiNumber.ValidationError expected = GetInvalidRangeResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new GbChiNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidChiNumberDateOfBirthValues))]
   public void GbChiNumber_Constructor_ShouldThrowValidationError_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String separator)
   {
      // Arrange.
      var value = GetChiNumberWithValidCheckDigit(dateOfBirth, separator: separator);
      GbChiNumber.ValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbChiNumber(value))
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Gender Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData('1')]
   [InlineData('3')]
   [InlineData('5')]
   [InlineData('7')]
   [InlineData('9')]
   public void GbChiNumber_Gender_ShouldReturnMale_ForValuesWithOddGenderIndicator(Char gender)
   {
      // Arrange.
      var value = GetChiNumberWithValidCheckDigit(gender: gender);
      var sut = new GbChiNumber(value);
      Gender.BinaryGender expected = default(Gender.Male);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData('0')]
   [InlineData('2')]
   [InlineData('4')]
   [InlineData('6')]
   [InlineData('8')]
   public void GbChiNumber_Gender_ShouldReturnFemale_ForValuesWithEvenGenderIndicator(Char gender)
   {
      // Arrange.
      var value = GetChiNumberWithValidCheckDigit(gender: gender);
      var sut = new GbChiNumber(value);
      Gender.BinaryGender expected = default(Gender.Female);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbChiNumber_Value_ShouldReturnValidIdentifier(String value)
   {
      // Arrange.
      var sut = new GbChiNumber(value);
      var expected = GetRawValue(value);

      // Act/assert.
      sut.Value.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbChiNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidUnformattedChiNumber;
      var sut = new GbChiNumber(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void GbChiNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = AltValidFormattedChiNumber;
      var sut = new GbChiNumber(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void GbChiNumber_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      GbChiNumber sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void GbChiNumber_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      GbChiNumber sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbChiNumber_ExplicitCastToBeGbChiNumber_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new GbChiNumber(value);

      // Act.
      var sut = (GbChiNumber)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbChiNumber_ExplicitCastToBeGbChiNumber_ShouldThrowValidationError_WhenValueIsEmpty(String value)
   {
      // Arrange.
      GbChiNumber.ValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbChiNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbChiNumber_ExplicitCastToBeGbChiNumber_ShouldThrowValidationError_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      GbChiNumber.ValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbChiNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<GbChiNumber.ValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterData))]
   public void GbChiNumber_ExplicitCastToBeGbChiNumber_ShouldThrowValidationError_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      GbChiNumber.ValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbChiNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void GbChiNumber_ExplicitCastToBeGbChiNumber_ShouldThrowValidationError_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      GbChiNumber.ValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => (GbChiNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbChiNumber_ExplicitCastToBeGbChiNumber_ShouldThrowValidationError_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      GbChiNumber.ValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbChiNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRangeValues))]
   public void GbChiNumber_ExplicitCastToBeGbChiNumber_ShouldThrowValidationError_WhenValueIsOutsideOfValidRanges(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbChiNumber.ValidationError expected = GetInvalidRangeResult();

      // Act/assert.
      FluentActions
         .Invoking(() => (GbChiNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidChiNumberDateOfBirthValues))]
   public void GbChiNumber_ExplicitCastToBeGbChiNumber_ShouldThrowValidationError_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String separator)
   {
      // Arrange.
      var value = GetChiNumberWithValidCheckDigit(dateOfBirth, separator: separator);
      GbChiNumber.ValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (GbChiNumber)value)
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbChiNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidUnformattedChiNumber);
      var sut2 = new GbChiNumber(ValidUnformattedChiNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void GbChiNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidUnformattedChiNumber);
      var sut2 = new GbChiNumber(AltValidUnformattedChiNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void GbChiNumber_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 12 character versions for same person should still be equal.
      var sut1 = new GbChiNumber(ValidUnformattedChiNumber);
      var sut2 = new GbChiNumber(ValidFormattedChiNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void GbChiNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidFormattedChiNumber);
      var sut2 = new GbChiNumber(ValidFormattedChiNumber.Replace(' ', '.'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void GbChiNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidFormattedChiNumber.Replace(' ', 'A'));
      var sut2 = new GbChiNumber(ValidFormattedChiNumber.Replace(' ', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbChiNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidUnformattedChiNumber);
      var sut2 = new GbChiNumber(AltValidUnformattedChiNumber);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void GbChiNumber_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 12 character versions for same person should still be equal.
      var sut1 = new GbChiNumber(ValidUnformattedChiNumber);
      var sut2 = new GbChiNumber(ValidFormattedChiNumber);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void GbChiNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidUnformattedChiNumber);
      var sut2 = new GbChiNumber(ValidUnformattedChiNumber);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void GbChiNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidFormattedChiNumber);
      var sut2 = new GbChiNumber(ValidFormattedChiNumber.Replace(' ', '.'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void GbChiNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidFormattedChiNumber.Replace(' ', 'A'));
      var sut2 = new GbChiNumber(ValidFormattedChiNumber.Replace(' ', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbChiNumber_Create_ShouldReturnNewInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new GbChiNumber(value);

      // Act.
      var result = GbChiNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbChiNumber_Create_ShouldReturnEmptyValue_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (GbChiNumber.ValidationError)default(EmptyValue);

      // Act.
      var result = GbChiNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbChiNumber_Create_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (GbChiNumber.ValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = GbChiNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<GbChiNumber.ValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterData))]
   public void GbChiNumber_Create_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (GbChiNumber.ValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = GbChiNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void GbChiNumber_Create_ShouldReturnInvalidChecksum_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalCreateResult expected = (GbChiNumber.ValidationError)GetInvalidChecksumResult();

      // Act.
      var result = GbChiNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbChiNumber_Create_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (GbChiNumber.ValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = GbChiNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRangeValues))]
   public void GbChiNumber_Create_ShouldReturnInvalidRange_WhenValueIsOutsideOfValidRanges(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      LocalCreateResult expected = (GbChiNumber.ValidationError)GetInvalidRangeResult();

      // Act.
      var result = GbChiNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidChiNumberDateOfBirthValues))]
   public void GbChiNumber_Create_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String separator)
   {
      // Arrange.
      var value = GetChiNumberWithValidCheckDigit(dateOfBirth, separator: separator);
      LocalCreateResult expected = (GbChiNumber.ValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = GbChiNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbChiNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidFormattedChiNumber);
      var sut2 = new GbChiNumber(ValidFormattedChiNumber);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void GbChiNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new GbChiNumber(AltValidUnformattedChiNumber);
      var sut2 = new GbChiNumber(ValidUnformattedChiNumber);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void GbChiNumber_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 12 character versions for same person should still be equal.
      var sut1 = new GbChiNumber(ValidUnformattedChiNumber);
      var sut2 = new GbChiNumber(ValidFormattedChiNumber);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void GbChiNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidFormattedChiNumber);
      var sut2 = new GbChiNumber(ValidFormattedChiNumber.Replace(' ', '.'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void GbChiNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidFormattedChiNumber.Replace(' ', 'A'));
      var sut2 = new GbChiNumber(ValidFormattedChiNumber.Replace(' ', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void GbChiNumber_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new GbChiNumber(ValidFormattedChiNumber);

      // Act/assert.
      sut.Equals(ValidFormattedChiNumber).Should().BeFalse();
   }

   [Fact]
   public void GbChiNumber_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new GbChiNumber(ValidFormattedChiNumber);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbChiNumber_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new GbChiNumber(ValidUnformattedChiNumber);
      var expected = ValidFormattedChiNumber;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void GbChiNumber_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new GbChiNumber(ValidUnformattedChiNumber);
      var mask = "__________";
      var expected = ValidUnformattedChiNumber;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void GbChiNumber_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new GbChiNumber(ValidUnformattedChiNumber);
      String mask = null!;

      // Act/assert.
      FluentActions
         .Invoking(() => _ = sut.Format(mask))
         .Should()
         .ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(mask))
         .WithMessage(Messages.FormatMaskEmpty + "*");
   }

   [Theory]
   [InlineData("")]
   [InlineData("\t")]
   public void GbChiNumber_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new GbChiNumber(ValidUnformattedChiNumber);
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

   #region GetDateOfBirth Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("010100", "2000/01/01")]
   [InlineData("311299", "1999/12/31")]
   [InlineData("290200", "2000/02/29")]
   [InlineData("010150", "1950/01/01")]
   [InlineData("311249", "2049/12/31")]
   public void GbChiNumber_GetDateOfBirth_ShouldReturnExpectedValue_WhenCenturyCutoffIsDefault(
      String dateOfBirth,
      String expectedStringDob)
   {
      // Arrange.
      var value = GetChiNumberWithValidCheckDigit(dateOfBirth);
      var sut = new GbChiNumber(value);
      var expected = DateOnly.ParseExact(expectedStringDob, "yyyy/MM/dd", CultureInfo.InvariantCulture);

      // Act/assert.
      sut.GetDateOfBirth().Should().Be(expected);
   }

   [Theory]
   [InlineData("010100",  30, "2000/01/01")]
   [InlineData("311299",  30, "1999/12/31")]
   [InlineData("290200",  30, "2000/02/29")]
   [InlineData("010130",  30, "1930/01/01")]
   [InlineData("311229",  30, "2029/12/31")]
   public void GbChiNumber_GetDateOfBirth_ShouldReturnExpectedValue_WhenCenturyCutoffIsSupplied(
      String dateOfBirth,
      Int32 centuryCutoff,
      String expectedStringDob)
   {
      // Arrange.
      var value = GetChiNumberWithValidCheckDigit(dateOfBirth);
      var sut = new GbChiNumber(value);
      var expected = DateOnly.ParseExact(expectedStringDob, "yyyy/MM/dd", CultureInfo.InvariantCulture);

      // Act/assert.
      sut.GetDateOfBirth((CenturyCutoff)centuryCutoff).Should().Be(expected);
   }

   [Theory]
   [InlineData("010101", "1901/01/01")]
   [InlineData("311200", "2000/12/31")]
   public void GbChiNumber_GetDateOfBirth_ShouldReturnExpectedValue_WhenCenturyCutoffIsMinimumValidValue(
      String dateOfBirth,
      String expectedStringDob)
   {
      // Arrange.
      var value = GetChiNumberWithValidCheckDigit(dateOfBirth);
      var sut = new GbChiNumber(value);
      var centuryCutoff = 1;
      var expected = DateOnly.ParseExact(expectedStringDob, "yyyy/MM/dd", CultureInfo.InvariantCulture);

      // Act/assert.
      sut.GetDateOfBirth((CenturyCutoff)centuryCutoff).Should().Be(expected);
   }

   [Theory]
   [InlineData("311299", "2099/12/31")]
   public void GbChiNumber_GetDateOfBirth_ShouldReturnExpectedValue_WhenCenturyCutoffIsMaximumValidValue(
      String dateOfBirth,
      String expectedStringDob)
   {
      // Arrange.
      var value = GetChiNumberWithValidCheckDigit(dateOfBirth);
      var sut = new GbChiNumber(value);
      var centuryCutoff = 100;
      var expected = DateOnly.ParseExact(expectedStringDob, "yyyy/MM/dd", CultureInfo.InvariantCulture);

      // Act/assert.
      sut.GetDateOfBirth((CenturyCutoff)centuryCutoff).Should().Be(expected);
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbChiNumber_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidFormattedChiNumber);
      var sut2 = new GbChiNumber(ValidFormattedChiNumber);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void GbChiNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidFormattedChiNumber);
      var sut2 = new GbChiNumber(AltValidFormattedChiNumber);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void GbChiNumber_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 12 character versions for same person should still be equal.
      var sut1 = new GbChiNumber(ValidUnformattedChiNumber);
      var sut2 = new GbChiNumber(ValidFormattedChiNumber);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void GbChiNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidFormattedChiNumber);
      var sut2 = new GbChiNumber(ValidFormattedChiNumber.Replace(' ', '.'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void GbChiNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidFormattedChiNumber.Replace(' ', 'A'));
      var sut2 = new GbChiNumber(ValidFormattedChiNumber.Replace(' ', 'a'));

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

   // GbChiNumber does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void GbChiNumber_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new GbChiNumber(ValidFormattedChiNumber);
      var sut2 = new GbChiNumber(ValidFormattedChiNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void GbChiNumber_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new GbChiNumber(value);
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
   public void GbChiNumber_Validate_ShouldReturnValidValue_WhenValueIsValid(String value)
   {
      // Arrange.
      GbChiNumber.ValidationResult expected = default(ValidValue);

      // Act.
      var result = GbChiNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbChiNumber_Validate_ShouldReturnEmptyValue_WhenValueIsEmpty(String value)
   {
      // Arrange.
      GbChiNumber.ValidationResult expected = default(EmptyValue);

      // Act.
      var result = GbChiNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbChiNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      GbChiNumber.ValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = GbChiNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<GbChiNumber.ValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterData))]
   public void GbChiNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      GbChiNumber.ValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = GbChiNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void GbChiNumber_Validate_ShouldReturnInvalidChecksum_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      GbChiNumber.ValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = GbChiNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbChiNumber_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      GbChiNumber.ValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = GbChiNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRangeValues))]
   public void GbChiNumber_Validate_ShouldReturnInvalidRange_WhenValueIsOutsideOfValidRanges(String nineDigits)
   {
      // Arrange.
      var value = nineDigits + GetCheckDigit(nineDigits);
      GbChiNumber.ValidationResult expected = GetInvalidRangeResult();

      // Act.
      var result = GbChiNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidChiNumberDateOfBirthValues))]
   public void GbChiNumber_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String separator)
   {
      // Arrange.
      var value = GetChiNumberWithValidCheckDigit(dateOfBirth, separator: separator);
      GbChiNumber.ValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = GbChiNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbChiNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new GbChiNumber(ValidUnformattedChiNumber);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<GbChiNumber>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void GbChiNumber_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new GbChiNumber(AltValidFormattedChiNumber);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public GbChiNumber ChiNumber { get; set; } = null!;
   }

   [Fact]
   public void GbChiNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { ChiNumber = new GbChiNumber(ValidUnformattedChiNumber) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void GbChiNumber_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"ChiNumber\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void GbChiNumber_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"ChiNumber\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.ChiNumber.Should().BeNull();
   }

   [Fact]
   public void GbChiNumber_JsonDeserialization_ShouldThrowKfValidationException_WhenValueIsInvalid()
   {
      // Arrange.
      var json = "{\"ChiNumber\":\"4000900004\"}";  // Invalid check digit
      GbChiNumber.ValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<UKfValidationException<GbChiNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
