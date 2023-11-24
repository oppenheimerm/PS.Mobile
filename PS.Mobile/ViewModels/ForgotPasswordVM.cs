using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PS.Mobile.Pages;
using PS.Mobile.Services.Interfaces;

namespace PS.Mobile.ViewModels
{
    public partial class ForgotPasswordVM : BaseViewModel
    {
        public ForgotPasswordVM(IAuthService authService, IConnectivity connectivity, IMemberService memberService) : base(authService, connectivity)
        {
            
        }

        [RelayCommand]
        async Task GoToLoginPageAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}
