using PS.Mobile.ViewModels;

namespace PS.Mobile.Pages;

public partial class ForgotPasswordPage : ContentPage
{  

    public ForgotPasswordPage(ForgotPasswordVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}