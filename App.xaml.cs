using System;
using System.Windows;

namespace WPFCalc
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Обработка исключения и предотвращение аварийного завершения приложения
            MessageBox.Show("Notikusi kļūme: " + e.Exception.Message, "Kļūme", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Обработка исключения
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                MessageBox.Show("Notikusi kļūme: " + ex.Message, "Kļūme", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
