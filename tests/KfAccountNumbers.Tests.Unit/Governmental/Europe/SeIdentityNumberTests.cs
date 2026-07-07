#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

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

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SeIdentityNumber_DateOfBirth_ShouldReturnExpectedValue(
      String dateOfBirth,
      Char separator,
      String expectedDateOfBirth)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      var sut = new SeIdentityNumber(value);
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
   [InlineData("811228", '1')]
   [InlineData("811228", '3')]
   [InlineData("811228", '5')]
   [InlineData("811288", '7')]
   [InlineData("811288", '9')]
   [InlineData("19811228", '1')]
   [InlineData("19811228", '3')]
   [InlineData("19811228", '5')]
   [InlineData("19811288", '7')]
   [InlineData("19811288", '9')]
   public void SeIdentityNumber_Gender_ShouldReturnMale_ForValuesWithOddGenderIndicator(
      String dateOfBirth,
      Char digit)
   {
      // Arrange.
      var birthSerialNumber = $"54{digit}";
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         birthSerialNumber: birthSerialNumber);
      var sut = new SeIdentityNumber(value);
      Gender.BinaryGender expected = default(Gender.Male);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData("811228", '0')]
   [InlineData("811228", '2')]
   [InlineData("811228", '4')]
   [InlineData("811288", '6')]
   [InlineData("811288", '8')]
   [InlineData("19811228", '0')]
   [InlineData("19811228", '2')]
   [InlineData("19811228", '4')]
   [InlineData("19811288", '6')]
   [InlineData("19811288", '8')]
   public void SeIdentityNumber_Gender_ShouldReturnFemale_ForValuesWithEvenGenderIndicator(
      String dateOfBirth,
      Char digit)
   {
      // Arrange.
      var birthSerialNumber = $"54{digit}";
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         birthSerialNumber: birthSerialNumber);
      var sut = new SeIdentityNumber(value);
      Gender.BinaryGender expected = default(Gender.Female);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region IdentifierType Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void SeIdentityNumber_IdentifierType_ShouldReturnExpectedIdentifierType_WhenValueIsPersonnummber(String value)
   {
      // Arrange.
      var sut = new SeIdentityNumber(value);
      SeIdentityNumberBase.IdentifierCategory expected = default(SeIdentifierType.Personnummer);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SeIdentityNumber_IdentifierType_ShouldReturnExpectedIdentifierType_WhenValueIsSamordningsnummer(String value)
   {
      // Arrange.
      var sut = new SeIdentityNumber(value);
      SeIdentityNumberBase.IdentifierCategory expected = default(SeIdentifierType.Samordningsnummer);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SeIdentityNumber_Value_ShouldReturnValidatedIdentifier(String value)
   {
      // Arrange.
      var sut = new SeIdentityNumber(value);
      var expected = GetNormalizedValue(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidShortFormatDashPersonnummer)]
   [InlineData(ValidShortFormatDashSamordningsnummer)]
   public void SeIdentityNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull(String value)
   {
      // Arrange.
      var sut = new SeIdentityNumber(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Theory]
   [InlineData(ValidShortFormatDashPersonnummer)]
   [InlineData(ValidShortFormatDashSamordningsnummer)]
   public void SeIdentityNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull(String value)
   {
      // Arrange.
      var sut = new SeIdentityNumber(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void SeIdentityNumber_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      SeIdentityNumber sut = null!;

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void SeIdentityNumber_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      SeIdentityNumber sut = null!;

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SeIdentityNumber_ExplicitCastToSeIdentityNumber_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new SeIdentityNumber(value);

      // Act.
      var sut = (SeIdentityNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerUndetectableCheckDigitErrors))]
   [MemberData(nameof(SamordningsnummerUndetectableCheckDigitErrors))]
   public void SeIdentityNumber_ExplicitCastToSeIdentityNumber_ShouldCreateInstance_WhenCheckDigitHasUndetectableError(String value)
   {
      // Arrange.
      var expected = new SeIdentityNumber(value);

      // Act.
      var sut = (SeIdentityNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SeIdentityNumber_ExplicitCastToSeIdentityNumber_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      var sut = (SeIdentityNumber)value;
      var expected = new SeIdentityNumber(value);

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SeIdentityNumber_ExplicitCastToSeIdentityNumber_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SeIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SeIdentityNumber_ExplicitCastToSeIdentityNumber_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SeIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SeIdentityNumber_ExplicitCastToSeIdentityNumber_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SeIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SeIdentityNumber_ExplicitCastToSeIdentityNumber_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SeIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SeIdentityNumber_ExplicitCastToSeIdentityNumber_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SeIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerInvalidDateOfBirthValues))]
   [MemberData(nameof(SamordningsnummerInvalidDateOfBirthValues))]
   public void SeIdentityNumber_ExplicitCastToSeIdentityNumber_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
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
         .Invoking(() => _ = (SeIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SeIdentityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashPersonnummer);
      var sut2 = new SeIdentityNumber(ValidShortFormatDashPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void SeIdentityNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeIdentityNumber(AltValidShortFormatDashSamordningsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void SeIdentityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashPersonnummer);
      var sut2 = new SeIdentityNumber("19" + ValidShortFormatDashPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void SeIdentityNumber_EqualityOperator_ShouldReturnFalse_WhenShortFormatValuesDifferOnlyBySeparator()
   {
      // Arrange.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeIdentityNumber(ValidShortFormatDashSamordningsnummer.Replace('-', '+'));

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SeIdentityNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashPersonnummer);
      var sut2 = new SeIdentityNumber(AltValidShortFormatDashPersonnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void SeIdentityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeIdentityNumber("19" + ValidShortFormatDashSamordningsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void SeIdentityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashPersonnummer);
      var sut2 = new SeIdentityNumber(ValidShortFormatDashPersonnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void SeIdentityNumber_InequalityOperator_ShouldReturnTrue_WhenShortFormatValuesDifferOnlyBySeparator()
   {
      // Arrange.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeIdentityNumber(ValidShortFormatDashSamordningsnummer.Replace('-', '+'));

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SeIdentityNumber_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new SeIdentityNumber(value);

      // Act.
      var result = SeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerUndetectableCheckDigitErrors))]
   [MemberData(nameof(SamordningsnummerUndetectableCheckDigitErrors))]
   public void SeIdentityNumber_Create_ShouldCreateInstance_WhenValueHasUndetectableCheckDigitError(String value)
   {
      // Arrange.
      LocalCreateResult expected = new SeIdentityNumber(value);

      // Act.
      var result = SeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SeIdentityNumber_Create_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalCreateResult expected = new SeIdentityNumber(value);

      // Act.
      var result = SeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SeIdentityNumber_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = SeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SeIdentityNumber_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = SeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SeIdentityNumber_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = SeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SeIdentityNumber_Create_ShouldReturnInvalidCheckDigitValidationResult_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = SeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SeIdentityNumber_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = SeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerInvalidDateOfBirthValues))]
   [MemberData(nameof(SamordningsnummerInvalidDateOfBirthValues))]
   public void SeIdentityNumber_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = SeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SeIdentityNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SeIdentityNumber(ValidLongFormatPlusPersonnummer);
      var sut2 = new SeIdentityNumber(ValidLongFormatPlusPersonnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void SeIdentityNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new SeIdentityNumber(ValidShortFormatPlusSamordningsnummer);
      var sut2 = new SeIdentityNumber(AltValidShortFormatPlusSamordningsnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void SeIdentityNumber_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SeIdentityNumber(ValidShortFormatPlusPersonnummer);
      var sut2 = new SeIdentityNumber("18" + ValidShortFormatPlusPersonnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void SeIdentityNumber_Equals_ShouldReturnFalse_WhenShortFormatValuesDifferOnlyBySeparator()
   {
      // Arrange.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeIdentityNumber(ValidShortFormatDashSamordningsnummer.Replace('-', '+'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void SeIdentityNumber_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new SeIdentityNumber(ValidLongFormatPlusPersonnummer);

      // Act/assert.
      sut.Equals(ValidLongFormatPlusSamordningsnummer).Should().BeFalse();
   }

   [Fact]
   public void SeIdentityNumber_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new SeIdentityNumber(ValidLongFormatPlusSamordningsnummer);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SeIdentityNumber_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashPersonnummer);
      var sut2 = new SeIdentityNumber(ValidShortFormatDashPersonnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void SeIdentityNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeIdentityNumber(AltValidShortFormatDashSamordningsnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void SeIdentityNumber_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashPersonnummer);
      var sut2 = new SeIdentityNumber("19" + ValidShortFormatDashPersonnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void SeIdentityNumber_GetHashCode_ShouldReturnDifferentValues_WhenShortFormatValuesDifferOnlyBySeparator()
   {
      // Arrange.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeIdentityNumber(ValidShortFormatDashSamordningsnummer.Replace('-', '+'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   #endregion

   #region ReferenceEquals Method Tests
   // ==========================================================================
   // ==========================================================================

   // SeIdentityNumber does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void SeIdentityNumber_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new SeIdentityNumber(ValidShortFormatDashPersonnummer);
      var sut2 = new SeIdentityNumber(ValidShortFormatDashPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToLongFormat Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SeIdentityNumber_ToLongFormat_ShouldReturnExpectedValue_WhenTimeProviderIsNull(String value)
   {
      // Arrange.
      var sut = new SeIdentityNumber(value);
      var expected = sut.Value[..8] + '-' + sut.Value[^4..];

      // Act/assert.
      sut.ToLongFormatValue().Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(FormatValueTestData))]
   public void SeIdentityNumber_ToLongFormat_ShouldReturnExpectedValue_WhenTimeProviderIsSupplied(
      Int32 years,
      Int32 days,
      Char expectedSeparator)
   {
      // Arrange.
      var sut = new SeIdentityNumber(ValidShortFormatDashPersonnummer);
      var currentDate = sut.DateOfBirth.AddYears(years).AddDays(days).ToDateTime(TimeOnly.MinValue);
      var timeProvider = new FakeTimeProvider(currentDate);
      var expected = $"{sut.Value[..8]}{expectedSeparator}{sut.Value[^4..]}";

      // Act.
      var result = sut.ToLongFormatValue(timeProvider);

      // Assert.
      result.Should().Be(expected);
   }

   #endregion

   #region ToShortFormat Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SeIdentityNumber_ToShortFormat_ShouldReturnExpectedValue_WhenTimeProviderIsNull(String value)
   {
      // Arrange.
      var sut = new SeIdentityNumber(value);
      var expected = sut.Value[2..8] + '-' + sut.Value[^4..];

      // Act/assert.
      sut.ToShortFormatValue().Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(FormatValueTestData))]
   public void SeIdentityNumber_ToShortFormat_ShouldReturnExpectedValue_WhenTimeProviderIsSupplied(
      Int32 years,
      Int32 days,
      Char expectedSeparator)
   {
      // Arrange.
      var sut = new SeIdentityNumber(ValidShortFormatDashSamordningsnummer);
      var currentDate = sut.DateOfBirth.AddYears(years).AddDays(days).ToDateTime(TimeOnly.MinValue);
      var timeProvider = new FakeTimeProvider(currentDate);
      var expected = $"{sut.Value[2..8]}{expectedSeparator}{sut.Value[^4..]}";

      // Act.
      var result = sut.ToShortFormatValue(timeProvider);

      // Assert.
      result.Should().Be(expected);
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SeIdentityNumber_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new SeIdentityNumber(value);
      var expected = sut.ToLongFormatValue();

      // Act/assert.
      sut.ToString().Should().Be(expected);
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

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SeIdentityNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new SeIdentityNumber(ValidShortFormatDashPersonnummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<SeIdentityNumber>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void SeIdentityNumber_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new SeIdentityNumber(ValidShortFormatDashSamordningsnummer);
      var expected = sut.ToString();

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public SeIdentityNumber IdentityNumber { get; set; } = null!;
   }

   [Fact]
   public void SeIdentityNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { IdentityNumber = new SeIdentityNumber(ValidShortFormatDashPersonnummer) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void SeIdentityNumber_JsonSerialization_ShouldSerializeNullGracefully()
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
   public void SeIdentityNumber_JsonDeserialization_ShouldDeserializeNullGracefully()
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
   public void SeIdentityNumber_JsonDeserialization_ShouldThrowKfValidationException_WhenValueIsInvalid()
   {
      // Arrange.
      var json = "{\"IdentityNumber\":\"811228-9875\"}";  // Invalid check digit
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
