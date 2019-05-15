using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    public class Quad
    {


        string Operator;
        object OperandA;
        object OperandB;
        object Result;

        public Quad(string oprtr, string opA, string opB, string result)
        {

            Operator = oprtr;
            OperandA = opA;
            OperandB = opB;
            Result = result;
        }

    }
}
