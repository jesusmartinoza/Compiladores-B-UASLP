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
        private Primeros Prims;
        private Tokenizer Productions;
        private List<Node> AFD;


        /// <summary>
        /// AFD Generator constructor.
        /// </summary>
        /// <param name="productions"></param>
        /// <param name="primeros"></param>
        public AFDGenerator(Tokenizer productions, Primeros prims)
        {
            Productions = productions;
            Prims = prims;
            AFD = new List<Node>();
        }

        /// <summary>
        /// Main method that returs a fresh and pretty AFD.
        /// </summary>
        /// <returns></returns>
        public List<Node> GenerateAFD()
        {
            AddAugmentedProduction(); //Augmented production 
            Node I0 = GenerateFirstNode(); //TODO solo soporta gramaticas en orden de importancia descendente
            AFD.Add(I0);

            //TODO

            return AFD;
        }

        /// <summary>
        /// Method that adds augmented production to the grammar
        /// </summary>
        private void AddAugmentedProduction()
        {
            string FirstProduction = Productions.producciones[0].left.Content;

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
            Production FirstProduction = Productions.producciones[0];
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
            do
            {
                SomethingIsAdded = false;
                foreach (var NodeItem in CurrentNode.Elements)
                {   
                    // General syntax [A -> α.Bβ, a]
                    Token B = NodeItem.Gamma[0];
                    if (B.IsTerminal == false)
                    {
                        List<Token> βa = new List<Token>();
                        βa.AddRange(NodeItem.Gamma);    //Add all the Gamma tokens
                        βa.RemoveAt(0);                 //Except for the first token
                        βa.AddRange(NodeItem.Advance);  //Add all the Advance tokens
                        //b is each terminal of Primero(βa).
                        List<Token> b = Primeros.GetPrimerosDe(βa); 

                        //TODO checar si junta todo en un sola produccion 
                        List<Production> BProductions = Productions.producciones.FindAll(pred => pred.Left.Content == B.Content); 
                        foreach (var BProduction in BProductions)
                        {   //Finds all productions of B and convert to the Lr1elements of the form [B -> .γ, b]
                            
                            LR1Element BLr1Token = new LR1Element(BProduction,b); //construction B-Production
                            if(!CheckIfElementExist(CurrentNode.Elements,BLr1Token))
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
        /// Fast method that verifies if there is already an Lr1 token in the node's elements 
        /// </summary>
        /// <param name="Elements"></param>
        /// <param name="Lr1Token"></param>
        /// <returns></returns>
        private bool CheckIfElementExist(List<LR1Element> Elements, LR1Element Lr1Token)
        {
            return Elements.Any(pred => pred.ToString() == Lr1Token.ToString());
        }


        /*                      Compi A Algoritmo De Base                       */

        /*

        public void UpdateLr1Automata()
        {
            //GramSymbols = GeneraSimbGram();
            //GeneraAumentada();
            //foreach (string str in GramSymbols)
                //TablaDeEstados.Add(str, new Dictionary<int, int>());
            C = GeneraAutomata();

            for (int i = 0; i < C.Count; i++)
                foreach (string str in GramSymbols)
                {   //Det. si existe algo en T.Edos[GramSymbols][origen]
                    bool existe = TablaDeEstados[str].Any(query => query.Key == i);
                    if (!existe)
                        TablaDeEstados[str].Add(i, -1);
                }
        }

        private List<List<LR1Element>> GeneraAutomata()
        {
            // C = new List<List<LR1Element>>();
            //List<LR1Element> primElemento = new List<LR1Element>
            //        { new LR1Element(Aumentada[0][0], "."+ Aumentada[0][1], '$') };
            //primElemento = Cerradura(primElemento);
            //C.Add(primElemento);

            for (int i = 0; i < C.Count; i++)
            {
                for (int j = 0; j < GramSymbols.Count; j++)
                {
                    List<LR1Element> NextElemento = Ir_A(C[i], GramSymbols[j]);

                    int different = C.FindIndex(query => CheckDifferentState(query, NextElemento) == false);

                    if (NextElemento.Count > 0)
                    {
                        if (different == -1)
                        {
                            C.Add(NextElemento);
                            //Se hace relacion
                            TablaDeEstados[GramSymbols[j]].Add(i, C.Count - 1);
                        }
                        else
                        {
                            //Se hace relacion
                            TablaDeEstados[GramSymbols[j]].Add(i, different);
                        }
                    }
                }
            }
            return C;
        }

        private List<LR1Element> Ir_A(List<LR1Element> I, string X)
        {
            List<LR1Element> J = new List<LR1Element>();
            foreach (LR1Element elemLr1 in I)
            {
                int dotLocation = elemLr1.GetDotLocation();
                if (dotLocation != -1)
                {
                    string B = elemLr1.value[dotLocation + 1].ToString();
                    if (B == X)
                    {
                        string modValue = elemLr1.value;
                        modValue = modValue.Remove(dotLocation, 1);
                        modValue = modValue.Insert(dotLocation + 1, ".");
                        J.Add(new LR1Element(elemLr1.key, modValue, elemLr1.token));
                    }
                }
            }
            return Cerradura(J);
        }

        private List<LR1Element> Cerradura(List<LR1Element> elementosCerradura)
        {
            for (int i = 0; i < elementosCerradura.Count; i++)
            {
                int dotLocation = elementosCerradura[i].GetDotLocation();
                if (dotLocation != -1)
                {
                    string B = elementosCerradura[i].value[dotLocation + 1].ToString();
                    string β = "";
                    for (int j = dotLocation + 2; j < elementosCerradura[i].value.Length; j++)
                        β += elementosCerradura[i].value[j];
                    char a = elementosCerradura[i].token;
                    List<string> Prims = Primero(β + a, ""); //?
                    List<List<string>> BProds = Aumentada.FindAll(query => query[0] == B);
                    foreach (List<string> Prod in BProds)
                    {
                        foreach (string PrimToken in Prims)
                        {
                            LR1Element elemtonuevo;
                            if (Prod[1] != "Ɛ")
                                elemtonuevo = new LR1Element(Prod[0], "." + Prod[1], Char.Parse(PrimToken));
                            else
                                elemtonuevo = new LR1Element(Prod[0], ".", Char.Parse(PrimToken));

                            bool finded = elementosCerradura.Any(query => query.ToString() == elemtonuevo.ToString());
                            if (!finded)
                                elementosCerradura.Add(elemtonuevo);
                        }
                    }
                }
            }
            return elementosCerradura;
        }

        private bool CheckDifferentState(List<LR1Element> State1, List<LR1Element> State2)
        {
            List<string> s1 = new List<string>();
            List<string> s2 = new List<string>();
            foreach (LR1Element it1 in State1)
                s1.Add(it1.ToString());
            foreach (LR1Element it2 in State2)
                s2.Add(it2.ToString());
            bool isDifferent = !(s1.All(s2.Contains) && (s1.Count == s2.Count));
            return isDifferent;
        }

        */
    }
}

