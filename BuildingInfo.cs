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

        public double RoofSlope = 0; // angle of the roof default is flat roof
        public RoofSlopeTypes RoofSlopeType = RoofSlopeTypes.ROOF_SLOPE_FLAT;

        public virtual Vector4[] RoofProfile_1 {get; set;}
        public virtual Vector4[] RoofProfile_2 { get; set; }


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
        public Vector4 ORIGIN_1 { get; set; } = new Vector4();
        public Vector4 LW_H_1 { get; set; }
        public Vector4 LW_15_1 { get; set; }
        public Vector4 LW_GRD_1 { get; set; }

        // Contains the coordinate for the ridge point
        public Vector4 RIDGE_1 { get; set; }

        public Vector4[] RoofZonePts_1 { get; set; }

        public Vector4 WW_GRD_2 { get; set; }
        public Vector4 WW_15_2 { get; set; }
        public Vector4 WW_H_2 { get; set; }
        public Vector4 ORIGIN_2 { get; set; } = new Vector4();
        public Vector4 LW_H_2 { get; set; }
        public Vector4 LW_15_2 { get; set; }
        public Vector4 LW_GRD_2 { get; set; }

        // Contains the coordinate for the ridge point
        public Vector4 RIDGE_2 { get; set; }
        public Vector4[] RoofZonePts_2 { get; set; }


        /// <summary>
        /// Flat roof building constructor
        /// </summary>
        /// <param name="b"></param>
        /// <param name="l"></param>
        /// <param name="h"></param>
        /// <param name="angle"></param>
        /// <param name="cat"></param>

        public BuildingInfo(Vector4[] roof_profile_1, Vector4[] roof_profile_2, RoofSlopeTypes roof_slope_type, RiskCategories cat = RiskCategories.II)
        {
            string status_msg = "";

            int count_1 = roof_profile_1.Length;
            int count_2 = roof_profile_2.Length;

            // TODO:: Fix the B and L calculations here....B non functional and L broken for Parallel direction views
            B = Math.Abs(roof_profile_2[0].Z - roof_profile_1[0].Z);
            L = Math.Max(Math.Abs(roof_profile_1[count_1 -1].X-roof_profile_1[0].X), Math.Abs(roof_profile_2[count_2-1].X-roof_profile_2[0].X));

            // Find the mean height of the roof (the max of 0, 2, 4, ... elements of the roof profile
            RiskCat = cat;
            RoofSlopeType = roof_slope_type;

            // Vectors for the points of the east-west frame 1 structure profile based on the provided dimensions
            // 0,0 is assumed to be lower left for windward wall
            WW_GRD_1 = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            WW_15_1 = new Vector4(0.0f, 15.0f, 0.0f, 1.0f);
            WW_H_1 = new Vector4(roof_profile_1[0].X, roof_profile_1[0].Y, roof_profile_1[0].Z, 1.0f);
            LW_H_1 = new Vector4(roof_profile_1[count_1-1].X, roof_profile_1[count_1-1].Y, roof_profile_1[count_1-1].Z, 1.0f);
            LW_GRD_1 = new Vector4(LW_H_1.X, 0.0f, 0.0f, 1.0f);
            LW_15_1 = new Vector4(LW_H_1.X, 15.0f, 0.0f, 1.0f);

            // Set the origin of the model to be the midpoint at ground level
            ORIGIN_1 = new Vector4((float)0.5 * (WW_GRD_1.X + LW_GRD_1.X), (float)0.5 * (WW_GRD_1.Y + LW_GRD_1.Y), (float)0.5 * (WW_GRD_1.Z + LW_GRD_1.Z), 1.0f);

            // Vectors for the points of the east-west frame 2 structure profile based on the provided dimensions
            // 0,0 is assumed to be lower left for windward wall
            WW_GRD_2 = new Vector4(roof_profile_2[0].X, 0.0f, roof_profile_2[0].Z, 1.0f);
            WW_15_2 = new Vector4(roof_profile_2[0].X, 15.0f, roof_profile_2[0].Z, 1.0f);
            WW_H_2 = new Vector4(roof_profile_2[0].X, roof_profile_2[0].Y, roof_profile_2[0].Z, 1.0f);
            LW_H_2 = new Vector4(roof_profile_2[count_2 - 1].X, roof_profile_2[count_2 - 1].Y, roof_profile_2[count_2 - 1].Z, 1.0f);
            LW_GRD_2 = new Vector4(LW_H_1.X, 0.0f, roof_profile_2[count_2 - 1].Z, 1.0f);
            LW_15_2 = new Vector4(LW_H_1.X, 15.0f, roof_profile_2[count_2 - 1].Z, 1.0f);

            // Set the origin of the model to be the midpoint at ground level
            ORIGIN_2 = new Vector4((float)0.5 * (WW_GRD_2.X + LW_GRD_2.X), (float)0.5 * (WW_GRD_2.Y + LW_GRD_2.Y), (float)0.5 * (WW_GRD_2.Z + LW_GRD_2.Z), 1.0f);

            // The roof profile points for the roof
            RoofProfile_1 = roof_profile_1;
            RoofProfile_2 = roof_profile_2;

            // Use the East Wall 1 to determine the roof slope factor
            // TODO::  Update this to do more generically. Maybe a normal vector on the plane that makes up the roof?
            
            RoofSlope = Math.Atan((RoofProfile_1[1].Y-RoofProfile_1[0].Y) / (RoofProfile_1[1].X - RoofProfile_1[0].X));

            double max_ht = ComputeMeanRoofHeight();
            // Take the average of the lesser of the windward wall height and the leeward wall height -- needed in case of unequal wall heights.
            H = 0.5 * (max_ht + Math.Min(RoofProfile_1[0].Y, RoofProfile_1[RoofProfile_1.Length - 1].Y));

            // Must be after the H declaration -- define the H/2, H, 2H, >2H locatons
            GetRoofPressureZonePoints();
        }

        private double ComputeMeanRoofHeight()
        {
            // Determine our mean roof height -- using the y-coords (1, 3, 5, 7, ...) of roof_profile
            // and determine if the roof is flat or not.
            double max_ht = 0;
            double wall_ht = RoofProfile_1[0].Y;
            foreach (var item in RoofProfile_1)
            {
                if (item.Y > max_ht)
                {
                    max_ht = item.Y;
                }
            }

            return max_ht;
        }

        public virtual void GetSlopedRoofPressureZonePoints() { throw new InvalidOperationException("A flat roof should not have sloped pressure points"); }
        public virtual void GetRoofPressureZonePoints() { GetFlatRoofPressureZonePoints(); }

        /// <summary>
        /// Get the zone boundary locations for the roof measure relative to a reference point (usually 0,0)
        /// </summary>
        /// <returns></returns>
        public virtual void GetFlatRoofPressureZonePoints()
        { 
            if (L < H * 0.5)
            {
                RoofZonePts_1 = new Vector4[] {
                WW_H_1,
                LW_H_1};
                RoofZonePts_2 = new Vector4[] {
                WW_H_2,
                LW_H_2};
            }
            else if (L < H)
            {
                RoofZonePts_1 = new Vector4[] {
                    WW_H_1,
                    new Vector4((float)(WW_H_1.X + H * 0.5), WW_H_1.Y, WW_H_1.Z, 1.0f),
                    new Vector4((float)(WW_H_1.X + H * 0.5), WW_H_1.Y, WW_H_1.Z, 1.0f),
                    LW_H_1};
                RoofZonePts_2 = new Vector4[] {
                    WW_H_2,
                    new Vector4((float)(WW_H_2.X + H * 0.5), WW_H_2.Y, WW_H_2.Z, 1.0f),
                    new Vector4((float)(WW_H_2.X + H * 0.5), WW_H_2.Y, WW_H_2.Z, 1.0f),
                    LW_H_2};
            }
            else if (L < H * 2.0)
            {
                RoofZonePts_1 = new Vector4[] {
                    WW_H_1,
                    new Vector4((float)(WW_H_1.X + H * 0.5), WW_H_1.Y, WW_H_1.Z, 1.0f),
                    new Vector4((float)(WW_H_1.X + H * 0.5), WW_H_1.Y, WW_H_1.Z, 1.0f),
                    new Vector4((float)(WW_H_1.X + H), WW_H_1.Y, WW_H_1.Z, 1.0f),
                    new Vector4((float)(WW_H_1.X + H), WW_H_1.Y, WW_H_1.Z, 1.0f),
                    LW_H_1};
                RoofZonePts_2 = new Vector4[] {
                    WW_H_2,
                    new Vector4((float)(WW_H_2.X + H * 0.5), WW_H_2.Y, WW_H_2.Z, 1.0f),
                    new Vector4((float)(WW_H_2.X + H * 0.5), WW_H_2.Y, WW_H_2.Z, 1.0f),
                    new Vector4((float)(WW_H_2.X + H), WW_H_2.Y, WW_H_2.Z, 1.0f),
                    new Vector4((float)(WW_H_2.X + H), WW_H_2.Y, WW_H_1.Z, 1.0f),
                    LW_H_2};
            }
            else
            {
                RoofZonePts_1 = new Vector4[] {
                    WW_H_1,
                    new Vector4((float)(WW_H_1.X + H * 0.5), WW_H_1.Y, WW_H_1.Z, 1.0f),
                    new Vector4((float)(WW_H_1.X + H * 0.5), WW_H_1.Y, WW_H_1.Z, 1.0f),
                    new Vector4((float)(WW_H_1.X + H), WW_H_1.Y, WW_H_1.Z, 1.0f),
                    new Vector4((float)(WW_H_1.X + H), WW_H_1.Y, WW_H_1.Z, 1.0f),
                    new Vector4((float)(WW_H_1.X + H * 2.0), WW_H_1.Y, WW_H_1.Z, 1.0f),
                    new Vector4((float)(WW_H_1.X + H * 2.0), WW_H_1.Y, WW_H_1.Z, 1.0f),
                    LW_H_1};
                RoofZonePts_2 = new Vector4[] {
                    WW_H_2,
                    new Vector4((float)(WW_H_2.X + H * 0.5), WW_H_2.Y, WW_H_2.Z, 1.0f),
                    new Vector4((float)(WW_H_2.X + H * 0.5), WW_H_2.Y, WW_H_2.Z, 1.0f),
                    new Vector4((float)(WW_H_2.X + H), WW_H_2.Y, WW_H_2.Z, 1.0f),
                    new Vector4((float)(WW_H_2.X + H), WW_H_2.Y, WW_H_2.Z, 1.0f),
                    new Vector4((float)(WW_H_2.X + H * 2.0), WW_H_2.Y, WW_H_2.Z, 1.0f),
                    new Vector4((float)(WW_H_2.X + H * 2.0), WW_H_2.Y, WW_H_2.Z, 1.0f),
                    LW_H_2};
            }
        }
    }
}
