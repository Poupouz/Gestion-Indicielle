using LibrarySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestion_Indicielle.ViewModels
{
    class PortfolioViewModel
    {
        public List<ComponentInfo> ComponentInfoList { get; private set; }
        private ArrayList al;

        public PortfolioViewModel()
        {
            ComponentInfoList = new List<ComponentInfo>();
            DataRetriever dr = new DataRetriever();
            al = dr.getTickers();
            foreach (var v in al)
            {
                ComponentInfoList.Add(new ComponentInfo((string) v,false));
            }
            
        }

    }
}
