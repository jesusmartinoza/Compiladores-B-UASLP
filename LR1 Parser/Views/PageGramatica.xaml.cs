using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;

namespace LR1_Parser
{
    /// <summary>
    /// Lógica de interacción para PAgeGramatica.xaml
    /// </summary>
    public partial class PageGramatica : Page
    {

        public string TextoGramatica { get; set; }
        //Lista de T y NT
        public List<string> tokenNT;
        public List<string> tokenT;
        //Lista de producciones
        public List<Productions> producciones;

        //Lista de todos los Token existentes
        public List<Token> tokens;

        //indice para saber el numero de producciones
        int indice;
        public PageGramatica()
        {
            InitializeComponent();
           
            
        }

        private void Abrir_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "Archivos de texto (*.txt)| *.txt";
            dialog.AddExtension = true;
            


            if (dialog.ShowDialog() == true)
            {
                //Stream file = dialog.OpenFile();

                //string text = "";

                //byte[] buffer = new byte[file.Length];
                //file.ReadAsync(buffer, 0, (int)file.Length);

                //foreach (byte b in buffer)
                //    text += (char)b;

                EntradaGramatica.Text = File.ReadAllText(dialog.FileName);
            }


        

        }

        private void GenerarTabla_Click(object sender, RoutedEventArgs e)
        {
            //Inicializacion de variables
            producciones = new List<Productions>();
            tokenNT = new List<string>();
            tokenT = new List<string>();
            tokens = new List<Token>();
            TText.Text = NTtext.Text = "";
            indice = 1;
            List<string> grammar = new List<string>();
            string aux = grammarText.Text.Replace("\r","");
            aux = aux.Replace("->", "→");
            grammar = aux.Split('\n').ToList();

            //Separacion en NT y T
            foreach (string s in grammar)
            {
                if (!tokenNT.Contains(s.Split('→')[0]))//Verifica que no exista ya en la lista
                {
                    tokenNT.Add(s.Split('→')[0]);
                    tokens.Add(new Token(s.Split('→')[0], false));
                }
            }
            foreach(string s in grammar)
            {
                foreach(string ss in s.Split('→')[1].Split(' '))//Separa los diferentes tokens de la parte derecha de la flecha
                {
                    if (!tokenNT.Contains(ss) && !tokenT.Contains(ss) && ss!="|")//Verifica que no sea un NT, aun no exista en la lista o sea el operador "|" de las gramaticas
                    {
                        tokenT.Add(ss);
                        tokens.Add(new Token(ss, true));
                    }
                }
            }

            foreach (string s in grammar)
            {
                string[] aux2 = s.Split('→');
                List<string> list = aux2[1].Split('|').ToList();

                foreach(string ss in list)
                {
                    producciones.Add(new Productions(indice, getToken(aux2[0])));
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

            //Muestra en pantalla
            foreach(string s in tokenNT)
            {
                NTtext.Text += s + "\r\n";
            }
            foreach (string s in tokenT)
            {
                TText.Text += s + "\r\n";
            }
        }
        
        public Token getToken(string name)
        {
            foreach(Token t in tokens)
            {
                if (t.Content == name)
                {
                    return t;
                }
            }
            return null;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {


            string fileDir = System.IO.Path.Combine(Directory.GetCurrentDirectory(),"GramaticaSyntaxHL.xshd");


            if (File.Exists(fileDir))
            {
                using (XmlTextReader reader = new XmlTextReader(File.Open(fileDir, FileMode.Open)))
                {


                    EntradaGramatica.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);

                }
            }
            else
            {
                MessageBox.Show("Falta archivo xml de resaltado, no te preocupes solo no se verá bonito el texto de la gramatica, para activar el resaltado mueve el archivo GramaticaSyntaxHL.xshd a la carpeta del ejecutable ");
            }
           
        }
    }
}
