using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora_Framework.Modules.AI.YouTube_Streamer.Data
{
    public class Chat
    {

        public class User
        {
            public int ID;
            public string NickName;
        }

        public class Message
        {
            public User User;
            public string Massage;
            public DateTime Time;
        }
    }
}
