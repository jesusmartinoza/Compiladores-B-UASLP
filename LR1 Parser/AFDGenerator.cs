using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    /// <summary>
    /// https://github.com/jesusmartinoza/Compiladores-B-UASLP/wiki#c%C3%A1lculo-de-afd
    /// Módulo desarrollado por ¡Cuervos... Cuervos.... ehhhh!
    /// </summary> 
    class AFDGenerator
    {
        private Tokenizer Productions;
        private List<Node> AFD;

        /// <summary>
        /// AFD Generator constructor.
        /// </summary>
        /// <param name="productions"></param>
        public AFDGenerator(Tokenizer productions)
        {
            Productions = productions;
            AFD = new List<Node>();
        }

        /// <summary>
        /// Main method that returs a fresh and pretty AFD.
        /// </summary>
        /// <returns></returns>
        public List<Node> GenerateAFD()
        {
            AddAugmentedProduction(); //Augmented production 
            Node I0 = GenerateFirstNode(); //TODO solo soporta gramaticas en orden de importancia descendente?
            AFD.Add(I0);

            bool SomethingIsAdded;
            do  //repeat until there are no more new items
            {
                SomethingIsAdded = false;
                foreach (var Nodeitem in AFD)
                {
                    foreach (var GrammarSymbol in Productions.tokens)
                    {
                        Node J = Ir_A(Nodeitem, GrammarSymbol);
                        if(CheckNodeValidityToAdd(J))    //Verify if its Ok to add J 
                        {                                //TODO makes a Relation here 
                            AFD.Add(J);
                            SomethingIsAdded = true;
                        }
                    }
                }

            } while (SomethingIsAdded);
            return AFD;
        }

        /// <summary>
        /// Method that adds augmented production to the grammar
        /// </summary>
        private void AddAugmentedProduction()
        {
            string FirstProduction = Productions.producciones.First().left.Content;

            //Construction the first production 
            Production ProductionToAdd = new Production();
            ProductionToAdd.Left = new Token(FirstProduction + "'", false); //example [FirstProduction = Num] -> Num'
            ProductionToAdd.Right.Add(new Token(FirstProduction, false));   //Adds the first production NT to the rigth

            //adding all the stuff 
            Productions.producciones.Insert(0,ProductionToAdd);  //finally adds the Augmented Production at the start
        }

        /// <summary>
        /// Construction of the first: LR1 element and node
        /// </summary>
        /// <returns></returns>
        private Node GenerateFirstNode()
        {   
            Production FirstProduction = Productions.producciones.First();
            Node I0 = new Node();

            //first LR1 element
            LR1Element FirstElement = new LR1Element(FirstProduction, new List<Token>() { new Token("$", true) });
            I0.Elements.Add(FirstElement);
            I0 = Cerradura(I0);
            return I0;
        }

        /// <summary>
        /// Method method that handles the expansion of Lr1 elements
        /// </summary>
        /// <param name="CurrentNode"></param>
        /// <returns></returns>
        private Node Cerradura(Node CurrentNode)
        {
            bool SomethingIsAdded;
            do  //repeat until there are no more new elements
            {
                SomethingIsAdded = false;
                foreach (var NodeItem in CurrentNode.Elements)
                {   
                    // General syntax [A -> α.Bβ, a]
                    Token B = NodeItem.Gamma.First();
                    if (B.IsTerminal == false)
                    {
                        List<Token> βa = new List<Token>();
                        βa.AddRange(NodeItem.Gamma);    //Add all the Gamma tokens
                        βa.RemoveAt(0);                 //Except for the first token
                        βa.AddRange(NodeItem.Advance);  //Add all the Advance tokens
                        //b is each terminal of Primero(βa).
                        List<Token> b = Primeros.GetPrimerosDe(βa); 

                        List<Production> BProductions = Productions.producciones.FindAll(pred => pred.Left.Content == B.Content); 
                        foreach (var BProduction in BProductions)
                        {   //Finds all productions of B and convert to the Lr1elements of the form [B -> .γ, b]
                            
                            LR1Element BLr1Token = new LR1Element(BProduction,b); //construction B-Production
                            if(!CurrentNode.CheckIfElementExist(BLr1Token))
                            {   //isnt a repeated Lr1 token so its added to the elements list
                                CurrentNode.Elements.Add(BLr1Token);
                                SomethingIsAdded = true;
                            }
                        }
                    }
                }
            } while (SomethingIsAdded);
            return null;
        }

        /// <summary>
        /// method that links a node with a grammatical symbol to generate new nodes 
        /// </summary>
        /// <param name="CurrentNode"></param>
        /// <param name="X"></param>
        /// <returns></returns>
        private Node Ir_A(Node CurrentNode, Token X)
        {
            Node J = new Node();
            // for each element [A -> α.Xβ, a] on CurrentNode
            foreach (var Lr1item in CurrentNode.Elements)
            {
                if(Lr1item.Gamma.First().Content == X.Content) //finded 
                {
                    LR1Element Lr1ToAdd = new LR1Element(Lr1item); 
                    Lr1ToAdd.Alpha.Add(Lr1ToAdd.Gamma.First()); //adds X on alpha
                    Lr1ToAdd.Gamma.RemoveAt(0);                 //removes X from alpha
                    J.Elements.Add(Lr1ToAdd);
                }
            }
            return Cerradura(J);
        }

        /// <summary>
        /// Verify that the input node can be added to the AFD
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        private bool CheckNodeValidityToAdd(Node J)
        {
            if (J.Elements.Count <= 0)  //it's empty, doing nothing here
                return false;
            else
            {
                foreach (var IteratorNode in AFD)
                    if(IteratorNode.CheckNodesEquals(J)) //already exist the same node on the AFD, 
                    {                                    //TODO but makes a Relation here 
                        return false;
                    }
                return true;  //it's OK to add the input node
            }
        }
    }
}
