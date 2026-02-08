// Ignore Spelling: npi

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class UsNationalProviderIdentifierTests
{
   private const String _validNpi = "1245319599";        // Example from www.hippaspace.com
   private const String _altValidNpi = "1234567893";     // Example from Wikipedia article on Luhn algorithm

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Constructor_ShouldCreateObject_WhenValueContainsValidSin()
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
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueIsEmpty(String? npi)
      => FluentActions
         .Invoking(() => _ = new UsNationalProviderIdentifier(npi))
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiEmpty + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.Empty);

   [Theory]
   [InlineData("124531959")]
   [InlineData("12453195999")]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueHasInvalidLength(String? npi)
      => FluentActions
         .Invoking(() => _ = new UsNationalProviderIdentifier(npi))
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidLength + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidLength);

   [Theory]
   [InlineData("A245319599")]
   [InlineData("1A45319599")]
   [InlineData("12A5319599")]
   [InlineData("124A319599")]
   [InlineData("1245A19599")]
   [InlineData("12453A9599")]
   [InlineData("124531A599")]
   [InlineData("1245319A99")]
   [InlineData("12453195A9")]
   [InlineData("124531959A")]
   [InlineData("1;45319599")]
   [InlineData("1\u215345319599")]     // Unicode fraction 1/3
   [InlineData("1\u216745319599")]     // Unicode Roman numeral VII
   [InlineData("1\u0BEF45319599")]     // Unicode Tamil number 9
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueContainsNonAsciiDigit(String npi)
      => FluentActions
         .Invoking(() => _ = new UsNationalProviderIdentifier(npi))
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered);

   [Theory]
   [InlineData("1234569071")]    // Valid NPI 1234560971 with two digit transposition 09 -> 90
   [InlineData("1230967899")]    // Valid NPI 1239067899 with two digit transposition 90 -> 09
   [InlineData("1122334497")]    // Valid NPI 1122334497 with two digit twin error 22 -> 55
   [InlineData("1122337797")]    // Valid NPI 1122334497 with two digit twin error 44 -> 77
   [InlineData("1122664497")]    // Valid NPI 1122334497 with two digit twin error 33 -> 66
   public void UsNationalProviderIdentifier_Constructor_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String npi)
   {
      // Act.
      var sut = new UsNationalProviderIdentifier(npi);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(npi);
   }

   [Theory]
   [InlineData("1238560971")]    // Valid NPI 1234560971 with single digit transcription error 4 -> 8
   [InlineData("1243560971")]    // Valid NPI 1234560971 with two digit transposition error 34 -> 43
   [InlineData("4422334497")]    // Valid NPI 1122334497 with two digit twin error 11 -> 44
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
      UsNationalProviderIdentifier npi = default;
      String str;

      // Act/assert.
      FluentActions
         .Invoking(() => str = npi)
         .Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(npi))
         .WithMessage(Messages.UsNationalProviderIdentifierInvalidDefaultConversionToString + "*");
   }

   [Fact]
   public void UsNationalProviderIdentifier_CastUsNpiToString_ShouldThrowArgumentNullException_WhenValueIsDefault()
   {
      // Arrange.
      UsNationalProviderIdentifier npi = default;

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (String)npi)
         .Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(npi))
         .WithMessage(Messages.UsNationalProviderIdentifierInvalidDefaultConversionToString + "*");
   }

   [Fact]
   public void UsNationalProviderIdentifier_ImplicitStringToUsNpiConversion_ShouldCreateObject_WhenValueContainsValidSin()
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
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
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
   [InlineData("124531959")]
   [InlineData("12453195999")]
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
   [InlineData("A245319599")]
   [InlineData("1A45319599")]
   [InlineData("12A5319599")]
   [InlineData("124A319599")]
   [InlineData("1245A19599")]
   [InlineData("12453A9599")]
   [InlineData("124531A599")]
   [InlineData("1245319A99")]
   [InlineData("12453195A9")]
   [InlineData("124531959A")]
   [InlineData("1;45319599")]
   [InlineData("1\u215345319599")]     // Unicode fraction 1/3
   [InlineData("1\u216745319599")]     // Unicode Roman numeral VII
   [InlineData("1\u0BEF45319599")]     // Unicode Tamil number 9
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
   [InlineData("1234569071")]    // Valid NPI 1234560971 with two digit transposition 09 -> 90
   [InlineData("1230967899")]    // Valid NPI 1239067899 with two digit transposition 90 -> 09
   [InlineData("1122334497")]    // Valid NPI 1122334497 with two digit twin error 22 -> 55
   [InlineData("1122337797")]    // Valid NPI 1122334497 with two digit twin error 44 -> 77
   [InlineData("1122664497")]    // Valid NPI 1122334497 with two digit twin error 33 -> 66
   public void UsNationalProviderIdentifier_ImplicitStringToUsNpiConversion_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String npi)
   {
      // Act.
      UsNationalProviderIdentifier sut = npi;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(npi);
   }

   [Theory]
   [InlineData("1238560971")]    // Valid NPI 1234560971 with single digit transcription error 4 -> 8
   [InlineData("1243560971")]    // Valid NPI 1234560971 with two digit transposition error 34 -> 43
   [InlineData("4422334497")]    // Valid NPI 1122334497 with two digit twin error 11 -> 44
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

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnValidationPassed_WhenValueContainsValidNpi()
      => UsNationalProviderIdentifier.Validate(_validNpi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.ValidationPassed);

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.Empty);

   [Theory]
   [InlineData("124531959")]
   [InlineData("12453195999")]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.InvalidLength);

   [Theory]
   [InlineData("A245319599")]
   [InlineData("1A45319599")]
   [InlineData("12A5319599")]
   [InlineData("124A319599")]
   [InlineData("1245A19599")]
   [InlineData("12453A9599")]
   [InlineData("124531A599")]
   [InlineData("1245319A99")]
   [InlineData("12453195A9")]
   [InlineData("124531959A")]
   [InlineData("1;45319599")]
   [InlineData("1\u215345319599")]     // Unicode fraction 1/3
   [InlineData("1\u216745319599")]     // Unicode Roman numeral VII
   [InlineData("1\u0BEF45319599")]     // Unicode Tamil number 9
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidCharacterEncountered_WhenValueContainsNonAsciiDigit(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered);

   [Theory]
   [InlineData("1234569071")]    // Valid NPI 1234560971 with two digit transposition 09 -> 90
   [InlineData("1230967899")]    // Valid NPI 1239067899 with two digit transposition 90 -> 09
   [InlineData("1122334497")]    // Valid NPI 1122334497 with two digit twin error 22 -> 55
   [InlineData("1122337797")]    // Valid NPI 1122334497 with two digit twin error 44 -> 77
   [InlineData("1122664497")]    // Valid NPI 1122334497 with two digit twin error 33 -> 66
   public void UsNationalProviderIdentifier_Validate_ShouldReturnValidationPassed_WhenCheckDigitContainsUndetectableError(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.ValidationPassed);

   [Theory]
   [InlineData("1238560971")]    // Valid NPI 1234560971 with single digit transcription error 4 -> 8
   [InlineData("1243560971")]    // Valid NPI 1234560971 with two digit transposition error 34 -> 43
   [InlineData("4422334497")]    // Valid NPI 1122334497 with two digit twin error 11 -> 44
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidCheckDigit_WhenCheckDigitContainsDetectableError(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCheckDigit);

   #endregion
}
