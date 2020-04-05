using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Configuration;

namespace FinalTestTaskProject
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TimerCallback tm = new TimerCallback(Task);
                Console.WriteLine("Введите время повтора в секундах:");

                string timerValue = Console.ReadLine(); // timerValue - значение таймера для повторных дейтсвий
                Timer timer = new Timer(tm, 0, 0, Convert.ToInt32(timerValue) * 1000); // таймер со значениями в секундах

                Console.WriteLine("Для остановки программы введите 0");

                ConsoleKeyInfo comand; // команда, которую указывает пользователь, 0 - выход
                do
                {
                    comand = Console.ReadKey();
                } while (comand.KeyChar != '0');

                timer.Dispose(); // остановка таймера
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }

        }

        // метод, который запускается таймером
        public static void Task(object obj)
        {
            Console.WriteLine("Start");
            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            ServerInfo information = null; //строка с новыми данными о бд сервера
            ServerInfo information1 = null; //строка с данными о бд сервера после чтения гугл таблицы

            /** serverName - название сервера
             * dbName - название бд
             * dbSize - размер бд
             * dbDate - дата
             **/
            string serverName, dbName, dbSize, dbDate;

            GoogleTable googleTable = new GoogleTable(Scopes);
            var objectList = new List<object>(); //лист объектов, кторый будет добавлен в гугл таблицу

            // таблица с данными о сервере и базах данных
            var tableSizeInfo = new DataTable();
            tableSizeInfo.Columns.Add("Сервер", typeof(string));
            tableSizeInfo.Columns.Add("База данных", typeof(string));
            tableSizeInfo.Columns.Add("Размер в кб", typeof(string));
            tableSizeInfo.Columns.Add("Дата обновления", typeof(string));

            foreach (var server in GetServersList())
            {
                string serverStringName = server; // название сервера
                // строка подключения к бд
                var connectionParams = ConfigurationManager.ConnectionStrings[serverStringName].ConnectionString;
                var con = new DbConnection(server, connectionParams);
                //получаем значения строк из таблицы tableSizeInfo
                var tableValues = con.GetTable(tableSizeInfo);

                // коллекция объектов типа ServerInfo с новыми элементами
                List<ServerInfo> serverInfo1 = new List<ServerInfo>();
                // коллекция объектов типа ServerInfo с прочитанными из гугл таблицы элементами
                List<ServerInfo> serverInfo2 = new List<ServerInfo>();

                //счетчик для вычисления общего занятого места базами данных
                double count1 = 0;

                //заполнение serverInfo1 новыми данными - строками из tableValues
                foreach (DataRow row in tableValues.Rows)
                {
                    for (int i = 0; i < row.ItemArray.Length; i += 4)
                    {
                        serverName = row.ItemArray[i].ToString();
                        dbName = row.ItemArray[i + 1].ToString();
                        dbSize = row.ItemArray[i + 2].ToString();
                        dbDate = row.ItemArray[i + 3].ToString();
                        information = new ServerInfo(serverName, dbName, FormatSize(dbSize), dbDate);
                        serverInfo1.Add(information);// список с новыми элементами
                        count1 += Convert.ToDouble(dbSize.Substring(0, dbSize.Length - 2));
                    }
                }
                // коллекция объектов типа object с прочитанными из гугл таблицы элементами
                //List<object> listAfterRead = googleTable.ReadEntries(googleTable.GetSheetName());
                List<object> listAfterRead = googleTable.ReadEntries(serverStringName);


                //заполнение serverInfo2 данными, полученными после чтения гугл таблицы
                for (int j = 0; j < listAfterRead.Count; j += 4)
                {
                    serverName = listAfterRead[j].ToString();
                    dbName = listAfterRead[j + 1].ToString();
                    dbSize = listAfterRead[j + 2].ToString();
                    dbDate = listAfterRead[j + 3].ToString();
                    information1 = new ServerInfo(serverName, dbName, dbSize, dbDate);
                    serverInfo2.Add(information1); // список с элементами после чтения
                }

                foreach (ServerInfo info1 in serverInfo1)
                {
                    //если строк с данными нет в таблице
                    if (!serverInfo2.Contains(info1))
                    {
                        //если листа с названием сервера нет
                        if (googleTable.IsExistSheetName(serverStringName) == false)
                        {
                            //создается лист с названием сервера
                            googleTable.CreateNewSheet(serverStringName);
                        }
                        else serverStringName = googleTable.GetSheetName();

                        // в лист objectList добавляются новые данные о сервере и его бд
                        if (serverStringName.Equals(info1.getserverName()))
                        {
                            objectList.Add(info1.getserverName());
                            objectList.Add(info1.getdbName());
                            objectList.Add(info1.getdbSize());
                            objectList.Add(info1.getdate());
                            googleTable.CreateEntries(serverStringName, objectList); // создается строка в гугл таблице
                            objectList.Clear(); //очистка objectList
                        }
                    }
                }
                // добавляется последняя строка в гугл таблицу
                googleTable.GetValueForLastCell(serverInfo2, serverStringName, GetCountSize(serverInfo2, serverInfo1), GetCountRows(count1));
            }
            Console.WriteLine("End");
        }

        //метод для получения списка серверов в файле app.config
        public static List<string> GetServersList()
        {
            List<string> serverList = new List<string>();
            for (int i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
            {
                if (ConfigurationManager.ConnectionStrings[i].ProviderName == "Npgsql")
                    serverList.Add(ConfigurationManager.ConnectionStrings[i].Name);
            }
            return serverList;
        }

        /** метод переводит размер в кб в гб
         * @size - переменная, значенике которой представлено в кб
         **/
        public static string FormatSize(string size)
        {
            if (!size.Equals("Размер в ГБ"))
            {
                return (Convert.ToDouble(size.Substring(0, size.Length - 2)) / 1e+6).ToString() + " Gb";
            }
            else return size;
        }


        /** метод возвращает свободное место сервера в гб
         * @newCount - количество занятого места в кб
         **/
        public static string GetCountRows(double newCount)
        {
            double serverSize = Convert.ToDouble(ConfigurationManager.AppSettings["serverSize"]);
            return (serverSize - newCount / 1e+6).ToString() + " Gb";
        }


        /** метод возвращает количество строк в таблице документа
         * @read - коллекция объектов, полученных после чтения файла таблицы 
         * @write - коллекция объектов, которые требуется заисать
         **/
        public static int GetCountSize(List<ServerInfo> read, List<ServerInfo> write)
        {
            if (read.Count != 0)
            {
                return read.Count;
            }
            else return write.Count + 1;
        }

    }
}