using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using LibrarySQL;
using Gestion_Indicielle.Models;
using Gestion_Indicielle.ViewModels;
using Microsoft.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;

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
            ViewCharts Chart = new ViewCharts();
            Chart.createSerie();
            lineChart.Series.Add(Chart.series.ElementAt(0));
        }

        private List<Object> LoadCompanies()
        {
            List<Object> result=new List<Object>();
            DataRetriever dr = new DataRetriever();
            ArrayList al = dr.getTickers();

            AverageHistoricYield ahy = new AverageHistoricYield();
            double[,] matrice = ahy.getMatrixOfPrice(al, new DateTime(2012, 2, 3, 0, 0, 0), 5);

            foreach(var v in al)
            {
                result.Add(new { Name = v, IsInPortfolio=false });
            }
            
            return result;  

        }

        
            

        

    }
}
