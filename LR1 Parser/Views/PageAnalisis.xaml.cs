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
using LR1_Parser.Model;
using Microsoft.Win32;

namespace LR1_Parser
{
    /// <summary>
    /// Lógica de interacción para Analisis.xaml
    /// </summary>
    public partial class PageAnalisis : Page
    {

        List<Quad> CurrentQuads = new List<Quad>();

        public PageAnalisis()
        {
            InitializeComponent();
        }



        private void AbrirFuente_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "Archivos de texto (*.txt)| *.txt";
            dialog.AddExtension = true;

            if (dialog.ShowDialog() == true)
            {
                EntradaFuente.Text = File.ReadAllText(dialog.FileName);
                App.sourceFilePath = dialog.FileName;
            }
        }

        private void GuardarFuente_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(EntradaFuente.Text))
            {
                if (File.Exists(App.sourceFilePath))
                {
                    File.WriteAllText(App.sourceFilePath, EntradaFuente.Text);
                }
                else
                {
                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.DefaultExt = "txt";
                    dialog.AddExtension = true;

                    if (dialog.ShowDialog() == true)
                    {
                        App.sourceFilePath = dialog.FileName;
                        File.WriteAllText(App.sourceFilePath, EntradaFuente.Text);

                    }
                }

            }
        }

        private async void AnalizarFuente_Click(object sender, RoutedEventArgs e)
        {

            if (App.currentParser != null)
            {
                if (!string.IsNullOrEmpty(EntradaFuente.Text))
                {
                    TablaAcciones.ItemsSource = null;

                    if (App.currentParser.EvalString(EntradaFuente.Text))
                    {

                        Log.Text = "La cadena de entrada es valida!";
                        Log.Foreground = new SolidColorBrush(Color.FromRgb(51, 204, 51));
                        QuadGenerator quadGenerator = new QuadGenerator();
                        //CurrentQuads= quadGenerator.Generate(App.currentParser.NodeStack.Peek());

                    }
                    else
                    {
                        Log.Text = "La cadena de entrada no es valida. :( ";
                        Log.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));

                    }

                    TablaAcciones.ItemsSource = App.currentParser.Log;
                }
                else
                    MessageBox.Show("Introduzca código fuente");

            }
            else
                MessageBox.Show("Primero Cree una tabla de Analisis Sintáctico en la sección Gramática");
            

        }
    }
}
