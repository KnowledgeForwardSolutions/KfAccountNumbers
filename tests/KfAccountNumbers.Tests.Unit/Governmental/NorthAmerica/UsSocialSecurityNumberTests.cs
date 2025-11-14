// Ignore Spelling: ssn

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class UsSocialSecurityNumberTests
{
   private const String ValidNineCharSsn = "078051120";        // Actual SSN used in a 1930's advertising campaign
   private const String ValidElevenCharSsn = "078-05-1120";
   private const String ValidElevenCharSsnWithCustomSeparator = "078 05 1120";

   private const Char CustomSeparator = ' ';

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn, ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn, ValidNineCharSsn)]
   public void UsSocialSecurityNumber_Constructor_ShouldCreateObject_WhenValueContainsValidSsn(
      String ssn,
      String expected)
   {
      // Act.
      var sut = new UsSocialSecurityNumber(ssn);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueIsEmpty(String? ssn)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.Empty;
      var expectedMessage = Messages.UsSsnEmpty + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("01234567")]
   [InlineData("0123456789")]
   [InlineData("012-34-56789")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidLength(String ssn)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidLength;
      var expectedMessage = Messages.UsSsnInvalidLength + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("012 34-5678")]
   [InlineData("012-34 5678")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_When11CharacterValueContainsInvalidSeparator(String ssn)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered;
      var expectedMessage = Messages.UsSsnInvalidSeparatorEncountered + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("A12345678")]
   [InlineData("0A2345678")]
   [InlineData("01A345678")]
   [InlineData("012A45678")]
   [InlineData("0123A5678")]
   [InlineData("01234A678")]
   [InlineData("012345A78")]
   [InlineData("0123456A8")]
   [InlineData("01234567A")]
   [InlineData("0;2345678")]
   [InlineData("0\u21532345678")]      // Unicode fraction 1/3
   [InlineData("0\u21672345678")]      // Unicode Roman numeral VII
   [InlineData("0\u0BEF2345678")]      // Unicode Tamil number 9
   [InlineData("A12-34-5678")]
   [InlineData("0A2-34-5678")]
   [InlineData("01A-34-5678")]
   [InlineData("012-A4-5678")]
   [InlineData("012-3A-5678")]
   [InlineData("012-34-A678")]
   [InlineData("012-34-5A78")]
   [InlineData("012-34-56A8")]
   [InlineData("012-34-567A")]
   [InlineData("0;2-34-5678")]
   [InlineData("0\u21532-34-5678")]    // Unicode fraction 1/3
   [InlineData("0\u21672-34-5678")]    // Unicode Roman numeral VII
   [InlineData("0\u0BEF2-34-5678")]    // Unicode Tamil number 9
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueContainsNonAsciiDigit(String ssn)
   {
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered;
      // Arrange.
      var expectedMessage = Messages.UsSsnInvalidCharacterEncountered + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("000123456")]
   [InlineData("666123456")]
   [InlineData("900123456")]
   [InlineData("999123456")]
   [InlineData("000-12-3456")]
   [InlineData("666-12-3456")]
   [InlineData("900-12-3456")]
   [InlineData("999-12-3456")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidAreaNumber(String ssn)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidAreaNumber;
      var expectedMessage = Messages.UsSsnInvalidAreaNumber + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("012005678")]
   [InlineData("012-00-5678")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidGroupNumber(String ssn)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidGroupNumber;
      var expectedMessage = Messages.UsSsnInvalidGroupNumber + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("012340000")]
   [InlineData("012-34-0000")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidSerialNumber(String ssn)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidSerialNumber;
      var expectedMessage = Messages.UsSsnInvalidSerialNumber + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("111111111")]        // Note that missing cases ("000000000", "666666666" and "999999999"
   [InlineData("222222222")]        // will fail the validation for area number before reaching the
   [InlineData("333333333")]        // validation for identical digits
   [InlineData("444444444")]
   [InlineData("555555555")]
   [InlineData("777777777")]
   [InlineData("888888888")]
   [InlineData("111-11-1111")]
   [InlineData("222-22-2222")]
   [InlineData("333-33-3333")]
   [InlineData("444-44-4444")]
   [InlineData("555-55-5555")]
   [InlineData("777-77-7777")]
   [InlineData("888-88-8888")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHas9IdenticalDigits(String ssn)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.AllIdenticalDigits;
      var expectedMessage = Messages.UsSsnAllIdenticalDigits + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("123456789")]
   [InlineData("123-45-6789")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasConsecutiveRun(String ssn)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidRun;
      var expectedMessage = Messages.UsSsnInvalidRun + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   #endregion

   #region Constructor (With Custom Separator) Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn, ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsnWithCustomSeparator, ValidNineCharSsn)]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldCreateObject_WhenValueContainsValidSsn(
      String ssn,
      String expected)
   {
      // Arrange.
      var separator = CustomSeparator;

      // Act.
      var sut = new UsSocialSecurityNumber(ssn, separator);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [InlineData('0')]
   [InlineData('1')]
   [InlineData('2')]
   [InlineData('3')]
   [InlineData('4')]
   [InlineData('5')]
   [InlineData('6')]
   [InlineData('7')]
   [InlineData('8')]
   [InlineData('9')]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowArgumentOutOfRangeException_WhenCustomSeparatorIsDigit(Char separator)
   {
      // Arrange.
      var ssn = $"012{separator}34{separator}5678";
      var expectedMessage = Messages.UsSsnInvalidCustomSeparatorCharacter + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentOutOfRangeException>()
         .WithParameterName(nameof(separator))
         .WithMessage(expectedMessage)
         .And.ActualValue.Should().Be(separator);
   }

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueIsEmpty(String? ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.Empty;
      var expectedMessage = Messages.UsSsnEmpty + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn!, separator);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("01234567")]
   [InlineData("0123456789")]
   [InlineData("012 34 56789")]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidLength(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidLength;
      var expectedMessage = Messages.UsSsnInvalidLength + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("012.34 5678")]
   [InlineData("012 34.5678")]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidUsSocialSecurityNumberException_When11CharacterValueContainsInvalidSeparator(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered;
      var expectedMessage = Messages.UsSsnInvalidSeparatorEncountered + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("A12345678")]
   [InlineData("0A2345678")]
   [InlineData("01A345678")]
   [InlineData("012A45678")]
   [InlineData("0123A5678")]
   [InlineData("01234A678")]
   [InlineData("012345A78")]
   [InlineData("0123456A8")]
   [InlineData("01234567A")]
   [InlineData("0;2345678")]
   [InlineData("0\u21532345678")]      // Unicode fraction 1/3
   [InlineData("0\u21672345678")]      // Unicode Roman numeral VII
   [InlineData("0\u0BEF2345678")]      // Unicode Tamil number 9
   [InlineData("A12 34 5678")]
   [InlineData("0A2 34 5678")]
   [InlineData("01A 34 5678")]
   [InlineData("012 A4 5678")]
   [InlineData("012 3A 5678")]
   [InlineData("012 34 A678")]
   [InlineData("012 34 5A78")]
   [InlineData("012 34 56A8")]
   [InlineData("012 34 567A")]
   [InlineData("0;2 34 5678")]
   [InlineData("0\u21532 34 5678")]    // Unicode fraction 1/3
   [InlineData("0\u21672 34 5678")]    // Unicode Roman numeral VII
   [InlineData("0\u0BEF2 34 5678")]    // Unicode Tamil number 9
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueContainsNonAsciiDigit(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered;
      var expectedMessage = Messages.UsSsnInvalidCharacterEncountered + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("000123456")]
   [InlineData("666123456")]
   [InlineData("900123456")]
   [InlineData("999123456")]
   [InlineData("000 12 3456")]
   [InlineData("666 12 3456")]
   [InlineData("900 12 3456")]
   [InlineData("999 12 3456")]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidAreaNumber(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidAreaNumber;
      var expectedMessage = Messages.UsSsnInvalidAreaNumber + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("012005678")]
   [InlineData("012 00 5678")]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidGroupNumber(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidGroupNumber;
      var expectedMessage = Messages.UsSsnInvalidGroupNumber + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("012340000")]
   [InlineData("012 34 0000")]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidSerialNumber(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidSerialNumber;
      var expectedMessage = Messages.UsSsnInvalidSerialNumber + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("111111111")]        // Note that missing cases ("000000000", "666666666" and "999999999"
   [InlineData("222222222")]        // will fail the validation for area number before reaching the
   [InlineData("333333333")]        // validation for identical digits
   [InlineData("444444444")]
   [InlineData("555555555")]
   [InlineData("777777777")]
   [InlineData("888888888")]
   [InlineData("111 11 1111")]
   [InlineData("222 22 2222")]
   [InlineData("333 33 3333")]
   [InlineData("444 44 4444")]
   [InlineData("555 55 5555")]
   [InlineData("777 77 7777")]
   [InlineData("888 88 8888")]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHas9IdenticalDigits(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.AllIdenticalDigits;
      var expectedMessage = Messages.UsSsnAllIdenticalDigits + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("123456789")]
   [InlineData("123 45 6789")]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasConsecutiveRun(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidRun;
      var expectedMessage = Messages.UsSsnInvalidRun + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   #endregion

   #region Implicit Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn, ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn, ValidNineCharSsn)]
   public void UsSocialSecurityNumber_ImplicitUsSsnToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull(
      String ssn,
      String expected)
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ssn);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Theory]
   [InlineData(ValidNineCharSsn, ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn, ValidNineCharSsn)]
   public void UsSocialSecurityNumber_CastUsSsnToString_ShouldReturnExpectedValue_WhenValueIsNotNull(
      String ssn,
      String expected)
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ssn);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Fact]
   public void UsSocialSecurityNumber_ImplicitUsSsnToStringConversion_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueIsEmpty()
   {
      // Arrange.
      UsSocialSecurityNumber ssn = null!;
      String str;
      var expectedMessage = Messages.UsSsnInvalidNullConversionToString + "*";
      var act = () => str = ssn;


      // Act/assert.
      act.Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Fact]
   public void UsSocialSecurityNumber_CastUsSsnToString_ShouldThrowArgumentNullException_WhenValueIsNull()
   {
      // Arrange.
      UsSocialSecurityNumber ssn = null!;
      var expectedMessage = Messages.UsSsnInvalidNullConversionToString + "*";
      var act = () => _ = (String)ssn;


      // Act/assert.
      act.Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData(ValidNineCharSsn, ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn, ValidNineCharSsn)]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldCreateObject_WhenValueContainsValidSsn(
      String str,
      String expected)
   {
      // Act.
      UsSocialSecurityNumber sut = str;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueIsEmpty(String? str)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.Empty;
      var expectedMessage = Messages.UsSsnEmpty + "*";
      UsSocialSecurityNumber sut;
      var act = () => sut = str;

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("01234567")]
   [InlineData("0123456789")]
   [InlineData("012-34-56789")]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidLength(String str)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidLength;
      var expectedMessage = Messages.UsSsnInvalidLength + "*";
      UsSocialSecurityNumber sut;
      var act = () => sut = str;

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("012 34-5678")]
   [InlineData("012-34 5678")]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_When11CharacterValueContainsInvalidSeparator(String str)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered;
      var expectedMessage = Messages.UsSsnInvalidSeparatorEncountered + "*";
      UsSocialSecurityNumber sut;
      var act = () => sut = str;

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("A12345678")]
   [InlineData("0A2345678")]
   [InlineData("01A345678")]
   [InlineData("012A45678")]
   [InlineData("0123A5678")]
   [InlineData("01234A678")]
   [InlineData("012345A78")]
   [InlineData("0123456A8")]
   [InlineData("01234567A")]
   [InlineData("0;2345678")]
   [InlineData("0\u21532345678")]      // Unicode fraction 1/3
   [InlineData("0\u21672345678")]      // Unicode Roman numeral VII
   [InlineData("0\u0BEF2345678")]      // Unicode Tamil number 9
   [InlineData("A12-34-5678")]
   [InlineData("0A2-34-5678")]
   [InlineData("01A-34-5678")]
   [InlineData("012-A4-5678")]
   [InlineData("012-3A-5678")]
   [InlineData("012-34-A678")]
   [InlineData("012-34-5A78")]
   [InlineData("012-34-56A8")]
   [InlineData("012-34-567A")]
   [InlineData("0;2-34-5678")]
   [InlineData("0\u21532-34-5678")]    // Unicode fraction 1/3
   [InlineData("0\u21672-34-5678")]    // Unicode Roman numeral VII
   [InlineData("0\u0BEF2-34-5678")]    // Unicode Tamil number 9
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueContainsNonAsciiDigit(String str)
   {
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered;
      // Arrange.
      var expectedMessage = Messages.UsSsnInvalidCharacterEncountered + "*";
      UsSocialSecurityNumber sut;
      var act = () => sut = str;

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("000123456")]
   [InlineData("666123456")]
   [InlineData("900123456")]
   [InlineData("999123456")]
   [InlineData("000-12-3456")]
   [InlineData("666-12-3456")]
   [InlineData("900-12-3456")]
   [InlineData("999-12-3456")]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidAreaNumber(String str)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidAreaNumber;
      var expectedMessage = Messages.UsSsnInvalidAreaNumber + "*";
      UsSocialSecurityNumber sut;
      var act = () => sut = str;

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("012005678")]
   [InlineData("012-00-5678")]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidGroupNumber(String str)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidGroupNumber;
      var expectedMessage = Messages.UsSsnInvalidGroupNumber + "*";
      UsSocialSecurityNumber sut;
      var act = () => sut = str;

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("012340000")]
   [InlineData("012-34-0000")]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidSerialNumber(String str)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidSerialNumber;
      var expectedMessage = Messages.UsSsnInvalidSerialNumber + "*";
      UsSocialSecurityNumber sut;
      var act = () => sut = str;

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("111111111")]        // Note that missing cases ("000000000", "666666666" and "999999999"
   [InlineData("222222222")]        // will fail the validation for area number before reaching the
   [InlineData("333333333")]        // validation for identical digits
   [InlineData("444444444")]
   [InlineData("555555555")]
   [InlineData("777777777")]
   [InlineData("888888888")]
   [InlineData("111-11-1111")]
   [InlineData("222-22-2222")]
   [InlineData("333-33-3333")]
   [InlineData("444-44-4444")]
   [InlineData("555-55-5555")]
   [InlineData("777-77-7777")]
   [InlineData("888-88-8888")]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHas9IdenticalDigits(String str)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.AllIdenticalDigits;
      var expectedMessage = Messages.UsSsnAllIdenticalDigits + "*";
      UsSocialSecurityNumber sut;
      var act = () => sut = str;

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   [Theory]
   [InlineData("123456789")]
   [InlineData("123-45-6789")]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasConsecutiveRun(String str)
   {
      // Arrange.
      var expectedValidationResult = UsSocialSecurityNumberValidationResult.InvalidRun;
      var expectedMessage = Messages.UsSsnInvalidRun + "*";
      UsSocialSecurityNumber sut;
      var act = () => sut = str;

      // Act/assert.
      act.Should().ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(expectedMessage)
         .And.ValidationResult.Should().Be(expectedValidationResult);
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn, ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn, ValidNineCharSsn)]
   public void UsSocialSecurityNumber_Create_ShouldCreateObject_WhenValueContainsValidSsn(
      String ssn,
      String expected)
   {
      // Arrange.
      var expectedValue = new UsSocialSecurityNumber(expected);

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void UsSocialSecurityNumber_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String? ssn)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.Empty;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn!);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("01234567")]
   [InlineData("0123456789")]
   [InlineData("012-34-56789")]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String ssn)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidLength;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("012 34-5678")]
   [InlineData("012-34 5678")]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidSeparatorCharacterValidationResult_When11CharacterValueContainsInvalidSeparator(String ssn)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("A12345678")]
   [InlineData("0A2345678")]
   [InlineData("01A345678")]
   [InlineData("012A45678")]
   [InlineData("0123A5678")]
   [InlineData("01234A678")]
   [InlineData("012345A78")]
   [InlineData("0123456A8")]
   [InlineData("01234567A")]
   [InlineData("0;2345678")]
   [InlineData("0\u21532345678")]      // Unicode fraction 1/3
   [InlineData("0\u21672345678")]      // Unicode Roman numeral VII
   [InlineData("0\u0BEF2345678")]      // Unicode Tamil number 9
   [InlineData("A12-34-5678")]
   [InlineData("0A2-34-5678")]
   [InlineData("01A-34-5678")]
   [InlineData("012-A4-5678")]
   [InlineData("012-3A-5678")]
   [InlineData("012-34-A678")]
   [InlineData("012-34-5A78")]
   [InlineData("012-34-56A8")]
   [InlineData("012-34-567A")]
   [InlineData("0;2-34-5678")]
   [InlineData("0\u21532-34-5678")]    // Unicode fraction 1/3
   [InlineData("0\u21672-34-5678")]    // Unicode Roman numeral VII
   [InlineData("0\u0BEF2-34-5678")]    // Unicode Tamil number 9
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueContainsNonAsciiDigit(String ssn)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("000123456")]
   [InlineData("666123456")]
   [InlineData("900123456")]
   [InlineData("999123456")]
   [InlineData("000-12-3456")]
   [InlineData("666-12-3456")]
   [InlineData("900-12-3456")]
   [InlineData("999-12-3456")]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidAreaNumberValidationResult_WhenValueHasInvalidAreaNumber(String ssn)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidAreaNumber;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("012005678")]
   [InlineData("012-00-5678")]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidGroupNumberValidationResult_WhenValueHasInvalidGroupNumber(String ssn)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidGroupNumber;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("012340000")]
   [InlineData("012-34-0000")]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidSerialNumberValidationResult_WhenValueHasInvalidSerialNumber(String ssn)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidSerialNumber;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("111111111")]        // Note that missing cases ("000000000", "666666666" and "999999999"
   [InlineData("222222222")]        // will fail the validation for area number before reaching the
   [InlineData("333333333")]        // validation for identical digits
   [InlineData("444444444")]
   [InlineData("555555555")]
   [InlineData("777777777")]
   [InlineData("888888888")]
   [InlineData("111-11-1111")]
   [InlineData("222-22-2222")]
   [InlineData("333-33-3333")]
   [InlineData("444-44-4444")]
   [InlineData("555-55-5555")]
   [InlineData("777-77-7777")]
   [InlineData("888-88-8888")]
   public void UsSocialSecurityNumber_Create_ShouldReturnAllIdenticalDigitsValidationResult_WhenValueHas9IdenticalDigits(String ssn)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.AllIdenticalDigits;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("123456789")]
   [InlineData("123-45-6789")]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidRunValidationResult_WhenValueHasConsecutiveRun(String ssn)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidRun;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   #endregion

   #region Create (With Custom Separator) Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn, ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsnWithCustomSeparator, ValidNineCharSsn)]
   public void UsSocialSecurityNumber_CreateWithCustomSeparator_ShouldCreateObject_WhenValueContainsValidSsn(
      String ssn,
      String expected)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expectedValue = new UsSocialSecurityNumber(expected);

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, separator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().Be(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [InlineData('0')]
   [InlineData('1')]
   [InlineData('2')]
   [InlineData('3')]
   [InlineData('4')]
   [InlineData('5')]
   [InlineData('6')]
   [InlineData('7')]
   [InlineData('8')]
   [InlineData('9')]
   public void UsSocialSecurityNumber_CreateWithCustomSeparator__ShouldThrowArgumentOutOfRangeException_WhenCustomSeparatorIsDigit(Char separator)
   {
      // Arrange.
      var ssn = $"012{separator}34{separator}5678";
      var expectedMessage = Messages.UsSsnInvalidCustomSeparatorCharacter + "*";
      var act = () => _ = UsSocialSecurityNumber.Create(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentOutOfRangeException>()
         .WithParameterName(nameof(separator))
         .WithMessage(expectedMessage)
         .And.ActualValue.Should().Be(separator);
   }

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void UsSocialSecurityNumber_CreateWithCustomSeparator_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String? ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expected = UsSocialSecurityNumberValidationResult.Empty;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn!, separator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("01234567")]
   [InlineData("0123456789")]
   [InlineData("012 34 56789")]
   public void UsSocialSecurityNumber_CreateWithCustomSeparator_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expected = UsSocialSecurityNumberValidationResult.InvalidLength;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, separator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("012.34 5678")]
   [InlineData("012 34.5678")]
   public void UsSocialSecurityNumber_CreateWithCustomSeparator_ShouldReturnInvalidSeparatorCharacterValidationResult_When11CharacterValueContainsInvalidSeparator(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expected = UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, separator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("A12345678")]
   [InlineData("0A2345678")]
   [InlineData("01A345678")]
   [InlineData("012A45678")]
   [InlineData("0123A5678")]
   [InlineData("01234A678")]
   [InlineData("012345A78")]
   [InlineData("0123456A8")]
   [InlineData("01234567A")]
   [InlineData("0;2345678")]
   [InlineData("0\u21532345678")]       // Unicode fraction 1/3
   [InlineData("0\u21672345678")]       // Unicode Roman numeral VII
   [InlineData("0\u0BEF2345678")]       // Unicode Tamil number 9
   [InlineData("A12 34 5678")]
   [InlineData("0A2 34 5678")]
   [InlineData("01A 34 5678")]
   [InlineData("012 A4 5678")]
   [InlineData("012 3A 5678")]
   [InlineData("012 34 A678")]
   [InlineData("012 34 5A78")]
   [InlineData("012 34 56A8")]
   [InlineData("012 34 567A")]
   [InlineData("0;2 34 5678")]
   [InlineData("0\u21532 34 5678")]     // Unicode fraction 1/3
   [InlineData("0\u21672 34 5678")]     // Unicode Roman numeral VII
   [InlineData("0\u0BEF2 34 5678")]     // Unicode Tamil number 9
   public void UsSocialSecurityNumber_CreateWithCustomSeparator_ShouldReturnInvalidCharacterValidationResult_WhenValueContainsNonAsciiDigit(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expected = UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, separator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("000123456")]
   [InlineData("666123456")]
   [InlineData("900123456")]
   [InlineData("999123456")]
   [InlineData("000 12 3456")]
   [InlineData("666 12 3456")]
   [InlineData("900 12 3456")]
   [InlineData("999 12 3456")]
   public void UsSocialSecurityNumber_CreateWithCustomSeparator_ShouldReturnInvalidAreaNumberValidationResult_WhenValueHasInvalidAreaNumber(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expected = UsSocialSecurityNumberValidationResult.InvalidAreaNumber;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, separator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("012005678")]
   [InlineData("012 00 5678")]
   public void UsSocialSecurityNumber_CreateWithCustomSeparator_ShouldReturnInvalidGroupNumberValidationResult_WhenValueHasInvalidGroupNumber(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expected = UsSocialSecurityNumberValidationResult.InvalidGroupNumber;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, separator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("012340000")]
   [InlineData("012 34 0000")]
   public void UsSocialSecurityNumber_CreateWithCustomSeparator_ShouldReturnInvalidSerialNumberValidationResult_WhenValueHasInvalidSerialNumber(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expected = UsSocialSecurityNumberValidationResult.InvalidSerialNumber;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, separator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("111111111")]        // Note that missing cases ("000000000", "666666666" and "999999999"
   [InlineData("222222222")]        // will fail the validation for area number before reaching the
   [InlineData("333333333")]        // validation for identical digits
   [InlineData("444444444")]
   [InlineData("555555555")]
   [InlineData("777777777")]
   [InlineData("888888888")]
   [InlineData("111 11 1111")]
   [InlineData("222 22 2222")]
   [InlineData("333 33 3333")]
   [InlineData("444 44 4444")]
   [InlineData("555 55 5555")]
   [InlineData("777 77 7777")]
   [InlineData("888 88 8888")]
   public void UsSocialSecurityNumber_CreateWithCustomSeparator_ShouldReturnAllIdenticalDigitsValidationResult_WhenValueHas9IdenticalDigits(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expected = UsSocialSecurityNumberValidationResult.AllIdenticalDigits;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, separator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [InlineData("123456789")]
   [InlineData("123 45 6789")]
   public void UsSocialSecurityNumber_CreateWithCustomSeparator_ShouldReturnInvalidRunValidationResult_WhenValueHasConsecutiveRun(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;
      var expected = UsSocialSecurityNumberValidationResult.InvalidRun;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, separator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn)]
   public void UsSocialSecurityNumber_Validate_ShouldReturnValidationPassed_WhenValueContainsValidSsn(String ssn)
      => UsSocialSecurityNumber.Validate(ssn).Should().Be(UsSocialSecurityNumberValidationResult.ValidationPassed);

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void UsSocialSecurityNumber_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? ssn)
      => UsSocialSecurityNumber.Validate(ssn).Should().Be(UsSocialSecurityNumberValidationResult.Empty);

   [Theory]
   [InlineData("01234567")]
   [InlineData("0123456789")]
   [InlineData("012-34-56789")]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String ssn)
      => UsSocialSecurityNumber.Validate(ssn).Should().Be(UsSocialSecurityNumberValidationResult.InvalidLength);

   [Theory]
   [InlineData("012 34-5678")]
   [InlineData("012-34 5678")]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidSeparatorEncountered_When11CharacterValueContainsAnInvalidSeparator(String ssn)
      => UsSocialSecurityNumber.Validate(ssn).Should().Be(UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [InlineData("A12345678")]
   [InlineData("0A2345678")]
   [InlineData("01A345678")]
   [InlineData("012A45678")]
   [InlineData("0123A5678")]
   [InlineData("01234A678")]
   [InlineData("012345A78")]
   [InlineData("0123456A8")]
   [InlineData("01234567A")]
   [InlineData("0;2345678")]
   [InlineData("0\u21532345678")]      // Unicode fraction 1/3
   [InlineData("0\u21672345678")]      // Unicode Roman numeral VII
   [InlineData("0\u0BEF2345678")]      // Unicode Tamil number 9
   [InlineData("A12-34-5678")]
   [InlineData("0A2-34-5678")]
   [InlineData("01A-34-5678")]
   [InlineData("012-A4-5678")]
   [InlineData("012-3A-5678")]
   [InlineData("012-34-A678")]
   [InlineData("012-34-5A78")]
   [InlineData("012-34-56A8")]
   [InlineData("012-34-567A")]
   [InlineData("0;2-34-5678")]
   [InlineData("0\u21532-34-5678")]    // Unicode fraction 1/3
   [InlineData("0\u21672-34-5678")]    // Unicode Roman numeral VII
   [InlineData("0\u0BEF2-34-5678")]    // Unicode Tamil number 9
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidCharacterEncountered_WhenValueContainsNonAsciiDigit(String ssn)
      => UsSocialSecurityNumber.Validate(ssn).Should().Be(UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [InlineData("000123456")]
   [InlineData("666123456")]
   [InlineData("900123456")]
   [InlineData("999123456")]
   [InlineData("000-12-3456")]
   [InlineData("666-12-3456")]
   [InlineData("900-12-3456")]
   [InlineData("999-12-3456")]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidAreaNumber_WhenValueHasInvalidAreaNumber(String ssn)
      => UsSocialSecurityNumber.Validate(ssn).Should().Be(UsSocialSecurityNumberValidationResult.InvalidAreaNumber);

   [Theory]
   [InlineData("012005678")]
   [InlineData("012-00-5678")]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidGroupNumber_WhenValueHasInvalidGroupNumber(String ssn)
      => UsSocialSecurityNumber.Validate(ssn).Should().Be(UsSocialSecurityNumberValidationResult.InvalidGroupNumber);

   [Theory]
   [InlineData("012340000")]
   [InlineData("012-34-0000")]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidSerialNumber_WhenValueHasInvalidSerialNumber(String ssn)
      => UsSocialSecurityNumber.Validate(ssn).Should().Be(UsSocialSecurityNumberValidationResult.InvalidSerialNumber);

   [Theory]
   [InlineData("111111111")]        // Note that missing cases ("000000000", "666666666" and "999999999"
   [InlineData("222222222")]        // will fail the validation for area number before reaching the
   [InlineData("333333333")]        // validation for identical digits
   [InlineData("444444444")]
   [InlineData("555555555")]
   [InlineData("777777777")]
   [InlineData("888888888")]
   [InlineData("111-11-1111")]
   [InlineData("222-22-2222")]
   [InlineData("333-33-3333")]
   [InlineData("444-44-4444")]
   [InlineData("555-55-5555")]
   [InlineData("777-77-7777")]
   [InlineData("888-88-8888")]
   public void UsSocialSecurityNumber_Validate_ShouldReturnAllIdenticalDigits_WhenValueHas9IdenticalDigits(String ssn)
      => UsSocialSecurityNumber.Validate(ssn).Should().Be(UsSocialSecurityNumberValidationResult.AllIdenticalDigits);

   [Theory]
   [InlineData("123456789")]
   [InlineData("123-45-6789")]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidRun_WhenValueHasConsecutiveRun(String ssn)
      => UsSocialSecurityNumber.Validate(ssn).Should().Be(UsSocialSecurityNumberValidationResult.InvalidRun);

   #endregion

   #region Validate (With Custom Separator) Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData('0')]
   [InlineData('1')]
   [InlineData('2')]
   [InlineData('3')]
   [InlineData('4')]
   [InlineData('5')]
   [InlineData('6')]
   [InlineData('7')]
   [InlineData('8')]
   [InlineData('9')]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldThrowArgumentOutOfRangeException_WhenCustomSeparatorIsDigit(Char separator)
   {
      // Arrange.
      var ssn = $"012{separator}34{separator}5678";
      var expectedMessage = Messages.UsSsnInvalidCustomSeparatorCharacter + "*";
      var act = () => _ = UsSocialSecurityNumber.Validate(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentOutOfRangeException>()
         .WithParameterName(nameof(separator))
         .WithMessage(expectedMessage)
         .And.ActualValue.Should().Be(separator);
   }

   [Theory]
   [InlineData(ValidNineCharSsn, ' ')]
   [InlineData(ValidElevenCharSsn, '-')]
   [InlineData(ValidElevenCharSsnWithCustomSeparator, ' ')]
   [InlineData("078\t05\t1120", '\t')]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnValidationPassed_WhenValueContainsValidSsn(
      String ssn,
      Char separator)
      => UsSocialSecurityNumber.Validate(ssn, separator).Should().Be(UsSocialSecurityNumberValidationResult.ValidationPassed);

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnEmpty_WhenValueIsEmpty(String? ssn)
   {
      // Arrange.
      var separator = CustomSeparator;

      // Act.
      var result = UsSocialSecurityNumber.Validate(ssn, separator);

      // Assert.
      result.Should().Be(UsSocialSecurityNumberValidationResult.Empty);
   }

   [Theory]
   [InlineData("01234567")]
   [InlineData("0123456789")]
   [InlineData("012 34 56789")]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;

      // Act.
      var result = UsSocialSecurityNumber.Validate(ssn, separator);

      // Assert.
      result.Should().Be(UsSocialSecurityNumberValidationResult.InvalidLength);
   }

   [Theory]
   [InlineData("012.34 5678")]
   [InlineData("012 34.5678")]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnInvalidSeparatorEncountered_When11CharacterValueContainsAnInvalidSeparator(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;

      // Act/assert.
      UsSocialSecurityNumber.Validate(ssn, separator).Should().Be(UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered);
   }

   [Theory]
   [InlineData("A12345678")]
   [InlineData("0A2345678")]
   [InlineData("01A345678")]
   [InlineData("012A45678")]
   [InlineData("0123A5678")]
   [InlineData("01234A678")]
   [InlineData("012345A78")]
   [InlineData("0123456A8")]
   [InlineData("01234567A")]
   [InlineData("0;2345678")]
   [InlineData("0\u21532345678")]      // Unicode fraction 1/3
   [InlineData("0\u21672345678")]      // Unicode Roman numeral VII
   [InlineData("0\u0BEF2345678")]      // Unicode Tamil number 9
   [InlineData("A12 34 5678")]
   [InlineData("0A2 34 5678")]
   [InlineData("01A 34 5678")]
   [InlineData("012 A4 5678")]
   [InlineData("012 3A 5678")]
   [InlineData("012 34 A678")]
   [InlineData("012 34 5A78")]
   [InlineData("012 34 56A8")]
   [InlineData("012 34 567A")]
   [InlineData("0;2 34 5678")]
   [InlineData("0\u21532 34 5678")]    // Unicode fraction 1/3
   [InlineData("0\u21672 34 5678")]    // Unicode Roman numeral VII
   [InlineData("0\u0BEF2 34 5678")]    // Unicode Tamil number 9
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnInvalidCharacterEncountered_WhenValueContainsNonAsciiDigit(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;

      // Act/assert.
      UsSocialSecurityNumber.Validate(ssn, separator).Should().Be(UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered);
   }

   [Theory]
   [InlineData("000123456")]
   [InlineData("666123456")]
   [InlineData("900123456")]
   [InlineData("999123456")]
   [InlineData("000 12 3456")]
   [InlineData("666 12 3456")]
   [InlineData("900 12 3456")]
   [InlineData("999 12 3456")]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnInvalidAreaNumber_WhenValueHasInvalidAreaNumber(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;

      // Act/assert.
      UsSocialSecurityNumber.Validate(ssn, separator).Should().Be(UsSocialSecurityNumberValidationResult.InvalidAreaNumber);
   }

   [Theory]
   [InlineData("012005678")]
   [InlineData("012 00 5678")]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnInvalidGroupNumber_WhenValueHasInvalidGroupNumber(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;

      // Act/assert.
      UsSocialSecurityNumber.Validate(ssn, separator).Should().Be(UsSocialSecurityNumberValidationResult.InvalidGroupNumber);
   }

   [Theory]
   [InlineData("012340000")]
   [InlineData("012 34 0000")]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnInvalidSerialNumber_WhenValueHasInvalidSerialNumber(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;

      // Act/assert.
      UsSocialSecurityNumber.Validate(ssn, separator).Should().Be(UsSocialSecurityNumberValidationResult.InvalidSerialNumber);
   }

   [Theory]
   [InlineData("111111111")]        // Note that missing cases ("000000000", "666666666" and "999999999"
   [InlineData("222222222")]        // will fail the validation for area number before reaching the
   [InlineData("333333333")]        // validation for identical digits
   [InlineData("444444444")]
   [InlineData("555555555")]
   [InlineData("777777777")]
   [InlineData("888888888")]
   [InlineData("111 11 1111")]
   [InlineData("222 22 2222")]
   [InlineData("333 33 3333")]
   [InlineData("444 44 4444")]
   [InlineData("555 55 5555")]
   [InlineData("777 77 7777")]
   [InlineData("888 88 8888")]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnAllIdenticalDigits_WhenValueHas9IdenticalDigits(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;

      // Act/assert.
      UsSocialSecurityNumber.Validate(ssn, separator).Should().Be(UsSocialSecurityNumberValidationResult.AllIdenticalDigits);
   }

   [Theory]
   [InlineData("123456789")]
   [InlineData("123 45 6789")]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnInvalidRun_WhenValueHasConsecutiveRun(String ssn)
   {
      // Arrange.
      var separator = CustomSeparator;

      // Act/assert.
      UsSocialSecurityNumber.Validate(ssn, separator).Should().Be(UsSocialSecurityNumberValidationResult.InvalidRun);
   }

   #endregion
}
