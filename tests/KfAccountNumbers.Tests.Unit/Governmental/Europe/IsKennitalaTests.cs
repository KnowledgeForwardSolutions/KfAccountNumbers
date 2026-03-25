// Ignore Spelling: Fyrirtaeki Kennitala Kf

#pragma warning disable IDE0008 // Use explicit type

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class IsKennitalaTests
{
   private const String Valid10CharacterEinstaklingurKennitala = "1205854369";
   private const String Valid11CharacterEinstaklingurKennitala = "120585-4369";
   private const String AltValid10CharacterEinstaklingurKennitala = "1302058360";
   private const String AltValid11CharacterEinstaklingurKennitala = "130205-8360";
   private const String Valid10CharacterFyrirtaekiKennitala = "5311073810";
   private const String Valid11CharacterFyrirtaekiKennitala = "531107 3810";
   private const String AltValid10CharacterFyrirtaekiKennitala = "6005203690";
   private const String AltValid11CharacterFyrirtaekiKennitala = "600520 3690";

   private static String GetRawKennitala(String kennitala)
      => kennitala.Length == 10
         ? kennitala
         : kennitala[..6] + kennitala[7..];

   private static String GetKennitaliaWithValidCheckDigits(
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
      Valid10CharacterEinstaklingurKennitala,
      Valid11CharacterEinstaklingurKennitala,
      AltValid10CharacterEinstaklingurKennitala,
      AltValid11CharacterEinstaklingurKennitala,
      Valid10CharacterFyrirtaekiKennitala,
      Valid11CharacterFyrirtaekiKennitala,
      AltValid10CharacterFyrirtaekiKennitala, 
      AltValid11CharacterFyrirtaekiKennitala,
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

      { "310101", "25", "9" },         // maximum days for January, any year
      { "280291", "25", "9" },         // maximum days for Feburary, non leap year
      { "290296", "25", "9" },         // maximum days for Feburary, leap year
      { "290200", "25", "0" },         // maximum days for Feburary, leap year (2000 is leap-year)
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

      { "710101", "25", "9" },         // maximum days for January, any year
      { "680291", "24", "9" },         // maximum days for Feburary, non leap year
      { "690296", "24", "9" },         // maximum days for Feburary, leap year
      { "690200", "25", "0" },         // maximum days for Feburary, leap year (2000 is leap-year)
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

   public static TheoryData<String> InvalidCharacterValues =>
   [
      "A205854369",
      "1 05854369",
      "12#5854369",
      "120=854369",
      "1205B54369",
      "12058C4369",
      "120585D369",
      "1205854a69",
      "12058543b9",
      "120585436~",

      "A20585-4369",
      "1 0585-4369",
      "12#585-4369",
      "120=85-4369",
      "1205B5-4369",
      "12058C-4369",
      "120585 D369",
      "120585 4a69",
      "120585 43b9",
      "120585 436~",
   ];

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
      // Note random digits adjusted as necessary to ensure that value has valid check digit
      { "010004", "24", "9" },      // month = 0
      { "011304", "25", "0" },      // month = 13
      { "000104", "25", "9" },      // days = 0
      { "320104", "25", "0" },      // Invalid day of month for January, any year
      { "290201", "24", "9" },      // Invalid day of for Feburary, non-leap year
      { "300204", "25", "9" },      // Invalid day of for Feburary, leap year
      { "300200", "25", "0" },      // Invalid day of for Feburary, leap year (2000 is leap-year)
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
      { "690201", "24", "9" },      // Invalid day of for Feburary, non-leap year
      { "700204", "25", "9" },      // Invalid day of for Feburary, leap year
      { "700200", "25", "0" },      // Invalid day of for Feburary, leap year (2000 is leap-year)
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

   #region Constants Tests
   // ==========================================================================
   // ==========================================================================

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
      var value = GetKennitaliaWithValidCheckDigits(separator: separator);
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
      var value = GetKennitaliaWithValidCheckDigits(
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
      => FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaEmpty + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void IsKennitala_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
      => FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaInvalidLength + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void IsKennitala_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(String value)
      => FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaInvalidCharacter + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void IsKennitala_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
      => FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(InvalidCenturyIndicatorValues))]
   public void IsKennitala_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCenturyIndicator(String value)
      => FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaInvalidCentury + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.InvalidCentury);

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void IsKennitala_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(separator: separator);

      // Act/assert.
      FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaInvalidSeparator + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.InvalidSeparator);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void IsKennitala_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);

      // Act/assert.
      FluentActions
         .Invoking(() => new IsKennitala(value))
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaInvalidDateOfBirth + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.InvalidDateOfBirth);
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
   [InlineData("280200", "73", "9", "19000228")]      // Feburary 28, 1900 - non leap year
   [InlineData("290248", "73", "9", "19480229")]      // Feburary 29, 1948 - leap year
   [InlineData("290200", "73", "0", "20000229")]      // Feburary 29, 2000 - leap year

   // fyrirtaeki values
   [InlineData("410100", "73", "9", "19000101")]      // January 1, 1900
   [InlineData("711299", "73", "9", "19991231")]      // December 31, 1999
   [InlineData("410100", "73", "0", "20000101")]      // January 1, 2000
   [InlineData("711299", "73", "0", "20991231")]      // December 31, 2099
   [InlineData("680200", "73", "9", "19000228")]      // Feburary 28, 1900 - non leap year
   [InlineData("690248", "73", "9", "19480229")]      // Feburary 29, 1948 - leap year
   [InlineData("690200", "73", "0", "20000229")]      // Feburary 29, 2000 - leap year
   public void IsKennitala_DateOfBirth_ShouldReturnExpectedValue(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator,
      String expectedDateOfBirth)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      var sut = new IsKennitala(value);
      var expected = DateOnly.ParseExact(
         expectedDateOfBirth,
         "yyyyMMdd",
         System.Globalization.CultureInfo.InvariantCulture);

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
      var value = GetKennitaliaWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      var sut = new IsKennitala(value);

      // Act/assert.
      sut.IdentifierType.Should().Be(IsIdentifierType.Einstaklingur);
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
      var value = GetKennitaliaWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      var sut = new IsKennitala(value);

      // Act/assert.
      sut.IdentifierType.Should().Be(IsIdentifierType.Fyrirtaeki);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(Valid10CharacterEinstaklingurKennitala, Valid10CharacterEinstaklingurKennitala)]
   [InlineData(Valid11CharacterEinstaklingurKennitala, Valid10CharacterEinstaklingurKennitala)]
   [InlineData(Valid10CharacterFyrirtaekiKennitala, Valid10CharacterFyrirtaekiKennitala)]
   [InlineData(Valid11CharacterFyrirtaekiKennitala, Valid10CharacterFyrirtaekiKennitala)]
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
      var value = Valid10CharacterEinstaklingurKennitala;
      var sut = new IsKennitala(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(value);
   }

   [Fact]
   public void IsKennitala_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = Valid11CharacterEinstaklingurKennitala;
      var sut = new IsKennitala(value);
      var expected = GetRawKennitala(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Fact]
   public void IsKennitala_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      IsKennitala kennitala = null!;

      // Act.
      String str = kennitala;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void IsKennitala_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      IsKennitala kennitala = null!;

      // Act.
      var str = (String)kennitala;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidKennitalaValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawKennitala(value);

      // Act.
      var sut = (IsKennitala)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(separator: separator);
      var expected = GetRawKennitala(value);

      // Act.
      var sut = (IsKennitala)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      var expected = GetRawKennitala(value);

      // Act.
      var sut = (IsKennitala)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
      => FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaEmpty + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
      => FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaInvalidLength + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueHasInvalidNonDigitCharacter(String value)
      => FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaInvalidCharacter + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
      => FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(InvalidCenturyIndicatorValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueHasInvalidCenturyIndicator(String value)
      => FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaInvalidCentury + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.InvalidCentury);

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(separator: separator);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaInvalidSeparator + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.InvalidSeparator);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void IsKennitala_ExplicitCastToIsKennitala_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (IsKennitala)value)
         .Should().Throw<KfValidationException<IsKennitalaValidationResult>>()
         .WithMessage(Messages.IsKennitalaInvalidDateOfBirth + "*")
         .And.ValidationResult.Should().Be(IsKennitalaValidationResult.InvalidDateOfBirth);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IsKennitala_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var foedselsnummer1 = new IsKennitala(Valid11CharacterEinstaklingurKennitala);
      var foedselsnummer2 = new IsKennitala(Valid11CharacterEinstaklingurKennitala);

      // Act/assert.
      (foedselsnummer1 == foedselsnummer2).Should().BeTrue();
   }

   [Fact]
   public void IsKennitala_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var foedselsnummer1 = new IsKennitala(Valid11CharacterEinstaklingurKennitala);
      var foedselsnummer2 = new IsKennitala(AltValid11CharacterEinstaklingurKennitala);

      // Act/assert.
      (foedselsnummer1 == foedselsnummer2).Should().BeFalse();
   }

   [Fact]
   public void IsKennitala_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 11 character versions for same person should still be equal.
      var foedselsnummer1 = new IsKennitala(Valid10CharacterFyrirtaekiKennitala);
      var foedselsnummer2 = new IsKennitala(Valid11CharacterFyrirtaekiKennitala);

      // Act/assert.
      (foedselsnummer1 == foedselsnummer2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IsKennitala_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var foedselsnummer1 = new IsKennitala(Valid10CharacterFyrirtaekiKennitala);
      var foedselsnummer2 = new IsKennitala(AltValid10CharacterFyrirtaekiKennitala);

      // Act/assert.
      (foedselsnummer1 != foedselsnummer2).Should().BeTrue();
   }

   [Fact]
   public void IsKennitala_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var foedselsnummer1 = new IsKennitala(Valid10CharacterEinstaklingurKennitala);
      var foedselsnummer2 = new IsKennitala(Valid11CharacterEinstaklingurKennitala);

      // Act/assert.
      (foedselsnummer1 != foedselsnummer2).Should().BeFalse();
   }

   [Fact]
   public void IsKennitala_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var foedselsnummer1 = new IsKennitala(Valid10CharacterEinstaklingurKennitala);
      var foedselsnummer2 = new IsKennitala(Valid10CharacterEinstaklingurKennitala);

      // Act/assert.
      (foedselsnummer1 != foedselsnummer2).Should().BeFalse();
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
      var expectedValue = new IsKennitala(value);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void IsKennitala_Create_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(separator: separator);
      var expectedValue = new IsKennitala(value);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void IsKennitala_Create_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);
      var expectedValue = new IsKennitala(value);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void IsKennitala_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(IsKennitalaValidationResult.Empty);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void IsKennitala_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(IsKennitalaValidationResult.InvalidLength);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void IsKennitala_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacter(String value)
   {
      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(IsKennitalaValidationResult.InvalidCharacter);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void IsKennitala_Create_ShouldReturnInvalidCheckDigitsValidationResult_WhenValueHasInvalidCheckDigit(String value)
   {
      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(IsKennitalaValidationResult.InvalidCheckDigit);
   }

   [Theory]
   [MemberData(nameof(InvalidCenturyIndicatorValues))]
   public void IsKennitala_Create_ShouldReturnInvalidCheckDigitsValidationResult_WhenValueHasInvalidCenturyIndicator(String value)
   {
      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(IsKennitalaValidationResult.InvalidCentury);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void IsKennitala_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(separator: separator);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(IsKennitalaValidationResult.InvalidSeparator);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void IsKennitala_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);

      // Act.
      var result = IsKennitala.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(IsKennitalaValidationResult.InvalidDateOfBirth);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IsKennitala_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var foedselsnummer1 = new IsKennitala(Valid10CharacterEinstaklingurKennitala);
      var foedselsnummer2 = new IsKennitala(Valid10CharacterEinstaklingurKennitala);

      // Act/assert.
      foedselsnummer1.Equals(foedselsnummer2).Should().BeTrue();
   }

   [Fact]
   public void IsKennitala_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var foedselsnummer1 = new IsKennitala(Valid10CharacterFyrirtaekiKennitala);
      var foedselsnummer2 = new IsKennitala(AltValid10CharacterFyrirtaekiKennitala);

      // Act/assert.
      foedselsnummer1.Equals(foedselsnummer2).Should().BeFalse();
   }

   [Fact]
   public void IsKennitala_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var foedselsnummer1 = new IsKennitala(Valid10CharacterEinstaklingurKennitala);
      var foedselsnummer2 = new IsKennitala(Valid11CharacterEinstaklingurKennitala);

      // Act/assert.
      foedselsnummer1.Equals(foedselsnummer2).Should().BeTrue();
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidKennitalaValues))]
   public void IsKennitala_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => IsKennitala.Validate(value).Should().Be(IsKennitalaValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void IsKennitala_Validate_ShouldReturnValidationPassed_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(separator: separator);

      // Act/assert.
      IsKennitala.Validate(value).Should().Be(IsKennitalaValidationResult.ValidationPassed);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void IsKennitala_Validate_ShouldReturnValidationPassed_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);

      // Act/assert.
      IsKennitala.Validate(value).Should().Be(IsKennitalaValidationResult.ValidationPassed);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void IsKennitala_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => IsKennitala.Validate(value).Should().Be(IsKennitalaValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void IsKennitala_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => IsKennitala.Validate(value).Should().Be(IsKennitalaValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void IsKennitala_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(String value)
      => IsKennitala.Validate(value).Should().Be(IsKennitalaValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void IsKennitala_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigits(String value)
      => IsKennitala.Validate(value).Should().Be(IsKennitalaValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(InvalidCenturyIndicatorValues))]
   public void IsKennitala_Validate_ShouldReturnInvalidCentury_WhenValueHasInvalidCenturyIndicator(String value)
      => IsKennitala.Validate(value).Should().Be(IsKennitalaValidationResult.InvalidCentury);

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void IsKennitala_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(separator: separator);

      // Act/assert.
      IsKennitala.Validate(value).Should().Be(IsKennitalaValidationResult.InvalidSeparator);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void IsKennitala_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String randomDigits,
      String centuryIndicator)
   {
      // Arrange.
      var value = GetKennitaliaWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         randomDigits: randomDigits,
         centuryIndicator: centuryIndicator);

      // Act/assert.
      IsKennitala.Validate(value).Should().Be(IsKennitalaValidationResult.InvalidDateOfBirth);
   }

   #endregion
}
