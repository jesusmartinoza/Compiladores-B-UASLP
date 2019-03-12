using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class LR1Element
    {
        List<Token> alpha; // Conjunto de tokens del lado izquierdo del punto
        List<Token> gama; // Conjunto de tokens del lado derecho del punto
    }
}
