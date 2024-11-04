using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using SldWorks;
using SolidworksTest.Helper;
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

    private PointViewModel _startPointViewModel;
    public PointViewModel StartPointViewModel
    {
        get { return _startPointViewModel; }
        set { SetProperty(ref _startPointViewModel, value); }
    }

    private PointViewModel _endPointViewModel;
    public PointViewModel EndPointViewModel
    {
        get { return _endPointViewModel; }
        set { SetProperty(ref _endPointViewModel, value); }
    }

    #endregion

    #region Fields

    private SldWorks.SldWorks swApp;
    private ModelDoc2 swDoc;
    private string messageToShow;

    private readonly IEventAggregator eventAggregator;
    private readonly IContainerProvider container;

    #endregion

    #region Constructor

    public MainWindowViewModel(IEventAggregator eventAggregator, IContainerProvider container)
    {
        this.eventAggregator = eventAggregator;
        this.container = container;

        StartPointViewModel = this.container.Resolve<PointViewModel>();
        EndPointViewModel = this.container.Resolve<PointViewModel>();

        StartPointViewModel.Header = "Start Point Co-ordinates";
        EndPointViewModel.Header = "End Point Co-ordinates";
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
        swApp = new SldWorks.SldWorks();

        if (swApp == null)
        {
            messageToShow = "Failed to find Solidworks application";
            return false;
        }

        swApp.Visible = true;

        string defaultTemplate = swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);

        if (string.IsNullOrEmpty(defaultTemplate))
        {
            messageToShow = "Default Part template is empty.";
            return false;
        }

        swDoc = swApp.NewDocument(defaultTemplate,0,0,0);

        if (swDoc == null)
        {
            messageToShow = "Failed to get Solidworks document";
            return false;
        }

        bool boolStatus = swDoc.Extension.SelectByID2("Right Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);

        if (boolStatus == false)
        {
            messageToShow = "Failed to select Right Plane.";
            swApp.CloseAllDocuments(true);
            swApp.ExitApp();
            return false;
        }

        swDoc.SketchManager.InsertSketch(false);

        UnitConversionHelper conversionHelper = container.Resolve<UnitConversionHelper>();

        var units = swDoc.GetUnits();

        conversionHelper.UnitConversion((swLengthUnit_e)units[0]);

        double x1, y1, z1, x2, y2, z2;

        (x1, y1, z1) = ApplyUnitConversion(StartPointViewModel, conversionHelper.LengthConversionFactor);
        (x2, y2, z2) = ApplyUnitConversion(EndPointViewModel, conversionHelper.LengthConversionFactor);

        SketchSegment sketchSegment = swDoc.SketchManager.CreateLine(x1, y1, z1, x2, y2, z2);

        if (sketchSegment == null)
        {
            messageToShow = "Failed to create Sketch Line.";
            swApp.CloseAllDocuments(true);
            swApp.ExitApp();
            return false;
        }

        swDoc.ClearSelection2(true);
        swDoc.ViewZoomtofit2();
        messageToShow = "Sketch line successfully created.";
        return true;
    }

    private (double x1, double y1, double z1) ApplyUnitConversion(PointViewModel inputPoint, double lengthConversionFactor)
    {
        double x, y, z;

        x = inputPoint.XPoint * lengthConversionFactor;
        y = inputPoint.YPoint * lengthConversionFactor;
        z = inputPoint.ZPoint * lengthConversionFactor;

        return (x, y, z);
    }

    #endregion
}
