using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibrarySQL;
using Gestion_Indicielle.Models;
using WallRiskEngine;

namespace Gestion_Indicielle
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MyDataGrid.ItemsSource = LoadCompanies();
            
            
        }

        private List<Object> LoadCompanies()
        {
            List<Object> result=new List<Object>();
            DataRetriever dr = new DataRetriever();
            ArrayList al = dr.getTickers();


            /* ------ A ENLEVER POUR PLUS TARD ------ */
            Console.WriteLine(dr.nbDate());
            AverageHistoricYield ahy = new AverageHistoricYield();

            double[,] matrice = ahy.getMatrixOfPrice(al, new DateTime(2006, 1, 2, 0, 0, 0), 1000);

            double[,] returnsMatrix = ahy.getReturnsMatrix(matrice,1);

            double[] meanReturns = ahy.getMeanReturn(returnsMatrix);

            double[,] covMatrix = ahy.getCovMatrix(returnsMatrix);

            double[,] bench = dr.getDataBenchmark(new DateTime(2006, 1, 2, 0, 0, 0), 1000);
            double[,] returnsBench = ahy.getReturnsMatrix(bench, 1);

            double[,] concatMat = ahy.concatMatrix(matrice,bench);
            double[,] covConcat = ahy.getCovMatrix(concatMat);

            double[,] covMatrixExtract = ahy.extractCovReturnAssets(covConcat,covConcat.GetLength(0)-1 );
            double[]  covVectorExtract = ahy.extractCovReturnBench(covConcat,covConcat.GetLength(0) -1 );
            double varExtract = ahy.extractVarianceBench(covConcat,covConcat.GetLength(0) -1 );


            double[] coeff = API.OptimPortfolioWeight(covMatrixExtract, meanReturns, covVectorExtract, ahy.getMeanReturn(returnsBench)[0],0.000000001);
            Console.WriteLine(coeff.GetLength(0));
            double somme = 0.0;
            for (int i = 0; i < coeff.GetLength(0); i++)
            {
                Console.WriteLine(coeff[i]);
                somme += coeff[i];
            }

            Console.WriteLine(somme);

                /*-------------------------------------------------------*/


                foreach (var v in al)
                {
                    result.Add(new { Name = v, IsInPortfolio = false });
                }
            
            return result;  

        }


            

        

    }
}
