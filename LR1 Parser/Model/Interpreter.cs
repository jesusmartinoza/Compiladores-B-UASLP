using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_Parser.Model
{
    class Interpreter
    {
        Dictionary<string, dynamic> simbTable;
        List<Quad> quadsList;

        public struct FormItem
        {
            public string id;
            public int posX, posY, SizeX, SizeY;

            public void SetID(string id)
            {
                this.id = id;
            }

            public void SetPosition(int x, int y)
            {
                posX = x;
                posY = y;
            }

            public void SetSize(int x, int y)
            {
                SizeX = x;
                SizeY = y;
            }

        }

        public Interpreter()
        {
            GenerateManualQuads();
            ReadByQuads();
        }

        private void GenerateManualQuads()
        {
            quadsList.Add(new Quad(":=",    "2000",     null,   "numero"));
            quadsList.Add(new Quad(":=",    "0",        null,   "dato"));
            quadsList.Add(new Quad(">",     "numero",   "1000", "t1"));
            quadsList.Add(new Quad("gotoF", "t1",       "10",   null));
            quadsList.Add(new Quad("-",     "numero",   "1",    "t2"));
            quadsList.Add(new Quad(":=",    "t2",       null,   "numero"));
            quadsList.Add(new Quad("+",     "dato",     "1",    "t3"));
            quadsList.Add(new Quad(":=",    "t3",       null,   "dato"));
            quadsList.Add(new Quad("goto",  "3",        null,   null));
            quadsList.Add(new Quad("end",   null,       null,   null));
        }

        private void GenerateManualQuads2()
        {

        }

        private void ReadByQuads()
        {
            for (int i = 0; i < quadsList.Count; i++)
            {
                switch(quadsList[i].Operator)
                {
                    
                    case ":=":
                        string key = quadsList[i].Result.ToString();
                        string OpA = quadsList[i].ToString();
                        if (simbTable.Keys.Contains(key))
                        {
                            AddData(OpA, key);
                        }
                        else
                        {
                            simbTable.Add(key, null);
                            AddData(OpA, key);

                        }
                        break;
                    case "<":
                        break;
                    case "<=":
                        break;
                    case ">":
                        break;
                    case ">=":
                        break;
                    case "+":
                        break;
                    case "-":
                        break;
                    case "*":
                        break;
                    case "/":
                        break;
                    case "^":
                        //checar si es menor de 0 la potencia, es una raiz cuadrada
                        break;
                    case "goto":
                        break;
                    case "gotoF":
                        break;
                    case "gotoT":
                        break;
                    case "idV":
                        break;
                    case "posV":
                        break;
                    case "tamV":
                        break;
                    case "idB":
                        break;
                    case "posB":
                        break;
                    case "tamB":
                        break;
                    case "idL":
                        break;
                    case "posL":
                        break;
                }
            }
        }

        private void AddData(string variable, string key)
        {
            if (simbTable.Keys.Contains(variable))
            {
                variable = simbTable[variable].ToString();
            }
            

            bool isNumeric = int.TryParse(variable, out int n);
            bool isBoolean = bool.TryParse(variable, out bool b);
            if (isNumeric)
            {
                simbTable[key] = n;
            }
            else if (isBoolean)
            {
                simbTable[key] = b;
            }
            else
            {
                simbTable[key] = variable;
            }
        }
    }
}
