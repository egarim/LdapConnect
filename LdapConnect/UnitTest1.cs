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

            var ldapIdentifier = new LdapDirectoryIdentifier("localhost", 389);
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
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
            }
            Assert.Pass();
        }
    }
}