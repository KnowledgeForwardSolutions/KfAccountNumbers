namespace KfAccountNumbers.Tests.Unit.Utility;

public class ExtensionMethodsTests
{
   #region FormatWithMask Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("012345678", "___-__-____", "012-34-5678")]
   [InlineData("8005551212", "(___) ___-____", "(800) 555-1212")]
   [InlineData("abcdef", @"___ \_ ___", "abc _ def")]
   [InlineData("abcdef", @"___\\___", @"abc\def")]
   [InlineData("abc", "__.__.__", "ab.c.")]
   [InlineData("", "__.__.__", "..")]
   [InlineData("   ", "_._._", " . . ")]
   public void ExtensionMethods_FormatWithMask_ShouldReturnExpectedValue(
      String str,
      String mask,
      String expected)
      => str.FormatWithMask(mask).Should().Be(expected); 

   [Fact]
   public void ExtensionMethods_FormatWithMask_ShouldThrowArgumentNullException_WhenStrIsNull()
   {
      // Arrange.
      String str = null!;
      var mask = "___-__-____";
      var expectedMessage = Messages.FormatStrNull + "*";
      var act = () => _ = str.FormatWithMask(mask);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(str))
         .WithMessage(expectedMessage);
   }

   [Fact]
   public void ExtensionMethods_FormatWithMask_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var str = "012345678";
      String mask = null!;
      var expectedMessage = Messages.FormatMaskEmpty + "*";
      var act = () => _ = str.FormatWithMask(mask);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(mask))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("")]
   [InlineData("\t")]
   public void ExtensionMethods_FormatWithMask_ShouldThrowArgumentNullException_WhenMaskIsEmtpy(String mask)
   {
      // Arrange.
      var str = "012345678";
      var expectedMessage = Messages.FormatMaskEmpty + "*";
      var act = () => _ = str.FormatWithMask(mask);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(mask))
         .WithMessage(expectedMessage);
   }

   # endregion

   #region IsAsciiDigit Method Tests
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
   public void ExtensionMethods_IsDigit_ShouldReturnTrue_WhenCharacterIsADecimalDigit(Char ch)
      => ch.IsAsciiDigit().Should().BeTrue();

   [Theory]
   [InlineData('A')]
   [InlineData(';')]
   [InlineData('\0')]
   [InlineData('\t')]
   [InlineData('\u2153')]     // Unicode fraction 1/3
   [InlineData('\u2167')]     // Unicode Roman numeral VII
   [InlineData('\u217B')]     // Unicode Roman numeral xii
   [InlineData('\u0BEF')]     // Unicode Tamil number 9
   [InlineData('\u00B2')]     // Unicode superscript 2
   [InlineData('\u2083')]     // Unicode subscript 3
   public void ExtensionMethods_IsDigit_ShouldReturnFalse_WhenCharacterIsNotADecimalDigit(Char ch)
      => ch.IsAsciiDigit().Should().BeFalse();

   #endregion

   #region RequiresNotNullOrWhiteSpace Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void ExtensionMethods_RequiresNotNullOrWhiteSpace_ShouldReturnStringValue_WhenStringIsNotEmpty()
   {
      // Arrange.
      var str = "This is a test";
      var message = "String must not be empty";

      // Act/assert
      str.RequiresNotNullOrWhiteSpace(message).Should().BeSameAs(str);
   }

   [Fact]
   public void ExtensionMethods_RequiresNotNullOrWhiteSpace_ShouldThrowArgumentNullException_WhenStringIsNull()
   {
      // Arrange.
      String str = null!;
      var message = "String must not be empty";
      var expectedMessage = message + "*";
      var act = () => str.RequiresNotNullOrWhiteSpace(message);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(str))
         .WithMessage(expectedMessage);
   }

   [Theory]
   [InlineData("")]
   [InlineData("\t")]
   public void ExtensionMethods_RequiresNotNullOrWhiteSpace_ShouldThrowArgumentException_WhenStringIsEmptyOrWhiteSpace(String str)
   {
      // Arrange.
      var message = "String must not be empty";
      var expectedMessage = message + "*";
      var act = () => str.RequiresNotNullOrWhiteSpace(message);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(str))
         .WithMessage(expectedMessage);
   }

   #endregion
}
