#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.Governmental.Europe.NoIdentityNumber,
   KfAccountNumbers.Governmental.Europe.NoIdentityNumberBase.ValidationError>;
using LocalValidationError = KfAccountNumbers.Governmental.Europe.NoIdentityNumberBase.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.Governmental.Europe.NoIdentityNumberBase.ValidationError>;
using LocalValidationResult = KfAccountNumbers.Governmental.Europe.NoIdentityNumberBase.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class NoIdentityNumberTests : NoIdentityNumberTestsBase
{
   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.NoIdentityNumberInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(NoIdentityNumberBase.UnformattedLength, Messages.NoIdentityNumberUnformattedLength),
            new ValidLengthDefinition(NoIdentityNumberBase.FormattedLength, Messages.NoIdentityNumberFormattedLength),
         ]);

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.NoIdentityNumberInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.NoIdentityNumberInvalidCheckDigits,
         NoIdentityNumberBase.CheckDigitAlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(String value)
      => new(Messages.NoIdentityNumberInvalidSeparator, value[6], 6);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(Messages.NoIdentityNumberInvalidDateOfBirth, value[..6], DateFormatName.DDMMYY);

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   [MemberData(nameof(ValidDNummerValues))]
   public void NoIdentityNumber_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NoIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void NoIdentityNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("600505", separator, "294");
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NoIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerValidDateOfBirthValues))]
   [MemberData(nameof(DnummerValidDateOfBirthValues))]
   public void NoIdentityNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NoIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NoIdentityNumber_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = NoIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NoIdentityNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = NoIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NoIdentityNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = NoIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NoIdentityNumber_Validate_ShouldReturnInvalidCheckDigits_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = NoIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void NoIdentityNumber_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(separator: separator);
      LocalValidationResult expected = GetInvalidSeparatorResult(value);

      // Act.
      var result = NoIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerInvalidDateOfBirthValues))]
   [MemberData(nameof(DnummerInvalidDateOfBirthValues))]
   public void NoIdentityNumber_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String individualNumber)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         individualNumber: individualNumber);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = NoIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion
}
