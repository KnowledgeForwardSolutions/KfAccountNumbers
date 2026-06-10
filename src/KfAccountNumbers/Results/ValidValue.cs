#pragma warning disable CA1822 // Mark members as static

namespace KfAccountNumbers.Results;

/// <summary>
///   Validation result that indicates that the value is valid.
/// </summary>
public readonly record struct ValidValue
{
   /// <summary>
   ///   Gets a message that describes the value as being valid.
   /// </summary>
   public String Description => Messages.ValidValueDescription;
}
