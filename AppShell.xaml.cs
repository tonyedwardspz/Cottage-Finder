using Microsoft.Maui.Controls;
using CottageFinder.Services;

namespace CottageFinder
{
    public partial class AppShell : Shell
    {
        private BlazorNavigationService? _navigationService;

        public AppShell()
        {
            InitializeComponent();
            
            // Handle navigation events to intercept Shell navigation and convert to Blazor navigation
            Navigating += OnNavigating;
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            if (Handler?.MauiContext?.Services != null)
            {
                _navigationService = Handler.MauiContext.Services.GetService<BlazorNavigationService>();
            }
        }

        private async void OnNavigating(object? sender, ShellNavigatingEventArgs e)
        {
            var targetRoute = e.Target?.Location?.OriginalString;
            
            // If navigating to MainPage (default), navigate to "/" (cottages) in Blazor
            if (targetRoute == "//MainPage" || targetRoute == "/MainPage")
            {
                e.Cancel();
                FlyoutIsPresented = false;
                _navigationService?.NavigateTo("/");
            }
        }

        private void OnSensorReadingsClicked(object? sender, EventArgs e)
        {
            _navigationService?.NavigateTo("/sensor-readings");
            FlyoutIsPresented = false;
        }
    }
}
