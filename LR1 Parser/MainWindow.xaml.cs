using Microsoft.Win32;
using System.IO; 
using System;
using System.Collections.Generic;
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
using LR1_Parser.Model;

namespace LR1_Parser
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Page gramaticaPage;
        Page analisisPage;

        //Lista de producciones
        public static List<Production> productions;

        public MainWindow()
        {
            InitializeComponent();
            gramaticaPage = new PageGramatica();
            analisisPage = new PageAnalisis();
            productions = new List<Production>();
            Frame.Navigate(gramaticaPage);

            InitTestGrammar2();
        }

        private void Analisis_Tab_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate( analisisPage);
        }

        private void Gramatica_Tab_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate( gramaticaPage);
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private void InitTestGrammar()
        {
            /*NO TERMINALES*/

            Token n = new Token();
            n.Content = "S";
            n.IsTerminal = false;

            Token n1 = new Token();
            n1.Content = "A";
            n1.IsTerminal = false;

            Token n2 = new Token();
            n2.Content = "B";
            n2.IsTerminal = false;

            Token n3 = new Token();
            n3.Content = "C";
            n3.IsTerminal = false;

            Token n4 = new Token();
            n4.Content = "D";
            n4.IsTerminal = false;

            /*TERMINALES*/
            Token t = new Token();
            t.Content = "x";
            t.IsTerminal = true;

            Token t1 = new Token();
            t1.Content = "y";
            t1.IsTerminal = true;

            Token t2 = new Token();
            t2.Content = "z";
            t2.IsTerminal = true;

            Token t3 = new Token();
            t3.Content = "w";
            t3.IsTerminal = true;

            Token t4 = new Token();
            t4.Content = "ε";
            t4.IsTerminal = true;


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

            productions.Add(p);
            productions.Add(p1);
            productions.Add(p2);
            productions.Add(p3);
            productions.Add(p4);
            productions.Add(p5);
            productions.Add(p6);
            productions.Add(p7);
            productions.Add(p8);
        }


        /// <summary>
        /// Gramatica de prueba
        /// 
        /// E' -> E
        /// E -> E + n
        /// E -> E - n
        /// E -> n
        /// </summary>
        private void InitTestGrammar2()
        {
            Token Ep = new Token();
            Ep.Content = "E'";
            Ep.IsTerminal = false;

            Token E = new Token();
            E.Content = "E";
            E.IsTerminal = false;

            Token plusSymbol = new Token();
            plusSymbol.Content = "+";
            plusSymbol.IsTerminal = true;

            Token minusSymbol = new Token();
            minusSymbol.Content = "-";
            minusSymbol.IsTerminal = true;

            Token n = new Token();
            n.Content = "n";
            n.IsTerminal = true;

            // E' -> E
            Production p0 = new Production();
            p0.Left = Ep;
            p0.Right.Add(E);

            // E -> E + n
            Production p1 = new Production();
            p1.Left = E;
            p1.Right.Add(E);
            p1.Right.Add(plusSymbol);
            p1.Right.Add(n);

            // E -> E - n
            Production p2 = new Production();
            p2.Left = E;
            p2.Right.Add(E);
            p2.Right.Add(minusSymbol);
            p2.Right.Add(n);

            // E -> n
            Production p3 = new Production();
            p3.Left = E;
            p3.Right.Add(n);

            productions.Add(p0);
            productions.Add(p1);
            productions.Add(p2);
            productions.Add(p3);
        }
    }
}
