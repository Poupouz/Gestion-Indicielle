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
using WallRiskEngine;

namespace Gestion_Indicielle
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private double[] benchmarkIndex;
        private int numberDays;

        public MainWindow()
        {
            InitializeComponent();
            numberDays = 100;
            DataRetriever dr = new DataRetriever();
            benchmarkIndex = dr.extractColumnIndex(dr.getDataBenchmark(new DateTime(2012, 2, 3, 0, 0, 0), numberDays), 0);
            this.DataContext = new PortfolioViewModel();
            ViewCharts Chart = new ViewCharts();
            Chart.createSerie(benchmarkIndex,"Cac40");
            lineChart.Series.RemoveAt(0);
            lineChart.Series.Add(Chart.series.ElementAt(0));
        }
    }
}
