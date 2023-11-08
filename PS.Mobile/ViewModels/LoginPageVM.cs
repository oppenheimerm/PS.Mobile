using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Networking;
using PS.Core.Models.ApiRequestResponse;
using PS.Mobile.Pages;
using PS.Mobile.Services.Interfaces;

namespace PS.Mobile.ViewModels
{
    public partial class LoginPageVM : BaseViewModel
    {

        public LoginPageVM(IAuthService authService, IConnectivity connectivity) : base(authService, connectivity) { }


        [RelayCommand]
        async Task LoginUserAsync()
        {
            //  TODO
            //  Handle expired token / refesh token

            if (IsBusy) { return; }
            else
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await Shell.Current.DisplayAlert("Connectivity issue dected.", $"Please check you internet connection.", "OK");
                }


                IsBusy = true;
                var authRequest = new AuthenticateRequest() { EmailAddress = "joesephbrothag@gmail.com", Password = "==45&9!RrTtt=" };
                var error = await AuthService.LoginAsync(authRequest);
                if (error.success)
                {
                    IsBusy = false;
                    await Shell.Current.GoToAsync($"//{nameof(UsersPage)}");
                    
                }
                else
                {
                    IsBusy = false;
                    await Shell.Current.DisplayAlert("Login Error", error.errorMessage, "Ok");
                    
                }
            }


            //https://www.youtube.com/watch?v=-wu8d3IQO-Q&t=336s
            //    48:01
        }
    }
}