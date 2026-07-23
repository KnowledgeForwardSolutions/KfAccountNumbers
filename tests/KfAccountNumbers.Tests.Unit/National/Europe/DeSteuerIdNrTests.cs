using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.National.Europe.DeSteuerIdNr,
   KfAccountNumbers.National.Europe.DeSteuerIdNr.ValidationError>;
using LocalValidationError = KfAccountNumbers.National.Europe.DeSteuerIdNr.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.National.Europe.DeSteuerIdNr.ValidationError>;
using LocalValidationResult = KfAccountNumbers.National.Europe.DeSteuerIdNr.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class DeSteuerIdNrTests
{
   // See https://generator.avris.it/DE for tools to generate test Steuer-IdNr
   private const String ValidUnformattedSteuerIdNr = "43957380212";
   private const String AltValidUnformattedSteuerIdNr = "25986078148";
   private const String ValidFormattedSteuerIdNr = "43 957 380 212";
   private const String AltValidFormattedSteuerIdNr = "25/986/078/148";

   public static TheoryData<String> ValidSteuerIdNrValues =>
   [
      ValidUnformattedSteuerIdNr,
      AltValidUnformattedSteuerIdNr,
      ValidFormattedSteuerIdNr,
      AltValidFormattedSteuerIdNr,
   ];

   public static TheoryData<Char> ValidSeparators =>
   [
      ' ',
      '-',
      'A',
      'z',
      '/',
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "4395738021",           // Length 10
      "439573802123",         // Length 12
      "43 957 380 21",        // Length 13
      "43 957 380 2123",      // Length 15
      new String('1', 100)    // Very long string
   ];

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted values
      { ".3957380212", 0 },            // Non-digit character '.'
      { "4 957380212", 1 },            // Non-digit character ' '
      { "43A57380212", 2 },            // Non-digit character 'A'
      { "439Z7380212", 3 },            // Non-digit character 'Z'
      { "4395^380212", 4 },            // Non-digit character '^'
      { "43957a80212", 5 },            // Non-digit character 'a'
      { "439573z0212", 6 },            // Non-digit character 'z'
      { "4395738~212", 7 },            // Non-digit character '~'
      { "43957380\u215312", 8 },       // Non-digit character Unicode fraction 1/3
      { "439573802\u00D62", 9 },       // Invalid character unicode O with umlaut
      { "4395738021\u0BE6", 10 },      // Invalid character unicode Tamil digit 0

      // Formatted values
      { ".3 957 380 212", 0 },         // Non-digit character '.'
      { "4  957 380 212", 1 },         // Non-digit character ' '
      { "43 A57 380 212", 3 },         // Non-digit character 'A'
      { "43 9Z7 380 212", 4 },         // Non-digit character 'Z'
      { "43 95^ 380 212", 5 },         // Non-digit character '^'
      { "43-957-a80-212", 7 },         // Non-digit character 'a'
      { "43-957-3z0-212", 8 },         // Non-digit character 'z'
      { "43-957-38~-212", 9 },         // Non-digit character '~'
      { "43-957-380-\u215312", 11 },   // Non-digit character Unicode fraction 1/3
      { "43-957-380-2\u00D62", 12 },   // Invalid character unicode O with umlaut
      { "43-957-380-21\u0BE6", 13 },   // Invalid character unicode Tamil digit 0
   };

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "43957389212",       // 43957380212 with single digit transcription error 0 -> 9
      "43947380212",       // 2608832599 with single digit transcription error 5 -> 4
      "25986708148",       // 25986078148 with two digit transposition error 07 -> 70
      "25986087148",       // 25986078148 with two digit transposition error 78 -> 87
      "68223904129",       // 68443904129 with two digit twin error 44 -> 22
      "68443902149",       // 68443904129 with jump transposition error 412 -> 214
      "24395738021",       // 43957380212 with two digit right circular shift error
      "39573802124",       // 43957380212 with two digit left circular shift error

      "43 957 389 212",    // 43957380212 with single digit transcription error 0 -> 9
      "43 947 380 212",    // 2608832599 with single digit transcription error 5 -> 4
      "25 986 708 148",    // 25986078148 with two digit transposition error 07 -> 70
      "25 986 087 148",    // 25986078148 with two digit transposition error 78 -> 87
      "68-223-904-129",    // 68443904129 with two digit twin error 44 -> 22
      "68-443-902-149",    // 68443904129 with jump transposition error 412 -> 214
      "24-395-738-021",    // 43957380212 with two digit right circular shift error
      "39-573-802-124",    // 43957380212 with two digit left circular shift error
   ];

   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      // First separator position
      { "430957 380 212", 2 },
      { "431957 380 212", 2 },
      { "432957 380 212", 2 },
      { "433957 380 212", 2 },
      { "434957 380 212", 2 },
      { "435957 380 212", 2 },
      { "436957 380 212", 2 },
      { "437957 380 212", 2 },
      { "438957 380 212", 2 },
      { "439957 380 212", 2 },

      // Second separator position
      { "43 9570380 212", 6 },
      { "43 9571380 212", 6 },
      { "43 9572380 212", 6 },
      { "43 9573380 212", 6 },
      { "43 9574380 212", 6 },
      { "43 9575380 212", 6 },
      { "43 9576380 212", 6 },
      { "43 9577380 212", 6 },
      { "43 9578380 212", 6 },
      { "43 9579380 212", 6 },

      // Third separator position
      { "43 957 3800212", 10 },
      { "43 957 3801212", 10 },
      { "43 957 3802212", 10 },
      { "43 957 3803212", 10 },
      { "43 957 3804212", 10 },
      { "43 957 3805212", 10 },
      { "43 957 3806212", 10 },
      { "43 957 3807212", 10 },
      { "43 957 3808212", 10 },
      { "43 957 3809212", 10 },

      // Mixed separators
      { "43 957.380 212", 6 },
      { "43 957 380-212", 10 },
      { "43.957/380-212", 6 },
   };

   private static String GetFormattedValue(String value, Char separator)
      => value[..2] + separator + value[2..5] + separator + value[5..8] + separator +  value[8..];

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.DeSteuerIdNrInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.DeSteuerIdNrInvalidCheckDigit,
         Algorithms.Iso7064Mod11_10.AlgorithmName);

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.DeSteuerIdNrInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(DeSteuerIdNr.UnformattedLength, Messages.DeSteuerIdNrUnormattedLength),
            new ValidLengthDefinition(DeSteuerIdNr.FormattedLength, Messages.DeSteuerIdNrFormattedLength),
         ]);

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(
         Messages.DeSteuerIdNrInvalidSeparator,
         value[position],
         position);

   private static String GetRawDeSteuerIdNr(String value)
      => value.Length == DeSteuerIdNr.UnformattedLength
         ? value
         : value[..2] + value[3..6] + value[7..10] + value[11..];

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidSteuerIdNrValues))]
   public void DeSteuerIdNr_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawDeSteuerIdNr(value);

      // Act.
      var sut = new DeSteuerIdNr(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void DeSteuerIdNr_Constructor_ShouldCreateInstance_WhenValueHasValidSeparator(Char separator)
   {
      // Arrange.
      var value = GetFormattedValue(ValidUnformattedSteuerIdNr, separator: separator);
      var expected = GetRawDeSteuerIdNr(value);

      // Act.
      var sut = new DeSteuerIdNr(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void DeSteuerIdNr_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new DeSteuerIdNr(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DeSteuerIdNr_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new DeSteuerIdNr(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DeSteuerIdNr_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new DeSteuerIdNr(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void DeSteuerIdNr_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new DeSteuerIdNr(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DeSteuerIdNr_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new DeSteuerIdNr(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidSteuerIdNrValues))]
   public void DeSteuerIdNr_Value_ShouldReturnValidatedRijksregisternummer(String value)
   {
      // Arrange.
      var expected = GetRawDeSteuerIdNr(value);
      var sut = new DeSteuerIdNr(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DeSteuerIdNr_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidUnformattedSteuerIdNr;
      var sut = new DeSteuerIdNr(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void DeSteuerIdNr_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedSteuerIdNr;
      var sut = new DeSteuerIdNr(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void DeSteuerIdNr_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      DeSteuerIdNr sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void DeSteuerIdNr_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      DeSteuerIdNr sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidSteuerIdNrValues))]
   public void DeSteuerIdNr_ExplicitCastToDeSteuerIdNr_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new DeSteuerIdNr(value);

      // Act.
      var sut = (DeSteuerIdNr)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void DeSteuerIdNr_ExplicitCastToDeSteuerIdNr_ShouldCreateInstance_WhenValueHasValidSeparator(Char separator)
   {
      // Arrange.
      var value = GetFormattedValue(ValidUnformattedSteuerIdNr, separator: separator);
      var expected = new DeSteuerIdNr(value);

      // Act.
      var sut = (DeSteuerIdNr)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void DeSteuerIdNr_ExplicitCastToDeSteuerIdNr_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (DeSteuerIdNr)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DeSteuerIdNr_ExplicitCastToDeSteuerIdNr_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (DeSteuerIdNr)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DeSteuerIdNr_ExplicitCastToDeSteuerIdNr_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (DeSteuerIdNr)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void DeSteuerIdNr_ExplicitCastToDeSteuerIdNr_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (DeSteuerIdNr)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DeSteuerIdNr_ExplicitCastToDeSteuerIdNr_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (DeSteuerIdNr)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DeSteuerIdNr_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void DeSteuerIdNr_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(AltValidUnformattedSteuerIdNr);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void DeSteuerIdNr_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 14 character versions for same person should still be equal.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(ValidFormattedSteuerIdNr);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void DeSteuerIdNr_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidFormattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(ValidFormattedSteuerIdNr.Replace(' ', '.'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void DeSteuerIdNr_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidFormattedSteuerIdNr.Replace('.', 'A'));
      var sut2 = new DeSteuerIdNr(ValidFormattedSteuerIdNr.Replace('.', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DeSteuerIdNr_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(AltValidUnformattedSteuerIdNr);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void DeSteuerIdNr_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 14 character versions for same person should still be equal.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(ValidFormattedSteuerIdNr);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void DeSteuerIdNr_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void DeSteuerIdNr_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr.Replace(' ', '.'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void DeSteuerIdNr_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr.Replace(' ', 'A'));
      var sut2 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr.Replace(' ', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidSteuerIdNrValues))]
   public void DeSteuerIdNr_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new DeSteuerIdNr(value);

      // Act.
      var result = DeSteuerIdNr.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void DeSteuerIdNr_Create_ShouldCreateInstance_WhenValueHasValidSeparator(Char separator)
   {
      // Arrange.
      var value = GetFormattedValue(ValidUnformattedSteuerIdNr, separator: separator);
      LocalCreateResult expected = new DeSteuerIdNr(value);

      // Act.
      var result = DeSteuerIdNr.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void DeSteuerIdNr_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = DeSteuerIdNr.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DeSteuerIdNr_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = DeSteuerIdNr.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DeSteuerIdNr_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = DeSteuerIdNr.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void DeSteuerIdNr_Create_ShouldReturnInvalidCheckDigitsValidationResult_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = DeSteuerIdNr.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DeSteuerIdNr_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = DeSteuerIdNr.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DeSteuerIdNr_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void DeSteuerIdNr_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(AltValidUnformattedSteuerIdNr);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void DeSteuerIdNr_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 14 character versions for same person should still be equal.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(ValidFormattedSteuerIdNr);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void DeSteuerIdNr_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidFormattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(ValidFormattedSteuerIdNr.Replace(' ', '.'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void DeSteuerIdNr_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidFormattedSteuerIdNr.Replace(' ', 'A'));
      var sut2 = new DeSteuerIdNr(ValidFormattedSteuerIdNr.Replace(' ', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void DeSteuerIdNr_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);

      // Act/assert.
      sut.Equals(ValidUnformattedSteuerIdNr).Should().BeFalse();
   }

   [Fact]
   public void DeSteuerIdNr_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DeSteuerIdNr_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new DeSteuerIdNr(AltValidUnformattedSteuerIdNr);
      var expected = AltValidFormattedSteuerIdNr;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void DeSteuerIdNr_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var mask = "___________";
      var expected = ValidUnformattedSteuerIdNr;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void DeSteuerIdNr_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
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
   public void DeSteuerIdNr_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
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
   public void DeSteuerIdNr_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void DeSteuerIdNr_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(AltValidUnformattedSteuerIdNr);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void DeSteuerIdNr_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 14 character versions for same person should still be equal.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(ValidFormattedSteuerIdNr);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void DeSteuerIdNr_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr.Replace(' ', '.'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void DeSteuerIdNr_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr.Replace(' ', 'A'));
      var sut2 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr.Replace(' ', 'a'));

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

   // DeSteuerIdNr does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void DeSteuerIdNr_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);
      var sut2 = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidSteuerIdNrValues))]
   public void DeSteuerIdNr_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new DeSteuerIdNr(value);
      var expected = GetRawDeSteuerIdNr(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidSteuerIdNrValues))]
   public void DeSteuerIdNr_Validate_ShouldReturnValidValue_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void DeSteuerIdNr_Validate_ShouldReturnValidationPassed_WhenValueHasValidSeparator(Char separator)
   {
      // Arrange.
      var value = GetFormattedValue(ValidUnformattedSteuerIdNr, separator: separator);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void DeSteuerIdNr_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DeSteuerIdNr_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DeSteuerIdNr_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void DeSteuerIdNr_Validate_ShouldReturnInvalidChecksum_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DeSteuerIdNr_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DeSteuerIdNr_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new DeSteuerIdNr(ValidUnformattedSteuerIdNr);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<DeSteuerIdNr>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void DeSteuerIdNr_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new DeSteuerIdNr(AltValidFormattedSteuerIdNr);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public DeSteuerIdNr SteuerIdNr { get; set; } = null!;
   }

   [Fact]
   public void DeSteuerIdNr_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { SteuerIdNr = new DeSteuerIdNr(ValidUnformattedSteuerIdNr) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void DeSteuerIdNr_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"SteuerIdNr\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void DeSteuerIdNr_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"SteuerIdNr\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.SteuerIdNr.Should().BeNull();
   }

   [Fact]
   public void DeSteuerIdNr_JsonDeserialization_ShouldThrowKfValidationException_WhenRijksregisternummerIsInvalid()
   {
      // Arrange.
      var json = "{\"SteuerIdNr\":\"43957389212\"}";  // Invalid checksum
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
