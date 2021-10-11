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
    /// Building type wind building type classification (enclosed, partially enclosed, open)
    /// </summary>
    public enum WindBuildingTypes
    {
        WIND_BUILDING_TYPE_UNDEFINED = -1,
        WIND_BUILDING_TYPE_ENCLOSED = 0,
        WIND_BUILDING_TYPE_PARTIALLYENCLOSED = 1,
        WIND_BUILDING_TYPE_OPEN = 2
    }

    public enum WindCasesDesignation
    {
        WIND_CASE_UNDEFINED = -1,
        WIND_CASE_A = 0,
        WIND_CASE_B = 1,
    }

    /// <summary>
    /// Enum for the types of roof slopes (gable/hip, monoslope, single ridge, mansard, gambrel, and sawtooth
    /// </summary>
    public enum RoofSlopeTypes
    {
        ROOF_SLOPE_UNDEFINED = -1,
        ROOF_SLOPE_FLAT = 0,
        ROOF_SLOPE_SINGLERIDGE = 1,
        ROOF_SLOPE_GABLEHIP = 2,
        ROOF_SLOPE_MONOSLOPE_RISING = 3,
        ROOF_SLOPE_MONOSLOPE_DESCENDING = 4,
        ROOF_SLOPE_MANSARD = 5,
        ROOF_SLOPE_GAMBREL = 6,
        ROOF_SLOPE_TROUGHED = 7,
        ROOF_SLOPE_SAWTOOTH = 8
    }

    /// <summary>
    /// Enum for the orientation of the wind with respect to the ridge of the building
    /// </summary>
    public enum WindOrientations
    {
        WIND_ORIENTATION_UNDEFINED = -1,
        WIND_ORIENTATION_NORMALTORIDGE = 0,
        WIND_ORIENTATION_PARALLELTORIDGE = 1
    }

    public static class Constants
    {

    }
}
