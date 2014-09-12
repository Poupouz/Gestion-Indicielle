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
using Gestion_Indicielle.ViewModels;

namespace Gestion_Indicielle.View
{
    /// <summary>
    /// Logique d'interaction pour Portfolio.xaml
    /// </summary>
    public partial class Portfolio : UserControl
    {
        public Portfolio()
        {
            InitializeComponent();
            this.DataContext = new PortfolioViewModel();
        }
    }
}
