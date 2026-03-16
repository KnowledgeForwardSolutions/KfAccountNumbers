namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible types of identifiers that can be represented with a
///   <see cref="NoFodselsnummer"/> object.
/// </summary>
public enum NoIdentifierType
{
   /// <summary>
   ///   Personal identity number, issued to citizens or long-term residents of
   ///   Norway.
   /// </summary>
   Fodselsnummer = 0,

   /// <summary>
   ///   Identifier issued to foreign individuals not eligible for fødselsnummer.
   ///   Same format as a fødselsnummer, except 40 is added to the day component
   ///   of the day of birth (i.e. 130585 becomes 530485).
   /// </summary>
   DNummer = 1
}
