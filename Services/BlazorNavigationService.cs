using Microsoft.AspNetCore.Components;

namespace CottageFinder.Services
{
    public class BlazorNavigationService
    {
        private NavigationManager? _navigationManager;

        public void SetNavigationManager(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public void NavigateTo(string uri)
        {
            _navigationManager?.NavigateTo(uri);
        }
    }
}
