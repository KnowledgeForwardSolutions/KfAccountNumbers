namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Collection of French department codes.
/// </summary>
/// <remarks>
///   See https://en.wikipedia.org/wiki/Departments_of_France.
/// </remarks>
public static class FrDepartmentCodes
{
   private static readonly Dictionary<String, String>.AlternateLookup<ReadOnlySpan<Char>> _validDepartments =
      new Dictionary<String, String>(new CaseInsensitiveSpanComparer())
      {
         { "01", "Ain" },
         { "02", "Aisne" },
         { "03", "Allier" },
         { "04", "Alpes-de-Haute-Provence" },
         { "05", "Hautes-Alpes" },
         { "06", "Alpes-Maritimes" },
         { "07", "Ardèche" },
         { "08", "Ardennes" },
         { "09", "Ariège" },
         { "10", "Aube" },
         { "11", "Aude" },
         { "12", "Aveyron" },
         { "13", "Bouches-du-Rhône" },
         { "14", "Calvados" },
         { "15", "Cantal" },
         { "16", "Charente" },
         { "17", "Charente-Maritime" },
         { "18", "Cher" },
         { "19", "Corrèze" },
         { "2A", "Corse-du-Sud" },
         { "2B", "Haute-Corse" },
         { "21", "Côte-d'Or" },
         { "22", "Côtes-d'Armor" },
         { "23", "Creuse" },
         { "24", "Dordogne" },
         { "25", "Doubs" },
         { "26", "Drôme" },
         { "27", "Eure" },
         { "28", "Eure-et-Loire" },
         { "29", "Finistère" },
         { "30", "Gard" },
         { "31", "Haute-Garonne" },
         { "32", "Gers" },
         { "33", "Gironde" },
         { "34", "Hérault" },
         { "35", "Ille-et-Vilaine" },
         { "36", "Indre" },
         { "37", "Indre-et-Loire" },
         { "38", "Isère" },
         { "39", "Jura" },
         { "40", "Landes" },
         { "41", "Loir-et-Cher" },
         { "42", "Loire" },
         { "43", "Haute-Loire" },
         { "44", "Loire-Atlantique" },
         { "45", "Loiret" },
         { "46", "Lot" },
         { "47", "Lot-et-Garonne" },
         { "48", "Lozère" },
         { "49", "Maine-et-Loire" },
         { "50", "Manche" },
         { "51", "Marne" },
         { "52", "Haute-Marne" },
         { "53", "Mayenne" },
         { "54", "Meurthe-et-Moselle" },
         { "55", "Meuse" },
         { "56", "Morbihan" },
         { "57", "Moselle" },
         { "58", "Nièvre" },
         { "59", "Nord" },
         { "60", "Oise" },
         { "61", "Orne" },
         { "62", "Pas-de-Calais" },
         { "63", "Puy-de-Dôme" },
         { "64", "Pyrénées-Atlantiques" },
         { "65", "Hautes-Pyrénées" },
         { "66", "Pyrénées-Orientales" },
         { "67", "Bas-Rhin" },
         { "68", "Haut-Rhin" },
         { "69", "Rhône" },
         { "70", "Haute-Saône" },
         { "71", "Saône-et-Loire" },
         { "72", "Sarthe" },
         { "73", "Savoie" },
         { "74", "Haute-Savoie" },
         { "75", "Paris" },
         { "76", "Seine-Maritime" },
         { "77", "Seine-et-Marne" },
         { "78", "Yvelines" },
         { "79", "Deux-Sèvres" },
         { "80", "Somme" },
         { "81", "Tarn" },
         { "82", "Tarn-et-Garonne" },
         { "83", "Var" },
         { "84", "Vaucluse" },
         { "85", "Vendée" },
         { "86", "Vienne" },
         { "87", "Haute-Vienne" },
         { "88", "Vosges" },
         { "89", "Yonne" },
         { "90", "Territoire de Belfort" },
         { "91", "Essonne" },
         { "92", "Hauts-de-Seine" },
         { "93", "Seine-Saint-Denis" },
         { "94", "Val-de-Marne" },
         { "95", "Val-d'Oise" },
         { "971", "Guadeloupe" },
         { "972", "Martinique" },
         { "973", "Guyane" },
         { "974", "La Réunion" },
         { "976", "Mayotte" },
         { "99", "à l'étranger" },
      }
      .GetAlternateLookup<ReadOnlySpan<Char>>();

   /// <summary>
   ///   Get an unordered array of all valid department codes.
   /// </summary>
   /// <returns>
   ///   An unordered array of all valid department codes.
   /// </returns>
   public static String[] GetAllDepartmentCodes() => _validDepartments.Dictionary.Keys.ToArray();

   /// <summary>
   ///   Get the name of the department with the supplied <paramref name="departmentCode"/>.
   /// </summary>
   /// <param name="departmentCode">
   ///   The department code for the department name to retrieve.
   /// </param>
   /// <returns>
   ///   The name of the department with the supplied <paramref name="departmentCode"/> or
   ///   <see cref="String.Empty"/> if the departmentCode is not valid.
   /// </returns>
   /// <remarks>
   ///   This method performs a case-insensitive comparison of the supplied
   ///   <paramref name="departmentCode"/> to the valid department codes.
   /// </remarks>
   public static String GetDepartmentName(ReadOnlySpan<Char> departmentCode)
      => _validDepartments.TryGetValue(departmentCode, out var name) ? name : String.Empty;

   /// <summary>
   ///   Check if the supplied <paramref name="departmentCode"/> is one of the
   ///   valid French department codes.
   /// </summary>
   /// <param name="departmentCode">
   ///   The code to check.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if <paramref name="departmentCode"/> is a valid department
   ///   code; otherwise <see langword="false"/>.
   /// </returns>
   /// <remarks>
   ///   This method performs a case-insensitive comparison of the supplied
   ///   <paramref name="departmentCode"/> to the valid department codes.
   /// </remarks>
   public static Boolean ValidateDepartmentCode(ReadOnlySpan<Char> departmentCode)
      => _validDepartments.ContainsKey(departmentCode);
}
