using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestion_Indicielle.ViewModels
{
    class ComponentInfo
    {
        
        public string Tickers { get; set; }
        public bool IsSelected { get; set; }
        public ComponentInfo(string tickers, bool isSelected)
        {
            Tickers = tickers;
            IsSelected = isSelected;
        }

       

    }
}
