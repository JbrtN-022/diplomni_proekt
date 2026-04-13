using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rccs.MyClass
{
    internal class ClassWorkers
    {
        public static void SelectWorkers()
        {
            ConnectionBD.mycommand.CommandText = @"SELECT 
                    workers.id_workers,
                    workers.name,
                    workers.work_device_data,
                    workers.number,
                    company.company
                FROM rccs.workers 
                JOIN rccs.company 
                    ON company.id_company = workers.id_company
                JOIN rccs.users  
                    ON users.id_workers = workers.id_workers
                JOIN rccs.roll  
                    ON users.id_roll = roll.id_roll
                ";
            ConnectionBD.dtWorkers.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtWorkers);
        }

        public static void SelectWorkerForEdit(string name, string number, out int id_workers)
        {
            id_workers = 0;

            ConnectionBD.mycommand.CommandText = @"
        SELECT 
            workers.id_workers,
            workers.name,
            workers.work_device_data,
            workers.number,
            users.login,
            users.password,
            users.id_roll
        FROM rccs.workers
        JOIN rccs.users ON users.id_workers = workers.id_workers
        WHERE workers.name = @name 
          AND workers.number = @number";

           
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@name", name);
            ConnectionBD.mycommand.Parameters.AddWithValue("@number", number);

            ConnectionBD.dtTemp.Clear();                    
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

            if (ConnectionBD.dtTemp.Rows.Count > 0)
            {
                DataRow row = ConnectionBD.dtTemp.Rows[0];
                id_workers = Convert.ToInt32(row["id_workers"]);
            }
        }
        public static int AddWorker(string name, DateTime date, string phone, int id_company)
        {
            ConnectionBD.mycommand.CommandText = @"
        INSERT INTO rccs.workers (name, work_device_data, number, id_company) 
        VALUES (@name, @date, @phone, @id_company);
        
        SELECT LAST_INSERT_ID();";

            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@name", name);
            ConnectionBD.mycommand.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
            ConnectionBD.mycommand.Parameters.AddWithValue("@phone", phone);
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_company", id_company);

            object result = ConnectionBD.mycommand.ExecuteScalar();

            return Convert.ToInt32(result);
        }


        public static void AddUser(string login, string password, int workerId, int roleId)
            {
                ConnectionBD.mycommand.CommandText =
                    $"INSERT INTO rccs.users (login, password, id_workers, id_roll) " +
                    $"VALUES ('{login}', '{password}', {workerId}, {roleId});";

                ConnectionBD.mycommand.ExecuteNonQuery();
            }
        
        public static void DeleteWorker(int id_workers)
        {
           
            ConnectionBD.mycommand.CommandText = $"DELETE FROM rccs.users WHERE id_workers = {id_workers}";
            ConnectionBD.mycommand.ExecuteNonQuery();

           
            ConnectionBD.mycommand.CommandText = $"DELETE FROM rccs.workers WHERE id_workers = {id_workers}";
            ConnectionBD.mycommand.ExecuteNonQuery();
        }
        
    }
}
