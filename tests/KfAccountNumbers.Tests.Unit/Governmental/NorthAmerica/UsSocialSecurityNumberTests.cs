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
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentNullException_WhenSsnIsNull()
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
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenSsnIsEmptyOrWhitespace(String ssn)
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
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_WhenSsnHasInvalidLength(String ssn)
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
   [InlineData("012 34-5678", 3)]
   [InlineData("012-34 5678", 6)]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowArgumentException_When11CharacterStringOnlyFindsInvalidSeparator(
      String ssn,
      Int32 offset)
   {
      // Arrange.
      var expectedMessage = String.Format(Messages.UsSsnInvalidSeparator, offset, '-', ssn[offset]) + "*";
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
      var expectedMessage = String.Format(Messages.UsSsnInvalidSeparator, offset, separator, ssn[offset]) + "*";
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
      var expectedMessage = String.Format(Messages.UsSsnInvalidCharacter, offset, ssn[offset]) + "*";
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
      var expectedMessage = String.Format(Messages.UsSsnInvalidCharacter, offset, ssn[offset]) + "*";
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

   # endregion
}
