using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class QuadGenerator
    {
        delegate void  Instruct2Quads(BinaryTreeNode node);
        static List<Quad> Quads;
        static Stack<Quad> ReturnsStack;
        static Stack<object> AuxValuesStack;
        static Dictionary<string, Instruct2Quads> Dictionary;

        public  List<Quad> Generate(BinaryTreeNode tree)
        {

            SwitchNodes(tree); 
            return Quads;
        }



        public void SwitchNodes(BinaryTreeNode node)
        {
            switch (node.Content)
            {

                // Crea ventana 1
                case "CV1":
                    string id = node.Left.Left.Left.Content;
                    Quads.Add(new Quad("idV", node.Left.Left.Right.Content, null, id));
                    Quads.Add(new Quad("posV", node.Left.Right.Left.Left.Content, node.Left.Right.Left.Right.Content, id));
                    Quads.Add(new Quad("tamV", node.Left.Right.Right.Left.Content, node.Left.Right.Right.Right.Content, id));

                    node.Solved = true;
                    SwitchNodes(node.Right);

                    break;


            }

        }


       

        private void  GenericNode(BinaryTreeNode node)
        {

           
        }
    }

   


}
