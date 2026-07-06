## SeIdentityNumber

Composite type that represents either a Swedish personal identity number (personnummer) or a Swedish coordination number (samordningsnummer). These identifiers have similar format and validation rules and are distinguished by the encoding of the person's date of birth.

A Swedish personnummer is Swedish national identification number and is issued to permanent residents of Sweden (> 12 months). A Swedish samordningsnummer is an identifier issued to temporary residents (those without a personnummer).

| Element | Description |
| :------ | :---------- |
| Class name: | SeIdentityNumber |
| Is composite: | Yes |
| Composite subtypes: | SePersonnummer, SeSamordningsnummer |
| Length: | 11 (short format with 2-digit year), 13 (long format with 4-digit year) |
| Check digit algorithm: | Luhn |
| Allowed characters: | Digits ('0'-'9') |
| Allowed separator characters: | Dash ('-') or plus ('+') |
| Structure: | ***YYMMDD-SSSC*** (short format) or ***YYYYMMDD-SSSC*** (long format), where: <dl><dt>YYMMDD</dt><dd>6-digit date of birth in YYMMDD format</dd><dt>YYYYMMDD</dt><dd>8-digit date of birth in YYYYMMDD format</dd><dt>-</dt><dd>Separator character, either dash ('-') if the person is less than 100 years old or plus ('+') if the person is 100 years or older</dd><dt>SSS</dt><dd>3-digit birth serial number used to distinguish between persons born on the same date. The last digit indicates the person's gender, with odd numbers = male and even numbers = female</dd><dt>C</dt><dd>Luhn algorithm check digit</dd></dl> |
| Example values: | <dl><dt>890201-3286</dt><dd>personnummer, short format, date of birth 890201, less than 100 years old, gender = female, check digit = 6</dd><dt>19890201-3286</dt><dd>personnummer, long format, date of birth 19890201, less than 100 years old, gender = female, check digit = 6</dd><dt>811228+9874</dt><dd>personnummer, short format, date of birth 811228, greater than 100 years old, gender = male, check digit = 4</dd><dt>890261-3283</dt><dd>samordningsnummer, short format, date of birth 890261 (actual date of birth = 890201), less than 100 years old, gender = female, check digit = 3</dd><dt>19890261-3283</dt><dd>samordningsnummer, long format, date of birth 19890261 (actual date of birth = 19890201), less than 100 years old, gender = female, check digit = 3</dd><dt>811288+9871</dt><dd>samordningsnummer, short format, date of birth 811288 (actual date of birth = 811228), greater than 100 years old, gender = male, check digit = 1</dd></dl> |

### Validation rules
| Rule | Description | Error Result Type |
| :--- | :---------- | :---------------- |
| 1. | The string value may not be null, String.Empty or all whitespace characters. | EmptyValue |
| 2. | The string length must be the short format length (11 characters) or the long format length (13 characters). | InvalidLength |
| 3. | All non-separator characters must be ASCII digits ('0'-'9'). | InvalidCharacter |
| 4. | The trailing character must be a valid Luhn algorithm check digit. | InvalidChecksum |
| 5. | The separator character must follow the date of birth (either 6 or 8 digits) and must be either a dash ('-') or a plus ('+'). | InvalidSeparator |
| 6. | The date of birth (either 6 or 8 digits) must be a valid date (after adjusting for samordningsnummer day offset, if present) | InvalidDateOfBirth |

### Additional Properties

| Name | Description |
| :--- | :---------- |
| DateOfBirth | Gets the person's date of birth, derived from the first six or eight digits and the separator character |
| Gender | Gets the person's gender, as encoded in the third digit of the birth serial number |
| IdentifierType | Gets the specific type of identifier that this instance represents |

### Additional Methods

| Name | Description |
| :--- | :---------- |
| ToPersonnummer | Convert this instance to a SePersonnummber |
| ToSamordningsnummer | Convert this instance to a SeSamordningsnummer |

### Notes:

The Luhn check digit is calculated using the 6 digits of the YYMMDD date of birth and the 3-digit birth serial number. If the value includes an 8-digit YYYYMMDD date of birth, the leading two digits of the year are ignored.

The encoded date of birth may not be the person's actual date of birth. If the possible birth serial numbers for a particular date are exhausted, a nearby date is used instead.

Samordningsnummers are distinguished from other identity numbers by an offset added to the day component of the date of birth. In a samordningsnummer, 60 is added to the day, resulting in a day between 61 and 91. The date of birth property takes the offset into account and returns the actual date of birth.

When validating the date of birth of a short format value with a 6-digit date of birth, a century cutoff of 50 is used. Years less than the century cutoff (00-49) are considered to be 2000-2049 and years greater than or equal to the century cutoff (50-99) are considered to be 1950-1999. If the separator character is a plus ('+'), then 100 is subtracted from the year: years 00-49 become 1900-1949 and years 50-99 become 1850-1899.

### References:

[Wikipedia - Personal identity number (Sweden)](https://en.wikipedia.org/wiki/Personal_identity_number_%28Sweden%29) for more info.
