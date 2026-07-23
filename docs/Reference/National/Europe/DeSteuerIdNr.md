## DeSteuerIdNr

German tax identification number (Steuerliche Identifikationsnummer or Steuer-IdNr).

| Element | Description |
| :------ | :---------- |
| Class name: | KfAccountNumbers.National.Europe.DeSteuerIdNr |
| Is composite: | No |
| Length: | 11 |
| Check digit algorithm: | ISO/IEC 7064 MOD 11,10 |
| Allowed characters: | ASCII digits ('0'-'9') |
| Allowed separator characters: | Typically a space (' '), though any non digit character is allowed |
| Structure: | ***DDDDDDDDDDC*** (unformatted) or ***DD DDD DDD DDC*** (formatted), where: <dl><dt>DDDDDDDDDD</dt><dd>Ten random digits</dd><dt>C</dt><dd>Check digit generated using the ISO/IEC 7064, MOD 11,10 algorithm</dd></dl> |
| Example values: | <dl><dt>43957380212</dt><dd>unformatted</dd><dt>25 986 078 148</dt><dd>formatted</dd><dt>91 215 743 612</dt><dd>formatted</dd></dl> |

### Validation rules
| Rule | Description | Error Result Type |
| :--- | :---------- | :---------------- |
| 1. | The string value may not be null, String.Empty or all whitespace characters. | EmptyValue |
| 2. | The string length must be 11 characters (unformatted) or 14 characters (formatted). | InvalidLength |
| 3. | All non-separator characters must be ASCII digits ('0'-'9'). | InvalidCharacter |
| 4. | The trailing character must be a valid ISO/IEC 7064, MOD 11,10 check digit. | InvalidChecksum |
| 5. | If the value has length 14, then characters at positions 2, 6 and 10 (zero-based) must not be ASCII digits ('0'-'9') and all separator positions must be the same character | InvalidSeparator |

### Notes

A Steuer-IdNr does not encode any personal information.

### References

[Wikipedia (German) - Steuerliche Identifikationsnummer](https://de.wikipedia.org/wiki/Steuerliche_Identifikationsnummer)

