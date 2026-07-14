## ItCodiceFiscale

Italian tax identifier assigned to individuals by the Italian tax office, the Agenzia delle Entrate.

| Element | Description |
| :------ | :---------- |
| Class name: | KfAccountNumbers.National.Europe.ItCodiceFiscale |
| Is composite: | No |
| Length: | 16 |
| Check digit algorithm: | Weighted modulus 11, with two different sets of weights |
| Allowed characters: | Alphanumeric ('0'-'9', 'A-Z', 'a-z') |
| Structure: | ***SSSGGGYYMDDLLLLC*** (formatted), where: <dl><dt>SSS</dt><dd>Three characters from the person's surname</dd><dt>GGG</dt><dd>Three characters from the person's given name</dd><dt>YY</dt><dd>2-digit year of birth</dd><dt>M</dt><dd>Alphabetic character that indicates the person's month of birth.</dd><dt>DD</dt><dd>Two digit day of birth. Also used to indicate the person's gender where 0-31 indicates a male and 41-71 indicates a female</dd><dt>LLLL</dt><dd>Four character Belifore code that indicates the person's town of birth. The code consists of one alphabetic character followed by three digits. Persons who were born in a foreign country will have a code starting with Z and a three-digit code indicating the country of birth.</dd><dt>C</dt><dd>Check character generated from the preceding 15 characters</dd></dl> |
| Example values: | <dl><dt>MRTMTT91D08F205J</dt><dd>Matteo Moretti (male), born in Milan on 8 April 1991 (example from wikipedia): surname characters = MRT, given name characters = MTT, year of birth = 91, month of birth = D, day of birth = 01, gender = male, town of birth = F205, check character = J</dd><dt>MLLSNT82P65Z404U</dt><dd>Samantha Miller (female), born in the USA on 25 September 1982, living in Italy (example from wikipedia): surname characters = MLL, given name characters = SNT, year of birth = 82, month of birth = P, day of birth = 65, gender = female, town of birth = Z404, check character = U</dd><dt>HYSMHL80H05A203Y</dt><dd>Michael Hays (male), born in Aliminusa on 6 May 1980: surname characters = HYS, given name characters = MHL, year of birth = 80, month of birth = H, day of birth = 05, gender = male, town of birth = A203, check character = Y</dd></dl> |

### Validation rules
| Rule | Description | Error Result Type |
| :--- | :---------- | :---------------- |
| 1. | The string value may not be null, String.Empty or all whitespace characters | EmptyValue |
| 2. | The string length must be 16 | InvalidLength |
| 3. | Character positions 0-2 (zero-based) must be upper-case or lower-case alphabetic characters ('A'-'Z', 'a'-z') | InvalidSurname |
| 4. | Character positions 3-5 (zero-based) must be upper-case or lower-case alphabetic characters ('A'-'Z', 'a'-z') | InvalidGivenName |
| 5. | Character positions 6-7 (zero-based) must be ASCII digits ('0'-'9') | InvalidYear |
| 6. | Character position 8 (zero-based) must be an upper-case or lower-case alphabetic character that indicates the month of birth. Valid month characters are "ABCDEHLMPRST", where 'A' = January and 'T' = December | InvalidMonth |
| 7. | Character positions 9-10 (zero-based) must be two ASCII digits ('0'-'9') that indicate the month of birth and gender. Male = 01-31 and female = 61-91 | InvalidDay |
| 7. | Character positions 11-14 (zero-based) must be an upper-case or lower-case alphabetic character ('A'-'Z', 'a'-z') followee by three ASCII digits ('0'-'9') that indicate the town of birth | InvalidLocationCode |
| 8. | Character position 15 must be a valid upper-case or lower-case alphanumeric ('0'-'9', 'A'-'Z', 'a'-z') check character | InvalidChecksum |

