namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Defines the possible types of identifiers that can be represented with a
///   <see cref="NoFoedselsnummer"/> object.
/// </summary>
public class NoIdentifierType
{
   /// <summary>
   ///   Personal identity number, issued to citizens or long-term residents of
   ///   Norway.
   /// </summary>
   public struct Foedselsnummer { }

   /// <summary>
   ///   Identifier issued to foreign individuals not eligible for fødselsnummer.
   ///   Same format as a fødselsnummer, except 40 is added to the day component
   ///   of the day of birth (i.e. 130585 becomes 530585).
   /// </summary>
   public struct DNummer { }
}
