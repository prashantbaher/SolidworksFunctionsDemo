using Moq;
using Prism.Events;
using Prism.Ioc;
using SldWorks;
using SolidworksTest.Interfaces;
using SolidworksTest.ViewModels;
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
}
