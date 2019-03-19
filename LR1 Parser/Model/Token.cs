using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class Token
    {
        /*string content;
        bool terminal; // Bandera para indicar si es terminal o no-terminal
        string val; // Guarda lexema o el resultado de evaluar un conjunto de atributos
        //Getters and Setters
        public string Content { get => content; set => content = value; }
        public bool Terminal { get => terminal; set => terminal = value; }
        public string Val { get => val; set => val = value; }*/
        string content;
        bool terminal; // Bandera para indicar si es terminal o no-terminal
        string val; // Guarda lexema o el resultado de evaluar un conjunto de atributos

        public Token(string s, bool t)
        {
            terminal = t;
            content = s;
        }

        //Getters and Setters
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public bool Terminal
        {
            get { return terminal; }
            set { terminal = value; }
        }

        public string Val
        {
            get { return val; }
            set { val = value; }
        }
    }
}
