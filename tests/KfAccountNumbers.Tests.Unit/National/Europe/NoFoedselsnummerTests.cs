// Ignore Spelling: Deserialize Deserialization Foedselsnummer Json Kf Nummer

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.National.Europe.NoFoedselsnummer,
   KfAccountNumbers.National.Europe.NoIdentityNumberBase.ValidationError>;
using LocalValidationError = KfAccountNumbers.National.Europe.NoIdentityNumberBase.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.National.Europe.NoIdentityNumberBase.ValidationError>;
using LocalValidationResult = KfAccountNumbers.National.Europe.NoIdentityNumberBase.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class NoFoedselsnummerTests : NoIdentityNumberTestsBase
{
   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.NoFoedselsnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(NoIdentityNumberBase.UnformattedLength, Messages.NoFoedselsnummerUnformattedLength),
            new ValidLengthDefinition(NoIdentityNumberBase.FormattedLength, Messages.NoFoedselsnummerFormattedLength),
         ]);

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.NoFoedselsnummerInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.NoFoedselsnummerInvalidCheckDigits,
         NoIdentityNumberBase.CheckDigitAlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(String value)
      => new(Messages.NoFoedselsnummerInvalidSeparator, value[6], 6);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(Messages.NoFoedselsnummerInvalidDateOfBirth, value[..6], DateFormatName.DDMMYY);

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   public void NoFoedselsnummer_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new NoFoedselsnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void NoFoedselsnummer_Constructor_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("200505", separator, "294");
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new NoFoedselsnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerValidDateOfBirthValues))]
   public void NoFoedselsnummer_Constructor_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new NoFoedselsnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NoFoedselsnummer_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoFoedselsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NoFoedselsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoFoedselsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NoFoedselsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoFoedselsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NoFoedselsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new NoFoedselsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void NoFoedselsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(separator: separator);
      LocalValidationError expected = GetInvalidSeparatorResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoFoedselsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerInvalidDateOfBirthValues))]
   public void NoFoedselsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
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
         .Invoking(() => new NoFoedselsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(DnummerValidDateOfBirthValues))]
   [MemberData(nameof(HnummerValidDateOfBirthValues))]
   public void NoFoedselsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasNonFoedselsnummerDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new NoFoedselsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(FoedselsnummerValidDateOfBirthValues))]
   public void NoFoedselsnummer_DateOfBirth_ShouldReturnExpectedValue(
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
      var sut = new NoFoedselsnummer(value);
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
   [InlineData("541", "")]
   [InlineData("543", "")]
   [InlineData("545", "")]
   [InlineData("547", "")]
   [InlineData("549", "")]
   [InlineData("541", " ")]
   [InlineData("543", " ")]
   [InlineData("545", " ")]
   [InlineData("547", " ")]
   [InlineData("549", " ")]
   public void NoFoedselsnummer_Gender_ShouldReturnMale_ForValuesWithOddGenderIndicator(
      String individualNumber,
      String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("200505", separator, individualNumber);
      var sut = new NoFoedselsnummer(value);
      Gender.BinaryGender expected = default(Gender.Male);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData("540", "")]
   [InlineData("642", "")]       // expected individual number 542 changed to 642 because of check digit constraints
   [InlineData("544", "")]
   [InlineData("546", "")]
   [InlineData("648", "")]       // expected individual number 548 changed to 648 because of check digit constraints
   [InlineData("540", " ")]
   [InlineData("642", " ")]      // expected individual number 542 changed to 642 because of check digit constraints
   [InlineData("544", " ")]
   [InlineData("546", " ")]
   [InlineData("648", " ")]      // expected individual number 548 changed to 648 because of check digit constraints
   public void NoFoedselsnummer_Gender_ShouldReturnFemale_ForValuesWithEvenGenderIndicator(
      String individualNumber,
      String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("200505", separator, individualNumber);
      var sut = new NoFoedselsnummer(value);
      Gender.BinaryGender expected = default(Gender.Female);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   public void NoFoedselsnummer_Value_ShouldReturnValidatedFoedselsnummer(String value)
   {
      // Arrange.
      var sut = new NoFoedselsnummer(value);
      var expected = GetNormalizedValue(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoFoedselsnummer_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidUnformattedFoedselsnummer;
      var sut = new NoFoedselsnummer(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void NoFoedselsnummer_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedFoedselsnummer;
      var sut = new NoFoedselsnummer(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void NoFoedselsnummer_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      NoFoedselsnummer sut = null!;

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void NoFoedselsnummer_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      NoFoedselsnummer sut = null!;

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   public void NoFoedselsnummer_ExplicitCastToNoFoedselsnummer_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new NoFoedselsnummer(value);

      // Act.
      var sut = (NoFoedselsnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void NoFoedselsnummer_ExplicitCastToNoFoedselsnummer_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("200505", separator, "294");
      var expected = new NoFoedselsnummer(value);

      // Act.
      var sut = (NoFoedselsnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerValidDateOfBirthValues))]
   public void NoFoedselsnummer_ExplicitCastToNoFoedselsnummer_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      var expected = new NoFoedselsnummer(value);

      // Act.
      var sut = (NoFoedselsnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NoFoedselsnummer_ExplicitCastToNoFoedselsnummer_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoFoedselsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NoFoedselsnummer_ExplicitCastToNoFoedselsnummer_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoFoedselsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NoFoedselsnummer_ExplicitCastToNoFoedselsnummer_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoFoedselsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NoFoedselsnummer_ExplicitCastToNoFoedselsnummer_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoFoedselsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void NoFoedselsnummer_ExplicitCastToNoFoedselsnummer_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(separator: separator);
      LocalValidationError expected = GetInvalidSeparatorResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoFoedselsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerInvalidDateOfBirthValues))]
   public void NoFoedselsnummer_ExplicitCastToNoFoedselsnummer_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
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
         .Invoking(() => _ = (NoFoedselsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(DnummerValidDateOfBirthValues))]
   [MemberData(nameof(HnummerValidDateOfBirthValues))]
   public void NoFoedselsnummer_ExplicitCastToNoFoedselsnummer_ShouldThrowKfValidationException_WhenValueHasNonFoedselsnummerDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NoFoedselsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoFoedselsnummer_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void NoFoedselsnummer_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(AltValidUnformattedFoedselsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void NoFoedselsnummer_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var sut1 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(ValidFormattedFoedselsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void NoFoedselsnummer_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidFormattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(ValidFormattedFoedselsnummer.Replace(' ', '.'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void NoFoedselsnummer_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidFormattedFoedselsnummer.Replace(' ', 'A'));
      var sut2 = new NoFoedselsnummer(ValidFormattedFoedselsnummer.Replace(' ', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoFoedselsnummer_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(AltValidUnformattedFoedselsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void NoFoedselsnummer_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var sut1 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(ValidFormattedFoedselsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void NoFoedselsnummer_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void NoFoedselsnummer_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidFormattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(ValidFormattedFoedselsnummer.Replace(' ', '.'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void NoFoedselsnummer_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidFormattedFoedselsnummer.Replace(' ', 'A'));
      var sut2 = new NoFoedselsnummer(ValidFormattedFoedselsnummer.Replace(' ', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   public void NoFoedselsnummer_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new NoFoedselsnummer(value);

      // Act.
      var result = NoFoedselsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void NoFoedselsnummer_Create_ShouldCreateInstance_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("200505", separator, "294");
      LocalCreateResult expected = new NoFoedselsnummer(value);

      // Act.
      var result = NoFoedselsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerValidDateOfBirthValues))]
   public void NoFoedselsnummer_Create_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      LocalCreateResult expected = new NoFoedselsnummer(value);

      // Act.
      var result = NoFoedselsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NoFoedselsnummer_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = NoFoedselsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NoFoedselsnummer_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = NoFoedselsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NoFoedselsnummer_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = NoFoedselsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NoFoedselsnummer_Create_ShouldReturnInvalidCheckDigitsValidationResult_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = NoFoedselsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void NoFoedselsnummer_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(separator: separator);
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value);

      // Act.
      var result = NoFoedselsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerInvalidDateOfBirthValues))]
   public void NoFoedselsnummer_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String individualNumber)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         individualNumber: individualNumber);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = NoFoedselsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(DnummerValidDateOfBirthValues))]
   [MemberData(nameof(HnummerValidDateOfBirthValues))]
   public void NoFoedselsnummer_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasNonFoedselsnummerDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = NoFoedselsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoFoedselsnummer_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NoFoedselsnummer_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(AltValidUnformattedFoedselsnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void NoFoedselsnummer_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var sut1 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(ValidFormattedFoedselsnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NoFoedselsnummer_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidFormattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(ValidFormattedFoedselsnummer.Replace(' ', '.'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NoFoedselsnummer_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidFormattedFoedselsnummer.Replace(' ', 'A'));
      var sut2 = new NoFoedselsnummer(ValidFormattedFoedselsnummer.Replace(' ', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NoFoedselsnummer_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new NoFoedselsnummer(ValidFormattedFoedselsnummer);

      // Act/assert.
      sut.Equals(ValidFormattedFoedselsnummer).Should().BeFalse();
   }

   [Fact]
   public void NoFoedselsnummer_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new NoFoedselsnummer(ValidFormattedFoedselsnummer);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoFoedselsnummer_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var expected = ValidFormattedFoedselsnummer;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void NoFoedselsnummer_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new NoFoedselsnummer(AltValidUnformattedFoedselsnummer);
      var mask = "______-_____";
      var expected = AltValidFormattedFoedselsnummer;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void NoFoedselsnummer_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
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
   public void NoFoedselsnummer_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new NoFoedselsnummer(AltValidUnformattedFoedselsnummer);
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
   public void NoFoedselsnummer_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void NoFoedselsnummer_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(AltValidUnformattedFoedselsnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void NoFoedselsnummer_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 12 character versions for same person should still be equal.
      var sut1 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(ValidFormattedFoedselsnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void NoFoedselsnummer_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidFormattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(ValidFormattedFoedselsnummer.Replace(' ', '.'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void NoFoedselsnummer_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidFormattedFoedselsnummer.Replace(' ', 'A'));
      var sut2 = new NoFoedselsnummer(ValidFormattedFoedselsnummer.Replace(' ', 'a'));

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

   // NoFoedselsnummer does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void NoFoedselsnummer_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);
      var sut2 = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   public void NoFoedselsnummer_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new NoFoedselsnummer(value);
      var expected = GetNormalizedValue(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFoedselsnummerValues))]
   public void NoFoedselsnummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NoFoedselsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void NoFoedselsnummer_Validate_ShouldReturnValidationPassed_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("200505", separator, "294");
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NoFoedselsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerValidDateOfBirthValues))]
   public void NoFoedselsnummer_Validate_ShouldReturnValidationPassed_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NoFoedselsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NoFoedselsnummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = NoFoedselsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NoFoedselsnummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = NoFoedselsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NoFoedselsnummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = NoFoedselsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NoFoedselsnummer_Validate_ShouldReturnInvalidCheckDigits_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = NoFoedselsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void NoFoedselsnummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(separator: separator);
      LocalValidationResult expected = GetInvalidSeparatorResult(value);

      // Act.
      var result = NoFoedselsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(FoedselsnummerInvalidDateOfBirthValues))]
   public void NoFoedselsnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      String individualNumber)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         dateOfBirth: dateOfBirth,
         individualNumber: individualNumber);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = NoFoedselsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(DnummerValidDateOfBirthValues))]
   [MemberData(nameof(HnummerValidDateOfBirthValues))]
   public void NoFoedselsnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasNonFoedselsnummerDateOfBirth(
      String dateOfBirth,
      String separator,
      String individualNumber,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(dateOfBirth, separator, individualNumber);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = NoFoedselsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NoFoedselsnummer_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new NoFoedselsnummer(ValidUnformattedFoedselsnummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<NoFoedselsnummer>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void NoFoedselsnummer_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new NoFoedselsnummer(ValidFormattedFoedselsnummer);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public NoFoedselsnummer Foedselsnummer { get; set; } = null!;
   }

   [Fact]
   public void NoFoedselsnummer_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Foedselsnummer = new NoFoedselsnummer(ValidFormattedFoedselsnummer) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void NoFoedselsnummer_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Foedselsnummer\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void NoFoedselsnummer_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Foedselsnummer\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Foedselsnummer.Should().BeNull();
   }

   [Fact]
   public void NoFoedselsnummer_JsonDeserialization_ShouldThrowKfValidationException_WhenFoedselsnummerIsInvalid()
   {
      // Arrange.
      var json = "{\"Foedselsnummer\":\"13039597140\"}";  // Invalid check digits
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
