// Ignore Spelling: Burgerservicenummer

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class NlBurgerservicenummerTests
{
   private const String ValidBurgerservicenummer = "123456782";
   private const String AltValidBurgerservicenummer = "111222333";
   private const String ValidFormattedBurgerservicenummer = "1234-56-782";
   private const String AltValidFormattedBurgerservicenummer = "1112-22-333";
   private const String AltSeparatorCharBurgerservicenummer = "1638.97.426";

   public static TheoryData<String> ValidBurgerservicenummerValues =>
   [
      ValidBurgerservicenummer,
      AltValidBurgerservicenummer,
      ValidFormattedBurgerservicenummer,
      AltValidFormattedBurgerservicenummer,
      AltSeparatorCharBurgerservicenummer,
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "12345678",          // Length 8
      "1112223334",        // Length 10
      "1234-56-78",        // Length 10
      "1234-56-7823",      // Length 12
      new String('1', 100) // Very long string
   ];

   public static TheoryData<String> InvalidCharacterValues =>
   [
      "A23456782",         // Non-digit character 'A'
      "1 3456782",         // Non-digit character ' '
      "12-456782",         // Non-digit character '-'
      "123=56782",         // Non-digit character '='
      "1234B6782",         // Non-digit character 'B'
      "12345C782",         // Non-digit character 'C'
      "123456a82",         // Non-digit character 'a'
      "1234567b2",         // Non-digit character 'b'
      "12345678~",         // Non-digit character '~'
      "12345678\u2153",    // Non-digit character Unicode fraction 1/3

      "A234 56 782",       // Non-digit character 'A'
      "1 34 56 782",       // Non-digit character ' '
      "12-4 56 782",       // Non-digit character '-'
      "123= 56 782",       // Non-digit character '='
      "1234 B6 782",       // Non-digit character 'B'
      "1234 5C 782",       // Non-digit character 'C'
      "1234 56 a82",       // Non-digit character 'a'
      "1234 56 7b2",       // Non-digit character 'b'
      "1234 56 78~",       // Non-digit character '~'
      "1234 56 78\u2153",  // Non-digit character Unicode fraction 1/3
   ];

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "122456782",         // 123456782 with single digit transcription error, 3 -> 2
      "111223333",         // 111222333 with single digit transcription error, 2 -> 3
      "123456783",         // 123456782 with check digit transcription error, 3 -> 2
      "124356782",         // 123456782 with two digit transposition error, 34 -> 43
      "112122333",         // 111222333 with two digit transposition error, 12 -> 21
      "123458762",         // 123456782 with two digit jump transposition, 678 -> 876
      "100222333",         // 111222333 with two digit twin error, 11 -> 00
      "111222344",         // 111222333 with two digit twin error, 33 -> 44
   ];

   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "1234056-782",
      "1234156-782",
      "1234256-782",
      "1234356-782",
      "1234456-782",
      "1234556-782",
      "1234656-782",
      "1234756-782",
      "1234856-782",
      "1234956-782",

      "1234-560782",
      "1234-561782",
      "1234-562782",
      "1234-563782",
      "1234-564782",
      "1234-565782",
      "1234-566782",
      "1234-567782",
      "1234-568782",
      "1234-569782",

      "1234-56.782",
      "1234.56-782",
   ];

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidBurgerservicenummerValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.ValidationPassed);

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacterWhereDigitExpected(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidCheckDigit(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidSeparator(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.InvalidSeparator);

   #endregion
}
