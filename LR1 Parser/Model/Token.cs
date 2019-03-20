using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    public class Token
    {
        string content;
        bool terminal; // Bandera para indicar si es terminal o no-terminal
        string val; // Guarda lexema o el resultado de evaluar un conjunto de atributos

        //Getters and Setters
        public string Content { get => content; set => content = value; }
        public bool Terminal { get => terminal; set => terminal = value; }
        public string Val { get => val; set => val = value; }
      

        public Token(string s, bool t)
        {
            terminal = t;
            content = s;
        }
        public Token()
        { }


        public override string ToString()
        {
            return content;
        }

      
    }
}
