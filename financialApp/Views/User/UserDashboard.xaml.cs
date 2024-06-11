using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using financialApp.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using financialApp.Views;
using financialApp.Models;

namespace financialApp.Views
{
    public partial class UserDashboard : Window, INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private decimal _totalBalance;
        private decimal _totalExpenses;
        private decimal _totalSavings;
        private decimal _totalTransactionsAmount;
        private string _username;
        private string _email;
        private int _userId;

        public event PropertyChangedEventHandler PropertyChanged;

        public decimal TotalBalance
        {
            get { return _totalBalance; }
            set { _totalBalance = value; OnPropertyChanged(); }
        }

        public decimal TotalExpenses
        {
            get { return _totalExpenses; }
            set { _totalExpenses = value; OnPropertyChanged(); }
        }

        public decimal TotalSavings
        {
            get { return _totalSavings; }
            set { _totalSavings = value; OnPropertyChanged(); }
        }

        public decimal TotalTransactionsAmount
        {
            get { return _totalTransactionsAmount; }
            set { _totalTransactionsAmount = value; OnPropertyChanged(); }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Transaction> _incomes;
        public ObservableCollection<Transaction> Incomes
        {
            get { return _incomes; }
            set { _incomes = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Transaction> _spendings;
        public ObservableCollection<Transaction> Spendings
        {
            get { return _spendings; }
            set { _spendings = value; OnPropertyChanged(); }
        }

        public UserDashboard(IUserService userService, int userId)
        {
            InitializeComponent();
            DataContext = this;
            _userService = userService;
            _userId = userId;

            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            TotalBalance = _userService.GetTotalBalance(_userId);
            TotalExpenses = _userService.GetTotalExpenses(_userId);
            TotalSavings = _userService.GetTotalSavings(_userId);
            Username = _userService.GetUsername(_userId);
            Email = _userService.GetEmail(_userId);
            TotalTransactionsAmount = _userService.GetUserTransactions(_userId).Sum(t => t.Amount);

            Incomes = new ObservableCollection<Transaction>(_userService.GetUserTransactions(_userId).Where(t => t.Type == TransactionType.Incomes));
            Spendings = new ObservableCollection<Transaction>(_userService.GetUserTransactions(_userId).Where(t => t.Type == TransactionType.Spendings));
        }

        private void ViewTransactions_Click(object sender, RoutedEventArgs e)
        {
            var graphWindow = new GraphWindow(_userService, _userId);
            graphWindow.Show();
        }

        private void ViewSavingGoals_Click(object sender, RoutedEventArgs e)
        {
            // Otwórz widok celów oszczędnościowych
            //var savingGoalsWindow = new SavingGoalsWindow(_userService, _userId);
            //savingGoalsWindow.Show();
        }

        private void ViewReports_Click(object sender, RoutedEventArgs e)
        {
            // Otwórz widok raportów
            //var reportsWindow = new ReportsWindow(_userService, _userId);
            //reportsWindow.Show();
        }

        private void ViewReminders_Click(object sender, RoutedEventArgs e)
        {
            // Otwórz widok przypomnień
            //var remindersWindow = new RemindersWindow(_userService, _userId);
            //remindersWindow.Show();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow(_userService);
            loginWindow.Show();
            this.Close();
        }

        private void AddIncomes_Click(object sender, RoutedEventArgs e)
        {
            var addIncomeWindow = new AddIncomeWindow();
            if (addIncomeWindow.ShowDialog() == true)
            {
                decimal newIncomeAmount = addIncomeWindow.IncomeAmount;
                string description = addIncomeWindow.Description;
                var newTransaction = new Transaction
                {
                    UserID = _userId,
                    Amount = newIncomeAmount,
                    Date = DateTime.Now,
                    Description = description,
                    Type = TransactionType.Incomes
                };

                _userService.AddTransaction(newTransaction);
                LoadDashboardData(); // Refresh data
                MessageBox.Show($"Income added: {newIncomeAmount}", "Income Added", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddSpendings_Click(object sender, RoutedEventArgs e)
        {
            var addSpendingsWindow = new AddSpendingsWindow();
            if (addSpendingsWindow.ShowDialog() == true)
            {
                decimal newSpendingsAmount = addSpendingsWindow.SpendingsAmount;
                string description = addSpendingsWindow.Description;
                var newTransaction = new Transaction
                {
                    UserID = _userId,
                    Amount = -newSpendingsAmount, // Wydatki są zapisywane jako ujemne wartości
                    Date = DateTime.Now,
                    Description = description,
                    Type = TransactionType.Spendings
                };

                _userService.AddTransaction(newTransaction);
                LoadDashboardData(); // Refresh data
                MessageBox.Show($"Spendings added: {newSpendingsAmount}", "Spendings Added", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
