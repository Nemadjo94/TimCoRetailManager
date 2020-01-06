using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEventModel>// We listen only to LogOnEventModel events
    {
        
        private IEventAggregator _eventAggregator;
        private SalesViewModel _salesViewModel;
        private ILoggedInUserModel _loggedInUserModel;
        private IAPIHelper _apiHelper;

        public ShellViewModel( IEventAggregator eventAggregator, SalesViewModel salesViewModel, ILoggedInUserModel loggedInUserModel, IAPIHelper apiHelper)
        {
            
            _eventAggregator = eventAggregator;   
            _salesViewModel = salesViewModel;
            _loggedInUserModel = loggedInUserModel;
            _apiHelper = apiHelper;
            _eventAggregator.Subscribe(this); // Subscribe this class to the event aggregator
            
            // Starts LoginView, and since its not a singleton we get a fresh instance
            ActivateItem(IoC.Get<LoginViewModel>());
        }

        public bool IsAccountVisible
        {

            get
            {
                bool output = false;

                if(string.IsNullOrWhiteSpace(_loggedInUserModel.Id) == false)
                {
                    output = true;
                }

                return output;

            }            
        }

        public void LogOut()
        {
            // Reset login credentials
            UserHelper.ResetUserModel(_loggedInUserModel);
            _apiHelper.LogOffUser(); // We need to clear the request header from all the authorization info after user logs out
            // Starts LoginView, and since its not a singleton we get a fresh instance
            ActivateItem(IoC.Get<LoginViewModel>());

            NotifyOfPropertyChange(() => IsAccountVisible);
        }

        public void ExitApplication()
        {
            this.TryClose();
        }

        public void Handle(LogOnEventModel message)
        {
            // After user logs in, event is triggered and the SalesView window opens
            ActivateItem(_salesViewModel);

            NotifyOfPropertyChange(() => IsAccountVisible);
        }
    }
}
