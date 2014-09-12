using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySQL
{
    public class DataRetriever
    {
        private DataRetrieverDataContext _dataRetrieverDataContext;

        public DataRetrieverDataContext DataRetrieverDataContext
        {
            get { return _dataRetrieverDataContext; }
            set { _dataRetrieverDataContext = value; }
        }

        public DataRetriever()
        {
            this.DataRetrieverDataContext = new DataRetrieverDataContext();
        }

        //Méthode permettant d'extraire les tickers distincts
        public ArrayList getTickers()
        {
            using (DataRetrieverDataContext dc = new DataRetrieverDataContext())
            {
                var req = (from lines in dc.HistoComponents select lines.name).Distinct().ToArray();
                return new ArrayList(req);
            }
        }

        //Méthode permettant d'extraire les prix de l'action d'une entreprise sur une période donnée 
        public double[] getData(System.DateTime date , String name_Entrprise , int period)
        {
            using (DataRetrieverDataContext dc = new DataRetrieverDataContext()){
                double[] resultat = new double[period];
                var req = (from lines in dc.HistoComponents where lines.date >= date && lines.name.Equals(name_Entrprise)  select lines.value).ToArray();
                for (int i = 0; i < period; i++)
                {
                    resultat[i] = req[i];
                }
                return resultat;
            }
        }

        //Méthode permettant d'extraire les dates distinctes
        public int nbDate()
        {
            using (DataRetrieverDataContext dc = new DataRetrieverDataContext())
            {
                var req = (from lines in dc.HistoComponents select lines.date).Distinct().ToArray();
                return req.GetLength(0);
                
            }
        }

        //Méthode permettant d'extraire les valeurs du benchmark
        public double[,] getDataBenchmark(System.DateTime date, int period)
        {
            using (DataRetrieverDataContext dc = new DataRetrieverDataContext())
            {
                double[,] resultat = new double[period,1];
                var req = (from lines in dc.HistoIndices where lines.date >= date select lines.value).ToArray();
                for (int i = 0; i < period; i++)
                {
                    resultat[i,0] = req[i]; 
                }
                return resultat;
            }
        }

        public double[] extractColumnIndex(double[,] table, int index)
        {
            double[] ret=new double[table.GetLength(0)];
            
            Parallel.For(0, table.GetLength(0), i => ret[i] = table[i, index]);
            return ret;
        }

        public double[] extractRowIndex(double[,] table, int index)
        {
            double[] ret = new double[table.GetLength(1)];
            Parallel.For(0, table.GetLength(1), i => ret[i] = table[index, i]);
            return ret;
        }
    }
}
