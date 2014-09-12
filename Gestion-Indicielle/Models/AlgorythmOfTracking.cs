using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallRiskEngine;

namespace Gestion_Indicielle.Models
{
    class AlgorythmOfTracking
    {

        //Historique des valeurs du porte feuille
        private ArrayList _histPricePortFolio;

        public ArrayList HistPricePortFolio
        {
            get { return _histPricePortFolio; }
            set { _histPricePortFolio = value; }
        }

        //Date de debut a enlever plus tard
        private DateTime _startDate;

        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        //Date de fin
        private DateTime _endDate;

        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }

        //Matrice de donnée sur le marché
        private double[,] globalData;

        public double[,] GlobalData
        {
            get { return globalData; }
            set { globalData = value; }
        }

        //Tableau général des données de la bench
        private double[,] globalBench;

        public double[,] GlobalBench
        {
            get { return globalBench; }
            set { globalBench = value; }
        }

        //Attribut de tracking error
        private double trackingError;

        public double TrackingError
        {
            get { return trackingError; }
            set { trackingError = value; }
        }

        private int nbDate;

        public int NbDate
        {
            get { return nbDate; }
            set { nbDate = value; }
        }


        public AlgorythmOfTracking(ArrayList tickers, double initCash, int estimWin, int periodRebalance)
        {
            Tickers = tickers;
            InitialCash = initCash;
            EstimWindows = estimWin;
            PeriodOfRebalance = periodRebalance;
            Ahy = new AverageHistoricYield();
            StartDate = new DateTime(2006, 1, 2, 0, 0, 0);
            HistPricePortFolio = new ArrayList();
            EndDate = new DateTime(2008, 9, 3, 0, 0, 0);
            int ecartJour = EndDate.Subtract(StartDate).Days;
            NbDate = Ahy.DataRetriever.nbDate();
            GlobalData = Ahy.getMatrixOfPrice(Tickers, StartDate, NbDate);
            GlobalBench = Ahy.DataRetriever.getDataBenchmark(StartDate, NbDate);

            //GlobalData = Ahy.getMatrixOfPrice(Tickers, StartDate, ecartJour);
           // GlobalBench = Ahy.DataRetriever.getDataBenchmark(StartDate, ecartJour);
           
        }

        //Cash disponible au début de la simulation en euros
        private double _initialCash;

        //List of tickers
        private ArrayList _tickers;

        //Nombre de jours de la fenetre d estimation
        private int _estimWindows;

        //Nombre de jour entre 2 rebalancement
        private int _periodOfRebalance;

        //Outils d'aide au calcul
        private AverageHistoricYield _ahy;

        //Propriete d outils de calculs
        public AverageHistoricYield Ahy
        {
            get { return _ahy; }
            set { _ahy = value; }
        }

        //Definition de prop pour une periode de rebalancement
        public int PeriodOfRebalance
        {
            get { return _periodOfRebalance; }
            set { _periodOfRebalance = value; }
        }

        //Definition de propriete de la fenetre d estimation
        public int EstimWindows
        {
            get { return _estimWindows; }
            set { _estimWindows = value; }
        } 

        //Définition de propriété de la list de tickers
        public ArrayList Tickers
        {
            get { return _tickers; }
            set { _tickers = value; }
        }

        
        //Definition de la propriete des cash initial
        public double InitialCash
        {
            get { return _initialCash; }
            set { _initialCash = value; }
        }

        //Fonction d extraction de matrice
        public double[,] extractSubMatrixOfGlobalData(int indiceDeDebutExtract)
        {
            double[,] extract = new double[EstimWindows,Tickers.Count]; 
            
            //On parcours les lignes voulu
            for (int i = indiceDeDebutExtract; i < indiceDeDebutExtract + EstimWindows; i++)
            {
                //On parcourt chaque colonne
                for (int j = 0; j < Tickers.Count; j++)
                {
                    extract[i - indiceDeDebutExtract, j] = GlobalData[i, j];
                }
            }
            return extract;

        }

        //Permet d extraire la sous matrice correspondante a l indice de la date voulue
        public double[,] extractSubMatrixOfGlobalBench(int indiceDeDebutExtract)
        {
            double[,] extract = new double[EstimWindows, 1];

            //On parcours les lignes voulu
            for (int i = indiceDeDebutExtract; i < indiceDeDebutExtract + EstimWindows; i++)
            {
                
                extract[i - indiceDeDebutExtract, 0] = GlobalBench[i, 0];
                
            }
            return extract;

        }

