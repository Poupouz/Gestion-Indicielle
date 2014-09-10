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


        public ArrayList getTickers()
        {
            using (DataRetrieverDataContext dc = new DataRetrieverDataContext())
            {
                var req = (from lines in dc.HistoComponents select lines.name).Distinct().ToArray();
                return new ArrayList(req);
            }
        }

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

        public int nbDate()
        {
            ArrayList dates = new ArrayList();
            System.Data.Linq.Table<HistoComponents> table = DataRetrieverDataContext.HistoComponents;

            foreach (HistoComponents element in table)
            {
                if (!dates.Contains(element.date))
                {
                    dates.Add(element.date);
                }
            }

            return dates.Count;
        }

    }
}
