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
                    string idV1 = node.Left.Left.Left.Content;
                    Quads.Add(new Quad("idV", node.Left.Left.Right.Content, null, idV1));
                    Quads.Add(new Quad("posV", node.Left.Right.Left.Left.Content, node.Left.Right.Left.Right.Content, idV1));
                    Quads.Add(new Quad("tamV", node.Left.Right.Right.Left.Content, node.Left.Right.Right.Right.Content, idV1));

                    node.Solved = true;
                    SwitchNodes(node.Right);

                    Quads.Add(new Quad("endV", null, null, idV1));

                    break;



                case ";":
                    if (!node.Left.Solved)
                        SwitchNodes(node.Left);
                    node.Solved = true;


                    SwitchNodes(node.Right);


                    break;

                // Crea Boton
                case "CB":
                    string idB = node.Left.Left.Left.Content;
                    Quads.Add(new Quad("idB", node.Left.Left.Right.Content, null, idB));
                    Quads.Add(new Quad("posB", node.Left.Right.Left.Left.Content, node.Left.Right.Left.Right.Content, idB));
                    Quads.Add(new Quad("tamB", node.Left.Right.Right.Left.Content, node.Left.Right.Right.Right.Content, idB));

                    node.Solved = true;
                    SwitchNodes(node.Right);

                    Quads.Add(new Quad("endB",null,null,idB));

                    break;


                // Crea Label
                case "CL":

                    string idL = node.Left.Left.Content;

                    Quads.Add(new Quad("idL", node.Left.Right.Content, null, idL));
                    Quads.Add(new Quad("posL", node.Right.Left.Content, node.Right.Right.Content, idL));
                    

                    node.Solved = true;
                   

                    break;

                case "sent-if":



                    break;



            }

        }


       

        private void  GenericNode(BinaryTreeNode node)
        {

           
        }
    }

   


}
