﻿using System;
using System.Data;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
        double[,]  inputFromExcel = new double[100, 4];
        String data = " "; byte[] bytes;
        //Decleration of variables for the initiolization of neural network
        int[] layers = new int[3] { 2, 3, 1 };
        string[] activation = new string[2] { "sigmoid", "sigmoid" };
        // form onload method
        private void Cavitation_Recognition_Load_1(object sender, EventArgs e)
        {
            kb = new Graph();
            np.Load(kb1, @"Hagos_Inconsistency_checking_rules.ttl");
            // set the fixed value of bais for the whole network
            b[0, 0] = -1; b[0, 1] = 1; b[0, 2] = -1; b[0, 3] = 1; b[0, 4] = -1; b[0, 5] = 1; b[0, 6] = -2; b[0, 7] = -2; b[0, 8] = -2; b[0, 9] = -2;

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
        public void CalculateWeight(string state)
            double[] wt = new double[2];
                             WHERE
                                 {
                                  " + state + @" prop:isResultOf ?interval1.
                                  ?interval1 prop:belongTo ?KPI;
                                             prop:minformulaeId ?minId;
                                             prop:midformulaeId ?midId;
                                             prop:maxformulaeId ?maxId.
                                   }";
                    //Calling to Formulae  to get interval value using python Iron
                    min = calculateFormulae(r["minId"].ToString());
                    if (state == "ind:cavitationIndex")
        //Method for call formulae while calculating weights
        public double calculateFormulae(string formulaeId)
        {
            dynamic function = scope.GetVariable("getInterval");
            dynamic result = function(formulaeId);
            return result;
        }
        //display result
        private void output_Click(object sender, EventArgs e)
                double[] data = new double[2];
                {
                    data = getData("output");
                    // if data is from file  
                    x1 = data[0];
                    x2 = data[1];
                    pre.Text = data[0].ToString();
                    flow.Text = data[1].ToString();
                }
                {
                    x1 = double.Parse(pre.Text);
                    x2 = double.Parse(flow.Text);
                }
                mo_op.Text = "Pressure" + "         " + "Flowrate" + Environment.NewLine;
                mo_op.Text +=" "+ Math.Round(x1, 4).ToString() + "                     " + Math.Round(x2, 4).ToString() + Environment.NewLine + Environment.NewLine;
                mo_op.Text += "Current condition is :" + Environment.NewLine;
                if (x1 != 0 && x2 != 0)
                    //hidden layer 2 w21=1.5,w22=1.5,w23=2, w24=3 w25=1.5, w26=1.5, w27=2 w28=3
                    if ((y[1] * 1.5 + y[2] * 1.5 + b[0, 6]) > 0) { y[12] = 1; } else { y[12] = 0; }
                    // Display output
                    if (y[16] == 1)
                    {
                        mo_op.Text += "Incipient Cavitation";
                    }
        //Call method for Saving weight of monitoring system to a file
        private void savew_Click(object sender, EventArgs e)
        {
            Save(path);

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
        private void reset_Click(object sender, EventArgs e)
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
        }
                    {
                        displayInput.Text = Column1 + "            " + Column2  +  Environment.NewLine;
                        for (int i = 0; i < 100; i++)
                        {
                            inputFromExcel[i, 0] = double.Parse(csvTable.Rows[i][0].ToString());
                            inputFromExcel[i, 1] = double.Parse(csvTable.Rows[i][1].ToString());
                            displayInput.Text += csvTable.Rows[i][0].ToString() + "                       " + csvTable.Rows[i][1].ToString()  + Environment.NewLine;
                        }
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
            neuralNetwork net = new neuralNetwork(layers, activation);
            {
                  net.BackPropagate(new float[] {  xy[i, 0], xy[i, 1] }, new float[] { xy[i, 2] });
            }
            float MSE = net.Total_cost/ 400;
            //display.Text= "Summation of Mean squar error is "+net.Total_cost.ToString();
            display.Text = net.cost_total;
            inf.Text = "Weights and Bias are save to a file data.txt...........";
        }
        // read input data from excel file
        public float[,] readInput()
            try
            {
                var csvTable = new DataTable();
                using (var csvReader = new CsvReader(new StreamReader(System.IO.File.OpenRead(@"test.csv")), true))
                {
                    csvTable.Load(csvReader);
            }
            catch (Exception ex) { //MessageBox.Show(ex.Message); 
        //this is used for saving the biases and weights from recognition unit to a file.
        private void test_Click(object sender, EventArgs e)
        {
            string path = "data.txt";
            float[,] xy = readInput();
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

    }