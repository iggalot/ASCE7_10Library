using ASCE7_10Library;
using System.Windows;

namespace ASCE7_10ConsoleDriver
{


    public class Program
    { 
        static void Main(string[] args)
        {
            ExposureCategories exp = ExposureCategories.B;
            double V = 115;   // mph

            double theta = 25; // roof slope

            // Create a building object
            BuildingInfo bldg = new BuildingInfo(85, 48, 35, 25, RiskCategories.II);
            WindProvisions wind_prov = new WindProvisions(V, bldg, exp);


            MessageBox.Show(wind_prov.Q_H.ToString() + " " + bldg.GetL_over_B().ToString() + " : " + bldg.GetH_over_L().ToString());
        }
    }
}
