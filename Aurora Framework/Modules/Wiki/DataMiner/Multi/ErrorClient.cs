using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;

namespace Aurora_Framework.Modules.Wiki.DataMiner.Multi
{
    public class Client : IDisposable
    {
        WebClient client;
        Stream reader;

        public Client()
        {
            client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        }

        public void Reader(string URL)
        {
            reader = client.OpenRead(URL);
        }



        public void Dispose() => client.Dispose();
    }
}
