# GetDomainSID
 
## What is it?
 
A simple command line utility that gets the SID from a domain when you're running on a non domain machine.

## Why make this tool?

The domain SID is often needed for various Kerberos related attacks but if you google how to get it you'll mostly just find powershell exampels that don't work if you're not a member of the domain. Of course if you have a shell as a domain user then you can run `whoami /user` and strip the last part off the SID it shows you, but this tool is for when you don't have such a shell and only have credentials. 

## Usage

It takes 4 arguments:

`GetDomainSid.exe DomainName DcName Username Password`


**DomainName** - The domain name you want to get the SID for

**DcName** - The name or IP address of a domain controller in the domain

**Username** - The username to use for the LDAP connection

**Password** - The password for username


Example:

`GetDomainSid.exe mydomain.local 10.10.14.30 SomeUser SomePassword`

Note: If you want this to use Kerberos authentication then ensure you use the FQDN of a DC instead of its IP address. Also include the full domain name with the username (e.g "mydomain.local\UserA" instead of just "UserA")
