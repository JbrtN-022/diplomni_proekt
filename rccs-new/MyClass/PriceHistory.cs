using rccs.MyClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static rccs_new.форма_прайса;

namespace rccs_new.MyClass
{
    internal class PriceHistory
    {
        public enum PriceType
        {
            Meter = 1,
            Program = 2,
            Service = 3
        }

        public static void LoadPriceData(DataGrid dg, PriceType type)
        {
            switch (type)
            {
                case PriceType.Meter:
                    LoadPriceMeter(dg);
                    break;
                case PriceType.Program:
                    LoadPricePrograms(dg);
                    break;
                case PriceType.Service:
                    LoadPriceServices(dg);
                    break;
            }
        }

        //сделать обработку с помещениями

        public static void LoadPriceMeter(DataGrid dg)
        {
            string sql = @"
                SELECT 
                    pm.id_price_meter as ID,
                    pm.price as Цена,
                    pm.date as Дата,
                    o.office  as Помещение 
                FROM rccs.price_meter pm
                JOIN rccs.room r ON r.id_room = pm.id_room
                JOIN rccs.office o ON o.id_office = r.id_office
                ORDER BY pm.date DESC";

            FillDataGrid(dg, sql);
        }

        public static void LoadPricePrograms(DataGrid dg)
        {
            string sql = @"
                SELECT 
                    pp.id_price_programs as ID,
                    pp.price as Цена,
                    pp.date as Дата,
                    p.name as Программа
                FROM rccs.price_programs pp
                JOIN rccs.program p ON p.id_program = pp.id_programs
                ORDER BY pp.date DESC";

            FillDataGrid(dg, sql);
        }

        public static void LoadPriceServices(DataGrid dg)
        {
            string sql = @"
                SELECT 
                    ps.id_price_services as ID,
                    ps.price as Цена,
                    ps.date as Дата,
                    s.name as Услуга
                FROM rccs.price_services ps
                JOIN rccs.services s ON s.id_services = ps.id_services
                ORDER BY ps.date DESC";

            FillDataGrid(dg, sql);
        }

        // Универсальный метод заполнения DataGrid
        private static void FillDataGrid(DataGrid dg, string sql)
        {
            dg.Columns.Clear();
            dg.ItemsSource = null;

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.dtTemp.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

            dg.ItemsSource = ConnectionBD.dtTemp.DefaultView;
        }

        // ====================== ДОБАВЛЕНИЕ ======================
        public static bool AddPrice(PriceType priceType, decimal price, int linkId)
        {
            string sql = "";

            switch (priceType)
            {
                case PriceType.Meter:
                    sql = "INSERT INTO rccs.price_meter (price, date, id_room) VALUES (@price, NOW(), @linkId)";
                    break;
                case PriceType.Program:
                    sql = "INSERT INTO rccs.price_programs (price, date, id_programs) VALUES (@price, NOW(), @linkId)";
                    break;
                case PriceType.Service:
                    sql = "INSERT INTO rccs.price_services (price, date, id_services) VALUES (@price, NOW(), @linkId)";
                    break;
            }

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@price", price);
            ConnectionBD.mycommand.Parameters.AddWithValue("@linkId", linkId);

            int result = ConnectionBD.mycommand.ExecuteNonQuery();
            return result > 0;
        }

        // ====================== УДАЛЕНИЕ ======================
        public static bool DeletePrice(PriceType priceType, int id)
        {
            string sql = "";

            switch (priceType)
            {
                case PriceType.Meter:
                    sql = "DELETE FROM rccs.price_meter WHERE id_price_meter = @id";
                    break;
                case PriceType.Program:
                    sql = "DELETE FROM rccs.price_programs WHERE id_price_programs = @id";
                    break;
                case PriceType.Service:
                    sql = "DELETE FROM rccs.price_services WHERE id_price_services = @id";
                    break;
            }

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@id", id);

            int result = ConnectionBD.mycommand.ExecuteNonQuery();
            return result > 0;
        }
    }
}

