// Ignore Spelling: Deserialize Deserialization Json Kf

using LocalCreateResult = KfAccountNumbers.Results.UCreateResult<
   KfAccountNumbers.Governmental.Europe.IePpsNumber,
   KfAccountNumbers.Governmental.Europe.IePpsNumber.ValidationError>;
using LocalValidationError = KfAccountNumbers.Governmental.Europe.IePpsNumber.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.Governmental.Europe.IePpsNumber.ValidationError>;
using LocalValidationResult = KfAccountNumbers.Governmental.Europe.IePpsNumber.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

public class IePpsNumberTests
{
   private const String ValidOriginalLengthPpsNumber = "1234567T";
   private const String AltValidOriginalLengthPpsNumber = "7654321G";
   private const String ValidExtendedLengthPpsNumber = "1234567FA";
   private const String AltValidExtendedLengthPpsNumber = "7654321PA";
   private const String ValidOriginalLengthPpsNumberWithWSuffix = "1234567TW";
   private const String LowerCaseValidOriginalLengthPpsNumber = "1234567t";
   private const String LowerCaseAltValidOriginalLengthPpsNumber = "7654321g";
   private const String LowerCaseValidExtendedLengthPpsNumber = "1234567fa";
   private const String LowerCaseAltValidExtendedLengthPpsNumber = "7654321pa";
   private const String LowerCaseValidOriginalLengthPpsNumberWithWSuffix = "1234567tw";
   private const String MixedCaseValidExtendedLengthPpsNumber = "1234567Fa";
   private const String MixedCaseAltValidExtendedLengthPpsNumber = "7654321pA";

   private static String GetPpsNumberWithValidCheckDigit(
      String digits = "1234567",
      String trailingCharacter = "")
   {
      var temp = $"{digits}_{trailingCharacter}";     // Underscore correctly pads length to support optional trailing character when calculating check digit
      var checkCharacter = GetCheckDigit(temp);

      return $"{digits}{checkCharacter}{trailingCharacter}";
   }

   private static Char GetCheckDigit(String ppsNumber)
   {
      var sum = 0;
      var weight = 8;
      for (var index = 0; index < 7; index++)
      {
         var num = ppsNumber[index] - Chars.DigitZero;
         sum += num * weight;
         weight--;
      }

      if (ppsNumber.Length == 9)
      {
         var trailingChar = Char.ToUpper(ppsNumber[8], CultureInfo.InvariantCulture);
         var trailingCharValue = trailingChar switch
         {
            >= Chars.UpperCaseA and <= Chars.UpperCaseI => trailingChar - Chars.UpperCaseA + 1,
            Chars.UpperCaseW => 0,
            _ => throw new InvalidOperationException(),
         };
         sum += trailingCharValue * 9;
      }

      var remainder = sum % 23;
      var checkCharacter = "WABCDEFGHIJKLMNOPQRSTUV"[remainder];     // Note leading W because W is used for zero remainder

      return checkCharacter;
   }

   public static TheoryData<String> ValidPpsNumberValues =>
   [
      ValidOriginalLengthPpsNumber,
      AltValidOriginalLengthPpsNumber,
      ValidExtendedLengthPpsNumber,
      AltValidExtendedLengthPpsNumber,
      ValidOriginalLengthPpsNumberWithWSuffix,
      LowerCaseValidOriginalLengthPpsNumber,
      LowerCaseAltValidOriginalLengthPpsNumber,
      LowerCaseValidExtendedLengthPpsNumber,
      LowerCaseAltValidExtendedLengthPpsNumber,
      LowerCaseValidOriginalLengthPpsNumberWithWSuffix,
      MixedCaseValidExtendedLengthPpsNumber,
      MixedCaseAltValidExtendedLengthPpsNumber,
   ];

