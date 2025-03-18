using Moq;
using Prism.Events;
using Prism.Ioc;
using SldWorks;
using SolidworksTest.Interfaces;
using SolidworksTest.ViewModels;
using SwConst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidworksDemo.Test.ViewModel.Test;
public class MainWindowViewModelTests
{
    #region Private Fields

    private readonly Mock<IUnitConversionHelper> _mockConversionHelper;
    private readonly Mock<IEventAggregator> _mockEventAggregator;
    private readonly Mock<IContainerProvider> _mockContainer;
    private readonly Mock<IPointViewModel> _mockStartPoint;
    private readonly Mock<IPointViewModel> _mockEndPoint;
    private readonly Mock<SldWorks.SldWorks> _mockSwApp;
    private readonly Mock<ModelDoc2> _mockSwDoc;
    private readonly Mock<SketchManager> _mockSketchManager;
    private readonly Mock<ModelDocExtension> _mockModelDocExtension;
    private readonly Mock<SketchSegment> _mockSketchSegment;

    private readonly MainWindowViewModel _viewModel;

    #endregion

    #region Constructor

    public MainWindowViewModelTests()
    {
        _mockConversionHelper = new Mock<IUnitConversionHelper>();
        _mockEventAggregator = new Mock<IEventAggregator>();
        _mockContainer = new Mock<IContainerProvider>();
        _mockStartPoint = new Mock<IPointViewModel>();
        _mockEndPoint = new Mock<IPointViewModel>();
        _mockSwApp = new Mock<SldWorks.SldWorks>();
        _mockSwDoc = new Mock<ModelDoc2>();
        _mockSketchManager = new Mock<SketchManager>();
        _mockModelDocExtension = new Mock<ModelDocExtension>();
        _mockSketchSegment = new Mock<SketchSegment>();

        _viewModel = new MainWindowViewModel(_mockEventAggregator.Object, _mockContainer.Object, _mockConversionHelper.Object);

        _viewModel.StartPointViewModel = _mockStartPoint.Object;
        _viewModel.EndPointViewModel = _mockEndPoint.Object;
    }

    #endregion

    #region Test Method - [CreateLine]

