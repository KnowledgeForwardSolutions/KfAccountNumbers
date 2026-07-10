## NoFoedselsnummer

Norwegian national identity number issued to citizens and long-term residents of Norway.

| Element | Description |
| :------ | :---------- |
| Class name: | KfAccountNumbers.National.Europe.NoFoedselsnummer |
| Is composite: | No |
| Length: | 11 (unformatted), 12 (formatted for readability) |
| Check digit algorithm: | Weighted modulus 11, with two different sets of weights |
| Allowed characters: | Digits ('0'-'9') |
| Allowed separator characters: | Typically a space (' '), though any non digit character is allowed |
| Structure: | ***DDMMYYIIICC*** (unformatted) or ***DDMMYY IIICC*** (formatted), where: <dl><dt>DDMMYY</dt><dd>6-digit date of birth in DDMMYY format.</dd><dt>III</dt><dd>3-digit individual number used to distinguish between persons born on the same date. The first digit indicates the century of the person's birth (see rules below). The last digit indicates the person's gender, with odd numbers = male and even numbers = female</dd><dt>CC</dt><dd>2 weighted modulus 11 check digits, each calculated with different weights</dd></dl> |
| Example values: | <dl><dt>13029597140</dt><dd>unformatted, date of birth = February 13, 1995, gender = male, check digits = 40</dd><dt>20050559433</dt><dd>unformatted, date of birth = May 20, 2005, gender = female, check digits = 33</dd><dt>130682 27938</dt><dd>formatted, date of birth = June 13, 1982, gender = male, check digits = 38</dd></dl> |

### Validation rules
| Rule | Description | Error Result Type |
| :--- | :---------- | :---------------- |
| 1. | The string value may not be null, String.Empty or all whitespace characters. | EmptyValue |
| 2. | The string length must be 11 characters (unformatted) or 12 characters (formatted). | InvalidLength |
| 3. | All non-separator characters must be ASCII digits ('0'-'9'). | InvalidCharacter |
| 4. | The trailing two characters must be valid weighted modulus 11 check digits. | InvalidChecksum |
| 5. | If the value has length 12, then character position 6 (zero-based) must not be an ASCII digit ('0'-'9') | InvalidSeparator |
| 6. | The date of birth must be a valid date between 01/01/1854 and 31/12/2039 | InvalidDateOfBirth |

### Additional Properties

| Name | Description |
| :--- | :---------- |
| DateOfBirth | Gets the person's date of birth, derived from the 6-digit date of birth and the first digit of the individual number |
| Gender | Gets the person's gender, as encoded in the third digit of the individual number |

### Notes

The first check digit is calculated from the preceeding 9 digits (the 6-digit date of birth and the 3-digit individual number). The second check digit character is calculated from the preceeding 10 digits (including the first check digit).

Fødselsnummers have fairly complex rules for determining the century of birth due to additional requirements being layered upon the individual number element over time. NoFoedselsnummer uses the rules described in this [article](https://blog.variant.no/ssns-and-pattern-matching-in-c-9-498f96aa71d4) published on Medium.com. The rules
are:

* Rule 1 - If the individual number is >= 500 and <= 749 **AND** the two digit year is >= 54 then the century = 1800.
* Rule 2 - If the individual number is < 500 then the century = 1900.
* Rule 3 - If the individual number is >= 900 **AND** the two digit year is >= 40 then the century = 1900.
* Rule 4 - If the individual number is >= 500 **AND** the two digit year is <= 39 then the century =2000.
* Rule 5 - Otherwise invalid. Validation will report an invalid date of birth.

The similar temporary identifier, the Norwegian D-nummer, uses much simpler rules to derive the century of birth.
