using System;
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

        

    }
}
