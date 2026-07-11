## NoHnummer

Temporary identity number issued by Norwegian health organizations (such as a hospital) to persons needing medical assistance and who do not have either a fødselsnummer or a D-nummer. A H-nummer is unique only within the issuing organization. A H-nummer is distinguished by a +40 offset added to the month component of the date of birth (1-12 becomes 41-52).

| Element | Description |
| :------ | :---------- |
| Class name: | KfAccountNumbers.National.Europe.NoHnummer |
| Is composite: | No |
| Length: | 11 (unformatted), 12 (formatted for readability) |
| Check digit algorithm: | Weighted modulus 11, with two different sets of weights |
| Allowed characters: | Digits ('0'-'9') |
| Allowed separator characters: | Typically a space (' '), though any non digit character is allowed |
| Structure: | ***DDMMYYIIICC*** (unformatted) or ***DDMMYY IIICC*** (formatted), where: <dl><dt>DDMMYY</dt><dd>6-digit date of birth in DDMMYY format. Note that the <b>MM</b> portion of the date of birth will be 41-52 because H-nummers offset the month of birth by +40 to distinguish from fødselsnummer values</dd><dt>III</dt><dd>3-digit individual number used to distinguish between persons born on the same date. The first digit indicates the century of the person's birth (0-4 = 20th century or 1900-1999 and 5-9 = 21st century or 2000-2099). The last digit indicates the person's gender, with odd numbers = male and even numbers = female</dd><dt>CC</dt><dd>2 weighted modulus 11 check digits, each calculated with different weights</dd></dl> |
| Example values: | <dl><dt>07417942720</dt><dd>unformatted, date of birth = January 7, 1979, gender = male, check digits = 20</dd><dt>21501350017</dt><dd>unformatted, date of birth = October 21, 2013, gender = female, check digits = 17</dd><dt>135095 02069</dt><dd>formatted, date of birth = October 13, 1995, gender = female, check digits = 69</dd></dl> |

### Validation rules
| Rule | Description | Error Result Type |
| :--- | :---------- | :---------------- |
| 1. | The string value may not be null, String.Empty or all whitespace characters. | EmptyValue |
| 2. | The string length must be 11 characters (unformatted) or 12 characters (formatted). | InvalidLength |
| 3. | All non-separator characters must be ASCII digits ('0'-'9'). | InvalidCharacter |
| 4. | The trailing two characters must be valid weighted modulus 11 check digits. | InvalidChecksum |
| 5. | If the value has length 12, then the character at position 6 (zero-based) must not be an ASCII digit ('0'-'9') | InvalidSeparator |
| 6. | The date of birth (after adjusting for the +40 H-nummer month offset and after determining the century from the individual number) must be a valid date between 01/01/1854 and 31/12/2039 | InvalidDateOfBirth |

### Additional Properties

| Name | Description |
| :--- | :---------- |
| DateOfBirth | Gets the person's date of birth, derived from the 6-digit date of birth and the first digit of the individual number |
| Gender | Gets the person's gender, as encoded in the third digit of the individual number |

### Notes

The first check digit is calculated from the preceding 9 digits (the 6-digit date of birth and the 3-digit individual number). The second check digit character is calculated from the preceding 10 digits (including the first check digit).

The H-nummer's +40 month offset is taken into account when creating or validating a value and the actual date is validated, not the offset date. Similarly, the DateOfBirth property will return the actual date and not an offset date.

Note that fødselsnummers use the individual number to determine the century of birth, but the rules are more complicated. Refer to the fødselsnummer documentation for more detail.
