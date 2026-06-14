// Ignore Spelling: Deserialization Deserialize Json Kf Luhn

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

using LocalCreateResult = KfAccountNumbers.Results.UCreateResult<
   KfAccountNumbers.Governmental.NorthAmerica.CaSocialInsuranceNumber,
   KfAccountNumbers.Governmental.NorthAmerica.CaSocialInsuranceNumber.ValidationError>;
using LocalValidationError = KfAccountNumbers.Governmental.NorthAmerica.CaSocialInsuranceNumber.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.Governmental.NorthAmerica.CaSocialInsuranceNumber.ValidationError>;
using LocalValidationResult = KfAccountNumbers.Governmental.NorthAmerica.CaSocialInsuranceNumber.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class CaSocialInsuranceNumberTests
{
   private const String ValidUnformattedSin = "558199428";     // From singen.ca
   private const String AltValidUnformattedSin = "226019727";  // From singen.ca
   private const String ValidFormattedSin = "558-199-428";
   private const String AltFormattedSin = "226 019 727";

   // Values that will successfully create a CaSocialInsuranceNumber object
   public static TheoryData<String> ValidValues =>
   [
      ValidUnformattedSin,
      AltValidUnformattedSin,
      ValidFormattedSin,
      AltFormattedSin
   ];

   // Values that will report an invalid length
   public static TheoryData<String> InvalidLengthValues =>
   [
      "55819942",
      "5581994288",
      "558-199-4288",
      "558 199 4288"
   ];

   // Values that will report an invalid separator character
   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      // First separator position
      { "5580199 428", 3 },
      { "5581199 428", 3 },
      { "5582199 428", 3 },
      { "5583199 428", 3 },
      { "5584199 428", 3 },
      { "5585199 428", 3 },
      { "5586199 428", 3 },
      { "5587199 428", 3 },
      { "5588199 428", 3 },
      { "5589199 428", 3 },

      // Second separator position
      { "558 1990428", 7 },
      { "558 1991428", 7 },
      { "558 1992428", 7 },
      { "558 1993428", 7 },
      { "558 1994428", 7 },
      { "558 1995428", 7 },
      { "558 1996428", 7 },
      { "558 1997428", 7 },
      { "558 1998428", 7 },
      { "558 1999428", 7 },

      // Mixed separators
      { "558 199-428", 7 },
      { "558-199 428", 7 },
   };

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted values
      { ".58199428", 0 },           // Non-digit character '.'
      { "5 8199428", 1 },           // Non-digit character ' '
      { "55A199628", 2 },           // Non-digit character 'A'
      { "558Z99628", 3 },           // Non-digit character 'Z'
      { "5581^9628", 4 },           // Non-digit character '^'
      { "55819a628", 5 },           // Non-digit character 'a'
      { "558199z28", 6 },           // Non-digit character 'z'
      { "5581994~8", 7 },           // Non-digit character '~'
      { "55819942\u2153", 8 },      // Non-digit character Unicode fraction 1/3
      { "55819942\u00D6", 8 },      // Invalid character unicode O with umlaut

      // Formatted values
      { ".58 199 428", 0 },           // Non-digit character '.'
      { "5 8 199 428", 1 },           // Non-digit character ' '
      { "55A 199 628", 2 },           // Non-digit character 'A'
      { "558 Z99 628", 4 },           // Non-digit character 'Z'
      { "558 1^9 628", 5 },           // Non-digit character '^'
      { "558-19a-628", 6 },           // Non-digit character 'a'
      { "558-199-z28", 8 },           // Non-digit character 'z'
      { "558-199-4~8", 9 },           // Non-digit character '~'
      { "558-199-42\u2153", 10 },     // Non-digit character Unicode fraction 1/3
      { "558-199-42\u00D6", 10 },     // Invalid character unicode O with umlaut
   };

   // Values that will report an invalid province
   public static TheoryData<String> InvalidProvinceValues =>
   [
      "012345674",
      "876543216",
      "012 345 674",
      "876 543 216",
   ];

   // Values that contain a transcription error that is undetectable by the Luhn algorithm
   // and will successfully create a CaSocialInsuranceNumber object
   public static TheoryData<String> CheckDigitUndetectableErrorValues =>
   [
      "780912341",            // 789012341 with two digit transposition 90 -> 09
      "123459018",            // 123450918 with two digit transposition 09 -> 90
      "100005503",            // 100005503 with two digit twin error 55 -> 22
      "107700007",            // 104400007 with two digit twin error 44 -> 77
      "103300000",            // 106600000 with two digit twin error 66 -> 33
      "558199428",            // 558199428 with two digit jump transposition 994 -> 499
      "780-912-341",          // 789012341 with two digit transposition 90 -> 09
      "123-459-018",          // 123450918 with two digit transposition 09 -> 90
      "100-005-503",          // 100005503 with two digit twin error 55 -> 22
      "107-700-007",          // 104400007 with two digit twin error 44 -> 77
      "103-300-000",          // 106600000 with two digit twin error 66 -> 33
      "558-199-428",          // 558199428 with two digit jump transposition 994 -> 499
   ];

   // Values that will report an invalid check digit
   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "558299428",            // 558199428 with single digit transcription error 1 -> 2
      "559199428",            // 558199428 with single digit transcription error 8 -> 9
      "551899428",            // 558199428 with two digit transcription error -> 81 -> 18
      "558199248",            // 558199428 with two digit transcription error -> 42 -> 24
      "448199428",            // 558199428 with two digit twin error 55 -> 44
      "558188428",            // 558199428 with two digit twin error 99 -> 88
      "558 299 428",          // 558199428 with single digit transcription error 1 -> 2
      "559 199 428",          // 558199428 with single digit transcription error 8 -> 9
      "551 899 428",          // 558199428 with two digit transcription error -> 81 -> 18
      "558 199 248",          // 558199428 with two digit transcription error -> 42 -> 24
      "448 199 428",          // 558199428 with two digit twin error 55 -> 44
      "558 188 428",          // 558199428 with two digit twin error 99 -> 88
   ];

   // Values that contain a Luhn check digit that calculates as zero and will
   // successfully create a CaSocialInsuranceNumber object
   public static TheoryData<String> ZeroCheckDigitValues =>
   [
      "123456790",
      "123-456-790",
   ];

   /// <summary>
   /// Extracts unformatted SIN value. If SIN is 9 characters then value is
   /// returned unchanged. If an 11-character formatted SIN then assumes
   /// separators at positions 3 and 7.
   /// </summary>
   private static String GetRawValue(String sin)
      => sin.Length switch
      {
         9 => sin,
         11 => sin[0..3] + sin[4..7] + sin[8..11],
         _ => throw new ArgumentException("Input must be 9 or 11 characters", nameof(sin)),
      };

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.CaSinInvalidLength,
         value.Length,
         CaSocialInsuranceNumber.GetInvalidLengthDefinitions());

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(
         Messages.CaSinInvalidSeparator,
         value[position],
         position);

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.CaSinInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.CaSinInvalidCheckDigit, Algorithms.Luhn.AlgorithmName);

   private static CaSinInvalidProvince GetInvalidProvinceResult(String value)
      => new(Messages.CaSinInvalidProvince, value[0]);

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = new CaSocialInsuranceNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenValueIsEmpty(String? value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new CaSocialInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new CaSocialInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new CaSocialInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new CaSocialInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidProvinceValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidProvinceCode(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidProvinceResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new CaSocialInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldCreateInstance_WhenCheckDigitContainsUndetectableError(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = new CaSocialInsuranceNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenCheckDigitContainsDetectableError(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new CaSocialInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldCreateInstance_WhenCheckDigitCalculatesAsZero(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = new CaSocialInsuranceNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidUnformattedSin)]
   [InlineData(ValidFormattedSin)]
   public void CaSocialInsuranceNumber_Value_ShouldReturnRawSin(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);
      var sut = new CaSocialInsuranceNumber(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);
      var sut = new CaSocialInsuranceNumber(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);
      var sut = new CaSocialInsuranceNumber(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Fact]
   public void CaSocialInsuranceNumber_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      CaSocialInsuranceNumber sut = null!;

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void CaSocialInsuranceNumber_CastCaSinToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      CaSocialInsuranceNumber sut = null!;

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = (CaSocialInsuranceNumber)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldThrowKfValidationException_WhenValueIsEmpty(String? value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => (CaSocialInsuranceNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new CaSocialInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => (CaSocialInsuranceNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldThrowKfValidationException_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => (CaSocialInsuranceNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidProvinceValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldThrowKfValidationException_WhenValueHasInvalidProvinceCode(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidProvinceResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (CaSocialInsuranceNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldCreateInstance_WhenCheckDigitContainsUndetectableError(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = (CaSocialInsuranceNumber)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldThrowKfValidationException_WhenCheckDigitContainsDetectableError(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => (CaSocialInsuranceNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldCreateInstance_WhenCheckDigitCalculatesAsZero(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = (CaSocialInsuranceNumber)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CaSocialInsuranceNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var sut2 = new CaSocialInsuranceNumber(ValidUnformattedSin);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void CaSocialInsuranceNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var sut2 = new CaSocialInsuranceNumber(AltValidUnformattedSin);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void CaSocialInsuranceNumber_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void CaSocialInsuranceNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidFormattedSin);
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin.Replace('-', '.'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void CaSocialInsuranceNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidFormattedSin.Replace('-', 'A'));
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin.Replace('-', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CaSocialInsuranceNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var sut2 = new CaSocialInsuranceNumber(AltValidUnformattedSin);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void CaSocialInsuranceNumber_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void CaSocialInsuranceNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var sut2 = new CaSocialInsuranceNumber(ValidUnformattedSin);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void CaSocialInsuranceNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidFormattedSin);
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin.Replace('-', '.'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void CaSocialInsuranceNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidFormattedSin.Replace('-', 'A'));
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin.Replace('-', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new CaSocialInsuranceNumber(value);

      // Act.
      var result = CaSocialInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void CaSocialInsuranceNumber_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = CaSocialInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void CaSocialInsuranceNumber_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = CaSocialInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void CaSocialInsuranceNumber_Create_ShouldReturnInvalidSeparatorResult_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = CaSocialInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void CaSocialInsuranceNumber_Create_ShouldReturnInvalidCharacterResult_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = CaSocialInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidProvinceValues))]
   public void CaSocialInsuranceNumber_Create_ShouldReturnInvalidProvinceResult_WhenValueHasInvalidProvinceCode(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidProvinceResult(value);

      // Act.
      var result = CaSocialInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void CaSocialInsuranceNumber_Create_ShouldCreateInstance_WhenCheckDigitContainsUndetectableError(String value)
   {
      // Arrange.
      LocalCreateResult expected = new CaSocialInsuranceNumber(value);

      // Act.
      var result = CaSocialInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void CaSocialInsuranceNumber_Create_ShouldReturnInvalidCheckDigit_WhenCheckDigitContainsDetectableError(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = CaSocialInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValues))]
   public void CaSocialInsuranceNumber_Create_ShouldCreateInstance_WhenCheckDigitCalculatesAsZero(String value)
   {
      // Arrange.
      LocalCreateResult expected = new CaSocialInsuranceNumber(value);

      // Act.
      var result = CaSocialInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CaSocialInsuranceNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var sut2 = new CaSocialInsuranceNumber(ValidUnformattedSin);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void CaSocialInsuranceNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var sut2 = new CaSocialInsuranceNumber(AltValidUnformattedSin);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void CaSocialInsuranceNumber_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void CaSocialInsuranceNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidFormattedSin);
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin.Replace('-', '.'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void CaSocialInsuranceNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidFormattedSin.Replace('-', 'A'));
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin.Replace('-', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void CaSocialInsuranceNumber_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(ValidUnformattedSin);

      // Act/assert.
      sut.Equals(ValidUnformattedSin).Should().BeFalse();
   }

   [Fact]
   public void CaSocialInsuranceNumber_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(ValidUnformattedSin);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CaSocialInsuranceNumber_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var expected = ValidFormattedSin;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void CaSocialInsuranceNumber_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(AltValidUnformattedSin);
      var mask = "___ ___ ___";
      var expected = AltFormattedSin;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void CaSocialInsuranceNumber_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(ValidUnformattedSin);
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
   public void CaSocialInsuranceNumber_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(ValidUnformattedSin);
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
   public void CaSocialInsuranceNumber_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin);    // Same internal value

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void CaSocialInsuranceNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var sut2 = new CaSocialInsuranceNumber(AltValidUnformattedSin);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void CaSocialInsuranceNumber_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void CaSocialInsuranceNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidFormattedSin);
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin.Replace('-', '.'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void CaSocialInsuranceNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidFormattedSin.Replace('-', 'A'));
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin.Replace('-', 'a'));

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

   // CaSocialInsuranceNumber does not override Object.ReferenceEquals, so this
   // test just confirms that two different instances with the same value are
   // not considered reference equal.

   [Fact]
   public void CaSocialInsuranceNumber_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new CaSocialInsuranceNumber(ValidUnformattedSin);
      var sut2 = new CaSocialInsuranceNumber(ValidFormattedSin);    // Same internal value

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(value);
      var expected = GetRawValue(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnValidValue_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = CaSocialInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = CaSocialInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = CaSocialInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = CaSocialInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidCharacter_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = CaSocialInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidProvinceValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidProvince_WhenValueHasInvalidProvinceCode(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidProvinceResult(value);

      // Act.
      var result = CaSocialInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnValidValue_WhenCheckDigitContainsUndetectableError(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = CaSocialInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidCheckDigit_WhenCheckDigitContainsDetectableError(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = CaSocialInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnValidValue_WhenCheckDigitCalculatesAsZero(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = CaSocialInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CaSocialInsuranceNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(ValidUnformattedSin);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<CaSocialInsuranceNumber>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void CaSocialInsuranceNumber_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(ValidUnformattedSin);

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{ValidUnformattedSin}\"");  // Simple string, not object
   }

   public class Foo
   {
      public CaSocialInsuranceNumber Sin { get; set; } = null!;
   }

   [Fact]
   public void CaSocialInsuranceNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Sin = new CaSocialInsuranceNumber(ValidUnformattedSin) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void CaSocialInsuranceNumber_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Sin\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void CaSocialInsuranceNumber_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Sin\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Sin.Should().BeNull();
   }

   [Fact]
   public void CaSocialInsuranceNumber_JsonDeserialization_ShouldThrowKfValidationException_WhenSinIsInvalid()
   {
      // Arrange.
      var json = "{\"Sin\":\"558299428\"}";  // Invalid check digit
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
