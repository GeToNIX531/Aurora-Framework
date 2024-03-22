using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Saver;
using Modules.Wiki;

namespace Aurora_Framework.Modules.Wiki.V1._0.Miner
{
    public class Client
    {
        private API api;
        public Client()
        {
            api = new API();
        }

        public async Task<bool> Search(string Find, int Step = 0, int Max = 4)
        {
            if (Max == 0) return false; 

            var urls = await api.GetUrls(Find);

            bool find = false;
            if (urls.Count > 0) find = true;

            for (int i = 0; i < urls.Count; i++)
            {
                string title = urls.ElementAt(i).Key;
                if (Find == title) continue;

                if (api.TryGetPage(title, out var Page))
                {
                    string text = api.GetText(Page);
                    Data data = new Data(Find, Step, i, text, title);
                    data.Save($"Wiki/{data.Hash}.wiki", out var err);

                    await Search(title, Step + 1, Max - 1);
                }
            }

            return find;
        }
    }
}
