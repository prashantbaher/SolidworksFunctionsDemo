using SwConst;

namespace SolidworksTest.Interfaces;
public interface IUnitConversionHelper
{
    double AngleConversionFactor { get; set; }
    double LengthConversionFactor { get; set; }

    void UnitConversion(swLengthUnit_e swUnit);
}