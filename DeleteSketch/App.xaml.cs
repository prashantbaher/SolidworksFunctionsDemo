using DeleteSketch.Views;
using Prism.Ioc;
using Syncfusion.Licensing;
using System.Windows;

namespace DeleteSketch
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly string key = "Ngo9BigBOggjHTQxAR8/V1NCaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXhfeXVXRmheVkF0V0I=";

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
