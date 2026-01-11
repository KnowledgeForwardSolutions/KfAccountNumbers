namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Collection of state codes (claves de entidades federativas) used in a
///   Mexican Clave Única de Registro de Población (CURP).
/// </summary>
/// <remarks>
///   See https://es.wikipedia.org/wiki/Anexo:Cat%C3%A1logo_de_claves_de_entidades_federativas
/// </remarks>
public static class MxCurpStateCodes
{
   private static readonly Dictionary<String, String>.AlternateLookup<ReadOnlySpan<Char>> _validStates =
      new Dictionary<String, String>(new CaseInsensitiveSpanComparer())
      {
         { "AS", "Aguascalientes" },
         { "BC", "Baja California" },
         { "BS", "Baja California Sur" },
         { "CC", "Campeche" },
         { "CL", "Coahuila de Zaragoza" },
         { "CM", "Colima" },
         { "CS", "Chiapas" },
         { "CH", "Chihuahua" },
         { "DF", "Ciudad de México" },
         { "DG", "Durango" }, 
         { "GT", "Guanajuato" }, 
         { "GR", "Guerrero" }, 
         { "HG", "Hidalgo" }, 
         { "JC", "Jalisco" }, 
         { "MC", "México" }, 
         { "MN", "Michoacán de Ocampo" }, 
         { "MS", "Morelos" }, 
         { "NT", "Nayarit" }, 
         { "NL", "Nuevo León" }, 
         { "OC", "Oaxaca" }, 
         { "PL", "Puebla" }, 
         { "QT", "Querétaro" }, 
         { "QR", "Quintana Roo" }, 
         { "SP", "San Luis Potosí" }, 
         { "SL", "Sinaloa" }, 
         { "SR", "Sonora" }, 
         { "TC", "Tabasco" }, 
         { "TS", "Tamaulipas" }, 
         { "TL", "Tlaxcala" }, 
         { "VZ", "Veracruz" }, 
         { "YN", "Yucatán" }, 
         { "ZS", "Zacatecas" }, 
         { "NE", "Nacido en el Extranjero" }, 


      }
      .GetAlternateLookup<ReadOnlySpan<Char>>();

   /// <summary>
   ///   Get an unordered array of all valid state codes.
   /// </summary>
   /// <returns>
   ///   An unordered array of all valid state codes.
   /// </returns>
   public static String[] GetAllStateCodes() => _validStates.Dictionary.Keys.ToArray();

   /// <summary>
   ///   Get the name of the state with the supplied <paramref name="stateCode"/>.
   /// </summary>
   /// <param name="stateCode">
   ///   The state code for the state name to retrieve.
   /// </param>
   /// <returns>
   ///   The name of the state with the supplied <paramref name="stateCode"/> or
   ///   <see cref="String.Empty"/> if the stateCode is not valid.
   /// </returns>
   /// <remarks>
   ///   This method performs a case-insensitive comparison of the supplied 
   ///   <paramref name="stateCode"/> to the valid state codes.
   /// </remarks>
   public static String GetStateName(ReadOnlySpan<Char> stateCode)
      => _validStates.TryGetValue(stateCode, out var name) ? name : String.Empty;

   /// <summary>
   ///   Check if the supplied <paramref name="stateCode"/> is one of the 33 
   ///   codes that are valid for a CURP.
   /// </summary>
   /// <param name="stateCode">
   ///   The code to check.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if <paramref name="stateCode"/> is a valid state
   ///   code; otherwise <see langword="false"/>.
   /// </returns>
   /// <remarks>
   ///   This method performs a case-insensitive comparison of the supplied 
   ///   <paramref name="stateCode"/> to the valid state codes.
   /// </remarks>
   public static Boolean ValidateStateCode(ReadOnlySpan<Char> stateCode)
      => _validStates.ContainsKey(stateCode);
}
