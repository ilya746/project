using System;

namespace FinalTestTaskProject
{
    //Класс содержащий информацию о сервере и его бд
    public class ServerInfo
    {
        private string serverName, dbName, dbSize, date;

        /** 
         * конструктор класса ServerInfo
         * serverName - название сервера
         * dbName - название бд
         * dbSize - размер бд
         * dbDate - дата
         **/
        public ServerInfo(string serverName, string dbName, string dbSize, string date)
        {
            this.serverName = serverName;
            this.dbName = dbName;
            this.dbSize = dbSize;
            this.date = date;
        }

        // метод возвращает название сервера
        public string getserverName()
        {
            return serverName;
        }
        // метод возвращает название бд
        public string getdbName()
        {
            return dbName;
        }
        // метод возвращает размер бд
        public string getdbSize()
        {
            return dbSize;
        }
        // метод возвращает дату
        public string getdate()
        {
            return date;
        }

        // переопределение метода Equals
        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }

            ServerInfo other = (ServerInfo)obj;
            if (!serverName.Equals(other.getserverName()))
            {
                return false;
            }
            if (!dbName.Equals(other.getdbName()))
            {
                return false;
            }
            if (!dbSize.Equals(other.getdbSize()))
            {
                return false;
            }
            if (!date.Equals(other.getdate()))
            {
                return false;
            }

            return true;
        }


    }
}