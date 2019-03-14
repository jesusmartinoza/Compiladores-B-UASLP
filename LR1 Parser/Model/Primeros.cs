using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class Primeros
    {


        private Dictionary<Token, List<Token>> ListaPrimeros;

        /// <summary>
        /// Inicializa el diccionario de primeros en base a una lista de producciones dada
        /// </summary>
        /// <param name="producciones">Lista de producciones en base a la cual se calculara el conjunto primero de cada No Terminal</param>
        public static void Inicializa(List<Production> producciones)
        {
            

        }


        /// <summary>
        /// Regresa el conjunto de primeros para un No Terminal dado.
        /// </summary>
        /// <param name="NT">Token no terminal</param>
        /// <returns> Lista de elementos de la clase Token</returns>
        public static List<Token> GetPrimeros(Token NT)
        {
            return null;
        }




    }

    
}
