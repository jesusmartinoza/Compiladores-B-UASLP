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
    /// Lógica de interacción para Analisis.xaml
    /// </summary>
    public partial class PageAnalisis : Page
    {
        public PageAnalisis()
        {
            InitializeComponent();

            Parser parser = new Parser();

            parser.EvalString("n + n - n");
        }

       
    }
}
