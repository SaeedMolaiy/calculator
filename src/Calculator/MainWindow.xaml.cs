using System.Windows;
using System.Windows.Input;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void Minimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Maximize(object sender, RoutedEventArgs e) =>
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;

        private void Close(object sender, RoutedEventArgs e) => Close();
    }
}