using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace рццс.MyClass
{
    internal class Autorization
    {
        public static void AutorizationBD(string log, string pass)
        {
            try
            {
                ConnectionBD.mycommand.CommandText = $@"";
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
                MessageBox.Show(e.Message);
            }
        }
    }
}
