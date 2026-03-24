// Ignore Spelling: Fyrirtaeki Kennitala

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

   public static TheoryData<String> InvalidLengthValues =>
   [
      "120585436",        // Length 9
      "600520 36900",     // Length 13
      new String('1', 100) // Very long string
   ];

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

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidKennitalaValues))]
   public void IsKennitala_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => IsKennitala.Validate(value).Should().Be(IsKennitalaValidationResult.ValidationPassed);

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

   #endregion
}
