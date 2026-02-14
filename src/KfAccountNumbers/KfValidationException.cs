// Ignore Spelling: Kf

namespace KfAccountNumbers;

/// <summary>
///   Represents errors that occur during validation processes.
/// </summary>
/// <remarks>
///   This exception is typically thrown when validation rules are violated,
///   providing details about the specific validation failures.
/// </remarks>
/// <typeparam name="TV">
///   Enum type that enumerates the possible validation errors.
/// </typeparam>
public class KfValidationException<TV>(
   TV validationResult,
   String message) : Exception(message) where TV : Enum
{
   /// <summary>
   ///   Enum value that indicates the validation rule that was failed.
   /// </summary>
   public TV ValidationResult { get; private init; } = validationResult;
}
