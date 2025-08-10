// Ignore Spelling: Ssn

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class UsSocialSecurityNumberTests
{
   private const String ValidNineCharSsn = "078051120";        // Actual SSN used in 1930's advertising campaign
   private const String ValidElevenCharSsn = "078-05-1120";    

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn, ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn, ValidNineCharSsn)]
   public void UsSocialSecurityNumber_Constructor_ShouldCreateObject_WhenStringValueOnlyContainsValidSsn(
      String ssn,
      String expected)
   {
      // Act.
      var sut = new UsSocialSecurityNumber(ssn);

      // Assert.
      sut.Should().NotBeNull();
      String str = sut;
      str.Should().Be(expected);
   }

   [Theory]
   [InlineData(ValidElevenCharSsn, '-', ValidNineCharSsn)]
   [InlineData("078 05 1120", ' ', ValidNineCharSsn)]
   [InlineData("078\t05\t1120", '\t', ValidNineCharSsn)]
   public void UsSocialSecurityNumber_Constructor_ShouldCreateObject_WhenStringValueAndSeparatorContainsValidSsn(
      String ssn,
      Char separator,
      String expected)
   {
      // Act.
      var sut = new UsSocialSecurityNumber(ssn, separator);

      // Assert.
      sut.Should().NotBeNull();
      String str = sut;
      str.Should().Be(expected);
   }

   [Fact]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentNullException_WhenStringValueIsNull()
   {
      // Arrange.
      String ssn = null!;
      var expectedMessage = Messages.UsSsnEmpty + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("")]
   [InlineData("\t")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueIsEmptyOrWhitespace(String ssn)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnEmpty + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("01234567")]
   [InlineData("0123456789")]
   [InlineData("012-34-56789")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueHasInvalidLength(String ssn)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnInvalidLength + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
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
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentOutOfRangeException_WhenSeparatorIsDigit(Char separator)
   {
      // Arrange.
      var ssn = $"012{separator}34{separator}5678";
      var expectedMessage = Messages.UsSsnInvalidSeparatorCharacter + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentOutOfRangeException>()
         .WithParameterName(nameof(separator))
         .WithMessage(expectedMessage)
         .And.ActualValue.Should().Be(separator);
   }

   [Theory]
   [InlineData("012 34-5678", 3)]
   [InlineData("012-34 5678", 6)]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_When11CharacterStringOnlyFindsInvalidSeparator(
      String ssn,
      Int32 offset)
   {
      // Arrange.
      var expectedMessage = String.Format(Messages.UsSsnInvalidSeparatorEncountered, offset, '-', ssn[offset]) + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("012 34.5678", '.', 3)]
   [InlineData("012.34 5678", '.', 6)]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_When11CharacterStringAndSeparatorFindsInvalidSeparator(
      String ssn,
      Char separator,
      Int32 offset)
   {
      // Arrange.
      var expectedMessage = String.Format(Messages.UsSsnInvalidSeparatorEncountered, offset, separator, ssn[offset]) + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("012345A78", 6)]
   [InlineData("012345;78", 6)]
   [InlineData("012345\u215378", 6)]      // Unicode fraction 1/3
   [InlineData("012345\u216778", 6)]      // Unicode Roman numeral VII
   [InlineData("012345\u0BEF78", 6)]      // Unicode Tamil number 9
   [InlineData("012-34-5A78", 8)]
   [InlineData("012-34-5;78", 8)]
   [InlineData("012-34-5\u215378", 8)]      // Unicode fraction 1/3
   [InlineData("012-34-5\u216778", 8)]      // Unicode Roman numeral VII
   [InlineData("012-34-5\u0BEF78", 8)]      // Unicode Tamil number 9
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueOnlyContainsNonAsciiDigit(
      String ssn,
      Int32 offset)
   {
      // Arrange.
      var expectedMessage = String.Format(Messages.UsSsnInvalidCharacterEncountered, offset, ssn[offset]) + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("012 34 5A78", ' ', 8)]
   [InlineData("012 34 5;78", ' ', 8)]
   [InlineData("012 34 5\u215378", ' ', 8)]      // Unicode fraction 1/3
   [InlineData("012 34 5\u216778", ' ', 8)]      // Unicode Roman numeral VII
   [InlineData("012 34 5\u0BEF78", ' ', 8)]      // Unicode Tamil number 9
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueAndSeparatorContainsNonAsciiDigit(
      String ssn,
      Char separator,
      Int32 offset)
   {
      // Arrange.
      var expectedMessage = String.Format(Messages.UsSsnInvalidCharacterEncountered, offset, ssn[offset]) + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
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
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueOnlyHasInvalidAreaNumber(String ssn)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnInvalidAreaNumber + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("000 12 3456", ' ')]
   [InlineData("666 12 3456", ' ')]
   [InlineData("900 12 3456", ' ')]
   [InlineData("999 12 3456", ' ')]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueAndSeparatorHasInvalidAreaNumber(
      String ssn,
      Char separator)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnInvalidAreaNumber + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("012005678")]
   [InlineData("012-00-5678")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueOnlyHasInvalidGroupNumber(String ssn)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnInvalidGroupNumber + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Fact]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueAndSeparatorHasInvalidGroupNumber()
   {
      // Arrange.
      var ssn = "012 00 5678";
      var separator = ' ';
      var expectedMessage = Messages.UsSsnInvalidGroupNumber + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("012340000")]
   [InlineData("012-34-0000")]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueOnlyHasInvalidSerialNumber(String ssn)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnInvalidSerialNumber + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Fact]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueAndSeparatorHasInvalidSerialNumber()
   {
      // Arrange.
      var ssn = "012 34 0000";
      var separator = ' ';
      var expectedMessage = Messages.UsSsnInvalidSerialNumber + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
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
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueOnlyHas9IdenticalDigits(String ssn)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnAllIdenticalDigits + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("111 11 1111", ' ')]    // Note that missing cases ("000 00 0000", "666 66 6666" and "999 99 9999"
   [InlineData("222 22 2222", ' ')]    // will fail the validation for area number before reaching the
   [InlineData("333 33 3333", ' ')]    // validation for identical digits
   [InlineData("444 44 4444", ' ')]
   [InlineData("555 55 5555", ' ')]
   [InlineData("777 77 7777", ' ')]
   [InlineData("888 88 8888", ' ')]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueAndSeparatorHas9IdenticalDigits(
      String ssn,
      Char separator)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnAllIdenticalDigits + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Fact]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueOnlyHasConsecutiveRun()
   {
      // Arrange.
      var ssn = "123456789";
      var expectedMessage = Messages.UsSsnInvalidRun + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Fact]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenStringValueAndSeparatorHasConsecutiveRun()
   {
      // Arrange.
      var ssn = "123 45 6789";
      var separator = ' ';
      var expectedMessage = Messages.UsSsnInvalidRun + "*";
      var act = () => _ = new UsSocialSecurityNumber(ssn, separator);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   # endregion

   #region implicit String operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsSocialSecurityNumber_ImplicitStringOperator_ShouldReturn9DigitSsn()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidElevenCharSsn);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(ValidNineCharSsn);
   }

   # endregion

   #region implicit UsSocialSecurityNumber operator Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn, ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn, ValidNineCharSsn)]
   public void UsSocialSecurityNumber_ImplicitUsSocialSecurityNumberOperator_ShouldCreateObject_WhenStringValueContainsValidSsn(
      String ssn,
      String expected)
   {
      // Act.
      UsSocialSecurityNumber sut = ssn;

      // Assert.
      sut.Should().NotBeNull();
      String str = sut;
      str.Should().Be(expected);
   }

   [Fact]
   public void UsSocialSecurityNumber_ImplicitUsSocialSecurityNumberOperator_ShouldThrowArgumentNullException_WhenStringValueIsNull()
   {
      // Arrange.
      String ssn = null!;
      var expectedMessage = Messages.UsSsnEmpty + "*";
      var act = () => _ = (UsSocialSecurityNumber)ssn;

      // Act/assert.
      act.Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("")]
   [InlineData("\t")]
   public void UsSocialSecurityNumber_ImplicitUsSocialSecurityNumberOperator_ShouldThrowArgumentException_WhenStringValueIsEmptyOrWhitespace(String ssn)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnEmpty + "*";
      var act = () => _ = (UsSocialSecurityNumber)ssn;

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("01234567")]
   [InlineData("0123456789")]
   [InlineData("012-34-56789")]
   public void UsSocialSecurityNumber_ImplicitUsSocialSecurityNumberOperator_ShouldThrowArgumentException_WhenStringValueHasInvalidLength(String ssn)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnInvalidLength + "*";
      var act = () => _ = (UsSocialSecurityNumber)ssn;

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("012 34-5678", 3)]
   [InlineData("012-34 5678", 6)]
   public void UsSocialSecurityNumber_ImplicitUsSocialSecurityNumberOperator_ShouldThrowArgumentException_When11CharacterStringOnlyFindsInvalidSeparator(
      String ssn,
      Int32 offset)
   {
      // Arrange.
      var expectedMessage = String.Format(Messages.UsSsnInvalidSeparatorEncountered, offset, '-', ssn[offset]) + "*";
      var act = () => _ = (UsSocialSecurityNumber)ssn;

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("012345A78", 6)]
   [InlineData("012345;78", 6)]
   [InlineData("012345\u215378", 6)]      // Unicode fraction 1/3
   [InlineData("012345\u216778", 6)]      // Unicode Roman numeral VII
   [InlineData("012345\u0BEF78", 6)]      // Unicode Tamil number 9
   [InlineData("012-34-5A78", 8)]
   [InlineData("012-34-5;78", 8)]
   [InlineData("012-34-5\u215378", 8)]      // Unicode fraction 1/3
   [InlineData("012-34-5\u216778", 8)]      // Unicode Roman numeral VII
   [InlineData("012-34-5\u0BEF78", 8)]      // Unicode Tamil number 9
   public void UsSocialSecurityNumber_ImplicitUsSocialSecurityNumberOperator_ShouldThrowArgumentException_WhenStringValueContainsNonAsciiDigit(
      String ssn,
      Int32 offset)
   {
      // Arrange.
      var expectedMessage = String.Format(Messages.UsSsnInvalidCharacterEncountered, offset, ssn[offset]) + "*";
      var act = () => _ = (UsSocialSecurityNumber)ssn;

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
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
   public void UsSocialSecurityNumber_ImplicitUsSocialSecurityNumberOperator_ShouldThrowArgumentException_WhenStringValueHasInvalidAreaNumber(String ssn)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnInvalidAreaNumber + "*";
      var act = () => _ = (UsSocialSecurityNumber)ssn;

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("012005678")]
   [InlineData("012-00-5678")]
   public void UsSocialSecurityNumber_ImplicitUsSocialSecurityNumberOperator_ShouldThrowArgumentException_WhenStringValueOnlyHasInvalidGroupNumber(String ssn)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnInvalidGroupNumber + "*";
      var act = () => _ = (UsSocialSecurityNumber)ssn;

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("012340000")]
   [InlineData("012-34-0000")]
   public void UsSocialSecurityNumber_ImplicitUsSocialSecurityNumberOperator_ShouldThrowArgumentException_WhenStringValueOnlyHasInvalidSerialNumber(String ssn)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnInvalidSerialNumber + "*";
      var act = () => _ = (UsSocialSecurityNumber)ssn;

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
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
   public void UsSocialSecurityNumber_ImplicitUsSocialSecurityNumberOperator_ShouldThrowArgumentException_WhenStringValueOnlyHas9IdenticalDigits(String ssn)
   {
      // Arrange.
      var expectedMessage = Messages.UsSsnAllIdenticalDigits + "*";
      var act = () => _ = (UsSocialSecurityNumber)ssn;

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   [Fact]
   public void UsSocialSecurityNumber_ImplicitUsSocialSecurityNumberOperator_ShouldThrowArgumentException_WhenStringValueOnlyHasConsecutiveRun()
   {
      // Arrange.
      var ssn = "123456789";
      var expectedMessage = Messages.UsSsnInvalidRun + "*";
      var act = () => _ = (UsSocialSecurityNumber)ssn;

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(ssn))
         .WithMessage(expectedMessage);
   }

   # endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsSocialSecurityNumber_ToString_ShouldReturn9DigitSsn()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidElevenCharSsn);

      // Act.
      var str = sut.ToString();

      // Assert.
      str.Should().Be(ValidNineCharSsn);
   }

   # endregion
}
