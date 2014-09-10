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

        public ArrayList tickers()
        {
            ArrayList names = new ArrayList();
            System.Data.Linq.Table<HistoComponents> table = DataRetrieverDataContext.HistoComponents;

            foreach(HistoComponents element in table){
                if (!names.Contains(element.name))
                {
                    names.Add(element.name);
                }
            }

            return names;
        }

    }
}
