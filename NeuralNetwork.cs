using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
namespace KE_Course_Work
{
    public partial class neuralNetwork
    {
        Cavitation_Recognition cr;
        public neuralNetwork(Cavitation_Recognition Ncr)
        {
            cr = Ncr;
        }
        int i = 0;
        //fundamental 
        public int[] layers;//layers
        public float[][] neurons;//neurons
        public float[][] biases;//biasses
        public float[][][] weights;//weights
        public int[] activations;//layers
        //backprop
        public float learningRate = 0.1f;//learning rate
        public float cost = 0;
        public string cost_total;
        public string loaded_data;
        public float Total_cost = 0;
        public int count=0;
        public neuralNetwork(int[] layers, string[] layerActivations)
        {
            try
            {
                this.layers = new int[layers.Length];
                for (int i = 0; i < layers.Length; i++)
                {
                    this.layers[i] = layers[i];
                }
                activations = new int[layers.Length - 1];
                for (int i = 0; i < layers.Length - 1; i++)
                {
                    string action = layerActivations[i];
                    switch (action)
                    {
                        case "sigmoid":
                            activations[i] = 0;
                            break;
                        case "relu":
                            activations[i] = 2;
                            break;
                        default:
                            activations[i] = 2;
                            break;
                    }
                }
                InitNeurons();
                InitBiases();
                InitWeights();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }
        //create empty storage array for the neurons in the network.
        public void InitNeurons()
        {
            List<float[]> neuronsList = new List<float[]>();
            for (int i = 0; i < layers.Length; i++)
            {
                neuronsList.Add(new float[layers[i]]);
            }
            neurons = neuronsList.ToArray();
        }
        //initializes random array for the biases being held within the network.
        public void InitBiases()
        {
            Random random = new Random();
            //.NextDouble(1.23, 5.34);
            List<float[]> biasList = new List<float[]>();
            for (int i = 0; i < layers.Length; i++)
            {
                float[] bias = new float[layers[i]];
                for (int j = 0; j < layers[i]; j++)
                {
                    double value = random.NextDouble();
                    bias[j] = float.Parse(value.ToString());//Random(-0.5f, 0.5f);
                }
                biasList.Add(bias);
            }
            biases = biasList.ToArray();
        }
        //initializes random array for the weights being held in the network.
        public void InitWeights()
        {
            Random random = new Random();

            List<float[][]> weightsList = new List<float[][]>();
            for (int i = 1; i < layers.Length; i++)
            {
                List<float[]> layerWeightsList = new List<float[]>();
                int neuronsInPreviousLayer = layers[i - 1];
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    float[] neuronWeights = new float[neuronsInPreviousLayer];
                    for (int k = 0; k < neuronsInPreviousLayer; k++)
                    {
                        double value = random.NextDouble();
                        neuronWeights[k] = float.Parse(value.ToString()); //(-0.5f, 0.5f);
                    }
                    layerWeightsList.Add(neuronWeights);
                }
                weightsList.Add(layerWeightsList.ToArray());
            }
            weights = weightsList.ToArray();
        }
        public float[] FeedForward(float[] inputs)//feed forward, inputs >==> outputs.
        {
            try
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                        neurons[0][i] = inputs[i];
                }
                int count = 0;
                for (int i = 1; i < layers.Length; i++)
                {
                    int layer = i - 1;
                    for (int j = 0; j < neurons[i].Length; j++)
                    {
                        float value = 0f;
                        int g = 0;
                        for (int k = 0; k < neurons[i - 1].Length; k++)
                        {
                            value += weights[i - 1][j][k] * neurons[i - 1][k] ;
                        }
                        float x = value + biases[i][j];
                        count++;
                        neurons[i][j] = activate(value + biases[i][j], layer);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return neurons[neurons.Length - 1];
        }
        //Backpropagation implemtation down until mutation.
        public float activate(float value, int layer)//all activation functions
        {
            switch (activations[layer])
            {
                case 0:
                    return sigmoid(value);
                case 1:
                    return relu(value);
                case 2:
                    return tanh(value);
                default:
                    return sigmoid(value);
            }
        }
        public float activateDer(float value, int layer)//all activation function derivatives
        {
            switch (activations[layer])
            {
                case 0:
                    return sigmoidDer(value);
                case 1:
                    return reluDer(value);
                default:
                    return sigmoidDer(value);
            }
        }
        public float sigmoid(float x)
        {
            float k = (float)Math.Exp(x);
            return k / (1.0f + k);
        }
        public float relu(float x)
        {
            return (0 <= x) ? 1 : 0;
        }
        public float tanh(float x)
        {
            return (float)Math.Tanh(x);
        }
        public float sigmoidDer(float x)
        {
            return x * (1 - x);
        }
        public float reluDer(float x)
        {
            return (0 <= x) ? 1: 0;
        }
        //Back propagate algorism 
        public void BackPropagate(float[] inputs, float[] expected)//backpropogation;
        {
            float[] output = FeedForward(inputs);//runs feed forward to ensure neurons are populated correctly
            cost = 0;
            for (int i = 0; i < output.Length; i++)
                cost += (float)Math.Pow(output[i] - expected[i], 2);//calculated cost of network
            cost = cost / output.Length;//this value is not used in calculions, rather used to identify the performance of the network
            i++; count++;
            Total_cost += Total_cost + cost;
            cost_total += "Cost of Epoch " +i +" = "+ cost.ToString() + Environment.NewLine;
            float[][] alpha;
            List<float[]> alphaList = new List<float[]>();
            for (int i = 0; i < layers.Length; i++)
            {
                alphaList.Add(new float[layers[i]]);
            }
            alpha = alphaList.ToArray();//alpha initialization

            int layer = layers.Length - 2;
            for (int i = 0; i < output.Length; i++) alpha[layers.Length - 1][i] = (output[i] - expected[i]) * activateDer(output[i], layer);//alpha calculation
            for (int i = 0; i < neurons[layers.Length - 1].Length; i++)//calculates the w' and b' for the last layer in the network
            {
                biases[layers.Length - 1][i] -= alpha[layers.Length - 1][i] * learningRate;
                for (int j = 0; j < neurons[layers.Length - 2].Length; j++)
                {
                    weights[layers.Length - 2][i][j] -= alpha[layers.Length - 1][i] * neurons[layers.Length - 2][j] * learningRate;//*learning 
                }
            }
            for (int i = layers.Length - 2; i > 0; i--)//runs on all hidden layers
            {
                layer = i - 1;
                for (int j = 0; j < neurons[i].Length; j++)//outputs
                {
                    alpha[i][j] = 0;
                    for (int k = 0; k < alpha[i + 1].Length; k++)
                    {
                        alpha[i][j] = alpha[i + 1][k] * weights[i][k][j];
                    }
                    alpha[i][j] *= activateDer(neurons[i][j], layer);//calculate alpha
                }
                for (int j = 0; j < neurons[i].Length; j++)//itterate over outputs of layer
                {
                    biases[i][j] -= alpha[i][j] * learningRate;//modify biases of network
                    for (int k = 0; k < neurons[i - 1].Length; k++)//itterate over inputs to layer
                    {
                        weights[i - 1][j][k] -= alpha[i][j] * neurons[i - 1][k] * learningRate;//modify weights of network
                    }
                }
            }
            if (count == 400) { Save("data.txt"); }
        }
        //save and load functions
        public string Load(string path)//this loads the biases and weights from within a file into the neural network.
        {
            try
            {
                loaded_data = "Reading from " + path+Environment.NewLine + Environment.NewLine;
                TextReader tr = new StreamReader(path);
                int NumberOfLines = (int)new FileInfo(path).Length;
                string[] ListLines = new string[NumberOfLines];
                int index = 1;
                for (int i = 1; i < NumberOfLines; i++)
                {
                    string value= tr.ReadLine();
                    if (value == null)
                    {
                        break;
                    }
                    ListLines[i] = value;
                }
                tr.Close();
                if (NumberOfLines > 0)
                {
                   loaded_data += "Calculated  Biases are: " + Environment.NewLine;
                    for (int i = 0; i < biases.Length; i++)
                    {
                        for (int j = 0; j < biases[i].Length; j++)
                        {
                            biases[i][j] = float.Parse(ListLines[index]);
                            loaded_data += "[" + biases[i][j].ToString() + "]";
                            index++;
                        }
                    }
                    loaded_data += Environment.NewLine + Environment.NewLine+ "Calculated Weights are:" + Environment.NewLine;
                    for (int i = 0; i < weights.Length; i++)
                    {
                        int c = i + 1;
                        loaded_data += "For each neuron of Layer: " + c +Environment.NewLine;
                        for (int j = 0; j < weights[i].Length; j++)
                        { 
                            for (int k = 0; k < weights[i][j].Length; k++)
                            {
                                weights[i][j][k] = float.Parse(ListLines[index]);
                                loaded_data += "[" + weights[i][j][k].ToString() + " ]";
                                index++;
                            }
                            loaded_data += "   ";
                        }
                        loaded_data += Environment.NewLine;
                    } }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return loaded_data;
        }
        public void Save(string path)//this is used for saving the biases and weights within the network to a file.
        {
            try
            {
                File.Create(path).Close();
                StreamWriter writer = new StreamWriter(path, true);
                for (int i = 0; i < biases.Length; i++)
                {
                    for (int j = 0; j < biases[i].Length; j++)
                    {
                        writer.WriteLine(biases[i][j]);
                    }
                }
                for (int i = 0; i < weights.Length; i++)
                {
                    for (int j = 0; j < weights[i].Length; j++)
                    {
                        for (int k = 0; k < weights[i][j].Length; k++)
                        {
                            writer.WriteLine(weights[i][j][k]);
                        }
                    }
                }
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public float[] test(string path,float[] inputs)
        {
            Load(path);
            float[] output = FeedForward(inputs);
            return output;
        }
    }
}
