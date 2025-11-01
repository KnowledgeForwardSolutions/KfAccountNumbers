namespace KfAccountNumbers.Tests.Unit.Utility;

public class CreateResultTests
{
   #region Implicit Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CreateResult_SuccessImplicitOperator_ShouldReturnExpectedResult_WhenValueIsNotNull()
   {
      // Arrange.
      var foo = new Foo("A good foo...");

      // Act.
      CreateResult<Foo, FooErrorType> sut = foo;

      // Assert.
      sut.Should().NotBeNull();
      sut.ValidationFailure.Should().Be(default);
      sut.IsSuccess.Should().BeTrue();
      sut.Value.Should().Be(foo);
   }

   [Fact]
   public void CreateResult_SuccessImplicitOperator_ShouldThrowArgumentNullException_WhenValueIsNull()
   {
      // Arrange.
      Foo value = null!;
      var act = () => _ = (CreateResult<Foo, FooErrorType>)value;
      var expectedError = Messages.CreateResultValueNull + "*";

      // Act/assert.
      act.Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(value))
         .WithMessage(expectedError);
   }

   [Fact]
   public void CreateResult_FailureImplicitOperator_ShouldReturnExpectedResult_WhenErrorIsNotNull()
   {
      // Arrange.
      var error = FooErrorType.Bar;

      // Act.
      CreateResult<Foo, FooErrorType> sut = error;

      // Assert.
      sut.Should().NotBeNull();
      sut.ValidationFailure.Should().Be(error);
      sut.IsSuccess.Should().BeFalse();
      sut.Value.Should().BeNull();
   }

   [Fact]
   public void CreateResult_FailureImplicitOperator_ShouldThrowArgumentNullException_WhenErrorIsNull()
   {
      // Arrange.
      FooErrorType error = default;
      var act = () => _ = (CreateResult<Foo, FooErrorType>)error;
      var expectedError = Messages.CreateResultErrorNull + "*";

      // Act/assert.
      act.Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(error))
         .WithMessage(expectedError);
   }

   #endregion
}
