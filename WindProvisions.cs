using System;

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

        // Windward wall pressures
        public double P_H_WW { get; set; } = 0.0;
        public double P_15_WW { get; set; } = 0.0;
        public double P_0_WW { get; set; } = 0.0;

        // Leeward wall pressures
        public double P_H_LW { get; set; } = 0.0;

        // Sidewall pressures
        public double P_H_SW { get; set; } = 0.0;


        // Roof pressures


        public double CP_WW { get; set; } = -0.8;
        public double CP_SW { get; set; } = -0.7;
        public double CP_LW { get; set; } = -0.5;

        public double[] CP_WW_Roof { get; set; } = new double[] { 1.3, -0.9 };
        public double[] CP_LW_Roof { get; set; } = new double[] { 1.3, -0.9 };
        public double CP_Parapet { get; set; } = 2.5;

        public WindProvisions(double speed, BuildingInfo bldg, ExposureCategories exp = ExposureCategories.B, double gust=0.85)
        {
            Speed = speed;
            Building = bldg;
            Exposure = exp;
            GustFactor = gust;

            Console.WriteLine("Speed: " + Speed.ToString() + " MPH" + "    Exposure: " + Exposure.ToString());
            Console.WriteLine("GustFactor: " + GustFactor.ToString() + "   L/B: " + bldg.GetL_over_B().ToString() + "    h/L: " + bldg.GetH_over_L().ToString());

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

            Console.WriteLine("Dynamic Pressures:    Q_H = " + Q_H.ToString() + "  Q_15 = " + Q_15.ToString() + "  Q_0 = " + Q_0.ToString());

            // Load the CP values
            CP_WW =  0.8;
            CP_SW = -0.7;
            CP_LW = ComputeCP_LW(out status_msg);
            Console.WriteLine("CP_WW = " + CP_WW.ToString() + "  CP_SW = " + CP_SW.ToString() + "  " + status_msg);

            // Compute the WW pressures
            P_H_WW = Q_H * GustFactor * CP_WW;
            P_0_WW = Q_0 * GustFactor * CP_WW;
            P_15_WW = Q_15 * GustFactor * CP_WW;

            Console.WriteLine("Pressures:    P_H_WW = " + P_H_WW.ToString() + "  P_15_WW = " + P_15_WW.ToString() + "  P_0_WW = " + P_0_WW.ToString());

            // Compute the LW pressure
            P_H_LW = Q_H * GustFactor * CP_LW;

            // Compute the SW pressure
            P_H_SW = Q_H * GustFactor * CP_SW;

            Console.WriteLine("              P_H_LW = " + P_H_LW.ToString() + "  P_H_SW = " + P_H_SW.ToString());


        }

        /// <summary>
        /// Finds the leeward wall coefficient for CP from Figure 27.4.1.  Uses interpolation for L/B values.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected double ComputeCP_LW(out string msg)
        {
            msg = "CP_LW = ";
            double cp = -0.2;

            if (Building.GetL_over_B() <= 1.0)
            {
                msg = "1.0 ";
                cp = -0.5;
            } else if (Building.GetL_over_B() <= 2.0)
            {
                cp = -0.5 + (Building.GetL_over_B() - 1.0) * (0.5 - 0.3) / (2.0 - 1.0);
                msg = cp.ToString();
            } else if (Building.GetL_over_B() < 4.0)
            {
                cp = -0.3 + (Building.GetL_over_B() - 2.0) * (0.3 - 0.2) / (4.0 - 2.0);
                msg = cp.ToString();
            }
            return cp;

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
        /// sets the coefficients for computing KZ from Table 26.9-1 and 29.3-1
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
