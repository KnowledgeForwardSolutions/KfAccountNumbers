## EsNif

Spanish tax identifier, Número de Identificación Fiscal (NIF).

| Element | Description |
| :------ | :---------- |
| Class name: | EsNif |
| Is composite: | No |
| Length: | 9 unformatted, 10 formatted |
| Check digit algorithm: | Modulus 23 |
| Allowed characters: | Digits ('0'-'9') and upper-case or lower-case alphabetic characters, except 'I', 'O', 'U' and 'i', 'o', 'u' |
| Allowed separator characters | Any non-ASCII digit ('0'-'9'). The typical separator character is a dash ('-'). |
| Structure: | ***DDDDDDDDC*** (unformatted) or ***DDDDDDDD-C*** (formatted), where: <dl><dt>DDDDDDDD</dt><dd>Eight integer digits</dd><dt>-</dt><dd>Separator character</dd><dt>C</dt><dd>Modulus 23 check character</dd></dl> |
| Additional properties: | <dl><dt>Value</dt><dd>Gets the unformatted 9-character value, normalized to upper-case.</dd></dl> |
| Additional methods: | <dl><dt>Format</dt><dd>Formats the value according to the supplied format mask.</dd><dt>ToString</dt><dd>Converts the instance to a string.</dd></dl> |
| Example values: | <ul><li>12345678Z - Unformatted</li><li>50487563-X - Formatted</li></ul>  |

### Validation rules
| Rule | Description | Error Result Type |
| :--- | :---------- | :---------------- |
| 1. | The string value may not be null, String.Empty or all whitespace characters. | EmptyValue |
| 2. | The string length must be the unformatted length (9 characters) or formatted lengh (10 characters). | InvalidLength |
| 3. | The leading eight characters must all be ASCII digits ('0'-'9') and the trailing character must be an alphabetic character from the subset allowed by the Modulus 23 algorithm (TRWAGMYFPDXBNJZSQVHLCKE). | InvalidCharacter |
| 4. | The trailing character must be a valid Modulus 23 check character. | InvalidChecksum |
| 5. | If the string length equals the formatted length (10 characters), the eigth character (zero-based) is considered a separator character and may not be an ASCII digit ('0'-'9') | InvalidSeparator |

### Notes:

The Modulus 23 algorithm calculates the modulus 23 of the leading eight digits and the remainder from 0 to 22 is used to generate a check character from the string "TRWAGMYFPDXBNJZSQVHLCKE", where a remainder of 0 equals 'T' and a remainder of 22 equals 'E'. The trailing character of the input value must be equal to the generated check character.

### References:

[Wikipedia - National Identity Card (Spain)](https://en.wikipedia.org/wiki/National_Identity_Card_%28Spain%29)

[Wikipedia (Spanish) - Número de identificación fiscal](https://es.wikipedia.org/wiki/N%C3%BAmero_de_identificaci%C3%B3n_fiscal)
