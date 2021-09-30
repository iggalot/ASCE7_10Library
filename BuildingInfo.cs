using System;

namespace ASCE7_10Library
{
    /// <summary>
    /// Class to hold basic building information
    /// </summary>
    public class BuildingInfo
    {
        public double B { get; set; }    // length normal to wind
        public double L { get; set; }    // length parallel to wind
        public double H { get; set; }    // mean roof height

        RiskCategories RiskCat = RiskCategories.II;

        public double RoofSlope = 25; // roof slope

        public double GetL_over_B()
        {
            return L / B;
        }

        public double GetH_over_L()
        {
            return H / L;
        }

        public BuildingInfo(double b, double l, double h, double angle, RiskCategories cat = RiskCategories.II)
        {
            string status_msg = "";
            bool validInput = true;
            if (b <= 0)
            {
                status_msg += "B cannot be less than zero! " + b.ToString() + "\n";
                validInput = false;
            }

            if (l <= 0)
            {
                status_msg += "L cannot be less than zero! " + l.ToString() + "\n";
                validInput = false;
            }

            if (h <= 0)
            {
                status_msg += "H cannot be less than zero! " + h.ToString() + "\n";
                validInput = false;
            }

            if (!validInput)
                throw new InvalidOperationException(status_msg);


            // Otherwise create the object
            B = b;
            L = l;
            H = h;
            RoofSlope = angle;
        }
    }
}
