using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SharpUpSQL
{
    class SQLConnect
    {
        public static SqlConnection sqlConnect(string sqlInstanceParsedName, string sqlInstanceParsedPort, string sqlUsername, string sqlPassword)
        {
            string connString;
            SqlConnection cnn;
            if (Program.opts.currentuser)
            {
                Console.WriteLine("[*] Testing SQL connection with current user");
                connString = @"Data Source=" + sqlInstanceParsedName + "," + sqlInstanceParsedPort + ";Integrated Security=True";
                cnn = new SqlConnection(connString);
                cnn.Open();
                Console.WriteLine("[+] Connection succeeded with current user");
            }
            else if (string.IsNullOrEmpty(Program.opts.LdapUserName))
            {
                Console.WriteLine("[*] Testing SQL connection with " + sqlUsername);
                connString = @"Data Source=" + sqlInstanceParsedName + "," + sqlInstanceParsedPort + ";User ID=" + sqlUsername + ";Password=" + sqlPassword;
                cnn = new SqlConnection(connString);
                cnn.Open();
                Console.WriteLine("[+] Connection succeeded with : " + sqlUsername + ":" + sqlPassword);
            }
            else
            {
                Console.WriteLine("[*] Testing SQL connection with " + sqlUsername);
                connString = @"Data Source=" + sqlInstanceParsedName + "," + sqlInstanceParsedPort + ";User ID=" + sqlUsername + ";Password=" + sqlPassword;
                cnn = new SqlConnection(connString);
                cnn.Open();
                Console.WriteLine("[+] Connection succeeded with : " + sqlUsername + ":" + sqlPassword);
            }
            return cnn;
        }
    }
}
