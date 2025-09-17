namespace KfAccountNumbers.Utility;

/// <summary>
///   Result returned by a KfAccountNumbers business object Create method.
/// </summary>
/// <typeparam name="T">
///   The business object type.
/// </typeparam>
/// <typeparam name="E">
///   Enumerates the possible error that could be encountered when creating a
///   KfAccountNumbers business object.
/// </typeparam>
public record CreateResult<T, E> where E : Enum
{
   private CreateResult(T value)
   {
      Value = value ?? throw new ArgumentNullException(nameof(value), Messages.CreateResultValueNull);
      IsSuccess = true;
   }

   private CreateResult(CreateError<E> error)
   {
      Error = error ?? throw new ArgumentNullException(nameof(error), Messages.CreateResultErrorNull);
      IsSuccess = false;
   }

   /// <summary>
   ///   The error encountered when creating the business object. Will be 
   ///   <see langword="null"/> when <see cref="IsSuccess"/> is 
   ///   <see langword="false"/>.
   /// </summary>
   public CreateError<E>? Error { get; private init; } = default!;

   /// <summary>
   ///   <see langword="true"/> if the business object was created successfully;
   ///   otherwise <see langword="false"/>.
   /// </summary>
   public Boolean IsSuccess { get; private init; }

   /// <summary>
   ///   The created business object if <see cref="IsSuccess"/> is 
   ///   <see langword="true"/>; otherwise <see langword="null"/>.
   /// </summary>
   public T? Value { get; private init; } = default!;

   public static implicit operator CreateResult<T, E>(T value) => new(value);

   public static implicit operator CreateResult<T, E>(CreateError<E> error) => new(error);
}
