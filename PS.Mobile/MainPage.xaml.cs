using PS.Mobile.ViewModels;

namespace PS.Mobile
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageVM ViewModel;

        public MainPage(MainPageVM vm)
        {
            InitializeComponent();
            BindingContext = vm;
            ViewModel = vm;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            ViewModel.GoToApplicationDetailsPageCommand.Execute(this);
        }
    }
}