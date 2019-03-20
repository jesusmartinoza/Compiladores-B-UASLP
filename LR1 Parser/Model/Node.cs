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
        List<LR1Element> elements;
        Dictionary<int, Token> edges; // Guarda destinos del nodo. Diccionario indexado por numero del siguiente estado y que contiene el token para llegar a él.
        //Getters and Setters
        internal List<LR1Element> Elements { get => elements; set => elements = value; }
        internal Dictionary<int, Token> Edges { get => edges; set => edges = value; }

        public Node()
        {
            elements = new List<LR1Element>();
            edges = new Dictionary<int, Token>();
        }
    }
}
