#pragma warning disable CA1822 // Mark members as static

namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a US Social Security Number
///   (SSN) is composed of nine identical digits.
/// </summary>
public readonly record struct UsSsnAllIdenticalDigits
{
   /// <summary>
   ///   Gets a message describing the validation error, indicating that the
   ///   value is composed of nine identical digits.
   /// </summary>
   public String Description => Messages.UsSsnAllIdenticalDigits;
}
