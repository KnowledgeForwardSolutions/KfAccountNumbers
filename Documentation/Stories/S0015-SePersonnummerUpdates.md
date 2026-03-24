# S0015-SePersonnummer Updates

## Overview
Update SePersonnummer object to correct several deficiencies in original implementation

## Background
* Change handling of 6 digit date format. Currently assumes 1900. Change to use century cutoff threshold   similar to how SQL Server handles 6 digit dates (49 is century cutoff - 0-49 = 2000's, 50-99 = 1900's). If separator = '+' then 0-49 = 1900's, 50-99 = 1800's
* Change internal representation to 12 digits (YYYYMMDDBBBC, where BBB = 3 digit birth serial number and C = check digit). Remove boolean IsCentinarian flag.
* Change Value property to return raw YYYYMMDDBBBC value.
* Add ShortFormat method that returns YYMMDDSBBBC format, where S = separator character
* Add LongFormat method that returns YYYYMMDDSBBBC format
* Add overloads to format methods that accepts a System.TimeProvider that is used to determine the value of the separator ('+' for over 100 years of age according to time provider).
* Equality for entities created using short format and long format for same person should return TRUE 
