using InsertSketch.Services;
using Prism.Events;
using Syncfusion.Windows.Shared;
using System;
using System.Windows;

namespace InsertSketch.Views
{
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
            this.eventAggregator.GetEvent<ConfirmationMessagesService>().Subscribe(ConfirmationMessages);
            this.eventAggregator.GetEvent<InformationMessagesService>().Subscribe(InformationMessages);
            this.eventAggregator.GetEvent<ErrorMessagesService>().Subscribe(ErrorMessages);
        }

        private void ConfirmationMessages(string messageToShow)
        {
            var result = MessageBox.Show(messageToShow, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        private void InformationMessages(string messageToShow)
        {
            MessageBox.Show(messageToShow, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ErrorMessages(string messageToShow)
        {
            MessageBox.Show(messageToShow, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
