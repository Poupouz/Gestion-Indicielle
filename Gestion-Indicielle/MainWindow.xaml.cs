using Gestion_Indicielle.Models;
using Gestion_Indicielle.ViewModels;
using LibrarySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;

namespace Gestion_Indicielle
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
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

        private int previousEstimationWindow = -1;

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
                informationRatio = algo.computeInformationRatio(ahy.getReturnsMatrix(matData, 1), trackingError);

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
        private void displayTracking(ViewCharts chart, ArrayList tickers, int estimWindow, int periodRebalance, double targetPerformance)
        {
            AlgorythmOfTracking algoTracking = new AlgorythmOfTracking(tickers, 100, estimWindow, periodRebalance, targetPerformance);
            trackingValues = (double[])algoTracking.tracking().ToArray(typeof(double));
            addChartWithoutDots(chart, chart.createSerie(trackingValues, "Tracking_" + estimWindow + '_' + periodRebalance + '_' + targetPerformance * 10000 + '_' + lineChart.Series.Count), null);
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

            private String _targetPerformanceInput;

            public String TargetPerformanceInput
            {
                get { return _targetPerformanceInput; }
            }
            public LaunchArguments(String estimationWindowInput, String rebalanceWindowInput, String targetPerformanceInput)
            {
                _estimationWindowInput = estimationWindowInput;
                _rebalanceWindowInput = rebalanceWindowInput;
                _targetPerformanceInput = targetPerformanceInput;
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
                bw.RunWorkerAsync(new LaunchArguments(EstimationWindowInput.Text, RebalanceWindowInput.Text, TargetPerformanceInput.Text));
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
            double targetPerformance;
            try
            {
                estimationWindow = int.Parse( ((LaunchArguments) e.Argument).EstimationWindowInput );
                rebalanceWindow = int.Parse( ((LaunchArguments) e.Argument).RebalanceWindowInput );
                targetPerformance = 0.0001*double.Parse(((LaunchArguments)e.Argument).TargetPerformanceInput.Replace(".", ","));
            }
            catch (System.FormatException exception)
            {
                System.Windows.MessageBox.Show(exception.Message);
                return;
            }
            catch (OverflowException)
            {
                MessageBox.Show("Number too high.", "Error");
                return;
            }

            tickers = new ArrayList(p.ComponentInfoList
                .Where(x => x.IsSelected)
                .Select(y => PortfolioViewModel.hashtableCompanies[y.Tickers]).ToArray());

            if (tickers.Count <= 1)
            {
                System.Windows.MessageBox.Show("Non defined and positive matrix, please select more than one company", "Error");
                return;
            }
            if (rebalanceWindow > MAX_DAY_WINDOW)
            {
                System.Windows.MessageBox.Show("Rebalance window must be lower than " + MAX_DAY_WINDOW, "Error");
                return;
            }
            if (rebalanceWindow <= 0)
            {
                System.Windows.MessageBox.Show("Rebalance window must be positive", "Error");
                return;
            }
            if (estimationWindow <= tickers.Count)
            {
                System.Windows.MessageBox.Show("Estimation window must be greater than the number of companies selected", "Error");
                return;
            }
            if (estimationWindow > MAX_DAY_WINDOW)
            {
                System.Windows.MessageBox.Show("Estimation window must be lower than " + MAX_DAY_WINDOW, "Error");
                return;
            }

            if (targetPerformance > 0.001)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Target performance : " + targetPerformance + " is really high, are you sure you want to continue ?", 
                    "Target performance confirmation", 
                    System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                    return;
            }

            //if (targetPerformance > 0.0004)

            worker.ReportProgress(20);
            
            ViewCharts chart = new ViewCharts();
            Application.Current.Dispatcher.Invoke(new displayChartsDelegate(displayCharts),
                new object[] {chart, estimationWindow, rebalanceWindow, targetPerformance, worker});
        }

        private delegate void displayChartsDelegate(ViewCharts chart, int estimationWindow, int rebalanceWindow, double targetPerformance, BackgroundWorker worker);

        private void displayCharts(ViewCharts chart, int estimationWindow, int rebalanceWindow, double targetPerformance, BackgroundWorker worker)
        {
            if (previousEstimationWindow != estimationWindow)
            {
                Reset_Click(null, null);
                displayCAC40Chart(chart, MAX_DAY_WINDOW, estimationWindow);
                previousEstimationWindow = estimationWindow;
            }
            worker.ReportProgress(30);
            try
            {
                displayTracking(chart, tickers, estimationWindow, rebalanceWindow, targetPerformance);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message,"Error");
            }
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

        public void Dispose()
        {
            bw.Dispose();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                do {
                    previousEstimationWindow = -1;
                    lineChart.Series.RemoveAt(0);
                } while (true);
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
        }

        private void checkAll_Click(object sender, RoutedEventArgs e)
        {
            if ((bool) checkAll.IsChecked)
            {
                foreach (var v in p.ComponentInfoList)
                {
                    v.IsSelected = true;
                }
                checkAll.Content = "Uncheck all";
            }
            else
            {
                foreach (var v in p.ComponentInfoList)
                {
                    v.IsSelected = false;
                }
                checkAll.Content = "Check all";
            }
            listCheckBox.Items.Refresh();
        }
    }
}
