using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;

namespace ConsoleApp.NetCore.Ldap
{
    class Program
    {
        static void Main(string[] args)
        {
            // https://www.forumsys.com/tutorials/integration-how-to/ldap/online-ldap-test-server/
            int ldapPort = LdapConnection.DEFAULT_PORT;
            int ldapVersion = LdapConnection.Ldap_V3;
            string ldapHost = "ldap.forumsys.com";
            var loginDN = "cn=read-only-admin,dc=example,dc=com";
            var password = "password";

            string searchLdapUser = "riemann";

            string searchFilter = "(objectclass=*)";
            string searchBase = $"uid={searchLdapUser}, dc=example, dc=com"; // "ou = scientists, dc = example, dc = com"; //"uid=gauss, dc=example, dc=com"; 
          
            LdapSearchConstraints constraints = new LdapSearchConstraints{};

            var users = new HashSet<string>();
            try
            {
                using (var cn = new LdapConnection())
                {
                    // connect
                    cn.Connect(ldapHost, ldapPort);
                    cn.Bind(loginDN, password);

                    LdapSearchResults searchResults = cn.Search(
                       searchBase,
                       LdapConnection.SCOPE_SUB,
                       searchFilter,
                       null, // no specified attributes
                       true, // false = return attr and value
                       constraints);

                   
                    while (searchResults.HasMore())
                    {
                        if(searchResults.Count == 1)
                        { 
                            Console.WriteLine("true - found");
                        }
                        searchResults.Next();
                    }
                }
            }
            catch (LdapException ldapEx)
            {
                Console.WriteLine(ldapEx.ToString()); // ocassional time outs
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            foreach (var u in users)
            {
                Console.WriteLine("Key:" + u);
            }
            Console.ReadKey();

        }

        // DirectoryEntry - not working on linux atm 04/2020

        //static void Main(string[] args)
        //{
        //    string ldapServer = "LDAP://ldap.forumsys.com:389/dc=example,dc=com";
        //    string userName = "cn=read-only-admin,dc=example,dc=com";
        //    string password = "password";

        //    var directoryEntry = new System.DirectoryServices.DirectoryEntry(ldapServer, userName, password, AuthenticationTypes.ServerBind);

        //    // Bind to server with admin. Real life should use a service user.
        //    object obj = directoryEntry.NativeObject;
        //    if (obj == null)
        //    {
        //        Console.WriteLine("Bind with admin failed!.");
        //        Environment.Exit(1);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Bind with admin succeeded!");
        //    }

        //    // Search for the user first.
        //    DirectorySearcher searcher = new DirectorySearcher(directoryEntry);
        //    searcher.Filter = "(uid=riemann)";
        //    searcher.PropertiesToLoad.Add("*");
        //    SearchResult rc = searcher.FindOne();
        //    // First we should handle user not found.
        //    // To simplify, skip it and try to bind to the user.
        //    var validator = new System.DirectoryServices.DirectoryEntry(ldapServer, "uid=riemann,dc=example,dc=com", password, AuthenticationTypes.ServerBind);
        //    if (validator.NativeObject.Equals(null))
        //    {
        //        Console.WriteLine("Cannot bind to user!");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Bind with user succeeded!");
        //    }

        //    Console.ReadKey();
        //}
    }
}
