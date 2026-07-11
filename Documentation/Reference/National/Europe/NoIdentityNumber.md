## NoIdentityNumber

Composite type that represents any of several different Norwegian personal identity numbers: fødselsnummer, D-nummer, H-nummer and Fh-nummer. These identifiers have similar format and validation rules and are distinguished by the encoding of the person's date of birth.

A fødselsnummer is the Norwegian national identity number issued to citizens and long-term residents of Norway.

A D-nummer serves many of the same purposes of a fødselsnummer, but is issued to foreign individuals who are not eligible for a fødselsnummer. A D-nummer is distinguished by a +40 offset added to the day component of the date of birth (1-31 becomes 41-71).

A H-nummer is a temporary identifier issued by local health organizations (such as a hospital) to persons needing medical assistance and who do not have either a fødselsnummer or a D-nummer. A H-nummer is unique only within the issuing organization. A H-nummer is distinguished by a +40 offset added to the month component of the date of birth (1-12 becomes 41-52).

A Fh-nummer is similar to a H-nummer, an identifier issued to persons needing medical assistance and who do not have a fødselsnummer or a D-nummer. However, while H-nummers are issued by a single organization and only unique within that organization, Fh-nummers are issued by Norsk Helsenett (the Norwegian Health Network) and are unique across the entire Norwegian health system. Fh-nummers do not encode the person's date of birth or gender and are distinguished by an initial digit = 8 or 9.

| Element | Description |
| :------ | :---------- |
| Class name: | KfAccountNumbers.National.Europe.NoIdentityNumber |
| Is composite: | Yes |
| Composite subtypes: | NoFoedselsnummer, NoDnummer |
| Length: | 11 (unformatted), 12 (formatted for readability) |
| Check digit algorithm: | Weighted modulus 11, with two different sets of weights |
| Allowed characters: | Digits ('0'-'9') |
| Allowed separator characters: | Typically a space (' '), though any non digit character is allowed |
| Structure: | ***DDMMYYIIICC*** (unformatted) or ***DDMMYY IIICC*** (formatted), where: <dl><dt>DDMMYY</dt><dd>6-digit date of birth in DDMMYY format. Note that the date of birth can be altered by adding different offsets to the day or month to differentiate between different types of identifiers as described above.[^1]</dd><dt>III</dt><dd>3-digit individual number used to distinguish between persons born on the same date. Also used to determine the century of the person's birth, though the rules used by fødselsnummer are different from D-nummer and H-nummer. See below for more detail. The last digit indicates the person's gender, with odd numbers = male and even numbers = female</dd><dt>CC</dt><dd>2 weighted modulus 11 check digits, each calculated with different weights</dd></dl> |
| Example values: | <b>Fødselsnummer values:</b><dl><dt>13029597140</dt><dd>unformatted, date of birth = February 13, 1995, gender = male, check digits = 40</dd><dt>20050559433</dt><dd>unformatted, date of birth = May 20, 2005, gender = female, check digits = 33</dd><dt>130682 27938</dt><dd>formatted, date of birth = June 13, 1982, gender = male, check digits = 38</dd></dl><b>D-nummer values:</b><dl><dt>60055029566</dt><dd>unformatted, date of birth = May 20, 1950, gender = male, check digits = 66</dd><dt>70100567871</dt><dd>unformatted, date of birth = October 30, 2005, gender = female, check digits = 71</dd><dt>530295 34272</dt><dd>formatted, date of birth = February 13, 1995, gender = female, check digits = 72</dd></dl><b>H-nummer values:</b><dl><dt>07417942720</dt><dd>unformatted, date of birth = January 7, 1979, gender = male, check digits = 20</dd><dt>21501350017</dt><dd>unformatted, date of birth = October 21, 2013, gender = female, check digits = 17</dd><dt>135095 02069</dt><dd>formatted, date of birth = October 13, 1995, gender = female, check digits = 69</dd></dl> |

### Validation rules
| Rule | Description | Error Result Type |
| :--- | :---------- | :---------------- |
| 1. | The string value may not be null, String.Empty or all whitespace characters. | EmptyValue |
| 2. | The string length must be 11 characters (unformatted) or 12 characters (formatted). | InvalidLength |
| 3. | All non-separator characters must be ASCII digits ('0'-'9'). | InvalidCharacter |
| 4. | The trailing two characters must be valid weighted modulus 11 check digits. | InvalidChecksum |
| 5. | If the value has length 12, then the character at position 6 (zero-based) must not be an ASCII digit ('0'-'9') | InvalidSeparator |
| 6. | If the value is a fødselsnummer, D-nummer or H-nummer, the date of birth (after adjusting for identifier specific offsets and after determining the century from the individual number) must be a valid date between 01/01/1854 and 31/12/2039 | InvalidDateOfBirth |

### Additional Properties

| Name | Description |
| :--- | :---------- |
| IdentifierType | Gets the specific type of identifier that this instance represents |

### Additional Methods

| Name | Description |
| :--- | :---------- |
| ToDnummer | Convert this instance to a NoDnummer |
| ToFoedselsnummer | Convert this instance to a NoFoedselsnummer |
| ToHnummer | Convert this instance to a NoHnummer |

### Notes

[^1]: For D-nummers, +40 is added to the day, so 01-31 becomes 41-71.

The first check digit is calculated from the preceding 9 digits (the 6-digit date of birth and the 3-digit individual number). The second check digit character is calculated from the preceding 10 digits (including the first check digit).

If the value is a D-nummer, the D-nummer's +40 day offset is taken into account when creating or validating a value and the actual date is validated, not the offset date. Similarly, for H-nummers, the +40 month offset is taken into account when creating or validating a value.

The century of birth is encoded in the individual number and when combined with the 6-digit date of birth can determine an actual date of birth. The rules for determining the century of birth vary by the type of identifier.

Fødselsnummers have fairly complex rules for determining the century of birth due to additional requirements being layered upon the individual number element over time. NoIdentityNumber and NoFoedselsnummer use the rules described in this [article](https://blog.variant.no/ssns-and-pattern-matching-in-c-9-498f96aa71d4) published on Medium.com. The rules
are:

* Rule 1 - If the individual number is >= 500 and <= 749 **AND** the two digit year is >= 54 then the century = 1800.
* Rule 2 - If the individual number is < 500 then the century = 1900.
* Rule 3 - If the individual number is >= 900 **AND** the two digit year is >= 40 then the century = 1900.
* Rule 4 - If the individual number is >= 500 **AND** the two digit year is <= 39 then the century =2000.
* Rule 5 - Otherwise invalid. Validation will report an invalid date of birth.

For D-nummers and H-nummers, the first digit of the individual number indicates the century of the person's birth (0-4 = 20th century or 1900-1999 and 5-9 = 21st century or 2000-2099).
