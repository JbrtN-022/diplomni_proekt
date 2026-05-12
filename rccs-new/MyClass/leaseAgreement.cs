using rccs.MyClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace rccs_new.MyClass
{
    internal class leaseAgreement
    {
        public class ServiceItem
        {
            public bool IsSelected { get; set; } = false;
            public int IdServices { get; set; }
            public string Name { get; set; }
            public int? IdWorker { get; set; }
        }

        public static void leaseAgreementSelect(ItemsControl itemsControl)
        {
            leaseAgreementSelect(itemsControl, "", null, null, null);
        }
        public static void leaseAgreementSelect(ItemsControl itemsControl,
             string searchText,
             int? workerId,
             DateTime? dateFrom,
             DateTime? dateTo)
        {
            itemsControl.Items.Clear();

            string sql = @"
                  SELECT 
                lease_agreement.id_lease_agreement,
                counterparty.name as 'counterpartyFirm', counterparty.cont_person_name,
                company.company,company.actual_address, concat(company.series_COPC , ' ', company.number_COPC) as 'COPC', workers.name AS 'worker',
                lease_agreement.rental_date_from, lease_agreement.rental_date_until, lease_agreement.conclusion_date,
                floor.floor, office.office, 
                room.square, pm.price AS price_per_m2, (room.square * pm.price) AS price,  ((room.square * pm.price) /100 * 95) as total_price,
                lease_agreement.approved, 	company.BIC, company.INN,company.Correspondent_account, company.Payment_account
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
            ON room.id_room = pm.id_room
            WHERE 1=1";

            if (!string.IsNullOrWhiteSpace(searchText))
                sql += " AND ( workers.name LIKE @search OR counterparty.name LIKE @search OR  office.office LIKE @search)";

            if (workerId.HasValue)
                sql += " AND lease_agreement.id_workers = @workerId";

            if (dateFrom.HasValue)
                sql += " AND lease_agreement.conclusion_date >= @dateFrom";

            if (dateTo.HasValue)
                sql += " AND lease_agreement.conclusion_date <= @dateTo";

            sql += " ORDER BY lease_agreement.conclusion_date DESC";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();

            if (!string.IsNullOrWhiteSpace(searchText))
                ConnectionBD.mycommand.Parameters.AddWithValue("@search", "%" + searchText + "%");

            if (workerId.HasValue) ConnectionBD.mycommand.Parameters.AddWithValue("@workerId", workerId.Value);

            if (dateFrom.HasValue) ConnectionBD.mycommand.Parameters.AddWithValue("@dateFrom", dateFrom.Value);
            if (dateTo.HasValue) ConnectionBD.mycommand.Parameters.AddWithValue("@dateTo", dateTo.Value);

            ConnectionBD.dtTempLease.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTempLease);

            foreach (DataRow row in ConnectionBD.dtTempLease.Rows)
            {
                var card = new UserControlLease(row);
                itemsControl.Items.Add(card);
            }
        }

        public static DataRow GetLeaseDataForPrint(int idLeaseAgreement)
        {
            string sql = @"
        SELECT
            lease_agreement.id_lease_agreement,
            counterparty.name as counterpartyFirm, 
            counterparty.cont_person_name,
            company.company,
            company.actual_address, 
            CONCAT(company.series_COPC, ' ', company.number_COPC) as COPC, 
            workers.name AS worker,
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
            company.Payment_account
        FROM rccs.lease_agreement
        JOIN counterparty ON lease_agreement.id_counterparty = counterparty.id_counterparty
        JOIN workers ON lease_agreement.id_workers = workers.id_workers
        JOIN company ON workers.id_company = company.id_company
        JOIN room ON lease_agreement.id_room = room.id_room
        JOIN office ON room.id_office = office.id_office
        JOIN floor ON room.id_floor = floor.id_floor
        LEFT JOIN (
            SELECT pm1.*
            FROM rccs.price_meter pm1
            JOIN (
                SELECT id_room, MAX(date) AS max_date
                FROM rccs.price_meter GROUP BY id_room
            ) pm2 ON pm1.id_room = pm2.id_room AND pm1.date = pm2.max_date
        ) pm ON room.id_room = pm.id_room
        WHERE lease_agreement.id_lease_agreement = @id";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@id", idLeaseAgreement);

            ConnectionBD.dtTemp.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

            return ConnectionBD.dtTemp.Rows.Count > 0 ? ConnectionBD.dtTemp.Rows[0] : null;
        }

        public static DataRow LoadDraftById(int idLeaseAgreement)
        {
            string sql = @"
                     SELECT
            lease_agreement.id_lease_agreement,
            lease_agreement.id_counterparty,
            floor.floor       AS floor_name,
            office.office     AS office_name,
            lease_agreement.rental_date_from,
            lease_agreement.rental_date_until,
            lease_agreement.conclusion_date,
            lease_agreement.id_workers,
            lease_agreement.approved
        FROM rccs.lease_agreement
        JOIN room   ON room.id_room = lease_agreement.id_room
        JOIN office ON office.id_office = room.id_office
        JOIN floor  ON floor.id_floor = room.id_floor
        WHERE lease_agreement.id_lease_agreement = @id";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@id", idLeaseAgreement);

            ConnectionBD.dtLoadDraftById.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtLoadDraftById);

            return ConnectionBD.dtLoadDraftById.Rows.Count > 0 ? ConnectionBD.dtLoadDraftById.Rows[0] : null;
        }

        public static void SelectComboBoxRoom(int? floor)
        {
            string sql = @"SELECT r.id_room, o.office
            FROM rccs.room AS r
            JOIN floor AS f ON f.id_floor = r.id_floor
            JOIN office AS o ON o.id_office = r.id_office
            LEFT JOIN lease_agreement AS la ON la.id_room = r.id_room
            WHERE  (
                la.id_room IS NULL
                OR la.rental_date_until <= CURRENT_DATE()
                OR la.approved != 'Утверждён'
              ) ";
            if (floor.HasValue)
            {
                sql += $@" AND f.floor = {floor}";
            }
            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.dtFloorForLeaseComboBox.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtFloorForLeaseComboBox);
        }

        public static bool UpdateLeaseAgreement( int idLeaseAgreement, int idCounterparty, int idRoom, DateTime dateFrom, DateTime dateUntil, string approved)
        {
            try
            {
                string sql = @"
                UPDATE rccs.lease_agreement
                SET
                    id_counterparty = @id_counterparty,
                    id_room = @id_room,
                    rental_date_from = @date_from,
                    rental_date_until = @date_until,
                    approved = @approved
                WHERE id_lease_agreement = @id";

                ConnectionBD.mycommand.CommandText = sql;
                ConnectionBD.mycommand.Parameters.Clear();
                ConnectionBD.mycommand.Parameters.AddWithValue("@id",idLeaseAgreement);
                ConnectionBD.mycommand.Parameters.AddWithValue("@id_counterparty",idCounterparty);
                ConnectionBD.mycommand.Parameters.AddWithValue("@id_room",idRoom);
                ConnectionBD.mycommand.Parameters.AddWithValue("@date_from",dateFrom);
                ConnectionBD.mycommand.Parameters.AddWithValue("@date_until",dateUntil);
                ConnectionBD.mycommand.Parameters.AddWithValue("@approved",approved);
                ConnectionBD.mycommand.ExecuteNonQuery();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public static int SaveAsDraft(string leaseNumber, int idCounterparty, int idRoom,
                                      DateTime dateFrom, DateTime dateUntil, int idWorker)
        {
            string sql = @"
        INSERT INTO rccs.lease_agreement 
        (id_lease_agreement, id_counterparty, id_room, 
         rental_date_from, rental_date_until, conclusion_date,
         id_workers, approved)
        VALUES 
        (@id_lease_agreement, @id_counterparty, @id_room, 
         @rental_date_from, @rental_date_until, CURDATE(), 
         @id_workers, 'Не утверждён')";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();

            ConnectionBD.mycommand.Parameters.AddWithValue("@id_lease_agreement", leaseNumber);
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_counterparty", idCounterparty);
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_room", idRoom);
            ConnectionBD.mycommand.Parameters.AddWithValue("@rental_date_from", dateFrom);
            ConnectionBD.mycommand.Parameters.AddWithValue("@rental_date_until", dateUntil);
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_workers", idWorker);

            int rows = ConnectionBD.mycommand.ExecuteNonQuery();
            return rows > 0 ? Convert.ToInt32(leaseNumber) : 0;
        }

        
        public static int AddLeaseAgreement(string leaseNumber, int idCounterparty, int idRoom,
                                    DateTime dateFrom, DateTime dateUntil, int idWorker)
        {
            string sql = @"
        INSERT INTO rccs.lease_agreement 
        (id_lease_agreement, id_counterparty, id_room, 
         rental_date_from, rental_date_until, conclusion_date, 
         id_workers, approved)
        VALUES 
        (@id_lease_agreement, @id_counterparty, @id_room, 
         @rental_date_from, @rental_date_until, CURDATE(), 
         @id_workers, 'Утверждён')";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();

            ConnectionBD.mycommand.Parameters.AddWithValue("@id_lease_agreement", leaseNumber);
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_counterparty", idCounterparty);
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_room", idRoom);
            ConnectionBD.mycommand.Parameters.AddWithValue("@rental_date_from", dateFrom);
            ConnectionBD.mycommand.Parameters.AddWithValue("@rental_date_until", dateUntil);
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_workers", idWorker);

            int rows = ConnectionBD.mycommand.ExecuteNonQuery();
            return rows > 0 ? Convert.ToInt32(leaseNumber) : 0;
        }

      
        public static bool ApproveDraft(int idLeaseAgreement)
        {
            string sql = @"
        UPDATE rccs.lease_agreement 
        SET approved = 'Утверждён', 
            conclusion_date = CURDATE()
        WHERE id_lease_agreement = @id";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@id", idLeaseAgreement);

            int result = ConnectionBD.mycommand.ExecuteNonQuery();
            return result > 0;
        }

        
        public static void LoadDraftLeases(ComboBox cmbDraft)
        {
            cmbDraft.ItemsSource = null;

            string sql = @"
        SELECT 
            la.id_lease_agreement,
            CONCAT(c.name, ' • каб. ', o.office) AS zapis
        FROM rccs.lease_agreement la
        JOIN rccs.counterparty c ON c.id_counterparty = la.id_counterparty
        JOIN rccs.room r ON r.id_room = la.id_room
        JOIN rccs.office o ON o.id_office = r.id_office
        WHERE la.approved = 'Не утверждён'
        ORDER BY la.id_lease_agreement DESC";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();

            ConnectionBD.dtTemp.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

            cmbDraft.ItemsSource = ConnectionBD.dtTemp.DefaultView;
            cmbDraft.DisplayMemberPath = "zapis";
            cmbDraft.SelectedValuePath = "id_lease_agreement";
        }

    }
}
