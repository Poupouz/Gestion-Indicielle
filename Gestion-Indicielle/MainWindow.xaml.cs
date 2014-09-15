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
        PortfolioViewModel p;
        ArrayList tickers;

        public MainWindow()
        {
            InitializeComponent();
            numberDays = 1998;
            DataRetriever dr = new DataRetriever();
            benchmarkIndex = dr.extractColumnIndex(dr.getDataBenchmark(new DateTime(2006, 1, 2, 0, 0, 0), numberDays), 0);
            p = new PortfolioViewModel();
            this.DataContext = p;
            
            ViewCharts Chart = new ViewCharts();
            Chart.createSerie(benchmarkIndex,"Cac40");
            Style dataPointStyle = GetNewDataPointStyle();
            Chart.series.ElementAt(0).DataPointStyle = dataPointStyle;
            lineChart.Series.Add(Chart.series.ElementAt(0));
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tickers = new ArrayList(p.ComponentInfoList.Where(x => x.IsSelected).ToArray());
        }

        private static Style GetNewDataPointStyle()
        {
            
            Style style = new Style(typeof(DataPoint));
            
            Setter st2 = new Setter(DataPoint.BorderBrushProperty,
                                        new SolidColorBrush(Colors.Red));
            Setter st3 = new Setter(DataPoint.BorderThicknessProperty, new Thickness(0.1));
            Setter st4 = new Setter(DataPoint.TemplateProperty, null);
            
            style.Setters.Add(st2);
            style.Setters.Add(st3);

            style.Setters.Add(st4);
            return style;

        }
    }
}
