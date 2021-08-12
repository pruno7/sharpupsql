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
            Console.WriteLine("\t[*] Checking rights...");
            string querycurrentuser = "SELECT ORIGINAL_LOGIN();";
            SqlCommand cmd1 = new SqlCommand(querycurrentuser, conn);
            using (SqlDataReader reader = cmd1.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine("\t[*] You are the following SQL user : " + reader[0].ToString());
                    currentuser = reader[0].ToString();
                }
                reader.Close();
            }
            string query = "select r.name as Role, m.name as Principal from master.sys.server_role_members rm inner join master.sys.server_principals r on r.principal_id = rm.role_principal_id and r.type = 'R' inner join master.sys.server_principals m on m.principal_id = rm.member_principal_id where m.name ='"+ currentuser + "';";
            SqlCommand cmd2 = new SqlCommand(query, conn);
            using (SqlDataReader reader = cmd2.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader[0].ToString() == "sysadmin")
                    {
                        Console.WriteLine("\t[+] You are sysadmin !");
                    }
                    // not working, don't know why yet
                    else
                    {
                        Console.WriteLine("\t[+] No particular rights, did not check for impersonation, to be implemented...");
                    }
                }
                reader.Close();
            }
        }
    }
}
