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
        /// Constructor that make a copy a input Lr1Element
        /// </summary>
        public LR1Element(LR1Element inputLR1)
        {
            Left = inputLR1.Left;
            Alpha = inputLR1.Alpha;
            Gamma = inputLR1.Gamma;
            Advance = inputLR1.Advance;
        }

        /// <summary>
        /// Constructor that makes Lr1elements of the form [B-> .γ, b]
        /// </summary>
        /// <param name="InProduction"></param>
        /// <param name="inAdvance"></param>
        public LR1Element(Production InProduction, List<Token> inAdvance )
        {
            Left = new Token(InProduction.left.Content, false);
            Alpha = new List<Token>();
            Gamma = new List<Token>(InProduction.right);
            Advance = new List<Token>(inAdvance);
        }

        /// <summary>
        /// Override ToString method that converts a Lr1 element into a structured string
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return Left.Content + " -> " + ListTokenString(Alpha, "") +  "." 
                + ListTokenString(Gamma, "") + ", { " + ListTokenString(Advance,", ") + "}";
        }

        /// <summary>
        /// Simple tool that concatenates a list of tokens with a defined separator  
        /// </summary>
        /// <param name="inputList"></param>
        /// <param name="elementBetween"></param>
        /// <returns></returns>
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
