namespace KfAccountNumbers.Utility;

/// <summary>
///   Represents a date with optional year, month, and day components.
/// </summary>
/// <remarks>
///   Use this type to represent partial or complete dates when some components
///   may be missing. The presence of each component can be determined using the
///   corresponding property or the AvailableElements property.
/// </remarks>
public record DateResult
{
   /// <summary>
   ///   Initializes a new instance of the DateResult class with the specified
   ///   year, month, and day values.
   /// </summary>
   /// <param name="year">
   ///   The year component of the date. Must be between 1 and 9999, or null if
   ///   unspecified.
   /// </param>
   /// <param name="month">
   ///   The month component of the date. Must be between 1 and 12, or null if
   ///   unspecified.
   /// </param>
   /// <param name="day">
   ///   The day component of the date. Must be between 1 and 31, or null if
   ///   unspecified. If all components are specified, the day must be valid for
   ///   the given month and year.
   /// </param>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="year"/> is not between 1 and 9999.
   ///   - or -
   ///   <paramref name="month"/> is not between 1 and 12.
   ///   - or -
   ///   <paramref name="day"/> is not between 1 and 31.
   ///   - or -
   ///   <paramref name="day"/> is not valid for the supplied
   ///   <paramref name="year"/> and <paramref name="month"/>.
   /// </exception>
   public DateResult(Int32? year = null, Int32? month = null, Int32? day = null)
   {
      if (year is < 1 or > 9999)
      {
         throw new ArgumentOutOfRangeException(nameof(year), year, Messages.YearOutOfRange);
      }

      if (month is < 1 or > 12)
      {
         throw new ArgumentOutOfRangeException(nameof(month), month, Messages.MonthOutOfRange);
      }

      if (year is not null && month is not null && day is not null)
      {
         if (day > DateTime.DaysInMonth(year.Value, month.Value))
         {
            throw new ArgumentOutOfRangeException(nameof(day),
               day,
               String.Format(Messages.DayOutOfRangeForYearAndMonth, day, year, month));
         }
      }
      else if (day is < 1 or > 31)
      {
         throw new ArgumentOutOfRangeException(nameof(day), day, Messages.DayOutOfRange);
      }


      Year = year;
      Month = month;
      Day = day;
   }

   /// <summary>
   ///   Gets the set of date elements that are available for this instance.
   /// </summary>
   /// <remarks>
   ///   The returned value indicates which components (year, month, day) are
   ///   present and can be accessed. Use this property to determine which date
   ///   parts are valid before attempting to read them.
   /// </remarks>
   public DateElements AvailableElements =>
      (Year is not null ? DateElements.Year : DateElements.None)
      | (Month is not null ? DateElements.Month : DateElements.None)
      | (Day is not null ? DateElements.Day : DateElements.None);

   /// <summary>
   ///   Gets the day of the month, if specified.
   /// </summary>
   public Int32? Day { get; private init; }

   /// <summary>
   ///   Gets the month component of the date, if specified.
   /// </summary>
   public Int32? Month { get; private init; }

   /// <summary>
   ///   Gets the year associated with the current instance, if specified.
   /// </summary>
   public Int32? Year { get; private init; }

   /// <summary>
   ///   Converts the current instance to a DateOnly value representing the
   ///   complete date.
   /// </summary>
   /// <returns>
   ///   A DateOnly value constructed from the year, month, and day components
   ///   of the current instance.
   /// </returns>
   /// <exception cref="InvalidOperationException">
   ///   Thrown if the current instance does not contain a complete date (year,
   ///   month, and day).
   /// </exception>
   public DateOnly ToDateOnly()
      => AvailableElements != DateElements.CompleteDate
         ? throw new InvalidOperationException(Messages.DateResultPartialDateToDateOnly)
         : new DateOnly(Year!.Value, Month!.Value, Day!.Value);
}
