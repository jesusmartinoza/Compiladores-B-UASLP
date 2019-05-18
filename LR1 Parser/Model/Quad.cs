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
        public object OperandA { get; set; }
        public object OperandB { get; set; }
        public object Result { get; set; }

        public Quad(string oprtr, string opA, string opB, string result)
        {

            Operator = oprtr;
            OperandA = opA;
            OperandB = opB;
            Result = result;
        }

    }
}
