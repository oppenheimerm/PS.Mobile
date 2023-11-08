using PS.Mobile.ViewModels;

namespace PS.Mobile.Pages;

public partial class ApplicationDetailsPage : ContentPage
{
	public ApplicationDetailsPage(ApplicationDetailsPageVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}