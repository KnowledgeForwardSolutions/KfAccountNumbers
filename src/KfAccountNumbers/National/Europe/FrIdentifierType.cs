namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Defines the possible types of identifiers in use in France.
/// </summary>
public class FrIdentifierType
{
   /// <summary>
   ///   French NIR (numéro d'inscription au répertoire des personnes physiques)
   ///   issued to citizens and permanent residents. Permanent NIR values have
   ///   a '1' or '2' as the leading digit (1 = male, 2 = female).
   /// </summary>
   public struct Insee { }

   /// <summary>
   /// Temporary NIR by foreign persons before a permanent NIR is issued.
   /// Temporary NIR values have a '7' or '8' as the leading digit (7 = male,
   /// 8 = female).
   /// </summary>
   public struct TemporaryInsee { }
}
