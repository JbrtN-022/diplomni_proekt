using rccs.MyClass;
using System;
using System.Collections.Generic;
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
    }
}
