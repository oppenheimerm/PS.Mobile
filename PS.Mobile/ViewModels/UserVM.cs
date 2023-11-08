using CommunityToolkit.Mvvm.Input;
using PS.Core.Models.ApiRequestResponse;
using PS.Mobile.Services.Interfaces;
using System.Collections.ObjectModel;

namespace PS.Mobile.ViewModels
{
    public partial class UserVM : BaseViewModel
    {
        private readonly IMemberService MemberService;
        public ObservableCollection<StationLite> Stations { get; } = new();
        public UserVM(IAuthService authService, IConnectivity connectivity,
            IMemberService memberService) : base(authService, connectivity)
        {
            this.MemberService = memberService;
        }


        [RelayCommand]
        async Task GetStationsAsync()
        {
            if (IsBusy) { return; }
            else
            {
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
                        Stations.Add(item);
                    }
                    IsBusy = false;
                }

            }
        }
    }
}
