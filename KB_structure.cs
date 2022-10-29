using System;
using System.Collections;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace KE_Course_Work
{
    class KB_structure
    {
        Cavitation_Recognition cr;
        Graph kb;
        public KB_structure(Cavitation_Recognition Ncr)
        {
            cr = Ncr;
            kb = new Graph();
            Notation3Parser np = new Notation3Parser();
            np.Load(kb, @"Hagos_KB1.ttl");
        }
        
        public void graph_KB()
        {
            if (cr.tabControl1.SelectedTab.Text == "Structure_KB")
            {
                cr.TreeView1.Nodes.Add("cavitation recognition Model for Hydro Turbine");//root of the structure
                string targetNode = "predictionModel";
                try
                {
                    String q1 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                          prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                          prefix xsd: <http://www.w3.org/2001/XMLSchema#> 
                          prefix ind: <urn:inds:>
                          prefix prop: <urn:prop:>
                          prefix class: <urn:class:>
                          SELECT  ?type ?label ?subProcessOf 
                             WHERE
                                 {
                                   ?data  rdf:type ?type;
                                          rdfs:label ?label;
                                          prop:subProcessOf ?subProcessOf.
                                    }";
                    //Local variables used for controlling the possition for the treeview
                    // stack data structure 
                    Stack stack = new Stack(); int[] index = { 0, 0, 0, 0, 0 }; int i = 0;
                    int x = 0; String snode = ""; int level = 0;
                    while (stack != null || i == 0)
                    {
                        if (i != 0)
                        {
                            targetNode = stack.Pop().ToString();
                        }
                        int y = 0; i++;
                        SparqlResultSet rs = (SparqlResultSet)kb.ExecuteQuery(q1);
                        foreach (SparqlResult r in rs)
                        {
                            String node = r["label"].ToString();
                            snode = r["subProcessOf"].ToString().Substring(9);
                            if (snode.CompareTo(targetNode) == 0)
                            {
                                stack.Push(r["type"].ToString().Substring(12));
                                y++;
                                if (level == 0)
                                {
                                    cr.TreeView1.Nodes[0].Nodes.Add(node);
                                }
                                else if (level == 1)
                                {
                                    cr.TreeView1.Nodes[0].Nodes[index[0]].Nodes.Add(node);
                                }
                                else if (level == 2)
                                {
                                    cr.TreeView1.Nodes[0].Nodes[index[0]].Nodes[index[1]].Nodes.Add(node);
                                }
                                else if (level == 3)
                                {
                                    cr.TreeView1.Nodes[0].Nodes[index[0]].Nodes[index[1]].Nodes[index[2]].Nodes.Add(node);
                                }
                            }
                        }
                        if (y == 0)
                        {
                            x--; index[level - 1]--;
                        }
                        else
                        {
                            index[level] = y;
                            index[level]--; x = index[level]; level += 1;
                        }
                        while (x == -1) { level--; index[level - 1]--; x = index[level - 1]; }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                cr.display_element.Text = "Click the treeview to show the KB information... " + Environment.NewLine;

            }
        }
        public void display_prop()
        {
            cr.TreeView1.Refresh();
            cr.display_element.Text = " ";
            String item = cr.TreeView1.SelectedNode.Text.ToString();
            String q1 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                          prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                          prefix xsd: <http://www.w3.org/2001/XMLSchema#> 
                          prefix ind: <urn:inds:>
                          prefix prop: <urn:prop:>
                          prefix class: <urn:class:>
                          prefix process: <urn:process:>

                       SELECT  ?type ?label ?hasKPI  ?comment ?subProcessOf
                             WHERE
                                 {
                                  ?data  rdf:type ?type;
                                         rdfs:label ?label;
                                         rdfs:comment ?comment.
                                   OPTIONAL { ?data  prop:subProcessOf ?subProcessOf }                   
                                   OPTIONAL { ?data  prop:hasKPI ?hasKPI}
                                     }";
            try
            {
                SparqlResultSet rs = (SparqlResultSet)kb.ExecuteQuery(q1);
                foreach (SparqlResult r in rs)
                {
                    String node = r["label"].ToString();
                    if (node.CompareTo(item) == 0)
                    {
                        cr.display_element.Text += "RDF Type: " + r["type"].ToString().Substring(12) + Environment.NewLine;
                        cr.display_element.Text += Environment.NewLine + "Label: " + r["label"].ToString() + Environment.NewLine;
                        cr.display_element.Text += Environment.NewLine + "KPI: " + r["hasKPI"].ToString().Substring(9) + Environment.NewLine;
                        cr.display_element.Text += Environment.NewLine + "Comment: " + r["comment"].ToString() + Environment.NewLine;
                        cr.display_element.Text += Environment.NewLine + "Sub Processe of : " + r["subProcessOf"].ToString().Substring(9) + Environment.NewLine;
                    }
                }
                if (cr.display_element.Text == " ")
                {
                    cr.display_element.Text = "Title = Power Prediction Model for Hydro Turbine" + Environment.NewLine; ;
                    cr.display_element.Text += Environment.NewLine + "Description = KB of for the process and sub process of the model";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


    }
}
