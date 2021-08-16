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
        
        public static List<string> searchSPNs(string dc, string user, string password)
        {
            List<string> instances = new List<string>();
            DirectoryEntry Ldap;
            Utils.print.blue("\t[*] Searching for SPNs");
            if (Program.opts.currentuser)
            {
                Utils.print.blue("\t\t[*] Checking AD with current user");
                Ldap = new DirectoryEntry("LDAP://" + dc);
            }
            else
            {
                Utils.print.blue("\t\t[*] Checking AD with provided user : " + user);
                Ldap = new DirectoryEntry("LDAP://" + dc, user, password);
            }
            DirectorySearcher searcher = new DirectorySearcher(Ldap);
            searcher.Filter = "(servicePrincipalName=*SQL*)";
            foreach (SearchResult result in searcher.FindAll())
            {
                //Search for SQL Instances
                string sqlInstance = Convert.ToString(result.Properties["servicePrincipalName"][0]);
                Utils.print.green("\t\t\t[*] Found SQL Instance : " + sqlInstance);
                string sqlInstanceParsed = sqlInstance.Split('/')[1];
                string recoveredSQLInstance = sqlInstanceParsed.Split(':')[0] + "," + sqlInstanceParsed.Split(':')[1];
                instances.Add(recoveredSQLInstance);
            }
            return instances;
        }
    }
}
