// Ignore Spelling: Burgerservicenummer Deserialize Deserialization Json Kf proef

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.National.Europe.NlBurgerservicenummer,
   KfAccountNumbers.National.Europe.NlBurgerservicenummer.ValidationError>;
using LocalValidationError = KfAccountNumbers.National.Europe.NlBurgerservicenummer.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.National.Europe.NlBurgerservicenummer.ValidationError>;
using LocalValidationResult = KfAccountNumbers.National.Europe.NlBurgerservicenummer.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class NlBurgerservicenummerTests
{
   private const String ValidBurgerservicenummer = "123456782";
   private const String AltValidBurgerservicenummer = "111222333";
   private const String ValidFormattedBurgerservicenummer = "1234-56-782";
   private const String AltValidFormattedBurgerservicenummer = "1112-22-333";
   private const String AltSeparatorCharBurgerservicenummer = "1638.97.426";

   private static String GetRawBurgerservicenummer(String burgerservicenummer)
      => burgerservicenummer.Length == 9
         ? burgerservicenummer
         : burgerservicenummer[..4] + burgerservicenummer[5..7] + burgerservicenummer[8..];

   public static TheoryData<String> ValidBurgerservicenummerValues =>
   [
      ValidBurgerservicenummer,
      AltValidBurgerservicenummer,
      ValidFormattedBurgerservicenummer,
      AltValidFormattedBurgerservicenummer,
      AltSeparatorCharBurgerservicenummer,
   ];

   public static TheoryData<String> ValidSeparatorValues =>
   [
      "1234-56-782",
      "1234 56 782",
      "1234.56.782",
      "1234~56~782",
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "12345678",          // Length 8
      "1112223334",        // Length 10
      "1234-56-78",        // Length 10
      "1234-56-7823",      // Length 12
      new String('1', 100) // Very long string
   ];

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted values
      { ".23456782", 0 },           // Non-digit character '.'
      { "1 3456782", 1 },           // Non-digit character ' '
      { "12A456782", 2 },           // Non-digit character 'A'
      { "123Z56782", 3 },           // Non-digit character 'Z'
      { "1234^6782", 4 },           // Non-digit character '^'
      { "12345a782", 5 },           // Non-digit character 'a'
      { "123456z36", 6 },           // Non-digit character 'z'
      { "1234567~6", 7 },           // Non-digit character '~'
      { "12345678\u2153", 8 },      // Non-digit character Unicode fraction 1/3
      { "12345678\u00D6", 8 },      // Invalid character unicode O with umlaut

      // Formatted values
      { ".234 56 782", 0 },         // Non-digit character '.'
      { "1 34 56 782", 1 },         // Non-digit character ' '
      { "12A4 56 782", 2 },         // Non-digit character 'A'
      { "123Z 56 782", 3 },         // Non-digit character 'Z'
      { "1234 ^6 782", 5 },         // Non-digit character '^'
      { "1234 5a 782", 6 },         // Non-digit character 'a'
      { "1234 56 z86", 8 },         // Non-digit character 'z'
      { "1234 56 7~6", 9 },         // Non-digit character '~'
      { "1234 56 78\u2153", 10 },   // Non-digit character Unicode fraction 1/3
      { "1234 56 78\u00D6", 10 },   // Invalid character unicode O with umlaut
   };

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "122456782",         // 123456782 with single digit transcription error, 3 -> 2
      "111223333",         // 111222333 with single digit transcription error, 2 -> 3
      "123456783",         // 123456782 with check digit transcription error, 3 -> 2
      "124356782",         // 123456782 with two digit transposition error, 34 -> 43
      "112122333",         // 111222333 with two digit transposition error, 12 -> 21
      "123458762",         // 123456782 with two digit jump transposition, 678 -> 876
      "100222333",         // 111222333 with two digit twin error, 11 -> 00
      "111222344",         // 111222333 with two digit twin error, 33 -> 44
   ];

   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      // First separator position
      { "1234056-782", 4 },
      { "1234156-782", 4 },
      { "1234256-782", 4 },
      { "1234356-782", 4 },
      { "1234456-782", 4 },
      { "1234556-782", 4 },
      { "1234656-782", 4 },
      { "1234756-782", 4 },
      { "1234856-782", 4 },
      { "1234956-782", 4 },

      // Second separator position
      { "1234-560782", 7 },
      { "1234-561782", 7 },
      { "1234-562782", 7 },
      { "1234-563782", 7 },
      { "1234-564782", 7 },
      { "1234-565782", 7 },
      { "1234-566782", 7 },
      { "1234-567782", 7 },
      { "1234-568782", 7 },
      { "1234-569782", 7 },

      // Mixed separators
      { "1234-56.782", 7 },
      { "1234.56-782", 7 },
   };

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.NlBurgerservicenummerInvalidLength,
         value.Length,
         NlBurgerservicenummer.GetValidLengthDefinitions());

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.NlBurgerservicenummerInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.NlBurgerservicenummerInvalidCheckDigit,
         NlBurgerservicenummer.CheckDigitAlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(String value, Int32 position)
      => new(Messages.NlBurgerservicenummerInvalidSeparator, value[position], position);

   #region Constants Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NlBurgerservicenummer_CheckDigitAlgorithmName_ShouldHaveExpectedValue()
      => NlBurgerservicenummer.CheckDigitAlgorithmName.Should().Be("11-proef");

   #endregion

   #region Check Digit Algorithm Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   // Values designed to produce all possible check digits 0-9
                                 // Weights: 9 8 7 6 5 4 3 2 -1
   [InlineData("110000006")]     //          9 8             -6 = 11, mod 11 = 0
   [InlineData("101000005")]     //          9   7           -5
   [InlineData("100100004")]     //          9     6         -4
   [InlineData("100010003")]     //          9       5       -3
   [InlineData("100001002")]     //          9         4     -2
   [InlineData("100000101")]     //          9           3   -1
   [InlineData("110010000")]     //          9 8   6          0
   [InlineData("100110009")]     //          9     6 5       -9
   [InlineData("010110008")]     //            8   6 5       -8
   [InlineData("001110007")]     //              7 6 5       -7
   public void NlBurgerservicenummer_CheckDigitAlgorithm_ShouldValidateAllPossibleCheckDigits(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NlBurgerservicenummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidBurgerservicenummerValues))]
   public void NlBurgerservicenummer_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawBurgerservicenummer(value);

      // Act.
      var sut = new NlBurgerservicenummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparatorValues))]
   public void NlBurgerservicenummer_Constructor_ShouldCreateInstance_WhenValueHasValidSeparator(String value)
   {
      // Arrange.
      var expected = GetRawBurgerservicenummer(value);

      // Act.
      var sut = new NlBurgerservicenummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NlBurgerservicenummer_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new NlBurgerservicenummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NlBurgerservicenummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new NlBurgerservicenummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NlBurgerservicenummer_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new NlBurgerservicenummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NlBurgerservicenummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new NlBurgerservicenummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void NlBurgerservicenummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new NlBurgerservicenummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidBurgerservicenummer, ValidBurgerservicenummer)]
   [InlineData(AltValidBurgerservicenummer, AltValidBurgerservicenummer)]
   [InlineData(ValidFormattedBurgerservicenummer, ValidBurgerservicenummer)]
   [InlineData(AltValidFormattedBurgerservicenummer, AltValidBurgerservicenummer)]
   public void NlBurgerservicenummer_Value_ShouldReturnValidatedBurgerservicenummer(
      String value,
      String expected)
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NlBurgerservicenummer_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidBurgerservicenummer;
      var sut = new NlBurgerservicenummer(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(value);
   }

   [Fact]
   public void NlBurgerservicenummer_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedBurgerservicenummer;
      var sut = new NlBurgerservicenummer(value);
      var expected = GetRawBurgerservicenummer(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Fact]
   public void NlBurgerservicenummer_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      NlBurgerservicenummer sut = null!;

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void NlBurgerservicenummer_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      NlBurgerservicenummer sut = null!;

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidBurgerservicenummerValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new NlBurgerservicenummer(value);

      // Act.
      var sut = (NlBurgerservicenummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparatorValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldCreateInstance_WhenValueHasValidSeparator(String value)
   {
      // Arrange.
      var expected = new NlBurgerservicenummer(value);

      // Act.
      var sut = (NlBurgerservicenummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NlBurgerservicenummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NlBurgerservicenummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NlBurgerservicenummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NlBurgerservicenummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (NlBurgerservicenummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NlBurgerservicenummer_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidBurgerservicenummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void NlBurgerservicenummer_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(AltValidBurgerservicenummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void NlBurgerservicenummer_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void NlBurgerservicenummer_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer.Replace('-', '.'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void NlBurgerservicenummer_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer.Replace('-', 'A'));
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer.Replace('-', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NlBurgerservicenummer_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(AltValidBurgerservicenummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void NlBurgerservicenummer_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void NlBurgerservicenummer_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidBurgerservicenummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void NlBurgerservicenummer_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer.Replace('-', '.'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void NlBurgerservicenummer_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer.Replace('-', 'A'));
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer.Replace('-', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidBurgerservicenummerValues))]
   public void NlBurgerservicenummer_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new NlBurgerservicenummer(value);

      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparatorValues))]
   public void NlBurgerservicenummer_Create_ShouldCreateInstance_WhenValueHasValidSeparator(String value)
   {
      // Arrange.
      LocalCreateResult expected = new NlBurgerservicenummer(value);

      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NlBurgerservicenummer_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NlBurgerservicenummer_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NlBurgerservicenummer_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NlBurgerservicenummer_Create_ShouldReturnInvalidCheckDigitValidationResult_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void NlBurgerservicenummer_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NlBurgerservicenummer_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidBurgerservicenummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NlBurgerservicenummer_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(AltValidBurgerservicenummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void NlBurgerservicenummer_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NlBurgerservicenummer_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer.Replace('-', '.'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NlBurgerservicenummer_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer.Replace('-', 'A'));
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer.Replace('-', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NlBurgerservicenummer_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);

      // Act/assert.
      sut.Equals(ValidFormattedBurgerservicenummer).Should().BeFalse();
   }

   [Fact]
   public void NlBurgerservicenummer_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NlBurgerservicenummer_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var expected = ValidFormattedBurgerservicenummer;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void NlBurgerservicenummer_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var mask = "_________";
      var expected = ValidBurgerservicenummer;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void NlBurgerservicenummer_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(ValidBurgerservicenummer);
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
   public void NlBurgerservicenummer_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(ValidBurgerservicenummer);
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
   public void NlBurgerservicenummer_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidBurgerservicenummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void NlBurgerservicenummer_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(AltValidBurgerservicenummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void NlBurgerservicenummer_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void NlBurgerservicenummer_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer.Replace('-', '.'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void NlBurgerservicenummer_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer.Replace('-', 'A'));
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer.Replace('-', 'a'));

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

   // NlBurgerservicenummer does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void NlBurgerservicenummer_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidBurgerservicenummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidBurgerservicenummerValues))]
   public void NlBurgerservicenummer_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(value);
      var expected = GetRawBurgerservicenummer(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidBurgerservicenummerValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NlBurgerservicenummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparatorValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnValidationPassed_WhenValueHasValidSeparator(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NlBurgerservicenummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = NlBurgerservicenummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = NlBurgerservicenummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = NlBurgerservicenummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = NlBurgerservicenummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = NlBurgerservicenummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NlBurgerservicenummer_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(ValidBurgerservicenummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<NlBurgerservicenummer>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void NlBurgerservicenummer_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(AltValidFormattedBurgerservicenummer);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public NlBurgerservicenummer Burgerservicenummer { get; set; } = null!;
   }

   [Fact]
   public void NlBurgerservicenummer_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Burgerservicenummer = new NlBurgerservicenummer(AltValidBurgerservicenummer) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void NlBurgerservicenummer_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Burgerservicenummer\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void NlBurgerservicenummer_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Burgerservicenummer\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Burgerservicenummer.Should().BeNull();
   }

   [Fact]
   public void NlBurgerservicenummer_JsonDeserialization_ShouldThrowKfValidationException_WhenBurgerservicenummerIsInvalid()
   {
      // Arrange.
      var json = "{\"Burgerservicenummer\":\"122456782\"}";  // Invalid check digit
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
