using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class LR1Element
    {
        //Getters and Setters L -> α.γ, {δ}
        internal Token Left { get; set; }
        internal List<Token> Alpha { get; set; }
        internal List<Token> Gamma { get; set; }
        internal List<Token> Advance { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public LR1Element()
        {
            Left = new Token();
            Alpha = new List<Token>();
            Gamma = new List<Token>();
            Advance = new List<Token>();
        }

        /// <summary>
        /// Constructor that makes Lr1elements of the form [B-> .γ, b]
        /// </summary>
        /// <param name="InProduction"></param>
        /// <param name="inAdvance"></param>
        public LR1Element(Production InProduction, List<Token> inAdvance )
        {
            Left = new Token(InProduction.Left.Content, false);
            Alpha = new List<Token>();
            Gamma = new List<Token>(InProduction.Right);
            Advance = new List<Token>(inAdvance);
        }

        public override string ToString() {
            return Left.Content + " -> " + ListTokenString(Alpha, "") +  "." 
                + ListTokenString(Gamma, "") + ", { " + ListTokenString(Advance,", ") + "}";
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
