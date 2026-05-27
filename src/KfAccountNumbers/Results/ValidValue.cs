namespace KfAccountNumbers.Results;

/// <summary>
///   Validation result that indicates that the value is valid.
/// </summary>
public record struct ValidValue
{
   /// <summary>
   ///   Gets a message that describes the value as being valid.
   /// </summary>
#pragma warning disable CA1822 // Mark members as static
   public String Description => Messages.ValidValueDescription;
#pragma warning restore CA1822 // Mark members as static
}
