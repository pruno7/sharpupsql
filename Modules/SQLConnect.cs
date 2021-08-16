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
        public static SqlConnection sqlConnect(string sqlInstanceParsedName, string sqlUsername, string sqlPassword)
        {
            string connString;
            SqlConnection cnn;
            if (Program.opts.currentuser)
            {
                connString = @"Data Source=" + sqlInstanceParsedName + ";Integrated Security=True";
                cnn = new SqlConnection(connString);
                cnn.Open();
                Utils.print.green("\t[+] Connection succeeded with current user");
                // check user rights on SQL instance
                if (Program.opts.checkRights)
                {
                    CheckRights.checkRights(cnn);
                }
            }
            else
            {
                connString = @"Data Source=" + sqlInstanceParsedName + ";User ID=" + sqlUsername + ";Password=" + sqlPassword;
                cnn = new SqlConnection(connString);
                cnn.Open();
                Utils.print.green("\t[+] Connection succeeded with : " + sqlUsername + ":" + sqlPassword);
                // check user rights on SQL instance
                if (Program.opts.checkRights)
                {
                    CheckRights.checkRights(cnn);
                }
            }
            return cnn;
        }
    }
}
