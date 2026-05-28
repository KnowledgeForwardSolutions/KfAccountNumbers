namespace KfAccountNumbers.Results;

/// <summary>
///   The EmptyValue struct represents a validation error indicating that a
///   required value is empty, null, or all whitespace characters.
/// </summary>
public record struct EmptyValue
{
   /// <summary>
   ///   Gets a message describing the validation error, indicating that the
   ///   value is empty, null, or consists solely of whitespace characters.
   /// </summary>
#pragma warning disable CA1822 // Mark members as static
   public String Description => Messages.EmptyValueDescription;
#pragma warning restore CA1822 // Mark members as static
}
