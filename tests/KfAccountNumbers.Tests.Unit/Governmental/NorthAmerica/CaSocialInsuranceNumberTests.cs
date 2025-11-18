// Ignore Spelling: Luhn

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class CaSocialInsuranceNumberTests
{
   private const String ValidNineCharSin = "558199428";  // From singen.ca
   private const String ValidElevenCharSin = "558-199-428";
   private const String ValidElevenCharSinWithCustomSeparator = "558 199 428";

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSin)]
   [InlineData(ValidElevenCharSin)]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenValueContainsValidSin(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.Empty);

   [Theory]
   [InlineData("55819942")]
   [InlineData("5581994288")]
   [InlineData("558-199-4288")]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidLength);

   [Theory]
   [InlineData("046 454-286")]
   [InlineData("046-454 286")]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidSeparatorEncountered_When11CharacterValueContainsAnInvalidSeparator(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [InlineData("A46454286")]
   [InlineData("0A6454286")]
   [InlineData("04A454286")]
   [InlineData("046A54286")]
   [InlineData("0464A4286")]
   [InlineData("04645A286")]
   [InlineData("046454A86")]
   [InlineData("0464542A6")]
   [InlineData("04645428A")]
   [InlineData("0;6454286")]
   [InlineData("0\u21536454286")]      // Unicode fraction 1/3
   [InlineData("0\u21676454286")]      // Unicode Roman numeral VII
   [InlineData("0\u0BEF6454286")]      // Unicode Tamil number 9
   [InlineData("A46-454-286")]
   [InlineData("0A6-454-286")]
   [InlineData("04A-454-286")]
   [InlineData("046-A54-286")]
   [InlineData("046-4A4-286")]
   [InlineData("046-45A-286")]
   [InlineData("046-454-A86")]
   [InlineData("046-454-2A6")]
   [InlineData("046-454-28A")]
   [InlineData("0;6-454-286")]
   [InlineData("0\u21536-454-286")]      // Unicode fraction 1/3
   [InlineData("0\u21676-454-286")]      // Unicode Roman numeral VII
   [InlineData("0\u0BEF6-454-286")]      // Unicode Tamil number 9
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidCharacterEncountered_WhenValueContainsNonAsciiDigit(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [InlineData("012345674")]
   [InlineData("876543216")]
   [InlineData("012-345-674")]
   [InlineData("876-543-216")]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidProvince_WhenValueHasInvalidLeadingDigit(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidProvince);

   [Theory]
   [InlineData("100000017")]     // Note leading digit = 1. The best pattern for this test is to 
   [InlineData("100001007")]     // have only a single non-zero digit and the calculated check digit
   [InlineData("100100007")]     // but zero is not a valid leading digit for a SIN.
   [InlineData("110000007")]
   [InlineData("100-000-017")]
   [InlineData("100-001-007")]
   [InlineData("100-100-007")]
   [InlineData("110-000-007")]
   public void CaSocialInsightsNumber_Validate_ShouldCorrectlyWeightOddPositionCharacters_WhenCalculatingLuhnCheckDigit(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   [Theory]
   [InlineData("100000108")]     // Note leading digit = 1. The best pattern for this test is to 
   [InlineData("100010008")]     // have only a single non-zero digit and the calculated check digit
   [InlineData("101000008")]     // but zero is not a valid leading digit for a SIN.
   [InlineData("100000009")]
   [InlineData("100-000-108")]
   [InlineData("100-010-008")]
   [InlineData("101-000-008")]
   [InlineData("100-000-009")]
   public void CaSocialInsightsNumber_Validate_ShouldCorrectlyWeightEvenPositionCharacters_WhenCalculatingLuhnCheckDigit(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   [Theory]
   [InlineData("100000009")]
   [InlineData("100000017")]
   [InlineData("100000025")]
   [InlineData("100000033")]
   [InlineData("100000041")]
   [InlineData("100000058")]
   [InlineData("100000066")]
   [InlineData("100000074")]
   [InlineData("100000082")]
   [InlineData("100000090")]
   [InlineData("100-000-009")]
   [InlineData("100-100-007")]
   [InlineData("100-200-005")]
   [InlineData("100-300-003")]
   [InlineData("100-400-001")]
   [InlineData("100-500-008")]
   [InlineData("100-600-006")]
   [InlineData("100-700-004")]
   [InlineData("100-800-002")]
   [InlineData("100-900-000")]
   public void CaSocialInsightsNumber_Validate_ShouldCorrectlyDoubleOddPositionCharacters_WhenCalculatingLuhnCheckDigit(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   [Theory]
   [InlineData("780912341")]     // 789012341 with two digit transposition 90 -> 09
   [InlineData("123459018")]     // 123450918 with two digit transposition 09 -> 90
   [InlineData("100005503")]     // 100005503 with two digit twin error 55 -> 22
   [InlineData("107700007")]     // 104400007 with two digit twin error 44 -> 77
   [InlineData("103300000")]     // 106600000 with two digit twin error 66 -> 33
   [InlineData("558199428")]     // 558199428 with two digit jump transposition 994 -> 499
   [InlineData("780-912-341")]   // 789012341 with two digit transposition 90 -> 09
   [InlineData("123-459-018")]   // 123450918 with two digit transposition 09 -> 90
   [InlineData("100-005-503")]   // 100005503 with two digit twin error 55 -> 22
   [InlineData("107-700-007")]   // 104400007 with two digit twin error 44 -> 77
   [InlineData("103-300-000")]   // 106600000 with two digit twin error 66 -> 33
   [InlineData("558-199-428")]   // 558199428 with two digit jump transposition 994 -> 499
   public void CaSocialInsightsNumber_Validate_ShouldReturnValidationPassed_WhenCheckDigitContainsUndetectableError(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   [Theory]
   [InlineData("558299428")]     // 558199428 with single digit transcription error 1 -> 2
   [InlineData("559199428")]     // 558199428 with single digit transcription error 8 -> 9
   [InlineData("551899428")]     // 558199428 with two digit transcription error -> 81 -> 18
   [InlineData("558199248")]     // 558199428 with two digit transcription error -> 42 -> 24
   [InlineData("448199428")]     // 558199428 with two digit twin error 55 -> 44
   [InlineData("558188428")]     // 558199428 with two digit twin error 99 -> 88
   [InlineData("558-299-428")]   // 558199428 with single digit transcription error 1 -> 2
   [InlineData("559-199-428")]   // 558199428 with single digit transcription error 8 -> 9
   [InlineData("551-899-428")]   // 558199428 with two digit transcription error -> 81 -> 18
   [InlineData("558-199-248")]   // 558199428 with two digit transcription error -> 42 -> 24
   [InlineData("448-199-428")]   // 558199428 with two digit twin error 55 -> 44
   [InlineData("558-188-428")]   // 558199428 with two digit twin error 99 -> 88
   public void CaSocialInsightsNumber_Validate_ShouldReturnValidationPassed_WhenCheckDigitContainsDetectableError(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCheckDigit);

   [Fact]
   public void CaSocialInsightsNumber_Validate_ShouldReturnValidationPassed_WhenCheckDigitCalculatesAsZero()
      => CaSocialInsuranceNumber.Validate("123456790").Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   #endregion
}
