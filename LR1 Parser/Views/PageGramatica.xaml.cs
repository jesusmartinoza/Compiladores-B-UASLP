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
