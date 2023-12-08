using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Modules.Wiki
{
    public class Simple
    {
        public Simple()
        {
            api = new API();
        }

        API api;
        public async void Start(string StartSearch) => await Search(StartSearch);

        private async Task<bool> Search(string Find, int Ignore = 0)
        {
            var urls = await api.GetUrls(Find);

            bool find = false;
            for (int i = Ignore; i < urls.Count; i++)
            {
                find = true;

                string title = urls.ElementAt(i).Key;
                if (api.TryGetPage(title, out var Page))
                {
                    string text = api.GetText(Page);
                    Data data = new Data(title, text);
                    if (data.Save(out var Error) == false)
                        MessageBox.Show(Error.Message);
                    await Search(title, 1);
                }
            }

            return find;
        }
    }
}
