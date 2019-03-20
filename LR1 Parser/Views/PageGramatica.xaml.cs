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
using LR1_Parser.Model;
using Microsoft.Win32;


namespace LR1_Parser
{
    /// <summary>
    /// Lógica de interacción para PAgeGramatica.xaml
    /// </summary>
    public partial class PageGramatica : Page
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
        public string TextoGramatica { get; set; }

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
            

            TestPrimeros();

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

        public void TestPrimeros()
        {
            /*NO TERMINALES*/

            Token n = new Token();
            n.Content = "S";
            n.Terminal = false;

            Token n1 = new Token();
            n1.Content = "A";
            n1.Terminal = false;

            Token n2 = new Token();
            n2.Content = "B";
            n2.Terminal = false;

            Token n3 = new Token();
            n3.Content = "C";
            n3.Terminal = false;

            Token n4 = new Token();
            n4.Content = "D";
            n4.Terminal = false;

            /*TERMINALES*/
            Token t = new Token();
            t.Content = "x";
            t.Terminal = true;

            Token t1 = new Token();
            t1.Content = "y";
            t1.Terminal = true;

            Token t2 = new Token();
            t2.Content = "z";
            t2.Terminal = true;

            Token t3 = new Token();
            t3.Content = "w";
            t3.Terminal = true;

            Token t4 = new Token();
            t4.Content = "ε";
            t4.Terminal = true;


            // **********************PRODUCCIONES*****************************



            // S ->ABCD
            Production p = new Production();
            p.Id = 0;
            p.left = n;
            List<Token> ltp = new List<Token>();
            ltp.Add(n1);
            ltp.Add(n2);
            ltp.Add(n3);
            ltp.Add(n4);
            p.right = ltp;


            /*A->Bx*/
            Production p1 = new Production();
            p1.Id = 1;
            p1.left = n1;
            List<Token> ltp1 = new List<Token>();
            ltp1.Add(n2);
            ltp1.Add(t);
            p1.right = ltp1;


            //Produccion con epsilon
            //  A -> ε
            Production p2 = new Production();
            p2.Id = 2;
            p2.left = n1;
            List<Token> ltp2 = new List<Token>();
            ltp2.Add(t4);
            p2.right = ltp2;



            /*B->Cy*/
            Production p3 = new Production();
            p3.Id = 3;
            p3.left = n2;
            List<Token> ltp3 = new List<Token>();
            ltp3.Add(n3);
            ltp3.Add(t1);
            p3.right = ltp3;


            //*B->ε/
            Production p4 = new Production();
            p4.Id = 4;
            p4.left = n2;
            List<Token> ltp4 = new List<Token>();
            ltp4.Add(t4);
            p4.right = ltp4;

            /*TerceraProduccion*/
            /*C->Dz*/
            Production p5 = new Production();
            p5.Id = 5;
            p5.left = n3;
            List<Token> ltp5 = new List<Token>();
            ltp5.Add(n4);
            ltp5.Add(t2);
            p5.right = ltp5;


            //*C->ε/
            Production p6 = new Production();
            p6.Id = 6;
            p6.left = n3;
            List<Token> ltp6 = new List<Token>();
            ltp6.Add(t4);
            p6.right = ltp6;

            /*D->w*/
            Production p7 = new Production();
            p7.Id = 7;
            p7.left = n4;
            List<Token> ltp7 = new List<Token>();
            ltp7.Add(t3);
            p7.right = ltp7;

            // D->ε
            Production p8 = new Production();
            p8.Id = 8;
            p8.left = n4;
            List<Token> ltp8 = new List<Token>();
            ltp8.Add(t4);
            p8.right = ltp8;


            List<Production> Gramatica = new List<Production>();
            Gramatica.Add(p);
            Gramatica.Add(p1);
            Gramatica.Add(p2);
            Gramatica.Add(p3);
            Gramatica.Add(p4);
            Gramatica.Add(p5);
            Gramatica.Add(p6);
            Gramatica.Add(p7);
            Gramatica.Add(p8);

            Primeros primeros = new Primeros(Gramatica);
            var nada = primeros.AllToStrings();

        }

    }
}
