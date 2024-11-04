using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SldWorks;
using SwConst;
using System.Threading.Tasks;
using static ChangeSketchName.Services.MessagesService;

namespace ChangeSketchName.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Test App";
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

        private string _selectedName;

        public string SelectedName
        {
            get { return _selectedName; }
            set { SetProperty(ref _selectedName, value); }
        }

        #region Private Fields

        private SldWorks.SldWorks swApp;

        private ModelDoc2 swDoc;

        private string messageToShow;

        #endregion


        public MainWindowViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        private DelegateCommand _changeNameCommand;
        private readonly IEventAggregator eventAggregator;

        public DelegateCommand ChangeNameCommand =>
            _changeNameCommand ?? (_changeNameCommand = new DelegateCommand(ExecuteChangeNameCommand));

        async void ExecuteChangeNameCommand()
        {
            // Show busy indicator
            IsBusy = true;

            bool result = await Task.Run(ChangeSketchName);

            if (result)
                this.eventAggregator.GetEvent<InformationMessagesService>().Publish(messageToShow);
            else
                this.eventAggregator.GetEvent<ErrorMessagesService>().Publish(messageToShow);

            // Hide busy indicator
            IsBusy = false;
        }

        bool ChangeSketchName()
        {
            swApp = new SldWorks.SldWorks();

            if (swApp == null)
            {
                messageToShow = "Failed to find Solidworks application.";
                return false;
            }

            swDoc = swApp.ActiveDoc;

            if (swDoc == null)
            {
                messageToShow = "Failed to get Solidworks document.";
                return false;
            }

            bool isSketchRenamed = swDoc.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, SelectedName);

            if (isSketchRenamed == false)
            {
                messageToShow = "Failed to rename selected sketch.";
                return false;
            }

            messageToShow = "Selected Sketch is renamed.";
            return true;
        }
    }
}
