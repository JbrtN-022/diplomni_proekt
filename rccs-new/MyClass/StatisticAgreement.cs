using rccs.MyClass;
using System;
using System.Data;
using System.Windows;

namespace rccs_new.MyClass.document
{
    internal class StatisticAgreement
    {
        /// <summary>
        /// Количество арендованных помещений за текущий месяц
        /// </summary>
        public static int GetCurrentMonthLeaseCount()
        {
            string sql = @"
                SELECT COUNT(*) AS total
                FROM lease_agreement
                WHERE MONTH(rental_date_from) = MONTH(CURDATE())
                  AND YEAR(rental_date_from) = YEAR(CURDATE())";

            return ExecuteScalar(sql);
        }

        /// <summary>
        /// Количество арендованных помещений за прошлый месяц
        /// </summary>
        public static int GetPreviousMonthLeaseCount()
        {
            string sql = @"
                SELECT COUNT(*) AS total
                FROM lease_agreement
                WHERE MONTH(rental_date_from) = MONTH(DATE_SUB(CURDATE(), INTERVAL 1 MONTH))
                  AND YEAR(rental_date_from) = YEAR(DATE_SUB(CURDATE(), INTERVAL 1 MONTH))";

            return ExecuteScalar(sql);
        }

        /// <summary>
        /// Процент изменения аренды относительно прошлого месяца
        /// </summary>
        public static decimal GetLeasePercentDifference()
        {
            int current = GetCurrentMonthLeaseCount();
            int previous = GetPreviousMonthLeaseCount();

            if (previous == 0) return 0;

            decimal percent = ((decimal)(current - previous) / previous) * 100;
            return Math.Round(percent, 2);
        }

        /// <summary>
        /// Таблица договоров аренды текущего месяца
        /// </summary>
        public static DataTable GetCurrentMonthLeaseTable()
        {
            string sql = @"
                SELECT
    CONCAT('Этаж: ', f.floor, ' | Офис: ', o.office) AS office_info,
     la.id_lease_agreement,
     la.rental_date_from,
     la.rental_date_until
    
 FROM lease_agreement la
  LEFT JOIN room r ON r.id_room = la.id_room
 LEFT JOIN office o ON o.id_office = r.id_office
 LEFT JOIN floor f ON f.id_floor = r.id_floor
 WHERE MONTH(la.rental_date_from) = MONTH(CURDATE())
   AND YEAR(la.rental_date_from) = YEAR(CURDATE())
 ORDER BY la.rental_date_from DESC";

            return ExecuteQuery(sql);
        }

        /// <summary>
        /// Общая статистика для документа
        /// </summary>
        public static DataTable GetStatisticSummary()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("current_month", typeof(int));
            dt.Columns.Add("previous_month", typeof(int));
            dt.Columns.Add("percent_difference", typeof(decimal));

            DataRow row = dt.NewRow();
            row["current_month"] = GetCurrentMonthLeaseCount();
            row["previous_month"] = GetPreviousMonthLeaseCount();
            row["percent_difference"] = GetLeasePercentDifference();

            dt.Rows.Add(row);
            return dt;
        }

        /// <summary>
        /// Получение текста изменения статистики
        /// </summary>
        public static string GetStatisticText()
        {
            decimal percent = GetLeasePercentDifference();

            if (percent > 0)
                return $"Количество аренд выросло на {percent}% по сравнению с прошлым месяцем.";

            if (percent < 0)
                return $"Количество аренд снизилось на {Math.Abs(percent)}% по сравнению с прошлым месяцем.";

            return "Количество аренд не изменилось по сравнению с прошлым месяцем.";
        }

        // ====================== Вспомогательные методы ======================

        private static int ExecuteScalar(string sql)
        {
            if (ConnectionBD.myconnection?.State != ConnectionState.Open)
            {
                MessageBox.Show("Нет активного подключения к базе данных!");
                return 0;
            }

            try
            {
                ConnectionBD.mycommand.CommandText = sql;
                object result = ConnectionBD.mycommand.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка выполнения запроса:\n" + ex.Message);
                return 0;
            }
        }

        private static DataTable ExecuteQuery(string sql)
        {
            DataTable dt = new DataTable();

            if (ConnectionBD.myconnection?.State != ConnectionState.Open)
            {
                MessageBox.Show("Нет активного подключения к базе данных!");
                return dt;
            }

            try
            {
                ConnectionBD.mycommand.CommandText = sql;
                ConnectionBD.myDataAdapter.SelectCommand = ConnectionBD.mycommand;

                dt.Clear();
                ConnectionBD.myDataAdapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка выполнения запроса:\n" + ex.Message);
                return dt;
            }
        }
    }
}