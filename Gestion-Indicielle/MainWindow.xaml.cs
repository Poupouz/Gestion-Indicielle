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
        PortfolioViewModel p;
        ArrayList tickers;
        private const int MAX_DAY_WINDOW = 1998;
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            p = new PortfolioViewModel();
            this.DataContext = p;
        }

        private void addChartWithoutDots(ViewCharts chart, LineSeries series, Style dataPointStyle)
        {
            if (dataPointStyle == null)
                dataPointStyle = GetNewDataPointStyle();
            series.DataPointStyle = dataPointStyle;
            lineChart.Series.Add(series);
        }

        /// <summary>
        /// Get the CAC40 data for given numberOfDays and plot it into chart
        /// </summary>
        /// <param name="numberOfDays"></param>
        private void displayCAC40Chart(ViewCharts chart, int numberOfDays)
        {
            DataRetriever dr = new DataRetriever();
            double[] tmp = dr.extractColumnIndex(dr.getDataBenchmark(new DateTime(2006, 1, 2, 0, 0, 0), numberOfDays), 0);
            benchmarkIndex = new double[tmp.GetLength(0)-int.Parse(EstimationWindowInput.Text)];
            for (int i = 0; i < benchmarkIndex.GetLength(0); i++)
            {
                benchmarkIndex[i] = tmp[i + int.Parse(EstimationWindowInput.Text)];
            }
            addChartWithoutDots(chart, chart.createSerie(benchmarkIndex, "Cac40"),  CACStyle());
        }

        /// <summary>
        /// Compute tracking and display data into the given chart
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="tickers"></param>
        /// <param name="initCash"></param>
        /// <param name="estimWindow"></param>
        /// <param name="periodRebalance"></param>
        private void displayTracking(ViewCharts chart, ArrayList tickers, int estimWindow, int periodRebalance)
        {
            AlgorythmOfTracking algoTracking = new AlgorythmOfTracking(tickers, 100, estimWindow, periodRebalance);
            double[] trackingValues = (double[]) algoTracking.tracking().ToArray(typeof(double));
            addChartWithoutDots(chart, chart.createSerie(trackingValues, "Tracking"), null);
        }

        /// <summary>
        /// Action when Launch Simulation Button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Launch_Simulation_Click(object sender, RoutedEventArgs e)
        {
            int estimationWindow;
            int rebalanceWindow;
            try
            {
                estimationWindow = int.Parse(EstimationWindowInput.Text);
                rebalanceWindow = int.Parse(RebalanceWindowInput.Text);
            }
            catch (System.FormatException exception)
            {
                System.Windows.MessageBox.Show(exception.Message);
                return;
            }

            tickers = new ArrayList(p.ComponentInfoList
                .Where(x => x.IsSelected)
                .Select(y => PortfolioViewModel.hashtableCompanies[y.Tickers]).ToArray());

            if (rebalanceWindow > MAX_DAY_WINDOW)
            {
                System.Windows.MessageBox.Show("Estimation window must be lower than " + MAX_DAY_WINDOW);
                return;
            }

            if (estimationWindow <= tickers.Count)
            {
                System.Windows.MessageBox.Show("Estimation window must be greater than the number of companies selected");
                return;
            }

            ViewCharts chart = new ViewCharts();
            lineChart.Series.RemoveAt(0);
            displayCAC40Chart(chart, MAX_DAY_WINDOW);

            displayTracking(chart, tickers, estimationWindow, rebalanceWindow);
        }

        /// <summary>
        /// <summary>
        /// Gets the new data point style.
        /// </summary>
        /// <returns></returns>
        private Style GetNewDataPointStyle()
        {
            Color background = Color.FromRgb((byte)this.random.Next(100),
                                             (byte)this.random.Next(100),
                                             (byte)this.random.Next(100));
            Style style = new Style(typeof(DataPoint));
            Setter st1 = new Setter(DataPoint.BackgroundProperty,
                                        new SolidColorBrush(background));
            Setter st2 = new Setter(DataPoint.BorderBrushProperty,
                                        new SolidColorBrush(Colors.White));
            Setter st3 = new Setter(DataPoint.BorderThicknessProperty, new Thickness(0.1));

            Setter st4 = new Setter(DataPoint.TemplateProperty, null);
            style.Setters.Add(st1);
            style.Setters.Add(st2);
            style.Setters.Add(st3);
            style.Setters.Add(st4);
            return style;
        }

        private Style CACStyle()
        {
            Color background = Colors.Crimson;
            Style style = new Style(typeof(DataPoint));
            Setter st1 = new Setter(DataPoint.BackgroundProperty,
                                        new SolidColorBrush(background));
            Setter st2 = new Setter(DataPoint.BorderBrushProperty,
                                        new SolidColorBrush(Colors.White));
            Setter st3 = new Setter(DataPoint.BorderThicknessProperty, new Thickness(0.1));

            Setter st4 = new Setter(DataPoint.TemplateProperty, null);
            style.Setters.Add(st1);
            style.Setters.Add(st2);
            style.Setters.Add(st3);
            style.Setters.Add(st4);
            return style;
        }
    }
}
