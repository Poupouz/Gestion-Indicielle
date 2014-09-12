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

        public ViewCharts()
        {
            DataRetriever dr = new DataRetriever();
            al = dr.getTickers();
            AverageHistoricYield ahy = new AverageHistoricYield();
        }

        public LineSeries createSerie(double[] newseries, String Title)
        {
            // Coordinate are given in an object like this :
            // { x = 2.0, y = -3.0 }

            List<Object> valueList = new List<Object>();

            for (int i = 0; i < newseries.GetLength(0); i++)
            {
                valueList.Insert(i, new { x = i + 1, y = newseries[i] / newseries[0] * 100} );
            }
            
            LineSeries lineSeries1 = new LineSeries();
            lineSeries1.Title = Title;
            lineSeries1.DependentValuePath = "y";
            lineSeries1.IndependentValuePath = "x";
            lineSeries1.ItemsSource = valueList;
            return lineSeries1;
        }


    }
}
