using System;
using System.Data;using System.IO;using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;using IronPython.Hosting;using LumenWorks.Framework.IO.Csv;using Microsoft.Scripting.Hosting;using VDS.RDF;using VDS.RDF.Parsing;using VDS.RDF.Query;namespace KE_Course_Work{    public partial class Cavitation_Recognition : Form    {        inconsistency_check ic;        KB_structure ks;        neuralNetwork nn;        public Cavitation_Recognition()        {            InitializeComponent();            ic = new inconsistency_check(this);            ks = new KB_structure(this);            nn = new neuralNetwork(this);        }        Graph kb,kb1;        //Global variables for monitoring system        double[,] WeightMatrix = new double[2, 6]; //holds the value of matrix         int[] y = new int[20]; //holds the value of each neuron        int[,] b = new int[2, 10]; // holds value of each bias
        double[,]  inputFromExcel = new double[100, 4];        ScriptEngine engine = Python.CreateEngine();        ScriptScope scope;        String path = "F_StoreWB.txt";        string filePath;// this is set by TCP connection from client
        String data = " "; byte[] bytes;
        //Decleration of variables for the initiolization of neural network
        int[] layers = new int[3] { 2, 3, 1 };
        string[] activation = new string[2] { "sigmoid", "sigmoid" };
        // form onload method
        private void Cavitation_Recognition_Load_1(object sender, EventArgs e)
        {
            kb = new Graph();            kb1 = new Graph();            Notation3Parser np = new Notation3Parser();            np.Load(kb, @"Hagos_KB.ttl");
            np.Load(kb1, @"Hagos_Inconsistency_checking_rules.ttl");            scope = engine.CreateScope();            engine.ExecuteFile(@"Formulae.py", scope);
            // set the fixed value of bais for the whole network
            b[0, 0] = -1; b[0, 1] = 1; b[0, 2] = -1; b[0, 3] = 1; b[0, 4] = -1; b[0, 5] = 1; b[0, 6] = -2; b[0, 7] = -2; b[0, 8] = -2; b[0, 9] = -2;            b[1, 0] = 1; b[1, 1] = -1; b[1, 2] = 1; b[1, 3] = -1; b[1, 4] = 1; b[1, 5] = -1; b[1, 6] = -2; b[1, 7] = -2; b[1, 8] = -2; b[1, 9] = -2;

        }
       //Method and opertions for file transfer
       //recieve file from TCP/IP cleint
        private void Get_Click_1(object sender, EventArgs e)
        {
            try { 
               int port = int.Parse(portno.Text);
                String ip = ipAddress.Text;
                TcpListener server = new TcpListener(IPAddress.Parse(ip), port);
                // Start listening for client requests
                 MessageBox.Show("Connect to server/client ", " procced to continue", MessageBoxButtons.OK,MessageBoxIcon.Information);
                server.Start();
                //Buffer for reading data
                bytes = new byte[1024];
                //Enter the listening loop
                TcpClient client = server.AcceptTcpClient();
                StreamReader sr = new StreamReader(client.GetStream());
                //If tcp is mobile 
                NetworkStream stream = client.GetStream();
                int i;
                // Loop to receive all the data sent by the client.
                i = stream.Read(bytes, 0, bytes.Length);
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                MessageBox.Show("Data is recieved from cliet ");
                URLaddress.Enabled = true;
                browse.Enabled = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //browse location of file to save
        private void Browse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog op = new FolderBrowserDialog();
            if (op.ShowDialog() == DialogResult.OK)
            {
                URLaddress.Text = op.SelectedPath;
                sav.Enabled = true;
            }
        }
        //Saving TCP/IP formulae transfer
        private void sav_Click_1(object sender, EventArgs e)
        {
            string filename = "Folrmulae.py";
            bytes = Encoding.ASCII.GetBytes(data);
            File.WriteAllBytes(URLaddress.Text + "\\" + filename, bytes);
            filePath = URLaddress.Text + "\\" + filename;
            label1.Text = "File is saved......";
            MessageBox.Show("File is saved......");
        }

        //Monitoring System Methods 
        //Calling for weigth calculation
        private void WeightCal_Click(object sender, EventArgs e)
        {
            //calling of methods and generate the weight matrix based on the KB
            CalculateWeight("ind:cavitationIndex");
            CalculateWeight("ind:flowRateIndex");
            textBox1.Text = " Weights for KBNN is calculated and stored in an array" + Environment.NewLine+Environment.NewLine;
            textBox1.Text += "[" + WeightMatrix[0, 0] + "  " + WeightMatrix[0, 1] + "  " + WeightMatrix[0, 2] + "  " + WeightMatrix[0, 3] + "  " + WeightMatrix[0, 4] + "  " + WeightMatrix[0, 5] + Environment.NewLine;
            textBox1.Text += "[" + WeightMatrix[1, 0] + "  " + WeightMatrix[1, 1] + "  " + WeightMatrix[1, 2] + "  " + WeightMatrix[1, 3] + " ]" + WeightMatrix[1, 4] + "  " + WeightMatrix[1, 5] + Environment.NewLine;
            savew.Enabled = true;
            output.Enabled = true;
        }
        //Method for calculation of weights
        public void CalculateWeight(string state)        {
            double[] wt = new double[2];            double min, mid, max;            String q1 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>                           prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#>                           prefix xsd: <http://www.w3.org/2001/XMLSchema#>                           prefix ind: <urn:inds:>                          prefix prop: <urn:prop:>                          prefix class: <urn:class:>                          prefix process: <urn:process:>                          SELECT  ?KPI ?minId ?midId ?maxId 
                             WHERE
                                 {
                                  " + state + @" prop:isResultOf ?interval1.
                                  ?interval1 prop:belongTo ?KPI;
                                             prop:minformulaeId ?minId;
                                             prop:midformulaeId ?midId;
                                             prop:maxformulaeId ?maxId.
                                   }";            try            {                SparqlResultSet rs = (SparqlResultSet)kb.ExecuteQuery(q1);                foreach (SparqlResult r in rs)                {
                    //Calling to Formulae  to get interval value using python Iron
                    min = calculateFormulae(r["minId"].ToString());                    mid = calculateFormulae(r["midId"].ToString());                    max = calculateFormulae(r["maxId"].ToString());
                    if (state == "ind:cavitationIndex")                    {                        WeightMatrix[0, 0] = Math.Round((1 / max), 3);                        WeightMatrix[0, 1] = Math.Round((-1 / max), 3);                        WeightMatrix[0, 2] = Math.Round((1 / mid), 3);                        WeightMatrix[0, 3] = Math.Round((-1 / mid), 3);                        WeightMatrix[0, 4] = Math.Round((1 / min), 3);                        WeightMatrix[0, 5] = Math.Round((-1 / min), 3);                    }                    else if (state == "ind:flowRateIndex")                    {                        WeightMatrix[1, 0] = Math.Round((-1 / min), 3);                        WeightMatrix[1, 1] = Math.Round((1 / min), 3);                        WeightMatrix[1, 2] = Math.Round((-1 / mid), 3);                        WeightMatrix[1, 3] = Math.Round((1 / mid), 3);                        WeightMatrix[1, 4] = Math.Round((-1 / max), 3);                        WeightMatrix[1, 5] = Math.Round((1 / max), 3);                    }                }            }            catch (Exception ex)            {                MessageBox.Show(ex.Message);            }        }
        //Method for call formulae while calculating weights
        public double calculateFormulae(string formulaeId)
        {
            dynamic function = scope.GetVariable("getInterval");
            dynamic result = function(formulaeId);
            return result;
        }
        //display result
        private void output_Click(object sender, EventArgs e)        {            try            {
                double[] data = new double[2];                double x2, x1;                if (pre.Text == "" || flow.Text == " ")
                {
                    data = getData("output");
                    // if data is from file  
                    x1 = data[0];
                    x2 = data[1];
                    pre.Text = data[0].ToString();
                    flow.Text = data[1].ToString();
                }                else
                {
                    x1 = double.Parse(pre.Text);
                    x2 = double.Parse(flow.Text);
                }
                mo_op.Text = "Pressure" + "         " + "Flowrate" + Environment.NewLine;
                mo_op.Text +=" "+ Math.Round(x1, 4).ToString() + "                     " + Math.Round(x2, 4).ToString() + Environment.NewLine + Environment.NewLine;
                mo_op.Text += "Current condition is :" + Environment.NewLine;
                if (x1 != 0 && x2 != 0)                {                    //hidden layer1                    if ((WeightMatrix[0, 0] * x1 + b[0, 0]) >= 0) { y[0] = 1; } else { y[0] = 0; }                    if ((WeightMatrix[0, 1] * x1 + b[0, 1]) >= 0) { y[1] = 1; } else { y[1] = 0; }                    if ((WeightMatrix[0, 2] * x1 + b[0, 2]) >= 0) { y[2] = 1; } else { y[2] = 0; }                    if ((WeightMatrix[0, 3] * x1 + b[0, 3]) >= 0) { y[3] = 1; } else { y[3] = 0; }                    if ((WeightMatrix[0, 4] * x1 + b[0, 4]) >= 0) { y[4] = 1; } else { y[4] = 0; }                    if ((WeightMatrix[0, 5] * x1 + b[0, 5]) >= 0) { y[5] = 1; } else { y[5] = 0; }                    if ((WeightMatrix[1, 0] * x2 + b[1, 0]) >= 0) { y[6] = 1; } else { y[6] = 0; }                    if ((WeightMatrix[1, 1] * x2 + b[1, 1]) >= 0) { y[7] = 1; } else { y[7] = 0; }                    if ((WeightMatrix[1, 2] * x2 + b[1, 2]) >= 0) { y[8] = 1; } else { y[8] = 0; }                    if ((WeightMatrix[1, 3] * x2 + b[1, 3]) >= 0) { y[9] = 1; } else { y[9] = 0; }                    if ((WeightMatrix[1, 4] * x2 + b[1, 4]) >= 0) { y[10] = 1; } else { y[10] = 0; }                    if ((WeightMatrix[1, 5] * x2 + b[1, 5]) >= 0) { y[11] = 1; } else { y[11] = 0; }
                    //hidden layer 2 w21=1.5,w22=1.5,w23=2, w24=3 w25=1.5, w26=1.5, w27=2 w28=3
                    if ((y[1] * 1.5 + y[2] * 1.5 + b[0, 6]) > 0) { y[12] = 1; } else { y[12] = 0; }                    if ((y[3] * 1.5 + y[4] * 1.5 + b[0, 7]) > 0) { y[13] = 1; } else { y[13] = 0; }                    //MessageBox.Show(y[12].ToString()+"layer 2"+ y[13].ToString());                    if ((y[7] * 1.5 + y[8] * 1.5 + b[1, 6]) > 0) { y[14] = 1; } else { y[14] = 0; }//and operation                    if ((y[9] * 1.5 + y[10] * 1.5 + b[1, 7]) > 0) { y[15] = 1; } else { y[15] = 0; }    //or operation                    if ((y[0] * 1.5 + y[6] * 1.5 + b[0, 8]) > 0) { y[16] = 1; } else { y[16] = 0; }                    if ((y[12] * 1.5 + y[14] * 1.5 + b[0, 9]) > 0) { y[17] = 1; } else { y[17] = 0; }                    if ((y[13] * 1.5 + y[15] * 1.5 + b[1, 8]) > 0) { y[18] = 1; } else { y[18] = 0; }                    if ((y[5] * 1.5 + y[11] * 1.5 + b[1, 9]) > 0) { y[19] = 1; } else { y[19] = 0; }
                    // Display output
                    if (y[16] == 1)                    {                        mo_op.Text += "No Cavitation";                    }                    else if (y[17] == 1)                    {                        mo_op.Text += "Incipient Cavitation";                    }                   else if (y[18] == 1)                    {                        mo_op.Text += "Developed Cavitation";                    }                   else if (y[19] == 1)                    {                        mo_op.Text += "Super Cavitation";                    }                    else
                    {
                        mo_op.Text += "Incipient Cavitation";
                    }                }                else                {                    MessageBox.Show("Please insert input value", "Alert Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);                }            }            catch(Exception ex)            {                MessageBox.Show(ex.Message);                mo_op.Text = ex.Message;            }        }
        //Call method for Saving weight of monitoring system to a file
        private void savew_Click(object sender, EventArgs e)
        {
            Save(path);          infor.Text="Weights are stored in a file....";

        }
        //this is used for saving the biases and weights to a file.
        public void Save(string path)
        {
            File.Create(path).Close();
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine("Bias: ");
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    writer.WriteLine(b[i, j]);
                }
            }
            writer.WriteLine("Weight: ");
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    writer.WriteLine(WeightMatrix[i, j]);
                }
            }
            writer.Close();
        }
        //reset textfield
        private void reset_Click(object sender, EventArgs e)        {            textBox1.Text = " ";   mo_op.Text = " ";         }
        //information system 
        //to display the data at interval of time
        private void input_Info_Click(object sender, EventArgs e)
        {
            getData("display");
            dynamic_Monitoring();
        }
        //dynamic interval function
        public void dynamic_Monitoring()
        {
            //dynamic change with time
            System.Windows.Forms.Timer dynamic = new System.Windows.Forms.Timer();
            dynamic.Interval = 20000;//20 seconds
            dynamic.Tick += new System.EventHandler(input_Info_Click);
            dynamic.Tick += new System.EventHandler(output_Click);
            dynamic.Start();
        }        public double [] getData(string request)        {            double[] input = new double[3];            var csvTable = new DataTable();            using (var csvReader = new CsvReader(new StreamReader(System.IO.File.OpenRead(@"train.csv")), true))            {                                try                {                    csvTable.Load(csvReader);                    string Column1 = csvTable.Columns[0].ToString();                    string Column2 = csvTable.Columns[1].ToString();                    string Column3 = csvTable.Columns[2].ToString();                    input[0] = double.Parse(csvTable.Rows[0][0].ToString());                    input[1] = double.Parse(csvTable.Rows[0][1].ToString());                    input[2] = double.Parse(csvTable.Rows[0][2].ToString());                    if (request == "display")
                    {
                        displayInput.Text = Column1 + "            " + Column2  +  Environment.NewLine;
                        for (int i = 0; i < 100; i++)
                        {
                            inputFromExcel[i, 0] = double.Parse(csvTable.Rows[i][0].ToString());
                            inputFromExcel[i, 1] = double.Parse(csvTable.Rows[i][1].ToString());
                            displayInput.Text += csvTable.Rows[i][0].ToString() + "                       " + csvTable.Rows[i][1].ToString()  + Environment.NewLine;
                        }                    }                }                catch(Exception ex) { MessageBox.Show(ex.Message); textBox1.Text = ex.Message; }            }            return input;        }       
        //display the structur of KB after change tab by
        //call graph_KB() from Kb_structur class
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            KB_structure ks = new KB_structure(this);
            ks.graph_KB();
        }
        //action performed after select each tree view node
        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            KB_structure ks = new KB_structure(this);
            ks.display_prop();
        }
        //recognition system model
        // calling neuraNnetwork class functions
        private void NeuralNW_Click(object sender, EventArgs e)
        {
            neuralNetwork net = new neuralNetwork(layers, activation);            float[,] xy = readInput();            for (int i = 0; i <110; i++)
            {
                  net.BackPropagate(new float[] {  xy[i, 0], xy[i, 1] }, new float[] { xy[i, 2] });
            }
            float MSE = net.Total_cost/ 400;
            //display.Text= "Summation of Mean squar error is "+net.Total_cost.ToString();
            display.Text = net.cost_total;
            inf.Text = "Weights and Bias are save to a file data.txt...........";
        }
        // read input data from excel file
        public float[,] readInput()        {            float[,] input = new float[400, 3];
            try
            {
                var csvTable = new DataTable();
                using (var csvReader = new CsvReader(new StreamReader(System.IO.File.OpenRead(@"test.csv")), true))
                {
                    csvTable.Load(csvReader);                    for (int i = 0; i < 100; i++)                    {                        input[i, 0] = float.Parse(csvTable.Rows[i][0].ToString());                        input[i, 1] = float.Parse(csvTable.Rows[i][1].ToString());                        input[i, 2] = float.Parse(csvTable.Rows[i][2].ToString());                    }                }
            }
            catch (Exception ex) { //MessageBox.Show(ex.Message);             }            return input;        }
        //this is used for saving the biases and weights from recognition unit to a file.
        private void test_Click(object sender, EventArgs e)
        {
            string path = "data.txt";
            float[,] xy = readInput();            neuralNetwork net = new neuralNetwork(layers, activation);            display.Text = "The result after feed " + Environment.NewLine + " pressure" + "      " + "flow rate " + "  " + "Expected" + "     " + "Output" + Environment.NewLine;
            for (int i = 0; i < 20; i++)
            {
                float []output=net.test(path, new float[] { xy[i, 0], xy[i, 1] });
                display.Text +="          "+ xy[i, 0]+"            " + xy[i, 1]+"               " +xy[i, 2]+"          "+ (output[0]*20).ToString()+ Environment.NewLine;
            }
        }
        //Load weight 
        private void load_Click(object sender, EventArgs e)
        {
            string path = "data.txt";
            neuralNetwork net = new neuralNetwork(layers,activation);
            display.Text = net.Load(path);
            inf.Text = "Weights and Bias are loaded to array from  a file data.txt...........";
        }
        //Calling of inconsistency checking class for checking inconsistncy
        //
        private void Check_Click_1(object sender, EventArgs e)
        {
            inconsistency_check ic = new inconsistency_check(this);
            ic.check_inconsistency();
        }
        //clear all checkbox
        private void clearAll_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = false; checkBox2.Checked = false; checkBox3.Checked = false; checkBox4.Checked = false; checkBox5.Checked = false;
            checkBox6.Checked = false; checkBox7.Checked = false; checkBox8.Checked = false; checkBox9.Checked = false;
        }
        private void R_reset_Click(object sender, EventArgs e)
        {
            display.Text = " ";
        }
        //sellect all checkbox
        private void selectAll_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = true; checkBox2.Checked = true; checkBox3.Checked = true; checkBox4.Checked = true; checkBox5.Checked = true;
            checkBox6.Checked = true; checkBox7.Checked = true; checkBox8.Checked = true; checkBox9.Checked = true;
        }

    }}