// Ignore Spelling: npi

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class UsNationalProviderIdentifierTests
{
   private const String _validNpi = "1245319599";     // Example from www.hippaspace.com

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnValidationPassed_WhenValueContainsValidNpi()
      => UsNationalProviderIdentifier.Validate(_validNpi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.ValidationPassed);

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.Empty);

   [Theory]
   [InlineData("124531959")]
   [InlineData("12453195999")]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.InvalidLength);

   [Theory]
   [InlineData("A245319599")]
   [InlineData("1A45319599")]
   [InlineData("12A5319599")]
   [InlineData("124A319599")]
   [InlineData("1245A19599")]
   [InlineData("12453A9599")]
   [InlineData("124531A599")]
   [InlineData("1245319A99")]
   [InlineData("12453195A9")]
   [InlineData("124531959A")]
   [InlineData("1;45319599")]
   [InlineData("1\u215345319599")]     // Unicode fraction 1/3
   [InlineData("1\u216745319599")]     // Unicode Roman numeral VII
   [InlineData("1\u0BEF45319599")]     // Unicode Tamil number 9
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidCharacterEncountered_WhenValueContainsNonAsciiDigit(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered);

   [Theory]
   [InlineData("1234569071")]    // Valid NPI 1234560971 with two digit transposition 09 -> 90
   [InlineData("1230967899")]    // Valid NPI 1239067899 with two digit transposition 90 -> 09
   [InlineData("1122334497")]    // Valid NPI 1122334497 with two digit twin error 22 -> 55
   [InlineData("1122337797")]    // Valid NPI 1122334497 with two digit twin error 44 -> 77
   [InlineData("1122664497")]    // Valid NPI 1122334497 with two digit twin error 33 -> 66
   public void UsNationalProviderIdentifier_Validate_ShouldReturnValidationPassed_WhenCheckDigitContainsUndetectableError(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.ValidationPassed);

   [Theory]
   [InlineData("1238560971")]    // Valid NPI 1234560971 with single digit transcription error 4 -> 8
   [InlineData("1243560971")]    // Valid NPI 1234560971 with two digit transposition error 34 -> 43
   [InlineData("4422334497")]    // Valid NPI 1122334497 with two digit twin error 11 -> 44
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidCheckDigit_WhenCheckDigitContainsDetectableError(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCheckDigit);

   #endregion
}
