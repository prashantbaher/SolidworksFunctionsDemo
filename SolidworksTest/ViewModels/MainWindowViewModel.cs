using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using SldWorks;
using SolidworksTest.Helper;
using SolidworksTest.Interfaces;
using SolidworksTest.Service;
using SwConst;
using System;
using System.Threading.Tasks;

namespace SolidworksTest.ViewModels;
public class MainWindowViewModel : BindableBase
{
    #region Properties

    private string _title = "Prism Application";
    public string Title
    {
        get { return _title; }
        set { SetProperty(ref _title, value); }
    }

    private bool _isBusy;

    public bool IsBusy
    {
        get { return _isBusy; }
        set { SetProperty(ref _isBusy, value); }
    }

    private IPointViewModel _startPointViewModel;
    public IPointViewModel StartPointViewModel
    {
        get { return _startPointViewModel; }
        set { SetProperty(ref _startPointViewModel, value); }
    }

    private IPointViewModel _endPointViewModel;
    public IPointViewModel EndPointViewModel
    {
        get { return _endPointViewModel; }
        set { SetProperty(ref _endPointViewModel, value); }
    }

    #endregion

    #region Fields

    public string messageToShow;

    private readonly IEventAggregator eventAggregator;
    private readonly IContainerProvider container;
    private readonly IUnitConversionHelper unitConversionHelper;

    #endregion

    #region Constructor

    public MainWindowViewModel(IEventAggregator eventAggregator, IContainerProvider container, IUnitConversionHelper unitConversionHelper)
    {
        this.eventAggregator = eventAggregator;
        this.container = container;
        this.unitConversionHelper = unitConversionHelper;
        StartPointViewModel = this.container.Resolve<IPointViewModel>();
        EndPointViewModel = this.container.Resolve<IPointViewModel>();
    }

    #endregion

    #region Command

    private DelegateCommand _createLineCommand;
    public DelegateCommand CreateLineCommand =>
        _createLineCommand ?? (_createLineCommand = new DelegateCommand(ExecuteCreateLineCommand));

    async void ExecuteCreateLineCommand()
    {
        IsBusy = true;

        bool result = await Task.Run(CreateLineMethod);

        if (result)
            this.eventAggregator.GetEvent<InformationMessagesService>().Publish(messageToShow);
        else
            this.eventAggregator.GetEvent<ErrorMessagesService>().Publish(messageToShow);

        IsBusy = false;
    }

    private bool CreateLineMethod()
    {
        ModelDoc2 swDoc;

        if (CreateSolidworksInstance(out swDoc) == false)
            return false;

        swDoc.ClearSelection2(true);
        swDoc.ViewZoomtofit2();
        messageToShow = "Sketch line created successfully.";
        return true;
    }

    private bool CreateSolidworksInstance(out ModelDoc2 swDoc)
    {
        SldWorks.SldWorks swApp = new SldWorks.SldWorks();

        if (swApp == null)
        {
            messageToShow = "Failed to find Solidworks application.";
            swDoc = null;
            return false;
        }

        swApp.Visible = true;

        return CreatePartDocument(swApp, out swDoc);
    }

    private bool CreatePartDocument(SldWorks.SldWorks swApp, out ModelDoc2 swDoc)
    {
        string defaultTemplate = swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);

        if (string.IsNullOrEmpty(defaultTemplate))
        {
            messageToShow = "Default part template is empty.";
            swDoc = null;
            return false;
        }

        swDoc = swApp.NewDocument(defaultTemplate,0,0,0);

        if (swDoc == null)
        {
            messageToShow = "Failed to create Part document.";
            return false;
        }

        return SelectSketchPlane(swApp, swDoc);
    }

    private bool SelectSketchPlane(SldWorks.SldWorks swApp, ModelDoc2 swDoc)
    {
        bool status = swDoc.Extension.SelectByID2("Right Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);

        if (status == false)
        {
            messageToShow = "Failed to select Right plane.";
            swApp.CloseAllDocuments(true);
            swApp.ExitApp();
            return false;
        }

        swDoc.SketchManager.InsertSketch(false);

        double x1, y1, z1, x2, y2, z2;
        var lengthUnit = swDoc.LengthUnit;

        unitConversionHelper.UnitConversion((swLengthUnit_e)lengthUnit);

        (x1, y1, z1) = ApplyUnitConversion(StartPointViewModel, unitConversionHelper.LengthConversionFactor);
        (x2, y2, z2) = ApplyUnitConversion(EndPointViewModel, unitConversionHelper.LengthConversionFactor);

        SketchSegment sketchSegment = null;

        return CreateLine(sketchSegment, x1, y1, z1, swApp, swDoc.SketchManager, x2, y2, z2);
    }

    public bool CreateLine(SketchSegment sketchSegment, double x1, double y1, double z1, SldWorks.SldWorks swApp, SketchManager sketchManager, double x2, double y2, double z2)
    {
        sketchSegment = sketchManager.CreateLine(x1, y1, z1, x2, y2, z2);

        if (sketchSegment == null)
        {
            messageToShow = "Failed to create Sketch line";
            swApp.CloseAllDocuments(true);
            swApp.ExitApp();
            return false;
        }

        return true;
    }

    private (double x1, double y1, double z1) ApplyUnitConversion(IPointViewModel inputPoint, double lengthConversionFactor)
    {
        double x, y, z;

        x = inputPoint.XPoint * lengthConversionFactor;
        y = inputPoint.YPoint * lengthConversionFactor;
        z = inputPoint.ZPoint * lengthConversionFactor;

        return (x, y, z);
    }

    #endregion
}
