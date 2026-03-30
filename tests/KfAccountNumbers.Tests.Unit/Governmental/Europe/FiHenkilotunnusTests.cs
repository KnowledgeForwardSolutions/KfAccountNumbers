// Ignore Spelling: Fi Henkilotunnus

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class FiHenkilotunnusTests
{
   private const String ValidHenkilotunnus = "230526-034N";
   private const String AltValidHenkilotunnus = "160117A275C";
   private const String ValidTestHenkilotunnus = "020508D929B";
   private const String AltValidTestHenkilotunnus = "051272X990D";

   private static String GetHenkilotunnusWithValidCheckDigit(
      String dateOfBirth = "160117",
      Char centuryIndicator = 'A',
      String individualNumber = "275")
   {
      const String checkCharacters = "0123456789ABCDEFHJKLMNPRSTUVWXY";

      var temp = dateOfBirth + individualNumber;
      var sum = 0;
      foreach(var ch in temp)
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
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "230526-034",           // Length 10
      "160117A2754C",         // Length 12
      new String('1', 100)    // Very long string
   ];

   public static TheoryData<String> InvalidCharacterValues =>
   [
      "A30526-034N",          // Invalid character A
      "2 0526-034N",          // Invalid character space
      "23#526-034N",          // Invalid character #
      "230=26-034N",          // Invalid character =
      "2305c6-034N",          // Invalid character c
      "23052d-034N",          // Invalid character d
      "230526-^34N",          // Invalid character ^
      "230526-0~4N",          // Invalid character ~
      "230526-03\u00D6N",     // Invalid character unicode O with umlaut
      "230526-03\u00F6N",     // Invalid character unicode o with umlaut
   ];

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

   #region Constants Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FiHenkilotunnus_MinimumValidYearOfBirth_ShouldHaveExpectedValue()
      => FiHenkilotunnus.MinimumValidYearOfBirth.Should().Be(1800);

   [Fact]
   public void FiHenkilotunnus_MaximumValidYearOfBirth_ShouldHaveExpectedValue()
      => FiHenkilotunnus.MaximumValidYearOfBirth.Should().Be(2099);

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidHenkilotunnusValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => FiHenkilotunnus.Validate(value).Should().Be(FiHenkilotunnusValidationResult.ValidationPassed);

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => FiHenkilotunnus.Validate(value).Should().Be(FiHenkilotunnusValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => FiHenkilotunnus.Validate(value).Should().Be(FiHenkilotunnusValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnInvalidCharacters_WhenValueHasNonDigitCharacterWhereDigitExpected(String value)
      => FiHenkilotunnus.Validate(value).Should().Be(FiHenkilotunnusValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String value)
      => FiHenkilotunnus.Validate(value).Should().Be(FiHenkilotunnusValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(InvalidCenturyIndicatorValues))]
   public void FiHenkilotunnus_Validate_ShouldReturnInvalidCenturyIndicator_WhenValueHasInvalidCenturyIndicator(Char centuryIndicator)
   {
      // Arrange.
      var value = GetHenkilotunnusWithValidCheckDigit(centuryIndicator: centuryIndicator);

      // Act/assert.
      FiHenkilotunnus.Validate(value).Should().Be(FiHenkilotunnusValidationResult.InvalidCenturyIndicator);
   }

   [Fact]
   public void FiHenkilotunnus_Validate_ShouldReturnInvalidIndividualNumber_WhenValueHasInvalidIndividualNumber()
   {
      // Arrange.
      var individualNumber = "001";    // The only inavlid individual number
      var value = GetHenkilotunnusWithValidCheckDigit(individualNumber: individualNumber);

      // Act/assert.
      FiHenkilotunnus.Validate(value).Should().Be(FiHenkilotunnusValidationResult.InvalidIndividualNumber);
   }

   #endregion
}
