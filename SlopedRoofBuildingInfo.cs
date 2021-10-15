using System;
using System.Numerics;

namespace ASCE7_10Library
{
    public class SlopedRoofBuildingInfo : BuildingInfo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="b">horizontal dimension perpendicular to the wind</param>
        /// <param name="l">horizontal dimension parallel to wind</param>
        /// <param name="h">mean roof height</param>
        /// <param name="roof_profile">array of (x,y) points that define the roof profile. Xi, Yi, Xi+1, Yi+1</param>
        /// <param name="angle">angle of the roof</param>
        /// <param name="risk">ASCE7-10 risk category</param>
        public SlopedRoofBuildingInfo(Vector4[] roof_profile_east_1, Vector4[] roof_profile_east_2, RoofSlopeTypes roof_slope_type, RiskCategories risk) : base(roof_profile_east_1, roof_profile_east_2, roof_slope_type, risk)
        {
            if ((roof_profile_east_1.Length < 2) || (roof_profile_east_2.Length < 2))
                throw new InvalidOperationException("Roof profile for a sloped roof building must contain at least three valid.  Your input has: " 
                    + roof_profile_east_1.Length / 2.0 + " " + roof_profile_east_2.Length / 2.0);

            // Find a ridge point on the east profile if it exists
            Vector4 ridge_1 = roof_profile_east_1[0];
            if(roof_profile_east_1.Length > 2)
            {
                // find the highest point
                foreach (var item in roof_profile_east_1)
                {
                    if (item.Y >= ridge_1.Y)
                        ridge_1 = item;
                }
            }
            RIDGE_1 = ridge_1;

            // Find a ridge point on the east profile if it exists
            Vector4 ridge_2 = roof_profile_east_2[0];
            if (roof_profile_east_2.Length > 2)
            {
                // find the highest point
                foreach (var item in roof_profile_east_2)
                {
                    if (item.Y >= ridge_2.Y)
                        ridge_2 = item;
                }
            }
            RIDGE_2 = ridge_2;


            // Compute the slope from  WW wall to 1st Ridge point
            //           if (HasSingleRidge)
            //          {
            double roof_slope_east_1= Math.Atan((RIDGE_1.Y - WW_H_1.Y) / (RIDGE_1.X - WW_H_1.X)) * 180.0 / Math.PI;
            RoofSlope = Math.Atan((RIDGE_1.Y - WW_H_1.Y) / (RIDGE_1.X - WW_H_1.X)) * 180.0 / Math.PI;
 //           } else
 //           {
 //               throw new InvalidOperationException("undefined roof type in calculating RoofSlope -- must be either flat or single ridge for now");
  //          }

            GetSlopedRoofPressureZonePoints();
        }

        /// <summary>
        /// Returns the necessary pressure locations for a sloped roof.
        /// </summary>
        /// <returns></returns>
        public override void GetSlopedRoofPressureZonePoints()
        {
            RoofZonePts_1 = new Vector4[] { WW_H_1, RIDGE_1, RIDGE_1, LW_H_1 };
            RoofZonePts_2 = new Vector4[] { WW_H_2, RIDGE_2, RIDGE_2, LW_H_2 };
        }

        public override void GetRoofPressureZonePoints() { GetSlopedRoofPressureZonePoints(); }
    }
}
