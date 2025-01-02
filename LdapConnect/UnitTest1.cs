using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Net;

namespace LdapConnect
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {

            var ldapIdentifier = new LdapDirectoryIdentifier("172.105.135.186", 389);
            var userDn = "uid=test1,ou=users,dc=example,dc=com";
            var password = "1234567890";

            try
            {
                using (var connection = new LdapConnection(ldapIdentifier))
                {
                    connection.SessionOptions.ProtocolVersion = 3;
                    connection.AuthType = AuthType.Basic;
                    connection.Bind(new NetworkCredential(userDn, password));

                    var request = new SearchRequest(
                        "dc=example,dc=com",
                        "(&(objectClass=inetOrgPerson)(uid=test1))",
                        SearchScope.Subtree,
                        "cn", "uid", "sn"
                    );

                    var response = (SearchResponse)connection.SendRequest(request);
                    foreach (SearchResultEntry entry in response.Entries)
                    {
                        Console.WriteLine($"Found user: {entry.DistinguishedName}");
                        //foreach (var attr in entry.Attributes.AttributeNames)
                        //{
                        //    Console.WriteLine($"{attr}: {entry.Attributes[attr][0]}");
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                Debug.WriteLine($"Stack: {ex.StackTrace}");
            }
            Assert.Pass();
        }

        [Test]
        public void ListUserTest()
        {
            var ldapIdentifier = new LdapDirectoryIdentifier("localhost", 389);

            using (var connection = new LdapConnection(ldapIdentifier))
            {
                connection.SessionOptions.ProtocolVersion = 3;
                connection.AuthType = AuthType.Anonymous;
                connection.Bind(); // Anonymous bind

                var request = new SearchRequest(
                    "dc=example,dc=com",
                    "(objectClass=inetOrgPerson)",
                    SearchScope.Subtree,
                    "uid", "cn"
                );

                var response = (SearchResponse)connection.SendRequest(request);

                foreach (SearchResultEntry entry in response.Entries)
                {
                    Debug.WriteLine($"User: {entry.Attributes["uid"][0]}");
                    Debug.WriteLine($"Name: {entry.Attributes["cn"][0]}");
                    Debug.WriteLine("-------------------");
                }
            }
        }
    }
}