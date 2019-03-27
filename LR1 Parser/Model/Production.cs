using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    public class Production
    {
        /*string id;
        List<Token> left; // Conjunto de tokens del lado izquierdo de la flecha 
        List<Token> right;  // Conjunto de tokens del lado derecho de la flecha 
        HashSet<Token> first; // Conjunto de primeros de forma desordenada
        //Getters and Setters
        public string Id { get => id; set => id = value; }
        internal List<Token> Left { get => left; set => left = value; }
        internal List<Token> Right { get => right; set => right = value; }
        internal HashSet<Token> First { get => first; set => first = value; }*/
        int id;
        Token left; // Conjunto de tokens del lado izquierdo de la flecha 
        List<Token> right;  // Conjunto de tokens del lado derecho de la flecha 
        HashSet<Token> first; // Conjunto de primeros de forma desordenada

        public Production(int i, Token l){
            id = i;
            left = l;
            right = new List<Token>();
        }

        public Production()
        {
            right = new List<Token>();
            first = new HashSet<Token>();
        }

        /// <summary>
        /// Comparar una produccion con informacion de una segunda produccion.
        /// 
        /// </summary>
        /// <param name="p1">Production 1</param>
        /// <param name="p2Left">Left token of production 2</param>
        /// <param name="p2Right">Right tokens of producion 2</param>
        /// <returns></returns>
        public static bool Comparator(Production p1, Token p2Left, List<Token> p2Right)
        {
            bool isRightEqual = p1.Right.Count == p2Right.Count;

            for (var i = 0; i < p1.Right.Count && isRightEqual; i++)
                isRightEqual = p1.Right[i].Content == p2Right[i].Content;

            return p1.Left.Content == p2Left.Content && isRightEqual;
        }

        //Getters and Setters
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        internal Token Left
        {
            get { return left; }
            set { left = value; }
        }

        internal List<Token> Right
        {
            get { return right; }
            set { right = value; }
        }

        internal HashSet<Token> First
        {
            get { return first; }
            set { first = value; }
        }


    }
}
