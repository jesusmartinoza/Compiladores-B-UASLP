using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser
{
    /// <summary>
    /// Conjunto de metodos utiles para todo el proyecto
    /// </summary>
    class Helpers
    {
        /// <summary>
        /// Transforma una lista a string
        /// 
        /// </summary>
        /// <typeparam name="T">Tipo de dato</typeparam>
        /// <param name="list">Lista a transformar</param>
        /// <returns></returns>
        public static string ListToString<T>(List<T> list)
        {
            string strList = "";

            // TODO: Usar string builder
            foreach (var item in list)
                strList += item.ToString();

            return strList;
        }
    }
}
