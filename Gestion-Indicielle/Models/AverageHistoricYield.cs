using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrarySQL;
using System.Collections;
using WallRiskEngine;

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

        public double[,] getReturnsMatrix(double[,] matrixOfPrice, int horizon)
        {
            double[,] res = API.computeSimpleNetReturns(matrixOfPrice,horizon);
            return res;
        }

        //Renvoie la matrice de covariance de la matrice de rentabilité
        public double[,] getCovMatrix(double[,] returnMatrix)
        {
            double[,] res = API.computeCovarianceMatrix(returnMatrix);
            return res;

        }
    }
}
