using Npgsql;
using System;
using System.Data;

namespace FinalTestTaskProject

{
    public class DbConnection
    {
        private string serverName;
        private string parametr;

        /*
         * конструктор класса DbConnection
         * @serverName - название сервера
         * @parametr - строка подключения
         **/
        public DbConnection(string serverName, string parametr)
        {
            this.serverName = serverName;
            this.parametr = parametr;
        }

        /**
         * метод для получения данных из бд
         * @table - название таблицы, в которую будут добавлены данные, полученные из бд
         **/
        public DataTable GetTable(DataTable table)
        {
            // строка запроса к бд
            var sql =
                 "SELECT"
                  + " pg_database.datname AS db_name,"
                  + " pg_size_pretty(pg_database_size(pg_database.datname)) AS db_size_kb"
                  + " FROM pg_database";

            var connection = new NpgsqlConnection(parametr); // подключение к бд
            var comm = new NpgsqlCommand(sql, connection); // команда выполнения запроса к бд

            try
            {
                connection.Open(); //открывается соединение
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to database connection. {0}", ex.ToString());
            }
            var reader = comm.ExecuteReader(); // выполняется команда
            try
            {
                while (reader.Read())
                {
                    var currentRow = table.NewRow(); // создаются строки таблицы
                    currentRow[0] = serverName; // название сервера
                    currentRow[1] = reader.GetString(0); // название бд
                    currentRow[2] = reader.GetString(1); // размер в кб
                    currentRow[3] = (DateTime.Now).ToShortDateString(); // дата
                    table.Rows.Add(currentRow); // добавляются строки в таблицу
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed request. {0}", ex.ToString());
            }
            connection.Close(); // закрывается соединение
            return table;
        }

    }
}
