using Prism.Events;
using SolidworksTest.Service;
using Syncfusion.Windows.Shared;
using System;
using System.Windows;

namespace SolidworksTest.Views;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : ChromelessWindow
{
    private readonly IEventAggregator eventAggregator;

    public MainWindow(IEventAggregator eventAggregator)
    {
        InitializeComponent();
        this.eventAggregator = eventAggregator;
        this.eventAggregator.GetEvent<InformationMessagesService>().Subscribe(InfotmationMessage);
        this.eventAggregator.GetEvent<ErrorMessagesService>().Subscribe(ErrorMessage);
    }

    private void ErrorMessage(string messageToShow)
    {
        MessageBox.Show(messageToShow,"Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void InfotmationMessage(string messageToShow)
    {
        MessageBox.Show(messageToShow, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
