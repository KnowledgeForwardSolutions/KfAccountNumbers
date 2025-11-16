namespace KfAccountNumbers.Utility;

/// <summary>
///   Result returned by a KfAccountNumbers business object Create method.
/// </summary>
/// <typeparam name="T">
///   The business object type.
/// </typeparam>
/// <typeparam name="V">
///   Enumerates the possible validation rules that could be failed when 
///   creating a KfAccountNumbers business object.
/// </typeparam>
public record CreateResult<T, V> where V : Enum
{
   private CreateResult(T value)
   {
      Value = value ?? throw new ArgumentNullException(nameof(value), Messages.CreateResultValueNull);
      IsSuccess = true;
   }

   private CreateResult(V validationFailure)
   {
      ValidationFailure = validationFailure;
      IsSuccess = false;
   }

   /// <summary>
   ///   The validation rule that was failed when attempting to create the
   ///   business object. Will be <see langword="null"/> when 
   ///   <see cref="IsSuccess"/> is <see langword="true"/>.
   /// </summary>
   public V? ValidationFailure { get; private init; } = default!;

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

   public static implicit operator CreateResult<T, V>(T value) => new(value);

   public static implicit operator CreateResult<T, V>(V validationFailure) => new(validationFailure);
}

