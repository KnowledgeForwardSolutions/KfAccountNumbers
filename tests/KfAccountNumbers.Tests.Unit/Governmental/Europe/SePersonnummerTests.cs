// Ignore Spelling: Deserialize Deserialization Json Kf Personnummer Samordningsnummer

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.Governmental.Europe.SePersonnummer,
   KfAccountNumbers.Governmental.Europe.SeIdentityNumberBase.ValidationError>;
using LocalValidationError = KfAccountNumbers.Governmental.Europe.SeIdentityNumberBase.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.Governmental.Europe.SeIdentityNumberBase.ValidationError>;
using LocalValidationResult = KfAccountNumbers.Governmental.Europe.SeIdentityNumberBase.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class SePersonnummerTests : SeIdentityNumberTestsBase
{
   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.SePersonnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(SeIdentityNumberBase.ShortFormatLength, Messages.SePersonnummerShortFormatLength),
            new ValidLengthDefinition(SeIdentityNumberBase.LongFormatLength, Messages.SePersonnummerLongFormatLength),
         ]);

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.SePersonnummerInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.SePersonnummerInvalidCheckDigit,
         Algorithms.Luhn.AlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(
         Messages.SePersonnummerInvalidSeparator,
         value[position],
         position);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => value.Length == 11
         ? new InvalidDateOfBirth(Messages.SePersonnummerInvalidDateOfBirth, value[..6], DateFormatName.YYMMDD)
         : new InvalidDateOfBirth(Messages.SePersonnummerInvalidDateOfBirth, value[..8], DateFormatName.YYYYMMDD);

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void SePersonnummer_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new SePersonnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerUndetectableCheckDigitErrors))]
   public void SePersonnummer_Constructor_ShouldCreateInstance_WhenCheckDigitHasUndetectableError(String value)
   {
      // Arrange.
      var expected = GetNormalizedValue(value);

      // Act.
      var sut = new SePersonnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   public void SePersonnummer_Constructor_ShouldCreateInstance_WhenDateOfBirthIsValid(
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
      var sut = new SePersonnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerInvalidDateOfBirthValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
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
         .Invoking(() => new SePersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   // This test is critical: it verifies that SePersonnummer rejects
   // valid samordningsnummer dates (day 61-91) because samordningsnummer
   // requires day 61-91 (personnummer day + 60 offset).
   [Theory]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasValidSamordningsnummerDateOfBirth(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   public void SePersonnummer_DateOfBirth_ShouldReturnExpectedValue(
      String dateOfBirth,
      Char separator,
      String expectedDateOfBirth)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      var sut = new SePersonnummer(value);
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
   [InlineData("811228", '7')]
   [InlineData("811228", '9')]
   [InlineData("19811228", '1')]
   [InlineData("19811228", '3')]
   [InlineData("19811228", '5')]
   [InlineData("19811228", '7')]
   [InlineData("19811228", '9')]
   public void SePersonnummer_Gender_ShouldReturnMale_ForValuesWithOddGenderIndicator(
      String dateOfBirth,
      Char digit)
   {
      // Arrange.
      var birthSerialNumber = $"54{digit}";
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         birthSerialNumber: birthSerialNumber);
      var sut = new SePersonnummer(value);
      Gender.BinaryGender expected = default(Gender.Male);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData("811228", '0')]
   [InlineData("811228", '2')]
   [InlineData("811228", '4')]
   [InlineData("811228", '6')]
   [InlineData("811228", '8')]
   [InlineData("19811228", '0')]
   [InlineData("19811228", '2')]
   [InlineData("19811228", '4')]
   [InlineData("19811228", '6')]
   [InlineData("19811228", '8')]
   public void SePersonnummer_Gender_ShouldReturnFemale_ForValuesWithEvenGenderIndicator(
      String dateOfBirth,
      Char digit)
   {
      // Arrange.
      var birthSerialNumber = $"54{digit}";
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         birthSerialNumber: birthSerialNumber);
      var sut = new SePersonnummer(value);
      Gender.BinaryGender expected = default(Gender.Female);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void SePersonnummer_Value_ShouldReturnValidatedPersonnummer(String personnummer)
   {
      // Arrange.
      var sut = new SePersonnummer(personnummer);
      var expected = GetNormalizedValue(personnummer);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidShortFormatDashPersonnummer;
      var sut = new SePersonnummer(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void SePersonnummer_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidShortFormatDashPersonnummer;
      var sut = new SePersonnummer(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void SePersonnummer_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      SePersonnummer sut = null!;

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void SePersonnummer_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      SePersonnummer sut = null!;

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new SePersonnummer(value);

      // Act.
      var sut = (SePersonnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerUndetectableCheckDigitErrors))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldCreateInstance_WhenCheckDigitHasUndetectableError(String value)
   {
      // Arrange.
      var expected = new SePersonnummer(value);

      // Act.
      var sut = (SePersonnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      var sut = (SePersonnummer)value;
      var expected = new SePersonnummer(value);

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerInvalidDateOfBirthValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
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
         .Invoking(() => _ = (SePersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasValidSamordningsnummerDateOfBirth(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var sut2 = new SePersonnummer(ValidShortFormatDashPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void SePersonnummer_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var sut2 = new SePersonnummer(AltValidShortFormatDashPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var sut2 = new SePersonnummer("19" + ValidShortFormatDashPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var sut2 = new SePersonnummer(AltValidShortFormatDashPersonnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void SePersonnummer_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var sut2 = new SePersonnummer("19" + ValidShortFormatDashPersonnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var sut2 = new SePersonnummer(ValidShortFormatDashPersonnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void SePersonnummer_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new SePersonnummer(value);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerUndetectableCheckDigitErrors))]
   public void SePersonnummer_Create_ShouldCreateInstance_WhenValueHasUndetectableCheckDigitError(String value)
   {
      // Arrange.
      LocalCreateResult expected = new SePersonnummer(value);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   public void SePersonnummer_Create_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalCreateResult expected = new SePersonnummer(value);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SePersonnummer_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidCheckDigitValidationResult_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerInvalidDateOfBirthValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasValidSamordningsnummerDateOfBirth(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = SePersonnummer.Create(value);

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
      var sut1 = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var sut2 = new SePersonnummer(ValidShortFormatDashPersonnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var sut2 = new SePersonnummer(AltValidShortFormatDashPersonnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var sut2 = new SePersonnummer("19" + ValidShortFormatDashPersonnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new SePersonnummer(ValidShortFormatDashPersonnummer);

      // Act/assert.
      sut.Equals(ValidShortFormatDashPersonnummer).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new SePersonnummer(ValidShortFormatDashPersonnummer);

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
      var sut1 = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var sut2 = new SePersonnummer(ValidShortFormatDashPersonnummer);

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
      var sut1 = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var sut2 = new SePersonnummer(AltValidShortFormatDashPersonnummer);

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
      var sut1 = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var sut2 = new SePersonnummer("19" + ValidShortFormatDashPersonnummer);

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

   // SePersonnummer does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void SePersonnummer_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var sut2 = new SePersonnummer(ValidShortFormatDashPersonnummer);

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
   public void SePersonnummer_ToLongFormat_ShouldReturnExpectedValue_WhenTimeProviderIsNull(String value)
   {
      // Arrange.
      var sut = new SePersonnummer(value);
      var expected = sut.Value[..8] + '-' + sut.Value[^4..];

      // Act/assert.
      sut.ToLongFormatValue().Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(FormatValueTestData))]
   public void SePersonnummer_ToLongFormat_ShouldReturnExpectedValue_WhenTimeProviderIsSupplied(
      Int32 years,
      Int32 days,
      Char expectedSeparator)
   {
      // Arrange.
      var sut = new SePersonnummer(ValidShortFormatDashPersonnummer);
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
   public void SePersonnummer_ToShortFormat_ShouldReturnExpectedValue_WhenTimeProviderIsNull(String value)
   {
      // Arrange.
      var sut = new SePersonnummer(value);
      var expected = sut.Value[2..8] + '-' + sut.Value[^4..];

      // Act/assert.
      sut.ToShortFormatValue().Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(FormatValueTestData))]
   public void SePersonnummer_ToShortFormat_ShouldReturnExpectedValue_WhenTimeProviderIsSupplied(
      Int32 years,
      Int32 days,
      Char expectedSeparator)
   {
      // Arrange.
      var sut = new SePersonnummer(ValidShortFormatDashPersonnummer);
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
   public void SePersonnummer_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new SePersonnummer(value);
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
   public void SePersonnummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerUndetectableCheckDigitErrors))]
   public void SePersonnummer_Validate_ShouldReturnValidationPassed_WhenValueHasUndetectableCheckDigitError(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerValidDateOfBirthValues))]
   public void SePersonnummer_Validate_ShouldReturnValidationPassed_WhenDateOfBirthIsValid(
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
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SePersonnummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(PersonnummerInvalidDateOfBirthValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(SamordningsnummerValidDateOfBirthValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasValidSamordningsnummerDateOfBirth(
      String dateOfBirth,
      Char separator,
      String _)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new SePersonnummer(ValidShortFormatDashPersonnummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<SePersonnummer>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void SePersonnummer_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new SePersonnummer(ValidShortFormatDashPersonnummer);
      var expected = sut.ToLongFormatValue();

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public SePersonnummer Personnummer { get; set; } = null!;
   }

   [Fact]
   public void SePersonnummer_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Personnummer = new SePersonnummer(ValidShortFormatDashPersonnummer) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void SePersonnummer_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Personnummer\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void SePersonnummer_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Personnummer\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Personnummer.Should().BeNull();
   }

   [Fact]
   public void SePersonnummer_JsonDeserialization_ShouldThrowKfValidationException_WhenPersonnummerIsInvalid()
   {
      // Arrange.
      var json = "{\"Personnummer\":\"811228-9875\"}";  // Invalid check digit
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
