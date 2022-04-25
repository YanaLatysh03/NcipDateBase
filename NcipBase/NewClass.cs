using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace NcipBase
{
    public class NewClass
    {
        public static List<List<string>> Method()
        {
            List<List<string>> rows = new List<List<string>>();
            string path = "C:/Users/shoto/source/repos/NcipBase/NcipBase/Page.html";
            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var infoAboutPages = new HtmlDocument();
                infoAboutPages.Load(fs);
                var rowsNextDisabled = infoAboutPages.DocumentNode.SelectNodes("//li[@onclick]");

                string nextLink = "";
                for (int i = 0; i < rowsNextDisabled.Count; i++)
                {
                    if (rowsNextDisabled[i].InnerText.StartsWith("Вперед"))
                    {
                        nextLink = rowsNextDisabled[i].GetAttributeValue("onClick", "");
                    }
                }

                SendDataToService(nextLink);

                var trs = infoAboutPages.DocumentNode.SelectNodes("//table/tbody/tr");
                int n = 0;

                foreach (HtmlNode tr in trs)
                {
                    List<string> columns = new List<string>();
                    var tds = tr.SelectNodes("//td[@nowrap='nowrap']");
                    string numberOfRequest = tds[n++].InnerText;
                    string numberOfRegistration = tds[n++].InnerText;
                    string moreInfoLink = tr.GetAttributeValue("onClick", "")
                        .Replace("&amp;", "&")
                        .Replace("toLink('", "")
                        .Replace("');", "");
                    Console.WriteLine(numberOfRequest);
                    Console.WriteLine(numberOfRegistration);
                    Console.WriteLine(moreInfoLink);

                    var moreInfoPage = new HtmlWeb();
                    moreInfoPage.OverrideEncoding = Encoding.GetEncoding(1251);
                    var response = moreInfoPage.Load("http://search.ncip.by/database/index.php" + moreInfoLink);
                    var rowsText = response.DocumentNode.SelectNodes("//div[@class='col-md-6 w4']");
                    var rowsValue = response.DocumentNode.SelectNodes("//div[@class='col-md-6 wauto']");
                    if (rowsText[0].InnerText != "Дата прекращения действия:")
                    {
                        columns.Add(String.Empty);
                    }
                    for (int i = 0; i < rowsText.Count - 2; i++)
                    {
                        Console.WriteLine(rowsText[i].InnerText);
                        Console.WriteLine(rowsValue[i].InnerText);
                        columns.Add(rowsValue[i].InnerText.Trim());
                    }
                    rows.Add(columns);
                }
            }
            return rows;
        }

        public static void SendDataToService(string parameters)
        {
            var modifiedStringParameters = parameters.Replace("post", "").Replace("(", "").Replace(")", "").Replace("{", "").Replace("}", "")
                .Replace(":", "=").Replace("'", "").Replace(",", "&");
            var stringParameters = modifiedStringParameters.Remove(modifiedStringParameters.Length - 1, 1);

            byte[] byteArray = Encoding.UTF8.GetBytes(stringParameters);

            string url = "http://search.ncip.by/database/index.php?pref=tz&lng=ru&page=2";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";

            request.ContentType = "application/x-www-form-urlencoded";
            request.Credentials = CredentialCache.DefaultCredentials;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(byteArray, 0, byteArray.Length);
            }

            HttpWebResponse responce = (HttpWebResponse)request.GetResponse();
            Stream dataStream = responce.GetResponseStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            string textFromFile = Encoding.UTF8.GetString(byteArray);

            responce.Close();
        }
    }
}
