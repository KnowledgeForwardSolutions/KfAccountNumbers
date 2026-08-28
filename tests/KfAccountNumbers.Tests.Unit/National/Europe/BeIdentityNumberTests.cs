#pragma warning disable SA1008 // Opening parenthesis should be spaced correctly

using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.National.Europe.BeIdentityNumber,
   KfAccountNumbers.National.Europe.BeIdentityNumberBase.ValidationError>;
using LocalValidationError = KfAccountNumbers.National.Europe.BeIdentityNumberBase.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.National.Europe.BeIdentityNumberBase.ValidationError>;
using LocalValidationResult = KfAccountNumbers.National.Europe.BeIdentityNumberBase.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class BeIdentityNumberTests : BeIdentityNumberBaseTests
{
   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.BeIdentityNumberInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(BeIdentityNumberBase.UnformattedLength, Messages.BeIdentityNumberUnformattedLength),
            new ValidLengthDefinition(BeIdentityNumberBase.FormattedLength, Messages.BeIdentityNumberFormattedLength),
         ]);

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.BeIdentityNumberInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.BeIdentityNumberInvalidCheckDigits,
         BeIdentityNumberBase.CheckDigitAlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(Messages.BeIdentityNumberInvalidSeparator, value[position], position);

   private static InvalidSequenceNumber GetInvalidSequenceNumberResult(String value)
      => new(
         Messages.BeIdentityNumberInvalidSequenceNumber,
         value.Length == 11 ? value[6..9] : value[9..12]);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(
         Messages.BeIdentityNumberInvalidDateOfBirth,
         value.Length == 11 ? value[..6] : value[..8],
         DateFormatName.YYMMDD);

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   [MemberData(nameof(ValidBisnummerValues))]
   public void BeIdentityNumber_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = new BeIdentityNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerSequenceNumberBoundaryValues))]
   [MemberData(nameof(ValidBisnummerSequenceNumberBoundaryValues))]
   public void BeIdentityNumber_Constructor_ShouldCreateInstance_WhenValueHasValidSerialNumber(
      Int32 year,
      Int32 month,
      Int32 sequenceNumber,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         year,
         month,
         sequenceNumber: sequenceNumber,
         formatted: formatted);
      var expected = GetRawValue(value);

      // Act.
      var sut = new BeIdentityNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerDateOfBirthValues))]
   [MemberData(nameof(ValidBisnummerDateOfBirthValues))]
   public void BeIdentityNumber_Constructor_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(year, month, day, formatted: formatted);
      var expected = GetRawValue(value);

      // Act.
      var sut = new BeIdentityNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void BeIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new BeIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void BeIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new BeIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void BeIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new BeIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void BeIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new BeIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void BeIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new BeIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRijksregisternummerSequenceNumberValues))]
   [MemberData(nameof(InvalidBisnummerSequenceNumberValues))]
   public void BeIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSequenceNumber(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSequenceNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new BeIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRijksregisternummerDateOfBirthValues))]
   [MemberData(nameof(InvalidBisnummerDateOfBirthValues))]
   public void BeIdentityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(year, month, day, formatted: formatted);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new BeIdentityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerDateOfBirthValues))]
   [MemberData(nameof(ValidBisnummerDateOfBirthValues))]
   public void BeIdentityNumber_DateOfBirth_ShouldReturnExpectedValue(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(year, month, day, formatted: formatted);
      var sut = new BeIdentityNumber(value);

      year = year == 0 ? 1900 : year;
      month = month switch
      {
         >= 20 and <= 32 => month - BeIdentityNumberBase.BisNummerUnknownGenderMonthOffset,
         >= 40 and <= 52 => month - BeIdentityNumberBase.BisNummerMonthOffset,
         _ => month,
      };
      var expected = new DateResult(
         year > 0 ? year : null,
         month > 0 ? month : null,
         day > 0 ? day : null);

      // Act/assert.
      sut.DateOfBirth.Should().Be(expected);
   }

   #endregion

   #region Gender Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData( 1, 181, false)]
   [InlineData( 0, 183, false)]
   [InlineData( 0, 185, true)]
   [InlineData( 1, 187, true)]
   [InlineData( 1, 189, true)]
   [InlineData(41, 181, false)]
   [InlineData(40, 183, false)]
   [InlineData(40, 185, true)]
   [InlineData(41, 187, true)]
   [InlineData(41, 189, true)]
   public void BeIdentityNumber_Gender_ShouldReturnMale_WhenSequenceNumberIsOdd(
      Int32 month,
      Int32 sequenceNumber,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         month: month,
         sequenceNumber: sequenceNumber,
         formatted: formatted);
      var sut = new BeIdentityNumber(value);
      KfOption<Gender.BinaryGender> expected = (Gender.BinaryGender)default(Gender.Male);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData( 0, 180, false)]
   [InlineData( 0, 182, false)]
   [InlineData( 1, 184, false)]
   [InlineData( 1, 186, true)]
   [InlineData( 0, 188, true)]
   [InlineData(40, 180, false)]
   [InlineData(40, 182, false)]
   [InlineData(41, 184, false)]
   [InlineData(41, 186, true)]
   [InlineData(40, 188, true)]
   public void BeIdentityNumber_Gender_ShouldReturnFemale_WhenSequenceNumberIsEven(
      Int32 month,
      Int32 sequenceNumber,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         month: month,
         sequenceNumber: sequenceNumber,
         formatted: formatted);
      var sut = new BeIdentityNumber(value);
      KfOption<Gender.BinaryGender> expected = (Gender.BinaryGender)default(Gender.Female);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData(21, 181, false)]
   [InlineData(32, 182, true)]
   public void BeIdentityNumber_Gender_ShouldReturnNone_WhenMonthIndicatesUnknownGender(
      Int32 month,
      Int32 sequenceNumber,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         month: month,
         sequenceNumber: sequenceNumber,
         formatted: formatted);
      var sut = new BeIdentityNumber(value);
      KfOption<Gender.BinaryGender> expected = default(None);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region IdentifierType Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   public void BeIdentityNumber_IdentifierType_ShouldReturnExpectedIdentifierType_WhenValueIsRijksregisternummer(String value)
   {
      // Arrange.
      var sut = new BeIdentityNumber(value);
      BeIdentityNumberBase.IdentifierCategory expected = default(BeIdentifierType.Rijksregisternummer);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidBisnummerValues))]
   public void BeIdentityNumber_IdentifierType_ShouldReturnExpectedIdentifierType_WhenValueIsBisnummer(String value)
   {
      // Arrange.
      var sut = new BeIdentityNumber(value);
      BeIdentityNumberBase.IdentifierCategory expected = default(BeIdentifierType.BisNummer);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   [MemberData(nameof(ValidBisnummerValues))]
   public void BeIdentityNumber_Value_ShouldReturnValidatedBisnummer(String value)
   {
      // Arrange.
      var sut = new BeIdentityNumber(value);
      var expected = GetRawValue(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeIdentityNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidUnformattedRijksregisternummer;
      var sut = new BeIdentityNumber(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void BeIdentityNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedBisnummer;
      var sut = new BeIdentityNumber(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void BeIdentityNumber_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      BeIdentityNumber sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void BeIdentityNumber_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      BeIdentityNumber sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   [MemberData(nameof(ValidBisnummerValues))]
   public void BeIdentityNumber_ExplicitCastToBeIdentityNumber_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new BeIdentityNumber(value);

      // Act.
      var sut = (BeIdentityNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerSequenceNumberBoundaryValues))]
   [MemberData(nameof(ValidBisnummerSequenceNumberBoundaryValues))]
   public void BeIdentityNumber_ExplicitCastToBeIdentityNumber_ShouldCreateInstance_WhenValueHasValidSequenceNumber(
      Int32 year,
      Int32 month,
      Int32 sequenceNumber,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         year,
         month,
         sequenceNumber: sequenceNumber,
         formatted: formatted);
      var expected = new BeIdentityNumber(value);

      // Act.
      var sut = (BeIdentityNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerDateOfBirthValues))]
   [MemberData(nameof(ValidBisnummerDateOfBirthValues))]
   public void BeIdentityNumber_ExplicitCastToBeIdentityNumber_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(year, month, day, formatted: formatted);
      var expected = new BeIdentityNumber(value);

      // Act.
      var sut = (BeIdentityNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void BeIdentityNumber_ExplicitCastToBeIdentityNumber_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (BeIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void BeIdentityNumber_ExplicitCastToBeIdentityNumber_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (BeIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void BeIdentityNumber_ExplicitCastToBeIdentityNumber_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (BeIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void BeIdentityNumber_ExplicitCastToBeIdentityNumber_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (BeIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void BeIdentityNumber_ExplicitCastToBeIdentityNumber_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (BeIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRijksregisternummerSequenceNumberValues))]
   [MemberData(nameof(InvalidBisnummerSequenceNumberValues))]
   public void BeIdentityNumber_ExplicitCastToBeIdentityNumber_ShouldThrowKfValidationException_WhenValueHasInvalidSequenceNumber(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSequenceNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (BeIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRijksregisternummerDateOfBirthValues))]
   [MemberData(nameof(InvalidBisnummerDateOfBirthValues))]
   public void BeIdentityNumber_ExplicitCastToBeIdentityNumber_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(year, month, day, formatted: formatted);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (BeIdentityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeIdentityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidUnformattedRijksregisternummer);
      var sut2 = new BeIdentityNumber(ValidUnformattedRijksregisternummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void BeIdentityNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidUnformattedBisnummer);
      var sut2 = new BeIdentityNumber(AltValidUnformattedBisnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void BeIdentityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 15 character versions for same person should still be equal.
      var sut1 = new BeIdentityNumber(ValidUnformattedRijksregisternummer);
      var sut2 = new BeIdentityNumber(ValidFormattedRijksregisternummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void BeIdentityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqualAndHaveIncompleteDateOfBirth()
   {
      var sut1 = new BeIdentityNumber(FormattedRijksregisternummerUnknownDob);
      var sut2 = new BeIdentityNumber(FormattedRijksregisternummerUnknownDob);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void BeIdentityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidFormattedBisnummer);
      var sut2 = new BeIdentityNumber(ValidFormattedBisnummer.Replace('.', ' '));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void BeIdentityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidFormattedRijksregisternummer.Replace('.', 'A'));
      var sut2 = new BeIdentityNumber(ValidFormattedRijksregisternummer.Replace('.', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeIdentityNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidUnformattedRijksregisternummer);
      var sut2 = new BeIdentityNumber(AltValidUnformattedRijksregisternummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void BeIdentityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 15 character versions for same person should still be equal.
      var sut1 = new BeIdentityNumber(ValidUnformattedBisnummer);
      var sut2 = new BeIdentityNumber(ValidFormattedBisnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void BeIdentityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidUnformattedRijksregisternummer);
      var sut2 = new BeIdentityNumber(ValidUnformattedRijksregisternummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void BeIdentityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqualAndHaveIncompleteDateOfBirth()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(UnformattedBisnummerUnknownDob);
      var sut2 = new BeIdentityNumber(UnformattedBisnummerUnknownDob);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void BeIdentityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidFormattedRijksregisternummer);
      var sut2 = new BeIdentityNumber(ValidFormattedRijksregisternummer.Replace('.', ' '));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void BeIdentityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidFormattedBisnummer.Replace('.', 'A'));
      var sut2 = new BeIdentityNumber(ValidFormattedBisnummer.Replace('.', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   [MemberData(nameof(ValidBisnummerValues))]
   public void BeIdentityNumber_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new BeIdentityNumber(value);

      // Act.
      var result = BeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerSequenceNumberBoundaryValues))]
   [MemberData(nameof(ValidBisnummerSequenceNumberBoundaryValues))]
   public void BeIdentityNumber_Create_ShouldCreateInstance_WhenValueHasValidSequenceNumber(
      Int32 year,
      Int32 month,
      Int32 sequenceNumber,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         year,
         month,
         sequenceNumber: sequenceNumber,
         formatted: formatted);
      LocalCreateResult expected = new BeIdentityNumber(value);

      // Act.
      var result = BeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerDateOfBirthValues))]
   [MemberData(nameof(ValidBisnummerDateOfBirthValues))]
   public void BeIdentityNumber_Create_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(year, month, day, formatted: formatted);
      LocalCreateResult expected = new BeIdentityNumber(value);

      // Act.
      var result = BeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void BeIdentityNumber_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = BeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void BeIdentityNumber_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = BeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void BeIdentityNumber_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = BeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void BeIdentityNumber_Create_ShouldReturnInvalidCheckDigitsValidationResult_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = BeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void BeIdentityNumber_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = BeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRijksregisternummerSequenceNumberValues))]
   [MemberData(nameof(InvalidBisnummerSequenceNumberValues))]
   public void BeIdentityNumber_Create_ShouldReturnInvalidSequenceNumberValidationResult_WhenValueHasInvalidSequenceNumber(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSequenceNumberResult(value);

      // Act.
      var result = BeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRijksregisternummerDateOfBirthValues))]
   [MemberData(nameof(InvalidBisnummerDateOfBirthValues))]
   public void BeIdentityNumber_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(year, month, day, formatted: formatted);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = BeIdentityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeIdentityNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidUnformattedRijksregisternummer);
      var sut2 = new BeIdentityNumber(ValidUnformattedRijksregisternummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void BeIdentityNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidUnformattedBisnummer);
      var sut2 = new BeIdentityNumber(AltValidUnformattedBisnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void BeIdentityNumber_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 15 character versions for same person should still be equal.
      var sut1 = new BeIdentityNumber(ValidUnformattedRijksregisternummer);
      var sut2 = new BeIdentityNumber(ValidFormattedRijksregisternummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void BeIdentityNumber_Equals_ShouldReturnTrue_WhenValuesAreEqualAndHaveIncompleteDateOfBirth()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(UnformattedRijksregisternummerYearDayOnlyDob);
      var sut2 = new BeIdentityNumber(UnformattedRijksregisternummerYearDayOnlyDob);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void BeIdentityNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidUnformattedRijksregisternummer);
      var sut2 = new BeIdentityNumber(ValidUnformattedRijksregisternummer.Replace('.', ' '));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void BeIdentityNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidUnformattedRijksregisternummer.Replace('.', 'A'));
      var sut2 = new BeIdentityNumber(ValidUnformattedRijksregisternummer.Replace('.', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void BeIdentityNumber_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new BeIdentityNumber(ValidUnformattedBisnummer);

      // Act/assert.
      sut.Equals(ValidUnformattedBisnummer).Should().BeFalse();
   }

   [Fact]
   public void BeIdentityNumber_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new BeIdentityNumber(ValidUnformattedBisnummer);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeIdentityNumber_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new BeIdentityNumber(ValidUnformattedRijksregisternummer);
      var expected = ValidFormattedRijksregisternummer;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void BeIdentityNumber_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new BeIdentityNumber(ValidUnformattedRijksregisternummer);
      var mask = "___________";
      var expected = ValidUnformattedRijksregisternummer;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void BeIdentityNumber_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new BeIdentityNumber(ValidUnformattedRijksregisternummer);
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
   public void BeIdentityNumber_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new BeIdentityNumber(ValidUnformattedRijksregisternummer);
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
   public void BeIdentityNumber_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidUnformattedRijksregisternummer);
      var sut2 = new BeIdentityNumber(ValidUnformattedRijksregisternummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void BeIdentityNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidUnformattedBisnummer);
      var sut2 = new BeIdentityNumber(AltValidUnformattedBisnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void BeIdentityNumber_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 15 character versions for same person should still be equal.
      var sut1 = new BeIdentityNumber(ValidUnformattedRijksregisternummer);
      var sut2 = new BeIdentityNumber(ValidFormattedRijksregisternummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void BeIdentityNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidFormattedRijksregisternummer);
      var sut2 = new BeIdentityNumber(ValidFormattedRijksregisternummer.Replace('.', ' '));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void BeIdentityNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(ValidFormattedRijksregisternummer.Replace('.', 'A'));
      var sut2 = new BeIdentityNumber(ValidFormattedRijksregisternummer.Replace('.', 'a'));

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

   // BeIdentityNumber does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void BeIdentityNumber_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new BeIdentityNumber(UnformattedRijksregisternummerUnknownDob);
      var sut2 = new BeIdentityNumber(UnformattedRijksregisternummerUnknownDob);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToBisnummer Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidBisnummerValues))]
   public void BeIdentityNumber_ToBisnummer_ShouldReturnExpectedResult_WhenValueIsBisnummer(String value)
   {
      // Arrange.
      var sut = new BeIdentityNumber(value);
      var expected = new BeBisnummer(value);

      // Act.
      KfOption<BeBisnummer> result = sut.ToBisnummer();

      // Assert.
      result.Value.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   public void BeIdentityNumber_ToBisnummer_ShouldReturnExpectedResult_WhenValueIsNotBisnummer(String value)
   {
      // Arrange.
      var sut = new BeIdentityNumber(value);
      var expected = default(None);

      // Act.
      KfOption<BeBisnummer> result = sut.ToBisnummer();

      // Assert.
      result.Value.Should().Be(expected);
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   [MemberData(nameof(ValidBisnummerValues))]
   public void BeIdentityNumber_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new BeIdentityNumber(value);
      var expected = GetRawValue(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   [MemberData(nameof(ValidBisnummerValues))]
   public void BeIdentityNumber_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = BeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerSequenceNumberBoundaryValues))]
   [MemberData(nameof(ValidBisnummerSequenceNumberBoundaryValues))]
   public void BeIdentityNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidSequenceNumber(
      Int32 year,
      Int32 month,
      Int32 sequenceNumber,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         year,
         month,
         sequenceNumber: sequenceNumber,
         formatted: formatted);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = BeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerDateOfBirthValues))]
   [MemberData(nameof(ValidBisnummerDateOfBirthValues))]
   public void BeIdentityNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(year, month, day, formatted: formatted);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = BeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void BeIdentityNumber_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = BeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void BeIdentityNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = BeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void BeIdentityNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = BeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void BeIdentityNumber_Validate_ShouldReturnInvalidCheckDigits_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = BeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void BeIdentityNumber_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = BeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRijksregisternummerSequenceNumberValues))]
   [MemberData(nameof(InvalidBisnummerSequenceNumberValues))]
   public void BeIdentityNumber_Validate_ShouldReturnInvalidSequenceNumber_WhenValueHasInvalidSequenceNumber(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSequenceNumberResult(value);

      // Act.
      var result = BeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRijksregisternummerDateOfBirthValues))]
   [MemberData(nameof(InvalidBisnummerDateOfBirthValues))]
   public void BeIdentityNumber_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(year, month, day, formatted: formatted);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = BeIdentityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeIdentityNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new BeIdentityNumber(ValidFormattedRijksregisternummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<BeIdentityNumber>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void BeIdentityNumber_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new BeIdentityNumber(AltValidUnformattedBisnummer);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public BeIdentityNumber IdentityNumber { get; set; } = null!;
   }

   [Fact]
   public void BeIdentityNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { IdentityNumber = new BeIdentityNumber(ValidFormattedRijksregisternummer) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void BeIdentityNumber_JsonSerialization_ShouldSerializeNullGracefully()
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
   public void BeIdentityNumber_JsonDeserialization_ShouldDeserializeNullGracefully()
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
   public void BeIdentityNumber_JsonDeserialization_ShouldThrowKfValidationException_WhenValueIsInvalid()
   {
      // Arrange.
      var json = "{\"IdentityNumber\":\"85072003328\"}";  // Invalid checksum
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
