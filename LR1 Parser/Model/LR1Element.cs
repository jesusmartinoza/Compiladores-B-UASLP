using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class LR1Element
    {
        Token left; // Token del lado izquierdo de la flecha 
        List<Token> alpha; // Conjunto de tokens del lado izquierdo del punto
        List<Token> gamma; // Conjunto de tokens del lado derecho del punto
        List<Token> advance; //Conjunto de tokens de anticipación
        //Getters and Setters
        internal Token Left { get => left; set => left = value; }
        internal List<Token> Alpha { get => alpha; set => alpha = value; }
        internal List<Token> Gamma { get => gamma; set => gamma = value; }
        internal List<Token> Advance { get => advance; set => advance = value; }


        public LR1Element()
        {
        }

        public override string ToString() {
            return ListTokenString(Left, "") + " -> " + ListTokenString(Alpha, "") +  "." 
                + ListTokenString(Gamma, "") + ", { " + ListTokenString(advance,", ") + "}";
        }

        private string ListTokenString(List<Token> inputList, string elementBetween)
        {
            string strResult = "";
            for (int i = 0; i < inputList.Count; i++)
            {
                strResult += inputList[i].Content;
                if (i != inputList.Count - 1)
                    strResult += elementBetween;
            }
            return strResult;
        }


    }
}
