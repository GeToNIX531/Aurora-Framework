using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using Tensorflow;
using Tensorflow.NumPy;

using Whetstone.ChatGPT;
using Whetstone.ChatGPT.Models;

namespace AuroraFramework.Net
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var layers = keras.layers;
            // input layer
            var inputs = keras.Input(shape: (32, 32, 3), name: "img");
            // convolutional layer
            var x = layers.Conv2D(32, 3, activation: "relu").Apply(inputs);
            x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            var block_1_output = layers.MaxPooling2D(3).Apply(x);
            x = layers.Conv2D(64, 3, activation: "relu", padding: "same").Apply(block_1_output);
            x = layers.Conv2D(64, 3, activation: "relu", padding: "same").Apply(x);
            var block_2_output = layers.Add().Apply(new Tensors(x, block_1_output));
            x = layers.Conv2D(64, 3, activation: "relu", padding: "same").Apply(block_2_output);
            x = layers.Conv2D(64, 3, activation: "relu", padding: "same").Apply(x);
            var block_3_output = layers.Add().Apply(new Tensors(x, block_2_output));
            x = layers.Conv2D(64, 3, activation: "relu").Apply(block_3_output);
            x = layers.GlobalAveragePooling2D().Apply(x);
            x = layers.Dense(256, activation: "relu").Apply(x);
            x = layers.Dropout(0.5f).Apply(x);
            // output layer
            var outputs = layers.Dense(10).Apply(x);
            // build keras model
            var model = keras.Model(inputs, outputs, name: "toy_resnet");
            model.summary();
            // compile keras model in tensorflow static graph
            model.compile(optimizer: keras.optimizers.RMSprop(1e-3f),
                loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
                metrics: new[] { "acc" });
            // prepare dataset
            var ((x_train, y_train), (x_test, y_test)) = keras.datasets.cifar10.load_data();
            // normalize the input
            x_train = x_train / 255.0f;
            // training
            model.fit(x_train[new Slice(0, 2000)], y_train[new Slice(0, 2000)],
                        batch_size: 64,
                        epochs: 10,
                        validation_split: 0.2f);
            // save the model
            model.save("./toy_resnet_model");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            IChatGPTClient client = new ChatGPTClient("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");

            string message = "GeToNiX: Кто ты?";
            var request = new ChatGPTChatCompletionRequest()
            {
                Model = ChatGPT35Models.Turbo,
                Messages = new List<ChatGPTChatCompletionMessage>()
                {
                    new ChatGPTChatCompletionMessage()
                    {
                        Role = ChatGPTMessageRoles.System,
                        Content = "Ты игровой и увлекательны стриммер. Твоя задача глупо отвечать на сообщения в чате. Тебя зовут Neuro-chan. "
                    },
                    new ChatGPTChatCompletionMessage()
                    {
                        Role = ChatGPTMessageRoles.User,
                        Content = message
                    }
                },
                Temperature = 0.9f,
                MaxTokens = 100
            };

            var response = await client.CreateChatCompletionAsync(request);
            textBox1.Text = response.GetCompletionText();
        }
    }
}
