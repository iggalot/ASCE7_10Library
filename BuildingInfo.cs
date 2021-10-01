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

        public double[] RoofZonePts { get; set; }



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

            RoofZonePts = GetFlatRoofPressureZonePoints(0, 0);
        }

        /// <summary>
        /// Get the zone boundary locations for the roof measure relative to a reference point (usually 0,0)
        /// </summary>
        /// <param name="x_ref">x-coord reference location (usually the top of the windward wall</param>
        /// <param name="y_ref">y-coord reference location (usually the top of the windward wall</param>
        /// <returns></returns>
        private double[] GetFlatRoofPressureZonePoints(double x_ref, double y_ref)
        {
            // Z1
            double x_roof_z1_start = x_ref;
            double x_roof_z2_start = x_ref;
            double x_roof_z3_start = x_ref;
            double x_roof_z4_start = x_ref;

            double x_roof_z1_end;
            double x_roof_z2_end;
            double x_roof_z3_end;
            double x_roof_z4_end;

            if (L < H * 0.5)
            {
                x_roof_z1_end = x_ref + L;
                x_roof_z2_start = x_ref + L;
                x_roof_z2_end = x_ref + L;
                x_roof_z3_start = x_ref +L;
                x_roof_z3_end = x_ref + L;
                x_roof_z4_start = x_ref +L;
                x_roof_z4_end = x_ref + L;
            }
            else if (L < H)
            {
                x_roof_z1_end = x_ref + H * 0.5;
                x_roof_z2_start = x_ref + H * 0.5;
                x_roof_z2_end = x_ref + L;
                x_roof_z3_start = x_ref + L;
                x_roof_z3_end = x_ref + L;
                x_roof_z4_start = x_ref + L;
                x_roof_z4_end = x_ref + L;
            }
            else if (L < H * 2.0)
            {
                x_roof_z1_end = x_ref + H * 0.5;
                x_roof_z2_start = x_ref + H * 0.5;
                x_roof_z2_end = x_ref + H;
                x_roof_z3_start = x_ref + H;
                x_roof_z3_end = x_ref + L;
                x_roof_z4_start = x_ref +L;
                x_roof_z4_end = x_ref + L;
            }
            else
            {
                x_roof_z1_end = x_ref + H * 0.5;
                x_roof_z2_start = x_ref + H * 0.5;
                x_roof_z2_end = x_ref + H;
                x_roof_z3_start = x_ref + H;
                x_roof_z3_end = x_ref + H * 2.0;
                x_roof_z4_start = x_ref + H * 2.0;
                x_roof_z4_end = x_ref + L;
            }

            return new double[] { x_roof_z1_start, x_roof_z1_end, x_roof_z2_start, x_roof_z2_end, x_roof_z3_start, x_roof_z3_end, x_roof_z4_start, x_roof_z4_end };
        }

    }
}
