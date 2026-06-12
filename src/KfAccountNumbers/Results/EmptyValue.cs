#pragma warning disable CA1822 // Mark members as static

namespace KfAccountNumbers.Results;

/// <summary>
///   The EmptyValue struct represents a validation error indicating that a
///   required value is empty, null, or all whitespace characters.
/// </summary>
public readonly record struct EmptyValue
{
   /// <summary>
   ///   Gets a message describing the validation error, indicating that the
   ///   value is empty, null, or consists solely of whitespace characters.
   /// </summary>
   public String Description => Messages.EmptyValueDescription;
}
