# sharpupsql

WIP (aka my code sucks and i'm a bad programmer, but w/e)

Porting some of PowerUPSQL features to C#

Can also be used with Cobalt Strike's execute-assembly. (not yet usable)

## Options

```
--ldapusername - User to use to connect to LDAP

--ldappassword - Password to use to connect to LDAP

--ldapdomain - Domain to connect to

--bruteforce - Bruteforce with common sql credentials

--discoverspn - Search for mssql instances in LDAP (SPN Discovery)

--sqlinstance - Sql instance to connect to, format : IP,PORT

--checkrights - Check rights of the user after connecting to the instance (for now will only check if you're sysadmin)

--currentuser - Use current domain user for everything

--sqluser - Use supplied user for SQL connection

--sqlpassword - Use supplied password for SQL connection

--testconnect - Test to connect to the found/provided sql instance(s) using provided method (sqluser/ldapuser/currentuser)

--list - Gives a list of all the SQL instances after SPN discovery

--help - Do i really to explain this ?
```

## TODO (feel free to PR, i'm a lazy ass)
- Use suboptions instead of everything at once
- Try Privesc (impersonation stuff after checking rights)
- check for xp_cmdshell (present/activated?) and run supplied command
- xp_dirtree/xp_fileexist for NetNTLM grabbing/relaying
- Prepare a BOF.NET usable version
