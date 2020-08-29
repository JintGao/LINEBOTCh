using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System.IO;


namespace BOTCH.Controllers
{
    public class GooglesHeet
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "LineBotWebHookController";
        static readonly string SpreadsheetId = "1_wCOtZG5cCgJljtDDtTGieki9bJ7AkwbzR1O8luODuc";
        static readonly string sheet = "legislators-current";
        static SheetsService service;

        public void SstGooglesHeet()
        {
            GoogleCredential credential;
            using (var stream = new FileStream(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "client_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            // Create Google Sheets API service.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        //讀取 - 共用
        public  IList<IList<object>> ReadEntries(string sheet, string Start, string End)
        {
            var range = $"{sheet}!{Start}:{End}";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(SpreadsheetId, range);

            var response = request.Execute();
            IList<IList<object>> values = response.Values;

            return values;
            //if (values != null && values.Count > 0)
            //{
            //    foreach (var row in values)
            //    {
            //        // Print columns A to F, which correspond to indices 0 and 4.
            //        Console.WriteLine("{0} | {1} | {2} | {3} | {4} | {5}", row[0], row[1], row[2], row[3], row[4], row[5]);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("No data found.");
            //}
        }


        //讀取 特定角色
        public List<string> ReadRoleEntries(string name)
        {
            List<string> 回傳角色資訊 = new List<string>(); ;

            string sheet = "角色";
            string Start = "A";
            string End = "I";

            var range = $"{sheet}!{Start}:{End}";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(SpreadsheetId, range);

            var response = request.Execute();
            IList<IList<object>> values = response.Values;

            //List<string> 回傳角色資訊 = new List<string>();
            //return values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if(row[0].ToString() == name)
                    {
                         回傳角色資訊 = new List<string>() { row[0].ToString(), row[1].ToString(), row[2].ToString()
                            , row[3].ToString(), row[4].ToString(), row[5].ToString(), row[6].ToString(), row[7].ToString(), row[8].ToString() };

                        break;
                    }
                }
            }

            return 回傳角色資訊;
 
        }

        //讀取 特定技能
        public List<string> ReadSkillEntries(string name)
        {
            List<string> 回傳技能資訊 = new List<string>(); ;

            string sheet = "技能";
            string Start = "A";
            string End = "D";

            var range = $"{sheet}!{Start}:{End}";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(SpreadsheetId, range);

            var response = request.Execute();
            IList<IList<object>> values = response.Values;

            //List<string> 回傳角色資訊 = new List<string>();
            //return values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (row[0].ToString() == name)
                    {
                        回傳技能資訊 = new List<string>() {  row[0].ToString(), row[1].ToString(), row[2].ToString() , row[3].ToString() };

                        break;
                    }
                }
            }

            return 回傳技能資訊;

        }

        //讀取 特定職業
        public List<string> ReadProfessionEntries(string name)
        {
            List<string> 回傳職業資訊 = new List<string>(); ;

            string sheet = "職業";
            string Start = "A";
            string End = "D";

            var range = $"{sheet}!{Start}:{End}";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(SpreadsheetId, range);

            var response = request.Execute();
            IList<IList<object>> values = response.Values;

            //List<string> 回傳角色資訊 = new List<string>();
            //return values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (row[0].ToString().Contains(name))
                    {
                        回傳職業資訊 = new List<string>() { row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString() };

                        break;
                    }
                }
            }

            return 回傳職業資訊;

        }

        //插入
        public void CreateEntry(string sheet, string Start, string End, List<string> insertStringList)
        {
            var range = $"{sheet}!{Start}:{End}";
            var valueRange = new ValueRange();

            //var oblist = new List<object>() { "Hello!", "This", "was", "insertd", "via", "C#" };
            var oblist = new List<object>() { };

            foreach (string insertString in insertStringList)
            {
                oblist.Add(insertString);
            }

            valueRange.Values = new List<IList<object>> { oblist };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = appendRequest.Execute();
        }

        //更新
        public void UpdateEntry(string sheet, string location, string updatString)
        {
            var range = $"{sheet}!{location}";
            var valueRange = new ValueRange();

            var oblist = new List<object>() { updatString };
            valueRange.Values = new List<IList<object>> { oblist };

            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = updateRequest.Execute();
        }

        //刪除
        public void DeleteEntry(string sheet, string Start, string End)
        {
            var range = $"{sheet}!{Start}:{End}";
            var requestBody = new ClearValuesRequest();

            var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, SpreadsheetId, range);
            var deleteReponse = deleteRequest.Execute();
        }
    }
}