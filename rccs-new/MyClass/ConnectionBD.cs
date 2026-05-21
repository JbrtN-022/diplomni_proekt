using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace rccs.MyClass
{
    internal class ConnectionBD
    {
        //public static string connectionString = @"Database = rccs; Data Source = localhost; password =qwerty; user=root; charset = utf8;";
        public static MySqlConnection myconnection;
        public static MySqlCommand mycommand;
        public static MySqlDataAdapter myDataAdapter;

        public static string currentDataBase;
        public static string currentDataSource;
        public static string currentUser;
        public static string currentPassword;

        public static string login;
        public static string roll;
        public static string resFio;
    
        public static DataTable dtVidLica = new DataTable();    
        public static DataTable dtGoroda = new DataTable();
        public static DataTable dtEtaj = new DataTable();
        public static DataTable dtOffice = new DataTable();
        public static DataTable dtRoll = new DataTable();
        public static DataTable dtWorkers = new DataTable();
        public static DataTable dtTemp = new DataTable();
        public static DataTable dtTemp1 = new DataTable();
        public static DataTable dtTempLicense = new DataTable();
        public static DataTable dtTempLease = new DataTable();
        public static DataTable dtCompanyCombobox = new DataTable();
        public static DataTable dtOfficeComboBox = new DataTable();
        public static DataTable dtProgramsComboBox = new DataTable();
        public static DataTable dtServicesComboBox = new DataTable();
        public static DataTable dtFloorForLeaseComboBox = new DataTable();
        public static DataTable dtWorkersComboBox = new DataTable();
        public static  DataTable dtCounterparty = new DataTable();
        public static DataTable dtLoadDraftById = new DataTable();
        public static DataTable dtLoadAllServices = new DataTable();
        public static DataTable dtLoadDraftByIdLicense = new DataTable();
        public static DataTable dtLoadDraftLicensesComboBox = new DataTable(); 
        public static DataTable dtCounterpartyComboBox = new DataTable(); 
        public static DataTable dtGetLicenseAgreementForPrint = new DataTable(); 
        public static DataTable GetLicenseAgreementForPrintTable = new DataTable();
        public static bool ConnectBD(string dataBase, string dataSource, string user, string password)
        {
            try
            {
                currentDataBase = dataBase;
                currentDataSource = dataSource;
                currentUser = user;
                currentPassword = password;


                string connectionString = $@"Database = {dataBase}; Data Source = {dataSource}; password ={password}; user={user}; charset = utf8;";
                myconnection = new MySqlConnection(connectionString);
                myconnection.Open();
                mycommand = new MySqlCommand();
                mycommand.Connection = myconnection;
                myDataAdapter = new MySqlDataAdapter(mycommand);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
