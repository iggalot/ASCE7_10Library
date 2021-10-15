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

        public double Q_H_WW { get; set; } = 0.0;
        public double Q_0_WW { get; set; } = 0.0;
        public double Q_15_WW { get; set; } = 0.0;

        // Windward wall pressures
        public double P_H_WW { get; set; } = 0.0;
        public double P_15_WW { get; set; } = 0.0;
        public double P_0_WW { get; set; } = 0.0;

        // Leeward wall pressures
        public double P_H_LW { get; set; } = 0.0;

        // Sidewall pressures
        public double P_H_SW { get; set; } = 0.0;

        // Roof pressures Normal to Ridge - CASE A (dowward) and CASE B (upward)
        // for 0 to h/2, h/2 to h, h to 2h, > h
        public double[] P_H_ROOF_WW_NORMAL_CASEA { get; set; } = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
        public double[] P_H_ROOF_WW_NORMAL_CASEB { get; set; } = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
        public double P_H_ROOF_LW_NORMAL { get; set; } = 0.0;



        // Roof pressures PArallel to Ridge - CASE A (dowward) and CASE B (upward)
        // for 0 to h/2, h/2 to h, h to 2h, > h
        public double[] P_H_ROOF_WW_PARALLEL_CASEA { get; set; } = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
        public double[] P_H_ROOF_WW_PARALLEL_CASEB { get; set; } = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
        public double P_H_ROOF_LW_PARALLEL { get; set; } = 0.0;



        public double CP_WW { get; set; } = -0.8;
        public double CP_SW { get; set; } = -0.7;
        public double CP_LW { get; set; } = -0.5;

        // CP_ROOF coefficiencts --- 4x CaseA, 4x CaseB, LW
        public double[] CP_ROOF_NORMALTORIDGE { get; set; } = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public double[] CP_ROOF_PARALLELTORIDGE { get; set; } = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        /// <summary>
        /// Returns the Case A negative WW roof pressure plus the LW value
        /// </summary>
        public double[] CP_ROOF_NORMALTORIDGE_CaseA { 
            get
            {
                return new double[] { CP_ROOF_NORMALTORIDGE[0], CP_ROOF_NORMALTORIDGE[2], CP_ROOF_NORMALTORIDGE[4], CP_ROOF_NORMALTORIDGE[6], CP_ROOF_NORMALTORIDGE[8] };
            }
        }

        /// <summary>
        /// Returns the Case B positive WW roof pressure plus the LW value
        /// </summary>
        public double[] CP_ROOF_NORMALTORIDGE_CaseB
        {
            get
            {
                return new double[] { CP_ROOF_NORMALTORIDGE[1], CP_ROOF_NORMALTORIDGE[3], CP_ROOF_NORMALTORIDGE[5], CP_ROOF_NORMALTORIDGE[7], CP_ROOF_NORMALTORIDGE[8]};
            }
        }

        public double [] CP_ROOF_PARALLELTORIDGE_CaseA
        {
            get
            {
                return new double[] { CP_ROOF_PARALLELTORIDGE[0], CP_ROOF_PARALLELTORIDGE[2], CP_ROOF_PARALLELTORIDGE[4], CP_ROOF_PARALLELTORIDGE[6], CP_ROOF_PARALLELTORIDGE[8] };

            }
        }

        public double[] CP_ROOF_PARALLELTORIDGE_CaseB
        {
            get
            {
                return new double[] { CP_ROOF_PARALLELTORIDGE[1], CP_ROOF_PARALLELTORIDGE[3], CP_ROOF_PARALLELTORIDGE[5], CP_ROOF_PARALLELTORIDGE[7], CP_ROOF_PARALLELTORIDGE[8] };
            }
        }


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
            Q_0_WW = ASCE7_10_Compute_Q(0.0, out status_msg);
            Console.WriteLine(status_msg);
            //Q15
            Q_15_WW = ASCE7_10_Compute_Q(15.0, out status_msg);
            Console.WriteLine(status_msg);
            // Qh
            Q_H_WW = ASCE7_10_Compute_Q_H(out status_msg);
            Console.WriteLine(status_msg);

            Console.WriteLine("Dynamic Pressures:    Q_H = " + Q_H_WW.ToString() + "  Q_15 = " + Q_15_WW.ToString() + "  Q_0 = " + Q_0_WW.ToString());

            // Load the CP values
            CP_WW =  0.8;
            CP_SW = -0.7;
            CP_LW = ComputeCP_LW(out status_msg);
            Console.WriteLine("CP_WW = " + CP_WW.ToString() + "  CP_SW = " + CP_SW.ToString() + "  CP_LW = " + status_msg);
            CP_ROOF_NORMALTORIDGE = ComputeCP_Roof(true, Building.RoofSlope, out status_msg);
            CP_ROOF_PARALLELTORIDGE = ComputeCP_Roof(false, Building.RoofSlope, out status_msg);

            Console.WriteLine("Normal to Ridge\n" + displayCPRoofValues(CP_ROOF_NORMALTORIDGE));
            Console.WriteLine("Parallel to Ridge\n" + displayCPRoofValues(CP_ROOF_PARALLELTORIDGE));

            // Compute the WW pressures
            P_H_WW = Q_H_WW * GustFactor * CP_WW;
            P_0_WW = Q_0_WW * GustFactor * CP_WW;
            P_15_WW = Q_15_WW * GustFactor * CP_WW;

            Console.WriteLine("Pressures:    P_H_WW = " + P_H_WW.ToString() + "  P_15_WW = " + P_15_WW.ToString() + "  P_0_WW = " + P_0_WW.ToString());

            // Compute the LW pressure
            P_H_LW = Q_H_WW * GustFactor * CP_LW;

            // Compute the SW pressure
            P_H_SW = Q_H_WW * GustFactor * CP_SW;

            Console.WriteLine("              P_H_LW = " + P_H_LW.ToString() + "  P_H_SW = " + P_H_SW.ToString());

            // Compute the Roof pressures
            // Normal to Ridge Case A
            P_H_ROOF_WW_NORMAL_CASEA[0] = Q_H_WW * GustFactor * CP_ROOF_NORMALTORIDGE_CaseA[0];
            P_H_ROOF_WW_NORMAL_CASEA[1] = Q_H_WW * GustFactor * CP_ROOF_NORMALTORIDGE_CaseA[1];
            P_H_ROOF_WW_NORMAL_CASEA[2] = Q_H_WW * GustFactor * CP_ROOF_NORMALTORIDGE_CaseA[2];
            P_H_ROOF_WW_NORMAL_CASEA[3] = Q_H_WW * GustFactor * CP_ROOF_NORMALTORIDGE_CaseA[3];
            Console.WriteLine("Normal to Ridge Case A\n" + displayPressureValues(P_H_ROOF_WW_NORMAL_CASEA));

            // Normal to Ridge Case B
            P_H_ROOF_WW_NORMAL_CASEB[0] = Q_H_WW * GustFactor * CP_ROOF_NORMALTORIDGE_CaseB[0];
            P_H_ROOF_WW_NORMAL_CASEB[1] = Q_H_WW * GustFactor * CP_ROOF_NORMALTORIDGE_CaseB[1];
            P_H_ROOF_WW_NORMAL_CASEB[2] = Q_H_WW * GustFactor * CP_ROOF_NORMALTORIDGE_CaseB[2];
            P_H_ROOF_WW_NORMAL_CASEB[3] = Q_H_WW * GustFactor * CP_ROOF_NORMALTORIDGE_CaseB[3];
            Console.WriteLine("Normal to Ridge Case B\n" + displayPressureValues(P_H_ROOF_WW_NORMAL_CASEB));

            // Leeward Roof Values
            P_H_ROOF_LW_NORMAL = GustFactor * CP_ROOF_NORMALTORIDGE_CaseA[4];




            //Parallel to Ridge Case A
            P_H_ROOF_WW_PARALLEL_CASEA[0] = Q_H_WW * GustFactor * CP_ROOF_PARALLELTORIDGE_CaseA[0];
            P_H_ROOF_WW_PARALLEL_CASEA[1] = Q_H_WW * GustFactor * CP_ROOF_PARALLELTORIDGE_CaseA[1];
            P_H_ROOF_WW_PARALLEL_CASEA[2] = Q_H_WW * GustFactor * CP_ROOF_PARALLELTORIDGE_CaseA[2];
            P_H_ROOF_WW_PARALLEL_CASEA[3] = Q_H_WW * GustFactor * CP_ROOF_PARALLELTORIDGE_CaseA[3];
            Console.WriteLine("Parallel to Ridge Case A\n" + displayPressureValues(P_H_ROOF_WW_PARALLEL_CASEA));

            // Parallel to Ridge Case B
            P_H_ROOF_WW_PARALLEL_CASEB[0] = Q_H_WW * GustFactor * CP_ROOF_PARALLELTORIDGE_CaseB[0];
            P_H_ROOF_WW_PARALLEL_CASEB[1] = Q_H_WW * GustFactor * CP_ROOF_PARALLELTORIDGE_CaseB[1];
            P_H_ROOF_WW_PARALLEL_CASEB[2] = Q_H_WW * GustFactor * CP_ROOF_PARALLELTORIDGE_CaseB[2];
            P_H_ROOF_WW_PARALLEL_CASEB[3] = Q_H_WW * GustFactor * CP_ROOF_PARALLELTORIDGE_CaseB[3];
            Console.WriteLine("Parallel to Ridge Case B\n" + displayPressureValues(P_H_ROOF_WW_PARALLEL_CASEB));

            // Leeward Roof Values
            P_H_ROOF_LW_PARALLEL = GustFactor * CP_ROOF_PARALLELTORIDGE_CaseA[4];
        }

        /// <summary>
        /// Determines the roof CP value cases from Figure 27.4-1
        /// </summary>
        /// <param name="windIsNormalToRidge"></param>
        /// <param name="theta"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected double[] ComputeCP_Roof(bool windIsNormalToRidge, double theta, out string msg)
        {
            double[] cp_values;

            if((windIsNormalToRidge == true && theta < 10) || (windIsNormalToRidge == false))
            {
                if (Building.GetH_over_L() <= 0.5)
                {
                    //                           A       B     A     B     A      B     A      B    LW
                    cp_values = new double[] { -0.9, -0.18, -0.9, -0.18, -0.5, -0.18, -0.3, -0.18, 0.0 };
                }
                else if (Building.GetH_over_L() >= 1.0)
                {
                    //                           A       B     A     B     A      B     A      B    LW
                    cp_values = new double[] { -1.3, -0.18, -0.7, -0.18, -0.7, -0.18, -0.7, -0.18, 0.0 };

                    // TODO:  Still needs to do area reduction factors
                }
                else
                {
                    double interp1 = -0.9 + (Building.GetH_over_L() - 0.5) * (-1.3 - (-0.9)) / (1.0 - 0.5);
                    double interp2 = -0.9 + (Building.GetH_over_L() - 0.5) * (-0.7 - (-0.9)) / (1.0 - 0.5);
                    double interp3 = -0.5 + (Building.GetH_over_L() - 0.5) * (-0.7 - (-0.5)) / (1.0 - 0.5);
                    double interp4 = -0.3 + (Building.GetH_over_L() - 0.5) * (-0.7 - (-0.3)) / (1.0 - 0.5);

                    //                           A       B     A     B     A      B     A      B    LW
                    cp_values = new double[] { interp1, -0.18, interp2, -0.18, interp3, -0.18, interp4, -0.18, 0.0 };
                }

            } else
            {
                // h/L <= 0.25 (middle row of Figure 27.4-1 - Normal to  Ridge for theta >= 10 deg.

                if (Building.GetH_over_L() <= 0.25)
                {
                    double lw_val = 0.0;
                    if (Building.RoofSlope < 10)
                        lw_val = -0.3;
                    else if (Building.RoofSlope < 15)
                        lw_val = -0.5;
                    else
                        lw_val = -0.6;

                    //                           A       B     A     B     A      B     A      B    LW
                    if (Building.RoofSlope < 10)
                        cp_values = new double[] { -0.7, -0.18, -0.7, -0.18, -0.7, -0.18, -0.7, -0.18, lw_val };
                    else if (Building.RoofSlope < 15)
                        cp_values = new double[] { -0.5, 0.0, -0.5, 0.0, -0.5, 0.0, -0.5, 0.0, lw_val }; 
                    else if (Building.RoofSlope < 20)
                        cp_values = new double[] { -0.3, 0.2, -0.3, 0.2, -0.3, 0.2, -0.3, 0.2, lw_val };
                    else if (Building.RoofSlope < 25)
                        cp_values = new double[] { -0.2, 0.3, -0.2, 0.3, -0.2, 0.3, -0.2, 0.3, lw_val };
                    else if (Building.RoofSlope < 30)
                        cp_values = new double[] { -0.2, 0.3, -0.2, 0.3, -0.2, 0.3, -0.2, 0.3, lw_val };
                    else if (Building.RoofSlope < 35)
                        cp_values = new double[] { 0.0, 0.4, 0.0, 0.4, 0.0, 0.4, 0.0, 0.4, lw_val };
                    else if (Building.RoofSlope < 45)
                        cp_values = new double[] { 0.0, 0.4, 0.0, 0.4, 0.0, 0.4, 0.0, 0.4, lw_val }; 
                    else 
                        cp_values = new double[] { 0.0, 0.01 * Building.RoofSlope, 0.0, 0.01 * Building.RoofSlope, 0.0, 0.01 * Building.RoofSlope, 0.0, 0.01 * Building.RoofSlope, lw_val };
                } 
                // h/L = 0.5 (middle row of Figure 27.4-1 - Normal to  Ridge for theta >= 10 deg.
                else if (Building.GetH_over_L() < 1.0)
                {
                    double lw_val = 0.0;
                    if (Building.RoofSlope < 10)
                        lw_val = -0.5;
                    else if (Building.RoofSlope < 15)
                        lw_val = -0.5;
                    else
                        lw_val = -0.6;

                    //                               A       B     A     B     A      B     A      B    LW
                    if (Building.RoofSlope < 10)
                        cp_values = new double[] { -0.9, -0.18, -0.9, -0.18, -0.9, -0.18, -0.9, -0.18, lw_val };
                    else if (Building.RoofSlope < 15)
                        cp_values = new double[] { -0.7, -0.18, -0.7, -0.18, -0.7, -0.18, -0.7, -0.18, lw_val };
                    else if (Building.RoofSlope < 20)
                        cp_values = new double[] { -0.4, 0.0, -0.4, 0.0, -0.4, 0.0, -0.4, 0.0, lw_val };
                    else if (Building.RoofSlope < 25)
                        cp_values = new double[] { -0.3, 0.2, -0.3, 0.2, -0.3, 0.2, -0.3, 0.2, lw_val };
                    else if (Building.RoofSlope < 30)
                        cp_values = new double[] { -0.2, 0.2, -0.2, 0.2, -0.2, 0.2, -0.2, 0.2, lw_val };
                    else if (Building.RoofSlope < 35)
                        cp_values = new double[] { -0.2, 0.3, -0.2, 0.3, -0.2, 0.3, -0.2, 0.3, lw_val };
                    else if (Building.RoofSlope < 45)
                        cp_values = new double[] { 0.0, 0.4, 0.0, 0.4, 0.0, 0.4, 0.0, 0.4, lw_val };
                    else
                        cp_values = new double[] { 0.0, 0.01 * Building.RoofSlope, 0.0, 0.01 * Building.RoofSlope, 0.0, 0.01 * Building.RoofSlope, 0.0, 0.01 * Building.RoofSlope, lw_val };
                }
                else
                {
                    double lw_val = 0.0;
                    if (Building.RoofSlope < 10)
                        lw_val = -0.7;
                    else if (Building.RoofSlope < 15)
                        lw_val = -0.6;
                    else
                        lw_val = -0.6;

                    if (Building.RoofSlope < 10)
                        cp_values = new double[] { -1.3, -0.18, -1.3, -0.18, -1.3, -0.18, -1.3, -0.18, lw_val };
                    else if (Building.RoofSlope < 15)
                        cp_values = new double[] { -1.0, -0.18, -1.0, -0.18, -1.0, -0.18, -1.0, -0.18, lw_val };
                    else if (Building.RoofSlope < 20)
                        cp_values = new double[] { -0.7, -0.18, -0.7, -0.18, -0.7, -0.18, -0.7, -0.18, lw_val };
                    else if (Building.RoofSlope < 25)
                        cp_values = new double[] { -0.5, 0.0, -0.5, 0.0, -0.5, 0.0, -0.5, 0.0, lw_val };
                    else if (Building.RoofSlope < 30)
                        cp_values = new double[] { -0.3, 0.2, -0.3, 0.2, -0.3, 0.2, -0.3, 0.2, lw_val };
                    else if (Building.RoofSlope < 35)
                        cp_values = new double[] { -0.2, 0.2, -0.2, 0.2, -0.2, 0.2, -0.2, 0.2, lw_val };
                    else if (Building.RoofSlope < 45)
                        cp_values = new double[] { 0.0, 0.3, 0.0, 0.3, 0.0, 0.3, 0.0, 0.3, lw_val };
                    else
                        cp_values = new double[] { 0.0, 0.01 * Building.RoofSlope, 0.0, 0.01 * Building.RoofSlope, 0.0, 0.01 * Building.RoofSlope, 0.0, 0.01 * Building.RoofSlope, lw_val };
                }
            }
            msg = displayCPRoofValues(cp_values);
            return cp_values;
        }

        private string displayCPRoofValues(double[] arr)
        {
            string msg = "Cp Roof Values " + "  h/L: " + Building.GetH_over_L().ToString() + "  theta: " + Building.RoofSlope.ToString() + "\n";
            msg += " WW ROOF:\n";
            msg += "             CASE A         CASE B\n";
            msg += "0 to h/2     " + arr[0] + "      " + arr[1] + "\n";
            msg += "h/2 to h     " + arr[2] + "      " + arr[3] + "\n";
            msg += "h to 2h      " + arr[4] + "      " + arr[5] + "\n";
            msg += "> 2h         " + arr[6] + "      " + arr[7] + "\n";
            msg += "---------------------------------------\n";
            msg += " LW ROOF:    CP_ROOF_LW: " + arr[8].ToString();
            return msg;
        }

        private string displayPressureValues(double[] arr)
        {
            string msg = "Roof Pressure Values: " + "  h/L: " + Building.GetH_over_L().ToString() + "  theta: " + Building.RoofSlope.ToString() + "\n";
            msg += "0 to h/2     " + arr[0] + "\n";
            msg += "h/2 to h     " + arr[1] + "\n";
            msg += "h to 2h      " + arr[2] + "\n";
            msg += "> 2h         " + arr[3] + "\n";
            msg += "---------------------------------------\n";
            return msg;
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
            msg += "Q at H=" + Q_H_WW.ToString() + "\n";
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
