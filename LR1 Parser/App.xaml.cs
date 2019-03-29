using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LR1_Parser
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static string grammarFilePath= null;
        internal static string sourceFilePath = null;
        internal static Model.Parser currentParser = null;

    }


}
