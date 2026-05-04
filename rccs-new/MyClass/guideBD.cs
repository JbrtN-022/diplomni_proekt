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
        public static void selectVidLicaForTable()
        {
            ConnectionBD.mycommand.CommandText = "SELECT type_of_face as 'тип лица' FROM rccs.type_of_face order by type_of_face asc;";
            ConnectionBD.dtVidLica.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtVidLica);
        }
        public static void selectGorodaForTable()
        {
            ConnectionBD.mycommand.CommandText = "SELECT  city as 'города' FROM rccs.city order by city asc;";
            ConnectionBD.dtGoroda.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtGoroda);
        }
        public static void selectEtajForTable()
        {
            ConnectionBD.mycommand.CommandText = "SELECT floor as 'этаж' FROM rccs.floor order by floor asc;";
            ConnectionBD.dtEtaj.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtEtaj);
        }
        public static void selectOfficeForTable()
        {
            ConnectionBD.mycommand.CommandText = "SELECT office as 'помещение' FROM rccs.office order by office asc;";
            ConnectionBD.dtOffice.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtOffice);
        }
        public static void selectRollForTable()
        {
            ConnectionBD.mycommand.CommandText = "SELECT roll as 'роль работника' FROM rccs.roll order by roll asc;";
            ConnectionBD.dtRoll.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtRoll);
        }
        public static void selectVidLica()
        {
            ConnectionBD.mycommand.CommandText = "SELECT * FROM rccs.type_of_face order by type_of_face asc;";
            ConnectionBD.dtVidLica.Clear(); 
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtVidLica);
        }
        public static void selectGoroda()
        {
            ConnectionBD.mycommand.CommandText = "SELECT  * FROM rccs.city order by city asc;";
            ConnectionBD.dtGoroda.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtGoroda);
        }
        public static void selectEtaj()
        {
            ConnectionBD.mycommand.CommandText = "SELECT * FROM rccs.floor order by floor asc;";
            ConnectionBD.dtEtaj.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtEtaj);
        }
        public static void selectOffice()
        {
            ConnectionBD.mycommand.CommandText = "SELECT * FROM rccs.office order by office asc;";
            ConnectionBD.dtOffice.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtOffice);
        }
        public static void selectRoll()
        {
            ConnectionBD.mycommand.CommandText = "SELECT * FROM rccs.roll order by roll asc;";
            ConnectionBD.dtRoll.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtRoll);
        }
        public static void selectCompany()
        {
            ConnectionBD.mycommand.CommandText = "SELECT id_company, company FROM rccs.company;";
            ConnectionBD.dtCompanyCombobox.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtCompanyCombobox);

        }
        public static void selectPrograms()
        {
            ConnectionBD.mycommand.CommandText =
                "SELECT id_program, name FROM rccs.program";

            ConnectionBD.dtProgramsComboBox.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtProgramsComboBox);
        }

        public static void selectServices()
        {
            ConnectionBD.mycommand.CommandText =
                "SELECT id_services, name FROM rccs.services";

            ConnectionBD.dtServicesComboBox.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtServicesComboBox);
        }

        public static void selectWorkers()
        {
            ConnectionBD.mycommand.CommandText =
                "SELECT id_workers,name FROM rccs.workers;";

            ConnectionBD.dtWorkersComboBox.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtWorkersComboBox);
        }
        public static void selectCounterpartyComboBox()
        {
            ConnectionBD.mycommand.CommandText =
                "SELECT id_counterparty,name FROM rccs.counterparty;";

            ConnectionBD.dtCounterpartyComboBox.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtCounterpartyComboBox);
        }
        //vidlica

        public static void AddVidLica(string name)
        {
            ConnectionBD.mycommand.CommandText = $@"INSERT INTO rccs.type_of_face (type_of_face) VALUES ('{name}');";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static void UppVidLica(string id, string name)
        {
            ConnectionBD.mycommand.CommandText = $@"UPDATE rccs.type_of_face
            SET type_of_face = '{name}'
            WHERE id_type_of_face = {id};";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static void DelVidLica(string name)
        {
            ConnectionBD.mycommand.CommandText = $@"DELETE FROM rccs.type_of_face WHERE id_type_of_face = '{name}';";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }

        public static bool DublicateVidLica(string name)
        {
            ConnectionBD.mycommand.CommandText = $"SELECT id_type_of_face FROM rccs.type_of_face WHERE type_of_face = @name";
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@name", name);

            int count = Convert.ToInt32(ConnectionBD.mycommand.ExecuteScalar());
            return count > 0;
        }
        // city

        public static void AddCity(string name)
        {
            ConnectionBD.mycommand.CommandText = $@"INSERT INTO rccs.city (city) VALUES ('{name}');";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static void UppCity(string id, string name)
        {
            ConnectionBD.mycommand.CommandText = $@"UPDATE rccs.city
            SET city = '{name}'
            WHERE id_city = '{id}';";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static void DelCity(string name)
        {
            ConnectionBD.mycommand.CommandText = $@"DELETE FROM rccs.city WHERE id_city = '{name}';";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static bool DublicateCity(string name)
        {
            ConnectionBD.mycommand.CommandText = $"SELECT id_city FROM rccs.city WHERE city = @name";
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@name", name);

            int count = Convert.ToInt32(ConnectionBD.mycommand.ExecuteScalar());
            return count > 0;
        }
        // roll

        public static void AddRoll(string name)
        {
            ConnectionBD.mycommand.CommandText = $@"INSERT INTO rccs.roll  (roll ) VALUES ('{name}');";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static void UppRoll(string id, string name)
        {
            ConnectionBD.mycommand.CommandText = $@"UPDATE rccs.roll
            SET roll =  '{name}'
            WHERE id_roll = {id};";

            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static void DelRoll(string name)
        {
            ConnectionBD.mycommand.CommandText = $@"DELETE FROM rccs.roll WHERE id_roll = '{name}';";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static bool DublicateRoll(string name)
        {
            ConnectionBD.mycommand.CommandText = $"SELECT id_roll FROM rccs.roll WHERE roll = @name";
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@name", name);

            int count = Convert.ToInt32(ConnectionBD.mycommand.ExecuteScalar());
            return count > 0;
        }
        // office

        public static void AddOffice(string name)
        {
            ConnectionBD.mycommand.CommandText = $@"INSERT INTO rccs.office  (office ) VALUES ('{name}');";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static void UppOffice(string id, string name)
        {
            ConnectionBD.mycommand.CommandText = $@"UPDATE rccs.office
            SET office = '{name}'
            WHERE id_office  = {id};";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static void DelOffice(string name)
        {
            ConnectionBD.mycommand.CommandText = $@"DELETE FROM rccs.office WHERE id_office = '{name}';";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static bool DublicateOffice(string name)
        {
            ConnectionBD.mycommand.CommandText = $"SELECT COUNT(*) FROM rccs.office WHERE office = @name";
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@name", name);

            int count = Convert.ToInt32(ConnectionBD.mycommand.ExecuteScalar());
            return count > 0;
        }
        // floor 

        public static void AddFloor(string name)
        {
            ConnectionBD.mycommand.CommandText = $@"INSERT INTO rccs.floor  (floor ) VALUES ('{name}');";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static void UppFloor(string id, string name)
        {
            ConnectionBD.mycommand.CommandText = $@"UPDATE rccs.floor
            SET floor = '{name}'
            WHERE id_floor  = {id};";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static void DelFloor(string name)
        {
            ConnectionBD.mycommand.CommandText = $@"DELETE FROM rccs.floor WHERE id_floor  = '{name}';";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        public static bool DublicateFloor(string name)
        {
            ConnectionBD.mycommand.CommandText = $"SELECT id_floor FROM rccs.floor WHERE floor = @name";
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@name", name);

            int count = Convert.ToInt32(ConnectionBD.mycommand.ExecuteScalar());
            return count > 0;
        }
    }
}
