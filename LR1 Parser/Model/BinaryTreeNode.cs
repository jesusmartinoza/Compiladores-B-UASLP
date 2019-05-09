using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class BinaryTreeNode
    {
        String content;
        BinaryTreeNode left;
        BinaryTreeNode right;

        internal BinaryTreeNode Right { get => right; set => right = value; }
        internal BinaryTreeNode Left { get => left; set => left = value; }
        public string Content { get => content; set => content = value; }

        public BinaryTreeNode(String content)
        {
            Content = content;
        }

        public BinaryTreeNode(String content, BinaryTreeNode left, BinaryTreeNode right)
        {
            Content = content;
            Left = left;
            Right = right;
        }
    }
}
