## BeBisnummer

A BIS-nummer (or numéro bis) is the identifier issued to non-residents and individuals who are not recorded in Belgium's National Register (Rijksregister/Registre national).

BIS-nummers apply an offset to the month portion of the date of birth element to distinguish them from rijksregisternummers issued to Belgian citizens and permanent residents.

| Element | Description |
| :------ | :---------- |
| Class name: | KfAccountNumbers.National.Europe.BeBisnummer |
| Is composite: | No |
| Composite parent: | BeIdentityNumber |
| Length: | 11 (unformatted), 15 (formatted for readability) |
| Check digit algorithm: | Modulus 97 |
| Allowed characters: | Digits ('0'-'9') |
| Allowed separator characters: | Typically a period ('.') and a dash ('-'), though any non digit character is allowed |
| Structure: | ***YYMMDDXXXCC*** (unformatted) or ***YY.MM.DD-XXX.CC*** (formatted), where: <dl><dt>YYMMDD</dt><dd>6-digit date of birth in YYMMDD format. Note that the <b>MM</b> portion of the date of birth will be 21-32 or 41-52 because BIS-nummers offset the month of birth by either +20 or +40 to distinguish from rijksregisternummer values. The date of birth may be unknown/incomplete and in that case zeros are used in place of the unknown elements.</dd><dt>XXX</dt><dd>3-digit sequence number used to distinguish between persons born on the same date. The last digit indicates the person's gender, with odd numbers = male and even numbers = female. (See below for exception when gender is unknown.)</dd><dt>CC</dt><dd>Two digit modulus 97 check sum calculated for the YYMMDD and XXX elements. The check sum is also used to indicate century of birth. If CC is equal to the normal modulus 97 check sum then the person's century of birth is 1900-1999. If CC is equal to the modulus 97 check sum calculated by first prefixing YYMMDDXXX with the digit 2 (i.e. 2YYMMDDXXX) then the person's century of birth is 2000-2099.</dd></dl> |
| Example values: | <dl><dt>17.51.08-046.40</dt><dd>formatted, date of birth November 8, 1917, gender = female, check digit calculation 97 - (175108046 mod 97) = 97 - 57 = 40</dd><dt>09200000265</dt><dd>unformatted, date of birth incomplete, year of birth 2009, gender unknown, check digit calculation 97 - (2092000002 mod 97) = 97 - 32 = 65</dd></dl> |

### Validation rules
| Rule | Description | Error Result Type |
| :--- | :---------- | :---------------- |
| 1. | The string value may not be null, String.Empty or all whitespace characters. | EmptyValue |
| 2. | The string length must be 11 characters (unformatted) or 15 characters (formatted). | InvalidLength |
| 3. | All non-separator characters must be ASCII digits ('0'-'9'). | InvalidCharacter |
| 4. | The two trailing (right-most) characters must be a valid modulus 97 check sum (taking into account the possibility of a person born in the year 2000 or later). | InvalidChecksum |
| 5. | If the value has length 15, then the characters at positions 2, 5, 8 and 12 (zero-based) must not be ASCII digits ('0'-'9') | InvalidSeparator |
| 6. | The date of birth, after deriving the century of birth from the check sum and taking into account the BIS number offset, must be a valid date between January 1, 1900 and December 31, 2099. <br><b>OR</b><br> The date of birth may use zeros to indicate that some or all of the person's date of birth is unknown (see below for more details). | InvalidDateOfBirth |
| 7. | The sequence number may not be 000 or 999. | InvalidBeRijksregisternummerSequenceNumber |

### Additional Properties

| Name | Description |
| :--- | :---------- |
| DateOfBirth | Gets the person's date of birth, derived from the first six digits in YYMMDD format and the exact century of birth derived from the check digits. May be incomplete due to the person's exact date of birth being unknown when the BIS-nummer was issued. |
| Gender | Gets the person's gender, as encoded in the third digit of the sequence number or None if the person's gender was unknown when the BIS-nummer was issued. |

### Notes

The date of birth can be adjusted in a variety of ways:
* If the person's date of birth is incomplete, then the two digit year is used and zeros are used for
  month and year (for example, 40.00.00-955.69).
* If there are too many people with incomplete dates of birth for a particular year than can be represented by a three digit sequence number (i.e. more than 499 males with incomplete dates of birth for the year 1940), then 01 is used for the day of birth and the sequence number rolls over to 001 (ex. 40.00.01-001.33). (Note that `BeBisnummer` does not enforce an upper limit on the day component in cases of rollover, though multiple rollovers in a single year should be rare.)
* If the person's date of birth is unknown, then the constant 00.00.01 is used.
* As noted above, BIS-nummers apply an offset to the month component of the date of birth to distinguish them from rijksregisternummers. If the person's gender is known when the BIS-nummer is issued, then **40** is added to the month; Otherwise **20** is added to the month.

For cases of a person with an incomplete or unknown date of birth, `BeBisnummer`
stacks the appropriate rules. For example, 87.40.00-023.47 would be the BIS number for a person with an incomplete date of birth born in 1987.

### References

[Wikipedia (French) - Numéro de registre national](https://fr.wikipedia.org/wiki/Num%C3%A9ro_de_registre_national)
