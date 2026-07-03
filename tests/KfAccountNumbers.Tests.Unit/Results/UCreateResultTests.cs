namespace KfAccountNumbers.Tests.Unit.Results;

public class UCreateResultTests
{
   public readonly record struct IntBetweenOneAndTen(Int32 Value);

   public readonly record struct TooLow(Int32 Value);

   public readonly record struct TooHigh(Int32 Value);

   public union NumberValidationError(TooLow, TooHigh);

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CreateResult_Constructor_ShouldCreateInstance_WhenValidationSucceeds()
   {
      // Arrange.
      var number = 9;
      var success = new IntBetweenOneAndTen(number);

      // Act.
      var sut = new CreateResult<IntBetweenOneAndTen, NumberValidationError>(success);

      // Assert.
      sut.IsSuccess.Should().BeTrue();
      sut.Value.Should().Be(success);
   }

   [Fact]
   public void CreateResult_Constructor_ShouldCreateInstance_WhenValidationFails()
   {
      // Arrange.
      var number = -1;
      var validationError = new NumberValidationError(new TooLow(number));

      // Act.
      var sut = new CreateResult<IntBetweenOneAndTen, NumberValidationError>(validationError);

      // Assert.
      sut.IsSuccess.Should().BeFalse();
      sut.Value.Should().Be(validationError);
   }

   #endregion

   #region HasValue Property Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CreateResult_HasValue_ShouldReturnTrue_WhenValidationSucceeds()
   {
      // Arrange.
      var number = 9;
      var success = new IntBetweenOneAndTen(number);
      var sut = new CreateResult<IntBetweenOneAndTen, NumberValidationError>(success);

      // Act/assert.
      sut.HasValue.Should().BeTrue();
   }

   [Fact]
   public void CreateResult_HasValue_ShouldReturnTrue_WhenValidationFails()
   {
      // Arrange.
      var number = -1;
      var validationError = new NumberValidationError(new TooLow(number));
      var sut = new CreateResult<IntBetweenOneAndTen, NumberValidationError>(validationError);

      // Act/assert.
      sut.HasValue.Should().BeTrue();
   }

   #endregion

   #region IsSuccess Property Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CreateResult_IsSuccess_ShouldReturnTrue_WhenValidationSucceeds()
   {
      // Arrange.
      var number = 9;
      var success = new IntBetweenOneAndTen(number);
      var sut = new CreateResult<IntBetweenOneAndTen, NumberValidationError>(success);

      // Act/assert.
      sut.IsSuccess.Should().BeTrue();
   }

   [Fact]
   public void CreateResult_IsSuccess_ShouldReturnFalse_WhenValidationFails()
   {
      // Arrange.
      var number = -1;
      var validationError = new NumberValidationError(new TooLow(number));
      var sut = new CreateResult<IntBetweenOneAndTen, NumberValidationError>(validationError);

      // Act/assert.
      sut.IsSuccess.Should().BeFalse();
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CreateResult_Value_ShouldReturnExpectedResult_WhenValidationSucceeds()
   {
      // Arrange.
      var number = 9;
      var success = new IntBetweenOneAndTen(number);
      var sut = new CreateResult<IntBetweenOneAndTen, NumberValidationError>(success);

      // Act/assert.
      sut.Value.Should().Be(success);
   }

   [Fact]
   public void CreateResult_Value_ShouldReturnExpectedResult_WhenValidationFails()
   {
      // Arrange.
      var number = -1;
      var validationError = new NumberValidationError(new TooLow(number));
      var sut = new CreateResult<IntBetweenOneAndTen, NumberValidationError>(validationError);

      // Act/assert.
      sut.Value.Should().Be(validationError);
   }

   #endregion

   #region Implicit Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CreateResult_ImplicitSuccessConversion_ShouldReturnExpectedResult()
   {
      // Arrange.
      var number = 9;
      var success = new IntBetweenOneAndTen(number);

      // Act.
      CreateResult<IntBetweenOneAndTen, NumberValidationError> sut = success;

      // Assert.
      sut.IsSuccess.Should().BeTrue();
      sut.Value.Should().Be(success);
   }

   [Fact]
   public void CreateResult_ImplicitFailureConversion_ShouldReturnExpectedResult()
   {
      // Arrange.
      var number = -1;
      var validationError = new NumberValidationError(new TooLow(number));

      // Act.
      CreateResult<IntBetweenOneAndTen, NumberValidationError> sut = validationError;

      // Assert.
      sut.IsSuccess.Should().BeFalse();
      sut.Value.Should().Be(validationError);
   }

   #endregion

   #region TryGetValue Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CreateResult_TryGetValueSuccessOverload_ShouldReturnTrue_WhenValidationSucceeds()
   {
      // Arrange.
      var number = 9;
      var success = new IntBetweenOneAndTen(number);
      var sut = new CreateResult<IntBetweenOneAndTen, NumberValidationError>(success);

      // Act.
      var result = sut.TryGetValue(out IntBetweenOneAndTen value);

      // Assert.
      result.Should().BeTrue();
      value.Should().Be(success);
   }

   [Fact]
   public void CreateResult_TryGetValueSuccessOverload_ShouldReturnFalse_WhenValidationFails()
   {
      // Arrange.
      var number = -1;
      var validationError = new NumberValidationError(new TooLow(number));
      var sut = new CreateResult<IntBetweenOneAndTen, NumberValidationError>(validationError);
      IntBetweenOneAndTen expected = default;

      // Act.
      var result = sut.TryGetValue(out IntBetweenOneAndTen value);

      // Assert.
      result.Should().BeFalse();
      value.Should().BeEquivalentTo(expected);
   }

   [Fact]
   public void CreateResult_TryGetValueFailureOverload_ShouldReturnTrue_WhenValidationFails()
   {
      // Arrange.
      var number = -1;
      var validationError = new NumberValidationError(new TooLow(number));
      var sut = new CreateResult<IntBetweenOneAndTen, NumberValidationError>(validationError);

      // Act.
      var result = sut.TryGetValue(out NumberValidationError value);

      // Assert.
      result.Should().BeTrue();
      value.Should().Be(validationError);
   }

   [Fact]
   public void CreateResult_TryGetValueFailureOverload_ShouldReturnFalse_WhenValidationSucceeds()
   {
      // Arrange.
      var number = 9;
      var success = new IntBetweenOneAndTen(number);
      var sut = new CreateResult<IntBetweenOneAndTen, NumberValidationError>(success);
      NumberValidationError expected = default;

      // Act.
      var result = sut.TryGetValue(out NumberValidationError value);

      // Assert.
      result.Should().BeFalse();
      value.Should().BeEquivalentTo(expected);
   }

   #endregion
}
