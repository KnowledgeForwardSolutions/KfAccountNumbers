// Ignore Spelling: ssn

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class UsSocialSecurityNumberTests
{
   private const String ValidNineCharSsn = "078051120";        // Actual SSN used in a 1930's advertising campaign
   private const String ValidElevenCharSsn = "078-05-1120";
   private const String ValidElevenCharSsnWithCustomSeparator = "078 05 1120";

   private const Char CustomSeparator = ' ';
   private const Char DefaultSeparator = '-';

   public static TheoryData<String, Char> InvalidCustomSeparatorData
   {
      get
      {
         var sins = new[] { ValidNineCharSsn, ValidElevenCharSsn };
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
   [InlineData(ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn)]
   public void UsSocialSecurityNumber_Constructor_ShouldCreateObject_WhenValueContainsValidSsn(String ssn)
   {
      // Arrange.
      var expected = ssn.Length == 9 ? ssn : ssn.Replace(DefaultSeparator.ToString(), String.Empty);

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
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnEmpty + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.Empty);

   [Theory]
   [InlineData("01234567")]
   [InlineData("0123456789")]
   [InlineData("012-34-56789")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidLength(String ssn)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidLength + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidLength);

   [Theory]
   [InlineData("012 34-5678")]
   [InlineData("012-34 5678")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_When11CharacterValueContainsInvalidSeparator(String ssn)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidSeparatorEncountered + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered);

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
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered);

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
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidAreaNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidAreaNumber);

   [Theory]
   [InlineData("012005678")]
   [InlineData("012-00-5678")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidGroupNumber(String ssn)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidGroupNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidGroupNumber);

   [Theory]
   [InlineData("012340000")]
   [InlineData("012-34-0000")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidSerialNumber(String ssn)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidSerialNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidSerialNumber);

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
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnAllIdenticalDigits + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.AllIdenticalDigits);

   [Theory]
   [InlineData("123456789")]
   [InlineData("123-45-6789")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasConsecutiveRun(String ssn)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidRun + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidRun);

   #endregion

   #region Constructor (With Custom Separator) Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsnWithCustomSeparator)]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldCreateObject_WhenValueContainsValidSsn(String ssn)
   {
      // Arrange.
      var expected = ssn.Length == 9 ? ssn : ssn.Replace(CustomSeparator.ToString(), String.Empty);

      // Act.
      var sut = new UsSocialSecurityNumber(ssn, CustomSeparator);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCustomSeparatorData))]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowArgumentOutOfRangeException_WhenCustomSeparatorIsDigit(
      String ssn,
      Char customSeparator)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn, customSeparator))
         .Should()
         .ThrowExactly<ArgumentOutOfRangeException>()
         .WithMessage(Messages.UsSsnInvalidCustomSeparatorCharacter + "*");

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueIsEmpty(String? ssn)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnEmpty + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.Empty);

   [Theory]
   [InlineData("01234567")]
   [InlineData("0123456789")]
   [InlineData("012 34 56789")]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidLength(String ssn)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidLength + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidLength);

   [Theory]
   [InlineData("012.34 5678")]
   [InlineData("012 34.5678")]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidUsSocialSecurityNumberException_When11CharacterValueContainsInvalidSeparator(String ssn)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidSeparatorEncountered + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered);

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
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered);

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
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidAreaNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidAreaNumber);

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
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidSerialNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidSerialNumber);

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
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnAllIdenticalDigits + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.AllIdenticalDigits);

   [Theory]
   [InlineData("123456789")]
   [InlineData("123 45 6789")]
   public void UsSocialSecurityNumber_ConstructorWithCustomSeparator_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasConsecutiveRun(String ssn)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn, CustomSeparator))
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidRun + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidRun);

   #endregion

   #region Implicit Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn)]
   public void UsSocialSecurityNumber_ImplicitUsSsnToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull(String ssn)
   {
      // Arrange.
      var expected = ssn.Length == 9 ? ssn : ssn.Replace(DefaultSeparator.ToString(), String.Empty);
      var sut = new UsSocialSecurityNumber(ssn);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Theory]
   [InlineData(ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn)]
   public void UsSocialSecurityNumber_CastUsSsnToString_ShouldReturnExpectedValue_WhenValueIsNotNull(String ssn)
   {
      // Arrange.
      var expected = ssn.Length == 9 ? ssn : ssn.Replace(DefaultSeparator.ToString(), String.Empty);
      var sut = new UsSocialSecurityNumber(ssn);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Fact]
   public void UsSocialSecurityNumber_ImplicitUsSsnToStringConversion_ShouldThrowArgumentNullException_WhenValueIsNull()
   {
      // Arrange.
      UsSocialSecurityNumber ssn = null!;
      String str;

      // Act/assert.
      FluentActions
         .Invoking(() => str = ssn)
         .Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(Messages.UsSsnInvalidNullConversionToString + "*");
   }

   [Fact]
   public void UsSocialSecurityNumber_CastUsSsnToString_ShouldThrowArgumentNullException_WhenValueIsNull()
   {
      // Arrange.
      UsSocialSecurityNumber ssn = null!;

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (String)ssn)
         .Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(Messages.UsSsnInvalidNullConversionToString + "*");
   }

   [Theory]
   [InlineData(ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn)]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldCreateObject_WhenValueContainsValidSsn(String ssn)
   {
      // Arrange.
      var expected = ssn.Length == 9 ? ssn : ssn.Replace(DefaultSeparator.ToString(), String.Empty);

      // Act.
      UsSocialSecurityNumber sut = ssn;

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
      UsSocialSecurityNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnEmpty + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.Empty);
   }

   [Theory]
   [InlineData("01234567")]
   [InlineData("0123456789")]
   [InlineData("012-34-56789")]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidLength(String str)
   {
      // Arrange.
      UsSocialSecurityNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidLength + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidLength);
   }

   [Theory]
   [InlineData("012 34-5678")]
   [InlineData("012-34 5678")]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_When11CharacterValueContainsInvalidSeparator(String str)
   {
      // Arrange.
      UsSocialSecurityNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidSeparatorEncountered + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered);
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
      // Arrange.
      UsSocialSecurityNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered);
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
      UsSocialSecurityNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidAreaNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidAreaNumber);
   }

   [Theory]
   [InlineData("012005678")]
   [InlineData("012-00-5678")]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidGroupNumber(String str)
   {
      // Arrange.
      UsSocialSecurityNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidGroupNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidGroupNumber);
   }

   [Theory]
   [InlineData("012340000")]
   [InlineData("012-34-0000")]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasInvalidSerialNumber(String str)
   {
      // Arrange.
      UsSocialSecurityNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidSerialNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidSerialNumber);
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
      UsSocialSecurityNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnAllIdenticalDigits + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.AllIdenticalDigits);
   }

   [Theory]
   [InlineData("123456789")]
   [InlineData("123-45-6789")]
   public void UsSocialSecurityNumber_ImplicitStringToUsSsnConversion_ShouldThrowInvalidUsSocialSecurityNumberException_WhenValueHasConsecutiveRun(String str)
   {
      // Arrange.
      UsSocialSecurityNumber sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = str)
         .Should()
         .ThrowExactly<InvalidUsSocialSecurityNumberException>()
         .WithMessage(Messages.UsSsnInvalidRun + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidRun);
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn)]
   public void UsSocialSecurityNumber_Create_ShouldCreateObject_WhenValueContainsValidSsn(String ssn)
   {
      // Arrange.
      var expected = ssn.Length == 9 ? ssn : ssn.Replace(DefaultSeparator.ToString(), String.Empty);
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
   [InlineData(ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsnWithCustomSeparator)]
   public void UsSocialSecurityNumber_CreateWithCustomSeparator_ShouldCreateObject_WhenValueContainsValidSsn(String ssn)
   {
      // Arrange.
      var expected = ssn.Length == 9 ? ssn : ssn.Replace(CustomSeparator.ToString(), String.Empty);
      var expectedValue = new UsSocialSecurityNumber(expected);

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, CustomSeparator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().Be(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(InvalidCustomSeparatorData))]
   public void UsSocialSecurityNumber_CreateWithCustomSeparator_ShouldThrowArgumentOutOfRangeException_WhenCustomSeparatorIsDigit(
      String ssn,
      Char customSeparator)
      => FluentActions
         .Invoking(() => _ = UsSocialSecurityNumber.Create(ssn, customSeparator))
         .Should()
         .ThrowExactly<ArgumentOutOfRangeException>()
         .WithMessage(Messages.UsSsnInvalidCustomSeparatorCharacter + "*");

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void UsSocialSecurityNumber_CreateWithCustomSeparator_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String? ssn)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.Empty;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn!, CustomSeparator);

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
      var expected = UsSocialSecurityNumberValidationResult.InvalidLength;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, CustomSeparator);

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
      var expected = UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, CustomSeparator);

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
      var expected = UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, CustomSeparator);

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
      var expected = UsSocialSecurityNumberValidationResult.InvalidAreaNumber;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, CustomSeparator);

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
      var expected = UsSocialSecurityNumberValidationResult.InvalidGroupNumber;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, CustomSeparator);

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
      var expected = UsSocialSecurityNumberValidationResult.InvalidSerialNumber;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, CustomSeparator);

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
      var expected = UsSocialSecurityNumberValidationResult.AllIdenticalDigits;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, CustomSeparator);

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
      var expected = UsSocialSecurityNumberValidationResult.InvalidRun;

      // Act.
      var result = UsSocialSecurityNumber.Create(ssn, CustomSeparator);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsSocialSecurityNumber_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidNineCharSsn);
      var expected = ValidElevenCharSsn;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void UsSocialSecurityNumber_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidNineCharSsn);
      var mask = "___ __ ____";
      var expected = ValidElevenCharSsnWithCustomSeparator;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void UsSocialSecurityNumber_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidNineCharSsn);
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
   public void UsSocialSecurityNumber_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidNineCharSsn);
      var expectedMessage = Messages.FormatMaskEmpty + "*";
      var act = () => _ = sut.Format(mask);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = sut.Format(mask))
         .Should()
         .ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(mask))
         .WithMessage(Messages.FormatMaskEmpty + "*");
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn, ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn, ValidNineCharSsn)]
   public void UsSocialSecurityNumber_ToString_ShouldReturnExpectedValue(
      String ssn,
      String expected)
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      sut.ToString().Should().Be(expected);
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
   [MemberData(nameof(InvalidCustomSeparatorData))]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldThrowArgumentOutOfRangeException_WhenCustomSeparatorIsDigit(
      String ssn,
      Char customSeparator)
      => FluentActions
         .Invoking(() => _ = UsSocialSecurityNumber.Validate(ssn, customSeparator))
         .Should()
         .ThrowExactly<ArgumentOutOfRangeException>()
         .WithMessage(Messages.UsSsnInvalidCustomSeparatorCharacter + "*");

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
      => UsSocialSecurityNumber.Validate(ssn, CustomSeparator).Should().Be(UsSocialSecurityNumberValidationResult.Empty);

   [Theory]
   [InlineData("01234567")]
   [InlineData("0123456789")]
   [InlineData("012 34 56789")]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String ssn)
      => UsSocialSecurityNumber.Validate(ssn, CustomSeparator).Should().Be(UsSocialSecurityNumberValidationResult.InvalidLength);

   [Theory]
   [InlineData("012.34 5678")]
   [InlineData("012 34.5678")]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnInvalidSeparatorEncountered_When11CharacterValueContainsAnInvalidSeparator(String ssn)
      => UsSocialSecurityNumber.Validate(ssn, CustomSeparator).Should().Be(UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered);

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
      => UsSocialSecurityNumber.Validate(ssn, CustomSeparator).Should().Be(UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered);

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
      => UsSocialSecurityNumber.Validate(ssn, CustomSeparator).Should().Be(UsSocialSecurityNumberValidationResult.InvalidAreaNumber);

   [Theory]
   [InlineData("012005678")]
   [InlineData("012 00 5678")]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnInvalidGroupNumber_WhenValueHasInvalidGroupNumber(String ssn)
      => UsSocialSecurityNumber.Validate(ssn, CustomSeparator).Should().Be(UsSocialSecurityNumberValidationResult.InvalidGroupNumber);

   [Theory]
   [InlineData("012340000")]
   [InlineData("012 34 0000")]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnInvalidSerialNumber_WhenValueHasInvalidSerialNumber(String ssn)
      => UsSocialSecurityNumber.Validate(ssn, CustomSeparator).Should().Be(UsSocialSecurityNumberValidationResult.InvalidSerialNumber);

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
      => UsSocialSecurityNumber.Validate(ssn, CustomSeparator).Should().Be(UsSocialSecurityNumberValidationResult.AllIdenticalDigits);

   [Theory]
   [InlineData("123456789")]
   [InlineData("123 45 6789")]
   public void UsSocialSecurityNumber_ValidateWithCustomSeparator_ShouldReturnInvalidRun_WhenValueHasConsecutiveRun(String ssn)
      => UsSocialSecurityNumber.Validate(ssn, CustomSeparator).Should().Be(UsSocialSecurityNumberValidationResult.InvalidRun);

   #endregion
}
