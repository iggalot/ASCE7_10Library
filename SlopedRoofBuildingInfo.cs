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
        // Contains the coordinate for the ridge point
        public Vector3 RIDGE_1 { get; set; }

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

        }
    }
}
