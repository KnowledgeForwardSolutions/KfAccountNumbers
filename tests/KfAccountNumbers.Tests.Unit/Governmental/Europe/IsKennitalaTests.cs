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

   #endregion
}
