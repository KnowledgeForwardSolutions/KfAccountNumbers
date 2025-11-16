namespace KfAccountNumbers.Utility;

/// <summary>
///   Abstract base type for exceptions thrown when attempting to convert a 
///   string to one of the strongly typed business objects in KfAccountNumbers.
/// </summary>
/// <typeparam name="V">
///   Enum type that enumerates the possible validation errors.
/// </typeparam>
public abstract class InvalidAccountNumberException<V> : Exception where V : Enum
{
   protected InvalidAccountNumberException(
      V validationResult,
      String message,
      Int32? invalidCharacterOffset = null) : base(message)
   {
      ValidationResult = validationResult;
   }

   /// <summary>
   ///   Enum value that indicates the validation rule that was failed.
   /// </summary>
   public V ValidationResult { get; init; }
}
