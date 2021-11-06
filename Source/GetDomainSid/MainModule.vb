Imports System.DirectoryServices

Module MainModule



    Const SidAttributeName As String = "objectSid"
    Const LdapFilter As String = "(&(objectClass=user)(objectSid=*)(!(objectClass=foreignSecurityPrincipal)))"

    Sub Main()
        If My.Application.CommandLineArgs.Count < 4 Then
            WriteHelpText()
            Return
        End If
        GetSid(My.Application.CommandLineArgs(0), My.Application.CommandLineArgs(1), My.Application.CommandLineArgs(2), My.Application.CommandLineArgs(3))
    End Sub

    Private Sub GetSid(DomainName As String, DcName As String, Username As String, Password As String)
        Try
            If Not DomainName.Contains(".") Then
                Console.WriteLine("Domain name must be in DNS format not NetBIOS format (e.g: mydomain.local instead of MYDOMAIN")
                Return
            End If

            Using RootDE As New DirectoryEntry("LDAP://" & DcName & "/DC=" & DomainName.TrimEnd("."c).Replace(".", ",DC="))
                RootDE.AuthenticationType = AuthenticationTypes.Secure
                If Not String.IsNullOrEmpty(Username) Then
                    RootDE.Username = Username
                    RootDE.Password = Password
                End If
                Using Searcher As New DirectorySearcher(RootDE)
                    Searcher.Filter = LdapFilter
                    Searcher.PropertiesToLoad.Add(SidAttributeName)
                    Dim Result As SearchResult = Searcher.FindOne()
                    If Result Is Nothing Then
                        Console.WriteLine("Unable to find any objects in the domain that match the filter " & LdapFilter)
                        Return
                    End If
                    If Not Result.Properties.Contains(SidAttributeName) Then
                        Console.WriteLine("No LDAP attribute named " & SidAttributeName & " found on object " & Result.Path)
                        Return
                    End If
                    Dim RawSid As Byte() = Result.Properties(SidAttributeName)(0)
                    Dim Sid As New Security.Principal.SecurityIdentifier(RawSid, 0)
                    Console.WriteLine(Sid.AccountDomainSid)
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub WriteHelpText()
        Console.WriteLine()
        Console.WriteLine("Gets the SID from an external domain by locating a user account in the domain and trimming the RID from their SID")
        Console.WriteLine()
        Console.WriteLine("Usage:")
        Console.WriteLine()
        Console.WriteLine("  GetDomainSid.exe DomainName DcName Username Password")
        Console.WriteLine()
        Console.WriteLine("Arguments:")
        Console.WriteLine()
        Console.WriteLine("  DomainName" & vbTab & "The domain name you want to get the SID for")
        Console.WriteLine("  DcName" & vbTab & "The name or IP address of a domain controller in the domain")
        Console.WriteLine("  Username" & vbTab & "The username to use for the LDAP connection")
        Console.WriteLine("  Password" & vbTab & "The password for username")
        Console.WriteLine()
        Console.WriteLine("Example:")
        Console.WriteLine()
        Console.WriteLine("  GetDomainSid.exe mydomain.local 10.10.14.30 SomeUser SomePassword")
        Console.WriteLine()
        Console.WriteLine("Note: If you want this to use Kerberos authentication then ensure you use the FQDN of a DC instead of its IP address. " &
                          "Also include the full domain name with the username (e.g ""mydomain.local\UserA"" instead of just ""UserA"")")
        Console.WriteLine()
    End Sub

End Module
