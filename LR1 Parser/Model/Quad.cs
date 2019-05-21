using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
	[Serializable]
    public class Quad
    {


        public string Operator { get; set; }
        public string OperandA { get; set; }
        public string OperandB { get; set; }
        public string Result { get; set; }

        public Quad(string oprtr, string opA, string opB, string result)
        {

            Operator = oprtr;
            OperandA = opA;
            OperandB = opB;
            Result = result;
        }

        public override string ToString()
        {
            return "|  " + Operator.ToString() + "  |  " + OperandA.ToString() + "  |  " + OperandB.ToString() + "  |  " + Result.ToString() + "  |";
            
        }

    }
}
