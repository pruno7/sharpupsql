using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SharpUpSQL
{
    class CheckRights
    {
        public static string currentuser;
        public static void checkRights(SqlConnection conn)
        {
            bool sysadmin = false;
            bool impersonate = false;
            List<string> databases = new List<string>();
            Utils.print.blue("\t[*] Checking rights...");
            string querycurrentuser = "SELECT ORIGINAL_LOGIN();";
            SqlCommand cmd1 = new SqlCommand(querycurrentuser, conn);
            using (SqlDataReader reader = cmd1.ExecuteReader())
            {
                while (reader.Read())
                {
                    Utils.print.green("\t\t[+] You are the following SQL user : " + reader[0].ToString());
                    currentuser = reader[0].ToString();
                }
                reader.Close();
            }
            string query = "select r.name as Role, m.name as Principal " +
                "from " +
                    "master.sys.server_role_members rm " +
                    "inner join " +
                    "master.sys.server_principals r on r.principal_id = rm.role_principal_id and r.type = 'R' " +
                    "inner join " +
                    "master.sys.server_principals m on m.principal_id = rm.member_principal_id " +
                    "where m.name = '" + currentuser + "';";
            SqlCommand cmd = new SqlCommand(query, conn);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {

                    if (reader[0].ToString() == "sysadmin")
                    {
                        sysadmin = true;
                        Utils.print.green("\t\t\t[+] You are sysadmin !");
                        Utils.print.green("\t\t\t[+] Go use xp_cmdshell or check the database's content");
                    }
                    // not working, don't know why yet
                }
                reader.Close();
            }
            if (sysadmin == false)
            {
                Utils.print.red("\t\t\t[-] Not a sysadmin, looking for impersonation...");
                string queryimpersonate = "SELECT distinct b.name FROM sys.server_permissions a " +
                    "INNER JOIN sys.server_principals b ON a.grantor_principal_id = b.principal_id" +
                    " WHERE a.permission_name = 'IMPERSONATE'";
                cmd = new SqlCommand(queryimpersonate, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        impersonate = true;
                        Utils.print.green("\t\t\t\t[+] Can impersonate users !");
                        Utils.print.blue("\t\t\t\t[*] Users you can impersonate : ");
                        while (reader.Read())
                        {
                            Utils.print.white("\t\t\t\t\t" + reader[0].ToString());
                        }
                    }
                    reader.Close();
                }
                if (impersonate == false)
                {
                    Utils.print.red("\t\t\t[-] Not a sysadmin, can't impersonate, checking other rights...");
                    string queryserverrights = "SELECT permission_name FROM fn_my_permissions(NULL, 'SERVER');";
                    cmd = new SqlCommand(queryserverrights, conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Utils.print.blue("\t\t\t\t[*] Your rights on the server : ");
                        while (reader.Read())
                        {
                            Utils.print.white("\t\t\t\t\t" + reader[0].ToString());
                        }
                        reader.Close();
                    }
                    string querylistdb = "SELECT * FROM sys.databases";
                    cmd = new SqlCommand(querylistdb, conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Utils.print.blue("\t\t\t\t[*] Database list : ");
                        while (reader.Read())
                        {
                            Utils.print.white("\t\t\t\t\t" + reader[0].ToString());
                            databases.Add(reader[0].ToString());
                        }
                        reader.Close();
                    }
                    Utils.print.blue("\t\t\t\t[*] You could try the following : ");
                    Utils.print.white("\t\t\t\t\tXP_DIRTREE (for NetNTLM authentication triggering (relaying/bruteforce))");
                    Utils.print.white("\t\t\t\t\tXP_FILEEXIST (for NetNTLM authentication triggering (relaying/bruteforce))");
                    Utils.print.white("\t\t\t\t\tCheck databases' content");
                }
            }
        }
    }
}
