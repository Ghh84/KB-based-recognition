using System;
using System.Windows.Forms;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace KE_Course_Work 
{
    class inconsistency_check
    {
        Cavitation_Recognition cr;
        public inconsistency_check(Cavitation_Recognition Ncr)
        {
            cr = Ncr;
        }
        public void check_inconsistency()
        {
            Graph kb = new Graph();
            Graph kb1 = new Graph();
            Notation3Parser np = new Notation3Parser();
            np.Load(kb, @"Hagos_KB.ttl");
            np.Load(kb1, @"Hagos_Inconsistency_checking_rules.ttl");
            cr.disp_Inc.Text = " ";
            int counter = 0;
            // inconsistency checking
            if (cr.checkBox1.Checked == true)
            {
                String q1 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                          prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                          prefix owl: <http://www.w3.org/2002/07/owl#>
                          prefix ind: <urn:inds:>
                          prefix prop: <urn:prop:>
                          prefix class: <urn:class:>
                          prefix process: <urn:process:> 
                       SELECT  ?hasKPI ?process
                             WHERE
                                 {
                                  ?hasKPI rdfs:domain ?process.
                                  }";

                SparqlResultSet rs = (SparqlResultSet)kb1.ExecuteQuery(q1);
                String q2 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                          prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                          prefix owl: <http://www.w3.org/2002/07/owl#>
                          prefix ind: <urn:inds:>
                          prefix prop: <urn:prop:>
                          prefix class: <urn:class:>
                          prefix process: <urn:process:> 
                       SELECT  ?ind1 ?ind2
                             WHERE
                                 {
                                 ?ind1 <" + rs[0]["hasKPI"].ToString() + @"> ?ind2.
                                 FILTER EXISTS{?ind1 a <" + rs[0]["process"].ToString() + @">}
                                   }";
                SparqlResultSet r = (SparqlResultSet)kb.ExecuteQuery(q2);
                foreach (SparqlResult rs1 in r)
                {
                    if (counter == 0)
                    {
                        cr.disp_Inc.Text += Environment.NewLine + "Domain Inconsistency are:" + Environment.NewLine;
                    }
                    cr.disp_Inc.Text += rs1["ind1"].ToString() + "  ,  " + rs[0]["process"].ToString() + Environment.NewLine;
                    counter++;
                }
            }
            if (cr.checkBox2.Checked == true)
            {
                counter = 0;
                String q1 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                          prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                          prefix owl: <http://www.w3.org/2002/07/owl#>
                          prefix ind: <urn:inds:>
                          prefix prop: <urn:prop:>
                          prefix class: <urn:class:>
                          prefix process: <urn:process:> 
                       SELECT  ?hasKPI ?process
                             WHERE
                                 {
                                  ?hasKPI rdfs:range ?process.
                                  }";

                SparqlResultSet rs = (SparqlResultSet)kb1.ExecuteQuery(q1);
                String q2 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                              prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                              prefix owl: <http://www.w3.org/2002/07/owl#>
                              prefix ind: <urn:inds:>
                              prefix prop: <urn:prop:>
                              prefix class: <urn:class:>
                              prefix process: <urn:process:> 
                       SELECT  ?ind1 ?object
                             WHERE
                                 {
                                  ind:predResult <" + rs[0]["hasKPI"].ToString() + @"> ?object.
                                  ?object a ?ind1.
                                  FILTER(?object!=<" + rs[0]["process"].ToString() + @">)
                                  }";
                SparqlResultSet r = (SparqlResultSet)kb.ExecuteQuery(q2);
                foreach (SparqlResult rs1 in r)
                {
                    if (counter == 0)
                    {
                        cr.disp_Inc.Text += Environment.NewLine + "Range Inconsistency are:" + Environment.NewLine;
                    }
                    cr.disp_Inc.Text += rs1["object"].ToString() + " of object->" + rs[0]["process"].ToString() + Environment.NewLine;
                    counter++;
                }

            }
            if (cr.checkBox3.Checked == true)
            {
                counter = 0;
                String q1 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                          prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                          prefix owl: <http://www.w3.org/2002/07/owl#>
                          prefix ind: <urn:inds:>
                          prefix prop: <urn:prop:>
                          prefix class: <urn:class:>
                          prefix process: <urn:process:> 
                       SELECT  ?ind1 ?ind2 ?ind3 ?ind4
                             WHERE
                                 {
                                     ?ind1 prop:subProcessOf ?ind4.
                                     ?ind2 prop:subProcessOf ?ind4.
                                     ?ind3 a owl:Class;
                                               owl:oneOf(?ind1 ?ind2).
                                  }";

                SparqlResultSet rs = (SparqlResultSet)kb1.ExecuteQuery(q1);
                String q2 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                              prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                              prefix owl: <http://www.w3.org/2002/07/owl#>
                              prefix ind: <urn:inds:>
                              prefix prop: <urn:prop:>
                              prefix class: <urn:class:>
                              prefix process: <urn:process:> 
                       SELECT  ?ind1 
                             WHERE
                                 {
                                  ?ind1 prop:subProcessOf <" + rs[0]["ind3"].ToString() + @">.
                                   FILTER(?ind1!=<" + rs[0]["ind1"].ToString() + @">)
                                   FILTER(?ind1!=<" + rs[0]["ind2"].ToString() + @">)
                                  }";
                SparqlResultSet r = (SparqlResultSet)kb.ExecuteQuery(q2);
                foreach (SparqlResult rs1 in r)
                {
                    if (counter == 0)
                    {
                        cr.disp_Inc.Text += Environment.NewLine + "oneOf Inconsistency are:" + Environment.NewLine;
                    }
                    cr.disp_Inc.Text += rs1["ind1"].ToString() + " subProcessOf->" + rs[0]["ind4"].ToString() + Environment.NewLine;
                    counter++;
                }
            }
            if (cr.checkBox4.Checked == true)
            {
                counter = 0;
                //DisjointWith
                String q1 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                          prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                          prefix owl: <http://www.w3.org/2002/07/owl#>
                          prefix ind: <urn:inds:>
                          prefix prop: <urn:prop:>
                          prefix class: <urn:class:>
                          prefix process: <urn:process:> 
                       SELECT  ?process1 ?process2 
                             WHERE
                                 {
                                    ?process1 a owl:Class.
                                    ?process2 a owl:Class.
                                    ?process1 owl:disjointWith ?process2.
                                  }";

                SparqlResultSet rs = (SparqlResultSet)kb1.ExecuteQuery(q1);
                String q2 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                              prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                              prefix owl: <http://www.w3.org/2002/07/owl#>
                              prefix ind: <urn:inds:>
                              prefix prop: <urn:prop:>
                              prefix class: <urn:class:>
                              prefix process: <urn:process:> 
                       SELECT  ?ind1 
                             WHERE
                                 {
                                   ?ind1 a <" + rs[0]["process1"].ToString() + @"> .
                                   ?ind1 a <" + rs[0]["process2"].ToString() + @"> .
                                  }";
                SparqlResultSet r = (SparqlResultSet)kb.ExecuteQuery(q2);
                foreach (SparqlResult rs1 in r)
                {
                    if (counter == 0)
                    {
                        cr.disp_Inc.Text += Environment.NewLine + "disjointWith Inconsistency are:" + Environment.NewLine;
                    }
                    cr.disp_Inc.Text += " disjointWith at ->" + rs1["ind1"].ToString() + Environment.NewLine;
                    counter++;
                }
            }
            if (cr.checkBox5.Checked == true)
            {
                counter = 0; ;
                //AllDisjointWith
                String q1 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                          prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                          prefix owl: <http://www.w3.org/2002/07/owl#>
                          prefix ind: <urn:inds:>
                          prefix prop: <urn:prop:>
                          prefix class: <urn:class:>
                          prefix process: <urn:process:> 
                          SELECT  ?ind1 ?ind2 ?ind3
                             WHERE
                                 { 
                                    ?ind1 a owl:Class.
                                    ?ind2 a owl:Class.
                                    ?ind3 a owl:Class.
                                    [] a owl:AllDisjointClasses;
                                    owl:members(?ind1 ?ind2 ?ind3).
                                  }";

                SparqlResultSet rs = (SparqlResultSet)kb1.ExecuteQuery(q1);
                String q2 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                              prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                              prefix owl: <http://www.w3.org/2002/07/owl#>
                              prefix ind: <urn:inds:>
                              prefix prop: <urn:prop:>
                              prefix class: <urn:class:>
                              prefix process: <urn:process:> 
                       SELECT  ?ind1 ?ind2 ?ind3
                             WHERE
                                 {
                                   ?ind1 a <" + rs[0]["ind1"].ToString() + @">.
                                   ?ind2 a <" + rs[0]["ind2"].ToString() + @">.
                                   ?ind3 a <" + rs[0]["ind3"].ToString() + @">.
                                   FILTER(?ind1 = ?ind2)
                                   FILTER(?ind2 = ?ind3)
                                   FILTER(?ind1 = ?ind3)
                                  }";
                SparqlResultSet r = (SparqlResultSet)kb.ExecuteQuery(q2);
                foreach (SparqlResult rs1 in r)
                {
                    if (counter == 0)
                    {
                        cr.disp_Inc.Text += Environment.NewLine + "AllDisjointWith Inconsistency are:" + Environment.NewLine;
                    }
                    cr.disp_Inc.Text += "AlldisjointWith at->" + rs1["ind1"].ToString() + "  and " + rs1["ind2"].ToString() + "  and " + rs1["ind3"].ToString() + Environment.NewLine;
                    counter++;
                }
            }
            if (cr.checkBox6.Checked == true)//propertyDisjointWith
            {
                counter = 0;
                String q1 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                          prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                          prefix owl: <http://www.w3.org/2002/07/owl#>
                          prefix ind: <urn:inds:>
                          prefix prop: <urn:prop:>
                          prefix class: <urn:class:>
                          prefix process: <urn:process:> 
                       SELECT  ?prop1 ?prop2     
                             WHERE
                                 {
                                  ?prop1 a owl:ObjectProperty.
                                  ?prop2 a owl:ObjectProperty.
                                  ?prop1 owl:propertyDisjointWith ?prop2.
                                  }";
                SparqlResultSet rs = (SparqlResultSet)kb1.ExecuteQuery(q1);
                String q2 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                              prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                              prefix owl: <http://www.w3.org/2002/07/owl#>
                              prefix ind: <urn:inds:>
                              prefix prop: <urn:prop:>
                              prefix class: <urn:class:>
                              prefix process: <urn:process:> 
                       SELECT  ?ind1 ?ind2 ?ind3
                             WHERE
                                 {
                                  ?ind1 <" + rs[0]["prop1"].ToString() + @"> ?ind2.
                                  ?ind1 <" + rs[0]["prop2"].ToString() + @"> ?ind2.
                                  }";
                SparqlResultSet r = (SparqlResultSet)kb.ExecuteQuery(q2);
                foreach (SparqlResult rs1 in r)
                {
                    if (counter == 0)
                    {
                        cr.disp_Inc.Text += Environment.NewLine + "propertDisjointWith Inconsistency are:" + Environment.NewLine;

                    }
                    cr.disp_Inc.Text += "propertyDisjointWith->" + rs1["ind1"].ToString() + Environment.NewLine;
                    counter++;
                }
            }
            if (cr.checkBox7.Checked == true)//IrreflexiveProperty
            {
                counter = 0;
                String q1 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                          prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                          prefix owl: <http://www.w3.org/2002/07/owl#>
                          prefix ind: <urn:inds:>
                          prefix prop: <urn:prop:>
                          prefix class: <urn:class:>
                          prefix process: <urn:process:> 
                       SELECT  ?prop     
                             WHERE
                                 {
                                  ?prop a owl:ObjectProperty,
                                          owl:IrreflexiveProperty.
                                  }";
                SparqlResultSet rs = (SparqlResultSet)kb1.ExecuteQuery(q1);
                String q2 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                              prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                              prefix owl: <http://www.w3.org/2002/07/owl#>
                              prefix ind: <urn:inds:>
                              prefix prop: <urn:prop:>
                              prefix class: <urn:class:>
                              prefix process: <urn:process:> 
                       SELECT  ?ind1 ?ind2
                             WHERE
                                 {
                                  ?ind1 <" + rs[0]["prop"].ToString() + @"> ?ind1.
                                  }";//FILTER NOT EXISTS(?ind1 <" + rs[0]["prop"].ToString() + @"> ?ind1)
                SparqlResultSet r = (SparqlResultSet)kb.ExecuteQuery(q2);
                foreach (SparqlResult rs1 in r)
                {
                    if (counter == 0)
                    {
                        cr.disp_Inc.Text += Environment.NewLine + "IrreflexiveProperty Inconsistency are:" + Environment.NewLine;
                    }
                    cr.disp_Inc.Text += "IrreflexiveProperty at->" + rs1["ind1"].ToString() + Environment.NewLine;
                    counter++;
                }
            }
            if (cr.checkBox8.Checked == true)//AsymmetricProperty
            {
                counter = 0;
                String q1 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                          prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                          prefix owl: <http://www.w3.org/2002/07/owl#>
                          prefix ind: <urn:inds:>
                          prefix prop: <urn:prop:>
                          prefix class: <urn:class:>
                          prefix process: <urn:process:> 
                       SELECT  ?prop     
                             WHERE
                                 {
                                  ?prop a owl:ObjectProperty,
                                          owl:AsymmetricProperty.
                                  }";
                SparqlResultSet rs = (SparqlResultSet)kb1.ExecuteQuery(q1);
                String q2 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                              prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                              prefix owl: <http://www.w3.org/2002/07/owl#>
                              prefix ind: <urn:inds:>
                              prefix prop: <urn:prop:>
                              prefix class: <urn:class:>
                              prefix process: <urn:process:> 
                          SELECT  ?ind1 ?ind2 
                             WHERE
                                 {
                                  ?ind1 <" + rs[0]["prop"].ToString() + @"> ?ind2.
                                  FILTER EXISTS{?ind2 <" + rs[0]["prop"].ToString() + @"> ?ind1.}
                                  }";
                SparqlResultSet r = (SparqlResultSet)kb.ExecuteQuery(q2);
                foreach (SparqlResult rs1 in r)
                {
                    if (counter == 0)
                    {
                        cr.disp_Inc.Text += Environment.NewLine + "AsymmetricProperty Inconsistency are:" + Environment.NewLine;
                    }
                    cr.disp_Inc.Text += "AsymmetricProperty->" + rs1["ind1"].ToString() + Environment.NewLine;
                    counter++;
                }
            }
            if (cr.checkBox9.Checked == true)//NegativePropertyAssertion
            {
                counter = 0;
                String q1 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                          prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                          prefix owl: <http://www.w3.org/2002/07/owl#>
                          prefix ind: <urn:inds:>
                          prefix prop: <urn:prop:>
                          prefix class: <urn:class:>
                          prefix process: <urn:process:> 
                          SELECT  ?ind1 ?ind2 ?prop     
                             WHERE
                                 {
                                  ind:np1 a owl:NegativePropertyAssertion;
                                                   owl:sourceIndividual ?ind1;
                                                   owl:assertionProperty ?prop;
                                                   owl:targetIndividual ?ind2.
                                  }";
                SparqlResultSet rs = (SparqlResultSet)kb1.ExecuteQuery(q1);
                String q2 = @"prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
                              prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
                              prefix owl: <http://www.w3.org/2002/07/owl#>
                              prefix ind: <urn:inds:>
                              prefix prop: <urn:prop:>
                              prefix class: <urn:class:>
                              prefix process: <urn:process:> 
                       SELECT   ?target
                             WHERE
                                 {
                                    <" + rs[0]["ind1"].ToString() + @"> <" + rs[0]["prop"].ToString() + @"> ?target.
                                    FILTER(?target = <" + rs[0]["ind2"].ToString() + @">)
                                    }";
                SparqlResultSet r = (SparqlResultSet)kb1.ExecuteQuery(q2);
                foreach (SparqlResult rs1 in r)
                {
                    if (counter == 0)
                    {
                        cr.disp_Inc.Text += Environment.NewLine + "NegativePropertyAssertion Inconsistency are:" + Environment.NewLine;
                    }
                    cr.disp_Inc.Text += "NegativePropertyAssertion>" + rs1["target"].ToString() + Environment.NewLine;
                    counter++;
                }
            }
            if (cr.disp_Inc.Text == " ")
            {
                MessageBox.Show(" There is no any inconsistency in the KB.", " Inconsistency checking", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cr.disp_Inc.Text = "No inconsistency is found";
            }
        }


    }
}
