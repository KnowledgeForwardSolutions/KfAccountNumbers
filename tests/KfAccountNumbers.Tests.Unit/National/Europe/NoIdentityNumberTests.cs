#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.National.Europe.NoIdentityNumber,
   KfAccountNumbers.National.Europe.NoIdentityNumber.ValidationError>;
using LocalValidationError = KfAccountNumbers.National.Europe.NoIdentityNumber.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.National.Europe.NoIdentityNumber.ValidationError>;
using LocalValidationResult = KfAccountNumbers.National.Europe.NoIdentityNumber.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.National.Europe;

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

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   [MemberData(nameof(ValidDnummerValues))]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoIdentityNumber_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new NoIdentityNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void NoIdentityNumber_Constructor_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("600505", separator, "294");
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new NoIdentityNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerValidDateOfBirthValues))]
   [MemberData(nameof(DnummerValidDateOfBirthValues))]
   [MemberData(nameof(HnummerValidDateOfBirthValues))]
   public void NoIdentityNumber_Constructor_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new NoIdentityNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NoIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NoIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NoIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NoIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new NoIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void NoIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(separator: separator);
      LocalValidationError expected = GetInvalidSeparatorResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerInvalidDateOfBirthValues))]
   [MemberData(nameof(DnummerInvalidDateOfBirthValues))]
   [MemberData(nameof(HnummerInvalidDateOfBirthValues))]
   public void NoIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String individualNumber)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         individualNumber: individualNumber);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region IdentifierType Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   public void NoIdentityNumber_IdentifierType_ShouldReturnExpectedIdentifierType_WhenValueIsFoedselsnummer(String value)
   {
      // Arrange.
      var sut = new NoIdentityNumber(value);
      NoIdentityNumberBase.IdentifierCategory expected = default(NoIdentifierType.Foedselsnummer);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDnummerValues))]
   public void NoIdentityNumber_IdentifierType_ShouldReturnExpectedIdentifierType_WhenValueIsDnummer(String value)
   {
      // Arrange.
      var sut = new NoIdentityNumber(value);
      NoIdentityNumberBase.IdentifierCategory expected = default(NoIdentifierType.Dnummer);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoIdentityNumber_IdentifierType_ShouldReturnExpectedIdentifierType_WhenValueIsHnummer(String value)
   {
      // Arrange.
      var sut = new NoIdentityNumber(value);
      NoIdentityNumberBase.IdentifierCategory expected = default(NoIdentifierType.Hnummer);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   [MemberData(nameof(ValidDnummerValues))]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoIdentityNumber_Value_ShouldReturnValidatedIdentityNumber(String value)
   {
      // Arrange.
      var sut = new NoIdentityNumber(value);
      var expected = GetNormalizedValue(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoIdentityNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedFoedselsnummer;
      var sut = new NoIdentityNumber(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void NoIdentityNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedDnummer;
      var sut = new NoIdentityNumber(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void NoIdentityNumber_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      NoIdentityNumber sut = null!;

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void NoIdentityNumber_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      NoIdentityNumber sut = null!;

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   [MemberData(nameof(ValidDnummerValues))]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoIdentityNumber_ExplicitCastToNoIdentityNumber_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new NoIdentityNumber(value);

      // Act.
      var sut = (NoIdentityNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void NoIdentityNumber_ExplicitCastToNoIdentityNumber_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("600505", separator, "294");
      var expected = new NoIdentityNumber(value);

      // Act.
      var sut = (NoIdentityNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerValidDateOfBirthValues))]
   [MemberData(nameof(DnummerValidDateOfBirthValues))]
   [MemberData(nameof(HnummerValidDateOfBirthValues))]
   public void NoIdentityNumber_ExplicitCastToNoIdentityNumber_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      var expected = new NoIdentityNumber(value);

      // Act.
      var sut = (NoIdentityNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NoIdentityNumber_ExplicitCastToNoIdentityNumber_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NoIdentityNumber_ExplicitCastToNoIdentityNumber_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NoIdentityNumber_ExplicitCastToNoIdentityNumber_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NoIdentityNumber_ExplicitCastToNoIdentityNumber_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void NoIdentityNumber_ExplicitCastToNoIdentityNumber_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(separator: separator);
      LocalValidationError expected = GetInvalidSeparatorResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerInvalidDateOfBirthValues))]
   [MemberData(nameof(DnummerInvalidDateOfBirthValues))]
   [MemberData(nameof(HnummerInvalidDateOfBirthValues))]
   public void NoIdentityNumber_ExplicitCastToNoIdentityNumber_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String individualNumber)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         individualNumber: individualNumber);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoIdentityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);
      var sut2 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void NoIdentityNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);
      var sut2 = new NoIdentityNumber(AltValidUnformattedFoedselsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void NoIdentityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var sut1 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);
      var sut2 = new NoIdentityNumber(ValidFormattedFoedselsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void NoIdentityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidFormattedDnummer);
      var sut2 = new NoIdentityNumber(ValidFormattedDnummer.Replace(' ', '.'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void NoIdentityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidFormattedDnummer.Replace(' ', 'A'));
      var sut2 = new NoIdentityNumber(ValidFormattedDnummer.Replace(' ', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoIdentityNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);
      var sut2 = new NoIdentityNumber(AltValidUnformattedFoedselsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void NoIdentityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var sut1 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);
      var sut2 = new NoIdentityNumber(ValidFormattedFoedselsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void NoIdentityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);
      var sut2 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void NoIdentityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidFormattedDnummer);
      var sut2 = new NoIdentityNumber(ValidFormattedDnummer.Replace(' ', '.'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void NoIdentityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidFormattedDnummer.Replace(' ', 'A'));
      var sut2 = new NoIdentityNumber(ValidFormattedDnummer.Replace(' ', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   [MemberData(nameof(ValidDnummerValues))]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoIdentityNumber_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new NoIdentityNumber(value);

      // Act.
      var result = NoIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void NoIdentityNumber_Create_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("600505", separator, "294");
      LocalCreateResult expected = new NoIdentityNumber(value);

      // Act.
      var result = NoIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerValidDateOfBirthValues))]
   [MemberData(nameof(DnummerValidDateOfBirthValues))]
   [MemberData(nameof(HnummerValidDateOfBirthValues))]
   public void NoIdentityNumber_Create_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      LocalCreateResult expected = new NoIdentityNumber(value);

      // Act.
      var result = NoIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NoIdentityNumber_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = NoIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NoIdentityNumber_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = NoIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NoIdentityNumber_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = NoIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NoIdentityNumber_Create_ShouldReturnInvalidCheckDigitsValidationResult_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = NoIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void NoIdentityNumber_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(separator: separator);
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value);

      // Act.
      var result = NoIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerInvalidDateOfBirthValues))]
   [MemberData(nameof(DnummerInvalidDateOfBirthValues))]
   [MemberData(nameof(HnummerInvalidDateOfBirthValues))]
   public void NoIdentityNumber_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String individualNumber)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         individualNumber: individualNumber);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = NoIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoIdentityNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);
      var sut2 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NoIdentityNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidUnformattedDnummer);
      var sut2 = new NoIdentityNumber(AltValidUnformattedDnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void NoIdentityNumber_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var sut1 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);
      var sut2 = new NoIdentityNumber(ValidFormattedFoedselsnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NoIdentityNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidFormattedDnummer);
      var sut2 = new NoIdentityNumber(ValidFormattedDnummer.Replace(' ', '.'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NoIdentityNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidFormattedFoedselsnummer.Replace(' ', 'A'));
      var sut2 = new NoIdentityNumber(ValidFormattedFoedselsnummer.Replace(' ', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NoIdentityNumber_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new NoIdentityNumber(ValidFormattedFoedselsnummer);

      // Act/assert.
      sut.Equals(ValidFormattedFoedselsnummer).Should().BeFalse();
   }

   [Fact]
   public void NoIdentityNumber_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new NoIdentityNumber(ValidFormattedFoedselsnummer);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoIdentityNumber_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new NoIdentityNumber(ValidUnformattedFoedselsnummer);
      var expected = ValidFormattedFoedselsnummer;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void NoIdentityNumber_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new NoIdentityNumber(AltValidUnformattedDnummer);
      var mask = "______-_____";
      var expected = AltValidFormattedDnummer;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void NoIdentityNumber_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new NoIdentityNumber(ValidUnformattedDnummer);
      String mask = null!;

      // Act/assert.
      FluentActions
         .Invoking(() => _ = sut.Format(mask))
         .Should()
         .ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(mask))
         .WithMessage(Messages.FormatMaskEmpty + "*");
   }

   [Theory]
   [InlineData("")]
   [InlineData("\t")]
   public void NoIdentityNumber_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new NoIdentityNumber(AltValidUnformattedDnummer);
      var expectedMessage = Messages.FormatMaskEmpty + "*";
      var act = () => _ = sut.Format(mask);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(mask))
         .WithMessage(expectedMessage);
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoIdentityNumber_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);
      var sut2 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void NoIdentityNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidUnformattedDnummer);
      var sut2 = new NoIdentityNumber(AltValidUnformattedDnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void NoIdentityNumber_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var sut1 = new NoIdentityNumber(ValidUnformattedDnummer);
      var sut2 = new NoIdentityNumber(ValidFormattedDnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void NoIdentityNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidFormattedFoedselsnummer);
      var sut2 = new NoIdentityNumber(ValidFormattedFoedselsnummer.Replace(' ', '.'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void NoIdentityNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidFormattedDnummer.Replace(' ', 'A'));
      var sut2 = new NoIdentityNumber(ValidFormattedDnummer.Replace(' ', 'a'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   #endregion

   #region ReferenceEquals Method Tests
   // ==========================================================================
   // ==========================================================================

   // NoIdentityNumber does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void NoIdentityNumber_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);
      var sut2 = new NoIdentityNumber(ValidUnformattedFoedselsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToDnummer Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidDnummerValues))]
   public void NoIdentityNumber_ToDnummer_ShouldReturnExpectedResult_WhenValueIsDnummer(String value)
   {
      // Arrange.
      var sut = new NoIdentityNumber(value);
      var expected = new NoDnummer(value);

      // Act.
      KfOption<NoDnummer> result = sut.ToDnummer();

      // Assert.
      result.Value.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoIdentityNumber_ToDnummer_ShouldReturnExpectedResult_WhenValueIsNotDnummer(String value)
   {
      // Arrange.
      var sut = new NoIdentityNumber(value);
      var expected = default(None);

      // Act.
      KfOption<NoDnummer> result = sut.ToDnummer();

      // Assert.
      result.Value.Should().Be(expected);
   }

   #endregion

   #region ToFoedselsnummer Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   public void NoIdentityNumber_ToFoedselsnummer_ShouldReturnExpectedResult_WhenValueIsFoedselsnummer(String value)
   {
      // Arrange.
      var sut = new NoIdentityNumber(value);
      var expected = new NoFoedselsnummer(value);

      // Act.
      KfOption<NoFoedselsnummer> result = sut.ToFoedselsnummer();

      // Assert.
      result.Value.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDnummerValues))]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoIdentityNumber_ToFoedselsnummer_ShouldReturnExpectedResult_WhenValueIsNotFoedselsnummer(String value)
   {
      // Arrange.
      var sut = new NoIdentityNumber(value);
      var expected = default(None);

      // Act.
      KfOption<NoFoedselsnummer> result = sut.ToFoedselsnummer();

      // Assert.
      result.Value.Should().Be(expected);
   }

   #endregion

   #region ToHnummer Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoIdentityNumber_ToHnummer_ShouldReturnExpectedResult_WhenValueIsHnummer(String value)
   {
      // Arrange.
      var sut = new NoIdentityNumber(value);
      var expected = new NoHnummer(value);

      // Act.
      KfOption<NoHnummer> result = sut.ToHnummer();

      // Assert.
      result.Value.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   [MemberData(nameof(ValidDnummerValues))]
   public void NoIdentityNumber_ToHnummer_ShouldReturnExpectedResult_WhenValueIsNotHnummer(String value)
   {
      // Arrange.
      var sut = new NoIdentityNumber(value);
      var expected = default(None);

      // Act.
      KfOption<NoHnummer> result = sut.ToHnummer();

      // Assert.
      result.Value.Should().Be(expected);
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   [MemberData(nameof(ValidDnummerValues))]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoIdentityNumber_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new NoIdentityNumber(value);

      // Act/assert.
      sut.ToString().Should().Be(sut.Value);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   [MemberData(nameof(ValidDnummerValues))]
   [MemberData(nameof(ValidHnummerValues))]
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
   [MemberData(nameof(HnummerValidDateOfBirthValues))]
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
   [MemberData(nameof(HnummerInvalidDateOfBirthValues))]
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

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoIdentityNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new NoIdentityNumber(ValidUnformattedDnummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<NoIdentityNumber>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void NoIdentityNumber_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new NoIdentityNumber(ValidFormattedDnummer);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public NoIdentityNumber IdentityNumber { get; set; } = null!;
   }

   [Fact]
   public void NoIdentityNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { IdentityNumber = new NoIdentityNumber(ValidFormattedDnummer) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void NoIdentityNumber_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"IdentityNumber\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void NoIdentityNumber_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"IdentityNumber\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.IdentityNumber.Should().BeNull();
   }

   [Fact]
   public void NoIdentityNumber_JsonDeserialization_ShouldThrowKfValidationException_WhenValueIsInvalid()
   {
      // Arrange.
      var json = "{\"IdentityNumber\":\"13039597140\"}";  // Invalid check digits
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
