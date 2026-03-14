namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible types of identifiers that can be represented with a
///   <see cref="SePersonnummer"/> object.
/// </summary>
public enum SeIdentifierType
{
   /// <summary>
   ///   Personal identity number, issued to individuals residing in Sweden for
   ///   more than 12 months (or planning to do so).
   /// </summary>
   Personnummer = 0,

   /// <summary>
   ///   Coordination number serving many of the purposes of a personnummer
   ///   issued to individuals residing in Sweden for less than a year. Same
   ///   format as a personnummer, except 60 is added to the day component of
   ///   the day of birth (i.e. "950123" -> "950183").
   /// </summary>
   Samordningsnummer = 1
}
