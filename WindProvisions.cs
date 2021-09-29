using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCE7_10Library
{
    /// <summary>
    /// Enum for the ASCE7-10 Exposure Categories
    /// </summary>
    public enum WindStructureClassifications
    { 
        WIND_ENCLOSED = 0,
        WIND_PARTIALLY_ENCLOSED = 1,
        WIND_OPEN = 2
    }
    /// <summary>
    /// Enum for the ASCE7-10 Exposure Categories
    /// </summary>
    public enum ExposureCategories
    {
        B = 0,
        C = 1,
        D = 2
    }

    /// <summary>
    /// Enum for the ASCE7-10 Risk Categories
    /// </summary>
    public enum RiskCategories
    {
        I = 0,
        II = 1,
        III = 2,
        IV = 3
    }

    /// <summary>
    /// Class to hold basic building information
    /// </summary>
    public class BuildingInfo
    {
        public double B { get; set; }    // length normal to wind
        public double L { get; set; }    // length parallel to wind
        public double H { get; set; }    // mean roof height

        RiskCategories RiskCat = RiskCategories.II;

        double theta = 25; // roof slope

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
            theta = angle;
        }
    }
    public class WindProvisions
    {
        private double kz_alpha = 7;
        private double kz_zg = 1200;

        private double Kzt = 1.0;
        private double Kd = 1.0;
        private double GustFactor = 0.85; // 3 second gust factor

        public ExposureCategories Exposure { get; set; } = ExposureCategories.B;
        public BuildingInfo Building;
        public Double Speed;

        public double Q_H { get; set; } = 0.0;
        public double Q_0 { get; set; } = 0.0;
        public double Q_15 { get; set; } = 0.0;

        public WindProvisions(double speed, BuildingInfo bldg, ExposureCategories exp = ExposureCategories.B)
        {
            Speed = speed;
            Building = bldg;
            Exposure = exp;

            string status_msg = "";
            
            /// windward pressure points
            // Q0
            Q_0 = ASCE7_10_Compute_Q(0.0, out status_msg);
            Console.WriteLine(status_msg);
            //Q15
            Q_15 = ASCE7_10_Compute_Q(15.0, out status_msg);
            Console.WriteLine(status_msg);
            // Qh
            Q_H = ASCE7_10_Compute_Q_H(out status_msg);
            Console.WriteLine(status_msg);

        }

        /// <summary>
        /// Compute the Kz coefficient per ASCE7-10
        /// </summary>
        /// <param name="elev"></param>
        /// <returns></returns>
        protected double ComputeKz(double elev, out string msg)
        {
            double kz = (elev <= 15) ? (2.01 * Math.Pow(15.0 / kz_zg, 2.0 / kz_alpha)) : (2.01 * Math.Pow(elev / kz_zg, 2.0 / kz_alpha));
            msg = "- Kz: " + kz.ToString();

            return kz;
        }

        protected double ASCE7_10_Compute_Q_H(out string msg)
        {
            string msg2 = "";
            double qh = ASCE7_10_Compute_Q(Building.H, out msg2);
            msg = msg2 + "\n";
            msg += "Q at H=" + Q_H.ToString() + "\n";
            return qh;
        }


        protected double ASCE7_10_Compute_Q(double elev, out string msg)
        {
            string msg2 = "";
            ASCE7_10_Wind_ComputeAlphaZG();
            msg = "Kz params -- alpha: " + kz_alpha.ToString() + "   zg: " + kz_zg.ToString() + "\n";
            double q = 0.00256 * Speed * Speed * Kzt * Kd * ComputeKz(elev, out msg2);
            msg += msg2 + "\n";
            msg += "Q @ " + elev.ToString() + " = " + q.ToString() + "\n";
            return q;
        }

        /// <summary>
        /// sets the KZ parameter coefficients from Table 26.9-1 and 29.3-1
        /// </summary>
        protected void ASCE7_10_Wind_ComputeAlphaZG()
        {
            switch (this.Exposure)
            {
                case ExposureCategories.B:
                    {
                        kz_alpha = 7;
                        kz_zg = 1200;
                        break;
                    }
                case ExposureCategories.C:
                    {
                        kz_alpha = 9.5;
                        kz_zg = 900;
                        break;
                    }
                case ExposureCategories.D:
                    {
                        kz_alpha = 11.5;
                        kz_zg = 700;
                        break;
                    }
                default:
                    throw new InvalidOperationException(this.Exposure + " is not a valid exposure");
            }
        }
    }
}
