namespace KfAccountNumbers;

/// <summary>
///   Exception thrown when a validation error occurs. The type parameter TE
///   represents the specific validation error type, which must implement the
///   IUnion interface. The exception message is derived from the validation
///   error's string representation, or can be provided explicitly through the
///   optional message parameter.
/// </summary>
/// <typeparam name="TE">
///   The type of the validation error. This type must implement the IUnion
///   interface.
/// </typeparam>
/// <param name="validationError">
///   The validation error that caused the exception.
/// </param>
/// <param name="message">
///   An optional message that describes the error. If not provided, the message
///   is derived from the validation error's string representation.
/// </param>
public class UKfValidationException<TE>(TE validationError, String? message = null)
   : Exception(message ?? validationError.ToString())
   where TE : IUnion
{
   /// <summary>
   ///   Gets the specific validation error that caused the exception.
   /// </summary>
   public TE ValidationError { get; } = validationError;
}
