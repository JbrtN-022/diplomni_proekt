using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace rccs.MyClass
{
    internal class guideBD
    {
        public static void selectVidLica()
        {
            ConnectionBD.mycommand.CommandText = "SELECT * FROM rccs.type_of_face;";
            ConnectionBD.dtVidLica.Clear(); 
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtVidLica);
        }
        public static void selectGoroda()
        {
            ConnectionBD.mycommand.CommandText = "SELECT * FROM rccs.city;";
            ConnectionBD.dtGoroda.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtGoroda);
        }
        public static void selectEtaj()
        {
            ConnectionBD.mycommand.CommandText = "SELECT * FROM rccs.floor;";
            ConnectionBD.dtEtaj.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtEtaj);
        }
        public static void selectOffice()
        {
            ConnectionBD.mycommand.CommandText = "SELECT * FROM rccs.office;";
            ConnectionBD.dtOffice.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtOffice);
        }
        public static void selectRoll()
        {
            ConnectionBD.mycommand.CommandText = "SELECT * FROM rccs.roll;";
            ConnectionBD.dtRoll.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtRoll);
        }
        public static void selectCompany()
        {
            ConnectionBD.mycommand.CommandText = "SELECT id_company, company FROM rccs.company;";
            ConnectionBD.dtCompanyCombobox.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtCompanyCombobox);
        }

    }
}
