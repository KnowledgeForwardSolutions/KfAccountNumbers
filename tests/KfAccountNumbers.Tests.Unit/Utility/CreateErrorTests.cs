namespace KfAccountNumbers.Tests.Unit.Utility;

public class CreateErrorTests
{
   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CreateError_Constructor_ShouldCreateObject_WhenAllValuesSupplied()
   {
      // Arrange.
      var errorType = FooErrorType.Bar;
      var description = "Bar not valid";

      // Act.
      var sut = new CreateError<FooErrorType>(errorType, description);

      // Assert.
      sut.Should().NotBeNull();
      sut.ErrorType.Should().Be(errorType);
      sut.Description.Should().Be(description);
   }

   [Fact]
   public void CreateError_Constructor_ShouldThrowArgumentOutOfRangeException_WhenErrorTypeIsDefault()
   {
      // Arrange.
      var errorType = FooErrorType.None;
      var description = "Bar not valid";
      var act = () => _ = new CreateError<FooErrorType>(errorType, description);
      var expectedError = Messages.CreateErrorDefaultErrorType + "*";

      // Act/assert.
      act.Should().ThrowExactly<ArgumentOutOfRangeException>()
         .WithParameterName(nameof(errorType))
         .WithMessage(expectedError)
         .And.ActualValue.Should().Be(errorType);
   }

   [Fact]
   public void CreateError_Constructor_ShouldThrowArgumentNullException_WhenDescriptionIsNull()
   {
      // Arrange.
      var errorType = FooErrorType.Bar;
      String description = null!;
      var act = () => _ = new CreateError<FooErrorType>(errorType, description);
      var expectedError = Messages.CreateErrorDescriptionEmpty + "*";

      // Act/assert.
      act.Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(description))
         .WithMessage(expectedError);
   }

   [Theory]
   [InlineData("")]
   [InlineData("\t")]
   public void CreateError_Constructor_ShouldThrowArgumentException_WhenDescriptionIsEmpty(String description)
   {
      // Arrange.
      var errorType = FooErrorType.Bar;
      var act = () => _ = new CreateError<FooErrorType>(errorType, description);
      var expectedError = Messages.CreateErrorDescriptionEmpty + "*";

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(description))
         .WithMessage(expectedError);
   }

   #endregion
}
