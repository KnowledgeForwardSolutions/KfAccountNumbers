namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible types of identifiers that can be represented with a
///   <see cref="BeRijksregisternummer"/> object.
/// </summary>
public class BeIdentifierType
{
   /// <summary>
   ///   Identifier assigned to a person registered in Belgium's National
   ///   Register.
   /// </summary>
   public struct Rijksregisternummer { }

   /// <summary>
   ///   Identifier assigned to a person who does not have a rijksregisternummer,
   ///   but who still needs an identifier for tax or other purposes.
   /// </summary>
   public struct BisNummer { }
}
