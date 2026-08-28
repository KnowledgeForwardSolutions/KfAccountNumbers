## BeRijksregisternummer

A Belgian National Register Number (Rijksregisternummer in Dutch, Numéro de registre national in French) is a unique identifier assigned to all persons (Belgian citizens and foreign residents) who are registered in Belgium's National Register (Rijksregister/Registre national).

| Element | Description |
| :------ | :---------- |
| Class name: | KfAccountNumbers.National.Europe.BeRijksregisternummer |
| Is composite: | No |
| Composite parent: | [BeIdentityNumber](https://github.com/KnowledgeForwardSolutions/KfAccountNumbers/blob/main/docs/Reference/National/Europe/BeIdentityNumber.md) |
| Length: | 11 (unformatted), 15 (formatted for readability) |
| Check digit algorithm: | Modulus 97 |
| Allowed characters: | Digits ('0'-'9') |
| Allowed separator characters: | Typically a period ('.') and a dash ('-'), though any non digit character is allowed |
| Structure: | ***YYMMDDXXXCC*** (unformatted) or ***YY.MM.DD-XXX.CC*** (formatted), where: <dl><dt>YYMMDD</dt><dd>6-digit date of birth in YYMMDD format. The date of birth may be unknown/incomplete and in that case zeros are used in place of the unknown elements.</dd><dt>XXX</dt><dd>3-digit sequence number used to distinguish between persons born on the same date. The last digit indicates the person's gender, with odd numbers = male and even numbers = female.</dd><dt>CC</dt><dd>Two digit modulus 97 check sum calculated for the YYMMDD and XXX elements. The check sum is also used to indicate century of birth. If CC is equal to the normal modulus 97 check sum then the person's century of birth is 1900-1999. If CC is equal to the modulus 97 check sum calculated by first prefixing YYMMDDXXX with the digit 2 (i.e. 2YYMMDDXXX) then the person's century of birth is 2000-2099.</dd></dl> |
| Example values: | <dl><dt>87092100294</dt><dd>unformatted, date of birth September 21, 1987, gender = female, check digit calculation 97 - (870921002 mod 97) = 97 - 3 = 94</dd><dt>05.03.11-017.02</dt><dd>formatted, date of birth March 11, 2005, gender = male, check digit calculation 97 - (050311017 mod 97) = 97 - 95 = 2</dd><dt>55000007612</dt><dd>unformatted, date of birth 1955, day/month unknown, gender = female, check digit calculation 97 - (550000076 mod 97) = 97 - 85 = 12</dd></dl> |

### Validation rules
| Rule | Description | Error Result Type |
| :--- | :---------- | :---------------- |
| 1. | The string value may not be null, String.Empty or all whitespace characters. | EmptyValue |
| 2. | The string length must be 11 characters (unformatted) or 15 characters (formatted). | InvalidLength |
| 3. | All non-separator characters must be ASCII digits ('0'-'9'). | InvalidCharacter |
| 4. | The two trailing (right-most) characters must be a valid modulus 97 check sum (taking into account the possibility of a person born in the year 2000 or later). | InvalidChecksum |
| 5. | If the value has length 15, then the characters at positions 2, 5, 8 and 12 (zero-based) must not be ASCII digits ('0'-'9') | InvalidSeparator |
| 6. | The date of birth, after deriving the century of birth from the check sum must be a valid date between January 1, 1900 and December 31, 2099. <br><b>OR</b><br> The date of birth may use zeros to indicate that some or all of the person's date of birth is unknown (see below for more details). | InvalidDateOfBirth |
| 7. | The sequence number may not be 000 or 999. | InvaliSequenceNumber |

### Additional Properties

| Name | Description |
| :--- | :---------- |
| DateOfBirth | Gets the person's date of birth, derived from the first six digits in YYMMDD format and the exact century of birth derived from the check digits. May be incomplete due to the person's exact date of birth being unknown when the rijksregisternummer was issued. |
| Gender | Gets the person's gender, as encoded in the third digit of the sequence number. |

### Notes

The date of birth can be adjusted in a variety of ways:
* If the person's date of birth is incomplete, then the two digit year is used and zeros are used for month and day (for example, 40.00.00-955.69).
* If there are too many people with incomplete dates of birth for a particular year than can be represented by a three digit sequence number (i.e. more than 499 males with incomplete dates of birth for the year 1940), then 01 is used for the day of birth and the sequence number rolls over to 001 (ex. 40.00.01-001.33). (Note that `BeRijksregiseterNumber` does not enforce an upper limit on the day component in cases of rollover, though multiple rollovers in a single year should be rare.)
* If the person's date of birth is unknown, then the constant 00.00.01 is used.

### References

[Wikipedia (French) - Numéro de registre national](https://fr.wikipedia.org/wiki/Num%C3%A9ro_de_registre_national)
