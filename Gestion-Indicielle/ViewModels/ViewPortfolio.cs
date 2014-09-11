using LibrarySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestion_Indicielle.ViewModels
{
    class ViewPortfolio
    {
        private ArrayList al;
        private List<Object> _result;
        public List<Object> result{get; set;} 
        
        public ViewPortfolio()
        {
            DataRetriever dr = new DataRetriever();
            al = dr.getTickers();
            result = new List<Object>();
            foreach (var v in al)
            {
                result.Add(new { Name = v, IsInPortfolio = false, Weight = 0 });
            }
        }


    }
}
