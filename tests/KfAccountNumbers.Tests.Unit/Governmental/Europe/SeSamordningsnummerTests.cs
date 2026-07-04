using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.Governmental.Europe.SeSamordningsnummer,
   KfAccountNumbers.Governmental.Europe.SeIdentityNumberBase.ValidationError>;
using LocalValidationError = KfAccountNumbers.Governmental.Europe.SeIdentityNumberBase.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.Governmental.Europe.SeIdentityNumberBase.ValidationError>;
using LocalValidationResult = KfAccountNumbers.Governmental.Europe.SeIdentityNumberBase.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class SeSamordningsnummerTests : SeIdentityNumberTestsBase
{
   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.SeSamordningsnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(SeIdentityNumberBase.ShortFormatLength, Messages.SeSamordningsnummerShortFormatLength),
            new ValidLengthDefinition(SeIdentityNumberBase.LongFormatLength, Messages.SeSamordningsnummerLongFormatLength),
         ]);

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.SeSamordningsnummerInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.SeSamordningsnummerInvalidCheckDigit,
         Algorithms.Luhn.AlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(
         Messages.SeSamordningsnummerInvalidSeparator,
         value[position],
         position);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
   {
      var isShortFormat = value.Length == SeIdentityNumberBase.ShortFormatLength;

      return new InvalidDateOfBirth(
         Messages.SeSamordingsnummerrInvalidDateOfBirth,
         isShortFormat ? value[..6] : value[..8],
         isShortFormat ? DateFormatName.YYMMDD : DateFormatName.YYYYMMDD);
   }

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SeSamordningsnummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = SeSamordningsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerUndetectableCheckDigitErrors))]
   public void SeSamordningsnummer_Validate_ShouldReturnValidationPassed_WhenValueHasUndetectableCheckDigitError(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = SeSamordningsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SeSamordningsnummer_Validate_ShouldReturnValidationPassed_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = SeSamordningsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SeSamordningsnummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = SeSamordningsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SeSamordningsnummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = SeSamordningsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SeSamordningsnummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = SeSamordningsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SeSamordningsnummer_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = SeSamordningsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SeSamordningsnummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = SeSamordningsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerInvalidDateOfBirthValues))]
   public void SeSamordningsnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = SeSamordningsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion
}
