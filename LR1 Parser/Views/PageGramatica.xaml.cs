using System;
using System.Collections.Generic;
using System.Data;
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
using Action = LR1_Parser.Model.Action;

namespace LR1_Parser
{
    /// <summary>
    /// Lógica de interacción para PAgeGramatica.xaml
    /// </summary>
    public partial class PageGramatica : Page
    {
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

                EntradaGramatica.Text = File.ReadAllText(dialog.FileName);
                App.grammarFilePath = dialog.FileName;
                
            }

        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(EntradaGramatica.Text))
            {
                if (File.Exists(App.grammarFilePath))
                {
                    File.WriteAllText(App.grammarFilePath, EntradaGramatica.Text);
                }
                else
                {
                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.DefaultExt = "txt";
                    dialog.AddExtension = true;
                    
                    if (dialog.ShowDialog() == true)
                    {
                        App.grammarFilePath = dialog.FileName;
                        File.WriteAllText(App.grammarFilePath, EntradaGramatica.Text);

                    }
                }

            }
        }

        private void GenerarTabla_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(EntradaGramatica.Text))
            {

                // Se limpia UI tabla A.S
                TablaAnalisis.ItemsSource = null;


                // Se separan el texto de entrada de la gramática y se crea la lista de producciones 
                Tokenizer obtenProd = new Tokenizer();   
                MainWindow.productions = obtenProd.obtenProducciones(EntradaGramatica.Text);

                List<Token> simbolosGramaticales = obtenProd.tokens;
                simbolosGramaticales.RemoveAll(pred => pred.Content == "ε");

                // Se calcula el conjunto de primeros para la gramática
                Primeros primeros = new Primeros(MainWindow.productions);

                //Se calcula el AFD de la lista de producciones 
                AFDGenerator AFDGen = new AFDGenerator(MainWindow.productions, primeros, simbolosGramaticales);
                List<Node> AFD = AFDGen.GenerateAFD();

                App.currentParser= new Parser(AFD);
                

                


                App.currentParser.EvalString("n + n - n - n + n");

                // Se muestran los primeros en la UI
                PrimerosTable.ItemsSource = primeros.GetView();

                // Se crea la tabla de Analisis Sintáctico
                App.currentParser.CreateSyntaxisAnalysisTable();
                // Se muestra la tabla de Analisis Sintáctico
                ShowTablaAS(App.currentParser.States);









            }
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


        // Función con una gram'atica estatica para probar primeros...
        public void TestPrimeros()
        {
            Primeros primeros = new Primeros(MainWindow.productions);
            var nada = primeros.GetView();
            PrimerosTable.ItemsSource = nada;
        }

        /// <summary>
        /// Recibe la lista de estados del AFD que conforman la tabla de analisis sintáctico y los despliega en la GUI
        /// </summary>
        /// <param name="states">Lista de estados</param>
        private void ShowTablaAS(List<State> states)
        {
            DataTable tabla = new DataTable();
            for (int i = 0; i < states.Count; i++)
            {

                Dictionary<string, Action> allTokens = states[i].NonTerminals.Concat(states[i].Terminals).ToDictionary(x => x.Key, x => x.Value);

                if (i == 0)
                {
                    DataColumn colEstado = new DataColumn();
                    colEstado.ColumnName = "Estado";
                    tabla.Columns.Add(colEstado);
                    colEstado.ReadOnly = true;
                    foreach (KeyValuePair<string, Action> token in allTokens)
                    {
                        DataColumn col = new DataColumn();
                        col.ColumnName = token.Key;
                        tabla.Columns.Add(col);
                        col.ReadOnly = true;

                    }

                }

                DataRow r = tabla.NewRow();

                r[0] = i;
                foreach (KeyValuePair<string, Action> token in allTokens)
                {
                    r[token.Key] = token.Value;

                }
                tabla.Rows.Add(r);

            }
            TablaAnalisis.ItemsSource = tabla.DefaultView;

        }

        private void Epsilon2clipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("ε");
        }

       
    }
}
