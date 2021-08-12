# sharpupsql

WIP (aka my code sucks and i'm a bad programmer, but w/e)

Porting some of PowerUPSQL features to C#

Can also be used with Cobalt Strike's execute-assembly.

## Options

```
--ldapusername - User to use to connect to LDAP

--ldappassword - Password to use to connect to LDAP

--ldapdomain - Domain to connect to

--bruteforce - Bruteforce with common sql credentials

--discoverspn - Search for mssql instances in LDAP (SPN Discovery)

--sqlinstance - Sql instance to connect to, format : IP,PORT

--checkrights - Check rights of the user after connecting to the instance

--currentuser - Use current domain user for everything

--sqluser - Use supplied user for SQL connection

--sqlpassword - Use supplied password for SQL connection

--help - Do i really to explain this ?
```