    [Fact]
    public void CreateLine_ReturnsFalse_WhenLineCreationFails()
    {
        // Arrange
        SketchSegment sketchSegment = null;
        _mockSketchManager.Setup(sm => sm.CreateLine(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
            .Returns((SketchSegment)null);

        // Act
        bool result = _viewModel.CreateLine(sketchSegment, 0,0,0, _mockSwApp.Object, _mockSketchManager.Object, 10,10,0);

        // Assert
        Assert.False(result);
        Assert.Equal("Failed to create Sketch line", _viewModel.messageToShow);
        _mockSwApp.Verify(app => app.CloseAllDocuments(true), Times.Once);
        _mockSwApp.Verify(app => app.ExitApp(), Times.Once);
    }

    [Fact]
    public void CreateLine_ReturnsTrue_WhenLineCreationSucceeds()
    {
        // Arrange
        _mockSketchManager.Setup(sm => sm.CreateLine(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
            .Returns(_mockSketchSegment.Object);

        // Act
        bool result = _viewModel.CreateLine(null, 0, 0, 0, _mockSwApp.Object, _mockSketchManager.Object, 10, 10, 0);

        // Assert
        Assert.True(result);
        _mockSwApp.Verify(app => app.CloseAllDocuments(true), Times.Never);
        _mockSwApp.Verify(app => app.ExitApp(), Times.Never);
    }

    #endregion

    #region Test Method - [ApplyUnitConversion]

    [Fact]
    public void ApplyUnitConversion_PositiveValues_ReturnsConvertedCoordinates()
    {
        // Arrange
        var mockPoint = new Mock<IPointViewModel>();
        mockPoint.Setup(p => p.XPoint).Returns(2);
        mockPoint.Setup(p => p.YPoint).Returns(3);
        mockPoint.Setup(p => p.ZPoint).Returns(4);
        double lengthConversionFactor = 2.5;

        // Act
        var result = _viewModel.ApplyUnitConversion(mockPoint.Object, lengthConversionFactor);

        // Assert
        Assert.Equal((5.0,7.5,10.0), result);
    }

    [Fact]
    public void ApplyUnitConversion_ZeroConversionFactor_ReturnsZeroCoordinates()
    {
        // Arrange
        var mockPoint = new Mock<IPointViewModel>();
        mockPoint.Setup(p => p.XPoint).Returns(2);
        mockPoint.Setup(p => p.YPoint).Returns(3);
        mockPoint.Setup(p => p.ZPoint).Returns(4);
        double lengthConversionFactor = 0;

        // Act
        var result = _viewModel.ApplyUnitConversion(mockPoint.Object, lengthConversionFactor);

        // Assert
        Assert.Equal((0.0, 0.0, 0.0), result);
    }

    [Fact]
    public void ApplyUnitConversion_NegativeConversionFactor_ReturnsNegativeScaledCoordinates()
    {
        // Arrange
        var mockPoint = new Mock<IPointViewModel>();
        mockPoint.Setup(p => p.XPoint).Returns(2);
        mockPoint.Setup(p => p.YPoint).Returns(3);
        mockPoint.Setup(p => p.ZPoint).Returns(4);
        double lengthConversionFactor = -1.5;

        // Act
        var result = _viewModel.ApplyUnitConversion(mockPoint.Object, lengthConversionFactor);

        // Assert
        Assert.Equal((-3.0, -4.5, -6.0), result);
    }

    [Fact]
    public void ApplyUnitConversion_ZeroPointValues_ReturnsZeroCoordinates()
    {
        // Arrange
        var mockPoint = new Mock<IPointViewModel>();
        mockPoint.Setup(p => p.XPoint).Returns(0);
        mockPoint.Setup(p => p.YPoint).Returns(0);
        mockPoint.Setup(p => p.ZPoint).Returns(0);
        double lengthConversionFactor = 2.5;

        // Act
        var result = _viewModel.ApplyUnitConversion(mockPoint.Object, lengthConversionFactor);

        // Assert
        Assert.Equal((0.0, 0.0, 0.0), result);
    }

    [Fact]
    public void ApplyUnitConversion_MixedValues_ReturnsCorrectlyScaledCoordinates()
    {
        // Arrange
        var mockPoint = new Mock<IPointViewModel>();
        mockPoint.Setup(p => p.XPoint).Returns(-1);
        mockPoint.Setup(p => p.YPoint).Returns(0);
        mockPoint.Setup(p => p.ZPoint).Returns(5);
        double lengthConversionFactor = 3;

        // Act
        var result = _viewModel.ApplyUnitConversion(mockPoint.Object, lengthConversionFactor);

        // Assert
        Assert.Equal((-3.0, 0.0, 15.0), result);
    }

    #endregion

    #region Test Method - [SelectSketchPlane]

    [Fact]
    public void SelectSketchPlane_SuccessfullySelectPlane_ReturnTrue()
    {
        // Arrange
        _mockSwDoc.Setup(d => d.Extension).Returns(_mockModelDocExtension.Object);
        _mockSwDoc.Setup(d => d.SketchManager).Returns(_mockSketchManager.Object);
        _mockSwDoc.Setup(d => d.LengthUnit).Returns((int)swLengthUnit_e.swINCHES);
        _mockModelDocExtension.Setup(e => e.SelectByID2("Right Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault)).Returns(true);
        _mockConversionHelper.Setup(c => c.UnitConversion(It.IsAny<swLengthUnit_e>()));
        _mockConversionHelper.Setup(c => c.LengthConversionFactor).Returns(0.0254);

        _mockEndPoint.Setup(p => p.XPoint).Returns(2);
        _mockEndPoint.Setup(p => p.YPoint).Returns(3);
        _mockEndPoint.Setup(p => p.ZPoint).Returns(0);

        _mockSketchManager.Setup(sm => sm.CreateLine(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
                          .Returns(_mockSketchSegment.Object);

        // Act
        var result = _viewModel.SelectSketchPlane(_mockSwApp.Object, _mockSwDoc.Object);

        // Assert
        Assert.True(result);
        _mockSketchManager.Verify(s => s.InsertSketch(false), Times.Once());
        _mockConversionHelper.Verify(c => c.UnitConversion(It.IsAny<swLengthUnit_e>()), Times.Once);
    }

    [Fact]
    public void SelectSketchPlane_FailedToSelectPlane_ReturnFalse()
    {
        // Arrange
        _mockSwDoc.Setup(d => d.Extension).Returns(_mockModelDocExtension.Object);
        _mockModelDocExtension.Setup(e => e.SelectByID2("Right Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault)).Returns(false);
        
        // Act
        var result = _viewModel.SelectSketchPlane(_mockSwApp.Object, _mockSwDoc.Object);

        // Assert
        Assert.False(result);
        Assert.Equal("Failed to select Right plane.", _viewModel.messageToShow);
        _mockSwApp.Verify(app => app.CloseAllDocuments(true), Times.Once);
        _mockSwApp.Verify(app => app.ExitApp(), Times.Once);
    }

    #endregion

    #region Test Method - [CreatePartDocument]

    [Fact]
    public void CreatePartDocument_WhenTemplateIsEmpty_ReturnsFalse()
    {
        // Arrange
        _mockSwApp.Setup(a => a.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart))
            .Returns(string.Empty);

        // Act
        bool result = _viewModel.CreatePartDocument(_mockSwApp.Object, out ModelDoc2 swDoc);

        // Assert
        Assert.False(result);
        Assert.Null(swDoc);
        Assert.Equal("Default part template is empty.", _viewModel.messageToShow);
    }

    [Fact]
    public void CreatePartDocument_WhenNewDocumentFails_ReturnsFalse()
    {
        // Arrange
        _mockSwApp.Setup(a => a.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart))
                  .Returns("defaultTemplatePath");
        _mockSwApp.Setup(a => a.NewDocument("defaultTemplatePath", 0, 0, 0))
                  .Returns((ModelDoc2)null);

        // Act
        bool result = _viewModel.CreatePartDocument(_mockSwApp.Object, out ModelDoc2 swDoc);

        // Assert
        Assert.False(result);
        Assert.Null(swDoc);
        Assert.Equal("Failed to create Part document.", _viewModel.messageToShow);
    }

    [Fact]
    public void CreatePartDocument_FailedSketchPlaneSelection_ReturnsFalse()
    {
        // Arrange
        _mockSwApp.Setup(app => app.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart))
                  .Returns("DefaultTemplatePath");
        _mockSwApp.Setup(app => app.NewDocument("DefaultTemplatePath", 0, 0, 0))
                  .Returns(_mockSwDoc.Object);

        // Mock SelectSketchPlane to return false, simulating failure
        var viewModelMock = new Mock<MainWindowViewModel>(_mockEventAggregator.Object, _mockContainer.Object, _mockConversionHelper.Object) { CallBase = true };
        viewModelMock.Setup(vm => vm.SelectSketchPlane(_mockSwApp.Object, _mockSwDoc.Object))
                      .Returns(false);

        // Act
        var result = viewModelMock.Object.CreatePartDocument(_mockSwApp.Object, out var createdDoc);

        // Assert
        Assert.False(result);
        Assert.Equal(_mockSwDoc.Object, createdDoc);
    }

    [Fact]
    public void CreatePartDocument_WhenDocumentCreatedSuccessfully_ReturnsTrue()
    {
        // Arrange
        var mockDoc = new Mock<ModelDoc2>();
        _mockSwApp.Setup(a => a.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart))
                  .Returns("defaultTemplatePath");
        _mockSwApp.Setup(a => a.NewDocument("defaultTemplatePath", 0, 0, 0))
                  .Returns(mockDoc.Object);

        // Mock the SelectSketchPlane method to return true
        var viewModel = new Mock<MainWindowViewModel>(_mockEventAggregator.Object, _mockContainer.Object, _mockConversionHelper.Object) { CallBase = true };
        viewModel.Setup(vm => vm.SelectSketchPlane(_mockSwApp.Object, mockDoc.Object)).Returns(true);

        // Act
        bool result = viewModel.Object.CreatePartDocument(_mockSwApp.Object, out ModelDoc2 swDoc);

        // Assert
        Assert.True(result);
        Assert.NotNull(swDoc);
    }

    #endregion
}
