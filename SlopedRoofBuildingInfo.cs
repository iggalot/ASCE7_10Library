using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ASCE7_10Library
{
    public class SlopedRoofBuildingInfo : BuildingInfo
    {
        public override double[] RoofProfile {get; set;}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="b">horizontal dimension perpendicular to the wind</param>
        /// <param name="l">horizontal dimension parallel to wind</param>
        /// <param name="h">mean roof height</param>
        /// <param name="roof_profile">array of (x,y) points that define the roof profile. Xi, Yi, Xi+1, Yi+1</param>
        /// <param name="angle">angle of the roof</param>
        /// <param name="risk">ASCE7-10 risk category</param>
        public SlopedRoofBuildingInfo(double b, double l, double h, double[] roof_profile, RiskCategories risk) : base(b, l, h, roof_profile, risk)
        {
            if (roof_profile.Length / 2.0 <= 4)
                throw new InvalidOperationException("Roof profile for a sloped roof building must contain at least three valid.  Your input has: " + roof_profile.Length / 2.0);
            if  (roof_profile.Length % 2.0 != 0)
            {
                throw new InvalidOperationException("Roof profile must have an even number of elements for x and y coords");
            }


            // Compute the slope from ridge point to WW wall
            // TODO:: Need a better calculation for this -- gambrel roof?  sawtooth roof?
            if (HasSingleRidge)
            {
                RoofSlope = Math.Atan((RIDGE_1.Y - WW_H_1.Y) / (RIDGE_1.X - WW_H_1.X)) * 180.0 / Math.PI;
            } else
            {
                throw new InvalidOperationException("undefined roof type in calculating RoofSlope -- must be either flat or single ridge for now");
            }

            RoofZonePts = GetFlatRoofPressureZonePoints();
        }

        /// <summary>
        /// Get the zone boundary locations for the roof measure relative to a reference point (usually 0,0)
        /// </summary>
        /// <param name="x_ref">x-coord reference location (usually the top of the windward wall</param>
        /// <param name="y_ref">y-coord reference location (usually the top of the windward wall</param>
        /// <returns></returns>
        public override Vector4[] GetFlatRoofPressureZonePoints()
        {
            if (HasSingleRidge)
            {
                return new Vector4[] { WW_H_1, RIDGE_1, RIDGE_1, LW_H_1 };
            } else
            {
                throw new InvalidOperationException("undefined roof type in calculating RoofSlope -- must be either flat or single ridge for now");
            }
        }
    }
}
