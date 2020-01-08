using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private IUserEndpoint _userEndpoint;
        private UserModel _selectedUser;
        private string _selectedUserName;
        private BindingList<string> _selectedUserRole = new BindingList<string>();
        private BindingList<string> _availableRoles = new BindingList<string>();
        private string _selectedRoleToRemove;
        private string _selectedRoleToAdd;

        private BindingList<UserModel> _users { get; set; }

        public BindingList<UserModel> Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
                NotifyOfPropertyChange(() => Users);
            }
        }

        

        public UserModel SelectedUser
        {
            get { return _selectedUser; }
            set 
            { 
                _selectedUser = value;
                SelectedUserName = value.Email;
                SelectedUserRole = new BindingList<string>(value.Roles.Select(x => x.Value).ToList());
                LoadAvailableRoles();
                NotifyOfPropertyChange(() => SelectedUser);
            }
        }

        public string SelectedUserName
        {
            get { return _selectedUserName; }
            set
            { 
                _selectedUserName = value;
                NotifyOfPropertyChange(() => SelectedUserName);
            }
        }

        public BindingList<string> SelectedUserRole
        {
            get { return _selectedUserRole; }
            set 
            { 
                _selectedUserRole = value;
                NotifyOfPropertyChange(() => SelectedUserRole);
            }
        }

        public BindingList<string> AvailableRoles 
        {
            get { return _availableRoles; }
            set 
            {
                _availableRoles = value;

                NotifyOfPropertyChange(() => AvailableRoles);
            } 
        }

        public string SelectedRoleToRemove
        {
            get { return _selectedRoleToRemove; }
            set
            {
                _selectedRoleToRemove = value;
                NotifyOfPropertyChange(() => SelectedRoleToRemove);
            }
        }

        public string SelectedRoleToAdd
        {
            get { return _selectedRoleToAdd; }
            set
            {
                _selectedRoleToAdd = value;
                NotifyOfPropertyChange(() => SelectedRoleToAdd);
            }
        }


        public UserDisplayViewModel(StatusInfoViewModel status, IWindowManager window, IUserEndpoint userEndpoint)
        {
            _status = status;
            _window = window;
            _userEndpoint = userEndpoint;
        }

        /// <summary>
        /// Fires an event after page is loaded and await loads a list of products
        /// </summary>
        /// <param name="view"></param>
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadUsers();
            }
            catch (Exception)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Message";

                _status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form");
                _window.ShowDialog(_status, null, settings);
                TryClose();
            }
        }

        private async Task LoadUsers()
        {
            var usersList = await _userEndpoint.GetAll();

            //bind users to screen
            Users = new BindingList<UserModel>(usersList);
        }

        private async Task LoadAvailableRoles()
        {
            // call api to get all roles
            var roles = await _userEndpoint.GetAllRoles();

            foreach(var role in roles)
            {
                // check selected user role for existing roles, -1 is returned if the role is not asigned to current user
                if(SelectedUserRole.IndexOf(role.Value) < 0)
                {
                    // add those unasigned roles to the list of available roles
                    AvailableRoles.Add(role.Value);
                }
            }
        }

        public async void AddSelectedRole()
        {
            try
            {
                var role = SelectedRoleToAdd;
                await _userEndpoint.AddUserToRole(SelectedUser.Id, SelectedRoleToAdd);
                SelectedUserRole.Add(SelectedRoleToAdd);
                AvailableRoles.Remove(SelectedRoleToAdd);

            }
            catch (Exception exc)
            {

                throw exc;
            }
        }

        public async void RemoveSelectedRole()
        {
            try
            {
                await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedRoleToRemove);
                SelectedUserRole.Remove(SelectedRoleToRemove);
                AvailableRoles.Add(SelectedRoleToRemove);
                await LoadAvailableRoles();
                NotifyOfPropertyChange(() => AvailableRoles);

            }
            catch (Exception exc)
            {

                throw exc;
            }
        }

    }
}
