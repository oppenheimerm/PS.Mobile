using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PS.Core.Models.ApiRequestResponse;
using PS.Mobile.Pages;
using PS.Mobile.Services;
using PS.Mobile.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace PS.Mobile.ViewModels
{
    public partial class LoginPageVM : BaseViewModel
    {
        [ObservableProperty]
        private PS.Mobile.Models.AuthenticateRequest formLoginModel;

        [ObservableProperty]

        string validationErrors;

        [ObservableProperty]
        private TextValidationBehavior passwordValidator;

		[ObservableProperty]
		private TextValidationBehavior emailValidator;
        private readonly IMemberService MemberService;

        public ObservableCollection<StationLite> Stationss { get; } = new();


        public LoginPageVM(IAuthService authService, IConnectivity connectivity, IMemberService memberService) : base(authService, connectivity) {
            FormLoginModel = new Models.AuthenticateRequest();            
            PasswordValidator = new TextValidationBehavior();
            emailValidator = new TextValidationBehavior();
            this.MemberService = memberService;
        }


        async Task DoLoginAsync()
        {
            //  TODO
            //  Handle expired token / refesh token

            if (IsBusy) { 

                return; 
            }
            else
            {
                
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await Shell.Current.DisplayAlert("Connectivity issue dected.", $"Please check you internet connection.", "OK");
                }


				//https://www.youtube.com/watch?v=sNter79tWb4&t=409s
				//https://stackoverflow.com/questions/72429055/how-to-displayalert-in-a-net-maui-viewmodel


				IsBusy = true;


				var error = await AuthService.LoginAsync(FormLoginModel);
                if (error.success)
                {
                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                }
                else
                {
                    IsBusy = false;
                    await Shell.Current.DisplayAlert("Login Error", error.errorMessage, "Ok");
                    
                }
            }
		}


		[RelayCommand]
		async Task GoToRegisterPageAsync()
		{
			await Shell.Current.GoToAsync($"//{nameof(RegisterPage)}");
		}

        [RelayCommand]
        async Task GoToForgotPasswordPageAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(ForgotPasswordPage)}");
        }

        [RelayCommand]
        public async Task LoginUserAsync()
        {
            await Validate();
        }

        //// https://www.linkedin.com/pulse/using-net-out-box-validators-maui-eduardo-fonseca
        private async Task Validate()

        {

            ValidationContext validationContext = new ValidationContext(this.FormLoginModel);

            List<ValidationResult> errors = new();

            var isValid = Validator.TryValidateObject(this.FormLoginModel, validationContext, errors, true);

            if (!isValid)

            {

                var errorString = String.Join("\r\n", errors);

                this.ValidationErrors = errorString;

                await Application.Current.MainPage.DisplayAlert("Validation error", errorString, "OK");

                return;

            }

            else

            {
                this.ValidationErrors = null;
                await DoLoginAsync();
            }

        }

    }
}