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

        public static Hashtable hashtableTickers = new Hashtable();
        public static Hashtable hashtableCompanies = new Hashtable();

        public PortfolioViewModel()
        {
            initHashtableTickers();
            initHashtableCompanies();
            ComponentInfoList = new List<ComponentInfo>();
            DataRetriever dr = new DataRetriever();
            al = dr.getTickers();
            foreach (var v in al)
            {
                ComponentInfoList.Add(new ComponentInfo((string) hashtableTickers[v], true));
            }
            
        }

        private static void initHashtableTickers()
        {
            hashtableTickers.Add("^FCHI", "CAC 40");
            hashtableTickers.Add("AC.PA", "Accor SA");
            hashtableTickers.Add("ACA.PA", "Crédit Agricole");
            hashtableTickers.Add("AI.PA", "Air Liquide SA");
            hashtableTickers.Add("ALO.PA", "Alstom SA");
            hashtableTickers.Add("BN.PA", "Danone");
            hashtableTickers.Add("BNP.PA", "BNP Paribas");
            hashtableTickers.Add("CA.PA", "Carrefour");
            hashtableTickers.Add("CAP.PA", "Capgemini");
            hashtableTickers.Add("CS.PA", "AXA SA");
            hashtableTickers.Add("EAD.PA", "European Aeronautic Defence and Space NV");
            hashtableTickers.Add("EDF.PA", "Electricité de France S.A.");
            hashtableTickers.Add("EI.PA", "Essilor International SA");
            hashtableTickers.Add("EN.PA", "Bouygues");
            hashtableTickers.Add("GLE.PA", "Société Générale");
            hashtableTickers.Add("GSZ.PA", "GDF Suez");
            hashtableTickers.Add("GTO.PA", "Gemalto");
            hashtableTickers.Add("LG.PA", "Lafarge SA");
            hashtableTickers.Add("MC.PA", "LVMH Moet Hennessy Louis Vuitton SA");
            hashtableTickers.Add("ML.PA", "Michelin");
            hashtableTickers.Add("OR.PA", "L'Oreal SA");
            hashtableTickers.Add("PUB.PA", "Publicis Groupe SA");
            hashtableTickers.Add("RI.PA", "Pernod Ricard NV");
            hashtableTickers.Add("RNO.PA", "Renault SA");
            hashtableTickers.Add("SAF.PA", "Safran SA");
            hashtableTickers.Add("SGO.PA", "Compagnie de Saint-Gobain");
            hashtableTickers.Add("STM.PA", "STMicroelectronics NV");
            hashtableTickers.Add("TEC.PA", "Technip");
            hashtableTickers.Add("VIE.PA", "Veolia Environnement SA");
            hashtableTickers.Add("VIV.PA", "Vivendi SA");
        }

        private static void initHashtableCompanies()
        {
            hashtableCompanies.Add("CAC 40", "^FCHI");
            hashtableCompanies.Add("Accor SA", "AC.PA");
            hashtableCompanies.Add("Crédit Agricole", "ACA.PA");
            hashtableCompanies.Add("Air Liquide SA", "AI.PA");
            hashtableCompanies.Add("Alstom SA", "ALO.PA");
            hashtableCompanies.Add("Danone", "BN.PA");
            hashtableCompanies.Add("BNP Paribas", "BNP.PA");
            hashtableCompanies.Add("Carrefour", "CA.PA");
            hashtableCompanies.Add("Capgemini", "CAP.PA");
            hashtableCompanies.Add("AXA SA", "CS.PA");
            hashtableCompanies.Add("European Aeronautic Defence and Space NV", "EAD.PA");
            hashtableCompanies.Add("Electricité de France S.A.", "EDF.PA");
            hashtableCompanies.Add("Essilor International SA", "EI.PA");
            hashtableCompanies.Add("Bouygues", "EN.PA");
            hashtableCompanies.Add("Société Générale", "GLE.PA");
            hashtableCompanies.Add("GDF Suez", "GSZ.PA");
            hashtableCompanies.Add("Gemalto", "GTO.PA");
            hashtableCompanies.Add("Lafarge SA", "LG.PA");
            hashtableCompanies.Add("LVMH Moet Hennessy Louis Vuitton SA", "MC.PA");
            hashtableCompanies.Add("Michelin", "ML.PA");
            hashtableCompanies.Add("L'Oreal SA", "OR.PA");
            hashtableCompanies.Add("Publicis Groupe SA", "PUB.PA");
            hashtableCompanies.Add("Pernod Ricard NV", "RI.PA");
            hashtableCompanies.Add("Renault SA", "RNO.PA");
            hashtableCompanies.Add("Safran SA", "SAF.PA");
            hashtableCompanies.Add("Compagnie de Saint-Gobain", "SGO.PA");
            hashtableCompanies.Add("STMicroelectronics NV", "STM.PA");
            hashtableCompanies.Add("Technip", "TEC.PA");
            hashtableCompanies.Add("Veolia Environnement SA", "VIE.PA");
            hashtableCompanies.Add("Vivendi SA", "VIV.PA");
        }
    }
}
