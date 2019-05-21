using LR1_Parser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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

        //**************************SERIALIZACION - DESERIALIZACION*********************************************
        public static void serializacionBinaria(List<State> States)
        {
            //Seleccion de formateador
            BinaryFormatter formateador = new BinaryFormatter();
            //XmlSerializer formateadorXml = new XmlSerializer(typeof(List<State>));

            //Se crea el Stream
            Stream miStream = new FileStream(Environment.CurrentDirectory + "\\estadosSerializados11", FileMode.Create, FileAccess.Write, FileShare.None);


            //Serializacion
            formateador.Serialize(miStream, States);


            //Cerrar Stream
            miStream.Close();
        }

        public static List<State> deserializacionBinaria()
        {
            List<State> States = new List<State>();

            //Seleccion de formateador
            BinaryFormatter formateador = new BinaryFormatter();


            //Se crea el Stream
            Stream miStream = new FileStream(Environment.CurrentDirectory + "\\estadosSerializados11", FileMode.Open, FileAccess.Read, FileShare.None);


            //Deserializacion
            States = (List<State>)formateador.Deserialize(miStream);


            //Cerrar Stream
            miStream.Close();

            return States;
        }
        //**************************SERIALIZACION - DESERIALIZACION*********************************************


    }
}
