using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SharpUpSQL
{
    class Variables
    {
        public static string sqlInstanceName;
        public static string sqlInstancePort;
    }
    class Options
    {
        [Option("ldapusername", Required = false, HelpText = "User to use to connect to LDAP")]
        public string UserName { get; set; }

        [Option("ldappassword", Required = false, HelpText = "Password to use to connect to LDAP")]
        public string Password { get; set; }
        
        [Option("ldapdomain", Required = false, HelpText = "ldapdomain")]
        public string domain { get; set; }

        /*[Option("dc", Required = false, HelpText = "IP/FQDN of the DC for LDAP search")]
        public string dcIP { get; set; }*/

        [Option("bruteforce", Required = false, HelpText = "Bruteforce with common sql credentials")]
        public bool testBruteforce { get; set; }

        [Option("discoverspn", Required = false, HelpText = "Search for mssql instances in LDAP (SPN Discovery)")]
        public bool discoverspn { get; set; }

        [Option("sqlinstance", Required = false, HelpText = "IP,PORT")]
        public string sqlInstance { get; set; }

        [Option("checkrights", Required = false, HelpText = "Check rights of the user after connecting to the instance")]
        public bool checkRights { get; set; }

        [Option("currentuser", Required = false, HelpText = "IP,PORT")]
        public bool currentuser { get; set; }

        [Option("testldapuser", Required = false, HelpText = "IP,PORT")]
        public bool testldapuser { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Options opts = new Options();
                var parsedResult = Parser.Default.ParseArguments<Options>(args).WithParsed(parser => opts = parser);
                if (args.Length == 0)
                {
                    Environment.Exit(1);
                }
                string user = opts.UserName;
                string password = opts.Password;
                string domain = opts.domain;
                //string dcIP = opts.dcIP;
                string sqlInstance = opts.sqlInstance;

                if (opts.discoverspn)
                {
                    if (opts.currentuser)
                    {
                        Console.WriteLine("TODO...");
                    }
                    else
                    {
                        Console.WriteLine("[*] Testing with supplied LDAP user");
                        var sqlInstanceADInfos = SearchAD.searchSPNs(domain, user, password);
                        Variables.sqlInstanceName = sqlInstanceADInfos.Item1;
                        Variables.sqlInstancePort = sqlInstanceADInfos.Item2;
                        if (opts.testldapuser)
                        {
                            SQLConnect.sqlConnect(Variables.sqlInstanceName, Variables.sqlInstancePort, domain + "\\" + user, password, opts.checkRights);
                        }
                    }

                }
                else
                {
                    Variables.sqlInstanceName = sqlInstance.Split(',')[0];
                    Console.WriteLine("[+] Formated name for SQL connection : " + Variables.sqlInstanceName);
                    Variables.sqlInstancePort = sqlInstance.Split(',')[1];
                    Console.WriteLine("[+] Formated port for SQL connection : " + Variables.sqlInstancePort);
                }
                // Bruteforce with default users
                if (opts.testBruteforce)
                {
                    Bruteforce.bruteforceSQL(Variables.sqlInstanceName, Variables.sqlInstancePort, opts.checkRights);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
