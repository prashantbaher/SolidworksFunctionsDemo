using InsertSketch.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SldWorks;
using SwConst;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InsertSketch.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        private List<string> _planeName;
        public List<string> PlaneName
        {
            get { return _planeName; }
            set { SetProperty(ref _planeName, value); }
        }

        private string _selectedPlane;

        public string SelectedPlane
        {
            get { return _selectedPlane; }
            set { SetProperty(ref _selectedPlane, value); }
        }

        private readonly IEventAggregator eventAggregator;
        public MainWindowViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            PlaneName = new List<string> { "Front Plane", "Right Plane", "Top Plane" };
        }

        private DelegateCommand _insertSketchCommand;
        public DelegateCommand InsertSketchCommand =>
            _insertSketchCommand ?? (_insertSketchCommand = new DelegateCommand(ExecuteInsertSketchCommand));

        async void ExecuteInsertSketchCommand()
        {
            if (string.IsNullOrEmpty(SelectedPlane))
            {
                this.eventAggregator.GetEvent<ErrorMessagesService>().Publish("No plane is selected in application. Please select a plane.");
                return;
            }

            IsBusy = true;

            bool insertSketch = await Task.Run(InsertSketch);

            IsBusy = false;

            if (insertSketch == false)
                return;

            this.eventAggregator.GetEvent<InformationMessagesService>().Publish("Sketch inserted successfully.");

        }

        private bool InsertSketch()
        {
            SldWorks.SldWorks swApp = new SldWorks.SldWorks();

            string defaultTemplate = swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);

            if (string.IsNullOrEmpty(defaultTemplate))
            {
                this.eventAggregator.GetEvent<ErrorMessagesService>().Publish("Default Part template is empty.");
                return false;
            }

            ModelDoc2 swDoc = swApp.NewDocument(defaultTemplate, 0, 0, 0);

            if (swDoc == null)
            {
                this.eventAggregator.GetEvent<ErrorMessagesService>().Publish("Failed to create new Part document.");
                return false;
            }

            bool boolStatus = swDoc.Extension.SelectByID2(SelectedPlane, "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);

            if(boolStatus == false)
            {
                this.eventAggregator.GetEvent<ErrorMessagesService>().Publish($"Failed to select [{SelectedPlane}].");
                swApp.CloseAllDocuments(true);
                swApp.ExitApp();
                return false;
            }

            swDoc.Visible = true;

            swDoc.SketchManager.InsertSketch(false);

            return true;
        }
    }
}
