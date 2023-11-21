using PS.Mobile.ViewModels;

namespace PS.Mobile.Pages;

public partial class StationsPage : ContentPage
{

	public StationsPage(StationsVM vm)
	{
		InitializeComponent();
        BindingContext = vm;
	}

}