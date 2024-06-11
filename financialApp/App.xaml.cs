using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using financialApp.Views;
using financialApp.Views.Admin; 
using financialApp.Data;
using financialApp.Repositories;
using financialApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace financialApp
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            _serviceProvider = ConfigureServices();
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register the DbContext with the connection string
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer("Server=localhost;Database=FinancialDatabaseApp;Trusted_Connection=True;TrustServerCertificate=True;"));

            // Register repositories and services
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            // Register main application windows
            services.AddSingleton<LoginWindow>();
            services.AddSingleton<RegistrationWindow>();
            services.AddSingleton<AdminDashboard>(); 
            services.AddSingleton<UserDashboard>();

            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var loginWindow = _serviceProvider.GetService<LoginWindow>();
            if (loginWindow != null)
            {
                loginWindow.Show();
            }
            else
            {
                // Handle the case when loginWindow is null, for example log the error or close the application
                MessageBox.Show("Login window could not be loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
