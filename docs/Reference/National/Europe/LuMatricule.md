## LuMatricule

Luxembourg National Identification Number (numéro d'identification), commonly called the matricule.

| Element | Description |
| :------ | :---------- |
| Class name: | KfAccountNumbers.National.Europe.LuMatricule |
| Is composite: | No |
| Length: | 13 |
| Check digit algorithm: | Luhn, Verhoeff |
| Allowed characters: | ASCII digits ('0'-'9') |
| Structure: | ***YYYYMMDDXXXLV***, where: <dl><dt>YYYYMMDD</dt><dd>Eight digit date of birth in YYYYMMDD format</dd><dt>XXX</dt><dd>Three digits used to differentiate between persons born on the same day</dd><dt>L</dt><dd>Check digit calculated using the Luhn algorithm on the initial 11 digits</dd><dt>V</dt><dd>Check digit calculated using the Verhoeff algorithm on the initial 11 digits</dd></dl> |
| Example values: | <dl><dt>1960090900163</dt><dd>date of birth = September 9, 1960, Luhn checkdigit = 6, Verhoeff check digit = 3</dd><dt>1985011500173</dt><dd>date of birth = January 15, 1985, Luhn checkdigit = 7, Verhoeff check digit = 3</dd></dl> |

### Validation rules
| Rule | Description | Error Result Type |
| :--- | :---------- | :---------------- |
| 1. | The string value may not be null, String.Empty or all whitespace characters. | EmptyValue |
| 2. | The string length must be 13 characters. | InvalidLength |
| 3. | All characters must be ASCII digits ('0'-'9'). | InvalidCharacter |
| 4. | The character at position 11 (zero-based) must be a valid Luhn algorithm check digit. | InvalidChecksum |
| 5. | The character at position 12 (zero-based) must be a valid Verhoeff algorithm check digit. | InvalidChecksum |
| 6. | The leading eight characters must be a valid date of birth in YYYYMMDD format. | InvalidDateOfBirth |

### Additional Properties

| Name | Description |
| :--- | :---------- |
| DateOfBirth | Gets the person's date of birth, encoded in the leading eight digits of the value. |

### Notes

Both check digits are calculated independently on the same 11 initial digits, with the Verhoeff algorithm not including the Luhn result in its calculation. The use of two different check digit algorithms results in greater error detection capability than either algorithm alone. 

### References

[Luxembourg Business Registers - Frequently Asked Questions](https://www.lbr.lu/mjrcs/jsp/webapp/static/mjrcs/en/mjrcs/pdf/FAQ_National_Identification_Number.pdf)

[Luxembourg Tax ID Guide - Matricule, No. TVA & RCS Number Explained](https://lookuptax.com/docs/tax-identification-number/luxembourg-tax-id-guide)
