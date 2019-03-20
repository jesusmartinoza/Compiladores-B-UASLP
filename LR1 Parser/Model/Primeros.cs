using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class Primeros
    {


        Dictionary<Token, List<Token>> primeros = new Dictionary<Token, List<Token>>();
      

        /// <summary>
        /// Regresa el conjunto de primeros para una lista de Tokens Terminales o No Terminales.
        /// </summary>
        /// <param name="tokens">Lista de Tokens</param>
        /// <returns> Lista de elementos de la clase Token</returns>
        public static List<Token> GetPrimerosDe(List<Token> tokens)
        {
            return null;
            // Falta implementar

        }


        /// <summary>
        /// Constructor: Inicializa el diccionario de primeros en base a una lista de producciones dada
        /// </summary>
        /// <param name="Gramatica">Lista de producciones en base a la cual se calculara el conjunto primero de cada No Terminal</param>

        public Primeros(List<Production> Gramatica)
        {
            ObtenerPrimeros(Gramatica);


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

            for (int i = 0; i < p.right.Count; i++)
            {

                Token t = p.right[i];

                if (t.Terminal == false)
                {
                    //Se revisa si este NT tiene primeros
                    List<Token> primerosdelNT = ObtenerPrimerosdelNT(t);
                    if (primerosdelNT != null)
                    {

                        // Si el NT es anulable (contiene epsilon)
                        if (primerosdelNT.Any(x => x.Content == "ε"))
                        {
                            // Se saca el token epsilon de los primeros que se agregarán 
                            Token ep = primerosdelNT.Find(x => x.Content == "ε");
                            primerosdelNT.Remove(ep);

                            //Regreso algo en la lista, tiene primeros
                            cambio += Agregaprimeros(p.left, primerosdelNT);
                        }
                        else
                        {
                            //Regreso algo en la lista, tiene primeros
                            cambio += Agregaprimeros(p.left, primerosdelNT);
                            break;
                        }





                    }
                    else
                        break;

                }
                else
                {
                    cambio += Agregaprimeros(p.left, t);
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
