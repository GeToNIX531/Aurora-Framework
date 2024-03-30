using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora_Framework.Main.Buffers
{
    public class ActionCollection
    {
        Cache<ActionHandler> actions;
        public int index;

        public int Size { get; private set; }
        private int startSize;
        public ActionCollection(int Size)
        {
            actions = new Cache<ActionHandler>(Size);
            index = 0;
            this.Size = startSize = Size;
        }

        public void Add(ActionHandler Action, bool Opitimized = false)
        {
            if (actions.Add(Action) && Opitimized == false)
                actions.AddSize(startSize);

            if (Opitimized) actions.Optimized();
        }

        public Result LastResult;
        public Result Next()
        {
            index++;
            index = index % actions.Size;
            var action = actions.Get(index);
            LastResult = action.Invoke(this, LastResult);
            return LastResult;
        }

        public delegate Result ActionHandler(ActionCollection Collection, Result BackResult = null);

        public class Result
        {
            public bool OK;
            public object[] Value;

            public Result() => OK = true;
            public Result(bool OK = true)
            {
                this.OK = OK;
            }

            public Result(params object[] Value)
            {
                this.Value = Value;
                this.OK = true;
            }

            public Result(bool OK, params object[] Value)
            {
                this.OK = OK;
                this.Value = Value;
            }
        }
    }
}
