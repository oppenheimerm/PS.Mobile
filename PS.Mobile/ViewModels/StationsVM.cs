
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PS.Core.Models.ApiRequestResponse;
using PS.Mobile.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace PS.Mobile.ViewModels
{
    [QueryProperty("Stations", "Stations")]
    public partial class StationsVM : BaseViewModel
    {
        private readonly IMemberService MemberService;
        

        [ObservableProperty]
        bool isRefreshing;


        public ObservableCollection<StationLite> Stations { get; } = new();


        public StationsVM(IAuthService authService, IConnectivity connectivity,
            IMemberService memberService) : base(authService, connectivity)
        {
            this.MemberService = memberService;
        }


       
        [RelayCommand]
        async Task GetStationsAsync()
        {
            if (IsBusy) return;

            try {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await Shell.Current.DisplayAlert("Connectivity issue dected.", $"Please check you internet connection.", "OK");
                }
                IsBusy = true;
                var request = await MemberService.GetStationsAsync();
                if (request.success)
                {
                    foreach (var item in request.Stations)
                    {
                        item.Logos = Helpers.ViewHelpers.GetLogoPaths(item.Logos);
                        Stations.Add(item);
                    }
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                await Shell.Current.DisplayAlert("Error!", $"Unable to get stations: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;

            }

        }

       
    }
}
