using MySql.Data.MySqlClient;
using rccs.MyClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace rccs_new.MyClass
{
    internal class leaseAndLicenseAgreement
    {
        public static void LoadLeaseAgreements(System.Windows.Controls.ComboBox cmb, string approvedFilter = null)
        {
            cmb.ItemsSource = null;
            string sql = @"
        SELECT 
            la.id_lease_agreement,
            CONCAT(la.id_lease_agreement, ' — ', c.name, ' • ', o.office) AS display_name
        FROM rccs.lease_agreement la
        JOIN rccs.counterparty c ON c.id_counterparty = la.id_counterparty
        JOIN rccs.room r ON r.id_room = la.id_room
        JOIN rccs.office o ON o.id_office = r.id_office";

            if (!string.IsNullOrEmpty(approvedFilter))
                sql += " WHERE la.approved = @approved";

            sql += " ORDER BY la.id_lease_agreement DESC";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();

            if (!string.IsNullOrEmpty(approvedFilter))
                ConnectionBD.mycommand.Parameters.AddWithValue("@approved", approvedFilter);

            ConnectionBD.dtTemp.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

            cmb.ItemsSource = ConnectionBD.dtTemp.DefaultView;
            cmb.DisplayMemberPath = "display_name";
            cmb.SelectedValuePath = "id_lease_agreement";
        }

        public static void LoadLicenseAgreements(System.Windows.Controls.ComboBox cmb, string approvedFilter = null)
        {
            cmb.ItemsSource = null;
            string sql = @"
        SELECT 
            la.id_license_agreement,
            CONCAT(la.id_license_agreement, ' — ', c.name, ': ', p.name) AS display_name
        FROM rccs.license_agreement la
        JOIN rccs.counterparty c ON c.id_counterparty = la.id_counterparty
        JOIN rccs.program p ON p.id_program = la.id_program";

            if (!string.IsNullOrEmpty(approvedFilter))
                sql += " WHERE la.approved = @approved";

            sql += " ORDER BY la.id_license_agreement DESC";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();

            if (!string.IsNullOrEmpty(approvedFilter))
                ConnectionBD.mycommand.Parameters.AddWithValue("@approved", approvedFilter);

            ConnectionBD.dtTemp.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

            cmb.ItemsSource = ConnectionBD.dtTemp.DefaultView;
            cmb.DisplayMemberPath = "display_name";
            cmb.SelectedValuePath = "id_license_agreement";
        }
        public static bool CanDeleteDocument(
            int idDocument,
            string documentType)
        {
            string sql = "";

            if (documentType == "Договор аренды")
            {
                sql = @"
SELECT rental_date_until
FROM rccs.lease_agreement
WHERE id_lease_agreement = @id";
            }

            else if (documentType == "Лицензия на ПО")
            {
                sql = @"
SELECT rental_date_until
FROM rccs.license_agreement
WHERE id_license_agreement = @id";
            }

            else
            {
                return false;
            }

            ConnectionBD.mycommand.Parameters.Clear();

            ConnectionBD.mycommand.CommandText = sql;

            ConnectionBD.mycommand.Parameters
                .AddWithValue("@id", idDocument);

            object result =
                ConnectionBD.mycommand.ExecuteScalar();

           
            if (result == null ||
                result == DBNull.Value)
            {
                MessageBox.Show(
                    "Документ не найден!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            DateTime rentalDateUntil =
                Convert.ToDateTime(result);
            
            
            if (rentalDateUntil.Date >=
                DateTime.Today)
            {
                MessageBox.Show(
                    "Нельзя удалить активный документ!\n\n" +
                    $"Срок действия документа ещё не истёк. (до '{rentalDateUntil.ToString("yyyy-MM-dd")}')",
                    "Удаление запрещено",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            return true;
        }

        public static bool DeleteDocument(
           int idDocument,
           string documentType)
        {
            try
            {
                ConnectionBD.mycommand.Parameters
                    .Clear();

                string sql = "";

              
                if (documentType ==
                    "Договор аренды")
                {
                    sql = @"
DELETE FROM rccs.lease_agreement
WHERE id_lease_agreement = @id";
                }

                
                else if (documentType ==
                    "Лицензия на ПО")
                {
                    sql = @"
DELETE FROM rccs.service_in_agreement
WHERE id_license_agreement = @id;

DELETE FROM rccs.license_agreement
WHERE id_license_agreement = @id";
                }

                else
                {
                    return false;
                }

                ConnectionBD.mycommand.CommandText =
                    sql;

                ConnectionBD.mycommand.Parameters
                    .AddWithValue("@id", idDocument);

                ConnectionBD.mycommand
                    .ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка удаления:\n" +
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }
        }



        public static DataTable LoadLicenseAgreementsToList(bool onlyApproved = false)
        {
            string query = @"
SELECT 
    la.id_license_agreement,
    la.id_counterparty,
    la.approved,
    la.rental_date_from,
    la.rental_date_until,
    la.conclusion_date,
    c.name as counterparty_name,
    p.name as program_name,
    w.name as worker_name,
    CONCAT('№', la.id_license_agreement, ' | ', c.name) as DisplayName
FROM license_agreement la
LEFT JOIN counterparty c ON c.id_counterparty = la.id_counterparty
LEFT JOIN program p ON p.id_program = la.id_program
LEFT JOIN workers w ON w.id_workers = la.id_workers";

            if (onlyApproved)
            {
                query += " WHERE la.approved = 'Утверждён'";
            }

            query += " ORDER BY la.id_license_agreement DESC";

            DataTable dataTable = SelectQuery(query);
            if (!dataTable.Columns.Contains("services_list"))
            {
                dataTable.Columns.Add("services_list", typeof(string));
            }
            foreach (DataRow row in dataTable.Rows)
            {
                int idLicense = Convert.ToInt32(row["id_license_agreement"]);
                string servicesList = GetServicesForLicense(idLicense);
                row["services_list"] = servicesList;
            }

            return dataTable;
        }

        public static DataTable LoadLeaseAgreementsToList(bool onlyApproved = false)
        {
            string query = @"
SELECT 
    lease_agreement.id_lease_agreement,
    counterparty.name as 'counterpartyFirm', 
    counterparty.name as 'cont_person_name',
    company.company,
    company.actual_address, 
    concat(company.series_COPC, ' ', company.number_COPC) as 'COPC', 
    workers.name AS 'worker',
    lease_agreement.rental_date_from, 
    lease_agreement.rental_date_until, 
    lease_agreement.conclusion_date,
    floor.floor, 
    office.office, 
    room.square, 
    pm.price AS price_per_m2, 
    (room.square * pm.price) AS price,  
    ((room.square * pm.price) / 100 * 95) as total_price,
    lease_agreement.approved, 
    company.BIC, 
    company.INN,
    company.Correspondent_account, 
    company.Payment_account,
    CONCAT('№', lease_agreement.id_lease_agreement, ' | ', counterparty.name) as DisplayName
FROM rccs.lease_agreement
JOIN counterparty 
    ON lease_agreement.id_counterparty = counterparty.id_counterparty
JOIN workers 
    ON lease_agreement.id_workers = workers.id_workers
JOIN company 
    ON workers.id_company = company.id_company
JOIN room 
    ON lease_agreement.id_room = room.id_room
JOIN office 
    ON room.id_office = office.id_office
JOIN floor 
    ON room.id_floor = floor.id_floor   
LEFT JOIN (
    SELECT pm1.*
    FROM rccs.price_meter pm1
    JOIN (
        SELECT id_room, MAX(date) AS max_date
        FROM rccs.price_meter
        GROUP BY id_room
    ) pm2 
    ON pm1.id_room = pm2.id_room 
    AND pm1.date = pm2.max_date
) pm 
ON room.id_room = pm.id_room";

            if (onlyApproved)
            {
                query += " WHERE lease_agreement.approved = 'Утверждён'";
            }

            query += " ORDER BY lease_agreement.id_lease_agreement DESC";

            return SelectQuery(query);
        }

        public static string GetServicesForLicense(int idLicenseAgreement)
        {
            string sql = @"
SELECT CONCAT(services.name, ' : ', service_in_agreement.kolvo, ' шт.') as name
FROM rccs.service_in_agreement
JOIN rccs.services ON services.id_services = service_in_agreement.id_services
WHERE service_in_agreement.id_license_agreement = @id";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@id", idLicenseAgreement);

            DataTable dt = new DataTable();
            ConnectionBD.myDataAdapter.Fill(dt);

            if (dt.Rows.Count == 0)
                return "Услуги не указаны";

            string result = "";
            foreach (DataRow row in dt.Rows)
            {
                result += "• " + row["name"].ToString() + "\n";
            }

            return result.TrimEnd('\n');
        }

        public static DataTable SelectQuery(string query)
        {
            DataTable dataTable = new DataTable();

            try
            {
                if (ConnectionBD.myconnection == null)
                {
                    throw new Exception("Подключение к базе данных не установлено!");
                }

                if (ConnectionBD.myconnection.State != ConnectionState.Open)
                    ConnectionBD.myconnection.Open();

                using (MySqlCommand command = new MySqlCommand(query, ConnectionBD.myconnection))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                ConnectionBD.myconnection.Close();
                return dataTable;
            }
            catch (Exception ex)
            {
                try { ConnectionBD.myconnection.Close(); } catch { }
                throw new Exception($"Ошибка выполнения запроса: {ex.Message}");
            }
        }
    }
}
