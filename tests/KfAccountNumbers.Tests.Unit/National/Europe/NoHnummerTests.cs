#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.National.Europe.NoHnummer,
   KfAccountNumbers.National.Europe.NoHnummer.ValidationError>;
using LocalValidationError = KfAccountNumbers.National.Europe.NoHnummer.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.National.Europe.NoHnummer.ValidationError>;
using LocalValidationResult = KfAccountNumbers.National.Europe.NoHnummer.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class NoHnummerTests : NoIdentityNumberTestsBase
{
   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.NoHnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(NoIdentityNumberBase.UnformattedLength, Messages.NoHnummerUnformattedLength),
            new ValidLengthDefinition(NoIdentityNumberBase.FormattedLength, Messages.NoHnummerFormattedLength),
         ]);

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.NoHnummerInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.NoHnummerInvalidCheckDigits,
         NoIdentityNumberBase.CheckDigitAlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(String value)
      => new(Messages.NoHnummerInvalidSeparator, value[6], 6);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(Messages.NoHnummerInvalidDateOfBirth, value[..6], DateFormatName.DDMMYY);

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoHnummer_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new NoHnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void NoHnummer_Constructor_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("204505", separator, "294");
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new NoHnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(HnummerValidDateOfBirthValues))]
   public void NoHnummer_Constructor_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new NoHnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NoHnummer_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoHnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NoHnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoHnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NoHnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoHnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NoHnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new NoHnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void NoHnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(separator: separator);
      LocalValidationError expected = GetInvalidSeparatorResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoHnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(HnummerInvalidDateOfBirthValues))]
   public void NoHnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
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
         .Invoking(() => new NoHnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   [MemberData(nameof(ValidDnummerValues))]
   [MemberData(nameof(ValidFhnummerValues))]
   public void NoHnummer_Constructor_ShouldThrowKfValidationException_WhenValueIsNotHnummer(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoHnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(HnummerValidDateOfBirthValues))]
   public void NoHnummer_DateOfBirth_ShouldReturnExpectedValue(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String expectedDateOfBirth)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         dateOfBirth,
         separator,
         individualNumber);
      var sut = new NoHnummer(value);
      var expected = DateOnly.ParseExact(
         expectedDateOfBirth,
         "yyyyMMdd",
         CultureInfo.InvariantCulture);

      // Act/assert.
      sut.DateOfBirth.Should().Be(expected);
   }

   #endregion

   #region Gender Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("641", "")]    // expected individual number 541 changed to 641 because of check digit constraints
   [InlineData("543", "")]
   [InlineData("645", "")]    // expected individual number 545 changed to 645 because of check digit constraints
   [InlineData("547", "")]
   [InlineData("549", "")]
   [InlineData("641", " ")]   // expected individual number 541 changed to 641 because of check digit constraints
   [InlineData("543", " ")]
   [InlineData("645", " ")]   // expected individual number 545 changed to 645 because of check digit constraints
   [InlineData("547", " ")]
   [InlineData("549", " ")]
   public void NoHnummer_Gender_ShouldReturnMale_ForValuesWithOddGenderIndicator(
      String individualNumber,
      String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("204505", separator, individualNumber);
      var sut = new NoHnummer(value);
      Gender.BinaryGender expected = default(Gender.Male);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData("540", "")]
   [InlineData("542", "")]
   [InlineData("544", "")]
   [InlineData("546", "")]
   [InlineData("548", "")]
   [InlineData("540", " ")]
   [InlineData("542", " ")]
   [InlineData("544", " ")]
   [InlineData("546", " ")]
   [InlineData("548", " ")]
   public void NoHnummer_Gender_ShouldReturnFemale_ForValuesWithEvenGenderIndicator(
      String individualNumber,
      String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("204505", separator, individualNumber);
      var sut = new NoHnummer(value);
      Gender.BinaryGender expected = default(Gender.Female);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoHnummer_Value_ShouldReturnValidatedHnummer(String value)
   {
      // Arrange.
      var sut = new NoHnummer(value);
      var expected = GetNormalizedValue(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoHnummer_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidUnformattedHnummer;
      var sut = new NoHnummer(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void NoHnummer_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedHnummer;
      var sut = new NoHnummer(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void NoHnummer_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      NoHnummer sut = null!;

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void NoHnummer_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      NoHnummer sut = null!;

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoHnummer_ExplicitCastToNoHnummer_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new NoHnummer(value);

      // Act.
      var sut = (NoHnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void NoHnummer_ExplicitCastToNoHnummer_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("204505", separator, "294");
      var expected = new NoHnummer(value);

      // Act.
      var sut = (NoHnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(HnummerValidDateOfBirthValues))]
   public void NoHnummer_ExplicitCastToNoHnummer_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      var expected = new NoHnummer(value);

      // Act.
      var sut = (NoHnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NoHnummer_ExplicitCastToNoHnummer_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoHnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NoHnummer_ExplicitCastToNoHnummer_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoHnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NoHnummer_ExplicitCastToNoHnummer_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoHnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NoHnummer_ExplicitCastToNoHnummer_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoHnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void NoHnummer_ExplicitCastToNoHnummer_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(separator: separator);
      LocalValidationError expected = GetInvalidSeparatorResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoHnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(HnummerInvalidDateOfBirthValues))]
   public void NoHnummer_ExplicitCastToNoHnummer_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
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
         .Invoking(() => _ = (NoHnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   [MemberData(nameof(ValidDnummerValues))]
   [MemberData(nameof(ValidFhnummerValues))]
   public void NoHnummer_ExplicitCastToNoHnummer_ShouldThrowKfValidationException_WhenValueIsNotHnummer(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoHnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoHnummer_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidUnformattedHnummer);
      var sut2 = new NoHnummer(ValidUnformattedHnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void NoHnummer_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidUnformattedHnummer);
      var sut2 = new NoHnummer(AltValidUnformattedHnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void NoHnummer_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var sut1 = new NoHnummer(ValidUnformattedHnummer);
      var sut2 = new NoHnummer(ValidFormattedHnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void NoHnummer_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidFormattedHnummer);
      var sut2 = new NoHnummer(ValidFormattedHnummer.Replace(' ', '.'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void NoHnummer_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidFormattedHnummer.Replace(' ', 'A'));
      var sut2 = new NoHnummer(ValidFormattedHnummer.Replace(' ', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoHnummer_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidUnformattedHnummer);
      var sut2 = new NoHnummer(AltValidUnformattedHnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void NoHnummer_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var sut1 = new NoHnummer(ValidUnformattedHnummer);
      var sut2 = new NoHnummer(ValidFormattedHnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void NoHnummer_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidUnformattedHnummer);
      var sut2 = new NoHnummer(ValidUnformattedHnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void NoHnummer_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidFormattedHnummer);
      var sut2 = new NoHnummer(ValidFormattedHnummer.Replace(' ', '.'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void NoHnummer_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidFormattedHnummer.Replace(' ', 'A'));
      var sut2 = new NoHnummer(ValidFormattedHnummer.Replace(' ', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoHnummer_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new NoHnummer(value);

      // Act.
      var result = NoHnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void NoHnummer_Create_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("204505", separator, "294");
      LocalCreateResult expected = new NoHnummer(value);

      // Act.
      var result = NoHnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(HnummerValidDateOfBirthValues))]
   public void NoHnummer_Create_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      LocalCreateResult expected = new NoHnummer(value);

      // Act.
      var result = NoHnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NoHnummer_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = NoHnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NoHnummer_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = NoHnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NoHnummer_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = NoHnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NoHnummer_Create_ShouldReturnInvalidCheckDigitsValidationResult_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = NoHnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void NoHnummer_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(separator: separator);
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value);

      // Act.
      var result = NoHnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(HnummerInvalidDateOfBirthValues))]
   public void NoHnummer_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String individualNumber)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         individualNumber: individualNumber);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = NoHnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   [MemberData(nameof(ValidDnummerValues))]
   [MemberData(nameof(ValidFhnummerValues))]
   public void NoHnummer_Create_ShouldReturnInvalidInvalidDateOfBirthValidationResult_WhenValueIsNotHnummer(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = NoHnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoHnummer_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidUnformattedHnummer);
      var sut2 = new NoHnummer(ValidUnformattedHnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NoHnummer_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidUnformattedHnummer);
      var sut2 = new NoHnummer(AltValidUnformattedHnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void NoHnummer_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var sut1 = new NoHnummer(ValidUnformattedHnummer);
      var sut2 = new NoHnummer(ValidFormattedHnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NoHnummer_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidFormattedHnummer);
      var sut2 = new NoHnummer(ValidFormattedHnummer.Replace(' ', '.'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NoHnummer_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidFormattedHnummer.Replace(' ', 'A'));
      var sut2 = new NoHnummer(ValidFormattedHnummer.Replace(' ', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NoHnummer_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new NoHnummer(ValidFormattedHnummer);

      // Act/assert.
      sut.Equals(ValidFormattedHnummer).Should().BeFalse();
   }

   [Fact]
   public void NoHnummer_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new NoHnummer(ValidFormattedHnummer);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoHnummer_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new NoHnummer(ValidUnformattedHnummer);
      var expected = ValidFormattedHnummer;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void NoHnummer_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new NoHnummer(AltValidUnformattedHnummer);
      var mask = "______-_____";
      var expected = AltValidFormattedHnummer;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void NoHnummer_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new NoHnummer(ValidUnformattedHnummer);
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
   public void NoHnummer_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new NoHnummer(AltValidUnformattedHnummer);
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
   public void NoHnummer_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidUnformattedHnummer);
      var sut2 = new NoHnummer(ValidUnformattedHnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void NoHnummer_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidUnformattedHnummer);
      var sut2 = new NoHnummer(AltValidUnformattedHnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void NoHnummer_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var sut1 = new NoHnummer(ValidUnformattedHnummer);
      var sut2 = new NoHnummer(ValidFormattedHnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void NoHnummer_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidFormattedHnummer);
      var sut2 = new NoHnummer(ValidFormattedHnummer.Replace(' ', '.'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void NoHnummer_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidFormattedHnummer.Replace(' ', 'A'));
      var sut2 = new NoHnummer(ValidFormattedHnummer.Replace(' ', 'a'));

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

   // NoHnummer does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void NoHnummer_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new NoHnummer(ValidUnformattedHnummer);
      var sut2 = new NoHnummer(ValidUnformattedHnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoHnummer_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new NoHnummer(value);

      // Act/assert.
      sut.ToString().Should().Be(sut.Value);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidHnummerValues))]
   public void NoHnummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NoHnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void NoHnummer_Validate_ShouldReturnValidationPassed_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("204505", separator, "294");
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NoHnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(HnummerValidDateOfBirthValues))]
   public void NoHnummer_Validate_ShouldReturnValidationPassed_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NoHnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NoHnummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = NoHnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NoHnummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = NoHnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NoHnummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = NoHnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NoHnummer_Validate_ShouldReturnInvalidCheckDigits_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = NoHnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void NoHnummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(separator: separator);
      LocalValidationResult expected = GetInvalidSeparatorResult(value);

      // Act.
      var result = NoHnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(HnummerInvalidDateOfBirthValues))]
   public void NoHnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String individualNumber)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         individualNumber: individualNumber);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = NoHnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   [MemberData(nameof(ValidDnummerValues))]
   [MemberData(nameof(ValidFhnummerValues))]
   public void NoHnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueIsNotHnummer(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = NoHnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoHnummer_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new NoHnummer(ValidUnformattedHnummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<NoHnummer>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void NoHnummer_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new NoHnummer(ValidFormattedHnummer);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public NoHnummer Hnummer { get; set; } = null!;
   }

   [Fact]
   public void NoHnummer_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Hnummer = new NoHnummer(ValidFormattedHnummer) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void NoHnummer_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Hnummer\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void NoHnummer_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Hnummer\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Hnummer.Should().BeNull();
   }

   [Fact]
   public void NoHnummer_JsonDeserialization_ShouldThrowKfValidationException_WhenValueIsInvalid()
   {
      // Arrange.
      var json = "{\"Hnummer\":\"13039597140\"}";  // Invalid check digits
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
