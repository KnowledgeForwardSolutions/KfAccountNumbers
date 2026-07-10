// Ignore Spelling: Deserialize Deserialization Einstaklingur Fyrirtaeki Json Kennitala Kf

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.National.Europe.IsKennitala,
   KfAccountNumbers.National.Europe.IsKennitala.ValidationError>;
using LocalValidationError = KfAccountNumbers.National.Europe.IsKennitala.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.National.Europe.IsKennitala.ValidationError>;
using LocalValidationResult = KfAccountNumbers.National.Europe.IsKennitala.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class IsKennitalaTests
{
   private const String ValidUnformattedEinstaklingurKennitala = "1205854369";
   private const String ValidFormattedEinstaklingurKennitala = "120585-4369";
   private const String AltValidUnformattedEinstaklingurKennitala = "1302058360";
   private const String AltValidFormattedEinstaklingurKennitala = "130205-8360";
   private const String ValidUnformattedFyrirtaekiKennitala = "5311073810";
   private const String ValidFormattedFyrirtaekiKennitala = "531107 3810";
   private const String AltValidUnformattedFyrirtaekiKennitala = "6005203690";
   private const String AltValidFormattedFyrirtaekiKennitala = "600520 3690";

   private static String GetRawKennitala(String kennitala)
      => kennitala.Length == 10
         ? kennitala
         : kennitala[..6] + kennitala[7..];

   private static String GetKennitalaWithValidCheckDigit(
      String dateOfBirth = "130295",
      String separator = "",
      String randomDigits = "37",
      String centuryIndicator = "9")
   {
      var d1 = dateOfBirth[0] - Chars.DigitZero;
      var d2 = dateOfBirth[1] - Chars.DigitZero;
      var m1 = dateOfBirth[2] - Chars.DigitZero;
      var m2 = dateOfBirth[3] - Chars.DigitZero;
      var y1 = dateOfBirth[4] - Chars.DigitZero;
      var y2 = dateOfBirth[5] - Chars.DigitZero;
      var r1 = randomDigits[0] - Chars.DigitZero;
      var r2 = randomDigits[1] - Chars.DigitZero;

      var sum = (3 * d1) + (2 * d2) + (7 * m1) + (6 * m2) + (5 * y1) + (4 * y2) + (3 * r1) + (2 * r2);
      var remainder = sum % 11;
      if (remainder == 1)
      {
         // Values that would result in a check digit = 10 are not issued.
         return String.Empty;
      }

      var checkDigit = (remainder == 0) ? 0 : 11 - remainder;

      return $"{dateOfBirth}{separator}{randomDigits}{checkDigit}{centuryIndicator}";
   }

   public static TheoryData<String> ValidKennitalaValues =>
   [
      ValidUnformattedEinstaklingurKennitala,
      ValidFormattedEinstaklingurKennitala,
      AltValidUnformattedEinstaklingurKennitala,
      AltValidFormattedEinstaklingurKennitala,
      ValidUnformattedFyrirtaekiKennitala,
      ValidFormattedFyrirtaekiKennitala,
      AltValidUnformattedFyrirtaekiKennitala,
      AltValidFormattedFyrirtaekiKennitala,
   ];

   public static TheoryData<String> ValidSeparators =>
   [
      " ",
      "-",
      "A",
      "z",
      "!",
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "120585436",        // Length 9
      "600520 36900",     // Length 13
      new String('1', 100) // Very long string
   ];

   public static TheoryData<String, String, String> ValidDateOfBirthValues = new()
   {
      // Note random digits adjusted as necessary to ensure that value has valid check digit
      { "010100", "25", "9" },         // January 1, 1900
      { "311299", "25", "9" },         // December 31, 2099
      { "010100", "25", "0" },         // January 1, 2000
      { "311299", "25", "0" },         // December 31, 2099

      // Max days per month
      { "310101", "25", "9" },         // maximum days for January, any year
      { "280291", "25", "9" },         // maximum days for February, non leap year
      { "290296", "25", "9" },         // maximum days for February, leap year
      { "290200", "25", "0" },         // maximum days for February, leap year (2000 is leap-year)
      { "310304", "25", "9" },         // maximum days for March, any year
      { "300404", "25", "0" },         // maximum days for April, any year
      { "310504", "25", "9" },         // maximum days for May, any year
      { "300604", "25", "0" },         // maximum days for June, any year
      { "310704", "25", "9" },         // maximum days for July, any year
      { "310804", "25", "0" },         // maximum days for August, any year
      { "300904", "25", "9" },         // maximum days for September, any year
      { "311004", "25", "0" },         // maximum days for October, any year
      { "301104", "25", "9" },         // maximum days for November, any year
      { "311204", "25", "0" },         // maximum days for December, any year

      // repeat for fyrirtaeki
      { "410100", "25", "9" },         // January 1, 1900
      { "711299", "25", "9" },         // December 31, 2099
      { "410100", "25", "0" },         // January 1, 2000
      { "711299", "25", "0" },         // December 31, 2099

      // Max days per month
      { "710101", "25", "9" },         // maximum days for January, any year
      { "680291", "24", "9" },         // maximum days for February, non leap year
      { "690296", "24", "9" },         // maximum days for February, leap year
      { "690200", "25", "0" },         // maximum days for February, leap year (2000 is leap-year)
      { "710304", "25", "9" },         // maximum days for March, any year
      { "700404", "25", "0" },         // maximum days for April, any year
      { "710504", "25", "9" },         // maximum days for May, any year
      { "700604", "24", "0" },         // maximum days for June, any year
      { "710704", "25", "9" },         // maximum days for July, any year
      { "710804", "25", "0" },         // maximum days for August, any year
      { "700904", "25", "9" },         // maximum days for September, any year
      { "711004", "25", "0" },         // maximum days for October, any year
      { "701104", "25", "9" },         // maximum days for November, any year
      { "711204", "25", "0" },         // maximum days for December, any year
   };

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted values
      { ".205854369", 0 },          // Non-digit character '.'
      { "1 05854369", 1 },          // Non-digit character ' '
      { "12A5854369", 2 },          // Non-digit character 'A'
      { "120Z854369", 3 },          // Non-digit character 'Z'
      { "1205^54369", 4 },          // Non-digit character '^'
      { "12058a4369", 5 },          // Non-digit character 'a'
      { "120585z369", 6 },          // Non-digit character 'z'
      { "1205854~69", 7 },          // Non-digit character '~'
      { "12058543\u21539", 8 },     // Non-digit character Unicode fraction 1/3
      { "120585436\u00D6", 9 },     // Invalid character unicode O with umlaut

      // Formatted values
      { ".10585 4369", 0 },         // Non-digit character '.'
      { "1 0585 4369", 1 },         // Non-digit character ' '
      { "12A585 4369", 2 },         // Non-digit character 'A'
      { "120Z85 4369", 3 },         // Non-digit character 'Z'
      { "1205^5 4369", 4 },         // Non-digit character '^'
      { "12058a 4369", 5 },         // Non-digit character 'a'
      { "120585 z369", 7 },         // Non-digit character 'z'
      { "120585-4~69", 8 },         // Non-digit character '~'
      { "120585-43\u21539", 9 },    // Non-digit character Unicode fraction 1/3
      { "120585-436\u00D6", 10 },   // Invalid character unicode O with umlaut
   };

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "1295854369",        // 1205854369 with single digit transcription error 0 -> 9
      "2608832499",        // 2608832599 with single digit transcription error 5 -> 4
      "5707700780",        // 5707070780 with two digit transposition error 07 -> 70
      "6005230690",        // 6005203690 with two digit transposition error 03 -> 30
      "2603882599",        // 2608832599 with jump transposition error 883 -> 388
      "6115203690",        // 6005203690 with two digit twin error 00 -> 11

      "129585-4369",       // 1205854369 with single digit transcription error 0 -> 9
      "260883-2499",       // 2608832599 with single digit transcription error 5 -> 4
      "570770-0780",       // 5707070780 with two digit transposition error 07 -> 70
      "600523 0690",       // 6005203690 with two digit transposition error 03 -> 30
      "260388 2599",       // 2608832599 with jump transposition error 883 -> 388
      "611520 3690",       // 6005203690 with two digit twin error 00 -> 11
   ];

   public static TheoryData<String> InvalidCenturyIndicatorValues =>
   [
      "1205854361",
      "1205854362",
      "1205854363",
      "1205854364",
      "1205854365",
      "1205854366",
      "1205854367",
      "1205854368",

      "120585-4361",
      "120585-4362",
      "120585-4363",
      "120585-4364",
      "120585 4365",
      "120585 4366",
      "120585 4367",
      "120585 4368",
   ];

   public static TheoryData<String> InvalidSeparators =>
   [
      "0",
      "1",
      "2",
      "3",
      "4",
      "5",
      "6",
      "7",
      "8",
      "9",
   ];

   public static TheoryData<String, String, String> InvalidDateOfBirthValues = new()
   {
      // Note that dates of birth < 1900 or > 2099 will return invalid century result
      // and are not checked here.

      // Note random digits adjusted as necessary to ensure that value has valid check digit.
      { "010004", "24", "9" },      // month = 0
      { "011304", "25", "0" },      // month = 13
      { "000104", "25", "9" },      // days = 0
      { "320104", "25", "0" },      // Invalid day of month for January, any year
      { "290201", "24", "9" },      // Invalid day of for February, non-leap year
      { "300204", "25", "9" },      // Invalid day of for February, leap year
      { "300200", "25", "0" },      // Invalid day of for February, leap year (2000 is leap-year)
      { "320304", "25", "9" },      // Invalid day of for March, any year
      { "310404", "24", "0" },      // Invalid day of for April, any year
      { "320504", "25", "9" },      // Invalid day of for May, any year
      { "310604", "25", "9" },      // Invalid day of for June, any year
      { "320704", "25", "0" },      // Invalid day of for July, any year
      { "320804", "25", "9" },      // Invalid day of for August, any year
      { "310904", "25", "0" },      // Invalid day of for September, any year
      { "321004", "25", "9" },      // Invalid day of for October, any year
      { "311104", "24", "0" },      // Invalid day of for November, any year
      { "321204", "25", "9" },      // Invalid day of for December, any year

      // repeat for fyrirtaeki
      { "410004", "25", "9" },      // month = 0
      { "411304", "25", "0" },      // month = 13
      { "400104", "25", "9" },      // days = 0
      { "720104", "25", "0" },      // Invalid day of month for January, any year
      { "690201", "24", "9" },      // Invalid day of for February, non-leap year
      { "700204", "25", "9" },      // Invalid day of for February, leap year
      { "700200", "25", "0" },      // Invalid day of for February, leap year (2000 is leap-year)
      { "720304", "25", "9" },      // Invalid day of for March, any year
      { "710404", "25", "0" },      // Invalid day of for April, any year
      { "720504", "25", "9" },      // Invalid day of for May, any year
      { "710604", "25", "9" },      // Invalid day of for June, any year
      { "720704", "25", "0" },      // Invalid day of for July, any year
      { "720804", "25", "9" },      // Invalid day of for August, any year
      { "710904", "25", "0" },      // Invalid day of for September, any year
      { "721004", "25", "9" },      // Invalid day of for October, any year
      { "711104", "25", "0" },      // Invalid day of for November, any year
      { "721204", "25", "9" },      // Invalid day of for December, any year
   };

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.IsKennitalaInvalidLength,
         value.Length,
         IsKennitala.GetValidLengthDefinitions());

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.IsKennitalaInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.IsKennitalaInvalidCheckDigit,
         IsKennitala.CheckDigitAlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(String value)
      => new(Messages.IsKennitalaInvalidSeparator, value[6], 6);

   private static InvalidCentury GetInvalidCenturyResult(String value)
      => new(Messages.IsKennitalaInvalidCentury, value[^1].ToString());

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(Messages.IsKennitalaInvalidDateOfBirth, value[..6], DateFormatName.DDMMYY);

   #region Constants Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IsKennitala_CheckDigitAlgorithmName_ShouldHaveExpectedValue()
      => IsKennitala.CheckDigitAlgorithmName.Should().Be("Weighted Modulus 11");

   [Fact]
   public void IsKennitala_MinimumValidYearOfBirth_ShouldHaveExpectedValue()
      => IsKennitala.MinimumValidYearOfBirth.Should().Be(1900);

   [Fact]
   public void IsKennitala_MaximumValidYearOfBirth_ShouldHaveExpectedValue()
      => IsKennitala.MaximumValidYearOfBirth.Should().Be(2099);

   [Fact]
   public void IsKennitala_FyrirtaekiDayOffset_ShouldHaveExpectedValue()
      => IsKennitala.FyrirtaekiDayOffset.Should().Be(40);

   #endregion

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidKennitalaValues))]
   public void IsKennitala_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawKennitala(value);

      // Act.
      var sut = new IsKennitala(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void IsKennitala_Constructor_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(separator: separator);
      var expected = GetRawKennitala(value);

      // Act.
      var sut = new IsKennitala(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void IsKennitala_Constructor_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      var expected = GetRawKennitala(value);

      // Act.
      var sut = new IsKennitala(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void IsKennitala_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void IsKennitala_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void IsKennitala_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void IsKennitala_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCenturyIndicatorValues))]
   public void IsKennitala_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCenturyIndicator(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCenturyResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void IsKennitala_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(separator: separator);
      LocalValidationError expected = GetInvalidSeparatorResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void IsKennitala_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("010100", "73", "9", "19000101")]      // January 1, 1900
   [InlineData("311299", "73", "9", "19991231")]      // December 31, 1999
   [InlineData("010100", "73", "0", "20000101")]      // January 1, 2000
   [InlineData("311299", "73", "0", "20991231")]      // December 31, 2099
   [InlineData("280200", "73", "9", "19000228")]      // February 28, 1900 - non leap year
   [InlineData("290248", "73", "9", "19480229")]      // February 29, 1948 - leap year
   [InlineData("290200", "73", "0", "20000229")]      // February 29, 2000 - leap year

   // fyrirtaeki values
   [InlineData("410100", "73", "9", "19000101")]      // January 1, 1900
   [InlineData("711299", "73", "9", "19991231")]      // December 31, 1999
   [InlineData("410100", "73", "0", "20000101")]      // January 1, 2000
   [InlineData("711299", "73", "0", "20991231")]      // December 31, 2099
   [InlineData("680200", "73", "9", "19000228")]      // February 28, 1900 - non leap year
   [InlineData("690248", "73", "9", "19480229")]      // February 29, 1948 - leap year
   [InlineData("690200", "73", "0", "20000229")]      // February 29, 2000 - leap year
   public void IsKennitala_DateOfBirth_ShouldReturnExpectedValue(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator,
      String expectedDateOfBirth)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      var sut = new IsKennitala(value);
      var expected = DateOnly.ParseExact(
         expectedDateOfBirth,
         "yyyyMMdd",
         CultureInfo.InvariantCulture);

      // Act/assert.
      sut.DateOfBirth.Should().Be(expected);
   }

   #endregion

   #region IdentifierType Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("010154", "15", "9")]
   [InlineData("311239", "15", "9")]
   public void IsKennitala_IdentifierType_ShouldReturnExpectedValue_WhenValueIsEinstaklingur(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      var sut = new IsKennitala(value);
      IsKennitala.IdentifierCategory expected = default(IsIdentifierType.Einstaklingur);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   [Theory]
   [InlineData("410154", "15", "0")]
   [InlineData("711239", "15", "0")]
   public void IsKennitala_IdentifierType_ShouldReturnExpectedValue_WhenValueIsFyrirtaeki(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      var sut = new IsKennitala(value);
      IsKennitala.IdentifierCategory expected = default(IsIdentifierType.Fyrirtaeki);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidUnformattedEinstaklingurKennitala, ValidUnformattedEinstaklingurKennitala)]
   [InlineData(ValidFormattedEinstaklingurKennitala, ValidUnformattedEinstaklingurKennitala)]
   [InlineData(ValidUnformattedFyrirtaekiKennitala, ValidUnformattedFyrirtaekiKennitala)]
   [InlineData(ValidFormattedFyrirtaekiKennitala, ValidUnformattedFyrirtaekiKennitala)]
   public void IsKennitala_Value_ShouldReturnValidatedKennitala(
      String value,
      String expected)
   {
      // Arrange.
      var sut = new IsKennitala(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IsKennitala_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidUnformattedEinstaklingurKennitala;
      var sut = new IsKennitala(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void IsKennitala_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedEinstaklingurKennitala;
      var sut = new IsKennitala(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void IsKennitala_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      IsKennitala sut = null!;

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void IsKennitala_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      IsKennitala sut = null!;

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidKennitalaValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new IsKennitala(value);

      // Act.
      var sut = (IsKennitala)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(separator: separator);
      var expected = new IsKennitala(value);

      // Act.
      var sut = (IsKennitala)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      var expected = new IsKennitala(value);

      // Act.
      var sut = (IsKennitala)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueHasInvalidNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCenturyIndicatorValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueHasInvalidCenturyIndicator(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCenturyResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(separator: separator);
      LocalValidationError expected = GetInvalidSeparatorResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IsKennitala_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidFormattedEinstaklingurKennitala);
      var sut2 = new IsKennitala(ValidFormattedEinstaklingurKennitala);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void IsKennitala_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidFormattedEinstaklingurKennitala);
      var sut2 = new IsKennitala(AltValidFormattedEinstaklingurKennitala);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void IsKennitala_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 11 character versions for same person should still be equal.
      var sut1 = new IsKennitala(ValidUnformattedFyrirtaekiKennitala);
      var sut2 = new IsKennitala(ValidFormattedFyrirtaekiKennitala);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void IsKennitala_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidFormattedEinstaklingurKennitala);
      var sut2 = new IsKennitala(ValidFormattedEinstaklingurKennitala.Replace('-', '.'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void IsKennitala_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidFormattedEinstaklingurKennitala.Replace('-', 'A'));
      var sut2 = new IsKennitala(ValidFormattedEinstaklingurKennitala.Replace('-', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IsKennitala_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidUnformattedFyrirtaekiKennitala);
      var sut2 = new IsKennitala(AltValidUnformattedFyrirtaekiKennitala);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void IsKennitala_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 11 character versions for same person should still be equal.
      var sut1 = new IsKennitala(ValidUnformattedEinstaklingurKennitala);
      var sut2 = new IsKennitala(ValidFormattedEinstaklingurKennitala);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void IsKennitala_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidUnformattedEinstaklingurKennitala);
      var sut2 = new IsKennitala(ValidUnformattedEinstaklingurKennitala);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void IsKennitala_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidFormattedEinstaklingurKennitala);
      var sut2 = new IsKennitala(ValidFormattedEinstaklingurKennitala.Replace('-', '.'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void IsKennitala_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidFormattedEinstaklingurKennitala.Replace('-', 'A'));
      var sut2 = new IsKennitala(ValidFormattedEinstaklingurKennitala.Replace('-', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidKennitalaValues))]
   public void IsKennitala_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new IsKennitala(value);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void IsKennitala_Create_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(separator: separator);
      LocalCreateResult expected = new IsKennitala(value);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void IsKennitala_Create_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      LocalCreateResult expected = new IsKennitala(value);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void IsKennitala_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void IsKennitala_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void IsKennitala_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void IsKennitala_Create_ShouldReturnInvalidCheckDigitValidationResult_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCenturyIndicatorValues))]
   public void IsKennitala_Create_ShouldReturnInvalidCenturyValidationResult_WhenValueHasInvalidCenturyIndicator(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCenturyResult(value);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void IsKennitala_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(separator: separator);
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void IsKennitala_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IsKennitala_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidUnformattedEinstaklingurKennitala);
      var sut2 = new IsKennitala(ValidUnformattedEinstaklingurKennitala);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void IsKennitala_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidUnformattedFyrirtaekiKennitala);
      var sut2 = new IsKennitala(AltValidUnformattedFyrirtaekiKennitala);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void IsKennitala_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 11 character versions for same person should still be equal.
      var sut1 = new IsKennitala(ValidUnformattedEinstaklingurKennitala);
      var sut2 = new IsKennitala(ValidFormattedEinstaklingurKennitala);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void IsKennitala_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidFormattedEinstaklingurKennitala);
      var sut2 = new IsKennitala(ValidFormattedEinstaklingurKennitala.Replace('-', '.'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void IsKennitala_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidFormattedEinstaklingurKennitala.Replace('-', 'A'));
      var sut2 = new IsKennitala(ValidFormattedEinstaklingurKennitala.Replace('-', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void IsKennitala_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new IsKennitala(ValidFormattedEinstaklingurKennitala);

      // Act/assert.
      sut.Equals(ValidFormattedEinstaklingurKennitala).Should().BeFalse();
   }

   [Fact]
   public void IsKennitala_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new IsKennitala(ValidFormattedEinstaklingurKennitala);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IsKennitala_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new IsKennitala(ValidUnformattedEinstaklingurKennitala);
      var expected = ValidFormattedEinstaklingurKennitala;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void IsKennitala_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new IsKennitala(AltValidUnformattedFyrirtaekiKennitala);
      var mask = "______ ____";
      var expected = AltValidFormattedFyrirtaekiKennitala;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void IsKennitala_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new IsKennitala(ValidUnformattedEinstaklingurKennitala);
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
   public void IsKennitala_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new IsKennitala(AltValidUnformattedFyrirtaekiKennitala);
      var expectedMessage = Messages.FormatMaskEmpty + "*";
      var act = () => _ = sut.Format(mask);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(mask))
         .WithMessage(expectedMessage);
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IsKennitala_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidUnformattedEinstaklingurKennitala);
      var sut2 = new IsKennitala(ValidUnformattedEinstaklingurKennitala);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void IsKennitala_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidFormattedEinstaklingurKennitala);
      var sut2 = new IsKennitala(AltValidFormattedEinstaklingurKennitala);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void IsKennitala_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 11 character versions for same person should still be equal.
      var sut1 = new IsKennitala(ValidUnformattedFyrirtaekiKennitala);
      var sut2 = new IsKennitala(ValidFormattedFyrirtaekiKennitala);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void IsKennitala_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidFormattedFyrirtaekiKennitala);
      var sut2 = new IsKennitala(ValidFormattedFyrirtaekiKennitala.Replace('-', '.'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void IsKennitala_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidFormattedFyrirtaekiKennitala.Replace('-', 'A'));
      var sut2 = new IsKennitala(ValidFormattedFyrirtaekiKennitala.Replace('-', 'a'));

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

   // IsKennitala does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void IsKennitala_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new IsKennitala(ValidUnformattedEinstaklingurKennitala);
      var sut2 = new IsKennitala(ValidUnformattedEinstaklingurKennitala);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidKennitalaValues))]
   public void IsKennitala_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new IsKennitala(value);
      var expected = GetRawKennitala(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidKennitalaValues))]
   public void IsKennitala_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = IsKennitala.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void IsKennitala_Validate_ShouldReturnValidationPassed_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(separator: separator);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = IsKennitala.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void IsKennitala_Validate_ShouldReturnValidationPassed_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = IsKennitala.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void IsKennitala_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = IsKennitala.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void IsKennitala_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = IsKennitala.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void IsKennitala_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = IsKennitala.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void IsKennitala_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = IsKennitala.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCenturyIndicatorValues))]
   public void IsKennitala_Validate_ShouldReturnInvalidCentury_WhenValueHasInvalidCenturyIndicator(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCenturyResult(value);

      // Act.
      var result = IsKennitala.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void IsKennitala_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(separator: separator);
      LocalValidationResult expected = GetInvalidSeparatorResult(value);

      // Act.
      var result = IsKennitala.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void IsKennitala_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitalaWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = IsKennitala.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IsKennitala_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new IsKennitala(ValidUnformattedEinstaklingurKennitala);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<IsKennitala>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void IsKennitala_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new IsKennitala(ValidFormattedFyrirtaekiKennitala);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public IsKennitala Kennitala { get; set; } = null!;
   }

   [Fact]
   public void IsKennitala_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Kennitala = new IsKennitala(ValidFormattedEinstaklingurKennitala) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void IsKennitala_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Kennitala\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void IsKennitala_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Kennitala\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Kennitala.Should().BeNull();
   }

   [Fact]
   public void IsKennitala_JsonDeserialization_ShouldThrowKfValidationException_WhenKennitalaIsInvalid()
   {
      // Arrange.
      var json = "{\"Kennitala\":\"1295854369\"}";  // Invalid check digit
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
