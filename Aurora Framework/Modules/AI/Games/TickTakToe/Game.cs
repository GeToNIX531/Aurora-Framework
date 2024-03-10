using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aurora_Framework.Modules.AI.Games.TickTakToe
{
    public partial class Game : Form
    {
        Field field;

        public Game()
        {
            InitializeComponent();
            buttons = new Button[]
                {
                    button1,
                    button2,
                    button3,
                    button4,
                    button5,
                    button6,
                    button7,
                    button8,
                    button9,
                };

            List<Position> temp = new List<Position>();
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    temp.Add(new Position() { X = x, Y = y });

            positions = temp.ToArray();

            foreach (var button in buttons)
            {
                button.Click += buttonClick;
            }

            field = new Field();
            client1 = new Base.BaseV2._1.Client(10, 5, 9, 9);
            client2 = new Base.BaseV2._1.Client(10, 5, 9, 9);
        }

        Cell cell = Cell.Player1;
        Base.BaseV2._1.Client client1;
        Base.BaseV2._1.Client client2;
        Random rnd = new Random();
        private void buttonClick(object sender, EventArgs e)
        {
            var button = (Button)sender;
            int index;
            for (index = 0; index < buttons.Length && buttons[index] != button; index++) ;

            if (field.IsEnd(out var Player))
            {
                foreach (var _b in buttons)
                    _b.Text = string.Empty;

                if (Player != Cell.Null)
                    if (field.AIDataLearn(out var inputs, out var outputs))
                    {
                        for (int i = 0; i < inputs.Length; i++)
                        {
                            var input = inputs[i];
                            var output = outputs[i];

                            client1.Learn(input, output, 0.001d);
                            client2.Learn(input, output, 0.001d);
                        }
                    }

                field.Clear();

                return;
            }

            Position position = positions[index];
            if (field.Get(position.X, position.Y) == Cell.Null)
            {
                field.Set(position, cell);

                var input = field.AIInput(Cell.Player2);
                var result = client1.Result(input);

                textBox1.Clear();
                foreach (var r in result)
                    textBox1.AppendText(r + "\r\n");

                if (cell == Cell.Player1)
                {
                    button.Text = "O";

                    /*
                    cell = Cell.Player2;
                    */

                    var max = result.Max();
                    int i;
                    for (i = 0; i < 8; i++)
                        if (result[i] == max)
                            break;

                    if (field.IsEnd(out _)) return;

                    position = positions[i];
                    if (field.Get(position.X, position.Y) == Cell.Null)
                    {
                        field.Set(position, Cell.Player2);
                        buttons[i].Text = "X";
                    }
                    else
                    {
                        bool find = false;

                        do
                        {
                            i = rnd.Next(0, 9);
                            position = positions[i];

                            if (field.Get(position.X, position.Y) == Cell.Null)
                            {
                                field.Set(position, Cell.Player2);
                                buttons[i].Text = "X";
                                find = true;
                            }

                        }
                        while (find == false);
                    }

                }
                else
                {
                    cell = Cell.Player1;
                    button.Text = "X";
                }
            }
        }

        public Button[] buttons;
        public Position[] positions;

        int player1Score;
        int player2Score;

        int Nicha;

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int inter = 0; inter < 1; inter++)
            {
                if (field.IsEnd(out var Player))
                {
                    foreach (var _b in buttons)
                        _b.Text = string.Empty;


                    if (field.AIDataLearn(out var inputs, out var outputs))
                    {
                        for (int i = 0; i < inputs.Length; i++)
                        {
                            var input = inputs[i];
                            var output = outputs[i];

                            for (int k = 0; k < 100; k++)
                            {
                                if (Player == Cell.Null)
                                {
                                    client1.Learn(input, output, 0.01d * i / (inputs.Length * inputs.Length));
                                    client2.Learn(input, output, 0.01d * i / (inputs.Length * inputs.Length));
                                }
                                else
                                {
                                    client1.Learn(input, output, 0.01d / inputs.Length);
                                    client2.Learn(input, output, 0.01d / inputs.Length);
                                }
                            }
                        }
                    }

                    field.Clear();

                    if (Player == Cell.Player1)
                    {
                        player2Score++;
                        label2.Text = player2Score.ToString();
                    }
                    else if (Player == Cell.Player2)
                    {
                        player1Score++;
                        label1.Text = player1Score.ToString();
                    }
                    else
                    {
                        Nicha++;
                        label3.Text = Nicha.ToString();
                    }

                    return;
                }

                {

                    double[] input;
                    double[] result;
                    if (cell == Cell.Player1)
                    {
                        input = field.AIInput(cell);
                        result = client1.Result(input);
                    }
                    else
                    {
                        input = field.AIInput(cell);
                        result = client2.Result(input);
                    }

                    if (cell == Cell.Player1)
                    {
                        textBox1.Clear();
                        foreach (var r in result)
                            textBox1.AppendText(r + "\r\n");
                    }
                    else
                    {
                        textBox2.Clear();
                        foreach (var r in result)
                            textBox2.AppendText(r + "\r\n");
                    }

                    var max = result.Max();
                    int i;
                    for (i = 0; i <= 8; i++)
                        if (result[i] == max)
                            break;

                    if (i >= 9) return;

                    var position = positions[i];
                    if (field.Get(position.X, position.Y) == Cell.Null)
                    {
                        field.Set(position, cell);
                        if (cell == Cell.Player1)
                            buttons[i].Text = "O";
                        else buttons[i].Text = "X";
                    }
                    else
                    {
                        bool find = false;

                        do
                        {
                            i = rnd.Next(0, 9);
                            position = positions[i];

                            if (field.Get(position.X, position.Y) == Cell.Null)
                            {
                                field.Set(position, cell);
                                if (cell == Cell.Player1)
                                    buttons[i].Text = "O";
                                else buttons[i].Text = "X";
                                find = true;
                            }

                        }
                        while (find == false);
                    }

                    if (cell == Cell.Player1)
                        cell = Cell.Player2;
                    else cell = Cell.Player1;
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }
    }
}
