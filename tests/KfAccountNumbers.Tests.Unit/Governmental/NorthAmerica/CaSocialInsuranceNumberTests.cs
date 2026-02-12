// Ignore Spelling: Deserialization Deserialize Json Luhn

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class CaSocialInsuranceNumberTests
{
   private const String ValidNineCharSin = "558199428";     // From singen.ca
   private const String AltValidNineCharSin = "226019727";  // From singen.ca
   private const String ValidElevenCharSin = "558-199-428";
   private const String AltElevenCharSin = "226 019 727";

   // Values that will successfully create a CaSocialInsuranceNumber object
   public static TheoryData<String> ValidValues =>
   [
      ValidNineCharSin,
      ValidElevenCharSin,
      AltElevenCharSin
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
   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "046 454-286",
      "046-454 286",
      "04604540286",
      "04614541286",
      "04624542286",
      "04634543286",
      "04644544286",
      "04654545286",
      "04664546286",
      "04674547286",
      "04684548286",
      "04694549286",
   ];

   // Values that will report an invalid character encountered
   public static TheoryData<String> InvalidCharacterValues =>
   [
      "A46454286",
      "0A6454286",
      "04A454286",
      "046A54286",
      "0464A4286",
      "04645A286",
      "046454A86",
      "0464542A6",
      "04645428A",
      "0;6454286",
      "0\u21536454286",       // Unicode fraction 1/3
      "0\u21676454286",       // Unicode Roman numeral VII
      "0\u0BEF6454286",       // Unicode Tamil number 9
      "A46-454-286",
      "0A6-454-286",
      "04A-454-286",
      "046-A54-286",
      "046-4A4-286",
      "046-45A-286",
      "046-454-A86",
      "046-454-2A6",
      "046-454-28A",
      "0;6-454-286",
      "0\u21536-454-286",     // Unicode fraction 1/3
      "0\u21676-454-286",     // Unicode Roman numeral VII
      "0\u0BEF6-454-286",     // Unicode Tamil number 9
   ];

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

   // Values that contain a Luhn check digit that calcuates as zero and will
   // successfully create a CaSocialInsuranceNumber object
   public static TheoryData<String> ZeroCheckDigitValues =>
   [
      "123456790",
      "123-456-790",
   ];

   public static TheoryData<String> EmptySinValues =>
   [
      null!,
      String.Empty,
      "\t"
   ];

   /// <summary>
   /// Extracts unformatted SIN value. If Sin is 9 characters then value is
   /// returned unchanged. If an 11-character formatted SIN then assumes
   /// separators at positions 3 and 7.
   /// </summary>
   private static String GetRawSin(String sin)
      => sin.Length switch
      {
         9 => sin,
         11 => sin[0..3] + sin[4..7] + sin[8..11],
         _ => throw new ArgumentException("Input must be 9 or 11 characters", nameof(sin))
      };

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldCreateObject_WhenValueContainsValidSin(String sin)
   {
      // Arrange.
      var expected = GetRawSin(sin);

      // Act.
      var sut = new CaSocialInsuranceNumber(sin);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(EmptySinValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueIsEmpty(String? sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinEmpty + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueHasInvalidLength(String? sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidLength + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowInvalidCaSocialInsuranceNumberException_When11CharacterValueContainsAnInvalidSeparator(String sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidSeparatorEncountered + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueContainsNonAsciiDigit(String sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidProvinceValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueHasInvalidLeadingDigit(String sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidProvince + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidProvince);

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String sin)
   {
      // Arrange.
      var expected = GetRawSin(sin);

      // Act.
      var sut = new CaSocialInsuranceNumber(sin);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenCheckDigitContainsDetectableError(String sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValues))]
   public void CaSocialInsuranceNumber_Constructor_ShouldCreateObject_WhenCheckDigitCalculatesAsZero(String sin)
   {
      // Arrange.
      var expected = GetRawSin(sin);

      // Act.
      var sut = new CaSocialInsuranceNumber(sin);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSin)]
   [InlineData(ValidElevenCharSin)]
   public void CaSocialInsuranceNumber_Value_ShouldReturnUnformattedItin(String sin)
   {
      // Arrange.
      var expected = GetRawSin(sin);
      var sut = new CaSocialInsuranceNumber(sin);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Implicit Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull(String sin)
   {
      // Arrange.
      var expected = GetRawSin(sin);
      var sut = new CaSocialInsuranceNumber(sin);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull(String sin)
   {
      // Arrange.
      var expected = GetRawSin(sin);
      var sut = new CaSocialInsuranceNumber(sin);

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
      CaSocialInsuranceNumber sin = null!;

      // Act.
      String str = sin;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void CaSocialInsuranceNumber_CastCaSinToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      CaSocialInsuranceNumber sin = null!;

      // Act.
      var str = (String)sin;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldCreateObject_WhenValueContainsValidSin(String str)
   {
      // Arrange.
      var expected = GetRawSin(str);

      // Act.
      var sut = (CaSocialInsuranceNumber)str;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(EmptySinValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueIsEmpty(String? str)
      => FluentActions
         .Invoking(() => _ = (CaSocialInsuranceNumber)str)
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinEmpty + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueHasInvalidLength(String str)
      => FluentActions
         .Invoking(() => _ = (CaSocialInsuranceNumber)str)
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidLength + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldThrowInvalidCaSocialInsuranceNumberException_When11CharacterValueContainsAnInvalidSeparator(String str)
      => FluentActions
         .Invoking(() => _ = (CaSocialInsuranceNumber)str)
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidSeparatorEncountered + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueContainsNonAsciiDigit(String str)
      => FluentActions
         .Invoking(() => _ = (CaSocialInsuranceNumber)str)
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidProvinceValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueHasInvalidLeadingDigit(String str)
      => FluentActions
         .Invoking(() => _ = (CaSocialInsuranceNumber)str)
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidProvince + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidProvince);

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String str)
   {
      // Arrange.
      var expected = GetRawSin(str);

      // Act.
      var sut = (CaSocialInsuranceNumber)str;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenCheckDigitContainsDetectableError(String str)
      => FluentActions
         .Invoking(() => _ = (CaSocialInsuranceNumber)str)
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValues))]
   public void CaSocialInsuranceNumber_ExplicitCastToCaSin_ShouldCreateObject_WhenCheckDigitCalculatesAsZero(String str)
   {
      // Arrange.
      var expected = GetRawSin(str);

      // Act.
      var sut = (CaSocialInsuranceNumber)str;

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
      var sin1 = new CaSocialInsuranceNumber(ValidNineCharSin);
      var sin2 = new CaSocialInsuranceNumber(ValidElevenCharSin);    // Same internal value

      // Act/assert.
      (sin1 == sin2).Should().BeTrue();
   }

   [Fact]
   public void CaSocialInsuranceNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sin1 = new CaSocialInsuranceNumber(ValidNineCharSin);
      var sin2 = new CaSocialInsuranceNumber(AltValidNineCharSin);

      // Act/assert.
      (sin1 == sin2).Should().BeFalse();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CaSocialInsuranceNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sin1 = new CaSocialInsuranceNumber(ValidNineCharSin);
      var sin2 = new CaSocialInsuranceNumber(AltValidNineCharSin);

      // Act/assert.
      (sin1 != sin2).Should().BeTrue();
   }

   [Fact]
   public void CaSocialInsuranceNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sin1 = new CaSocialInsuranceNumber(ValidNineCharSin);
      var sin2 = new CaSocialInsuranceNumber(ValidElevenCharSin);    // Same internal value

      // Act/assert.
      (sin1 != sin2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_Create_ShouldCreateObject_WhenValueContainsValidSin(String sin)
   {
      // Arrange.
      var expected = GetRawSin(sin);
      var expectedValue = new CaSocialInsuranceNumber(expected);

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(EmptySinValues))]
   public void CaSocialInsuranceNumber_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String? sin)
   {
      // Arrange.
      var expected = CaSocialInsuranceNumberValidationResult.Empty;

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin!);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void CaSocialInsuranceNumber_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String sin)
   {
      // Arrange.
      var expected = CaSocialInsuranceNumberValidationResult.InvalidLength;

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void CaSocialInsuranceNumber_Create_ShouldReturnInvalidSeparatorEncounteredResult_When11CharacterValueContainsAnInvalidSeparator(String sin)
   {
      // Arrange.
      var expected = CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered;

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void CaSocialInsuranceNumber_Create_ShouldReturnInvalidCharacterEncounteredResult_WhenValueContainsNonAsciiDigit(String sin)
   {
      // Arrange.
      var expected = CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered;

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidProvinceValues))]
   public void CaSocialInsuranceNumber_Create_ShouldReturnInvalidProvinceResult_WhenValueHasInvalidLeadingDigit(String sin)
   {
      // Arrange.
      var expected = CaSocialInsuranceNumberValidationResult.InvalidProvince;

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void CaSocialInsuranceNumber_Create_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String sin)
   {
      // Arrange.
      var expected = GetRawSin(sin);
      var expectedValue = new CaSocialInsuranceNumber(expected);

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void CaSocialInsuranceNumber_Create_ShouldReturnInvalidCheckDigit_WhenCheckDigitContainsDetectableError(String sin)
   {
      // Arrange.
      var expected = CaSocialInsuranceNumberValidationResult.InvalidCheckDigit;

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValues))]
   public void CaSocialInsuranceNumber_Create_ShouldCreateObject_WhenCheckDigitCalculatesAsZero(String sin)
   {
      // Arrange.
      var expected = GetRawSin(sin);
      var expectedValue = new CaSocialInsuranceNumber(expected);

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CaSocialInsuranceNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var itin1 = new CaSocialInsuranceNumber(ValidNineCharSin);
      var itin2 = new CaSocialInsuranceNumber(ValidElevenCharSin);    // Same internal value

      // Act/assert.
      itin1.Equals(itin2).Should().BeTrue();
   }

   [Fact]
   public void CaSocialInsuranceNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var itin1 = new CaSocialInsuranceNumber(ValidNineCharSin);
      var itin2 = new CaSocialInsuranceNumber(AltValidNineCharSin);

      // Act/assert.
      itin1.Equals(itin2).Should().BeFalse();
   }

   [Fact]
   public void CaSocialInsuranceNumber_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(ValidNineCharSin);

      // Act/assert.
      sut.Equals(ValidNineCharSin).Should().BeFalse();
   }

   [Fact]
   public void CaSocialInsuranceNumber_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(ValidNineCharSin);

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
      var sut = new CaSocialInsuranceNumber(ValidNineCharSin);
      var expected = ValidElevenCharSin;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void CaSocialInsuranceNumber_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(AltValidNineCharSin);
      var mask = "___ ___ ___";
      var expected = AltElevenCharSin;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void CaSocialInsuranceNumber_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(ValidNineCharSin);
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
      var sut = new CaSocialInsuranceNumber(ValidNineCharSin);
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
      var sin1 = new CaSocialInsuranceNumber(ValidNineCharSin);
      var sin2 = new CaSocialInsuranceNumber(ValidElevenCharSin);    // Same internal value

      // Act.
      var hash1 = sin1.GetHashCode();
      var hash2 = sin2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void CaSocialInsuranceNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sin1 = new CaSocialInsuranceNumber(ValidNineCharSin);
      var sin2 = new CaSocialInsuranceNumber(AltValidNineCharSin);

      // Act.
      var hash1 = sin1.GetHashCode();
      var hash2 = sin2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   #endregion

   #region ObjectReferenceEquals Method Tests
   // ==========================================================================
   // ==========================================================================

   // CaSocialInsuranceNumber does not override Object.ReferenceEquals, so this
   // test just confirms that two different instances with the same value are
   // not considered reference equal.

   [Fact]
   public void CaSocialInsuranceNumber_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var itin1 = new CaSocialInsuranceNumber(ValidNineCharSin);
      var itin2 = new CaSocialInsuranceNumber(ValidElevenCharSin);    // Same internal value

      // Act/assert.
      (itin1 == itin2).Should().BeTrue();                         // Value equality should be true
      Object.ReferenceEquals(itin1, itin2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_ToString_ShouldReturnExpectedValue(String sin)
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(sin);
      var expected = GetRawSin(sin);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenValueContainsValidSin(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(EmptySinValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidSeparatorEncountered_When11CharacterValueContainsAnInvalidSeparator(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidCharacterEncountered_WhenValueContainsNonAsciiDigit(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidProvinceValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidProvince_WhenValueHasInvalidLeadingDigit(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidProvince);

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenCheckDigitContainsUndetectableError(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenCheckDigitContainsDetectableError(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenCheckDigitCalculatesAsZero(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CaSocialInsuranceNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(ValidNineCharSin);

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
      var sut = new CaSocialInsuranceNumber(ValidNineCharSin);

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{ValidNineCharSin}\"");  // Simple string, not object
   }

   public class Foo
   {
      public CaSocialInsuranceNumber Sin { get; set; } = null!;
   }

   [Fact]
   public void CaSocialInsuranceNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Sin = new CaSocialInsuranceNumber(ValidNineCharSin) };
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
   public void CaSocialInsuranceNumber_JsonDeserialization_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenSinIsInvalid()
   {
      // Arrange.
      var json = "{\"Sin\":\"55819942\"}";  // Invalid length

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidLength + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidLength);
   }

   #endregion
}
