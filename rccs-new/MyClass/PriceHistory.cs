using rccs.MyClass;
using System.Data;
using System.Windows.Controls;

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

        

        public static void LoadPriceMeter(DataGrid dg)
        {
            string sql = @"
                SELECT 
                    o.office AS 'Объект',
                    DATE_FORMAT(pm.date, '%d-%m-%Y') AS 'Дата',
                    pm.price AS 'Прайс'
                FROM rccs.price_meter pm
                JOIN rccs.room r 
                    ON r.id_room = pm.id_room
                JOIN rccs.office o 
                    ON o.id_office = r.id_office

                ORDER BY o.office ASC, pm.date DESC";

            FillDataGrid(dg, sql);
        }

        

        public static void LoadPricePrograms(DataGrid dg)
        {
            string sql = @"
                SELECT 
                    p.name AS 'Объект',
                    DATE_FORMAT(pp.date, '%d-%m-%Y') AS 'Дата',
                  
                    pp.price AS 'Прайс'
                FROM rccs.price_programs pp
                JOIN rccs.program p 
                    ON p.id_program = pp.id_programs

                ORDER BY p.name ASC, pp.date DESC";

            FillDataGrid(dg, sql);
        }

      
        public static void LoadPriceServices(DataGrid dg)
        {
            string sql = @"
                SELECT 
                    s.name AS 'Объект',
                    DATE_FORMAT(ps.date, '%d-%m-%Y') AS 'Дата',
                    ps.price AS 'Прайс'
                FROM rccs.price_services ps
                JOIN rccs.services s 
                    ON s.id_services = ps.id_services

                ORDER BY s.name ASC, ps.date DESC";

            FillDataGrid(dg, sql);
        }

       

        private static void FillDataGrid(DataGrid dg, string sql)
        {
            DataTable dt = new DataTable();

            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.CommandText = sql;

            ConnectionBD.myDataAdapter.SelectCommand = ConnectionBD.mycommand;

            ConnectionBD.myDataAdapter.Fill(dt);

            dg.ItemsSource = dt.DefaultView;
        }

      

        public static bool AddPrice(
            PriceType priceType,
            decimal price,
            int linkId)
        {
            string sql = "";

            switch (priceType)
            {
                case PriceType.Meter:

                    sql = @"
                    INSERT INTO rccs.price_meter
                    (
                        price,
                        date,
                        id_room
                    )
                    VALUES
                    (
                        @price,
                        NOW(),
                        @linkId
                    )";

                    break;

                case PriceType.Program:

                    sql = @"
                    INSERT INTO rccs.price_programs
                    (
                        price,
                        date,
                        id_programs
                    )
                    VALUES
                    (
                        @price,
                        NOW(),
                        @linkId
                    )";

                    break;

                case PriceType.Service:

                    sql = @"
                    INSERT INTO rccs.price_services
                    (
                        price,
                        date,
                        id_services
                    )
                    VALUES
                    (
                        @price,
                        NOW(),
                        @linkId
                    )";

                    break;
            }

            ConnectionBD.mycommand.Parameters.Clear();

            ConnectionBD.mycommand.CommandText = sql;

            ConnectionBD.mycommand.Parameters.AddWithValue("@price", price);

            ConnectionBD.mycommand.Parameters.AddWithValue("@linkId", linkId);

            int result =
                ConnectionBD.mycommand.ExecuteNonQuery();

            return result > 0;
        }

        public static bool DeletePrice(
            PriceType priceType,
            string objectName,
            decimal price,
            string date)
        {
            string sql = "";

            switch (priceType)
            {
                case PriceType.Meter:

                    sql = @"
                    DELETE pm
                    FROM rccs.price_meter pm
                    JOIN rccs.room r 
                        ON r.id_room = pm.id_room
                    JOIN rccs.office o 
                        ON o.id_office = r.id_office
                    WHERE 
                        o.office = @objectName
                        AND pm.price = @price
                        AND pm.date = @date";

                    break;

                case PriceType.Program:

                    sql = @"
                    DELETE pp
                    FROM rccs.price_programs pp
                    JOIN rccs.program p 
                        ON p.id_program = pp.id_programs
                    WHERE 
                        p.name = @objectName
                        AND pp.price = @price
                        AND pp.date = @date";

                    break;

                case PriceType.Service:

                    sql = @"
                    DELETE ps
                    FROM rccs.price_services ps
                    JOIN rccs.services s 
                        ON s.id_services = ps.id_services
                    WHERE 
                        s.name = @objectName
                        AND ps.price = @price
                        AND ps.date = @date";

                    break;
            }

            ConnectionBD.mycommand.Parameters.Clear();

            ConnectionBD.mycommand.CommandText = sql;

            ConnectionBD.mycommand.Parameters.AddWithValue("@objectName", objectName);

            ConnectionBD.mycommand.Parameters.AddWithValue("@price", price);

            ConnectionBD.mycommand.Parameters.AddWithValue("@date", date);

            int result =
                ConnectionBD.mycommand.ExecuteNonQuery();

            return result > 0;
        }
    }
}