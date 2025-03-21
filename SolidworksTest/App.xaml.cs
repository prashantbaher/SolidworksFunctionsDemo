﻿using Prism.Ioc;
using SolidworksTest.Helper;
using SolidworksTest.Interfaces;
using SolidworksTest.ViewModels;
using SolidworksTest.Views;
using System.Windows;

namespace SolidworksTest;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<IPointViewModel, PointViewModel>();
        containerRegistry.Register<IUnitConversionHelper, UnitConversionHelper>();
    }
}
