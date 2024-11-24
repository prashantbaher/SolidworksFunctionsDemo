using SolidworksTest.Interfaces;
using SwConst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidworksTest.Helper;

public class UnitConversionHelper : IUnitConversionHelper
{
    #region Public Properties

    public double LengthConversionFactor { get; set; }
    public double AngleConversionFactor { get; set; }

    #endregion

    #region Public Method

    public void UnitConversion(swLengthUnit_e swUnit)
    {
        if (swUnit == swLengthUnit_e.swMETER)
        {
            LengthConversionFactor = AngleConversionFactor = 1;
        }
        else if (swUnit == swLengthUnit_e.swMM)
        {
            LengthConversionFactor = 1 / 1000;
            AngleConversionFactor = 1 * 0.01745329;
        }
        else if (swUnit == swLengthUnit_e.swCM)
        {
            LengthConversionFactor = 1 / 100;
            AngleConversionFactor = 1 * 0.01745329;
        }
        else if (swUnit == swLengthUnit_e.swINCHES)
        {
            LengthConversionFactor = 1 * 0.0254;
            AngleConversionFactor = 1 * 0.01745329;
        }
        else if (swUnit == swLengthUnit_e.swFEET)
        {
            LengthConversionFactor = 1 * (0.0254 * 12);
            AngleConversionFactor = 1 * 0.01745329;
        }
        else if (swUnit == swLengthUnit_e.swFEETINCHES)
        {
            LengthConversionFactor = 1 * 0.0254;
            AngleConversionFactor = 1 * 0.01745329;
        }
    }

    #endregion
}
