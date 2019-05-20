
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

using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;


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
        Stack<string> operatorsStack;
        string globalType;
        int counter; // Contador de nodos visitados en DFSSearch

        // Stuff for graphviz
        List<EdgeStatement> graphVizEdges;
        IRenderer renderer;

        internal List<Node> AFD;
        internal List<State> States { get { return states; } set { states = value; } }
        internal List<ActionLog> Log { get { return log; } set { log = value; } }
        internal Stack<BinaryTreeNode> NodeStack { get { return nodesStack; } set { nodesStack = value; } }

        public Parser(List<Node> AFDList)
        {
            AFD = AFDList;
            //States = new List<State>();
            stackAnalysis = new List<TokenState>();
            log = new List<ActionLog>();
            nodesStack = new Stack<BinaryTreeNode>();
            operatorsStack = new Stack<string>();
            renderer = new Renderer(@"C:\Program Files\Graphviz2.38\bin");
            graphVizEdges = new List<EdgeStatement>();
            counter = 1;

            //InitTestAFD();
            //CreateSyntaxisAnalysisTable();
			deserializacionBinaria();
        }

		public Parser()
		{			
			stackAnalysis = new List<TokenState>();
			log = new List<ActionLog>();
			nodesStack = new Stack<BinaryTreeNode>();
			operatorsStack = new Stack<string>();
			renderer = new Renderer(@"C:\Program Files\Graphviz2.38\bin");
			graphVizEdges = new List<EdgeStatement>();
			deserializacionBinaria();
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
            nodesStack.Clear();
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

                    if (nextAction.state == 0) // Estado R0 o aceptar
                        break;

                    // Encuentra match
                    for (var i = 0; i < rLen; i++)
                    {
                        Token r = production.Right[i];
                        int indexStack = (stackAnalysis.Count) - rLen + i;
                        var itState = stackAnalysis[indexStack];

                        if (!itState.dirty && itState.token.Content == r.Content)
                        {
                            itState.dirty = true;
                            r.Val = itState.token.Val;
                        }
                    }

                    // Remplazar los TokenState sucios
                    for (var i = stackAnalysis.Count - 1; i >= 0; i--)
                    {
                        var itState = stackAnalysis[i];

                        if (itState.dirty)
                        {
                            stackAnalysis.RemoveAt(i);
                        }
                    }

                    SemanticAnalysis(production, nextAction.state);

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
            if(valid)
            {
                graphVizEdges.Clear();

                counter = 1;
                DFSSearch(nodesStack.Peek());
                CreateGraphFile();
            }

            return valid;
        }

        private async void CreateGraphFile()
        {
            var graph = Graph.Directed
                .Add(AttributeStatement.Graph.Set("labelloc", "t"))
                .Add(AttributeStatement.Graph.Set("bgcolor", "#F9ECD1"))
                .Add(AttributeStatement.Node.Set("style", "filled"))
                .Add(AttributeStatement.Node.Set("color", "#000000"))
                .Add(AttributeStatement.Node.Set("fillcolor", "#5BC5BF"))
                .Add(AttributeStatement.Graph.Set("label", "Árbol Semántico"))
                .AddRange(graphVizEdges);

            using (Stream file = File.Create("semantic_tree.png"))
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
                case 5:
                {
                    BinaryTreeNode a = new BinaryTreeNode("idV", new BinaryTreeNode(p.Right[2].Val), new BinaryTreeNode(p.Right[4].Val));
                    BinaryTreeNode b = new BinaryTreeNode("posV", new BinaryTreeNode(p.Right[6].Val), new BinaryTreeNode(p.Right[8].Val));
                    BinaryTreeNode c = new BinaryTreeNode("tamV", new BinaryTreeNode(p.Right[10].Val), new BinaryTreeNode(p.Right[12].Val));
                    BinaryTreeNode n = new BinaryTreeNode("vista", b, c);

                    b = new BinaryTreeNode("at", a, n);
                    c = nodesStack.Pop();

                    nodesStack.Push(new BinaryTreeNode("CV1", b, c));
                }
                break;

                // def-vent -> CreaVentana ( id , cadena ) { secuencia-ctrl }
                case 6:
                {
                    BinaryTreeNode a = new BinaryTreeNode("idV", new BinaryTreeNode(p.Right[2].Val), nodesStack.Peek());
                    BinaryTreeNode b = nodesStack.Pop();

                    nodesStack.Push(new BinaryTreeNode("CV2", a, b));
                }
                break;

                // secuencia-ctrl -> secuencia-ctrl def-ctrl
                case 7:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Count == 0 ? null : nodesStack.Pop();

                    nodesStack.Push(new BinaryTreeNode(";", a, b));
                }
                break;

                // def-ctrl -> CreaBoton ( id , cadena , num , num , num , num ) { def-evnt }
                case 9:
                {
                    BinaryTreeNode a = new BinaryTreeNode("idB", new BinaryTreeNode(p.Right[2].Val), new BinaryTreeNode(p.Right[4].Val));
                    BinaryTreeNode b = new BinaryTreeNode("posB", new BinaryTreeNode(p.Right[6].Val), new BinaryTreeNode(p.Right[8].Val));
                    BinaryTreeNode c = new BinaryTreeNode("tamB", new BinaryTreeNode(p.Right[10].Val), new BinaryTreeNode(p.Right[12].Val));
                    BinaryTreeNode n = new BinaryTreeNode("vista", b, c);

                    b = new BinaryTreeNode("at", a, n);

                    c = nodesStack.Pop();

                    nodesStack.Push(new BinaryTreeNode("CB", b, c));
                }
                break;

                // def-ctrl -> CreaTextbox ( id , num , num , num , num ) ;
                case 10:
                {
                    BinaryTreeNode a = new BinaryTreeNode(p.Right[2].Content);
                    BinaryTreeNode b = new BinaryTreeNode("posT", new BinaryTreeNode(p.Right[4].Val), new BinaryTreeNode(p.Right[6].Val));
                    BinaryTreeNode c = new BinaryTreeNode("tamT", new BinaryTreeNode(p.Right[8].Val), new BinaryTreeNode(p.Right[10].Val));
                    BinaryTreeNode n = new BinaryTreeNode("vista", b, c);

                    nodesStack.Push(new BinaryTreeNode("CT", a, n));
                }
                break;

                // def-ctrl -> CreaLabel ( id , cadena , num , num ) ;
                case 11:
                {
                    BinaryTreeNode a = new BinaryTreeNode("idL", new BinaryTreeNode(p.Right[2].Val), new BinaryTreeNode(p.Right[4].Val));
                    BinaryTreeNode b = new BinaryTreeNode("posL", new BinaryTreeNode(p.Right[6].Val), new BinaryTreeNode(p.Right[8].Val));

                    nodesStack.Push(new BinaryTreeNode("CL", a, b));
                }
                break;

                // secuencia-sent -> sentencia secuencia-sent
                case 14:
                {
                    var b = nodesStack.Pop();
                    var a = nodesStack.Count == 0 ? null : nodesStack.Pop();

                    nodesStack.Push(new BinaryTreeNode(";", a, b));
                }
                break;

                // sent-if -> if (exp) { secuencia - sent }
                case 26:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();
                        
                    nodesStack.Push(new BinaryTreeNode("sent-if", a, b));
                }
                break;

                // sent-if -> if (exp) { secuencia - sent } else { secuencia - sent }
                case 27:
                {
                    BinaryTreeNode c = nodesStack.Pop();
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();

                    BinaryTreeNode n = new BinaryTreeNode("else", b, c);
                    nodesStack.Push(new BinaryTreeNode("sent-if-else", a, n));
                }
                break;
               
                // sent-repeat->repeat { secuencia - sent } until(exp)
                case 28:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();

                    nodesStack.Push(new BinaryTreeNode("repeat", a, b));
                }
                break;

                // sent-assign->id := exp;
                case 29:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = new BinaryTreeNode(p.Right[0].Val);
                    nodesStack.Push(new BinaryTreeNode(":=", a, b));
                }
                break;

                // sent-assign -> id [ indice ] := exp ;
                case 30:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = new BinaryTreeNode("[ ]", new BinaryTreeNode(p.Right[0].Val), new BinaryTreeNode(p.Right[2].Val));
                    nodesStack.Push(new BinaryTreeNode(":=", a, b));
                }
                break;

                // sent-while -> while ( exp ) { secuencia-sent }
                case 31:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();
                    nodesStack.Push(new BinaryTreeNode("while", a, b));
                }

                break;

                // sent-do-while -> do { secuencia-sent } while ( exp ) ;
                case 32:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();
                    nodesStack.Push(new BinaryTreeNode("do", a, b));
                }
                break;

                // sent-switch -> switch ( id ) { secuencia-case }
                case 33:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = new BinaryTreeNode(p.Right[2].Val);

                    nodesStack.Push(new BinaryTreeNode("switch", a, b));
                }
                break;

                // secuencia-case -> secuencia-case sentencia-case
                case 34:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();

                    nodesStack.Push(new BinaryTreeNode("case-sep", a, b));
                }
                break;

                // sentencia-case -> case id { secuencia-sent } break ;
                case 36:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = new BinaryTreeNode(p.Right[1].Val);
                    
                    nodesStack.Push(new BinaryTreeNode("case", a, b));
                }
                break;

                // sent-for -> for ( id := num : num , num ) { secuencia-sent }
                case 37:
                {
                    BinaryTreeNode c = nodesStack.Pop();
                    BinaryTreeNode b = new BinaryTreeNode("incremento", new BinaryTreeNode(p.Right[6].Val), new BinaryTreeNode(p.Right[8].Val));
                    BinaryTreeNode n = new BinaryTreeNode(";", b, c);
                    BinaryTreeNode a = new BinaryTreeNode(":=", new BinaryTreeNode(p.Right[2].Val), new BinaryTreeNode(p.Right[4].Val));

                    nodesStack.Push(new BinaryTreeNode("for", a, n));
                }
                break;

                // sent-func -> MessageBox ( cadena )
                case 38:
                {
                    BinaryTreeNode a = new BinaryTreeNode(p.Right[2].Val);
                    nodesStack.Push(new BinaryTreeNode("MS", a, null));
                }
                break;
		//sent-declara -> tipo identificadores    
		case 39:
                {
			a=BinaryTreeNode (tipo.vallex,null,null)
			b=BinaryTreeNode(identificadores.vallex,null,null)
			nodesStack.push(“dec”, a, b);
                }
                 break;
		//sent-declara -> tipo [ indice ] identificadores
		case 40:
                {
			a = BinaryTreeNode(identificador, null, null),b=BinaryTreeNode("tam",BinaryTreeNode(tipo.vallex, null, null),BinaryTreeNode(indice.vallex, null, null)) ;
			nodesStack.push(BinaryTreeNode ("Arr", a, b)); 
                }
                 break;
		//identificadores -> identificadores , id 	    
		case 41:
                {
			BinaryTreeNode b = nodesStack.Pop();
			BinaryTreeNode a = nodesStack.Pop();

			nodesStack.Push(new BinaryTreeNode("Identificadores", a, b));
                }
                 break;

                // exp -> exp-simple opcomparacion exp-simple
                // exp-simple -> exp-simple opsuma term
                case 45:
                case 47:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();
                    //nodesStack.Push(new BinaryTreeNode(operatorsStack.Pop(), a, b));
                    nodesStack.Push(new BinaryTreeNode(p.Right[1].Val, a, b));
                }
                break;

                // tipos
                case 49:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                {
                    globalType = p.Right[0].Content;
                }
                break;

                // term->term opmult factor
                case 56:
                {
                    BinaryTreeNode b = nodesStack.Pop();
                    BinaryTreeNode a = nodesStack.Pop();

                    nodesStack.Push(new BinaryTreeNode(p.Right[1].Val, a, b));
                }
                break;

                // term -> factor
                case 57:
                {
                    //nodesStack.Push(new BinaryTreeNode(p.Right[0].Val));
                }
                break;

                //  factor-> num
                //  factor-> id
                //  factor->cadena
                case 59:
                case 60:
                case 61:
                {
                    nodesStack.Push(new BinaryTreeNode(p.Right[0].Val));
                }
                break;
            }
        }

        /// <summary>
        /// Iterate over graph to create graphviz visualization.
        /// </summary>
        private void DFSSearch(BinaryTreeNode parent)
        {
            var parentId = parent.Content + " " + counter;

            if (parent.Id == 0)
                parent.Id = counter;
            
            parent.Visited = true;

            counter++;
            if (parent.Left != null && !parent.Left.Visited)
            {
                var leftId = parent.Left.Content + " " + counter;

                parent.Left.Id = counter;
                graphVizEdges.Add(EdgeStatement.For(parentId, leftId));
                DFSSearch(parent.Left);
            }
            if (parent.Right != null && !parent.Right.Visited)
            {
                var rightId = parent.Right.Content + " " + counter;

                parent.Right.Id = counter;
                graphVizEdges.Add(EdgeStatement.For(parentId, rightId));
                DFSSearch(parent.Right);
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
			serializacionBinaria();
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

		//**************************SERIALIZACION - DESERIALIZACION*********************************************
		private void serializacionBinaria()
		{
			//Seleccion de formateador
			BinaryFormatter formateador = new BinaryFormatter();
			//XmlSerializer formateadorXml = new XmlSerializer(typeof(List<State>));

			//Se crea el Stream
			Stream miStream = new FileStream(Environment.CurrentDirectory + "\\estadosSerializados11", FileMode.Create, FileAccess.Write, FileShare.None);
			
			
			//Serializacion
			formateador.Serialize(miStream, States);
			
			
			//Cerrar Stream
			miStream.Close();
		}

		private void deserializacionBinaria()
		{
			States = new List<State>();

			//Seleccion de formateador
			BinaryFormatter formateador = new BinaryFormatter();
			

			//Se crea el Stream
			Stream miStream = new FileStream(Environment.CurrentDirectory + "\\estadosSerializados11", FileMode.Open, FileAccess.Read, FileShare.None);


			//Deserializacion
			States = (List<State>)formateador.Deserialize(miStream);


			//Cerrar Stream
			miStream.Close();
		}
		//**************************SERIALIZACION - DESERIALIZACION*********************************************
	}
}