        //Algo de rebalancement qui permet de renvoyer le tracker
        public double[] weightsComputation(int indiceDeDebut) //v2
        {
            //Recuperation des résultats obtenus
            double[,] estimatedData = this.extractSubMatrixOfGlobalData(indiceDeDebut); //v2
            double[,] returnsEstimData = Ahy.getReturnsMatrix(estimatedData,1);
            //Calcul des rentabilité moyenne des données estimé
            double[] meanReturnEstimData = Ahy.getMeanReturn(returnsEstimData);

            //Recuperation de la Benchmark
            double[,] benchMark = this.extractSubMatrixOfGlobalBench(indiceDeDebut); // v2
            double[,] returnsBenchmark = Ahy.getReturnsMatrix(benchMark,1);

            //Calcul de la rentabilité moyenne de la benchmark
            double[] meanReturnBenchmark = Ahy.getMeanReturn(returnsBenchmark);

            //Concatenation des 2 matrices
            double[,] concatMat = Ahy.concatMatrix(returnsEstimData, returnsBenchmark);

            //Calcul de la covariance
            double[,] covMat = Ahy.getCovMatrix(concatMat);

            //Extraction des données
            double[,] covAssets = Ahy.extractCovReturnAssets(covMat , covMat.GetLength(0) - 1) ;
            double[] covBenchWithAsset = Ahy.extractCovReturnBench(covMat, covMat.GetLength(0) - 1);

            //Calcul de poids optimaux
            double[] optimWeights = API.OptimPortfolioWeight(covAssets,meanReturnEstimData,covBenchWithAsset,meanReturnBenchmark[0],-0.00001);

            return optimWeights;
        }

        //Fonction renvoyant les priceOfAssets
        // assetsMatrix est la matrice des prix des assets pour une période donnée entre la date de début et la date de fin d estimation
        public double[] getPriceOfAssets(int indiceDate)
        {
            double[] priceOfAssets = new double[GlobalData.GetLength(1)];

            for (int i = 0; i < priceOfAssets.GetLength(0); i++)
            {

                priceOfAssets[i] = GlobalData[indiceDate , i];
            }
                return priceOfAssets;
        }

        //Fonction permettant d obtenir les volumes a investir
        public double[] getVolumeOfAssets(double[] optimWeights, double[] priceOfAssets , double currentPFPrice)
        {
            double[] volumeOfAssets = new double[priceOfAssets.GetLength(0)];

            for (int i = 0; i < volumeOfAssets.GetLength(0); i++)
            {
                volumeOfAssets[i] = currentPFPrice * (optimWeights[i] / priceOfAssets[i]);
            }

            return volumeOfAssets;
        }


        public double getPricePF2(double[] volOfAssets, int indiceDateRebalancement)
        {
            double[] portFolio = new double[volOfAssets.GetLength(0)];

            double[] priceOfAssets = new double[volOfAssets.GetLength(0)];

            //On recupere les prix sur le marché a la date de rebalancement
            priceOfAssets = this.getPriceOfAssets(indiceDateRebalancement);

            for (int i = 0; i < priceOfAssets.GetLength(0); i++)
            {
                portFolio[i] = volOfAssets[i] * priceOfAssets[i];
            }

            double sum = 0;
            for (int i = 0; i < portFolio.GetLength(0); i++)
            {
                sum += portFolio[i];
            }
            return sum;
        
        }

        public ArrayList tracking()
        {
            ArrayList res = new ArrayList();


            double CurrentCash = InitialCash;
            double[] weights = weightsComputation(0); //v2
            double[] priceOfCurrentAssets = getPriceOfAssets(0);
            double[] volumofCurrentAssets = getVolumeOfAssets(weights, priceOfCurrentAssets, CurrentCash);
    
            for (int i = EstimWindows; i < NbDate; i++)
            {
                if( (i - EstimWindows) % PeriodOfRebalance == 0){
                    //On est a une date de rebalancement donc
                    //Recalcul des poids pour la date voulu
                    weights = weightsComputation(i - EstimWindows);  //v2
                    priceOfCurrentAssets = getPriceOfAssets(i);
                    volumofCurrentAssets = getVolumeOfAssets(weights, priceOfCurrentAssets, CurrentCash);
                }

                //Récupère les prix sur le marché a la date courante
                priceOfCurrentAssets = getPriceOfAssets(i);

                //Calcul de la valeur du pf actuel
                double currentValuePf = getPricePF2(volumofCurrentAssets, i);
                CurrentCash = currentValuePf;

                //On ajoute la valeur du pf pour une periode de rebalancement
                res.Add(currentValuePf);
            }

            return res;
        }
       
        //Fonction permettant de calculer la tracking error
        public double computeTrackingError(double[,] currentReturnBench, double[,] currentReturnAssets)
        {

            double trackingError = 0;
            double[] deltaReturn = new double[currentReturnAssets.GetLength(0)];

            //Si on a 2 vecteurs de taille différentes
            if(currentReturnBench.GetLength(0) == currentReturnAssets.GetLength(0)){
                for (int i = 0; i < currentReturnAssets.GetLength(0); i++)
                {
                    deltaReturn[i] = currentReturnAssets[i,0] -  currentReturnBench[i,0];
                }
                //Calcul de variance
                double moyDelta = 0;

                //Calcul de la moyenne
                for (int i = 0; i < currentReturnAssets.GetLength(0); i++)
                {
                    moyDelta += deltaReturn[i];
                }
                moyDelta = moyDelta / deltaReturn.GetLength(0);

                //Initialisation et calcul de la variance des ecarts de rentabilité
                double variance = 0.0;

                for (int i = 0; i < deltaReturn.GetLength(0); i++)
                {
                    variance += (moyDelta - deltaReturn[i]) * (moyDelta - deltaReturn[i]);
                }

                variance = variance / deltaReturn.GetLength(0);


                trackingError = System.Math.Sqrt(variance);

            }

            return trackingError;
        }



    }//fin class
}//fin namespace
