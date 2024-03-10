using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;
using AI_Aurora_V1.Data.Vector;
using Buffers.Tokenezator;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base = AI_Aurora_V1.Modules.AI.Base;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace Aurora_Framework.Modules.AI.LLM.Forms
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }

        Inputer inputer;
        Outputer outputer;
        private void button1_Click(object sender, EventArgs e)
        {
            #region Создание токенов

            string abcRus = "йцукенгшщзхъфывапролджэячсмитьбюё";
            string abcEng = "qwertyuiopasdfghjklzxcvbnm";

            string Num = "-1234567890";
            string Symb = @"!@#$%^&*()_+{}:|<>?№;?[]-=\/:,. ";

            inputer = new Inputer();
            foreach (var value1 in abcEng)
            {
                var a = value1.ToString();
                var A = a.ToUpper();

                foreach (var value2 in abcEng)
                {
                    var b = value2.ToString();

                    var ab = a + b;
                    var Ab = A + b;

                    inputer.Add(a);
                    inputer.Add(A);
                    inputer.Add(ab);
                    inputer.Add(Ab);
                }

                foreach (var value2 in Symb)
                {
                    var b = value2.ToString();
                    var ab = a + b;
                    inputer.Add(ab);
                }
            }

            foreach (var value1 in abcRus)
            {
                var a = value1.ToString();
                var A = a.ToUpper();

                foreach (var value2 in abcRus)
                {
                    var b = value2.ToString();

                    var ab = a + b;
                    var Ab = A + b;

                    inputer.Add(a);
                    inputer.Add(A);
                    inputer.Add(ab);
                    inputer.Add(Ab);
                }

                foreach (var value2 in Symb)
                {
                    var b = value2.ToString();
                    var ab = a + b;
                    inputer.Add(ab);
                }
            }

            foreach (var value1 in Num)
            {
                var a = value1.ToString();

                foreach (var value2 in Symb)
                {
                    var b = value2.ToString();
                    var ab = a + b;

                    inputer.Add(a);
                    inputer.Add(ab);
                }
            }

            foreach (var value1 in Symb)
            {
                var a = value1.ToString();

                foreach (var value2 in Symb)
                {
                    var b = value2.ToString();
                    var ab = a + b;

                    inputer.Add(a);
                    inputer.Add(ab);
                }
            }

            int count = Num.Length;
            for (int i = 0; i < count; i++)
            {
                string A = Num[i].ToString();

                for (int k = 0; k < count; k++)
                {
                    string B = Num[k].ToString();
                    string result = A + B;
                    inputer.Add(result);
                }
            }

            inputer.Add(((char)13).ToString() + ((char)10).ToString());

            outputer = inputer.ToOutputer();
            label1.Text = inputer.Count.ToString();
            #endregion
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            #region Токенизация
            string input = textBox1.Text;
            if (outputer.FromTextV2(input, out int[] Tokens, out string NotFind) == false)
                MessageBox.Show(((int)NotFind[0]).ToString());

            richTextBox1.Clear();
            richTextBox2.Clear();
            Color[] colors = new Color[] { Color.Orange, Color.DarkOliveGreen, Color.BlueViolet };

            for (int i = 0; i < Tokens.Length; i++)
            {
                var color = colors[i % colors.Length];
                var value = Tokens[i];

                richTextBox1.SelectionBackColor = color;
                richTextBox2.SelectionBackColor = color;
                richTextBox1.AppendText(value.ToString());
                richTextBox1.AppendText(" ");

                if (outputer.FromIndex(value, out string Value))
                    richTextBox2.AppendText(Value);
            }
            #endregion
            buffer.Clear();

            buffer.Add(Tokens);
            await GenerateAnswer();
        }

        int max = 100;
        Buffers.BPointer<int> buffer = new Buffers.BPointer<int>(1024);
        private async Task GenerateAnswer(int Count = 0)
        {
            if (Count == max) return;

            var vectorInput = buffer.ToArray().ToVector<double>();

            var output = aiClient.Output(vectorInput);
            if (FromOutput(output, out int Token) == false)
            {
                outputer.FromIndex(Token, out string Text);
                richTextBox3.AppendText(Text);
                buffer.Add(Token);
                await GenerateAnswer(Count + 1);
            }
        }

        AI_Aurora_V1.Modules.AI.Base.Client aiClient;
        private void button3_Click(object sender, EventArgs e)
        {
            aiClient = new AI_Aurora_V1.Modules.AI.Base.Client(1024, 256, inputer.Count + 1);
            chat = api.Chat.CreateConversation(new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
                Temperature = 0.9,
                MaxTokens = 4096,
            });

            richTextBox4.Clear();
        }

        private bool FromOutput(Vector<double> Output, out int Token)
        {
            int count = Output.Count;
            int max = 0;
            for (int i = 1; i < count; i++)
            {
                if (Output[max] < Output[i])
                    max = i;
            }

            Token = max;
            if (max == inputer.Count) return true;
            return false;
        }

        OpenAI_API.OpenAIAPI api = new OpenAI_API.OpenAIAPI("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");
        Conversation chat;
        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                chat.AppendUserInput(textBox1.Text);

                string result = string.Empty;
                await chat.StreamResponseFromChatbotAsync(res =>
                {
                    richTextBox4.AppendText(res);
                    result += res;
                });
                chat.AppendExampleChatbotOutput(result);
                richTextBox4.Text += "\n";
                richTextBox4.Text += "\n";
            }
            catch
            {
                
            }
        }
    }
}
