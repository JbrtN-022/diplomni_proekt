using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace рццс.MyClass
{
    internal class ConnectionBD
    {
        public static string connectionString = @"Database = rccs; Data Source = localhost; password =qwerty; user=root; charset = utf8;";
        public static MySqlConnection myconnection;
        public static MySqlCommand mycommand;
        public static MySqlDataAdapter myDataAdapter;

        public static string login;
        public static string roll;

        public static bool ConnectBD()
        {
            try
            {
                myconnection = new MySqlConnection(connectionString);
                myconnection.Open();
                mycommand = new MySqlCommand();
                mycommand.Connection = myconnection;
                myDataAdapter = new MySqlDataAdapter(mycommand);

                return true;
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
