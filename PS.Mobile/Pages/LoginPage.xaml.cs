using PS.Mobile.ViewModels;

namespace PS.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginPageVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}