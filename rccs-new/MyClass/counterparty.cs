using MySql.Data.MySqlClient;
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
    internal class counterparty
    {
        public static void SelectCounterparty(ItemsControl itemsControl, string search = "", int cityId = 0, int typeId = 0)
        {
            itemsControl.Items.Clear();

            string sql = @"
        SELECT 
            counterparty.id_counterparty,
            counterparty.name,
            city.city,
            counterparty.actual_address,
            counterparty.legal_address,
            counterparty.`e-mail`,
            counterparty.web_page,
            counterparty.INN,
            counterparty.BIC,
            type_of_face.type_of_face,
            counterparty.cont_person_name,
            counterparty.cont_person_phone
        FROM rccs.counterparty
        JOIN rccs.city        ON city.id_city = counterparty.id_city
        JOIN rccs.type_of_face ON type_of_face.id_type_of_face = counterparty.id_type_of_face
        WHERE 1=1";

            // Поиск по всем полям
            if (!string.IsNullOrEmpty(search))
            {
                sql += @"
            AND (
                counterparty.name LIKE @search OR
                city.city LIKE @search OR
                counterparty.actual_address LIKE @search OR
                counterparty.legal_address LIKE @search OR
                counterparty.`e-mail` LIKE @search OR
                counterparty.web_page LIKE @search OR
                counterparty.INN LIKE @search OR
                counterparty.BIC LIKE @search OR
                type_of_face.type_of_face LIKE @search OR
                counterparty.cont_person_name LIKE @search OR
                counterparty.cont_person_phone LIKE @search
            )";
            }

            // Фильтр по городу
            if (cityId > 0)
            {
                sql += " AND city.id_city = @cityId";
            }

            // Фильтр по типу лица
            if (typeId > 0)
            {
                sql += " AND type_of_face.id_type_of_face = @typeId";
            }



            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();

            if (!string.IsNullOrEmpty(search))
                ConnectionBD.mycommand.Parameters.AddWithValue("@search", "%" + search + "%");

            if (cityId > 0)
                ConnectionBD.mycommand.Parameters.AddWithValue("@cityId", cityId);

            if (typeId > 0)
                ConnectionBD.mycommand.Parameters.AddWithValue("@typeId", typeId);

            ConnectionBD.dtCounterparty.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtCounterparty);

            foreach (DataRow row in ConnectionBD.dtCounterparty.Rows)
            {
                var card = new UserControlCounterparty(
                    name: row["name"]?.ToString() ?? "",
                    city: row["city"]?.ToString() ?? "",
                    typeFace: row["type_of_face"]?.ToString() ?? "",
                    actualAddress: row["actual_address"]?.ToString() ?? "",
                    legalAddress: row["legal_address"]?.ToString() ?? "",
                    email: row["e-mail"]?.ToString() ?? "",
                    phone: row["cont_person_phone"]?.ToString() ?? "",
                    inn: row["INN"]?.ToString() ?? "",
                    bic: row["BIC"]?.ToString() ?? "",
                    contPerson: row["cont_person_name"]?.ToString() ?? ""
                );

                itemsControl.Items.Add(card);
            }
        }
    }
}
