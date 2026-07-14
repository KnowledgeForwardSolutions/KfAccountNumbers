namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Strongly typed business object that represents an Italian tax identifier
///   assigned to individuals by the Italian tax office, the Agenzia delle
///   Entrate.
/// </summary>
/// <remarks>
///   <para>
///      A Codice Fiscale is a 16-character value structured as
///      SSSGGGYYMDDLLLLC, with the following elements:
///      <list type="bullet">
///         <item>
///            <term>SSS</term>
///            <description>
///               Three characters from the person's surname.
///            </description>
///         </item>
///         <item>
///            <term>GGG</term>
///            <description>
///               Three characters from the person's given name.
///            </description>
///         </item>
///         <item>
///            <term>YY</term>
///            <description>
///               Two-digit year of birth.
///            </description>
///         </item>
///         <item>
///            <term>M</term>
///            <description>
///               Month of birth encoded as a single alphabetic character.
///            </description>
///         </item>
///         <item>
///            <term>DD</term>
///            <description>
///               Two-digit day of birth. Also encodes the person's gender,
///               where 0-31 is used for males and 41-71 (day +40) is used for
///               females.
///            </description>
///         </item>
///         <item>
///            <term>LLLL</term>
///            <description>
///               Four-character Belifore code that indicates the person's town
///               of birth. The code consists of one alphabetic character
///               followed by three digits. Persons who were born in a foreign
///               country will have a code starting with Z and a three-digit
///               code indicating the country of birth.
///            </description>
///         </item>
///         <item>
///            <term>C</term>
///            <description>
///               Modulus 26 check character.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      In cases where the personal details for two different individuals
///      result in the same tax code (known as "omocodia" or "same code"), one
///      or more digits of the value are replaced with letter equivalents until
///      a unique code is generated. See below.
///   </para>
///   <para>
///      When creating a new <see cref="ItCodiceFiscale"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The string must be 16 characters long.
///            </description>
///         </item>
///         <item>
///            <description>
///               The surename characters, positions 0-2 (zero-based), must be
///               upper-case or lower-case alphabetic characters ('A'-'Z',
///               'a'-'z')
///            </description>
///         </item>
///         <item>
///            <description>
///               The given name characters, positions 3-5 (zero-based), must be
///               upper-case or lower-case alphabetic characters ('A'-'Z',
///               'a'-'z')
///            </description>
///         </item>
///         <item>
///            <description>
///               The year of birth, character positions 6-7 (zero-based), must
///               be ASCII digits ('0'-'9') or the equivalent Omocodia letter
///               (see below)
///            </description>
///         </item>
///         <item>
///            <description>
///               The month of birth, character position 8 (zero-based) must be
///               an upper-case or lower-case alphabetic character. Valid month
///               characters are "ABCDEHLMPRST", where 'A' = January and 'T' =
///               December
///            </description>
///         </item>
///         <item>
///            <description>
///               The day of birth, character positions 9-10 (zero-based) must
///               be two ASCII digits ('0'-'9') the equivalent Omocodia letter
///               (see below). Male = 01-31 and female = 61-91
///            </description>
///         </item>
///         <item>
///            <description>
///               The town of birth, character positions 11-14 (zero-based) must
///               be an upper-case or lower-case alphabetic character ('A'-'Z',
///               'a'-'z') followed by three ASCII digits ('0'-'9') the
///               equivalent Omocodia letter (see below)
///            </description>
///         </item>
///         <item>
///            <description>
///               The check character, position 15 (zero-based) must be a valid
///               upper-case or lower-case alphabetic ('A'-'Z', 'a'-'z') modulus
///               26 check character
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>MRTMTT91D08F205J</term>
///            <description>
///               Matteo Moretti (male), born in Milan on 8 April 1991 (example
///               from wikipedia): surname characters = MRT, given name
///               characters = MTT, year of birth = 91, month of birth = D, day
///               of birth = 01, gender = male, town of birth = F205, check
///               character = J
///            </description>
///         </item>
///         <item>
///            <term>MLLSNT82P65Z404U</term>
///            <description>
///               Samantha Miller (female), born in the USA on 25 September
///               1982, living in Italy (example from wikipedia): surname
///               characters = MLL, given name characters = SNT, year of birth =
///               82, month of birth = P, day of birth = 65 (actual day of
///               birth = 25), gender = female, town of birth = Z404, check
///               character = U
///            </description>
///         </item>
///         <item>
///            <term>RSSMRA85C15H5LMY</term>
///            <description>
///               example of omocodia with the two right-most digits replaced by
///               letter equivalents: surname characters = RSS, given name
///               characters = MRA, year of birth = 85, month of birth = C, day
///               of birth = 15, gender = male, town of birth = H5LM (actual
///               town of birth = H501), check character = Y
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      <see cref="ItCodiceFiscale"/> is case-insensitive for validation and
///      parsing purposes. The ItCodiceFiscale constructor, Create method and
///      explicit string to ItCodiceFiscale operator will normalize any
///      lowercase letters to uppercase. Equality and inequality comparisons
///      between instances of ItCodiceFiscale will compare the normalized
///      uppercase versions of the value.
///   </para>
///   <para>
///      <see cref="ItCodiceFiscale"/> does not validate the town of birth (the
///      Belifore code) against a comprehensive list of valid values (because
///      the list of values exceeds 8000 entries). ItCodiceFiscale only
///      validates the format, one alphabetic character followed by three digit
///      characters.
///   </para>
///   <para>
///      While rare, it is possible that two different people can generate the
///      same tax code, a situation referred to as "omocodia" (or "same code").
///      The Agenzia delle Entrate handles omocodia by progressively replacing
///      digits with a letter equivalent, starting at the right-most digit and
///      progressing leftward until a unique code is generated. After a unique
///      code is generated, the check character is calculated for the unique
///      code. The letter substitutions are "LMNPQRSTUV", where 0 = L and 9 = V.
///      See<see href="https://codicefiscale.expert/en/omocodia">CodiceFiscale.expert</see>
///      for a full description of the handling of omocodia.
///   </para>
///   <para>
///      If a value contains an omocidia substitution, the substituted letters
///      are converted to the equivalent digits before validation or retrieving
///      the BirthYear, Gender or Belifore code.
///   </para>
///   <para>
///      ItCodiceFiscale uses a custom modulus 26 check digit algorithm. Refer
///      to the English Wikipedia article below for a full description of the
///      algorithm. The custom algorithm has some weaknesses described in the
///      Italian Wikipedia article below. For example, the algorithm cannot two
///      character jump transpositions (i.e. DEF ⇒ FED) nor transpositions of
///      the characters WY (i.e. WY ⇒ YW).
///   </para>
///   <para>
///      See <see href="https://en.wikipedia.org/wiki/Italian_fiscal_code">Wikipedia - Italian fiscal code</see>
///      and <see href="https://it.wikipedia.org/wiki/Codice_fiscale">Wikipedia (Italian) - Codice fiscale</see>
///      for more information.
///   </para>
/// </remarks>
public record ItCodiceFiscale
{
}
