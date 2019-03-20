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

namespace LR1_Parser
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Page gramaticaPage;
        Page analisisPage;

        public MainWindow()
        {
            InitializeComponent();
            gramaticaPage = new PageGramatica();
            analisisPage = new PageAnalisis();
            Frame.Navigate(gramaticaPage);
            
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

       
    }
}
