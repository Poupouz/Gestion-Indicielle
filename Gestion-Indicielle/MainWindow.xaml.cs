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

namespace Gestion_Indicielle
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MyDataGrid.ItemsSource = LoadCompanies();
        }

        private List<Object> LoadCompanies()
        {
            List<Object> result=new List<Object>();
            result.Add(new {Name="Michelin", IsActive=false});
            result.Add(new { Name = "Total" });
            result.Add(new { Name = "Total" });
            result.Add(new { Name = "Total" });
            result.Add(new { Name = "Total" });
            result.Add(new { Name = "Total" });
            result.Add(new { Name = "Total" });
            result.Add(new { Name = "Total" });
            result.Add(new { Name = "Total" });
            return result;  
        }

    }
}
