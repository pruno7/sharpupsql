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
        public static void checkRights(SqlConnection conn)
        {
            Console.WriteLine("\t[*] Checking rights...");
            string query = "select r.name as Role, m.name as Principal from master.sys.server_role_members rm inner join master.sys.server_principals r on r.principal_id = rm.role_principal_id and r.type = 'R' inner join master.sys.server_principals m on m.principal_id = rm.member_principal_id where m.name ='sa';";
            SqlCommand cmd = new SqlCommand(query, conn);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if(reader[0].ToString() == "sysadmin")
                    {
                        Console.WriteLine("\t[+] You are sysadmin !");
                    }
                    else
                    {
                        Console.WriteLine("\t[*] You are " + reader[0].ToString());
                    }
                }
                reader.Close();
            }
        }
    }
}
