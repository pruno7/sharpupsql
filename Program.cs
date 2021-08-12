using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.Data.SqlClient;

namespace SharpUpSQL
{
    class Variables
    {
        public static string sqlInstanceName;
        public static string sqlInstancePort;
        public static SqlConnection cnn;
    }
    class Options
    {
        [Option("ldapusername", Required = false, HelpText = "User to use to connect to LDAP")]
        public string LdapUserName { get; set; }

        [Option("ldappassword", Required = false, HelpText = "Password to use to connect to LDAP")]
        public string LdapPassword { get; set; }
        
        [Option("ldapdomain", Required = false, HelpText = "Domain to connect to")]
        public string domain { get; set; }

        [Option("bruteforce", Required = false, HelpText = "Bruteforce with common sql credentials")]
        public bool testBruteforce { get; set; }

        [Option("discoverspn", Required = false, HelpText = "Search for mssql instances in LDAP (SPN Discovery)")]
        public bool discoverspn { get; set; }

        [Option("sqlinstance", Required = false, HelpText = "IP,PORT")]
        public string sqlInstance { get; set; }

        [Option("checkrights", Required = false, HelpText = "Check rights of the user after connecting to the instance")]
        public bool checkRights { get; set; }

        [Option("currentuser", Required = false, HelpText = "Use current user ?")]
        public bool currentuser { get; set; }

        [Option("sqluser", Required = false, HelpText = "Use supplied ldap user for SQL connection")]
        public string sqlUser { get; set; }
        
        [Option("sqlpassword", Required = false, HelpText = "Use supplied ldap user for SQL connection")]
        public string sqlPassword { get; set; }

    }
    class Program
    {
        public static Options opts = new Options();
        static void Main(string[] args)
        {
            try
            {
                var parsedResult = Parser.Default.ParseArguments<Options>(args).WithParsed(parser => opts = parser);
                if (args.Length == 0)
                {
                    Environment.Exit(1);
                }
                // discover SQL instances from SPN discovery
                if (opts.discoverspn)
                {
                    /// check if domain is provided
                    if (!string.IsNullOrEmpty(opts.domain))
                    { 
                        // use current user ? NEEDS TO BE RUN ON DOMAIN JOINED MACHINE (runas /ne needs to be tested)
                        if (opts.currentuser)
                        {
                            Console.WriteLine("[*] Testing with current user");
                            var sqlInstanceADInfos = SearchAD.searchSPNs(opts.domain, "", "");
                            Variables.sqlInstanceName = sqlInstanceADInfos.Item1;
                            Variables.sqlInstancePort = sqlInstanceADInfos.Item2;
                            Variables.cnn = SQLConnect.sqlConnect(Variables.sqlInstanceName, Variables.sqlInstancePort, "", "");
                        }
                        // Use supplied ldap user and password
                        else if (!string.IsNullOrEmpty(opts.LdapUserName))
                        {
                            Console.WriteLine("[*] Testing with supplied LDAP user");
                            var sqlInstanceADInfos = SearchAD.searchSPNs(opts.domain, opts.LdapUserName, opts.LdapPassword);
                            Variables.sqlInstanceName = sqlInstanceADInfos.Item1;
                            Variables.sqlInstancePort = sqlInstanceADInfos.Item2;
                            Variables.cnn = SQLConnect.sqlConnect(Variables.sqlInstanceName, Variables.sqlInstancePort, opts.domain + "\\" + opts.LdapUserName, opts.LdapPassword);
                        }
                    }
                    else
                    {
                        Console.WriteLine("[-] Provide the domain !");
                        System.Environment.Exit(2);
                    }
                }
                // supplied instance
                else
                {
                    if (!string.IsNullOrEmpty(opts.domain))
                    {
                        // use current user ? NEEDS TO BE RUN ON DOMAIN JOINED MACHINE (runas /ne needs to be tested)
                        if (opts.currentuser)
                        {
                            Variables.sqlInstanceName = opts.sqlInstance.Split(',')[0];
                            Console.WriteLine("[+] Formated name for SQL connection : " + Variables.sqlInstanceName);
                            Variables.sqlInstancePort = opts.sqlInstance.Split(',')[1];
                            Console.WriteLine("[+] Formated port for SQL connection : " + Variables.sqlInstancePort);
                            Variables.cnn = SQLConnect.sqlConnect(Variables.sqlInstanceName, Variables.sqlInstancePort, "", "");
                        }
                        // Use supplied ldap user and password
                        else if (!string.IsNullOrEmpty(opts.LdapUserName))
                        {
                            Variables.sqlInstanceName = opts.sqlInstance.Split(',')[0];
                            Console.WriteLine("[+] Formated name for SQL connection : " + Variables.sqlInstanceName);
                            Variables.sqlInstancePort = opts.sqlInstance.Split(',')[1];
                            Console.WriteLine("[+] Formated port for SQL connection : " + Variables.sqlInstancePort);
                            Variables.cnn = SQLConnect.sqlConnect(Variables.sqlInstanceName, Variables.sqlInstancePort, opts.domain + "\\" + opts.LdapUserName, opts.LdapPassword);
                        }
                    }
                    else
                    {
                        Console.WriteLine("[-] Provide the domain !");
                        System.Environment.Exit(2);
                    }
                    // use supplied credz for MSSQL
                    if (!string.IsNullOrEmpty(opts.sqlUser))
                    {
                        Variables.sqlInstanceName = opts.sqlInstance.Split(',')[0];
                        Console.WriteLine("[+] Formated name for SQL connection : " + Variables.sqlInstanceName);
                        Variables.sqlInstancePort = opts.sqlInstance.Split(',')[1];
                        Console.WriteLine("[+] Formated port for SQL connection : " + Variables.sqlInstancePort);
                        Variables.cnn = SQLConnect.sqlConnect(Variables.sqlInstanceName, Variables.sqlInstancePort, opts.sqlUser, opts.sqlPassword);
                    }
                    else
                    {
                        Console.WriteLine("[-] Provide sqluser or use LDAP !");
                        System.Environment.Exit(2);
                    }

                }
                // Bruteforce with default users
                if (opts.testBruteforce)
                {
                    Bruteforce.bruteforceSQL(Variables.sqlInstanceName, Variables.sqlInstancePort);
                }
                // check user rights on SQL instance
                if (opts.checkRights)
                {
                    CheckRights.checkRights(Variables.cnn);
                }

                // TODO 
                // Try Privesc
                // run xp_cmdshell command
                // xp_dirtree
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
