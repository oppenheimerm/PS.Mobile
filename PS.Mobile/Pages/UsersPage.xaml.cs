using PS.Mobile.ViewModels;

namespace PS.Mobile.Pages;

public partial class UsersPage : ContentPage
{
	public UsersPage(UserVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}