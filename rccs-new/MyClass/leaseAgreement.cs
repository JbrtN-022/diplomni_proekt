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
        public static void leaseAgreementSelect(ItemsControl itemsControl)
        {
            leaseAgreementSelect(itemsControl, "", null,  null, null);
        }
        public static void leaseAgreementSelect(ItemsControl itemsControl,
             string searchText,
             int? workerId,
             DateTime? dateFrom,
             DateTime? dateTo)
        {
            itemsControl.Items.Clear();

            string sql = @"
               SELECT lease_agreement.id_lease_agreement,
                counterparty.name,
                workers.name as 'worker',
                lease_agreement.rental_date_from,
                lease_agreement.rental_date_until,
                lease_agreement.conclusion_date,
                office.office,
                lease_agreement.approved
                 FROM rccs.lease_agreement
                 join counterparty on lease_agreement.id_counterparty = counterparty.id_counterparty
                join workers on lease_agreement.id_workers = workers.id_workers
                join room on lease_agreement.id_room = room.id_room
                join office on room.id_office = office.id_office
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
                int idLicense = Convert.ToInt32(row["id_lease_agreement"]);




                var card = new UserControlLease(
                    leaseID: row["id_lease_agreement"].ToString(),
                    counterpartyName: row["name"].ToString(),
                    roomNum: row["office"].ToString(),
                    dateFrom: Convert.ToDateTime(row["rental_date_from"]).ToShortDateString(),
                    dateUntil: Convert.ToDateTime(row["rental_date_until"]).ToShortDateString(),
                    conclusionDate: Convert.ToDateTime(row["conclusion_date"]).ToShortDateString(),
                    workerName: row["worker"].ToString(),

                     approved: row["approved"].ToString()
                );

                itemsControl.Items.Add(card);
            }
        }
    }
}
