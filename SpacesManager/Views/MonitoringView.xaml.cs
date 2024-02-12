using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Eneca.SpacesManager.Views;
/// <summary>
/// Логика взаимодействия для MonitoringView.xaml
/// </summary>
public partial class MonitoringView 
{
    public MonitoringView()
    {
        InitializeComponent();
    }

    private void TopStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
    private void WindowClose_BTN_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void WindowHide_BTN_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
    private void EnecaLogo_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start($"https://enecagroup.com/");
    }
}
