using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class QuadGenerator
    {
        delegate BinaryTreeNode Instruct2Quads(BinaryTreeNode node);
        static List<Quad> Quads;
        static Stack<Quad> ReturnsStack;
        static Stack<object> AuxValuesStack;
        static Dictionary<string, Instruct2Quads> Dictionary;

        public static List<Quad> Generate(BinaryTreeNode tree)
        {





            return Quads;
        }


       

        public static void SentWhile()
        {

        }




    }

    public struct Quad
    {
        string Operator;
        object OperandA;
        object OperandB;
        object Result;

    }
}
