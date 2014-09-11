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

        public double[] getMeanReturn(double[,] returnMatrix)
        {
            double[] res = new double[returnMatrix.GetLength(1)];

            //On parcourt chacun des titres
            for (int j = 0; j < returnMatrix.GetLength(1); j++)
            {
                double sum = 0.0;

                //On parcourt chacune des dates pour une action donnée
                for (int i = 0; i < returnMatrix.GetLength(0); i++)
                {
                    sum += returnMatrix[i, j];
                }

                res[j] = sum/returnMatrix.GetLength(0);

            }
            
            return res;
        }
    }
}
