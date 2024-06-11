using System;
using System.Windows;
using OxyPlot;
using OxyPlot.Series;
using financialApp.Interfaces;
using System.Linq;

namespace financialApp.Views
{
    public partial class GraphWindow : Window
    {
        public PlotModel PlotModel { get; private set; }

        public GraphWindow(IUserService userService, int userId)
        {
            InitializeComponent();
            DataContext = this;

            PlotModel = new PlotModel
            {
                Title = "Incomes vs Spendings",
                TitleColor = OxyColor.Parse("#ffd700") // Set the title color here
            };

            var transactions = userService.GetUserTransactions(userId);
            var incomes = transactions.Where(t => t.Type == TransactionType.Incomes).Sum(t => t.Amount);
            var spendings = transactions.Where(t => t.Type == TransactionType.Spendings).Sum(t => t.Amount);

            // Ensure spendings is positive for the pie chart and calculate percentages
            spendings = Math.Abs(spendings);
            var total = incomes + spendings;
            var incomesPercentage = total != 0 ? (double)(incomes / total * 100) : 0;
            var spendingsPercentage = total != 0 ? (double)(spendings / total * 100) : 0;

            var pieSeries = new PieSeries
            {
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.8,
                AngleSpan = 360,
                StartAngle = 0,
                TextColor = OxyColors.White
            };

            pieSeries.Slices.Add(new PieSlice("Incomes", incomesPercentage) { IsExploded = true, Fill = OxyColors.Green });
            pieSeries.Slices.Add(new PieSlice("Spendings", spendingsPercentage) { IsExploded = true, Fill = OxyColors.Red });

            PlotModel.Series.Add(pieSeries);
        }
    }
}
