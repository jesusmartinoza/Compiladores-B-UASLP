
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shields.GraphViz.Components;
using Shields.GraphViz.Models;
using Shields.GraphViz.Services;

namespace LR1_Parser.Model
{
    /// <summary>
    /// Modulo que recibe un AFD, genera la tabla de analisis sintactico y evalúa
    /// una cadena usando una tabla de acciones de analisis sintactico.
    ///
    /// Módulo desarrollado por Pumas :D
    /// </summary>
    class Parser
    {
        List<State> states;
        List<TokenState> stackAnalysis; // Pila de analisis sintático
        string input; // Cadena a evaluar
        List<ActionLog> log;
        Stack<BinaryTreeNode> nodesStack;

        // Stuff for graphviz
        List<EdgeStatement> graphVizEdges;
        IRenderer renderer;

        internal List<Node> AFD;
        internal List<State> States { get { return states; } set { states = value; } }
        internal List<ActionLog> Log { get { return log; } set { log = value; } }

        public Parser(List<Node> AFDList)
        {
            AFD = AFDList;
            States = new List<State>();
            stackAnalysis = new List<TokenState>();
            log = new List<ActionLog>();
            nodesStack = new Stack<BinaryTreeNode>();
            renderer = new Renderer(@"C:\Program Files (x86)\Graphviz2.38\bin");
            graphVizEdges = new List<EdgeStatement>();

            //InitTestAFD();
            CreateSyntaxisAnalysisTable();
        }

        public bool EvalString(String inputString)
        {
            bool valid = true;
            List<Token> inputTokens = new List<Token>();
            input = inputString;
            // Se limpia al log
            Log.Clear();

            inputTokens = Tokenizer.Convert(input);

            inputTokens.Add(new Token("$", true));
            stackAnalysis.Clear();
            stackAnalysis.Add(new TokenState() { token = new Token("$", true), state = 0 });
            
            while(valid) 
            {
                TokenState cAction = stackAnalysis.Last(); // Current Action
                Token cToken = inputTokens.First(); // Current Token
                Action nextAction;

                // Imprimir pila de A.S.

                // Limpiar estados de la pila de A.S.
                for (var i = 0; i < stackAnalysis.Count; i++)
                    stackAnalysis[i].dirty = false;

                // Verificar si el token existe en las listas
                valid = states[cAction.state].Terminals.ContainsKey(cToken.Content) || states[cAction.state].NonTerminals.ContainsKey(cToken.Content);

                if (!valid) break;

                if (cToken.IsTerminal)
                    nextAction = states[cAction.state].Terminals[cToken.Content];
                else
                    nextAction = states[cAction.state].NonTerminals[cToken.Content];

                if (nextAction.IsEmpty())
				{
					valid = false;
					break;
				} else
                {
                    // Agregar acciones al log
                    log.Add(new ActionLog()
                    {
                        Stack = Helpers.ListToString(stackAnalysis),
                        Input = Helpers.ListToString(inputTokens),
                        Action = nextAction.action == 'S' ? nextAction.ToString() : nextAction.ToString() + " (" + MainWindow.productions[nextAction.state].ToString() + " )"
                    });
                }

                // Continua Análisis Sintáctico
                if (nextAction.action == 'S')
                {
                    stackAnalysis.Add(new TokenState() { token = cToken, state = nextAction.state });
                    inputTokens.RemoveAt(0);
                } else if(nextAction.action == 'R')
                {
                    Production production = MainWindow.productions[nextAction.state];
                    var rLen = production.Right.Count;

                    SemanticAnalysis(production, nextAction.state);

                    if (nextAction.state == 0) // Estado R0 o aceptar
                        break;

                    // Encuentra match
                    for (var i = 0; i < rLen; i++)
                    {
                        Token r = production.Right[i];
                        int indexStack = (stackAnalysis.Count) - rLen + i;
                        var itState = stackAnalysis[indexStack];

                        if (!itState.dirty && itState.token.Content == r.Content)
                            itState.dirty = true;
                    }

                    // Remplazar los TokenState sucios
                    for (var i = stackAnalysis.Count - 1; i >= 0; i--)
                    {
                        var itState = stackAnalysis[i];

                        if (itState.dirty)
                            stackAnalysis.RemoveAt(i);
                    }

                    int newState;
                    TokenState lastState = stackAnalysis.Last();

                    if (production.Left.IsTerminal)
                        newState = states[lastState.state].Terminals[production.Left.Content].state;
                    else
                        newState = states[lastState.state].NonTerminals[production.Left.Content].state;

                    stackAnalysis.Add(new TokenState() { token = production.Left, state = newState});
                }
            }

            // TODO: Descomentar cuando esten todos los esquemas de traduccion
            /*if(valid)
            {
                DFSSearch(nodesStack.Peek(), 1);
                CreateGraphFile();
            }*/

            return valid;
        }

