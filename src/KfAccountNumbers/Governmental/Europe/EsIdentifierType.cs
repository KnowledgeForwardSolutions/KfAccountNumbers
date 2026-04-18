namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible types of identifiers that can be represented with a
///   <see cref="EsNif"/> object.
/// </summary>
public enum EsIdentifierType
{
   /// <summary>
   ///   Identifier assigned to Spanish citizens.
   /// </summary>
   Dni = 0,

   /// <summary>
   ///   Identifier assigned to foreigners residing in Spain.
   /// </summary>
   Nie = 1
}
