// Ignore Spelling: Curp Mx

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Strongly typed business object for a Clave Unica de Registro de Poblacion
///   (CURP).
/// </summary>
/// <remarks>
///   <para>
///      A valid CURP is an 18-character alphanumeric identifier assigned to 
///      Mexican citizens by the Registry Nacional de Poblacion (RENAPO). 
///      Portions of the string are generated from a person's personal 
///      information and the two trailing characters are assigned by RENAPO. 
///   </para>
///   <para>
///      A valid CURP meets the following criteria:
///      <list type="bullet">
///         <item>
///            <description>
///               Not is <see langword="null"/>, <see cref="String.Empty"/> or 
///               all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               Eighteen (18) characters in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               Has alphabetic characters derived the persons surname(s) and
///               given name in positions 0-3 and 13-15 (zero-based).
///            </description>
///         </item>
///         <item>
///            <description>
///               Has a valid date of birth in positions 4-9 (zero-based).
///            </description>
///         </item>
///         <item>
///            <description>
///               Has a valid gender character in position 10 (zero-based). Must
///               be H (Hombre), M (Mujer) or X (non-binary).
///            </description>
///         </item>
///         <item>
///            <description>
///               Has a valid state code in positions 11-12 (zero-based).
///            </description>
///         </item>
///         <item>
///            <description>
///               Has an alphanumeric homoclave character in position 16 
///               (zero-based). The homoclave is assigned by RENAPO to avoid 
///               duplicate CURP values.
///            </description>
///         </item>
///         <item>
///            <description>
///               Has a digit check digit character in position 17 (zero-based).
///               The check digit is assigned by RENAPO.
///            </description>
///         </item>
///      </list>
///   </para>
/// </remarks>
public record MxCurp
{
   private const Int32 ValidLength = 18;

   public MxCurp(String curp)
   {
      throw new NotImplementedException();
   }

   /// <summary>
   ///   The raw CURP value.
   /// </summary>
   public String Value { get; init; }

   public static implicit operator String(MxCurp curp)
      => curp?.Value ?? throw new ArgumentNullException(nameof(curp), Messages.MxCurpInvalidNullConversionToString);

   public static implicit operator MxCurp(String curp) => new(curp);

   /// <summary>
   ///   Get a string representation of the CURP.
   /// </summary>
   public override String ToString() => Value;

   public CreateResult<MxCurp, MxCurpValidationResult> Create(String curp)
   {
      throw new NotImplementedException();
   }

   public static MxCurpValidationResult Validate(String curp)
   {
      throw new NotImplementedException();
   }
}
