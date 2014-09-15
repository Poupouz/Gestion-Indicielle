using Gestion_Indicielle.Models;
using Gestion_Indicielle.ViewModels;
using LibrarySQL;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;
using WallRiskEngine;

namespace Gestion_Indicielle
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double[] benchmarkIndex;
        private double[] trackingValues;
        private double trackingError;
        private double informationRatio;
        PortfolioViewModel p;
        ArrayList tickers;
        private const int MAX_DAY_WINDOW = 1998;
        private Random random = new Random();
        private BackgroundWorker bw;

        public MainWindow()
        {
            InitializeComponent();
            p = new PortfolioViewModel();
            this.DataContext = p;
            lineChart.Series.RemoveAt(0);

            /* Init background worker */
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
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
        private void displayCAC40Chart(ViewCharts chart, int numberOfDays, int estimationWindow)
        {
            DataRetriever dr = new DataRetriever();
            double[] tmp = dr.extractColumnIndex(dr.getDataBenchmark(new DateTime(2006, 1, 2, 0, 0, 0), numberOfDays), 0);
            benchmarkIndex = new double[tmp.GetLength(0) - estimationWindow];
            for (int i = 0; i < benchmarkIndex.GetLength(0); i++)
            {
                benchmarkIndex[i] = tmp[i + estimationWindow];
            }
            addChartWithoutDots(chart, chart.createSerie(benchmarkIndex, "Cac40"), CACStyle());
        }

        /// <summary>
        /// Get the tracking error from the bench extract and the portfolio values
        /// </summary>
        /// <param name="algo"></param>
        private void getIndicator(AlgorythmOfTracking algo)
        {
            if (benchmarkIndex.GetLength(0) == trackingValues.GetLength(0))
            {
                double[,] matData = new double[benchmarkIndex.GetLength(0), 2];
                for (int i = 0; i < matData.GetLength(0); i++)
                {
                    matData[i, 0] = trackingValues[i];
                    matData[i, 1] = benchmarkIndex[i];
                }
                AverageHistoricYield ahy = new AverageHistoricYield();
                trackingError = algo.computeTrackingError(ahy.getReturnsMatrix(matData,1));
                informationRatio = algo.computeInformationRation(ahy.getReturnsMatrix(matData, 1), trackingError);

            }
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
            trackingValues = (double[])algoTracking.tracking().ToArray(typeof(double));
            addChartWithoutDots(chart, chart.createSerie(trackingValues, "Tracking"), null);
            this.getIndicator(algoTracking);
            TrackingErrorOutput.Text = (trackingError*100).ToString("F")+" %";
            InformationRatioOutput.Text = (informationRatio * 100).ToString("F") + " %";
        }

        private class LaunchArguments
        {
            private String _estimationWindowInput;

            public String EstimationWindowInput
            {
                get { return _estimationWindowInput; }
            }
            private String _rebalanceWindowInput;

            public String RebalanceWindowInput
            {
                get { return _rebalanceWindowInput; }
            }

            public LaunchArguments(String estimationWindowInput, String rebalanceWindowInput)
            {
                _estimationWindowInput = estimationWindowInput;
                _rebalanceWindowInput = rebalanceWindowInput;
            }
        }

        /// <summary>
        /// Action when Launch Simulation Button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param
        private void Launch_Simulation_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Value = 0;
            if (bw.IsBusy != true)
            {
                // Start the asynchronous operation.
                bw.RunWorkerAsync(new LaunchArguments(EstimationWindowInput.Text, RebalanceWindowInput.Text));
            }
        } 
        
        private void Cancel_Simulation_Click(object sender, RoutedEventArgs e)
        {
            if (bw.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                bw.CancelAsync();
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            int estimationWindow;
            int rebalanceWindow;
            try
            {
                estimationWindow = int.Parse( ((LaunchArguments) e.Argument).EstimationWindowInput );
                rebalanceWindow = int.Parse( ((LaunchArguments) e.Argument).RebalanceWindowInput );
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

            worker.ReportProgress(20);
            
            ViewCharts chart = new ViewCharts();
            Application.Current.Dispatcher.Invoke(new displayChartsDelegate(displayCharts),
                new object[] {chart, estimationWindow, rebalanceWindow, worker });

        }

        private delegate void displayChartsDelegate(ViewCharts chart, int estimationWindow, int rebalanceWindow, BackgroundWorker worker);

        private void displayCharts(ViewCharts chart, int estimationWindow, int rebalanceWindow, BackgroundWorker worker)
        {
            displayCAC40Chart(chart, MAX_DAY_WINDOW, estimationWindow);
            worker.ReportProgress(30);
            displayTracking(chart, tickers, estimationWindow, rebalanceWindow);
            worker.ReportProgress(50);
        }

        // This event handler updates the progress.
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }
        // This event handler deals with the results of the background operation.
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBar.Value = 100;
        }     

        private delegate void displayDelegate(int estimationWindow, int rebalanceWindow);

        private void displayChart(int estimationWindow, int rebalanceWindow)
        {

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
