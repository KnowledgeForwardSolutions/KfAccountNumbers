using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.Governmental.Europe.SeIdentityNumber,
   KfAccountNumbers.Governmental.Europe.SeIdentityNumberBase.ValidationError>;
using LocalValidationError = KfAccountNumbers.Governmental.Europe.SeIdentityNumberBase.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.Governmental.Europe.SeIdentityNumber.ValidationError>;
using LocalValidationResult = KfAccountNumbers.Governmental.Europe.SeIdentityNumberBase.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class SeIdentityNumberTests : SeIdentityNumberTestsBase
{
   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.SeIdentityNumberInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(SeIdentityNumberBase.ShortFormatLength, Messages.SeIdentityNumberShortFormatLength),
            new ValidLengthDefinition(SeIdentityNumberBase.LongFormatLength, Messages.SeIdentityNumberLongFormatLength),
         ]);

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.SeIdentityNumberInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.SeIdentityNumberInvalidCheckDigit,
         Algorithms.Luhn.AlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(
         Messages.SeIdentityNumberInvalidSeparator,
         value[position],
         position);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
   {
      var isShortFormat = value.Length == SeIdentityNumberBase.ShortFormatLength;

      return new InvalidDateOfBirth(
         Messages.SeIdentityNumberInvalidDateOfBirth,
         isShortFormat ? value[..6] : value[..8],
         isShortFormat ? DateFormatName.YYMMDD : DateFormatName.YYYYMMDD);
   }

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SeIdentityNumber_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new SeIdentityNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerUndetectableCheckDigitErrors))]
   [MemberData(nameof(SamordningsnummerUndetectableCheckDigitErrors))]
   public void SeIdentityNumber_Constructor_ShouldCreateInstance_WhenCheckDigitHasUndetectableError(String value)
   {
      // Arrange.
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new SeIdentityNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SeIdentityNumber_Constructor_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new SeIdentityNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SeIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new SeIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SeIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new SeIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SeIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new SeIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SeIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new SeIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SeIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new SeIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerInvalidDateOfBirthValues))]
   [MemberData(nameof(SamordningsnummerInvalidDateOfBirthValues))]
   public void SeIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new SeIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }


   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SeIdentityNumber_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = SeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerUndetectableCheckDigitErrors))]
   [MemberData(nameof(SamordningsnummerUndetectableCheckDigitErrors))]
   public void SeIdentityNumber_Validate_ShouldReturnValidationPassed_WhenValueHasUndetectableCheckDigitError(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = SeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SeIdentityNumber_Validate_ShouldReturnValidationPassed_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = SeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SeIdentityNumber_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = SeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SeIdentityNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = SeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SeIdentityNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = SeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SeIdentityNumber_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = SeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SeIdentityNumber_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = SeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerInvalidDateOfBirthValues))]
   [MemberData(nameof(SamordningsnummerInvalidDateOfBirthValues))]
   public void SeIdentityNumber_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = SeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion
}
