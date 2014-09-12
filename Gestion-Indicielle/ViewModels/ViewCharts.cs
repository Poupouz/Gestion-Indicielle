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
        private ArrayList al;
        public List<LineSeries> series { get; set; }

        public ViewCharts()
        {
           
            DataRetriever dr = new DataRetriever();
            al = dr.getTickers();
            AverageHistoricYield ahy = new AverageHistoricYield();
            series = new List<LineSeries>();
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

                valueList.Insert(i, new KeyValuePair<int, double>(i + 1, newseries[i]/newseries[0]*100));
            }
            addSerieToChart(Title, valueList);
        }


    }
}
