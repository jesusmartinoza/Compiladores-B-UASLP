using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class Token
    {
        string id;
        List<Token> left; // Conjunto de tokens del lado izquierdo de la flecha 
        List<Token> right;  // Conjunto de tokens del lado derecho de la flecha 
        HashSet<Token> first; // Conjunto de primeros de forma desordenada
    }
}
