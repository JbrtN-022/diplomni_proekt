using rccs.MyClass;
using System;
using System.Data;
using System.Windows.Controls;

namespace rccs_new.MyClass
{
    internal class licenseAgreement
    {
        
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

               

                var card = new UserControlLicenseAgreement(
                    licenseId: row["id_license_agreement"].ToString(),
                    counterpartyName: row["counterparty_name"].ToString(),
                    programName: row["program_name"].ToString(),
                    dateFrom: Convert.ToDateTime(row["rental_date_from"]).ToShortDateString(),
                    dateUntil: Convert.ToDateTime(row["rental_date_until"]).ToShortDateString(),
                    conclusionDate: Convert.ToDateTime(row["conclusion_date"]).ToShortDateString(),
                    workerName: row["worker_name"].ToString(),
                    servicesList: servicesList,
                    approved: row["approved"].ToString()
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
    }
}