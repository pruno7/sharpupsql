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
        public static SqlConnection sqlConnect(string sqlInstanceParsedName, string sqlInstanceParsedPort, string sqlUsername, string sqlPassword, bool checkRights)
        {
            Console.WriteLine("[*] Testing connection with " + sqlUsername);
            string connString = @"Data Source=" + sqlInstanceParsedName + "," + sqlInstanceParsedPort + ";User ID=" + sqlUsername + ";Password=" + sqlPassword;
            SqlConnection cnn = new SqlConnection(connString);
            cnn.Open();
            Console.WriteLine("[+] Connection succeeded with : " + sqlUsername + ":" + sqlPassword);
            if (checkRights)
            {
                CheckRights.checkRights(cnn);
            }
            return cnn;
        }
    }
}
