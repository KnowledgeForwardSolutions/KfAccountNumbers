namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible types of identifiers that can be represented with a
///   <see cref="BeRijksregisternummer"/> object.
/// </summary>
public enum BeIdentifierType
{
   /// <summary>
   ///   Identifier assigned to a person registered in Belgium's National
   ///   Register.
   /// </summary>
   Rijksregisternummer = 0,

   /// <summary>
   ///   Identifier assigned to a person who does not have a rijksregisternummer,
   ///   but who still needs an identifier for tax or other purposes.
   /// </summary>
   BisNummer = 1
}
