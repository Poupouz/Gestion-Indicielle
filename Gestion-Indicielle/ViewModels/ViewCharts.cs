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
        
        private int numberDays;
        private ArrayList al;
        private List<LineSeries> _series;
        public List<LineSeries> series { get; set; }

        public ICommand DisplayCharts{get; set;}

        public ViewCharts()
        {
            numberDays=5;
            DataRetriever dr = new DataRetriever();
            al = dr.getTickers();
            AverageHistoricYield ahy = new AverageHistoricYield();
            matricePrice = ahy.getMatrixOfPrice(al, new DateTime(2012, 2, 3, 0, 0, 0), numberDays);
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

        public void createSerie(double[] newseries, String Title)
        {
            List<KeyValuePair<int, double>> valueList = new List<KeyValuePair<int, double>>();

            for (int i = 0; i < newseries.GetLength(0); i++)
            {

                valueList.Insert(i, new KeyValuePair<int, double>(i + 1, newseries[i]));
            }
            addSerieToChart(Title, valueList);
        }


    }
}
