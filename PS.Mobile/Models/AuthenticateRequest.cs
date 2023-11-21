
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PS.Mobile.Models
{
    //	See: https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/observablevalidator


    public class AuthenticateRequest : ObservableValidator
    {
        public AuthenticateRequest()
        {
			ValidateAllProperties();
		}

        private string emailAddress;
		private string password;

		[Required]
		[DataType(DataType.EmailAddress)]
		public string EmailAddress
		{
			get => emailAddress;
			/* third parameter supplied,"true", which indicates that the validation 
			 * rules should be run as part of setting the underlying field. */
			set => SetProperty(ref emailAddress, value, true);
		}

		[Required]		
		[MinLength(2)]
		public string Password
		{
			get => password;
			set => SetProperty(ref password, value, true);
		}

    }
}
