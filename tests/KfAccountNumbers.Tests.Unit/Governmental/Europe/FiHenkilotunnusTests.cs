// Ignore Spelling: Deserialize Deserialization Fi Henkilotunnus Json Kf

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

using LocalCreateResult = KfAccountNumbers.Results.UCreateResult<
   KfAccountNumbers.Governmental.Europe.FiHenkilotunnus,
   KfAccountNumbers.Governmental.Europe.FiHenkilotunnus.ValidationError>;
using LocalValidationError = KfAccountNumbers.Governmental.Europe.FiHenkilotunnus.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.Governmental.Europe.FiHenkilotunnus.ValidationError>;
using LocalValidationResult = KfAccountNumbers.Governmental.Europe.FiHenkilotunnus.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class FiHenkilotunnusTests
{
   private const String ValidHenkilotunnus = "230526-034N";
   private const String AltValidHenkilotunnus = "160117A275C";
   private const String ValidTestHenkilotunnus = "020508D929B";
   private const String AltValidTestHenkilotunnus = "051272X990D";
   private const String ValidLowercaseHenkilotunnus = "230526-034n";
   private const String AltValidLowercaseHenkilotunnus = "160117a275c";
   private const String ValidLowercaseTestHenkilotunnus = "020508d929b";
   private const String AltValidLowercaseTestHenkilotunnus = "051272x990d";

   private static String GetHenkilotunnusWithValidCheckDigit(
      String dateOfBirth = "160117",
      Char centuryIndicator = 'A',
      String individualNumber = "275")
   {
      const String checkCharacters = "0123456789ABCDEFHJKLMNPRSTUVWXY";

      var temp = dateOfBirth + individualNumber;
      var sum = 0;
      foreach (var ch in temp)
      {
         sum *= 10;
         var num = ch - Chars.DigitZero;
         sum += num;
      }

      var checkDigit = sum % 31;
      var checkCharacter = checkCharacters[checkDigit];

      return $"{dateOfBirth}{centuryIndicator}{individualNumber}{checkCharacter}";
   }

   public static TheoryData<String> ValidHenkilotunnusValues =>
   [
      ValidHenkilotunnus,
      AltValidHenkilotunnus,
      ValidTestHenkilotunnus,
      AltValidTestHenkilotunnus,
      ValidLowercaseHenkilotunnus,
      AltValidLowercaseHenkilotunnus,
      ValidLowercaseTestHenkilotunnus,
      AltValidLowercaseTestHenkilotunnus,
   ];

   public static TheoryData<String, Char> ValidDateOfBirthValues = new()
   {
      // 1800s
      { "010100", '+' },      // January 1, 1800
      { "311299", '+' },      // December 31, 1899

      // 1900s
      { "010100", '-' },      // January 1, 1900
      { "311299", '-' },      // December 31, 1999
      { "010100", 'U' },      // January 1, 1900
      { "311299", 'U' },      // December 31, 1999
      { "010100", 'V' },      // January 1, 1900
      { "311299", 'V' },      // December 31, 1999
      { "010100", 'W' },      // January 1, 1900
      { "311299", 'W' },      // December 31, 1999
      { "010100", 'X' },      // January 1, 1900
      { "311299", 'X' },      // December 31, 1999
      { "010100", 'Y' },      // January 1, 1900
      { "311299", 'Y' },      // December 31, 1999

      // 2000s
      { "010100", 'A' },      // January 1, 2000
      { "311299", 'A' },      // December 31, 2099
      { "010100", 'B' },      // January 1, 2000
      { "311299", 'B' },      // December 31, 2099
      { "010100", 'C' },      // January 1, 2000
      { "311299", 'C' },      // December 31, 2099
      { "010100", 'D' },      // January 1, 2000
      { "311299", 'D' },      // December 31, 2099
      { "010100", 'E' },      // January 1, 2000
      { "311299", 'E' },      // December 31, 2099
      { "010100", 'F' },      // January 1, 2000
      { "311299", 'F' },      // December 31, 2099

      // Maximum days per month
      { "310104", '+' },      // maximum days for January, any year
      { "280201", '-' },      // maximum days for February, non-leap year
      { "290204", 'U' },      // maximum days for February, leap year
      { "290200", 'A' },      // maximum days for February, leap year (2000 is leap-year)
      { "310304", '+' },      // maximum days for March, any year
      { "300404", 'V' },      // maximum days for April, any year
      { "310504", 'B' },      // maximum days for May, any year
      { "300604", '+' },      // maximum days for June, any year
      { "310704", 'W' },      // maximum days for July, any year
      { "310804", 'C' },      // maximum days for August, any year
      { "300904", '+' },      // maximum days for September, any year
      { "311004", 'X' },      // maximum days for October, any year
      { "301104", 'D' },      // maximum days for November, any year
      { "311204", '+' },      // maximum days for December, any year
   };

   public static TheoryData<Char> ValidCenturyIndicatorValues =>
   [
      '+',
      '-',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'A',
      'B',
      'C',
      'D',
      'E',
      'F',
      'u',
      'v',
      'w',
      'x',
      'y',
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "230526-034",           // Length 10
      "160117A2754C",         // Length 12
      new String('1', 100)    // Very long string
   ];

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      { ".30526-034N", 0 },         // Non-digit character '.'
      { "2 0526-034N", 1 },         // Non-digit character ' '
      { "23A526-034N", 2 },         // Non-digit character 'A'
      { "230Z26-034N", 3 },         // Non-digit character 'Z'
      { "2305^6-034N", 4 },         // Non-digit character '^'
      { "23052a-034N", 5 },         // Non-digit character 'a'
      { "230526-z24N", 7 },         // Non-digit character 'z'
      { "230526-8~4N", 8 },         // Non-digit character '~'
      { "230526-83\u2153N", 9 },    // Non-digit character Unicode fraction 1/3
      { "230526-83\u00D6N", 9 },    // Invalid character unicode O with umlaut
   };

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "230626-034N",          // 230526-034N with single digit transcription error, 5 -> 6
      "020508D029B",          // 020508D929B with single digit transcription error, 9 -> 0
      "160112A775C",          // 160117A275C with two digit transposition error, 72 -> 27
      "015272X990D",          // 051272X990D with two digit transposition error, 51 -> 15
      "230625-034N",          // 230526-034N with two digit jump transposition, 526 -> 625
      "020502D989B",          // 020508D929B with two digit jump transposition, 892 -> 298
      "160227A275C",          // 160117A275C with two digit twin error, 11 -> 22
      "190886V941V",          // 190776V941V with two digit twin error, 77 -> 88
   ];

   public static TheoryData<Char> InvalidCenturyIndicatorValues =>
   [
      '=',
      '~',
      '0',
      '9',
      '\u00D6',
      '\u00F6',
      'G',
      'H',
      'I',
      'J',
      'K',
      'L',
      'M',
      'N',
      'O',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'g',
      'h',
      'i',
      'j',
      'k',
      'l',
      'm',
      'n',
      'o',
      'p',
      'q',
      'r',
      's',
      't',
   ];

   public static TheoryData<String> InvalidIndividualNumberValues =>
   [
      "000",
      "001",
   ];

   public static TheoryData<String, Char> InvalidDateOfBirthValues = new()
   {
      // Invalid month values
      { "010004", '+' },      // month = 0
      { "011304", '-' },      // month = 13
      { "000104", 'A' },      // days = 0

      // Invalid day values
      { "320104", '+' },      // Invalid day of month for January, any year
      { "290201", 'U' },      // Invalid day of for February, non-leap year
      { "300204", 'B' },      // Invalid day of for February, leap year
      { "300200", 'B' },      // Invalid day of for February, leap year (2000 is leap-year)
      { "320304", '+' },      // Invalid day of for March, any year
      { "310404", 'V' },      // Invalid day of for April, any year
      { "320504", 'C' },      // Invalid day of for May, any year
      { "310604", '+' },      // Invalid day of for June, any year
      { "320704", 'W' },      // Invalid day of for July, any year
      { "320804", 'D' },      // Invalid day of for August, any year
      { "310904", '+' },      // Invalid day of for September, any year
      { "321004", 'X' },      // Invalid day of for October, any year
      { "311104", 'E' },      // Invalid day of for November, any year
      { "321204", '+' },      // Invalid day of for December, any year

      // Invalid day values (lowercase century indicator)
      { "320104", 'a' },      // Invalid day of month for January, any year
      { "290201", 'b' },      // Invalid day of for February, non-leap year
      { "300204", 'c' },      // Invalid day of for February, leap year
      { "300200", 'd' },      // Invalid day of for February, leap year (2000 is leap-year)
      { "320304", 'e' },      // Invalid day of for March, any year
      { "310404", 'f' },      // Invalid day of for April, any year
      { "320504", 'u' },      // Invalid day of for May, any year
      { "310604", 'v' },      // Invalid day of for June, any year
      { "320704", 'w' },      // Invalid day of for July, any year
      { "320804", 'x' },      // Invalid day of for August, any year
      { "310904", 'y' },      // Invalid day of for September, any year
      { "321004", 'a' },      // Invalid day of for October, any year
      { "311104", 'b' },      // Invalid day of for November, any year
      { "321204", 'c' },      // Invalid day of for December, any year
   };

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.FiHenkilotunnusInvalidLength,
         value.Length,
         new ValidLengthDefinition(11, Messages.FiHenkilotunnusValidLength));

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.FiHenkilotunnusInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.FiHenkilotunnusInvalidCheckDigit,
         FiHenkilotunnus.CheckDigitAlgorithmName);

   private static FiHenkilotunnusInvalidIndividualNumber GetInvalidIndividualNumberResult(String value)
      => new(
         Messages.FiHenkilotunnusInvalidIndividualNumber,
         value[7..10]);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(Messages.FiHenkilotunnusInvalidDateOfBirth, value[..6], DateFormatName.DDMMYY);

   private static InvalidCentury GetInvalidCenturyResult(String value)
      => new(Messages.FiHenkilotunnusInvalidCenturyIndicator, value[6]);

   #region Constants Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FiHenkilotunnus_CheckDigitAlgorithmName_ShouldHaveExpectedValue()
      => FiHenkilotunnus.CheckDigitAlgorithmName.Should().Be("Modulus 31");

   [Fact]
   public void FiHenkilotunnus_MinimumValidYearOfBirth_ShouldHaveExpectedValue()
      => FiHenkilotunnus.MinimumValidYearOfBirth.Should().Be(1800);

   [Fact]
   public void FiHenkilotunnus_MaximumValidYearOfBirth_ShouldHaveExpectedValue()
      => FiHenkilotunnus.MaximumValidYearOfBirth.Should().Be(2099);

   #endregion

   #region Check Digit Algorithm Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   // Values below are valid henkilötunnus, and designed to produce all possible
   // check characters. "010100+000" mod 31 has remainder 14. From there it is
   // possible to derive the individual numbers that produce both valid henkilötunnus
   // and all possible check characters.
   [InlineData("010100+002H")]      // Starting point. "010100+002" mod 31 = 16, or check character H.
   [InlineData("010100+003J")]      // Increment individual number to get next check character
   [InlineData("010100+004K")]
   [InlineData("010100+005L")]
   [InlineData("010100+006M")]
   [InlineData("010100+007N")]
   [InlineData("010100+008P")]
   [InlineData("010100+009R")]
   [InlineData("010100+010S")]
   [InlineData("010100+011T")]
   [InlineData("010100+012U")]
   [InlineData("010100+013V")]
   [InlineData("010100+014W")]
   [InlineData("010100+015X")]
   [InlineData("010100+016Y")]
   [InlineData("010100+0170")]      // Roll over to check character 0
   [InlineData("010100+0181")]
   [InlineData("010100+0192")]
   [InlineData("010100+0203")]
   [InlineData("010100+0214")]
   [InlineData("010100+0225")]
   [InlineData("010100+0236")]
   [InlineData("010100+0247")]
   [InlineData("010100+0258")]
   [InlineData("010100+0269")]
   [InlineData("010100+027A")]
   [InlineData("010100+028B")]
   [InlineData("010100+029C")]
   [InlineData("010100+030D")]
   [InlineData("010100+031E")]
   [InlineData("010100+032F")]

   // Lowercase check characters
   [InlineData("010100+002h")]      // Starting point. "010100+002" mod 31 = 16, or check character H.
   [InlineData("010100+003j")]      // Increment individual number to get next check character
   [InlineData("010100+004k")]
   [InlineData("010100+005l")]
   [InlineData("010100+006m")]
   [InlineData("010100+007n")]
   [InlineData("010100+008p")]
   [InlineData("010100+009r")]
   [InlineData("010100+010s")]
   [InlineData("010100+011t")]
   [InlineData("010100+012u")]
   [InlineData("010100+013v")]
   [InlineData("010100+014w")]
   [InlineData("010100+015x")]
   [InlineData("010100+016y")]
   // [InlineData("010100+0170")]      // Roll over to check character 0
   // [InlineData("010100+0181")]      // Digits ignored because not lowercase
   // [InlineData("010100+0192")]
   // [InlineData("010100+0203")]
   // [InlineData("010100+0214")]
   // [InlineData("010100+0225")]
   // [InlineData("010100+0236")]
   // [InlineData("010100+0247")]
   // [InlineData("010100+0258")]
   // [InlineData("010100+0269")]
   [InlineData("010100+027a")]
   [InlineData("010100+028b")]
   [InlineData("010100+029c")]
   [InlineData("010100+030d")]
   [InlineData("010100+031e")]
   [InlineData("010100+032f")]
   public void FiHenkilotunnus_CheckDigitAlgorithm_ShouldGenerateAllPossibleCharacters(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = FiHenkilotunnus.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidHenkilotunnusValues))]
   public void FiHenkilotunnus_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Act.
      var sut = new FiHenkilotunnus(value);
      var expected = value.ToUpperInvariant();

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void FiHenkilotunnus_Constructor_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(dateOfBirth, centuryIndicator);

      // Act.
      var sut = new FiHenkilotunnus(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(value);
   }

   [Theory]
   [MemberData(nameof(ValidCenturyIndicatorValues))]
   public void FiHenkilotunnus_Constructor_ShouldCreateInstance_WhenValueHasValidCenturyIndicator(Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(centuryIndicator: centuryIndicator);
      var expected = value.ToUpperInvariant();

      // Act.
      var sut = new FiHenkilotunnus(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void FiHenkilotunnus_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new FiHenkilotunnus(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void FiHenkilotunnus_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new FiHenkilotunnus(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void FiHenkilotunnus_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new FiHenkilotunnus(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void FiHenkilotunnus_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new FiHenkilotunnus(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCenturyIndicatorValues))]
   public void FiHenkilotunnus_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCenturyIndicator(Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(centuryIndicator: centuryIndicator);
      LocalValidationError expected = GetInvalidCenturyResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new FiHenkilotunnus(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidIndividualNumberValues))]
   public void FiHenkilotunnus_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidIndividualNumber(String individualNumber)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(individualNumber: individualNumber);
      LocalValidationError expected = GetInvalidIndividualNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new FiHenkilotunnus(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void FiHenkilotunnus_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(dateOfBirth, centuryIndicator);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new FiHenkilotunnus(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   // 1800s
   [InlineData("010100", '+', "18000101")]      // January 1, 1800
   [InlineData("311299", '+', "18991231")]      // December 31, 1899

   // 1900s
   [InlineData("010100", '-', "19000101")]      // January 1, 1900
   [InlineData("311299", '-', "19991231")]      // December 31, 1999
   [InlineData("010100", 'U', "19000101")]      // January 1, 1900
   [InlineData("311299", 'U', "19991231")]      // December 31, 1999
   [InlineData("010100", 'V', "19000101")]      // January 1, 1900
   [InlineData("311299", 'V', "19991231")]      // December 31, 1999
   [InlineData("010100", 'W', "19000101")]      // January 1, 1900
   [InlineData("311299", 'W', "19991231")]      // December 31, 1999
   [InlineData("010100", 'X', "19000101")]      // January 1, 1900
   [InlineData("311299", 'X', "19991231")]      // December 31, 1999
   [InlineData("010100", 'Y', "19000101")]      // January 1, 1900
   [InlineData("311299", 'Y', "19991231")]      // December 31, 1999

   // 2000s
   [InlineData("010100", 'A', "20000101")]      // January 1, 2000
   [InlineData("311299", 'A', "20991231")]      // December 31, 2099
   [InlineData("010100", 'B', "20000101")]      // January 1, 2000
   [InlineData("311299", 'B', "20991231")]      // December 31, 2099
   [InlineData("010100", 'C', "20000101")]      // January 1, 2000
   [InlineData("311299", 'C', "20991231")]      // December 31, 2099
   [InlineData("010100", 'D', "20000101")]      // January 1, 2000
   [InlineData("311299", 'D', "20991231")]      // December 31, 2099
   [InlineData("010100", 'E', "20000101")]      // January 1, 2000
   [InlineData("311299", 'E', "20991231")]      // December 31, 2099
   [InlineData("010100", 'F', "20000101")]      // January 1, 2000
   [InlineData("311299", 'F', "20991231")]      // December 31, 2099

   // Maximum valid days per month
   [InlineData("310104", '+', "18040131")]      // maximum days for January, any year
   [InlineData("280201", '-', "19010228")]      // maximum days for February, non-leap year
   [InlineData("290204", 'U', "19040229")]      // maximum days for February, leap year
   [InlineData("290200", 'A', "20000229")]      // maximum days for February, leap year (2000 is leap-year)
   [InlineData("310304", '+', "18040331")]      // maximum days for March, any year
   [InlineData("300404", 'V', "19040430")]      // maximum days for April, any year
   [InlineData("310504", 'B', "20040531")]      // maximum days for May, any year
   [InlineData("300604", '+', "18040630")]      // maximum days for June, any year
   [InlineData("310704", 'W', "19040731")]      // maximum days for July, any year
   [InlineData("310804", 'C', "20040831")]      // maximum days for August, any year
   [InlineData("300904", '+', "18040930")]      // maximum days for September, any year
   [InlineData("311004", 'X', "19041031")]      // maximum days for October, any year
   [InlineData("301104", 'D', "20041130")]      // maximum days for November, any year
   [InlineData("311204", '+', "18041231")]      // maximum days for December, any year
   public void FiHenkilotunnus_DateOfBirth_ShouldReturnExpectedValue(
      String dateOfBirth,
      Char centuryIndicator,
      String expectedDateOfBirth)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(dateOfBirth, centuryIndicator);
      var sut = new FiHenkilotunnus(value);
      var expected = DateOnly.ParseExact(
         expectedDateOfBirth,
         "yyyyMMdd",
         CultureInfo.InvariantCulture);

      // Act/assert.
      sut.DateOfBirth.Should().Be(expected);
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
   public void FiHenkilotunnus_Gender_ShouldReturnMale_ForValuesWithOddGenderIndicator(Char gender)
   {
      // Arrange.
      var individualNumber = $"12{gender}";
      var value = GetHenkilotunnusWithValidCheckDigit(individualNumber: individualNumber);
      var sut = new FiHenkilotunnus(value);
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
   public void FiHenkilotunnus_Gender_ShouldReturnFemale_ForValuesWithEvenGenderIndicator(Char gender)
   {
      // Arrange.
      var individualNumber = $"12{gender}";
      var value = GetHenkilotunnusWithValidCheckDigit(individualNumber: individualNumber);
      var sut = new FiHenkilotunnus(value);
      Gender.BinaryGender expected = default(Gender.Female);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region IdentifierType Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("002")]
   [InlineData("101")]
   [InlineData("201")]
   [InlineData("301")]
   [InlineData("401")]
   [InlineData("501")]
   [InlineData("601")]
   [InlineData("701")]
   [InlineData("899")]
   public void FiHenkilotunnus_IdentifierType_ShouldReturnPermanentResident_WhenIdentifierNumberIsLessThan900(String individualNumber)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(individualNumber: individualNumber);
      var sut = new FiHenkilotunnus(value);
      FiHenkilotunnus.IdentifierCategory expected = default(FiIdentifierType.PermanentResident);

      // Act/assert.
      sut.IdentifierType.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData("900")]
   [InlineData("999")]
   public void FiHenkilotunnus_IdentifierType_ShouldReturnTemporary_WhenIdentifierNumberIsGreaterThanOrEqualTo900(String individualNumber)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(individualNumber: individualNumber);
      var sut = new FiHenkilotunnus(value);
      FiHenkilotunnus.IdentifierCategory expected = default(FiIdentifierType.Temporary);

      // Act/assert.
      sut.IdentifierType.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidHenkilotunnusValues))]
   public void FiHenkilotunnus_Value_ShouldReturnValidatedHenkilotunnus(String value)
   {
      // Arrange.
      var sut = new FiHenkilotunnus(value);
      var expected = value.ToUpperInvariant();

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FiHenkilotunnus_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidHenkilotunnus;
      var sut = new FiHenkilotunnus(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void FiHenkilotunnus_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = AltValidHenkilotunnus;
      var sut = new FiHenkilotunnus(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void FiHenkilotunnus_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      FiHenkilotunnus sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void FiHenkilotunnus_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      FiHenkilotunnus sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidHenkilotunnusValues))]
   public void FiHenkilotunnus_ExplicitCastToFiHenkilotunnus_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange
      var expected = new FiHenkilotunnus(value);

      // Act.
      var sut = (FiHenkilotunnus)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void FiHenkilotunnus_ExplicitCastToFiHenkilotunnus_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(dateOfBirth, centuryIndicator);
      var expected = new FiHenkilotunnus(value);

      // Act.
      var sut = (FiHenkilotunnus)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidCenturyIndicatorValues))]
   public void FiHenkilotunnus_ExplicitCastToFiHenkilotunnus_ShouldCreateInstance_WhenValueHasValidCenturyIndicator(Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(centuryIndicator: centuryIndicator);
      var expected = new FiHenkilotunnus(value);

      // Act.
      var sut = (FiHenkilotunnus)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void FiHenkilotunnus_ExplicitCastToFiHenkilotunnus_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FiHenkilotunnus)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void FiHenkilotunnus_ExplicitCastToFiHenkilotunnus_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FiHenkilotunnus)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void FiHenkilotunnus_ExplicitCastToFiHenkilotunnus_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FiHenkilotunnus)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void FiHenkilotunnus_ExplicitCastToFiHenkilotunnus_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FiHenkilotunnus)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCenturyIndicatorValues))]
   public void FiHenkilotunnus_ExplicitCastToFiHenkilotunnus_ShouldThrowKfValidationException_WhenValueHasInvalidCenturyIndicator(Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(centuryIndicator: centuryIndicator);
      LocalValidationError expected = GetInvalidCenturyResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FiHenkilotunnus)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidIndividualNumberValues))]
   public void FiHenkilotunnus_ExplicitCastToFiHenkilotunnus_ShouldThrowKfValidationException_WhenValueHasInvalidIndividualNumber(String individualNumber)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(individualNumber: individualNumber);
      LocalValidationError expected = GetInvalidIndividualNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FiHenkilotunnus)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void FiHenkilotunnus_ExplicitCastToFiHenkilotunnus_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(dateOfBirth, centuryIndicator);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FiHenkilotunnus)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FiHenkilotunnus_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new FiHenkilotunnus(ValidHenkilotunnus);
      var sut2 = new FiHenkilotunnus(ValidHenkilotunnus);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void FiHenkilotunnus_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new FiHenkilotunnus(ValidHenkilotunnus);
      var sut2 = new FiHenkilotunnus(AltValidHenkilotunnus);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Theory]
   [InlineData(ValidHenkilotunnus, ValidLowercaseHenkilotunnus)]
   [InlineData(AltValidHenkilotunnus, AltValidLowercaseHenkilotunnus)]
   [InlineData("160117A275C", "160117a275C")]
   [InlineData("160117A275C", "160117A275c")]
   public void FiHenkilotunnus_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyByCase(
      String value1,
      String value2)
   {
      // Arrange.
      var sut1 = new FiHenkilotunnus(value1);
      var sut2 = new FiHenkilotunnus(value2);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FiHenkilotunnus_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new FiHenkilotunnus(ValidHenkilotunnus);
      var sut2 = new FiHenkilotunnus(AltValidHenkilotunnus);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void FiHenkilotunnus_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      var sut1 = new FiHenkilotunnus(ValidHenkilotunnus);
      var sut2 = new FiHenkilotunnus(ValidHenkilotunnus);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Theory]
   [InlineData(ValidHenkilotunnus, ValidLowercaseHenkilotunnus)]
   [InlineData(AltValidHenkilotunnus, AltValidLowercaseHenkilotunnus)]
   [InlineData("160117A275C", "160117a275C")]
   [InlineData("160117A275C", "160117A275c")]
   public void FiHenkilotunnus_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyByCase(
      String value1,
      String value2)
   {
      // Arrange.
      var sut1 = new FiHenkilotunnus(value1);
      var sut2 = new FiHenkilotunnus(value2);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidHenkilotunnusValues))]
   public void FiHenkilotunnus_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new FiHenkilotunnus(value);

      // Act.
      var result = FiHenkilotunnus.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void FiHenkilotunnus_Create_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(dateOfBirth, centuryIndicator);
      LocalCreateResult expected = new FiHenkilotunnus(value);

      // Act.
      var result = FiHenkilotunnus.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidCenturyIndicatorValues))]
   public void FiHenkilotunnus_Create_ShouldCreateInstance_WhenValueHasValidCenturyIndicator(Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(centuryIndicator: centuryIndicator);
      LocalCreateResult expected = new FiHenkilotunnus(value);

      // Act.
      var result = FiHenkilotunnus.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void FiHenkilotunnus_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = FiHenkilotunnus.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void FiHenkilotunnus_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = FiHenkilotunnus.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void FiHenkilotunnus_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = FiHenkilotunnus.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void FiHenkilotunnus_Create_ShouldReturnInvalidCheckDigitValidationResult_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = FiHenkilotunnus.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCenturyIndicatorValues))]
   public void FiHenkilotunnus_Create_ShouldReturnInvalidCenturyValidationResult_WhenValueHasInvalidCenturyIndicator(Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(centuryIndicator: centuryIndicator);
      LocalCreateResult expected = (LocalValidationError)GetInvalidCenturyResult(value);

      // Act.
      var result = FiHenkilotunnus.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidIndividualNumberValues))]
   public void FiHenkilotunnus_Create_ShouldReturnInvalidIndividualNumberValidationResult_WhenValueHasInvalidIndividualNumber(String individualNumber)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(individualNumber: individualNumber);
      LocalCreateResult expected = (LocalValidationError)GetInvalidIndividualNumberResult(value);

      // Act.
      var result = FiHenkilotunnus.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void FiHenkilotunnus_Create_ShouldReturnDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(dateOfBirth, centuryIndicator);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = FiHenkilotunnus.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FiHenkilotunnus_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new FiHenkilotunnus(ValidHenkilotunnus);
      var sut2 = new FiHenkilotunnus(ValidHenkilotunnus);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void FiHenkilotunnus_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new FiHenkilotunnus(ValidHenkilotunnus);
      var sut2 = new FiHenkilotunnus(AltValidHenkilotunnus);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Theory]
   [InlineData(ValidHenkilotunnus, ValidLowercaseHenkilotunnus)]
   [InlineData(AltValidHenkilotunnus, AltValidLowercaseHenkilotunnus)]
   [InlineData("160117A275C", "160117a275C")]
   [InlineData("160117A275C", "160117A275c")]
   public void FiHenkilotunnus_Equals_ShouldReturnTrue_WhenValuesDifferOnlyByCase(
      String value1,
      String value2)
   {
      // Arrange.
      var sut1 = new FiHenkilotunnus(value1);
      var sut2 = new FiHenkilotunnus(value2);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FiHenkilotunnus_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new FiHenkilotunnus(ValidHenkilotunnus);
      var sut2 = new FiHenkilotunnus(ValidHenkilotunnus);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void FiHenkilotunnus_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new FiHenkilotunnus(ValidHenkilotunnus);
      var sut2 = new FiHenkilotunnus(AltValidHenkilotunnus);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Theory]
   [InlineData(ValidHenkilotunnus, ValidLowercaseHenkilotunnus)]
   [InlineData(AltValidHenkilotunnus, AltValidLowercaseHenkilotunnus)]
   [InlineData("160117A275C", "160117a275C")]
   [InlineData("160117A275C", "160117A275c")]
   public void FiHenkilotunnus_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyByCase(
      String value1,
      String value2)
   {
      // Arrange.
      var sut1 = new FiHenkilotunnus(value1);
      var sut2 = new FiHenkilotunnus(value2);

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

   // FiHenkilotunnus does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void FiHenkilotunnus_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new FiHenkilotunnus(ValidHenkilotunnus);
      var sut2 = new FiHenkilotunnus(ValidHenkilotunnus);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidHenkilotunnusValues))]
   public void FiHenkilotunnus_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new FiHenkilotunnus(value);
      var expected = value.ToUpperInvariant();

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidHenkilotunnusValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = FiHenkilotunnus.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnValidationPassed_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(dateOfBirth, centuryIndicator);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = FiHenkilotunnus.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidCenturyIndicatorValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnValidationPassed_WhenValueHasValidCenturyIndicator(Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(centuryIndicator: centuryIndicator);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = FiHenkilotunnus.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = FiHenkilotunnus.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = FiHenkilotunnus.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = FiHenkilotunnus.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = FiHenkilotunnus.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCenturyIndicatorValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnInvalidCenturyIndicator_WhenValueHasInvalidCenturyIndicator(Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(centuryIndicator: centuryIndicator);
      LocalValidationResult expected = GetInvalidCenturyResult(value);

      // Act.
      var result = FiHenkilotunnus.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidIndividualNumberValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnInvalidIndividualNumber_WhenValueHasInvalidIndividualNumber(String individualNumber)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(individualNumber: individualNumber);
      LocalValidationResult expected = GetInvalidIndividualNumberResult(value);

      // Act.
      var result = FiHenkilotunnus.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(dateOfBirth, centuryIndicator);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = FiHenkilotunnus.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FiHenkilotunnus_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new FiHenkilotunnus(ValidHenkilotunnus);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<FiHenkilotunnus>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void FiHenkilotunnus_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new FiHenkilotunnus(AltValidHenkilotunnus);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public FiHenkilotunnus Henkilotunnus { get; set; } = null!;
   }

   [Fact]
   public void FiHenkilotunnus_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Henkilotunnus = new FiHenkilotunnus(ValidHenkilotunnus) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void FiHenkilotunnus_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Henkilotunnus\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void FiHenkilotunnus_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Henkilotunnus\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Henkilotunnus.Should().BeNull();
   }

   [Fact]
   public void FiHenkilotunnus_JsonDeserialization_ShouldThrowKfValidationException_WhenHenkilotunnusIsInvalid()
   {
      // Arrange.
      var json = "{\"Henkilotunnus\":\"230626-034N\"}";  // Invalid check digit
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
