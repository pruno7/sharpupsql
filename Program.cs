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
        public static SqlConnection cnn;
        public static List<string> instances = new List<string>();
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

        [Option("sqlinstance", Required = false, HelpText = "Sql instance to connect to, format : IP,PORT")]
        public string sqlInstance { get; set; }

        [Option("checkrights", Required = false, HelpText = "Check rights of the user after connecting to the instance")]
        public bool checkRights { get; set; }

        [Option("currentuser", Required = false, HelpText = "Use current domain user for everything")]
        public bool currentuser { get; set; }

        [Option("sqluser", Required = false, HelpText = "Use supplied user for SQL connection")]
        public string sqlUser { get; set; }
        
        [Option("sqlpassword", Required = false, HelpText = "Use supplied password for SQL connection")]
        public string sqlPassword { get; set; }
        
        [Option("testconnect", Required = false, HelpText = "Test to connect every supplied or discovered instance using chosen method (supplied sql user/pass, supplied domain user/pass, current user")]
        public bool testConnect { get; set; }

        [Option("list", Required = false, HelpText = "print discovered spn list")]
        public bool list { get; set; }

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
                    Utils.print.red("[-] Use --help to show options");
                    Environment.Exit(1);
                }
                // SPN Discvory
                if (opts.discoverspn)
                {
                    Utils.print.blue("[*] Using SPN Discovery");
                    if (!string.IsNullOrEmpty(opts.domain))
                    {
                        Utils.print.blue("\t[*] The domain to use is : " + opts.domain);
                        if (opts.currentuser)
                        {
                            Variables.instances = SearchAD.searchSPNs(opts.domain, "", "");
                        }
                        else if (!string.IsNullOrEmpty(opts.LdapUserName))
                        {
                            Variables.instances = SearchAD.searchSPNs(opts.domain, opts.LdapUserName, opts.LdapPassword);
                        }
                    }
                    else
                    {
                        Utils.print.red("[-] Provide the domain !");
                        Environment.Exit(2);
                    }
                    if (opts.list)
                    {
                        Utils.print.blue("\t[*] Discovered instances : ");
                        foreach (string instance in Variables.instances)
                        {
                            Utils.print.white("\t\t" + instance);
                        }
                    }
                }
                // Test connect to SQL instance
                if (opts.testConnect)
                {
                    // found sql instances via spn discovery
                    if (opts.discoverspn)
                    {
                        // test current user
                        if (opts.currentuser)
                        {
                            foreach(string instance in Variables.instances)
                            {
                                Utils.print.blue("[*] Testing the following instance : " + instance);
                                Variables.cnn = SQLConnect.sqlConnect(instance, "", "");
                            }
                        }
                        // test provided ldap user
                        if (!string.IsNullOrEmpty(opts.LdapUserName))
                        {
                            // no check of domain because spn discovery handles that case
                            foreach (string instance in Variables.instances)
                            {
                                Utils.print.blue("[*] Testing the following instance : " + instance);
                                Variables.cnn = SQLConnect.sqlConnect(instance, opts.domain + "\\" + opts.LdapUserName, opts.LdapPassword);
                            }
                        }
                        else if (!string.IsNullOrEmpty(opts.sqlUser))
                        {
                            foreach (string instance in Variables.instances)
                            {
                                Utils.print.blue("[*] Testing the following instance : " + instance);
                                Variables.cnn = SQLConnect.sqlConnect(opts.sqlInstance, opts.sqlUser, opts.sqlPassword);
                            }
                        }
                    }
                    // provided sql instance
                    else if (!string.IsNullOrEmpty(opts.sqlInstance))
                    {
                        // test current user against provided sql instance
                        if (opts.currentuser)
                        {
                            Variables.cnn = SQLConnect.sqlConnect(opts.sqlInstance, "", "");
                        }
                        // test provided ldap user against provided sql instance
                        else if (!string.IsNullOrEmpty(opts.LdapUserName))
                        {
                            if (!string.IsNullOrEmpty(opts.domain))
                            {
                                Variables.cnn = SQLConnect.sqlConnect(opts.sqlInstance, opts.domain + "\\" + opts.LdapUserName, opts.LdapPassword);
                            }
                            else
                            {
                                Utils.print.red("[-] Provide the domain !");
                                Environment.Exit(2);
                            }
                        }
                        else if (!string.IsNullOrEmpty(opts.sqlUser))
                        {
                            Variables.cnn = SQLConnect.sqlConnect(opts.sqlInstance, opts.sqlUser, opts.sqlPassword);
                        }
                        else
                        {
                            Utils.print.red("[-] Provide some ways to connect, --help for options");
                        }
                    }
                }
                
                // use bruteforce module
                if (opts.testBruteforce)
                {
                    // use found sql instances from spn discovery
                    if (opts.discoverspn)
                    {
                        foreach (string instance in Variables.instances)
                        {
                            Utils.print.blue("[*] Testing the following instance : " + instance);
                            Bruteforce.bruteforceSQL(instance);
                        }
                    }
                    // use provided sql instance
                    else if (!string.IsNullOrEmpty(opts.sqlInstance))
                    {
                        Utils.print.blue("[*] Testing the following instance : " + opts.sqlInstance);
                        Bruteforce.bruteforceSQL(opts.sqlInstance);
                    }
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
