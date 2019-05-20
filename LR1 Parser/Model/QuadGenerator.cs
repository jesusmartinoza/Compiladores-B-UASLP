using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class QuadGenerator
    {
        delegate void  Instruct2Quads(BinaryTreeNode node);
        static List<Quad> Quads;
        static Stack<Quad> ReturnsStack;
        static Stack<string> TempValuesStack;
        static Dictionary<string, Instruct2Quads> Dictionary;
        int TempCounter;

        public  List<Quad> Generate(BinaryTreeNode tree)
        {

            SwitchNodes(tree); 
            return Quads;
        }



        public void SwitchNodes(BinaryTreeNode node)
        {
            switch (node.Content)
            {

                // Crea ventana 1: #5 en Parser
                case "CV1":
                    string idV1 = node.Left.Left.Left.Content;
                    Quads.Add(new Quad("idV", node.Left.Left.Right.Content, null, idV1));
                    Quads.Add(new Quad("posV", node.Left.Right.Left.Left.Content, node.Left.Right.Left.Right.Content, idV1));
                    Quads.Add(new Quad("tamV", node.Left.Right.Right.Left.Content, node.Left.Right.Right.Right.Content, idV1));

                    node.Solved = true;
                    SwitchNodes(node.Right);

                    Quads.Add(new Quad("endV", null, null, idV1));

                    break;

                
                // # 7 en Parser
                case ";":
                    if (!node.Left.Solved)
                        SwitchNodes(node.Left);
                    node.Solved = true;
            
                    SwitchNodes(node.Right);

                    break;

                // Crea Boton: #9 en Parser
                case "CB":
                    string idB = node.Left.Left.Left.Content;
                    Quads.Add(new Quad("idB", node.Left.Left.Right.Content, null, idB));
                    Quads.Add(new Quad("posB", node.Left.Right.Left.Left.Content, node.Left.Right.Left.Right.Content, idB));
                    Quads.Add(new Quad("tamB", node.Left.Right.Right.Left.Content, node.Left.Right.Right.Right.Content, idB));

                    node.Solved = true;
                    SwitchNodes(node.Right);

                    Quads.Add(new Quad("endB",null,null,idB));

                    break;


                // Crea Label: # 11 en Parser
                case "CL":

                    string idL = node.Left.Left.Content;

                    Quads.Add(new Quad("idL", node.Left.Right.Content, null, idL));
                    Quads.Add(new Quad("posL", node.Right.Left.Content, node.Right.Right.Content, idL));
                    

                    node.Solved = true;
                   

                    break;

                // # 26 en Parser
                case "sent-if":
                    SwitchNodes(node.Left);
                    Quad ifCondition = new Quad("GOTOFALSE", TempValuesStack.Pop(), null, null);
                    Quads.Add(ifCondition);
                    SwitchNodes(node.Right);
                    ifCondition.OperandB = Quads.Count - 1;

                    break;
                // #27 en Parser
                case "sent-if-else":
                    SwitchNodes(node.Left);
                    Quad ifElseCondition = new Quad("GOTOFALSE", TempValuesStack.Pop(), null, null);
                    Quads.Add(ifElseCondition);
                    SwitchNodes(node.Left.Right); // --> (else) --> (left)
                    ifElseCondition.OperandB = Quads.Count - 1;
                    SwitchNodes(node.Left.Right);  

                    break;



                // #31 en Parser 
                case "while":

                    SwitchNodes(node.Left);
                    Quad condition = new Quad("GOTOFALSE", TempValuesStack.Pop(), null, null);
                    Quads.Add(condition);
                    SwitchNodes(node.Right);
                    condition.OperandB = Quads.Count - 1;
                    
                    break;
                // # 31 en Parser
                case "do":
                    int quadIndex = Quads.Count - 1;
                    SwitchNodes(node.Left);
                    SwitchNodes(node.Right);
                    Quads.Add(new Quad("GOTOTRUE", TempValuesStack.Pop(), quadIndex.ToString(), null));

                    break;

                case "CT":
                    string idT = node.Left.Content;
                    Quads.Add(new Quad("idT", node.Left.Content, null,idT));
                    Quads.Add(new Quad("postT", node.Right.Left.Left.Content , node.Right.Left.Right.Content, idT));
                    Quads.Add(new Quad("tamT" , node.Right.Right.Left.Content,node.Right.Right.Right.Content,idT));

                    node.Solved = true;                    
                                 
                    break;
                case ":=":
                    if (!node.Right.Solved)
                    {
                        TempCounter++;
                        string t = "t" + TempCounter.ToString();
                        TempValuesStack.Push(t);
                        /*Basandome en: // sent-assign->id := exp;*/
                        Quads.Add(new Quad(":=", t, null, node.Left.Content));
                        SwitchNodes(node.Right);
                        node.Solved = true;
                    }
                    else {
                        Quads.Add(new Quad(":=",node.Right.Content,null,node.Left.Content));
                        node.Solved = true;                    
                    }
                    break;

                case "switch":
                    //Quads.Add(new Quad("swicth",null,null,node.Left.Content));

                    TempValuesStack.Push(node.Left.Content);
                    SwitchNodes(node.Right);

                    break;
                case "case-sep":
                    Quad caseSepTemp = new Quad("GOTOFALSE", null, null, null);
                    Quads.Add(caseSepTemp);
                    SwitchNodes( node.Left);
                    caseSepTemp.OperandA = TempValuesStack.Pop();
                    caseSepTemp.OperandB = Quads.Count - 1;
                    SwitchNodes(node.Right);

                break;

                case "case":
                    TempCounter++;
                    string tmp = "T" + TempCounter.ToString();
                    Quads.Add(new Quad("=", node.Left.Content, TempValuesStack.Peek(),tmp));
                    TempValuesStack.Push(tmp);
                    break;

                case "for":
                    Quads.Add(new Quad("for",null,null,null));
                    SwitchNodes(node.Left);
                    SwitchNodes(node.Right);
                    /***********************************************************************/
                    break;

                case "MS":
                    Quads.Add(new Quad("MS",node.Left.Content,null,node.Content));
                    node.Solved = true;
                    break;

                case "+":
                    if (!node.Right.Solved)
                    {
                        SwitchNodes(node.Right);
                        Quads.Add(new Quad("+", TempValuesStack.Pop(), null, node.Left.Content));
                        node.Solved = true;
                    }
                    else {
                        TempCounter++;
                        string t = "t" + TempCounter.ToString();
                        TempValuesStack.Push(t);
                        Quads.Add(new Quad("+",node.Left.Content,node.Right.Content,t));
                    }
                    break;
                case "*":
                    if (!node.Right.Solved)
                    {
                        SwitchNodes(node.Right);
                        Quads.Add(new Quad("*", TempValuesStack.Pop(), null, node.Left.Content));
                        node.Solved = true;
                    }
                    else
                    {
                        TempCounter++;
                        string t = "t" + TempCounter.ToString();
                        TempValuesStack.Push(t);
                        Quads.Add(new Quad("*", node.Left.Content, node.Right.Content, t));
                    }
                    break;
                case "-":
                    if (!node.Right.Solved)
                    {
                        SwitchNodes(node.Right);
                        Quads.Add(new Quad("-", TempValuesStack.Pop(), null, node.Left.Content));
                        node.Solved = true;
                    }
                    else
                    {
                        TempCounter++;
                        string t = "t" + TempCounter.ToString();
                        TempValuesStack.Push(t);
                        Quads.Add(new Quad("-", node.Left.Content, node.Right.Content, t));
                    }
                    break;
                case "/":
                    if (!node.Right.Solved)
                    {
                        SwitchNodes(node.Right);
                        Quads.Add(new Quad("/", TempValuesStack.Pop(), null, node.Left.Content));
                        node.Solved = true;
                    }
                    else
                    {
                        TempCounter++;
                        string t = "t" + TempCounter.ToString();
                        TempValuesStack.Push(t);
                        Quads.Add(new Quad("/", node.Left.Content, node.Right.Content, t));
                    }
                    break;
            }

        }


       

        private void  GenericNode(BinaryTreeNode node)
        {

           
        }
    }

   


}
