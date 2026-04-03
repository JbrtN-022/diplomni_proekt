using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace rccs.MyClass
{
    internal class Autorization
    {
        public static void AutorizationBD(string log, string pass)
        {
            try
            {
                ConnectionBD.mycommand.CommandText = $@"SELECT id_roll FROM rccs.users where password = '{pass}' and  login = '{log}';";
                Object res = ConnectionBD.mycommand.ExecuteScalar();
                if (res != null)
                {
                    ConnectionBD.roll = res.ToString();
                    ConnectionBD.login = log;
                }
                else
                {
                    ConnectionBD.roll = null;
                }
            }
            catch (Exception e)
            {
                ConnectionBD.login = ConnectionBD.roll = null;
                
            }
        }
        public static void GetFioUser(string log, string pass)
        {
            try
            {
                ConnectionBD.mycommand.CommandText = $@"SELECT workers.name FROM rccs.workers, rccs.users where users.id_workers = workers.id_workers and login ='{log}' and password= '{pass}';";
                Object fio = ConnectionBD.mycommand.ExecuteScalar();
                if (fio != null)
                {
                    ConnectionBD.resFio = fio.ToString();
                }
                else
                {
                    ConnectionBD.resFio = null;
                }
            }
            catch (Exception e)
            {
                ConnectionBD.login = ConnectionBD.roll = null;
                MessageBox.Show(e.Message);
            }
        }
    }
}
