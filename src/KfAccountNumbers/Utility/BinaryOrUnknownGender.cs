namespace KfAccountNumbers.Utility;

/// <summary>
///   Specifies the gender of an individual as either male, female or unknown.
/// </summary>
/// <remarks>
///   Used in historical cases where only binary gender options are defined but
///   where an empty or null value is allowed.
/// </remarks>
public enum BinaryOrUnknownGender
{
   Unknown = 0,
   Male = 1,
   Female = 2
}
