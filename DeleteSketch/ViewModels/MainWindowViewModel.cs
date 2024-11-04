using DeleteSketch.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SldWorks;
using SwConst;
using System.Threading;
using System.Threading.Tasks;

namespace DeleteSketch.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Setting Project

        #region Public Properties
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
        #endregion

        #region Private Fields

        private SldWorks.SldWorks swApp;

        private ModelDoc2 swDoc;

        private string messageToShow;

        private readonly IEventAggregator eventAggregator;
        #endregion

        #region Constructor
        public MainWindowViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        #endregion

        #endregion

        #region Commands

        private DelegateCommand _deleteCommand;
        public DelegateCommand DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new DelegateCommand(ExecuteDeleteCommand));

        async void ExecuteDeleteCommand()
        {
            // Show busy indicator
            IsBusy = true;

            bool result = await Task.Run(DeleteSketch);

            // Hide busy indicator
            IsBusy = false;

            if (result)
                this.eventAggregator.GetEvent<InformationMessagesService>().Publish(messageToShow);
            else
                this.eventAggregator.GetEvent<ErrorMessagesService>().Publish(messageToShow);
        }

        private bool DeleteSketch()
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

            //swDoc.EditDelete();

            bool result = swDoc.Extension.DeleteSelection2((int)swDeleteSelectionOptions_e.swDelete_Children);

            if (result == false)
            {
                messageToShow = "Failed to delete selected sketch.";
                return false;
            }

            messageToShow = "Selected sketch is deleted.";
            return true;
        }

        #endregion
    }
}
