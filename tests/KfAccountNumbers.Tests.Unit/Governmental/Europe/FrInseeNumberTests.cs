// Ignore Spelling: Insee

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

public class FrInseeNumberTests
{
   private const String Valid15CharacterInseeNumber = "188121884813236";
   private const String AltValid15CharacterInseeNumber = "255102445387701";
   private const String Valid15CharacterInseeNumberCorsica = "112072A28806058";
   private const String Valid15CharacterTemporaryInseeNumber = "821099901013371";
   private const String Valid21CharacterInseeNumber = "1 88 12 18 848 132 36";
   private const String AltValid21CharacterInseeNumber = "2 55 10 24 453 877 01";
   private const String Valid21CharacterInseeNumberCorsica = "1 12 07 2A 288 060 58";
   private const String Valid21CharacterTemporaryInseeNumber = "8 21 09 99 010 133 71";


   public static TheoryData<String> ValidInseeNumbers =>
   [
      Valid15CharacterInseeNumber,
      AltValid15CharacterInseeNumber,
      Valid15CharacterInseeNumberCorsica,
      Valid15CharacterTemporaryInseeNumber,
      Valid21CharacterInseeNumber,
      AltValid21CharacterInseeNumber,
      Valid21CharacterInseeNumberCorsica,
      Valid21CharacterTemporaryInseeNumber,

      "295109912611193",
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "18812188481326",                // Length 14
      "1881218848132613",              // Length 16
      "1 88 12 18 848 132 6",          // Length 20
      "1 88 12 18 848 132 613",        // Length 22
      new String('1', 100)    // Very long string
   ];

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidInseeNumbers))]
   public void FrInseeNumber_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.ValidationPassed);

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void FrInseeNumber_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.InvalidLength);

   #endregion
}
