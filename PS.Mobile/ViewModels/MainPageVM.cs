using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Maui.ApplicationModel.Communication;
using PS.Core.Models.ApiRequestResponse;
using PS.Mobile.Pages;
using PS.Mobile.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace PS.Mobile.ViewModels
{
    public partial class MainPageVM : BaseViewModel
    {

        IConnectivity connectivity;
        private readonly IMemberService MemberService;

        public ObservableCollection<StationLite> Stations { get; } = new();

        public MainPageVM(IAuthService authService, IConnectivity connectivity, IMemberService memberService) : base(authService, connectivity)
        {
            this.MemberService = memberService;
        }

        [RelayCommand]
        async Task GoToApplicationDetailsPageAsync()
        {
            //AuthService.Logout();
            var isAuthenticated = await AuthService.IsUserAuthenticated();

            if (isAuthenticated.Success)
            {
                //await Shell.Current.GoToAsync($"{nameof(StationsPage)}");
                //  Prevent back button
                await Shell.Current.GoToAsync("//Stations");
            }
            else
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }


    }
}