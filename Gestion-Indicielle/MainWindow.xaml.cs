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
            //Console.WriteLine(dr.nbDate());
            
           // AlgorythmOfTracking algo = new AlgorythmOfTracking(al, 1000.0, 1000, 1000);
            //double[] coeff = algo.weightsComputation();

            //ArrayList mesRes = algo.tracking();

           //foreach (double d in mesRes)
           // {
           //     Console.WriteLine(d);
           // }

            /*double somme = 0.0;
                for (int i = 0; i < coeff.GetLength(0); i++)
                {
                    Console.WriteLine(coeff[i]);
                    somme += coeff[i];
                }*/

                //Console.WriteLine(somme);


                /*-------------------------------------------------------*/


                foreach (var v in al)
                {
                    result.Add(new { Name = v, IsInPortfolio = false });
                }
            
            return result;  

        }


            

        

    }
}
