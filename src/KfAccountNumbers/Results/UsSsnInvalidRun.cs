#pragma warning disable CA1822 // Mark members as static

namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a validation error indicating that a US Social Security Number
///   (SSN) is composed of nine sequential digits (123456789).
/// </summary>
public readonly record struct UsSsnInvalidRun
{
   /// <summary>
   ///   Gets a message describing the validation error, indicating that the
   ///   value is composed of nine sequential digits (123456789).
   /// </summary>
   public String Description => Messages.UsSsnInvalidRun;
}

