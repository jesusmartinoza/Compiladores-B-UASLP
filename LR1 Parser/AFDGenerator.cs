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
        private List<Production> Productions;
        static internal List<Token> GrammarSymbols;
        private Primeros Prims;
        private List<Node> AFD;

        enum ValidationOutput { NothingToDo, AlreadyExisit, NewRelation };
        struct ValNodeResult
        {
            public ValidationOutput ValOut;
            public int IndexFinded;
        }

        /// <summary>
        /// AFD Generator constructor.
        /// </summary>
        /// <param name="productions"></param>
        /// <param name="inPrims"></param>
        /// <param name="inGramSim"></param>
        public AFDGenerator(List<Production> productions, Primeros inPrims, List<Token> inGramSim)
        {
            Productions = productions;
            GrammarSymbols = inGramSim;
            Prims = inPrims;
            AFD = new List<Node>();
        }

        /// <summary>
        /// Main method that returs a fresh and pretty AFD.
        /// </summary>
        /// <returns></returns>
        public List<Node> GenerateAFD()
        {
            AddAugmentedProduction(); //Augmented production 
            Node I0 = GenerateFirstNode(); //olo soporta gramaticas en orden de importancia descendente? creo que si wey
            AFD.Add(I0);

            bool SomethingIsAdded;
            do  //repeat until there are no more new items
            {
                SomethingIsAdded = false;
                foreach (Node Nodeitem in AFD.ToList())
                {
                    foreach (Token GrammarSymbol in GrammarSymbols.ToList())
                    {
                        Node J = Ir_A(Nodeitem, GrammarSymbol);
                        ValNodeResult Result = CheckNodeValidityToAdd(J); //Verify J content 
                        switch (Result.ValOut)
                        {
                            case ValidationOutput.NothingToDo:
                                //Literally nothing to do LOL
                                break;
                            case ValidationOutput.AlreadyExisit:
                                //Create Relation: Nodeitem -> GrammarSymbol -> J_index(already exist)
                                if(!Nodeitem.Edges.ContainsKey(Result.IndexFinded))
                                    Nodeitem.Edges.Add(Result.IndexFinded, GrammarSymbol);
                                break;
                            case ValidationOutput.NewRelation:
                                AFD.Add(J);
                                SomethingIsAdded = true;
                                //Create Relation: Nodeitem -> GrammarSymbol -> J_index(new node)
                                if (!Nodeitem.Edges.ContainsKey(AFD.Count-1))
                                    Nodeitem.Edges.Add(AFD.Count - 1, GrammarSymbol);
                                break;
                        }
                    }
                }

            } while (SomethingIsAdded);
            return AFD;
        }

        /// <summary>
        /// Creates and returns the grammar's augmented production.
        /// </summary>
        private void AddAugmentedProduction()
        {
            string FirstProduction = Productions[0].Left.Content;

            //Construction the first production 
            Production ProductionToAdd = new Production();
            ProductionToAdd.Left = new Token(FirstProduction + "'", false); //example [FirstProduction = Num] -> Num'
            ProductionToAdd.Right.Add(new Token(FirstProduction, false));   //Adds the first production NT to the rigth
            ProductionToAdd.Id = 0;

            //adding all the stuff 
            Productions.Insert(0,ProductionToAdd);  //finally adds the Augmented Production at the start
        }

        /// <summary>
        /// Construction of the first: LR1 element and node.
        /// </summary>
        /// <returns></returns>
        private Node GenerateFirstNode()
        {   
            Production FirstProduction = Productions.First();
            Node I0 = new Node();

            //first LR1 element
            LR1Element FirstElement = new LR1Element(FirstProduction, new List<Token>() { new Token("$", true) });
            I0.Elements.Add(FirstElement);
            I0 = Cerradura(I0);
            return I0;
        }

        /// <summary>
        /// Handles the expansion of Lr1 elements.
        /// </summary>
        /// <param name="CurrentNode"></param>
        /// <returns></returns>
        private Node Cerradura(Node CurrentNode)
        {
            bool SomethingIsAdded;
            do  //repeat until there are no more new elements
            {
                SomethingIsAdded = false;
                foreach (LR1Element NodeItem in CurrentNode.Elements.ToList())
                {   
                    //General syntax [A -> α.Bβ, {a1, a2, ..an}]
                    if(NodeItem.Gamma.Count > 0) //only if there is something after the dot
                    {   
                        Token B = NodeItem.Gamma.First();
                        if (B.IsTerminal == false)
                        {
                            List<Token> βa = new List<Token>();
                            List<Token> β = new List<Token>(NodeItem.Gamma); //Add all the Gamma tokens  
                            β.RemoveAt(0);                                   //Except for the first token
                            foreach (Token a in NodeItem.Advance)            //Loops for all the Elements from Advance
                            {
                                List<Token> ResPrimAdv = new List<Token>(β);
                                ResPrimAdv.Add(a);
                                ResPrimAdv = Prims.GetPrimerosDe(ResPrimAdv);
                                βa.AddRange(ResPrimAdv);                    //Adding GetPrimerosDe() result
                            }
                            βa = βa.Distinct().ToList();                         //Deleting all the duplicates

                            List<Production> BProductions = Productions.FindAll(pred => pred.Left.Content == B.Content);
                            foreach (Production BProduction in BProductions.ToList())
                            {
                                //Finds all productions of B and convert to the Lr1elements of the form [B -> .γ, b]
                                LR1Element BLr1Token = new LR1Element(BProduction, βa); //construction B-Production
                                if (!CurrentNode.CheckIfElementExist(BLr1Token))
                                {
                                    //isnt a repeated Lr1 token so its added to the elements list
                                    CurrentNode.Elements.Add(BLr1Token);
                                    SomethingIsAdded = true;
                                }
                            }
                        }
                    }
                }

            } while (SomethingIsAdded);
            return CurrentNode;
        }

        /// <summary>
        /// Links a node with a grammatical symbol to generate new nodes.
        /// </summary>
        /// <param name="CurrentNode"></param>
        /// <param name="X"></param>
        /// <returns></returns>
        private Node Ir_A(Node CurrentNode, Token X)
        {
            Node J = new Node();
            // for each element [A -> α.Xβ, a] on CurrentNode
            foreach (LR1Element Lr1item in CurrentNode.Elements.ToList())
            {
                if (Lr1item.Gamma.Count > 0) //only if there is something after the dot
                {
                    if (Lr1item.Gamma.First().Content == X.Content) //finded 
                    {
                        LR1Element Lr1ToAdd = new LR1Element(Lr1item);
                        Lr1ToAdd.Alpha.Add(Lr1ToAdd.Gamma.First()); //adds X on alpha
                        Lr1ToAdd.Gamma.RemoveAt(0);                 //removes X from gamma
                        J.Elements.Add(Lr1ToAdd);
                    }
                }
            }
            return Cerradura(J);
        }

        /// <summary>
        /// Verify that the input node can be added to the AFD.
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        private ValNodeResult CheckNodeValidityToAdd(Node J)
        {
            ValNodeResult Result;
            Result.IndexFinded = -1;

            if (J.Elements.Count <= 0)
            {   //it's empty, doing nothing here
                Result.ValOut = ValidationOutput.NothingToDo;
                return Result;
            }   
            else
            {
                for (int i = 0; i < AFD.Count; i++)
                {
                    bool CheckResEquals = AFD[i].CheckNodeEquals(J);
                    if (CheckResEquals)
                    {   //already exist the same node on the AFD
                        Result.ValOut = ValidationOutput.AlreadyExisit;
                        Result.IndexFinded = i;
                        return Result;
                    }
                }
                //it's OK to add the input node
                Result.ValOut = ValidationOutput.NewRelation;
                return Result;           
            }
        }
    }
}
