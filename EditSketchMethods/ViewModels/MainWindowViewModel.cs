using Prism.Commands;
using Prism.Mvvm;
using SldWorks;
using SwConst;
using System.Threading.Tasks;

namespace EditSketchMethods.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Private Fields

        private SldWorks.SldWorks swApp;

        private ModelDoc2 swDoc;

        #endregion

        #region Properties

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

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            swApp = new SldWorks.SldWorks();
        }

        #endregion

        #region Commands

        private DelegateCommand _editSketchMethod;
        public DelegateCommand EditSketchMethod =>
            _editSketchMethod ?? (_editSketchMethod = new DelegateCommand(ExecuteEditSketchMethod ));

        async void ExecuteEditSketchMethod ()
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                if (SelectSketch())
                    swDoc.EditSketch();
            });

            IsBusy = false;
        }

        private DelegateCommand _editSketchOrSingleSketchFeatureMethod;
        public DelegateCommand EditSketchOrSingleSketchFeatureMethod =>
            _editSketchOrSingleSketchFeatureMethod ?? (_editSketchOrSingleSketchFeatureMethod = new DelegateCommand(ExecuteEditSketchOrSingleSketchFeatureMethod ));

        async void ExecuteEditSketchOrSingleSketchFeatureMethod ()
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                if (SelectSketch())
                    swDoc.EditSketchOrSingleSketchFeature();
            });

            IsBusy = false;
        }

        private bool SelectSketch()
        {
            swDoc = null;
            swDoc = swApp.ActiveDoc;

            bool isSelect = swDoc?.Extension.SelectByID2("Sketch2", "SKETCH",0,0,0,false, 4 , null, (int)swSelectOption_e.swSelectOptionDefault) ?? false;

            return isSelect;
        }

        #endregion
    }
}
