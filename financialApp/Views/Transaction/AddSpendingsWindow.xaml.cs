using System.Windows;

namespace financialApp.Views
{
    public partial class AddSpendingsWindow : Window
    {
        public decimal SpendingsAmount { get; private set; }
        public string Description { get; private set; }

        public AddSpendingsWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(SpendingsAmountTextBox.Text, out decimal amount))
            {
                SpendingsAmount = amount;
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
