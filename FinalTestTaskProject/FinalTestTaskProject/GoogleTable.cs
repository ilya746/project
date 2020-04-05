using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace FinalTestTaskProject
{
    public class GoogleTable
    {

        static readonly string SpreadSheetID = ConfigurationManager.AppSettings["SheetID"]; //id листа таблицы
        SheetsService service;
        public GoogleTable(string[] Scopes)
        {
            //подключение к аккаунту гугл
            UserCredential credential;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["credentials"])))
            {
                var credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");
                credential =
                    GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
            }
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
        }

        /**
         * Метод создает записи на выбранном листе в таблице
         * @sheetName - название листа таблицы
         * @objectList - коллекция объектов, которые будут добавлены на выбранный лист таблицы
         **/
        public void CreateEntries(string sheetName, List<object> objectList)
        {
            try
            {
                var range = $"{sheetName}!A1:D";

                var valueRange = new ValueRange();
                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadSheetID, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /**
         * Метод возвращает коллекцию объектов, которые были считаны с выбранного листа таблицы
         * @sheetName - название листа таблицы
         **/
        public List<object> ReadEntries(string sheet)
        {
            if (IsExistSheetName(sheet) == false)
            {
                sheet = GetSheetName();
            }
            List<object> objectList = new List<object>();
            var range = $"{sheet}!A1:D";
            var request = service.Spreadsheets.Values.Get(SpreadSheetID, range);

            var response = request.Execute();

            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    objectList.Add(row[0]);
                    objectList.Add(row[1]);
                    objectList.Add(row[2]);
                    objectList.Add(row[3]);
                }
            }
            return objectList;
        }



        /**
         * Метод создания нового листа      
         * @sheetName - название листа таблицы
         **/
        public void CreateNewSheet(string sheetName)
        {
            var newSheetRequest = new AddSheetRequest();
            newSheetRequest.Properties = new SheetProperties();
            newSheetRequest.Properties.Title = sheetName;
            BatchUpdateSpreadsheetRequest SpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            SpreadsheetRequest.Requests = new List<Request>();
            SpreadsheetRequest.Requests.Add(new Request
            {
                AddSheet = newSheetRequest
            });
            var request = service.Spreadsheets.BatchUpdate(SpreadsheetRequest, SpreadSheetID);

            request.Execute();
            CreateEntries(sheetName, new List<object>() { "Сервер", "База данных", "Размер в ГБ", "дата обновления" });
        }

        /** 
         * Метод проверки существования названия листа, возвращает true/false
         * @value - название листа таблицы, которое нужно проверить на существование
         **/
        public bool IsExistSheetName(string value)
        {
            List<string> sheetsName = new List<string>();
            var request = service.Spreadsheets.Get(SpreadSheetID).Execute();
            foreach (Sheet sheet in request.Sheets)
            {
                sheetsName.Add(sheet.Properties.Title);
            }
            if (sheetsName.Contains(value))
            {
                return true;
            }
            else return false;
        }


        // метод возвращает название последнего листа в документе               
        public string GetSheetName()
        {
            List<string> sheetsName = new List<string>();
            var request = service.Spreadsheets.Get(SpreadSheetID).Execute();
            foreach (Sheet sheet in request.Sheets)
            {
                sheetsName.Add(sheet.Properties.Title);
            }
            return sheetsName.Last();
        }

        /** 
         * Метод удаляет последнюю строку на указанном листе
         * @sheet - название листа таблицы
         * @value - номер строки для удаления
         **/
        public void DeleteEntry(string sheet, int value)
        {
            var range = $"{sheet}!A{value}:D{value}";
            var request = new ClearValuesRequest();
            var delRequest = service.Spreadsheets.Values.Clear(request, SpreadSheetID, range);
            var response = delRequest.Execute();
        }


        /** 
         * Метод для добавления в конец таблицы записи о свободном месте на диске
         * @serverInfo - коллекция объектов записанных в таблице
         * @sheet - название листа таблицы
         * @countSize - номер последней строки для изменения информации
         * @freeValue - значение свободного места на диске
         **/
        public void GetValueForLastCell(List<ServerInfo> serverInfo, string sheet, int countSize, string freeValue)
        {
            if (serverInfo.Count > 0)
            {
                DeleteEntry(sheet, countSize);
            }

            List<object> objectList = new List<object>();
            objectList.Add(sheet);
            objectList.Add("Свободно");
            objectList.Add(freeValue);
            objectList.Add((DateTime.Now).ToShortDateString());
            var range1 = $"{sheet}!A1:D";

            var valueRange = new ValueRange();
            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadSheetID, range1);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }

    }
}