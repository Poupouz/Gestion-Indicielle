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

        //Permet la concatenation de la matrice data avec celle de la benchmark
        public double[,] concatMatrix(double[,] data, double[,] bench)
        {
            if (data.GetLength(0) == bench.GetLength(0))
            {
                double[,] res = new double[data.GetLength(0), data.GetLength(1) + bench.GetLength(1)];

                for (int i = 0; i < res.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        res[i, j] = data[i, j];
                    }
                    for (int k = 0; k < bench.GetLength(1); k++)
                    {
                        res[i, data.GetLength(1) + k] = bench[i, k];
                    }
                }

                return res;
            }
            else
            {
                return null;
            }
        }

        //Permet d extraire la matrice de variance-covariance des actifs
        public double[,] extractCovReturnAssets(double[,] covMat, int nbAsset)
        {
            double[,] res = new double[nbAsset, nbAsset];

            for (int i = 0; i < res.GetLength(0); i++)
            {
                for (int j = 0; j < res.GetLength(1); j++)
                {
                    res[i, j] = covMat[i, j];
                }
            }
          
            return res;
        }

        //Permet d'extraire le vecteur de cov entre les actifs et le benchmark
        public double[] extractCovReturnBench(double[,] covMat, int nbAsset)
        {
            double[] res = new double[nbAsset];

            for (int i = 0; i < res.GetLength(0); i++)
            {
                res[i] = covMat[i,nbAsset];
            }

            return res;
        }


        //Permet d extraire la variance du benchmark
        public double extractVarianceBench(double[,] covMat, int nbAsset)
        {
            return covMat[nbAsset, nbAsset];
        }

    }
}
