﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{

    /// <summary>
    /// https://github.com/jesusmartinoza/Compiladores-B-UASLP/wiki#tokenizacion-an%C3%A1lisis-l%C3%A9xico
    /// 
    /// Módulo desarrollado por Chivas :D
    /// </summary>
    public class Tokenizer
    {
        //Lista de T y NT
        public List<string> tokenNT;
        public List<string> tokenT;
        //Lista de producciones
        public List<Production> producciones;

        //Lista de todos los Token existentes
        public List<Token> tokens;

        //indice para saber el numero de producciones
        int indice;

        /// <summary>
        /// Obtiene un token especifico de la lista de tokens
        /// </summary>
        /// <param name="name">Nombre del token</param>
        /// <returns>Token con el nombre dado</returns>
        public Token getToken(string name)
        {
            foreach (Token t in tokens)
            {
                if (t.Content == name)
                {
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// A partir de una gramatica dada obtiene las producciones
        /// </summary>
        /// <param name="EntradaGramatica">Gramatica</param>
        /// <returns>Lista de producciones</returns>
        public List<Production> obtenProducciones(string EntradaGramatica)
        {
            //Inicializacion de variables
            producciones = new List<Production>();
            tokenNT = new List<string>();
            tokenT = new List<string>();
            tokens = new List<Token>();
            //TText.Text = NTtext.Text = "";
            indice = 1;
            List<string> grammar = new List<string>();
            string aux = EntradaGramatica.Replace("\r", "");
            aux = aux.Replace("->", "→");
            grammar = aux.Split('\n').ToList();

            //Separacion en NT y T
            SeparaNT(grammar);
            SeparaT(grammar);


            foreach (string s in grammar)
            {
                string[] aux2 = s.Split('→');
                List<string> list = aux2[1].Split('|').ToList();

                foreach (string ss in list)
                {
                    producciones.Add(new Production(indice, getToken(aux2[0])));
                    List<string> tokenProd = ss.Split(' ').ToList();
                    foreach (string ss2 in tokenProd)
                    {
                        if (ss2 != "")
                        {
                            producciones[producciones.Count - 1].Right.Add(getToken(ss2));
                        }
                    }
                    indice++;
                }
            }

            return producciones;
        }

        /// <summary>
        /// Obtiene los token No Terminales de la gramatica
        /// </summary>
        /// <param name="grammar">Gramatica</param>
        private void SeparaNT(List<string> grammar)
        {
            foreach (string s in grammar)
            {
                if (!tokenNT.Contains(s.Split('→')[0]))//Verifica que no exista ya en la lista
                {
                    tokenNT.Add(s.Split('→')[0]);
                    tokens.Add(new Token(s.Split('→')[0], false));
                }
            }
        }

        /// <summary>
        /// Obtiene los token Terminales de la gramatica
        /// </summary>
        /// <param name="grammar">Gramatica</param>
        private void SeparaT(List<string> grammar)
        {
            foreach (string s in grammar)
            {
                foreach (string ss in s.Split('→')[1].Split(' '))//Separa los diferentes tokens de la parte derecha de la flecha
                {
                    if (!tokenNT.Contains(ss) && !tokenT.Contains(ss) && ss != "|")//Verifica que no sea un NT, aun no exista en la lista o sea el operador "|" de las gramaticas
                    {
                        tokenT.Add(ss);
                        tokens.Add(new Token(ss, true));
                    }
                }
            }
        }
    
        /// <summary>
        /// Metodo para tokenizar una cadena
        /// </summary>
        /// <param name="input">Cadena de entrada</param>
        /// <returns>Tokens de la cadena</returns>
        public static List<Token> Convert(string input)
        {
            List<Token> aux = new List<Token>();
            List<string> list = input.Split(' ').ToList();
            
            foreach(string s in list)
            {
                aux.Add(new Token(s, true));
            }

            return aux;
        }
    }
}
