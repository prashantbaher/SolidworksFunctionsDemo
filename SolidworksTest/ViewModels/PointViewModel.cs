using Prism.Mvvm;
using SolidworksTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidworksTest.ViewModels;
public class PointViewModel : BindableBase, IPointViewModel
{
    #region Properties

    private string _header;
    public string Header
    {
        get { return _header; }
        set { SetProperty(ref _header, value); }
    }

    private double _xpoint;
    public double XPoint
    {
        get { return _xpoint; }
        set { SetProperty(ref _xpoint, value); }
    }

    private double _ypoint;
    public double YPoint
    {
        get { return _ypoint; }
        set { SetProperty(ref _ypoint, value); }
    }

    private double _zpoint;
    public double ZPoint
    {
        get { return _zpoint; }
        set { SetProperty(ref _zpoint, value); }
    }

    #endregion

    #region Constructor

    public PointViewModel()
    {

    }

    #endregion
}
