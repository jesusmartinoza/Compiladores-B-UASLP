
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal List<Node> AFD;
        internal List<State> States { get { return states; } set { states = value; } }
        internal List<ActionLog> Log { get { return log; } set { log = value; } }

        public Parser(List<Node> AFDList)
        {
            AFD = AFDList;
            States = new List<State>();
            stackAnalysis = new List<TokenState>();
            log = new List<ActionLog>();

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


            // TODO: Tokenizar cadena 
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

                // Agregar acciones al log
                log.Add(new ActionLog()
                {
                    Stack = Helpers.ListToString(stackAnalysis),
                    Input = Helpers.ListToString(inputTokens),
                    Action = nextAction.ToString()
                });

                Console.WriteLine(
                    Helpers.ListToString(stackAnalysis) + " " +
                    Helpers.ListToString(inputTokens) + " " +
                    nextAction.ToString()
                );

                if (nextAction.IsEmpty())
				{
					valid = false;
					break;
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

            return valid;
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
