using Genbox.Wikipedia;
using Genbox.Wikipedia.Enums;
using Genbox.Wikipedia.Objects;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Modules.Wiki
{
    public class API
    {
        WebClient webClient;
        public API()
        {
            webClient = new WebClient();
        }

        public bool TryGetPage(string Title, out XmlDocument Document, string Lang = "ru", bool Redirects = true)
        {
            try
            {
                string url = string.Format("http://{0}.wikipedia.org/w/api.php?format=xml&action=query&prop=extracts&titles={1}&redirects={2}", Lang, Title, Redirects);
                var page = webClient.DownloadString(url);

                Document = new XmlDocument();
                Document.LoadXml(page);
                return true;
            }
            catch
            {
                Document = null;
                return false;
            }
        }

        public string GetText(XmlDocument Page)
        {
            var fnode = Page.GetElementsByTagName("extract")[0];

            if (fnode == null) return string.Empty;
            string ss = fnode.InnerText;


            Regex regex = new Regex("\\<[^\\>]*\\>");

            String.Format("Before:{0}", ss); // HTML Text

            ss = FormatXMLToView(ss);

            ss = regex.Replace(ss, String.Empty);

            byte[] bytes = Encoding.Default.GetBytes(ss);
            ss = Encoding.UTF8.GetString(bytes);

            return ss;
        }

        private string FormatXMLToView(string XMLText)
        {
            XMLText = XMLText.Replace("</p>", "\r\n");
            XMLText = XMLText.Replace("</li>", "\r\n");
            XMLText = XMLText.Replace("<ul>", "\r\n");
            XMLText = XMLText.Replace("</ul>", "\r\n");
            for (int i = 0; i < 10; i++)
            {
                XMLText = XMLText.Replace("</h" + i + ">", "\r\n");

            }

            bool find;
            do
            {
                find = false;

                if (XMLText.Contains("  "))
                    find = true;

                if (find) XMLText = XMLText.Replace("  ", " ");
            }
            while (find);

            do
            {
                find = false;

                if (XMLText.Contains("\n\n"))
                    find = true;

                if (find) XMLText = XMLText.Replace("\n\n", "\n");
            }
            while (find);

            do
            {
                find = false;

                if (XMLText.EndsWith("\r\n"))
                    find = true;

                if (find) XMLText = XMLText.Remove(XMLText.Length - 1, 1);
            }
            while (find);

            do
            {
                find = false;

                if (XMLText.StartsWith("\r\n"))
                    find = true;

                if (find) XMLText = XMLText.Remove(0, 1);
            }
            while (find);

            return XMLText;
        }

        public async Task<Dictionary<string, Uri>> GetUrls(string Text, int Depth = 25) //Название, ссылка
        {
            Dictionary<string, Uri> result = new Dictionary<string, Uri>();

            using (WikipediaClient client = new WikipediaClient())
            {

                WikiSearchRequest req = new WikiSearchRequest(Text)
                {
                    Limit = Depth //We would like 5 results
                };

                req.WikiLanguage = WikiLanguage.Russian;
                req.IncludeLanguageUsed = true;

                WikiSearchResponse resp = await client.SearchAsync(req);

                foreach (SearchResult s in resp.QueryResult.SearchResults)
                    result.Add(s.Title, s.Url);
            }

            return result;
        }
    }
}
