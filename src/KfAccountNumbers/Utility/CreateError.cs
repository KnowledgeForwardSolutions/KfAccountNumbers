namespace KfAccountNumbers.Utility;

/// <summary>
///   Details for an error encountered when creating a KfAccountNumbers business
///   object.
/// </summary>
/// <typeparam name="E">
///   Enumerates the possible error that could be encountered when creating a
///   KfAccountNumbers business object.
/// </typeparam>
public record CreateError<E> where E : Enum
{
   /// <summary>
   ///   Initialize a new <see cref="CreateError{E}"/> object.
   /// </summary>
   /// <param name="errorType">
   ///   Identifies the error encountered.
   /// </param>
   /// <param name="description">
   ///   Description of the error encountered.
   /// </param>
   /// <exception cref="ArgumentException">
   ///   <paramref name="description"/> is <see cref="String.Empty"/> or all 
   ///   whitespace characters.
   /// </exception>
   /// <exception cref="ArgumentNullException">
   ///   <paramref name="description"/> is <see langword="null"/>.
   /// </exception>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="errorType"/> is default <typeparamref name="E"/>.
   /// </exception>
   public CreateError(E errorType, String description)
   {
      if (EqualityComparer<E>.Default.Equals(errorType, default))
      {
         throw new ArgumentOutOfRangeException(nameof(errorType), errorType, Messages.CreateErrorDefaultErrorType);
      }

      ErrorType = errorType;
      Description = description.RequiresNotNullOrWhiteSpace(Messages.CreateErrorDescriptionEmpty);
   }

   /// <summary>
   ///   Description of the error encountered.
   /// </summary>
   public String Description { get; private init; }

   /// <summary>
   ///   Identifies the error encountered.
   /// </summary>
   public E ErrorType { get; private init; }
}
