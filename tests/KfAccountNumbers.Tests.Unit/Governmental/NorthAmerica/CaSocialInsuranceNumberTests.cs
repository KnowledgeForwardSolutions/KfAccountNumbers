// Ignore Spelling: Luhn

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class CaSocialInsuranceNumberTests
{
   private const String ValidNineCharSin = "558199428";     // From singen.ca
   private const String AltValidNineCharSin = "226019727";  // From singen.ca
   private const String ValidElevenCharSin = "558-199-428";
   private const String ValidElevenCharSinWithCustomSeparator = "558 199 428";

   private const Char CustomSeparator = ' ';
   private const Char DefaultSeparator = '-';

   // Values that will successfully create a CaSocialInsuranceNumber object
   public static TheoryData<String> ValidValuesNoSeparator =>
   [
      ValidNineCharSin,
   ];

   public static TheoryData<String> ValidValuesDefaultSeparator =>
   [
      ValidElevenCharSin
   ];

   public static TheoryData<String> ValidValuesCustomSeparator =>
   [
      ValidElevenCharSinWithCustomSeparator
   ];

   // Values that will report an invalid length
   public static TheoryData<String> InvalidLengthValuesNoSeparator =>
   [
      "55819942",
      "5581994288",
   ];

   public static TheoryData<String> InvalidLengthValuesDefaultSeparator =>
   [
      "558-199-4288"
   ];

   public static TheoryData<String> InvalidLengthValuesCustomSeparator =>
   [
      "558 199 4288"
   ];

   // Values that will report an invalid separator character
   public static TheoryData<String> InvalidSeparatorValuesDefaultSeparator =>
   [
      "046_454-286",
      "046-454_286",
   ];

   public static TheoryData<String> InvalidSeparatorValuesCustomSeparator =>
   [
      "046_454 286",
      "046 454_286",
   ];

   // Values that will report an invalid character encountered
   public static TheoryData<String> InvalidCharacterValuesNoSeparator =>
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
   ];

   public static TheoryData<String> InvalidCharacterValuesDefaultSeparator =>
   [
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

   // Values that will report an invalid character when a custom separator is used
   public static TheoryData<String> InvalidCharacterValuesCustomSeparator =>
   [
      "A46 454 286",
      "0A6 454 286",
      "04A 454 286",
      "046 A54 286",
      "046 4A4 286",
      "046 45A 286",
      "046 454 A86",
      "046 454 2A6",
      "046 454 28A",
      "0;6 454 286",
      "0\u21536 454 286",     // Unicode fraction 1/3
      "0\u21676 454 286",     // Unicode Roman numeral VII
      "0\u0BEF6 454 286",     // Unicode Tamil number 9
   ];

   // Values that will report an invalid province
   public static TheoryData<String> InvalidProvinceValuesNoSeparator =>
   [
      "012345674",
      "876543216",
   ];

   public static TheoryData<String> InvalidProvinceValuesDefaultSeparator =>
   [
      "012-345-674",
      "876-543-216",
   ];

   public static TheoryData<String> InvalidProvinceValuesCustomSeparator =>
   [
      "012 345 674",
      "876 543 216",
   ];

   // Values that contain a transcription error that is undetectable by the Luhn algorithm
   // and will successfully create a CaSocialInsuranceNumber object
   public static TheoryData<String> UndetectableErrorValuesNoSeparator =>
   [
      "780912341",            // 789012341 with two digit transposition 90 -> 09
      "123459018",            // 123450918 with two digit transposition 09 -> 90
      "100005503",            // 100005503 with two digit twin error 55 -> 22
      "107700007",            // 104400007 with two digit twin error 44 -> 77
      "103300000",            // 106600000 with two digit twin error 66 -> 33
      "558199428",            // 558199428 with two digit jump transposition 994 -> 499
   ];

   public static TheoryData<String> UndetectableErrorValuesDefaultSeparator =>
   [
      "780-912-341",          // 789012341 with two digit transposition 90 -> 09
      "123-459-018",          // 123450918 with two digit transposition 09 -> 90
      "100-005-503",          // 100005503 with two digit twin error 55 -> 22
      "107-700-007",          // 104400007 with two digit twin error 44 -> 77
      "103-300-000",          // 106600000 with two digit twin error 66 -> 33
      "558-199-428",          // 558199428 with two digit jump transposition 994 -> 499
   ];

   public static TheoryData<String> UndetectableErrorValuesCustomSeparator =>
   [
      "780 912 341",          // 789012341 with two digit transposition 90 -> 09
      "123 459 018",          // 123450918 with two digit transposition 09 -> 90
      "100 005 503",          // 100005503 with two digit twin error 55 -> 22
      "107 700 007",          // 104400007 with two digit twin error 44 -> 77
      "103 300 000",          // 106600000 with two digit twin error 66 -> 33
      "558 199 428",          // 558199428 with two digit jump transposition 994 -> 499
   ];

   // Values that will report an invalid check digit
   public static TheoryData<String> InvalidCheckDigitValuesNoSeparator =>
   [
      "558299428",            // 558199428 with single digit transcription error 1 -> 2
      "559199428",            // 558199428 with single digit transcription error 8 -> 9
      "551899428",            // 558199428 with two digit transcription error -> 81 -> 18
      "558199248",            // 558199428 with two digit transcription error -> 42 -> 24
      "448199428",            // 558199428 with two digit twin error 55 -> 44
      "558188428",            // 558199428 with two digit twin error 99 -> 88
   ];

   public static TheoryData<String> InvalidCheckDigitValuesDefaultSeparator =>
   [
      "558-299-428",          // 558199428 with single digit transcription error 1 -> 2
      "559-199-428",          // 558199428 with single digit transcription error 8 -> 9
      "551-899-428",          // 558199428 with two digit transcription error -> 81 -> 18
      "558-199-248",          // 558199428 with two digit transcription error -> 42 -> 24
      "448-199-428",          // 558199428 with two digit twin error 55 -> 44
      "558-188-428",          // 558199428 with two digit twin error 99 -> 88
   ];

   public static TheoryData<String> InvalidCheckDigitValuesCustomSeparator =>
   [
      "558 299 428",          // 558199428 with single digit transcription error 1 -> 2
      "559 199 428",          // 558199428 with single digit transcription error 8 -> 9
      "551 899 428",          // 558199428 with two digit transcription error -> 81 -> 18
      "558 199 248",          // 558199428 with two digit transcription error -> 42 -> 24
      "448 199 428",          // 558199428 with two digit twin error 55 -> 44
      "558 188 428",          // 558199428 with two digit twin error 99 -> 88
   ];

   // Values that contain a Luhn check digit that calcuates as zero and will
   // successfully create a CaSocialInsuranceNumber object
   public static TheoryData<String> ZeroCheckDigitValuesNoSeparator =>
   [
      "123456790",
   ];

   public static TheoryData<String> ZeroCheckDigitValuesDefaultSeparator =>
   [
      "123-456-790",
   ];

   public static TheoryData<String> ZeroCheckDigitValuesCustomSeparator =>
   [
      "123 456 790",
   ];

   public static TheoryData<String> EmptySinValues =>
   [
      null!,
      String.Empty,
      "\t"
   ];

   public static TheoryData<String, Char> InvalidCustomSeparatorData
   {
      get
      {
         var sins = new[] { ValidNineCharSin, ValidElevenCharSin };
         var data = new TheoryData<String, Char>();
         foreach (var sin in sins)
         {
            foreach (var ch in Enumerable.Range('0', 10).Select(i => (Char)i))
            {
               data.Add(sin, ch);
            }
         }

         return data;
      }
   }

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValuesNoSeparator))]
   [MemberData(nameof(ValidValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Constructor_ShouldCreateObject_WhenValueContainsValidSin(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(DefaultSeparator.ToString(), String.Empty);

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
   [MemberData(nameof(InvalidLengthValuesNoSeparator))]
   [MemberData(nameof(InvalidLengthValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueHasInvalidLength(String? sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidLength + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowInvalidCaSocialInsuranceNumberException_When11CharacterValueContainsAnInvalidSeparator(String sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidSeparatorEncountered + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValuesNoSeparator))]
   [MemberData(nameof(InvalidCharacterValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueContainsNonAsciiDigit(String sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidProvinceValuesNoSeparator))]
   [MemberData(nameof(InvalidProvinceValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueHasInvalidLeadingDigit(String sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidProvince + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidProvince);

   [Theory]
   [MemberData(nameof(UndetectableErrorValuesNoSeparator))]
   [MemberData(nameof(UndetectableErrorValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Constructor_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(DefaultSeparator.ToString(), String.Empty);

      // Act.
      var sut = new CaSocialInsuranceNumber(sin);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValuesNoSeparator))]
   [MemberData(nameof(InvalidCheckDigitValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Constructor_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenCheckDigitContainsDetectableError(String sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValuesNoSeparator))]
   [MemberData(nameof(ZeroCheckDigitValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Constructor_ShouldCreateObject_WhenCheckDigitCalculatesAsZero(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(DefaultSeparator.ToString(), String.Empty);

      // Act.
      var sut = new CaSocialInsuranceNumber(sin);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Constructor (With Custom Separator) Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValuesNoSeparator))]
   [MemberData(nameof(ValidValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ConstructorWithCustomSeparator_ShouldCreateObject_WhenValueContainsValidSin(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(CustomSeparator.ToString(), String.Empty);

      // Act.
      var sut = new CaSocialInsuranceNumber(sin, CustomSeparator);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCustomSeparatorData))]
   public void CaSocialInsuranceNumber_ConstructorWithCustomSeparator_ShouldThrowArgumentOutOfRangeException_WhenCustomSeparatorIsDigit(
      String sin,
      Char customSeparator)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin, customSeparator))
         .Should()
         .ThrowExactly<ArgumentOutOfRangeException>()
         .WithMessage(Messages.CaSinInvalidCustomSeparatorCharacter + "*");

   [Theory]
   [MemberData(nameof(EmptySinValues))]
   public void CaSocialInsuranceNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueIsEmpty(String? sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinEmpty + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValuesNoSeparator))]
   [MemberData(nameof(InvalidLengthValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueHasInvalidLength(String? sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidLength + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidCaSocialInsuranceNumberException_When11CharacterValueContainsAnInvalidSeparator(String sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidSeparatorEncountered + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValuesNoSeparator))]
   [MemberData(nameof(InvalidCharacterValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueContainsNonAsciiDigit(String sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidProvinceValuesNoSeparator))]
   [MemberData(nameof(InvalidProvinceValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueHasInvalidLeadingDigit(String sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidProvince + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidProvince);

   [Theory]
   [MemberData(nameof(UndetectableErrorValuesNoSeparator))]
   [MemberData(nameof(UndetectableErrorValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ConstructorWithCustomSeparator_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(CustomSeparator.ToString(), String.Empty);

      // Act.
      var sut = new CaSocialInsuranceNumber(sin, CustomSeparator);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValuesNoSeparator))]
   [MemberData(nameof(InvalidCheckDigitValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenCheckDigitContainsDetectableError(String sin)
      => FluentActions
         .Invoking(() => _ = new CaSocialInsuranceNumber(sin, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValuesNoSeparator))]
   [MemberData(nameof(ZeroCheckDigitValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ConstructorWithCustomSeparator_ShouldCreateObject_WhenCheckDigitCalculatesAsZero(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(CustomSeparator.ToString(), String.Empty);

      // Act.
      var sut = new CaSocialInsuranceNumber(sin, CustomSeparator);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Implicit Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValuesNoSeparator))]
   [MemberData(nameof(ValidValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_ImplicitCaSinToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(DefaultSeparator.ToString(), String.Empty);
      var sut = new CaSocialInsuranceNumber(sin);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidValuesNoSeparator))]
   [MemberData(nameof(ValidValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_CastCaSinToString_ShouldReturnExpectedValue_WhenValueIsNotNull(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(DefaultSeparator.ToString(), String.Empty);
      var sut = new CaSocialInsuranceNumber(sin);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Fact]
   public void CaSocialInsuranceNumber_ImplicitCaSinToStringConversion_ShouldThrowArgumentNullException_WhenValueIsNull()
   {
      // Arrange.
      CaSocialInsuranceNumber sin = null!;
      String str;

      // Act/assert.
      FluentActions
         .Invoking(() => str = sin)
         .Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(sin))
         .WithMessage(Messages.CaSinInvalidNullConversionToString + "*");
   }

   [Fact]
   public void CaSocialInsuranceNumber_CastCaSinToString_ShouldThrowArgumentNullException_WhenValueIsNull()
   {
      // Arrange.
      CaSocialInsuranceNumber sin = null!;

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (String)sin)
         .Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(sin))
         .WithMessage(Messages.CaSinInvalidNullConversionToString + "*");
   }

   [Theory]
   [MemberData(nameof(ValidValuesNoSeparator))]
   [MemberData(nameof(ValidValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_ImplicitStringToCaSinConversion_ShouldCreateObject_WhenValueContainsValidSin(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(DefaultSeparator.ToString(), String.Empty);

      // Act.
      CaSocialInsuranceNumber sut = sin;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(EmptySinValues))]
   public void CaSocialInsuranceNumber_ImplicitStringToCaSinConversion_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueIsEmpty(String? str)
   {
      // Arrange.
      CaSocialInsuranceNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinEmpty + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.Empty);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValuesNoSeparator))]
   [MemberData(nameof(InvalidLengthValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_ImplicitStringToCaSinConversion_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueHasInvalidLength(String? str)
   {
      // Arrange.
      CaSocialInsuranceNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidLength + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidLength);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_ImplicitStringToCaSinConversion_ShouldThrowInvalidCaSocialInsuranceNumberException_When11CharacterValueContainsAnInvalidSeparator(String str)
   {
      // Arrange.
      CaSocialInsuranceNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidSeparatorEncountered + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValuesNoSeparator))]
   [MemberData(nameof(InvalidCharacterValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_ImplicitStringToCaSinConversion_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueContainsNonAsciiDigit(String str)
   {
      // Arrange.
      CaSocialInsuranceNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered);
   }

   [Theory]
   [MemberData(nameof(InvalidProvinceValuesNoSeparator))]
   [MemberData(nameof(InvalidProvinceValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_ImplicitStringToCaSinConversion_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenValueHasInvalidLeadingDigit(String str)
   {
      // Arrange.
      CaSocialInsuranceNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidProvince + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidProvince);
   }

   [Theory]
   [MemberData(nameof(UndetectableErrorValuesNoSeparator))]
   [MemberData(nameof(UndetectableErrorValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_ImplicitStringToCaSinConversion_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String str)
   {
      // Arrange.
      var expected = str.Length == 9 ? str : str.Replace(DefaultSeparator.ToString(), String.Empty);
      CaSocialInsuranceNumber sut;

      // Act.
      sut = new CaSocialInsuranceNumber(str);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValuesNoSeparator))]
   [MemberData(nameof(InvalidCheckDigitValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_ImplicitStringToCaSinConversion_ShouldThrowInvalidCaSocialInsuranceNumberException_WhenCheckDigitContainsDetectableError(String str)
   {
      // Arrange.
      CaSocialInsuranceNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidCaSocialInsuranceNumberException>()
         .WithMessage(Messages.CaSinInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCheckDigit);
   }

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValuesNoSeparator))]
   [MemberData(nameof(ZeroCheckDigitValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_ImplicitStringToCaSinConversion_ShouldCreateObject_WhenCheckDigitCalculatesAsZero(String str)
   {
      // Arrange.
      var expected = str.Length == 9 ? str : str.Replace(DefaultSeparator.ToString(), String.Empty);
      CaSocialInsuranceNumber sut;

      // Act.
      sut = new CaSocialInsuranceNumber(str);

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
   [MemberData(nameof(ValidValuesNoSeparator))]
   [MemberData(nameof(ValidValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Create_ShouldCreateObject_WhenValueContainsValidSin(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(DefaultSeparator.ToString(), String.Empty);
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
   [MemberData(nameof(InvalidLengthValuesNoSeparator))]
   [MemberData(nameof(InvalidLengthValuesDefaultSeparator))]
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
   [MemberData(nameof(InvalidSeparatorValuesDefaultSeparator))]
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
   [MemberData(nameof(InvalidCharacterValuesNoSeparator))]
   [MemberData(nameof(InvalidCharacterValuesDefaultSeparator))]
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
   [MemberData(nameof(InvalidProvinceValuesNoSeparator))]
   [MemberData(nameof(InvalidProvinceValuesDefaultSeparator))]
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
   [MemberData(nameof(UndetectableErrorValuesNoSeparator))]
   [MemberData(nameof(UndetectableErrorValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Create_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(DefaultSeparator.ToString(), String.Empty);
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
   [MemberData(nameof(InvalidCheckDigitValuesNoSeparator))]
   [MemberData(nameof(InvalidCheckDigitValuesDefaultSeparator))]
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
   [MemberData(nameof(ZeroCheckDigitValuesNoSeparator))]
   [MemberData(nameof(ZeroCheckDigitValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Create_ShouldCreateObject_WhenCheckDigitCalculatesAsZero(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(DefaultSeparator.ToString(), String.Empty);
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

   #region Create (With Custom Separator) Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValuesNoSeparator))]
   [MemberData(nameof(ValidValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_CreateWithCustomSeparator_ShouldCreateObject_WhenValueContainsValidSin(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(CustomSeparator.ToString(), String.Empty);
      var expectedValue = new CaSocialInsuranceNumber(expected);

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin, CustomSeparator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(InvalidCustomSeparatorData))]
   public void CaSocialInsuranceNumber_CreateWithCustomSeparator_ShouldThrowArgumentOutOfRangeException_WhenCustomSeparatorIsDigit(
      String sin,
      Char customSeparator)
      => FluentActions
         .Invoking(() => _ = CaSocialInsuranceNumber.Create(sin, customSeparator))
         .Should()
         .ThrowExactly<ArgumentOutOfRangeException>()
         .WithMessage(Messages.CaSinInvalidCustomSeparatorCharacter + "*");

   [Theory]
   [MemberData(nameof(EmptySinValues))]
   public void CaSocialInsuranceNumber_CreateWithCustomSeparator_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String? sin)
   {
      // Arrange.
      var expected = CaSocialInsuranceNumberValidationResult.Empty;

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin!, CustomSeparator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValuesNoSeparator))]
   [MemberData(nameof(InvalidLengthValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_CreateWithCustomSeparator_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String sin)
   {
      // Arrange.
      var expected = CaSocialInsuranceNumberValidationResult.InvalidLength;

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin, CustomSeparator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_CreateWithCustomSeparator_ShouldReturnInvalidSeparatorEncounteredResult_When11CharacterValueContainsAnInvalidSeparator(String sin)
   {
      // Arrange.
      var expected = CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered;

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin, CustomSeparator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValuesNoSeparator))]
   [MemberData(nameof(InvalidCharacterValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_CreateWithCustomSeparator_ShouldReturnInvalidCharacterEncounteredResult_WhenValueContainsNonAsciiDigit(String sin)
   {
      // Arrange.
      var expected = CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered;

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin, CustomSeparator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidProvinceValuesNoSeparator))]
   [MemberData(nameof(InvalidProvinceValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_CreateWithCustomSeparator_ShouldReturnInvalidProvinceResult_WhenValueHasInvalidLeadingDigit(String sin)
   {
      // Arrange.
      var expected = CaSocialInsuranceNumberValidationResult.InvalidProvince;

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin, CustomSeparator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(UndetectableErrorValuesNoSeparator))]
   [MemberData(nameof(UndetectableErrorValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_CreateWithCustomSeparator_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(CustomSeparator.ToString(), String.Empty);
      var expectedValue = new CaSocialInsuranceNumber(expected);

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin, CustomSeparator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValuesNoSeparator))]
   [MemberData(nameof(InvalidCheckDigitValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_CreateWithCustomSeparator_ShouldReturnInvalidCheckDigit_WhenCheckDigitContainsDetectableError(String sin)
   {
      // Arrange.
      var expected = CaSocialInsuranceNumberValidationResult.InvalidCheckDigit;

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin, CustomSeparator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValuesNoSeparator))]
   [MemberData(nameof(ZeroCheckDigitValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_CreateWithCustomSeparator_ShouldCreateObject_WhenCheckDigitCalculatesAsZero(String sin)
   {
      // Arrange.
      var expected = sin.Length == 9 ? sin : sin.Replace(CustomSeparator.ToString(), String.Empty);
      var expectedValue = new CaSocialInsuranceNumber(expected);

      // Act.
      var result = CaSocialInsuranceNumber.Create(sin, CustomSeparator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
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
      var sut = new CaSocialInsuranceNumber(ValidNineCharSin);
      var mask = "___ ___ ___";
      var expected = ValidElevenCharSinWithCustomSeparator;

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

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValuesNoSeparator))]
   [MemberData(nameof(ValidValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_ToString_ShouldReturnExpectedValue(String sin)
   {
      // Arrange.
      var sut = new CaSocialInsuranceNumber(sin);
      var expected = sin.Length == 9 ? sin : sin.Replace(DefaultSeparator.ToString(), String.Empty);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValuesNoSeparator))]
   [MemberData(nameof(ValidValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenValueContainsValidSin(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(EmptySinValues))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValuesNoSeparator))]
   [MemberData(nameof(InvalidLengthValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidSeparatorEncountered_When11CharacterValueContainsAnInvalidSeparator(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValuesNoSeparator))]
   [MemberData(nameof(InvalidCharacterValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidCharacterEncountered_WhenValueContainsNonAsciiDigit(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidProvinceValuesNoSeparator))]
   [MemberData(nameof(InvalidProvinceValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnInvalidProvince_WhenValueHasInvalidLeadingDigit(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidProvince);

   [Theory]
   [MemberData(nameof(UndetectableErrorValuesNoSeparator))]
   [MemberData(nameof(UndetectableErrorValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenCheckDigitContainsUndetectableError(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValuesNoSeparator))]
   [MemberData(nameof(InvalidCheckDigitValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenCheckDigitContainsDetectableError(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValuesNoSeparator))]
   [MemberData(nameof(ZeroCheckDigitValuesDefaultSeparator))]
   public void CaSocialInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenCheckDigitCalculatesAsZero(String sin)
      => CaSocialInsuranceNumber.Validate(sin).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   #endregion

   #region Validate (With Custom Separator) Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValuesNoSeparator))]
   [MemberData(nameof(ValidValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ValidateWithCustomSeparator_ShouldReturnValidationPassed_WhenValueContainsValidSin(String sin)
      => CaSocialInsuranceNumber.Validate(sin, CustomSeparator).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(InvalidCustomSeparatorData))]
   public void CaSocialInsuranceNumber_ValidateWithCustomSeparator_ShouldThrowArgumentOutOfRangeException_WhenCustomSeparatorIsDigit(
      String sin,
      Char customSeparator)
      => FluentActions
         .Invoking(() => CaSocialInsuranceNumber.Validate(sin, customSeparator))
         .Should()
         .ThrowExactly<ArgumentOutOfRangeException>()
         .WithMessage(Messages.CaSinInvalidCustomSeparatorCharacter + "*");

   [Theory]
   [MemberData(nameof(EmptySinValues))]
   public void CaSocialInsuranceNumber_ValidateWithCustomSeparator_ShouldReturnEmpty_WhenValueIsEmpty(String? sin)
      => CaSocialInsuranceNumber.Validate(sin, CustomSeparator).Should().Be(CaSocialInsuranceNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValuesNoSeparator))]
   [MemberData(nameof(InvalidLengthValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ValidateWithCustomSeparator_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String sin)
      => CaSocialInsuranceNumber.Validate(sin, CustomSeparator).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ValidateWithCustomSeparator_ShouldReturnInvalidSeparatorEncountered_When11CharacterValueContainsAnInvalidSeparator(String sin)
      => CaSocialInsuranceNumber.Validate(sin, CustomSeparator).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValuesNoSeparator))]
   [MemberData(nameof(InvalidCharacterValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ValidateWithCustomSeparator_ShouldReturnInvalidCharacterEncountered_WhenValueContainsNonAsciiDigit(String sin)
      => CaSocialInsuranceNumber.Validate(sin, CustomSeparator).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidProvinceValuesNoSeparator))]
   [MemberData(nameof(InvalidProvinceValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ValidateWithCustomSeparator_ShouldReturnInvalidProvince_WhenValueHasInvalidLeadingDigit(String sin)
      => CaSocialInsuranceNumber.Validate(sin, CustomSeparator).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidProvince);

   [Theory]
   [MemberData(nameof(UndetectableErrorValuesNoSeparator))]
   [MemberData(nameof(UndetectableErrorValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ValidateWithCustomSeparator_ShouldReturnValidationPassed_WhenCheckDigitContainsUndetectableError(String sin)
      => CaSocialInsuranceNumber.Validate(sin, CustomSeparator).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValuesNoSeparator))]
   [MemberData(nameof(InvalidCheckDigitValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ValidateWithCustomSeparator_ShouldReturnInvalidCheckDigit_WhenCheckDigitContainsDetectableError(String sin)
      => CaSocialInsuranceNumber.Validate(sin, CustomSeparator).Should().Be(CaSocialInsuranceNumberValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(ZeroCheckDigitValuesNoSeparator))]
   [MemberData(nameof(ZeroCheckDigitValuesCustomSeparator))]
   public void CaSocialInsuranceNumber_ValidateWithCustomSeparator_ShouldReturnValidationPassed_WhenCheckDigitCalculatesAsZero(String sin)
      => CaSocialInsuranceNumber.Validate(sin, CustomSeparator).Should().Be(CaSocialInsuranceNumberValidationResult.ValidationPassed);

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

   #endregion
}
