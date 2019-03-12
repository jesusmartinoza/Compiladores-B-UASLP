using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    /// <summary>  
    /// Representa un estado en la tabla de análisis sintáctico.
    /// 
    /// Contiene dos diccionarios para terminales y no terminales, indexados por el token en forma de string.
    /// </summary> 
    class State
    {
        Dictionary<string, Action> nonTerminals; // Conjunto de acciones de los no terminales
        Dictionary<string, Action> terminals; // Conjunto de acciones de los terminales
    }

    /// <summary>  
    /// Representa una acción, por ejemplo s2, r1, etc...
    /// 
    /// La acción puede tener los valores:
    /// S which means SHIFT
    /// R which means REDUX
    /// A which means ACCEPT
    /// </summary> 
    struct Action
    {
        char action; // Puede ser 'S', 'R' u 'A'
        int state; // Estado al que va
    }
}
