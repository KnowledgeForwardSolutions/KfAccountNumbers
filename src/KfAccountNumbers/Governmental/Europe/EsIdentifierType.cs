namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible types of identifiers that can be represented with a
///   <see cref="EsNif"/> object.
/// </summary>
public class EsIdentifierType
{
   /// <summary>
   ///   Identifier assigned to Spanish citizens.
   /// </summary>
   public struct Dni { }

   /// <summary>
   ///   Identifier assigned to foreigners residing in Spain.
   /// </summary>
   public struct Nie { }
}
