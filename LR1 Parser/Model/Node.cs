using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    /// <summary>  
    /// Clase para representar un nodo en el AFD

    /// Cada nodo contiene una lista de producciones y aristas para ir a otro nodo.
    /// </summary> 
    class Node
    {
        //Getters and Setters
        internal List<LR1Element> Elements { get; set; }
        internal Dictionary<int, Token> Edges { get; set; }

        public Node()
        {
            Elements = new List<LR1Element>();
            Edges = new Dictionary<int, Token>();
        }
    }
}
