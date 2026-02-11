// Ignore Spelling: itin Json

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class UsIndividualTaxpayerIdentificationNumberTests
{
   private const String ValidNineCharItin = "901501234";
   private const String AltValidNineCharItin = "987654321";
   private const String ValidElevenCharItin = "901-50-1234";
   private const String AltValidElevenCharItin = "987 65 4321";

   // Values that will successfully create a UsIndividualTaxpayerIdentificationNumber object
   public static TheoryData<String> ValidValues =>
   [
      ValidNineCharItin,
      ValidElevenCharItin,
      AltValidElevenCharItin
   ];

   public static TheoryData<String> ValidGroupNumberBoundaryValues =>
   [
      "900000123",
      "900650123",
      "900700123",
      "900880123",
      "900900123",
      "900920123",
      "900940123",
      "900990123",
   ];

   // Values that will report an invalid length
   public static TheoryData<String> InvalidLengthValues =>
   [
      "90150123",
      "9015012345",
      "901-50-123",
      "987 65 43210",
   ];

   // Values that will report an invalid area number (first digit must be 9)
   public static TheoryData<String> InvalidAreaNumberValues =>
   [
      "001501234",
      "101501234",
      "201501234",
      "301501234",
      "401501234",
      "501501234",
      "601501234",
      "701501234",
      "801501234",
      "001-50-1234",
      "101-50-1234",
      "201-50-1234",
      "301-50-1234",
      "401-50-1234",
      "501-50-1234",
      "601-50-1234",
      "701-50-1234",
      "801-50-1234",
   ];

   // Values that will report an invalid separator character
   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "901 50-1234",
      "901-50 1234",
      "901050 1234",
      "90105001234",
      "90115011234",
      "90125021234",
      "90135031234",
      "90145041234",
      "90155051234",
      "90165061234",
      "90175071234",
      "90185081234",
      "90195091234",
   ];

   // Values that will report an invalid character encountered
   public static TheoryData<String> InvalidCharacterValues =>
   [
      //"A01501234",    - will fail invalid area number validation before invalid character validation
      "9A1501234",
      "90A501234",
      "901A01234",
      "9015A1234",
      "90150A234",
      "901501A34",
      "9015012A4",
      "90150123A",
      "9;1501234",
      "9\u21531501234",       // Unicode fraction 1/3
      "9\u21671501234",       // Unicode Roman numeral VII
      "9\u0BEF1501234",       // Unicode Tamil number 9
      //"A01-50-1234",  - will fail invalid area number validation before invalid character validation
      "9A1-50-1234",
      "90A-50-1234",
      "901-A0-1234",
      "901-5A-1234",
      "901-50-A234",
      "901-50-1A34",
      "901-50-12A4",
      "901-50-123A",
      "9;1-50-1234",
      "9\u21531 50 1234",       // Unicode fraction 1/3
      "9\u21671 50 1234",       // Unicode Roman numeral VII
      "9\u0BEF1 50 1234",       // Unicode Tamil number 9
   ];

   // Values that will report an invalid group number (group number must be in the range 0-65, 70-88, 90-92 or 94-99)
   public static TheoryData<String> InvalidGroupNumberValues =>
   [
      "900660123",
      "900690123",
      "900890123",
      "900930123",
   ];

   public static TheoryData<String> EmptyItinValues =>
   [
      null!,
      String.Empty,
      "\t"
   ];

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnValidationPassed_WhenValueContainsValidItin(String itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(EmptyItinValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidAreaNumber_WhenValueHasInvalidAreaNumber(String itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidAreaNumber);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidSeparatorEncountered_When11CharacterValueContainsAnInvalidSeparator(String itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidCharacterEncountered_WhenValueContainsNonAsciiDigit(String itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidGroupNumberValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidGroupNumber_WhenValueHasInvalidGroupNumber(String itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidGroupNumber);

   #endregion
}
