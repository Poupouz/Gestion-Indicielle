using Gestion_Indicielle.Models;
using LibrarySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Input;

namespace Gestion_Indicielle.ViewModels
{
    class ViewCharts
    {

        private double[,] matricePrice;
        //mini test
        private ArrayList portfolio;

        private ArrayList al;
        private List<LineSeries> _series;
        public List<LineSeries> series { get; set; }

        public ICommand DisplayCharts{get; set;}

        public ViewCharts()
        {
            DataRetriever dr = new DataRetriever();
            al = dr.getTickers();
            AverageHistoricYield ahy = new AverageHistoricYield();
            matricePrice = ahy.getMatrixOfPrice(al, new DateTime(2012, 2, 3, 0, 0, 0), 5);
            AlgorythmOfTracking algo = new AlgorythmOfTracking(al, 100.0, 300, 10);
            //double[] coeff = algo.weightsComputation();

            portfolio = algo.tracking();

            /*foreach (double d in portfolio)
            {
                Console.WriteLine(d);
            }*/

            series = new List<LineSeries>();
        }

        private void Run()
        {
            System.Windows.MessageBox.Show("aa");
        }

        private void addSerieToChart(String Title, List<KeyValuePair<int, double>> valueList)
        {
            LineSeries lineSeries1 = new LineSeries();
            lineSeries1.Title = Title;
            lineSeries1.DependentValuePath = "Value";
            lineSeries1.IndependentValuePath = "Key";
            lineSeries1.ItemsSource = valueList;
            series.Add(lineSeries1);
        }

        public void createSerie()
        {
            List<KeyValuePair<int, double>> valueList = new List<KeyValuePair<int, double>>();

            //for (int i = 0; i < matricePrice.GetLength(0); i++)
            //{

            //    valueList.Insert(i, new KeyValuePair<int, double>(i + 1, matricePrice[i, 1]));
            //}
            int i = 0;
            foreach (double d in portfolio)
            {
                valueList.Insert(i, new KeyValuePair<int, double>(i + 1, d));
                i++;
            }
            addSerieToChart((String)al[0], valueList);
        }


    }
}
