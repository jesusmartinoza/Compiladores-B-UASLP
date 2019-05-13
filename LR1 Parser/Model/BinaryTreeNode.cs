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
        Boolean visited;
        int id;

        internal BinaryTreeNode Right { get => right; set => right = value; }
        internal BinaryTreeNode Left { get => left; set => left = value; }
        public string Content { get => content; set => content = value; }
        public bool Visited { get => visited; set => visited = value; }
        public int Id { get => id; set => id = value; }

        public BinaryTreeNode(String content)
        {
            Content = content;
            Visited = false;
            Id = 0;
        }

        public BinaryTreeNode(String content, BinaryTreeNode left, BinaryTreeNode right)
        {
            Content = content;
            Left = left;
            Right = right;
            Visited = false;
            Id = 0;
        }
    }
}