   public static TheoryData<String> ValidTrailingCharacters =>
   [
      String.Empty,
      "A",
      "B",
      "C",
      "D",
      "E",
      "F",
      "G",
      "H",
      "I",
      "W",
      "a",
      "b",
      "c",
      "d",
      "e",
      "f",
      "g",
      "h",
      "i",
      "w",
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "123456T",              // Length 7
      "12345678FA",           // Length 10
      new String('1', 100)    // Very long string
   ];

   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      { " 234567T", 0 },           // non-digit character ' '
      { "1-34567T", 1 },           // non-digit character '-'
      { "12=4567T", 2 },           // non-digit character '='
      { "123A567T", 3 },           // non-digit character 'A'
      { "1234B67T", 4 },           // non-digit character 'B'
      { "12345C7T", 5 },           // non-digit character 'C'
      { "12345a7T", 5 },           // non-digit character 'a'
      { "12345b7T", 5 },           // non-digit character 'b'
      { "123456~T", 6 },           // non-digit character '~'
      { "123456\u2153T", 6 },      // non-digit character Unicode fraction 1/3
      { "123456\u00D6T", 6 },      // non-digit character unicode O with umlaut
      { "1234567 ", 7 },           // non-letter check character ' '
      { "12345678", 7 },           // non-letter check character '8'
      { "1234567-", 7 },           // non-letter check character '-'
      { "1234567~", 7 },           // non-letter check character '~'
      { "1234567\u2153", 7 },      // non-letter check character Unicode fraction 1/3
      { "1234567\u00D6", 7 },      // non-letter check character unicode O with umlaut
      { "1234567 W", 7 },          // non-letter check character ' '
      { "12345678W", 7 },          // non-letter check character '8'
      { "1234567-w", 7 },          // non-letter check character '-'
      { "1234567~w", 7 },          // non-letter check character '~'
      { "1234567\u2153W", 7 },     // non-letter check character Unicode fraction 1/3
      { "1234567\u00D6w", 7 },     // non-letter check character unicode O with umlaut
      { "1234567F ", 8 },          // non-letter trailing character ' '
      { "1234567F8", 8 },          // non-letter trailing character '8'
      { "1234567F-", 8 },          // non-letter trailing character '-'
      { "1234567F~", 8 },          // non-letter trailing character '~'
      { "1234567F\u2153", 8 },     // non-letter trailing character Unicode fraction 1/3
      { "1234567F\u00D6", 8 },     // non-letter trailing character unicode O with umlaut
      { "1234567FJ", 8 },          // invalid letter trailing character 'J'
      { "1234567Fj", 8 },          // invalid letter trailing character 'j'
      { "1234567FV", 8 },          // invalid letter trailing character 'V'
      { "1234567Fv", 8 },          // invalid letter trailing character 'v'
      { "1234567FX", 8 },          // invalid letter trailing character 'X'
      { "1234567Fx", 8 },          // invalid letter trailing character 'x'
   };

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "1224567T",                // 1234567T with single digit transcription error, 3 -> 2
      "7655321G",                // 7654321G with single digit transcription error, 4 -> 5
      "1122334OC",               // 1122334OB with trailing character transcription error, B -> C
      "1122334PB",               // 1122334OB with check digit transcription error, O -> P
      "1235467TW",               // 1234567TW with two digit transposition error, 45 -> 54
      "7564321PA",               // 7654321PA with two digit transposition error, 65 -> 56
      "1357910IG",               // 1357910GI with two character transposition error, GI -> IG
      "1234765T",                // 1234567T with two digit jump transposition, 567 -> 765
      "1122444OB",               // 1122334OB with two digit twin error, 33 -> 44
      "2222334T",                // 1122334T with two digit twin error, 11 -> 22
   ];

   private static InvalidLength GetInvalidLengthResult(
      String value,
      String? message = null)
      => new(
         message ?? Messages.IePpsNumberInvalidLength,
         value.Length,
         IePpsNumber.GetValidLengthDefinitions());

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.IePpsNumberInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.IePpsNumberInvalidCheckDigit,
         IePpsNumber.CheckDigitAlgorithmName);

   #region Check Digit Algorithm Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   // Values below are valid PPS Number values, and designed to produce all
   // possible check characters.
   [InlineData("0000054W")]         // Remainder = 0
   [InlineData("0000046A")]         // Remainder = 1
   [InlineData("0000055B")]         // Remainder = 2
   [InlineData("0000047C")]         // Remainder = 3
   [InlineData("0000056D")]         // Remainder = 4
   [InlineData("0000048E")]         // Remainder = 5
   [InlineData("0000057F")]         // Remainder = 6
   [InlineData("0000049G")]         // Remainder = 7
   [InlineData("0000058H")]         // Remainder = 8
   [InlineData("0000067I")]         // Remainder = 9
   [InlineData("0000059J")]         // Remainder = 10
   [InlineData("0000085K")]         // Remainder = 11
   [InlineData("0000094LW")]        // Remainder = 12
   [InlineData("0000086MW")]        // Remainder = 13
   [InlineData("0000095NW")]        // Remainder = 14
   [InlineData("0000087OW")]        // Remainder = 15
   [InlineData("0000096PW")]        // Remainder = 16
   [InlineData("0000088QW")]        // Remainder = 17
   [InlineData("0000097RW")]        // Remainder = 18
   [InlineData("0000089SW")]        // Remainder = 19
   [InlineData("0000098TW")]        // Remainder = 20
   [InlineData("0000904UW")]        // Remainder = 21
   [InlineData("0000099VW")]        // Remainder = 22

   // Lowercase values
   [InlineData("0000054w")]         // Remainder = 0
   [InlineData("0000046a")]         // Remainder = 1
   [InlineData("0000055b")]         // Remainder = 2
   [InlineData("0000047c")]         // Remainder = 3
   [InlineData("0000056d")]         // Remainder = 4
   [InlineData("0000048e")]         // Remainder = 5
   [InlineData("0000057f")]         // Remainder = 6
   [InlineData("0000049g")]         // Remainder = 7
   [InlineData("0000058h")]         // Remainder = 8
   [InlineData("0000067i")]         // Remainder = 9
   [InlineData("0000059j")]         // Remainder = 10
   [InlineData("0000085k")]         // Remainder = 11
   [InlineData("0000094lw")]        // Remainder = 12
   [InlineData("0000086mw")]        // Remainder = 13
   [InlineData("0000095nw")]        // Remainder = 14
   [InlineData("0000087ow")]        // Remainder = 15
   [InlineData("0000096pw")]        // Remainder = 16
   [InlineData("0000088qw")]        // Remainder = 17
   [InlineData("0000097rw")]        // Remainder = 18
   [InlineData("0000089sw")]        // Remainder = 19
   [InlineData("0000098tw")]        // Remainder = 20
   [InlineData("0000904uw")]        // Remainder = 21
   [InlineData("0000099vw")]        // Remainder = 22

   // Mixed cases.
   [InlineData("0000094Lw")]        // Remainder = 12
   [InlineData("0000086mW")]        // Remainder = 13
   public void IePpsNumber_CheckDigitAlgorithm_ShouldGenerateAllPossibleCharacters(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = IePpsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPpsNumberValues))]
   public void IePpsNumber_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = value.ToUpperInvariant();

      // Act.
      var sut = new IePpsNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidTrailingCharacters))]
   public void IePpsNumber_Constructor_ShouldCreateInstance_WhenValueHasValidTrailingCharacter(String trailingCharacter)
   {
      // Arrange.
      var value = GetPpsNumberWithValidCheckDigit(trailingCharacter: trailingCharacter);
      var expected = value.ToUpperInvariant();

      // Act.
      var sut = new IePpsNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void IePpsNumber_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new IePpsNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void IePpsNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new IePpsNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void IePpsNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new IePpsNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void IePpsNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new IePpsNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPpsNumberValues))]
   public void IePpsNumber_Value_ShouldReturnValidatedPpsNumber(String value)
   {
      // Arrange.
      var sut = new IePpsNumber(value);
      var expected = value.ToUpperInvariant();

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IePpsNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidExtendedLengthPpsNumber;
      var sut = new IePpsNumber(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void IePpsNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = MixedCaseAltValidExtendedLengthPpsNumber;
      var sut = new IePpsNumber(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void IePpsNumber_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      IePpsNumber sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void IePpsNumber_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      IePpsNumber sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidPpsNumberValues))]
   public void IePpsNumber_ExplicitCastToIePpsNumber_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new IePpsNumber(value);

      // Act.
      var sut = (IePpsNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidTrailingCharacters))]
   public void IePpsNumber_ExplicitCastToIePpsNumber_ShouldCreateInstance_WhenValueHasValidTrailingCharacter(String trailingCharacter)
   {
      // Arrange.
      var value = GetPpsNumberWithValidCheckDigit(trailingCharacter: trailingCharacter);
      var expected = new IePpsNumber(value);

      // Act.
      var sut = (IePpsNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void IePpsNumber_ExplicitCastToIePpsNumber_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (IePpsNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void IePpsNumber_ExplicitCastToIePpsNumber_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (IePpsNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void IePpsNumber_ExplicitCastToIePpsNumber_ShouldThrowKfValidationException_WhenValueHasInvalidCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (IePpsNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void IePpsNumber_ExplicitCastToIePpsNumber_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (IePpsNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IePpsNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new IePpsNumber(ValidOriginalLengthPpsNumber);
      var sut2 = new IePpsNumber(ValidOriginalLengthPpsNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void IePpsNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new IePpsNumber(ValidExtendedLengthPpsNumber);
      var sut2 = new IePpsNumber(AltValidOriginalLengthPpsNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void IePpsNumber_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentCase()
   {
      // Arrange. different case versions for same person should still be equal.
      var sut1 = new IePpsNumber(ValidExtendedLengthPpsNumber);
      var sut2 = new IePpsNumber(MixedCaseValidExtendedLengthPpsNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IePpsNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new IePpsNumber(ValidExtendedLengthPpsNumber);
      var sut2 = new IePpsNumber(AltValidOriginalLengthPpsNumber);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void IePpsNumber_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentCase()
   {
      // Arrange. different case versions for same person should still be equal.
      var sut1 = new IePpsNumber(ValidExtendedLengthPpsNumber);
      var sut2 = new IePpsNumber(MixedCaseValidExtendedLengthPpsNumber);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void IePpsNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new IePpsNumber(ValidOriginalLengthPpsNumber);
      var sut2 = new IePpsNumber(ValidOriginalLengthPpsNumber);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPpsNumberValues))]
   public void IePpsNumber_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new IePpsNumber(value);

      // Act.
      var result = IePpsNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidTrailingCharacters))]
   public void IePpsNumber_Create_ShouldCreateInstance_WhenValueHasValidTrailingCharacter(String trailingCharacter)
   {
      // Arrange.
      var value = GetPpsNumberWithValidCheckDigit(trailingCharacter: trailingCharacter);
      LocalCreateResult expected = new IePpsNumber(value);

      // Act.
      var result = IePpsNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void IePpsNumber_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = IePpsNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void IePpsNumber_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = IePpsNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void IePpsNumber_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasInvalidCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = IePpsNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void IePpsNumber_Create_ShouldReturnInvalidCheckDigitsValidationResult_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = IePpsNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IePpsNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new IePpsNumber(ValidOriginalLengthPpsNumber);
      var sut2 = new IePpsNumber(ValidOriginalLengthPpsNumber);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void IePpsNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new IePpsNumber(ValidExtendedLengthPpsNumber);
      var sut2 = new IePpsNumber(AltValidOriginalLengthPpsNumber);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void IePpsNumber_Equals_ShouldReturnTrue_WhenValuesHaveDifferentCase()
   {
      // Arrange. different case versions for same person should still be equal.
      var sut1 = new IePpsNumber(ValidExtendedLengthPpsNumber);
      var sut2 = new IePpsNumber(MixedCaseValidExtendedLengthPpsNumber);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void IePpsNumber_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new IePpsNumber(ValidExtendedLengthPpsNumber);

      // Act/assert.
      sut.Equals(ValidExtendedLengthPpsNumber).Should().BeFalse();
   }

   [Fact]
   public void IePpsNumber_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new IePpsNumber(ValidExtendedLengthPpsNumber);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IePpsNumber_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new IePpsNumber(ValidOriginalLengthPpsNumber);
      var sut2 = new IePpsNumber(ValidOriginalLengthPpsNumber);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void IePpsNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new IePpsNumber(ValidExtendedLengthPpsNumber);
      var sut2 = new IePpsNumber(AltValidOriginalLengthPpsNumber);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void IePpsNumber_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentCase()
   {
      // Arrange. different case versions for same person should still be equal.
      var sut1 = new IePpsNumber(ValidExtendedLengthPpsNumber);
      var sut2 = new IePpsNumber(MixedCaseValidExtendedLengthPpsNumber);

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

   // IePpsNumber does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void IePpsNumber_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new IePpsNumber(ValidExtendedLengthPpsNumber);
      var sut2 = new IePpsNumber(ValidExtendedLengthPpsNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPpsNumberValues))]
   public void IePpsNumber_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new IePpsNumber(value);
      var expected = value.ToUpperInvariant();

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPpsNumberValues))]
   public void IePpsNumber_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = IePpsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidTrailingCharacters))]
   public void IePpsNumber_Validate_ShouldReturnValidationPassed_WhenTrailingCharacterIsValid(String trailingCharacter)
   {
      // Arrange.
      var value = GetPpsNumberWithValidCheckDigit(trailingCharacter: trailingCharacter);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = IePpsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void IePpsNumber_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = IePpsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void IePpsNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = IePpsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void IePpsNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = IePpsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void IePpsNumber_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = IePpsNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IePpsNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new IePpsNumber(ValidOriginalLengthPpsNumber);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<IePpsNumber>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void IePpsNumber_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new IePpsNumber(ValidExtendedLengthPpsNumber);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public IePpsNumber PpsNumber { get; set; } = null!;
   }

   [Fact]
   public void IePpsNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { PpsNumber = new IePpsNumber(AltValidOriginalLengthPpsNumber) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void IePpsNumber_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"PpsNumber\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void IePpsNumber_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"PpsNumber\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.PpsNumber.Should().BeNull();
   }

   [Fact]
   public void IePpsNumber_JsonDeserialization_ShouldThrowKfValidationException_WhenPpsNumberIsInvalid()
   {
      // Arrange.
      var json = "{\"PpsNumber\":\"1224567T\"}";  // Invalid check character
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
