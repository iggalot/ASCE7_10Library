using System;
using System.Numerics;

namespace ASCE7_10Library
{
    /// <summary>
    /// Class to hold basic building information for flat roof structures
    /// </summary>
    public class BuildingInfo
    {
        public double B { get; set; }    // length normal to wind
        public double L { get; set; }    // length parallel to wind
        public double H { get; set; }    // mean roof height

        RiskCategories RiskCat = RiskCategories.II;

        public double RoofSlope = 25; // roof slope

        public virtual double[] RoofProfile {get; set;}

        public double GetL_over_B()
        {
            return L / B;
        }

        public double GetH_over_L()
        {
            return H / L;
        }

        public Vector4 WW_GRD_1 { get; set; }
        public Vector4 WW_15_1 { get; set; }
        public Vector4 WW_H_1 { get; set; }
        public Vector4 ORIGIN { get; set; } = new Vector4();
        public Vector4 RIDGE_1 { get; set; } = new Vector4();

        public Vector4 LW_H_1 { get; set; }
        public Vector4 LW_15_1 { get; set; }
        public Vector4 LW_GRD_1 { get; set; }


        public double[] RoofZonePts { get; set; }


        /// <summary>
        /// Flat roof building constructor
        /// </summary>
        /// <param name="b"></param>
        /// <param name="l"></param>
        /// <param name="h"></param>
        /// <param name="angle"></param>
        /// <param name="cat"></param>

        public BuildingInfo(double b, double l, double h, double[] roof_profile, RiskCategories cat = RiskCategories.II)
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




            // Find the mean height of the roof (the max of 0, 2, 4, ... elements of the roof profile

            RiskCat = cat;

            // Vectors for the points of the structure profile based on the provided dimensions
            // 0,0 is assumed to be lower left for windward wall
            int count = roof_profile.Length;
            WW_GRD_1 = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            WW_15_1 = new Vector4(0.0f, 15.0f, 0.0f, 1.0f);
            WW_H_1 = new Vector4((float)roof_profile[0], (float)roof_profile[1], 0.0f, 1.0f);
            LW_H_1 = new Vector4((float)roof_profile[count-2], (float)roof_profile[count-1], 0.0f, 1.0f);
            LW_GRD_1 = new Vector4((float) l, 0.0f, 0.0f, 1.0f);
            LW_15_1 = new Vector4((float)l, 15.0f, 0.0f, 1.0f);

            // Set the origin of the model to be the midpoint at ground level
            ORIGIN = new Vector4((float)0.5 * (WW_GRD_1.X + LW_GRD_1.X), (float)0.5 * (WW_GRD_1.Y + LW_GRD_1.Y), (float)0.5 * (WW_GRD_1.Z + LW_GRD_1.Z), 1.0f);

            RoofZonePts = GetFlatRoofPressureZonePoints(0, 0);

            // The flat roof profile points for the roof
            RoofProfile = roof_profile;
            RoofSlope = Math.Atan((RoofProfile[3]-RoofProfile[1]) / (RoofProfile[2] - RoofProfile[0]));

            // Otherwise create the object
            B = b;
            L = l;

            // Determine our mean roof height -- using the y-coords (1, 3, 5, 7, ...) of roof_profile
            double max_ht = 0;
            for (int i = 1; i < roof_profile.Length; i=i+2)
            {
                if(roof_profile[i] > max_ht)
                {
                    max_ht = roof_profile[i];
                    RIDGE_1 = new Vector4((float)roof_profile[i - 1], (float)roof_profile[i], 0.0f, 1.0f);
                }
            }


            // Take the average of the lesser of the windward wall height and the leeward wall height -- needed in case of unequal wall heights.
            H = 0.5 * (max_ht + Math.Min(roof_profile[1], roof_profile[roof_profile.Length -1]));
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
