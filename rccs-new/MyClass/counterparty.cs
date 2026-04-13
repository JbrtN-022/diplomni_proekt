using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using rccs.MyClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

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
                    id: row["id_counterparty"]?.ToString() ?? "",
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
        public static bool AddCounterparty(string name, int id_city, string actual_address, string legal_address,
                                                   string email, string web_page, string inn, string bic,
                                                   int id_type_of_face, string cont_person_name, string cont_person_phone)
        {
            
            if (IsDuplicate(name, inn , legal_address))
            {
                MessageBox.Show("Контрагент с таким названием или ИНН уже существует!", "Дубликат",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            string sql = @"
                INSERT INTO rccs.counterparty 
                    (name, id_city, actual_address, legal_address, `e-mail`, web_page, INN, BIC, 
                     id_type_of_face, cont_person_name, cont_person_phone)
                VALUES 
                    (@name, @id_city, @actual_address, @legal_address, @email, @web_page, 
                     @INN, @BIC, @id_type_of_face, @cont_person_name, @cont_person_phone)";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();

            ConnectionBD.mycommand.Parameters.AddWithValue("@name", name);
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_city", id_city);
            ConnectionBD.mycommand.Parameters.AddWithValue("@actual_address", actual_address ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@legal_address", legal_address ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@email", email ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@web_page", web_page ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@INN", inn ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@BIC", bic ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_type_of_face", id_type_of_face);
            ConnectionBD.mycommand.Parameters.AddWithValue("@cont_person_name", cont_person_name ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@cont_person_phone", cont_person_phone ?? "");

            int result = ConnectionBD.mycommand.ExecuteNonQuery();
            return result > 0;
        }


        public static bool UpdateCounterparty(int id_counterparty, string name, int id_city, string actual_address,
                                              string legal_address, string email, string web_page, string inn,
                                              string bic, int id_type_of_face, string cont_person_name, string cont_person_phone)
        {
            string sql = @"
                UPDATE rccs.counterparty 
                SET name = @name,
                    id_city = @id_city,
                    actual_address = @actual_address,
                    legal_address = @legal_address,
                    `e-mail` = @email,
                    web_page = @web_page,
                    INN = @INN,
                    BIC = @BIC,
                    id_type_of_face = @id_type_of_face,
                    cont_person_name = @cont_person_name,
                    cont_person_phone = @cont_person_phone
                WHERE id_counterparty = @id_counterparty";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();

            ConnectionBD.mycommand.Parameters.AddWithValue("@id_counterparty", id_counterparty);
            ConnectionBD.mycommand.Parameters.AddWithValue("@name", name);
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_city", id_city);
            ConnectionBD.mycommand.Parameters.AddWithValue("@actual_address", actual_address ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@legal_address", legal_address ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@email", email ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@web_page", web_page ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@INN", inn ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@BIC", bic ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_type_of_face", id_type_of_face);
            ConnectionBD.mycommand.Parameters.AddWithValue("@cont_person_name", cont_person_name ?? "");
            ConnectionBD.mycommand.Parameters.AddWithValue("@cont_person_phone", cont_person_phone ?? "");

            int result = ConnectionBD.mycommand.ExecuteNonQuery();
            return result > 0;
        }


        public static bool DeleteCounterparty(int id_counterparty)
        {
            
            string checkSql = $"SELECT Count(*) FROM rccs.license_agreement where id_counterparty= {id_counterparty}";
            ConnectionBD.mycommand.CommandText = checkSql;
            int count = Convert.ToInt32(ConnectionBD.mycommand.ExecuteScalar());

            if (count > 0)
            {
                MessageBox.Show("Нельзя удалить контрагента, так как он используется в помещениях.",
                                "Запрет удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            string checkSql1 = $"SELECT count(*) FROM rccs.license_agreement where id_counterparty={id_counterparty}";
            ConnectionBD.mycommand.CommandText = checkSql;
            int count1 = Convert.ToInt32(ConnectionBD.mycommand.ExecuteScalar());

            if (count1 > 0)
            {
                MessageBox.Show("Нельзя удалить контрагента, так как он используется в помещениях.",
                                "Запрет удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            string sql = "DELETE FROM rccs.counterparty WHERE id_counterparty = @id";
            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@id", id_counterparty);

            int result = ConnectionBD.mycommand.ExecuteNonQuery();
            return result > 0;
        }

        public static bool IsDuplicate(string name, string inn, string legalAdress)
        {

            string sql = "SELECT COUNT(*) FROM rccs.counterparty WHERE name = @name and INN = @inn and legal_address = @legal_address";
            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@name", name);
            ConnectionBD.mycommand.Parameters.AddWithValue("@inn", inn);
            ConnectionBD.mycommand.Parameters.AddWithValue("@legal_address", legalAdress);

            int count = Convert.ToInt32(ConnectionBD.mycommand.ExecuteScalar());
            return count > 0;
        }

    }
}
