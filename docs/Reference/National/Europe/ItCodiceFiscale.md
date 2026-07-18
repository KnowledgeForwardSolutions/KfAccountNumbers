## ItCodiceFiscale

Italian tax identifier assigned to individuals by the Italian tax office, the Agenzia delle Entrate.

| Element | Description |
| :------ | :---------- |
| Class name: | KfAccountNumbers.National.Europe.ItCodiceFiscale |
| Is composite: | No |
| Length: | 16 |
| Check digit algorithm: | Weighted modulus 11, with two different sets of weights |
| Allowed characters: | Alphanumeric ('0'-'9', 'A-Z', 'a-z') |
| Structure: | ***SSSGGGYYMDDLLLLC*** (formatted), where: <dl><dt>SSS</dt><dd>Three characters from the person's surname</dd><dt>GGG</dt><dd>Three characters from the person's given name</dd><dt>YY</dt><dd>Two-digit year of birth</dd><dt>M</dt><dd>Alphabetic character that indicates the person's month of birth.</dd><dt>DD</dt><dd>Two-digit day of birth. Also encodes the person's gender, where 0-31 is used for males and 41-71 (day +40) is used for females.</dd><dt>LLLL</dt><dd>Four-character Belfiore code that indicates the person's comune (town) of birth. The code consists of one alphabetic character followed by three digits. Persons who were born in a foreign country will have a code starting with Z and a three-digit code indicating the country of birth.</dd><dt>C</dt><dd>Check character generated from the preceding 15 characters</dd></dl> |
| Example values: | <dl><dt>MRTMTT91D08F205J</dt><dd>Matteo Moretti (male), born in Milan on 8 April 1991 (example from wikipedia): surname characters = MRT, given name characters = MTT, year of birth = 91, month of birth = D, day of birth = 01, gender = male, comune of birth = F205, check character = J</dd><dt>MLLSNT82P65Z404U</dt><dd>Samantha Miller (female), born in the USA on 25 September 1982, living in Italy (example from wikipedia): surname characters = MLL, given name characters = SNT, year of birth = 82, month of birth = P, day of birth = 65 (actual day of birth = 25), gender = female, comune of birth = Z404, check character = U</dd><dt>RSSNTN86H08G2NST</dt><dd>example of omocodia with the two right-most digits replaced by letter equivalents: surname characters = RSS, given name characters = NTN, year of birth = 86, month of birth = H, day of birth = 08, gender = male, comune of birth = G2NS (actual comune of birth = G226), check character = T</dd></dl> |

### Validation rules
| Rule | Description | Error Result Type |
| :--- | :---------- | :---------------- |
| 1. | The string value may not be null, String.Empty or all whitespace characters | EmptyValue |
| 2. | The string length must be 16 | InvalidLength |
| 3. | All characters must be upper-case or lower-case letters ('A'-'Z', 'a'-'z') or ASCII digits ('0'-'9') | InvalidCharacter |
| 4. | The check character, position 15 (zero-based) must be a valid upper-case or lower-case alphabetic ('A'-'Z', 'a'-'z') modulus 26 check character | InvalidChecksum |
| 5. | The surename characters, positions 0-2 (zero-based), must be upper-case or lower-case alphabetic characters ('A'-'Z', 'a'-'z') | InvalidSurname |
| 6. | The given name characters, positions 3-5 (zero-based), must be upper-case or lower-case alphabetic characters ('A'-'Z', 'a'-'z') | InvalidGivenName |
| 7. | The year of birth, character positions 6-7 (zero-based), must be ASCII digits ('0'-'9') or the equivalent Omocodia letter (see below). | InvalidYear |
| 8. | The month of birth, character position 8 (zero-based) must be an upper-case or lower-case alphabetic character. Valid month characters are "ABCDEHLMPRST", where 'A' = January and 'T' = December | InvalidMonth |
| 9. | The day of birth, character positions 9-10 (zero-based) must be two ASCII digits ('0'-'9') the or the equivalent Omocodia letter (see below). The integer value must be between 01-31 for males and 61-91 for females. The integer value must also be valid for the year/month | InvalidDay |
| 10. | The comune of birth, character positions 11-14 (zero-based) must be an upper-case or lower-case alphabetic character ('A'-'Z', 'a'-'z') followed by three ASCII digits ('0'-'9') the equivalent Omocodia letter (see below) | InvalidLocationCode |

### Additional Properties

| Name | Description |
| :--- | :---------- |
| BelfioreCode | Gets the four character Belfiore code that indicates the person's comune of birth encoded in character positions 11-14 (zero-based) |
| Gender | Gets the person's gender, as encoded in the day of birth |

### Additional Methods

| Name | Description |
| :--- | :---------- |
| GetDateOfBirth | Extract the encoded date of birth  |

### Notes

ItCodiceFiscale is case-insensitive for validation and parsing purposes. The ItCodiceFiscale constructor, Create method and explicit string to ItCodiceFiscale operator will normalize any lowercase letters to uppercase. Equality and inequality comparisons between instances of ItCodiceFiscale will compare the normalized uppercase versions of the value.

ItCodiceFiscale does not validate the comune of birth (the Belfiore code) against a comprehensive list of valid values (because the list of values exceeds 8000 entries). ItCodiceFiscale only validates the format, one alphabetic character followed by three digit characters.

While rare, it is possible that two different people can generate the same tax code, a situation referred to as "omocodia" (or "same code"). The Agenzia delle Entrate handles omocodia by progressively replacing digits with a letter equivalent, starting at the right-most digit and progressing leftward until a unique code is generated. After a unique code is generated, the check character is calculated for the unique code. The letter substitutions are "LMNPQRSTUV", where 0 = L and 9 = V. See [CodiceFiscale.expert](https://codicefiscale.expert/en/omocodia) for a full description of the handling of omocodia.

If a value contains an omocidia substitution, the substituted letters are converted to the equivalent digits before validation or retrieving the DateOfBirth, Gender or Belfiore code.

ItCodiceFiscale uses a custom modulus 26 check digit algorithm. Refer to the English Wikipedia article below for a full description of the algorithm. The custom algorithm has some weaknesses described in the Italian Wikipedia article below. For example, the algorithm cannot two character jump transpositions (i.e. DEF => FED) nor transpositions of the characters WY (i.e. WY => YW).

### References:

[Wikipedia - Italian fiscal code](https://en.wikipedia.org/wiki/Italian_fiscal_code)

[Wikipedia (Italian) - Codice fiscale](https://it.wikipedia.org/wiki/Codice_fiscale)
