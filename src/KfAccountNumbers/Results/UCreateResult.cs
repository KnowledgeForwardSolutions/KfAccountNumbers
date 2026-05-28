namespace KfAccountNumbers.Results;

// TODO: rename to CreateResult and delete enum based CreateResult once all existing code converted to union pattern

/// <summary>
///   Represents a result of a create operation that can either be successful,
///   containing a value of type TS, or unsuccessful, containing an error of
///   type TE.
/// </summary>
/// <remarks>
///   <para>
///      This is a discriminated union type that supports implicit conversion
///      from either success or error values.
///   </para>
///   <para>
///      This struct explicitly implements "non-boxing access pattern" for
///      retrieving the success or error value without incurring boxing overhead.
///   </para>
/// </remarks>
/// <typeparam name="TS">
///   The type of the value created when the operation is successful.
/// </typeparam>
/// <typeparam name="TE">
///   The type of the error returned when the operation is not successful. This
///   type must implement the IUnion interface.
/// </typeparam>
[Union]
public readonly struct UCreateResult<TS, TE> : IUnion
   where TS : notnull
   where TE : IUnion
{
   private readonly TS _success;
   private readonly TE _error;

   /// <summary>
   ///   Initializes a new instance of the <see cref="UCreateResult{TS, TE}"/>
   ///   struct with a successful result.
   /// </summary>
   /// <param name="value">
   ///   The result of a successful create operation. This value must not be
   ///   <see langword="null"/>.
   /// </param>
   /// <exception cref="ArgumentNullException">
   ///   <paramref name="value"/> is <see langword="null"/>.
   /// </exception>
   public UCreateResult(TS value)
   {
      _success = value ?? throw new ArgumentNullException(nameof(value));
      _error = default!;
      IsSuccess = true;
   }

   /// <summary>
   ///   Initializes a new instance of the <see cref="UCreateResult{TS, TE}"/>
   ///   struct with a value that indicates a failed create operation.
   /// </summary>
   /// <param name="error">
   ///   The error that caused the failure.
   /// </param>
   public UCreateResult(TE error)
   {
      _error = error;
      _success = default!;
      IsSuccess = false;
   }

   /// <summary>
   ///   Gets a value indicating whether this instance has a value.
   /// </summary>
   /// <remarks>
   ///   Supports the "non-boxing access pattern" for retrieving the success or
   ///   error value without incurring boxing overhead.
   /// </remarks>
   public Boolean HasValue => true;

   /// <summary>
   ///   Gets a value indicating whether this instance represents a successful
   ///   creation operation.
   /// </summary>
   public Boolean IsSuccess { get; private init; }

   /// <summary>
   ///   Gets the success result or error value.
   /// </summary>
   public Object Value => IsSuccess ? _success! : _error!;

   /// <summary>
   ///   Implicitly converts a <typeparamref name="TS"/> success value to a
   ///   CreateResult.
   /// </summary>
   /// <param name="value">
   ///   The success value.
   /// </param>
   public static implicit operator UCreateResult<TS, TE>(TS value) => new(value);

   /// <summary>
   ///   Implicitly converts a <typeparamref name="TE"/> error value to a
   ///   CreateResult.
   /// </summary>
   /// <param name="error">
   ///   The error value.
   /// </param>
   public static implicit operator UCreateResult<TS, TE>(TE error) => new(error);

   /// <summary>
   ///   Gets the success value if available, or the default value otherwise.
   /// </summary>
   /// <param name="success">
   ///   When this method returns, contains the success value if available;
   ///   otherwise, the default value of type TS.</param>
   /// <returns>
   ///   <see langword="true"/> if this instance represents a successful
   ///   validation; otherwise <see langword="false"/>.
   /// </returns>
   public Boolean TryGetValue(out TS success)
   {
      success = IsSuccess ? _success : default!;
      return IsSuccess;
   }

   /// <summary>
   ///   Gets the error value if available, or the default value otherwise.
   /// </summary>
   /// <param name="error">
   ///   When this method returns, contains the error value if the result
   ///   represents a failure; otherwise, the default value.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if this instance represents a failure and
   ///   contains an error; otherwise <see langword="false"/>.
   /// </returns>
   public Boolean TryGetValue(out TE error)
   {
      error = !IsSuccess ? _error : default!;
      return !IsSuccess;
   }
}
