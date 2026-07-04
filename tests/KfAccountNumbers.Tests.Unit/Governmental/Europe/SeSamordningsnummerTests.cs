#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

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

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(
      String value,
      String? message = null)
   {
      var isShortFormat = value.Length == SeIdentityNumberBase.ShortFormatLength;

      return new InvalidDateOfBirth(
         message ?? Messages.SeSamordingsnummerrInvalidDateOfBirth,
         isShortFormat ? value[..6] : value[..8],
         isShortFormat ? DateFormatName.YYMMDD : DateFormatName.YYYYMMDD);
   }

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SeSamordningsnummer_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new SeSamordningsnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerUndetectableCheckDigitErrors))]
   public void SeSamordningsnummer_Constructor_ShouldCreateInstance_WhenCheckDigitHasUndetectableError(String value)
   {
      // Arrange.
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new SeSamordningsnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SeSamordningsnummer_Constructor_ShouldCreateInstance_WhenDateOfBirthIsValid(
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
      var sut = new SeSamordningsnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SeSamordningsnummer_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new SeSamordningsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SeSamordningsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new SeSamordningsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SeSamordningsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new SeSamordningsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SeSamordningsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new SeSamordningsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SeSamordningsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new SeSamordningsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerInvalidDateOfBirthValues))]
   public void SeSamordningsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
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
         .Invoking(() => new SeSamordningsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   public void SeSamordningsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasValidPersonnummerDateOfBirth(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationError expected = GetInvalidDateOfBirthResult(
         value,
         Messages.SeSamordingsnummerrInvalidDateOfBirthDayRange);

      // Act/assert.
      FluentActions
         .Invoking(() => new SeSamordningsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SeSamordningsnummer_DateOfBirth_ShouldReturnExpectedValue(
      String dateOfBirth,
      Char separator,
      String expectedDateOfBirth)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      var sut = new SeSamordningsnummer(value);
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
   [InlineData("811288", '1')]
   [InlineData("811288", '3')]
   [InlineData("811288", '5')]
   [InlineData("811288", '7')]
   [InlineData("811288", '9')]
   [InlineData("19811288", '1')]
   [InlineData("19811288", '3')]
   [InlineData("19811288", '5')]
   [InlineData("19811288", '7')]
   [InlineData("19811288", '9')]
   public void SeSamordningsnummer_Gender_ShouldReturnMale_ForValuesWithOddGenderIndicator(
      String dateOfBirth,
      Char digit)
   {
      // Arrange.
      var birthSerialNumber = $"54{digit}";
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         birthSerialNumber: birthSerialNumber);
      var sut = new SeSamordningsnummer(value);
      Gender.BinaryGender expected = default(Gender.Male);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData("811288", '0')]
   [InlineData("811288", '2')]
   [InlineData("811288", '4')]
   [InlineData("811288", '6')]
   [InlineData("811288", '8')]
   [InlineData("19811288", '0')]
   [InlineData("19811288", '2')]
   [InlineData("19811288", '4')]
   [InlineData("19811288", '6')]
   [InlineData("19811288", '8')]
   public void SeSamordningsnummer_Gender_ShouldReturnFemale_ForValuesWithEvenGenderIndicator(
      String dateOfBirth,
      Char digit)
   {
      // Arrange.
      var birthSerialNumber = $"54{digit}";
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         birthSerialNumber: birthSerialNumber);
      var sut = new SeSamordningsnummer(value);
      Gender.BinaryGender expected = default(Gender.Female);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SeSamordningsnummer_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidShortFormatDashSamordningsnummer;
      var sut = new SeSamordningsnummer(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void SeSamordningsnummer_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidShortFormatDashSamordningsnummer;
      var sut = new SeSamordningsnummer(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void SeSamordningsnummer_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      SeSamordningsnummer sut = null!;

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void SeSamordningsnummer_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      SeSamordningsnummer sut = null!;

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SeSamordningsnummer_ExplicitCastToSeSamordningsnummer_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new SeSamordningsnummer(value);

      // Act.
      var sut = (SeSamordningsnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerUndetectableCheckDigitErrors))]
   public void SeSamordningsnummer_ExplicitCastToSeSamordningsnummer_ShouldCreateInstance_WhenCheckDigitHasUndetectableError(String value)
   {
      // Arrange.
      var expected = new SeSamordningsnummer(value);

      // Act.
      var sut = (SeSamordningsnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SeSamordningsnummer_ExplicitCastToSeSamordningsnummer_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      var sut = (SeSamordningsnummer)value;
      var expected = new SeSamordningsnummer(value);

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SeSamordningsnummer_ExplicitCastToSeSamordningsnummer_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SeSamordningsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SeSamordningsnummer_ExplicitCastToSeSamordningsnummer_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SeSamordningsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SeSamordningsnummer_ExplicitCastToSeSamordningsnummer_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SeSamordningsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SeSamordningsnummer_ExplicitCastToSeSamordningsnummer_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SeSamordningsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SeSamordningsnummer_ExplicitCastToSeSamordningsnummer_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SeSamordningsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerInvalidDateOfBirthValues))]
   public void SeSamordningsnummer_ExplicitCastToSeSamordningsnummer_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
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
         .Invoking(() => _ = (SeSamordningsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   public void SeSamordningsnummer_ExplicitCastToSeSamordningsnummer_ShouldThrowKfValidationException_WhenValueHasValidPersonnummerDateOfBirth(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationError expected = GetInvalidDateOfBirthResult(
         value,
         Messages.SeSamordingsnummerrInvalidDateOfBirthDayRange);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SeSamordningsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SeSamordningsnummer_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void SeSamordningsnummer_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeSamordningsnummer(AltValidShortFormatDashSamordningsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void SeSamordningsnummer_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeSamordningsnummer("19" + ValidShortFormatDashSamordningsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void SeSamordningsnummer_EqualityOperator_ShouldReturnFalse_WhenShortFormatValuesDifferOnlyBySeparator()
   {
      // Arrange.
      var sut1 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer.Replace('-', '+'));

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SeSamordningsnummer_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeSamordningsnummer(AltValidShortFormatDashSamordningsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void SeSamordningsnummer_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeSamordningsnummer("19" + ValidShortFormatDashSamordningsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void SeSamordningsnummer_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void SeSamordningsnummer_InequalityOperator_ShouldReturnTrue_WhenShortFormatValuesDifferOnlyBySeparator()
   {
      // Arrange.
      var sut1 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer.Replace('-', '+'));

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SeSamordningsnummer_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new SeSamordningsnummer(value);

      // Act.
      var result = SeSamordningsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerUndetectableCheckDigitErrors))]
   public void SeSamordningsnummer_Create_ShouldCreateInstance_WhenValueHasUndetectableCheckDigitError(String value)
   {
      // Arrange.
      LocalCreateResult expected = new SeSamordningsnummer(value);

      // Act.
      var result = SeSamordningsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SeSamordningsnummer_Create_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalCreateResult expected = new SeSamordningsnummer(value);

      // Act.
      var result = SeSamordningsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SeSamordningsnummer_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = SeSamordningsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SeSamordningsnummer_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = SeSamordningsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SeSamordningsnummer_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = SeSamordningsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SeSamordningsnummer_Create_ShouldReturnInvalidCheckDigitValidationResult_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = SeSamordningsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SeSamordningsnummer_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = SeSamordningsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerInvalidDateOfBirthValues))]
   public void SeSamordningsnummer_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = SeSamordningsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   public void SeSamordningsnummer_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasValidPersonnummerDateOfBirth(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(
         value,
         Messages.SeSamordingsnummerrInvalidDateOfBirthDayRange);

      // Act.
      var result = SeSamordningsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(ValidLongFormatPlusSamordningsnummer);
      var sut2 = new SePersonnummer(ValidLongFormatPlusSamordningsnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(ValidShortFormatPlusSamordningsnummer);
      var sut2 = new SePersonnummer(AltValidShortFormatPlusSamordningsnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SePersonnummer(ValidShortFormatPlusSamordningsnummer);
      var sut2 = new SePersonnummer("18" + ValidShortFormatPlusSamordningsnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void SeSamordningsnummer_Equals_ShouldReturnFalse_WhenShortFormatValuesDifferOnlyBySeparator()
   {
      // Arrange.
      var sut1 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer.Replace('-', '+'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new SePersonnummer(ValidLongFormatPlusSamordningsnummer);

      // Act/assert.
      sut.Equals(ValidLongFormatPlusSamordningsnummer).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new SePersonnummer(ValidLongFormatPlusSamordningsnummer);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SePersonnummer(ValidShortFormatDashSamordningsnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void SePersonnummer_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new SePersonnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SePersonnummer(AltValidShortFormatDashSamordningsnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void SePersonnummer_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SePersonnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SePersonnummer("19" + ValidShortFormatDashSamordningsnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void SePersonnummer_GetHashCode_ShouldReturnDifferentValues_WhenShortFormatValuesDifferOnlyBySeparator()
   {
      // Arrange.
      var sut1 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SeSamordningsnummer(ValidShortFormatDashSamordningsnummer.Replace('-', '+'));

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

   // SePersonnummer does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void SePersonnummer_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new SePersonnummer(ValidShortFormatDashSamordningsnummer);
      var sut2 = new SePersonnummer(ValidShortFormatDashSamordningsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

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
      Char separator,
      String _)
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

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   public void SeSamordningsnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasValidPersonnummerDateOfBirth(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(
         value,
         Messages.SeSamordingsnummerrInvalidDateOfBirthDayRange);

      // Act.
      var result = SeSamordningsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion
}
