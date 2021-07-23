using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;

namespace SharpUpSQL
{
    class SearchAD
    {
        public static (string, string) searchSPNs(string dc, string user, string password)
        {
            Console.WriteLine("[*] Searching for SPNs");
            DirectoryEntry Ldap = new DirectoryEntry("LDAP://" + dc, user, password);
            DirectorySearcher searcher = new DirectorySearcher(Ldap);
            searcher.Filter = "(servicePrincipalName=*SQL*)";
            foreach (SearchResult result in searcher.FindAll())
            {
                //Search for SQL Instances
                string sqlInstance = Convert.ToString(result.Properties["servicePrincipalName"][0]);
                Console.WriteLine("[*] Found SQL Instance : " + sqlInstance);
                string sqlInstanceParsed = sqlInstance.Split('/')[1];
                string sqlInstanceParsedName = sqlInstanceParsed.Split(':')[0];
                Console.WriteLine("[*] Formated name for SQL connection : " + sqlInstanceParsedName);
                string sqlInstanceParsedPort = sqlInstanceParsed.Split(':')[1];
                Console.WriteLine("[*] Formated port for SQL connection : " + sqlInstanceParsedPort);
                return (sqlInstanceParsedName, sqlInstanceParsedPort);
            }
            return ("Not found", "not found");
        }
    }
}
