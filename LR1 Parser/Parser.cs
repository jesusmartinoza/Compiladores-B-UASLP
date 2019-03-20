
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
        List<Node> AFD;
        List<State> states;

        void CreateSyntaxisAnalysisTable()
        {
            states = new List<State>();

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
                        state.Terminals.Add(elem.Left.Content, new Action() { action = 'R', state = 0}); // TODO: Poner el estado correcto
                }

                states.Add(state);
            }
        }

        /// Ejemeplo con la información absolutamente necesaria para
        /// representar el siguiente AFD
        void InitTest()
        {
            AFD = new List<Node>();

            // Lista auxiliar para guadar tokens de anticipacion {$, +, -}
            // Este conjunto de tokens se repiten varias veces en este test.
            List<Token> commonAdvanceTokens = new List<Token>();
            commonAdvanceTokens.Add(new Token("$", true));
            commonAdvanceTokens.Add(new Token("+", true));
            commonAdvanceTokens.Add(new Token("-", true));

            // _________________________________________________________________
            // Info for Node 0
            // _________________________________________________________________
            Node n0 = new Node();
            n0.Edges.Add(1, new Token("E", false));
            n0.Edges.Add(6, new Token("n", true));

            // _________________________________________________________________
            // Info for Node 1
            // _________________________________________________________________
            Node n1 = new Node();
            LR1Element n1elem0 = new LR1Element(); // Node 1 Element 0
            n1elem0.Left = new Token("E'", false);
            n1elem0.Advance.Add(new Token("$", true));
            n1.Elements.Add(n1elem0);
            n1.Edges.Add(2, new Token("+", true));
            n1.Edges.Add(4, new Token("-", true));

            // _________________________________________________________________
            // Info for Node 2
            // _________________________________________________________________
            Node n2 = new Node();
            n2.Edges.Add(3, new Token("n", true));

            // _________________________________________________________________
            // Info for Node 3
            // _________________________________________________________________
            Node n3 = new Node();
            LR1Element n3elem0 = new LR1Element(); // Node 3 Element 0
            n3elem0.Left = new Token("E", false);
            n3elem0.Advance.AddRange(commonAdvanceTokens);
            n3.Elements.Add(n3elem0);

            // _________________________________________________________________
            // Info for Node 4
            // _________________________________________________________________
            Node n4 = new Node();
            n4.Edges.Add(5, new Token("n", true));

            // _________________________________________________________________
            // Info for Node 5
            // _________________________________________________________________
            Node n5 = new Node();
            LR1Element n5elem0 = new LR1Element(); // Node 5 Element 0
            n5elem0.Left = new Token("E", false);
            n5elem0.Advance.AddRange(commonAdvanceTokens);
            n5.Elements.Add(n5elem0);

            // _________________________________________________________________
            // Info for Node 6
            // _________________________________________________________________
            Node n6 = new Node();
            n6.Elements.Add(n5elem0);
        }
    }
}
