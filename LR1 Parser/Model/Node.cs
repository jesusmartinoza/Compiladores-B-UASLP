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

        /// <summary>
        /// Verify if another node's elements has the same data 
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        public bool CheckNodeEquals(Node inputNode)
        {
            List<string> List1 = new List<string>();
            foreach (LR1Element item in Elements)
                List1.Add(item.ToString());

            List<string> List2 = new List<string>();
            foreach (LR1Element item in inputNode.Elements)
                List2.Add(item.ToString());

            var deletedItems = List1.Except(List2).Any();
            var newItems = List2.Except(List1).Any();
            return !newItems && !deletedItems;
        }

        /// <summary>
        /// Fast method that verifies if there is already an Lr1 token in the node's elements. 
        /// </summary>
        /// <param name="Lr1Token"></param>
        /// <returns></returns>
        public bool CheckIfElementExist( LR1Element Lr1Token)
        {
            return Elements.Any(pred => pred.ToString() == Lr1Token.ToString());
        }
    }
}
