using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora_Framework.Modules.AI.Games.TickTakToe
{
    public class Field
    {
        private Cell[,] cells;
        public Field() => Clear();

        public List<Cell[,]> steps = new List<Cell[,]>();
        public void Set(Position Position, Cell Cell)
        {
            cells[Position.Y, Position.X] = Cell;
            var mem = new Cell[3, 3];
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    mem[y, x] = cells[y, x];

            steps.Add(mem);
        }

        public Cell Get(int X, int Y) => cells[Y, X];

        public bool IsEnd(out Cell cell)
        {
            cell = Cell.Null;
            int count = 0;
            for (int i = 1; i < 3; i++)
            {
                if (cells[i, i] == cells[i - 1, i - 1])
                    if (cells[i, i] != Cell.Null)
                    {
                        cell = cells[i, i];
                        count++;
                    }
            }

            if (count == 2) return true;
            count = 0;

            for (int i = 1; i < 3; i++)
            {
                int x = i;
                int y = 3 - i - 1;

                if (cells[y, x] == cells[y + 1, x - 1])
                    if (cells[y, x] != Cell.Null)
                    {
                        cell = cells[y, x];
                        count++;
                    }
            }

            if (count == 2) return true;
            count = 0;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 1; y < 3; y++)
                {
                    if (cells[y, x] == cells[y - 1, x])
                        if (cells[y, x] != Cell.Null)
                        {
                            cell = cells[y, x];
                            count++;
                        }
                }

                if (count == 2) return true;
                count = 0;
            }

            for (int y = 0; y < 3; y++)
            {
                for (int x = 1; x < 3; x++)
                {
                    if (cells[y, x] == cells[y, x - 1])
                        if (cells[y, x] != Cell.Null)
                        {
                            cell = cells[y, x];
                            count++;
                        }
                }

                if (count == 2) return true;
                count = 0;
            }

            foreach (var c in cells)
                if (c == Cell.Null)
                    return false;

            cell = Cell.Null;
            return true;
        }

        public void Clear()
        {
            cells = new Cell[3, 3];
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                {
                    cells[y, x] = Cell.Null;
                }

            steps.Clear();
        }


        public double[] AIInput(Cell Player)
        {
            List<double> result = new List<double>();
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                {
                    var cell = cells[y, x];
                    if (cell == Cell.Null) result.Add(0.2f);
                    else if (cell == Player) result.Add(0.1f);
                    else result.Add(0.7f);
                }

            result.Add(Player == Cell.Player1 ? 1d : 0d);

            return result.ToArray();
        }

        public bool AIDataLearn(out double[][] Input, out double[][] Output)
        {
            var input = new List<double[]>();
            var output = new List<double[]>();

            Input = null;
            Output = null;
            if (IsEnd(out Cell Player))
            {
                int time = 0;

                if (Player == Cell.Null)
                {
                    time = 2;
                    Player = Cell.Player1;
                }

                do
                {
                    for (int i = 0; i < steps.Count / 2 - 1; i++)
                    {
                        List<double> inpu = new List<double>();

                        var step = steps[2 * i];
                        for (int y = 0; y < 3; y++)
                            for (int x = 0; x < 3; x++)
                            {
                                var cell = step[y, x];
                                if (cell == Cell.Null) inpu.Add(0.2d);
                                else if (cell == Player) inpu.Add(0.1d);
                                else inpu.Add(0.7d);
                            }

                        inpu.Add(Player == Cell.Player1 ? 1d : 0d);

                        var nextStep = steps[2 * (i + 1)];
                        Position position = null;

                        int index = 0;
                        for (int y = 0; y < 3; y++)
                            for (int x = 0; x < 3; x++)
                            {
                                if (step[y, x] != nextStep[y, x])
                                    if (nextStep[y, x] == Player)
                                        position = new Position() { X = x, Y = y };

                                if (position == null)
                                    index++;
                            }

                        double[] outp = new double[9];
                        outp[index] = 1f;

                        input.Add(inpu.ToArray());
                        output.Add(outp.ToArray());
                    }


                    if (time == 2)
                        Player = Cell.Player2;
                    time--;
                }
                while (time > 0);

                Input = input.ToArray();
                Output = output.ToArray();
                return true;
            }
            else return false;
        }
    }

    public class Position
    {
        public int X;
        public int Y;

    }

    public enum Cell : uint
    {
        Null = 2,
        Player1 = 0,
        Player2 = 4,
    }
}
