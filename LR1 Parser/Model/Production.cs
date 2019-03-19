using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class Production
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
