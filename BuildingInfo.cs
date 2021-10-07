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

        public double RoofSlope = 0; // default is flat roof

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
        public Vector4 LW_H_1 { get; set; }
        public Vector4 LW_15_1 { get; set; }
        public Vector4 LW_GRD_1 { get; set; }

        // Contains the coordinate for the ridge point
        public Vector4 RIDGE_1 { get; set; }

        public Vector4[] RoofZonePts { get; set; }

        public bool HasSingleRidge { get; set; } = false;

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

            // Otherwise create the object
            B = b;
            L = l;

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

            // The flat roof profile points for the roof
            RoofProfile = roof_profile;
            RoofSlope = Math.Atan((RoofProfile[3]-RoofProfile[1]) / (RoofProfile[2] - RoofProfile[0]));

            // Determine our mean roof height -- using the y-coords (1, 3, 5, 7, ...) of roof_profile
            // and determine if the roof is flat or not.
            double max_ht = 0;
            double wall_ht = roof_profile[1];
            HasSingleRidge = false;
            for (int i = 1; i < roof_profile.Length; i = i + 2)
            {
                if (roof_profile[1] != roof_profile[i])
                    HasSingleRidge = true;
                if (roof_profile[i] > max_ht)
                {
                    max_ht = roof_profile[i];
                    RIDGE_1 = new Vector4((float)roof_profile[i - 1], (float)roof_profile[i], 0.0f, 1.0f);
                }
            }

            // Take the average of the lesser of the windward wall height and the leeward wall height -- needed in case of unequal wall heights.
            H = 0.5 * (max_ht + Math.Min(roof_profile[1], roof_profile[roof_profile.Length - 1]));

            // Must be after the H declaration
            RoofZonePts = GetFlatRoofPressureZonePoints();


        }

        /// <summary>
        /// Get the zone boundary locations for the roof measure relative to a reference point (usually 0,0)
        /// </summary>
        /// <returns></returns>
        public virtual Vector4[] GetFlatRoofPressureZonePoints()
        { 
            if (L < H * 0.5)
            {
                return new Vector4[] {
                WW_H_1,
                LW_H_1};
            }
            else if (L < H)
            {
                return new Vector4[] {
                WW_H_1,
                new Vector4((float)(WW_H_1.X + H * 0.5), WW_H_1.Y, 0.0f, 1.0f),
                new Vector4((float)(WW_H_1.X + H * 0.5), WW_H_1.Y, 0.0f, 1.0f),
                LW_H_1};
            }
            else if (L < H * 2.0)
            {
                return new Vector4[] {
                WW_H_1,
                new Vector4((float)(WW_H_1.X + H * 0.5), WW_H_1.Y, 0.0f, 1.0f),
                new Vector4((float)(WW_H_1.X + H * 0.5), WW_H_1.Y, 0.0f, 1.0f),
                new Vector4((float)(WW_H_1.X + H), WW_H_1.Y, 0.0f, 1.0f),
                new Vector4((float)(WW_H_1.X + H), WW_H_1.Y, 0.0f, 1.0f),
                LW_H_1};
            }
            else
            {
                return new Vector4[] {
                WW_H_1,
                new Vector4((float)(WW_H_1.X + H * 0.5), WW_H_1.Y, 0.0f, 1.0f),
                new Vector4((float)(WW_H_1.X + H * 0.5), WW_H_1.Y, 0.0f, 1.0f),
                new Vector4((float)(WW_H_1.X + H), WW_H_1.Y, 0.0f, 1.0f),
                new Vector4((float)(WW_H_1.X + H), WW_H_1.Y, 0.0f, 1.0f),
                new Vector4((float)(WW_H_1.X + H * 2.0), WW_H_1.Y, 0.0f, 1.0f),
                new Vector4((float)(WW_H_1.X + H * 2.0), WW_H_1.Y, 0.0f, 1.0f),
                LW_H_1};
            }
        }
    }
}
