using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrarySQL;
using System.Collections;

namespace Gestion_Indicielle.Models
{
    class AverageHistoricYield
    {
        private DataRetriever _dataRetriever;

        public DataRetriever DataRetriever
        {
            get { return _dataRetriever; }
            private set { _dataRetriever = value; }
        }

        public AverageHistoricYield()
        {
            this.DataRetriever = new DataRetriever(); 
        }

        //extraction des données sur une période donnée et pour des tickers donnés
        public double[,] getMatrixOfPrice(ArrayList tickers, System.DateTime startDate, int period)
        {
          double[,] res = new double[period,tickers.Count];
          int indiceEntreprise = 0;
          foreach (String s in tickers)
          {
              double[] tmp = this.DataRetriever.getData(startDate, s, period);
              //Recopie des données dans le tableau
              for (int i = 0; i < period; i++)
              {
                  res[i, indiceEntreprise] = tmp[i];
              }
              indiceEntreprise++;
          }

          return res;
        }
 
    }
}