        private async void CreateGraphFile()
        {
            var graph = Graph.Directed
                //.Add(AttributeStatement.Graph.Set("rankdir", "LR"))
                .Add(AttributeStatement.Graph.Set("labelloc", "t"))
                //.Add(AttributeStatement.Graph.Set("bgcolor", "#34495e"))
                .Add(AttributeStatement.Node.Set("style", "filled"))
                .Add(AttributeStatement.Node.Set("fillcolor", "#ECF0F1"))
                .Add(AttributeStatement.Graph.Set("label", "Arbol semántico"))
                .AddRange(graphVizEdges);

            using (Stream file = File.Create("Semantic Tree.png"))
            {
                await renderer.RunAsync(
                    graph, file,
                    RendererLayouts.Dot,
                    RendererFormats.Png,
                    CancellationToken.None);
            }
        }

        /// <summary>
        /// Ejecutar esquema de traduccion.
        /// 
        /// Indirectamente va generando el árbol de analisis sintáctico.
        /// 
        /// TODO: 
        /// 
        /// ⚠️ ⚠️ ⚠️ ⚠️ ⚠️ ⚠️ ⚠️ ⚠️ ¡ P E L I G R O   ! ⚠️ ⚠️ ⚠️ ⚠️ ⚠️ ⚠️ ⚠️ ⚠️ ⚠️ ⚠️ ⚠️ 
        /// 
        /// SI CAMBIA LA GRAMATICA, CAMBIAN LOS INDICES Y POR LO TANTO HAY QUE REACOMODAR ESTE SWITCH
        /// </summary>
        /// <param name="p">Produccion</param>
        private void SemanticAnalysis(Production p, int productionIndex)
        {
            switch (productionIndex)
            {
                // def-vent -> CreaVentana ( id , cadena , num , num1 , num2 , num3 ) { secuencia-ctrl }
                case 4:
                {
                        BinaryTreeNode a = new BinaryTreeNode("idV", new BinaryTreeNode(p.Right[2].Content), new BinaryTreeNode(p.Right[4].Content));
                        BinaryTreeNode b = new BinaryTreeNode("posV", new BinaryTreeNode(p.Right[6].Content), new BinaryTreeNode(p.Right[8].Content));
                        BinaryTreeNode c = new BinaryTreeNode("tamV", new BinaryTreeNode(p.Right[10].Content), new BinaryTreeNode(p.Right[12].Content));
                        BinaryTreeNode n = new BinaryTreeNode("vista", b, c);

                        b = new BinaryTreeNode("at", a, n);
                        c = nodesStack.Pop();

                        nodesStack.Push(new BinaryTreeNode("CV1", b, c));
                }
                break;

                // def-vent -> CreaVentana ( id , cadena ) { secuencia-ctrl }
                case 5:
                {
                    BinaryTreeNode a = new BinaryTreeNode("idV", new BinaryTreeNode(p.Right[2].Content), nodesStack.Peek());
                    BinaryTreeNode b = nodesStack.Pop();

                    nodesStack.Push(new BinaryTreeNode("CV2", a, b));
                }
                break;

                // secuencia-ctrl -> secuencia-ctrl def-ctrl
                case 6:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();

                    nodesStack.Push(new BinaryTreeNode(";", a, b));
                }
                break;

                // sent-if -> if (exp) { secuencia - sent }
                case 25:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();
                        
                    nodesStack.Push(new BinaryTreeNode("sent-if", a, b));
                }
                break;

