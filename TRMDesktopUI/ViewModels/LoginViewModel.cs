using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;

namespace TRMDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName;
        private string _password;
        private IAPIHelper _apiHelper;
        private string _errorMessage;
        private IEventAggregator _eventAggregator;

        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator eventAggregator)
        {
            _apiHelper = apiHelper;
            _eventAggregator = eventAggregator;
        }

        public string UserName
        {
            get { return _userName; }
            set 
            { 
                _userName = value;
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public bool IsErrorVisible
        {
            get 
            {
                bool output = false;

                if(ErrorMessage?.Length > 0)
                {
                    output = true;
                }

                return output; 
            }  
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(() => ErrorMessage);
                
                
            }
        }

        

        public bool CanLogIn
        {
            get
            {
                bool output = false;

                if (UserName?.Length > 0 && Password?.Length >= 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task LogIn()
        {
            try 
            {
                ErrorMessage = String.Empty;
                //1.Authenticate
                var result = await _apiHelper.Authenticate(UserName, Password);

                //2.Capture more information about the user in the model
                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

                //3. Raise event after user logs in
                _eventAggregator.PublishOnUIThread(new LogOnEventModel());
            }
            catch(Exception exc)
            {
                ErrorMessage = $"*{exc.Message}";
            }
           
        }
    }
}
