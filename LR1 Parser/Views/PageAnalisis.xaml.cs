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

        private void AnalizarFuente_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
