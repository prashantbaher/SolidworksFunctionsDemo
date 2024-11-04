using EditSketchMethods.Views;
using Prism.Ioc;
using Syncfusion.Licensing;
using System.Windows;

namespace EditSketchMethods
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly string key = "Ngo9BigBOggjHTQxAR8/V1NBaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXxfdXVXRWRfUkx1V0Y=";

        public App()
        {
            SyncfusionLicenseProvider.RegisterLicense(key);
        }
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
