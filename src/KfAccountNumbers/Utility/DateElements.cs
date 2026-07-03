namespace KfAccountNumbers.Utility;

/// <summary>
///   Specifies the components of a date that are present or relevant in a given
///   context.
/// </summary>
/// <remarks>
///   This enumeration supports bitwise combination of its member values to
///   represent partial or complete dates. For example, a value of Year | Month
///   indicates that both the year and month components are specified, but the
///   day is not. Use the CompleteDate value to represent a fully specified date,
///   and YearOnly to indicate that only the year component is present.
/// </remarks>
[Flags]
public enum DateElements
{
   /// <summary>
   ///   Indicates that none of the components of the date (year, month, day)
   ///   are available.
   /// </summary>
   None = 0,

   /// <summary>
   ///   Indicates that the year component of the date is available.
   /// </summary>
   Year = 1,

   /// <summary>
   ///   Indicates that the month component of the date is available.
   /// </summary>
   Month = 2,

   /// <summary>
   ///   Indicates that the day component of the date is available.
   /// </summary>
   Day = 4,

   /// <summary>
   ///   Indicates that the date is unknown and is equivalent to the value 'None'.
   /// </summary>
   UnknownDate = None,

   /// <summary>
   ///   Specifies all the components of a date (year, month, day) are available.
   /// </summary>
   CompleteDate = Year | Month | Day,

   /// <summary>
   ///   Specifies that only the year component of a date is available.
   /// </summary>
   YearOnly = Year,
}
