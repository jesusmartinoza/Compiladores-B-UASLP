using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class Primeros
    {
        public Dictionary<Token, List<Token>> primeros = new Dictionary<Token, List<Token>>();

        /// <summary>
        /// Regresa el conjunto de primeros para una lista de Tokens Terminales o No Terminales.
        /// </summary>
        /// <param name="cadenaDeEntrada">Lista de Tokens</param>
        /// <returns> Lista de elementos de la clase Token</returns>
        public List<Token> GetPrimerosDe(List<Token> cadenaDeEntrada)
        {
            List<Token> cadenaPrimeros = new List<Token>();
            for (int i = 0; i < cadenaDeEntrada.Count ; i++)
            {
                if(!cadenaDeEntrada[i].IsTerminal)
                {   //Es No Terminal, se agrega
                    cadenaPrimeros.AddRange(primeros[cadenaDeEntrada[i]]);
                    if(primeros[cadenaDeEntrada[i]].Any(token => token.Content == "ε"))
                    {   //No Terminal contiene Epsilon
                        if(i + 1 < cadenaDeEntrada.Count)
                        {   //No es el ultimo elemento de la cadena, se quita Epsilon al Resultado
                            cadenaPrimeros.RemoveAll(token => token.Content == "ε");
                        }
                    }
                    else
                    {   //No Terminal sin Epsilon
                        i = cadenaDeEntrada.Count; //Se termina el ciclo
                    }
                }
                else
                {   //Terminal, se agrega
                    cadenaPrimeros.Add(cadenaDeEntrada[i]);
                    i = cadenaDeEntrada.Count; //Se termina el ciclo
                }
            }
            return cadenaPrimeros.Distinct().ToList();
        }

        /// <summary>
        /// Constructor: Inicializa el diccionario de primeros en base a una lista de producciones dada
        /// </summary>
        /// <param name="Gramatica">Lista de producciones en base a la cual se calculara el conjunto primero de cada No Terminal</param>

        public Primeros(List<Production> Gramatica, List<Token> TokensNoTerminales)
        {
            InicializarDiccionario(TokensNoTerminales);
            ObtenerPrimeros(Gramatica);           
        }
        /// <summary>
        /// Metodo que inicializa el diccionario para asegurar que todos los NoTerminales se encuentren en el
        /// </summary>
        /// <returns>n</returns>
        private void InicializarDiccionario(List<Token> tokensNoTerminales)
        {
            foreach (Token t in tokensNoTerminales) {
                primeros = new Dictionary<Token, List<Token>>();
                primeros.Add(t,new List<Token>());

            }
        }

        /// <summary>
        /// Metodo que regresa una representación en cadenas de el diccionario de primeros [UI]
        /// </summary>
        /// <returns>Representación para mostrar en la UI</returns>
        public List<Tuple<string, string>> GetView()
        {
            List<Tuple<string, string>> vista = new List<Tuple<string, string>>();

            foreach(KeyValuePair<Token,List<Token>> item in primeros )
            {
                string val1 = item.Key.ToString();
                string val2="{ ";

                foreach (Token t in item.Value)
                {
                    val2 += t.ToString();
                    if (t != item.Value.Last())
                        val2 += ", ";
                }
                val2 += " }";

                vista.Add(new Tuple<string, string>(val1,val2));
            }
            return vista;
        }

        private Dictionary<Token, List<Token>> ObtenerPrimeros(List<Production> Gramatica)
        {
            int cambio = 0;
            do
            {
                cambio = 0;
                foreach (Production p in Gramatica)
                {
                    cambio += PrimerosdeProduccion(p, Gramatica);
                }
            }
            while (cambio != 0);
            return primeros;
        }

        private int PrimerosdeProduccion(Production p, List<Production> Gramatica)
        {
            int cambio = 0;
            int numEpsilons = 0;
            int numRightTokens= p.Right.Count; 
            for (int i = 0; i < numRightTokens; i++)
            {
                Token t = p.Right[i];                
                if (t.IsTerminal == false)
                {
                    //Se revisa si este NT tiene primeros
                    List<Token> primerosdelNT = ObtenerPrimerosdelNT(t);
                    if (primerosdelNT != null)
                    {
                        // Si el NT es anulable (contiene epsilon)
                        if (primerosdelNT.Any(x => x.Content == "ε"))
                        {
                            numEpsilons++;
                            // Si no todos los del lado derecho son anulables
                            if (numEpsilons < numRightTokens)
                            {
                                // Se saca el token epsilon de los primeros que se agregarán 
                                Token ep = primerosdelNT.Find(x => x.Content == "ε");
                                primerosdelNT.Remove(ep);
                            }                            
                            //Regreso algo en la lista, tiene primeros
                            cambio += Agregaprimeros(p.Left, primerosdelNT);
                        }
                        else
                        {
                            //Regreso algo en la lista, tiene primeros
                            cambio += Agregaprimeros(p.Left, primerosdelNT);
                            break;
                        }
                    }
                    else
                        break;
                }
                else
                {
                    cambio += Agregaprimeros(p.Left, t);
                }
            }
            return cambio;
        }

        private int Agregaprimeros(Token left, Token t)
        {
            if (primeros.ContainsKey(left))
            {
                if (!primeros[left].Contains(t))
                {
                    primeros[left].Add(t);
                    return 1;
                }

            }
            else
            {
                primeros.Add(left, new List<Token>());
                if (!primeros[left].Contains(t))
                {
                    primeros[left].Add(t);
                    return 1;
                }

            }
            return 0;
        }

        private int Agregaprimeros(Token left, List<Token> primerosdelNT)
        {
            if (primeros.ContainsKey(left))
            {
                bool agrego = false;
                foreach (Token to in primerosdelNT)
                {
                    if (!primeros[left].Contains(to))
                    {
                        primeros[left].Add(to);
                        agrego = true;
                    }
                }
                if (agrego == true)
                    return 1;
            }
            else
            {
                primeros.Add(left, primerosdelNT);
                return 1;
            }
            return 0;
        }

        private List<Token> ObtenerPrimerosdelNT(Token t)
        {
            if (primeros.ContainsKey(t))
            {
                return new List<Token>(primeros[t]);
            }
            return null;
        }
    }


}
