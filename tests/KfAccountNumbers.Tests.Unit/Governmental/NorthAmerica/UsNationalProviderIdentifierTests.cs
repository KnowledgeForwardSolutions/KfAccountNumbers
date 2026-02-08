// Ignore Spelling: npi

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class UsNationalProviderIdentifierTests
{
   private const String _validNpi = "1245319599";        // Example from www.hippaspace.com
   private const String _altValidNpi = "1234567893";     // Example from Wikipedia article on Luhn algorithm

   public static TheoryData<String> EmptyNpiValues =>
   [
      null!,
      String.Empty,
      "\t"
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "124531959",
      "12453195999"
   ];

   public static TheoryData<String> InvalidCharacterValues =>
   [
      "A245319599",
      "1A45319599",
      "12A5319599",
      "124A319599",
      "1245A19599",
      "12453A9599",
      "124531A599",
      "1245319A99",
      "12453195A9",
      "124531959A",
      "1;45319599",
      "1\u215345319599",      // Unicode fraction 1/3
      "1\u216745319599",      // Unicode Roman numeral VII
      "1\u0BEF45319599",      // Unicode Tamil number 9
   ];

   public static TheoryData<String> CheckDigitUndetectableErrorValues =>
   [
      "1234569071",           // Valid NPI 1234560971 with two digit transposition 09 -> 90
      "1230967899",           // Valid NPI 1239067899 with two digit transposition 90 -> 09
      "1122334497",           // Valid NPI 1122334497 with two digit twin error 22 -> 55
      "1122337797",           // Valid NPI 1122334497 with two digit twin error 44 -> 77
      "1122664497",           // Valid NPI 1122334497 with two digit twin error 33 -> 66
   ];

   public static TheoryData<String> CheckDigitDetectableErrorValues =>
   [
      "1238560971",           // Valid NPI 1234560971 with single digit transcription error 4 -> 8
      "1243560971",           // Valid NPI 1234560971 with two digit transposition error 34 -> 43
      "4422334497",           // Valid NPI 1122334497 with two digit twin error 11 -> 44
   ];

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Constructor_ShouldCreateObject_WhenValueContainsValidNpi()
   {
      // Arrange.
      var npi = _validNpi;

      // Act.
      var sut = new UsNationalProviderIdentifier(npi);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(npi);
   }

   [Theory]
   [MemberData(nameof(EmptyNpiValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueIsEmpty(String? npi)
      => FluentActions
         .Invoking(() => _ = new UsNationalProviderIdentifier(npi))
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiEmpty + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueHasInvalidLength(String? npi)
      => FluentActions
         .Invoking(() => _ = new UsNationalProviderIdentifier(npi))
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidLength + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueContainsNonAsciiDigit(String npi)
      => FluentActions
         .Invoking(() => _ = new UsNationalProviderIdentifier(npi))
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String npi)
   {
      // Act.
      var sut = new UsNationalProviderIdentifier(npi);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(npi);
   }

   [Theory]
   [MemberData(nameof(CheckDigitDetectableErrorValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenCheckDigitContainsDetectableError(String npi)
      => FluentActions
         .Invoking(() => _ = new UsNationalProviderIdentifier(npi))
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCheckDigit);

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_ImplicitUsNpiToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var npi = _validNpi;
      var sut = new UsNationalProviderIdentifier(npi);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(npi);
   }

   [Fact]
   public void UsNationalProviderIdentifier_CastUsNpiToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var npi = _validNpi;
      var sut = new UsNationalProviderIdentifier(npi);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(npi);
   }

   [Fact]
   public void UsNationalProviderIdentifier_ImplicitUsNpiToStringConversion_ShouldThrowArgumentNullException_WhenValueIsDefault()
   {
      // Arrange.
      UsNationalProviderIdentifier npi = null!;
      String str;

      // Act/assert.
      FluentActions
         .Invoking(() => str = npi)
         .Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(npi))
         .WithMessage(Messages.UsNationalProviderIdentifierInvalidNullConversionToString + "*");
   }

   [Fact]
   public void UsNationalProviderIdentifier_CastUsNpiToString_ShouldThrowArgumentNullException_WhenValueIsNull()
   {
      // Arrange.
      UsNationalProviderIdentifier npi = null!;

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (String)npi)
         .Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(npi))
         .WithMessage(Messages.UsNationalProviderIdentifierInvalidNullConversionToString + "*");
   }

   [Fact]
   public void UsNationalProviderIdentifier_ImplicitStringToUsNpiConversion_ShouldCreateObject_WhenValueContainsValidNpi()
   {
      // Arrange.
      var npi = _validNpi;

      // Act.
      UsNationalProviderIdentifier sut = npi;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(npi);
   }

   [Theory]
   [MemberData(nameof(EmptyNpiValues))]
   public void UsNationalProviderIdentifier_ImplicitStringToUsNpiConversion_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueIsEmpty(String? npi)
   {
      // Arrange.
      UsNationalProviderIdentifier sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = npi)
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiEmpty + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.Empty);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsNationalProviderIdentifier_ImplicitStringToUsNpiConversion_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueHasInvalidLength(String? npi)
   {
      // Arrange.
      UsNationalProviderIdentifier sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = npi)
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidLength + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidLength);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsNationalProviderIdentifier_ImplicitStringToUsNpiConversion_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueContainsNonAsciiDigit(String npi)
   {
      // Arrange.
      UsNationalProviderIdentifier sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = npi)
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered);
   }

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void UsNationalProviderIdentifier_ImplicitStringToUsNpiConversion_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String npi)
   {
      // Act.
      UsNationalProviderIdentifier sut = npi;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(npi);
   }

   [Theory]
   [MemberData(nameof(CheckDigitDetectableErrorValues))]
   public void UsNationalProviderIdentifier_ImplicitStringToUsNpiConversion_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenCheckDigitContainsDetectableError(String npi)
   {
      // Arrange.
      UsNationalProviderIdentifier sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = npi)
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCheckDigit);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var npi1 = new UsNationalProviderIdentifier(_validNpi);
      var npi2 = new UsNationalProviderIdentifier(_validNpi);

      // Act/assert.
      (npi1 == npi2).Should().BeTrue();
   }

   [Fact]
   public void UsNationalProviderIdentifier_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var npi1 = new UsNationalProviderIdentifier(_validNpi);
      var npi2 = new UsNationalProviderIdentifier(_altValidNpi);

      // Act/assert.
      (npi1 == npi2).Should().BeFalse();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var npi1 = new UsNationalProviderIdentifier(_validNpi);
      var npi2 = new UsNationalProviderIdentifier(_altValidNpi);

      // Act/assert.
      (npi1 != npi2).Should().BeTrue();
   }

   [Fact]
   public void UsNationalProviderIdentifier_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var npi1 = new UsNationalProviderIdentifier(_validNpi);
      var npi2 = new UsNationalProviderIdentifier(_validNpi);

      // Act/assert.
      (npi1 != npi2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Create_ShouldCreateObject_WhenValueContainsValidNpi()
   {
      // Arrange.
      var npi = _validNpi;
      var expected = new UsNationalProviderIdentifier(npi);

      // Act.
      var result = UsNationalProviderIdentifier.Create(npi);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expected);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(EmptyNpiValues))]
   public void UsNationalProviderIdentifier_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String? npi)
   {
      // Arrange.
      var expected = UsNationalProviderIdentifierValidationResult.Empty;

      // Act.
      var result = UsNationalProviderIdentifier.Create(npi!);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(default(UsNationalProviderIdentifier));
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsNationalProviderIdentifier_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String npi)
   {
      // Arrange.
      var expected = UsNationalProviderIdentifierValidationResult.InvalidLength;

      // Act.
      var result = UsNationalProviderIdentifier.Create(npi);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(default(UsNationalProviderIdentifier));
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsNationalProviderIdentifier_Create_ShouldReturnInvalidCharacterEncounteredResult_WhenValueContainsNonAsciiDigit(String npi)
   {
      // Arrange.
      var expected = UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered;

      // Act.
      var result = UsNationalProviderIdentifier.Create(npi);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(default(UsNationalProviderIdentifier));
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void UsNationalProviderIdentifier_Create_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String npi)
   {
      // Arrange.
      var expected = new UsNationalProviderIdentifier(npi);

      // Act.
      var result = UsNationalProviderIdentifier.Create(npi);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expected);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(CheckDigitDetectableErrorValues))]
   public void UsNationalProviderIdentifier_Create_ShouldReturnInvalidCheckDigit_WhenCheckDigitContainsDetectableError(String npi)
   {
      // Arrange.
      var expected = UsNationalProviderIdentifierValidationResult.InvalidCheckDigit;

      // Act.
      var result = UsNationalProviderIdentifier.Create(npi);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(default(UsNationalProviderIdentifier));
      result.ValidationFailure.Should().Be(expected);
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_ToString_ShouldReturnExpectedValue()
   {
      // Arrange.
      var npi = _validNpi;
      var sut = new UsNationalProviderIdentifier(npi);

      // Act/assert.
      sut.ToString().Should().Be(npi);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnValidationPassed_WhenValueContainsValidNpi()
      => UsNationalProviderIdentifier.Validate(_validNpi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(EmptyNpiValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidCharacterEncountered_WhenValueContainsNonAsciiDigit(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnValidationPassed_WhenCheckDigitContainsUndetectableError(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(CheckDigitDetectableErrorValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidCheckDigit_WhenCheckDigitContainsDetectableError(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCheckDigit);

   #endregion
}
