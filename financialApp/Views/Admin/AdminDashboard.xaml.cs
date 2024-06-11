using System.Windows;
using System.Windows.Controls;
using financialApp.Interfaces;
using financialApp.Models;

namespace financialApp.Views.Admin
{
    public partial class AdminDashboard : Window
    {
        private readonly IUserService _userService;
        private readonly string _userRole;

        public AdminDashboard(IUserService userService, string userRole)
        {
            InitializeComponent();
            _userService = userService;
            _userRole = userRole;
            if (_userRole != "Admin")
            {
                // Ukryj funkcje dostępne tylko dla administratorów
                AdminButton.Visibility = Visibility.Collapsed;
            }
            LoadUsers();
        }

        private void LoadUsers()
        {
            var users = _userService.GetAllUsers();
            UsersListView.ItemsSource = users;
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var user = new User
            {
                Username = UsernameTextBox.Text,
                Password = PasswordTextBox.Password,
                Email = EmailTextBox.Text
            };
            _userService.CreateUser(user);
            LoadUsers();
        }

        private void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem is User selectedUser)
            {
                selectedUser.Username = UsernameTextBox.Text;
                selectedUser.Password = PasswordTextBox.Password;
                selectedUser.Email = EmailTextBox.Text;
                _userService.UpdateUser(selectedUser);
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Please select a user to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem is User selectedUser)
            {
                _userService.DeleteUser(selectedUser.UserID);
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Please select a user to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow(_userService);
            loginWindow.Show();
            this.Close();
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
