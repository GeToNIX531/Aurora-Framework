namespace Aurora_Framework.Modules.AI.BaseV2.Data
{
    public class IOModify
    {
        public int iIndex { get; private set; }
        public int oIndex { get; private set; }

        public int iCount { get; private set; }
        public int oCount { get; private set; }

        public double[] Input { get; private set; }
        public double[] Output { get; private set; }
        public IOModify(int Input, int Output)
        {
            this.iCount = Input;
            this.oCount = Output;

            Clear();
        }

        public void AddInput(double Value)
        {
            Input[iIndex] = Value;
            iIndex = iIndex + 1;
        }

        public void AddOutput(double Value)
        {
            Output[oIndex] = Value;
            oIndex = oIndex + 1;
        }

        public void Clear()
        {
            iIndex = 0;
            oIndex = 0;

            this.Input = new double[iCount];
            this.Output = new double[oCount];
        }
    }
}