                // sent-if -> if (exp) { secuencia - sent } else { secuencia - sent }
                case 26:
                {
                    BinaryTreeNode c = nodesStack.Pop();
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();

                    BinaryTreeNode n = new BinaryTreeNode("else", b, c);
                    nodesStack.Push(new BinaryTreeNode("sent-if", a, n));
                }
                break;
               
                // sent-repeat->repeat { secuencia - sent } until(exp)
                case 27:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();

                    nodesStack.Push(new BinaryTreeNode("repeat", a, b));
                }
                break;

                // sent-assign->id := exp;
                case 28:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = new BinaryTreeNode(p.Right[0].Content);
                    nodesStack.Push(new BinaryTreeNode(":=", a, b));
                }
                break;

                // sent-assign -> id [ indice ] := exp ;
                case 29:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = new BinaryTreeNode("[ ]", new BinaryTreeNode(p.Right[0].Content), new BinaryTreeNode(p.Right[2].Content));
                    nodesStack.Push(new BinaryTreeNode(":=", a, b));
                }
                break;

                // sent-while -> while ( exp ) { secuencia-sent }
                case 30:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();
                    nodesStack.Push(new BinaryTreeNode("while", a, b));
                }

                break;

                // sent-while -> while ( exp ) { secuencia-sent }
                case 31:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();
                    nodesStack.Push(new BinaryTreeNode("do", a, b));
                }
                break;

                case 44:
                {
                    BinaryTreeNode a = nodesStack.Pop();
                    BinaryTreeNode b = nodesStack.Pop();
                    nodesStack.Push(new BinaryTreeNode("opmult", a, b));
                }
                break;
            }
        }

        /// <summary>
        /// Iterate over graph to create graphviz visualization.
        /// </summary>
        private void DFSSearch(BinaryTreeNode parent, int counter)
        {
            List<EdgeStatement> edges = new List<EdgeStatement>();
            var parentId = parent.Content + " " + counter;

            if (parent.Id == 0)
                parent.Id = counter;

            parent.Visited = true;
            counter++;

            if (parent.Left != null && !parent.Left.Visited)
            {
                var leftId = parent.Left.Content + " " + counter;
                edges.Add(EdgeStatement.For(parentId, leftId));
                DFSSearch(parent.Left, counter);
            }
            else if (parent.Right != null && !parent.Right.Visited)
            {
                var rightId = parent.Right.Content + " " + counter;
                edges.Add(EdgeStatement.For(parentId, rightId));
                DFSSearch(parent.Right, counter);
            }
        }

        /// <summary>
        /// 
        /// Iterar sobre los nodos del AFD para
        /// calcular tabla de analisis sintactico.
        /// 
        /// </summary>
        public void CreateSyntaxisAnalysisTable()
        {
            States = new List<State>();

            foreach (var node in AFD)
            {
                State state = new State();

                // Desplazamientos
                foreach (var edge in node.Edges)
                {
                    // if (Token) is terminal
                    if (edge.Value.IsTerminal)
                        state.Terminals.Add(edge.Value.Content, new Action() { action = 'S', state = edge.Key });
                    else
                        state.NonTerminals.Add(edge.Value.Content, new Action() { action = 'S', state = edge.Key });
                }

                // Reducciones
                foreach (var elem in node.Elements)
                {
                    if (elem.Gamma.Count == 0)
                    {
                        if (elem.Alpha.Count > 0)   //verifica si no es un elemento LR1 del tipo A -> ., {a1, a2, ..an} 
                        {
                            // Obtener produccion de la lista global usando el token left y tokens como elemento de busqueda
                            Production production = MainWindow.productions
                                .Where(p => Production.Comparator(p, elem.Left, elem.Alpha))//p.Left.Content == elem.Left.Content)
                                .First();

                            foreach (var advanceToken in elem.Advance)
                            {
                                if(!state.Terminals.ContainsKey(advanceToken.Content))
                                {
                                    state.Terminals.Add(
                                        advanceToken.Content,
                                        new Action() { action = 'R', state = MainWindow.productions.IndexOf(production) }
                                    );
                                }
                            }
                        }
                    }
                }

                // Agregar todos los tokens al estado
                // Para tener consistencia en el diccionario
                foreach (Token t in AFDGenerator.GrammarSymbols)
                {
                    if (t.IsTerminal && !state.Terminals.ContainsKey(t.Content))
                        state.Terminals.Add(t.Content, new Action());

                    if (!t.IsTerminal && !state.NonTerminals.ContainsKey(t.Content))
                        state.NonTerminals.Add(t.Content, new Action());
                }

                if(!state.Terminals.ContainsKey("$"))
                    state.Terminals.Add("$", new Action());

                States.Add(state);
            }
        }

        /// Ejemeplo con la información absolutamente necesaria para
        /// representar el siguiente AFD
        private void InitTestAFD()
        {
            AFD = new List<Node>();

            // Tokens
            Token tE = new Token("E", false);
            Token tN = new Token("n", true);
            Token tP = new Token("+", true);
            Token tM = new Token("-", true);

            // Lista auxiliar para guadar tokens de anticipacion {$, +, -}
            // Este conjunto de tokens se repiten varias veces en este test.
            List<Token> commonAdvanceTokens = new List<Token>();
            commonAdvanceTokens.Add(new Token("$", true));
            commonAdvanceTokens.Add(tP);
            commonAdvanceTokens.Add(tM);

            // _________________________________________________________________
            // Info for Node 0
            // _________________________________________________________________
            Node n0 = new Node();
            n0.Edges.Add(1, tE);
            n0.Edges.Add(6, tN);

            // _________________________________________________________________
            // Info for Node 1
            // _________________________________________________________________
            Node n1 = new Node();
            LR1Element n1elem0 = new LR1Element(); // Node 1 Element 0
            n1elem0.Left = new Token("E'", false);
            n1elem0.Alpha.Add(tE);
            n1elem0.Advance.Add(new Token("$", true));
            n1.Elements.Add(n1elem0);
            n1.Edges.Add(2, tP);
            n1.Edges.Add(4, tM);

            // _________________________________________________________________
            // Info for Node 2
            // _________________________________________________________________
            Node n2 = new Node();
            n2.Edges.Add(3, tN);

            // _________________________________________________________________
            // Info for Node 3
            // _________________________________________________________________
            Node n3 = new Node();
            LR1Element n3elem0 = new LR1Element(); // Node 3 Element 0
            n3elem0.Left = tE;
            n3elem0.Alpha.Add(tE);
            n3elem0.Alpha.Add(tP);
            n3elem0.Alpha.Add(tN);
            n3elem0.Advance.AddRange(commonAdvanceTokens);
            n3.Elements.Add(n3elem0);

            // _________________________________________________________________
            // Info for Node 4
            // _________________________________________________________________
            Node n4 = new Node();
            n4.Edges.Add(5, tN);

            // _________________________________________________________________
            // Info for Node 5
            // _________________________________________________________________
            Node n5 = new Node();
            LR1Element n5elem0 = new LR1Element(); // Node 5 Element 0
            n5elem0.Left = tE;
            n5elem0.Alpha.Add(tE);
            n5elem0.Alpha.Add(tM);
            n5elem0.Alpha.Add(tN);
            n5elem0.Advance.AddRange(commonAdvanceTokens);
            n5.Elements.Add(n5elem0);

            // _________________________________________________________________
            // Info for Node 6
            // _________________________________________________________________
            Node n6 = new Node();
            LR1Element n6elem0 = new LR1Element(); // Node 6 Element 0
            n6elem0.Left = tE;
            n6elem0.Alpha.Add(tN);
            n6elem0.Advance.AddRange(commonAdvanceTokens);
            n6.Elements.Add(n6elem0);

            AFD.Add(n0);
            AFD.Add(n1);
            AFD.Add(n2);
            AFD.Add(n3);
            AFD.Add(n4);
            AFD.Add(n5);
            AFD.Add(n6);
        }
    }
}
