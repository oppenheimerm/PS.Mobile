using CommunityToolkit.Mvvm.Input;
using PS.Mobile.Pages;
using PS.Mobile.Services.Interfaces;

namespace PS.Mobile.ViewModels
{
    public partial class MainPageVM : BaseViewModel
    {

        IConnectivity connectivity;

        public MainPageVM(IAuthService authService, IConnectivity connectivity) : base(authService, connectivity)
        {

        }

        [RelayCommand]
        async Task GoToApplicationDetailsPageAsync()
        {
            AuthService.Logout();

            if (await AuthService.IsUserAuthenticated())
            {
                await Shell.Current.GoToAsync($"//{nameof(UsersPage)}");
            }
            else
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
    }
}