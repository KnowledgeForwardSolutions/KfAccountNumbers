## ChSozialversicherungsnummer

Swiss Social Security Number (Sozialversicherungsnummer or Neue AVH Nummer).

| Element | Description |
| :------ | :---------- |
| Class name: | KfAccountNumbers.National.Europe.ChSozialversicherungsnummer |
| Is composite: | No |
| Length: | 13 |
| Check digit algorithm: | EAN-13 |
| Allowed characters: | ASCII digits ('0'-'9') |
| Allowed separator characters: | Typically a period ('.'), though any non digit character is allowed |
| Structure: | ***756XXXXXXXXXY*** (unformatted) or ***756.XXXX.XXXX.XY***, where: <dl><dt>756</dt><dd>Constant "756", the ISO 3166-1 code for Switzerland</dd><dt>XXXXXXXXX or XXXX.XXXX.X</dt><dd>Nine random digits</dd><dt>Y</dt><dd>EAN-13 check digit</dd></dl> |
| Example values: | <dl><dt>7560850652826</dt><dd>unformatted, check digit = 6</dd><dt>756.8814.3009.98</dt><dd>formatted check digit = 8</dd></dl> |

### Validation rules
| Rule | Description | Error Result Type |
| :--- | :---------- | :---------------- |
| 1. | The string value may not be null, String.Empty or all whitespace characters. | EmptyValue |
| 2. | The string length must be 13 characters (unformatted) or 16 characters (formatted). | InvalidLength |
| 3. | All non-separator characters must be ASCII digits ('0'-'9'). | InvalidCharacter |
| 4. | The trailing character must be a valid EAN-13 check digit. | InvalidChecksum |
| 5. | If the value has length 16, then characters at positions 3, 8 and 13 (zero-based) must not be ASCII digits ('0'-'9') and all separator positions must be the same character | InvalidSeparator |
| 6. | The leading three digits must be 756 | InvalidPrefix |

### Notes

Sozialversicherungsnummer does not encode any personal information.

### References

[Wikipedia - National identification number - Switzerland](https://en.wikipedia.org/wiki/National_identification_number#Switzerland)

[Wikipedia (German) - Sozialversicherungsnummer](https://de.wikipedia.org/wiki/Sozialversicherungsnummer#Versichertennummer)
