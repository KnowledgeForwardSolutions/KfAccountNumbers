// Ignore Spelling: Personnummer

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class DkPersonnummerTests
{
   private const String Valid10CharacterPersonnummer = "0707614285";
   private const String Valid11CharacterPersonnummer = "070761-4285";
   private const String AltValid10CharacterPersonnummer = "0102036234";
   private const String AltValid11CharacterPersonnummer = "010203-6234";

   public static TheoryData<String> ValidPersonnummerValues =>
   [
      Valid10CharacterPersonnummer,
      Valid11CharacterPersonnummer,
      AltValid10CharacterPersonnummer,
      AltValid11CharacterPersonnummer,
   ];

   public static TheoryData<Char> ValidCenturyIndicators =>
   [
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "070761428",        // Length 9
      "070761-42856",     // Length 13
      new String('1', 100) // Very long string
   ];

   public static TheoryData<String> InvalidCharacterValues =>
   [
      "A707614285",           // Non-digit character 'A'
      "0 07614285",           // Non-digit character ' '
      "07-7614285",           // Non-digit character '-'
      "070=614285",           // Non-digit character '='
      "0707B14285",           // Non-digit character 'B'
      "07076C4285",           // Non-digit character 'C'
      "070761a285",           // Non-digit character 'a'
      "0707614b85",           // Non-digit character 'b'
      "07076142~5",           // Non-digit character '~'
      "070761428\u2153",      // Non-digit character Unicode fraction 1/3

      "A70761-4285",          // Non-digit character 'A'
      "0 0761-4285",          // Non-digit character ' '
      "07-761-4285",          // Non-digit character '-'
      "070=61-4285",          // Non-digit character '='
      "0707B1-4285",          // Non-digit character 'B'
      "07076C-4285",          // Non-digit character 'C'
      "070761-a285",          // Non-digit character 'a'
      "070761-4b85",          // Non-digit character 'b'
      "070761-42~5",          // Non-digit character '~'
      "070761-428\u2153",     // Non-digit character Unicode fraction 1/3
   ];

   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "070761 4285",
      "070761=4285",
      "070761~4285",
      "070761\u21534285",
   ];

   public static TheoryData<String> InvalidDateOfBirthValues =>
   [
      // It's not really possible to represent dates outside the valid
      // range of 1858 - 2057. So instead, focus on invalid day/month values.
      "0100000112",        // Invalid month = 0
      "0113000112",        // Invalid month = 13
      "000100-0112",       // Invalid day = 0

      "3201040112",        // Invalid day of month for January, any year
      "290203-1112",       // Invalid day of month for February, non-leap year
      "300204-2112",       // Invalid day of month for February, leap year
      "300200-4112",       // Invalid day of month for February, leap year (2000 is leap-year)
      "7203043112",        // Invalid day of for March, any year
      "710404-5112",       // Invalid day of for April, any year
      "7205046112",        // Invalid day of for May, any year
      "710604-7112",       // Invalid day of for June, any year
      "7207048112",        // Invalid day of for July, any year
      "720804-9112",       // Invalid day of for August, any year
      "7109040112",        // Invalid day of for September, any year
      "721004-1112",       // Invalid day of for October, any year
      "7111042112",        // Invalid day of for November, any year
      "721204-3112",       // Invalid day of for December, any year
   ];

   #region Constants Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DkPersonnummer_MinimumValidYearOfBirth_ShouldHaveExpectedValue()
      => DkPersonnummer.MinimumValidYearOfBirth.Should().Be(1858);

   [Fact]
   public void DkPersonnummer_MaximumValidYearOfBirth_ShouldHaveExpectedValue()
      => DkPersonnummer.MaximumValidYearOfBirth.Should().Be(2057);

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void DkPersonnummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(ValidCenturyIndicators))]
   public void DkPersonnummer_Validate_ShouldReturnValidationPassed_WhenMinimumDateOfBirthForAllCenturyIndicators(Char centuryIndicator)
   {
      // Arrange.
      var value = $"010100{centuryIndicator}123";

      // Act/assert.
      DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.ValidationPassed);
   }

   [Theory]
   [MemberData(nameof(ValidCenturyIndicators))]
   public void DkPersonnummer_Validate_ShouldReturnValidationPassed_WhenMaximumDateOfBirthForAllCenturyIndicators(Char centuryIndicator)
   {
      // Arrange.
      var value = $"311299{centuryIndicator}123";

      // Act/assert.
      DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.ValidationPassed);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void DkPersonnummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DkPersonnummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DkPersonnummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(String value)
      => DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DkPersonnummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String value)
      => DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.InvalidSeparator);

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void DkPersonnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(String value)
      => DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.InvalidDateOfBirth);

   #endregion
}
