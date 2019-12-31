using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.EventModels;

namespace TRMDesktopUI.ViewModels
{
    
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEventModel>// We listen only to LogOnEventModel events
    {
        
        private IEventAggregator _eventAggregator;
        private SalesViewModel _salesViewModel;
        private SimpleContainer _container;

        public ShellViewModel( IEventAggregator eventAggregator, SalesViewModel salesViewModel, SimpleContainer container)
        {
            
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this); // Subscribe this class to the event aggregator
            _salesViewModel = salesViewModel;
            _container = container;

            // Starts LoginView, and since its not a singleton we get a fresh instance
            ActivateItem(_container.GetInstance<LoginViewModel>());
        }

        public void Handle(LogOnEventModel message)
        {
            // After user logs in, event is triggered and the SalesView window opens
            ActivateItem(_salesViewModel);           
        }
    }
}
