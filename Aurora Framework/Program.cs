using Aurora_Framework.Modules.AI.BaseV2.Test;
using Aurora_Framework.Modules.AI.Games.OSU.Forms;
using Aurora_Framework.Modules.AI.LLM.Forms;
using Aurora_Framework.Modules.AI.TextGenerate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aurora_Framework
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Modules.Pythone.Main());
        }
    }
}
