using System.Windows;

namespace financialApp.Views
{
    public partial class AddIncomeWindow : Window
    {
        public decimal IncomeAmount { get; private set; }
        public string Description { get; private set; }

        public AddIncomeWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(IncomeAmountTextBox.Text, out decimal amount))
            {
                IncomeAmount = amount;
                Description = DescriptionTextBox.Text;
                DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid amount.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
