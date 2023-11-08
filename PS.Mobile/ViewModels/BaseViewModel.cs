using CommunityToolkit.Mvvm.ComponentModel;
using PS.Mobile.Services.Interfaces;

namespace PS.Mobile.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        public readonly IAuthService AuthService;
        private readonly IConnectivity Connectivity;

        public BaseViewModel(IAuthService authService, IConnectivity connectivity)
        {
            AuthService = authService;
            Connectivity = connectivity;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;
        [ObservableProperty]
        string title;

        //  Return the opposite of isbusy
        public bool IsNotBusy => !isBusy;

    }
}