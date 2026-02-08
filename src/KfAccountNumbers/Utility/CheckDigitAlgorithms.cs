// Ignore Spelling: Luhn

namespace KfAccountNumbers.Utility;

using GeneralCheckDigitAlgorithms = CheckDigits.Net.GeneralAlgorithms;

/// <summary>
///   Lazy instantiated singleton instances of check digit algorithms used by
///   various identifier types.
/// </summary>
/// TODO: Rethink the need for this class once CheckDigits.Net supports MaskedAlgorithms class
public static class CheckDigitAlgorithms
{
   private static readonly Lazy<GeneralCheckDigitAlgorithms.LuhnAlgorithm> _luhn =
      new(() => new GeneralCheckDigitAlgorithms.LuhnAlgorithm());

   /// <summary>
   ///   Luhn algorithm.
   /// </summary>
   public static GeneralCheckDigitAlgorithms.LuhnAlgorithm Luhn => _luhn.Value;
}
