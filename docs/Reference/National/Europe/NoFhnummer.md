## NoFhnummer

A Norwegian Fh-nummer (Felles Hjelpenummer or Common Help Number) is similar to a Norwegian H-nummer, an identifier issued to persons needing medical assistance and who do not have a fødselsnummer or a D-nummer such as tourists, newborns or persons with unknown identities. However, while H-nummers are issued by a single organization and only unique within that organization, Fh-nummers are issued by Norsk Helsenett (the Norwegian Health Network) and are unique across the entire Norwegian health system. Unlike other Norwegian identity numbers such as fødselsnummer, D-nummer and H-nummer, Fh-nummers do not encode the person's date of birth or gender and consist of 9 random digits and two check digits. Fh-nummers are distinguished by an initial digit = 8 or 9.

| Element | Description |
| :------ | :---------- |
| Class name: | KfAccountNumbers.National.Europe.NoFhnummer |
| Is composite: | No |
| Length: | 11 (unformatted), 12 (formatted for readability) |
| Check digit algorithm: | Weighted modulus 11, with two different sets of weights |
| Allowed characters: | Digits ('0'-'9') |
| Allowed separator characters: | Typically a space (' '), though any non digit character is allowed |
| Structure: | ***NNNNNNNNNCC*** (unformatted) or ***NNNNNN NNNCC*** (formatted), where: <dl><dt>NNNNNNNNN</dt><dd>9 random digits. The first digit must be 8 or 9</dd><dt>CC</dt><dd>2 weighted modulus 11 check digits, each calculated with different weights</dd></dl> |
| Example values: | <dl><dt>98075450605</dt><dd>unformatted, check digits = 05</dd><dt>87207009367</dt><dd>unformatted, check digits = 67</dd><dt>809390 27371</dt><dd>formatted, check digits = 71</dd></dl> |

### Validation rules
| Rule | Description | Error Result Type |
| :--- | :---------- | :---------------- |
| 1. | The string value may not be null, String.Empty or all whitespace characters. | EmptyValue |
| 2. | The string length must be 11 characters (unformatted) or 12 characters (formatted). | InvalidLength |
| 3. | All non-separator characters must be ASCII digits ('0'-'9'). | InvalidCharacter |
| 4. | The trailing two characters must be valid weighted modulus 11 check digits. | InvalidChecksum |
| 5. | If the value has length 12, then the character at position 6 (zero-based) must not be an ASCII digit ('0'-'9') | InvalidSeparator |

### Notes

The first check digit is calculated from the preceding 9 digits. The second check digit character is calculated from the preceding 10 digits (including the first check digit).
