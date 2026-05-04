using MySql.Data.MySqlClient;
using rccs.MyClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace rccs_new.MyClass
{
    internal class licenseAgreement
    {
        public class ServiceItem
        {
            public bool IsSelected { get; set; } = false;
            public int IdServices { get; set; }
            public string Name { get; set; }
            public int? IdWorker { get; set; }   
        }
        public static void licenseAgreementSelect(ItemsControl itemsControl)
        {
            licenseAgreementSelect(itemsControl, "", null, null, null, null);
        }


        public static void licenseAgreementSelect(ItemsControl itemsControl,
             string searchText,
             int? workerId,
             int? programId,
             DateTime? dateFrom,
             DateTime? dateTo)
        {
            itemsControl.Items.Clear();

            string sql = @"
                SELECT 
                    license_agreement.id_license_agreement,
                    counterparty.name AS counterparty_name,
                    program.name AS program_name,
                    license_agreement.rental_date_from,
                    license_agreement.rental_date_until,
                    license_agreement.conclusion_date,
                    workers.name AS worker_name,
                    license_agreement.approved
                FROM rccs.license_agreement
                JOIN rccs.counterparty ON counterparty.id_counterparty = license_agreement.id_counterparty
                JOIN rccs.program ON program.id_program = license_agreement.id_program
                JOIN rccs.workers ON workers.id_workers = license_agreement.id_workers
                WHERE 1=1";

            if (!string.IsNullOrWhiteSpace(searchText))
                sql += " AND ( license_agreement.id_license_agreement  LIKE @search OR counterparty.name LIKE @search OR program.name LIKE @search)";

            if (workerId.HasValue)
                sql += " AND license_agreement.id_workers = @workerId";

            if (programId.HasValue)
                sql += " AND license_agreement.id_program = @programId";

            if (dateFrom.HasValue)
                sql += " AND license_agreement.conclusion_date >= @dateFrom";

            if (dateTo.HasValue)
                sql += " AND license_agreement.conclusion_date <= @dateTo";

            sql += " ORDER BY license_agreement.conclusion_date DESC";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();

            if (!string.IsNullOrWhiteSpace(searchText))
                ConnectionBD.mycommand.Parameters.AddWithValue("@search", "%" + searchText + "%");

            if (workerId.HasValue) ConnectionBD.mycommand.Parameters.AddWithValue("@workerId", workerId.Value);
            if (programId.HasValue) ConnectionBD.mycommand.Parameters.AddWithValue("@programId", programId.Value);
            if (dateFrom.HasValue) ConnectionBD.mycommand.Parameters.AddWithValue("@dateFrom", dateFrom.Value);
            if (dateTo.HasValue) ConnectionBD.mycommand.Parameters.AddWithValue("@dateTo", dateTo.Value);

            ConnectionBD.dtTempLicense.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTempLicense);

            foreach (DataRow row in ConnectionBD.dtTempLicense.Rows)
            {
                int idLicense = Convert.ToInt32(row["id_license_agreement"]);
                string servicesList = GetServicesForLicense(idLicense);

                string dateFromStr = row["rental_date_from"] != DBNull.Value
            ? Convert.ToDateTime(row["rental_date_from"]).ToShortDateString()
            : "-";

                string dateUntilStr = row["rental_date_until"] != DBNull.Value
                    ? Convert.ToDateTime(row["rental_date_until"]).ToShortDateString()
                    : "-";

                string conclusionDateStr = row["conclusion_date"] != DBNull.Value
                    ? Convert.ToDateTime(row["conclusion_date"]).ToShortDateString()
                    : "-";

                string approvedText = row["approved"]?.ToString() ?? "Не утверждён";

                var card = new UserControlLicenseAgreement(
            licenseId: idLicense.ToString(),
            counterpartyName: row["counterparty_name"]?.ToString() ?? "-",
            programName: row["program_name"]?.ToString() ?? "-",
            dateFrom: dateFromStr,
            dateUntil: dateUntilStr,
            conclusionDate: conclusionDateStr,
            workerName: row["worker_name"]?.ToString() ?? "-",
            servicesList: servicesList,
            approved: approvedText
        );

                itemsControl.Items.Add(card);
            }
        }


        public static string GetServicesForLicense(int idLicenseAgreement)
        {
            string sql = @"
                SELECT services.name
                FROM rccs.service_in_agreement
                JOIN rccs.services ON services.id_services = service_in_agreement.id_services
                WHERE service_in_agreement.id_license_agreement = @id";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@id", idLicenseAgreement);

            ConnectionBD.dtTemp.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

            if (ConnectionBD.dtTemp.Rows.Count == 0)
                return "Услуги не указаны";

            string result = "";
            foreach (DataRow row in ConnectionBD.dtTemp.Rows)
            {
                result += "• " + row["name"].ToString() + "\n";
            }

            return result.TrimEnd('\n');
        }
        public static int AddlicenseAgreement(string id, int IDcounterparty, int IDprogram,
                                      DateTime dateFrom, DateTime dateUntil, int idWorker)
        {
            try
            {
                string sql = @"
            INSERT INTO rccs.license_agreement 
            (id_license_agreement, id_counterparty, id_program, 
             rental_date_from, rental_date_until, conclusion_date, 
             id_workers, approved)
            VALUES 
            (@id_license_agreement, @id_counterparty, @id_program, 
             @rental_date_from, @rental_date_until, @conclusion_date, 
             @id_workers, 'Утверждён')";

                ConnectionBD.mycommand.CommandText = sql;
                ConnectionBD.mycommand.Parameters.Clear();

                ConnectionBD.mycommand.Parameters.AddWithValue("@id_license_agreement", id);
                ConnectionBD.mycommand.Parameters.AddWithValue("@id_counterparty", IDcounterparty);
                ConnectionBD.mycommand.Parameters.AddWithValue("@id_program", IDprogram);
                ConnectionBD.mycommand.Parameters.AddWithValue("@rental_date_from", dateFrom);
                ConnectionBD.mycommand.Parameters.AddWithValue("@rental_date_until", dateUntil);
                ConnectionBD.mycommand.Parameters.AddWithValue("@conclusion_date", DateTime.Today);
                ConnectionBD.mycommand.Parameters.AddWithValue("@id_workers", idWorker);

                int rowsAffected = ConnectionBD.mycommand.ExecuteNonQuery();

                // Возвращаем id_license_agreement, который мы сами задали
                return rowsAffected > 0 ? Convert.ToInt32(id) : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении лицензии:\n" + ex.Message, "Ошибка БД",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }
        public static int SaveAddlicenseAgreement(string id, int IDcounterparty, int IDprogram,
                                          DateTime dateFrom, DateTime dateUntil, int idWorker)
        {
            try
            {
                string sql = @"
            INSERT INTO rccs.license_agreement 
            (id_license_agreement, id_counterparty, id_program, 
             rental_date_from, rental_date_until, 
             id_workers, approved)
            VALUES 
            (@id_license_agreement, @id_counterparty, @id_program, 
             @rental_date_from, @rental_date_until, 
             @id_workers, 'Не утверждён')";

                ConnectionBD.mycommand.CommandText = sql;
                ConnectionBD.mycommand.Parameters.Clear();

                ConnectionBD.mycommand.Parameters.AddWithValue("@id_license_agreement", id);
                ConnectionBD.mycommand.Parameters.AddWithValue("@id_counterparty", IDcounterparty);
                ConnectionBD.mycommand.Parameters.AddWithValue("@id_program", IDprogram);
                ConnectionBD.mycommand.Parameters.AddWithValue("@rental_date_from", dateFrom);
                ConnectionBD.mycommand.Parameters.AddWithValue("@rental_date_until", dateUntil);
                ConnectionBD.mycommand.Parameters.AddWithValue("@id_workers", idWorker);

                int rowsAffected = ConnectionBD.mycommand.ExecuteNonQuery();

                return rowsAffected > 0 ? Convert.ToInt32(id) : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении черновика:\n" + ex.Message, "Ошибка БД",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }
        public static bool AddServicesToLicense(int idLicenseAgreement, List<ServiceItem> selectedServices)
        {
            if (selectedServices == null || selectedServices.Count == 0)
                return false;

            foreach (var service in selectedServices)
            {
                string sql = @"
            INSERT INTO rccs.service_in_agreement 
            (id_license_agreement, id_services, id_workers)
            VALUES (@idLicense, @idService, @idWorker)";

                ConnectionBD.mycommand.CommandText = sql;
                ConnectionBD.mycommand.Parameters.Clear();

                ConnectionBD.mycommand.Parameters.AddWithValue("@idLicense", idLicenseAgreement);
                ConnectionBD.mycommand.Parameters.AddWithValue("@idService", service.IdServices);
                ConnectionBD.mycommand.Parameters.AddWithValue("@idWorker", service.IdWorker ?? 1); 

                ConnectionBD.mycommand.ExecuteNonQuery();
            }

            return true;
        }

        public static void LoadAllServices(DataGrid dgServices, DataGrid dgWorkersSource = null)
        {
            string sql = "SELECT id_services, name FROM rccs.services ORDER BY name";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.dtTemp.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

            var serviceList = new List<ServiceItem>();

            foreach (DataRow row in ConnectionBD.dtTemp.Rows)
            {
                serviceList.Add(new ServiceItem
                {
                    IdServices = Convert.ToInt32(row["id_services"]),
                    Name = row["name"].ToString(),
                    IsSelected = false
                });
            }

            dgServices.ItemsSource = serviceList;
            guideBD.selectWorkers();

            if (dgWorkersSource != null)
            {
              
                var workerColumn = dgServices.Columns[2] as DataGridComboBoxColumn; 
                if (workerColumn != null)
                {
                    workerColumn.ItemsSource = ConnectionBD.dtWorkersComboBox.DefaultView;
                    workerColumn.DisplayMemberPath = "name";
                    workerColumn.SelectedValuePath = "id_workers";
                    workerColumn.SelectedValueBinding = new Binding("IdWorker");
                }
            }
        }

        public static void LoadDraftLicensesComboBox(ComboBox cmbDraft)
        {
           

            string sql = @"
        SELECT 
            id_license_agreement,
            CONCAT(id_license_agreement, ' — ', counterparty.name, ': ', program.name) AS display_name
        FROM rccs.license_agreement
        JOIN rccs.counterparty ON counterparty.id_counterparty = license_agreement.id_counterparty
        JOIN rccs.program ON program.id_program = license_agreement.id_program
        WHERE license_agreement.approved = 'Не утверждён'
        ORDER BY license_agreement.conclusion_date DESC";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();

            ConnectionBD.dtTemp.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

            cmbDraft.ItemsSource = ConnectionBD.dtTemp.DefaultView;
            cmbDraft.DisplayMemberPath = "display_name";
            cmbDraft.SelectedValuePath = "id_license_agreement";
        }

        public static DataRow LoadDraftById(int idLicenseAgreement)
        {
            string sql = @"
        SELECT 
            id_license_agreement,
            id_counterparty,
            id_program,
            rental_date_from,
            rental_date_until,
            conclusion_date,
            id_workers,
            approved
        FROM rccs.license_agreement 
        WHERE id_license_agreement = @id";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@id", idLicenseAgreement);

            ConnectionBD.dtTemp.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

            return ConnectionBD.dtTemp.Rows.Count > 0 ? ConnectionBD.dtTemp.Rows[0] : null;
        }

       
        public static bool ApproveDraft(int idLicenseAgreement)
        {
            string sql = @"
        UPDATE rccs.license_agreement 
        SET approved = 'Утверждён', 
            conclusion_date = CURDATE()
        WHERE id_license_agreement = @id";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@id", idLicenseAgreement);

            int result = ConnectionBD.mycommand.ExecuteNonQuery();
            return result > 0;
        }
    }
}