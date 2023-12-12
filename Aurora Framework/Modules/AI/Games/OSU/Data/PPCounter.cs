using Newtonsoft.Json;
using System;
using System.Net;

namespace Aurora_Framework.Modules.AI.Games.OSU.Data
{
    public class OsuPPCounter : IDisposable
    {
        public WebClient client;

        public OsuPPCounter(WebClient client)
        {
            this.client = client;
        }

        public OsuPPCounter()
        {
            client = new WebClient();
        }

        string url = "http://127.0.0.1:24050/json";
        public string Get() => client.DownloadString(url);

        public void Dispose()
        {
            client.Dispose();
        }

        public bool isClose = false;
        public bool Update(out Data Data)
        {
            try
            {
                string value = client.DownloadString(url);
                isClose = false;

                Data = null;
                if (value.Contains("error")) return false;
                Data = JsonConvert.DeserializeObject<Data>(value);
                return true;
            }
            catch
            {
                Data = null;
                isClose = true;
                return false;
            }
        }


        public class Data
        {
            public GamePlay gameplay;

            public class GamePlay
            {
                public bool gameMode;
                public long score;
                public float accuracy;

                public Combo combo;

                public class Combo
                {
                    public int current;
                    public int max;
                }

                public Hits hits;

                public class Hits
                {
                    [Newtonsoft.Json.JsonProperty("300")]
                    public int Score_300;
                    public int geki;

                    [Newtonsoft.Json.JsonProperty("100")]
                    public int Score_100;
                    public int katu;

                    [Newtonsoft.Json.JsonProperty("50")]
                    public int Score_50;
                    [Newtonsoft.Json.JsonProperty("0")]
                    public int Score_0;

                    public int sliderBreaks;
                }
            }
        }
    }
}
