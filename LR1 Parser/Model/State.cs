using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LR1_Parser.Model
{
	[Serializable]
    /// <summary>  
    /// Representa un estado en la tabla de análisis sintáctico.
    /// 
    /// Contiene dos diccionarios para terminales y no terminales, indexados por el token en forma de string.
    /// </summary> 
    class State
    {
        Dictionary<string, Action> nonTerminals; // Conjunto de acciones de los no terminales
        Dictionary<string, Action> terminals; // Conjunto de acciones de los terminales

        internal Dictionary<string, Action> Terminals { get { return terminals; } set { terminals = value; } }
        internal Dictionary<string, Action> NonTerminals { get { return nonTerminals; } set { nonTerminals = value; } }

        public State()
        {
            nonTerminals = new Dictionary<string, Action>();
            terminals = new Dictionary<string, Action>();            
        }
    }

	/// <summary>  
	/// Representa una acción, por ejemplo s2, r1, etc...
	/// 
	/// La acción puede tener los valores:
	/// S which means SHIFT
	/// R which means REDUX
	/// A which means ACCEPT
	/// 
	/// </summary> }
	[Serializable]
	struct Action
	{
		internal char action; // Puede ser 'S', 'R' u 'A'
		internal int state; // Estado al que va

		// Se sobrecarga metodo para facilitar despliegue
		public override string ToString()
		{
			if (action != '\0')
				return action + "" + state.ToString();
			else
				return " ";
		}

		public override bool Equals(object a2)
		{
			return action == ((Action)a2).action && state == ((Action)a2).state;
		}

		public bool IsEmpty()
		{
			return action == '\0';
		}
    }

    /// <summary>
    /// 
    /// Clase para guardar una estructura que tenga Token y estado
    /// 
    /// Queda más amigable que una tupla
    /// </summary>
    class TokenState
    {
        internal int state; // Estado al que va
        internal Token token; // Token
        internal bool dirty;

        // Se sobrecarga metodo para facilitar despliegue
        public override string ToString()
        {
            return token.Content + state;
        }
    }
}
